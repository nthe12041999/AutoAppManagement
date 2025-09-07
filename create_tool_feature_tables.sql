-- Tool Feature Management Tables

-- Bảng quản lý tính năng tool
CREATE TABLE [dbo].[ToolFeature] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [FeatureCode] NVARCHAR(100) NOT NULL,
    [FeatureName] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    [Category] NVARCHAR(100) NULL,
    [FeatureType] NVARCHAR(50) NOT NULL DEFAULT 'Feature',
    [Priority] INT NOT NULL DEFAULT 0,
    [RequiresLicense] BIT NOT NULL DEFAULT 1,
    [DefaultLimits] NTEXT NULL,
    [Metadata] NTEXT NULL,
    [CreatedDate] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedDate] DATETIME2(7) NULL,
    [CreatedBy] BIGINT NULL,
    [UpdatedBy] BIGINT NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedDate] DATETIME2(7) NULL,
    [DeletedBy] BIGINT NULL,
    [RowVersion] ROWVERSION NOT NULL,
    [Notes] NVARCHAR(1000) NULL,
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Active',
    
    CONSTRAINT [PK_ToolFeature] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ToolFeature_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Account]([Id]),
    CONSTRAINT [FK_ToolFeature_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[Account]([Id])
);

-- Bảng mapping giữa License và ToolFeature
CREATE TABLE [dbo].[LicenseFeature] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [LicenseId] BIGINT NOT NULL,
    [ToolFeatureId] BIGINT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1,
    [ResourceLimits] NTEXT NULL,
    [UsageQuota] NTEXT NULL,
    [EffectiveFrom] DATETIME2(7) NULL,
    [EffectiveTo] DATETIME2(7) NULL,
    [Configuration] NTEXT NULL,
    [CreatedDate] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedDate] DATETIME2(7) NULL,
    [CreatedBy] BIGINT NULL,
    [UpdatedBy] BIGINT NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedDate] DATETIME2(7) NULL,
    [DeletedBy] BIGINT NULL,
    [RowVersion] ROWVERSION NOT NULL,
    [Notes] NVARCHAR(1000) NULL,
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Active',
    
    CONSTRAINT [PK_LicenseFeature] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LicenseFeature_License] FOREIGN KEY ([LicenseId]) REFERENCES [dbo].[License]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_LicenseFeature_ToolFeature] FOREIGN KEY ([ToolFeatureId]) REFERENCES [dbo].[ToolFeature]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_LicenseFeature_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Account]([Id]),
    CONSTRAINT [FK_LicenseFeature_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[Account]([Id])
);

-- Bảng tracking việc sử dụng tính năng
CREATE TABLE [dbo].[FeatureUsage] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [AccountId] BIGINT NOT NULL,
    [LicenseId] BIGINT NOT NULL,
    [ToolFeatureId] BIGINT NOT NULL,
    [UsageType] NVARCHAR(50) NOT NULL DEFAULT 'Access',
    [UsageCount] INT NOT NULL DEFAULT 1,
    [ResourceConsumed] DECIMAL(18,4) NOT NULL DEFAULT 0,
    [UsageData] NTEXT NULL,
    [UsageDate] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IpAddress] NVARCHAR(45) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [SessionId] NVARCHAR(100) NULL,
    [RequestId] NVARCHAR(100) NULL,
    [CreatedDate] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedDate] DATETIME2(7) NULL,
    [CreatedBy] BIGINT NULL,
    [UpdatedBy] BIGINT NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedDate] DATETIME2(7) NULL,
    [DeletedBy] BIGINT NULL,
    [RowVersion] ROWVERSION NOT NULL,
    [Notes] NVARCHAR(1000) NULL,
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Completed',
    
    CONSTRAINT [PK_FeatureUsage] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_FeatureUsage_Account] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account]([Id]),
    CONSTRAINT [FK_FeatureUsage_License] FOREIGN KEY ([LicenseId]) REFERENCES [dbo].[License]([Id]),
    CONSTRAINT [FK_FeatureUsage_ToolFeature] FOREIGN KEY ([ToolFeatureId]) REFERENCES [dbo].[ToolFeature]([Id]),
    CONSTRAINT [FK_FeatureUsage_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Account]([Id]),
    CONSTRAINT [FK_FeatureUsage_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[Account]([Id])
);

-- Indexes cho performance
CREATE UNIQUE NONCLUSTERED INDEX [IX_ToolFeature_FeatureCode] ON [dbo].[ToolFeature] ([FeatureCode] ASC) WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX [IX_ToolFeature_Category] ON [dbo].[ToolFeature] ([Category] ASC) WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX [IX_ToolFeature_FeatureType] ON [dbo].[ToolFeature] ([FeatureType] ASC) WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX [IX_ToolFeature_Status] ON [dbo].[ToolFeature] ([Status] ASC) WHERE [IsDeleted] = 0;

