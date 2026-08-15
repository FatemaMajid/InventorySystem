using ClosedXML.Excel;
using InventorySystem.Domain.Entities;
using InventorySystem.Application.Common.Excel;
using InventorySystem.Application.Common.Excel.Validation;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Common.Localization;
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
        ValidateFile(
            request.BeforeFileStream,
            request.BeforeFileName);

        ValidateFile(
            request.AfterFileStream,
            request.AfterFileName);

        var beforeRows = ReadExcel(request.BeforeFileStream);
        var afterRows = ReadExcel(request.AfterFileStream);

        if (beforeRows.Count == 0 ||
            afterRows.Count == 0)
            throw new InvalidOperationException(
                LocalizationKeys.Excel.EmptyFile);

        var beforeHeaderRow =
            DetectHeader(request.BeforeFileStream);

        var afterHeaderRow =
            DetectHeader(request.AfterFileStream);

        // ValidateExcelRows(
        //     beforeRows,
        //     beforeHeaderRow);

        // ValidateExcelRows(
        //     afterRows,
        //     afterHeaderRow);
        var beforeValidation = ExcelValidator.Validate(
    beforeRows,
    beforeHeaderRow);

        if (!beforeValidation.IsValid)
        {
            var error = beforeValidation.Errors.First();

            throw new InvalidOperationException(
                $"Before Excel: Row {error.RowNumber}, ItemCode '{error.ItemCode}', {error.MessageKey}");
        }

        var afterValidation = ExcelValidator.Validate(
            afterRows,
            afterHeaderRow);

        if (!afterValidation.IsValid)
        {
            var error = afterValidation.Errors.First();

            throw new InvalidOperationException(
                $"After Excel: Row {error.RowNumber}, ItemCode '{error.ItemCode}', {error.MessageKey}");
        }

        var (beforeBranch, beforeStore) =
            await GetLocationAsync(
                beforeRows,
                cancellationToken);

        var (afterBranch, afterStore) =
            await GetLocationAsync(
                afterRows,
                cancellationToken);

        if (beforeBranch.Id != afterBranch.Id)
            throw new InvalidOperationException(
                LocalizationKeys.Branch.NotMatch);

        if (beforeStore.Id != afterStore.Id)
            throw new InvalidOperationException(
                LocalizationKeys.Store.NotMatch);

        var sessionNumber =
            await GenerateSessionNumberAsync(
                cancellationToken);

        var sessionId = await _mediator.Send(
            new ConfirmInventoryImportCommand(
                beforeRows,
                afterRows,
                beforeBranch.Id,
                beforeStore.Id,
                DateTime.UtcNow,
                sessionNumber,
                request.InventoryType,
                request.BeforeFileName ?? string.Empty,
                request.AfterFileName ?? string.Empty),
            cancellationToken);

        var totalItems = beforeRows
            .Select(x => x.ItemCode?.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Union(
                afterRows
                    .Select(x => x.ItemCode?.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x)),
                StringComparer.OrdinalIgnoreCase)
            .Count();

        return new InventoryImportConfirmResponse
        {
            IsSuccess = true,
            InventorySessionId = sessionId,
            SessionNumber = sessionNumber,
            TotalItems = totalItems,
            Message = LocalizationKeys.Inventory.ImportSuccess
        };
    }

    private static void ValidateFile(
        Stream? stream,
        string? fileName)
    {
        if (stream == null)
            throw new ArgumentNullException(
                nameof(stream));

        if (!stream.CanRead ||
            !stream.CanSeek ||
            stream.Length == 0)
            throw new InvalidOperationException(
                LocalizationKeys.Excel.InvalidFile);

        if (string.IsNullOrWhiteSpace(fileName))
            throw new InvalidOperationException(
                LocalizationKeys.Excel.InvalidFile);
    }

    private static List<InventoryExcelRow> ReadExcel(
        Stream stream)
    {
        stream.Position = 0;
        return ExcelReader.Read(stream);
    }

    private static int DetectHeader(
        Stream stream)
    {
        stream.Position = 0;

        using var workbook =
            new XLWorkbook(stream);

        var worksheet =
            workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
            throw new InvalidOperationException(
                LocalizationKeys.Excel.NoWorksheet);

        var (headerRow, _) =
            ExcelHeaderDetector.Detect(worksheet);

        return headerRow;
    }

    private static void ValidateExcelRows(
        IReadOnlyList<InventoryExcelRow> rows,
        int headerRow)
    {
        var validation =
            ExcelValidator.Validate(
                rows,
                headerRow);

        if (!validation.IsValid)
            throw new InvalidOperationException(
                LocalizationKeys.Excel.InvalidHeaders);

        var locationValidation =
            InventoryLocationValidator.Validate(
                rows,
                headerRow);

        if (!locationValidation.IsValid)
            throw new InvalidOperationException(
                LocalizationKeys.Excel.InvalidFile);
    }

    private async Task<(Branch Branch, Store Store)>
        GetLocationAsync(
            IReadOnlyList<InventoryExcelRow> rows,
            CancellationToken cancellationToken)
    {
        var branchName =
            rows.First().Branch?.Trim();

        var storeName =
            rows.First().Store?.Trim();

        if (string.IsNullOrWhiteSpace(branchName))
            throw new InvalidOperationException(
                LocalizationKeys.Branch.Required);

        if (string.IsNullOrWhiteSpace(storeName))
            throw new InvalidOperationException(
                LocalizationKeys.Store.Required);

        var branch =
            await _context.Branches
                .FirstOrDefaultAsync(
                    x =>
                        x.BranchNameArabic == branchName,
                    cancellationToken);

        if (branch == null)
            throw new InvalidOperationException(
                LocalizationKeys.Branch.NotFound);

        var store =
            await _context.Stores
                .FirstOrDefaultAsync(
                    x =>
                        x.StoreNameArabic == storeName &&
                        x.BranchCode == branch.BranchCode,
                    cancellationToken);

        if (store == null)
            throw new InvalidOperationException(
                LocalizationKeys.Store.NotFound);

        return (branch, store);
    }

    private async Task<string>
        GenerateSessionNumberAsync(
            CancellationToken cancellationToken)
    {
        var prefix =
            $"INV-{DateTime.UtcNow:yyyyMMdd}";

        var lastSession =
            await _context.InventorySessions
                .Where(x =>
                    x.SessionNumber.StartsWith(prefix))
                .OrderByDescending(x =>
                    x.SessionNumber)
                .Select(x =>
                    x.SessionNumber)
                .FirstOrDefaultAsync(
                    cancellationToken);

        var nextNumber = 1;

        if (!string.IsNullOrWhiteSpace(lastSession))
        {
            var parts =
                lastSession.Split('-');

            if (parts.Length == 3 &&
                int.TryParse(
                    parts[2],
                    out var number))
            {
                nextNumber = number + 1;
            }
        }

        return $"{prefix}-{nextNumber:D4}";
    }
}