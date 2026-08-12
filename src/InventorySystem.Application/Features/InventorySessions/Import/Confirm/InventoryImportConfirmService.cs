using InventorySystem.Application.Common.Excel;
using InventorySystem.Application.Common.Excel.Validation;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Features.InventorySessions.Commands.ConfirmInventoryImport;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.InventorySessions.Import.Confirm;

public class InventoryImportConfirmService
{
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public InventoryImportConfirmService(
        IApplicationDbContext context,
        IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<InventoryImportConfirmResponse> ConfirmAsync(
        InventoryImportConfirmRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateFile(request);

        // Read Excel
        request.FileStream.Position = 0;

        var rows = ExcelReader.Read(request.FileStream);

        if (rows.Count == 0)
            throw new InvalidOperationException(
                "ملف Excel لا يحتوي على بيانات.");

        // Detect header
        request.FileStream.Position = 0;

        using var workbook =
            new ClosedXML.Excel.XLWorkbook(request.FileStream);

        var worksheet =
            workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
            throw new InvalidOperationException(
                "ملف Excel لا يحتوي على أي Sheet.");

        var (headerRow, _) =
            ExcelHeaderDetector.Detect(worksheet);

        // Validate rows
        var validation =
            ExcelValidator.Validate(
                rows,
                headerRow);

        if (!validation.IsValid)
            throw new InvalidOperationException(
                "ملف Excel يحتوي على أخطاء في بيانات الأصناف.");

        // Validate locations
        var locationValidation =
            InventoryLocationValidator.Validate(
                rows,
                headerRow);

        if (!locationValidation.IsValid)
            throw new InvalidOperationException(
                "ملف Excel يحتوي على أخطاء في الفرع أو المستودع.");

        // Get branch/store from Excel
        var branchName =
            rows.First().Branch?.Trim();

        var storeName =
            rows.First().Store?.Trim();

        if (string.IsNullOrWhiteSpace(branchName))
            throw new InvalidOperationException(
                "الفرع غير موجود في ملف Excel.");

        if (string.IsNullOrWhiteSpace(storeName))
            throw new InvalidOperationException(
                "المستودع غير موجود في ملف Excel.");

        var branch =
            await _context.Branches
                .FirstOrDefaultAsync(
                    x => x.BranchNameArabic == branchName,
                    cancellationToken);

        if (branch == null)
            throw new InvalidOperationException(
                $"الفرع '{branchName}' غير موجود في النظام.");

        var store =
            await _context.Stores
                .FirstOrDefaultAsync(
                    x =>
                        x.StoreNameArabic == storeName &&
                        x.BranchCode == branch.BranchCode,
                    cancellationToken);

        if (store == null)
            throw new InvalidOperationException(
                $"المستودع '{storeName}' غير موجود تحت الفرع '{branchName}'.");

        // Generate session number
        var sessionNumber =
            await GenerateSessionNumberAsync(
                cancellationToken);

        // Send command to Handler
        var sessionId =
            await _mediator.Send(
                new ConfirmInventoryImportCommand(
                    rows,
                    branch.Id,
                    store.Id,
                    DateTime.UtcNow,
                    sessionNumber,
                    request.InventoryType),
                cancellationToken);

        return new InventoryImportConfirmResponse
        {
            IsSuccess = true,

            InventorySessionId = sessionId,

            SessionNumber = sessionNumber,

            TotalItems = rows.Count,

            Message =
                $"تم استيراد الجرد بنجاح. رقم الجلسة: {sessionNumber}"
        };
    }

    private static void ValidateFile(
        InventoryImportConfirmRequest request)
    {
        if (request.FileStream == null)
            throw new ArgumentNullException(
                nameof(request.FileStream));

        if (!request.FileStream.CanRead)
            throw new InvalidOperationException(
                "ملف Excel غير قابل للقراءة.");

        if (!request.FileStream.CanSeek)
            throw new InvalidOperationException(
                "Stream الخاص بملف Excel يجب أن يكون قابلًا للبحث.");

        if (request.FileStream.Length == 0)
            throw new InvalidOperationException(
                "ملف Excel فارغ.");
    }

    private async Task<string> GenerateSessionNumberAsync(
        CancellationToken cancellationToken)
    {
        var prefix =
            $"INV-{DateTime.UtcNow:yyyyMMdd}";

        var count =
            await _context.InventorySessions
                .CountAsync(
                    x => x.SessionNumber.StartsWith(prefix),
                    cancellationToken);

        return $"{prefix}-{count + 1:D4}";
    }
}