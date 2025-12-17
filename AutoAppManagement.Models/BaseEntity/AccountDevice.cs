using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Models.BaseEntity;

public partial class AccountDevice: BaseCUEntity
{
    public long AccountId { get; set; }

    [StringLength(255)]
    public string DeviceId { get; set; }

    [StringLength(255)]
    public string DeviceName { get; set; }

    public DeviceType DeviceType { get; set; }

    public short OperatingSystem { get; set; }

    [StringLength(50)]
    public string OSVersion { get; set; }

    [StringLength(255)]
    public string BrowserInfo { get; set; }

    [StringLength(45)]
    public string IpAddress { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public bool IsPrimaryDevice { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("AccountDevices")]
    public virtual Account Account { get; set; }
}
