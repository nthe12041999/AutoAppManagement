-- Create AutoAppManagement Database Tables
USE AutoAppManagement;

-- Create __EFMigrationsHistory table
CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId] nvarchar(150) NOT NULL,
    [ProductVersion] nvarchar(32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);

-- Create Accounts table
CREATE TABLE [Accounts] (
    [ID] bigint NOT NULL,
    [UserName] nvarchar(50) NOT NULL,
    [Password] nvarchar(255) NOT NULL,
    [Level] int NOT NULL,
    [Phone] nvarchar(20) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [RegisterDate] datetime2 NULL,
    [ExpiredDate] datetime2 NULL,
    [CreatedDate] datetime2 NULL DEFAULT (getdate()),
    [Language] nvarchar(10) NOT NULL,
    [IsLocked] bit NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Gender] smallint NOT NULL,
    [DateOfBirth] datetime2 NULL,
    [ImgAvatar] nvarchar(max) NOT NULL,
    [MaxAccountFb] int NOT NULL,
    CONSTRAINT [PK__Account__B9BE370FF1367EA2] PRIMARY KEY ([ID])
);

-- Create Roles table
CREATE TABLE [Roles] (
    [ID] bigint NOT NULL IDENTITY,
    [RoleName] nvarchar(max) NOT NULL,
    [RoleDescription] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([ID])
);

-- Create AccountsFb table
CREATE TABLE [AccountsFb] (
    [Id] bigint NOT NULL IDENTITY,
    [AccountId] bigint NOT NULL,
    [FacebookId] nvarchar(max) NOT NULL,
    [FacebookUserName] nvarchar(max) NOT NULL,
    [FacebookPassword] nvarchar(max) NOT NULL,
    [FacebookEmail] nvarchar(max) NOT NULL,
    [FacebookName] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedDate] datetime2 NULL,
    [LastLoginDate] datetime2 NULL,
    [Status] nvarchar(max) NOT NULL,
    [AccessToken] nvarchar(max) NOT NULL,
    [TokenExpiry] datetime2 NULL,
    CONSTRAINT [PK_AccountsFb] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AccountsFb_Accounts_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Accounts] ([ID]) ON DELETE CASCADE
);

-- Create CustomerDevices table
CREATE TABLE [CustomerDevices] (
    [ID] bigint NOT NULL IDENTITY,
    [AccountId] bigint NOT NULL,
    [DeviceId] nvarchar(255) NOT NULL,
    [DeviceName] nvarchar(255) NOT NULL,
    [DeviceType] nvarchar(50) NOT NULL,
    [OperatingSystem] nvarchar(100) NOT NULL,
    [OSVersion] nvarchar(50) NOT NULL,
    [BrowserInfo] nvarchar(255) NOT NULL,
    [IpAddress] nvarchar(45) NOT NULL,
    [Status] nvarchar(20) NOT NULL DEFAULT N'Active',
    [LastLoginDate] datetime2 NULL,
    [CreatedDate] datetime2 NULL DEFAULT (getdate()),
    [UpdatedDate] datetime2 NULL,
    [IsPrimaryDevice] bit NOT NULL,
    [Notes] nvarchar(500) NOT NULL,
    CONSTRAINT [PK_CustomerDevice] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_CustomerDevice_Account] FOREIGN KEY ([AccountId]) REFERENCES [Accounts] ([ID]) ON DELETE NO ACTION
);

-- Create CustomerLicenses table (without problematic foreign keys)
CREATE TABLE [CustomerLicenses] (
    [ID] bigint NOT NULL IDENTITY,
    [AccountId] bigint NOT NULL,
    [LicenseKey] nvarchar(255) NOT NULL,
    [LicenseName] nvarchar(100) NOT NULL,
    [LicenseType] nvarchar(50) NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    [MaxDevices] int NOT NULL,
    [MaxUsers] int NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [ExpiryDate] datetime2 NOT NULL,
    [Status] nvarchar(20) NOT NULL DEFAULT N'Active',
    [IsAutoRenewal] bit NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Currency] nvarchar(10) NOT NULL DEFAULT N'VND',
    [PaymentInfo] nvarchar(500) NOT NULL,
    [AllowedFeatures] ntext NOT NULL,
    [UsageLimits] ntext NOT NULL,
    [CreatedDate] datetime2 NULL DEFAULT (getdate()),
    [CreatedBy] bigint NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] bigint NULL,
    [Notes] nvarchar(1000) NOT NULL,
    CONSTRAINT [PK_CustomerLicense] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_CustomerLicense_Account] FOREIGN KEY ([AccountId]) REFERENCES [Accounts] ([ID]) ON DELETE NO ACTION
);

-- Create Notifications table
CREATE TABLE [Notifications] (
    [ID] bigint NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [Type] nvarchar(10) NOT NULL,
    [Icon] nvarchar(255) NOT NULL,
    [Image] nvarchar(255) NOT NULL,
    [AccountId] bigint NOT NULL,
    [IsReaded] bit NOT NULL,
    [CreatedDate] datetime2 NULL DEFAULT (getdate()),
    CONSTRAINT [PK__notifica__3213E83F850145B2] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_Notifications_Accounts_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Accounts] ([ID]) ON DELETE CASCADE
);

-- Create RoleAccounts table
CREATE TABLE [RoleAccounts] (
    [ID] bigint NOT NULL,
    [RoleID] bigint NOT NULL,
    [AccountID] bigint NOT NULL,
    [CreatedDate] datetime2 NULL,
    [CreatedBy] bigint NULL,
    CONSTRAINT [PK__RoleAcco__3213E83F4F7CD00D] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_RoleAccounts_Accounts_AccountID] FOREIGN KEY ([AccountID]) REFERENCES [Accounts] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleAccounts_Roles_RoleID] FOREIGN KEY ([RoleID]) REFERENCES [Roles] ([ID]) ON DELETE CASCADE
);

-- Insert migration history
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES ('20250803184810_AddCustomerDeviceAndLicense', '8.0.7');

-- Insert sample data for testing
INSERT INTO [Accounts] ([ID], [UserName], [Password], [Level], [Phone], [Email], [RegisterDate], [ExpiredDate], [Language], [IsLocked], [Name], [Gender], [ImgAvatar], [MaxAccountFb])
VALUES 
(1, 'admin', 'admin123', 1, '0123456789', 'admin@example.com', GETDATE(), DATEADD(year, 1, GETDATE()), 'vi', 0, 'Administrator', 1, '', 10),
(2, 'user1', 'user123', 2, '0987654321', 'user1@example.com', GETDATE(), DATEADD(year, 1, GETDATE()), 'vi', 0, 'User One', 1, '', 5),
(3, 'user2', 'user123', 2, '0111222333', 'user2@example.com', GETDATE(), DATEADD(year, 1, GETDATE()), 'vi', 0, 'User Two', 2, '', 5);

INSERT INTO [Roles] ([RoleName], [RoleDescription])
VALUES 
('Admin', 'System Administrator'),
('User', 'Regular User'),
('Moderator', 'Content Moderator');

PRINT 'Database created successfully!';
