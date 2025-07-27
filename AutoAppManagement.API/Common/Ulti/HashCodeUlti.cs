using System.Text;
using System.Security.Cryptography;

namespace AutoAppManagement.API.Common.Ulti
{
    public class HashCodeUlti
    {
        /// <summary>
        /// Hàm encode password với md5
        /// CreatedBy ntthe 25.02.2024
        /// </summary>
        /// <param name="originalPassword"></param>
        /// <returns></returns>
        public static string EncodePassword(string originalPassword)
        {
            using (var md5 = MD5.Create())  // Thay MD5CryptoServiceProvider bằng MD5.Create()
            {
                var originalBytes = Encoding.Default.GetBytes(originalPassword);
                var encodedBytes = md5.ComputeHash(originalBytes);

                // Convert encoded bytes back to a 'readable' string
                return BitConverter.ToString(encodedBytes).Replace("-", "");  // Loại bỏ dấu "-" để trả về chuỗi gọn gàng hơn
            }
        }

    }
}
