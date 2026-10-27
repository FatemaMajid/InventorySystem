using InventorySystem.Application.Common.Excel;
using InventorySystem.Domain.Enums;
using MediatR;

namespace InventorySystem.Application.Features.InventorySessions.Commands.ConfirmInventoryImport;

public record ConfirmInventoryImportCommand(
    IReadOnlyList<InventoryExcelRow> BeforeRows,
    IReadOnlyList<InventoryExcelRow> AfterRows,
    int BranchId,
    int StoreId,
    DateTime InventoryDate,
    string SessionNumber,
    InventoryType InventoryType,
    string BeforeFileName,
    string AfterFileName
) : IRequest<int>;