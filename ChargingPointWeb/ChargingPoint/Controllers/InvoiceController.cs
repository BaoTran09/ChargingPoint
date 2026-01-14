/*
 * using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ChargingPoint.DB;
using ChargingPoint.Models;
using ChargingPoint.Services;
using System.Security.Claims;

namespace ChargingPoint.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly StoreDBContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly IInvoiceService _invoiceService;
        private readonly IPdfInvoiceService _pdfService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(
            StoreDBContext context,
            UserManager<Users> userManager,
            IInvoiceService invoiceService,
            IPdfInvoiceService pdfService,
            ILogger<InvoiceController> logger)
        {
            _context = context;
            _userManager = userManager;
            _invoiceService = invoiceService;
            _pdfService = pdfService;
            _logger = logger;
        }

        // =========================================================
        // GENERATE INVOICE
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateInvoice(long sessionId)
        {
            _logger.LogInformation("GenerateInvoice called with sessionId: {SessionId}", sessionId);

            if (sessionId <= 0)
            {
                TempData["Error"] = "Session ID không hợp lệ.";
                return RedirectToAction("Detail", "ChargingSession", new { id = sessionId });
            }

            try
            {
                var session = await _context.ChargingSession
                    .Include(s => s.Vehicle)
                        .ThenInclude(v => v.Customer)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session == null)
                {
                    TempData["Error"] = "Phiên sạc không tồn tại.";
                    return RedirectToAction("Index", "ChargingSession");
                }

                // Check permissions
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin") || User.IsInRole("Employee");

                if (!isAdmin)
                {
                    var customer = await _context.Customer
                        .FirstOrDefaultAsync(c => c.UserId == userId);

                    if (customer == null || session.Vehicle?.CustomerId != customer.CustomerId)
                    {
                        TempData["Error"] = "Bạn không có quyền tạo hóa đơn cho phiên này.";
                        return RedirectToAction("Detail", "ChargingSession", new { id = sessionId });
                    }
                }

                // Check session status
                if (session.Status != "Completed" && session.Status != "PoweredOff")
                {
                    TempData["Error"] = "Chỉ có thể tạo hóa đơn cho phiên đã hoàn tất.";
                    return RedirectToAction("Detail", "ChargingSession", new { id = sessionId });
                }

                // Generate invoice
                var invoice = await _invoiceService.GenerateInvoiceAsync(sessionId);

                // Send email (non-blocking)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _invoiceService.SendInvoiceEmailAsync(invoice.InvoiceId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to send invoice email for {InvoiceId}", invoice.InvoiceId);
                    }
                });

                TempData["Success"] = $"Hóa đơn #{invoice.InvoiceId} đã được tạo thành công.";
                return RedirectToAction("Detail", new { id = invoice.InvoiceId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice for session {SessionId}", sessionId);
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("Detail", "ChargingSession", new { id = sessionId });
            }
        }

        // =========================================================
        // CUSTOMER VIEWS
        // =========================================================

        // GET: Invoice/MyInvoices
        public async Task<IActionResult> MyInvoices(string? status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var customer = await _context.Customer
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
            {
                return View(new List<Invoice>());
            }

            var query = _context.Invoice
                .Include(i => i.ChargingSession)
                    .ThenInclude(s => s.Vehicle)
                .Include(i => i.ChargingSession)
                    .ThenInclude(s => s.Connector)
                        .ThenInclude(c => c.Charger)
                            .ThenInclude(ch => ch.Station)
                .Include(i => i.Customer)
                .Where(i => i.CustomerId == customer.CustomerId);

            // Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "Overdue")
                {
                    query = query.Where(i => i.Status == "Unpaid" && i.ExpireDate < DateTime.Now);
                }
                else
                {
                    query = query.Where(i => i.Status == status);
                }
            }

            var invoices = await query
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            ViewBag.FilterStatus = status;
            return View(invoices);
        }

        // GET: Invoice/Detail/5
        public async Task<IActionResult> Detail(long id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            // Check authorization
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Employee");

            if (!isAdmin)
            {
                var customer = await _context.Customer
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (customer == null || invoice.CustomerId != customer.CustomerId)
                {
                    return Forbid();
                }
            }

            return View(invoice);
        }

        // GET: Invoice/DownloadPdf/5
        public async Task<IActionResult> DownloadPdf(long id)
        {
            var invoice = await _context.Invoice
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            // Check authorization
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Employee");

            if (!isAdmin)
            {
                var customer = await _context.Customer
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (customer == null || invoice.CustomerId != customer.CustomerId)
                {
                    return Forbid();
                }
            }

            try
            {
                var pdfBytes = _pdfService.GenerateInvoicePdf(invoice);
                return File(pdfBytes, "application/pdf", $"Invoice_{id}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF for invoice {InvoiceId}", id);
                TempData["Error"] = $"Không thể tạo PDF: {ex.Message}";
                return RedirectToAction("Detail", new { id });
            }
        }

        // POST: Invoice/UploadPaymentProof
        [HttpPost]
        public async Task<IActionResult> UploadPaymentProof(long invoiceId, IFormFile proof, string transactionId)
        {
            try
            {
                var invoice = await _context.Invoice.FindAsync(invoiceId);
                if (invoice == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy hóa đơn" });
                }

                // Check authorization
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var customer = await _context.Customer
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (customer == null || invoice.CustomerId != customer.CustomerId)
                {
                    return Json(new { success = false, message = "Không có quyền" });
                }

                // Save file
                if (proof != null && proof.Length > 0)
                {
                    var uploadsFolder = Path.Combine("wwwroot", "uploads", "payment-proofs");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"invoice_{invoiceId}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(proof.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await proof.CopyToAsync(stream);
                    }

                    invoice.SignatureFile = $"/uploads/payment-proofs/{fileName}";
                }

                if (!string.IsNullOrEmpty(transactionId))
                {
                    invoice.Notes = $"Khách hàng đã upload chứng từ. Mã GD: {transactionId}";
                }
                else
                {
                    invoice.Notes = "Khách hàng đã upload chứng từ thanh toán. Đang chờ xác nhận.";
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã gửi chứng từ thành công. Chúng tôi sẽ xác nhận trong vòng 24h." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading payment proof");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =========================================================
        // ADMIN FUNCTIONS
        // =========================================================

        // GET: Invoice/AdminList
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> AdminList(string? status, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Invoice
                .Include(i => i.ChargingSession)
                    .ThenInclude(s => s.Vehicle)
                        .ThenInclude(v => v.Customer)
                .Include(i => i.InvoiceDetails)
                .AsQueryable();

            // Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(i => i.Status == status);
            }

            // Filter by date range
            if (fromDate.HasValue)
            {
                query = query.Where(i => i.CreatedAt >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                query = query.Where(i => i.CreatedAt <= toDate.Value.AddDays(1));
            }

            var invoices = await query
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            ViewBag.FilterStatus = status;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            return View(invoices);
        }

        // POST: Invoice/MarkAsPaid
        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> MarkAsPaid(long invoiceId, string paymentMethod, string transactionId)
        {
            try
            {
                var result = await _invoiceService.MarkAsPaidAsync(invoiceId, paymentMethod, transactionId);

                if (result)
                {
                    return Json(new { success = true, message = "Đã xác nhận thanh toán" });
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy hóa đơn" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking invoice as paid");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Invoice/ResendEmail
        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> ResendEmail(long invoiceId)
        {
            try
            {
                var result = await _invoiceService.SendInvoiceEmailAsync(invoiceId);

                if (result)
                {
                    return Json(new { success = true, message = "Đã gửi lại email" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể gửi email" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending invoice email");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
*/