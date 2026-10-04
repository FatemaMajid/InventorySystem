using InventorySystem.Domain.Common;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Domain.FinancialAnalysis.Entities;

public class TransferRecord : BaseEntity
{
    public int FinancialUploadId { get; set; }
    public int BranchId { get; set; }
    public FinancialGender Gender { get; set; }

    public decimal Value { get; set; }
    public int RowOrder { get; set; }

    public FinancialUpload? FinancialUpload { get; set; }
    public Branch? Branch { get; set; }
}
