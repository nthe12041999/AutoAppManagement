using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.Verification;
using AutoAppManagement.Models.Enum;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Repository.Repositories.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutoAppManagement.Service.Services
{
    public interface IVerificationService
    {
        Task<BaseResponse> SendOtpAsync(SendOtpRequest request);
        Task<BaseResponse> VerifyOtpAsync(VerifyOtpRequest request);
        Task<BaseResponse> ResendOtpAsync(string email, VerificationType type);
        bool ValidateVerificationToken(string token, out string email, out VerificationType type);
    }

    public class VerificationService : IVerificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IServiceProvider _serviceProvider;
        private IGenericRepository<VerificationCode>? _verificationRepository;
        private IAccountsRepository? _accountRepository;

        protected IGenericRepository<VerificationCode> VerificationRepository
            => _verificationRepository ??= _unitOfWork.GetRepository<VerificationCode>();

        protected IAccountsRepository AccountRepository
            => _accountRepository ??= _unitOfWork.AccountsRepository;

        public VerificationService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _serviceProvider = serviceProvider;
        }

        public async Task<BaseResponse> SendOtpAsync(SendOtpRequest request)
        {
            try
            {
                // Kiểm tra email có tồn tại không (đối với ForgotPassword và ChangePassword)
                if (request.Type != VerificationType.Register)
                {
                    var account = await AccountRepository.FirstOrDefault(a => a.Email == request.Email && a.Status == StatusEnum.Active);
                    if (account == null)
                    {
                        return BaseResponse.Error("Email không tồn tại trong hệ thống");
                    }
                }

                // Xóa các OTP cũ chưa sử dụng của email này
                var oldOtps = await VerificationRepository.GetByCondition(v =>
                    v.Email == request.Email &&
                    v.Type == request.Type &&
                    !v.IsUsed &&
                    v.Status == StatusEnum.Active);

                foreach (var oldOtp in oldOtps)
                {
                    oldOtp.Status = StatusEnum.Inactive;
                }

                // Tạo OTP mới (6 chữ số)
                var otpCode = GenerateOtpCode();
                var verification = new VerificationCode
                {
                    Email = request.Email,
                    Code = otpCode,
                    Type = request.Type,
                    ExpiryDate = DateTime.UtcNow.AddMinutes(10), // OTP có hiệu lực 10 phút
                    IsUsed = false,
                    Status = StatusEnum.Active
                };

                verification.SetCreated();
                await VerificationRepository.Insert(verification);
                await _unitOfWork.SaveAsync();

                // Gửi email OTP
                var purposeText = request.Type.ToString();
                var emailResult = await _emailService.SendOtpEmailAsync(request.Email, otpCode, purposeText);

                if (!emailResult.IsSuccess)
                {
                    return BaseResponse.Error("Không thể gửi email OTP. Vui lòng thử lại sau.");
                }

                return BaseResponse.Success(new
                {
                    Email = request.Email,
                    ExpiryMinutes = 10
                }, "Mã OTP đã được gửi đến email của bạn");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gửi OTP: {ex.Message}");
            }
        }

        public async Task<BaseResponse> VerifyOtpAsync(VerifyOtpRequest request)
        {
            try
            {
                // Tìm OTP
                var verification = await VerificationRepository.FirstOrDefault(v =>
                    v.Email == request.Email &&
                    v.Code == request.Code &&
                    v.Type == request.Type &&
                    !v.IsUsed &&
                    v.Status == StatusEnum.Active);

                if (verification == null)
                {
                    return BaseResponse.Error("Mã OTP không hợp lệ");
                }

                // Kiểm tra hết hạn
                if (verification.ExpiryDate < DateTime.UtcNow)
                {
                    return BaseResponse.Error("Mã OTP đã hết hạn");
                }

                // Đánh dấu OTP đã sử dụng
                verification.IsUsed = true;
                verification.UsedDate = DateTime.UtcNow;
                verification.SetUpdated();
                await _unitOfWork.SaveAsync();

                // Tạo token để thực hiện action tiếp theo
                var token = GenerateVerificationToken(request.Email, request.Type);

                return BaseResponse.Success(new VerifyOtpResponse
                {
                    IsValid = true,
                    Message = "Xác thực thành công",
                    Token = token
                }, "Xác thực OTP thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi xác thực OTP: {ex.Message}");
            }
        }

        public async Task<BaseResponse> ResendOtpAsync(string email, VerificationType type)
        {
            try
            {
                // Kiểm tra xem có OTP nào đang active không
                var existingOtp = await VerificationRepository.FirstOrDefault(v =>
                    v.Email == email &&
                    v.Type == type &&
                    !v.IsUsed &&
                    v.Status == StatusEnum.Active &&
                    v.ExpiryDate > DateTime.UtcNow);

                if (existingOtp != null)
                {
                    // Nếu OTP còn hiệu lực, không cho gửi lại
                    var remainingMinutes = (int)(existingOtp.ExpiryDate - DateTime.UtcNow).TotalMinutes;
                    return BaseResponse.Error($"Vui lòng đợi {remainingMinutes} phút trước khi gửi lại OTP");
                }

                // Gửi OTP mới
                return await SendOtpAsync(new SendOtpRequest
                {
                    Email = email,
                    Type = type
                });
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gửi lại OTP: {ex.Message}");
            }
        }

        private string GenerateOtpCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string GenerateVerificationToken(string email, VerificationType type)
        {
            // Tạo token JWT đơn giản để verify
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("YourSecretKeyForVerificationTokenAtLeast32Characters!");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("email", email),
                    new Claim("type", type.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(30), // Token có hiệu lực 30 phút
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateVerificationToken(string token, out string email, out VerificationType type)
        {
            email = string.Empty;
            type = VerificationType.Register;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("YourSecretKeyForVerificationTokenAtLeast32Characters!");

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                email = jwtToken.Claims.First(x => x.Type == "email").Value;
                type = Enum.Parse<VerificationType>(jwtToken.Claims.First(x => x.Type == "type").Value);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
