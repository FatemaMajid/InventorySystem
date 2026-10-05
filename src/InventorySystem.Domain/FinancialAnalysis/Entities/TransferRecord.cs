using InventorySystem.Domain.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Domain.FinancialAnalysis.Entities;

public class TransferRecord
{
    public int Id { get; set; }

    public int FinancialUploadId { get; set; }

    public int BranchId { get; set; }

    public string AccountCode { get; set; } = string.Empty;

    public string AccountName { get; set; } = string.Empty;

    public FinancialGender Gender { get; set; }

    public decimal Value { get; set; }

    public int RowOrder { get; set; }

    public FinancialUpload? FinancialUpload { get; set; }

    public Branch? Branch { get; set; }
}