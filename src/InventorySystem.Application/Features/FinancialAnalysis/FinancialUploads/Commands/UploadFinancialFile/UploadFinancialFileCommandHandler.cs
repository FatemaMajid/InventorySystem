using System.Security.Cryptography;
using InventorySystem.Application.Common.Exceptions;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialUpload;
using InventorySystem.Application.Features.FinancialAnalysis.Common.Excel;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialUploads.Commands.UploadFinancialFile;

public class UploadFinancialFileCommandHandler
    : IRequestHandler<
        UploadFinancialFileCommand,
        FinancialUploadDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLogService;
    private readonly IFinancialFileStorageService _fileStorage;
    private readonly FinancialExcelParser _excelParser;

    public UploadFinancialFileCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLogService,
        IFinancialFileStorageService fileStorage,
        FinancialExcelParser excelParser)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLogService = auditLogService;
        _fileStorage = fileStorage;
        _excelParser = excelParser;
    }

    public async Task<FinancialUploadDto> Handle(
        UploadFinancialFileCommand request,
        CancellationToken cancellationToken)
    {
        var period =
            await _context.FinancialPeriods
                .FirstOrDefaultAsync(
                    x =>
                        x.Id ==
                            request.FinancialPeriodId &&
                        !x.IsDeleted,
                    cancellationToken);

        if (period is null)
        {
            throw new KeyNotFoundException(
                "Financial period was not found.");
        }

        if (period.Status ==
            FinancialPeriodStatus.Approved)
        {
            throw new InvalidOperationException(
                "Files cannot be uploaded to an approved financial period.");
        }

        string? branchCode = null;
        string? branchNameArabic = null;

        if (request.Scope ==
            FinancialFileScope.SingleBranch)
        {
            var branch =
                await _context.Branches
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                request.BranchId &&
                            !x.IsDeleted &&
                            x.IsActive &&
                            x.BranchCode != "0" &&
                            x.BranchCode != "001" &&
                            x.BranchCode != "002",
                        cancellationToken);

            if (branch is null)
            {
                throw new InvalidOperationException(
                    "The selected financial branch was not found or is inactive.");
            }

            branchCode =
                branch.BranchCode;

            branchNameArabic =
                branch.BranchNameArabic;
        }

        var fileHash =
            Convert.ToHexString(
                SHA256.HashData(
                    request.Content));

        var activeUpload =
            await _context.FinancialUploads
                .FirstOrDefaultAsync(
                    x =>
                        x.FinancialPeriodId ==
                            request.FinancialPeriodId &&
                        x.FileType ==
                            request.FileType &&
                        x.BranchId ==
                            request.BranchId &&
                        x.Gender ==
                            request.Gender &&
                        x.IsActive &&
                        !x.IsDeleted,
                    cancellationToken);

        if (activeUpload is not null &&
            activeUpload.FileHash ==
                fileHash)
        {
            throw new DuplicateFinancialUploadException();
        }

        var lastVersion =
            await _context.FinancialUploads
                .Where(
                    x =>
                        x.FinancialPeriodId ==
                            request.FinancialPeriodId &&
                        x.FileType ==
                            request.FileType &&
                        x.BranchId ==
                            request.BranchId &&
                        x.Gender ==
                            request.Gender &&
                        !x.IsDeleted)
                .MaxAsync(
                    x =>
                        (int?)x.Version,
                    cancellationToken)
                ?? 0;

        var extension =
            Path.GetExtension(
                request.OriginalFileName)
                .ToLowerInvariant();

        var storedFileName =
            $"{Guid.NewGuid():N}{extension}";

        var storagePath =
            await _fileStorage.SaveAsync(
                request.Content,
                storedFileName,
                period.StartDate.Year,
                period.StartDate.Month,
                cancellationToken);

        var upload =
            new FinancialUpload
            {
                FinancialPeriodId =
                    request.FinancialPeriodId,

                FileType =
                    request.FileType,

                BranchId =
                    request.BranchId,

                Gender =
                    request.Gender,

                Scope =
                    request.Scope,

                OriginalFileName =
                    Path.GetFileName(
                        request.OriginalFileName),

                StoredFileName =
                    storedFileName,

                StoragePath =
                    storagePath,

                FileHash =
                    fileHash,

                FileSize =
                    request.Content.LongLength,

                Status =
                    FinancialUploadStatus.Validating,

                Version =
                    lastVersion + 1,

                IsActive =
                    false,

                UploadedAt =
                    DateTime.UtcNow,

                UploadedBy =
                    _currentUser.Username,

                CreatedAt =
                    DateTime.UtcNow,

                CreatedBy =
                    _currentUser.Username
            };

        _context.FinancialUploads.Add(
            upload);

        if (period.Status ==
            FinancialPeriodStatus.Draft)
        {
            period.Status =
                FinancialPeriodStatus.Uploading;

            period.UpdatedAt =
                DateTime.UtcNow;

            period.UpdatedBy =
                _currentUser.Username;
        }

        await _context.SaveChangesAsync(
            cancellationToken);

        try
        {
            var parseResult =
                await _excelParser.ParseAsync(
                    upload,
                    request.Content,
                    cancellationToken);

            if (parseResult.AccountRows.Count > 0)
            {
                _context.FinancialAccountRows
                    .AddRange(
                        parseResult.AccountRows);
            }

            if (parseResult.TransferRecords.Count > 0)
            {
                _context.TransferRecords
                    .AddRange(
                        parseResult.TransferRecords);
            }

            if (parseResult.SettlementRows.Count > 0)
            {
                _context.SettlementRows
                    .AddRange(
                        parseResult.SettlementRows);
            }

            if (parseResult.SettlementSummary is not null)
            {
                _context.SettlementSummaries
                    .Add(
                        parseResult
                            .SettlementSummary);
            }

            if (activeUpload is not null)
            {
                activeUpload.IsActive =
                    false;

                activeUpload.UpdatedAt =
                    DateTime.UtcNow;

                activeUpload.UpdatedBy =
                    _currentUser.Username;
            }

            upload.Status =
                FinancialUploadStatus.Valid;

            upload.IsActive =
                true;

            upload.ValidatedAt =
                DateTime.UtcNow;

            upload.ValidationMessage =
                $"Valid. Parsed rows: {parseResult.ParsedRowCount}.";

            upload.UpdatedAt =
                DateTime.UtcNow;

            upload.UpdatedBy =
                _currentUser.Username;

            await _context.SaveChangesAsync(
                cancellationToken);

            await UpdatePeriodStatusAsync(
                period,
                cancellationToken);

            await _auditLogService.LogAsync(
                action: "Upload",
                entity: "FinancialUpload",
                entityId:
                    upload.Id.ToString(),
                details:
                    $"PeriodId: {period.Id}, " +
                    $"FileType: {upload.FileType}, " +
                    $"BranchId: {upload.BranchId}, " +
                    $"Gender: {upload.Gender}, " +
                    $"Version: {upload.Version}, " +
                    $"Status: Valid, " +
                    $"Rows: {parseResult.ParsedRowCount}",
                cancellationToken:
                    cancellationToken);
        }
        catch (Exception ex)
        {
            upload.Status =
                FinancialUploadStatus.Invalid;

            upload.IsActive =
                false;

            upload.ValidatedAt =
                DateTime.UtcNow;

            upload.ValidationMessage =
                Truncate(
                    ex.Message,
                    2000);

            upload.UpdatedAt =
                DateTime.UtcNow;

            upload.UpdatedBy =
                _currentUser.Username;

            await _context.SaveChangesAsync(
                cancellationToken);

            await _auditLogService.LogAsync(
                action: "Upload",
                entity: "FinancialUpload",
                entityId:
                    upload.Id.ToString(),
                details:
                    $"PeriodId: {period.Id}, " +
                    $"FileType: {upload.FileType}, " +
                    $"BranchId: {upload.BranchId}, " +
                    $"Gender: {upload.Gender}, " +
                    $"Version: {upload.Version}, " +
                    $"Status: Invalid, " +
                    $"Error: {upload.ValidationMessage}",
                cancellationToken:
                    cancellationToken);
        }

        return new FinancialUploadDto
        {
            Id =
                upload.Id,

            FinancialPeriodId =
                upload.FinancialPeriodId,

            FileType =
                upload.FileType,

            BranchId =
                upload.BranchId,

            BranchCode =
                branchCode,

            BranchNameArabic =
                branchNameArabic,

            Gender =
                upload.Gender,

            Scope =
                upload.Scope,

            OriginalFileName =
                upload.OriginalFileName,

            FileSize =
                upload.FileSize,

            Status =
                upload.Status,

            Version =
                upload.Version,

            IsActive =
                upload.IsActive,

            UploadedAt =
                upload.UploadedAt,

            UploadedBy =
                upload.UploadedBy,

            ValidationMessage =
                upload.ValidationMessage
        };
    }

    private async Task UpdatePeriodStatusAsync(
        FinancialPeriod period,
        CancellationToken cancellationToken)
    {
        var activeBranchCount =
            await _context.Branches
                .CountAsync(
                    x =>
                        !x.IsDeleted &&
                        x.IsActive &&
                        x.BranchCode != "0" &&
                        x.BranchCode != "001" &&
                        x.BranchCode != "002",
                    cancellationToken);

        var expectedFiles =
            2 +
            (activeBranchCount * 2) +
            activeBranchCount +
            activeBranchCount;

        var validUploads =
            await _context.FinancialUploads
                .CountAsync(
                    x =>
                        x.FinancialPeriodId ==
                            period.Id &&
                        x.IsActive &&
                        !x.IsDeleted &&
                        x.Status ==
                            FinancialUploadStatus.Valid,
                    cancellationToken);

        if (validUploads ==
            expectedFiles)
        {
            period.Status =
                FinancialPeriodStatus.Ready;
        }
        else if (period.Status !=
                 FinancialPeriodStatus.Approved)
        {
            period.Status =
                FinancialPeriodStatus.Uploading;
        }

        period.UpdatedAt =
            DateTime.UtcNow;

        period.UpdatedBy =
            _currentUser.Username;

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    private static string Truncate(
        string value,
        int maxLength)
    {
        if (string.IsNullOrEmpty(
                value))
        {
            return value;
        }

        return value.Length <=
               maxLength
            ? value
            : value[..maxLength];
    }
}


