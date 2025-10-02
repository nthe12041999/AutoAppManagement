-- =============================================
-- Script: Create ToolVersion Table
-- Description: Creates table for managing tool versions
-- Author: AutoAppManagement
-- Date: 2024
-- =============================================

USE AutoAppManagement;
GO

-- Drop table if exists (be careful in production)
IF OBJECT_ID('dbo.ToolVersions', 'U') IS NOT NULL
BEGIN
    PRINT 'Dropping existing ToolVersions table...';
    DROP TABLE dbo.ToolVersions;
END
GO

-- Create ToolVersions table
CREATE TABLE [dbo].[ToolVersions] (
    [ID] BIGINT NOT NULL IDENTITY(1,1),
    [ToolCode] NVARCHAR(100) NOT NULL,
    [ToolName] NVARCHAR(200) NOT NULL,
    [CurrentVersion] NVARCHAR(50) NOT NULL,
    [MinimumVersion] NVARCHAR(50) NULL,
    [Description] NVARCHAR(1000) NULL,
    [DownloadUrl] NVARCHAR(500) NULL,
    [ReleaseNotes] NTEXT NULL,
    [ReleaseDate] DATETIME2 NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [IsRequired] BIT NOT NULL DEFAULT 0,
    [Platform] NVARCHAR(50) NULL,
    [FileSize] BIGINT NULL,
    [Checksum] NVARCHAR(255) NULL,
    [Features] NTEXT NULL,
    [BugFixes] NTEXT NULL,
    [Category] NVARCHAR(100) NULL,
    [Priority] INT NULL DEFAULT 0,
    [CreatedDate] DATETIME2 NULL DEFAULT (GETDATE()),
    [CreatedBy] BIGINT NULL,
    [UpdatedDate] DATETIME2 NULL,
    [UpdatedBy] BIGINT NULL,
    CONSTRAINT [PK_ToolVersions] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ToolVersions_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[AdminAccounts]([ID]),
    CONSTRAINT [FK_ToolVersions_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[AdminAccounts]([ID])
);
GO

-- Create indexes for better performance
CREATE NONCLUSTERED INDEX [IX_ToolVersions_ToolCode] ON [dbo].[ToolVersions] ([ToolCode]);
GO

CREATE NONCLUSTERED INDEX [IX_ToolVersions_IsActive] ON [dbo].[ToolVersions] ([IsActive]);
GO

CREATE NONCLUSTERED INDEX [IX_ToolVersions_Platform] ON [dbo].[ToolVersions] ([Platform]);
GO

CREATE NONCLUSTERED INDEX [IX_ToolVersions_Category] ON [dbo].[ToolVersions] ([Category]);
GO

CREATE NONCLUSTERED INDEX [IX_ToolVersions_ReleaseDate] ON [dbo].[ToolVersions] ([ReleaseDate] DESC);
GO

-- Create unique constraint for ToolCode + Version combination
CREATE UNIQUE NONCLUSTERED INDEX [IX_ToolVersions_ToolCode_Version] 
ON [dbo].[ToolVersions] ([ToolCode], [CurrentVersion]);
GO

-- Insert sample data for testing
INSERT INTO [dbo].[ToolVersions] 
    ([ToolCode], [ToolName], [CurrentVersion], [MinimumVersion], [Description], 
     [DownloadUrl], [ReleaseNotes], [ReleaseDate], [IsActive], [IsRequired], 
     [Platform], [FileSize], [Checksum], [Features], [BugFixes], [Category], [Priority])
VALUES 
    -- AutoClicker Tool
    ('AUTO_CLICKER', 'Auto Clicker Pro', '2.5.0', '2.0.0', 
     'Professional auto clicking tool with advanced features', 
     'https://download.example.com/autoclicker/v2.5.0/setup.exe',
     'Major update with new UI and performance improvements',
     '2024-01-15', 1, 0, 'Windows', 5242880, 
     'SHA256:abc123def456...', 
     '["New modern UI","Multi-threading support","Custom scripts","Hotkey customization"]',
     '["Fixed memory leak issue","Resolved crash on Windows 11","Fixed coordinate detection bug"]',
     'Automation', 1),
    
    -- Social Media Manager
    ('SOCIAL_MANAGER', 'Social Media Manager', '1.8.2', '1.5.0',
     'All-in-one social media management tool',
     'https://download.example.com/socialmanager/v1.8.2/setup.exe',
     'Bug fixes and stability improvements',
     '2024-01-10', 1, 0, 'Windows', 10485760,
     'SHA256:xyz789abc123...',
     '["Facebook integration","Instagram automation","Schedule posts","Analytics dashboard"]',
     '["Fixed login issues","Improved API rate limiting","Fixed timezone problems"]',
     'Social Media', 2),
    
    -- Data Scraper
    ('DATA_SCRAPER', 'Web Data Scraper', '3.2.1', '3.0.0',
     'Advanced web scraping tool with AI capabilities',
     'https://download.example.com/scraper/v3.2.1/setup.exe',
     'Added AI-powered data extraction',
     '2024-01-05', 1, 1, 'Windows', 15728640,
     'SHA256:def456ghi789...',
     '["AI data extraction","Cloud storage support","Proxy rotation","Export to multiple formats"]',
     '["Fixed CAPTCHA detection","Improved JavaScript rendering","Fixed CSV export encoding"]',
     'Data Tools', 3),
    
    -- Mobile App version for Android
    ('MOBILE_APP', 'AutoApp Mobile', '1.0.5', '1.0.0',
     'Mobile companion app for AutoApp Management',
     'https://download.example.com/mobile/v1.0.5/app.apk',
     'Initial release for Android platform',
     '2024-01-01', 1, 0, 'Android', 25165824,
     'SHA256:mobile123abc...',
     '["Remote monitoring","Push notifications","Real-time sync","Dark mode"]',
     '["Fixed connection timeout","Improved battery usage","Fixed notification delays"]',
     'Mobile', 4),
    
    -- Mac version example
    ('AUTO_CLICKER', 'Auto Clicker Pro', '2.5.0', '2.0.0',
     'Professional auto clicking tool for macOS',
     'https://download.example.com/autoclicker/v2.5.0/AutoClicker.dmg',
     'Major update with new UI and performance improvements - macOS version',
     '2024-01-15', 1, 0, 'MacOS', 6291456,
     'SHA256:mac123def456...',
     '["New modern UI","Multi-threading support","Custom scripts","Hotkey customization","macOS Ventura support"]',
     '["Fixed memory leak issue","Resolved crash on macOS Sonoma","Fixed coordinate detection bug"]',
     'Automation', 1),
    
    -- Old versions (inactive)
    ('AUTO_CLICKER', 'Auto Clicker Pro', '2.4.0', '2.0.0',
     'Previous stable version',
     'https://download.example.com/autoclicker/v2.4.0/setup.exe',
     'Stable release with bug fixes',
     '2023-12-01', 0, 0, 'Windows', 4194304,
     'SHA256:old123def456...',
     '["Bug fixes","Performance improvements"]',
     '["Various bug fixes"]',
     'Automation', 0),
    
    ('SOCIAL_MANAGER', 'Social Media Manager', '1.8.0', '1.5.0',
     'Previous version',
     'https://download.example.com/socialmanager/v1.8.0/setup.exe',
     'Feature update',
     '2023-12-15', 0, 0, 'Windows', 9437184,
     'SHA256:old789abc123...',
     '["New features"]',
     '["Bug fixes"]',
     'Social Media', 0);
GO

-- Verify the table was created successfully
IF OBJECT_ID('dbo.ToolVersions', 'U') IS NOT NULL
BEGIN
    PRINT 'ToolVersions table created successfully!';
    
    -- Show sample data
    SELECT TOP 5
        [ToolCode],
        [ToolName],
        [CurrentVersion],
        [Platform],
        [IsActive],
        [ReleaseDate]
    FROM [dbo].[ToolVersions]
    ORDER BY [ReleaseDate] DESC;
END
ELSE
BEGIN
    PRINT 'ERROR: ToolVersions table was not created!';
END
GO

-- Grant permissions (adjust as needed for your environment)
-- GRANT SELECT ON [dbo].[ToolVersions] TO [your_app_user];
-- GRANT INSERT, UPDATE, DELETE ON [dbo].[ToolVersions] TO [your_admin_user];
GO

PRINT 'Script execution completed.';
GO

