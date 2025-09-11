using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Models.BaseEntity;

public partial class License : BaseEntity
{
    /// <summary>
    /// Mã license duy nhất
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string LicenseKey { get; set; } = string.Empty;

    /// <summary>
    /// Tên gói license
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string LicenseName { get; set; } = string.Empty;

    /// <summary>
    /// Loại license (Basic, Premium, Enterprise, etc.)
    /// </summary>
    public LicenseTypeEnum LicenseType { get; set; }

    /// <summary>
    /// Mô tả chi tiết về license
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Số lượng thiết bị tối đa được phép
    /// </summary>
    public int MaxDevices { get; set; } = 1;

    /// <summary>
    /// Số lượng user tối đa được phép
    /// </summary>
    public int MaxUsers { get; set; } = 1;

    /// <summary>
    /// Ngày bắt đầu hiệu lực
    /// </summary>
    public DateTime StartDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Ngày hết hạn
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Giá trị license (để tính toán)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    /// <summary>
    /// Giảm giá
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Discount { get; set; }

    /// <summary>
    /// Đơn vị tiền tệ
    /// </summary>
    [MaxLength(10)]
    public string Currency { get; set; } = "VND";

    /// <summary>
    /// Thông tin thanh toán
    /// </summary>
    [MaxLength(500)]
    public string? PaymentInfo { get; set; }

    /// <summary>
    /// Danh sách Feature IDs được phép sử dụng (JSON array format)
    /// Ví dụ: [1,2,3,5,8] hoặc ["CHAT_AI","BULK_ACTION","EXPORT_DATA"]
    /// </summary>
    [Column(TypeName = "ntext")]
    public string? Features { get; set; }

    /// <summary>
    /// Giới hạn sử dụng cho từng feature (JSON format)
    /// Ví dụ: {"CHAT_AI": {"daily": 100, "monthly": 3000}, "BULK_ACTION": {"monthly": 1000}}
    /// </summary>
    [Column(TypeName = "ntext")]
    public string? FeatureLimits { get; set; }

    // Navigation properties
    public virtual Account? CreatedByNavigation { get; set; }
    public virtual Account? UpdatedByNavigation { get; set; }
    // Commented out old navigation properties
    // public virtual ICollection<LicenseFeature> LicenseFeatures { get; set; } = new List<LicenseFeature>();
    public virtual ICollection<LicenseUser> LicenseUsers { get; set; } = new List<LicenseUser>();

    #region Helper Methods để làm việc với Features JSON

    /// <summary>
    /// Lấy danh sách Feature IDs từ JSON string
    /// </summary>
    public List<long> GetFeatureIds()
    {
        if (string.IsNullOrEmpty(Features))
            return new List<long>();

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<long>>(Features) ?? new List<long>();
        }
        catch
        {
            return new List<long>();
        }
    }

    /// <summary>
    /// Lấy danh sách Feature Codes từ JSON string
    /// </summary>
    public List<string> GetFeatureCodes()
    {
        if (string.IsNullOrEmpty(Features))
            return new List<string>();

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(Features) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    /// <summary>
    /// Set danh sách Feature IDs vào JSON string
    /// </summary>
    public void SetFeatureIds(List<long> featureIds)
    {
        Features = System.Text.Json.JsonSerializer.Serialize(featureIds);
    }

    /// <summary>
    /// Set danh sách Feature Codes vào JSON string
    /// </summary>
    public void SetFeatureCodes(List<string> featureCodes)
    {
        Features = System.Text.Json.JsonSerializer.Serialize(featureCodes);
    }

    /// <summary>
    /// Kiểm tra feature có được bật không (theo ID)
    /// </summary>
    public bool HasFeature(long featureId)
    {
        var featureIds = GetFeatureIds();
        return featureIds.Contains(featureId);
    }

    /// <summary>
    /// Kiểm tra feature có được bật không (theo Code)
    /// </summary>
    public bool HasFeature(string featureCode)
    {
        var featureCodes = GetFeatureCodes();
        return featureCodes.Contains(featureCode);
    }

    /// <summary>
    /// Lấy giới hạn sử dụng của một feature
    /// </summary>
    public Dictionary<string, int>? GetFeatureLimit(string featureCode)
    {
        if (string.IsNullOrEmpty(FeatureLimits))
            return null;

        try
        {
            var allLimits = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(FeatureLimits);
            return allLimits?.ContainsKey(featureCode) == true ? allLimits[featureCode] : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Set giới hạn sử dụng cho một feature
    /// </summary>
    public void SetFeatureLimit(string featureCode, Dictionary<string, int> limits)
    {
        var allLimits = new Dictionary<string, Dictionary<string, int>>();
        
        if (!string.IsNullOrEmpty(FeatureLimits))
        {
            try
            {
                allLimits = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(FeatureLimits) ?? new();
            }
            catch { }
        }

        allLimits[featureCode] = limits;
        FeatureLimits = System.Text.Json.JsonSerializer.Serialize(allLimits);
    }

    #endregion
}
