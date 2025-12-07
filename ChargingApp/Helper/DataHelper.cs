using System;
using System.Data;
using System.Data.SqlClient;

namespace ChargingApp.Helpers
{
    public static class DataHelper
    {
        private static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=ChargingPoint;Integrated Security=True;Connect Timeout=30";

        #region Load Invoice Data

        public static DataTable LoadInvoiceDataById(long invoiceId)
        {
            string query = @"
                SELECT 
                    i.InvoiceId, 
                    i.InvoiceNumber, 
                    i.InvoiceTemplate, 
                    i.InvoiceSymbol, 
                    i.CreatedAt, 
                    i.ExpireDate, 
                    i.Status, 
                    i.PaymentMethod,
                    -- FIXED: Ưu tiên Snapshot (dữ liệu tại thời điểm tạo HĐ)
                    ISNULL(i.Snashot_CustomerName, ISNULL(c.FullName, 'N/A')) AS Snashot_CustomerName,
                    ISNULL(i.Snashot_CustomerPhone, c.PhoneNumber) AS Snashot_CustomerPhone,
                    ISNULL(i.Snashot_CustomerEmail, c.Email) AS Snashot_CustomerEmail,
                    i.Total, 
                    i.TotalAmountService, 
                    i.TotalAmountTax, 
                    i.TotalAmountDiscount,
                    -- Thông tin Customer hiện tại (để tham khảo)
                    ISNULL(c.CustomerId, 0) AS CustomerId, 
                    ISNULL(c.FullName, i.Snashot_CustomerName) AS CustomerName, 
                    ISNULL(c.PhoneNumber, i.Snashot_CustomerPhone) AS CustomerPhone, 
                    ISNULL(c.Email, i.Snashot_CustomerEmail) AS CustomerEmail, 
                    ISNULL(c.Address, '') AS CustomerAddress, 
                    ISNULL(c.TaxCode, '') AS CustomerTaxCode,
                    -- Thông tin Station
                    ISNULL(s.StationId, 0) AS StationId, 
                    ISNULL(s.Name, 'N/A') AS StationName, 
                    ISNULL(s.Address, '') AS StationAddress, 
                    ISNULL(s.Phone_Number, '') AS StationPhone,
                    i.SignatureFile AS SignatureFile
                FROM Invoices i
                LEFT JOIN Customer c ON i.CustomerId = c.CustomerId
                LEFT JOIN ChargingSession cs ON i.SessionId = cs.SessionId
                LEFT JOIN Connector con ON cs.ConnectorId = con.ConnectorId
                LEFT JOIN Charger ch ON con.ChargerId = ch.ChargerId
                LEFT JOIN Station s ON ch.StationId = s.StationId
                WHERE i.InvoiceId = @InvoiceId";

            return ExecuteQuery(query, new SqlParameter("@InvoiceId", invoiceId));
        }

        public static DataTable LoadInvoiceDetailDataById(long invoiceId)
        {
            string query = @"
                SELECT 
                    id.STT, 
                    id.ItemId, 
                    ISNULL(ri.ItemName, 'N/A') AS ItemName, 
                    id.Quantities, 
                    ISNULL(ri.Unit, '') AS Unit, 
                    id.UnitPrice, 
                    id.Amount,
                    id.DiscountPercent, 
                    id.DiscountAmount, 
                    id.AmountAfterDiscount, 
                    id.Tax, 
                    id.TaxAmount, 
                    id.TotalLine
                FROM InvoiceDetail id
                LEFT JOIN RevenueItem ri ON id.ItemId = ri.ItemId
                WHERE id.InvoiceId = @InvoiceId
                ORDER BY id.STT";

            return ExecuteQuery(query, new SqlParameter("@InvoiceId", invoiceId));
        }

        #endregion

        #region Load Document List (Invoice, Receipt, Revenue)

