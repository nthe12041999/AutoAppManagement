using Microsoft.AspNetCore.Authorization;

namespace AutoAppManagement.WebApp.Common.Attribute
{
    public class RolesAttribute : AuthorizeAttribute
    {
        public RolesAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}
