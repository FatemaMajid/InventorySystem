using InventorySystem.Domain.Entities.Authorization;
using InventorySystem.Infrastructure.Authentication;
using InventorySystem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace InventorySystem.Infrastructure.Seed;

public static class UserSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        var username =
            configuration["SeedManager:Username"];

        var password =
            configuration["SeedManager:Password"];

        if (string.IsNullOrWhiteSpace(username))
        {
            throw new InvalidOperationException(
                "SeedManager:Username is not configured.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "SeedManager:Password is not configured.");
        }

        var managerRole =
            await context.Roles
                .FirstOrDefaultAsync(
                    x => x.Name == "Manager");

        if (managerRole is null)
        {
            throw new InvalidOperationException(
                "Manager role was not found.");
        }

        var existingUser =
            await context.Users
                .FirstOrDefaultAsync(
                    x => x.Username == username);

        if (existingUser is null)
        {
            var passwordService =
                new PasswordService();

            var user = new User
            {
                Username = username,

                PasswordHash =
                    passwordService.HashPassword(
                        password),

                IsActive = true,

                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);

            await context.SaveChangesAsync();

            context.UserRoles.Add(
                new UserRole
                {
                    UserId = user.Id,
                    RoleId = managerRole.Id
                });

            await context.SaveChangesAsync();

            return;
        }

        var alreadyManager =
            await context.UserRoles
                .AnyAsync(
                    x =>
                        x.UserId == existingUser.Id &&
                        x.RoleId == managerRole.Id);

        if (!alreadyManager)
        {
            context.UserRoles.Add(
                new UserRole
                {
                    UserId = existingUser.Id,
                    RoleId = managerRole.Id
                });

            await context.SaveChangesAsync();
        }
    }
}