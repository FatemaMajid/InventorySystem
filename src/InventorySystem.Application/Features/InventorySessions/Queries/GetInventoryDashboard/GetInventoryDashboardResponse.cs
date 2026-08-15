namespace InventorySystem.Application.Features.InventorySessions.Queries.GetInventoryDashboard;

public class GetInventoryDashboardResponse
{
    public SessionInfo Session { get; init; } = new();

    public DashboardSummary Summary { get; init; } = new();

    public FinancialSummary Financial { get; init; } = new();

    public IReadOnlyList<StatusSummary> Statuses { get; init; } =
        Array.Empty<StatusSummary>();

    public IReadOnlyList<ValueDifferenceItem> TopValueDifferences { get; init; } =
        Array.Empty<ValueDifferenceItem>();

    public AttentionSummary Attention { get; init; } = new();
}

public class SessionInfo
{
    public int SessionId { get; init; }
    public string SessionNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string InventoryType { get; init; } = string.Empty;
    public DateTime InventoryDate { get; init; }

    public int BranchId { get; init; }
    public string BranchName { get; init; } = string.Empty;

    public int StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;

    public string? BeforeFileName { get; init; }
    public string? AfterFileName { get; init; }
}

public class DashboardSummary
{
    public int TotalItems { get; init; }
    public int Increase { get; init; }
    public int Decrease { get; init; }
    public int Match { get; init; }
    public int NewlyCounted { get; init; }
    public int FullyDepleted { get; init; }
    public int PriceChanged { get; init; }
    public int UnitNotDefined { get; init; }
}

public class FinancialSummary
{
    public decimal TotalValueBefore { get; init; }
    public decimal TotalValueAfter { get; init; }
    public decimal TotalDifference { get; init; }
    public decimal DifferencePercentage { get; init; }
}

public class StatusSummary
{
    public string Status { get; init; } = string.Empty;
    public int Count { get; init; }
    public decimal Percentage { get; init; }
}

public class ValueDifferenceItem
{
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public decimal ValueDifference { get; init; }
}

public class AttentionSummary
{
    public int Total { get; init; }
    public int NewlyCounted { get; init; }
    public int FullyDepleted { get; init; }
    public int UnitNotDefined { get; init; }
    public int PriceChanged { get; init; }
}