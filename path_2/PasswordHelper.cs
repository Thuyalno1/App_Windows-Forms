using System;
using System.Security.Cryptography;
using System.Text;

namespace path_2
{
    /// <summary>
    /// Helper class để mã hóa password với SHA256 và Salt
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// Tạo Salt ngẫu nhiên
        /// </summary>
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// Hash password với Salt sử dụng SHA256
        /// </summary>
        public static string HashPassword(string password, string salt)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException(Constants.ValidationMessages.RequiredField("Password"));

            if (string.IsNullOrEmpty(salt))
                throw new ArgumentException(Constants.ValidationMessages.RequiredField("Salt"));

            // Kết hợp password + salt
            string saltedPassword = password + salt;

            // Hash bằng SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(saltedPassword);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// Verify password: So sánh password nhập vào với hash đã lưu
        /// </summary>
        public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            string hashOfInput = HashPassword(enteredPassword, storedSalt);
            return hashOfInput == storedHash;
        }
    }
}
