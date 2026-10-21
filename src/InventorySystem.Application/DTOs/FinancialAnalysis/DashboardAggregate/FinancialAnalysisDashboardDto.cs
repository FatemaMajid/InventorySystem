using System.Text.Json;
using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.DTOs.FinancialAnalysis.DashboardAggregate;

public sealed class FinancialDashboardBranchDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;
}

public sealed class FinancialAnalysisDashboardDto
{
    public int FinancialPeriodId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public FinancialPeriodStatus Status { get; set; }

    public int ActiveBranches { get; set; }

    public int ExpectedFiles { get; set; }

    public int ValidFiles { get; set; }

    public int MissingFiles { get; set; }

    public decimal CompletionPercentage { get; set; }

    public bool IsAllBranches { get; set; }

    public FinancialDashboardBranchDto? SelectedBranch { get; set; }

    public List<string> SelectedSections { get; set; } =
        new();

    public Dictionary<string,JsonElement?> Sections { get; set; } =
        new(StringComparer.OrdinalIgnoreCase);
}
