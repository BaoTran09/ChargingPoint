using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ChargingApp.Helper;
using ChargingApp.Helpers;
using ChargingApp.Report;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace ChargingApp.Accounting
{
    public partial class frmHachToan : DevExpress.XtraEditors.XtraForm
    {
        private string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=ChargingPoint;Integrated Security=True;Connect Timeout=30";
        private long? currentRecognitionId = null;
        private long? selectedInvoiceId = null;
        private long? selectedReceiptId = null;
        private long? selectedCustomerId = null;
      
       // long? CurrentEmployeeId = GlobalSession.CurrentEmployeeId;

        public frmHachToan()
        {
            InitializeComponent();
        }

        private void frmHachToan_Load(object sender, EventArgs e)
        {
           // this.AutoScroll = true;
            this.AutoSize = false;
          //  this.AutoSizeMode = AutoSizeMode.GrowOnly;


            this.ControlBox = false;
            InitializeForm();
            LoadPaymentMethods();
            SetDefaultDates();
            
        }

        #region Initialization Methods

        private void InitializeForm()
        {
            lbRecognitionId.Text = "Mã mới";
            hlInvocieID.Text = "Chưa có";
            hlReceipt_id.Text = "Chưa có";
            cbMethods_payment.Enabled = true ;
            lbMethods_payment.Enabled = true;
            SetupGridView();
            gcRecognitionDetail.DataSource = CreateRecordDetailTable();
            // Wire up events
            /*btnSave.Click += btnSave_Click;
             btnSave_Print.Click += btnSave_Print_Click;
             btnThamChieuHD.Click += btnThamChieuHD_Click;
             btnThamChieuPT.Click += btnThamChieuPT_Click;
             hlInvocieID.Click += HlInvocieID_Click;
             CkMoney_Received.CheckedChanged += CkMoney_Received_CheckedChanged;
             cbMethods_payment.SelectedIndexChanged += cbMethods_payment_SelectedIndexChanged;
             ckNewcustomer.CheckedChanged += ckNewcustomer_CheckedChanged;
            */
            // Initially disable customer fields
            SetCustomerFieldsReadOnly(false);
            ckNewcustomer.Checked = false;
            btnClear.Click += (s, e) =>
             {
                 if (XtraMessageBox.Show("Xóa tất cả dữ liệu?", "Xác nhận",
                     MessageBoxButtons.YesNo) == DialogResult.Yes)
                     {
                         ClearForm();
                     }
             };
        }

        private void SetCustomerFieldsReadOnly(bool readOnly)
        {
            txtCustomerName.ReadOnly = readOnly;
            txtCustomerNumberPhone.ReadOnly = readOnly;
            txtCustomerAddress.ReadOnly = readOnly;
            txtCustomerTaxCode.ReadOnly = readOnly;
            txtCustomerId.ReadOnly = readOnly;
            txtEmployeeName.ReadOnly = readOnly;// Always read-only
        }

        private void SetupGridView()
        {
            try
            {
                // Setup GridView options
           

                gvRecognition_Detail.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
                gvRecognition_Detail.OptionsBehavior.Editable = true;
                gvRecognition_Detail.OptionsView.ShowFooter = true;
                gvRecognition_Detail.OptionsView.ColumnAutoWidth = false;
                gvRecognition_Detail.OptionsView.AllowCellMerge = false;
                gvRecognition_Detail.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;

                // Bind columns
                glSTT.FieldName = "STT";
                glServiceName.FieldName = "ServiceName";
                glDebitAccount.FieldName = "Debit_account";
                glCreditAccount.FieldName = "Credit_account";
                glUnitMeasure.FieldName = "Unit_measure";
                glQuantities.FieldName = "Quantities";
                glUnitPrice.FieldName = "UnitPrice";
                glAmount.FieldName = "Amount";
                glTaxPercentag.FieldName = "TaxPercentage";
                glTaxAmount.FieldName = "TaxAmount";
                glTaxAccount.FieldName = "Tax_account";
                glDiscountAccount.FieldName = "Discount_account";
                glDiscountPercent.FieldName = "DiscountPercent";
                glDiscountAmount.FieldName = "DiscountAmount";
                AddAccountRepositoryItems();
                AddRevenueItemRepositoryItems();
                gvRecognition_Detail.CellValueChanged += GvRecognition_Detail_CellValueChanged;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi setup GridView: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void AddRevenueItemRepositoryItems()
        {
            var riRevenueItem = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            riRevenueItem.DataSource = GetRevenueItem();
            riRevenueItem.DisplayMember = "ItemName";
            riRevenueItem.ValueMember = "ItemId";
            glServiceName.ColumnEdit = riRevenueItem;
        }
            private void AddAccountRepositoryItems()
        {
            var riDebitAccount = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            riDebitAccount.DataSource = GetAccounts();
            riDebitAccount.DisplayMember = "Acc_Name";
            riDebitAccount.ValueMember = "Acc_Code2";
            glDebitAccount.ColumnEdit = riDebitAccount;

            var riCreditAccount = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            riCreditAccount.DataSource = GetAccounts();
            riCreditAccount.DisplayMember = "Acc_Name";
            riCreditAccount.ValueMember = "Acc_Code2";
            glCreditAccount.ColumnEdit = riCreditAccount;

            var riTaxAccount = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            riTaxAccount.DataSource = GetAccounts();
            riTaxAccount.DisplayMember = "Acc_Name";
            riTaxAccount.ValueMember = "Acc_Code2";
            glTaxAccount.ColumnEdit = riTaxAccount;


            var riDiscountAccount = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            riDiscountAccount.DataSource = GetAccounts();
            riDiscountAccount.DisplayMember = "Acc_Name";
            riDiscountAccount.ValueMember = "Acc_Code2";
            glDiscountAccount.ColumnEdit = riDiscountAccount;
        }

        private DataTable GetAccounts()
        {
            string query = "SELECT Acc_Code2, Acc_Name FROM COA ORDER BY Acc_Code2";
            return DatabaseHelper.ExecuteQuery(connectionString, query);
        }
        private DataTable GetRevenueItem()
                {
                    string query = "SELECT ItemId, ItemName FROM RevenueItem ORDER BY ItemId";
                    return DatabaseHelper.ExecuteQuery(connectionString, query);
                }
        private void LoadPaymentMethods()
        {
            cbMethods_payment.Properties.Items.Clear();
            cbMethods_payment.Properties.Items.Add("Tiền mặt");
            cbMethods_payment.Properties.Items.Add("Tiền ngân hàng");
        }
      
        private void SetDefaultDates()
        {
            DERecord_date.EditValue = DateTime.Now;
            DEDocumentDate.EditValue = DateTime.Now;
        }

        #endregion

        #region Reference Invoice & Receipt
        /// <summary>
        /// Khóa các trường Khách hàng và Phương thức thanh toán sau khi tham chiếu thành công.
        /// </summary>
        private void LockControlsOnReference(bool isLocked)
        {
            // Khóa/Mở khóa trường Khách hàng
            SetCustomerFieldsReadOnly(isLocked);

            // Khóa/Mở khóa Checkbox tạo Khách hàng mới
            ckNewcustomer.Enabled = !isLocked;

            // Khóa/Mở khóa Trạng thái Thanh toán (Đã nhận tiền)
            CkMoney_Received.Enabled = !isLocked;

            // Khóa/Mở khóa Phương thức Thanh toán
            cbMethods_payment.Enabled = !isLocked;
            lbMethods_payment.Enabled = !isLocked;

            // Khóa luôn nút tham chiếu còn lại (để tránh tham chiếu HD và PT cùng lúc)
            if (selectedInvoiceId.HasValue || selectedReceiptId.HasValue)
            {
                btnThamChieuHD.Enabled = !selectedInvoiceId.HasValue;
                btnThamChieuPT.Enabled = !selectedReceiptId.HasValue;
            }
            else
            {
                btnThamChieuHD.Enabled = true;
                btnThamChieuPT.Enabled = true;
            }
        }
        private void btnThamChieuHD_Click(object sender, EventArgs e)
        {
            // FIX: Using block instead of using declaration (C# 7.3 compatible)
            using (var searchForm = new frmSearchDocument("Invoice"))
            {
                if (searchForm.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(searchForm.SelectedDocumentId))
                {
                    selectedInvoiceId = long.Parse(searchForm.SelectedDocumentId);
                    hlInvocieID.Text = "HD #" + selectedInvoiceId;
                    LoadInvoiceAndCustomerData(selectedInvoiceId.Value);
                    GenerateRecordDetailFromInvoice();
                    LockControlsOnReference(true);
                }
            }
        }

        #region FIX 4: Update btnThamChieuPT_Click

        /// <summary>
        /// Sửa lại để gọi GenerateRecordDetailFromReceipt()
        /// </summary>
        private void btnThamChieuPT_Click(object sender, EventArgs e)
        {
            using (var searchForm = new frmSearchDocument("Receipt"))
            {
                if (searchForm.ShowDialog() == DialogResult.OK)
                {
                    selectedReceiptId = long.Parse(searchForm.SelectedDocumentId);
                    hlReceipt_id.Text = "PT #" + selectedReceiptId;

                    // Load thông tin khách hàng và payment
                    LoadReceiptData(selectedReceiptId.Value);

                    // ✅ THÊM: Tạo chi tiết hạch toán
                    GenerateRecordDetailFromReceipt();

                    // Lock controls
                    LockControlsOnReference(true);
                }
            }
        }

        #endregion

        #region Reference Invoice & Receipt - VERSION 4 UPDATED

        private void LoadInvoiceAndCustomerData(long invoiceId)
        {
            // VERSION 4: Updated query để join với IndividualVehicle và Vehicle
            string query = @"
        SELECT 
            i.InvoiceId, 
            i.Snashot_CustomerName, 
            i.Snashot_CustomerPhone, 
            i.Snashot_CustomerEmail,
            i.Status, 
            i.PaymentMethod, 
            i.CustomerId,
            c.FullName, 
            c.PhoneNumber, 
            c.Email, 
            c.Address, 
            c.TaxCode,
            -- VERSION 4: Thêm thông tin Vehicle
            cs.SessionId,
            cs.VIN,
            iv.LicensePlate,
            v.Model AS VehicleModel,
            v.VehicleType,
            v.Manufacturer
        FROM Invoices i
            LEFT JOIN Customer c ON i.CustomerId = c.CustomerID
            LEFT JOIN ChargingSession cs ON i.SessionId = cs.SessionId
            LEFT JOIN IndividualVehicle iv ON cs.VIN = iv.VIN
            LEFT JOIN Vehicle v ON iv.VehicleId = v.VehicleId
        WHERE i.InvoiceId = @InvoiceId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@InvoiceId", invoiceId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return;

                    // Prioritize Customer table, fallback to Snapshot
                    string custName = reader["FullName"]?.ToString();
                    if (string.IsNullOrEmpty(custName))
                        custName = reader["Snashot_CustomerName"]?.ToString() ?? "";

                    string custPhone = reader["PhoneNumber"]?.ToString();
                    if (string.IsNullOrEmpty(custPhone))
                        custPhone = reader["Snashot_CustomerPhone"]?.ToString() ?? "";

                    string custEmail = reader["Email"]?.ToString();
                    if (string.IsNullOrEmpty(custEmail))
                        custEmail = reader["Snashot_CustomerEmail"]?.ToString() ?? "";

                    txtCustomerName.Text = custName;
                    txtCustomerNumberPhone.Text = custPhone;
                    txtCustomerAddress.Text = reader["Address"]?.ToString() ?? "";
                    txtCustomerTaxCode.Text = reader["TaxCode"]?.ToString() ?? "";

                    // CustomerId
                    if (reader["CustomerId"] != DBNull.Value)
                    {
                        selectedCustomerId = Convert.ToInt64(reader["CustomerId"]);
                        txtCustomerId.Text = selectedCustomerId.ToString();
                        //ckNewcustomer.Enabled = false;
                        //ckNewcustomer.Checked = false;
                    }
                    else
                    {
                        selectedCustomerId = null;
                        txtCustomerId.Text = "";
                        //ckNewcustomer.Enabled = true;
                    }

                    // Payment status
                    bool isPaid = (reader["Status"]?.ToString() ?? "").Equals("Paid", StringComparison.OrdinalIgnoreCase);
                    CkMoney_Received.Checked = isPaid;

                    if (isPaid)
                    {
                        string method = reader["PaymentMethod"]?.ToString() ?? "";
                        cbMethods_payment.Text = method.Contains("Bank") || method.Contains("Chuyển khoản")
                            ? "Tiền ngân hàng" : "Tiền mặt";
                    }
                    txtEmployeeName.Enabled = true;
                    // VERSION 4: Hiển thị thông tin Vehicle (optional - nếu có textbox hiển thị)
                    // string vehicleInfo = "";
                    // if (reader["LicensePlate"] != DBNull.Value)
                    //     vehicleInfo = $"{reader["LicensePlate"]} - {reader["VehicleModel"]}";
                    // txtVehicleInfo.Text = vehicleInfo;
                }
            }
        }
        /// <summary>
        /// Lấy EmployeeId dựa trên EmployeeName. Nếu chưa tồn tại, tạo mới Employee và trả về ID mới.
        /// </summary>
        private long GetOrCreateEmployeeId(string employeeName)
        {
            // Cắt bỏ khoảng trắng hai đầu
            employeeName = employeeName.Trim();
            if (string.IsNullOrWhiteSpace(employeeName))
            {
                throw new ArgumentException("Tên nhân viên không được để trống.");
            }

            // 1. KIỂM TRA: Tìm EmployeeId hiện có
            string selectQuery = "SELECT EmployeeId FROM Employee WHERE FullName = @EmployeeName";

            // SỬA: Sử dụng DatabaseHelper
            object employeeIdResult = DatabaseHelper.ExecuteScalar(
                connectionString,
                selectQuery,
                new SqlParameter("@EmployeeName", employeeName)
            );

            // Nếu tìm thấy ID, trả về ngay lập tức
            if (employeeIdResult != null && employeeIdResult != DBNull.Value)
            {
                return Convert.ToInt64(employeeIdResult);
            }

            // 2. TẠO MỚI: Nếu chưa tồn tại, thêm Employee mới (Dùng OUTPUT để lấy ID)
            string insertQuery = @"
        INSERT INTO Employee (FullName, CreatedAt, Status)
        OUTPUT INSERTED.EmployeeId
        VALUES (@EmployeeName, GETDATE(), 'Active')";

            // SỬA: Sử dụng DatabaseHelper
            object newIdResult = DatabaseHelper.ExecuteScalar(
                connectionString,
                insertQuery,
                new SqlParameter("@EmployeeName", employeeName)
            );

            if (newIdResult != null && newIdResult != DBNull.Value)
            {
                return Convert.ToInt64(newIdResult);
            }
            else
            {
                throw new Exception($"Không thể tạo Employee mới với tên: {employeeName}. Lỗi ExecuteScalar trả về NULL.");
            }
        }

        /// <summary>
        /// Tạo cấu trúc DataTable chuẩn cho chi tiết Ghi nhận Doanh thu (Record_detail) 
        /// trên GridView.
        /// </summary>
        private DataTable CreateRecordDetailTable()
        {
            DataTable dt = new DataTable();

            // 1. CÁC CỘT CHÍNH (Được lưu vào Record_detail)
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("ItemId", typeof(long));        // ID của Hàng hóa/Dịch vụ (ValueMember)
            dt.Columns.Add("ServiceName", typeof(string)); // Tên hiển thị của ItemId
            dt.Columns.Add("Interpretation", typeof(string));

            // 2. CÁC CỘT TÀI KHOẢN (Được lưu vào Record_detail)
            dt.Columns.Add("Debit_account", typeof(string));
            dt.Columns.Add("Credit_account", typeof(string));
            dt.Columns.Add("Tax_account", typeof(string));
            dt.Columns.Add("Discount_account", typeof(string));

            // 3. CÁC CỘT SỐ LƯỢNG VÀ GIÁ (Được lưu vào Record_detail)
            dt.Columns.Add("Unit_measure", typeof(string));
            dt.Columns.Add("Quantities", typeof(decimal));
            dt.Columns.Add("UnitPrice", typeof(decimal));
            dt.Columns.Add("Amount", typeof(decimal));       // Thành tiền (Gross Amount = Qty * Price)

            // 4. CÁC CỘT CHIẾT KHẤU (Được lưu vào Record_detail)
            dt.Columns.Add("DiscountPercent", typeof(decimal));
            dt.Columns.Add("DiscountAmount", typeof(decimal));

            // 5. CÁC CỘT THUẾ (Được lưu vào Record_detail)
            dt.Columns.Add("TaxPercentage", typeof(decimal));
            dt.Columns.Add("TaxAmount", typeof(decimal));

            // 6. CỘT TÍNH TOÁN/HIỂN THỊ (KHÔNG lưu vào Record_detail, chỉ dùng ở tầng UI)
            // Cột này giúp tính toán tổng tiền trên từng dòng GridView
            dt.Columns.Add("Total", typeof(decimal));

            return dt;
        }


        private void GenerateRecordDetailFromInvoice()
        {
            if (!selectedInvoiceId.HasValue)
            {
                XtraMessageBox.Show("Chưa chọn hóa đơn!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = @"
        SELECT 
            id.InvoiceId, 
            id.STT, 
            id.ItemId, 
            id.Quantities, 
            id.Unit, 
            id.UnitPrice,
            id.Amount, 
            id.DiscountPercent, 
            id.DiscountAmount, 
            id.AmountAfterDiscount,
            id.Tax, 
            id.TaxAmount,
            ri.ItemName, 
            ri.ItemType
        FROM InvoiceDetail id
        LEFT JOIN RevenueItem ri ON id.ItemId = ri.ItemId
        WHERE id.InvoiceId = @InvoiceId
        ORDER BY id.STT";

                // Tạo DataTable
                DataTable dt = CreateRecordDetailTable();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@InvoiceId", selectedInvoiceId.Value);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        string debitAccount = GetDebitAccount();

                        while (reader.Read())
                        {
                            DataRow row = dt.NewRow();

                            // Helper đọc an toàn
                            int ReadInt(string col)
                                => reader[col] == DBNull.Value ? 0 : Convert.ToInt32(reader[col]);

                            long ReadLong(string col)
                                => reader[col] == DBNull.Value ? 0L : Convert.ToInt64(reader[col]);

                            decimal ReadDecimal(string col)
                                => reader[col] == DBNull.Value ? 0m : Convert.ToDecimal(reader[col]);

                            string ReadString(string col)
                                => reader[col] == DBNull.Value ? "" : reader[col].ToString();

                            // Gán giá trị
                            row["STT"] = ReadInt("STT");
                            row["ItemId"] = ReadLong("ItemId");
                            row["ServiceName"] = ReadString("ItemName");
                            row["Interpretation"] = $"Bán dịch vụ theo hóa đơn #{selectedInvoiceId}";
                            row["Debit_account"] = debitAccount;

                            string itemType = ReadString("ItemType");
                            if (itemType.Contains("phí phạt") || itemType.Contains("penalty"))
                                row["Credit_account"] = "711";
                            else
                                row["Credit_account"] = "5111";

                            row["Unit_measure"] = ReadString("Unit");
                            row["Quantities"] = ReadDecimal("Quantities");
                            row["UnitPrice"] = ReadDecimal("UnitPrice");
                            row["Amount"] = ReadDecimal("Amount");

                            row["Discount_account"] = "521"; // Ví dụ: TK Chiết khấu thương mại
                            row["DiscountPercent"] = ReadDecimal("DiscountPercent");
                            row["DiscountAmount"] = ReadDecimal("DiscountAmount");

                            row["TaxPercentage"] = ReadDecimal("Tax");
                            row["TaxAmount"] = ReadDecimal("TaxAmount");
                            row["Tax_account"] = "33311";

                            // Tính tổng
                            decimal amount = ReadDecimal("Amount");
                            decimal discount = ReadDecimal("DiscountAmount");
                            decimal tax = ReadDecimal("TaxAmount");
                         row["Total"] = amount - discount + tax;

                            dt.Rows.Add(row);
                        }
                    }
                }

                gcRecognitionDetail.DataSource = dt;
                gcRecognitionDetail.RefreshDataSource();
                gvRecognition_Detail.RefreshData();

                XtraMessageBox.Show(
                    $"Đã tải {dt.Rows.Count} dòng chi tiết từ hóa đơn!",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Lỗi load chi tiết: {ex.Message}\n\n{ex.StackTrace}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

      
        private void LoadReceiptData(long receiptId)
        {
            // VERSION 4: Query không thay đổi vì Receipt không liên quan đến Vehicle
            string query = @"
        SELECT 
            r.*, 
            c.FullName, 
            c.PhoneNumber, 
            c.Email, 
            c.Address, 
            c.TaxCode, 
            c.CustomerID
        FROM Receipt r
        LEFT JOIN Customer c ON r.CustomerId = c.CustomerID
        WHERE r.ReceiptId = @ReceiptId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ReceiptId", receiptId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtCustomerId.Text = reader["CustomerID"]?.ToString() ?? "";
                        txtCustomerName.Text = reader["FullName"]?.ToString() ?? "";
                        txtCustomerTaxCode.Text = reader["TaxCode"]?.ToString() ?? "";
                        txtCustomerAddress.Text = reader["Address"]?.ToString() ?? "";
                        txtCustomerNumberPhone.Text = reader["PhoneNumber"]?.ToString() ?? "";

                        string methodPayment = reader["PaymentMethod"]?.ToString() ?? "";
                        cbMethods_payment.Text = methodPayment;
                        CkMoney_Received.Checked = true;
                    }
                }
            }
        }

        #endregion
        private string GetDebitAccount()
        {
            if (CkMoney_Received.Checked)
            {
                if (cbMethods_payment.Text == "Tiền mặt")
                    return "1111";
                else if (cbMethods_payment.Text == "Tiền ngân hàng")
                    return "1112";
                else
                    return "1111";
            }
            return "131"; // Receivable
        }

        #endregion

        #region GridView Events

        private void GvRecognition_Detail_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column == glQuantities || e.Column == glUnitPrice || e.Column == glTaxPercentag)
            {
                decimal quantities = gvRecognition_Detail.GetRowCellValue(e.RowHandle, glQuantities) != DBNull.Value
                    ? Convert.ToDecimal(gvRecognition_Detail.GetRowCellValue(e.RowHandle, glQuantities)) : 0;

                decimal unitPrice = gvRecognition_Detail.GetRowCellValue(e.RowHandle, glUnitPrice) != DBNull.Value
                    ? Convert.ToDecimal(gvRecognition_Detail.GetRowCellValue(e.RowHandle, glUnitPrice)) : 0;

                decimal amount = quantities * unitPrice;
                gvRecognition_Detail.SetRowCellValue(e.RowHandle, glAmount, amount);

                decimal taxRate = gvRecognition_Detail.GetRowCellValue(e.RowHandle, glTaxPercentag) != DBNull.Value
                    ? Convert.ToDecimal(gvRecognition_Detail.GetRowCellValue(e.RowHandle, glTaxPercentag)) : 0;

                decimal taxAmount = amount * taxRate / 100;
                gvRecognition_Detail.SetRowCellValue(e.RowHandle, glTaxAmount, taxAmount);
                
                decimal discountPercent =   gvRecognition_Detail.GetRowCellValue(e.RowHandle, glDiscountPercent) != DBNull.Value
                    ? Convert.ToDecimal(gvRecognition_Detail.GetRowCellValue(e.RowHandle, glDiscountPercent)) : 0;
                decimal discountAmount = amount * discountPercent / 100;

                gvRecognition_Detail.SetRowCellValue(e.RowHandle, glDiscountAmount, discountAmount); // ⭐️ Gán CK Amount



                decimal total = amount + taxAmount - discountAmount;
                 gvRecognition_Detail.SetRowCellValue(e.RowHandle, "Total", total);
            }
        }

        #endregion

        #region Save Methods

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveRecognition(false);
        }

        private void btnSave_Print_Click(object sender, EventArgs e)
        {
            SaveRecognition(true);
        }

        private void SaveRecognition(bool printAfterSave)
        {
            try
            {
                if (!ValidateForm())
                    return;

                // 1. Kiểm tra và Tạo Khách hàng mới (Nếu cần)
                if (ckNewcustomer.Checked && !selectedCustomerId.HasValue)
                {
                    selectedCustomerId = CreateNewCustomer();
                    if (!selectedCustomerId.HasValue)
                        return;

                    // Cập nhật trường hiển thị
                    txtCustomerId.Text = selectedCustomerId.ToString();
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // 2. Cập nhật CustomerId vào bảng Invoices (nếu có hóa đơn tham chiếu và có CustomerId)
                        if (selectedInvoiceId.HasValue && selectedCustomerId.HasValue)
                        {
                            UpdateInvoiceCustomer(selectedInvoiceId.Value, selectedCustomerId.Value, conn, transaction);
                        }

                        // 3. Lưu Revenue Recognition và chi tiết
                        long recognitionId = InsertRevenueRecognition(conn, transaction);
                        InsertRecordDetails(recognitionId, conn, transaction);

                        transaction.Commit();

                        currentRecognitionId = recognitionId;
                        lbRecognitionId.Text = recognitionId.ToString();

                        XtraMessageBox.Show("Lưu dữ liệu thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (printAfterSave)
                        {
                            PrintRecognition(recognitionId);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
 
        #region Save Methods - VERSION 4 (No Changes Needed)

        /// Cập nhật CustomerId vào bảng Invoices.
        /// VERSION 4: Không thay đổi vì không liên quan đến Vehicle
        private void UpdateInvoiceCustomer(long invoiceId, long customerId, SqlConnection conn, SqlTransaction transaction)
        {
            string query = @"
        UPDATE Invoices
        SET CustomerId = @CustomerId
        WHERE InvoiceId = @InvoiceId 
            AND (CustomerId IS NULL OR CustomerId != @CustomerId)";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                cmd.Parameters.AddWithValue("@InvoiceId", invoiceId);
                cmd.ExecuteNonQuery();
            }
        }

        private long? CreateNewCustomer()
        {
            try
            {
                // VERSION 4: Query không thay đổi
                string query = @"
            INSERT INTO Customer (FullName, PhoneNumber, Email, Address, TaxCode, CreatedAt)
            VALUES (@FullName, @PhoneNumber, @Email, @Address, @TaxCode, GETDATE());
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FullName", txtCustomerName.Text ?? "");
                    cmd.Parameters.AddWithValue("@PhoneNumber", txtCustomerNumberPhone.Text ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", txtCustomerAddress.Text ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TaxCode", txtCustomerTaxCode.Text ?? (object)DBNull.Value);

                    conn.Open();
                    long newCustomerId = (long)cmd.ExecuteScalar();

                    txtCustomerId.Text = newCustomerId.ToString();

                    XtraMessageBox.Show("Đã tạo khách hàng mới thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return newCustomerId;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi tạo khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        #endregion

        #region FIX 6: Enhanced Validation

        /// <summary>
        /// Validation chặt chẽ hơn
        /// </summary>
        private bool ValidateForm()
        {
            // 1. Kiểm tra chi tiết
            if (gvRecognition_Detail.RowCount == 0)
            {
                XtraMessageBox.Show("Vui lòng nhập chi tiết hạch toán!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 2. KHÔNG được tham chiếu cả Invoice VÀ Receipt
            if (selectedInvoiceId.HasValue && selectedReceiptId.HasValue)
            {
                XtraMessageBox.Show(
                    "Không thể tham chiếu cả Hóa đơn và Phiếu thu cùng lúc!\n" +
                    "Vui lòng chỉ chọn một trong hai.",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            // 3. Kiểm tra Employee
            if (string.IsNullOrWhiteSpace(txtEmployeeName.Text))
            {
                XtraMessageBox.Show("Vui lòng nhập tên nhân viên!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmployeeName.Focus();
                return false;
            }

            // 4. Kiểm tra Customer
            if (ckNewcustomer.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
                {
                    XtraMessageBox.Show("Vui lòng nhập tên khách hàng!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCustomerName.Focus();
                    return false;
                }
            }
            else if (!selectedCustomerId.HasValue)
            {
                XtraMessageBox.Show(
                    "Vui lòng chọn khách hàng hoặc tích 'Tạo khách hàng mới'!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            // 5. Kiểm tra Payment Method nếu đã nhận tiền
            if (CkMoney_Received.Checked && string.IsNullOrEmpty(cbMethods_payment.Text))
            {
                XtraMessageBox.Show(
                    "Vui lòng chọn phương thức thanh toán!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                cbMethods_payment.Focus();
                return false;
            }

            return true;
        }

        #endregion


        #region FIX 5: Update InsertRevenueRecognition to include ReceiptId

        /// <summary>
        /// Sửa lại để lưu cả ReceiptId
        /// </summary>
        private long InsertRevenueRecognition(SqlConnection conn, SqlTransaction transaction)
        {
            long employeeIdToRecord = 0;
            try
            {
                employeeIdToRecord = GetOrCreateEmployeeId(txtEmployeeName.Text);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi xác định ID Nhân viên: {ex.Message}");
            }

            // ✅ SỬA: Thêm ReceiptId vào query
            string query = @"
        INSERT INTO Revenue_recognition 
        (Receipt_Delivery, RecordDate, Documentdate, ExpireDate,
         InvoiceId, ReceiptId, InvoiceDetailId, CustomerId, Employeeid)
        VALUES 
        (@Receipt_Delivery, @RecordDate, @Documentdate, @ExpireDate,
         @InvoiceId, @ReceiptId, @InvoiceDetailId, @CustomerId, @Employeeid);
        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@Receipt_Delivery", CkInvoice_Delivery.Checked);
                cmd.Parameters.AddWithValue("@RecordDate", DERecord_date.EditValue);
                cmd.Parameters.AddWithValue("@Documentdate", DEDocumentDate.EditValue);
                cmd.Parameters.AddWithValue("@ExpireDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@InvoiceId", selectedInvoiceId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ReceiptId", selectedReceiptId ?? (object)DBNull.Value); // ✅ THÊM
                cmd.Parameters.AddWithValue("@InvoiceDetailId", DBNull.Value);
                cmd.Parameters.AddWithValue("@CustomerId", selectedCustomerId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Employeeid", employeeIdToRecord);

                return (long)cmd.ExecuteScalar();
            }
        }

        #endregion

        private void InsertRecordDetails(long recognitionId, SqlConnection conn, SqlTransaction transaction)
        {
            DataTable dt = gcRecognitionDetail.DataSource as DataTable;
            if (dt == null) return;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];

                string query = @"
                    INSERT INTO Record_detail 
                    (STT, Recognition_id, ItemId, Interpretation, Debit_account, Credit_account, 
                     Tax_account, Discount_account, UnitPrice, Quantities, Amount, 
                     TaxPercentage, TaxAmount, DiscountPercent, DiscountAmount, Unit_measure)
                    VALUES 
                    (@STT, @Recognition_id, @ItemId, @Interpretation, @Debit_account, @Credit_account,
                     @Tax_account, @Discount_account, @UnitPrice, @Quantities, @Amount,
                     @TaxPercentage, @TaxAmount, @DiscountPercent, @DiscountAmount, @Unit_measure)";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@STT", row["STT"]);
                    cmd.Parameters.AddWithValue("@Recognition_id", recognitionId);
                    cmd.Parameters.AddWithValue("@ItemId", row["ItemId"] ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Interpretation", row["Interpretation"] ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Debit_account", row["Debit_account"] ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Credit_account", row["Credit_account"] ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Tax_account", row["Tax_account"] ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Discount_account", DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitPrice", row["UnitPrice"] ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Quantities", row["Quantities"] ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Amount", row["Amount"] ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TaxPercentage", row["TaxPercentage"] ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TaxAmount", row["TaxAmount"] ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiscountPercent", row["DiscountPercent"] ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiscountAmount", row["DiscountAmount"] ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Unit_measure", row["Unit_measure"] ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        /*
        private long GetCurrentEmployeeId()
        {
            // TODO: Get from session/login
            return 1; // Placeholder
        }*/

        #endregion
        #region Generate Record Detail from Receipt

        /// <summary>
        /// Tạo chi tiết hạch toán từ Receipt
        /// </summary>
        private void GenerateRecordDetailFromReceipt()
        {
            if (!selectedReceiptId.HasValue)
            {
                XtraMessageBox.Show("Chưa chọn phiếu thu!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Load Receipt data
                string query = @"
            SELECT 
                r.ReceiptId,
                r.ReceiptNumber,
                r.TotalAmount,
                r.Description,
                r.PaymentMethod
            FROM Receipt r
            WHERE r.ReceiptId = @ReceiptId";

                DataTable dt = CreateRecordDetailTable();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ReceiptId", selectedReceiptId.Value);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Tạo 1 dòng hạch toán tổng cho Receipt
                            DataRow row = dt.NewRow();

                            decimal totalAmount = reader["TotalAmount"] != DBNull.Value
                                ? Convert.ToDecimal(reader["TotalAmount"])
                                : 0m;

                            string description = reader["Description"]?.ToString() ?? "";
                            string receiptNumber = reader["ReceiptNumber"]?.ToString() ?? "";

                            // Thông tin cơ bản
                            row["STT"] = 1;
                            row["ItemId"] = DBNull.Value; // Receipt không có ItemId
                            row["ServiceName"] = "Thu tiền khách hàng";
                            row["Interpretation"] = string.IsNullOrEmpty(description)
                                ? $"Thu tiền theo phiếu thu #{receiptNumber}"
                                : description;

                            // Tài khoản
                            string debitAccount = GetDebitAccount();
                            row["Debit_account"] = debitAccount;  // TK Tiền (1111 hoặc 1112)
                            row["Credit_account"] = "131";        // TK Phải thu khách hàng

                            // Số lượng và đơn giá
                            row["Unit_measure"] = "Lần";
                            row["Quantities"] = 1;
                            row["UnitPrice"] = totalAmount;
                            row["Amount"] = totalAmount;

                            // Không có chiết khấu và thuế cho Receipt
                            row["Discount_account"] = DBNull.Value;
                            row["DiscountPercent"] = 0;
                            row["DiscountAmount"] = 0;

                            row["TaxPercentage"] = 0;
                            row["TaxAmount"] = 0;
                            row["Tax_account"] = DBNull.Value;

                            // Tổng = Amount (không có CK và thuế)
                            row["Total"] = totalAmount;

                            dt.Rows.Add(row);
                        }
                    }
                }

                gcRecognitionDetail.DataSource = dt;
                gcRecognitionDetail.RefreshDataSource();
                gvRecognition_Detail.RefreshData();

                XtraMessageBox.Show(
                    "Đã tạo chi tiết hạch toán từ phiếu thu!",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Lỗi tạo chi tiết: {ex.Message}\n\n{ex.StackTrace}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        

        #endregion
        #region Print & Open Invoice

        private void PrintRecognition(long recognitionId)
        {
            try
            {
                // Open frmChungTu to print the report
                if (selectedInvoiceId.HasValue)
                {
                    OpenInvoiceReport(selectedInvoiceId.Value);
                }
                else
                {
                    XtraMessageBox.Show("Chức năng in đang được phát triển!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi khi in: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HlInvocieID_Click(object sender, EventArgs e)
        {
            if (!selectedInvoiceId.HasValue)
            {
                XtraMessageBox.Show("Chưa có hóa đơn được chọn!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenInvoiceReport(selectedInvoiceId.Value);
        }

        private void OpenInvoiceReport(long invoiceId)
        {
           /* try
            {
                frmChungTu frm = new Accounting.frmChungTu(invoiceId);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi mở hóa đơn: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        */
         }

        #endregion

        #region Control Events

        private void CkMoney_Received_CheckedChanged(object sender, EventArgs e)
        {
            cbMethods_payment.Enabled = CkMoney_Received.Checked;
            lbMethods_payment.Enabled = CkMoney_Received.Checked;

            if (!CkMoney_Received.Checked)
            {
                cbMethods_payment.EditValue = null;
            }

            UpdateDebitAccountsInGrid();
        }

        private void cbMethods_payment_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDebitAccountsInGrid();
        }

        private void ckNewcustomer_CheckedChanged(object sender, EventArgs e)
        {
            bool isNewCustomer = ckNewcustomer.Checked;
            SetCustomerFieldsReadOnly(!isNewCustomer);

            if (isNewCustomer)
            {
                // Clear customer fields for new entry
                txtCustomerId.Text = "";
                txtCustomerName.Text = "";
                txtCustomerNumberPhone.Text = "";
                txtCustomerAddress.Text = "";
                txtCustomerTaxCode.Text = "";
                selectedCustomerId = null;
            }
        }

        private void UpdateDebitAccountsInGrid()
        {
            string debitAccount = GetDebitAccount();
            DataTable dt = gcRecognitionDetail.DataSource as DataTable;

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    row["Debit_account"] = debitAccount;
                }
                gcRecognitionDetail.RefreshDataSource();
            }
        }

        #endregion

        #region Placeholder Event Handlers (from Designer)

        private void labelControl1_Click(object sender, EventArgs e) { }
        private void lbRecognitionId_Click(object sender, EventArgs e) { }
        private void lbMethods_payment_Click(object sender, EventArgs e) { }
        private void CkInvoice_Delivery_CheckedChanged(object sender, EventArgs e) { }
        private void CkReceipt_Delivery_CheckedChanged(object sender, EventArgs e) { }
        private void labelControl2_Click(object sender, EventArgs e) { }
        private void buttonEdit1_EditValueChanged(object sender, EventArgs e) { }
        private void labelControl4_Click(object sender, EventArgs e) { }
        private void txtCustomerId_EditValueChanged(object sender, EventArgs e) { }
        private void labelControl5_Click(object sender, EventArgs e) { }
        private void txtCustomerName_EditValueChanged(object sender, EventArgs e) { }
        private void labelControl6_Click(object sender, EventArgs e) { }
        private void txtCustomerTaxCode_EditValueChanged(object sender, EventArgs e) { }
        private void labelControl7_Click(object sender, EventArgs e) { }
        private void txtCustomerAddress_EditValueChanged(object sender, EventArgs e) { }
        private void labelControl8_Click(object sender, EventArgs e) { }
        private void txtCustomerNumberPhone_EditValueChanged(object sender, EventArgs e) { }
        private void hlInvocieID_Click_1(object sender, EventArgs e) { }
        private void gridControl1_Click(object sender, EventArgs e) { }
        private void lbInvoiceId_Click(object sender, EventArgs e) { }
        private void gcRecognitionDetail_Click(object sender, EventArgs e) { }
        private void btnThamChieuHD_EditValueChanged(object sender, EventArgs e) { }
        private void btnThamChieuPT_EditValueChanged(object sender, EventArgs e) { }

        #region FIX 2: Add Receipt Report Handler

        /// <summary>
        /// Click vào Receipt ID để xem Report
        /// </summary>
        private void HlReceipt_id_Click(object sender, EventArgs e)
        {
            if (!selectedReceiptId.HasValue)
            {
                XtraMessageBox.Show("Chưa có phiếu thu được chọn!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenReceiptReport(selectedReceiptId.Value);
        }

        /// <summary>
        /// Mở Receipt Report
        /// </summary>
        private void OpenReceiptReport(long receiptId)
        {
            try
            {
                frmChungTu frm = new frmChungTu(receiptId, true); // true = isReceipt
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi mở phiếu thu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion


        #endregion











        #region FIX 7: Add Clear/Reset Function (Optional but Recommended)

        /// <summary>
        /// Xóa tất cả dữ liệu và bắt đầu lại
        /// </summary>
        private void ClearForm()
        {
            // Reset IDs
            selectedInvoiceId = null;
            selectedReceiptId = null;
            selectedCustomerId = null;
            currentRecognitionId = null;

            // Reset labels
            lbRecognitionId.Text = "Mã mới";
            hlInvocieID.Text = "Chưa có";
            hlReceipt_id.Text = "Chưa có";

            // Reset customer fields
            txtCustomerId.Text = "";
            txtCustomerName.Text = "";
            txtCustomerNumberPhone.Text = "";
            txtCustomerAddress.Text = "";
            txtCustomerTaxCode.Text = "";

            // Reset employee
            txtEmployeeName.Text = "";

            // Reset checkboxes
            ckNewcustomer.Checked = false;
            CkMoney_Received.Checked = false;
            CkInvoice_Delivery.Checked = false;

            // Reset payment method
            cbMethods_payment.EditValue = null;

            // Reset dates
            SetDefaultDates();

            // Clear grid
            gcRecognitionDetail.DataSource = null;

            // Unlock controls
            LockControlsOnReference(false);
        }

        // Wire up trong InitializeForm():
        // btnClear.Click += (s, e) => 
        // {
        //     if (XtraMessageBox.Show("Xóa tất cả dữ liệu?", "Xác nhận",
        //         MessageBoxButtons.YesNo) == DialogResult.Yes)
        //     {
        //         ClearForm();
        //     }
        // };













    }



























    #region Helper Class - DatabaseHelper

    public static class DatabaseHelper
    {
        public static DataTable ExecuteQuery(string connectionString, string query, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    conn.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        public static int ExecuteNonQuery(string connectionString, string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string connectionString, string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }
    }

    #endregion
}

#endregion
