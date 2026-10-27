using Microsoft.AspNetCore.Authorization;

namespace InventorySystem.API.Authorization;

public sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Manager has full access
        if (context.User.IsInRole("Manager"))
        {
            context.Succeed(requirement);

            return Task.CompletedTask;
        }

        // Admin / User:
        // Check permission claim in JWT
        var hasPermission =
            context.User.Claims.Any(
                claim =>
                    claim.Type == "permission" &&
                    claim.Value.Equals(
                        requirement.Permission,
                        StringComparison.OrdinalIgnoreCase));

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}