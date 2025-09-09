using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Models.BaseEntity;

public partial class Notification : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; } = NotificationType.Info;

    public string Icon { get; set; } = "";

    public string Image { get; set; } = string.Empty;
    public long AccountId { get; set; }
    public bool IsReaded { get; set; } = false;
    public virtual Account? Account { get; set; }
}
