using System;
using System.Security.Cryptography;
using System.Text;

namespace ApiService.Core
{
    public class Util
    {
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static bool VerifyPassword(string inputPassword, string storedPasswordHash)
        {
            // Trong thực tế, bạn nên sử dụng một thuật toán băm mạnh như BCrypt hoặc PBKDF2
            // Đây là một ví dụ sử dụng PBKDF2 (một thuật toán băm mạnh hơn MD5)

            // Giả sử storedPasswordHash có định dạng salt:hash
            // Trong thực tế, bạn cần lưu salt và hash riêng biệt hoặc theo một định dạng nhất định

            // Đây chỉ là một ví dụ đơn giản, trong môi trường thực tế cần triển khai đầy đủ
            try
            {
                // Đối với mục đích demo, chúng ta sẽ so sánh trực tiếp
                // Trong thực tế, bạn nên sử dụng thư viện như BCrypt.Net
                return storedPasswordHash.Equals(HashPassword(inputPassword));
            }
            catch
            {
                return false;
            }
        }

        public static string HashPassword(string password)
        {
            // Đây là một ví dụ đơn giản về PBKDF2
            // Trong thực tế, bạn nên sử dụng một thư viện chuyên dụng

            // Đối với mục đích demo, chúng ta sẽ sử dụng SHA256
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
