using ClosedXML.Excel;
using InventorySystem.Application.Common.Excel;
using InventorySystem.Application.Common.Excel.Preview;
using InventorySystem.Application.Common.Excel.Validation;

namespace InventorySystem.Application.Features.InventorySessions.Import.Preview;

public class InventoryImportPreviewService
{
    public InventoryImportPreviewResponse BuildPreview(
        InventoryImportPreviewRequest request)
    {
        if (request.FileStream == null)
        {
            throw new ArgumentNullException(
                nameof(request.FileStream));
        }

        if (!request.FileStream.CanRead)
        {
            throw new InvalidOperationException(
                "ملف Excel غير قابل للقراءة.");
        }

        if (!request.FileStream.CanSeek)
        {
            throw new InvalidOperationException(
                "Stream الخاص بملف Excel يجب أن يكون قابلًا للبحث.");
        }

        if (request.FileStream.Length == 0)
        {
            throw new InvalidOperationException(
                "ملف Excel فارغ.");
        }

        // Make sure the stream starts from the beginning.
        request.FileStream.Position = 0;

        // Detect the Excel header.
        using var workbook =
            new XLWorkbook(request.FileStream);

        var worksheet =
            workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
        {
            throw new InvalidOperationException(
                "ملف Excel لا يحتوي على أي Sheet.");
        }

        var (headerRow, _) =
            ExcelHeaderDetector.Detect(
                worksheet);

        // Reset stream before reading rows.
        request.FileStream.Position = 0;

        var rows =
            ExcelReader.Read(
                request.FileStream);

        // Validate item data.
        var validationResult =
            ExcelValidator.Validate(
                rows,
                headerRow);

        // Validate branch and store.
        var locationValidationResult =
            InventoryLocationValidator.Validate(
                rows,
                headerRow);

        // Build preview.
        var preview =
            InventoryPreviewBuilder.Build(
                rows,
                validationResult,
                locationValidationResult,
                headerRow);

        var isValid =
            validationResult.IsValid &&
            locationValidationResult.IsValid;

        return new InventoryImportPreviewResponse
        {
            IsValid = isValid,

            Preview = preview
        };
    }
}