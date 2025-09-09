-- Create LicenseFeatures table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='LicenseFeatures' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[LicenseFeatures] (
        [Id] [bigint] IDENTITY(1,1) NOT NULL,
        [LicenseId] [bigint] NOT NULL,
        [ToolFeatureId] [bigint] NOT NULL,
        [ResourceLimits] [nvarchar](500) NULL,
        [UsageQuota] [nvarchar](500) NULL,
        [IsEnabled] [bit] NOT NULL DEFAULT 1,
        [EffectiveFrom] [datetime2](7) NULL,
        [EffectiveTo] [datetime2](7) NULL,
        [Configuration] [ntext] NULL,
        [CreatedDate] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedDate] [datetime2](7) NULL,
        [CreatedBy] [bigint] NULL,
        [UpdatedBy] [bigint] NULL,
        [IsDeleted] [bit] NOT NULL DEFAULT 0,
        [DeletedDate] [datetime2](7) NULL,
        [DeletedBy] [bigint] NULL,
        [RowVersion] [rowversion] NULL,
        [Notes] [nvarchar](1000) NULL,
        [Status] [nvarchar](20) NOT NULL DEFAULT 'Active',
        
        CONSTRAINT [PK_LicenseFeature] PRIMARY KEY CLUSTERED ([Id] ASC)
        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

    -- Add Foreign Key constraints
    ALTER TABLE [dbo].[LicenseFeatures] WITH CHECK ADD CONSTRAINT [FK_LicenseFeature_License] 
        FOREIGN KEY([LicenseId]) REFERENCES [dbo].[Licenses] ([Id]) ON DELETE CASCADE

    ALTER TABLE [dbo].[LicenseFeatures] CHECK CONSTRAINT [FK_LicenseFeature_License]

    -- FK to ToolFeatures table (assuming it exists)
    IF EXISTS (SELECT * FROM sysobjects WHERE name='ToolFeatures' AND xtype='U')
    BEGIN
        ALTER TABLE [dbo].[LicenseFeatures] WITH CHECK ADD CONSTRAINT [FK_LicenseFeature_ToolFeature] 
            FOREIGN KEY([ToolFeatureId]) REFERENCES [dbo].[ToolFeatures] ([Id]) ON DELETE CASCADE
        
        ALTER TABLE [dbo].[LicenseFeatures] CHECK CONSTRAINT [FK_LicenseFeature_ToolFeature]
    END

    -- FK to Accounts table for CreatedBy
    IF EXISTS (SELECT * FROM sysobjects WHERE name='Accounts' AND xtype='U')
    BEGIN
        ALTER TABLE [dbo].[LicenseFeatures] WITH CHECK ADD CONSTRAINT [FK_LicenseFeature_CreatedBy] 
            FOREIGN KEY([CreatedBy]) REFERENCES [dbo].[Accounts] ([Id]) ON DELETE NO ACTION
        
        ALTER TABLE [dbo].[LicenseFeatures] CHECK CONSTRAINT [FK_LicenseFeature_CreatedBy]

        ALTER TABLE [dbo].[LicenseFeatures] WITH CHECK ADD CONSTRAINT [FK_LicenseFeature_UpdatedBy] 
            FOREIGN KEY([UpdatedBy]) REFERENCES [dbo].[Accounts] ([Id]) ON DELETE NO ACTION
        
        ALTER TABLE [dbo].[LicenseFeatures] CHECK CONSTRAINT [FK_LicenseFeature_UpdatedBy]
    END

    PRINT 'LicenseFeatures table created successfully'
END
ELSE
BEGIN
    PRINT 'LicenseFeatures table already exists'
END
