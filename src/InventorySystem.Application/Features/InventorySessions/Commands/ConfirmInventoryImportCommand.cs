using InventorySystem.Application.Common.Excel;
using InventorySystem.Domain.Enums;
using MediatR;

namespace InventorySystem.Application.Features.InventorySessions.Commands.ConfirmInventoryImport;

public record ConfirmInventoryImportCommand(
    IReadOnlyList<InventoryExcelRow> Rows,
    int BranchId,
    int StoreId,
    DateTime InventoryDate,
    string SessionNumber,
    InventoryType InventoryType
) : IRequest<int>;