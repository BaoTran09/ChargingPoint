using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using ChargingApp.Report;
using ChargingApp.Helpers;
using System.Drawing;
using MailKit.Net.Smtp;
using MimeKit;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using System.IO;
using ChargingApp.Helper;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using Newtonsoft.Json;


namespace ChargingApp.Accounting
{
    public partial class frmDSChungTu : DevExpress.XtraEditors.XtraForm
    {
        private string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=ChargingPoint;Integrated Security=True;Connect Timeout=30";
        private DataTable dtInvoices;
        private DataTable dtDocuments;

        public frmDSChungTu()
        {
            InitializeComponent();
        }

        #region Form Load & Initialization

        private void frmDSChungTu_Load(object sender, EventArgs e)
        {
            InitializeForm();
            InitializeFilters();
            LoadDataAsync();

        }
        private void InitializeForm()
        {
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(1024, 768);

            // Setup GridView options
            gvDocument.OptionsBehavior.Editable = true;
            gvDocument.OptionsView.ShowAutoFilterRow = true;
            gvDocument.OptionsFind.AlwaysVisible = true;
            gvDocument.OptionsSelection.MultiSelect = true;
            gvDocument.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
            gvDocument.OptionsBehavior.ReadOnly = false;
            // Setup Detail GridView
            gvDocumentDetail.OptionsBehavior.Editable = false;
            gvDocumentDetail.OptionsView.ShowFooter = true;

         


            // Setup column formats
            var colTotal = gvDocument.Columns.ColumnByFieldName("TotalAmount");
            if (colTotal != null)
            {
                colTotal.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                colTotal.DisplayFormat.FormatString = "N0";
            }

            
            // Wire up events
            btnOpenFilter.Click += BtnOpenFilter_Click;
            btnFilter.Click += BtnFilter_Click;
            btnReset.Click += BtnReset_Click;
            btnCancel.Click += BtnCancel_Click;

            btnReloadtoolstrip.Click += BtnReload_Click;
            btnAdd.Click += BtnAdd_Click;
            btnExportExcel.Click += BtnExportExcel_Click;
            btnExportPDF.Click += BtnExportPDF_Click;

            gvDocument.FocusedRowChanged += GvDocument_FocusedRowChanged;
          
            // Tạo nút đẹp có chữ + mũi tên xuống


            repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();

            repositoryItemButtonEdit1.Buttons.Clear();

            // Tạo nút đẹp có chữ + mũi tên xuống
            var btn = new DevExpress.XtraEditors.Controls.EditorButton();
            btn.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Combo;   // Mũi tên xuống
            btn.Caption = "Thao tác";                                          // CHỮ HIỆN TRÊN NÚT
            btn.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            btn.Appearance.ForeColor = System.Drawing.Color.DarkBlue;

            repositoryItemButtonEdit1.Buttons.Add(btn);

            // 2 DÒNG THẦN THÁNH – THIẾU 1 CÁI LÀ NÚT CHỈ HIỆN "..." VÀ KHÔNG BẤM ĐƯỢC!
            repositoryItemButtonEdit1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // repositoryItemButtonEdit1.ButtonPressed += repositoryItemButtonEdit1_ButtonPressed; // DÙNG ButtonPressed, KHÔNG DÙNG ButtonClick!!!
            repositoryItemButtonEdit1.ButtonClick += repositoryItemButtonEdit1_ButtonClick;
           // repositoryItemButtonEdit1.ButtonPressed += repositoryItemButtonEdit1_ButtonClick;
            // Bonus: cho đẹp hơn
            repositoryItemButtonEdit1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            

        }

