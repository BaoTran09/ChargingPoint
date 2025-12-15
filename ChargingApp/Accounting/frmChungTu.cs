using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ChargingApp.Report;
using ChargingApp.Helpers;
using DevExpress.XtraReports.UI;

namespace ChargingApp.Accounting
{
  /*  public partial class frmChungTu : DevExpress.XtraEditors.XtraForm
    {
        private long invoiceId;

        public frmChungTu()
        {
            InitializeComponent();
        }
 
        public frmChungTu(long invoiceId)
        {
            InitializeComponent();
            this.invoiceId = invoiceId;
        }

        private void frmChungTu_Load(object sender, EventArgs e)
        {
            if (invoiceId > 0)
            {
                LoadInvoiceNumberFromId(invoiceId);
                LoadInvoiceReport(invoiceId);
            }
        }
       
        private void LoadInvoiceNumberFromId(long invoiceId)
        {
            try
            {
                string query = "SELECT InvoiceNumber FROM Invoices WHERE InvoiceId = @InvoiceId";
                object result = DataHelper.ExecuteScalar(query,
                    new SqlParameter("@InvoiceId", invoiceId));

                if (result != null && txtInvocieNumber != null)
                {
                    txtInvocieNumber.Text = result.ToString();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi load invoice number: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnView_Click(object sender, EventArgs e)
        {
            if (txtInvocieNumber == null || string.IsNullOrWhiteSpace(txtInvocieNumber.Text))
            {
                XtraMessageBox.Show("Vui lòng nhập số hóa đơn!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "SELECT InvoiceId FROM Invoices WHERE InvoiceNumber = @InvoiceNumber";
                object result = DataHelper.ExecuteScalar(query,
                    new SqlParameter("@InvoiceNumber", txtInvocieNumber.Text.Trim()));

                if (result == null)
                {
                    XtraMessageBox.Show("Không tìm thấy hóa đơn!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                invoiceId = Convert.ToInt64(result);
                LoadInvoiceReport(invoiceId);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi tìm kiếm: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadInvoiceReport(long invoiceId)
        {
            try
            {
                // Load data using DataHelper
                DataTable dtInvoice = DataHelper.LoadInvoiceDataById(invoiceId);
                DataTable dtInvoiceDetail = DataHelper.LoadInvoiceDetailDataById(invoiceId);

                if (dtInvoice == null || dtInvoice.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không tìm thấy hóa đơn!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create and populate report using ReportHelper
                InvoiceReport report = new InvoiceReport();
                ReportHelper.PopulateInvoiceReport(report, dtInvoice, dtInvoiceDetail);

                // Display report
                report.CreateDocument();

                if (documentViewer1 != null)
                {
                    documentViewer1.DocumentSource = report;
                }
                else
                {
                    XtraMessageBox.Show("DocumentViewer chưa được khởi tạo!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi tải báo cáo:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/

        // Cập nhật frmChungTu - SIMPLIFIED VERSION (chỉ xử lý View/Export/Print)

        public partial class frmChungTu : DevExpress.XtraEditors.XtraForm
        {
            private long? currentInvoiceId = null;
            private long? currentReceiptId = null;
            private bool isReceipt = false; // Flag để phân biệt loại chứng từ

            // Constructor với tham số cho Invoice
            public frmChungTu(long invoiceId)
            {
                InitializeComponent();
                currentInvoiceId = invoiceId;
                isReceipt = false;
                LoadInvoiceNumberFromId(invoiceId);
            }

            // Constructor với tham số cho Receipt
            public frmChungTu(long receiptId, bool isReceiptType)
            {
                InitializeComponent();
                currentReceiptId = receiptId;
                isReceipt = isReceiptType;
                LoadReceiptNumberFromId(receiptId);
            }

            // Constructor mặc định (nhập số thủ công)
            public frmChungTu()
            {
                InitializeComponent();
            }

            /// <summary>
            /// Phát hiện loại chứng từ dựa vào prefix của số
            /// </summary>
            private bool DetectDocumentType(string documentNumber)
            {
                if (string.IsNullOrWhiteSpace(documentNumber))
                    return false;

                // Receipt number có prefix "PT" (Phiếu Thu)
                // Invoice number không có prefix hoặc có prefix khác
                return documentNumber.Trim().ToUpper().StartsWith("PT");
            }

            /// <summary>
            /// Button View - Hiển thị chứng từ
            /// </summary>
            private void BtnView_Click(object sender, EventArgs e)
            {
                if (txtInvocieNumber == null || string.IsNullOrWhiteSpace(txtInvocieNumber.Text))
                {
                    XtraMessageBox.Show("Vui lòng nhập số chứng từ!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    string documentNumber = txtInvocieNumber.Text.Trim();

                    // Phát hiện loại chứng từ
                    isReceipt = DetectDocumentType(documentNumber);

                    if (isReceipt)
                    {
                        // Load Receipt
                        long? receiptId =ReportHelper.GetReceiptIdByNumber(documentNumber);
                        if (receiptId == null)
                        {
                            XtraMessageBox.Show("Không tìm thấy phiếu thu!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        currentReceiptId = receiptId;
                        currentInvoiceId = null;
                        LoadReceiptReport(receiptId.Value);
                    }
                    else
                    {
                        // Load Invoice
                        string query = "SELECT InvoiceId FROM Invoices WHERE InvoiceNumber = @InvoiceNumber";
                        object result = DataHelper.ExecuteScalar(query,
                            new SqlParameter("@InvoiceNumber", documentNumber));

                        if (result == null)
                        {
                            XtraMessageBox.Show("Không tìm thấy hóa đơn!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        currentInvoiceId = Convert.ToInt64(result);
                        currentReceiptId = null;
                        LoadInvoiceReport(currentInvoiceId.Value);
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Lỗi tìm kiếm: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            /// <summary>
            /// Load Invoice Report
            /// </summary>
            private void LoadInvoiceReport(long invoiceId)
            {
                try
                {
                    DataTable dtInvoice = DataHelper.LoadInvoiceDataById(invoiceId);
                    DataTable dtInvoiceDetail = DataHelper.LoadInvoiceDetailDataById(invoiceId);

                    if (dtInvoice == null || dtInvoice.Rows.Count == 0)
                    {
                        XtraMessageBox.Show("Không tìm thấy hóa đơn!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    InvoiceReport report = new InvoiceReport();
                    ReportHelper.PopulateInvoiceReport(report, dtInvoice, dtInvoiceDetail);

                    report.CreateDocument();
                    if (documentViewer1 != null)
                    {
                        documentViewer1.DocumentSource = report;
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Lỗi tải báo cáo hóa đơn:\n{ex.Message}",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            /// <summary>
            /// Load Receipt Report
            /// </summary>
            private void LoadReceiptReport(long receiptId)
            {
                try
                {
                    DataTable dtReceipt = ReportHelper.LoadReceiptDataById(receiptId);
                    DataTable dtReceiptDetail = ReportHelper.LoadReceiptDetailDataById(receiptId);

                    if (dtReceipt == null || dtReceipt.Rows.Count == 0)
                    {
                        XtraMessageBox.Show("Không tìm thấy phiếu thu!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    ReceiptReport report = new ReceiptReport();
                ReportHelper.PopulateReceiptReport(report, dtReceipt, dtReceiptDetail);

                    report.CreateDocument();
                    if (documentViewer1 != null)
                    {
                        documentViewer1.DocumentSource = report;
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Lỗi tải báo cáo phiếu thu:\n{ex.Message}",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            /// <summary>
            /// Button Export PDF
            /// </summary>
            private void BtnExportPdf_Click(object sender, EventArgs e)
            {
                if (documentViewer1?.DocumentSource == null)
                {
                    XtraMessageBox.Show("Chưa có chứng từ để xuất!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    string defaultFileName = isReceipt
                        ? $"PhieuThu_{txtInvocieNumber.Text.Trim()}.pdf"
                        : $"HoaDon_{txtInvocieNumber.Text.Trim()}.pdf";

                    SaveFileDialog sfd = new SaveFileDialog
                    {
                        Filter = "PDF Files|*.pdf",
                        FileName = defaultFileName
                    };

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        if (isReceipt && currentReceiptId.HasValue)
                        {
                            ExportReceiptToPdf(currentReceiptId.Value, sfd.FileName);
                        }
                        else if (!isReceipt && currentInvoiceId.HasValue)
                        {
                            ExportInvoiceToPdf(currentInvoiceId.Value, sfd.FileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Lỗi xuất PDF: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            /// <summary>
            /// Button Export Excel
            /// </summary>
            private void BtnExportExcel_Click(object sender, EventArgs e)
            {
                if (documentViewer1?.DocumentSource == null)
                {
                    XtraMessageBox.Show("Chưa có chứng từ để xuất!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    string defaultFileName = isReceipt
                        ? $"PhieuThu_{txtInvocieNumber.Text.Trim()}.xlsx"
                        : $"HoaDon_{txtInvocieNumber.Text.Trim()}.xlsx";

                    SaveFileDialog sfd = new SaveFileDialog
                    {
                        Filter = "Excel Files|*.xlsx",
                        FileName = defaultFileName
                    };

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        XtraReport report = documentViewer1.DocumentSource as XtraReport;
                        if (report != null)
                        {
                            report.ExportToXlsx(sfd.FileName);
                            XtraMessageBox.Show("Xuất Excel thành công!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Lỗi xuất Excel: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            /// <summary>
            /// Button Print
            /// </summary>
            private void BtnPrint_Click(object sender, EventArgs e)
            {
                if (documentViewer1?.DocumentSource == null)
                {
                    XtraMessageBox.Show("Chưa có chứng từ để in!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    XtraReport report = documentViewer1.DocumentSource as XtraReport;
                    if (report != null)
                    {
                        report.Print();
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Lỗi in: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            /// <summary>
            /// Export Invoice to PDF
            /// </summary>
            private void ExportInvoiceToPdf(long invoiceId, string outputPath)
            {
                try
                {
                    DataTable dtInvoice = DataHelper.LoadInvoiceDataById(invoiceId);
                    DataTable dtInvoiceDetail = DataHelper.LoadInvoiceDetailDataById(invoiceId);

                    if (dtInvoice == null || dtInvoice.Rows.Count == 0)
                    {
                        throw new Exception("Không tìm thấy dữ liệu hóa đơn");
                    }

                    InvoiceReport report = new InvoiceReport();
                    ReportHelper.PopulateInvoiceReport(report, dtInvoice, dtInvoiceDetail);

                    report.CreateDocument();
                    report.ExportToPdf(outputPath);

                    XtraMessageBox.Show("Xuất PDF thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi xuất PDF hóa đơn: {ex.Message}", ex);
                }
            }

            /// <summary>
            /// Export Receipt to PDF
            /// </summary>
            private void ExportReceiptToPdf(long receiptId, string outputPath)
            {
                try
                {
                    DataTable dtReceipt = ReportHelper.LoadReceiptDataById(receiptId);
                    DataTable dtReceiptDetail = ReportHelper.LoadReceiptDetailDataById(receiptId);

                    if (dtReceipt == null || dtReceipt.Rows.Count == 0)
                    {
                        throw new Exception("Không tìm thấy dữ liệu phiếu thu");
                    }

                    ReceiptReport report = new ReceiptReport();
                    ReportHelper.PopulateReceiptReport(report, dtReceipt, dtReceiptDetail);

                    report.CreateDocument();
                    report.ExportToPdf(outputPath);

                    XtraMessageBox.Show("Xuất PDF thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi xuất PDF phiếu thu: {ex.Message}", ex);
                }
            }

            /// <summary>
            /// Load Invoice Number from Id (gọi từ form khác)
            /// </summary>
            private void LoadInvoiceNumberFromId(long invoiceId)
            {
                try
                {
                    string query = "SELECT InvoiceNumber FROM Invoices WHERE InvoiceId = @InvoiceId";
                    object result = DataHelper.ExecuteScalar(query,
                        new SqlParameter("@InvoiceId", invoiceId));

                    if (result != null && txtInvocieNumber != null)
                    {
                        txtInvocieNumber.Text = result.ToString();
                        LoadInvoiceReport(invoiceId);
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Lỗi load invoice: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            /// <summary>
            /// Load Receipt Number from Id (gọi từ form khác)
            /// </summary>
            private void LoadReceiptNumberFromId(long receiptId)
            {
                try
                {
                    string query = "SELECT ReceiptNumber FROM Receipt WHERE ReceiptId = @ReceiptId";
                    object result = DataHelper.ExecuteScalar(query,
                        new SqlParameter("@ReceiptId", receiptId));

                    if (result != null && txtInvocieNumber != null)
                    {
                        txtInvocieNumber.Text = result.ToString();
                        LoadReceiptReport(receiptId);
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Lỗi load receipt: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            // 2. Đóng Form
            this.Close();
        }




    }

        // ==========================================
        // CÁCH GỌI TỪ CÁC FORM KHÁC
        // ==========================================
/*
        // Từ form quản lý Invoice
        private void ViewInvoiceButton_Click(object sender, EventArgs e)
        {
            if (gridView1.GetFocusedRow() is DataRow row)
            {
                long invoiceId = Convert.ToInt64(row["InvoiceId"]);
                frmChungTu frm = new frmChungTu(invoiceId);
                frm.ShowDialog();
            }
        }

        // Từ form quản lý Receipt hoặc sau khi tạo Receipt
        private void ViewReceiptButton_Click(object sender, EventArgs e)
        {
            if (gridView1.GetFocusedRow() is DataRow row)
            {
                long receiptId = Convert.ToInt64(row["ReceiptId"]);
                frmChungTu frm = new frmChungTu(receiptId, true);
                frm.ShowDialog();
            }
        }

        // Sau khi tạo Receipt từ Payment
        private void AfterCreateReceipt(long receiptId)
        {
            frmChungTu frm = new frmChungTu(receiptId, true);
            frm.ShowDialog();
        }

    */

















    
    
}