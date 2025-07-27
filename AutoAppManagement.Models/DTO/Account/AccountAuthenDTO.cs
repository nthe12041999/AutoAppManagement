using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace AutoAppManagement.Models.DTO.Account
{
    public class AccountAuthenDTO
    {
        public ClaimsIdentity ClaimsIdentity { get; set; }
        public AuthenticationProperties AuthProperties { get; set; }
        public string UserName { get; set; }
    }
}
