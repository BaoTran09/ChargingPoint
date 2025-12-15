using System;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Templates;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace ChargingApp.Helpers
{
    public partial class frmSendEmail : DevExpress.XtraEditors.XtraForm
    {
        private string attachmentPath;
        private string recipientEmail;
        private bool isReminder;
        private long? invoiceId;
        private bool isReceipt;
        private decimal? invoiceAmount;
        private DateTime? dueDate;

        // Constructor đơn giản
        public frmSendEmail(string toEmail, string pdfPath, string subject, bool isReminder = false, bool isReceipt = false)
        {
            InitializeComponent();

            this.recipientEmail = toEmail;
            this.attachmentPath = pdfPath;
            this.isReminder = isReminder;
             this.isReceipt = isReceipt;

            InitializeForm(toEmail, subject);
        }

        // Constructor với đầy đủ thông tin (cho reminder email)
        public frmSendEmail(
            string toEmail,
            string toName,
            long invoiceId,
            decimal invoiceAmount,
            DateTime dueDate,
            string pdfPath)
        {
            InitializeComponent();

            this.recipientEmail = toEmail;
            this.attachmentPath = pdfPath;
            this.isReminder = true;
            this.invoiceId = invoiceId;
            this.invoiceAmount = invoiceAmount;
            this.dueDate = dueDate;

            InitializeFormForReminder(toEmail, toName, invoiceId, invoiceAmount, dueDate);
        }

        private void InitializeForm(string toEmail, string subject)
        {
            // Set recipient
            txtTo.Text = toEmail;

            // Set subject
            txtSubject.Text = subject;
            // Load email body template
            if (isReceipt)
            {
                // Load Receipt template
                // Có thể load từ DB hoặc dùng GetReceiptEmailBody()
                txtBody.Text = GetReceiptDefaultBody();
            }
            else if (isReminder)
            {
                // Load Reminder template
                txtBody.Text = GetReminderEmailBody();
            }
            else
            {
                // Load Invoice template
                txtBody.Text = GetInvoiceEmailBody();
            }


            // Display attachment name
            DisplayAttachment();

            // Wire up events
            btnSend.Click += BtnSend_Click;
            btnCancel.Click += BtnCancel_Click;
            btnTestConnection.Click += BtnTestConnection_Click;
        }
        private string GetReceiptDefaultBody()
        {
                        return @"Kính gửi Quý khách,

            Cảm ơn Quý khách đã thanh toán. Chúng tôi xin xác nhận đã nhận được khoản thanh toán của Quý khách.

            Phiếu thu chi tiết được đính kèm theo email này.

            Nếu có bất kỳ thắc mắc nào, xin vui lòng liên hệ với chúng tôi.

            Trân trọng,
            CÔNG TY CỔ PHẦN PHÁT TRIỂN TRẠM SẠC TOÀN CẦU V-GREEN";
        }
        private void InitializeFormForReminder(
            string toEmail,
            string toName,
            long invoiceId,
            decimal amount,
            DateTime dueDate)
        {
            txtTo.Text = toEmail;
            txtSubject.Text = $"Nhắc nhở thanh toán - Hóa đơn #{invoiceId}";

            txtBody.Text = $@"Kính gửi {toName},

                    Chúng tôi xin gửi đến Quý khách thông báo nhắc nhở thanh toán hóa đơn:

                    - Số hóa đơn: #{invoiceId}
                    - Số tiền: {amount:N0} VND
                    - Hạn thanh toán: {dueDate:dd/MM/yyyy}
                    - Trạng thái: Chưa thanh toán

                    Vui lòng thanh toán hóa đơn trong thời gian sớm nhất để tránh phát sinh phí phạt.

                    Chi tiết hóa đơn xin xem file đính kèm.

                    Mọi thắc mắc xin vui lòng liên hệ hotline: 1900 1234

                    Trân trọng cảm ơn!
                    CÔNG TY CỔ PHẦN PHÁT TRIỂN TRẠM SẠC TOÀN CẦU V-GREEN";

            DisplayAttachment();

            btnSend.Click += BtnSend_Click;
            btnCancel.Click += BtnCancel_Click;
            btnTestConnection.Click += BtnTestConnection_Click;
        }

        private string GetInvoiceEmailBody()
        {
            return @"Kính gửi Quý khách hàng,

                Chúng tôi xin gửi đến Quý khách hóa đơn thanh toán dịch vụ sạc điện.

                Chi tiết hóa đơn xin xem file đính kèm.

                Mọi thắc mắc xin vui lòng liên hệ:
                - Hotline: 1900 1234
                - Email: support@vgreen.com

                Trân trọng cảm ơn!
                CÔNG TY CỔ PHẦN PHÁT TRIỂN TRẠM SẠC TOÀN CẦU V-GREEN";
        }

        private string GetReminderEmailBody()
        {
            return @"Kính gửi Quý khách hàng,

                Chúng tôi xin gửi đến Quý khách hóa đơn thanh toán dịch vụ sạc điện đã quá hạn.

                Vui lòng thanh toán hóa đơn trong thời gian sớm nhất để tránh phát sinh phí phạt.

                Chi tiết hóa đơn xin xem file đính kèm.

                Mọi thắc mắc xin vui lòng liên hệ hotline: 1900 1234

                Trân trọng cảm ơn!
                CÔNG TY CỔ PHẦN PHÁT TRIỂN TRẠM SẠC TOÀN CẦU V-GREEN";
        }

        private void DisplayAttachment()
        {
            if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
            {
                lblAttachmentName.Text = "📎 " + Path.GetFileName(attachmentPath);
            }
            else
            {
                lblAttachmentName.Text = "Không có file đính kèm";
            }
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                btnSend.Enabled = false;
                btnSend.Text = "Đang gửi...";
                this.Cursor = Cursors.WaitCursor;

                // Tạo EmailMessage
                var emailMessage = new EmailMessage
                {
                    To = txtTo.Text.Trim(),
                    Subject = txtSubject.Text.Trim(),
                    Body = txtBody.Text,
                    IsHtml = false
                };

                // Thêm attachment nếu có
                if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                {
                    emailMessage.AttachmentPaths.Add(attachmentPath);
                }

                // Gửi email qua EmailHelper
                bool success = EmailHelper.SendEmail(emailMessage, out string errorMessage);

                if (success)
                {
                    XtraMessageBox.Show(
                        "Gửi email thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    XtraMessageBox.Show(
                        errorMessage ?? "Lỗi không xác định khi gửi email!",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Lỗi gửi email: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnSend.Enabled = true;
                btnSend.Text = "Gửi Email";
                this.Cursor = Cursors.Default;
            }
        }

        private bool ValidateForm()
        {
            // Validate email
            if (string.IsNullOrWhiteSpace(txtTo.Text))
            {
                XtraMessageBox.Show("Vui lòng nhập email người nhận!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTo.Focus();
                return false;
            }

            if (!EmailHelper.IsValidEmail(txtTo.Text.Trim()))
            {
                XtraMessageBox.Show("Email không hợp lệ!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTo.Focus();
                return false;
            }

            // Validate subject
            if (string.IsNullOrWhiteSpace(txtSubject.Text))
            {
                XtraMessageBox.Show("Vui lòng nhập tiêu đề email!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSubject.Focus();
                return false;
            }

            // Validate body
            if (string.IsNullOrWhiteSpace(txtBody.Text))
            {
                XtraMessageBox.Show("Vui lòng nhập nội dung email!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBody.Focus();
                return false;
            }

            return true;
        }

        private void BtnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                btnTestConnection.Enabled = false;
                btnTestConnection.Text = "Đang test...";

                bool success = EmailHelper.TestConnection(out string errorMessage);

                if (success)
                {
                    XtraMessageBox.Show(
                        "Kết nối SMTP thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    XtraMessageBox.Show(
                        $"Kết nối SMTP thất bại!\n\n{errorMessage}",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Lỗi test connection: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnTestConnection.Enabled = true;
                btnTestConnection.Text = "Test Connection";
                this.Cursor = Cursors.Default;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}