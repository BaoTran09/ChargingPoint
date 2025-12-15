using ChargingApp.Helpers;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;

namespace ChargingApp.Accounting
{
    public partial class frmGeneralJournal : DevExpress.XtraEditors.XtraForm
    {
        private string reportId;
        private string currentReportId;
/*
        public frmGeneralJournal(string reportId)
            {            
                this.ControlBox = false;
                this.AutoScroll = true;
                InitializeComponent();
                this.reportId = reportId;
                LoadJournalData();
                LoadReportInfo();
                ConfigureGridView();
            }
        */
        // Constructor không tham số (gọi từ Form1)
        public frmGeneralJournal()
        {
            InitializeComponent();
            ConfigureGridView();
            this.ControlBox = false;
            this.WindowState = FormWindowState.Maximized;  // full màn 
            this.Dock = DockStyle.Fill;


        }
        // Constructor có tham số (gọi từ frmReportInput)
        public frmGeneralJournal(string reportId) : this()
        {
            this.currentReportId = reportId;
            LoadReportData(reportId);
           this.ControlBox = false;


        }
        
        // THÊM method mới để show input dialog
        public void ShowReportInputDialog()
        {
            frmReportInput inputForm = new frmReportInput(this);
            inputForm.ShowDialog();
        }

        // THÊM method để load dữ liệu từ reportId
        public void LoadReportData(string reportId)
        {
            this.currentReportId = reportId;
            LoadReportInfo();
            LoadJournalData();
        }

