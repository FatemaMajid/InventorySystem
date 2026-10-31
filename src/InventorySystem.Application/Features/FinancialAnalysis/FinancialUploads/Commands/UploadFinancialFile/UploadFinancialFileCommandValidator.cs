using FluentValidation;
using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialUploads.Commands.UploadFinancialFile;

public class UploadFinancialFileCommandValidator
    : AbstractValidator<UploadFinancialFileCommand>
{
    private const int MaxFileSize =
        20 * 1024 * 1024;

    public UploadFinancialFileCommandValidator()
    {
        RuleFor(x => x.FinancialPeriodId)
            .GreaterThan(0);

        RuleFor(x => x.FileType)
            .IsInEnum();

        RuleFor(x => x.Scope)
            .IsInEnum();

        RuleFor(x => x.OriginalFileName)
            .NotEmpty()
            .Must(BeExcelFile)
            .WithMessage(
                "Only .xlsx Excel files are supported.");

        RuleFor(x => x.Content)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage(
                "The uploaded file is empty.");

        RuleFor(x => x.Content)
            .Must(x => x.Length <= MaxFileSize)
            .When(x => x.Content is not null)
            .WithMessage(
                "The maximum allowed file size is 20 MB.");

        RuleFor(x => x)
            .Must(HaveValidMetadata)
            .WithMessage(
                "Invalid branch, scope or gender combination for this financial file type.");
    }

    private static bool BeExcelFile(
        string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        var extension =
            Path.GetExtension(fileName)
                .ToLowerInvariant();

        return extension == ".xlsx";
    }

    private static bool HaveValidMetadata(
        UploadFinancialFileCommand command)
    {
        switch (command.FileType)
        {
            case FinancialFileType.IncomeStatement:
            case FinancialFileType.FinancialPosition:
                return
                    command.Scope ==
                        FinancialFileScope.SingleBranch &&
                    command.BranchId.HasValue &&
                    command.BranchId.Value > 0 &&
                    command.Gender is null;

            case FinancialFileType.Transfer:
                return
                    command.Scope ==
                        FinancialFileScope.AllBranches &&
                    command.BranchId is null &&
                    command.Gender.HasValue;

            case FinancialFileType.Settlement:
                return
                    command.Scope ==
                        FinancialFileScope.SingleBranch &&
                    command.BranchId.HasValue &&
                    command.BranchId.Value > 0 &&
                    command.Gender.HasValue;

            default:
                return false;
        }
    }
}
