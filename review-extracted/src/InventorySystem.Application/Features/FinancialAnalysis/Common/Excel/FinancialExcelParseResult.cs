using InventorySystem.Domain.FinancialAnalysis.Entities;

namespace InventorySystem.Application.Features.FinancialAnalysis.Common.Excel;

public class FinancialExcelParseResult
{
    public List<FinancialAccountRow> AccountRows { get; set; } = [];
    public List<TransferRecord> TransferRecords { get; set; } = [];
    public List<SettlementRow> SettlementRows { get; set; } = [];
    public SettlementSummary? SettlementSummary { get; set; }

    public int ParsedRowCount =>
        AccountRows.Count +
        TransferRecords.Count +
        SettlementRows.Count;
}
