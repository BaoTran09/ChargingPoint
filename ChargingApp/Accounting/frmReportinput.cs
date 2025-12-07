using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ChargingApp.Accounting;
using ChargingApp.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Wizards;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using ChargingApp.Management;

namespace ChargingApp.Accounting
{
    public partial class frmReportInput : XtraForm
    {
        private frmGeneralJournal parentJournalForm;

        //  constructor
        public frmReportInput(frmGeneralJournal parentForm)
        {
            InitializeComponent();
            this.parentJournalForm = parentForm;
            InitializeGridColumns();
            LoadInitialData();
            RegisterEvents();
        }


        private void InitializeGridColumns()
        {
            // Clear existing columns
            gvReportType.Columns.Clear();

            // Add checkbox column for selection
            GridColumn colSelect = new GridColumn
            {
                Caption = "Chọn",
                FieldName = "Selected",
                UnboundType = DevExpress.Data.UnboundColumnType.Boolean,
                Visible = true,
                Width = 60
            };
            gvReportType.Columns.Add(colSelect);

            // Add ReportTypeCode column
            GridColumn colCode = new GridColumn
            {
                Caption = "Mã báo cáo",
                FieldName = "ReportTypeCode",
                Visible = true,
                Width = 120
            };
            gvReportType.Columns.Add(colCode);

            // Add ReportTypeName column
            GridColumn colName = new GridColumn
            {
                Caption = "Tên báo cáo",
                FieldName = "ReportTypeName",
                Visible = true,
                Width = 300
            };
            gvReportType.Columns.Add(colName);

            // Add EntryType column (hidden, for reference)
            GridColumn colEntryType = new GridColumn
            {
                Caption = "Loại bút toán",
                FieldName = "EntryType",
                Visible = false
            };
            gvReportType.Columns.Add(colEntryType);

            // Configure GridView options
            gvReportType.OptionsView.ShowGroupPanel = false;
            gvReportType.OptionsSelection.MultiSelect = false;
            gvReportType.OptionsSelection.EnableAppearanceFocusedCell = false;
            gvReportType.OptionsBehavior.Editable = true;
            gvReportType.OptionsView.ColumnAutoWidth = false;
        }

        private void RegisterEvents()
        {
            // Event when year changes
            cbYear.SelectedIndexChanged += cbYear_SelectedIndexChanged;

            // Event when period changes
            cbPeriod.SelectedIndexChanged += cbPeriod_SelectedIndexChanged;

            // Event when date edit changes
            DEFromDate.EditValueChanged += DEFromDate_EditValueChanged;
            DEToDate.EditValueChanged += DEToDate_EditValueChanged;
        }

        private void LoadInitialData()
        {
            // Load years (current year and previous 5 years)
            cbYear.Properties.Items.Clear();
            int currentYear = DateTime.Now.Year;
            for (int i = 0; i <= 5; i++)
            {
                cbYear.Properties.Items.Add(currentYear - i);
            }
            cbYear.SelectedIndex = 0;

            // Load periods
            cbPeriod.Properties.Items.Clear();
            cbPeriod.Properties.Items.AddRange(new string[] {
                "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6",
                "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12",
                "Quý 1", "Quý 2", "Quý 3", "Quý 4",
                "Từ đầu năm đến hiện tại",
                "Tùy chỉnh"
            });
            cbPeriod.SelectedIndex = 0;

            // Load ReportTypes into GridView
            LoadReportTypes();

            // Set default dates
            DEFromDate.EditValue = new DateTime(DateTime.Now.Year, 1, 1);
            DEToDate.EditValue = DateTime.Now;
        }

