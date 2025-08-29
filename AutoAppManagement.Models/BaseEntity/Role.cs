using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity;

public partial class Role : BaseEntity
{
    public string RoleName { get; set; }

    public string RoleDescription { get; set; }

    public virtual ICollection<RoleAccount> RoleAccounts { get; set; } = new List<RoleAccount>();
}