        /// <summary>
        /// Load danh sách chứng từ (Invoice, Receipt, Revenue_recognition)
        /// FIXED: Xử lý tham số an toàn hơn, đảm bảo có cột Action
        /// </summary>
        public static DataTable LoadDocumentList(
            string documentType = "All",
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string paymentStatus = null,
            string recordingStatus = null)
        {
            DataTable result = new DataTable();

            switch (documentType)
            {
                case "Invoice":
                    result = LoadInvoiceList(fromDate, toDate, paymentStatus, recordingStatus);
                    break;

                case "Receipt":
                    result = LoadReceiptList(fromDate, toDate, paymentStatus);
                    break;

                case "Revenue":
                    result = LoadRevenueRecognitionList(fromDate, toDate);
                    break;

                case "All":
                default:
                    result = LoadAllDocuments(fromDate, toDate, paymentStatus, recordingStatus);
                    break;
            }

            // FIXED: Đảm bảo luôn có cột Action
            if (!result.Columns.Contains("Action"))
            {
                result.Columns.Add("Action", typeof(string));
            }

            return result;
        }

        private static DataTable LoadInvoiceList(
            DateTime? fromDate,
            DateTime? toDate,
            string paymentStatus,
            string recordingStatus)
        {
            string query = @"
                SELECT 
                    'Invoice' AS DocumentType,
                    i.InvoiceId AS DocumentId,
                    CAST(i.InvoiceNumber AS NVARCHAR(50)) AS DocumentNumber,
                    i.CreatedAt AS DocumentDate,
                    -- FIXED: Ưu tiên Snapshot trước
                    ISNULL(i.Snashot_CustomerName, ISNULL(c.FullName, 'N/A')) AS CustomerName,
                    ISNULL(i.Snashot_CustomerEmail, ISNULL(c.Email, '')) AS CustomerEmail,
                    i.Total AS TotalAmount,
                    i.Status AS PaymentStatus,
                    i.PaymentMethod,
                    i.CustomerId,
                    CASE 
                        WHEN EXISTS (
                            SELECT 1 FROM Revenue_recognition rr 
                            WHERE rr.InvoiceId = i.InvoiceId
                        ) THEN N'Đã ghi sổ'
                        ELSE N'Chưa ghi sổ'
                    END AS RecordingStatus
                FROM Invoices i
                LEFT JOIN Customer c ON i.CustomerId = c.CustomerId
                WHERE 1=1";

            if (fromDate.HasValue)
                query += " AND i.CreatedAt >= @FromDate";

            if (toDate.HasValue)
                query += " AND i.CreatedAt <= @ToDate";

            if (!string.IsNullOrEmpty(paymentStatus) && paymentStatus != "All")
                query += " AND i.Status = @PaymentStatus";

            if (!string.IsNullOrEmpty(recordingStatus))
            {
                if (recordingStatus == "Posted")
                    query += " AND EXISTS (SELECT 1 FROM Revenue_recognition rr WHERE rr.InvoiceId = i.InvoiceId)";
                else if (recordingStatus == "Unposted")
                    query += " AND NOT EXISTS (SELECT 1 FROM Revenue_recognition rr WHERE rr.InvoiceId = i.InvoiceId)";
            }

            query += " ORDER BY i.CreatedAt DESC";

            return ExecuteQueryWithParams(query, fromDate, toDate, paymentStatus);
        }

        private static DataTable LoadReceiptList(
            DateTime? fromDate,
            DateTime? toDate,
            string paymentStatus)
        {
            string query = @"
                SELECT 
                    'Receipt' AS DocumentType,
                    r.ReceiptId AS DocumentId,
                    r.ReceiptNumber AS DocumentNumber,
                    r.ReceiptDate AS DocumentDate,
                    ISNULL(r.PayerName, ISNULL(c.FullName, 'N/A')) AS CustomerName,
                    ISNULL(c.Email, '') AS CustomerEmail,
                    r.TotalAmount AS TotalAmount,
                    -- FIXED: Mapping rõ ràng cho Receipt Status
                    CASE r.Status
                        WHEN 'Posted' THEN N'Đã ghi sổ'
                        WHEN 'Cancelled' THEN N'Đã hủy'
                        WHEN 'Draft' THEN N'Nháp'
                        ELSE r.Status
                    END AS PaymentStatus,
                    r.PaymentMethod,
                    r.CustomerId,
                    -- Receipt luôn có RecordingStatus = Status (vì không có bước ghi sổ riêng)
                    CASE r.Status
                        WHEN 'Posted' THEN N'Đã ghi sổ'
                        ELSE N'Chưa ghi sổ'
                    END AS RecordingStatus
                FROM Receipt r
                LEFT JOIN Customer c ON r.CustomerId = c.CustomerId
                WHERE 1=1";

            if (fromDate.HasValue)
                query += " AND r.ReceiptDate >= @FromDate";

            if (toDate.HasValue)
                query += " AND r.ReceiptDate <= @ToDate";

            // FIXED: Filter theo Status của Receipt
            if (!string.IsNullOrEmpty(paymentStatus) && paymentStatus != "All")
            {
                // Map UI value sang DB value
                string dbStatus = paymentStatus;
                if (paymentStatus == "Đã ghi sổ") dbStatus = "Posted";
                if (paymentStatus == "Đã hủy") dbStatus = "Cancelled";

                query += " AND r.Status = @PaymentStatus";
            }

            query += " ORDER BY r.ReceiptDate DESC";

            return ExecuteQueryWithParams(query, fromDate, toDate, paymentStatus);
        }

