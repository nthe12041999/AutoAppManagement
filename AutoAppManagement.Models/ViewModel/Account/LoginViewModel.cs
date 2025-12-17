namespace AutoAppManagement.Models.ViewModel.Account
{
    public class TokenDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpired { get; set; }
    }

    public class LoginViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
    }

    public class TokenViewModel: TokenDTO
    {
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpired { get; set; }
        public BaseEntity.AdminAccount AccountInfor { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
