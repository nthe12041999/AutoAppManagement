using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity
{
    /// <summary>
    /// Base entity class with common properties for all entities
    /// </summary>
    public abstract class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Entity creation date
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Entity last update date
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// ID of user who created this entity
        /// </summary>
        public long? CreatedBy { get; set; }

        /// <summary>
        /// ID of user who last updated this entity
        /// </summary>
        public long? UpdatedBy { get; set; }

        /// <summary>
        /// Soft delete flag
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Soft delete date
        /// </summary>
        public DateTime? DeletedDate { get; set; }

        /// <summary>
        /// ID of user who deleted this entity
        /// </summary>
        public long? DeletedBy { get; set; }

        /// <summary>
        /// Entity version for optimistic concurrency
        /// </summary>
        [Timestamp]
        public byte[]? RowVersion { get; set; }

        /// <summary>
        /// Additional notes or comments
        /// </summary>
        [StringLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Entity status (Active, Inactive, etc.)
        /// </summary>
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        /// <summary>
        /// Check if entity is active
        /// </summary>
        [NotMapped]
        public virtual bool IsActive
        {
            get => Status == "Active" && !IsDeleted;
            set
            {
                if (value)
                {
                    Status = "Active";
                    IsDeleted = false;
                }
                else
                {
                    Status = "Inactive";
                }
            }
        }

        /// <summary>
        /// Update the UpdatedDate and UpdatedBy when entity is modified
        /// </summary>
        /// <param name="updatedBy">ID of user making the update</param>
        public virtual void SetUpdated(long? updatedBy = null)
        {
            UpdatedDate = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }

        /// <summary>
        /// Soft delete the entity
        /// </summary>
        /// <param name="deletedBy">ID of user performing the delete</param>
        public virtual void SetDeleted(long? deletedBy = null)
        {
            IsDeleted = true;
            DeletedDate = DateTime.UtcNow;
            DeletedBy = deletedBy;
            Status = "Deleted";
        }

        /// <summary>
        /// Restore a soft deleted entity
        /// </summary>
        /// <param name="restoredBy">ID of user performing the restore</param>
        public virtual void SetRestored(long? restoredBy = null)
        {
            IsDeleted = false;
            DeletedDate = null;
            DeletedBy = null;
            Status = "Active";
            SetUpdated(restoredBy);
        }
    }
}