        private static DataTable LoadRevenueRecognitionList(
            DateTime? fromDate,
            DateTime? toDate)
        {
            string query = @"
                SELECT 
                    'Revenue' AS DocumentType,
                    rr.Id AS DocumentId,
                    CAST(rr.Id AS NVARCHAR(50)) AS DocumentNumber,
                    rr.RecordDate AS DocumentDate,
                    ISNULL(c.FullName, 'N/A') AS CustomerName,
                    ISNULL(c.Email, '') AS CustomerEmail,
                    ISNULL((SELECT SUM(Amount) FROM Record_detail WHERE Recognition_id = rr.Id), 0) AS TotalAmount,
                    N'Đã ghi sổ' AS PaymentStatus,
                    rr.Methods_payment AS PaymentMethod,
                    rr.CustomerId,
                    N'Đã ghi sổ' AS RecordingStatus
                FROM Revenue_recognition rr
                LEFT JOIN Customer c ON rr.CustomerId = c.CustomerId
                WHERE 1=1";

            if (fromDate.HasValue)
                query += " AND rr.RecordDate >= @FromDate";

            if (toDate.HasValue)
                query += " AND rr.RecordDate <= @ToDate";

            query += " ORDER BY rr.RecordDate DESC";

            return ExecuteQueryWithParams(query, fromDate, toDate, null);
        }

