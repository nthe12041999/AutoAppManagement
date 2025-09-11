using AutoAppManagement.Models.BaseEntity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Repository.Common.Repository;

public partial class AutoAppManagementContext : DbContext
{
    public AutoAppManagementContext(DbContextOptions<AutoAppManagementContext> options)
        : base(options)
    {

    }

    #region Các model hứng dữ liệu

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleAccount> RoleAccounts { get; set; }

    public virtual DbSet<AccountDevice> CustomerDevices { get; set; }

    public virtual DbSet<License> Licenses { get; set; }

    public virtual DbSet<AdminAccount> AdminAccounts { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    // OLD: Commented out Tool-related entities
    // public virtual DbSet<Tool> Tools { get; set; }
    // public virtual DbSet<ToolFeature> ToolFeatures { get; set; }
    // public virtual DbSet<LicenseFeature> LicenseFeatures { get; set; }
    // public virtual DbSet<FeatureUsage> FeatureUsages { get; set; }

    // NEW: Simple Feature Management entities (3 entities mới)
    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<LicenseUser> LicenseUsers { get; set; }

    public virtual DbSet<FeatureUsageTracking> FeatureUsageTrackings { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account__B9BE370FF1367EA2");

            entity.HasIndex(e => e.UserName, "UQ__Account__F3DBC5720E43739A").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Language).HasMaxLength(10);
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(50);
            
            // Configuration cho LicenseId (one-to-one relationship)
            entity.HasOne(d => d.LicenseNavigation)
                .WithOne()
                .HasForeignKey<Account>(d => d.LicenseId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Account_License");
        });
        
        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<RoleAccount>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Account).WithMany(p => p.RoleAccounts)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleAccounts_Accounts");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RoleAccountCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_RoleAccounts_Accounts1");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleAccounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleAccounts_Roles");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__notifica__3213E83F850145B2");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Icon)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(10);
        });

        // Cấu hình cho CustomerDevice
        modelBuilder.Entity<AccountDevice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CustomerDevice");

            entity.HasIndex(e => new { e.AccountId, e.DeviceId }, "IX_CustomerDevice_AccountId_DeviceId").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DeviceId).IsRequired().HasMaxLength(255);
            entity.Property(e => e.DeviceName).HasMaxLength(255);
            entity.Property(e => e.DeviceType).HasMaxLength(50);
            entity.Property(e => e.OperatingSystem).HasMaxLength(100);
            entity.Property(e => e.OSVersion).HasMaxLength(50);
            entity.Property(e => e.BrowserInfo).HasMaxLength(255);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Active");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(d => d.Account).WithMany(p => p.CustomerDevices)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerDevice_Account");
        });

        // Cấu hình cho License
        modelBuilder.Entity<License>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_License");

            entity.HasIndex(e => e.LicenseKey, "IX_License_LicenseKey").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.LicenseKey).IsRequired().HasMaxLength(255);
            entity.Property(e => e.LicenseName).IsRequired().HasMaxLength(100);
            
            // Configure enum to be stored as string in database
            entity.Property(e => e.LicenseType)
                .HasConversion(
                    v => v.ToString(),
                    v => (AutoAppManagement.Models.Enum.DataModelType.LicenseTypeEnum)Enum.Parse(typeof(AutoAppManagement.Models.Enum.DataModelType.LicenseTypeEnum), v))
                .IsRequired();
                
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Currency).HasMaxLength(10).HasDefaultValue("VND");
            entity.Property(e => e.PaymentInfo).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne(d => d.CreatedByNavigation).WithMany()
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_License_CreatedBy");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany()
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_License_UpdatedBy");
        });

        // AdminAccount configuration
        modelBuilder.Entity<AdminAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserName).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("Active");
        });

        // Permission configuration
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Permission");
            
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => new { e.Resource, e.Action }).IsUnique();
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Resource).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DisplayName).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("Active");
        });

        // RolePermission configuration
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_RolePermission");
            
            entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleId).IsRequired();
            entity.Property(e => e.PermissionId).IsRequired();
            entity.Property(e => e.ScopeDefault).IsRequired().HasMaxLength(20).HasDefaultValue("own");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("Active");

            // Configure foreign keys
            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_RolePermission_Role");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_RolePermission_Permission");

            // Configure table with check constraint
            entity.ToTable(t => t.HasCheckConstraint("CK_RolePermission_ScopeDefault", 
                "scope_default IN ('own','team','org','all')"));
        });

        // NEW: Simple Feature Management Entities Configuration
        ConfigureSimpleFeatureManagementEntities(modelBuilder);

        OnModelCreatingPartial(modelBuilder);
    }

    private void ConfigureSimpleFeatureManagementEntities(ModelBuilder modelBuilder)
    {
        // NEW Feature configuration
        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
        });

        // NEW LicenseUser configuration
        modelBuilder.Entity<LicenseUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.AccountId, e.LicenseId }).IsUnique();
            entity.HasIndex(e => new { e.AccountId, e.IsActive });
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

            // Relationships
            entity.HasOne(d => d.Account).WithMany()
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.NoAction)  // NoAction to avoid cascade conflicts
                .HasConstraintName("FK_LicenseUser_Account");

            entity.HasOne(d => d.License).WithMany(p => p.LicenseUsers)
                .HasForeignKey(d => d.LicenseId)
                .OnDelete(DeleteBehavior.NoAction)  // NoAction to avoid cascade conflicts
                .HasConstraintName("FK_LicenseUser_License");
        });

        // NEW FeatureUsageTracking configuration
        modelBuilder.Entity<FeatureUsageTracking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.FeatureId, e.UsageDate });
            entity.HasIndex(e => e.UsageDate);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Feature).WithMany()
                .HasForeignKey(d => d.FeatureId)
                .OnDelete(DeleteBehavior.NoAction)  // NoAction to avoid cascade conflicts
                .HasConstraintName("FK_FeatureUsageTracking_Feature");
        });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    #region override custom savechanges
    public override int SaveChanges()
    {
        var validModel = ValidateModel();
        if (!validModel.IsValid && !string.IsNullOrEmpty(validModel.ErrorMessage))
        {
            // Ném ra một exception với thông điệp lỗi
            throw new InvalidOperationException(validModel.ErrorMessage);
        }
        TrimStringPropertype();
        return base.SaveChanges();
    }

    /// <summary>
    /// Xử lý trim dữ liệu trước khi lưu
    /// CreatedBy ntthe 25.02.2024
    /// </summary>
    private void TrimStringPropertype()
    {
        var entities = ChangeTracker.Entries()
                        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
        foreach (var item in entities)
        {
            var properties = item.Properties.Where(p => p.CurrentValue is string).Select(p => p);
            foreach (var property in properties)
            {
                var currentValue = property.CurrentValue?.ToString();
                if (currentValue != null)
                {
                    property.CurrentValue = currentValue.Trim();
                }
            }
        }
    }

    /// <summary>
    /// Kiểm tra dữ liệu null có phù hợp kiểu dữ liệu không trước khi lưu (chỉ xảy ra với string)
    /// CreatedBy ntthe 25.02.2024
    /// </summary>
    public (bool IsValid, string ErrorMessage) ValidateModel()
    {
        var entities = ChangeTracker.Entries()
                        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).Select(e => e.Entity);
        var validationContext = new ValidationContext(entities);
        var validationResults = new List<ValidationResult>();

        // Kiểm tra tính hợp lệ của model
        bool isValid = Validator.TryValidateObject(entities, validationContext, validationResults, true);
        if (!isValid)
        {
            // Lặp qua các lỗi và tạo thông điệp lỗi
            string errorMessage = string.Join(Environment.NewLine, validationResults.Select(r => r.ErrorMessage));
            return (false, errorMessage);
        }

        return (true, "");
    }
    #endregion
}
