using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.DTOs.FinancialAnalysis.FinancialDashboard;

public class FinancialDashboardDto
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

    public int CompleteBranches { get; set; }
    public int IncompleteBranches { get; set; }

    public int IncomeStatementUploads { get; set; }
    public int FinancialPositionUploads { get; set; }
    public int TransferUploads { get; set; }
    public int SettlementUploads { get; set; }

    public decimal TransferMaleTotal { get; set; }
    public decimal TransferFemaleTotal { get; set; }

    public decimal SettlementDebitTotal { get; set; }
    public decimal SettlementCreditTotal { get; set; }
    public decimal SettlementBalanceTotal { get; set; }
}
