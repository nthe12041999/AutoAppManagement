using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity
{
    [Table("VerificationCode")]
    public class VerificationCode : BaseCUEntity
    {
        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public VerificationType Type { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public bool IsUsed { get; set; } = false;

        public DateTime? UsedDate { get; set; }
    }

    public enum VerificationType: short
    {
        Register = 1,
        ForgotPassword = 2,
        ChangePassword = 3
    }
}
