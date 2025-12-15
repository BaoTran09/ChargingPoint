using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data.ExpressionEditor;
using DevExpress.RichEdit.Core.Accessibility;
using DevExpress.XtraEditors;
using DevExpress.XtraExport.Implementation;

namespace ChargingApp.Management
{
    public partial class frmCharger: DevExpress.XtraEditors.XtraForm
    {
        private string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=ChargingPoint;Integrated Security=True;Connect Timeout=30";
        private enum FormMode { View, Add, Edit }
        private FormMode currentMode = FormMode.View;
        private long? currentChargerId = null;
        private string currentImagePath = null;

        // Mapping ToolStripButtons
        private ToolStripButton btnAdd => tsbtnAdd;
        private ToolStripButton btnEdit => tsbtnEdit;
        private ToolStripButton btnDelete => tsbtnDelete;
        private ToolStripButton btnSave => tsbtnSave;
        private ToolStripButton btnCancel => tsbtnCancel;
        private long? _stationId;

        public frmCharger(long? stationId = null)
        {
            InitializeComponent();
                _stationId = stationId;

        }

        private void frmCharger_Load(object sender, EventArgs e)
        {
            InitializeForm();
            LoadStations();
            LoadChargerTypes();
            LoadDesignTypes();
            LoadChargers();
            SetFormMode(FormMode.View);
            this.ControlBox = false;

            // Select the station if one was passed
            if (_stationId.HasValue)
            {
                cbStation.EditValue = _stationId.Value;
            }

        }

        #region Initialization

