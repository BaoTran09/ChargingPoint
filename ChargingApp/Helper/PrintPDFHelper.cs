using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting; // Đã thêm: Cần thiết cho ExportToPdf
using System.Diagnostics;      // Đã thêm: Cần thiết cho Process.Start

namespace ChargingApp.Accounting
{
    public static class PrintPDFHelper
    {
        public static void PrintGridToPDF(DevExpress.XtraGrid.GridControl gridControl, string title = "Báo cáo")
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = $"{title}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                    Title = "Lưu file PDF"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Đảm bảo ExportToPdf được gọi trên View chính (ví dụ: GridView) nếu GridControl có nhiều View
                    // Nhưng việc gọi trên GridControl cũng thường hoạt động.
                    gridControl.ExportToPdf(saveDialog.FileName);

                    if (XtraMessageBox.Show("Xuất PDF thành công!\n\nBạn có muốn mở file ngay?",
                        "Thành công", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // Sử dụng Process.StartInfo để tránh lỗi khi dùng System.Diagnostics.Process.Start(string)
                        System.Diagnostics.Process.Start(new ProcessStartInfo(saveDialog.FileName) { UseShellExecute = true });
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi khi xuất PDF: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void PrintRecognitionToPDF(long recognitionId, string connectionString)
        {
            try
            {
                DataSet ds = LoadRecognitionData(recognitionId, connectionString);

                // Kiểm tra null an toàn hơn
                if (ds == null || !ds.Tables.Contains("Header") || ds.Tables["Header"].Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không tìm thấy dữ liệu hạch toán!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = $"HachToan_{recognitionId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                    Title = "Lưu chứng từ hạch toán"
                };

                if (saveDialog.ShowDialog() != DialogResult.OK) return;

                string html = BuildHTMLReport(ds);

                // Lưu file tạm dưới dạng HTML
                string htmlPath = Path.ChangeExtension(saveDialog.FileName, ".html");
                File.WriteAllText(htmlPath, html, System.Text.Encoding.UTF8);

                XtraMessageBox.Show(
                    $"Đã tạo file thành công!\n\nFile HTML tạm: {htmlPath}\n\n" +
                    "Mở file → Ctrl + P → Chọn 'Save as PDF' để có file PDF đẹp nhất.",
                    "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Mở file HTML tạm trong trình duyệt mặc định
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(htmlPath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi khi in chứng từ: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static DataSet LoadRecognitionData(long recognitionId, string connectionString)
        {
            var ds = new DataSet();

            string queryHeader = @"
                SELECT 
                    r.*,
                    e.FullName AS EmployeeName,
                    ISNULL(c.FullName, i.Snashot_CustomerName) AS CustomerName,  -- Sửa Snashot_CustomerName
                    ISNULL(c.Address, '') AS CustomerAddress,
                    ISNULL(c.TaxCode, '') AS CustomerTaxCode,
                    ISNULL(c.PhoneNumber, i.Snashot_CustomerPhone) AS CustomerPhone, -- Sửa Snashot_CustomerPhone
                    i.InvoiceNumber,
                    ISNULL(s.Station_Name, 'Trạm sạc') AS StationName
                FROM Revenue_recognition r
                LEFT JOIN Employee e ON r.Employeeid = e.EmployeeId
                LEFT JOIN Invoices i ON r.InvoiceId = i.InvoiceId
                LEFT JOIN Customer c ON i.CustomerId = c.CustomerId
                LEFT JOIN ChargingSession cs ON i.SessionId = cs.SessionId
                LEFT JOIN Connector con ON cs.ConnectorId = con.ConnectorId
                LEFT JOIN Charger ch ON con.ChargerId = ch.ChargerId
                LEFT JOIN Station s ON ch.StationId = s.StationId
                WHERE r.Id = @Id";

            string queryDetail = @"
                SELECT 
                    rd.*,
                    ISNULL(ri.ItemName, rd.Interpretation) AS ServiceName_Display,
                    ISNULL(ri.Unit, rd.Unit_measure) AS DisplayUnit
                FROM Record_detail rd
                LEFT JOIN RevenueItem ri ON rd.ItemId = ri.ItemId
                WHERE rd.Recognition_id = @Id
                ORDER BY rd.STT";

            // Sửa lại cú pháp using declaration thành khối using truyền thống
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(queryHeader, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", recognitionId);
                    new SqlDataAdapter(cmd).Fill(ds, "Header");
                }

                using (SqlCommand cmd = new SqlCommand(queryDetail, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", recognitionId);
                    new SqlDataAdapter(cmd).Fill(ds, "Detail");
                }
            } // conn.Dispose() được gọi ở đây

            return ds;
        }

        private static string BuildHTMLReport(DataSet ds)
        {
            DataRow h = ds.Tables["Header"].Rows[0];
            DataTable d = ds.Tables["Detail"];

            decimal totalAmount = 0;
            decimal totalTax = 0;

            var sb = new System.Text.StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><head><meta charset='utf-8'><title>Chứng từ hạch toán</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body{font-family:'Times New Roman',serif;margin:40px;font-size:14px;}");
            sb.AppendLine(".header{text-align:center;margin-bottom:30px;}");
            sb.AppendLine(".label{font-weight:bold;width:180px;display:inline-block;}");
            sb.AppendLine("table{width:100%;border-collapse:collapse;margin-top:20px;}");
            sb.AppendLine("th,td{border:1px solid #000;padding:8px;text-align:center;}");
            sb.AppendLine("th{background:#f0f0f0;}");
            sb.AppendLine(".text-right{text-align:right;}");
            sb.AppendLine(".total{font-weight:bold;background:#f9f9f9;}");
            sb.AppendLine(".signature{margin-top:80px;text-align:center;width:45%;display:inline-block;}");
            sb.AppendLine("</style></head><body>");

            sb.AppendLine("<div class='header'>");
            sb.AppendLine("<h1>CHỨNG TỪ GHI SỔ DOANH THU</h1>");
            sb.AppendLine($"<p><strong>Số chứng từ:</strong> HT{h["Id"]} &nbsp;&nbsp;&nbsp;&nbsp;" +
                          $"<strong>Ngày hạch toán:</strong> {Convert.ToDateTime(h["RecordDate"]):dd/MM/yyyy}</p>");
            sb.AppendLine("</div>");

            sb.AppendLine("<div style='line-height:1.8;'>");
            sb.AppendLine($"<div><span class='label'>Khách hàng:</span>{h["CustomerName"]}</div>");
            sb.AppendLine($"<div><span class='label'>Mã số thuế:</span>{h["CustomerTaxCode"]}</div>");
            sb.AppendLine($"<div><span class='label'>Địa chỉ:</span>{h["CustomerAddress"]}</div>");
            sb.AppendLine($"<div><span class='label'>Điện thoại:</span>{h["CustomerPhone"]}</div>");
            sb.AppendLine($"<div><span class='label'>Hóa đơn số:</span>{h["InvoiceNumber"]}</div>");
            sb.AppendLine($"<div><span class='label'>Trạm sạc:</span>{h["StationName"]}</div>");
            sb.AppendLine($"<div><span class='label'>Thanh toán:</span>" +
                          $"{(Convert.ToBoolean(h["Money_received"]) ? "Đã thu tiền" : "Chưa thu tiền")} - {h["Methods_payment"]}</div>");
            sb.AppendLine("</div>");

            sb.AppendLine("<h3 style='text-align:center;margin:30px 0 10px;'>CHI TIẾT HẠCH TOÁN</h3>");
            sb.AppendLine("<table>");
            sb.AppendLine("<thead><tr>"); // Đã sửa lỗi thiếu dấu chấm phẩy
            sb.AppendLine("<th>STT</th><th>Diễn giải</th><th>TK Nợ</th><th>TK Có</th>");
            sb.AppendLine("<th>ĐVT</th><th>Số lượng</th><th>Đơn giá</th>");
            sb.AppendLine("<th>Thành tiền</th><th>Thuế GTGT</th><th>Tiền thuế</th></tr></thead><tbody>");

            int stt = 1;
            foreach (DataRow row in d.Rows)
            {
                // Sử dụng Convert.ToDecimal an toàn hơn, không cần ?? 0 nếu giá trị được đảm bảo là số
                decimal amount = Convert.ToDecimal(row["Amount"]);
                decimal taxAmt = Convert.ToDecimal(row["TaxAmount"]);
                totalAmount += amount;
                totalTax += taxAmt;

                sb.AppendLine($"<tr>");
                sb.AppendLine($"<td>{stt++}</td>");
                sb.AppendLine($"<td style='text-align:left'>{row["ServiceName_Display"]}</td>");
                sb.AppendLine($"<td>{row["Debit_account"]}</td>");
                sb.AppendLine($"<td>{row["Credit_account"]}</td>");
                sb.AppendLine($"<td>{row["DisplayUnit"]}</td>");
                sb.AppendLine($"<td class='text-right'>{Convert.ToDecimal(row["Quantities"]):N2}</td>");
                sb.AppendLine($"<td class='text-right'>{Convert.ToDecimal(row["UnitPrice"]):N0}</td>");
                sb.AppendLine($"<td class='text-right'>{amount:N0}</td>");
                sb.AppendLine($"<td class='text-right'>{Convert.ToDecimal(row["TaxPercentage"]):N2}%</td>");
                sb.AppendLine($"<td class='text-right'>{taxAmt:N0}</td>");
                sb.AppendLine($"</tr>");
            }

            decimal grandTotal = totalAmount + totalTax;

            sb.AppendLine("<tr class='total'>");
            sb.AppendLine("<td colspan='7' class='text-right'><strong>TỔNG CỘNG:</strong></td>");
            sb.AppendLine($"<td class='text-right'><strong>{totalAmount:N0}</strong></td>");
            sb.AppendLine("<td></td>");
            sb.AppendLine($"<td class='text-right'><strong>{totalTax:N0}</strong></td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr class='total'>");
            sb.AppendLine("<td colspan='7' class='text-right'><strong>TỔNG TIỀN THANH TOÁN:</strong></td>");
            sb.AppendLine($"<td colspan='3' class='text-right'><strong>{grandTotal:N0} VNĐ</strong></td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("</tbody></table>");

            sb.AppendLine("<div style='margin-top:80px;'>");
            sb.AppendLine("<div class='signature' style='float:left;'><strong>Người lập biểu</strong><br><br><br>" + h["EmployeeName"] + "</div>");
            sb.AppendLine("<div class='signature' style='float:right;'><strong>Kế toán trưởng</strong><br><br><br>(Ký, họ tên)</div>");
            sb.AppendLine("<div style='clear:both;'></div></div>");

            sb.AppendLine($"<p style='text-align:center;margin-top:50px;font-style:italic;'>Ngày in: {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>");
            sb.AppendLine("</body></html>");

            return sb.ToString();
        }

        public static void PrintGridDirectly(DevExpress.XtraGrid.GridControl gridControl)
        {
            try { gridControl.ShowPrintPreview(); }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi khi in: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}