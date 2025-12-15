using System;
using DevExpress.XtraReports.UI;

namespace ChargingApp.Report
{
    public partial class InvoiceReport : DevExpress.XtraReports.UI.XtraReport
    {
        public InvoiceReport()
        {
            InitializeComponent();

            // Thiết lập Detail Band để hiển thị InvoiceDetail
            SetupDetailBand();
            

        }

        private void SetupDetailBand()
        {
            // Đảm bảo Detail band sử dụng đúng data member
            if (this.Detail != null)
            {
                // Detail band sẽ tự động lặp qua InvoiceDetail rows
                // khi DataMember được set từ frmChungTu
            }
        }

        // Custom function để convert số thành chữ (nếu dùng trong Expression)
        public static string ConvertNumberToWords(object value)
        {
            if (value == null || value == DBNull.Value)
                return "Không";

            decimal number = Convert.ToDecimal(value);
            return NumberToVietnameseWords((long)number);
        }

        private static string NumberToVietnameseWords(long number)
        {
            if (number == 0) return "Không";
            if (number < 0) return "Âm " + NumberToVietnameseWords(-number);

            string[] ones = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };

            string ReadThreeDigits(int n)
            {
                if (n == 0) return "";

                int h = n / 100;
                int t = (n % 100) / 10;
                int o = n % 10;

                string result = "";

                if (h > 0)
                    result = ones[h] + " trăm";

                if (t > 0 || o > 0)
                {
                    if (t == 0 && h > 0)
                        result += " linh";
                    else if (t == 1)
                        result += " mười";
                    else if (t > 1)
                        result += " " + ones[t] + " mươi";

                    if (o > 0)
                    {
                        if (t > 1 && o == 1)
                            result += " mốt";
                        else if (t > 0 && o == 5)
                            result += " lăm";
                        else
                            result += " " + ones[o];
                    }
                }

                return result.Trim();
            }

            string res = "";

            if (number >= 1_000_000_000)
            {
                res += ReadThreeDigits((int)(number / 1_000_000_000)) + " tỷ";
                number %= 1_000_000_000;
            }

            if (number >= 1_000_000)
            {
                res += " " + ReadThreeDigits((int)(number / 1_000_000)) + " triệu";
                number %= 1_000_000;
            }

            if (number >= 1000)
            {
                res += " " + ReadThreeDigits((int)(number / 1000)) + " nghìn";
                number %= 1000;
            }

            if (number > 0)
                res += " " + ReadThreeDigits((int)number);

            res = res.Trim();
            return res.Length > 0 ? char.ToUpper(res[0]) + res.Substring(1) + " đồng" : "Không đồng";
        }

        // Event handler cho BeforePrint nếu cần
        private void InvoiceReport_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Có thể thêm logic xử lý trước khi in ở đây
        }
    }
}