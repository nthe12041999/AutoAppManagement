using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity;

public partial class License: BaseCUEntity
{
    [StringLength(255)]
    public string LicenseKey { get; set; }
     
    [StringLength(100)]
    public string LicenseName { get; set; }

    public string LicenseType { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public int MaxDevices { get; set; }

    public int MaxUsers { get; set; }

    public DateTime StartDate { get; set; } = DateTime.Now;

    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Khoảng thời gian hiệu lực của License (số ngày)
    /// </summary>
    [Column("DurationDays")]
    public int DurationDays { get; set; } = 30; // Mặc định 30 ngày

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [StringLength(10)]
    public string Currency { get; set; }

    [StringLength(500)]
    public string PaymentInfo { get; set; }

    [Column(TypeName = "ntext")]
    public string FeatureLimits { get; set; }

    [Column(TypeName = "ntext")]
    public string Features { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Discount { get; set; }

    [InverseProperty("License")]
    public virtual Account Account { get; set; }
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
    public Dictionary<string, int> GetFeatureLimit(string featureCode)
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
