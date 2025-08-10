using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoAppManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class CreateAdminAccountTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Create AdminAccounts table
                CREATE TABLE [dbo].[AdminAccounts] (
                    [Id] bigint IDENTITY(1,1) NOT NULL,
                    [FullName] nvarchar(100) NOT NULL,
                    [Email] nvarchar(255) NOT NULL,
                    [PhoneNumber] nvarchar(20) NOT NULL,
                    [UserName] nvarchar(50) NOT NULL,
                    [PasswordHash] nvarchar(255) NOT NULL,
                    [Role] nvarchar(50) NOT NULL,
                    [Permissions] nvarchar(1000) NULL,
                    [IsEmailVerified] bit NOT NULL DEFAULT 0,
                    [IsPhoneVerified] bit NOT NULL DEFAULT 0,
                    [IsTwoFactorEnabled] bit NOT NULL DEFAULT 0,
                    [LastLoginAt] datetime2 NULL,
                    [LoginCount] int NOT NULL DEFAULT 0,
                    [FailedLoginAttempts] int NOT NULL DEFAULT 0,
                    [LockedUntil] datetime2 NULL,
                    [LastLoginIp] nvarchar(45) NULL,
                    [LastLoginUserAgent] nvarchar(500) NULL,
                    [EmailVerifiedAt] datetime2 NULL,
                    [PhoneVerifiedAt] datetime2 NULL,
                    [PasswordChangedAt] datetime2 NULL,
                    [Avatar] nvarchar(255) NULL,
                    [Department] nvarchar(100) NULL,
                    [Position] nvarchar(100) NULL,
                    [TwoFactorSecret] nvarchar(255) NULL,
                    [RecoveryTokens] nvarchar(500) NULL,
                    [LastPasswordChangeRequest] datetime2 NULL,
                    [CreatedDate] datetime2 NOT NULL DEFAULT GETDATE(),
                    [UpdatedDate] datetime2 NULL,
                    [CreatedBy] bigint NULL,
                    [UpdatedBy] bigint NULL,
                    [Status] nvarchar(50) NOT NULL DEFAULT 'Active',
                    [IsDeleted] bit NOT NULL DEFAULT 0,
                    CONSTRAINT [PK_AdminAccounts] PRIMARY KEY ([Id])
                );

                -- Create unique indexes
                CREATE UNIQUE INDEX [IX_AdminAccounts_Email] ON [dbo].[AdminAccounts] ([Email]);
                CREATE UNIQUE INDEX [IX_AdminAccounts_UserName] ON [dbo].[AdminAccounts] ([UserName]);

                -- Create AdminLoginHistory table
                CREATE TABLE [dbo].[AdminLoginHistory] (
                    [Id] bigint IDENTITY(1,1) NOT NULL,
                    [AdminAccountId] bigint NOT NULL,
                    [IpAddress] nvarchar(45) NULL,
                    [UserAgent] nvarchar(500) NULL,
                    [Location] nvarchar(100) NULL,
                    [LoginResult] nvarchar(50) NOT NULL,
                    [FailureReason] nvarchar(255) NULL,
                    [LoginAttemptAt] datetime2 NOT NULL DEFAULT GETDATE(),
                    [CreatedDate] datetime2 NOT NULL DEFAULT GETDATE(),
                    [UpdatedDate] datetime2 NULL,
                    [CreatedBy] bigint NULL,
                    [UpdatedBy] bigint NULL,
                    [Status] nvarchar(50) NOT NULL DEFAULT 'Active',
                    [IsDeleted] bit NOT NULL DEFAULT 0,
                    CONSTRAINT [PK_AdminLoginHistory] PRIMARY KEY ([Id]),
                    CONSTRAINT [FK_AdminLoginHistory_AdminAccounts_AdminAccountId]
                        FOREIGN KEY ([AdminAccountId]) REFERENCES [dbo].[AdminAccounts] ([Id]) ON DELETE CASCADE
                );

                -- Create index for foreign key
                CREATE INDEX [IX_AdminLoginHistory_AdminAccountId] ON [dbo].[AdminLoginHistory] ([AdminAccountId]);

                -- Create AdminPermissionHistory table
                CREATE TABLE [dbo].[AdminPermissionHistory] (
                    [Id] bigint IDENTITY(1,1) NOT NULL,
                    [AdminAccountId] bigint NOT NULL,
                    [Action] nvarchar(50) NOT NULL,
                    [Permission] nvarchar(100) NOT NULL,
                    [OldValue] nvarchar(1000) NULL,
                    [NewValue] nvarchar(1000) NULL,
                    [Reason] nvarchar(500) NULL,
                    [ChangedAt] datetime2 NOT NULL DEFAULT GETDATE(),
                    [CreatedDate] datetime2 NOT NULL DEFAULT GETDATE(),
                    [UpdatedDate] datetime2 NULL,
                    [CreatedBy] bigint NULL,
                    [UpdatedBy] bigint NULL,
                    [Status] nvarchar(50) NOT NULL DEFAULT 'Active',
                    [IsDeleted] bit NOT NULL DEFAULT 0,
                    CONSTRAINT [PK_AdminPermissionHistory] PRIMARY KEY ([Id]),
                    CONSTRAINT [FK_AdminPermissionHistory_AdminAccounts_AdminAccountId]
                        FOREIGN KEY ([AdminAccountId]) REFERENCES [dbo].[AdminAccounts] ([Id]) ON DELETE CASCADE
                );

                -- Create index for foreign key
                CREATE INDEX [IX_AdminPermissionHistory_AdminAccountId] ON [dbo].[AdminPermissionHistory] ([AdminAccountId]);

                -- Insert sample admin account
                INSERT INTO [dbo].[AdminAccounts] (
                    [FullName], [Email], [PhoneNumber], [UserName], [PasswordHash], [Role],
                    [IsEmailVerified], [IsPhoneVerified], [Department], [Position]
                ) VALUES (
                    N'Super Admin',
                    'admin@autoapp.com',
                    '0901234567',
                    'admin',
                    '$2a$11$8K1p/a0dL2LkqvjyD0LlMeO/jnZpHwu1Sfh0.urF8VYBVMIUpOubK', -- password: admin123
                    'Admin',
                    1,
                    1,
                    'IT',
                    'System Administrator'
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TABLE IF EXISTS [dbo].[AdminLoginHistory];
                DROP TABLE IF EXISTS [dbo].[AdminPermissionHistory];
                DROP TABLE IF EXISTS [dbo].[AdminAccounts];
            ");
        }
    }
}
