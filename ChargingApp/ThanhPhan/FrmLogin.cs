using ChargingApp.ThanhPhan;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChargingApp.Helpers;
using ChargingApp.Helper;
using System.Data.SqlClient;

namespace ChargingApp.ThanhPhan

{
    public partial class FrmLogin : DevExpress.XtraEditors.XtraForm
    {

        public FrmLogin()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
           // this.FormBorderStyle = FormBorderStyle.None;
            
            // Gán sự kiện cho checkbox hiển thị mật khẩu
            cb_showpass.CheckedChanged += Cb_showpass_CheckedChanged;
            
            // Gán sự kiện cho nút đăng nhập
            btnLogin.Click += BtnLogin_Click;
            
            // Cho phép nhấn Enter để đăng nhập
            txtPassword.KeyDown += TxtPassword_KeyDown;
        }

    
        private void link_Signup_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmSignup signupForm = new FrmSignup();
            signupForm.Show();
            //this.Hide();
        }
       
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            // Kiểm tra dữ liệu nhập
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu", 
                    "Lỗi đăng nhập", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {

                // Kiểm tra thông tin đăng nhập
                if (AuthenticateUser(username, password))
                {
                    GlobalSession.CurrentUserAppId = UserHelper.GetUserAppId(username);

                    // Lấy EmployeeId
                    GlobalSession.CurrentEmployeeId =   UserHelper.GetEmployeeIdFromUser(GlobalSession.CurrentUserAppId);

                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng", 
                        "Đăng nhập thất bại", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng nhập: {ex.Message}", 
                    "Lỗi", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            string query = "SELECT PasswordHash FROM User_App WHERE Username = @Username";
            var parameter = new System.Data.SqlClient.SqlParameter("@Username", username);

            try
            {
                // Lấy mật khẩu đã hash từ database
                string storedHash = DatabaseHelper.ExecuteScalar(query, parameters: parameter)?.ToString();

                if (string.IsNullOrEmpty(storedHash))
                    return false;

                // Xác thực mật khẩu
                return SecurityHelper.VerifyPassword(password, storedHash);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xác thực người dùng: " + ex.Message);
            }
        }

        private void Cb_showpass_CheckedChanged(object sender, EventArgs e)
        {
            // Hiển thị hoặc ẩn mật khẩu
            txtPassword.PasswordChar = cb_showpass.Checked ? '\0' : '*';
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            // Nhấn Enter để đăng nhập
            if (e.KeyCode == Keys.Enter)
            {
                BtnLogin_Click(this, new EventArgs());
            }
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
