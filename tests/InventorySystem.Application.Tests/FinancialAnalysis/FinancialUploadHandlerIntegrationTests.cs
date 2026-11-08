using ClosedXML.Excel;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Features.FinancialAnalysis.Common.Excel;
using InventorySystem.Application.Features.FinancialAnalysis.FinancialUploads.Commands.UploadFinancialFile;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using InventorySystem.Infrastructure.Persistence.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace InventorySystem.Application.Tests.FinancialAnalysis;

public class FinancialUploadHandlerIntegrationTests {
    private const string DatabaseName = "InventorySystem_ConcurrencyTests";
    private const string ConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=InventorySystem_ConcurrencyTests;Integrated Security=True;TrustServerCertificate=True;";

    [Fact]
    public async Task Handle_InvalidExcel_PersistsInvalidUploadWithoutFinancialRows(){
        await VerifyDatabaseAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        int? periodId = null;

        try {
            await using (var setup = new ApplicationDbContext(options)){
                var period = new FinancialPeriod {
                    StartDate = new DateTime(2099, 2, 1),
                    EndDate = new DateTime(2099, 2, 7),
                    Status = FinancialPeriodStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                setup.FinancialPeriods.Add(period);
                await setup.SaveChangesAsync();
                periodId = period.Id;
            }

            using var workbook = new XLWorkbook();
            var sheet = workbook.AddWorksheet("Test");
            sheet.Cell(1, 1).Value = "Unrelated Column";

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            await using (var context = new ApplicationDbContext(options)){
                var storage = new FakeFileStorage();
                var audit = new FakeAuditLogService();

                var handler = new UploadFinancialFileCommandHandler(
                    context,
                    new FakeCurrentUserService(),
                    audit,
                    storage,
                    new FinancialExcelParser(context),
                    NullLogger<UploadFinancialFileCommandHandler>.Instance
                );

                var command = new UploadFinancialFileCommand {
                    FinancialPeriodId = periodId.Value,
                    FileType = FinancialFileType.IncomeStatement,
                    Scope = FinancialFileScope.AllBranches,
                    OriginalFileName = "invalid-headers.xlsx",
                    Content = stream.ToArray()
                };

                var result = await handler.Handle(command, CancellationToken.None);

                Assert.Equal(FinancialUploadStatus.Invalid, result.Status);
                Assert.False(result.IsActive);
                Assert.Equal(1, storage.SaveCount);
                Assert.Equal(1, audit.CallCount);
            }

            await using (var verify = new ApplicationDbContext(options)){
                var uploads = await verify.FinancialUploads
                    .AsNoTracking()
                    .Where(x => x.FinancialPeriodId == periodId.Value)
                    .ToListAsync();

                var upload = Assert.Single(uploads);

                Assert.Equal(FinancialUploadStatus.Invalid, upload.Status);
                Assert.False(upload.IsActive);
                Assert.Equal(1, upload.Version);

                var rowCount = await verify.FinancialAccountRows
                    .CountAsync(x => x.FinancialUploadId == upload.Id);

                Assert.Equal(0, rowCount);
            }
        }
        finally {
            if (periodId.HasValue){
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

[Fact]
public async Task Handle_ConcurrentSameSlot_RejectsDuplicateAndCleansUp(){
    await VerifyDatabaseAsync();

    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseSqlServer(ConnectionString)
        .Options;

    int? periodId = null;

    try {
        await using (var setup = new ApplicationDbContext(options)){
            var period = new FinancialPeriod {
                StartDate = new DateTime(2099, 3, 1),
                EndDate = new DateTime(2099, 3, 7),
                Status = FinancialPeriodStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            setup.FinancialPeriods.Add(period);
            await setup.SaveChangesAsync();
            periodId = period.Id;
        }

        using var workbook = new XLWorkbook();
        workbook.AddWorksheet("Test").Cell(1, 1).Value = "Invalid Header";
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        var storage = new CoordinatedFileStorage();
        var audit = new FakeAuditLogService();

        async Task<Exception?> ExecuteAsync(){
            await using var context = new ApplicationDbContext(options);
            var handler = new UploadFinancialFileCommandHandler(
                context,
                new FakeCurrentUserService(),
                audit,
                storage,
                new FinancialExcelParser(context),
                NullLogger<UploadFinancialFileCommandHandler>.Instance
            );

            try {
                await handler.Handle(new UploadFinancialFileCommand {
                    FinancialPeriodId = periodId!.Value,
                    FileType = FinancialFileType.IncomeStatement,
                    Scope = FinancialFileScope.AllBranches,
                    OriginalFileName = "invalid-headers.xlsx",
                    Content = content
                }, CancellationToken.None);
                return null;
            }
            catch (Exception ex){
                return ex;
            }
        }

        var results = await Task.WhenAll(
            ExecuteAsync(),
            ExecuteAsync()
        );

        Assert.Single(results, x => x is null);
        Assert.Single(results, x =>
            x is InventorySystem.Application.Common.Exceptions.DuplicateFinancialUploadException);

        await using var verify = new ApplicationDbContext(options);
        var uploads = await verify.FinancialUploads
            .AsNoTracking()
            .Where(x => x.FinancialPeriodId == periodId.Value)
            .ToListAsync();

        Assert.Single(uploads);
        Assert.Equal(1, storage.DeleteCount);
    }
    finally {
        if (periodId.HasValue){
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

private sealed class CoordinatedFileStorage : IFinancialFileStorageService {
    private readonly TaskCompletionSource<bool> _bothReady =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private int _saveCount;
    private int _deleteCount;

    public int DeleteCount => Volatile.Read(ref _deleteCount);

    public async Task<string> SaveAsync(
        byte[] content,
        string storedFileName,
        int year,
        int month,
        CancellationToken cancellationToken){
        if (Interlocked.Increment(ref _saveCount) == 2){
            _bothReady.TrySetResult(true);
        }

        await _bothReady.Task.WaitAsync(
            TimeSpan.FromSeconds(30),
            cancellationToken
        );

        return $"integration-test-only/{storedFileName}";
    }

    public Task DeleteAsync(
        string storagePath,
        CancellationToken cancellationToken){
        Interlocked.Increment(ref _deleteCount);
        return Task.CompletedTask;
    }
}

    [Fact]
    public async Task Handle_AuditFailure_RollsBackFinancialRows(){
        await VerifyDatabaseAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        int? periodId = null;

        try {
            await using (var setup = new ApplicationDbContext(options)){
                var period = new FinancialPeriod {
                    StartDate = new DateTime(2099, 4, 1),
                    EndDate = new DateTime(2099, 4, 7),
                    Status = FinancialPeriodStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                setup.FinancialPeriods.Add(period);
                await setup.SaveChangesAsync();
                periodId = period.Id;
            }

            using var workbook = new XLWorkbook();
            var sheet = workbook.AddWorksheet("Income");

            sheet.Cell(1, 1).Value = "رقم الحساب";
            sheet.Cell(1, 2).Value = "الاسم";
            sheet.Cell(1, 3).Value = "مدين";
            sheet.Cell(1, 4).Value = "دائن";
            sheet.Cell(1, 5).Value = "رصيد";

            sheet.Cell(2, 1).Value = "1001";
            sheet.Cell(2, 2).Value = "Test Account";
            sheet.Cell(2, 3).Value = 150;
            sheet.Cell(2, 4).Value = 50;
            sheet.Cell(2, 5).Value = 100;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            var storage = new FakeFileStorage();
            var audit = new FailFirstAuditLogService();

            await using (var context = new ApplicationDbContext(options)){
                var handler = new UploadFinancialFileCommandHandler(
                    context,
                    new FakeCurrentUserService(),
                    audit,
                    storage,
                    new FinancialExcelParser(context),
                    NullLogger<UploadFinancialFileCommandHandler>.Instance
                );

                var result = await handler.Handle(
                    new UploadFinancialFileCommand {
                        FinancialPeriodId = periodId.Value,
                        FileType = FinancialFileType.IncomeStatement,
                        Scope = FinancialFileScope.AllBranches,
                        OriginalFileName = "rollback-test.xlsx",
                        Content = stream.ToArray()
                    },
                    CancellationToken.None
                );

                Assert.Equal(FinancialUploadStatus.Invalid, result.Status);
                Assert.False(result.IsActive);
            }

            await using (var verify = new ApplicationDbContext(options)){
                var upload = Assert.Single(
                    await verify.FinancialUploads
                        .AsNoTracking()
                        .Where(x => x.FinancialPeriodId == periodId.Value)
                        .ToListAsync()
                );

                Assert.Equal(FinancialUploadStatus.Invalid, upload.Status);
                Assert.False(upload.IsActive);

                var accountRows = await verify.FinancialAccountRows
                    .CountAsync(x => x.FinancialUploadId == upload.Id);

                Assert.Equal(0, accountRows);

                var period = await verify.FinancialPeriods
                    .AsNoTracking()
                    .SingleAsync(x => x.Id == periodId.Value);

                Assert.NotEqual(FinancialPeriodStatus.Ready, period.Status);
            }

            Assert.Equal(1, storage.SaveCount);
            Assert.Equal(2, audit.CallCount);
        }
        finally {
            if (periodId.HasValue){
                await using var cleanup = new ApplicationDbContext(options);

                await cleanup.FinancialAccountRows
    .Where(x => x.FinancialUpload != null &&
        x.FinancialUpload.FinancialPeriodId == periodId.Value)
    .ExecuteDeleteAsync();

                await cleanup.FinancialUploads
                    .Where(x => x.FinancialPeriodId == periodId.Value)
                    .ExecuteDeleteAsync();

                await cleanup.FinancialPeriods
                    .Where(x => x.Id == periodId.Value)
                    .ExecuteDeleteAsync();
            }
        }
    }
    [Fact]
    public async Task Handle_StorageFailure_DoesNotCreateUploadOrChangePeriod(){
        await VerifyDatabaseAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        int? periodId = null;

        try {
            await using (var setup = new ApplicationDbContext(options)){
                var period = new FinancialPeriod {
                    StartDate = new DateTime(2099, 5, 1),
                    EndDate = new DateTime(2099, 5, 7),
                    Status = FinancialPeriodStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                setup.FinancialPeriods.Add(period);
                await setup.SaveChangesAsync();
                periodId = period.Id;
            }

            var storage = new FailingFileStorage();
            var audit = new FakeAuditLogService();

            await using (var context = new ApplicationDbContext(options)){
                var handler = new UploadFinancialFileCommandHandler(
                    context,
                    new FakeCurrentUserService(),
                    audit,
                    storage,
                    new FinancialExcelParser(context),
                    NullLogger<UploadFinancialFileCommandHandler>.Instance
                );

                var command = new UploadFinancialFileCommand {
                    FinancialPeriodId = periodId.Value,
                    FileType = FinancialFileType.IncomeStatement,
                    Scope = FinancialFileScope.AllBranches,
                    OriginalFileName = "storage-failure-test.xlsx",
                    Content = new byte[] { 1, 2, 3, 4 }
                };

                var exception = await Assert.ThrowsAsync<IOException>(
                    () => handler.Handle(command, CancellationToken.None)
                );

                Assert.Contains("Simulated storage failure", exception.Message);
            }

            await using (var verify = new ApplicationDbContext(options)){
                var uploadCount = await verify.FinancialUploads
                    .CountAsync(x => x.FinancialPeriodId == periodId.Value);

                Assert.Equal(0, uploadCount);

                var period = await verify.FinancialPeriods
                    .AsNoTracking()
                    .SingleAsync(x => x.Id == periodId.Value);

                Assert.Equal(FinancialPeriodStatus.Draft, period.Status);
            }

            Assert.Equal(1, storage.SaveCount);
            Assert.Equal(0, storage.DeleteCount);
            Assert.Equal(0, audit.CallCount);
        }
        finally {
            if (periodId.HasValue){
                await using var cleanup = new ApplicationDbContext(options);

                await cleanup.FinancialPeriods
                    .Where(x => x.Id == periodId.Value)
                    .ExecuteDeleteAsync();
            }
        }
    }
    [Fact]
    public async Task Handle_CancellationDuringTransaction_RollsBackAndMarksInvalid(){
        await VerifyDatabaseAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        int? periodId = null;

        try {
            await using (var setup = new ApplicationDbContext(options)){
                var period = new FinancialPeriod {
                    StartDate = new DateTime(2099, 6, 1),
                    EndDate = new DateTime(2099, 6, 7),
                    Status = FinancialPeriodStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                setup.FinancialPeriods.Add(period);
                await setup.SaveChangesAsync();
                periodId = period.Id;
            }

            using var workbook = new XLWorkbook();
            var sheet = workbook.AddWorksheet("Income");

            sheet.Cell(1, 1).Value = "رقم الحساب";
            sheet.Cell(1, 2).Value = "الاسم";
            sheet.Cell(1, 3).Value = "مدين";
            sheet.Cell(1, 4).Value = "دائن";
            sheet.Cell(1, 5).Value = "رصيد";

            sheet.Cell(2, 1).Value = "1001";
            sheet.Cell(2, 2).Value = "Test Account";
            sheet.Cell(2, 3).Value = 150;
            sheet.Cell(2, 4).Value = 50;
            sheet.Cell(2, 5).Value = 100;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            using var cancellation = new CancellationTokenSource();
            var storage = new FakeFileStorage();
            var audit = new CancelingAuditLogService(cancellation);

            await using (var context = new ApplicationDbContext(options)){
                var handler = new UploadFinancialFileCommandHandler(
                    context,
                    new FakeCurrentUserService(),
                    audit,
                    storage,
                    new FinancialExcelParser(context),
                    NullLogger<UploadFinancialFileCommandHandler>.Instance
                );

                var command = new UploadFinancialFileCommand {
                    FinancialPeriodId = periodId.Value,
                    FileType = FinancialFileType.IncomeStatement,
                    Scope = FinancialFileScope.AllBranches,
                    OriginalFileName = "cancellation-test.xlsx",
                    Content = stream.ToArray()
                };

                await Assert.ThrowsAnyAsync<OperationCanceledException>(
                    () => handler.Handle(command, cancellation.Token)
                );
            }

            Assert.True(cancellation.IsCancellationRequested);
            Assert.Equal(1, storage.SaveCount);
            Assert.Equal(2, audit.CallCount);

            await using (var verify = new ApplicationDbContext(options)){
                var upload = Assert.Single(
                    await verify.FinancialUploads
                        .AsNoTracking()
                        .Where(x => x.FinancialPeriodId == periodId.Value)
                        .ToListAsync()
                );

                Assert.Equal(FinancialUploadStatus.Invalid, upload.Status);
                Assert.False(upload.IsActive);

                var accountRows = await verify.FinancialAccountRows
                    .CountAsync(x => x.FinancialUploadId == upload.Id);

                Assert.Equal(0, accountRows);

                var period = await verify.FinancialPeriods
                    .AsNoTracking()
                    .SingleAsync(x => x.Id == periodId.Value);

                Assert.Equal(FinancialPeriodStatus.Uploading, period.Status);
            }
        }
        finally {
            if (periodId.HasValue){
                await using var cleanup = new ApplicationDbContext(options);

                await cleanup.FinancialAccountRows
                    .Where(x => x.FinancialUpload != null &&
                        x.FinancialUpload.FinancialPeriodId == periodId.Value)
                    .ExecuteDeleteAsync();

                await cleanup.FinancialUploads
                    .Where(x => x.FinancialPeriodId == periodId.Value)
                    .ExecuteDeleteAsync();

                await cleanup.FinancialPeriods
                    .Where(x => x.Id == periodId.Value)
                    .ExecuteDeleteAsync();
            }
        }
    }
    [Fact]
    public async Task Handle_CancelAfterStorage_CleansFileAndDoesNotSaveUpload(){
        await VerifyDatabaseAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        int? periodId = null;

        try {
            await using (var setup = new ApplicationDbContext(options)){
                var period = new FinancialPeriod {
                    StartDate = new DateTime(2099, 7, 1),
                    EndDate = new DateTime(2099, 7, 7),
                    Status = FinancialPeriodStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                setup.FinancialPeriods.Add(period);
                await setup.SaveChangesAsync();
                periodId = period.Id;
            }

            using var cancellation = new CancellationTokenSource();
            var storage = new CancelAfterSaveStorage(cancellation);
            var audit = new FakeAuditLogService();

            await using (var context = new ApplicationDbContext(options)){
                var handler = new UploadFinancialFileCommandHandler(
                    context,
                    new FakeCurrentUserService(),
                    audit,
                    storage,
                    new FinancialExcelParser(context),
                    NullLogger<UploadFinancialFileCommandHandler>.Instance
                );

                var command = new UploadFinancialFileCommand {
                    FinancialPeriodId = periodId.Value,
                    FileType = FinancialFileType.IncomeStatement,
                    Scope = FinancialFileScope.AllBranches,
                    OriginalFileName = "cancel-after-storage.xlsx",
                    Content = new byte[] { 1, 2, 3, 4 }
                };

                await Assert.ThrowsAnyAsync<OperationCanceledException>(
                    () => handler.Handle(command, cancellation.Token)
                );
            }

            Assert.True(cancellation.IsCancellationRequested);
            Assert.Equal(1, storage.SaveCount);
            Assert.Equal(1, storage.DeleteCount);
            Assert.Equal(0, audit.CallCount);
            Assert.Equal("integration-test-only", storage.DeletedPath);

            await using (var verify = new ApplicationDbContext(options)){
                var uploadCount = await verify.FinancialUploads
                    .CountAsync(x => x.FinancialPeriodId == periodId.Value);

                Assert.Equal(0, uploadCount);

                var period = await verify.FinancialPeriods
                    .AsNoTracking()
                    .SingleAsync(x => x.Id == periodId.Value);

                Assert.Equal(FinancialPeriodStatus.Draft, period.Status);
            }
        }
        finally {
            if (periodId.HasValue){
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
    [Fact]
    public async Task Handle_CleanupFailure_PreservesCancellationAndDatabaseState(){
        await VerifyDatabaseAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        int? periodId = null;

        try {
            await using (var setup = new ApplicationDbContext(options)){
                var period = new FinancialPeriod {
                    StartDate = new DateTime(2099, 8, 1),
                    EndDate = new DateTime(2099, 8, 7),
                    Status = FinancialPeriodStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                setup.FinancialPeriods.Add(period);
                await setup.SaveChangesAsync();
                periodId = period.Id;
            }

            using var cancellation = new CancellationTokenSource();
            var storage = new FailingCleanupStorage(cancellation);
            var audit = new FakeAuditLogService();

            await using (var context = new ApplicationDbContext(options)){
                var handler = new UploadFinancialFileCommandHandler(
                    context,
                    new FakeCurrentUserService(),
                    audit,
                    storage,
                    new FinancialExcelParser(context),
                    NullLogger<UploadFinancialFileCommandHandler>.Instance
                );

                var command = new UploadFinancialFileCommand {
                    FinancialPeriodId = periodId.Value,
                    FileType = FinancialFileType.IncomeStatement,
                    Scope = FinancialFileScope.AllBranches,
                    OriginalFileName = "cleanup-failure.xlsx",
                    Content = new byte[] { 1, 2, 3, 4 }
                };

                await Assert.ThrowsAnyAsync<OperationCanceledException>(
                    () => handler.Handle(command, cancellation.Token)
                );
            }

            Assert.True(cancellation.IsCancellationRequested);
            Assert.Equal(1, storage.SaveCount);
            Assert.Equal(1, storage.DeleteCount);
            Assert.True(storage.FileStillPresent);
            Assert.Equal(0, audit.CallCount);

            await using (var verify = new ApplicationDbContext(options)){
                Assert.Equal(
                    0,
                    await verify.FinancialUploads.CountAsync(
                        x => x.FinancialPeriodId == periodId.Value
                    )
                );

                var period = await verify.FinancialPeriods
                    .AsNoTracking()
                    .SingleAsync(x => x.Id == periodId.Value);

                Assert.Equal(FinancialPeriodStatus.Draft, period.Status);
            }
        }
        finally {
            if (periodId.HasValue){
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

    private sealed class FailingCleanupStorage : IFinancialFileStorageService {
        private readonly CancellationTokenSource _cancellation;

        public int SaveCount { get; private set; }
        public int DeleteCount { get; private set; }
        public bool FileStillPresent { get; private set; }

        public FailingCleanupStorage(CancellationTokenSource cancellation){
            _cancellation = cancellation;
        }

        public Task<string> SaveAsync(
            byte[] content,
            string storedFileName,
            int year,
            int month,
            CancellationToken cancellationToken){
            SaveCount++;
            FileStillPresent = true;
            _cancellation.Cancel();
            return Task.FromResult("integration-test-only");
        }

        public Task DeleteAsync(
            string storagePath,
            CancellationToken cancellationToken){
            DeleteCount++;
            throw new IOException("Simulated cleanup failure.");
        }
    }
    [Fact]
    public async Task ParseIncomeStatement_PreCanceledToken_ThrowsCancellation(){
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet("Income");

        sheet.Cell(1, 1).Value = "رقم الحساب";
        sheet.Cell(1, 2).Value = "الاسم";
        sheet.Cell(1, 3).Value = "مدين";
        sheet.Cell(1, 4).Value = "دائن";
        sheet.Cell(1, 5).Value = "رصيد";

        sheet.Cell(2, 1).Value = "1001";
        sheet.Cell(2, 2).Value = "Test Account";
        sheet.Cell(2, 3).Value = 150;
        sheet.Cell(2, 4).Value = 50;
        sheet.Cell(2, 5).Value = 100;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        await using var context = new ApplicationDbContext(options);
        var parser = new FinancialExcelParser(context);

        var upload = new FinancialUpload {
            Id = 1,
            FileType = FinancialFileType.IncomeStatement
        };

        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => parser.ParseAsync(
                upload,
                stream.ToArray(),
                cancellation.Token)
        );
    }
    private static async Task VerifyDatabaseAsync(){
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

        if (Convert.ToInt32(await command.ExecuteScalarAsync()) != 1){
            throw new InvalidOperationException(
                "Integration tests require the isolated LocalDB database.");
        }
    }

    private sealed class FakeFileStorage : IFinancialFileStorageService {
        public int SaveCount { get; private set; }

        public Task<string> SaveAsync(
            byte[] content,
            string storedFileName,
            int year,
            int month,
            CancellationToken cancellationToken){
            SaveCount++;
            return Task.FromResult("integration-test-only");
        }

        public Task DeleteAsync(
            string storagePath,
            CancellationToken cancellationToken){
            return Task.CompletedTask;
        }
    }

    private sealed class FakeAuditLogService : IAuditLogService {
        public int CallCount { get; private set; }

        public Task LogAsync(
            string action,
            string entity,
            string? entityId = null,
            string? details = null,
            string status = "Success",
            CancellationToken cancellationToken = default){
            CallCount++;
            return Task.CompletedTask;
        }
    }

        private sealed class FailFirstAuditLogService : IAuditLogService {
        public int CallCount { get; private set; }

        public Task LogAsync(
            string action,
            string entity,
            string? entityId = null,
            string? details = null,
            string status = "Success",
            CancellationToken cancellationToken = default){
            CallCount++;

            if (CallCount == 1){
                throw new InvalidOperationException(
                    "Simulated audit failure before transaction commit.");
            }

            return Task.CompletedTask;
        }
    }
        private sealed class FailingFileStorage : IFinancialFileStorageService {
        public int SaveCount { get; private set; }
        public int DeleteCount { get; private set; }

        public Task<string> SaveAsync(
            byte[] content,
            string storedFileName,
            int year,
            int month,
            CancellationToken cancellationToken){
            SaveCount++;

            throw new IOException("Simulated storage failure.");
        }

        public Task DeleteAsync(
            string storagePath,
            CancellationToken cancellationToken){
            DeleteCount++;
            return Task.CompletedTask;
        }
    }
        private sealed class CancelingAuditLogService : IAuditLogService {
        private readonly CancellationTokenSource _cancellation;

        public int CallCount { get; private set; }

        public CancelingAuditLogService(CancellationTokenSource cancellation){
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

            if (CallCount == 1){
                _cancellation.Cancel();
                throw new OperationCanceledException(_cancellation.Token);
            }

            return Task.CompletedTask;
        }
    }
    private sealed class CancelAfterSaveStorage : IFinancialFileStorageService {
        private readonly CancellationTokenSource _cancellation;

        public int SaveCount { get; private set; }
        public int DeleteCount { get; private set; }
        public string? DeletedPath { get; private set; }

        public CancelAfterSaveStorage(CancellationTokenSource cancellation){
            _cancellation = cancellation;
        }

        public Task<string> SaveAsync(
            byte[] content,
            string storedFileName,
            int year,
            int month,
            CancellationToken cancellationToken){
            SaveCount++;
            _cancellation.Cancel();
            return Task.FromResult("integration-test-only");
        }

        public Task DeleteAsync(
            string storagePath,
            CancellationToken cancellationToken){
            DeleteCount++;
            DeletedPath = storagePath;
            return Task.CompletedTask;
        }
    }
    private sealed class FakeCurrentUserService : ICurrentUserService {
        public int? UserId => null;
        public string? Username => "integration-test";
        public IReadOnlyCollection<string> Roles => Array.Empty<string>();
        public bool IsManager => false;
        public bool IsAdmin => false;
    }
}
