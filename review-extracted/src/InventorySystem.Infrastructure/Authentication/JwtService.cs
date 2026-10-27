using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventorySystem.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace InventorySystem.Infrastructure.Authentication;

public sealed class JwtService
    : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(
        int userId,
        string username,
        IEnumerable<string> roles,
        IEnumerable<string> permissions)
    {
        var secret =
            _configuration["Jwt:Secret"];

        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException(
                "JWT secret is not configured.");
        }

        var issuer =
            _configuration["Jwt:Issuer"]
            ?? "InventorySystem";

        var audience =
            _configuration["Jwt:Audience"]
            ?? "InventorySystem.Frontend";

        var expiresInHours =
            int.TryParse(
                _configuration["Jwt:ExpiresInHours"],
                out var hours)
                ? hours
                : 8;

        var claims =
            new List<Claim>
            {
                new(
                    JwtRegisteredClaimNames.Sub,
                    userId.ToString()),

                new(
                    JwtRegisteredClaimNames.UniqueName,
                    username),

                new(
                    ClaimTypes.NameIdentifier,
                    userId.ToString()),

                new(
                    ClaimTypes.Name,
                    username)
            };

        foreach (var role in roles)
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role));
        }

        foreach (var permission in permissions)
        {
            claims.Add(
                new Claim(
                    "permission",
                    permission));
        }

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secret));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(
                    expiresInHours),
                signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}