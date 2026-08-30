namespace InventorySystem.Application.Features.Home.Queries.GetHomeDashboard;

public sealed class GetHomeDashboardResponse
{
    public HomeUser User { get; init; } = new();

    public HomeStatistics Statistics { get; init; } = new();

    public IReadOnlyList<HomeRecentSession> RecentSessions { get; init; }
        = Array.Empty<HomeRecentSession>();

    public HomeOverview Overview { get; init; } = new();

    public HomeSystemStatus SystemStatus { get; init; } = new();
}


public sealed class HomeUser
{
    public string Name { get; init; } = string.Empty;
}


public sealed class HomeStatistics
{
    public int ActiveSessions { get; init; }

    public int TotalSessions { get; init; }

    public int AttentionItems { get; init; }
}


public sealed class HomeRecentSession
{
    public int Id { get; init; }

    public string SessionNumber { get; init; } = string.Empty;

    public string InventoryType { get; init; } = string.Empty;

    public DateTime InventoryDate { get; init; }

    public int BranchId { get; init; }

    public string BranchName { get; init; } = string.Empty;

    public int StoreId { get; init; }

    public string StoreName { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public int TotalItems { get; init; }

    public decimal TotalValue { get; init; }
}


public sealed class HomeOverview
{
    public int Branches { get; init; }

    public int Stores { get; init; }

    public int Categories { get; init; }

    public int Items { get; init; }
}


public sealed class HomeSystemStatus
{
    public string Api { get; init; } = "Connected";

    public string Database { get; init; } = "Connected";

    public string Health { get; init; } = "Healthy";
}