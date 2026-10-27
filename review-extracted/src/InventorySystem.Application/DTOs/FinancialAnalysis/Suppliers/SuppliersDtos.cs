using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.DTOs.FinancialAnalysis.Suppliers;

public sealed class BranchSuppliersDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public bool HasFinancialPosition { get; set; }

    public bool HasSupplierAccount { get; set; }

    public string? SupplierAccountCode { get; set; }

    public string? SupplierAccountName { get; set; }

    public decimal SuppliersTotal { get; set; }
}

public sealed class SupplierBranchRankingDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public decimal SuppliersTotal { get; set; }
}

public sealed class SuppliersAnalysisDto
{
    public int FinancialPeriodId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public FinancialPeriodStatus Status { get; set; }

    public int ActiveBranches { get; set; }

    public int BranchesWithData { get; set; }

    public int BranchesWithoutData { get; set; }

    public decimal AllBranchesSuppliers { get; set; }

    public decimal AverageSuppliers { get; set; }

    public SupplierBranchRankingDto? HighestSuppliersBranch { get; set; }

    public SupplierBranchRankingDto? LowestSuppliersBranch { get; set; }

    public List<BranchSuppliersDto> Branches { get; set; } =
        new();
}

public sealed class SupplierDetailDto
{
    public string AccountCode { get; set; } =
        string.Empty;

    public string AccountName { get; set; } =
        string.Empty;

    public decimal Liabilities { get; set; }

    public int RowOrder { get; set; }
}

public sealed class BranchSupplierDetailsDto
{
    public int FinancialPeriodId { get; set; }

    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public string SupplierAccountCode { get; set; } =
        string.Empty;

    public string SupplierAccountName { get; set; } =
        string.Empty;

    public decimal SuppliersTotal { get; set; }

    public int SuppliersCount { get; set; }

    public List<SupplierDetailDto> Suppliers { get; set; } =
        new();
}
