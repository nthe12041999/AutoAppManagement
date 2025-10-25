-- Create RefreshTokens table for AutoAppManagement
USE AutoAppManagement;

-- Drop table if exists (for development only)
-- DROP TABLE IF EXISTS [RefreshTokens];

-- Create RefreshTokens table
CREATE TABLE [RefreshTokens] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [Token] nvarchar(500) NOT NULL,
    [AccountId] bigint NOT NULL,
    [ExpiryDate] datetime2 NOT NULL,
    [IsUsed] bit NOT NULL DEFAULT 0,
    [IsRevoked] bit NOT NULL DEFAULT 0,
    [ReplacedByToken] nvarchar(500) NULL,
    [CreatedByIp] nvarchar(45) NULL,
    [RevokedByIp] nvarchar(45) NULL,
    [RevokedDate] datetime2 NULL,
    [DeviceInfo] nvarchar(255) NULL,
    [UserAgent] nvarchar(255) NULL,
    
    -- Base entity fields
    [CreatedDate] datetime2 NULL DEFAULT (getdate()),
    [CreatedBy] bigint NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] bigint NULL,
    [Status] int NOT NULL DEFAULT 1, -- 1 = Active, 0 = Inactive, -1 = Deleted
    
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_RefreshTokens_Accounts_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Accounts] ([ID]) ON DELETE CASCADE
);

-- Create indexes for better performance
CREATE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);
CREATE INDEX [IX_RefreshTokens_AccountId] ON [RefreshTokens] ([AccountId]);
CREATE INDEX [IX_RefreshTokens_ExpiryDate] ON [RefreshTokens] ([ExpiryDate]);
CREATE INDEX [IX_RefreshTokens_IsActive] ON [RefreshTokens] ([IsRevoked], [IsUsed], [ExpiryDate]) WHERE [Status] = 1;

-- Add some sample data for testing (optional)
/*
INSERT INTO [RefreshTokens] ([Token], [AccountId], [ExpiryDate], [CreatedBy])
VALUES 
    ('sample_refresh_token_1', 1, DATEADD(day, 7, GETDATE()), 1),
    ('sample_refresh_token_2', 2, DATEADD(day, 7, GETDATE()), 2);
*/

PRINT 'RefreshTokens table created successfully!';
