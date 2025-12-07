namespace ChargingApp.Accounting
{
    partial class frmHachToan
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHachToan));
            this.cbMethods_payment = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.CkMoney_Received = new DevExpress.XtraEditors.CheckEdit();
            this.lbRecognitionId = new DevExpress.XtraEditors.LabelControl();
            this.lbMethods_payment = new DevExpress.XtraEditors.LabelControl();
            this.CkInvoice_Delivery = new DevExpress.XtraEditors.CheckEdit();
            this.CkReceipt_Delivery = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.btnThamChieuHD = new DevExpress.XtraEditors.ButtonEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.txtCustomerId = new DevExpress.XtraEditors.TextEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.txtCustomerName = new DevExpress.XtraEditors.TextEdit();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.txtCustomerTaxCode = new DevExpress.XtraEditors.TextEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.txtCustomerAddress = new DevExpress.XtraEditors.TextEdit();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.txtCustomerNumberPhone = new DevExpress.XtraEditors.TextEdit();
            this.hlInvocieID = new DevExpress.XtraEditors.HyperlinkLabelControl();
            this.gcRecognitionDetail = new DevExpress.XtraGrid.GridControl();
            this.gvRecognition_Detail = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.glServiceName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.glDebitAccount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.glCreditAccount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.glUnitMeasure = new DevExpress.XtraGrid.Columns.GridColumn();
            this.glQuantities = new DevExpress.XtraGrid.Columns.GridColumn();
            this.glUnitPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.glAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.glTaxPercentag = new DevExpress.XtraGrid.Columns.GridColumn();
            this.glTaxAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.glTaxAccount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.glDeleteColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.glSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.comboBoxEdit1 = new DevExpress.XtraEditors.ComboBoxEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.DERecord_date = new DevExpress.XtraEditors.DateEdit();
            this.DEDocumentDate = new DevExpress.XtraEditors.DateEdit();
            this.btnThamChieuPT = new DevExpress.XtraEditors.ButtonEdit();
            this.hlReceipt_id = new DevExpress.XtraEditors.HyperlinkLabelControl();
            this.ckNewcustomer = new DevExpress.XtraEditors.CheckEdit();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnSave_Print = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.cbMethods_payment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CkMoney_Received.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CkInvoice_Delivery.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CkReceipt_Delivery.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnThamChieuHD.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerTaxCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerAddress.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerNumberPhone.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcRecognitionDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvRecognition_Detail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DERecord_date.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DERecord_date.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEDocumentDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEDocumentDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnThamChieuPT.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckNewcustomer.Properties)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbMethods_payment
            // 
            this.cbMethods_payment.Location = new System.Drawing.Point(559, 164);
            this.cbMethods_payment.Margin = new System.Windows.Forms.Padding(8);
            this.cbMethods_payment.Name = "cbMethods_payment";
            this.cbMethods_payment.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbMethods_payment.Properties.Appearance.Options.UseFont = true;
            this.cbMethods_payment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbMethods_payment.Size = new System.Drawing.Size(290, 28);
            this.cbMethods_payment.TabIndex = 0;
            this.cbMethods_payment.SelectedIndexChanged += new System.EventHandler(this.cbMethods_payment_SelectedIndexChanged);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(36, 49);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(8);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(290, 28);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "HẠCH TOÁN DOANH THU";
            // 
            // CkMoney_Received
            // 
            this.CkMoney_Received.Location = new System.Drawing.Point(36, 166);
            this.CkMoney_Received.Margin = new System.Windows.Forms.Padding(8);
            this.CkMoney_Received.Name = "CkMoney_Received";
            this.CkMoney_Received.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CkMoney_Received.Properties.Appearance.Options.UseFont = true;
            this.CkMoney_Received.Properties.Caption = "Đã thu tiền";
            this.CkMoney_Received.Size = new System.Drawing.Size(210, 25);
            this.CkMoney_Received.TabIndex = 2;
            this.CkMoney_Received.CheckedChanged += new System.EventHandler(this.CkMoney_Received_CheckedChanged);
            // 
            // lbRecognitionId
            // 
            this.lbRecognitionId.Appearance.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRecognitionId.Appearance.Options.UseFont = true;
            this.lbRecognitionId.Location = new System.Drawing.Point(442, 49);
            this.lbRecognitionId.Margin = new System.Windows.Forms.Padding(8);
            this.lbRecognitionId.Name = "lbRecognitionId";
            this.lbRecognitionId.Size = new System.Drawing.Size(186, 28);
            this.lbRecognitionId.TabIndex = 1;
            this.lbRecognitionId.Text = "lbRecognitionId";
            // 
            // lbMethods_payment
            // 
            this.lbMethods_payment.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMethods_payment.Appearance.Options.UseFont = true;
            this.lbMethods_payment.Location = new System.Drawing.Point(284, 169);
            this.lbMethods_payment.Margin = new System.Windows.Forms.Padding(8);
            this.lbMethods_payment.Name = "lbMethods_payment";
            this.lbMethods_payment.Size = new System.Drawing.Size(155, 21);
            this.lbMethods_payment.TabIndex = 3;
            this.lbMethods_payment.Text = "Phương thức thu tiền";
            // 
            // CkInvoice_Delivery
            // 
            this.CkInvoice_Delivery.Location = new System.Drawing.Point(36, 249);
            this.CkInvoice_Delivery.Margin = new System.Windows.Forms.Padding(8);
            this.CkInvoice_Delivery.Name = "CkInvoice_Delivery";
            this.CkInvoice_Delivery.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CkInvoice_Delivery.Properties.Appearance.Options.UseFont = true;
            this.CkInvoice_Delivery.Properties.Caption = "Xuất kèm hóa đơn";
            this.CkInvoice_Delivery.Size = new System.Drawing.Size(282, 25);
            this.CkInvoice_Delivery.TabIndex = 2;
            this.CkInvoice_Delivery.CheckedChanged += new System.EventHandler(this.CkInvoice_Delivery_CheckedChanged);
            // 
            // CkReceipt_Delivery
            // 
            this.CkReceipt_Delivery.Location = new System.Drawing.Point(284, 249);
            this.CkReceipt_Delivery.Margin = new System.Windows.Forms.Padding(8);
            this.CkReceipt_Delivery.Name = "CkReceipt_Delivery";
            this.CkReceipt_Delivery.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CkReceipt_Delivery.Properties.Appearance.Options.UseFont = true;
            this.CkReceipt_Delivery.Properties.Caption = "Xuất kèm phiếu thu";
            this.CkReceipt_Delivery.Size = new System.Drawing.Size(314, 25);
            this.CkReceipt_Delivery.TabIndex = 2;
            this.CkReceipt_Delivery.CheckedChanged += new System.EventHandler(this.CkReceipt_Delivery_CheckedChanged);
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(35, 320);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(8);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(301, 28);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "THÔNG TIN KHÁCH HÀNG";
            // 
            // btnThamChieuHD
            // 
            this.btnThamChieuHD.EditValue = "Tham chiếu hóa đơn";
            this.btnThamChieuHD.Location = new System.Drawing.Point(1040, 47);
            this.btnThamChieuHD.Margin = new System.Windows.Forms.Padding(6);
            this.btnThamChieuHD.Name = "btnThamChieuHD";
            this.btnThamChieuHD.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThamChieuHD.Properties.Appearance.Options.UseFont = true;
            this.btnThamChieuHD.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.btnThamChieuHD.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnThamChieuHD.Size = new System.Drawing.Size(254, 28);
            this.btnThamChieuHD.TabIndex = 4;
            this.btnThamChieuHD.EditValueChanged += new System.EventHandler(this.btnThamChieuHD_EditValueChanged);
            this.btnThamChieuHD.Click += new System.EventHandler(this.btnThamChieuHD_Click);
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl4.Appearance.Options.UseFont = true;
            this.labelControl4.Location = new System.Drawing.Point(35, 396);
            this.labelControl4.Margin = new System.Windows.Forms.Padding(8);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(111, 21);
            this.labelControl4.TabIndex = 3;
            this.labelControl4.Text = "Mã khách hàng";
            this.labelControl4.Click += new System.EventHandler(this.labelControl4_Click_1);
            // 
            // txtCustomerId
            // 
            this.txtCustomerId.Location = new System.Drawing.Point(208, 394);
            this.txtCustomerId.Margin = new System.Windows.Forms.Padding(6);
            this.txtCustomerId.Name = "txtCustomerId";
            this.txtCustomerId.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCustomerId.Properties.Appearance.Options.UseFont = true;
            this.txtCustomerId.Size = new System.Drawing.Size(289, 28);
            this.txtCustomerId.TabIndex = 6;
            this.txtCustomerId.EditValueChanged += new System.EventHandler(this.txtCustomerId_EditValueChanged);
            // 
            // labelControl5
            // 
            this.labelControl5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl5.Appearance.Options.UseFont = true;
            this.labelControl5.Location = new System.Drawing.Point(35, 465);
            this.labelControl5.Margin = new System.Windows.Forms.Padding(8);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(117, 21);
            this.labelControl5.TabIndex = 3;
            this.labelControl5.Text = "Tên khách hàng";
            this.labelControl5.Click += new System.EventHandler(this.labelControl5_Click_1);
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Location = new System.Drawing.Point(208, 461);
            this.txtCustomerName.Margin = new System.Windows.Forms.Padding(6);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCustomerName.Properties.Appearance.Options.UseFont = true;
            this.txtCustomerName.Size = new System.Drawing.Size(289, 28);
            this.txtCustomerName.TabIndex = 6;
            this.txtCustomerName.EditValueChanged += new System.EventHandler(this.txtCustomerName_EditValueChanged);
            // 
            // labelControl6
            // 
            this.labelControl6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelControl6.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl6.Appearance.Options.UseFont = true;
            this.labelControl6.Location = new System.Drawing.Point(51, 534);
            this.labelControl6.Margin = new System.Windows.Forms.Padding(8);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(82, 21);
            this.labelControl6.TabIndex = 3;
            this.labelControl6.Text = "Mã số thuế";
            // 
            // txtCustomerTaxCode
            // 
            this.txtCustomerTaxCode.Location = new System.Drawing.Point(208, 530);
            this.txtCustomerTaxCode.Margin = new System.Windows.Forms.Padding(6);
            this.txtCustomerTaxCode.Name = "txtCustomerTaxCode";
            this.txtCustomerTaxCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCustomerTaxCode.Properties.Appearance.Options.UseFont = true;
            this.txtCustomerTaxCode.Size = new System.Drawing.Size(291, 28);
            this.txtCustomerTaxCode.TabIndex = 6;
            this.txtCustomerTaxCode.EditValueChanged += new System.EventHandler(this.txtCustomerTaxCode_EditValueChanged);
            // 
            // labelControl7
            // 
            this.labelControl7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelControl7.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl7.Appearance.Options.UseFont = true;
            this.labelControl7.Location = new System.Drawing.Point(588, 463);
            this.labelControl7.Margin = new System.Windows.Forms.Padding(8);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(51, 21);
            this.labelControl7.TabIndex = 3;
            this.labelControl7.Text = "Địa chỉ";
            // 
            // txtCustomerAddress
            // 
            this.txtCustomerAddress.Location = new System.Drawing.Point(694, 462);
            this.txtCustomerAddress.Margin = new System.Windows.Forms.Padding(6);
            this.txtCustomerAddress.Name = "txtCustomerAddress";
            this.txtCustomerAddress.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCustomerAddress.Properties.Appearance.Options.UseFont = true;
            this.txtCustomerAddress.Size = new System.Drawing.Size(819, 28);
            this.txtCustomerAddress.TabIndex = 6;
            this.txtCustomerAddress.EditValueChanged += new System.EventHandler(this.txtCustomerAddress_EditValueChanged);
            // 
            // labelControl8
            // 
            this.labelControl8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelControl8.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl8.Appearance.Options.UseFont = true;
            this.labelControl8.Location = new System.Drawing.Point(588, 398);
            this.labelControl8.Margin = new System.Windows.Forms.Padding(8);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(45, 21);
            this.labelControl8.TabIndex = 3;
            this.labelControl8.Text = "Số ĐT";
            this.labelControl8.Click += new System.EventHandler(this.labelControl8_Click_1);
            // 
            // txtCustomerNumberPhone
            // 
            this.txtCustomerNumberPhone.Location = new System.Drawing.Point(694, 394);
            this.txtCustomerNumberPhone.Margin = new System.Windows.Forms.Padding(6);
            this.txtCustomerNumberPhone.Name = "txtCustomerNumberPhone";
            this.txtCustomerNumberPhone.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCustomerNumberPhone.Properties.Appearance.Options.UseFont = true;
            this.txtCustomerNumberPhone.Size = new System.Drawing.Size(254, 28);
            this.txtCustomerNumberPhone.TabIndex = 6;
            this.txtCustomerNumberPhone.EditValueChanged += new System.EventHandler(this.txtCustomerNumberPhone_EditValueChanged);
            // 
            // hlInvocieID
            // 
            this.hlInvocieID.Appearance.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hlInvocieID.Appearance.Options.UseFont = true;
            this.hlInvocieID.Location = new System.Drawing.Point(1336, 47);
            this.hlInvocieID.Margin = new System.Windows.Forms.Padding(6);
            this.hlInvocieID.Name = "hlInvocieID";
            this.hlInvocieID.Size = new System.Drawing.Size(135, 28);
            this.hlInvocieID.TabIndex = 8;
            this.hlInvocieID.Text = "Mã hóa đơn";
            this.hlInvocieID.Click += new System.EventHandler(this.hlInvocieID_Click_1);
            // 
            // gcRecognitionDetail
            // 
            this.gcRecognitionDetail.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(6);
            this.gcRecognitionDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcRecognitionDetail.Location = new System.Drawing.Point(18, 590);
            this.gcRecognitionDetail.MainView = this.gvRecognition_Detail;
            this.gcRecognitionDetail.Margin = new System.Windows.Forms.Padding(6);
            this.gcRecognitionDetail.Name = "gcRecognitionDetail";
            this.gcRecognitionDetail.Size = new System.Drawing.Size(2041, 390);
            this.gcRecognitionDetail.TabIndex = 9;
            this.gcRecognitionDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvRecognition_Detail});
            this.gcRecognitionDetail.Click += new System.EventHandler(this.gcRecognitionDetail_Click);
            // 
            // gvRecognition_Detail
            // 
            this.gvRecognition_Detail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.glServiceName,
            this.glDebitAccount,
            this.glCreditAccount,
            this.glUnitMeasure,
            this.glQuantities,
            this.glUnitPrice,
            this.glAmount,
            this.glTaxPercentag,
            this.glTaxAmount,
            this.glTaxAccount,
            this.glDeleteColumn,
            this.glSTT});
            this.gvRecognition_Detail.DetailHeight = 682;
            this.gvRecognition_Detail.GridControl = this.gcRecognitionDetail;
            this.gvRecognition_Detail.Name = "gvRecognition_Detail";
            this.gvRecognition_Detail.OptionsEditForm.PopupEditFormWidth = 1562;
            this.gvRecognition_Detail.OptionsView.ColumnAutoWidth = false;
            // 
            // glServiceName
            // 
            this.glServiceName.Caption = "Tên dịch vụ";
            this.glServiceName.MinWidth = 49;
            this.glServiceName.Name = "glServiceName";
            this.glServiceName.Visible = true;
            this.glServiceName.VisibleIndex = 1;
            this.glServiceName.Width = 465;
            // 
            // glDebitAccount
            // 
            this.glDebitAccount.Caption = "TK nợ";
            this.glDebitAccount.MinWidth = 49;
            this.glDebitAccount.Name = "glDebitAccount";
            this.glDebitAccount.Visible = true;
            this.glDebitAccount.VisibleIndex = 2;
            this.glDebitAccount.Width = 286;
            // 
            // glCreditAccount
            // 
            this.glCreditAccount.Caption = "TK có";
            this.glCreditAccount.MinWidth = 49;
            this.glCreditAccount.Name = "glCreditAccount";
            this.glCreditAccount.Visible = true;
            this.glCreditAccount.VisibleIndex = 3;
            this.glCreditAccount.Width = 242;
            // 
            // glUnitMeasure
            // 
            this.glUnitMeasure.Caption = "ĐVT";
            this.glUnitMeasure.MinWidth = 49;
            this.glUnitMeasure.Name = "glUnitMeasure";
            this.glUnitMeasure.Visible = true;
            this.glUnitMeasure.VisibleIndex = 5;
            this.glUnitMeasure.Width = 161;
            // 
            // glQuantities
            // 
            this.glQuantities.Caption = "Số lượng";
            this.glQuantities.MinWidth = 39;
            this.glQuantities.Name = "glQuantities";
            this.glQuantities.Visible = true;
            this.glQuantities.VisibleIndex = 4;
            this.glQuantities.Width = 214;
            // 
            // glUnitPrice
            // 
            this.glUnitPrice.Caption = "Đơn giá";
            this.glUnitPrice.MinWidth = 39;
            this.glUnitPrice.Name = "glUnitPrice";
            this.glUnitPrice.Visible = true;
            this.glUnitPrice.VisibleIndex = 6;
            this.glUnitPrice.Width = 155;
            // 
            // glAmount
            // 
            this.glAmount.Caption = "Thành tiền";
            this.glAmount.MinWidth = 39;
            this.glAmount.Name = "glAmount";
            this.glAmount.Visible = true;
            this.glAmount.VisibleIndex = 7;
            this.glAmount.Width = 126;
            // 
            // glTaxPercentag
            // 
            this.glTaxPercentag.Caption = "% thuế";
            this.glTaxPercentag.MinWidth = 39;
            this.glTaxPercentag.Name = "glTaxPercentag";
            this.glTaxPercentag.Visible = true;
            this.glTaxPercentag.VisibleIndex = 8;
            this.glTaxPercentag.Width = 120;
            // 
            // glTaxAmount
            // 
            this.glTaxAmount.Caption = "Tiền thuế";
            this.glTaxAmount.MinWidth = 39;
            this.glTaxAmount.Name = "glTaxAmount";
            this.glTaxAmount.Visible = true;
            this.glTaxAmount.VisibleIndex = 9;
            this.glTaxAmount.Width = 155;
            // 
            // glTaxAccount
            // 
            this.glTaxAccount.Caption = "TK thuế";
            this.glTaxAccount.MinWidth = 39;
            this.glTaxAccount.Name = "glTaxAccount";
            this.glTaxAccount.Visible = true;
            this.glTaxAccount.VisibleIndex = 10;
            this.glTaxAccount.Width = 200;
            // 
            // glDeleteColumn
            // 
            this.glDeleteColumn.MinWidth = 39;
            this.glDeleteColumn.Name = "glDeleteColumn";
            this.glDeleteColumn.Visible = true;
            this.glDeleteColumn.VisibleIndex = 11;
            this.glDeleteColumn.Width = 499;
            // 
            // glSTT
            // 
            this.glSTT.Caption = "STT";
            this.glSTT.MinWidth = 39;
            this.glSTT.Name = "glSTT";
            this.glSTT.Visible = true;
            this.glSTT.VisibleIndex = 0;
            this.glSTT.Width = 127;
            // 
            // comboBoxEdit1
            // 
            this.comboBoxEdit1.Location = new System.Drawing.Point(1325, 531);
            this.comboBoxEdit1.Margin = new System.Windows.Forms.Padding(10);
            this.comboBoxEdit1.Name = "comboBoxEdit1";
            this.comboBoxEdit1.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxEdit1.Properties.Appearance.Options.UseFont = true;
            this.comboBoxEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.comboBoxEdit1.Size = new System.Drawing.Size(188, 28);
            this.comboBoxEdit1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1239, 533);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "Loại tiền";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(832, 276);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Ngày hạch toán";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(832, 320);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Ngày chứng từ";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // DERecord_date
            // 
            this.DERecord_date.EditValue = null;
            this.DERecord_date.Location = new System.Drawing.Point(966, 272);
            this.DERecord_date.Margin = new System.Windows.Forms.Padding(6);
            this.DERecord_date.Name = "DERecord_date";
            this.DERecord_date.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DERecord_date.Properties.Appearance.Options.UseFont = true;
            this.DERecord_date.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DERecord_date.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DERecord_date.Size = new System.Drawing.Size(199, 28);
            this.DERecord_date.TabIndex = 11;
            this.DERecord_date.EditValueChanged += new System.EventHandler(this.DERecord_date_EditValueChanged);
            // 
            // DEDocumentDate
            // 
            this.DEDocumentDate.EditValue = null;
            this.DEDocumentDate.Location = new System.Drawing.Point(966, 323);
            this.DEDocumentDate.Margin = new System.Windows.Forms.Padding(8);
            this.DEDocumentDate.Name = "DEDocumentDate";
            this.DEDocumentDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DEDocumentDate.Properties.Appearance.Options.UseFont = true;
            this.DEDocumentDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DEDocumentDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DEDocumentDate.Size = new System.Drawing.Size(199, 28);
            this.DEDocumentDate.TabIndex = 11;
            this.DEDocumentDate.EditValueChanged += new System.EventHandler(this.DEDocumentDate_EditValueChanged);
            // 
            // btnThamChieuPT
            // 
            this.btnThamChieuPT.EditValue = "Tham chiếu phiếu thu";
            this.btnThamChieuPT.Location = new System.Drawing.Point(1040, 117);
            this.btnThamChieuPT.Margin = new System.Windows.Forms.Padding(8);
            this.btnThamChieuPT.Name = "btnThamChieuPT";
            this.btnThamChieuPT.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThamChieuPT.Properties.Appearance.Options.UseFont = true;
            this.btnThamChieuPT.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.btnThamChieuPT.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnThamChieuPT.Size = new System.Drawing.Size(254, 28);
            this.btnThamChieuPT.TabIndex = 4;
            this.btnThamChieuPT.EditValueChanged += new System.EventHandler(this.btnThamChieuPT_EditValueChanged);
            this.btnThamChieuPT.Click += new System.EventHandler(this.btnThamChieuPT_Click);
            // 
            // hlReceipt_id
            // 
            this.hlReceipt_id.Appearance.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hlReceipt_id.Appearance.Options.UseFont = true;
            this.hlReceipt_id.Location = new System.Drawing.Point(1336, 117);
            this.hlReceipt_id.Margin = new System.Windows.Forms.Padding(8);
            this.hlReceipt_id.Name = "hlReceipt_id";
            this.hlReceipt_id.Size = new System.Drawing.Size(154, 28);
            this.hlReceipt_id.TabIndex = 8;
            this.hlReceipt_id.Text = "Mã phiếu thu";
            this.hlReceipt_id.Click += new System.EventHandler(this.hlReceipt_id_Click);
            // 
            // ckNewcustomer
            // 
            this.ckNewcustomer.Location = new System.Drawing.Point(559, 249);
            this.ckNewcustomer.Margin = new System.Windows.Forms.Padding(8);
            this.ckNewcustomer.Name = "ckNewcustomer";
            this.ckNewcustomer.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckNewcustomer.Properties.Appearance.Options.UseFont = true;
            this.ckNewcustomer.Properties.Caption = "Thêm khách hàng mới";
            this.ckNewcustomer.Size = new System.Drawing.Size(268, 25);
            this.ckNewcustomer.TabIndex = 2;
            this.ckNewcustomer.CheckedChanged += new System.EventHandler(this.ckNewcustomer_CheckedChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Right;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.btnSave_Print});
            this.toolStrip1.Location = new System.Drawing.Point(1685, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(71, 931);
            this.toolStrip1.TabIndex = 14;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSave
            // 
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(68, 44);
            this.btnSave.Text = "Cất";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSave_Print
            // 
            this.btnSave_Print.Image = ((System.Drawing.Image)(resources.GetObject("btnSave_Print.Image")));
            this.btnSave_Print.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave_Print.Name = "btnSave_Print";
            this.btnSave_Print.Size = new System.Drawing.Size(68, 44);
            this.btnSave_Print.Text = "Cất và In";
            this.btnSave_Print.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSave_Print.Click += new System.EventHandler(this.btnSave_Print_Click);
            // 
            // frmHachToan
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1756, 931);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.DEDocumentDate);
            this.Controls.Add(this.DERecord_date);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.gcRecognitionDetail);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.hlReceipt_id);
            this.Controls.Add(this.hlInvocieID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCustomerNumberPhone);
            this.Controls.Add(this.txtCustomerTaxCode);
            this.Controls.Add(this.txtCustomerAddress);
            this.Controls.Add(this.labelControl8);
            this.Controls.Add(this.txtCustomerName);
            this.Controls.Add(this.labelControl6);
            this.Controls.Add(this.labelControl7);
            this.Controls.Add(this.txtCustomerId);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.btnThamChieuPT);
            this.Controls.Add(this.btnThamChieuHD);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.lbMethods_payment);
            this.Controls.Add(this.ckNewcustomer);
            this.Controls.Add(this.CkReceipt_Delivery);
            this.Controls.Add(this.CkInvoice_Delivery);
            this.Controls.Add(this.CkMoney_Received);
            this.Controls.Add(this.lbRecognitionId);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.comboBoxEdit1);
            this.Controls.Add(this.cbMethods_payment);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmHachToan";
            this.Text = "HachToan";
            this.Load += new System.EventHandler(this.frmHachToan_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cbMethods_payment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CkMoney_Received.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CkInvoice_Delivery.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CkReceipt_Delivery.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnThamChieuHD.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerTaxCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerAddress.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerNumberPhone.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcRecognitionDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvRecognition_Detail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DERecord_date.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DERecord_date.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEDocumentDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEDocumentDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnThamChieuPT.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckNewcustomer.Properties)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.ComboBoxEdit cbMethods_payment;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.CheckEdit CkMoney_Received;
        private DevExpress.XtraEditors.LabelControl lbRecognitionId;
        private DevExpress.XtraEditors.LabelControl lbMethods_payment;
        private DevExpress.XtraEditors.CheckEdit CkInvoice_Delivery;
        private DevExpress.XtraEditors.CheckEdit CkReceipt_Delivery;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.ButtonEdit btnThamChieuHD;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit txtCustomerId;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.TextEdit txtCustomerName;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.TextEdit txtCustomerTaxCode;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.TextEdit txtCustomerAddress;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.TextEdit txtCustomerNumberPhone;
        private DevExpress.XtraEditors.HyperlinkLabelControl hlInvocieID;
        private DevExpress.XtraGrid.GridControl gcRecognitionDetail;
        private DevExpress.XtraGrid.Views.Grid.GridView gvRecognition_Detail;
        private DevExpress.XtraGrid.Columns.GridColumn glServiceName;
        private DevExpress.XtraGrid.Columns.GridColumn glDebitAccount;
        private DevExpress.XtraGrid.Columns.GridColumn glCreditAccount;
        private DevExpress.XtraGrid.Columns.GridColumn glUnitMeasure;
        private DevExpress.XtraGrid.Columns.GridColumn glQuantities;
        private DevExpress.XtraGrid.Columns.GridColumn glUnitPrice;
        private DevExpress.XtraGrid.Columns.GridColumn glAmount;
        private DevExpress.XtraGrid.Columns.GridColumn glTaxPercentag;
        private DevExpress.XtraGrid.Columns.GridColumn glTaxAmount;
        private DevExpress.XtraGrid.Columns.GridColumn glTaxAccount;
        private DevExpress.XtraGrid.Columns.GridColumn glDeleteColumn;
        private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private DevExpress.XtraEditors.DateEdit DERecord_date;
        private DevExpress.XtraEditors.DateEdit DEDocumentDate;
        private DevExpress.XtraEditors.ButtonEdit btnThamChieuPT;
        private DevExpress.XtraEditors.HyperlinkLabelControl hlReceipt_id;
        private DevExpress.XtraGrid.Columns.GridColumn glSTT;
        private DevExpress.XtraEditors.CheckEdit ckNewcustomer;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnSave_Print;
    }
}