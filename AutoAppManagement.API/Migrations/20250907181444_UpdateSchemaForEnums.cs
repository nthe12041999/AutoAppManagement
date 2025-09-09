using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoAppManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchemaForEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_License_Account",
                table: "Licenses");

            migrationBuilder.DropIndex(
                name: "IX_Licenses_AccountId",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "is_inherited",
                table: "role_permissions");

            migrationBuilder.DropColumn(
                name: "priority",
                table: "role_permissions");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "IsAutoRenewal",
                table: "Licenses");

            migrationBuilder.RenameColumn(
                name: "action",
                table: "permissions",
                newName: "Action");

            migrationBuilder.RenameIndex(
                name: "IX_permissions_resource_action",
                table: "permissions",
                newName: "IX_permissions_resource_Action");

            migrationBuilder.AlterColumn<short>(
                name: "Action",
                table: "permissions",
                type: "smallint",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<short>(
                name: "Type",
                table: "Notifications",
                type: "smallint",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "UsageLimits",
                table: "Licenses",
                type: "ntext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "ntext");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Licenses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Active");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentInfo",
                table: "Licenses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<short>(
                name: "LicenseType",
                table: "Licenses",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Licenses",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "AllowedFeatures",
                table: "Licenses",
                type: "ntext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "ntext");

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Licenses",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<short>(
                name: "OperatingSystem",
                table: "CustomerDevices",
                type: "smallint",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<short>(
                name: "DeviceType",
                table: "CustomerDevices",
                type: "smallint",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoRenewal",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "LicenseId",
                table: "Accounts",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_LicenseId",
                table: "Accounts",
                column: "LicenseId",
                unique: true,
                filter: "[LicenseId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_License",
                table: "Accounts",
                column: "LicenseId",
                principalTable: "Licenses",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_License",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_LicenseId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "IsAutoRenewal",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "LicenseId",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "Action",
                table: "permissions",
                newName: "action");

            migrationBuilder.RenameIndex(
                name: "IX_permissions_resource_Action",
                table: "permissions",
                newName: "IX_permissions_resource_action");

            migrationBuilder.AddColumn<bool>(
                name: "is_inherited",
                table: "role_permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "priority",
                table: "role_permissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "action",
                table: "permissions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "UsageLimits",
                table: "Licenses",
                type: "ntext",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "ntext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Licenses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Active",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentInfo",
                table: "Licenses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LicenseType",
                table: "Licenses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Licenses",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AllowedFeatures",
                table: "Licenses",
                type: "ntext",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "ntext",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "Licenses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoRenewal",
                table: "Licenses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "OperatingSystem",
                table: "CustomerDevices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceType",
                table: "CustomerDevices",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_AccountId",
                table: "Licenses",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_License_Account",
                table: "Licenses",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "ID");
        }
    }
}
