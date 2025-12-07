namespace ChargingApp.Accounting
{
    partial class frmReportInput
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
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.cbPeriod = new DevExpress.XtraEditors.ComboBoxEdit();
            this.DEFromDate = new DevExpress.XtraEditors.DateEdit();
            this.DEToDate = new DevExpress.XtraEditors.DateEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.cbYear = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.ckEntryType = new DevExpress.XtraEditors.CheckEdit();
            this.gcReportType = new DevExpress.XtraGrid.GridControl();
            this.gvReportType = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colReportType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colReportTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.btnGenerate = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.cbPeriod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEFromDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEFromDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEToDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEToDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbYear.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckEntryType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcReportType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvReportType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(60, 51);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(81, 21);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Kỳ báo cáo";
            // 
            // cbPeriod
            // 
            this.cbPeriod.Location = new System.Drawing.Point(173, 49);
            this.cbPeriod.Margin = new System.Windows.Forms.Padding(4);
            this.cbPeriod.Name = "cbPeriod";
            this.cbPeriod.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbPeriod.Properties.Appearance.Options.UseFont = true;
            this.cbPeriod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbPeriod.Size = new System.Drawing.Size(179, 26);
            this.cbPeriod.TabIndex = 1;
            this.cbPeriod.SelectedIndexChanged += new System.EventHandler(this.cbPeriod_SelectedIndexChanged);
            // 
            // DEFromDate
            // 
            this.DEFromDate.EditValue = null;
            this.DEFromDate.Location = new System.Drawing.Point(173, 98);
            this.DEFromDate.Margin = new System.Windows.Forms.Padding(4);
            this.DEFromDate.Name = "DEFromDate";
            this.DEFromDate.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DEFromDate.Properties.Appearance.Options.UseFont = true;
            this.DEFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DEFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DEFromDate.Size = new System.Drawing.Size(179, 26);
            this.DEFromDate.TabIndex = 2;
            this.DEFromDate.EditValueChanged += new System.EventHandler(this.DEFromDate_EditValueChanged);
            // 
            // DEToDate
            // 
            this.DEToDate.EditValue = null;
            this.DEToDate.Location = new System.Drawing.Point(522, 98);
            this.DEToDate.Margin = new System.Windows.Forms.Padding(4);
            this.DEToDate.Name = "DEToDate";
            this.DEToDate.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DEToDate.Properties.Appearance.Options.UseFont = true;
            this.DEToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DEToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DEToDate.Size = new System.Drawing.Size(179, 26);
            this.DEToDate.TabIndex = 2;
            this.DEToDate.EditValueChanged += new System.EventHandler(this.DEToDate_EditValueChanged);
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(81, 100);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(60, 21);
            this.labelControl2.TabIndex = 0;
            this.labelControl2.Text = "Từ ngày";
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(421, 100);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(70, 21);
            this.labelControl3.TabIndex = 0;
            this.labelControl3.Text = "Đến ngày";
            // 
            // cbYear
            // 
            this.cbYear.Location = new System.Drawing.Point(522, 49);
            this.cbYear.Margin = new System.Windows.Forms.Padding(4);
            this.cbYear.Name = "cbYear";
            this.cbYear.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbYear.Properties.Appearance.Options.UseFont = true;
            this.cbYear.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbYear.Size = new System.Drawing.Size(179, 26);
            this.cbYear.TabIndex = 1;
            this.cbYear.SelectedIndexChanged += new System.EventHandler(this.cbYear_SelectedIndexChanged);
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl4.Appearance.Options.UseFont = true;
            this.labelControl4.Location = new System.Drawing.Point(457, 49);
            this.labelControl4.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(34, 21);
            this.labelControl4.TabIndex = 0;
            this.labelControl4.Text = "Năm";
            // 
            // ckEntryType
            // 
            this.ckEntryType.Location = new System.Drawing.Point(754, 51);
            this.ckEntryType.Name = "ckEntryType";
            this.ckEntryType.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckEntryType.Properties.Appearance.Options.UseFont = true;
            this.ckEntryType.Properties.Caption = "Gộp bút toán";
            this.ckEntryType.Size = new System.Drawing.Size(133, 24);
            this.ckEntryType.TabIndex = 3;
            // 
            // gcReportType
            // 
            this.gcReportType.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
            this.gcReportType.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcReportType.Location = new System.Drawing.Point(60, 162);
            this.gcReportType.MainView = this.gvReportType;
            this.gcReportType.Name = "gcReportType";
            this.gcReportType.Size = new System.Drawing.Size(836, 215);
            this.gcReportType.TabIndex = 4;
            this.gcReportType.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvReportType});
            // 
            // gvReportType
            // 
            this.gvReportType.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colReportType,
            this.colReportTypeName});
            this.gvReportType.DetailHeight = 333;
            this.gvReportType.GridControl = this.gcReportType;
            this.gvReportType.Name = "gvReportType";
            this.gvReportType.OptionsEditForm.PopupEditFormWidth = 889;
            // 
            // colReportType
            // 
            this.colReportType.Caption = "gridColumn1";
            this.colReportType.MinWidth = 28;
            this.colReportType.Name = "colReportType";
            this.colReportType.Visible = true;
            this.colReportType.VisibleIndex = 0;
            this.colReportType.Width = 144;
            // 
            // colReportTypeName
            // 
            this.colReportTypeName.Caption = "gridColumn2";
            this.colReportTypeName.MinWidth = 28;
            this.colReportTypeName.Name = "colReportTypeName";
            this.colReportTypeName.Visible = true;
            this.colReportTypeName.VisibleIndex = 1;
            this.colReportTypeName.Width = 662;
            // 
            // textEdit1
            // 
            this.textEdit1.Location = new System.Drawing.Point(189, 418);
            this.textEdit1.Name = "textEdit1";
            this.textEdit1.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textEdit1.Properties.Appearance.Options.UseFont = true;
            this.textEdit1.Size = new System.Drawing.Size(302, 26);
            this.textEdit1.TabIndex = 5;
            // 
            // labelControl7
            // 
            this.labelControl7.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl7.Appearance.Options.UseFont = true;
            this.labelControl7.Location = new System.Drawing.Point(71, 420);
            this.labelControl7.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(70, 21);
            this.labelControl7.TabIndex = 0;
            this.labelControl7.Text = "Người lập";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(717, 424);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(94, 29);
            this.btnGenerate.TabIndex = 7;
            this.btnGenerate.Text = "Tạo báo cáo";
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // frmReportInput
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 505);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.textEdit1);
            this.Controls.Add(this.gcReportType);
            this.Controls.Add(this.ckEntryType);
            this.Controls.Add(this.DEToDate);
            this.Controls.Add(this.DEFromDate);
            this.Controls.Add(this.cbYear);
            this.Controls.Add(this.cbPeriod);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl7);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.labelControl1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmReportInput";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmReportinput";
            ((System.ComponentModel.ISupportInitialize)(this.cbPeriod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEFromDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEFromDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEToDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEToDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbYear.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckEntryType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcReportType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvReportType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.ComboBoxEdit cbPeriod;
        private DevExpress.XtraEditors.DateEdit DEFromDate;
        private DevExpress.XtraEditors.DateEdit DEToDate;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.ComboBoxEdit cbYear;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.CheckEdit ckEntryType;
        private DevExpress.XtraGrid.GridControl gcReportType;
        private DevExpress.XtraGrid.Views.Grid.GridView gvReportType;
        private DevExpress.XtraGrid.Columns.GridColumn colReportType;
        private DevExpress.XtraGrid.Columns.GridColumn colReportTypeName;
        private DevExpress.XtraEditors.TextEdit textEdit1;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.SimpleButton btnGenerate;
    }
}