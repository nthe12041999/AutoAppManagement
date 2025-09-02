-- Add missing columns to AdminAccounts table
-- These columns are required by BaseEntity

USE AutoAppManagement;
GO

-- Add DeletedBy column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AdminAccounts]') AND name = 'DeletedBy')
BEGIN
    ALTER TABLE [dbo].[AdminAccounts] 
    ADD [DeletedBy] bigint NULL;
    PRINT 'Added DeletedBy column to AdminAccounts';
END

-- Add DeletedDate column  
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AdminAccounts]') AND name = 'DeletedDate')
BEGIN
    ALTER TABLE [dbo].[AdminAccounts] 
    ADD [DeletedDate] datetime2 NULL;
    PRINT 'Added DeletedDate column to AdminAccounts';
END

-- Add Notes column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AdminAccounts]') AND name = 'Notes')
BEGIN
    ALTER TABLE [dbo].[AdminAccounts] 
    ADD [Notes] nvarchar(1000) NULL;
    PRINT 'Added Notes column to AdminAccounts';
END

-- Add RowVersion column (for optimistic concurrency)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AdminAccounts]') AND name = 'RowVersion')
BEGIN
    ALTER TABLE [dbo].[AdminAccounts] 
    ADD [RowVersion] rowversion;
    PRINT 'Added RowVersion column to AdminAccounts';
END

-- Update AdminLoginHistory table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AdminLoginHistory]') AND name = 'DeletedBy')
BEGIN
    ALTER TABLE [dbo].[AdminLoginHistory] 
    ADD [DeletedBy] bigint NULL;
    PRINT 'Added DeletedBy column to AdminLoginHistory';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AdminLoginHistory]') AND name = 'DeletedDate')
BEGIN
    ALTER TABLE [dbo].[AdminLoginHistory] 
    ADD [DeletedDate] datetime2 NULL;
    PRINT 'Added DeletedDate column to AdminLoginHistory';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AdminLoginHistory]') AND name = 'Notes')
BEGIN
    ALTER TABLE [dbo].[AdminLoginHistory] 
    ADD [Notes] nvarchar(1000) NULL;
    PRINT 'Added Notes column to AdminLoginHistory';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AdminLoginHistory]') AND name = 'RowVersion')
BEGIN
    ALTER TABLE [dbo].[AdminLoginHistory] 
    ADD [RowVersion] rowversion;
    PRINT 'Added RowVersion column to AdminLoginHistory';
END

-- Update AdminPermissionHistory table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AdminPermissionHistory]') AND name = 'DeletedBy')
BEGIN
    ALTER TABLE [dbo].[AdminPermissionHistory] 
    ADD [DeletedBy] bigint NULL;
    PRINT 'Added DeletedBy column to AdminPermissionHistory';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AdminPermissionHistory]') AND name = 'DeletedDate')
BEGIN
    ALTER TABLE [dbo].[AdminPermissionHistory] 
    ADD [DeletedDate] datetime2 NULL;
    PRINT 'Added DeletedDate column to AdminPermissionHistory';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AdminPermissionHistory]') AND name = 'Notes')
BEGIN
    ALTER TABLE [dbo].[AdminPermissionHistory] 
    ADD [Notes] nvarchar(1000) NULL;
    PRINT 'Added Notes column to AdminPermissionHistory';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AdminPermissionHistory]') AND name = 'RowVersion')
BEGIN
    ALTER TABLE [dbo].[AdminPermissionHistory] 
    ADD [RowVersion] rowversion;
    PRINT 'Added RowVersion column to AdminPermissionHistory';
END

PRINT 'Database schema updated successfully!';