        private void InitializeForm()
        {
            // Set button texts
            btnAdd.Text = "Thêm";
            btnEdit.Text = "Sửa";
            btnDelete.Text = "Xóa";
            btnSave.Text = "Lưu";
            gvCharger.FocusedRowChanged += GvCharger_FocusedRowChanged;
            gvCharger.CustomColumnDisplayText += GvCharger_CustomColumnDisplayText;

            // Picture click event
            Picture.Click += Picture_Click;

            // ComboBox events for filtering
            ckMoto.CheckedChanged += FilterChargers;
            ckCar.CheckedChanged += FilterChargers;

            // Setup GridView search
            SetupGridSearch();

            // Disable controls initially
            txtChargerId.ReadOnly = true;
               cbStation.Enabled = true;
               cbStation.Visible = true;

            btnCancel.Enabled = true;


        }
        private void LoadStations()
        {
            try
            {
                string query = "SELECT StationId, Name FROM Station ORDER BY Name";
                DataTable dt = DatabaseHelper.ExecuteQuery(connectionString, query);

                cbStation.Properties.DataSource = dt;
                cbStation.Properties.DisplayMember = "Name";
                cbStation.Properties.ValueMember = "StationId";
                cbStation.Properties.PopulateColumns();
                cbStation.Properties.Columns["StationId"].Visible = false;
                // Thêm dòng "Tất cả trạm"
                DataRow allRow = dt.NewRow();
                allRow["StationId"] = DBNull.Value;
                allRow["Name"] = "-- Tất cả trạm --";
                dt.Rows.InsertAt(allRow, 0);

                // Đặt EditValue là 0 nhưng kiểu phải đúng
                cbStation.EditValue = 0L; // nếu ValueMember là long
            }
            catch (Exception ex)
            {

                XtraMessageBox.Show($"Lỗi load trạm: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadChargerTypes()
        {
            cbChargerType.Properties.Items.Clear();
            cbChargerType.Properties.Items.AddRange(new string[] { "AC", "DC", "AC/DC" });
        }

        private void LoadDesignTypes()
        {
            cbDesign.Properties.Items.Clear();
            cbDesign.Properties.Items.AddRange(new string[] {
                "Wall-mounted", "Floor-standing", "Portable", "Pedestal"
            });
        }

        private void SetupGridSearch()
        {
            // Enable built-in search panel
            gvCharger.OptionsFind.AlwaysVisible = true;
            gvCharger.OptionsFind.FindNullPrompt = "Tìm kiếm theo tên trụ sạc...";
            gvCharger.OptionsFind.ShowClearButton = true;
            gvCharger.OptionsFind.ShowFindButton = true;
        }

        #endregion

        #region Load Data

        private void LoadChargers(long? stationId = null, string useFor = null)
        {
            try
            {
                string query = @"
                    SELECT c.ChargerId, c.StationId, c.Name, c.SerialNumber, c.Model, 
                           c.ChargerType, c.MaxPowerKW, c.Phases, c.OutputVoltageMin, 
                           c.OutputVoltageMax, c.PortCount, c.Design, c.Protections, 
                           c.FirmwareVersion, c.InstalledAt, c.Status, c.CreatedAt, 
                           c.PicturePath, c.UseFor, c.Note,
                           s.Name
                    FROM Charger c
                    LEFT JOIN Station s ON c.StationId = s.StationId
                    WHERE 1=1";

                if (stationId.HasValue)
                {
                    query += " AND c.StationId = @StationId";
                }

                if (!string.IsNullOrEmpty(useFor))
                {
                    query += " AND c.UseFor = @UseFor";
                }

                query += " ORDER BY c.Name";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);

                    if (stationId.HasValue)
                        cmd.Parameters.AddWithValue("@StationId", stationId.Value);

                    if (!string.IsNullOrEmpty(useFor))
                        cmd.Parameters.AddWithValue("@UseFor", useFor);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    gcCharger1.DataSource = dt;
                }

                // Setup columns
                SetupGridColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi load dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupGridColumns()
        {
            gvCharger.Columns["ChargerId"].Caption = "Mã trụ";
            gvCharger.Columns["ChargerId"].Width = 80;

            gvCharger.Columns["Name"].Caption = "Tên trụ sạc";
            gvCharger.Columns["Name"].Width = 150;

            gvCharger.Columns["Name"].Caption = "Trạm";
            gvCharger.Columns["Name"].Width = 120;

            gvCharger.Columns["ChargerType"].Caption = "Loại";
            gvCharger.Columns["ChargerType"].Width = 60;

            gvCharger.Columns["MaxPowerKW"].Caption = "Công suất (kW)";
            gvCharger.Columns["MaxPowerKW"].Width = 100;
            gvCharger.Columns["MaxPowerKW"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gvCharger.Columns["MaxPowerKW"].DisplayFormat.FormatString = "N2";

            gvCharger.Columns["Status"].Caption = "Trạng thái";
            gvCharger.Columns["Status"].Width = 100;

            gvCharger.Columns["UseFor"].Caption = "Dùng cho";
            gvCharger.Columns["UseFor"].Width = 100;

            // Hide unnecessary columns
            gvCharger.Columns["StationId"].Visible = false;
            gvCharger.Columns["SerialNumber"].Visible = false;
            gvCharger.Columns["Model"].Visible = false;
            gvCharger.Columns["Phases"].Visible = false;
            gvCharger.Columns["OutputVoltageMin"].Visible = false;
            gvCharger.Columns["OutputVoltageMax"].Visible = false;
            gvCharger.Columns["PortCount"].Visible = false;
            gvCharger.Columns["Design"].Visible = false;
            gvCharger.Columns["Protections"].Visible = false;
            gvCharger.Columns["FirmwareVersion"].Visible = false;
            gvCharger.Columns["InstalledAt"].Visible = false;
            gvCharger.Columns["CreatedAt"].Visible = false;
            gvCharger.Columns["PicturePath"].Visible = false;
            gvCharger.Columns["Note"].Visible = false;
        }

        #endregion

        #region Filter & Search

        // Sửa 2 hàm filter – trước đây không hoạt động đúng
        private void cbStation_EditValueChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void FilterChargers(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            long? stationId = null;
            if (cbStation.EditValue != null && cbStation.EditValue != DBNull.Value && Convert.ToInt64(cbStation.EditValue) > 0)
                stationId = Convert.ToInt64(cbStation.EditValue);

            string useFor = null;
            if (ckMoto.Checked && ckCar.Checked)
                useFor = "Xe máy, Ô tô";
            else if (ckMoto.Checked)
                useFor = "Xe máy";
            else if (ckCar.Checked)
                useFor = "Ô tô";

            LoadChargers(stationId, useFor);
        }

        #endregion

        #region GridView Events

        private void GvCharger_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (currentMode == FormMode.View)
            {
                LoadChargerToForm();
            }
        }

        private void GvCharger_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "Status")
            {
                if (e.Value?.ToString() == "active")
                    e.DisplayText = "Hoạt động";
                else if (e.Value?.ToString() == "poweroff")
                    e.DisplayText = "Tắt";
            }
            else if (e.Column.FieldName == "UseFor")
            {
                e.DisplayText = e.Value?.ToString() ?? "";
            }
        }

        private void LoadChargerToForm()
        {
            try
            {

                int rowHandle = gvCharger.FocusedRowHandle;
                if (rowHandle < 0) return;

                DataRow row = gvCharger.GetDataRow(rowHandle);
                if (row == null) return;

                currentChargerId = Convert.ToInt64(row["ChargerId"]);

                // Fill form fields
                txtChargerId.Text = row["ChargerId"].ToString();
                txtChargerName.Text = row["Name"]?.ToString() ?? "";
                txtSerialNumber.Text = row["SerialNumber"]?.ToString() ?? "";
                txtModel.Text = row["Model"]?.ToString() ?? "";
                cbChargerType.Text = row["ChargerType"]?.ToString() ?? "";
                txtMaxPowerKW.Text = row["MaxPowerKW"]?.ToString() ?? "";
                sePhases.Value = row["Phases"] != DBNull.Value ? Convert.ToInt32(row["Phases"]) : 0;
                txtOutputVoltageMin.Text = row["OutputVoltageMin"]?.ToString() ?? "";
                txtOutputVoltageMax.Text = row["OutputVoltageMax"]?.ToString() ?? "";
                sePortCount.Value = row["PortCount"] != DBNull.Value ? Convert.ToInt32(row["PortCount"]) : 0;
                cbDesign.Text = row["Design"]?.ToString() ?? "";
                txtProtections.Text = row["Protections"]?.ToString() ?? "";
                txtFirmwareVersion.Text = row["FirmwareVersion"]?.ToString() ?? "";
                deInstalledAt.EditValue = row["InstalledAt"] != DBNull.Value ? row["InstalledAt"] : null;
                memoEdit1.Text = row["Note"]?.ToString() ?? "";

                // Status
                tsStatus.IsOn = row["Status"]?.ToString() == "active";

                // UseFor checkboxes
                string useFor = row["UseFor"]?.ToString() ?? "";
                ckMoto.Checked = useFor.Contains("Xe máy");
                ckCar.Checked = useFor.Contains("Ô tô");

                // Load image
                string picturePath = row["PicturePath"]?.ToString();
                LoadImage(picturePath);

                // Set StationId in lookupedit
                long stationId = Convert.ToInt64(row["StationId"]);
                cbStation.EditValue = stationId;

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi load dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Form Mode Management

        private void SetFormMode(FormMode mode)
        {
            currentMode = mode;

            bool isEditMode = (mode == FormMode.Add || mode == FormMode.Edit);

            // Enable/Disable controls
            txtChargerName.ReadOnly = !isEditMode;
            txtSerialNumber.ReadOnly = !isEditMode;
            txtModel.ReadOnly = !isEditMode;
            cbChargerType.Enabled = isEditMode;
            txtMaxPowerKW.ReadOnly = !isEditMode;
            sePhases.ReadOnly = !isEditMode;
            txtOutputVoltageMin.ReadOnly = !isEditMode;
            txtOutputVoltageMax.ReadOnly = !isEditMode;
            sePortCount.ReadOnly = !isEditMode;
            cbDesign.Enabled = isEditMode;
            txtProtections.ReadOnly = !isEditMode;
            txtFirmwareVersion.ReadOnly = !isEditMode;
            deInstalledAt.ReadOnly = !isEditMode;
            tsStatus.Enabled = isEditMode;
            ckMoto.Enabled = isEditMode;
            ckCar.Enabled = isEditMode;
            memoEdit1.ReadOnly = !isEditMode;
            Picture.Properties.ReadOnly = !isEditMode;
            
            // Station ComboBox - only editable in Add mode

            cbStation.Enabled = true;
            cbStation.Properties.ReadOnly = false;


            // Button states
            btnAdd.Enabled = (mode == FormMode.View);
            btnEdit.Enabled = (mode == FormMode.View && currentChargerId.HasValue);
            btnDelete.Enabled = (mode == FormMode.View && currentChargerId.HasValue);
            btnSave.Enabled = isEditMode;
            btnCancel.Enabled = true;
        //    btnCancel.Enabled = isEditMode;

            // GridView
            gcCharger1.Enabled = (mode == FormMode.View);
        }

        #endregion

        #region Button Events

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ClearForm();
            SetFormMode(FormMode.Add);
            txtChargerName.Focus();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (!currentChargerId.HasValue)
            {
                XtraMessageBox.Show("Vui lòng chọn một trụ sạc để sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetFormMode(FormMode.Edit);
            txtChargerName.Focus();
        }

       
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            // Nếu đang View thì không cần làm gì
            if (currentMode == FormMode.View)
                return;

            // Hỏi xác nhận nếu đang nhập dở
            if (XtraMessageBox.Show("Bạn có chắc muốn hủy thao tác đang thực hiện?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            // Quay về chế độ xem
            SetFormMode(FormMode.View);

            // Nếu đang edit -> load lại dữ liệu từ grid
            if (currentChargerId.HasValue)
                LoadChargerToForm();
            else
                ClearForm(); // nếu đang Add thì clear form
        }
        
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            if (currentMode == FormMode.Add)
            {
                InsertCharger();
            }
            else if (currentMode == FormMode.Edit)
            {
                UpdateCharger();
            }
        }

        #endregion

        #region CRUD Operations

        // Sửa hàm ValidateForm – dòng này đang sai
        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtChargerName.Text))
            {
                XtraMessageBox.Show("Vui lòng nhập tên trụ sạc!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtChargerName.Focus();
                return false;
            }
            if (cbStation.EditValue == null || cbStation.EditValue == DBNull.Value || Convert.ToInt64(cbStation.EditValue) == 0)
            {
                XtraMessageBox.Show("Vui lòng chọn trạm sạc!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbStation.Focus();
                return false;
            }
            if (!ckMoto.Checked && !ckCar.Checked)
            {
                XtraMessageBox.Show("Vui lòng chọn ít nhất một loại xe (Xe máy hoặc Ô tô)!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void InsertCharger()
        {
            try
            {
                string query = @"
                    INSERT INTO Charger 
                    (StationId, Name, SerialNumber, Model, ChargerType, MaxPowerKW, Phases, 
                     OutputVoltageMin, OutputVoltageMax, PortCount, Design, Protections, 
                     FirmwareVersion, InstalledAt, Status, PicturePath, UseFor, CreatedAt, Note)
                    VALUES 
                    (@StationId, @Name, @SerialNumber, @Model, @ChargerType, @MaxPowerKW, @Phases,
                     @OutputVoltageMin, @OutputVoltageMax, @PortCount, @Design, @Protections,
                     @FirmwareVersion, @InstalledAt, @Status, @PicturePath, @UseFor, GETDATE(), @Note)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                 //   cmd.Parameters.AddWithValue("@ChargerId", currentChargerId.Value);
                    AddParameters(cmd); // Thêm tất cả các parameter khác, KHÔNG trùng @StationId, @Name


                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                XtraMessageBox.Show("Thêm trụ sạc thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadChargers();
                SetFormMode(FormMode.View);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi thêm: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateCharger()
        {
            try
            {
                string query = @"
                    UPDATE Charger SET
                        StationId = @StationId,
                        Name = @Name,
                        SerialNumber = @SerialNumber,
                        Model = @Model,
                        ChargerType = @ChargerType,
                        MaxPowerKW = @MaxPowerKW,
                        Phases = @Phases,
                        OutputVoltageMin = @OutputVoltageMin,
                        OutputVoltageMax = @OutputVoltageMax,
                        PortCount = @PortCount,
                        Design = @Design,
                        Protections = @Protections,
                        FirmwareVersion = @FirmwareVersion,
                        InstalledAt = @InstalledAt,
                        Status = @Status,
                        PicturePath = @PicturePath,
                        UseFor = @UseFor,
                        Note = @Note
                    WHERE ChargerId = @ChargerId";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ChargerId", currentChargerId.Value);
                    AddParameters(cmd); // Thêm tất cả các parameter khác, KHÔNG trùng @StationId, @Name


                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                XtraMessageBox.Show("Cập nhật trụ sạc thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadChargers();
                SetFormMode(FormMode.View);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Thêm hàm ShowReasonInputForm vào class frmCharger
        // Thay thế hàm ShowReasonInputForm() cũ
        private string ShowReasonInputForm()
        {
            // Tạo và hiển thị Form nhập liệu
            using (frmReasonInput reasonForm = new frmReasonInput())
            {
                // Hiển thị Form với Form cha là 'this'
                if (reasonForm.ShowDialog(this) == DialogResult.OK)
                {
                    return reasonForm.Reason;
                }
            }
            // Trả về chuỗi rỗng nếu người dùng hủy (Cancel)
            return string.Empty;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (!currentChargerId.HasValue)
            {
                XtraMessageBox.Show("Vui lòng chọn một trụ sạc để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. Yêu cầu giải trình lý do
            string reason = ShowReasonInputForm();

            if (string.IsNullOrEmpty(reason))
            {
                return;
            }

            if (XtraMessageBox.Show(
                $"Bạn có chắc chắn muốn xóa trụ sạc '{txtChargerName.Text}' với lý do: {reason}?\n\n" +
                "Hành động này sẽ xóa CASCADE toàn bộ dữ liệu liên quan!",
                "Xác nhận xóa & Lưu Log",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DeleteChargerWithLogging(currentChargerId.Value, reason);
            }
        }
        /// <summary>
        /// Kiểm tra các bảng liên quan có FK của trụ không
        /// </summary>
        private bool CheckForDataToLog(long chargerId)
        {
            string query = @"
        SELECT COUNT(i.InvoiceId) 
        FROM Invoices i
        JOIN ChargingSession cs ON i.SessionId = cs.SessionId
        JOIN Connector c ON cs.ConnectorId = c.ConnectorId
        WHERE c.ChargerId = @ChargerId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ChargerId", chargerId);

                try
                {
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Lỗi kiểm tra dữ liệu: {ex.Message}", "Lỗi SQL",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }


        // xóa trụ , xảy ra khi trụ khong có dữ liệu liên quan cần log
        private void DeleteChargerDirectly(long chargerId)
        {
            try
            {
                string deleteQuery = "DELETE FROM Charger WHERE ChargerId = @ChargerId";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                    cmd.Parameters.AddWithValue("@ChargerId", chargerId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                XtraMessageBox.Show(this, "Xóa trụ sạc thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadChargers();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(this, $"Lỗi khi xóa: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xóa trụ sạc với việc kiểm tra và lưu log dữ liệu liên quan nếu cần thiết
        /// </summary>
        private void DeleteChargerWithLogging(long chargerId, string reason)
        {
            if (CheckForDataToLog(chargerId))
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        // BƯỚC 2a: LƯU LOG
                        LogDataForDeletion(conn, transaction, chargerId, reason);

                        // BƯỚC 2b: XÓA CHARGER (Kích hoạt CASCADE DELETE ở DB)
                        string deleteQuery = "DELETE FROM Charger WHERE ChargerId = @ChargerId";
                        SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, transaction);
                        deleteCmd.Parameters.AddWithValue("@ChargerId", chargerId);
                        deleteCmd.ExecuteNonQuery();

                        transaction.Commit();
                        XtraMessageBox.Show(this, "Xóa trụ sạc và lưu trữ dữ liệu liên quan thành công!",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ClearForm();
                        LoadChargers();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        XtraMessageBox.Show(this, $"Lỗi khi xóa (Rollback đã được thực hiện): {ex.Message}",
                            "Lỗi Xóa", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                if (XtraMessageBox.Show(this, "Trụ sạc không có dữ liệu giao dịch tài chính. Xóa trực tiếp?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DeleteChargerDirectly(chargerId);
                }
            }
        }

        // thêm log 

        /// <summary>
        /// Thêm log - ĐÃ SỬA: Bỏ DeletedBy, chỉ còn LogReason và LogTime
        /// </summary>
        private void LogDataForDeletion(SqlConnection conn, SqlTransaction transaction, long chargerId, string reason)
        {
            string logQuery = @"
        -- 1. KHAI BÁO BIẾN BẢNG ĐỂ LƯU TẠM SESSION LIST
        DECLARE @SessionTable TABLE (SessionId bigint);

        -- Đổ dữ liệu vào biến bảng (Lấy SessionId từ ConnectorId)
        INSERT INTO @SessionTable (SessionId)
        SELECT cs.SessionId 
        FROM ChargingSession cs
        JOIN Connector c ON cs.ConnectorId = c.ConnectorId
        WHERE c.ChargerId = @ChargerId;

        -- 2. LOG REVENUE
        INSERT INTO RevenueRecognitionLog 
            (Id, InvoiceId, Customerid, Invoice_Delivery, Receipt_Delivery, Money_received, Methods_payment, 
             InvoiceDetailId, ReceiptId, ReceiptDetailSTT, RecordDate, Documentdate, Employeeid, LogReason, LogTime)
        SELECT 
            rr.Id, rr.InvoiceId, rr.Customerid, rr.Invoice_Delivery, rr.Receipt_Delivery, rr.Money_received, 
            rr.Methods_payment, rr.InvoiceDetailId, rr.ReceiptId, rr.ReceiptDetailSTT, rr.RecordDate, 
            rr.Documentdate, rr.Employeeid,
            @Reason, GETDATE()
        FROM [dbo].[Revenue_recognition] rr
        JOIN Invoices i ON rr.InvoiceId = i.InvoiceId
        JOIN @SessionTable st ON i.SessionId = st.SessionId;

        -- 3. LOG RECORD DETAIL
        INSERT INTO RecordDetailLog
            (STT, Recognition_id, ItemId, Interpretation, Debit_account, Credit_account, Tax_account, 
             UnitPrice, Quantities, Amount, Tax_Percentae, TaxAmount, Unit_measure, DiscountPercent, 
             DiscountAmount, LogReason, LogTime)
        SELECT 
            rd.STT, rd.Recognition_id, rd.ItemId, rd.Interpretation, rd.Debit_account, rd.Credit_account, 
            rd.Tax_account, rd.UnitPrice, rd.Quantities, rd.Amount, rd.Tax_Percentae, rd.TaxAmount, 
            rd.Unit_measure, rd.DiscountPercent, rd.DiscountAmount,
            @Reason, GETDATE()
        FROM Record_Detail rd
        JOIN [dbo].[Revenue_recognition] rr ON rd.Recognition_id = rr.Id
        JOIN Invoices i ON rr.InvoiceId = i.InvoiceId
        JOIN @SessionTable st ON i.SessionId = st.SessionId;

        -- 4. LOG INVOICES
        INSERT INTO InvoiceLog
        (
            InvoiceId, SessionId, CustomerID,
            Snashot_CustomerName, Snashot_CustomerPhone, Snashot_CustomerEmail,
            InvoiceNumber, InvoiceTemplate, InvoiceSymbol, CreatedAt, ExpireDate,
            TotalAmountService, TotalAmountTax, TotalAmountDiscount, Total,
            PaymentLink, QRCodeData, Status, PaidAt, PaymentMethod,
            PdfFilePath, EmailSent, EmailSentAt,
            Customer_Signature, SignatureFile, Notes,
            LogReason, LogTime
        )
        SELECT 
            i.InvoiceId, i.SessionId, i.CustomerID,
            i.Snashot_CustomerName, i.Snashot_CustomerPhone, i.Snashot_CustomerEmail,
            i.InvoiceNumber, i.InvoiceTemplate, i.InvoiceSymbol, i.CreatedAt, i.ExpireDate,
            i.TotalAmountService, i.TotalAmountTax, i.TotalAmountDiscount, i.Total,
            i.PaymentLink, i.QRCodeData, i.Status, i.PaidAt, i.PaymentMethod,
            i.PdfFilePath, i.EmailSent, i.EmailSentAt,
            i.Customer_Signature, i.SignatureFile, i.Notes,
            @Reason, GETDATE()
        FROM Invoices i
        JOIN @SessionTable st ON i.SessionId = st.SessionId;

        -- 5. LOG INVOICE DETAILS
        INSERT INTO InvoiceDetailLog
        (
            InvoiceId, STT, ItemId, Quantities, Unit, UnitPrice, Amount,
            DiscountPercent, DiscountAmount, AmountAfterDiscount, Tax, TaxAmount,
            LogReason, LogTime
        )
        SELECT 
            id.InvoiceId, id.STT, id.ItemId, id.Quantities, id.Unit, id.UnitPrice, id.Amount,
            id.DiscountPercent, id.DiscountAmount, id.AmountAfterDiscount, id.Tax, id.TaxAmount,
            @Reason, GETDATE()
        FROM InvoiceDetail id
        JOIN Invoices i ON id.InvoiceId = i.InvoiceId
        JOIN @SessionTable st ON i.SessionId = st.SessionId;

        -- 6. LOG CHARGING SESSION
        INSERT INTO ChargingSessionLog
            (SessionId, ConnectorId, VIN, StartTime, EndTime, MeterStartKWh, MeterStopKWh, 
             StartSOC, EndSOC, TargetSOC, Status, LastUpdated, ExpectTime, PowerOffTime, 
             LogReason, LogTime)
        SELECT 
            cs.SessionId, cs.ConnectorId, cs.VIN, cs.StartTime, cs.EndTime, cs.MeterStartKWh, 
            cs.MeterStopKWh, cs.StartSOC, cs.EndSOC, cs.TargetSOC, cs.Status, cs.LastUpdated, 
            cs.ExpectTime, cs.PowerOffTime,
            @Reason, GETDATE()
        FROM ChargingSession cs
        JOIN @SessionTable st ON cs.SessionId = st.SessionId;

        -- 7. LOG CONNECTOR
        INSERT INTO ConnectorLog
            (ConnectorId, ChargerId, ConnectorIndex, ConnectorType, Status, LogReason, LogTime)
        SELECT 
            c.ConnectorId, c.ChargerId, c.ConnectorIndex, c.ConnectorType, c.Status,
            @Reason, GETDATE()
        FROM Connector c
        WHERE c.ChargerId = @ChargerId;
    ";

            using (SqlCommand cmd = new SqlCommand(logQuery, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@ChargerId", chargerId);
                cmd.Parameters.AddWithValue("@Reason", reason);
                cmd.CommandTimeout = 180; // 3 phút

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Lỗi SQL Log: " + ex.Message);
                }
            }
        }
        private void AddParameters(SqlCommand cmd)
        {
            // Get StationId from LookUpEdit
            long stationId = 0;
            if (cbStation.EditValue != null)
                stationId = Convert.ToInt64(cbStation.EditValue);
            cmd.Parameters.AddWithValue("@StationId", stationId);

            // UseFor
            string useFor = "";
            if (ckMoto.Checked && ckCar.Checked)
                useFor = "Xe máy, Ô tô";
            else if (ckMoto.Checked)
                useFor = "Xe máy";
            else if (ckCar.Checked)
                useFor = "Ô tô";
            cmd.Parameters.AddWithValue("@UseFor", useFor);

            // Các parameter khác
            cmd.Parameters.AddWithValue("@Name", txtChargerName.Text);
            cmd.Parameters.AddWithValue("@SerialNumber", string.IsNullOrEmpty(txtSerialNumber.Text) ? (object)DBNull.Value : txtSerialNumber.Text);
            cmd.Parameters.AddWithValue("@Model", string.IsNullOrEmpty(txtModel.Text) ? (object)DBNull.Value : txtModel.Text);
            cmd.Parameters.AddWithValue("@ChargerType", string.IsNullOrEmpty(cbChargerType.Text) ? (object)DBNull.Value : cbChargerType.Text);
            cmd.Parameters.AddWithValue("@MaxPowerKW", string.IsNullOrEmpty(txtMaxPowerKW.Text) ? (object)DBNull.Value : decimal.Parse(txtMaxPowerKW.Text));
            cmd.Parameters.AddWithValue("@Phases", sePhases.Value);
            cmd.Parameters.AddWithValue("@OutputVoltageMin", string.IsNullOrEmpty(txtOutputVoltageMin.Text) ? (object)DBNull.Value : decimal.Parse(txtOutputVoltageMin.Text));
            cmd.Parameters.AddWithValue("@OutputVoltageMax", string.IsNullOrEmpty(txtOutputVoltageMax.Text) ? (object)DBNull.Value : decimal.Parse(txtOutputVoltageMax.Text));
            cmd.Parameters.AddWithValue("@PortCount", sePortCount.Value);
            cmd.Parameters.AddWithValue("@Design", string.IsNullOrEmpty(cbDesign.Text) ? (object)DBNull.Value : cbDesign.Text);
            cmd.Parameters.AddWithValue("@Protections", string.IsNullOrEmpty(txtProtections.Text) ? (object)DBNull.Value : txtProtections.Text);
            cmd.Parameters.AddWithValue("@FirmwareVersion", string.IsNullOrEmpty(txtFirmwareVersion.Text) ? (object)DBNull.Value : txtFirmwareVersion.Text);
            cmd.Parameters.AddWithValue("@InstalledAt", deInstalledAt.EditValue ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", tsStatus.IsOn ? "active" : "poweroff");
            cmd.Parameters.AddWithValue("@PicturePath", currentImagePath ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Note", string.IsNullOrEmpty(memoEdit1.Text) ? (object)DBNull.Value : memoEdit1.Text);
        }

        #endregion

        #region Image Handling

        private void Picture_Click(object sender, EventArgs e)
        {
            if (currentMode != FormMode.Add && currentMode != FormMode.Edit)
                return;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                openFileDialog.Title = "Chọn ảnh trụ sạc";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Create Images directory if not exists
                        string imagesDir = Path.Combine(Application.StartupPath, "Images", "Chargers");
                        if (!Directory.Exists(imagesDir))
                            Directory.CreateDirectory(imagesDir);

                        // Generate unique filename
                        string extension = Path.GetExtension(openFileDialog.FileName);
                        string fileName = $"Charger_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                        string destPath = Path.Combine(imagesDir, fileName);

                        // Copy file
                        File.Copy(openFileDialog.FileName, destPath, true);

                        // Save relative path
                        currentImagePath = Path.Combine("Images", "Chargers", fileName);

                        // Display image
                        Picture.Image = Image.FromFile(destPath);
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show($"Lỗi khi tải ảnh: {ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadImage(string picturePath)
        {
            try
            {
                if (string.IsNullOrEmpty(picturePath))
                {
                    Picture.Image = null;
                    currentImagePath = null;
                    return;
                }

                string fullPath = Path.Combine(Application.StartupPath, picturePath);

                if (File.Exists(fullPath))
                {
                    Picture.Image = Image.FromFile(fullPath);
                    currentImagePath = picturePath;
                }
                else
                {
                    Picture.Image = null;
                    currentImagePath = null;
                }
            }
            catch
            {
                Picture.Image = null;
                currentImagePath = null;
            }
        }

        #endregion

        #region Helper Methods

        private void ClearForm()
        {
            currentChargerId = null;
            currentImagePath = null;

            txtChargerId.Text = "";
            txtChargerName.Text = "";
            txtSerialNumber.Text = "";
            txtModel.Text = "";
            cbChargerType.SelectedIndex = -1;
            txtMaxPowerKW.Text = "";
            sePhases.Value = 0;
            txtOutputVoltageMin.Text = "";
            txtOutputVoltageMax.Text = "";
            sePortCount.Value = 0;
            cbDesign.SelectedIndex = -1;
            txtProtections.Text = "";
            txtFirmwareVersion.Text = "";
            deInstalledAt.EditValue = null;
            tsStatus.IsOn = true;
            ckMoto.Checked = false;
            ckCar.Checked = false;
            memoEdit1.Text = "";
            Picture.Image = null;

            cbStation.EditValue = 0;
        }


        #endregion

       
    }

    #region DatabaseHelper
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
    }
    #endregion
}