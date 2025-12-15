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
                    ISNULL(c.CustomerID, 0) AS CustomerId, 
                    ISNULL(c.FullName, i.Snashot_CustomerName) AS CustomerName, 
                    ISNULL(c.PhoneNumber, i.Snashot_CustomerPhone) AS CustomerPhone, 
                    ISNULL(c.Email, i.Snashot_CustomerEmail) AS CustomerEmail, 
                    ISNULL(c.Address, '') AS CustomerAddress, 
                    ISNULL(c.TaxCode, '') AS CustomerTaxCode,
                    
                    -- VERSION 4: Thông tin Vehicle từ IndividualVehicle và Vehicle
                    ISNULL(iv.VIN, '') AS VIN,
                    ISNULL(iv.LicensePlate, '') AS LicensePlate,
                    ISNULL(v.Model, '') AS VehicleModel,
                    ISNULL(v.VehicleType, '') AS VehicleType,
                    ISNULL(v.Manufacturer, '') AS Manufacturer,
                    ISNULL(v.Version, '') AS VehicleVersion,
                    ISNULL(v.BatteryUsableKWh, 0) AS BatteryUsableKWh,
                    
                    -- Thông tin ChargingSession
                    ISNULL(cs.SessionId, 0) AS SessionId,
                    cs.StartTime,
                    cs.EndTime,
                    ISNULL(cs.EnergyDeliveredKWh, 0) AS EnergyDeliveredKWh,
                    ISNULL(cs.StartSOC, 0) AS StartSOC,
                    ISNULL(cs.EndSOC, 0) AS EndSOC,
                    
                    -- Thông tin Connector & Charger
                    ISNULL(con.ConnectorType, '') AS ConnectorType,
                    ISNULL(ch.Name, '') AS ChargerName,
                    ISNULL(ch.Model, '') AS ChargerModel,
                    
                    -- Thông tin Station
                    ISNULL(s.StationId, 0) AS StationId, 
                    ISNULL(s.Name, 'N/A') AS StationName, 
                    ISNULL(s.Address, '') AS StationAddress, 
                    '' AS StationPhone,  -- Thêm column Phone_Number vào bảng Station nếu cần
                    
                    i.SignatureFile AS SignatureFile
                    
                FROM Invoices i
                    LEFT JOIN Customer c ON i.CustomerId = c.CustomerID
                    LEFT JOIN ChargingSession cs ON i.SessionId = cs.SessionId
                    -- VERSION 4: Join với IndividualVehicle qua VIN
                    LEFT JOIN IndividualVehicle iv ON cs.VIN = iv.VIN
                    -- VERSION 4: Join với Vehicle qua VehicleId
                    LEFT JOIN Vehicle v ON iv.VehicleId = v.VehicleId
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
                    id.TaxAmount
                FROM InvoiceDetail id
                LEFT JOIN RevenueItem ri ON id.ItemId = ri.ItemId
                WHERE id.InvoiceId = @InvoiceId
                ORDER BY id.STT";

            return ExecuteQuery(query, new SqlParameter("@InvoiceId", invoiceId));
        }

        #endregion
        /*
                #region Load Document List (Invoice, Receipt, Revenue) - VERSION 4

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
                            result = LoadReceiptList(fromDate, toDate, paymentStatus, recordingStatus);
                            break;

                        case "Revenue":
                            result = LoadRevenueRecognitionList(fromDate, toDate, paymentStatus,recordingStatus);
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
                      ISNULL(i.Total, 0) AS Total,
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
                  LEFT JOIN Customer c ON i.CustomerId = c.CustomerID
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

                    // SỬA ĐỔI: Thêm recordingStatus vào ExecuteQueryWithParams
                    return ExecuteQueryWithParams(query, fromDate, toDate, paymentStatus, recordingStatus);
                }

                private static DataTable LoadReceiptList(
              DateTime? fromDate,
              DateTime? toDate,
              string paymentStatus,
              string recordingStatus) // THÊM THAM SỐ NÀY
                {
                    string query = @"
                  SELECT 
                      'Receipt' AS DocumentType,
                      r.ReceiptId AS DocumentId,
                      r.ReceiptNumber AS DocumentNumber,
                      r.ReceiptDate AS DocumentDate,
                      ISNULL(r.PayerName, ISNULL(c.FullName, 'N/A')) AS CustomerName,
                      ISNULL(c.Email, '') AS CustomerEmail,
                      r.TotalAmount AS Total,
                      N'Paid' AS PaymentStatus,
                      r.PaymentMethod,
                      r.CustomerId,
                      CASE WHEN EXISTS (
                              SELECT 1 FROM Revenue_recognition rr 
                              WHERE rr.ReceiptId = r.ReceiptId
                          ) THEN N'Posted' 
                          ELSE N'Unposted'
                      END AS RecordingStatus
                  FROM Receipt r
                  LEFT JOIN Customer c ON r.CustomerId = c.CustomerID
                  WHERE 1=1";

                    if (fromDate.HasValue)
                        query += " AND r.ReceiptDate >= @FromDate";

                    if (toDate.HasValue)
                        query += " AND r.ReceiptDate <= @ToDate";

                    // Filter theo Payment Status
                    if (!string.IsNullOrEmpty(paymentStatus) && paymentStatus != "All")
                    {
                        if (paymentStatus.ToUpper() != "PAID")
                        {
                            query += " AND 1 = 0"; // Force không trả về kết quả
                        }
                    }

                    // THÊM LOGIC LỌC THEO RECORDING STATUS
                    if (!string.IsNullOrEmpty(recordingStatus))
                    {
                        if (recordingStatus == "Posted")
                            query += " AND EXISTS (SELECT 1 FROM Revenue_recognition rr WHERE rr.ReceiptId = r.ReceiptId)";
                        else if (recordingStatus == "Unposted")
                            query += " AND NOT EXISTS (SELECT 1 FROM Revenue_recognition rr WHERE rr.ReceiptId = r.ReceiptId)";
                    }

                    query += " ORDER BY r.ReceiptDate DESC";

                    // SỬA ĐỔI: Thêm recordingStatus vào ExecuteQueryWithParams
                    return ExecuteQueryWithParams(query, fromDate, toDate, paymentStatus, recordingStatus);
                }

                private static DataTable LoadRevenueRecognitionList(
              DateTime? fromDate,
              DateTime? toDate,
              string paymentStatus,
              string recordingStatus) // Đã thêm tham số này
                {
                    string query = @"
                  SELECT 
                      'Revenue' AS DocumentType,
                      rr.Id AS DocumentId,
                      CAST(rr.Id AS NVARCHAR(50)) AS DocumentNumber,
                      rr.RecordDate AS DocumentDate,
                      ISNULL(c.FullName, 'N/A') AS CustomerName,
                      ISNULL(c.Email, '') AS CustomerEmail,
                      ISNULL((SELECT SUM(Amount) FROM Record_detail WHERE Recognition_id = rr.Id), 0) AS Total,
                      -- SỬA: Cần có giá trị trả về cho CASE
                      CASE 
                          WHEN rr.Money_received = 1 THEN N'Paid' 
                          ELSE N'Not Specified' 
                      END AS PaymentStatus,
                      rr.Methods_payment AS PaymentMethod,
                      rr.CustomerId,
                      N'Đã ghi sổ' AS RecordingStatus -- LUÔN LÀ 'Đã ghi sổ' vì bản ghi đã tồn tại
                  FROM Revenue_recognition rr
                  LEFT JOIN Customer c ON rr.CustomerId = c.CustomerID
                  WHERE 1=1";

                    if (fromDate.HasValue)
                        query += " AND rr.RecordDate >= @FromDate";

                    if (toDate.HasValue)
                        query += " AND rr.RecordDate <= @ToDate";

                    // Bổ sung lọc theo Payment Status (nếu cần)
                    if (!string.IsNullOrEmpty(paymentStatus) && paymentStatus != "All")
                    {
                        // Giả định PaymentStatus là giá trị đã được chuẩn hóa (Paid, Unpaid,...)
                        // Cần map giá trị này vào cột Money_received của Revenue_recognition.
                        // Tạm thời bỏ qua logic phức tạp này, chỉ lọc theo ngày.
                    }

                    // Bỏ qua lọc recordingStatus vì nó luôn là 'Đã ghi sổ'
                    // Tuy nhiên, nếu người dùng lọc "Unposted" thì hàm này sẽ không trả về gì.
                    if (recordingStatus == "Unposted")
                    {
                        query += " AND 1 = 0"; // Force không trả về kết quả nếu lọc Unposted
                    }

                    query += " ORDER BY rr.RecordDate DESC";

                    // SỬA ĐỔI: Thêm paymentStatus và recordingStatus vào ExecuteQueryWithParams
                    return ExecuteQueryWithParams(query, fromDate, toDate, paymentStatus, recordingStatus);
                }
                // Đặt trong DataHelper.cs
                public static DataTable LoadAllDocuments(
                    DateTime? fromDate,
                    DateTime? toDate,
                    string paymentStatus,
                    string recordingStatus)
                {
                    // BỘ LỌC C# TRUYỀN VÀO:
                    // paymentStatus: 'Paid', 'Unpaid'
                    // recordingStatus: 'Posted', 'Unposted'

                    string query = @"
                SELECT 
                    * FROM (
                    ---------------------------------------------------------------------
                    -- 1. INVOICES (Hóa đơn)
                    ---------------------------------------------------------------------
                    SELECT 
                        'Invoice' AS DocumentType,
                        i.InvoiceId AS DocumentId,
                        CAST(i.InvoiceNumber AS NVARCHAR(50)) AS DocumentNumber,
                        i.CreatedAt AS DocumentDate,
                        ISNULL(i.Snashot_CustomerName, ISNULL(c.FullName, N'N/A')) AS CustomerName,
                        ISNULL(i.Snashot_CustomerEmail, ISNULL(c.Email, N'')) AS CustomerEmail,
                        i.Total AS TotalAmount,
                        i.PaymentMethod,
                        i.CustomerId,

                        -- PaymentStatus: Trạng thái thanh toán của Hóa đơn (Paid/Pending/Cancelled)
                        i.Status AS PaymentStatus, 

                        -- RecordingStatus: Dựa trên tồn tại của bút toán ghi sổ
                        CASE 
                            WHEN EXISTS (SELECT 1 FROM Revenue_recognition rr WHERE rr.InvoiceId = i.InvoiceId) 
                            THEN N'Posted' 
                            ELSE N'Unposted' 
                        END AS RecordingStatus
                    FROM Invoices i
                    LEFT JOIN Customer c ON i.CustomerId = c.CustomerID

                    UNION ALL

                    ---------------------------------------------------------------------
                    -- 2. RECEIPTS (Phiếu Thu) - FIXED: Loại bỏ tham chiếu đến r.Status
                    ---------------------------------------------------------------------
                    SELECT 
                        'Receipt' AS DocumentType,
                        r.ReceiptId AS DocumentId,
                        r.ReceiptNumber AS DocumentNumber,
                        r.ReceiptDate AS DocumentDate,
                        r.TotalAmount AS TotalAmount,
                        ISNULL(r.PayerName, ISNULL(c.FullName, N'N/A')) AS CustomerName,
                        ISNULL(c.Email, N'') AS CustomerEmail,
                        r.PaymentMethod,
                        r.CustomerId,

                        -- PaymentStatus: FIXED - Phiếu thu tồn tại thì AUTO 'Paid'
                        N'Paid' AS PaymentStatus, 

                        -- RecordingStatus: Dựa trên tồn tại của bút toán ghi sổ
                        CASE 
                            WHEN EXISTS (SELECT 1 FROM Revenue_recognition rr WHERE rr.ReceiptId = r.ReceiptId) 
                            THEN N'Posted' 
                            ELSE N'Unposted'
                        END AS RecordingStatus
                    FROM Receipt r
                    LEFT JOIN Customer c ON r.CustomerId = c.CustomerID

                    UNION ALL

                    ---------------------------------------------------------------------
                    -- 3. REVENUE_RECOGNITION (Bút toán ghi sổ)
                    ---------------------------------------------------------------------
                    SELECT 
                        'Revenue' AS DocumentType,
                        rr.Id AS DocumentId,
                        CAST(rr.Id AS NVARCHAR(50)) AS DocumentNumber,
                        rr.RecordDate AS DocumentDate,
                        ISNULL(c.FullName, N'N/A') AS CustomerName,
                        ISNULL(c.Email, N'') AS CustomerEmail,
                        ISNULL((SELECT SUM(Amount) FROM Record_detail WHERE Recognition_id = rr.Id), 0) AS TotalAmount,
                        rr.Methods_payment AS PaymentMethod, -- Lấy PTTT từ bút toán
                        rr.CustomerId,

                        -- PaymentStatus: Mặc định Đã ghi sổ/Đã xử lý
                        N'Posted' AS PaymentStatus, 

                        -- RecordingStatus: Mặc định Đã ghi sổ
                        N'Posted' AS RecordingStatus
                    FROM Revenue_recognition rr
                    LEFT JOIN Customer c ON rr.CustomerId = c.CustomerID

                ) AS AllDocuments
                WHERE 1=1";

                    // ------------------------------------------------------------------
                    // KHU VỰC LỌC DỮ LIỆU C#
                    // ------------------------------------------------------------------

                    if (fromDate.HasValue)
                        query += " AND DocumentDate >= @FromDate";

                    if (toDate.HasValue)
                        query += " AND DocumentDate <= @ToDate";

                    // FIXED: Lọc theo PaymentStatus (Dùng giá trị tiếng Anh 'Paid', 'Unpaid', 'Cancelled')
                    if (!string.IsNullOrEmpty(paymentStatus) && paymentStatus != "All")
                    {
                        // Ta sử dụng tham số @PaymentStatus (là 'Paid' hoặc 'Unpaid')
                        query += " AND PaymentStatus = @PaymentStatus";
                    }

                    // Lọc theo RecordingStatus (Dùng giá trị tiếng Anh 'Posted', 'Unposted')
                    if (!string.IsNullOrEmpty(recordingStatus) && recordingStatus != "All")
                    {
                        // Dùng tham số @RecordingStatus
                        query += " AND RecordingStatus = @RecordingStatus";
                    }

                    query += " ORDER BY DocumentDate DESC";

                    // Chú ý: Hàm ExecuteQueryWithParams phải được sửa để chấp nhận recordingStatus
                    return ExecuteQueryWithParams(query, fromDate, toDate, paymentStatus, recordingStatus);
                }
                */
        // Thay thế hàm LoadReceiptList trong DataHelper
        // Thay thế trong DataHelper.cs

        #region Load Document List (Invoice, Receipt, Revenue) - VERSION 4
        /// <summary>
        /// Load tất cả documents (Invoice + Receipt + Revenue) - OPTIMIZED UNION VERSION
        /// </summary>
        public static DataTable LoadAllDocuments(
            DateTime? fromDate,
            DateTime? toDate,
            string paymentStatus,
            string recordingStatus)
        {
            // BỘ LỌC C# TRUYỀN VÀO:
            // paymentStatus: 'Paid', 'Unpaid', 'Cancelled', 'All'
            // recordingStatus: 'Posted', 'Unposted', 'All'

            string query = @"
                        SELECT 
                            * FROM (
                            ---------------------------------------------------------------------
                            -- 1. INVOICES (Hóa đơn)
                            ---------------------------------------------------------------------
                            SELECT 
                                'Invoice' AS DocumentType,
                                i.InvoiceId AS DocumentId,
                                CAST(i.InvoiceNumber AS NVARCHAR(50)) AS DocumentNumber,
                                i.CreatedAt AS DocumentDate,
                                ISNULL(i.Snashot_CustomerName, ISNULL(c.FullName, N'N/A')) AS CustomerName,
                                ISNULL(i.Snashot_CustomerEmail, ISNULL(c.Email, N'')) AS CustomerEmail,
                                i.Total AS TotalAmount,
                                i.PaymentMethod,
                                i.CustomerId,

                                -- PaymentStatus: Lấy trực tiếp từ cột Status của Invoice
                                i.Status AS PaymentStatus, 

                                -- RecordingStatus: Kiểm tra đã ghi sổ chưa
                                CASE 
                                    WHEN EXISTS (SELECT 1 FROM Revenue_recognition rr WHERE rr.InvoiceId = i.InvoiceId) 
                                    THEN N'Posted' 
                                    ELSE N'Unposted' 
                                END AS RecordingStatus
                            FROM Invoices i
                            LEFT JOIN Customer c ON i.CustomerId = c.CustomerID

                            UNION ALL

                            ---------------------------------------------------------------------
                            -- 2. RECEIPTS (Phiếu Thu) - FIXED: Receipt luôn là 'Paid'
                            ---------------------------------------------------------------------
                            SELECT 
                                'Receipt' AS DocumentType,
                                r.ReceiptId AS DocumentId,
                                r.ReceiptNumber AS DocumentNumber,
                                r.ReceiptDate AS DocumentDate,
                                ISNULL(r.PayerName, ISNULL(c.FullName, N'N/A')) AS CustomerName,
                                ISNULL(c.Email, N'') AS CustomerEmail,
                                r.TotalAmount AS TotalAmount,
                                r.PaymentMethod,
                                r.CustomerId,

                                -- CRITICAL FIX: Receipt = Phiếu thu = Đã thu tiền → Luôn 'Paid'
                                N'Paid' AS PaymentStatus, 

                                -- RecordingStatus: Kiểm tra đã ghi sổ chưa
                                CASE 
                                    WHEN EXISTS (SELECT 1 FROM Revenue_recognition rr WHERE rr.ReceiptId = r.ReceiptId) 
                                    THEN N'Posted' 
                                    ELSE N'Unposted'
                                END AS RecordingStatus
                            FROM Receipt r
                            LEFT JOIN Customer c ON r.CustomerId = c.CustomerID

                            UNION ALL

                            ---------------------------------------------------------------------
                            -- 3. REVENUE_RECOGNITION (Bút toán ghi sổ)
                            ---------------------------------------------------------------------
                            SELECT 
                                'Revenue' AS DocumentType,
                                rr.Id AS DocumentId,
                                CAST(rr.Id AS NVARCHAR(50)) AS DocumentNumber,
                                rr.RecordDate AS DocumentDate,
                                ISNULL(c.FullName, N'N/A') AS CustomerName,
                                ISNULL(c.Email, N'') AS CustomerEmail,
                                ISNULL((SELECT SUM(Amount) FROM Record_detail WHERE Recognition_id = rr.Id), 0) AS TotalAmount,
                                rr.Methods_payment AS PaymentMethod,
                                rr.CustomerId,

                                -- PaymentStatus: Revenue đã ghi sổ = Đã xử lý
                                N'Posted' AS PaymentStatus, 

                                -- RecordingStatus: Luôn là Posted (vì đã là bút toán ghi sổ rồi)
                                N'Posted' AS RecordingStatus
                            FROM Revenue_recognition rr
                            LEFT JOIN Customer c ON rr.CustomerId = c.CustomerID

                        ) AS AllDocuments
                        WHERE 1=1";

            // ------------------------------------------------------------------
            // KHU VỰC LỌC DỮ LIỆU
            // ------------------------------------------------------------------

            // Date range filter
            if (fromDate.HasValue)
                query += " AND DocumentDate >= @FromDate";

            if (toDate.HasValue)
                query += " AND DocumentDate <= @ToDate";

            // PaymentStatus filter
            if (!string.IsNullOrEmpty(paymentStatus) && paymentStatus != "All")
            {
                query += " AND PaymentStatus = @PaymentStatus";
            }

            // RecordingStatus filter
            if (!string.IsNullOrEmpty(recordingStatus) && recordingStatus != "All")
            {
                query += " AND RecordingStatus = @RecordingStatus";
            }

            query += " ORDER BY DocumentDate DESC";

            return ExecuteQueryWithParams(query, fromDate, toDate, paymentStatus, recordingStatus);
        }

        /// <summary>
        /// Load danh sách theo loại document cụ thể
        /// </summary>
        public static DataTable LoadDocumentList(
            string documentType,
            DateTime? fromDate,
            DateTime? toDate,
            string paymentStatus,
            string recordingStatus)
        {
            try
            {
                switch (documentType)
                {
                    case "Invoice":
                        return LoadInvoiceList(fromDate, toDate, paymentStatus, recordingStatus);

                    case "Receipt":
                        return LoadReceiptList(fromDate, toDate, paymentStatus, recordingStatus);

                    case "Revenue":
                        return LoadRevenueList(fromDate, toDate, recordingStatus);

                    case "All":
                    default:
                        return LoadAllDocuments(fromDate, toDate, paymentStatus, recordingStatus);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi load document list: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Load Invoice list
        /// </summary>
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
                ) THEN N'Posted'
                ELSE N'Unposted'
            END AS RecordingStatus
        FROM Invoices i
        LEFT JOIN Customer c ON i.CustomerId = c.CustomerID
        WHERE 1=1";

            if (fromDate.HasValue)
                query += " AND i.CreatedAt >= @FromDate";

            if (toDate.HasValue)
                query += " AND i.CreatedAt <= @ToDate";

            if (!string.IsNullOrEmpty(paymentStatus) && paymentStatus != "All")
                query += " AND i.Status = @PaymentStatus";

            if (!string.IsNullOrEmpty(recordingStatus) && recordingStatus != "All")
            {
                if (recordingStatus == "Posted")
                    query += " AND EXISTS (SELECT 1 FROM Revenue_recognition rr WHERE rr.InvoiceId = i.InvoiceId)";
                else
                    query += " AND NOT EXISTS (SELECT 1 FROM Revenue_recognition rr WHERE rr.InvoiceId = i.InvoiceId)";
            }

            query += " ORDER BY i.CreatedAt DESC";

            return ExecuteQueryWithParams(query, fromDate, toDate, paymentStatus, recordingStatus);
        }

        /// <summary>
        /// Load Receipt list
        /// </summary>
        private static DataTable LoadReceiptList(
            DateTime? fromDate,
            DateTime? toDate,
            string paymentStatus,
            string recordingStatus)
        {
            // CRITICAL: Receipt chỉ hiển thị khi PaymentStatus = "Paid" hoặc "All"
            if (!string.IsNullOrEmpty(paymentStatus) &&
                paymentStatus != "All" &&
                paymentStatus != "Paid")
            {
                return new DataTable(); // Return empty nếu filter Unpaid/Cancelled
            }

            string query = @"
        SELECT 
            'Receipt' AS DocumentType,
            r.ReceiptId AS DocumentId,
            r.ReceiptNumber AS DocumentNumber,
            r.ReceiptDate AS DocumentDate,
            ISNULL(r.PayerName, ISNULL(c.FullName, 'N/A')) AS CustomerName,
            ISNULL(c.Email, '') AS CustomerEmail,
            r.TotalAmount AS TotalAmount,
            N'Paid' AS PaymentStatus,
            r.PaymentMethod,
            r.CustomerId,
            CASE 
                WHEN EXISTS (
                    SELECT 1 FROM Revenue_recognition rr 
                    WHERE rr.ReceiptId = r.ReceiptId
                ) THEN N'Posted'
                ELSE N'Unposted'
            END AS RecordingStatus
        FROM Receipt r
        LEFT JOIN Customer c ON r.CustomerId = c.CustomerID
        WHERE 1=1";

            if (fromDate.HasValue)
                query += " AND r.ReceiptDate >= @FromDate";

            if (toDate.HasValue)
                query += " AND r.ReceiptDate <= @ToDate";

            if (!string.IsNullOrEmpty(recordingStatus) && recordingStatus != "All")
            {
                if (recordingStatus == "Posted")
                    query += " AND EXISTS (SELECT 1 FROM Revenue_recognition rr WHERE rr.ReceiptId = r.ReceiptId)";
                else
                    query += " AND NOT EXISTS (SELECT 1 FROM Revenue_recognition rr WHERE rr.ReceiptId = r.ReceiptId)";
            }

            query += " ORDER BY r.ReceiptDate DESC";

            return ExecuteQueryWithParams(query, fromDate, toDate, null, recordingStatus);
        }

        /// <summary>
        /// Load Revenue list
        /// </summary>
        private static DataTable LoadRevenueList(
            DateTime? fromDate,
            DateTime? toDate,
            string recordingStatus)
        {
            string query = @"
        SELECT 
            'Revenue' AS DocumentType,
            rr.Id AS DocumentId,
            CAST(rr.Id AS NVARCHAR(50)) AS DocumentNumber,
            rr.RecordDate AS DocumentDate,
            ISNULL(c.FullName, N'N/A') AS CustomerName,
            ISNULL(c.Email, N'') AS CustomerEmail,
            ISNULL((SELECT SUM(Amount) FROM Record_detail WHERE Recognition_id = rr.Id), 0) AS TotalAmount,
            N'Posted' AS PaymentStatus,
            rr.Methods_payment AS PaymentMethod,
            rr.CustomerId,
            N'Posted' AS RecordingStatus
        FROM Revenue_recognition rr
        LEFT JOIN Customer c ON rr.CustomerId = c.CustomerID
        WHERE 1=1";

            if (fromDate.HasValue)
                query += " AND rr.RecordDate >= @FromDate";

            if (toDate.HasValue)
                query += " AND rr.RecordDate <= @ToDate";

            // Revenue luôn là Posted, nên chỉ hiển thị khi filter = "Posted" hoặc "All"
            if (!string.IsNullOrEmpty(recordingStatus) && recordingStatus != "All" && recordingStatus != "Posted")
            {
                return new DataTable(); // Return empty nếu filter Unposted
            }

            query += " ORDER BY rr.RecordDate DESC";

            return ExecuteQueryWithParams(query, fromDate, toDate, null, recordingStatus);
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
                    (id.Amount - ISNULL(id.DiscountAmount, 0) + ISNULL(id.TaxAmount, 0)) AS TotalLine,
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
                    ISNULL(rd.Debit_Account, '') AS DebitAccount,
                    ISNULL(rd.Credit_Account, '') AS CreditAccount,
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
                    (ISNULL(rd.Amount, 0) + ISNULL(rd.TaxAmount, 0) - ISNULL(rd.DiscountAmount, 0)) AS TotalLine,
                    ISNULL(rd.Debit_account, '') AS DebitAccount,
                    ISNULL(rd.Credit_account, '') AS CreditAccount,
                    ISNULL(rd.Tax_account, '') AS TaxAccount,
                    ISNULL(rd.Discount_account, '') AS DiscountAccount
                FROM Record_detail rd
                LEFT JOIN RevenueItem ri ON rd.ItemId = ri.ItemId
                WHERE rd.Recognition_id = @DocumentId
                ORDER BY rd.STT";

            return ExecuteQuery(query, new SqlParameter("@DocumentId", revenueId));
        }

        #endregion


        /*
        /// <summary>
        /// Lấy ReceiptId theo ReceiptNumber (có prefix PT hoặc không)
        /// </summary>
        /// <param name="receiptNumber">Số phiếu thu, ví dụ: PT000123 hoặc pt000123</param>
        /// <returns>ReceiptId nếu tìm thấy, ngược lại null</returns>
        public static long? GetReceiptIdByNumber(string receiptNumber)
        {
            if (string.IsNullOrWhiteSpace(receiptNumber))
                return null;

            try
            {
                // Chuẩn hóa: loại bỏ khoảng trắng và chuyển về uppercase để so sánh chính xác
                string normalizedNumber = receiptNumber.Trim();

                string query = @"
            SELECT ReceiptId 
            FROM Receipt 
            WHERE ReceiptNumber = @ReceiptNumber";

                object result = ExecuteScalar(query, new SqlParameter("@ReceiptNumber", normalizedNumber));

                return result != null ? Convert.ToInt64(result) : (long?)null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy ReceiptId theo số phiếu thu '{receiptNumber}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Load dữ liệu chính của Phiếu thu theo ReceiptId
        /// Bao gồm thông tin khách hàng, nhân viên, hóa đơn liên kết, trạm sạc...
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
                r.CreatedBy,
                r.InvoiceId,
                r.TransactionId,

                -- Thông tin Hóa đơn (nếu có)
                i.InvoiceNumber,
                

                -- Thông tin Khách hàng
                c.FullName,
                c.PhoneNumber AS CustomerPhone,
                c.Email AS CustomerEmail,
                c.Address AS CustomerAddress,
             

                -- Thông tin Nhân viên
                e.FullName AS EmployeeName,

                (SELECT TOP 1 StationName FROM StationConfig) AS StationName,
                (SELECT TOP 1 StationAddress FROM StationConfig) AS StationAddress,
                (SELECT TOP 1 StationPhone FROM StationConfig) AS StationPhone,
                (SELECT TOP 1 TaxCode FROM StationConfig) AS StationTaxCode,

                -- Thông tin Giao dịch ngân hàng (nếu có)
                t.Trans_Date,
                t.From_Bank_Name,
                t.From_Acc_Name,
                t.To_Bank_Name,
                t.To_Acc_Name,
                t.Transaction_Code,
                t.Content AS TransactionContent

            FROM Receipt r
            LEFT JOIN Invoices i ON r.InvoiceId = i.InvoiceId
            LEFT JOIN Customer c ON r.CustomerId = c.CustomerID
            LEFT JOIN Employee e ON r.EmployeeId = e.EmployeeId
            LEFT JOIN Transactions t ON r.TransactionId = t.Id
            JOIN ChargingSession cs ON i.SessionId = cs.SessionId
            JOIN Charger ch ON cs.ConnectorId = ch.ChargerId
            JOIN Station s ON ch.StationId = s.StationId
            WHERE r.ReceiptId = @ReceiptId";

                return ExecuteQuery(query, new SqlParameter("@ReceiptId", receiptId));
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi load dữ liệu phiếu thu (ID: {receiptId}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Load chi tiết các bút toán của Phiếu thu
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

                return ExecuteQuery(query, new SqlParameter("@ReceiptId", receiptId));
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi load chi tiết phiếu thu (ReceiptId: {receiptId}): {ex.Message}", ex);
            }
        }
        */
        #region Helper Methods - FIXED
       


        /// <summary>
        /// FIXED: Xử lý tham số SQL an toàn và nhất quán
        /// </summary>
        private static DataTable ExecuteQueryWithParams(
            string query,
            DateTime? fromDate,
            DateTime? toDate,
            string paymentStatus,
            string recordingStatus)
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
                // 4. THAM SỐ ĐÃ SỬA: @RecordingStatus
                if (query.Contains("@RecordingStatus") && !string.IsNullOrEmpty(recordingStatus))
                {
                    cmd.Parameters.AddWithValue("@RecordingStatus", recordingStatus);
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