        private void InitializeFilters()
        {
            // 1. Document Type
            DataTable dtType = new DataTable();
            dtType.Columns.Add("ID", typeof(string));
            dtType.Columns.Add("Name", typeof(string));

            dtType.Rows.Add("All", "Tất cả");
            dtType.Rows.Add("Invoice", "Hóa đơn");
            dtType.Rows.Add("Receipt", "Phiếu thu");
            dtType.Rows.Add("Revenue", "Ghi nhận doanh thu");

            lupEDocumentType.Properties.DataSource = dtType;
            lupEDocumentType.Properties.DisplayMember = "Name";
            lupEDocumentType.Properties.ValueMember = "ID";
            lupEDocumentType.EditValue = "All";

            // 2. Document Status (Recording Status)
            DataTable dtDocStatus = new DataTable();
            dtDocStatus.Columns.Add("ID", typeof(string));
            dtDocStatus.Columns.Add("Name", typeof(string));

            dtDocStatus.Rows.Add("All", "Tất cả");
            dtDocStatus.Rows.Add("Posted", "Đã ghi sổ");
            dtDocStatus.Rows.Add("Unposted", "Chưa ghi sổ");

            lupEDocumentStatus.Properties.DataSource = dtDocStatus;
            lupEDocumentStatus.Properties.DisplayMember = "Name";
            lupEDocumentStatus.Properties.ValueMember = "ID";
            lupEDocumentStatus.EditValue = "All";

            // 3. Payment Status
            DataTable dtPayStatus = new DataTable();
            dtPayStatus.Columns.Add("ID", typeof(string));
            dtPayStatus.Columns.Add("Name", typeof(string));

            dtPayStatus.Rows.Add("All", "Tất cả");
            dtPayStatus.Rows.Add("Paid", "Đã thanh toán");
            dtPayStatus.Rows.Add("Unpaid", "Chưa thanh toán");
            dtPayStatus.Rows.Add("Posted", "Đã ghi sổ");
            dtPayStatus.Rows.Add("Cancelled", "Đã hủy");

            lupEPaymentStatus.Properties.DataSource = dtPayStatus;
            lupEPaymentStatus.Properties.DisplayMember = "Name";
            lupEPaymentStatus.Properties.ValueMember = "ID";
            lupEPaymentStatus.EditValue = "All";

            // 4. Date range
            DEtoDate.EditValue = DateTime.Now;
            DEfromDate.EditValue = DateTime.Now.AddMonths(-1);
        }
        #endregion

        #region Load Data - FIXED WITH DEBUG

