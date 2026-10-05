using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.API.Controllers.FinancialAnalysis;

public class FinancialUploadRequest
{
    public int FinancialPeriodId { get; set; }
    public FinancialFileType FileType { get; set; }
    public FinancialFileScope Scope { get; set; }
    public int? BranchId { get; set; }
    public FinancialGender? Gender { get; set; }
    public IFormFile File { get; set; } = default!;
}