        private static DataTable LoadAllDocuments(
            DateTime? fromDate,
            DateTime? toDate,
            string paymentStatus,
            string recordingStatus)
        {
            // FIXED: UNION ALL được tối ưu để tránh duplicate
            string query = @"
                SELECT * FROM (
                    -- Invoices
                    SELECT 
                        'Invoice' AS DocumentType,
                        i.InvoiceId AS DocumentId,
                        CAST(i.InvoiceNumber AS NVARCHAR(50)) AS DocumentNumber,
                        i.CreatedAt AS DocumentDate,
                        ISNULL(i.Snashot_CustomerName, ISNULL(c.FullName, 'N/A')) AS CustomerName,
                        ISNULL(i.Snashot_CustomerEmail, ISNULL(c.Email, '')) AS CustomerEmail,
                        i.Total AS TotalAmount,
                        i.Status AS PaymentStatus,
                        i.PaymentMethod,
                        i.CustomerId,
                        CASE 
                            WHEN EXISTS (SELECT 1 FROM Revenue_recognition rr WHERE rr.InvoiceId = i.InvoiceId) 
                            THEN N'Đã ghi sổ' ELSE N'Chưa ghi sổ' 
                        END AS RecordingStatus
                    FROM Invoices i
                    LEFT JOIN Customer c ON i.CustomerId = c.CustomerId

                    UNION ALL

                    -- Receipts
                    SELECT 
                        'Receipt' AS DocumentType,
                        r.ReceiptId AS DocumentId,
                        r.ReceiptNumber AS DocumentNumber,
                        r.ReceiptDate AS DocumentDate,
                        ISNULL(r.PayerName, ISNULL(c.FullName, 'N/A')) AS CustomerName,
                        ISNULL(c.Email, '') AS CustomerEmail,
                        r.TotalAmount AS TotalAmount,
                        CASE r.Status
                            WHEN 'Posted' THEN N'Đã ghi sổ'
                            WHEN 'Cancelled' THEN N'Đã hủy'
                            WHEN 'Draft' THEN N'Nháp'
                            ELSE r.Status
                        END AS PaymentStatus,
                        r.PaymentMethod,
                        r.CustomerId,
                        CASE r.Status
                            WHEN 'Posted' THEN N'Đã ghi sổ'
                            ELSE N'Chưa ghi sổ'
                        END AS RecordingStatus
                    FROM Receipt r
                    LEFT JOIN Customer c ON r.CustomerId = c.CustomerId

                    UNION ALL

                    -- Revenue_recognition
                    SELECT 
                        'Revenue' AS DocumentType,
                        rr.Id AS DocumentId,
                        CAST(rr.Id AS NVARCHAR(50)) AS DocumentNumber,
                        rr.RecordDate AS DocumentDate,
                        ISNULL(c.FullName, 'N/A') AS CustomerName,
                        ISNULL(c.Email, '') AS CustomerEmail,
                        ISNULL((SELECT SUM(Amount) FROM Record_detail WHERE Recognition_id = rr.Id), 0) AS TotalAmount,
                        N'Đã ghi sổ' AS PaymentStatus,
                        rr.Methods_payment AS PaymentMethod,
                        rr.CustomerId,
                        N'Đã ghi sổ' AS RecordingStatus
                    FROM Revenue_recognition rr
                    LEFT JOIN Customer c ON rr.CustomerId = c.CustomerId
                ) AS AllDocuments
                WHERE 1=1";

            if (fromDate.HasValue)
                query += " AND DocumentDate >= @FromDate";

            if (toDate.HasValue)
                query += " AND DocumentDate <= @ToDate";

            // FIXED: Filter theo PaymentStatus với logic rõ ràng
            if (!string.IsNullOrEmpty(paymentStatus) && paymentStatus != "All")
            {
                // Chuyển đổi UI value sang DB value
                if (paymentStatus == "Paid" || paymentStatus == "Unpaid")
                {
                    query += " AND PaymentStatus = @PaymentStatus";
                }
                else if (paymentStatus == "Posted" || paymentStatus == "Đã ghi sổ")
                {
                    query += " AND (PaymentStatus = N'Đã ghi sổ' OR PaymentStatus = 'Posted')";
                }
                else if (paymentStatus == "Cancelled" || paymentStatus == "Đã hủy")
                {
                    query += " AND (PaymentStatus = N'Đã hủy' OR PaymentStatus = 'Cancelled')";
                }
            }

            if (!string.IsNullOrEmpty(recordingStatus))
            {
                if (recordingStatus == "Posted")
                    query += " AND RecordingStatus = N'Đã ghi sổ'";
                else if (recordingStatus == "Unposted")
                    query += " AND RecordingStatus = N'Chưa ghi sổ'";
            }

            query += " ORDER BY DocumentDate DESC";

            return ExecuteQueryWithParams(query, fromDate, toDate, paymentStatus);
        }

        #endregion

        #region Load Document Detail (Invoice/Receipt/Revenue)

        /// <summary>
        /// Load chi tiết chứng từ dựa trên loại và ID
        /// </summary>
        public static DataTable LoadDocumentDetail(string documentType, long documentId)
        {
            switch (documentType)
            {
                case "Invoice":
                    return LoadInvoiceDetailForGrid(documentId);

                case "Receipt":
                    return LoadReceiptDetailForGrid(documentId);

                case "Revenue":
                    return LoadRevenueDetailForGrid(documentId);

                default:
                    return new DataTable();
            }
        }

        private static DataTable LoadInvoiceDetailForGrid(long invoiceId)
        {
            string query = @"
                SELECT 
                    id.STT,
                    ISNULL(ri.ItemName, 'N/A') AS ItemName,
                    ISNULL(ri.Unit, '') AS Unit,
                    id.Quantities,
                    id.UnitPrice,
                    id.Amount,
                    id.Tax AS TaxPercent,
                    id.TaxAmount,
                    id.DiscountPercent,
                    id.DiscountAmount,
                    id.TotalLine,
                    '' AS DebitAccount,
                    '' AS CreditAccount,
                    '' AS TaxAccount,
                    '' AS DiscountAccount
                FROM InvoiceDetail id
                LEFT JOIN RevenueItem ri ON id.ItemId = ri.ItemId
                WHERE id.InvoiceId = @DocumentId
                ORDER BY id.STT";

            return ExecuteQuery(query, new SqlParameter("@DocumentId", invoiceId));
        }