        private async void LoadDataAsync()
        {
            try
            {
                gvDocument.GridControl.Cursor = Cursors.WaitCursor;

                // CRITICAL FIX: Lấy giá trị TRƯỚC KHI chạy Task.Run
                DateTime? fromDate = DEfromDate.EditValue as DateTime?;
                DateTime? toDate = DEtoDate.EditValue as DateTime?;

                // CRITICAL FIX: Lấy VALUE (ID), không phải TEXT
                string docTypeValue = lupEDocumentType.EditValue?.ToString();
                string paymentStatusValue = lupEPaymentStatus.EditValue?.ToString();
                string recordingStatusValue = lupEDocumentStatus.EditValue?.ToString();

                // DEBUG: In ra giá trị để kiểm tra
                System.Diagnostics.Debug.WriteLine($"=== FILTER DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"DocType: {docTypeValue}");
                System.Diagnostics.Debug.WriteLine($"PaymentStatus: {paymentStatusValue}");
                System.Diagnostics.Debug.WriteLine($"RecordingStatus: {recordingStatusValue}");
                System.Diagnostics.Debug.WriteLine($"FromDate: {fromDate}");
                System.Diagnostics.Debug.WriteLine($"ToDate: {toDate}");

                // CRITICAL FIX: Map giá trị đúng TRƯỚC KHI gọi DataHelper
                string docType = MapDocumentType(docTypeValue);
                string paymentStatus = MapPaymentStatus(paymentStatusValue);
                string recordingStatus = MapRecordingStatus(recordingStatusValue);

                System.Diagnostics.Debug.WriteLine($"=== MAPPED VALUES ===");
                System.Diagnostics.Debug.WriteLine($"Mapped DocType: {docType}");
                System.Diagnostics.Debug.WriteLine($"Mapped PaymentStatus: {paymentStatus}");
                System.Diagnostics.Debug.WriteLine($"Mapped RecordingStatus: {recordingStatus}");

                // Chạy trên thread khác
                await Task.Run(() =>
                {
                    dtDocuments = DataHelper.LoadDocumentList(
                        docType,
                        fromDate,
                        toDate,
                        paymentStatus,
                        recordingStatus
                    );
                });

                // DEBUG: Kiểm tra kết quả
                System.Diagnostics.Debug.WriteLine($"=== RESULT ===");
                System.Diagnostics.Debug.WriteLine($"dtDocuments is null: {dtDocuments == null}");
                System.Diagnostics.Debug.WriteLine($"Row Count: {dtDocuments?.Rows.Count ?? 0}");

                if (dtDocuments != null && dtDocuments.Rows.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Columns: {string.Join(", ", dtDocuments.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}");
                    System.Diagnostics.Debug.WriteLine($"First Row Sample: {string.Join(", ", dtDocuments.Rows[0].ItemArray)}");
                }

                // CRITICAL FIX: Đảm bảo DataSource được set đúng
                if (dtDocuments == null)
                {
                    dtDocuments = new DataTable();
                    XtraMessageBox.Show("Không thể tải dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // CRITICAL FIX: Clear columns và auto-generate lại
                gvDocument.Columns.Clear();

                // Gán dữ liệu vào Grid
                gcDocument.DataSource = null; // Reset trước
                gcDocument.DataSource = dtDocuments;

                gcDocument.RefreshDataSource();
                if (!dtDocuments.Columns.Contains("DetailTable"))
                    dtDocuments.Columns.Add("DetailTable", typeof(DataTable));

                gcDocument.RefreshDataSource();
                gvDocument.BestFitColumns();
                // CRITICAL: Auto-generate columns from DataTable
                gcDocument.ForceInitialize();
                gvDocument.PopulateColumns();
               

                // Setup lại format cho các cột số
                SetupColumnFormats();

                // Setup lại Action column
                SetupActionColumn();

                gvDocument.BestFitColumns();

                // Hiển thị thông báo nếu không có dữ liệu
                if (dtDocuments.Rows.Count == 0)
                {
                    XtraMessageBox.Show(
                        "Không tìm thấy dữ liệu phù hợp với điều kiện lọc!\n\n" +
                        $"Loại chứng từ: {docType}\n" +
                        $"Trạng thái: {paymentStatus ?? "Tất cả"}\n" +
                        $"Ghi sổ: {recordingStatus ?? "Tất cả"}\n" +
                        $"Từ ngày: {fromDate:dd/MM/yyyy}\n" +
                        $"Đến ngày: {toDate:dd/MM/yyyy}",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                gvDocument.GridControl.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                gvDocument.GridControl.Cursor = Cursors.Default;
                XtraMessageBox.Show(
                    $"Lỗi tải dữ liệu: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"STACK: {ex.StackTrace}");
            }
        }

        // CRITICAL: Hàm mapping giá trị
        private string MapDocumentType(string value)
        {
            if (string.IsNullOrEmpty(value) || value == "All") return "All";
            // Value đã đúng rồi vì ta dùng ValueMember = "ID"
            return value;
        }

        private string MapPaymentStatus(string value)
        {
            if (string.IsNullOrEmpty(value) || value == "All") return null;
            // Value đã đúng rồi vì ta dùng ValueMember = "ID"
            return value;
        }

        private string MapRecordingStatus(string value)
        {
            if (string.IsNullOrEmpty(value) || value == "All") return null;
            // Value đã đúng rồi vì ta dùng ValueMember = "ID"
            return value;
        }

        /// <summary>
        /// Setup format cho các cột sau khi auto-generate
        /// </summary>
        private void SetupColumnFormats()
        {
            // Format cột TotalAmount
            var colTotal = gvDocument.Columns.ColumnByFieldName("TotalAmount");
            if (colTotal != null)
            {
                colTotal.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                colTotal.DisplayFormat.FormatString = "N0";
                colTotal.Caption = "Tổng tiền";
            }

            // Format cột DocumentDate
            var colDate = gvDocument.Columns.ColumnByFieldName("DocumentDate");
            if (colDate != null)
            {
                colDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                colDate.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                colDate.Caption = "Ngày chứng từ";
            }

            // Caption cho các cột khác
            var colType = gvDocument.Columns.ColumnByFieldName("DocumentType");
            if (colType != null) colType.Caption = "Loại chứng từ";

            var colNumber = gvDocument.Columns.ColumnByFieldName("DocumentNumber");
            if (colNumber != null) colNumber.Caption = "Số chứng từ";

            var colCustomer = gvDocument.Columns.ColumnByFieldName("CustomerName");
            if (colCustomer != null) colCustomer.Caption = "Khách hàng";

            var colEmail = gvDocument.Columns.ColumnByFieldName("CustomerEmail");
            if (colEmail != null) colEmail.Caption = "Email";

            var colPayStatus = gvDocument.Columns.ColumnByFieldName("PaymentStatus");
            if (colPayStatus != null) colPayStatus.Caption = "Trạng thái";

            var colPayMethod = gvDocument.Columns.ColumnByFieldName("PaymentMethod");
            if (colPayMethod != null) colPayMethod.Caption = "PTTT";

            var colRecStatus = gvDocument.Columns.ColumnByFieldName("RecordingStatus");
            if (colRecStatus != null) colRecStatus.Caption = "Ghi sổ";

            // Ẩn cột CustomerId
            var colCustId = gvDocument.Columns.ColumnByFieldName("CustomerId");
            if (colCustId != null) colCustId.Visible = false;

            // Ẩn cột DocumentId
            var colDetail = gvDocument.Columns.ColumnByFieldName("DetailTable");
            if (colDetail != null)
            {
                colDetail.Visible = false;
                colDetail.OptionsColumn.ShowInCustomizationForm = false;
            }

            // Ẩn DocumentId nếu chưa ẩn
            var colDocId = gvDocument.Columns.ColumnByFieldName("DocumentId");
            if (colDocId != null) colDocId.Visible = false;


        }
        private void SetupDetailColumns(string docType)
        {
            // Reset tất cả cột về ẩn trước
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gvDocumentDetail.Columns)
            {
                col.Visible = false;
            }

            // Cấu hình theo loại chứng từ
            switch (docType)
            {
                case "Invoice":
                    colSTT.Visible = true;
                    colSTT.FieldName = "STT";

                    colItemName.Visible = true;
                    colItemName.FieldName = "ItemName";
                    colItemName.Caption = "Tên hàng hóa, dịch vụ";

                    colUnit.Visible = true;
                    colUnit.FieldName = "Unit";

                    colQuantities.Visible = true;
                    colQuantities.FieldName = "Quantities";

                    colUnitPrice.Visible = true;
                    colUnitPrice.FieldName = "UnitPrice";

                    colAmount.Visible = true;
                    colAmount.FieldName = "Amount";
                    colAmount.Caption = "Thành tiền";

                    colTaxPercent.Visible = true;
                    colTaxPercent.FieldName = "TaxPercent";
                    colTaxPercent.Caption = "Thuế suất (%)";

                    colTaxAmount.Visible = true;
                    colTaxAmount.FieldName = "TaxAmount";
                    colTaxAmount.Caption = "Tiền thuế";

                    break;

                case "Receipt":
                    colSTT.Visible = true;
                    colSTT.FieldName = "STT";

                    colDescription.Visible = true;
                    colDescription.FieldName = "Description";
                    colDescription.Caption = "Nội dung thu";

                    colDebitAccount.Visible = true;
                    colDebitAccount.FieldName = "Debit_Account";
                    colDebitAccount.Caption = "TK Nợ";

                    colCreditAccount.Visible = true;
                    colCreditAccount.FieldName = "Credit_Account";
                    colCreditAccount.Caption = "TK Có";

                    colAmount.Visible = true;
                    colAmount.FieldName = "Amount";
                    colAmount.Caption = "Số tiền";

                    break;

                case "Revenue":
                    colSTT.Visible = true;
                    colSTT.FieldName = "STT";

                    colItemName.Visible = true;
                    colItemName.FieldName = "ItemName";
                    colItemName.Caption = "Nội dung ghi nhận";

                    colUnit.Visible = true;
                    colUnit.FieldName = "Unit_measure";

                    colQuantities.Visible = true;
                    colQuantities.FieldName = "Quantities";

                    colUnitPrice.Visible = true;
                    colUnitPrice.FieldName = "UnitPrice";

                    colAmount.Visible = true;
                    colAmount.FieldName = "Amount";

                    colDebitAccount.Visible = true;
                    colDebitAccount.FieldName = "Debit_account";
                    colDebitAccount.Caption = "TK Nợ";

                    colCreditAccount.Visible = true;
                    colCreditAccount.FieldName = "Credit_account";
                    colCreditAccount.Caption = "TK Có";

                    colTaxPercent.Visible = true;
                    colTaxPercent.FieldName = "TaxPercentage";

                    colTaxAmount.Visible = true;
                    colTaxAmount.FieldName = "TaxAmount";

                    break;
            }

            gvDocument.BestFitColumns();
        }

        /// <summary>
        /// Setup Action column với ComboBox
        /// </summary>


        #endregion

        #region Grid Events & Detail Loading

        private void GvDocument_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0) return;

            try
            {
                DataRow row = gvDocument.GetDataRow(e.FocusedRowHandle);
                if (row == null) return;

                string docType = row["DocumentType"]?.ToString() ?? "";
                long docId = Convert.ToInt64(row["DocumentId"]);

                DataTable detailData = DataHelper.LoadDocumentDetail(docType, docId);

                gcDocumentDetail.DataSource = detailData;

                // QUAN TRỌNG: Setup cột theo loại chứng từ
                SetupDetailColumns(docType);

                gvDocumentDetail.BestFitColumns();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi load chi tiết: {ex.Message}");
            }
        }
        private void LoadDocumentDetail(string documentType, long documentId)
        {
            try
            {
                DataTable dt = DataHelper.LoadDocumentDetail(documentType, documentId);
                gcDocumentDetail.DataSource = null;
                gcDocumentDetail.DataSource = dt;
                gcDocumentDetail.RefreshDataSource();
                gcDocumentDetail.ForceInitialize();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading detail: {ex.Message}");
            }
        }



        #endregion

        #region Actions

       private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            int rowHandle = gvDocument.FocusedRowHandle;
            if (rowHandle < 0) return;

            ShowActionMenu(rowHandle, sender as ButtonEdit);
        }
        private void ShowActionMenu(int rowHandle, ButtonEdit editor = null)
        {
            DataRow row = gvDocument.GetDataRow(rowHandle);
            if (row == null) return;

            string docType = row["DocumentType"]?.ToString() ?? "";

            ContextMenuStrip contextMenu = new ContextMenuStrip();

            // Thêm items
            if (docType == "Invoice" )
            {
                ToolStripMenuItem itemView = new ToolStripMenuItem("Xem chi tiết");
                itemView.Click += (s, e) => ExecuteAction(rowHandle, "Xem chi tiết");
                contextMenu.Items.Add(itemView);

                ToolStripMenuItem itemPDF = new ToolStripMenuItem("Xuất PDF");
                itemPDF.Click += (s, e) => ExecuteAction(rowHandle, "Xuất PDF");
                contextMenu.Items.Add(itemPDF);

                ToolStripMenuItem itemEmail = new ToolStripMenuItem("Gửi Email KH");
                itemEmail.Click += (s, e) => ExecuteAction(rowHandle, "Gửi Email KH");
                contextMenu.Items.Add(itemEmail);

                ToolStripMenuItem itemReminder = new ToolStripMenuItem("Gửi Email Nhắc nợ");
                itemReminder.Click += (s, e) => ExecuteAction(rowHandle, "Gửi Email Nhắc nợ");
                contextMenu.Items.Add(itemReminder);

                contextMenu.Items.Add(new ToolStripSeparator());
            }
            if (docType == "Receipt")
            {
                ToolStripMenuItem itemView = new ToolStripMenuItem("Xem chi tiết");
                itemView.Click += (s, e) => ExecuteAction(rowHandle, "Xem chi tiết");
                contextMenu.Items.Add(itemView);

                ToolStripMenuItem itemPDF = new ToolStripMenuItem("Xuất PDF");
                itemPDF.Click += (s, e) => ExecuteAction(rowHandle, "Xuất PDF");
                contextMenu.Items.Add(itemPDF);

                ToolStripMenuItem itemEmail = new ToolStripMenuItem("Gửi Email thanh toán thành công");
                itemEmail.Click += (s, e) => ExecuteAction(rowHandle, "Gửi Email thanh toán thành công");
                contextMenu.Items.Add(itemEmail);


                contextMenu.Items.Add(new ToolStripSeparator());
            }
           
            ToolStripMenuItem itemAccounting = new ToolStripMenuItem("Hạch toán");
            itemAccounting.Click += (s, e) => ExecuteAction(rowHandle, "Hạch toán");
            contextMenu.Items.Add(itemAccounting);

            // ✅ ĐÚNG: Dispose SAU KHI menu đã đóng hoàn toàn
            contextMenu.Closed += (s, e) =>
            {
                // Delay để đảm bảo click event đã xử lý xong
                System.Threading.Tasks.Task.Delay(100).ContinueWith(_ =>
                {
                    if (this.InvokeRequired)
                        this.Invoke(new Action(() => contextMenu.Dispose()));
                    else
                        contextMenu.Dispose();
                });
            };

            // Hiện menu
            if (editor != null)
            {
                Point pt = editor.PointToScreen(new Point(0, editor.Height));
                contextMenu.Show(pt);
            }
            else
            {
                contextMenu.Show(Cursor.Position);
            }
        }
        // 2. Setup cột Thao tác – CHẠY SAU PopulateColumns()!!!
        private void SetupActionColumn()
        {
            var colAction = gvDocument.Columns["Action"]
                ?? gvDocument.Columns.AddField("Action");

            colAction.Caption = "Thao tác";
            colAction.ColumnEdit = repositoryItemButtonEdit1;
            colAction.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            colAction.OptionsColumn.AllowEdit = true;
            colAction.OptionsColumn.ReadOnly = false;
            colAction.Width = 110;
            colAction.Visible = true;
            colAction.VisibleIndex = gvDocument.Columns.Count - 1;
        }

        // 3. ExecuteAction 
        // Thay thế hàm ExecuteAction trong frmDSChungTu

        private void ExecuteAction(int rowHandle, string action)
        {
            try
            {
                DataRow row = gvDocument.GetDataRow(rowHandle);
                if (row == null) return;

                string docType = row["DocumentType"]?.ToString() ?? "";
                long docId = Convert.ToInt64(row["DocumentId"]);
                string customerEmail = row.Table.Columns.Contains("CustomerEmail")
                    ? row["CustomerEmail"]?.ToString()
                    : "";

                // === XEM CHI TIẾT - Hỗ trợ tất cả loại ===
                if (action == "Xem chi tiết")
                {
                    if (docType == "Invoice")
                    {
                        new frmChungTu(docId).ShowDialog();
                    }
                    else if (docType == "Receipt")
                    {
                        new frmChungTu(docId, true).ShowDialog(); // true = isReceipt
                    }
                    else if (docType == "Revenue")
                    {
                        XtraMessageBox.Show($"Xem bút toán ghi nhận doanh thu ID: {docId}", "Thông báo");
                        // TODO: Mở form xem Revenue nếu có
                    }
                    return;
                }

                // === HẠCH TOÁN - Hỗ trợ tất cả loại ===
                if (action == "Hạch toán")
                {
                    new frmHachToan().ShowDialog();
                    LoadDataAsync();
                    return;
                }

                // === CÁC ACTION CHO INVOICE ===
                if (docType == "Invoice")
                {
                    switch (action)
                    {
                        case "Xuất PDF":
                            ExportInvoiceToPDF(docId);
                            break;

                        case "Gửi Email KH":
                        case "Gửi Email Nhắc nợ":
                            if (string.IsNullOrEmpty(customerEmail))
                            {
                                XtraMessageBox.Show("Khách hàng không có email!", "Cảnh báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            string pdfPath = GenerateInvoicePDF(docId);
                            if (!string.IsNullOrEmpty(pdfPath))
                            {
                                var frm = new frmSendEmail(
                                    toEmail: customerEmail,
                                    pdfPath: pdfPath,
                                    subject: action == "Gửi Email Nhắc nợ"
                                        ? $"[NHẮC NỢ] Hóa đơn #{docId} quá hạn thanh toán"
                                        : $"Hóa đơn điện tử #{docId}",
                                    isReminder: action == "Gửi Email Nhắc nợ"
                                );
                                frm.ShowDialog();
                            }
                            break;

                        default:
                            XtraMessageBox.Show($"Chức năng '{action}' chưa được hỗ trợ cho Hóa đơn.", "Thông báo");
                            break;
                    }
                }
                // === CÁC ACTION CHO RECEIPT ===
                else if (docType == "Receipt")
                {
                    switch (action)
                    {
                        case "Xuất PDF":
                            ExportReceiptToPDF(docId);
                            break;

                        case "Gửi Email thanh toán thành công":
                        case "Gửi Email xác nhận":
                            if (string.IsNullOrEmpty(customerEmail))
                            {
                                XtraMessageBox.Show("Khách hàng không có email!", "Cảnh báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            string receiptPdfPath = GenerateReceiptPDF(docId);
                            if (!string.IsNullOrEmpty(receiptPdfPath))
                            {
                                string receiptNumber = row["DocumentNumber"]?.ToString() ?? docId.ToString();

                                // Load Receipt data để tạo email body đẹp hơn
                                DataTable dtReceipt = ReportHelper.LoadReceiptDataById(docId);
                                string emailBody = dtReceipt != null && dtReceipt.Rows.Count > 0
                                    ? EmailHelper.GetReceiptEmailBody(dtReceipt.Rows[0])
                                    : "";

                                var frm = new frmSendEmail(
                                    toEmail: customerEmail,
                                    pdfPath: receiptPdfPath,
                                    subject: $"[XÁC NHẬN] Phiếu thu #{receiptNumber} - Thanh toán thành công",
                                    isReminder: false,
                                    isReceipt: true  // Flag để load template Receipt
                                );

                                /*// Set email body nếu có
                                if (!string.IsNullOrEmpty(emailBody))
                                {
                                    frm.SetEmailBody(emailBody);
                                }*/

                                frm.ShowDialog();
                            }
                            break;
                    }
                }
                // === CÁC ACTION CHO REVENUE ===
                else if (docType == "Revenue")
                {
                    XtraMessageBox.Show($"Chức năng '{action}' chưa được hỗ trợ cho Bút toán ghi sổ.", "Thông báo");
                }
                else
                {
                    XtraMessageBox.Show($"Loại chứng từ '{docType}' không được hỗ trợ.", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi thực hiện thao tác:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========================================
        // HELPER METHODS CHO RECEIPT
        // ========================================

        /// <summary>
        /// Export Receipt to PDF
        /// </summary>
        private void ExportReceiptToPDF(long receiptId)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "PDF Files|*.pdf",
                    FileName = $"PhieuThu_{receiptId}.pdf"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    DataTable dtReceipt = ReportHelper.LoadReceiptDataById(receiptId);
                    DataTable dtReceiptDetail = ReportHelper.LoadReceiptDetailDataById(receiptId);

                    if (dtReceipt == null || dtReceipt.Rows.Count == 0)
                    {
                        XtraMessageBox.Show("Không tìm thấy phiếu thu!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    ReceiptReport report = new ReceiptReport();
                    ReportHelper.PopulateReceiptReport(report, dtReceipt, dtReceiptDetail);

                    report.CreateDocument();
                    report.ExportToPdf(sfd.FileName);

                    XtraMessageBox.Show("Xuất PDF thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi xuất PDF phiếu thu:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Generate Receipt PDF for Email (return temp file path)
        /// </summary>
        private string GenerateReceiptPDF(long receiptId)
        {
            try
            {
                // Create temp file
                string tempPath = Path.Combine(Path.GetTempPath(), $"Receipt_{receiptId}_{DateTime.Now:yyyyMMddHHmmss}.pdf");

                DataTable dtReceipt = ReportHelper.LoadReceiptDataById(receiptId);
                DataTable dtReceiptDetail = ReportHelper.LoadReceiptDetailDataById(receiptId);

                if (dtReceipt == null || dtReceipt.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không tìm thấy phiếu thu!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                ReceiptReport report = new ReceiptReport();
                ReportHelper.PopulateReceiptReport(report, dtReceipt, dtReceiptDetail);

                report.CreateDocument();
                report.ExportToPdf(tempPath);

                return tempPath;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi tạo PDF phiếu thu:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

     
        /// <summary>
        /// Add menu item to popup
        /// </summary>
        private void AddMenuItem(DevExpress.XtraBars.PopupMenu popupMenu,
            DevExpress.XtraBars.BarManager barManager,
            string caption,
            int rowHandle)
        {
            var item = new DevExpress.XtraBars.BarButtonItem(barManager, caption);
            item.ItemClick += (s, e) =>
            {
                ExecuteAction(rowHandle, caption);
            };
            popupMenu.AddItem(item);
        }

        private void ViewReceiptDetail(long invoiceId)
        {
            using (frmChungTu frm = new frmChungTu(invoiceId))
            {
                frm.ShowDialog();
            }
        }


        private void ViewInvoiceDetail(long invoiceId)
        {
            using (frmChungTu frm = new frmChungTu(invoiceId))
            {
                frm.ShowDialog();
            }
        }

        private void SendEmailToCustomer(long invoiceId, string customerEmail)
        {
            try
            {
                string pdfPath = GenerateInvoicePDF(invoiceId);

                if (string.IsNullOrEmpty(pdfPath)) return;

                // FIX LỖI: Truyền đủ 4 tham số, isReminder = false
                using (frmSendEmail emailForm = new frmSendEmail(customerEmail, pdfPath, $"Hóa đơn #{invoiceId} đã sẵn sàng", false))
                {
                    emailForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi gửi email: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sửa hàm SendReminderEmail
        private void SendReminderEmail(long invoiceId, string customerEmail)
        {
            try
            {
                string pdfPath = GenerateInvoicePDF(invoiceId);

                // FIX LỖI: Truyền đủ 4 tham số, isReminder = true
                using (frmSendEmail emailForm = new frmSendEmail(
                    customerEmail,
                    pdfPath,
                    $"Nhắc nhở thanh toán - Hóa đơn #{invoiceId}",
                    isReminder: true)) // Sử dụng isReminder: true
                {
                    emailForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi gửi email nhắc nợ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportInvoiceToPDF(long invoiceId)
        {
            try
            {
                string pdfPath = GenerateInvoicePDF(invoiceId);

                if (!string.IsNullOrEmpty(pdfPath))
                {
                    XtraMessageBox.Show($"Đã xuất PDF thành công!\n{pdfPath}", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Open file
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = pdfPath,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi xuất PDF: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

     
        private string GetInvoiceFolder()
        {
            string rootPath = @"C:\Users\trann\Downloads\Invoices";   

            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            string todayFolder = Path.Combine(rootPath, DateTime.Today.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(todayFolder))
                Directory.CreateDirectory(todayFolder);

            return todayFolder;
        }

        private string GenerateInvoicePDF(long invoiceId)
        {
            try
            {
                string folder = GetInvoiceFolder();
                string fileName = $"Invoice_{invoiceId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string fullPath = Path.Combine(folder, fileName);

                var dtHeader = DataHelper.LoadInvoiceDataById(invoiceId);
                var dtDetail = DataHelper.LoadInvoiceDetailDataById(invoiceId);

                if (dtHeader.Rows.Count == 0) return null;

                using (var report = new InvoiceReport())
                {
                    ReportHelper.PopulateInvoiceReport(report, dtHeader, dtDetail);
                    report.ExportToPdf(fullPath);
                }

                return File.Exists(fullPath) ? fullPath : null;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi tạo PDF: " + ex.Message);
                return null;
            }
        }
        private void OpenAccountingForm(long invoiceId)
        {
            using (frmHachToan frm = new frmHachToan())
            {
                frm.ShowDialog();
            }
            LoadDataAsync();
        }

        #endregion

        #region Filter Events

        private void BtnOpenFilter_Click(object sender, EventArgs e)
        {
            flyoutFilter.ShowPopup();
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            flyoutFilter.HidePopup();
            LoadDataAsync();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            lupEDocumentType.EditValue = "All";
            lupEDocumentStatus.EditValue = "All";
            lupEPaymentStatus.EditValue = "All";
            DEfromDate.EditValue = DateTime.Now.AddMonths(-1);
            DEtoDate.EditValue = DateTime.Now;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            flyoutFilter.HidePopup();
        }

        #endregion

        #region Toolbar Events

        private void BtnReload_Click(object sender, EventArgs e)
        {
            LoadDataAsync();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (frmHachToan frm = new frmHachToan())
            {
                frm.ShowDialog();
            }
            LoadDataAsync();
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.xlsx";
                saveDialog.FileName = $"DanhSachHoaDon_{DateTime.Now:yyyyMMdd}.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    gvDocument.ExportToXlsx(saveDialog.FileName);
                    XtraMessageBox.Show("Xuất Excel thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi xuất Excel: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "PDF Files|*.pdf";
                saveDialog.FileName = $"DanhSachHoaDon_{DateTime.Now:yyyyMMdd}.pdf";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    gvDocument.ExportToPdf(saveDialog.FileName);
                    XtraMessageBox.Show("Xuất PDF thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi xuất PDF: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}