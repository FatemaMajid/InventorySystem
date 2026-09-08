using InventorySystem.Application.Common.Excel;
using InventorySystem.Application.Common.Excel.Normalization;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Common.Localization;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

using DomainUnit = InventorySystem.Domain.Entities.Unit;

namespace InventorySystem.Application.Features.InventorySessions.Commands.ConfirmInventoryImport;

public class ConfirmInventoryImportCommandHandler
    : IRequestHandler<ConfirmInventoryImportCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditLogService _auditLogService;

    public ConfirmInventoryImportCommandHandler(
        IApplicationDbContext context,
        IAuditLogService auditLogService)
    {
        _context = context;
        _auditLogService = auditLogService;
    }

    public async Task<int> Handle(
        ConfirmInventoryImportCommand request,
        CancellationToken cancellationToken)
    {
        await ValidateRequest(request, cancellationToken);

        await using var transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var session = new InventorySession
            {
                SessionNumber = request.SessionNumber.Trim(),
                InventoryType = request.InventoryType,
                InventoryDate = request.InventoryDate,
                BranchId = request.BranchId,
                StoreId = request.StoreId,
                BeforeFileName = request.BeforeFileName,
                AfterFileName = request.AfterFileName,
                Status = "Completed"
            };

            _context.InventorySessions.Add(session);

            var beforeRows = request.BeforeRows
                .Where(x => !string.IsNullOrWhiteSpace(x.ItemCode))
                .ToDictionary(
                    x => x.ItemCode.Trim(),
                    StringComparer.OrdinalIgnoreCase);

            var afterRows = request.AfterRows
                .Where(x => !string.IsNullOrWhiteSpace(x.ItemCode))
                .ToDictionary(
                    x => x.ItemCode.Trim(),
                    StringComparer.OrdinalIgnoreCase);

            var codes = beforeRows.Keys
                .Union(afterRows.Keys, StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var code in codes)
            {
                beforeRows.TryGetValue(code, out var beforeRow);
                afterRows.TryGetValue(code, out var afterRow);

                await AddComparisonRow(
                    session,
                    request,
                    beforeRow,
                    afterRow,
                    cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync(
                action: "Confirm",
                entity: "InventorySession",
                entityId: session.Id.ToString(),
                details: $"SessionNumber: {session.SessionNumber}, BranchId: {session.BranchId}, StoreId: {session.StoreId}",
                cancellationToken: cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return session.Id;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task ValidateRequest(
        ConfirmInventoryImportCommand request,
        CancellationToken ct)
    {
        if (request.BeforeRows.Count == 0)
            throw new InvalidOperationException(
                LocalizationKeys.Inventory.NoData);

        if (request.AfterRows.Count == 0)
            throw new InvalidOperationException(
                LocalizationKeys.Inventory.NoData);

        if (request.BranchId <= 0)
            throw new InvalidOperationException(
                LocalizationKeys.Branch.Required);

        if (request.StoreId <= 0)
            throw new InvalidOperationException(
                LocalizationKeys.Store.Required);

        if (string.IsNullOrWhiteSpace(request.SessionNumber))
            throw new InvalidOperationException(
                LocalizationKeys.Inventory.SessionNumberRequired);

        if (request.InventoryType != InventoryType.SemiAnnual &&
            request.InventoryType != InventoryType.Annual)
            throw new InvalidOperationException(
                LocalizationKeys.Inventory.InvalidType);

        var branch = await _context.Branches
            .FirstOrDefaultAsync(
                x => x.Id == request.BranchId,
                ct);

        if (branch == null)
            throw new InvalidOperationException(
                LocalizationKeys.Branch.NotFound);

        var store = await _context.Stores
            .FirstOrDefaultAsync(
                x => x.Id == request.StoreId,
                ct);

        if (store == null)
            throw new InvalidOperationException(
                LocalizationKeys.Store.NotFound);

        if (!string.Equals(
                store.BranchCode?.Trim(),
                branch.BranchCode?.Trim(),
                StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                LocalizationKeys.Store.NotBelongToBranch);

        var sessionNumber = request.SessionNumber.Trim();

        if (await _context.InventorySessions.AnyAsync(
                x => x.SessionNumber == sessionNumber,
                ct))
            throw new InvalidOperationException(
                LocalizationKeys.Inventory.SessionExists);

        ValidateDuplicateItems(request.BeforeRows);
        ValidateDuplicateItems(request.AfterRows);

        ValidateRows(request.BeforeRows);
        ValidateRows(request.AfterRows);
    }

    private static void ValidateDuplicateItems(
        IReadOnlyList<InventoryExcelRow> rows)
    {
        var duplicates = rows
            .Where(x => !string.IsNullOrWhiteSpace(x.ItemCode))
            .GroupBy(
                x => x.ItemCode.Trim(),
                StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count > 0)
            throw new InvalidOperationException(
                LocalizationKeys.DuplicateItemCode);
    }

    private static void ValidateRows(
        IReadOnlyList<InventoryExcelRow> rows)
    {
        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.ItemCode))
                throw new InvalidOperationException(
                    LocalizationKeys.ItemCodeRequired);

            if (string.IsNullOrWhiteSpace(row.ItemName1))
                throw new InvalidOperationException(
                    LocalizationKeys.ItemNameRequired);

            // if (string.IsNullOrWhiteSpace(row.Category))
            //     throw new InvalidOperationException(
            //         LocalizationKeys.Category.Required);
        }
    }

    private async Task AddComparisonRow(
        InventorySession session,
        ConfirmInventoryImportCommand request,
        InventoryExcelRow? beforeRow,
        InventoryExcelRow? afterRow,
        CancellationToken ct)
    {
        var sourceRow = afterRow ?? beforeRow!;

        var code = sourceRow.ItemCode.Trim();
        var name1 = sourceRow.ItemName1?.Trim() ?? string.Empty;
        var name2 = sourceRow.ItemName2?.Trim() ?? string.Empty;
        var categoryName = sourceRow.Category?.Trim() ?? string.Empty;
        var unitName = sourceRow.Unit?.Trim();

        var category = await GetOrCreateCategory(
            categoryName,
            ct);

        var unit = await GetUnit(
            unitName,
            ct);

        var item = await GetOrCreateItem(
            code,
            name1,
            name2,
            category,
            unit,
            ct);

        await EnsureLocation(
            item,
            request,
            ct);

        var quantityBefore = beforeRow?.Quantity;
        var quantityAfter = afterRow?.Quantity;

        var priceBefore = beforeRow?.Price;
        var priceAfter = afterRow?.Price;

        if (afterRow != null)
        {
            await UpdatePrice(
                item,
                priceAfter ?? 0m,
                ct);
        }
        else if (beforeRow != null)
        {
            await UpdatePrice(
                item,
                priceBefore ?? 0m,
                ct);
        }

        string? description = null;

        if (beforeRow != null && afterRow == null)
        {
            description =
                LocalizationKeys.Inventory.ItemExistsBeforeOnly;
        }
        else if (beforeRow == null && afterRow != null)
        {
            description =
                LocalizationKeys.Inventory.ItemExistsAfterOnly;
        }
        else if (unit == null)
        {
            description =
                LocalizationKeys.Unit.FirstUnitNotDefined;
        }

        session.Details.Add(
            CreateDetail(
                session,
                item,
                quantityBefore,
                quantityAfter,
                priceBefore,
                priceAfter,
                description));
    }

    private async Task<Item> GetOrCreateItem(
        string code,
        string name1,
        string name2,
        Category? category,
        DomainUnit? unit,
        CancellationToken ct)
    {
        var item = await _context.Items
            .FirstOrDefaultAsync(
                x => x.ItemCode == code,
                ct);

        if (item == null)
        {
            item = new Item
            {
                ItemCode = code,
                ItemName1 = name1,
                ItemName2 = name2,
                Category = category,
                Unit = unit,
                IsActive = true
            };

            _context.Items.Add(item);
        }
        else
        {
            item.ItemName1 = name1;
            item.ItemName2 = name2;
            item.Category = category;

            if (unit != null)
                item.Unit = unit;

            item.IsActive = true;
        }

        return item;
    }

    private async Task EnsureLocation(
        Item item,
        ConfirmInventoryImportCommand request,
        CancellationToken ct)
    {
        var exists = await _context.ItemLocations.AnyAsync(
            x =>
                x.ItemId == item.Id &&
                x.BranchId == request.BranchId &&
                x.StoreId == request.StoreId,
            ct);

        if (!exists)
        {
            _context.ItemLocations.Add(
                new ItemLocation
                {
                    Item = item,
                    BranchId = request.BranchId,
                    StoreId = request.StoreId
                });
        }
    }

    private async Task UpdatePrice(
        Item item,
        decimal price,
        CancellationToken ct)
    {
        var itemPrice = await _context.ItemPrices
            .FirstOrDefaultAsync(
                x => x.ItemId == item.Id,
                ct);

        if (itemPrice == null)
        {
            _context.ItemPrices.Add(
                new ItemPrice
                {
                    Item = item,
                    CustomerPrice = 0m,
                    ConsumerPrice = price
                });

            return;
        }

        if (itemPrice.ConsumerPrice == price)
            return;

        _context.ItemPriceHistories.Add(
            new ItemPriceHistory
            {
                ItemPrice = itemPrice,
                OldCustomerPrice = itemPrice.CustomerPrice,
                NewCustomerPrice = itemPrice.CustomerPrice,
                OldConsumerPrice = itemPrice.ConsumerPrice,
                NewConsumerPrice = price,
                ChangedAt = DateTime.Now,
                ChangedBy = "Excel Import",
                Source = "Excel Import"
            });

        itemPrice.ConsumerPrice = price;
    }

    private static InventoryDetail CreateDetail(
        InventorySession session,
        Item item,
        decimal? quantityBefore,
        decimal? quantityAfter,
        decimal? priceBefore,
        decimal? priceAfter,
        string? description)
    {
        decimal? beforeValue = null;

        if (quantityBefore.HasValue &&
            priceBefore.HasValue)
            beforeValue =
                quantityBefore.Value * priceBefore.Value;

        decimal? afterValue = null;

        if (quantityAfter.HasValue &&
            priceAfter.HasValue)
            afterValue =
                quantityAfter.Value * priceAfter.Value;

        decimal? quantityDifference = null;

        if (quantityBefore.HasValue &&
            quantityAfter.HasValue)
            quantityDifference =
                quantityAfter.Value - quantityBefore.Value;

        decimal? valueDifference = null;

        if (beforeValue.HasValue &&
            afterValue.HasValue)
            valueDifference =
                afterValue.Value - beforeValue.Value;

        var status =
            beforeValue == null && quantityBefore == null
                ? "AfterOnly"
                : afterValue == null && quantityAfter == null
                    ? "BeforeOnly"
                    : quantityDifference == 0
                        ? "NoDifference"
                        : quantityDifference > 0
                            ? "Increase"
                            : "Decrease";

        return new InventoryDetail
        {
            InventorySession = session,
            Item = item,
            QuantityBefore = quantityBefore,
            QuantityAfter = quantityAfter,
            QuantityDifference = quantityDifference,
            ConsumerPriceBefore = priceBefore,
            ConsumerPriceAfter = priceAfter,
            BeforeValue = beforeValue,
            AfterValue = afterValue,
            ValueDifference = valueDifference,
            Status = status,
            Description = description
        };
    }

    private async Task<Category?> GetOrCreateCategory(
        string name,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var category = await _context.Categories
            .FirstOrDefaultAsync(
                x => x.CategoryNameArabic == name,
                ct);

        if (category != null)
            return category;

        category = new Category
        {
            CategoryCode =
                $"CAT-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
            CategoryNameArabic = name,
            IsActive = true
        };

        _context.Categories.Add(category);

        return category;
    }

    private async Task<DomainUnit?> GetUnit(
        string? name,
        CancellationToken ct)
    {
        var normalized = UnitNormalizer.Normalize(name);

        if (normalized == null)
            return null;

        var supportedUnits = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
        {
            "قطعة",
            "درزن",
            "سيت",
            "غم",
            "سم",
            "كغم",
            "علبة"
        };

        if (!supportedUnits.Contains(normalized))
            throw new InvalidOperationException(
                $"الوحدة غير مدعومة: '{name}'");

        var units = await _context.Units
            .ToListAsync(ct);

        var unit = units.FirstOrDefault(
            x => string.Equals(
                UnitNormalizer.Normalize(x.UnitNameArabic),
                normalized,
                StringComparison.OrdinalIgnoreCase));

        if (unit == null)
            throw new InvalidOperationException(
                LocalizationKeys.Unit.NotFound);

        return unit;
    }
}