using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
/*
 using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace ChargingApp.Management
{
    public partial class frmReasonInput : DevExpress.XtraEditors.XtraForm
    {
        // Thuộc tính công khai để Form gọi lấy lý do sau khi xác nhận
        public string Reason { get; private set; }

        public frmReasonInput()
        {
            InitializeComponent();
            this.Text = "Giải trình Lý do Xóa";
            
            // Đảm bảo Form trả về kết quả
            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            // Kiểm tra: Bắt buộc phải nhập lý do
            if (string.IsNullOrWhiteSpace(memoReason.Text))
            {
                XtraMessageBox.Show(this, "Vui lòng nhập lý do giải trình.", "Lỗi Nhập Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                memoReason.Focus();
                return;
            }
            
            // Lưu lý do và đóng Form
            Reason = memoReason.Text.Trim();
            this.DialogResult = DialogResult.OK;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
 */
namespace ChargingApp.Management
{
    public partial class frmReasonInput : DevExpress.XtraEditors.XtraForm
    {
        // Thuộc tính công khai để Form gọi lấy lý do sau khi xác nhận
        public string Reason { get; private set; }

        public frmReasonInput()
        {
            InitializeComponent();
            this.Text = "Giải trình Lý do Xóa";

            // Đảm bảo Form trả về kết quả
            btnOK.Click += btnOK_Click;
            btnCancel.Click += btnCancel_Click;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Kiểm tra: Bắt buộc phải nhập lý do
            if (string.IsNullOrWhiteSpace(memoReason.Text))
            {
                XtraMessageBox.Show(this, "Vui lòng nhập lý do giải trình.", "Lỗi Nhập Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                memoReason.Focus();
                return;
            }

            // Lưu lý do và đóng Form
            Reason = memoReason.Text.Trim();
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
