using System;
using System.Data;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using ChargingApp.Report;
using System.Data.SqlClient;

namespace ChargingApp.Helpers
{
    public static class ReportHelper
    {
        /// <summary>
        /// Hàm chung để đổ dữ liệu vào InvoiceReport từ DataTable
        /// VERSION 4: Cập nhật cho cấu trúc DB mới với Vehicle và IndividualVehicle
        /// FIXED: Xử lý null an toàn, đường dẫn ảnh rõ ràng, kiểm tra đầy đủ
        /// </summary>
        public static void PopulateInvoiceReport(InvoiceReport report, DataTable dtInvoice, DataTable dtInvoiceDetail)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report), "Report không được null");

            // 1. Đổ dữ liệu Header (Thông tin chung)
            if (dtInvoice != null && dtInvoice.Rows.Count > 0)
            {
                DataRow r = dtInvoice.Rows[0];

                // === HEADER INFORMATION ===
                SafeSetLabelValue(report.lbInvoiceNumber, r["InvoiceNumber"]);
                SafeSetLabelValue(report.lbInvoiceTemplate, r["InvoiceTemplate"]);
                SafeSetLabelValue(report.lbInvoiceSymbol, r["InvoiceSymbol"]);

                // === DATE INFORMATION ===
                if (r["CreatedAt"] != DBNull.Value)
                {
                    DateTime d = Convert.ToDateTime(r["CreatedAt"]);
                    SafeSetLabelValue(report.lbDate, d.Day.ToString("00"));
                    SafeSetLabelValue(report.lbMonth, d.Month.ToString("00"));
                    SafeSetLabelValue(report.lbYear, d.Year.ToString());
                }

                if (r["ExpireDate"] != DBNull.Value)
                {
                    SafeSetLabelValue(report.lbExpireDate,
                        Convert.ToDateTime(r["ExpireDate"]).ToString("dd/MM/yyyy"));
                }

                // === STATION INFORMATION ===
                SafeSetLabelValue(report.lbStationName, r["StationName"]);
                SafeSetLabelValue(report.lbStationAddress, r["StationAddress"]);
                SafeSetLabelValue(report.lbStationNumberphone, r["StationPhone"]);

                // === CUSTOMER INFORMATION ===
                // FIXED: Ưu tiên Snapshot (dữ liệu tại thời điểm tạo hóa đơn)
                string custName = GetStringValue(r, "Snashot_CustomerName");
                if (string.IsNullOrEmpty(custName))
                    custName = GetStringValue(r, "CustomerName", "N/A");

                string custPhone = GetStringValue(r, "Snashot_CustomerPhone");
                if (string.IsNullOrEmpty(custPhone))
                    custPhone = GetStringValue(r, "CustomerPhone");

                string custEmail = GetStringValue(r, "Snashot_CustomerEmail");
                if (string.IsNullOrEmpty(custEmail))
                    custEmail = GetStringValue(r, "CustomerEmail");

                string custAddress = GetStringValue(r, "CustomerAddress");
                string custTaxCode = GetStringValue(r, "CustomerTaxCode");

                // FIXED: Gán vào xrTable1 với kiểm tra an toàn
                PopulateCustomerInfoTable(report.xrTable1, custName, custPhone, custEmail, custTaxCode);

                // FIXED: Gán vào xrTableRow2 và xrTableRow3
                SafeSetTableCellValue(report.xrTableRow2, 1, custName);
                SafeSetTableCellValue(report.xrTableRow3, 1, custAddress);

                // === PAYMENT INFORMATION ===
                SafeSetLabelValue(report.lbPaymentMethod, r["PaymentMethod"]);

                string status = GetStringValue(r, "Status", "Unpaid");
                string statusText = status.Equals("Paid", StringComparison.OrdinalIgnoreCase)
                    ? "ĐÃ THANH TOÁN"
                    : "CHƯA THANH TOÁN";
                SafeSetLabelValue(report.lbStatus, statusText);

                // === TOTALS & MONEY IN WORDS ===
                decimal total = GetDecimalValue(r, "Total");
                decimal totalService = GetDecimalValue(r, "TotalAmountService");
                decimal totalTax = GetDecimalValue(r, "TotalAmountTax");
                decimal totalDiscount = GetDecimalValue(r, "TotalAmountDiscount");

                SafeSetTableCellValue(report.tblCellTotal, total.ToString("N0") + " VND");
                SafeSetTableCellValue(report.tbTableCellTotalinWord, NumberToWords((long)total) + " đồng");

                // FIXED: Gán các tổng con vào xrTable2 với kiểm tra an toàn
                PopulateTotalsTable(report.xrTable2, totalService, totalDiscount, totalTax);

                // === SIGNATURE IMAGE ===
                // FIXED: Xử lý đường dẫn ảnh chữ ký rõ ràng
                LoadSignatureImage(report.picBSignature, r["SignatureFile"]);
            }

            // 2. Đổ dữ liệu Detail (Bảng hàng hóa)
            if (dtInvoiceDetail != null && dtInvoiceDetail.Rows.Count > 0)
            {
                report.DataSource = dtInvoiceDetail;
                report.DataMember = "";

                // FIXED: Lấy % Thuế - kiểm tra nếu tất cả dòng có cùng thuế suất
                decimal firstTax = GetDecimalValue(dtInvoiceDetail.Rows[0], "Tax");
                bool allSameTax = true;

                for (int i = 1; i < dtInvoiceDetail.Rows.Count; i++)
                {
                    if (GetDecimalValue(dtInvoiceDetail.Rows[i], "Tax") != firstTax)
                    {
                        allSameTax = false;
                        break;
                    }
                }

                // Nếu tất cả dòng có cùng thuế suất thì hiển thị, không thì để trống
                if (report.tblCelITaxpercent != null)
                {
                    report.tblCelITaxpercent.Text = allSameTax
                        ? firstTax.ToString("0.##")
                        : ""; // Để trống nếu thuế suất khác nhau
                }

                // Bind dữ liệu vào các ô
                BindDetailColumns(report);
            }
        }


        public static void PopulateReceiptReport(ReceiptReport report, DataTable dtReceipt, DataTable dtReceiptDetail)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report), "Report không được null");

            // 1. Đổ dữ liệu Header (Thông tin chung)
            if (dtReceipt != null && dtReceipt.Rows.Count > 0)
            {
                DataRow r = dtReceipt.Rows[0];

                // === HEADER INFORMATION ===
                SafeSetLabelValue(report.lbReceiptNumber, r["ReceiptNumber"]);

                // === DATE INFORMATION ===
                if (r["ReceiptDate"] != DBNull.Value)
                {
                    DateTime d = Convert.ToDateTime(r["ReceiptDate"]);
                    SafeSetLabelValue(report.lbDate, d.Day.ToString("00"));
                    SafeSetLabelValue(report.lbMonth, d.Month.ToString("00"));
                    SafeSetLabelValue(report.lbYear, d.Year.ToString());
                }

                // === STATION INFORMATION ===
                SafeSetLabelValue(report.lbStationName, r["StationName"]);
                SafeSetLabelValue(report.lbStationAddress, r["StationAddress"]);
                SafeSetLabelValue(report.lbStationNumberphone, r["StationPhone"]);

                // === PAYER INFORMATION ===
                string payerName = GetStringValue(r, "PayerName");
                if (string.IsNullOrEmpty(payerName))
                    payerName = GetStringValue(r, "CustomerName", "N/A");

                SafeSetLabelValue(report.lbPayerName, payerName);
              //  SafeSetLabelValue(report.lbCustomerName, r["CustomerName"]);
                SafeSetLabelValue(report.lbCustomerNumberPhone, r["CustomerPhone"]);
              //  SafeSetLabelValue(report.lbCustomerAddress, r["CustomerAddress"]);

                // === DESCRIPTION ===
                SafeSetLabelValue(report.lbDescription, r["Description"]);

                // === RELATED INVOICE INFO (nếu có) ===
                string invoiceNumber = GetStringValue(r, "InvoiceNumber");
                if (!string.IsNullOrEmpty(invoiceNumber))
                {
                    SafeSetLabelValue(report.lbInvoiceNumber, invoiceNumber);
                //    SafeSetLabelValue(report.lbInvoiceTemplate, r["InvoiceTemplate"]);
                 //   SafeSetLabelValue(report.lbInvoiceSymbol, r["InvoiceSymbol"]);
                }

                // === TOTAL AMOUNT ===
                decimal totalAmount = GetDecimalValue(r, "TotalAmount");
                SafeSetLabelValue(report.lbTotalAmount, totalAmount.ToString("N0") + " VND");
                SafeSetLabelValue(report.lbTotalAmountInWords, NumberToWords((long)totalAmount) + " đồng");

                // === PAYMENT METHOD ===
                SafeSetLabelValue(report.lbPaymentMethod, r["PaymentMethod"]);

                // === CREATOR INFO ===
                //SafeSetLabelValue(report.lbCreatedBy, r["CreatedBy"]);
                SafeSetLabelValue(report.lbEmployeeName, r["EmployeeName"]);
                SafeSetLabelValue(report.lbTransaction_Code, r["TransactionCode"]);

                // === SIGNATURE IMAGE (nếu có) ===
            //LoadSignatureImage(report.picBSignature, r["SignatureFile"]);
            }
            if (dtReceiptDetail != null && dtReceiptDetail.Rows.Count > 0)
            {
                report.DataSource = dtReceiptDetail;
                report.DataMember = "";

                // Bind các cột chi tiết nếu cần
                // Nếu design có detail table thì uncomment phần này:
                /*
                report.tblCellSTT?.BindCell("STT");
                report.tblCelItemName?.BindCell("Description");
                report.tblCelIUnit?.BindCell("Debit_Account");
                report.tblCellQuantities?.BindCell("Credit_Account");
                report.tblCelIAmount?.BindCell("Amount", "{0:N0}");
                */
            }
        }
      



        /// <summary>
        /// Load dữ liệu Receipt theo ReceiptId
        /// Gọi FK với Invoice để lấy thông tin Station, với Customer để lấy thông tin khách hàng
        /// </summary>
        public static DataTable LoadReceiptDataById(long receiptId)
        {
            try
            {
                string query = @"
                        SELECT 
                            r.ReceiptId,
                            r.ReceiptNumber,
                            r.ReceiptDate,
                            r.PayerName,
                            r.Description,
                            r.TotalAmount,
                            r.PaymentMethod,
                            r.CreatedAt,
                            
                            r.InvoiceId,

                            -- Invoice info
                            i.InvoiceNumber,
                            i.InvoiceTemplate,
                            i.InvoiceSymbol,

                            -- Station Info (ưu tiên trạm theo hóa đơn, nếu không có thì lấy StationConfig)
                            s.Name AS StationName,
                            
                            CONCAT_WS(', ', s.Address, s.Street, s.Ward, s.District, s.City, s.Province) AS StationAddress,
                            s.Phone_Number AS StationPhone,

                            -- Customer info
                            ISNULL(r.PayerName, c.FullName) AS CustomerName,
                            c.PhoneNumber AS CustomerPhone,
                            c.Email AS CustomerEmail,
                            c.Address AS CustomerAddress,
                            c.TaxCode AS CustomerTaxCode,

                            -- Employee info
                            e.FullName AS EmployeeName,
                            -- Transaction_Code    
                            sac.Id,
                            ISNULL(sac.Transaction_Code,'') AS TransactionCode
                                
                        FROM Receipt r
                        LEFT JOIN Invoices i ON r.InvoiceId = i.InvoiceId
                        LEFT JOIN ChargingSession onl ON i.SessionId = onl.SessionId
                        LEFT JOIN IndividualVehicle iv ON onl.VIN = iv.VIN         
                          LEFT JOIN Connector con ON onl.ConnectorId = con.ConnectorId
                        LEFT JOIN Charger er ON con.ChargerId = er.ChargerId
                        LEFT JOIN Station s ON er.StationId = s.StationId
                        -- CROSS JOIN (SELECT TOP 1 * FROM StationConfig) sc
                        LEFT JOIN Customer c ON r.CustomerId = c.CustomerID
                        LEFT JOIN Employee e ON r.EmployeeId = e.EmployeeId
                        LEFT JOIN Transactions sac  ON r.TransactionId = sac.Id
                        
                        WHERE r.ReceiptId = @ReceiptId";

                return DataHelper.ExecuteQuery(query, new SqlParameter("@ReceiptId", receiptId));

            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi load dữ liệu Receipt: {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Load chi tiết Receipt theo ReceiptId
        /// Giữ nguyên để phát triển thêm sau
        /// </summary>
        public static DataTable LoadReceiptDetailDataById(long receiptId)
        {
            try
            {
                string query = @"
            SELECT 
                rd.ReceiptId,
                rd.STT,
                rd.Description,
                rd.Debit_Account,
                rd.Credit_Account,
                rd.Amount
            FROM ReceiptDetail rd
            WHERE rd.ReceiptId = @ReceiptId
            ORDER BY rd.STT";

                return DataHelper.ExecuteQuery(query, new SqlParameter("@ReceiptId", receiptId));


            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi load chi tiết Receipt: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy ReceiptId từ ReceiptNumber
        /// </summary>
        public static long? GetReceiptIdByNumber(string receiptNumber)
        {
            try
            {
                string query = "SELECT ReceiptId FROM Receipt WHERE ReceiptNumber = @ReceiptNumber";
                object result = DataHelper.ExecuteScalar(
                query,
                new SqlParameter("@ReceiptNumber", receiptNumber)
            );

                return result != null ? Convert.ToInt64(result) : (long?)null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy ReceiptId: {ex.Message}", ex);
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// FIXED: Populate Customer Info Table với kiểm tra đầy đủ
        /// </summary>
        private static void PopulateCustomerInfoTable(XRTable table, string name, string phone, string email, string taxCode)
        {
            if (table == null || table.Rows.Count < 4) return;

            try
            {
                if (table.Rows[0].Cells.Count > 1)
                    table.Rows[0].Cells[1].Text = name ?? "";

                if (table.Rows[1].Cells.Count > 1)
                    table.Rows[1].Cells[1].Text = phone ?? "";

                if (table.Rows[2].Cells.Count > 1)
                    table.Rows[2].Cells[1].Text = email ?? "";

                if (table.Rows[3].Cells.Count > 1)
                    table.Rows[3].Cells[1].Text = taxCode ?? "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error populating customer table: {ex.Message}");
            }
        }

        /// <summary>
        /// FIXED: Populate Totals Table với kiểm tra đầy đủ
        /// </summary>
        private static void PopulateTotalsTable(XRTable table, decimal totalService, decimal totalDiscount, decimal totalTax)
        {
            if (table == null || table.Rows.Count < 3) return;

            try
            {
                // Tổng tiền hàng
                if (table.Rows[0].Cells.Count > 1)
                    table.Rows[0].Cells[1].Text = totalService.ToString("N0") + " VND";

                // Tổng tiền chiết khấu
                if (table.Rows[1].Cells.Count > 3)
                    table.Rows[1].Cells[3].Text = totalDiscount.ToString("N0") + " VND";

                // Tổng tiền thuế
                if (table.Rows[2].Cells.Count > 3)
                    table.Rows[2].Cells[3].Text = totalTax.ToString("N0") + " VND";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error populating totals table: {ex.Message}");
            }
        }

        /// <summary>
        /// FIXED: Load ảnh chữ ký với xử lý đường dẫn rõ ràng
        /// </summary>
        private static void LoadSignatureImage(XRPictureBox pictureBox, object signatureFileValue)
        {
            if (pictureBox == null || signatureFileValue == null || signatureFileValue == DBNull.Value)
                return;

            try
            {
                string path = signatureFileValue.ToString();
                if (string.IsNullOrWhiteSpace(path))
                    return;

                // FIXED: Xử lý đường dẫn tương đối và tuyệt đối
                string fullPath = path;

                // Nếu là đường dẫn tương đối, ghép với thư mục gốc ứng dụng
                if (!Path.IsPathRooted(path))
                {
                    // Thử các thư mục phổ biến
                    string appPath = Application.StartupPath;
                    string[] possiblePaths = new[]
                    {
                        Path.Combine(appPath, path),
                        Path.Combine(appPath, "Images", path),
                        Path.Combine(appPath, "Signatures", path),
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                     "ChargingApp", "Signatures", path)
                    };

                    foreach (string testPath in possiblePaths)
                    {
                        if (File.Exists(testPath))
                        {
                            fullPath = testPath;
                            break;
                        }
                    }
                }

                // Kiểm tra file tồn tại
                if (File.Exists(fullPath))
                {
                    pictureBox.ImageUrl = fullPath;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Signature file not found: {fullPath}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading signature: {ex.Message}");
            }
        }

        /// <summary>
        /// Bind các cột chi tiết hóa đơn
        /// </summary>
        private static void BindDetailColumns(InvoiceReport report)
        {
            report.tblCellSTT?.BindCell("STT");
            report.tblCelItemName?.BindCell("ItemName");
            report.tblCelIUnit?.BindCell("Unit");
            report.tblCellQuantities?.BindCell("Quantities", "{0:N0}");
            report.tblCelIUnitPrice?.BindCell("UnitPrice", "{0:N0}");
            report.tblCelIAmount?.BindCell("Amount", "{0:N0}");
            report.tblCelIDiscountPercent?.BindCell("DiscountPercent", "{0:0.00}%");
            report.tblCelIDiscountAmout?.BindCell("DiscountAmount", "{0:N0}");
            report.tblCelITaxAmount?.BindCell("TaxAmount", "{0:N0}");
        }

        /// <summary>
        /// Gán giá trị cho Label một cách an toàn
        /// </summary>
        private static void SafeSetLabelValue(XRLabel label, object value)
        {
            if (label == null) return;

            try
            {
                if (value == null || value == DBNull.Value)
                {
                    label.Text = "";
                }
                else
                {
                    label.Text = value.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting label value: {ex.Message}");
                label.Text = "";
            }
        }

        /// <summary>
        /// Gán giá trị cho Table Cell một cách an toàn
        /// </summary>
        private static void SafeSetTableCellValue(XRTableCell cell, object value)
        {
            if (cell == null) return;

            try
            {
                if (value == null || value == DBNull.Value)
                {
                    cell.Text = "";
                }
                else
                {
                    cell.Text = value.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting table cell value: {ex.Message}");
                cell.Text = "";
            }
        }


        /// <summary>
        /// Gán giá trị cho Table Cell theo index
        /// </summary>
        private static void SafeSetTableCellValue(XRTableRow row, int cellIndex, object value)
        {
            if (row == null || row.Cells.Count <= cellIndex) return;

            SafeSetTableCellValue(row.Cells[cellIndex], value);
        }

        /// <summary>
        /// Lấy giá trị string từ DataRow một cách an toàn
        /// </summary>
        private static string GetStringValue(DataRow row, string columnName, string defaultValue = "")
        {
            try
            {
                if (row.Table.Columns.Contains(columnName))
                {
                    object val = row[columnName];
                    if (val != null && val != DBNull.Value)
                    {
                        return val.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting string value for {columnName}: {ex.Message}");
            }

            return defaultValue;
        }

        /// <summary>
        /// Lấy giá trị decimal từ DataRow một cách an toàn
        /// </summary>
        private static decimal GetDecimalValue(DataRow row, string columnName, decimal defaultValue = 0)
        {
            try
            {
                if (row.Table.Columns.Contains(columnName))
                {
                    object val = row[columnName];
                    if (val != null && val != DBNull.Value)
                    {
                        if (decimal.TryParse(val.ToString(), out decimal result))
                        {
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting decimal value for {columnName}: {ex.Message}");
            }

            return defaultValue;
        }

        /// <summary>
        /// Chuyển số thành chữ tiếng Việt
        /// </summary>
        private static string NumberToWords(long number)
        {
            if (number == 0)
                return "Không";

            if (number < 0)
                return "Âm " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000000) > 0)
            {
                words += NumberToWords(number / 1000000000) + " tỷ ";
                number %= 1000000000;
            }

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " triệu ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " nghìn ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " trăm ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "lẻ ";

                var unitsMap = new[] { "", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
                var tensMap = new[] { "", "mười", "hai mươi", "ba mươi", "bốn mươi", "năm mươi",
                                      "sáu mươi", "bảy mươi", "tám mươi", "chín mươi" };

                if (number < 10)
                    words += unitsMap[number];
                else if (number < 20)
                {
                    words += "mười " + unitsMap[number % 10];
                }
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }

            return words.Trim();
        }

        /// <summary>
        /// Extension method để bind cell dễ dàng hơn
        /// </summary>
        private static void BindCell(this XRTableCell cell, string fieldName, string formatString = "")
        {
            if (cell == null) return;

            try
            {
                cell.DataBindings.Clear();

                var binding = new XRBinding("Text", null, fieldName);

                if (!string.IsNullOrEmpty(formatString))
                {
                    binding.FormatString = formatString;
                }

                cell.DataBindings.Add(binding);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error binding cell to {fieldName}: {ex.Message}");
            }
        }

        #endregion
    }
}