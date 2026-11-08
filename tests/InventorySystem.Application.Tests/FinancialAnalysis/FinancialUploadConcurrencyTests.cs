
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using InventorySystem.Infrastructure.Persistence.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InventorySystem.Application.Tests.FinancialAnalysis;

public class FinancialUploadConcurrencyTests
{
    private const string DatabaseName = "InventorySystem_ConcurrencyTests";
    private const string ConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=InventorySystem_ConcurrencyTests;Integrated Security=True;TrustServerCertificate=True;";

    [Fact]
    public async Task ConcurrentSameSlotVersion_AllowsOnlyOneUpload()
    {
        await VerifyTestDatabaseAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        int? periodId = null;

        try
        {
            await using (var setup = new ApplicationDbContext(options))
            {
                var period = new FinancialPeriod
                {
                    StartDate = new DateTime(2099, 1, 1),
                    EndDate = new DateTime(2099, 1, 7),
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                setup.FinancialPeriods.Add(period);
                await setup.SaveChangesAsync();
                periodId = period.Id;
            }

            await using var first = new ApplicationDbContext(options);
            await using var second = new ApplicationDbContext(options);

            first.FinancialUploads.Add(CreateUpload(periodId.Value));
            second.FinancialUploads.Add(CreateUpload(periodId.Value));

            var results = await Task.WhenAll(
                AttemptSaveAsync(first),
                AttemptSaveAsync(second)
            );

            Assert.Single(results, x => x is null);

            var failure = Assert.Single(results, x => x is not null);
            var dbException = Assert.IsType<DbUpdateException>(failure);
            var sqlException = Assert.IsType<SqlException>(
                dbException.InnerException
            );

            Assert.Contains(sqlException.Number, new[] { 2601, 2627 });

            await using var verify = new ApplicationDbContext(options);

            var saved = await verify.FinancialUploads
                .CountAsync(x => x.FinancialPeriodId == periodId.Value);

            Assert.Equal(1, saved);
        }
        finally
        {
            if (periodId.HasValue)
            {
                await using var cleanup = new ApplicationDbContext(options);

                await cleanup.FinancialUploads
                    .Where(x => x.FinancialPeriodId == periodId.Value)
                    .ExecuteDeleteAsync();

                await cleanup.FinancialPeriods
                    .Where(x => x.Id == periodId.Value)
                    .ExecuteDeleteAsync();
            }
        }
    }

    private static FinancialUpload CreateUpload(int periodId)
    {
        var fileName = $"{Guid.NewGuid():N}.xlsx";

        return new FinancialUpload
        {
            FinancialPeriodId = periodId,
            FileType = FinancialFileType.Transfer,
            Scope = FinancialFileScope.AllBranches,
            BranchId = null,
            Gender = null,
            OriginalFileName = fileName,
            StoredFileName = fileName,
            StoragePath = "integration-test-only",
            FileHash = Guid.NewGuid().ToString("N"),
            FileSize = 100,
            Version = 1,
            IsActive = false,
            IsDeleted = false,
            UploadedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static async Task<Exception?> AttemptSaveAsync(
        ApplicationDbContext context)
    {
        try
        {
            await context.SaveChangesAsync();
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private static async Task VerifyTestDatabaseAsync()
    {
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

        if (Convert.ToInt32(result) != 1)
        {
            throw new InvalidOperationException(
                "Integration test must run on the isolated LocalDB database.");
        }
    }
}
