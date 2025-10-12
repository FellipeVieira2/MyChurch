using Microsoft.AspNetCore.Authorization;
using MyChurch.Domain.Services;
using System.Security.Claims;

namespace MyChurch.Api.Web.Authorization
{
    /// <summary>
    /// Handler para verificar permissões granulares
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionAuthorizationHandler> _logger;

        public PermissionAuthorizationHandler(
            IPermissionService permissionService,
            ILogger<PermissionAuthorizationHandler> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // 1. Obter o UserId do claim
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                _logger.LogWarning("User ID claim not found or invalid");
                context.Fail();
                return;
            }

            try
            {
                // 2. Verificar permissões
                bool hasPermission;

                if (requirement.RequireAll)
                {
                    // Usuário deve ter TODAS as permissões
                    hasPermission = await _permissionService.HasAllPermissionsAsync(
                        userId, 
                        requirement.Permissions);
                }
                else
                {
                    // Usuário deve ter PELO MENOS UMA permissão
                    hasPermission = await _permissionService.HasAnyPermissionAsync(
                        userId, 
                        requirement.Permissions);
                }

                if (hasPermission)
                {
                    _logger.LogDebug("User {UserId} has required permissions", userId);
                    context.Succeed(requirement);
                }
                else
                {
                    _logger.LogWarning(
                        "User {UserId} does not have required permissions: {Permissions}", 
                        userId, 
                        string.Join(", ", requirement.Permissions));
                    context.Fail();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permissions for user {UserId}", userId);
                context.Fail();
            }
        }
    }
}