CREATE UNIQUE NONCLUSTERED INDEX [IX_LicenseFeature_Unique] ON [dbo].[LicenseFeature] ([LicenseId] ASC, [ToolFeatureId] ASC) WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX [IX_LicenseFeature_License] ON [dbo].[LicenseFeature] ([LicenseId] ASC) WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX [IX_LicenseFeature_ToolFeature] ON [dbo].[LicenseFeature] ([ToolFeatureId] ASC) WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX [IX_LicenseFeature_IsEnabled] ON [dbo].[LicenseFeature] ([IsEnabled] ASC) WHERE [IsDeleted] = 0;

CREATE NONCLUSTERED INDEX [IX_FeatureUsage_Account] ON [dbo].[FeatureUsage] ([AccountId] ASC, [UsageDate] DESC) WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX [IX_FeatureUsage_License] ON [dbo].[FeatureUsage] ([LicenseId] ASC, [UsageDate] DESC) WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX [IX_FeatureUsage_ToolFeature] ON [dbo].[FeatureUsage] ([ToolFeatureId] ASC, [UsageDate] DESC) WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX [IX_FeatureUsage_UsageType] ON [dbo].[FeatureUsage] ([UsageType] ASC, [UsageDate] DESC) WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX [IX_FeatureUsage_Date] ON [dbo].[FeatureUsage] ([UsageDate] DESC) WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX [IX_FeatureUsage_AccountFeature] ON [dbo].[FeatureUsage] ([AccountId] ASC, [ToolFeatureId] ASC, [UsageType] ASC, [UsageDate] ASC) WHERE [IsDeleted] = 0;

-- Sample data
INSERT INTO [dbo].[ToolFeature] ([FeatureCode], [FeatureName], [Description], [Category], [FeatureType], [RequiresLicense], [DefaultLimits], [CreatedBy])
VALUES 
('EXPORT_PDF', 'Xuất PDF', 'Tính năng xuất báo cáo dưới dạng PDF', 'Export', 'Feature', 1, '{"daily": 100, "monthly": 2000}', 1),
('EXPORT_EXCEL', 'Xuất Excel', 'Tính năng xuất dữ liệu dưới dạng Excel', 'Export', 'Feature', 1, '{"daily": 50, "monthly": 1000}', 1),
('ADVANCED_ANALYTICS', 'Phân tích nâng cao', 'Tính năng phân tích dữ liệu nâng cao với AI', 'Analytics', 'Feature', 1, '{"daily": 10, "monthly": 200}', 1),
('API_ACCESS', 'Truy cập API', 'Cho phép truy cập qua REST API', 'API', 'API', 1, '{"daily": 1000, "monthly": 50000}', 1),
('CLOUD_STORAGE', 'Lưu trữ đám mây', 'Dung lượng lưu trữ trên cloud', 'Storage', 'Resource', 1, '{"total": "10GB"}', 1),
('CONCURRENT_USERS', 'Người dùng đồng thời', 'Số lượng người dùng có thể sử dụng đồng thời', 'System', 'Resource', 1, '{"concurrent": 5}', 1),
('PREMIUM_SUPPORT', 'Hỗ trợ cao cấp', 'Hỗ trợ kỹ thuật 24/7', 'Support', 'Feature', 1, '{}', 1),
('DATA_BACKUP', 'Sao lưu dữ liệu', 'Sao lưu tự động và khôi phục dữ liệu', 'Backup', 'Feature', 1, '{"daily": 1, "retention_days": 30}', 1);

-- Sample license feature assignments (assuming license IDs exist)
-- Note: Chỉ uncomment khi đã có dữ liệu license
/*
INSERT INTO [dbo].[LicenseFeature] ([LicenseId], [ToolFeatureId], [IsEnabled], [UsageQuota], [CreatedBy])
SELECT 1, tf.Id, 1, 
    CASE 
        WHEN tf.FeatureCode = 'EXPORT_PDF' THEN '{"daily": 50, "monthly": 1000}'
        WHEN tf.FeatureCode = 'EXPORT_EXCEL' THEN '{"daily": 20, "monthly": 500}'
        WHEN tf.FeatureCode = 'API_ACCESS' THEN '{"daily": 500, "monthly": 10000}'
        WHEN tf.FeatureCode = 'CLOUD_STORAGE' THEN '{"total": "5GB"}'
        WHEN tf.FeatureCode = 'CONCURRENT_USERS' THEN '{"concurrent": 3}'
        ELSE tf.DefaultLimits
    END,
    1
FROM [dbo].[ToolFeature] tf
WHERE tf.FeatureCode IN ('EXPORT_PDF', 'EXPORT_EXCEL', 'API_ACCESS', 'CLOUD_STORAGE', 'CONCURRENT_USERS');
*/

PRINT 'Tool Feature Management tables created successfully!';
