using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialUpload;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialUploads.Commands.UploadFinancialFile;

public class UploadFinancialFileCommand
    : IRequest<FinancialUploadDto>
{
    public int FinancialPeriodId { get; set; }
    public FinancialFileType FileType { get; set; }
    public FinancialFileScope Scope { get; set; }
    public int? BranchId { get; set; }
    public FinancialGender? Gender { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = [];
}
