using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ChargingPoint.DB;
using ChargingPoint.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ChargingPoint.Services
{
    // ========================================================
    // INTERFACE
    // ========================================================
    public interface IPdfInvoiceService
    {
        byte[] GenerateInvoicePdf(Invoice invoice); // ← THÊM method này
        Task<byte[]> GenerateInvoice(long invoiceId);
        Task<string> SaveInvoicePdfAsync(long invoiceId, string folderPath);
    }

    // ========================================================
    // PDF INVOICE SERVICE V3 - Tương thích với DB Version 3
    // ========================================================
    /*
     public class PdfInvoiceService : IPdfInvoiceService
    {
        private readonly StoreDBContext _context;

        public PdfInvoiceService(StoreDBContext context)
        {
            _context = context;
        }

        // Sync version for compatibility
        public byte[] GenerateInvoicePdf(Invoice invoice)
        {
            // TODO: Implement PDF generation
            // Using library like iTextSharp, QuestPDF, or Rotativa
            return new byte[0]; // Placeholder
        }

        // Async version
        public async Task<byte[]> GenerateInvoice(long invoiceId)
        {
            var invoice = await _context.Invoice
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            if (invoice == null) return new byte[0];

            return GenerateInvoicePdf(invoice);
        }

        public async Task<string> SaveInvoicePdfAsync(long invoiceId, string folderPath)
        {
            var pdfBytes = await GenerateInvoice(invoiceId);
            var fileName = $"Invoice_{invoiceId}.pdf";
            var fullPath = Path.Combine(folderPath, fileName);

            Directory.CreateDirectory(folderPath);
            await File.WriteAllBytesAsync(fullPath, pdfBytes);

            return $"/invoices/{fileName}";
        }
    }
     */
    public class PdfInvoiceService : IPdfInvoiceService
    {
        private readonly StoreDBContext _context;
        private readonly ILogger<PdfInvoiceService> _logger;

        public PdfInvoiceService(StoreDBContext context, ILogger<PdfInvoiceService> logger)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            _context = context;
            _logger = logger;
        }

        // ========================================================
        // 1. GENERATE INVOICE PDF
        // ========================================================
        public byte[] GenerateInvoicePdf(Invoice invoice)
        {
            // TODO: Implement PDF generation
            // Using library like iTextSharp, QuestPDF, or Rotativa
            return new byte[0]; // Placeholder
        }
        public async Task<byte[]> GenerateInvoice(long invoiceId)
        {
            try
            {
                // Load Invoice với đầy đủ thông tin
                var invoice = await _context.Invoice
                    .Include(i => i.InvoiceDetails)
                        .ThenInclude(d => d.RevenueItem)
                    .Include(i => i.ChargingSession)
                        .ThenInclude(s => s.Connector)
                            .ThenInclude(c => c.Charger)
                                .ThenInclude(ch => ch.Station)
                    .Include(i => i.ChargingSession)
                        .ThenInclude(s => s.Vehicle)
                    .Include(i => i.Customer)
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

                if (invoice == null)
                {
                    _logger.LogError("Invoice {InvoiceId} not found", invoiceId);
                    throw new Exception($"Invoice {invoiceId} not found");
                }

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(40);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                        page.Header().Element(header => ComposeHeader(header, invoice));
                        page.Content().Element(content => ComposeContent(content, invoice));
                        page.Footer().Element(ComposeFooter);
                    });
                });

                _logger.LogInformation("Generated PDF for invoice {InvoiceId}", invoiceId);
                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF for invoice {InvoiceId}", invoiceId);
                throw;
            }
        }

        // ========================================================
        // 2. SAVE INVOICE PDF
        // ========================================================
        public async Task<string> SaveInvoicePdfAsync(long invoiceId, string outputDirectory)
        {
            try
            {
                var pdfBytes = await GenerateInvoice(invoiceId);
                var fileName = $"Invoice_{invoiceId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                var filePath = Path.Combine(outputDirectory, fileName);

                Directory.CreateDirectory(outputDirectory);
                await File.WriteAllBytesAsync(filePath, pdfBytes);

                _logger.LogInformation("Saved PDF for invoice {InvoiceId} to {FilePath}", invoiceId, filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving PDF for invoice {InvoiceId}", invoiceId);
                throw;
            }
        }

        // ========================================================
        // 3. COMPOSE HEADER
        // ========================================================
        private void ComposeHeader(IContainer container, Invoice invoice)
        {
            container.Row(row =>
            {
                // Left side - Company info
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("HÓA ĐƠN GIÁ TRỊ GIA TĂNG")
                        .FontSize(14).Bold().FontColor(Colors.Blue.Darken2);

                    column.Item().PaddingTop(3).Text("V-GREEN CHARGING STATION")
                        .FontSize(11).SemiBold();

                    column.Item().PaddingTop(8).Text("CÔNG TY CỔ PHẦN PHÁT TRIỂN")
                        .FontSize(9).Bold();

                    column.Item().Text("TRẠM SẠC TOÀN CẦU V-GREEN")
                        .FontSize(9).Bold();

                    column.Item().PaddingTop(5).Text("MST: 0123456789").FontSize(9);
                    column.Item().Text("Hotline: 1900-xxxx").FontSize(9);
                });

                // Right side - Invoice info
                row.ConstantItem(120).Column(column =>
                {
                    column.Item().AlignRight().Text($"Mẫu số: {invoice.InvoiceTemplate}").FontSize(8);
                    column.Item().AlignRight().Text($"Ký hiệu: {invoice.InvoiceSymbol}").FontSize(8);
                    column.Item().AlignRight().Text($"Số: {invoice.InvoiceNumber:D7}").FontSize(9).Bold();
                    column.Item().PaddingTop(5).AlignRight().Text($"Ngày: {invoice.CreatedAt:dd/MM/yyyy}").FontSize(8);
                });
            });
        }

        // ========================================================
        // 4. COMPOSE CONTENT
        // ========================================================
        private void ComposeContent(IContainer container, Invoice invoice)
        {
            container.PaddingVertical(15).Column(column =>
            {
                // ============================================
                // THÔNG TIN ĐƠN VỊ CUNG CẤP
                // ============================================
                var station = invoice.ChargingSession?.Connector?.Charger?.Station;

                column.Item().PaddingBottom(10).Column(col =>
                {
                    col.Item().Text("THÔNG TIN ĐƠN VỊ CUNG CẤP DỊCH VỤ")
                        .Bold().FontSize(10).FontColor(Colors.Blue.Darken1);

                    col.Item().PaddingTop(5).Row(row =>
                    {
                        row.ConstantItem(120).Text("Cơ sở:").FontSize(9);
                        row.RelativeItem().Text(station?.Name ?? "N/A").FontSize(9).SemiBold();
                    });

                    col.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("Địa chỉ:").FontSize(9);
                        row.RelativeItem().Text(station?.Address ?? "N/A").FontSize(9);
                    });

                    col.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("Số tài khoản:").FontSize(9);
                        row.RelativeItem().Text("LE BAO TRAN - 1030946681 - VIETCOMBANK")
                            .FontSize(9).Bold().FontColor(Colors.Blue.Darken2);
                    });
                });

                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                // ============================================
                // THÔNG TIN KHÁCH HÀNG (từ Snapshot - DB V3)
                // ============================================
                column.Item().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text("THÔNG TIN KHÁCH HÀNG")
                        .Bold().FontSize(10).FontColor(Colors.Blue.Darken1);

                    col.Item().PaddingTop(5).Row(row =>
                    {
                        row.ConstantItem(120).Text("Họ tên:").FontSize(9);
                        row.RelativeItem().Text(invoice.Snashot_CustomerName ?? "N/A")
                            .FontSize(9).SemiBold();
                    });

                    col.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("Số điện thoại:").FontSize(9);
                        row.RelativeItem().Text(invoice.Snashot_CustomerPhone ?? "N/A").FontSize(9);
                    });

                    col.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("Email:").FontSize(9);
                        row.RelativeItem().Text(invoice.Snashot_CustomerEmail ?? "N/A").FontSize(9);
                    });

                    // Thông tin xe
                    var vehicle = invoice.ChargingSession?.Vehicle;
                    if (vehicle != null)
                    {
                        col.Item().Row(row =>
                        {
                            row.ConstantItem(120).Text("Xe:").FontSize(9);
                            row.RelativeItem().Text($"{vehicle.Manufacturer} {vehicle.Model} {vehicle.Version}")
                                .FontSize(9);
                        });

                        col.Item().Row(row =>
                        {
                            row.ConstantItem(120).Text("Biển số xe:").FontSize(9);
                            row.RelativeItem().Text(vehicle.LicensePlate ?? "N/A")
                                .FontSize(9).Bold().FontColor(Colors.Red.Darken1);
                        });
                    }
                });

                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                // ============================================
                // BẢNG DỊCH VỤ (từ InvoiceDetail - DB V3)
                // ============================================
                column.Item().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);   // STT
                        columns.RelativeColumn(3);    // Dịch vụ
                        columns.RelativeColumn(1);    // Số lượng
                        columns.RelativeColumn(1);    // Đơn vị
                        columns.RelativeColumn(1.5f); // Đơn giá
                        columns.RelativeColumn(1);    // Giảm giá
                        columns.RelativeColumn(1.5f); // Thành tiền
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderStyle).Text("STT");
                        header.Cell().Element(HeaderStyle).Text("Dịch vụ");
                        header.Cell().Element(HeaderStyle).Text("SL");
                        header.Cell().Element(HeaderStyle).Text("ĐVT");
                        header.Cell().Element(HeaderStyle).Text("Đơn giá");
                        header.Cell().Element(HeaderStyle).Text("Giảm giá");
                        header.Cell().Element(HeaderStyle).Text("Thành tiền");

                        static IContainer HeaderStyle(IContainer c) => c
                            .Background(Colors.Grey.Lighten3)
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten1)
                            .Padding(5)
                            .AlignCenter()
                            .AlignMiddle();
                    });

                    // Rows từ InvoiceDetail
                    foreach (var detail in invoice.InvoiceDetails.OrderBy(d => d.STT))
                    {
                        table.Cell().Element(CellStyle).Text(detail.STT.ToString());
                        table.Cell().Element(CellStyle).Text(detail.RevenueItem?.ItemName ?? "N/A");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{detail.Quantities ?? 0}");
                        table.Cell().Element(CellStyle).AlignCenter().Text(detail.Unit ?? "");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{detail.UnitPrice ?? 0:N0}");
                        table.Cell().Element(CellStyle).AlignRight().Text(
                            (detail.DiscountAmount ?? 0) > 0 ? $"-{detail.DiscountAmount ?? 0:N0}" : "-"
                        );
                      /*  table.Cell().Element(CellStyle).AlignRight()
                            .Text($"{detail.TotalLine ?? 0:N0}").Bold();*/
                    }

                    static IContainer CellStyle(IContainer c) => c
                        .Border(1)
                        .BorderColor(Colors.Grey.Lighten1)
                        .Padding(5);
                });

                // ============================================
                // TỔNG CỘNG (từ Invoice - DB V3)
                // ============================================
                column.Item().PaddingTop(15).AlignRight().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.ConstantItem(150).Text("Tổng tiền hàng:").FontSize(9);
                        row.ConstantItem(100).AlignRight()
                            .Text($"{invoice.TotalAmountService ?? 0:N0} VND").FontSize(9);
                    });

                    if ((invoice.TotalAmountDiscount ?? 0) > 0)
                    {
                        col.Item().Row(row =>
                        {
                            row.ConstantItem(150).Text("Tổng giảm giá:").FontSize(9);
                            row.ConstantItem(100).AlignRight()
                                .Text($"-{invoice.TotalAmountDiscount ?? 0:N0} VND")
                                .FontSize(9).FontColor(Colors.Green.Medium);
                        });
                    }

                    col.Item().Row(row =>
                    {
                        row.ConstantItem(150).Text("Thuế GTGT:").FontSize(9);
                        row.ConstantItem(100).AlignRight()
                            .Text($"{invoice.TotalAmountTax ?? 0:N0} VND").FontSize(9);
                    });

                    col.Item().PaddingTop(5).LineHorizontal(2).LineColor(Colors.Blue.Darken2);

                    col.Item().PaddingTop(5).Row(row =>
                    {
                        row.ConstantItem(150).Text("TỔNG CỘNG:").FontSize(11).Bold();
                        row.ConstantItem(100).AlignRight()
                            .Text($"{invoice.Total ?? 0:N0} VND")
                            .FontSize(11).Bold().FontColor(Colors.Red.Darken2);
                    });

                    col.Item().PaddingTop(3).Text($"Bằng chữ: {NumberToWords((long)(invoice.Total ?? 0))} đồng")
                        .FontSize(9).Italic();
                });

                // ============================================
                // THÔNG TIN THANH TOÁN
                // ============================================
                column.Item().PaddingTop(20).Column(col =>
                {
                    col.Item().Text("THÔNG TIN THANH TOÁN")
                        .Bold().FontSize(10).FontColor(Colors.Blue.Darken1);

                    col.Item().PaddingTop(5).Text("Vui lòng chuyển khoản đến:")
                        .FontSize(9);

                    col.Item().PaddingTop(3).Background(Colors.Grey.Lighten4).Padding(10).Column(inner =>
                    {
                        inner.Item().Text("Chủ tài khoản: LE BAO TRAN").FontSize(9).Bold();
                        inner.Item().Text("Số tài khoản: 1030946681").FontSize(9).Bold();
                        inner.Item().Text("Ngân hàng: VIETCOMBANK").FontSize(9).Bold();
                        inner.Item().PaddingTop(5)
                            .Text($"Nội dung: VGREEN {invoice.InvoiceNumber:D7}")
                            .FontSize(10).Bold().FontColor(Colors.Red.Darken2);
                    });

                    if (invoice.ExpireDate.HasValue)
                    {
                        col.Item().PaddingTop(8)
                            .Text($"⚠️ Vui lòng thanh toán trước ngày {invoice.ExpireDate.Value:dd/MM/yyyy}")
                            .FontSize(9).FontColor(Colors.Orange.Darken2);
                    }

                    // Trạng thái thanh toán
                    if (invoice.Status == "Paid" && invoice.PaidAt.HasValue)
                    {
                        col.Item().PaddingTop(10).Background(Colors.Green.Lighten4).Padding(10)
                            .Text($"✓ ĐÃ THANH TOÁN - {invoice.PaidAt.Value:dd/MM/yyyy HH:mm}")
                            .FontSize(10).Bold().FontColor(Colors.Green.Darken2);
                    }
                });
            });
        }

        // ========================================================
        // 5. COMPOSE FOOTER
        // ========================================================
        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Column(column =>
            {
                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                column.Item().PaddingTop(10).Text(text =>
                {
                    text.Span("Cảm ơn quý khách đã sử dụng dịch vụ V-Green! ").FontSize(9);
                    text.Span("Hotline: 1900-xxxx").FontSize(9).FontColor(Colors.Blue.Medium);
                });

                column.Item().PaddingTop(3).Text("Website: www.v-green.com | Email: support@v-green.com")
                    .FontSize(8).FontColor(Colors.Grey.Medium);
            });
        }

        // ========================================================
        // 6. NUMBER TO WORDS (Vietnamese)
        // ========================================================
        private string NumberToWords(long number)
        {
            if (number == 0)
                return "không";

            if (number < 0)
                return "âm " + NumberToWords(Math.Abs(number));

            string[] ones = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] tens = { "", "", "hai mươi", "ba mươi", "bốn mươi", "năm mươi",
                             "sáu mươi", "bảy mươi", "tám mươi", "chín mươi" };

            if (number < 10)
                return ones[number];

            if (number < 20)
            {
                return "mười " + (number % 10 == 0 ? "" : (number % 10 == 5 ? "lăm" : ones[number % 10]));
            }

            if (number < 100)
            {
                string result = tens[number / 10];
                int remainder = (int)(number % 10);
                if (remainder == 0)
                    return result;
                if (remainder == 1)
                    return result + " mốt";
                if (remainder == 5)
                    return result + " lăm";
                return result + " " + ones[remainder];
            }

            if (number < 1000)
            {
                string result = ones[number / 100] + " trăm";
                int remainder = (int)(number % 100);
                if (remainder == 0)
                    return result;
                if (remainder < 10)
                    return result + " lẻ " + ones[remainder];
                return result + " " + NumberToWords(remainder);
            }

            if (number < 1000000)
            {
                string result = NumberToWords(number / 1000) + " nghìn";
                int remainder = (int)(number % 1000);
                if (remainder == 0)
                    return result;
                if (remainder < 10)
                    return result + " lẻ " + ones[remainder];
                if (remainder < 100)
                    return result + " không trăm " + NumberToWords(remainder);
                return result + " " + NumberToWords(remainder);
            }

            if (number < 1000000000)
            {
                string result = NumberToWords(number / 1000000) + " triệu";
                int remainder = (int)(number % 1000000);
                if (remainder == 0)
                    return result;
                if (remainder < 1000)
                    return result + " không nghìn " + NumberToWords(remainder);
                return result + " " + NumberToWords(remainder);
            }

            string billions = NumberToWords(number / 1000000000) + " tỷ";
            long billionRemainder = number % 1000000000;
            if (billionRemainder == 0)
                return billions;
            if (billionRemainder < 1000000)
                return billions + " không triệu " + NumberToWords(billionRemainder);
            return billions + " " + NumberToWords(billionRemainder);
        }
    }
}