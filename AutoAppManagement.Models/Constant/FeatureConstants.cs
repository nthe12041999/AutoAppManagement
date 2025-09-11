namespace AutoAppManagement.Models.Constant
{
    /// <summary>
    /// Constants for feature IDs used in the system
    /// </summary>
    public static class FeatureConstants
    {
        // Basic features - tất cả user đều có
        public const int SEND_MESSAGE = 1;
        public const int ADD_FRIEND = 2;
        public const int VIEW_FRIENDS = 3;
        public const int BASIC_INFO = 4;
        public const int USER_PROFILE = 5;

        // Bulk features - Premium và Enterprise
        public const int BULK_SEND_MESSAGE = 11;
        public const int BULK_ADD_FRIEND = 12;
        public const int BULK_INVITE_GROUP = 13;
        public const int BULK_IMPORT = 14;
        public const int BULK_EXPORT = 15;

        // AI features - Premium và Enterprise
        public const int AI_MESSAGE = 21;
        public const int AI_REPLY = 22;
        public const int AI_CONTENT_GENERATE = 23;
        public const int AI_TRANSLATION = 24;
        public const int AI_ANALYSIS = 25;

        // Advanced features - Premium và Enterprise
        public const int AUTO_REPLY = 31;
        public const int GROUP_MANAGEMENT = 32;
        public const int ANALYTICS = 33;
        public const int SCHEDULER = 34;
        public const int REPORTS = 35;

        // Premium features - chỉ Premium và Enterprise
        public const int MULTI_ACCOUNT = 41;
        public const int API_ACCESS = 42;
        public const int CUSTOM_SCRIPTS = 43;
        public const int WEBHOOKS = 44;
        public const int ADVANCED_ANALYTICS = 45;

        // Enterprise features - chỉ Enterprise
        public const int ENTERPRISE_SSO = 51;
        public const int ENTERPRISE_AUDIT = 52;
        public const int ENTERPRISE_BACKUP = 53;
        public const int ENTERPRISE_COMPLIANCE = 54;

        /// <summary>
        /// Get allowed features by license type
        /// </summary>
        /// <param name="licenseType">License type (Basic, Premium, Enterprise)</param>
        /// <returns>List of allowed feature IDs</returns>
        public static List<int> GetAllowedFeaturesByLicenseType(string licenseType)
        {
            return licenseType?.ToUpper() switch
            {
                "BASIC" => new List<int>
                {
                    SEND_MESSAGE,
                    ADD_FRIEND,
                    VIEW_FRIENDS,
                    BASIC_INFO,
                    USER_PROFILE
                },
                "PREMIUM" => new List<int>
                {
                    // Basic features
                    SEND_MESSAGE,
                    ADD_FRIEND,
                    VIEW_FRIENDS,
                    BASIC_INFO,
                    USER_PROFILE,
                    
                    // Bulk features
                    BULK_SEND_MESSAGE,
                    BULK_ADD_FRIEND,
                    BULK_INVITE_GROUP,
                    BULK_IMPORT,
                    BULK_EXPORT,
                    
                    // AI features
                    AI_MESSAGE,
                    AI_REPLY,
                    AI_CONTENT_GENERATE,
                    AI_TRANSLATION,
                    AI_ANALYSIS,
                    
                    // Advanced features
                    AUTO_REPLY,
                    GROUP_MANAGEMENT,
                    ANALYTICS,
                    SCHEDULER,
                    REPORTS,
                    
                    // Premium features
                    MULTI_ACCOUNT
                    // Không có API_ACCESS, CUSTOM_SCRIPTS để test restriction
                },
                "ENTERPRISE" => new List<int>
                {
                    // Tất cả features
                    SEND_MESSAGE,
                    ADD_FRIEND,
                    VIEW_FRIENDS,
                    BASIC_INFO,
                    USER_PROFILE,
                    
                    BULK_SEND_MESSAGE,
                    BULK_ADD_FRIEND,
                    BULK_INVITE_GROUP,
                    BULK_IMPORT,
                    BULK_EXPORT,
                    
                    AI_MESSAGE,
                    AI_REPLY,
                    AI_CONTENT_GENERATE,
                    AI_TRANSLATION,
                    AI_ANALYSIS,
                    
                    AUTO_REPLY,
                    GROUP_MANAGEMENT,
                    ANALYTICS,
                    SCHEDULER,
                    REPORTS,
                    
                    MULTI_ACCOUNT,
                    API_ACCESS,
                    CUSTOM_SCRIPTS,
                    WEBHOOKS,
                    ADVANCED_ANALYTICS,
                    
                    ENTERPRISE_SSO,
                    ENTERPRISE_AUDIT,
                    ENTERPRISE_BACKUP,
                    ENTERPRISE_COMPLIANCE
                },
                _ => new List<int> { SEND_MESSAGE, ADD_FRIEND, VIEW_FRIENDS, BASIC_INFO, USER_PROFILE }
            };
        }

        /// <summary>
        /// Check if feature is allowed for license type
        /// </summary>
        /// <param name="featureId">Feature ID to check</param>
        /// <param name="licenseType">License type</param>
        /// <returns>True if feature is allowed</returns>
        public static bool IsFeatureAllowed(int featureId, string licenseType)
        {
            var allowedFeatures = GetAllowedFeaturesByLicenseType(licenseType);
            return allowedFeatures.Contains(featureId);
        }

        /// <summary>
        /// Get feature name by ID
        /// </summary>
        /// <param name="featureId">Feature ID</param>
        /// <returns>Feature name</returns>
        public static string GetFeatureName(int featureId)
        {
            return featureId switch
            {
                SEND_MESSAGE => "Send Message",
                ADD_FRIEND => "Add Friend",
                VIEW_FRIENDS => "View Friends",
                BASIC_INFO => "Basic Info",
                USER_PROFILE => "User Profile",
                
                BULK_SEND_MESSAGE => "Bulk Send Message",
                BULK_ADD_FRIEND => "Bulk Add Friend",
                BULK_INVITE_GROUP => "Bulk Invite Group",
                BULK_IMPORT => "Bulk Import",
                BULK_EXPORT => "Bulk Export",
                
                AI_MESSAGE => "AI Message",
                AI_REPLY => "AI Reply",
                AI_CONTENT_GENERATE => "AI Content Generate",
                AI_TRANSLATION => "AI Translation",
                AI_ANALYSIS => "AI Analysis",
                
                AUTO_REPLY => "Auto Reply",
                GROUP_MANAGEMENT => "Group Management",
                ANALYTICS => "Analytics",
                SCHEDULER => "Scheduler",
                REPORTS => "Reports",
                
                MULTI_ACCOUNT => "Multi Account",
                API_ACCESS => "API Access",
                CUSTOM_SCRIPTS => "Custom Scripts",
                WEBHOOKS => "Webhooks",
                ADVANCED_ANALYTICS => "Advanced Analytics",
                
                ENTERPRISE_SSO => "Enterprise SSO",
                ENTERPRISE_AUDIT => "Enterprise Audit",
                ENTERPRISE_BACKUP => "Enterprise Backup",
                ENTERPRISE_COMPLIANCE => "Enterprise Compliance",
                
                _ => "Unknown Feature"
            };
        }
    }
}
