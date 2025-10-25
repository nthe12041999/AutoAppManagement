using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AutoAppManagement.Models.Common;

namespace AutoAppManagement.Service.Services
{
    public interface IEmailService
    {
        Task<BaseResponse> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
        Task<BaseResponse> SendPasswordResetEmailAsync(string toEmail, string newPassword, string userName);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<BaseResponse> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                // Lấy cấu hình SMTP từ appsettings
                var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var smtpUsername = _configuration["EmailSettings:Username"] ?? "";
                var smtpPassword = _configuration["EmailSettings:Password"] ?? "";
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? smtpUsername;
                var fromName = _configuration["EmailSettings:FromName"] ?? "AutoApp Management";

                if (string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
                {
                    return BaseResponse.Error("Cấu hình email chưa được thiết lập");
                }

                using var client = new SmtpClient(smtpHost, smtpPort);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);

                _logger.LogInformation($"Email sent successfully to {toEmail}");
                return BaseResponse.Success("Email đã được gửi thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
                return BaseResponse.Error($"Lỗi khi gửi email: {ex.Message}");
            }
        }

        public async Task<BaseResponse> SendPasswordResetEmailAsync(string toEmail, string newPassword, string userName)
        {
            try
            {
                var subject = "Đặt lại mật khẩu - AutoApp Management";
                
                var body = $@"
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
                            .content {{ background-color: #f8f9fa; padding: 30px; border-radius: 0 0 5px 5px; }}
                            .password-box {{ background-color: #e9ecef; padding: 15px; border-radius: 5px; margin: 20px 0; text-align: center; }}
                            .password {{ font-size: 18px; font-weight: bold; color: #007bff; letter-spacing: 2px; }}
                            .warning {{ color: #dc3545; font-weight: bold; margin-top: 20px; }}
                            .footer {{ margin-top: 30px; font-size: 12px; color: #6c757d; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h2>Đặt lại mật khẩu</h2>
                            </div>
                            <div class='content'>
                                <p>Xin chào <strong>{userName}</strong>,</p>
                                
                                <p>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Mật khẩu mới của bạn là:</p>
                                
                                <div class='password-box'>
                                    <div class='password'>{newPassword}</div>
                                </div>
                                
                                <div class='warning'>
                                    ⚠️ Vì lý do bảo mật, vui lòng đổi mật khẩu ngay sau khi đăng nhập thành công.
                                </div>
                                
                                <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng liên hệ với chúng tôi ngay lập tức.</p>
                                
                                <div class='footer'>
                                    <p>Trân trọng,<br>Đội ngũ AutoApp Management</p>
                                    <p>Email này được gửi tự động, vui lòng không trả lời.</p>
                                </div>
                            </div>
                        </div>
                    </body>
                    </html>";

                return await SendEmailAsync(toEmail, subject, body, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send password reset email to {toEmail}");
                return BaseResponse.Error($"Lỗi khi gửi email đặt lại mật khẩu: {ex.Message}");
            }
        }
    }
}
