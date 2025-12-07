namespace ChargingApp.Accounting
{
    partial class frmGeneralJournal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGeneralJournal));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.gcGeneralJournal = new DevExpress.XtraGrid.GridControl();
            this.gvGeneralJournal = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnExxportExcel = new System.Windows.Forms.ToolStripButton();
            this.btnExxportPdf = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnSelectAgain = new System.Windows.Forms.ToolStripButton();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.lbPeriod = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.gcGeneralJournal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvGeneralJournal)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(595, 29);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(286, 34);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "SỔ NHẬT KÝ CHUNG";
            // 
            // gcGeneralJournal
            // 
            this.gcGeneralJournal.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcGeneralJournal.Location = new System.Drawing.Point(24, 134);
            this.gcGeneralJournal.MainView = this.gvGeneralJournal;
            this.gcGeneralJournal.Name = "gcGeneralJournal";
            this.gcGeneralJournal.Size = new System.Drawing.Size(1420, 750);
            this.gcGeneralJournal.TabIndex = 1;
            this.gcGeneralJournal.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvGeneralJournal});
            // 
            // gvGeneralJournal
            // 
            this.gvGeneralJournal.GridControl = this.gcGeneralJournal;
            this.gvGeneralJournal.Name = "gvGeneralJournal";
            this.gvGeneralJournal.OptionsEditForm.PopupEditFormWidth = 700;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Right;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnExxportExcel,
            this.btnExxportPdf,
            this.btnSave,
            this.btnSelectAgain,
            this.btnRefresh});
            this.toolStrip1.Location = new System.Drawing.Point(1471, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(85, 812);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnExxportExcel
            // 
            this.btnExxportExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnExxportExcel.Image")));
            this.btnExxportExcel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExxportExcel.Name = "btnExxportExcel";
            this.btnExxportExcel.Size = new System.Drawing.Size(82, 44);
            this.btnExxportExcel.Text = "Xuất Excel";
            this.btnExxportExcel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnExxportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
            // 
            // btnExxportPdf
            // 
            this.btnExxportPdf.Image = ((System.Drawing.Image)(resources.GetObject("btnExxportPdf.Image")));
            this.btnExxportPdf.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExxportPdf.Name = "btnExxportPdf";
            this.btnExxportPdf.Size = new System.Drawing.Size(82, 44);
            this.btnExxportPdf.Text = "Xuất PDF";
            this.btnExxportPdf.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnExxportPdf.Click += new System.EventHandler(this.btnExportPdf_Click);
            // 
            // btnSave
            // 
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(82, 44);
            this.btnSave.Text = "Lưu";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSelectAgain
            // 
            this.btnSelectAgain.Image = ((System.Drawing.Image)(resources.GetObject("btnSelectAgain.Image")));
            this.btnSelectAgain.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelectAgain.Name = "btnSelectAgain";
            this.btnSelectAgain.Size = new System.Drawing.Size(82, 44);
            this.btnSelectAgain.Text = "Quay lại";
            this.btnSelectAgain.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSelectAgain.Click += new System.EventHandler(this.btnSelectAgain_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(82, 44);
            this.btnRefresh.Text = "btnRefresh";
            this.btnRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lbPeriod
            // 
            this.lbPeriod.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPeriod.Appearance.Options.UseFont = true;
            this.lbPeriod.Location = new System.Drawing.Point(688, 69);
            this.lbPeriod.Name = "lbPeriod";
            this.lbPeriod.Size = new System.Drawing.Size(91, 21);
            this.lbPeriod.TabIndex = 3;
            this.lbPeriod.Text = "Kỳ kế toán";
            // 
            // frmGeneralJournal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1556, 812);
            this.Controls.Add(this.lbPeriod);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.gcGeneralJournal);
            this.Controls.Add(this.labelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmGeneralJournal";
            this.Text = "BaoCao";
            this.Load += new System.EventHandler(this.frmGeneralJournal_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.gcGeneralJournal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvGeneralJournal)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraGrid.GridControl gcGeneralJournal;
        private DevExpress.XtraGrid.Views.Grid.GridView gvGeneralJournal;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnExxportExcel;
        private System.Windows.Forms.ToolStripButton btnExxportPdf;
        private System.Windows.Forms.ToolStripButton btnSave;
        private DevExpress.XtraEditors.LabelControl lbPeriod;
        private System.Windows.Forms.ToolStripButton btnSelectAgain;
        private System.Windows.Forms.ToolStripButton btnRefresh;
    }
}