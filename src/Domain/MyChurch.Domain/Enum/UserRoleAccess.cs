namespace MyChurch.Domain.Enum
{
    public static class UserRoleAccess
    {
        public const string FinancialViewRoles = "Admin,PlatformAdmin,Pastor,Administration";
        public const string FinancialManageRoles = "Admin,PlatformAdmin,Administration";

        public static bool CanViewFinancialModule(UserRole role)
            => role is UserRole.Admin or UserRole.PlatformAdmin or UserRole.Pastor or UserRole.Administration;

        public static bool CanManageFinancialModule(UserRole role)
            => role is UserRole.Admin or UserRole.PlatformAdmin or UserRole.Administration;
    }
}
