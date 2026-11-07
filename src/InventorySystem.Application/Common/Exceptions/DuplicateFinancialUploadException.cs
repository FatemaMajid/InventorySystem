using InventorySystem.Application.Common.Localization;

namespace InventorySystem.Application.Common.Exceptions;

public class DuplicateFinancialUploadException : Exception
{
    public DuplicateFinancialUploadException()
        : base(LocalizationKeys.FinancialUpload.DuplicateFile)
    {
    }

    public DuplicateFinancialUploadException(string message)
        : base(message)
    {
    }
}
