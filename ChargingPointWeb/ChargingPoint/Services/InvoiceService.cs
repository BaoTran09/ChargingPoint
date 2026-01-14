/*using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ChargingPoint.DB;
using ChargingPoint.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ChargingPoint.Services
{
    public interface IInvoiceService
    {
        // Cả 2 versions để tương thích với code cũ
        Task<Invoice> GenerateInvoice(long sessionId);
        Task<Invoice> GenerateInvoiceAsync(long sessionId);

        Task<bool> MarkAsPaid(long invoiceId, string paymentMethod, string transactionId);
        Task<bool> MarkAsPaidAsync(long invoiceId, string paymentMethod, string transactionId);

        Task<bool> SendInvoiceEmail(long invoiceId);
        Task<bool> SendInvoiceEmailAsync(long invoiceId);

        Task<Invoice> GetInvoiceByIdAsync(long invoiceId);
        Task<List<Invoice>> GetInvoicesByCustomerAsync(long customerId);
    }

    public class InvoiceService : IInvoiceService
    {
        private readonly StoreDBContext _context;
        private readonly IPdfInvoiceService _pdfService;
        private readonly IEmailService _emailService;
        private readonly ILogger<InvoiceService> _logger;

        // Pricing constants
        private const decimal UNIT_PRICE_KWH = 3858m;
        private const decimal OVERSTAY_PRICE_PER_MINUTE = 1000m;
        private const int GRACE_PERIOD_MINUTES = 10;
        private const decimal VAT_RATE = 10m;
        private const int INVOICE_EXPIRE_DAYS = 15;

        public InvoiceService(
            StoreDBContext context,
            IPdfInvoiceService pdfService,
            IEmailService emailService,
            ILogger<InvoiceService> logger)
        {
            _context = context;
            _pdfService = pdfService;
            _emailService = emailService;
            _logger = logger;
        }

        // Wrapper methods for backward compatibility
        public Task<Invoice> GenerateInvoice(long sessionId) => GenerateInvoiceAsync(sessionId);
        public Task<bool> MarkAsPaid(long invoiceId, string paymentMethod, string transactionId)
            => MarkAsPaidAsync(invoiceId, paymentMethod, transactionId);
        public Task<bool> SendInvoiceEmail(long invoiceId) => SendInvoiceEmailAsync(invoiceId);

        public async Task<Invoice> GenerateInvoiceAsync(long sessionId)
        {
            try
            {
                // Check existing
                var existingInvoice = await _context.Invoice
                    .FirstOrDefaultAsync(i => i.SessionId == sessionId);

                if (existingInvoice != null)
                {
                    _logger.LogInformation("Invoice {InvoiceId} already exists for session {SessionId}",
                        existingInvoice.InvoiceId, sessionId);
                    return await GetInvoiceByIdAsync(existingInvoice.InvoiceId);
                }

                // Load session with full navigation
                var session = await _context.ChargingSession
                    .Include(s => s.Connector)
                        .ThenInclude(c => c.Charger)
                            .ThenInclude(ch => ch.Station)
                    .Include(s => s.Vehicle)
                        .ThenInclude(v => v.Customer)
                            .ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session == null)
                {
                    throw new Exception($"ChargingSession {sessionId} not found");
                }

                if (session.Vehicle?.Customer == null)
                {
                    throw new InvalidOperationException("Cannot create invoice - No customer information");
                }

                var customer = session.Vehicle.Customer;

                // Get next invoice number
                var nextInvoiceNumber = await GetNextInvoiceNumberAsync();

                // Create Invoice header
                var invoice = new Invoice
                {
                    SessionId = sessionId,
                    CustomerId = customer.CustomerId,

                    // Snapshot customer data at invoice creation time
                    Snashot_CustomerName = customer.FullName,
                    Snashot_CustomerPhone = customer.PhoneNumber,
                    Snashot_CustomerEmail = customer.User?.Email ?? customer.Email,

                    // Invoice metadata
                    InvoiceNumber = nextInvoiceNumber,
                    InvoiceTemplate = "01GTKT3/001",
                    InvoiceSymbol = "TK/20E",
                    CreatedAt = DateTime.Now,
                    ExpireDate = DateTime.Now.AddDays(INVOICE_EXPIRE_DAYS),

                    // Will be calculated from InvoiceDetails
                    TotalAmountService = 0,
                    TotalAmountTax = 0,
                    TotalAmountDiscount = 0,
                    Total = 0,

                    // Payment
                    Status = "Unpaid",
                    PaymentLink = GenerateVietQRLink(nextInvoiceNumber, 0),
                    QRCodeData = GenerateQRCodeData(0, nextInvoiceNumber),

                    // Email
                    EmailSent = false,

                    Notes = $"Hóa đơn phiên sạc #{sessionId}"
                };

                _context.Invoice.Add(invoice);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created invoice {InvoiceId} for session {SessionId}",
                    invoice.InvoiceId, sessionId);

                // Create InvoiceDetails
                await CreateInvoiceDetailsAsync(invoice.InvoiceId, session);

                // Recalculate totals from details
                await RecalculateInvoiceTotalsAsync(invoice.InvoiceId);

                // Update payment links with correct amount
                invoice = await _context.Invoice.FindAsync(invoice.InvoiceId);
                invoice.PaymentLink = GenerateVietQRLink(invoice.InvoiceNumber, invoice.Total ?? 0);
                invoice.QRCodeData = GenerateQRCodeData(invoice.Total ?? 0, invoice.InvoiceNumber);
                await _context.SaveChangesAsync();

                // Generate PDF
                try
                {
                    var pdfBytes = await _pdfService.GenerateInvoice(invoice.InvoiceId);
                    var pdfPath = await _pdfService.SaveInvoicePdfAsync(invoice.InvoiceId, "wwwroot/invoices");
                    invoice.PdfFilePath = pdfPath;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Generated PDF for invoice {InvoiceId}", invoice.InvoiceId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to generate PDF for invoice {InvoiceId}", invoice.InvoiceId);
                }

                return await GetInvoiceByIdAsync(invoice.InvoiceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice for session {SessionId}", sessionId);
                throw;
            }
        }

        private async Task CreateInvoiceDetailsAsync(long invoiceId, ChargingSession session)
        {
            var details = new List<InvoiceDetail>();
            int stt = 1;

            // DETAIL 1: CHARGING_FEE
            var energyDelivered = session.EnergyDeliveredKWh ?? 0;
            if (energyDelivered > 0)
            {
                var chargingFeeItem = await GetOrCreateRevenueItemAsync(
                    "CHARGING_FEE",
                    "Dịch vụ sạc tại trạm",
                    "kWh");

                var quantity = (int)Math.Ceiling(energyDelivered);
                var amount = Math.Round(energyDelivered * UNIT_PRICE_KWH, 2);
                var taxAmount = Math.Round(amount * (VAT_RATE / 100), 2);

                details.Add(new InvoiceDetail
                {
                    InvoiceId = invoiceId,
                    STT = stt++,
                    ItemId = chargingFeeItem.ItemId,
                    Quantities = quantity,
                    Unit = "kWh",
                    UnitPrice = UNIT_PRICE_KWH,
                    Amount = amount,
                    DiscountPercent = 0,
                    DiscountAmount = 0,
                    Tax = VAT_RATE,
                    TaxAmount = taxAmount
                });
            }

            // DETAIL 2: OVERSTAY_FEE
            var overDueMinutes = session.OverDueTime ?? 0;
            if (overDueMinutes > GRACE_PERIOD_MINUTES)
            {
                var chargeableMinutes = overDueMinutes - GRACE_PERIOD_MINUTES;
                var overstayFeeItem = await GetOrCreateRevenueItemAsync(
                    "OVERSTAY_FEE",
                    "Phí phạt quá thời gian",
                    "Phút");

                var amount = Math.Round(chargeableMinutes * OVERSTAY_PRICE_PER_MINUTE, 2);

                details.Add(new InvoiceDetail
                {
                    InvoiceId = invoiceId,
                    STT = stt++,
                    ItemId = overstayFeeItem.ItemId,
                    Quantities = chargeableMinutes,
                    Unit = "Phút",
                    UnitPrice = OVERSTAY_PRICE_PER_MINUTE,
                    Amount = amount,
                    DiscountPercent = 0,
                    DiscountAmount = 0,
                    Tax = 0,
                    TaxAmount = 0
                });
            }

            // Save all details
            if (details.Any())
            {
                _context.InvoiceDetail.AddRange(details);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created {Count} invoice details for invoice {InvoiceId}",
                    details.Count, invoiceId);
            }
        }

        private async Task<RevenueItem> GetOrCreateRevenueItemAsync(
            string itemType,
            string itemName,
            string unit)
        {
            var item = await _context.RevenueItem
                .FirstOrDefaultAsync(r => r.ItemType == itemType && r.IsActive == true);

            if (item == null)
            {
                item = new RevenueItem
                {
                    ItemName = itemName,
                    Unit = unit,
                    ItemType = itemType,
                    Description = itemName, // ← Typo trong DB, dùng "Discription"
                    IsActive = true
                };

                _context.RevenueItem.Add(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created RevenueItem: {ItemType}", itemType);
            }

            return item;
        }

        private async Task RecalculateInvoiceTotalsAsync(long invoiceId)
        {
            var details = await _context.InvoiceDetail
                .Where(d => d.InvoiceId == invoiceId)
                .ToListAsync();

            if (!details.Any())
            {
                _logger.LogWarning("No details found to calculate totals for invoice {InvoiceId}", invoiceId);
                return;
            }

            decimal totalService = 0;
            decimal totalDiscount = 0;
            decimal totalTax = 0;
            decimal grandTotal = 0;

            foreach (var detail in details)
            {
                var amount = detail.Amount ?? 0;
                var discount = detail.DiscountAmount ?? 0;
                var taxAmount = detail.TaxAmount ?? 0;

                totalService += amount;
                totalDiscount += discount;
                totalTax += taxAmount;

                var lineTotal = amount - discount + taxAmount;
                grandTotal += lineTotal;
            }

            var invoice = await _context.Invoice.FindAsync(invoiceId);
            if (invoice != null)
            {
                invoice.TotalAmountService = totalService;
                invoice.TotalAmountDiscount = totalDiscount;
                invoice.TotalAmountTax = totalTax;
                invoice.Total = grandTotal;

                await _context.SaveChangesAsync();
            }
        }

        private async Task<long> GetNextInvoiceNumberAsync()
        {
            var lastInvoice = await _context.Invoice
                .OrderByDescending(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            return (lastInvoice?.InvoiceNumber ?? 0) + 1;
        }

        public async Task<bool> MarkAsPaidAsync(long invoiceId, string paymentMethod, string transactioncode)
        {
            try
            {
                var invoice = await _context.Invoice.FindAsync(invoiceId);
                if (invoice == null)
                {
                    _logger.LogWarning("Invoice {InvoiceId} not found", invoiceId);
                    return false;
                }

                invoice.Status = "Paid";
                invoice.PaidAt = DateTime.Now;
                invoice.PaymentMethod = paymentMethod;

                if (!string.IsNullOrEmpty(transactioncode))
                {
                    var transaction = await _context.Transaction
                        .FirstOrDefaultAsync(t => t.TransactionCode == transactioncode);

                    if (transaction != null)
                    {
                        transaction.InvoiceId = invoiceId;
                        transaction.InvoiceNumber = invoice.InvoiceNumber;
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Invoice {InvoiceId} marked as paid", invoiceId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking invoice {InvoiceId} as paid", invoiceId);
                return false;
            }
        }

        public async Task<bool> SendInvoiceEmailAsync(long invoiceId)
        {
            try
            {
                var invoice = await _context.Invoice
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

                if (invoice == null || string.IsNullOrEmpty(invoice.Snashot_CustomerEmail))
                {
                    _logger.LogWarning("Cannot send email for invoice {InvoiceId}", invoiceId);
                    return false;
                }

                var pdfBytes = await _pdfService.GenerateInvoice(invoiceId);

                await _emailService.SendInvoiceEmailAsync(
                    invoice.Snashot_CustomerEmail,
                    invoice.Snashot_CustomerName ?? "Khách hàng",
                    invoice.InvoiceNumber.ToString(),
                    pdfBytes);

                invoice.EmailSent = true;
                invoice.EmailSentAt = DateTime.Now;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Sent invoice email for {InvoiceId}", invoiceId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email for invoice {InvoiceId}", invoiceId);
                return false;
            }
        }

        public async Task<Invoice> GetInvoiceByIdAsync(long invoiceId)
        {
            return await _context.Invoice
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.RevenueItem)
                .Include(i => i.Customer)
                .Include(i => i.ChargingSession)
                    .ThenInclude(s => s.Vehicle)
                        .ThenInclude(v => v.Customer)
                .Include(i => i.ChargingSession)
                    .ThenInclude(s => s.Connector)
                        .ThenInclude(c => c.Charger)
                            .ThenInclude(ch => ch.Station)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
        }

        public async Task<List<Invoice>> GetInvoicesByCustomerAsync(long customerId)
        {
            return await _context.Invoice
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.RevenueItem)
                .Include(i => i.ChargingSession)
                .Where(i => i.CustomerId == customerId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        private string GenerateVietQRLink(long invoiceNumber, decimal amount)
        {
            var bankCode = "970436";
            var accountNumber = "1030946681";
            var content = $"VGREEN{invoiceNumber:D7}";

            return $"https://img.vietqr.io/image/{bankCode}-{accountNumber}-compact.png?amount={amount:F0}&addInfo={content}";
        }

        private string GenerateQRCodeData(decimal amount, long invoiceNumber)
        {
            return $"970436|1030946681|{amount:F0}|VGREEN{invoiceNumber:D7}";
        }
    }
}*/