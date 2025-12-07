using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ChargingApp.Report;
using ChargingApp.Helpers;

namespace ChargingApp.Accounting
{
    public partial class frmChungTu : DevExpress.XtraEditors.XtraForm
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
            }
        }
    }
}