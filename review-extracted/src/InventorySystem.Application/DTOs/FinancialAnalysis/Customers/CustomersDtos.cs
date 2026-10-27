using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.DTOs.FinancialAnalysis.Customers;

public sealed class BranchCustomersDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public bool HasFinancialPosition { get; set; }

    public bool HasCustomerAccount { get; set; }

    public bool HasEmployeeAccount { get; set; }

    public string? CustomerAccountCode { get; set; }

    public string? CustomerAccountName { get; set; }

    public decimal CustomersTotal { get; set; }

    public string? EmployeeAccountCode { get; set; }

    public string? EmployeeAccountName { get; set; }

    public decimal EmployeesTotal { get; set; }
}

public sealed class CustomerBranchRankingDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public decimal Value { get; set; }
}

public sealed class CustomersAnalysisDto
{
    public int FinancialPeriodId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public FinancialPeriodStatus Status { get; set; }

    public int ActiveBranches { get; set; }

    public int BranchesWithCustomerData { get; set; }

    public int BranchesWithoutCustomerData { get; set; }

    public int BranchesWithEmployeeData { get; set; }

    public decimal AllBranchesCustomersTotal { get; set; }

    public decimal AllBranchesEmployeesTotal { get; set; }

    public decimal AverageCustomers { get; set; }

    public decimal AverageEmployees { get; set; }

    public CustomerBranchRankingDto? HighestCustomersBranch { get; set; }

    public CustomerBranchRankingDto? LowestCustomersBranch { get; set; }

    public CustomerBranchRankingDto? HighestEmployeesBranch { get; set; }

    public CustomerBranchRankingDto? LowestEmployeesBranch { get; set; }

    public List<BranchCustomersDto> Branches { get; set; } =
        new();
}

public sealed class CustomerDetailRowDto
{
    public string AccountCode { get; set; } =
        string.Empty;

    public string AccountName { get; set; } =
        string.Empty;

    public decimal Assets { get; set; }

    public decimal Liabilities { get; set; }

    public decimal Value { get; set; }

    public int RowOrder { get; set; }
}

public sealed class CustomerEmployeeDetailsDto
{
    public int FinancialPeriodId { get; set; }

    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public string CustomerAccountCode { get; set; } =
        string.Empty;

    public string CustomerAccountName { get; set; } =
        string.Empty;

    public decimal CustomersTotal { get; set; }

    public string? EmployeeAccountCode { get; set; }

    public string? EmployeeAccountName { get; set; }

    public decimal EmployeesTotal { get; set; }

    public int CustomerDetailsCount { get; set; }

    public int EmployeeDetailsCount { get; set; }

    public List<CustomerDetailRowDto> CustomerDetails { get; set; } =
        new();

    public List<CustomerDetailRowDto> EmployeeDetails { get; set; } =
        new();
}