        private void LoadReportTypes()
        {
            try
            {
                string query = @"SELECT ReportTypeCode, ReportTypeName, EntryType 
                               FROM ReportType 
                               WHERE ReportCategory = 'JOURNAL'
                               ORDER BY ReportTypeCode";

                DataTable dt = DataHelper.ExecuteQuery(query);

                // Add Selected column
                dt.Columns.Add("Selected", typeof(bool));

                // Set all to unselected initially
                foreach (DataRow row in dt.Rows)
                {
                    row["Selected"] = false;
                }

                gcReportType.DataSource = dt;

                // Make only Selected column editable
                foreach (GridColumn col in gvReportType.Columns)
                {
                    if (col.FieldName != "Selected")
                    {
                        col.OptionsColumn.AllowEdit = false;
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi tải danh sách báo cáo: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update dates when year changes
            UpdateDatesFromPeriod();
        }

        private void cbPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update dates when period changes
            UpdateDatesFromPeriod();
        }

        private void UpdateDatesFromPeriod()
        {
            if (cbYear.SelectedIndex < 0 || cbPeriod.SelectedIndex < 0)
                return;

            string selectedPeriod = cbPeriod.Text;
            int year = Convert.ToInt32(cbYear.Text);

            // Temporarily remove event handlers to avoid recursive calls
            DEFromDate.EditValueChanged -= DEFromDate_EditValueChanged;
            DEToDate.EditValueChanged -= DEToDate_EditValueChanged;

            try
            {
                if (selectedPeriod.StartsWith("Tháng"))
                {
                    // Extract month number
                    int month = int.Parse(selectedPeriod.Replace("Tháng ", ""));
                    DEFromDate.EditValue = new DateTime(year, month, 1);
                    DEToDate.EditValue = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                    // Disable date edits
                    DEFromDate.Properties.ReadOnly = true;
                    DEToDate.Properties.ReadOnly = true;
                }
                else if (selectedPeriod.StartsWith("Quý"))
                {
                    // Extract quarter number
                    int quarter = int.Parse(selectedPeriod.Replace("Quý ", ""));
                    int startMonth = (quarter - 1) * 3 + 1;
                    int endMonth = quarter * 3;

                    DEFromDate.EditValue = new DateTime(year, startMonth, 1);
                    DEToDate.EditValue = new DateTime(year, endMonth, DateTime.DaysInMonth(year, endMonth));

                    // Disable date edits
                    DEFromDate.Properties.ReadOnly = true;
                    DEToDate.Properties.ReadOnly = true;
                }
                else if (selectedPeriod == "Từ đầu năm đến hiện tại")
                {
                    DEFromDate.EditValue = new DateTime(year, 1, 1);
                    DEToDate.EditValue = new DateTime(year, DateTime.Now.Month, DateTime.Now.Day);

                    // Disable date edits
                    DEFromDate.Properties.ReadOnly = true;
                    DEToDate.Properties.ReadOnly = true;
                }
                else if (selectedPeriod == "Tùy chỉnh")
                {
                    // Enable date edits for custom range
                    DEFromDate.Properties.ReadOnly = false;
                    DEToDate.Properties.ReadOnly = false;
                }
            }
            finally
            {
                // Re-attach event handlers
                DEFromDate.EditValueChanged += DEFromDate_EditValueChanged;
                DEToDate.EditValueChanged += DEToDate_EditValueChanged;
            }
        }

        private void DEFromDate_EditValueChanged(object sender, EventArgs e)
        {
            // If user manually changes date, switch to custom period
            if (!DEFromDate.Properties.ReadOnly && cbPeriod.Text != "Tùy chỉnh")
            {
                cbPeriod.SelectedIndexChanged -= cbPeriod_SelectedIndexChanged;
                cbPeriod.Text = "Tùy chỉnh";
                cbPeriod.SelectedIndexChanged += cbPeriod_SelectedIndexChanged;
            }
        }

        private void DEToDate_EditValueChanged(object sender, EventArgs e)
        {
            // If user manually changes date, switch to custom period
            if (!DEToDate.Properties.ReadOnly && cbPeriod.Text != "Tùy chỉnh")
            {
                cbPeriod.SelectedIndexChanged -= cbPeriod_SelectedIndexChanged;
                cbPeriod.Text = "Tùy chỉnh";
                cbPeriod.SelectedIndexChanged += cbPeriod_SelectedIndexChanged;
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate: Check if any report is selected
                DataTable dt = (DataTable)gcReportType.DataSource;
                DataRow selectedRow = null;

                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToBoolean(row["Selected"]))
                    {
                        selectedRow = row;
                        break;
                    }
                }

                if (selectedRow == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn loại báo cáo!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate dates
                if (DEFromDate.EditValue == null || DEToDate.EditValue == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn kỳ báo cáo!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DateTime fromDate = Convert.ToDateTime(DEFromDate.EditValue);
                DateTime toDate = Convert.ToDateTime(DEToDate.EditValue);

                if (fromDate > toDate)
                {
                    XtraMessageBox.Show("Ngày bắt đầu phải nhỏ hơn ngày kết thúc!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string period = cbPeriod.Text;
                bool isEntryTypeSummary = ckEntryType.Checked;
                string createdBy = textEdit1.Text;

                if (string.IsNullOrWhiteSpace(createdBy))
                {
                    XtraMessageBox.Show("Vui lòng nhập người lập báo cáo!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get selected report type
                string reportTypeCode = selectedRow["ReportTypeCode"].ToString();
                string entryType = isEntryTypeSummary ? "SUMMARY" : "DETAIL";


                // Create Report record
                string reportId = CreateReport(reportTypeCode, period, fromDate, toDate, createdBy, entryType);

                if (!string.IsNullOrEmpty(reportId))
                {
                    // Generate Journal Entries
                    GenerateJournalEntries(reportId, fromDate, toDate, entryType);

                    XtraMessageBox.Show("Tạo báo cáo thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Load dữ liệu vào parent form
                    if (parentJournalForm != null)
                    {
                        parentJournalForm.LoadReportData(reportId);
                    }

                    // Đóng form input
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Xóa bỏ việc tạo Form mới, chỉ tập trung giải phóng tài nguyên
        private void frmGeneralJournal_FormClosed(object sender, FormClosedEventArgs e)
        {
            (sender as Form)?.Dispose();

            
        }
        /*
        private string CreateReport(string reportTypeCode, string period, DateTime fromDate,
            DateTime toDate, string createdBy, string entryType)
        {
            string reportId = $"{reportTypeCode}_{fromDate.Year}{fromDate.Month:00}";

            try
            {
                // Check if report already exists
                string checkQuery = "SELECT COUNT(*) FROM Report WHERE Id = @Id";
                SqlParameter[] checkParams = {
                    new SqlParameter("@Id", reportId)
                };

                int count = Convert.ToInt32(DataHelper.ExecuteScalar(checkQuery, checkParams));

                if (count > 0)
                {
                    DialogResult result = XtraMessageBox.Show(
                        "Báo cáo này đã tồn tại. Bạn có muốn tạo lại không?",
                        "Xác nhận",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Delete existing report and journal entries
                        string deleteJournal = "DELETE FROM JournalEntries WHERE ReportId = @ReportId";
                        string deleteReport = "DELETE FROM Report WHERE Id = @Id";

                        DataHelper.ExecuteNonQuery(deleteJournal, new SqlParameter("@ReportId", reportId));
                        DataHelper.ExecuteNonQuery(deleteReport, new SqlParameter("@Id", reportId));
                    }
                    else
                    {
                        return null;
                    }
                }

                string query = @"INSERT INTO Report (Id, ReportTypeCode, Period, FromDate, ToDate, 
                               CreatedBy, CreatedDate, IsClosed) 
                               VALUES (@Id, @ReportTypeCode, @Period, @FromDate, @ToDate, 
                               @CreatedBy, GETDATE(), 0)";

                SqlParameter[] parameters = {
                    new SqlParameter("@Id", reportId),
                    new SqlParameter("@ReportTypeCode", reportTypeCode),
                    new SqlParameter("@Period", period),
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate),
                    new SqlParameter("@CreatedBy", createdBy)
                };

                DataHelper.ExecuteNonQuery(query, parameters);
                return reportId;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi tạo báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }*/
        private string CreateReport(string reportTypeCode, string period, DateTime fromDate, DateTime toDate, string createdBy, string entryType)
        {
            try
            {
                // 1. Xác định prefix theo kỳ
                string prefix;
                if (period.StartsWith("Quý"))
                {
                    int quarter = int.Parse(period.Replace("Quý ", ""));
                    prefix = $"{reportTypeCode}{fromDate.Year}Q{quarter}";
                }
                else if (period.StartsWith("Tháng"))
                {
                    int month = int.Parse(period.Replace("Tháng ", ""));
                    prefix = $"{reportTypeCode}{fromDate.Year}{month:00}";
                }
                else if (period == "Từ đầu năm đến hiện tại")
                {
                    prefix = $"{reportTypeCode}{fromDate.Year}YTD";
                }
                else // Tùy chỉnh hoặc các kỳ khác
                {
                    prefix = $"{reportTypeCode}{fromDate:yyyyMMdd}-{toDate:yyyyMMdd}";
                }

                // 2. Tìm Version lớn nhất hiện có với prefix này
                string findMaxVersionQuery = @"
            SELECT ISNULL(MAX(CAST(SUBSTRING(Id, LEN(@Prefix) + 1, LEN(Id)) AS INT)), 0) + 1
            FROM Report 
            WHERE Id LIKE @Prefix + '%' 
            AND ISNUMERIC(SUBSTRING(Id, LEN(@Prefix) + 1, LEN(Id))) = 1";

                SqlParameter[] p = { new SqlParameter("@Prefix", prefix) };
                int nextVersion = Convert.ToInt32(DataHelper.ExecuteScalar(findMaxVersionQuery, p));

                // 3. Tạo ID cuối cùng
                string finalReportId = $"{prefix}{nextVersion}"; // Ví dụ: BC01_2025032  hoặc BC01_2025Q12

                // 4. Kiểm tra xem ID này đã tồn tại chưa (phòng trường hợp race condition)
                string checkQuery = "SELECT COUNT(*) FROM Report WHERE Id = @Id";
                int count = Convert.ToInt32(DataHelper.ExecuteScalar(checkQuery, new SqlParameter("@Id", finalReportId)));

                if (count > 0)
                {
                    // Nếu có rồi (hiếm), tăng version thêm 1
                    nextVersion++;
                    finalReportId = $"{prefix}{nextVersion}";
                }

                // 5. Insert báo cáo mới với Version
                string insertQuery = @"
            INSERT INTO Report (Id, ReportTypeCode, Period, FromDate, ToDate, CreatedBy, CreatedDate, IsClosed, Version)
            VALUES (@Id, @ReportTypeCode, @Period, @FromDate, @ToDate, @CreatedBy, GETDATE(), 0, @Version)";

                SqlParameter[] parameters =
                {
            new SqlParameter("@Id", finalReportId),
            new SqlParameter("@ReportTypeCode", reportTypeCode),
            new SqlParameter("@Period", period),
            new SqlParameter("@FromDate", fromDate),
            new SqlParameter("@ToDate", toDate),
            new SqlParameter("@CreatedBy", createdBy),
            new SqlParameter("@Version", nextVersion)
        };

                DataHelper.ExecuteNonQuery(insertQuery, parameters);

                XtraMessageBox.Show($"Tạo báo cáo thành công!\nMã báo cáo: {finalReportId}", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                return finalReportId;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi tạo báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        private void GenerateJournalEntries(string reportId, DateTime fromDate,
            DateTime toDate, string entryType)
        {
            try
            {



                if (entryType == "SUMMARY")
                {
                    // Gộp bút toán - Group by InvoiceId or ReceiptId
                    string query = @"
                        INSERT INTO JournalEntries (STT, ReportId, RecordDate, DocumentDate, 
                            DocumentNumber, Interpretation, InvoiceId, ReceiptId, RecordId, 
                            RecordDetail_STT, Account, CorrespondingAccount, DebitAmount, CrebitAmount)
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY rr.RecordDate) as STT,
                            @ReportId,
                            rr.RecordDate,
                            rr.Documentdate,
                            CASE 
                            WHEN rr.Invoiceid IS NOT NULL THEN CAST(i.InvoiceNumber AS NVARCHAR(50))
                            WHEN rr.ReceiptId IS NOT NULL THEN CAST(r.ReceiptId AS NVARCHAR(50))
                             ELSE NULL 
                             END as DocumentNumber,

                            CASE 
                                WHEN rr.Invoiceid IS NOT NULL THEN N'Ghi nhận công nợ tổng hợp hóa đơn ' + CAST(i.InvoiceNumber AS NVARCHAR(50))
                                WHEN rr.ReceiptId IS NOT NULL THEN N'Thu tiền khách hàng theo phiếu ' + CAST(r.ReceiptId AS NVARCHAR)
                                ELSE N'Ghi nhận giao dịch'
                            END as Interpretation,
                            rr.Invoiceid,
                            rr.ReceiptId,
                            rr.Id,
                            NULL as RecordDetail_STT,
                            rd.Debit_account,
                            rd.Credit_account,
                            SUM(rd.Amount + ISNULL(rd.TaxAmount, 0) - ISNULL(rd.DiscountAmount, 0)) as DebitAmount,
                            SUM(rd.Amount + ISNULL(rd.TaxAmount, 0) - ISNULL(rd.DiscountAmount, 0)) as CrebitAmount
                        FROM Revenue_recognition rr
                        INNER JOIN Record_detail rd ON rr.Id = rd.Recognition_id
                        LEFT JOIN Invoices i ON rr.Invoiceid = i.Invoiceid
                        LEFT JOIN Receipt r ON rr.ReceiptId = r.ReceiptId
                        WHERE rr.RecordDate BETWEEN @FromDate AND @ToDate
                        GROUP BY rr.Id, rr.RecordDate, rr.Documentdate, rr.Invoiceid, rr.ReceiptId,
                                 i.InvoiceNumber, r.ReceiptId, rd.Debit_account, rd.Credit_account";

                    SqlParameter[] parameters = {
                        new SqlParameter("@ReportId", reportId),
                        new SqlParameter("@FromDate", fromDate),
                        new SqlParameter("@ToDate", toDate)
                    };

                    DataHelper.ExecuteNonQuery(query, parameters);
                }
                else
                {
                    // Chi tiết bút toán
                    string query = @"
                        INSERT INTO JournalEntries (STT, ReportId, RecordDate, DocumentDate, 
                            DocumentNumber, Interpretation, InvoiceId, ReceiptId, RecordId, 
                            RecordDetail_STT, Account, CorrespondingAccount, DebitAmount, CrebitAmount)
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY rr.RecordDate, rd.STT) as STT,
                            @ReportId,
                            rr.RecordDate,
                            rr.Documentdate,
                            CASE 
                            WHEN rr.Invoiceid IS NOT NULL THEN CAST(i.InvoiceNumber AS NVARCHAR(50))
                            WHEN rr.ReceiptId IS NOT NULL THEN CAST(r.ReceiptId AS NVARCHAR(50))
                            ELSE NULL 
                            END as DocumentNumber,
                            rd.Interpretation,
                            rr.Invoiceid,
                            rr.ReceiptId,
                            rr.Id,
                            rd.STT,
                            rd.Debit_account,
                            rd.Credit_account,
                            rd.Amount + ISNULL(rd.TaxAmount, 0) - ISNULL(rd.DiscountAmount, 0) as DebitAmount,
                            rd.Amount + ISNULL(rd.TaxAmount, 0) - ISNULL(rd.DiscountAmount, 0) as CrebitAmount
                        FROM Revenue_recognition rr
                        INNER JOIN Record_detail rd ON rr.Id = rd.Recognition_id
                        LEFT JOIN Invoices i ON rr.Invoiceid = i.Invoiceid
                        LEFT JOIN Receipt r ON rr.ReceiptId = r.ReceiptId
                        WHERE rr.RecordDate BETWEEN @FromDate AND @ToDate
                        ORDER BY rr.RecordDate, rd.STT";

                    SqlParameter[] parameters = {
                        new SqlParameter("@ReportId", reportId),
                        new SqlParameter("@FromDate", fromDate),
                        new SqlParameter("@ToDate", toDate)
                    };

                    DataHelper.ExecuteNonQuery(query, parameters);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi tạo bút toán: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}