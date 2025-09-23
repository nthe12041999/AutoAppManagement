using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Models.BaseEntity;

public partial class Notification: BaseCUEntity
{
    public string Title { get; set; }

    public string Message { get; set; }

    public NotificationType Type { get; set; }

    [StringLength(255)]
    public string Icon { get; set; }

    [StringLength(255)]
    public string Image { get; set; }

    public long AccountId { get; set; }

    public bool IsReaded { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("Notifications")]
    public virtual AdminAccount Account { get; set; }
}
