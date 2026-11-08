using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Entities.Authorization;
using InventorySystem.Infrastructure.Persistence.Contexts;
using InventorySystem.Infrastructure.Seed;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace InventorySystem.Application.Tests;

public class SeederBehaviorTests {
    private const string DatabaseName = "InventorySystem_ConcurrencyTests";
    private const string ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=InventorySystem_ConcurrencyTests;Integrated Security=True;TrustServerCertificate=True;";

    [Fact]
    public async Task StartupSeeders_PreserveExistingMasterDataAndRevokedManagerRole(){
        await VerifyIsolatedDatabaseAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        await using var context = new ApplicationDbContext(options);
        await using var transaction = await context.Database.BeginTransactionAsync();

        try {
            var branch = await context.Branches
                .SingleOrDefaultAsync(x => x.BranchCode == "001");

            if (branch is null){
                branch = new Branch { BranchCode = "001" };
                context.Branches.Add(branch);
            }

            branch.BranchNameArabic = "Test Custom Branch";
            branch.BranchNameEnglish = "Test Custom Branch";
            branch.IsActive = false;

            var store = await context.Stores
                .SingleOrDefaultAsync(x => x.BranchCode == "001" && x.StoreCode == "001");

            if (store is null){
                store = new Store {
                    BranchCode = "001",
                    StoreCode = "001"
                };
                context.Stores.Add(store);
            }

            store.StoreNameArabic = "Test Custom Store";
            store.StoreNameEnglish = "Test Custom Store";
            store.IsActive = false;

            var managerRole = await context.Roles
                .SingleOrDefaultAsync(x => x.Name == "Manager");

            if (managerRole is null){
                managerRole = new Role { Name = "Manager" };
                context.Roles.Add(managerRole);
            }

            await context.SaveChangesAsync();

            await BranchSeeder.SeedAsync(context);
            await StoreSeeder.SeedAsync(context);

            Assert.Equal("Test Custom Branch", branch.BranchNameEnglish);
            Assert.False(branch.IsActive);
            Assert.Equal("Test Custom Store", store.StoreNameEnglish);
            Assert.False(store.IsActive);

            var username = $"seed-test-{Guid.NewGuid():N}";
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    ["SeedManager:Username"] = username,
                    ["SeedManager:Password"] = "TestOnly-Password-2026!"
                })
                .Build();

            await UserSeeder.SeedAsync(context, configuration);

            var user = await context.Users
                .SingleAsync(x => x.Username == username);

            var userRole = await context.UserRoles
                .SingleAsync(x => x.UserId == user.Id && x.RoleId == managerRole.Id);

            context.UserRoles.Remove(userRole);
            await context.SaveChangesAsync();

            Assert.False(await context.UserRoles.AnyAsync(
                x => x.UserId == user.Id && x.RoleId == managerRole.Id));

            await UserSeeder.SeedAsync(context, configuration);

            Assert.False(await context.UserRoles.AnyAsync(x => x.UserId == user.Id && x.RoleId == managerRole.Id));
        }
        finally {
            await transaction.RollbackAsync(CancellationToken.None);
        }
    }

    private static async Task VerifyIsolatedDatabaseAsync(){
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT CASE
                WHEN CAST(SERVERPROPERTY('IsLocalDB') AS int) = 1
                 AND DB_NAME() = @databaseName
                THEN 1 ELSE 0
            END;";

        command.Parameters.AddWithValue("@databaseName", DatabaseName);

        var result = await command.ExecuteScalarAsync();

        if (Convert.ToInt32(result) != 1){
            throw new InvalidOperationException(
                "Test requires the isolated LocalDB database.");
        }
    }
}
