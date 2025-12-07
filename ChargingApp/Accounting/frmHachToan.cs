using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ChargingApp.Helpers;
using ChargingApp.Report;
using DevExpress.XtraEditors;

namespace ChargingApp.Accounting
{
    public partial class frmHachToan : DevExpress.XtraEditors.XtraForm
    {
        private string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=ChargingPoint;Integrated Security=True;Connect Timeout=30";
        private long? currentRecognitionId = null;
        private long? selectedInvoiceId = null;
        private long? selectedReceiptId = null;
        private long? selectedCustomerId = null;

        public frmHachToan()
        {
            InitializeComponent();
        }

        private void frmHachToan_Load(object sender, EventArgs e)
        {
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
            cbMethods_payment.Enabled = false;
            lbMethods_payment.Enabled = false;
            SetupGridView();

            // Wire up events
            btnSave.Click += btnSave_Click;
            btnSave_Print.Click += btnSave_Print_Click;
            btnThamChieuHD.Click += btnThamChieuHD_Click;
            btnThamChieuPT.Click += btnThamChieuPT_Click;
            hlInvocieID.Click += HlInvocieID_Click;
            CkMoney_Received.CheckedChanged += CkMoney_Received_CheckedChanged;
            cbMethods_payment.SelectedIndexChanged += cbMethods_payment_SelectedIndexChanged;
            ckNewcustomer.CheckedChanged += ckNewcustomer_CheckedChanged;

            // Initially disable customer fields
            SetCustomerFieldsReadOnly(true);
            ckNewcustomer.Checked = false;
        }

        private void SetCustomerFieldsReadOnly(bool readOnly)
        {
            txtCustomerName.ReadOnly = readOnly;
            txtCustomerNumberPhone.ReadOnly = readOnly;
            txtCustomerAddress.ReadOnly = readOnly;
            txtCustomerTaxCode.ReadOnly = readOnly;
        }

        private void SetupGridView()
        {
            try
            {
                gvRecognition_Detail.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
                gvRecognition_Detail.OptionsBehavior.Editable = true;

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

                AddAccountRepositoryItems();
                gvRecognition_Detail.CellValueChanged += GvRecognition_Detail_CellValueChanged;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi setup GridView: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        }

        private DataTable GetAccounts()
        {
            string query = "SELECT Acc_Code2, Acc_Name FROM COA ORDER BY Acc_Code2";
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
                }
            }
        }

        private void btnThamChieuPT_Click(object sender, EventArgs e)
        {
            using (var searchForm = new frmSearchDocument("Receipt"))
            {
                if (searchForm.ShowDialog() == DialogResult.OK)
                {
                    selectedReceiptId = long.Parse(searchForm.SelectedDocumentId);
                    hlReceipt_id.Text = "PT #" + selectedReceiptId;
                    LoadReceiptData(selectedReceiptId.Value);
                }
            }
        }

