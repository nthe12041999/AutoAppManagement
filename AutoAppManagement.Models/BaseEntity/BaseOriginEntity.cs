using AutoAppManagement.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity
{
    /// <summary>
    /// Base entity class with common properties for all entities
    /// </summary>
    public class BaseOriginEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        public StatusEnum Status { get; set; } = StatusEnum.Active;
    }
}
