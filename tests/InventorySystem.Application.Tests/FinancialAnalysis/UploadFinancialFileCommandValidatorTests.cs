using InventorySystem.Application.Features.FinancialAnalysis.FinancialUploads.Commands.UploadFinancialFile;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using Xunit;

namespace InventorySystem.Application.Tests.FinancialAnalysis;

public class UploadFinancialFileCommandValidatorTests
{
    [Fact]
    public void Validate_NullContent_ReturnsValidationError()
    {
        var validator = new UploadFinancialFileCommandValidator();
        var command = new UploadFinancialFileCommand
        {
            FinancialPeriodId = 1,
            FileType = FinancialFileType.IncomeStatement,
            Scope = FinancialFileScope.SingleBranch,
            BranchId = 1,
            Gender = null,
            OriginalFileName = "test.xlsx",
            Content = null!
        };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            error => error.PropertyName == nameof(UploadFinancialFileCommand.Content));
    }

    [Fact]
    public void Validate_EmptyContent_ReturnsValidationError()
    {
        var validator = new UploadFinancialFileCommandValidator();
        var command = new UploadFinancialFileCommand
        {
            FinancialPeriodId = 1,
            FileType = FinancialFileType.IncomeStatement,
            Scope = FinancialFileScope.SingleBranch,
            BranchId = 1,
            OriginalFileName = "test.xlsx",
            Content = Array.Empty<byte>()
        };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            error => error.PropertyName == nameof(UploadFinancialFileCommand.Content));
    }

    [Fact]
    public void Validate_OversizedContent_ReturnsValidationError()
    {
        var validator = new UploadFinancialFileCommandValidator();
        var command = new UploadFinancialFileCommand
        {
            FinancialPeriodId = 1,
            FileType = FinancialFileType.IncomeStatement,
            Scope = FinancialFileScope.SingleBranch,
            BranchId = 1,
            OriginalFileName = "test.xlsx",
            Content = new byte[20 * 1024 * 1024 + 1]
        };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            error => error.PropertyName == nameof(UploadFinancialFileCommand.Content));
    }
}