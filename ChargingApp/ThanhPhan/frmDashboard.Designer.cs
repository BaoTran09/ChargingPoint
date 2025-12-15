namespace ChargingApp.ThanhPhan
{
    partial class frmDashboard
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
            this.LineChartRevenue = new DevExpress.XtraCharts.ChartControl();
            this.PieChartPaymentStatus = new DevExpress.XtraCharts.ChartControl();
            this.DEfromDate = new DevExpress.XtraEditors.DateEdit();
            this.DEtoDate = new DevExpress.XtraEditors.DateEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.tablePanel1 = new DevExpress.Utils.Layout.TablePanel();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl11 = new DevExpress.XtraEditors.LabelControl();
            this.lbOverdueAmount = new DevExpress.XtraEditors.LabelControl();
            this.lbPaidAmount = new DevExpress.XtraEditors.LabelControl();
            this.lbUnpaidAmount = new DevExpress.XtraEditors.LabelControl();
            this.lbRevenueAmount = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.tablePanel2 = new DevExpress.Utils.Layout.TablePanel();
            this.ColChartCompareStation = new DevExpress.XtraCharts.ChartControl();
            this.PieChartChargerUseMost = new DevExpress.XtraCharts.ChartControl();
            ((System.ComponentModel.ISupportInitialize)(this.LineChartRevenue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PieChartPaymentStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEfromDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEfromDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEtoDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEtoDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tablePanel1)).BeginInit();
            this.tablePanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tablePanel2)).BeginInit();
            this.tablePanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ColChartCompareStation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PieChartChargerUseMost)).BeginInit();
            this.SuspendLayout();
            // 
            // LineChartRevenue
            // 
            this.LineChartRevenue.Location = new System.Drawing.Point(883, 12);
            this.LineChartRevenue.Name = "LineChartRevenue";
            this.LineChartRevenue.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.LineChartRevenue.Size = new System.Drawing.Size(521, 444);
            this.LineChartRevenue.TabIndex = 4;
            this.LineChartRevenue.Visible = false;
            // 
            // PieChartPaymentStatus
            // 
            this.PieChartPaymentStatus.Location = new System.Drawing.Point(925, 35);
            this.PieChartPaymentStatus.Name = "PieChartPaymentStatus";
            this.PieChartPaymentStatus.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.PieChartPaymentStatus.Size = new System.Drawing.Size(630, 378);
            this.PieChartPaymentStatus.TabIndex = 5;
            this.PieChartPaymentStatus.Visible = false;
            // 
            // DEfromDate
            // 
            this.DEfromDate.EditValue = null;
            this.DEfromDate.Location = new System.Drawing.Point(157, 74);
            this.DEfromDate.Name = "DEfromDate";
            this.DEfromDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DEfromDate.Properties.Appearance.Options.UseFont = true;
            this.DEfromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DEfromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DEfromDate.Size = new System.Drawing.Size(191, 28);
            this.DEfromDate.TabIndex = 6;
            this.DEfromDate.EditValueChanged += new System.EventHandler(this.DateEdit_EditValueChanged);
            // 
            // DEtoDate
            // 
            this.DEtoDate.EditValue = null;
            this.DEtoDate.Location = new System.Drawing.Point(471, 74);
            this.DEtoDate.Name = "DEtoDate";
            this.DEtoDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DEtoDate.Properties.Appearance.Options.UseFont = true;
            this.DEtoDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DEtoDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DEtoDate.Size = new System.Drawing.Size(207, 28);
            this.DEtoDate.TabIndex = 6;
            this.DEtoDate.EditValueChanged += new System.EventHandler(this.DateEdit_EditValueChanged);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(373, 77);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(70, 21);
            this.labelControl1.TabIndex = 7;
            this.labelControl1.Text = "Đến ngày";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(71, 77);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(60, 21);
            this.labelControl2.TabIndex = 7;
            this.labelControl2.Text = "Từ ngày";
            // 
            // tablePanel1
            // 
            this.tablePanel1.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 34.52F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 25.48F)});
            this.tablePanel1.Controls.Add(this.labelControl7);
            this.tablePanel1.Controls.Add(this.labelControl11);
            this.tablePanel1.Controls.Add(this.lbOverdueAmount);
            this.tablePanel1.Controls.Add(this.lbPaidAmount);
            this.tablePanel1.Controls.Add(this.lbUnpaidAmount);
            this.tablePanel1.Controls.Add(this.lbRevenueAmount);
            this.tablePanel1.Controls.Add(this.labelControl6);
            this.tablePanel1.Controls.Add(this.labelControl5);
            this.tablePanel1.Controls.Add(this.labelControl4);
            this.tablePanel1.Controls.Add(this.labelControl3);
            this.tablePanel1.Location = new System.Drawing.Point(60, 144);
            this.tablePanel1.Name = "tablePanel1";
            this.tablePanel1.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 77.2F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 32.39999F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 31.6F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 29.20001F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 36.39999F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F)});
            this.tablePanel1.Size = new System.Drawing.Size(633, 251);
            this.tablePanel1.TabIndex = 9;
            this.tablePanel1.UseSkinIndents = true;
            // 
            // labelControl7
            // 
            this.labelControl7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelControl7.Location = new System.Drawing.Point(364, 71);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(69, 16);
            this.labelControl7.TabIndex = 9;
            this.labelControl7.Text = "Đơn vị: VND";
            // 
            // labelControl11
            // 
            this.labelControl11.Appearance.Font = new System.Drawing.Font("Tahoma", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl11.Appearance.Options.UseFont = true;
            this.labelControl11.Location = new System.Drawing.Point(15, 30);
            this.labelControl11.Name = "labelControl11";
            this.labelControl11.Size = new System.Drawing.Size(309, 40);
            this.labelControl11.TabIndex = 8;
            this.labelControl11.Text = "Tình hình tài chính";
            // 
            // lbOverdueAmount
            // 
            this.lbOverdueAmount.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbOverdueAmount.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(164)))), ((int)(((byte)(71)))));
            this.lbOverdueAmount.Appearance.Options.UseFont = true;
            this.lbOverdueAmount.Appearance.Options.UseForeColor = true;
            this.lbOverdueAmount.Dock = System.Windows.Forms.DockStyle.Right;
            this.lbOverdueAmount.Location = new System.Drawing.Point(492, 184);
            this.lbOverdueAmount.Name = "lbOverdueAmount";
            this.lbOverdueAmount.Size = new System.Drawing.Size(126, 21);
            this.lbOverdueAmount.TabIndex = 7;
            this.lbOverdueAmount.Text = "labelControl10";
            // 
            // lbPaidAmount
            // 
            this.lbPaidAmount.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPaidAmount.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(164)))), ((int)(((byte)(71)))));
            this.lbPaidAmount.Appearance.Options.UseFont = true;
            this.lbPaidAmount.Appearance.Options.UseForeColor = true;
            this.lbPaidAmount.Dock = System.Windows.Forms.DockStyle.Right;
            this.lbPaidAmount.Location = new System.Drawing.Point(503, 155);
            this.lbPaidAmount.Name = "lbPaidAmount";
            this.lbPaidAmount.Size = new System.Drawing.Size(115, 21);
            this.lbPaidAmount.TabIndex = 6;
            this.lbPaidAmount.Text = "labelControl9";
            // 
            // lbUnpaidAmount
            // 
            this.lbUnpaidAmount.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbUnpaidAmount.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(164)))), ((int)(((byte)(71)))));
            this.lbUnpaidAmount.Appearance.Options.UseFont = true;
            this.lbUnpaidAmount.Appearance.Options.UseForeColor = true;
            this.lbUnpaidAmount.Dock = System.Windows.Forms.DockStyle.Right;
            this.lbUnpaidAmount.Location = new System.Drawing.Point(503, 123);
            this.lbUnpaidAmount.Name = "lbUnpaidAmount";
            this.lbUnpaidAmount.Size = new System.Drawing.Size(115, 21);
            this.lbUnpaidAmount.TabIndex = 5;
            this.lbUnpaidAmount.Text = "labelControl8";
            // 
            // lbRevenueAmount
            // 
            this.lbRevenueAmount.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRevenueAmount.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(164)))), ((int)(((byte)(71)))));
            this.lbRevenueAmount.Appearance.Options.UseFont = true;
            this.lbRevenueAmount.Appearance.Options.UseForeColor = true;
            this.lbRevenueAmount.Dock = System.Windows.Forms.DockStyle.Right;
            this.lbRevenueAmount.Location = new System.Drawing.Point(503, 91);
            this.lbRevenueAmount.Name = "lbRevenueAmount";
            this.lbRevenueAmount.Size = new System.Drawing.Size(115, 21);
            this.lbRevenueAmount.TabIndex = 4;
            this.lbRevenueAmount.Text = "labelControl7";
            // 
            // labelControl6
            // 
            this.labelControl6.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl6.Appearance.Options.UseFont = true;
            this.labelControl6.Location = new System.Drawing.Point(15, 189);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(150, 21);
            this.labelControl6.TabIndex = 3;
            this.labelControl6.Text = "Phải thu KH quá hạn";
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl5.Appearance.Options.UseFont = true;
            this.labelControl5.Location = new System.Drawing.Point(15, 157);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(66, 21);
            this.labelControl5.TabIndex = 2;
            this.labelControl5.Text = "Thực thu";
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl4.Appearance.Options.UseFont = true;
            this.labelControl4.Location = new System.Drawing.Point(15, 126);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(149, 21);
            this.labelControl4.TabIndex = 1;
            this.labelControl4.Text = "Phải thu khách hàng";
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(15, 94);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(81, 21);
            this.labelControl3.TabIndex = 0;
            this.labelControl3.Text = "Doanh Thu";
            // 
            // labelControl8
            // 
            this.labelControl8.Appearance.Font = new System.Drawing.Font("Tahoma", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl8.Appearance.Options.UseFont = true;
            this.labelControl8.Location = new System.Drawing.Point(12, 1);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(336, 40);
            this.labelControl8.TabIndex = 8;
            this.labelControl8.Text = "Dashboard thống kê";
            // 
            // tablePanel2
            // 
            this.tablePanel2.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 55.64F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 0.68F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 53.68F)});
            this.tablePanel2.Controls.Add(this.PieChartChargerUseMost);
            this.tablePanel2.Controls.Add(this.ColChartCompareStation);
            this.tablePanel2.Location = new System.Drawing.Point(39, 419);
            this.tablePanel2.Name = "tablePanel2";
            this.tablePanel2.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 31.60003F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F)});
            this.tablePanel2.Size = new System.Drawing.Size(1374, 510);
            this.tablePanel2.TabIndex = 10;
            this.tablePanel2.UseSkinIndents = true;
            // 
            // ColChartCompareStation
            // 
            this.ColChartCompareStation.Location = new System.Drawing.Point(15, 48);
            this.ColChartCompareStation.Name = "ColChartCompareStation";
            this.ColChartCompareStation.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.ColChartCompareStation.Size = new System.Drawing.Size(675, 444);
            this.ColChartCompareStation.TabIndex = 3;
            // 
            // PieChartChargerUseMost
            // 
            this.PieChartChargerUseMost.AppearanceNameSerializable = "Northern Lights";
            this.PieChartChargerUseMost.Location = new System.Drawing.Point(711, 48);
            this.PieChartChargerUseMost.Name = "PieChartChargerUseMost";
            this.PieChartChargerUseMost.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.PieChartChargerUseMost.Size = new System.Drawing.Size(648, 444);
            this.PieChartChargerUseMost.TabIndex = 2;
            // 
            // frmDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1434, 1102);
            this.Controls.Add(this.tablePanel2);
            this.Controls.Add(this.LineChartRevenue);
            this.Controls.Add(this.labelControl8);
            this.Controls.Add(this.tablePanel1);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.DEtoDate);
            this.Controls.Add(this.DEfromDate);
            this.Controls.Add(this.PieChartPaymentStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmDashboard";
            this.Text = "FrmDashboard";
            this.Load += new System.EventHandler(this.frmDashboard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.LineChartRevenue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PieChartPaymentStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEfromDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEfromDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEtoDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DEtoDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tablePanel1)).EndInit();
            this.tablePanel1.ResumeLayout(false);
            this.tablePanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tablePanel2)).EndInit();
            this.tablePanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ColChartCompareStation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PieChartChargerUseMost)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraCharts.ChartControl LineChartRevenue;
        private DevExpress.XtraCharts.ChartControl PieChartPaymentStatus;
        private DevExpress.XtraEditors.DateEdit DEfromDate;
        private DevExpress.XtraEditors.DateEdit DEtoDate;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.Utils.Layout.TablePanel tablePanel1;
        private DevExpress.XtraEditors.LabelControl lbOverdueAmount;
        private DevExpress.XtraEditors.LabelControl lbPaidAmount;
        private DevExpress.XtraEditors.LabelControl lbUnpaidAmount;
        private DevExpress.XtraEditors.LabelControl lbRevenueAmount;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl11;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.Utils.Layout.TablePanel tablePanel2;
        private DevExpress.XtraCharts.ChartControl PieChartChargerUseMost;
        private DevExpress.XtraCharts.ChartControl ColChartCompareStation;
    }
}