namespace AutoAppManagement.Models
{
    /// <summary>
    /// Account model for DataGrid demo
    /// </summary>
    public class Account
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string OnlineStatus { get; set; } = string.Empty;
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public decimal? Salary { get; set; }
        public int LoginCount { get; set; }
    }
}
