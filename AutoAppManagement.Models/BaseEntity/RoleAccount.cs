using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity;

public partial class RoleAccount : BaseEntity
{
    public long RoleId { get; set; }

    public long AccountId { get; set; }

    public virtual Account Account { get; set; }

    public virtual Account CreatedByNavigation { get; set; }

    public virtual Role Role { get; set; }
}