        private static DataTable LoadReceiptDetailForGrid(long receiptId)
        {
            string query = @"
        SELECT
            rd.STT,
            ISNULL(rd.Description, '') AS ItemName,
            '' AS Unit,
            1 AS Quantities,
            rd.Amount AS UnitPrice,
            rd.Amount,
            0 AS TaxPercent,
            0 AS TaxAmount,
            0 AS DiscountPercent,
            0 AS DiscountAmount,
            rd.Amount AS TotalLine,
            ISNULL(rd.Debit_Account, '') AS Debit_Account,      -- ĐÃ SỬA
            ISNULL(rd.Credit_Account, '') AS Credit_Account,    -- ĐÃ SỬA
            '' AS TaxAccount,
            '' AS DiscountAccount
        FROM ReceiptDetail rd
        WHERE rd.ReceiptId = @DocumentId
        ORDER BY rd.STT";
            return ExecuteQuery(query, new SqlParameter("@DocumentId", receiptId));
        }

        private static DataTable LoadRevenueDetailForGrid(long revenueId)
        {
            string query = @"
        SELECT
            rd.STT,
            ISNULL(ri.ItemName, ISNULL(rd.Interpretation, 'N/A')) AS ItemName,
            ISNULL(rd.Unit_measure, '') AS Unit,
            ISNULL(rd.Quantities, 0) AS Quantities,
            ISNULL(rd.UnitPrice, 0) AS UnitPrice,
            ISNULL(rd.Amount, 0) AS Amount,
            ISNULL(rd.TaxPercentage, 0) AS TaxPercent,
            ISNULL(rd.TaxAmount, 0) AS TaxAmount,
            ISNULL(rd.DiscountPercent, 0) AS DiscountPercent,
            ISNULL(rd.DiscountAmount, 0) AS DiscountAmount,
            ISNULL(rd.Amount, 0) + ISNULL(rd.TaxAmount, 0) AS TotalLine,
            ISNULL(rd.Debit_account, '') AS Debit_Account,      -- ĐÃ SỬA
            ISNULL(rd.Credit_account, '') AS Credit_Account,    -- ĐÃ SỬA
            ISNULL(rd.Tax_account, '') AS TaxAccount,
            ISNULL(rd.Discount_account, '') AS DiscountAccount
        FROM Record_detail rd
        LEFT JOIN RevenueItem ri ON rd.ItemId = ri.ItemId
        WHERE rd.Recognition_id = @DocumentId
        ORDER BY rd.STT";
            return ExecuteQuery(query, new SqlParameter("@DocumentId", revenueId));
        }

        #endregion

        #region Helper Methods - FIXED

        /// <summary>
        /// FIXED: Xử lý tham số SQL an toàn và nhất quán
        /// </summary>
        private static DataTable ExecuteQueryWithParams(
            string query,
            DateTime? fromDate,
            DateTime? toDate,
            string paymentStatus)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                // FIXED: Chỉ thêm parameter nếu query có sử dụng
                if (query.Contains("@FromDate") && fromDate.HasValue)
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Value.Date);
                }

                if (query.Contains("@ToDate") && toDate.HasValue)
                {
                    // FIXED: Lấy đến 23:59:59 của ngày được chọn
                    DateTime endDate = toDate.Value.Date.AddDays(1).AddSeconds(-1);
                    cmd.Parameters.AddWithValue("@ToDate", endDate);
                }

                if (query.Contains("@PaymentStatus") && !string.IsNullOrEmpty(paymentStatus))
                {
                    cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
                }

                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return dt;
                }
                catch (SqlException ex)
                {
                    // Log error nếu cần
                    System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
                    throw new Exception($"Lỗi truy vấn dữ liệu: {ex.Message}", ex);
                }
            }
        }

        public static DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
                catch (SqlException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
                    throw new Exception($"Lỗi truy vấn dữ liệu: {ex.Message}", ex);
                }
            }
        }

        public static int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
                    throw new Exception($"Lỗi thực thi lệnh: {ex.Message}", ex);
                }
            }
        }

        public static object ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                try
                {
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
                catch (SqlException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
                    throw new Exception($"Lỗi thực thi scalar: {ex.Message}", ex);
                }
            }
        }

        #endregion
    }
}