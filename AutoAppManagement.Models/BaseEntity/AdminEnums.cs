namespace AutoAppManagement.Models.BaseEntity
{
    /// <summary>
    /// Enum for admin roles
    /// </summary>
    public enum AdminRole
    {
        Admin,
        Moderator,
        Support,
        Viewer
    }

    /// <summary>
    /// Enum for admin permissions
    /// </summary>
    public enum AdminPermission
    {
        ManageUsers,
        ManageAdmins,
        ManageProducts,
        ManageOrders,
        ManageLicenses,
        ViewReports,
        ManageSettings,
        ManageFiles,
        ViewLogs,
        ManageRoles,
        ManagePermissions
    }
}
