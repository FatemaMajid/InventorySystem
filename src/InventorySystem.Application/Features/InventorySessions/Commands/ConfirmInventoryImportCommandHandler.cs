using InventorySystem.Application.Common.Excel;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Enums;
using DomainUnit = InventorySystem.Domain.Entities.Unit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.InventorySessions.Commands.ConfirmInventoryImport;

public class ConfirmInventoryImportCommandHandler
    : IRequestHandler<ConfirmInventoryImportCommand, int>
{
    private readonly IApplicationDbContext _context;

    public ConfirmInventoryImportCommandHandler(IApplicationDbContext context)
        => _context = context;

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
                Status = "Completed"
            };

            _context.InventorySessions.Add(session);

            foreach (var row in request.Rows)
            {
                await AddRow(
                    session,
                    request,
                    row.ItemCode.Trim(),
                    row.ItemName1?.Trim() ?? string.Empty,
                    row.ItemName2?.Trim() ?? string.Empty,
                    row.Category?.Trim() ?? string.Empty,
                    row.Unit?.Trim() ?? string.Empty,
                    row.Quantity ?? 0m,
                    row.Price ?? 0m,
                    cancellationToken
                );
            }

            await _context.SaveChangesAsync(cancellationToken);
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
        if (request.Rows == null || request.Rows.Count == 0)
            throw new InvalidOperationException("لا توجد بيانات للاستيراد.");

        if (request.BranchId <= 0)
            throw new InvalidOperationException("الفرع غير صالح.");

        if (request.StoreId <= 0)
            throw new InvalidOperationException("المخزن غير صالح.");

        if (string.IsNullOrWhiteSpace(request.SessionNumber))
            throw new InvalidOperationException("رقم جلسة الجرد مطلوب.");

        if (request.InventoryType != InventoryType.SemiAnnual &&
            request.InventoryType != InventoryType.Annual)
            throw new InvalidOperationException("نوع الجرد غير صالح.");

        var branch = await _context.Branches
            .FirstOrDefaultAsync(x => x.Id == request.BranchId, ct);

        if (branch == null)
            throw new InvalidOperationException("الفرع المحدد غير موجود.");

        var store = await _context.Stores
            .FirstOrDefaultAsync(x => x.Id == request.StoreId, ct);

        if (store == null)
            throw new InvalidOperationException("المخزن المحدد غير موجود.");

        if (!string.Equals(
                store.BranchCode?.Trim(),
                branch.BranchCode?.Trim(),
                StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "المخزن المحدد لا يتبع الفرع المحدد.");

        var sessionNumber = request.SessionNumber.Trim();

        if (await _context.InventorySessions.AnyAsync(
                x => x.SessionNumber == sessionNumber, ct))
            throw new InvalidOperationException(
                "رقم جلسة الجرد مستخدم مسبقًا.");

        var duplicateCodes = request.Rows
            .Where(x => !string.IsNullOrWhiteSpace(x.ItemCode))
            .GroupBy(x => x.ItemCode.Trim(), StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateCodes.Count > 0)
            throw new InvalidOperationException(
                "يوجد تكرار في رموز الأصناف داخل ملف Excel: " +
                string.Join(", ", duplicateCodes));

        foreach (var row in request.Rows)
        {
            if (string.IsNullOrWhiteSpace(row.ItemCode))
                throw new InvalidOperationException("يوجد صف بدون كود صنف.");

            if (string.IsNullOrWhiteSpace(row.ItemName1))
                throw new InvalidOperationException(
                    $"الصنف {row.ItemCode} لا يحتوي على الاسم الأول.");

            if (string.IsNullOrWhiteSpace(row.Category))
                throw new InvalidOperationException(
                    $"الصنف {row.ItemCode} لا يحتوي على تصنيف.");

            if (string.IsNullOrWhiteSpace(row.Unit))
                throw new InvalidOperationException(
                    $"الصنف {row.ItemCode} لا يحتوي على وحدة.");
        }
    }

    private async Task AddRow(
        InventorySession session,
        ConfirmInventoryImportCommand request,
        string code,
        string name1,
        string name2,
        string categoryName,
        string unitName,
        decimal quantity,
        decimal price,
        CancellationToken ct)
    {
        var category = await GetOrCreateCategory(categoryName, ct);
        var unit = await GetUnit(unitName, ct);

        var item = await GetOrCreateItem(
            code, name1, name2, category.Id, unit.Id, ct);

        await EnsureLocation(item, request, ct);
        await UpdatePrice(item, price, ct);

        var previous = await GetPreviousDetail(item.Id, request, ct);
        session.Details.Add(
            CreateDetail(session, item, quantity, price, previous));
    }

    private async Task<Item> GetOrCreateItem(
        string code,
        string name1,
        string name2,
        int categoryId,
        int unitId,
        CancellationToken ct)
    {
        var item = await _context.Items
            .FirstOrDefaultAsync(x => x.ItemCode == code, ct);

        if (item == null)
        {
            item = new Item
            {
                ItemCode = code,
                ItemName1 = name1,
                ItemName2 = name2,
                CategoryId = categoryId,
                UnitId = unitId,
                IsActive = true
            };

            _context.Items.Add(item);
        }
        else
        {
            item.ItemName1 = name1;
            item.ItemName2 = name2;
            item.CategoryId = categoryId;
            item.UnitId = unitId;
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
            x => x.ItemId == item.Id &&
                 x.BranchId == request.BranchId &&
                 x.StoreId == request.StoreId,
            ct);

        if (!exists)
        {
            _context.ItemLocations.Add(new ItemLocation
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
            .FirstOrDefaultAsync(x => x.ItemId == item.Id, ct);

        if (itemPrice == null)
        {
            _context.ItemPrices.Add(new ItemPrice
            {
                Item = item,
                CustomerPrice = 0m,
                ConsumerPrice = price
            });

            return;
        }

        if (itemPrice.ConsumerPrice == price)
            return;

        _context.ItemPriceHistories.Add(new ItemPriceHistory
        {
            ItemPrice = itemPrice,
            OldCustomerPrice = itemPrice.CustomerPrice,
            NewCustomerPrice = itemPrice.CustomerPrice,
            OldConsumerPrice = itemPrice.ConsumerPrice,
            NewConsumerPrice = price,
            ChangedAt = DateTime.UtcNow,
            ChangedBy = "Excel Import",
            Source = "Excel Import"
        });

        itemPrice.ConsumerPrice = price;
    }

    private async Task<InventoryDetail?> GetPreviousDetail(
        int itemId,
        ConfirmInventoryImportCommand request,
        CancellationToken ct)
    {
        return await _context.InventoryDetails
            .Include(x => x.InventorySession)
            .Where(x =>
                x.ItemId == itemId &&
                x.InventorySession != null &&
                x.InventorySession.BranchId == request.BranchId &&
                x.InventorySession.StoreId == request.StoreId &&
                x.InventorySession.InventoryDate < request.InventoryDate)
            .OrderByDescending(x => x.InventorySession!.InventoryDate)
            .ThenByDescending(x => x.Id)
            .FirstOrDefaultAsync(ct);
    }

    private static InventoryDetail CreateDetail(
        InventorySession session,
        Item item,
        decimal? quantityAfter,
        decimal priceAfter,
        InventoryDetail? previous)
    {
        decimal? quantityBefore = previous?.QuantityAfter;
        decimal? priceBefore = previous?.ConsumerPriceAfter;

        decimal? beforeValue =
            quantityBefore.HasValue && priceBefore.HasValue
                ? quantityBefore.Value * priceBefore.Value
                : null;

        decimal? afterValue =
            quantityAfter.HasValue
                ? quantityAfter.Value * priceAfter
                : null;

        decimal? quantityDifference =
            quantityBefore.HasValue && quantityAfter.HasValue
                ? quantityAfter.Value - quantityBefore.Value
                : null;

        decimal? valueDifference =
            beforeValue.HasValue
                ? afterValue - beforeValue.Value
                : null;

        var status = !quantityBefore.HasValue
            ? "FirstInventory"
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
            Description = null
        };
    }

    private async Task<Category> GetOrCreateCategory(
        string name,
        CancellationToken ct)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(
                x => x.CategoryNameArabic == name, ct);

        if (category != null)
            return category;

        category = new Category
        {
            CategoryCode = "CAT-" +
                Guid.NewGuid().ToString("N")[..8].ToUpperInvariant(),
            CategoryNameArabic = name,
            CategoryNameEnglish = null,
            IsActive = true
        };

        _context.Categories.Add(category);
        return category;
    }

    private async Task<DomainUnit> GetUnit(
    string name,
    CancellationToken ct)
    {
        var normalized = name.Trim();

        if (normalized != "قطعة" && normalized != "درزن")
        {
            throw new InvalidOperationException(
                $"الوحدة غير مدعومة: {name}. " +
                "الوحدات المسموحة هي: قطعة أو درزن.");
        }

        var unit = await _context.Units
            .FirstOrDefaultAsync(
                x => x.UnitNameArabic == normalized,
                ct);

        if (unit == null)
        {
            throw new InvalidOperationException(
                $"الوحدة '{normalized}' غير موجودة في النظام.");
        }

        return unit;
    }
}