        // SỬA lại LoadReportInfo và LoadJournalData để dùng currentReportId
        private void LoadReportInfo()
        {
            if (string.IsNullOrEmpty(currentReportId))
            {
                labelControl1.Text = "SỔ NHẬT KÝ CHUNG";
                lbPeriod.Text = "Vui lòng chọn điều kiện báo cáo";
                return;
            }

            try
            {
                string query = @"SELECT r.Period, r.FromDate, r.ToDate, r.CreatedBy, 
                       rt.ReportTypeName
                       FROM Report r
                       INNER JOIN ReportType rt ON r.ReportTypeCode = rt.ReportTypeCode
                       WHERE r.Id = @ReportId";

                SqlParameter[] parameters = {
            new SqlParameter("@ReportId", currentReportId)
        };

                DataTable dt = DataHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    string period = row["Period"].ToString();
                    DateTime fromDate = Convert.ToDateTime(row["FromDate"]);
                    DateTime toDate = Convert.ToDateTime(row["ToDate"]);
                    string createdBy = row["CreatedBy"].ToString();
                    string reportName = row["ReportTypeName"].ToString();

                    labelControl1.Text = $"SỔ NHẬT KÝ CHUNG - {reportName}";
                    lbPeriod.Text = $"Kỳ: {period} (Từ {fromDate:dd/MM/yyyy} đến {toDate:dd/MM/yyyy}) - Người lập: {createdBy}";
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi tải thông tin báo cáo: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadJournalData()
        {
            if (string.IsNullOrEmpty(currentReportId))
            {
                gcGeneralJournal.DataSource = null;
                return;
            }

            try
            {
                string query = @"SELECT 
                       STT, ReportId, RecordDate, DocumentDate,
                       DocumentNumber, Interpretation, InvoiceId,
                       ReceiptId, RecordId, RecordDetail_STT,
                       Account, CorrespondingAccount,
                       DebitAmount, CrebitAmount
                       FROM JournalEntries 
                       WHERE ReportId = @ReportId 
                       ORDER BY STT";

                SqlParameter[] parameters = {
            new SqlParameter("@ReportId", currentReportId)
        };

                DataTable dt = DataHelper.ExecuteQuery(query, parameters);
                gcGeneralJournal.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu bút toán cho kỳ này!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // THÊM button "Chọn lại điều kiện"
        private void btnSelectAgain_Click(object sender, EventArgs e)
        {
            ShowReportInputDialog();
        }
        private void ConfigureGridView()
            {

            gcGeneralJournal.Enabled = true;

            
            // Bật thanh cuộn ngang
            gvGeneralJournal.OptionsView.ColumnAutoWidth = false;
            gvGeneralJournal.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;

            // Tự động điều chỉnh chiều rộng cột
            gvGeneralJournal.BestFitColumns();
            // Format columns
            if (gvGeneralJournal.Columns["STT"] != null)
                {
                    gvGeneralJournal.Columns["STT"].Caption = "STT";
                    gvGeneralJournal.Columns["STT"].Width = 50;
                }
                if (gvGeneralJournal.Columns["RecordDate"] != null)
                {
                    gvGeneralJournal.Columns["RecordDate"].Caption = "Ngày ghi sổ";
                    gvGeneralJournal.Columns["RecordDate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    gvGeneralJournal.Columns["RecordDate"].DisplayFormat.FormatString = "dd/MM/yyyy";
                    gvGeneralJournal.Columns["RecordDate"].Width = 100;
                }
                if (gvGeneralJournal.Columns["DocumentDate"] != null)
                {
                    gvGeneralJournal.Columns["DocumentDate"].Caption = "Ngày chứng từ";
                    gvGeneralJournal.Columns["DocumentDate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    gvGeneralJournal.Columns["DocumentDate"].DisplayFormat.FormatString = "dd/MM/yyyy";
                    gvGeneralJournal.Columns["DocumentDate"].Width = 100;
                }
                if (gvGeneralJournal.Columns["DocumentNumber"] != null)
                {
                    gvGeneralJournal.Columns["DocumentNumber"].Caption = "Số chứng từ";
                    gvGeneralJournal.Columns["DocumentNumber"].Width = 120;
                }
                if (gvGeneralJournal.Columns["Interpretation"] != null)
                {
                    gvGeneralJournal.Columns["Interpretation"].Caption = "Diễn giải";
                    gvGeneralJournal.Columns["Interpretation"].Width = 300;
                }
                if (gvGeneralJournal.Columns["Account"] != null)
                {
                    gvGeneralJournal.Columns["Account"].Caption = "TK Nợ";
                    gvGeneralJournal.Columns["Account"].Width = 80;
                }
                if (gvGeneralJournal.Columns["CorrespondingAccount"] != null)
                {
                    gvGeneralJournal.Columns["CorrespondingAccount"].Caption = "TK Có";
                    gvGeneralJournal.Columns["CorrespondingAccount"].Width = 80;
                }
                if (gvGeneralJournal.Columns["DebitAmount"] != null)
                {
                    gvGeneralJournal.Columns["DebitAmount"].Caption = "Số tiền Nợ";
                    gvGeneralJournal.Columns["DebitAmount"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    gvGeneralJournal.Columns["DebitAmount"].DisplayFormat.FormatString = "n2";
                    gvGeneralJournal.Columns["DebitAmount"].Width = 120;
                    gvGeneralJournal.Columns["DebitAmount"].Summary.Clear();
                    gvGeneralJournal.Columns["DebitAmount"].Summary.Add(DevExpress.Data.SummaryItemType.Sum, "DebitAmount", "Tổng: {0:n2}");
                }
                if (gvGeneralJournal.Columns["CrebitAmount"] != null)
                {
                    gvGeneralJournal.Columns["CrebitAmount"].Caption = "Số tiền Có";
                    gvGeneralJournal.Columns["CrebitAmount"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    gvGeneralJournal.Columns["CrebitAmount"].DisplayFormat.FormatString = "n2";
                    gvGeneralJournal.Columns["CrebitAmount"].Width = 120;
                    gvGeneralJournal.Columns["CrebitAmount"].Summary.Clear();
                    gvGeneralJournal.Columns["CrebitAmount"].Summary.Add(DevExpress.Data.SummaryItemType.Sum, "CrebitAmount", "Tổng: {0:n2}");
                }

                // Hide ID columns
                if (gvGeneralJournal.Columns["ReportId"] != null)
                    gvGeneralJournal.Columns["ReportId"].Visible = false;
                if (gvGeneralJournal.Columns["InvoiceId"] != null)
                    gvGeneralJournal.Columns["InvoiceId"].Visible = false;
                if (gvGeneralJournal.Columns["ReceiptId"] != null)
                    gvGeneralJournal.Columns["ReceiptId"].Visible = false;
                if (gvGeneralJournal.Columns["RecordId"] != null)
                    gvGeneralJournal.Columns["RecordId"].Visible = false;
                if (gvGeneralJournal.Columns["RecordDetail_STT"] != null)
                    gvGeneralJournal.Columns["RecordDetail_STT"].Visible = false;

                // Enable footer to show totals
                gvGeneralJournal.OptionsView.ShowFooter = true;

                // Enable best fit
                gvGeneralJournal.BestFitColumns();
            }

           
            private void btnSave_Click(object sender, EventArgs e)
            {
                try
                {
                    gvGeneralJournal.CloseEditor();
                    gvGeneralJournal.UpdateCurrentRow();

                    DataTable dt = (DataTable)gcGeneralJournal.DataSource;

                    int updatedCount = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row.RowState == DataRowState.Modified)
                        {
                            string updateQuery = @"UPDATE JournalEntries 
                                             SET Interpretation = @Interpretation,
                                                 DebitAmount = @DebitAmount,
                                                 CrebitAmount = @CrebitAmount
                                             WHERE STT = @STT AND ReportId = @ReportId";

                            SqlParameter[] parameters = {
                            new SqlParameter("@STT", row["STT"]),
                            new SqlParameter("@ReportId", reportId),
                            new SqlParameter("@Interpretation", row["Interpretation"]),
                            new SqlParameter("@DebitAmount", row["DebitAmount"]),
                            new SqlParameter("@CrebitAmount", row["CrebitAmount"])
                        };

                            DataHelper.ExecuteNonQuery(updateQuery, parameters);
                            updatedCount++;
                        }
                    }

                    dt.AcceptChanges();

                    if (updatedCount > 0)
                    {
                        XtraMessageBox.Show($"Đã lưu {updatedCount} bút toán thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        XtraMessageBox.Show("Không có thay đổi nào để lưu!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Lỗi lưu dữ liệu: {ex.Message}",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void btnExportExcel_Click(object sender, EventArgs e)
            {
                try
                {
                    SaveFileDialog saveDialog = new SaveFileDialog
                    {
                        Filter = "Excel Files|*.xlsx",
                        FileName = $"SoNhatKyChung_{reportId}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                        DefaultExt = "xlsx"
                    };

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        gvGeneralJournal.OptionsPrint.PrintFooter = true;
                        gcGeneralJournal.ExportToXlsx(saveDialog.FileName);

                        DialogResult result = XtraMessageBox.Show(
                            "Xuất Excel thành công! Bạn có muốn mở file không?",
                            "Thông báo",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (result == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveDialog.FileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Lỗi xuất Excel: {ex.Message}",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void btnExportPdf_Click(object sender, EventArgs e)
            {
                try
                {
                    SaveFileDialog saveDialog = new SaveFileDialog
                    {
                        Filter = "PDF Files|*.pdf",
                        FileName = $"SoNhatKyChung_{reportId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                        DefaultExt = "pdf"
                    };

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        gvGeneralJournal.OptionsPrint.PrintFooter = true;
                        gcGeneralJournal.ExportToPdf(saveDialog.FileName);

                        DialogResult result = XtraMessageBox.Show(
                            "Xuất PDF thành công! Bạn có muốn mở file không?",
                            "Thông báo",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (result == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveDialog.FileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Lỗi xuất PDF: {ex.Message}",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void frmGeneralJournal_Load(object sender, EventArgs e)
            {
                // Optional: Add any initialization code here
            }

            private void btnRefresh_Click(object sender, EventArgs e)
            {
                LoadJournalData();
                XtraMessageBox.Show("Đã tải lại dữ liệu!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        private void frmGeneralJournal_Load_1(object sender, EventArgs e)
        {
            
        }
    }

}

