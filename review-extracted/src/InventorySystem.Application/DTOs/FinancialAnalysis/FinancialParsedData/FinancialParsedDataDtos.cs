using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.DTOs.FinancialAnalysis.FinancialParsedData;

public class FinancialAccountRowDto
{
    public int Id { get; set; }
    public int FinancialUploadId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal? Debit { get; set; }
    public decimal? Credit { get; set; }
    public decimal? Balance { get; set; }
    public decimal? Assets { get; set; }
    public decimal? Liabilities { get; set; }
    public int RowOrder { get; set; }
}

public class TransferRecordDto
{
    public int Id { get; set; }
    public int FinancialUploadId { get; set; }
    public int BranchId { get; set; }
    public string BranchCode { get; set; } = string.Empty;
    public string BranchNameArabic { get; set; } = string.Empty;
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public FinancialGender Gender { get; set; }
    public decimal Value { get; set; }
    public int RowOrder { get; set; }
}

public class SettlementRowDto
{
    public int Id { get; set; }
    public int FinancialUploadId { get; set; }
    public DateTime? TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? EntryNumber { get; set; }
    public decimal? Debit { get; set; }
    public decimal? Credit { get; set; }
    public decimal? Balance { get; set; }
    public bool IsMarked { get; set; }
    public bool IsMatched { get; set; }
    public int RowOrder { get; set; }
}

public class SettlementSummaryDto
{
    public int Id { get; set; }
    public int FinancialUploadId { get; set; }
    public decimal PeriodDebit { get; set; }
    public decimal PeriodCredit { get; set; }
    public decimal PeriodBalance { get; set; }
}

public class SettlementDataDto
{
    public int FinancialUploadId { get; set; }
    public List<SettlementRowDto> Rows { get; set; } = [];
    public SettlementSummaryDto? Summary { get; set; }
}
