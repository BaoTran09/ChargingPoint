using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChargingApp.Accounting;
using ChargingApp.Management;
using DevExpress.XtraBars.Navigation;


namespace ChargingApp.ThanhPhan
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        frmDashboard frmDashboard;//= new FrmDashboard();
        Accounting.frmGeneralJournal frmBaoCao;//= new Accounting.BaoCao();
        Accounting.frmChungTu frmChungTu;
        Accounting.frmDSChungTu frmDSChungTu;
        Accounting.frmGeneralJournal frmGeneralJournal;
        ThanhPhan.FrmMain FrmMain;//= new Management.Customer();

        Management.frmCharger frmCharger;//= new Management.Customer();
        Management.frmUser frmUser;//= new Management.Customer();
        Accounting.frmHachToan frmHachToan;//= new Accounting.frmHachToan();
        public Form1()
        {
            InitializeComponent();
            this.IsMdiContainer = true; 

            this.WindowState = FormWindowState.Maximized;
            mdiProperties();
        }
        private void mdiProperties()
        {
            this.SetBevel(false);

            var mdiClient = Controls.OfType<MdiClient>().FirstOrDefault();
            if (mdiClient != null)
            {
                mdiClient.BackColor = Color.FromArgb(232, 234, 237);
            }
        }

        private void btnManagement_Click(object sender, EventArgs e)
        {
            ManaTransition.Start();
        }

        
        bool manaExpand=false;
       private void ManaTransition_Tick(object sender, EventArgs e)
{
    if (!manaExpand)
    {
        flPl_ManagementContainer.Height += 10; // Increased step for faster animation
        
        if (flPl_ManagementContainer.Height >= 220) // Changed == to >= for safety
        {
            flPl_ManagementContainer.Height = 220; // Ensure exact height
            ManaTransition.Stop();
            manaExpand = true;
        }
    }
    else
    {
        flPl_ManagementContainer.Height -= 10; // Increased step for faster animation
        
        if (flPl_ManagementContainer.Height <= 60) // Slightly higher than 55 to ensure it stops
        {
            flPl_ManagementContainer.Height = 55; // Collapsed height
            ManaTransition.Stop();
            manaExpand = false;
        }
    }
}
        private void btnAccounting_Click(object sender, EventArgs e)
        {
            accountTransition.Start();
        }
        bool accountExpand = false;
        private void accountTransition_Tick(object sender, EventArgs e)
        {
            if (accountExpand == false)
            {

                flPl_AccountingContainer.Height += 5;

                if (flPl_AccountingContainer.Height >= 220)
                {
                    flPl_AccountingContainer.Height = 220;

                    accountTransition.Stop();

                    accountExpand = true;

                }
            }
            else
            {
                flPl_AccountingContainer.Height -= 5;
                if (flPl_AccountingContainer.Height <= 55)
                {
                     flPl_AccountingContainer.Height = 55;

                    accountTransition.Stop();
                    accountExpand = false;

                }
            }

        }



        bool sidebarExpand = true;
        private void sidebarTransition_Tick(object sender, EventArgs e)
        {
            if (sidebarExpand)
            {

                flPlSidebar.Width -= 5;

                if (flPlSidebar.Width <= 48)
                {
                    sidebarExpand = false;
                    sidebarTransition.Stop();


                    pnnothing.Width = flPlSidebar.Width;
                    pnDashboard.Width = flPlSidebar.Width;
                    // pnAccounting.Width = flPlSidebar.Width;
                    pnManagement.Width = flPlSidebar.Width;
                    pnLogout.Width = flPlSidebar.Width;
                    flPl_AccountingContainer.Width = flPlSidebar.Width;
                    flPl_ManagementContainer.Width = flPlSidebar.Width;

                }
            }
            else
            {
                flPlSidebar.Width += 5;
                if (flPlSidebar.Width >= 245)
                {
                    sidebarExpand = true;
                    sidebarTransition.Stop();

                    pnnothing.Width = flPlSidebar.Width;
                    pnDashboard.Width = flPlSidebar.Width;
                   // pnAccounting.Width = flPlSidebar.Width;
                    pnManagement.Width = flPlSidebar.Width;
                    pnLogout.Width = flPlSidebar.Width;
                    flPl_AccountingContainer.Width = flPlSidebar.Width;
                    flPl_ManagementContainer.Width = flPlSidebar.Width;

                }
            }
        }

        private void btnHam_Click(object sender, EventArgs e)
        {
            sidebarTransition.Start();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            if (frmDashboard == null)
            {
                frmDashboard = new ThanhPhan.frmDashboard();
                frmDashboard.FormClosed += frmDashboard_FormClosed;
                frmDashboard.MdiParent = this;
                frmDashboard.Show();
            }
            else
            {
                frmDashboard.Activate();
            }
        }
        private void frmDashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmDashboard =null;
        }
        private void btnUser_Click(object sender, EventArgs e)
        {
            if(frmUser==null)
            {
                frmUser = new Management.frmUser();
                frmUser.FormClosed += FrmUser_FormClosed;
                frmUser.MdiParent = this;
                frmUser.Dock = DockStyle.Fill;
                frmUser.Show();
            }
            else
            {
                frmUser.Activate();
            }
        }

        private void FrmUser_FormClosed(object sender, FormClosedEventArgs e)
        {
            btnUser = null;
        }
        private void btnButToan_Click(object sender, EventArgs e)
        {
            if (frmHachToan  == null)
            {
                frmHachToan = new Accounting.frmHachToan();
                frmHachToan.FormClosed += frmHachToan_FormClosed;
                frmHachToan.MdiParent = this;
                frmHachToan.Dock = DockStyle.Fill;
                frmHachToan.Show();
            }
            else
            {
                frmHachToan.Activate();
            }
        }
        private void frmHachToan_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmHachToan = null;
        }
        private void btnclose_Click(object sender, EventArgs e)
        {

        }
        private void btnCharger_Click(object sender, EventArgs e)
        {
            if (frmCharger == null)
            {
                frmCharger = new Management.frmCharger();
                frmCharger.FormClosed += frmCharger_FormClosed;
                frmCharger.MdiParent = this;
                frmCharger.Dock = DockStyle.Fill;

                frmCharger.Show();
            }
            else
            {
                frmCharger.Activate();
            }
        }
        private void frmCharger_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmCharger = null;
        }
        // mở frmChungTu mới
        private void btnChungTu_Click(object sender, EventArgs e)
        {
            if (frmDSChungTu == null)
            {
                frmDSChungTu = new Accounting.frmDSChungTu(); // Dùng mặc định invoiceId
                frmDSChungTu.FormClosed += frmDSChungTu_FormClosed;
                frmDSChungTu.MdiParent = this;
                frmDSChungTu.Dock = DockStyle.Fill;
                frmDSChungTu.Show();
            }
            else
            {
                frmDSChungTu.Activate();
            }

        }


        private void frmDSChungTu_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmDSChungTu = null;
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            // Mở form GeneralJournal trước
            if (frmGeneralJournal == null)
            {
                frmGeneralJournal = new Accounting.frmGeneralJournal();
                frmGeneralJournal.FormClosed += frmGeneralJournal_FormClosed;
                frmGeneralJournal.MdiParent = this;
                frmGeneralJournal.Show();

                // Sau đó load frmReportInput để chọn điều kiện
                frmGeneralJournal.ShowReportInputDialog();
            }
            else
            {
                frmGeneralJournal.Activate();
            }
        }
        private void frmGeneralJournal_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmGeneralJournal = null;
        }

        private void btnMain_Click(object sender, EventArgs e)
        {
            if (FrmMain == null)
            {
                FrmMain = new ThanhPhan.FrmMain();
                FrmMain.FormClosed += FrmMain_FormClosed;
                FrmMain.MdiParent = this;
                FrmMain.Show();

                FrmMain.Dock = DockStyle.Fill;
                FrmMain.Show();
            }
            else
            {
                FrmMain.Activate();
            }

        }


        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            FrmMain = null;
        }



    }
}
