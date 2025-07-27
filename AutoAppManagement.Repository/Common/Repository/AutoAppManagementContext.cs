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

            entity.HasOne(d => d.Account).WithMany(p => p.RoleAccountAccounts)
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
        OnModelCreatingPartial(modelBuilder);
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
