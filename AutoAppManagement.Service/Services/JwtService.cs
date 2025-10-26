using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.ViewModel.Account;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AutoAppManagement.Service.Services
{
    public interface IJwtService
    {
        Models.DTO.Account.TokenDTO GenerateToken(Account account, LicenseInfoDTO? licenseInfo = null);
        ClaimsPrincipal? ValidateToken(string token);
        bool IsTokenExpired(string token);
        string GenerateRefreshToken();
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryMinutes;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            _secretKey = _configuration["Jwt:SecretKey"] ?? "AutoAppManagement_Secret_Key_2024_Very_Long_Secret_Key_For_Security";
            _issuer = _configuration["Jwt:Issuer"] ?? "AutoAppManagement";
            _audience = _configuration["Jwt:Audience"] ?? "AutoAppManagement.Client";
            _expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "1440"); // Default 24 hours
        }

        /// <summary>
        /// Tạo JWT token cho account
        /// </summary>
        /// <param name="account"></param>
        /// <param name="licenseInfo"></param>
        /// <returns></returns>
        public Models.DTO.Account.TokenDTO GenerateToken(Account account, LicenseInfoDTO? licenseInfo = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.ID.ToString()),
                new Claim(ClaimTypes.Name, account.UserName ?? ""),
                new Claim(ClaimTypes.Email, account.Email ?? ""),
                new Claim("UserId", account.ID.ToString()),
                new Claim("phone", account.Phone ?? ""),
                new Claim("fullName", account.Name ?? ""),
                new Claim("loginTime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
            };

            // Thêm thông tin license vào token nếu có
            if (licenseInfo != null)
            {
                claims.AddRange(new[]
                {
                    new Claim("licenseId", licenseInfo.LicenseId.ToString()),
                    new Claim("licenseKey", licenseInfo.LicenseKey),
                    new Claim("licenseName", licenseInfo.LicenseName),
                    new Claim("licenseType", licenseInfo.LicenseType),
                    new Claim("licenseStatus", licenseInfo.Status.ToString()),
                    new Claim("licenseStartDate", licenseInfo.StartDate.ToString("yyyy-MM-dd HH:mm:ss")),
                    new Claim("licenseEndDate", licenseInfo.EndDate.ToString("yyyy-MM-dd HH:mm:ss")),
                    new Claim("licenseDaysRemaining", licenseInfo.DaysRemaining.ToString())
                });
            }

            var tokenExpires = DateTime.UtcNow.AddMinutes(_expiryMinutes);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenExpires,
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new Models.DTO.Account.TokenDTO {
                AccessToken = tokenHandler.WriteToken(token),
                AccessTokenExpired = tokenExpires
            };
        }

        /// <summary>
        /// Validate JWT token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra token đã hết hạn chưa
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsTokenExpired(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                
                return jwtToken.ValidTo <= DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}