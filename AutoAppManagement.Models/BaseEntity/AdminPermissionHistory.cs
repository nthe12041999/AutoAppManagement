using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity
{
    /// <summary>
    /// AdminPermissionHistory entity for tracking admin permission changes
    /// </summary>
    [Table("AdminPermissionHistory")]
    public class AdminPermissionHistory : BaseEntity
    {
        [Required]
        public long AdminAccountId { get; set; }

        [Required]
        [StringLength(50)]
        public string Action { get; set; } = string.Empty; // Grant, Revoke, Update

        [Required]
        [StringLength(100)]
        public string Permission { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? OldValue { get; set; }

        [StringLength(1000)]
        public string? NewValue { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        [Required]
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("AdminAccountId")]
        public virtual AdminAccount AdminAccount { get; set; } = null!;

        // Computed Properties
        [NotMapped]
        public string ActionDisplay
        {
            get
            {
                return Action switch
                {
                    "Grant" => "Cấp quyền",
                    "Revoke" => "Thu hồi quyền",
                    "Update" => "Cập nhật quyền",
                    _ => Action
                };
            }
        }

        [NotMapped]
        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.UtcNow - ChangedAt;
                if (timeSpan.TotalMinutes < 1)
                    return "Vừa xong";
                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes} phút trước";
                if (timeSpan.TotalHours < 24)
                    return $"{(int)timeSpan.TotalHours} giờ trước";
                return $"{(int)timeSpan.TotalDays} ngày trước";
            }
        }

        [NotMapped]
        public bool HasValueChange => !string.IsNullOrEmpty(OldValue) || !string.IsNullOrEmpty(NewValue);

        // Methods
        public static AdminPermissionHistory CreateGrantRecord(long adminAccountId, string permission, string newValue, string? reason = null, long? changedBy = null)
        {
            return new AdminPermissionHistory
            {
                AdminAccountId = adminAccountId,
                Action = "Grant",
                Permission = permission,
                NewValue = newValue,
                Reason = reason,
                ChangedAt = DateTime.UtcNow,
                CreatedBy = changedBy,
                CreatedDate = DateTime.UtcNow
            };
        }

        public static AdminPermissionHistory CreateRevokeRecord(long adminAccountId, string permission, string oldValue, string? reason = null, long? changedBy = null)
        {
            return new AdminPermissionHistory
            {
                AdminAccountId = adminAccountId,
                Action = "Revoke",
                Permission = permission,
                OldValue = oldValue,
                Reason = reason,
                ChangedAt = DateTime.UtcNow,
                CreatedBy = changedBy,
                CreatedDate = DateTime.UtcNow
            };
        }

        public static AdminPermissionHistory CreateUpdateRecord(long adminAccountId, string permission, string oldValue, string newValue, string? reason = null, long? changedBy = null)
        {
            return new AdminPermissionHistory
            {
                AdminAccountId = adminAccountId,
                Action = "Update",
                Permission = permission,
                OldValue = oldValue,
                NewValue = newValue,
                Reason = reason,
                ChangedAt = DateTime.UtcNow,
                CreatedBy = changedBy,
                CreatedDate = DateTime.UtcNow
            };
        }
    }
}
