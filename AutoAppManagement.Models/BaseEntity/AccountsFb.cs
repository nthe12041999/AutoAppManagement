using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity;

public partial class AccountsFb
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long AccountId { get; set; }

    public string FacebookId { get; set; }

    public string FacebookUserName { get; set; }

    public string FacebookPassword { get; set; }

    public string FacebookEmail { get; set; }

    public string FacebookName { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? CreatedDate { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public string Status { get; set; }

    public string AccessToken { get; set; }

    public DateTime? TokenExpiry { get; set; }

    public virtual Account Account { get; set; }
}
