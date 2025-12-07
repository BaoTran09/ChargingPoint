using ChargingApp.Report;
namespace ChargingApp.Accounting
{
    partial class frmDSChungTu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDSChungTu));
            this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnReloadtoolstrip = new System.Windows.Forms.ToolStripButton();
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.btnExportExcel = new System.Windows.Forms.ToolStripButton();
            this.btnExportPDF = new System.Windows.Forms.ToolStripButton();
            this.btnCollectMoney = new System.Windows.Forms.ToolStripButton();
            this.flyoutFilter = new DevExpress.Utils.FlyoutPanel();
            this.flyoutFilter1 = new DevExpress.Utils.FlyoutPanelControl();
            this.btnReset = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnFilter = new DevExpress.XtraEditors.SimpleButton();
            this.lupEDocumentType = new DevExpress.XtraEditors.LookUpEdit();
            this.lupEPaymentStatus = new DevExpress.XtraEditors.LookUpEdit();
            this.DEtoDate = new DevExpress.XtraEditors.DateEdit();
            this.lupEDocumentStatus = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.DEfromDate = new DevExpress.XtraEditors.DateEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnOpenFilter = new DevExpress.XtraEditors.DropDownButton();
            this.gcDocument = new DevExpress.XtraGrid.GridControl();
            this.gvDocument = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colSelect = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDocumentNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCustomerName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colToTal = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPaymentSatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAction = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.gcDocumentDetail = new DevExpress.XtraGrid.GridControl();
            this.gvDocumentDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colItemName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUnit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQuantities = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUnitPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTaxPercent = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTaxAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDiscountPercent = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDiscountAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDebitAccount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreditAccount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTaxAccount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDiscountAccount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.xtraScrollableControl1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.flyoutFilter)).BeginInit();
            this.flyoutFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.flyoutFilter1)).BeginInit();
            this.flyoutFilter1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lupEDocumentType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lupEPaymentStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEtoDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEtoDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lupEDocumentStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEfromDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEfromDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcDocument)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDocument)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcDocumentDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDocumentDetail)).BeginInit();
            this.SuspendLayout();
            // 
            // xtraScrollableControl1
            // 
            this.xtraScrollableControl1.Controls.Add(this.toolStrip1);
            this.xtraScrollableControl1.Controls.Add(this.flyoutFilter);
            this.xtraScrollableControl1.Controls.Add(this.btnOpenFilter);
            this.xtraScrollableControl1.Controls.Add(this.gcDocument);
            this.xtraScrollableControl1.Controls.Add(this.gcDocumentDetail);
            this.xtraScrollableControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraScrollableControl1.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xtraScrollableControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraScrollableControl1.Name = "xtraScrollableControl1";
            this.xtraScrollableControl1.Size = new System.Drawing.Size(1780, 812);
            this.xtraScrollableControl1.TabIndex = 7;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Right;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnReloadtoolstrip,
            this.btnAdd,
            this.btnExportExcel,
            this.btnExportPDF,
            this.btnCollectMoney});
            this.toolStrip1.Location = new System.Drawing.Point(1698, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(82, 812);
            this.toolStrip1.TabIndex = 11;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnReloadtoolstrip
            // 
            this.btnReloadtoolstrip.Image = ((System.Drawing.Image)(resources.GetObject("btnReloadtoolstrip.Image")));
            this.btnReloadtoolstrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReloadtoolstrip.Name = "btnReloadtoolstrip";
            this.btnReloadtoolstrip.Size = new System.Drawing.Size(79, 44);
            this.btnReloadtoolstrip.Text = "Tải lại";
            this.btnReloadtoolstrip.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // btnAdd
            // 
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(79, 44);
            this.btnAdd.Text = "Thêm";
            this.btnAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnExportExcel.Image")));
            this.btnExportExcel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(79, 44);
            this.btnExportExcel.Text = "Xuất Excel";
            this.btnExportExcel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // btnExportPDF
            // 
            this.btnExportPDF.Image = ((System.Drawing.Image)(resources.GetObject("btnExportPDF.Image")));
            this.btnExportPDF.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExportPDF.Name = "btnExportPDF";
            this.btnExportPDF.Size = new System.Drawing.Size(79, 44);
            this.btnExportPDF.Text = "Xuất PDF";
            this.btnExportPDF.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // btnCollectMoney
            // 
            this.btnCollectMoney.Image = ((System.Drawing.Image)(resources.GetObject("btnCollectMoney.Image")));
            this.btnCollectMoney.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCollectMoney.Name = "btnCollectMoney";
            this.btnCollectMoney.Size = new System.Drawing.Size(79, 44);
            this.btnCollectMoney.Text = "Thu tiền";
            this.btnCollectMoney.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // flyoutFilter
            // 
            this.flyoutFilter.Controls.Add(this.flyoutFilter1);
            this.flyoutFilter.Location = new System.Drawing.Point(645, 12);
            this.flyoutFilter.Name = "flyoutFilter";
            this.flyoutFilter.OwnerControl = this;
            this.flyoutFilter.Size = new System.Drawing.Size(660, 166);
            this.flyoutFilter.TabIndex = 10;
            // 
            // flyoutFilter1
            // 
            this.flyoutFilter1.Controls.Add(this.btnReset);
            this.flyoutFilter1.Controls.Add(this.btnCancel);
            this.flyoutFilter1.Controls.Add(this.btnFilter);
            this.flyoutFilter1.Controls.Add(this.lupEDocumentType);
            this.flyoutFilter1.Controls.Add(this.lupEPaymentStatus);
            this.flyoutFilter1.Controls.Add(this.DEtoDate);
            this.flyoutFilter1.Controls.Add(this.lupEDocumentStatus);
            this.flyoutFilter1.Controls.Add(this.labelControl4);
            this.flyoutFilter1.Controls.Add(this.labelControl5);
            this.flyoutFilter1.Controls.Add(this.DEfromDate);
            this.flyoutFilter1.Controls.Add(this.labelControl3);
            this.flyoutFilter1.Controls.Add(this.labelControl2);
            this.flyoutFilter1.Controls.Add(this.labelControl1);
            this.flyoutFilter1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flyoutFilter1.FlyoutPanel = this.flyoutFilter;
            this.flyoutFilter1.Location = new System.Drawing.Point(0, 0);
            this.flyoutFilter1.Name = "flyoutFilter1";
            this.flyoutFilter1.Size = new System.Drawing.Size(660, 166);
            this.flyoutFilter1.TabIndex = 0;
            // 
            // btnReset
            // 
            this.btnReset.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.Appearance.Options.UseFont = true;
            this.btnReset.Location = new System.Drawing.Point(432, 117);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(67, 29);
            this.btnReset.TabIndex = 7;
            this.btnReset.Text = "Đặt lại";
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(518, 117);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(67, 29);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Hủy";
            // 
            // btnFilter
            // 
            this.btnFilter.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilter.Appearance.Options.UseFont = true;
            this.btnFilter.Location = new System.Drawing.Point(343, 117);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(67, 29);
            this.btnFilter.TabIndex = 7;
            this.btnFilter.Text = "Lọc";
            // 
            // lupEDocumentType
            // 
            this.lupEDocumentType.Location = new System.Drawing.Point(133, 15);
            this.lupEDocumentType.Name = "lupEDocumentType";
            this.lupEDocumentType.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lupEDocumentType.Properties.Appearance.Options.UseFont = true;
            this.lupEDocumentType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lupEDocumentType.Size = new System.Drawing.Size(166, 28);
            this.lupEDocumentType.TabIndex = 6;
            // 
            // lupEPaymentStatus
            // 
            this.lupEPaymentStatus.Location = new System.Drawing.Point(133, 114);
            this.lupEPaymentStatus.Name = "lupEPaymentStatus";
            this.lupEPaymentStatus.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lupEPaymentStatus.Properties.Appearance.Options.UseFont = true;
            this.lupEPaymentStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lupEPaymentStatus.Size = new System.Drawing.Size(166, 28);
            this.lupEPaymentStatus.TabIndex = 6;
            // 
            // DEtoDate
            // 
            this.DEtoDate.EditValue = null;
            this.DEtoDate.Location = new System.Drawing.Point(413, 65);
            this.DEtoDate.Name = "DEtoDate";
            this.DEtoDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DEtoDate.Properties.Appearance.Options.UseFont = true;
            this.DEtoDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DEtoDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DEtoDate.Size = new System.Drawing.Size(172, 28);
            this.DEtoDate.TabIndex = 2;
            // 
            // lupEDocumentStatus
            // 
            this.lupEDocumentStatus.Location = new System.Drawing.Point(133, 65);
            this.lupEDocumentStatus.Name = "lupEDocumentStatus";
            this.lupEDocumentStatus.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lupEDocumentStatus.Properties.Appearance.Options.UseFont = true;
            this.lupEDocumentStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lupEDocumentStatus.Size = new System.Drawing.Size(166, 28);
            this.lupEDocumentStatus.TabIndex = 6;
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl4.Appearance.Options.UseFont = true;
            this.labelControl4.Location = new System.Drawing.Point(9, 22);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(55, 21);
            this.labelControl4.TabIndex = 3;
            this.labelControl4.Text = "Loại CT";
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl5.Appearance.Options.UseFont = true;
            this.labelControl5.Location = new System.Drawing.Point(9, 117);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(101, 21);
            this.labelControl5.TabIndex = 3;
            this.labelControl5.Text = "Trạng thái TT";
            // 
            // DEfromDate
            // 
            this.DEfromDate.EditValue = null;
            this.DEfromDate.Location = new System.Drawing.Point(413, 19);
            this.DEfromDate.Name = "DEfromDate";
            this.DEfromDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DEfromDate.Properties.Appearance.Options.UseFont = true;
            this.DEfromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DEfromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DEfromDate.Size = new System.Drawing.Size(172, 28);
            this.DEfromDate.TabIndex = 2;
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(9, 68);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(101, 21);
            this.labelControl3.TabIndex = 3;
            this.labelControl3.Text = "Trạng thái CT";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(325, 68);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(70, 21);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "Đến ngày";
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(335, 22);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(60, 21);
            this.labelControl1.TabIndex = 3;
            this.labelControl1.Text = "Từ ngày";
            // 
            // btnOpenFilter
            // 
            this.btnOpenFilter.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenFilter.Appearance.Options.UseFont = true;
            this.btnOpenFilter.Location = new System.Drawing.Point(1065, 24);
            this.btnOpenFilter.Margin = new System.Windows.Forms.Padding(6);
            this.btnOpenFilter.Name = "btnOpenFilter";
            this.btnOpenFilter.Size = new System.Drawing.Size(226, 65);
            this.btnOpenFilter.TabIndex = 9;
            this.btnOpenFilter.Text = "Lọc";
            // 
            // gcDocument
            // 
            this.gcDocument.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(6);
            this.gcDocument.Location = new System.Drawing.Point(25, 220);
            this.gcDocument.MainView = this.gvDocument;
            this.gcDocument.Margin = new System.Windows.Forms.Padding(6);
            this.gcDocument.Name = "gcDocument";
            this.gcDocument.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemButtonEdit1});
            this.gcDocument.Size = new System.Drawing.Size(1614, 232);
            this.gcDocument.TabIndex = 7;
            this.gcDocument.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvDocument});
            // 
            // gvDocument
            // 
            this.gvDocument.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colSelect,
            this.colDocumentNumber,
            this.colCustomerName,
            this.colToTal,
            this.colPaymentSatus,
            this.colAction});
            this.gvDocument.DetailHeight = 682;
            this.gvDocument.GridControl = this.gcDocument;
            this.gvDocument.Name = "gvDocument";
            this.gvDocument.OptionsEditForm.PopupEditFormWidth = 1562;
            // 
            // colSelect
            // 
            this.colSelect.Caption = "gridColumn1";
            this.colSelect.MinWidth = 49;
            this.colSelect.Name = "colSelect";
            this.colSelect.Visible = true;
            this.colSelect.VisibleIndex = 0;
            this.colSelect.Width = 182;
            // 
            // colDocumentNumber
            // 
            this.colDocumentNumber.Caption = "gridColumn2";
            this.colDocumentNumber.MinWidth = 49;
            this.colDocumentNumber.Name = "colDocumentNumber";
            this.colDocumentNumber.Visible = true;
            this.colDocumentNumber.VisibleIndex = 1;
            this.colDocumentNumber.Width = 182;
            // 
            // colCustomerName
            // 
            this.colCustomerName.Caption = "gridColumn3";
            this.colCustomerName.MinWidth = 49;
            this.colCustomerName.Name = "colCustomerName";
            this.colCustomerName.Visible = true;
            this.colCustomerName.VisibleIndex = 2;
            this.colCustomerName.Width = 182;
            // 
            // colToTal
            // 
            this.colToTal.Caption = "gridColumn4";
            this.colToTal.MinWidth = 49;
            this.colToTal.Name = "colToTal";
            this.colToTal.Visible = true;
            this.colToTal.VisibleIndex = 3;
            this.colToTal.Width = 182;
            // 
            // colPaymentSatus
            // 
            this.colPaymentSatus.Caption = "gridColumn1";
            this.colPaymentSatus.MinWidth = 49;
            this.colPaymentSatus.Name = "colPaymentSatus";
            this.colPaymentSatus.Visible = true;
            this.colPaymentSatus.VisibleIndex = 4;
            this.colPaymentSatus.Width = 182;
            // 
            // colAction
            // 
            this.colAction.Caption = "gridColumn2";
            this.colAction.ColumnEdit = this.repositoryItemButtonEdit1;
            this.colAction.MinWidth = 49;
            this.colAction.Name = "colAction";
            this.colAction.Visible = true;
            this.colAction.VisibleIndex = 5;
            this.colAction.Width = 182;
            // 
            // repositoryItemButtonEdit1
            // 
            this.repositoryItemButtonEdit1.AutoHeight = false;
            this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
            this.repositoryItemButtonEdit1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButtonEdit1.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEdit1_ButtonClick);
            // 
            // gcDocumentDetail
            // 
            this.gcDocumentDetail.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(6);
            this.gcDocumentDetail.Location = new System.Drawing.Point(25, 510);
            this.gcDocumentDetail.MainView = this.gvDocumentDetail;
            this.gcDocumentDetail.Margin = new System.Windows.Forms.Padding(6);
            this.gcDocumentDetail.Name = "gcDocumentDetail";
            this.gcDocumentDetail.Size = new System.Drawing.Size(1614, 196);
            this.gcDocumentDetail.TabIndex = 8;
            this.gcDocumentDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvDocumentDetail});
            // 
            // gvDocumentDetail
            // 
            this.gvDocumentDetail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colSTT,
            this.colItemName,
            this.colUnit,
            this.colQuantities,
            this.colUnitPrice,
            this.colAmount,
            this.colTaxPercent,
            this.colTaxAmount,
            this.colDiscountPercent,
            this.colDiscountAmount,
            this.colDebitAccount,
            this.colCreditAccount,
            this.colTaxAccount,
            this.colDiscountAccount,
            this.colDescription});
            this.gvDocumentDetail.DetailHeight = 682;
            this.gvDocumentDetail.GridControl = this.gcDocumentDetail;
            this.gvDocumentDetail.Name = "gvDocumentDetail";
            this.gvDocumentDetail.OptionsEditForm.PopupEditFormWidth = 1562;
            // 
            // colSTT
            // 
            this.colSTT.Caption = "gridColumn2";
            this.colSTT.MinWidth = 49;
            this.colSTT.Name = "colSTT";
            this.colSTT.Visible = true;
            this.colSTT.VisibleIndex = 0;
            this.colSTT.Width = 182;
            // 
            // colItemName
            // 
            this.colItemName.Caption = "gridColumn3";
            this.colItemName.MinWidth = 49;
            this.colItemName.Name = "colItemName";
            this.colItemName.Visible = true;
            this.colItemName.VisibleIndex = 1;
            this.colItemName.Width = 182;
            // 
            // colUnit
            // 
            this.colUnit.Caption = "gridColumn4";
            this.colUnit.MinWidth = 49;
            this.colUnit.Name = "colUnit";
            this.colUnit.Visible = true;
            this.colUnit.VisibleIndex = 2;
            this.colUnit.Width = 182;
            // 
            // colQuantities
            // 
            this.colQuantities.Caption = "gridColumn5";
            this.colQuantities.MinWidth = 49;
            this.colQuantities.Name = "colQuantities";
            this.colQuantities.Visible = true;
            this.colQuantities.VisibleIndex = 3;
            this.colQuantities.Width = 182;
            // 
            // colUnitPrice
            // 
            this.colUnitPrice.Caption = "gridColumn2";
            this.colUnitPrice.MinWidth = 49;
            this.colUnitPrice.Name = "colUnitPrice";
            this.colUnitPrice.Visible = true;
            this.colUnitPrice.VisibleIndex = 4;
            this.colUnitPrice.Width = 182;
            // 
            // colAmount
            // 
            this.colAmount.Caption = "gridColumn3";
            this.colAmount.MinWidth = 49;
            this.colAmount.Name = "colAmount";
            this.colAmount.Visible = true;
            this.colAmount.VisibleIndex = 5;
            this.colAmount.Width = 182;
            // 
            // colTaxPercent
            // 
            this.colTaxPercent.Caption = "gridColumn2";
            this.colTaxPercent.MinWidth = 49;
            this.colTaxPercent.Name = "colTaxPercent";
            this.colTaxPercent.Visible = true;
            this.colTaxPercent.VisibleIndex = 6;
            this.colTaxPercent.Width = 182;
            // 
            // colTaxAmount
            // 
            this.colTaxAmount.Caption = "gridColumn2";
            this.colTaxAmount.MinWidth = 49;
            this.colTaxAmount.Name = "colTaxAmount";
            this.colTaxAmount.Visible = true;
            this.colTaxAmount.VisibleIndex = 7;
            this.colTaxAmount.Width = 182;
            // 
            // colDiscountPercent
            // 
            this.colDiscountPercent.Caption = "gridColumn3";
            this.colDiscountPercent.MinWidth = 49;
            this.colDiscountPercent.Name = "colDiscountPercent";
            this.colDiscountPercent.Visible = true;
            this.colDiscountPercent.VisibleIndex = 8;
            this.colDiscountPercent.Width = 182;
            // 
            // colDiscountAmount
            // 
            this.colDiscountAmount.Caption = "gridColumn4";
            this.colDiscountAmount.MinWidth = 49;
            this.colDiscountAmount.Name = "colDiscountAmount";
            this.colDiscountAmount.Visible = true;
            this.colDiscountAmount.VisibleIndex = 9;
            this.colDiscountAmount.Width = 182;
            // 
            // colDebitAccount
            // 
            this.colDebitAccount.Caption = "gridColumn1";
            this.colDebitAccount.MinWidth = 25;
            this.colDebitAccount.Name = "colDebitAccount";
            this.colDebitAccount.Visible = true;
            this.colDebitAccount.VisibleIndex = 10;
            this.colDebitAccount.Width = 94;
            // 
            // colCreditAccount
            // 
            this.colCreditAccount.Caption = "gridColumn2";
            this.colCreditAccount.MinWidth = 25;
            this.colCreditAccount.Name = "colCreditAccount";
            this.colCreditAccount.Visible = true;
            this.colCreditAccount.VisibleIndex = 11;
            this.colCreditAccount.Width = 94;
            // 
            // colTaxAccount
            // 
            this.colTaxAccount.Caption = "gridColumn3";
            this.colTaxAccount.MinWidth = 25;
            this.colTaxAccount.Name = "colTaxAccount";
            this.colTaxAccount.Visible = true;
            this.colTaxAccount.VisibleIndex = 12;
            this.colTaxAccount.Width = 94;
            // 
            // colDiscountAccount
            // 
            this.colDiscountAccount.Caption = "gridColumn1";
            this.colDiscountAccount.MinWidth = 25;
            this.colDiscountAccount.Name = "colDiscountAccount";
            this.colDiscountAccount.Visible = true;
            this.colDiscountAccount.VisibleIndex = 13;
            this.colDiscountAccount.Width = 94;
            // 
            // colDescription
            // 
            this.colDescription.Caption = "gridColumn1";
            this.colDescription.MinWidth = 25;
            this.colDescription.Name = "colDescription";
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 14;
            this.colDescription.Width = 94;
            // 
            // frmDSChungTu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1780, 812);
            this.Controls.Add(this.xtraScrollableControl1);
            this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Shadow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmDSChungTu";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmDSChungTu_Load);
            this.xtraScrollableControl1.ResumeLayout(false);
            this.xtraScrollableControl1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.flyoutFilter)).EndInit();
            this.flyoutFilter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.flyoutFilter1)).EndInit();
            this.flyoutFilter1.ResumeLayout(false);
            this.flyoutFilter1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lupEDocumentType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lupEPaymentStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEtoDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEtoDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lupEDocumentStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEfromDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEfromDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcDocument)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDocument)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcDocumentDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDocumentDetail)).EndInit();
            this.ResumeLayout(false);

        }

        private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnReloadtoolstrip;
        private System.Windows.Forms.ToolStripButton btnAdd;
        private System.Windows.Forms.ToolStripButton btnExportExcel;
        private System.Windows.Forms.ToolStripButton btnExportPDF;
        private System.Windows.Forms.ToolStripButton btnCollectMoney;
        private DevExpress.Utils.FlyoutPanel flyoutFilter;
        private DevExpress.Utils.FlyoutPanelControl flyoutFilter1;
        private DevExpress.XtraEditors.SimpleButton btnReset;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnFilter;
        private DevExpress.XtraEditors.LookUpEdit lupEDocumentType;
        private DevExpress.XtraEditors.LookUpEdit lupEPaymentStatus;
        private DevExpress.XtraEditors.DateEdit DEtoDate;
        private DevExpress.XtraEditors.LookUpEdit lupEDocumentStatus;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.DateEdit DEfromDate;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.DropDownButton btnOpenFilter;
        private DevExpress.XtraGrid.GridControl gcDocument;
        private DevExpress.XtraGrid.Views.Grid.GridView gvDocument;
        private DevExpress.XtraGrid.Columns.GridColumn colSelect;
        private DevExpress.XtraGrid.Columns.GridColumn colDocumentNumber;
        private DevExpress.XtraGrid.Columns.GridColumn colCustomerName;
        private DevExpress.XtraGrid.Columns.GridColumn colToTal;
        private DevExpress.XtraGrid.Columns.GridColumn colPaymentSatus;
        private DevExpress.XtraGrid.Columns.GridColumn colAction;
        private DevExpress.XtraGrid.GridControl gcDocumentDetail;
        private DevExpress.XtraGrid.Views.Grid.GridView gvDocumentDetail;
        private DevExpress.XtraGrid.Columns.GridColumn colSTT;
        private DevExpress.XtraGrid.Columns.GridColumn colItemName;
        private DevExpress.XtraGrid.Columns.GridColumn colUnit;
        private DevExpress.XtraGrid.Columns.GridColumn colQuantities;
        private DevExpress.XtraGrid.Columns.GridColumn colUnitPrice;
        private DevExpress.XtraGrid.Columns.GridColumn colAmount;
        private DevExpress.XtraGrid.Columns.GridColumn colTaxPercent;
        private DevExpress.XtraGrid.Columns.GridColumn colTaxAmount;
        private DevExpress.XtraGrid.Columns.GridColumn colDiscountPercent;
        private DevExpress.XtraGrid.Columns.GridColumn colDiscountAmount;
        private DevExpress.XtraGrid.Columns.GridColumn colDebitAccount;
        private DevExpress.XtraGrid.Columns.GridColumn colCreditAccount;
        private DevExpress.XtraGrid.Columns.GridColumn colTaxAccount;
        private DevExpress.XtraGrid.Columns.GridColumn colDiscountAccount;
        //private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox1;
        private DevExpress.XtraGrid.Columns.GridColumn colDescription;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEdit1;

        #endregion
        //




    }
}