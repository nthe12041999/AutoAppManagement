using AutoAppManagement.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.Account
{
    public class RefreshTokenDTO : BaseEntity.RefreshToken, IStatefulDTO
    {
        public EntityState State { get; set; }
    }

    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Refresh token không được để trống")]
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpired { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpired { get; set; }
    }

    public class RevokeTokenRequest
    {
        [Required(ErrorMessage = "Token không được để trống")]
        public string Token { get; set; } = string.Empty;
    }
}
