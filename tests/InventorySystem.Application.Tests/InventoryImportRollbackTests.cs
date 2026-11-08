using InventorySystem.Application.Common.Excel;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Features.Inventory.InventorySessions.Commands.ConfirmInventoryImport;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Enums;
using InventorySystem.Infrastructure.Persistence.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Tests;

public class InventoryImportRollbackTests {
    private const string ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=InventorySystem_ConcurrencyTests;Integrated Security=True;TrustServerCertificate=True;";

    [Fact]
    public async Task CancellationAfterSave_RollsBackInventorySessionAndItems(){
        await VerifyDatabaseAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        int? branchId = null;
        int? storeId = null;
        var sessionNumber = $"TEST-ROLLBACK-{Guid.NewGuid():N}";
        var itemCode = $"TEST-{Guid.NewGuid():N}";

        try {
            await using (var setup = new ApplicationDbContext(options)){
                var usedCodes = await setup.Branches
                    .Select(x => x.BranchCode)
                    .ToListAsync();

                var branchCode = Enumerable.Range(900, 100)
                    .Select(x => x.ToString("D3"))
                    .FirstOrDefault(x => !usedCodes.Contains(x));

                Assert.False(string.IsNullOrEmpty(branchCode));

                var branch = new Branch {
                    BranchCode = branchCode!,
                    BranchNameArabic = "Rollback Test Branch",
                    IsActive = true
                };

                var store = new Store {
                    BranchCode = branchCode!,
                    StoreCode = "999",
                    StoreNameArabic = "Rollback Test Store",
                    IsActive = true
                };

                setup.Branches.Add(branch);
                setup.Stores.Add(store);
                await setup.SaveChangesAsync();

                branchId = branch.Id;
                storeId = store.Id;
            }

            using var cancellation = new CancellationTokenSource();
            var audit = new CancelAtAuditService(cancellation);

            await using (var context = new ApplicationDbContext(options)){
                var handler = new ConfirmInventoryImportCommandHandler(
                    context,
                    audit
                );

                var before = new InventoryExcelRow {
                    ItemCode = itemCode,
                    ItemName1 = "Rollback Test Item",
                    Quantity = 5,
                    Price = 100
                };

                var after = new InventoryExcelRow {
                    ItemCode = itemCode,
                    ItemName1 = "Rollback Test Item",
                    Quantity = 3,
                    Price = 100
                };

                var command = new ConfirmInventoryImportCommand(
                    new List<InventoryExcelRow> { before },
                    new List<InventoryExcelRow> { after },
                    branchId.Value,
                    storeId.Value,
                    new DateTime(2099, 9, 1),
                    sessionNumber,
                    InventoryType.Annual,
                    "before.xlsx",
                    "after.xlsx"
                );

                await Assert.ThrowsAnyAsync<OperationCanceledException>(
                    () => handler.Handle(command, cancellation.Token)
                );
            }

            Assert.True(cancellation.IsCancellationRequested);
            Assert.Equal(1, audit.CallCount);
            Assert.True(audit.SavedSessionId > 0);

            await using (var verify = new ApplicationDbContext(options)){
                Assert.False(await verify.InventorySessions.AsNoTracking()
                    .AnyAsync(x => x.SessionNumber == sessionNumber));

                Assert.False(await verify.InventoryDetails.AsNoTracking()
                    .AnyAsync(x => x.InventorySessionId == audit.SavedSessionId));

                Assert.False(await verify.Items.AsNoTracking()
                    .AnyAsync(x => x.ItemCode == itemCode));
            }
        }
        finally {
            if (branchId.HasValue && storeId.HasValue){
                await using var cleanup = new ApplicationDbContext(options);
                await cleanup.Stores
                    .Where(x => x.Id == storeId.Value)
                    .ExecuteDeleteAsync();

                await cleanup.Branches
                    .Where(x => x.Id == branchId.Value)
                    .ExecuteDeleteAsync();
            }
        }
    }

    private static async Task VerifyDatabaseAsync(){
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT CASE
                WHEN CAST(SERVERPROPERTY('IsLocalDB') AS int) = 1
                 AND DB_NAME() = 'InventorySystem_ConcurrencyTests'
                THEN 1 ELSE 0
            END;";

        var result = await command.ExecuteScalarAsync();

        if (Convert.ToInt32(result) != 1){
            throw new InvalidOperationException(
                "Only the isolated LocalDB test database is allowed.");
        }
    }

    private sealed class CancelAtAuditService : IAuditLogService {
        private readonly CancellationTokenSource _cancellation;

        public int CallCount { get; private set; }
        public int SavedSessionId { get; private set; }

        public CancelAtAuditService(CancellationTokenSource cancellation){
            _cancellation = cancellation;
        }

        public Task LogAsync(
            string action,
            string entity,
            string? entityId = null,
            string? details = null,
            string status = "Success",
            CancellationToken cancellationToken = default){
            CallCount++;

            Assert.True(int.TryParse(entityId, out var sessionId));
            SavedSessionId = sessionId;

            _cancellation.Cancel();
            throw new OperationCanceledException(_cancellation.Token);
        }
    }
}
