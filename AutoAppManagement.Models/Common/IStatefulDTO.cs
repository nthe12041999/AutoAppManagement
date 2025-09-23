using AutoAppManagement.Models.Enums;

namespace AutoAppManagement.Models.Common
{
    public interface IStatefulDTO
    {
        public EntityState State { get; set; }
        public long ID { get; set; }
    }
}