        private void LoadInvoiceAndCustomerData(long invoiceId)
        {
            string query = @"
                SELECT 
                    i.InvoiceId, i.Snashot_CustomerName, i.Snashot_CustomerPhone, i.Snashot_CustomerEmail,
                    i.Status, i.PaymentMethod, i.CustomerId,
                    c.FullName, c.PhoneNumber, c.Email, c.Address, c.TaxCode
                FROM Invoices i
                LEFT JOIN Customer c ON i.CustomerId = c.CustomerId
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
                        ckNewcustomer.Enabled = false;
                        ckNewcustomer.Checked = false;
                    }
                    else
                    {
                        selectedCustomerId = null;
                        txtCustomerId.Text = "";
                        ckNewcustomer.Enabled = true;
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
                }
            }
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
                        id.InvoiceId, id.STT, id.ItemId, id.Quantities, id.Unit, id.UnitPrice,
                        id.Amount, id.DiscountPercent, id.DiscountAmount, id.AmountAfterDiscount,
                        id.Tax, id.TaxAmount, id.TotalLine,
                        ri.ItemName, ri.ItemType
                    FROM InvoiceDetail id
                    LEFT JOIN RevenueItem ri ON id.ItemId = ri.ItemId
                    WHERE id.InvoiceId = @InvoiceId
                    ORDER BY id.STT";

                DataTable dt = new DataTable();
                dt.Columns.Add("STT", typeof(int));
                dt.Columns.Add("ItemId", typeof(long));
                dt.Columns.Add("ServiceName", typeof(string));
                dt.Columns.Add("Interpretation", typeof(string));
                dt.Columns.Add("Debit_account", typeof(string));
                dt.Columns.Add("Credit_account", typeof(string));
                dt.Columns.Add("Unit_measure", typeof(string));
                dt.Columns.Add("Quantities", typeof(int));
                dt.Columns.Add("UnitPrice", typeof(decimal));
                dt.Columns.Add("Amount", typeof(decimal));
                dt.Columns.Add("DiscountPercent", typeof(decimal));
                dt.Columns.Add("DiscountAmount", typeof(decimal));
                dt.Columns.Add("TaxPercentage", typeof(decimal));
                dt.Columns.Add("TaxAmount", typeof(decimal));
                dt.Columns.Add("Tax_account", typeof(string));
                dt.Columns.Add("Total", typeof(decimal));

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
                            row["STT"] = reader["STT"];
                            row["ItemId"] = reader["ItemId"] ?? DBNull.Value;
                            row["ServiceName"] = reader["ItemName"]?.ToString() ?? "Dịch vụ";
                            row["Interpretation"] = $"Bán dịch vụ theo hóa đơn #{selectedInvoiceId}";
                            row["Debit_account"] = debitAccount;

                            // Credit account based on ItemType
                            string itemType = reader["ItemType"]?.ToString() ?? "";
                            if (itemType.Contains("phí phạt") || itemType.Contains("penalty"))
                                row["Credit_account"] = "711"; // Thu nhập khác
                            else
                                row["Credit_account"] = "5111"; // Doanh thu bán hàng

                            row["Unit_measure"] = reader["Unit"]?.ToString() ?? "";
                            row["Quantities"] = reader["Quantities"] ?? 0;
                            row["UnitPrice"] = reader["UnitPrice"] ?? 0;
                            row["Amount"] = reader["Amount"] ?? 0;
                            row["DiscountPercent"] = reader["DiscountPercent"] ?? 0;
                            row["DiscountAmount"] = reader["DiscountAmount"] ?? 0;
                            row["TaxPercentage"] = reader["Tax"] ?? 10;
                            row["TaxAmount"] = reader["TaxAmount"] ?? 0;
                            row["Tax_account"] = "33311";
                            row["Total"] = reader["TotalLine"] ?? 0;

                            dt.Rows.Add(row);
                        }
                    }
                }

                gcRecognitionDetail.DataSource = dt;
                gcRecognitionDetail.RefreshDataSource();
                gvRecognition_Detail.RefreshData();

                XtraMessageBox.Show($"Đã tải {dt.Rows.Count} dòng chi tiết từ hóa đơn!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi load chi tiết: {ex.Message}\n\n{ex.StackTrace}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadReceiptData(long receiptId)
        {
            string query = @"
                SELECT r.*, c.FullName, c.PhoneNumber, c.Email, c.Address, c.TaxCode, c.CustomerId
                FROM Receipt r
                LEFT JOIN Customer c ON r.CustomerId = c.CustomerId
                WHERE r.Id = @ReceiptId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ReceiptId", receiptId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtCustomerId.Text = reader["CustomerId"]?.ToString() ?? "";
                        txtCustomerName.Text = reader["FullName"]?.ToString() ?? "";
                        txtCustomerTaxCode.Text = reader["TaxCode"]?.ToString() ?? "";
                        txtCustomerAddress.Text = reader["Address"]?.ToString() ?? "";
                        txtCustomerNumberPhone.Text = reader["PhoneNumber"]?.ToString() ?? "";

                        string methodPayment = reader["Method_payment"]?.ToString() ?? "";
                        cbMethods_payment.Text = methodPayment;
                        CkMoney_Received.Checked = true;
                    }
                }
            }
        }

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

                decimal total = amount + taxAmount;
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
        /// <summary>
        /// Cập nhật CustomerId vào bảng Invoices.
        /// </summary>
        private void UpdateInvoiceCustomer(long invoiceId, long customerId, SqlConnection conn, SqlTransaction transaction)
        {
            // Kiểm tra chỉ cập nhật nếu CustomerId trong Invoices hiện đang là NULL hoặc khác customerId truyền vào
            string query = @"
                UPDATE Invoices
                SET CustomerId = @CustomerId
                WHERE InvoiceId = @InvoiceId AND (CustomerId IS NULL OR CustomerId != @CustomerId)";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                cmd.Parameters.AddWithValue("@InvoiceId", invoiceId);
                cmd.ExecuteNonQuery();
            }
        }
        

        private bool ValidateForm()
        {
            if (gvRecognition_Detail.RowCount == 0)
            {
                XtraMessageBox.Show("Vui lòng nhập chi tiết hạch toán!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

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
                XtraMessageBox.Show("Vui lòng chọn khách hàng hoặc tích 'Tạo khách hàng mới'!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private long? CreateNewCustomer()
        {
            try
            {
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

        private long InsertRevenueRecognition(SqlConnection conn, SqlTransaction transaction)
        {
            string query = @"
                INSERT INTO Revenue_recognition 
                (Sell_type, Receipt_Delivery, RecordDate, Documentdate, ExpireDate,
                 InvoiceId, InvoiceDetailId, CustomerId, Employeeid)
                VALUES 
                (@Sell_type, @Receipt_Delivery, @RecordDate, @Documentdate, @ExpireDate,
                 @InvoiceId, @InvoiceDetailId, @CustomerId, @Employeeid);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@Sell_type", "1. bán hàng trong nước"); // Default
                cmd.Parameters.AddWithValue("@Receipt_Delivery", CkInvoice_Delivery.Checked);
                cmd.Parameters.AddWithValue("@RecordDate", DERecord_date.EditValue);
                cmd.Parameters.AddWithValue("@Documentdate", DEDocumentDate.EditValue);
                cmd.Parameters.AddWithValue("@ExpireDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@InvoiceId", selectedInvoiceId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@InvoiceDetailId", DBNull.Value); // Can be set if needed
                cmd.Parameters.AddWithValue("@CustomerId", selectedCustomerId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Employeeid", GetCurrentEmployeeId());

                return (long)cmd.ExecuteScalar();
            }
        }

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

        private long GetCurrentEmployeeId()
        {
            // TODO: Get from session/login
            return 1; // Placeholder
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
        private void hlReceipt_id_Click(object sender, EventArgs e) { }

        #endregion

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void DERecord_date_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void DEDocumentDate_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void labelControl8_Click_1(object sender, EventArgs e)
        {

        }

        private void labelControl5_Click_1(object sender, EventArgs e)
        {

        }

        private void labelControl4_Click_1(object sender, EventArgs e)
        {

        }
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

