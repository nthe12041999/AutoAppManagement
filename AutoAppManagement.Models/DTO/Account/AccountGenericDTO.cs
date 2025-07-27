namespace AutoAppManagement.Models.DTO.Account
{
    public class AccountGenericDTO
    {
        public long AccountId { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string Language { get; set; } = "vi-VN";
        public List<string> RoleList { get; set; }
        public string ImgAvatar { get; set; }
        public string Email { get; set; }
    }
}
