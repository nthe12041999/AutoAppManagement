namespace AutoAppManagement.Models.BaseEntity
{
    /// <summary>
    /// Base entity class with common properties for all entities
    /// </summary>
    public class BaseCUEntity : BaseOriginEntity
    {
        /// <summary>
        /// Entity creation date
        /// </summary>
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;

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
        /// Update the UpdatedDate and UpdatedBy when entity is modified
        /// </summary>
        /// <param name="updatedBy">ID of user making the update</param>
        public virtual void SetCreated(long? createdBy = null)
        {
            CreatedDate = DateTime.UtcNow;
            CreatedBy = createdBy;
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
        /// Update the UpdatedDate and UpdatedBy when entity is modified
        /// </summary>
        /// <param name="updatedBy">ID of user making the update</param>
        public virtual void SetDeleted(long? updatedBy = null)
        {
            Status = Enum.StatusEnum.Inactive;
            UpdatedDate = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }
    }
}
