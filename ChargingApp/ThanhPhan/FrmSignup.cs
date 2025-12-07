using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using ChargingApp.Helpers;

namespace ChargingApp.ThanhPhan
{
    public partial class FrmSignup : Form
    {
        public FrmSignup()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private void Cb_showpass_CheckedChanged(object sender, EventArgs e)
        {
            // Hiển thị hoặc ẩn mật khẩu
            txtPassword.PasswordChar = cb_showpass.Checked ? '\0' : '*';
        }
        private void btnSignup_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPass.Text;

            // Kiểm tra dữ liệu nhập
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Email không hợp lệ!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtEmail.Focus();
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConfirmPass.Focus();
                return;
            }

            if (IsEmailExists(email))
            {
                MessageBox.Show("Email đã tồn tại trong hệ thống!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtEmail.Focus();
                return;
            }

            if (IsUsernameExists(username))
            {
                MessageBox.Show("Tên đăng nhập đã tồn tại!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUsername.Focus();
                return;
            }

            // Tạo tài khoản mới
            if (CreateNewAccount(email, username, password))
            {
                MessageBox.Show("Đăng ký tài khoản thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại sau!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsEmailExists(string email)
        {
            string query = "SELECT COUNT(*) FROM User_App WHERE Email = @Email";
            var parameter = new SqlParameter("@Email", email);

            try
            {
                int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters: parameter));
                return count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kiểm tra email: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
        }

        private bool IsUsernameExists(string username)
        {
            string query = "SELECT COUNT(*) FROM User_App WHERE Username = @Username";
            var parameter = new SqlParameter("@Username", username);

            try
            {
                int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters: parameter));
                return count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kiểm tra tên đăng nhập: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
        }

        private bool CreateNewAccount(string email, string username, string password)
        {
            string query = @"
                INSERT INTO User_App (Email, Username, PasswordHash, Status, CreatedAt, UpdatedAt)
                VALUES (@Email, @Username, @PasswordHash, @Status, @CreatedAt, @UpdatedAt)";

            // Tạo mật khẩu hash
            string passwordHash = SecurityHelper.HashPassword(password);

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Email", email),
                new SqlParameter("@Username", username),
                new SqlParameter("@PasswordHash", passwordHash),
                new SqlParameter("@Status", "active"), // Trạng thái mặc định
                new SqlParameter("@CreatedAt", DateTime.UtcNow),
                new SqlParameter("@UpdatedAt", DateTime.UtcNow)
            };

            try
            {
                int result = DatabaseHelper.ExecuteNonQuery(query, parameters: parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo tài khoản: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void lnkLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void link_Signup_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ThanhPhan.FrmLogin loginForm = new ThanhPhan.FrmLogin();
            loginForm.Show();
            this.Hide();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
/*

private void link_Signup_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
{
    FrmLogin loginForm = new FrmLogin();
    loginForm.Show();
    this.Hide();

}

private void btnClose_Click(object sender, EventArgs e)
{
    if (MessageBox.Show("Are you sure you want to exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
    {
        Application.Exit();
    }
}*/

