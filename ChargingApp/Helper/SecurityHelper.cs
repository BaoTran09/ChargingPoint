using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace ChargingApp.Helpers
{
    public static class SecurityHelper
    {
        // Tạo salt ngẫu nhiên
        private static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // Tạo password hash
        public static string HashPassword(string password, string salt = null)
        {
            if (string.IsNullOrEmpty(salt))
            {
                salt = GenerateSalt();
            }

            using (var sha256 = SHA256.Create())
            {
                // Kết hợp password và salt
                string saltedPassword = password + salt;
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));

                // Trả về chuỗi hash kèm theo salt, phân cách bởi dấu :
                return $"{Convert.ToBase64String(hashedBytes)}:{salt}";
            }
        }

        // Xác minh mật khẩu
        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash))
                return false;

            // Tách hash và salt từ chuỗi lưu trữ
            var parts = storedHash.Split(':');
            if (parts.Length != 2)
                return false;

            string storedHashValue = parts[0];
            string salt = parts[1];

            // Tạo hash mới từ mật khẩu nhập vào và salt đã lưu
            string newHash = HashPassword(password, salt).Split(':')[0];

            // So sánh hash mới tạo với hash đã lưu
            return string.Equals(storedHashValue, newHash, StringComparison.Ordinal);
        }
    }
}