using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoAppManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLicenseFeatureTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ToolCategory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CategoryCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ColorCode = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    ParentCategoryId = table.Column<long>(type: "bigint", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    ToolName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ToolCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ToolType = table.Column<short>(type: "smallint", nullable: false),
                    CurrentVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DocumentationUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
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
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VersionName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReleaseNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DownloadUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    FileHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsStable = table.Column<bool>(type: "bit", nullable: false),
                    IsLatest = table.Column<bool>(type: "bit", nullable: false),
                    IsSupported = table.Column<bool>(type: "bit", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupportEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MinimumSystemVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Dependencies = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<short>(type: "smallint", maxLength: 20, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
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
                    ToolId = table.Column<long>(type: "bigint", nullable: false),
                    ToolVersionId = table.Column<long>(type: "bigint", nullable: true),
                    FeatureCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FeatureName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FeatureType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    RequiresLicense = table.Column<bool>(type: "bit", nullable: false),
                    DefaultLimits = table.Column<string>(type: "ntext", nullable: true),
                    Metadata = table.Column<string>(type: "ntext", nullable: true),
                    CreatedByNavigationId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByNavigationId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    LicenseId = table.Column<long>(type: "bigint", nullable: false),
                    ToolFeatureId = table.Column<long>(type: "bigint", nullable: false),
                    UsageType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    ResourceConsumed = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UsageData = table.Column<string>(type: "ntext", nullable: true),
                    UsageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ToolId = table.Column<long>(type: "bigint", nullable: false),
                    ToolVersionId = table.Column<long>(type: "bigint", nullable: true),
                    FeatureUsageId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_FeatureUsage_Tools_ToolId",
                        column: x => x.ToolId,
                        principalTable: "Tools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "LicenseFeatures",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LicenseId = table.Column<long>(type: "bigint", nullable: false),
                    ToolFeatureId = table.Column<long>(type: "bigint", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ResourceLimits = table.Column<string>(type: "ntext", maxLength: 500, nullable: true),
                    UsageQuota = table.Column<string>(type: "ntext", maxLength: 500, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Configuration = table.Column<string>(type: "ntext", nullable: true),
                    FeatureUsageId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_LicenseFeature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LicenseFeature_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
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
                        onDelete: ReferentialAction.NoAction);
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
