using System.Security.Cryptography;
using System.Text;

namespace AutoAppManagement.Service.Common.Ulti
{
    public static class PasswordGenerator
    {
        private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string DigitChars = "0123456789";
        private const string SpecialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        /// <summary>
        /// Tạo mật khẩu mạnh với độ dài và yêu cầu tùy chỉnh
        /// </summary>
        /// <param name="length">Độ dài mật khẩu (tối thiểu 8 ký tự)</param>
        /// <param name="includeUppercase">Bao gồm chữ hoa</param>
        /// <param name="includeLowercase">Bao gồm chữ thường</param>
        /// <param name="includeDigits">Bao gồm số</param>
        /// <param name="includeSpecialChars">Bao gồm ký tự đặc biệt</param>
        /// <returns>Mật khẩu được tạo</returns>
        public static string GenerateStrongPassword(
            int length = 12,
            bool includeUppercase = true,
            bool includeLowercase = true,
            bool includeDigits = true,
            bool includeSpecialChars = true)
        {
            if (length < 8)
                throw new ArgumentException("Độ dài mật khẩu phải ít nhất 8 ký tự");

            var characterSet = new StringBuilder();
            var requiredChars = new List<char>();

            // Xây dựng bộ ký tự có thể sử dụng
            if (includeUppercase)
            {
                characterSet.Append(UppercaseChars);
                requiredChars.Add(GetRandomChar(UppercaseChars));
            }

            if (includeLowercase)
            {
                characterSet.Append(LowercaseChars);
                requiredChars.Add(GetRandomChar(LowercaseChars));
            }

            if (includeDigits)
            {
                characterSet.Append(DigitChars);
                requiredChars.Add(GetRandomChar(DigitChars));
            }

            if (includeSpecialChars)
            {
                characterSet.Append(SpecialChars);
                requiredChars.Add(GetRandomChar(SpecialChars));
            }

            if (characterSet.Length == 0)
                throw new ArgumentException("Phải chọn ít nhất một loại ký tự");

            var allChars = characterSet.ToString();
            var password = new StringBuilder();

            // Thêm các ký tự bắt buộc trước
            foreach (var requiredChar in requiredChars)
            {
                password.Append(requiredChar);
            }

            // Thêm các ký tự ngẫu nhiên còn lại
            for (int i = requiredChars.Count; i < length; i++)
            {
                password.Append(GetRandomChar(allChars));
            }

            // Trộn các ký tự để tránh pattern có thể đoán được
            return ShuffleString(password.ToString());
        }

        /// <summary>
        /// Tạo mật khẩu mặc định với độ mạnh cao
        /// </summary>
        /// <returns>Mật khẩu mạnh 12 ký tự</returns>
        public static string GenerateDefaultStrongPassword()
        {
            return GenerateStrongPassword(12, true, true, true, true);
        }

        /// <summary>
        /// Lấy ký tự ngẫu nhiên từ chuỗi ký tự
        /// </summary>
        private static char GetRandomChar(string chars)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var randomIndex = Math.Abs(BitConverter.ToInt32(bytes, 0)) % chars.Length;
            return chars[randomIndex];
        }

        /// <summary>
        /// Trộn các ký tự trong chuỗi
        /// </summary>
        private static string ShuffleString(string input)
        {
            var array = input.ToCharArray();
            using var rng = RandomNumberGenerator.Create();
            
            for (int i = array.Length - 1; i > 0; i--)
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                var j = Math.Abs(BitConverter.ToInt32(bytes, 0)) % (i + 1);
                
                // Swap
                (array[i], array[j]) = (array[j], array[i]);
            }
            
            return new string(array);
        }

        /// <summary>
        /// Kiểm tra độ mạnh của mật khẩu
        /// </summary>
        /// <param name="password">Mật khẩu cần kiểm tra</param>
        /// <returns>Điểm số từ 0-100</returns>
        public static int CheckPasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
                return 0;

            int score = 0;

            // Độ dài
            if (password.Length >= 8) score += 25;
            if (password.Length >= 12) score += 10;
            if (password.Length >= 16) score += 10;

            // Có chữ thường
            if (password.Any(char.IsLower)) score += 10;

            // Có chữ hoa
            if (password.Any(char.IsUpper)) score += 10;

            // Có số
            if (password.Any(char.IsDigit)) score += 10;

            // Có ký tự đặc biệt
            if (password.Any(c => SpecialChars.Contains(c))) score += 15;

            // Không có pattern lặp lại
            if (!HasRepeatingPattern(password)) score += 10;

            return Math.Min(score, 100);
        }

        /// <summary>
        /// Kiểm tra xem mật khẩu có pattern lặp lại không
        /// </summary>
        private static bool HasRepeatingPattern(string password)
        {
            // Kiểm tra 3 ký tự liên tiếp giống nhau
            for (int i = 0; i < password.Length - 2; i++)
            {
                if (password[i] == password[i + 1] && password[i + 1] == password[i + 2])
                    return true;
            }

            // Kiểm tra pattern ABC hoặc 123
            for (int i = 0; i < password.Length - 2; i++)
            {
                if ((password[i + 1] == password[i] + 1) && (password[i + 2] == password[i + 1] + 1))
                    return true;
            }

            return false;
        }
    }
}
