using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.DTOs.FinancialAnalysis.Inventory;

public sealed class BranchInventoryDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public bool HasIncomeStatement { get; set; }

    public bool HasInventoryAccount { get; set; }

    public decimal OpeningInventory { get; set; }

    public decimal ClosingInventory { get; set; }

    public decimal InventoryChange =>
        ClosingInventory - OpeningInventory;
}

public sealed class InventoryBranchRankingDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public decimal Value { get; set; }
}

public sealed class InventoryAnalysisDto
{
    public int FinancialPeriodId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public FinancialPeriodStatus Status { get; set; }

    public int ActiveBranches { get; set; }

    public int BranchesWithData { get; set; }

    public int BranchesWithoutData { get; set; }

    public decimal AllBranchesOpeningTotal { get; set; }

    public decimal AllBranchesClosingTotal { get; set; }

    public decimal InventoryChange { get; set; }

    public decimal AverageOpeningInventory { get; set; }

    public decimal AverageClosingInventory { get; set; }

    public InventoryBranchRankingDto? HighestOpeningBranch { get; set; }

    public InventoryBranchRankingDto? LowestOpeningBranch { get; set; }

    public InventoryBranchRankingDto? HighestClosingBranch { get; set; }

    public InventoryBranchRankingDto? LowestClosingBranch { get; set; }

    public List<BranchInventoryDto> Branches { get; set; } =
        new();
}
