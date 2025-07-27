using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity;

public partial class Notification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public string Title { get; set; }
    public string Message { get; set; }

    public string Type { get; set; }

    public string Icon { get; set; } = "";

    public string Image { get; set; }
    public long AccountId { get; set; }
    public bool IsReaded { get; set; } = false;
    public DateTime? CreatedDate { get; set; } = DateTime.Now;
    public virtual Account Account { get; set; }
}
