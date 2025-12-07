using System;
using System.Data;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using ChargingApp.Report;

namespace ChargingApp.Helpers
{
    public static class ReportHelper
    {
        /// <summary>
        /// Hàm chung để đổ dữ liệu vào InvoiceReport từ DataTable
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
        /// FIXED: Lấy giá trị string an toàn từ DataRow
        /// </summary>
        private static string GetStringValue(DataRow row, string columnName, string defaultValue = "")
        {
            if (row == null || !row.Table.Columns.Contains(columnName))
                return defaultValue;

            object value = row[columnName];
            if (value == null || value == DBNull.Value)
                return defaultValue;

            return value.ToString().Trim();
        }

        /// <summary>
        /// FIXED: Lấy giá trị decimal an toàn từ DataRow
        /// </summary>
        private static decimal GetDecimalValue(DataRow row, string columnName, decimal defaultValue = 0)
        {
            if (row == null || !row.Table.Columns.Contains(columnName))
                return defaultValue;

            object value = row[columnName];
            if (value == null || value == DBNull.Value)
                return defaultValue;

            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Set giá trị cho XRLabel an toàn
        /// </summary>
        private static void SafeSetLabelValue(XRLabel label, object value)
        {
            if (label != null)
            {
                label.Text = value?.ToString() ?? "";
            }
        }

        /// <summary>
        /// Set giá trị cho XRTableCell an toàn
        /// </summary>
        private static void SafeSetTableCellValue(XRTableCell cell, object value)
        {
            if (cell != null)
            {
                cell.Text = value?.ToString() ?? "";
            }
        }

        /// <summary>
        /// Set giá trị cho cell trong XRTableRow theo index
        /// </summary>
        private static void SafeSetTableCellValue(XRTableRow row, int cellIndex, object value)
        {
            if (row != null && row.Cells.Count > cellIndex)
            {
                row.Cells[cellIndex].Text = value?.ToString() ?? "";
            }
        }

        #endregion

        #region Number To Words - Optimized

        /// <summary>
        /// FIXED: Hàm đọc số thành chữ tiếng Việt được tối ưu
        /// </summary>
        public static string NumberToWords(long number)
        {
            if (number == 0) return "Không";
            if (number < 0) return "Âm " + NumberToWords(Math.Abs(number));

            string[] ones = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] tens = { "", "mười", "hai mươi", "ba mươi", "bốn mươi", "năm mươi", "sáu mươi", "bảy mươi", "tám mươi", "chín mươi" };
            string[] hundreds = { "", "một trăm", "hai trăm", "ba trăm", "bốn trăm", "năm trăm", "sáu trăm", "bảy trăm", "tám trăm", "chín trăm" };

            string ReadThreeDigits(int n)
            {
                if (n == 0) return "";

                int h = n / 100;
                int t = (n % 100) / 10;
                int o = n % 10;

                string str = hundreds[h];

                if (t > 0 || o > 0)
                {
                    if (h > 0 && t == 0 && o > 0)
                    {
                        str += " linh " + ones[o];
                    }
                    else if (t == 0)
                    {
                        // Không có gì
                    }
                    else if (t == 1)
                    {
                        str += (o == 0) ? " mười" : " mười " + ones[o];
                    }
                    else
                    {
                        str += " " + tens[t];
                        if (o == 1) str += " mốt";
                        else if (o == 5 && t > 1) str += " lăm";
                        else if (o > 0) str += " " + ones[o];
                    }
                }

                return str.Trim();
            }

            string result = "";

            // Tỷ
            if (number >= 1_000_000_000)
            {
                result += ReadThreeDigits((int)(number / 1_000_000_000)) + " tỷ";
                number %= 1_000_000_000;
                if (number > 0) result += " ";
            }

            // Triệu
            if (number >= 1_000_000)
            {
                result += ReadThreeDigits((int)(number / 1_000_000)) + " triệu";
                number %= 1_000_000;
                if (number > 0) result += " ";
            }

            // Nghìn
            if (number >= 1000)
            {
                result += ReadThreeDigits((int)(number / 1000)) + " nghìn";
                number %= 1000;
                if (number > 0) result += " ";
            }

            // Đơn vị
            if (number > 0)
            {
                result += ReadThreeDigits((int)number);
            }

            result = result.Trim();

            // Viết hoa chữ cái đầu
            if (result.Length > 0)
            {
                result = char.ToUpper(result[0]) + result.Substring(1);
            }

            return result;
        }

        #endregion

        #region Extension Methods

        /// <summary>
        /// Extension method để set giá trị cho XRLabel
        /// </summary>
        public static void SetValue(this XRLabel label, string value)
        {
            if (label != null) label.Text = value ?? "";
        }

        /// <summary>
        /// Extension method để set giá trị cho XRTableCell
        /// </summary>
        public static void SetValue(this XRTableCell cell, string value)
        {
            if (cell != null) cell.Text = value ?? "";
        }

        /// <summary>
        /// Extension method để bind dữ liệu cho XRTableCell
        /// </summary>
        public static void BindCell(this XRTableCell cell, string fieldName, string formatString = "")
        {
            if (cell != null)
            {
                cell.DataBindings.Clear();
                cell.DataBindings.Add("Text", null, fieldName, formatString);
            }
        }

        #endregion
    }
}