using AutoAppManagement.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAppManagement.Models.BaseEntity
{
    [Table("AIConfig")]
    public class AIConfig: BaseCUEntity
    {
        [StringLength(100)]
        public string Name { get; set; }
        public long AccountId { get; set; }
        public AICategory Type { get; set; }

        [StringLength(50)]
        public string Model { get; set; }

        [StringLength(255)]
        public string APIKey { get; set; }
    }
}
