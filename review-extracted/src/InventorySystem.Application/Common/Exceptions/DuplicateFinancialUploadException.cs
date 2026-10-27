namespace InventorySystem.Application.Common.Exceptions;

public class DuplicateFinancialUploadException : Exception
{
    public DuplicateFinancialUploadException()
        : base("The same file has already been uploaded.")
    {
    }

    public DuplicateFinancialUploadException(string message)
        : base(message)
    {
    }
}
