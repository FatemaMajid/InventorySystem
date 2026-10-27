using ClosedXML.Excel;
using InventorySystem.Application.Common.Excel;
using InventorySystem.Application.Common.Excel.Preview;
using InventorySystem.Application.Common.Excel.Validation;
using InventorySystem.Application.Common.Localization;

namespace InventorySystem.Application.Features.InventorySessions.Import.Preview;

public class InventoryImportPreviewService
{
    public InventoryImportPreviewResponse BuildPreview(
        InventoryImportPreviewRequest request)
    {
        ValidateFile(request);

        var headerRow = DetectHeader(request.FileStream);
        var rows = ReadExcel(request.FileStream);

        var validationResult =
            ExcelValidator.Validate(rows, headerRow);

        var locationValidationResult =
            InventoryLocationValidator.Validate(
                rows,
                headerRow);

        var preview = InventoryPreviewBuilder.Build(
            rows,
            validationResult,
            locationValidationResult,
            headerRow);

        return new InventoryImportPreviewResponse
        {
            IsValid =
                validationResult.IsValid &&
                locationValidationResult.IsValid,

            Preview = preview
        };
    }

    private static void ValidateFile(
        InventoryImportPreviewRequest request)
    {
        if (request.FileStream == null)
            throw new ArgumentNullException(
                nameof(request.FileStream));

        if (!request.FileStream.CanRead ||
            !request.FileStream.CanSeek)
        {
            throw new InvalidOperationException(
                LocalizationKeys.Excel.InvalidFile);
        }

        if (request.FileStream.Length == 0)
            throw new InvalidOperationException(
                LocalizationKeys.Excel.EmptyFile);
    }

    private static int DetectHeader(Stream stream)
    {
        stream.Position = 0;

        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
            throw new InvalidOperationException(
                LocalizationKeys.Excel.NoWorksheet);

        var (headerRow, _) =
            ExcelHeaderDetector.Detect(worksheet);

        return headerRow;
    }

    private static List<InventoryExcelRow> ReadExcel(
        Stream stream)
    {
        stream.Position = 0;
        return ExcelReader.Read(stream);
    }
}