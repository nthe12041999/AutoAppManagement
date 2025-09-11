using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoAppManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class ConvertToSimpleFeatureManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_License_CreatedBy",
                table: "Licenses");

            migrationBuilder.DropForeignKey(
                name: "FK_License_UpdatedBy",
                table: "Licenses");

            migrationBuilder.DropTable(
                name: "LicenseFeatures");

            migrationBuilder.DropTable(
                name: "ToolToolCategory");

            migrationBuilder.DropTable(
                name: "FeatureUsage");

            migrationBuilder.DropTable(
                name: "ToolCategory");

            migrationBuilder.DropTable(
                name: "ToolFeatures");

            migrationBuilder.DropTable(
                name: "ToolVersion");

            migrationBuilder.DropTable(
                name: "Tools");

            migrationBuilder.RenameColumn(
                name: "UsageLimits",
                table: "Licenses",
                newName: "Features");

            migrationBuilder.RenameColumn(
                name: "AllowedFeatures",
                table: "Licenses",
                newName: "FeatureLimits");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseType",
                table: "Licenses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.CreateTable(
                name: "features",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsBeta = table.Column<bool>(type: "bit", nullable: false),
                    PriorityOrder = table.Column<int>(type: "int", nullable: false),
                    ResourceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DefaultLimit = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_features", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "license_users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    LicenseId = table.Column<long>(type: "bigint", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsTrial = table.Column<bool>(type: "bit", nullable: false),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_license_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LicenseUser_Account",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LicenseUser_License",
                        column: x => x.LicenseId,
                        principalTable: "Licenses",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "feature_usage_tracking",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    FeatureId = table.Column<long>(type: "bigint", nullable: false),
                    UsageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    ResourceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UsageType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feature_usage_tracking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeatureUsageTracking_Feature",
                        column: x => x.FeatureId,
                        principalTable: "features",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_feature_usage_tracking_FeatureId",
                table: "feature_usage_tracking",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_feature_usage_tracking_UsageDate",
                table: "feature_usage_tracking",
                column: "UsageDate");

            migrationBuilder.CreateIndex(
                name: "IX_feature_usage_tracking_UserId_FeatureId_UsageDate",
                table: "feature_usage_tracking",
                columns: new[] { "UserId", "FeatureId", "UsageDate" });

            migrationBuilder.CreateIndex(
                name: "IX_features_Code",
                table: "features",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_license_users_AccountId_IsActive",
                table: "license_users",
                columns: new[] { "AccountId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_license_users_AccountId_LicenseId",
                table: "license_users",
                columns: new[] { "AccountId", "LicenseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_license_users_LicenseId",
                table: "license_users",
                column: "LicenseId");

            migrationBuilder.AddForeignKey(
                name: "FK_License_CreatedBy",
                table: "Licenses",
                column: "CreatedBy",
                principalTable: "Accounts",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_License_UpdatedBy",
                table: "Licenses",
                column: "UpdatedBy",
                principalTable: "Accounts",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_License_CreatedBy",
                table: "Licenses");

            migrationBuilder.DropForeignKey(
                name: "FK_License_UpdatedBy",
                table: "Licenses");

            migrationBuilder.DropTable(
                name: "feature_usage_tracking");

            migrationBuilder.DropTable(
                name: "license_users");

            migrationBuilder.DropTable(
                name: "features");

            migrationBuilder.RenameColumn(
                name: "Features",
                table: "Licenses",
                newName: "UsageLimits");

            migrationBuilder.RenameColumn(
                name: "FeatureLimits",
                table: "Licenses",
                newName: "AllowedFeatures");

            migrationBuilder.AlterColumn<short>(
                name: "LicenseType",
                table: "Licenses",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "ToolCategory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentCategoryId = table.Column<long>(type: "bigint", nullable: true),
                    CategoryCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ColorCode = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToolCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToolCategory_ToolCategory_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "ToolCategory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tools",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DocumentationUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    ToolCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ToolName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ToolType = table.Column<short>(type: "smallint", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ToolToolCategory",
                columns: table => new
                {
                    ToolCategoriesId = table.Column<long>(type: "bigint", nullable: false),
                    ToolsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToolToolCategory", x => new { x.ToolCategoriesId, x.ToolsId });
                    table.ForeignKey(
                        name: "FK_ToolToolCategory_ToolCategory_ToolCategoriesId",
                        column: x => x.ToolCategoriesId,
                        principalTable: "ToolCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToolToolCategory_Tools_ToolsId",
                        column: x => x.ToolsId,
                        principalTable: "Tools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToolVersion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToolId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Dependencies = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DownloadUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FileHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsLatest = table.Column<bool>(type: "bit", nullable: false),
                    IsStable = table.Column<bool>(type: "bit", nullable: false),
                    IsSupported = table.Column<bool>(type: "bit", nullable: false),
                    MinimumSystemVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReleaseNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Status = table.Column<short>(type: "smallint", maxLength: 20, nullable: false),
                    SupportEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VersionName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToolVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToolVersion_Tools_ToolId",
                        column: x => x.ToolId,
                        principalTable: "Tools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToolFeatures",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByNavigationId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByNavigationId = table.Column<long>(type: "bigint", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DefaultLimits = table.Column<string>(type: "ntext", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FeatureCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FeatureName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FeatureType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Metadata = table.Column<string>(type: "ntext", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    RequiresLicense = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ToolId = table.Column<long>(type: "bigint", nullable: false),
                    ToolVersionId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToolFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToolFeatures_Accounts_CreatedByNavigationId",
                        column: x => x.CreatedByNavigationId,
                        principalTable: "Accounts",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ToolFeatures_Accounts_UpdatedByNavigationId",
                        column: x => x.UpdatedByNavigationId,
                        principalTable: "Accounts",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ToolFeatures_ToolVersion_ToolVersionId",
                        column: x => x.ToolVersionId,
                        principalTable: "ToolVersion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ToolFeatures_Tools_ToolId",
                        column: x => x.ToolId,
                        principalTable: "Tools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeatureUsage",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToolId = table.Column<long>(type: "bigint", nullable: false),
                    ToolVersionId = table.Column<long>(type: "bigint", nullable: true),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FeatureUsageId = table.Column<long>(type: "bigint", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LicenseId = table.Column<long>(type: "bigint", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ResourceConsumed = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ToolFeatureId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    UsageData = table.Column<string>(type: "ntext", nullable: true),
                    UsageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsageType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeatureUsage_FeatureUsage_FeatureUsageId",
                        column: x => x.FeatureUsageId,
                        principalTable: "FeatureUsage",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FeatureUsage_ToolFeatures_ToolFeatureId",
                        column: x => x.ToolFeatureId,
                        principalTable: "ToolFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeatureUsage_ToolVersion_ToolVersionId",
                        column: x => x.ToolVersionId,
                        principalTable: "ToolVersion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FeatureUsage_Tools_ToolId",
                        column: x => x.ToolId,
                        principalTable: "Tools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LicenseFeatures",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    LicenseId = table.Column<long>(type: "bigint", nullable: false),
                    ToolFeatureId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Configuration = table.Column<string>(type: "ntext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FeatureUsageId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ResourceLimits = table.Column<string>(type: "ntext", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsageQuota = table.Column<string>(type: "ntext", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseFeature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LicenseFeature_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LicenseFeature_License",
                        column: x => x.LicenseId,
                        principalTable: "Licenses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LicenseFeature_ToolFeature",
                        column: x => x.ToolFeatureId,
                        principalTable: "ToolFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LicenseFeature_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LicenseFeatures_FeatureUsage_FeatureUsageId",
                        column: x => x.FeatureUsageId,
                        principalTable: "FeatureUsage",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeatureUsage_FeatureUsageId",
                table: "FeatureUsage",
                column: "FeatureUsageId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureUsage_ToolFeatureId",
                table: "FeatureUsage",
                column: "ToolFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureUsage_ToolId",
                table: "FeatureUsage",
                column: "ToolId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureUsage_ToolVersionId",
                table: "FeatureUsage",
                column: "ToolVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseFeatures_CreatedBy",
                table: "LicenseFeatures",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseFeatures_FeatureUsageId",
                table: "LicenseFeatures",
                column: "FeatureUsageId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseFeatures_LicenseId",
                table: "LicenseFeatures",
                column: "LicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseFeatures_ToolFeatureId",
                table: "LicenseFeatures",
                column: "ToolFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseFeatures_UpdatedBy",
                table: "LicenseFeatures",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ToolCategory_ParentCategoryId",
                table: "ToolCategory",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolFeatures_CreatedByNavigationId",
                table: "ToolFeatures",
                column: "CreatedByNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolFeatures_ToolId",
                table: "ToolFeatures",
                column: "ToolId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolFeatures_ToolVersionId",
                table: "ToolFeatures",
                column: "ToolVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolFeatures_UpdatedByNavigationId",
                table: "ToolFeatures",
                column: "UpdatedByNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolToolCategory_ToolsId",
                table: "ToolToolCategory",
                column: "ToolsId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolVersion_ToolId",
                table: "ToolVersion",
                column: "ToolId");

            migrationBuilder.AddForeignKey(
                name: "FK_License_CreatedBy",
                table: "Licenses",
                column: "CreatedBy",
                principalTable: "Accounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_License_UpdatedBy",
                table: "Licenses",
                column: "UpdatedBy",
                principalTable: "Accounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
