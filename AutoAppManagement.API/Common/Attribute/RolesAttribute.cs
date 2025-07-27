using Microsoft.AspNetCore.Authorization;

namespace AutoAppManagement.API.Common.Attribute
{
    public class RolesAttribute : AuthorizeAttribute
    {
        public RolesAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}
