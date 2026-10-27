using Microsoft.AspNetCore.Authorization;

namespace InventorySystem.API.Authorization;

public sealed class HasPermissionAttribute
    : AuthorizeAttribute
{
    public HasPermissionAttribute(
        string permission)
    {
        Policy =
            PermissionPolicyProvider.PolicyPrefix
            + permission;
    }
}