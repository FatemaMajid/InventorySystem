using System.Security.Claims;
using Microsoft.AspNetCore.Http;

using InventorySystem.Application.Common.Interfaces;

namespace InventorySystem.Infrastructure.Authentication;

public sealed class CurrentUserService
    : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    public int? UserId
    {
        get
        {
            var value =
                User?.FindFirst(
                    ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(value, out var id)
                ? id
                : null;
        }
    }

    public string? Username =>
        User?.FindFirst(
            ClaimTypes.Name)?.Value;

    public IReadOnlyCollection<string> Roles =>
        User?
            .FindAll(ClaimTypes.Role)
            .Select(x => x.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList()
        ?? [];

    public bool IsManager =>
        User?.IsInRole("Manager") == true;

    public bool IsAdmin =>
        User?.IsInRole("Admin") == true;
}