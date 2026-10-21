using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.DTOs.FinancialAnalysis.Expenses;

public sealed class BranchExpenseDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public bool HasIncomeStatement { get; set; }

    public bool HasExpenseAccount { get; set; }

    public string? ExpenseAccountCode { get; set; }

    public string? ExpenseAccountName { get; set; }

    public decimal TotalExpenses { get; set; }
}

public sealed class ExpenseBranchRankingDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public decimal TotalExpenses { get; set; }
}

public sealed class ExpensesAnalysisDto
{
    public int FinancialPeriodId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public FinancialPeriodStatus Status { get; set; }

    public int ActiveBranches { get; set; }

    public int BranchesWithData { get; set; }

    public int BranchesWithoutData { get; set; }

    public decimal AllBranchesExpenses { get; set; }

    public decimal AverageExpenses { get; set; }

    public ExpenseBranchRankingDto? HighestBranch { get; set; }

    public ExpenseBranchRankingDto? LowestBranch { get; set; }

    public List<BranchExpenseDto> Branches { get; set; } =
        new();
}

public sealed class ExpenseDetailRowDto
{
    public string AccountCode { get; set; } =
        string.Empty;

    public string AccountName { get; set; } =
        string.Empty;

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }

    public decimal Balance { get; set; }

    public int RowOrder { get; set; }
}

public sealed class BranchExpenseDetailsDto
{
    public int FinancialPeriodId { get; set; }

    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public string ParentAccountCode { get; set; } =
        string.Empty;

    public string ParentAccountName { get; set; } =
        string.Empty;

    public decimal TotalExpenses { get; set; }

    public int DetailsCount { get; set; }

    public List<ExpenseDetailRowDto> Details { get; set; } =
        new();
}
