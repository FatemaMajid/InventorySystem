using System.Text.Json;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.DashboardAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Dashboard.Queries.GetFinancialAnalysisDashboard;

public sealed class GetFinancialAnalysisDashboardQueryHandler
    : IRequestHandler<
        GetFinancialAnalysisDashboardQuery,
        FinancialAnalysisDashboardDto?>
{
    private static readonly string[] AllSections =
    {
        "sales",
        "inventory",
        "expenses",
        "purchases",
        "profit",
        "customers",
        "debt",
        "cash",
        "suppliers",
        "capital",
        "transfers",
        "settlements"
    };

    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;

    public GetFinancialAnalysisDashboardQueryHandler(
        IMediator mediator,
        IApplicationDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    public async Task<FinancialAnalysisDashboardDto?> Handle(
        GetFinancialAnalysisDashboardQuery request,
        CancellationToken cancellationToken)
    {
        /*
         * Period information and Upload completion
         * come from the existing monitoring query.
         */
        var periodSummary =
            await _mediator.Send(
                new InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetFinancialPeriodSummary.GetFinancialPeriodSummaryQuery(
                    request.FinancialPeriodId),
                cancellationToken);

        if (periodSummary is null)
            return null;

        FinancialDashboardBranchDto? selectedBranch =
            null;

        if (request.BranchId.HasValue)
        {
            selectedBranch =
                await _context.Branches
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.Id ==
                                request.BranchId.Value &&
                            !x.IsDeleted &&
                            x.IsActive &&
                            x.BranchCode != "0" &&
                            x.BranchCode != "001" &&
                            x.BranchCode != "002")
                    .Select(
                        x =>
                            new FinancialDashboardBranchDto
                            {
                                BranchId =
                                    x.Id,

                                BranchCode =
                                    x.BranchCode,

                                BranchName =
                                    x.BranchNameArabic
                            })
                    .FirstOrDefaultAsync(
                        cancellationToken);

            if (selectedBranch is null)
            {
                throw new KeyNotFoundException(
                    "The selected financial branch was not found.");
            }
        }

        var sections =
            NormalizeSections(
                request.Sections);

        var result =
            new FinancialAnalysisDashboardDto
            {
                FinancialPeriodId =
                    periodSummary.FinancialPeriodId,

                StartDate =
                    periodSummary.StartDate,

                EndDate =
                    periodSummary.EndDate,

                Status =
                    periodSummary.Status,

                ActiveBranches =
                    periodSummary.ActiveBranches,

                ExpectedFiles =
                    periodSummary.ExpectedFiles,

                ValidFiles =
                    periodSummary.ValidFiles,

                MissingFiles =
                    periodSummary.MissingFiles,

                CompletionPercentage =
                    periodSummary.CompletionPercentage,

                IsAllBranches =
                    !request.BranchId.HasValue,

                SelectedBranch =
                    selectedBranch,

                SelectedSections =
                    sections.ToList()
            };

        /*
         * Sequential execution is intentional.
         *
         * EF DbContext is scoped and must not be
         * queried concurrently by multiple handlers.
         */

        foreach (var section in sections)
        {
            object? sectionData =
                section switch
                {
                    "sales" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Sales.Queries.GetSalesAnalysis.GetSalesAnalysisQuery(
                                request.FinancialPeriodId),
                            cancellationToken),

                    "inventory" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Inventory.Queries.GetInventoryAnalysis.GetInventoryAnalysisQuery(
                                request.FinancialPeriodId),
                            cancellationToken),

                    "expenses" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Expenses.Queries.GetExpensesAnalysis.GetExpensesAnalysisQuery(
                                request.FinancialPeriodId),
                            cancellationToken),

                    "purchases" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Purchases.Queries.GetPurchasesAnalysis.GetPurchasesAnalysisQuery(
                                request.FinancialPeriodId),
                            cancellationToken),

                    "profit" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.ProfitLoss.Queries.GetProfitLossAnalysis.GetProfitLossAnalysisQuery(
                                request.FinancialPeriodId),
                            cancellationToken),

                    "customers" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Customers.Queries.GetCustomersAnalysis.GetCustomersAnalysisQuery(
                                request.FinancialPeriodId),
                            cancellationToken),

                    "debt" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Debt.Queries.GetBranchDebtAnalysis.GetBranchDebtAnalysisQuery(
                                request.FinancialPeriodId),
                            cancellationToken),

                    "cash" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Cash.Queries.GetCashAnalysis.GetCashAnalysisQuery(
                                request.FinancialPeriodId),
                            cancellationToken),

                    "suppliers" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Suppliers.Queries.GetSuppliersAnalysis.GetSuppliersAnalysisQuery(
                                request.FinancialPeriodId),
                            cancellationToken),

                    "capital" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Capital.Queries.GetCapitalAnalysis.GetCapitalAnalysisQuery(
                                request.FinancialPeriodId),
                            cancellationToken),

                    "transfers" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Transfers.Queries.GetTransfersAnalysis.GetTransfersAnalysisQuery(
                                request.FinancialPeriodId),
                            cancellationToken),

                    "settlements" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Settlements.Queries.GetSettlementsAnalysis.GetSettlementsAnalysisQuery(
                                request.FinancialPeriodId),
                            cancellationToken),

                    _ =>
                        throw new InvalidOperationException(
                            $"Unsupported dashboard section: {section}")
                };

                        var serializedSection =
                SerializeSection(
                    sectionData,
                    request.BranchId);

            if(
                request.IncludeDetails &&
                serializedSection.HasValue &&
                SupportsBranchDetails(
                    section)
            )
            {
                serializedSection =
                    await EnrichSectionWithDetailsAsync(
                        section,
                        serializedSection.Value,
                        request.FinancialPeriodId,
                        request.BranchId,
                        cancellationToken);
            }

            result.Sections[section] =
                serializedSection;
        }

        return result;
    }

    private static bool SupportsBranchDetails(
        string section)
    {
        return section switch
        {
            "expenses" => true,
            "purchases" => true,
            "cash" => true,
            "suppliers" => true,
            "transfers" => true,
            "settlements" => true,
            "customers" => true,
            _ => false
        };
    }

    private async Task<JsonElement> EnrichSectionWithDetailsAsync(
        string section,
        JsonElement serializedSection,
        int financialPeriodId,
        int? selectedBranchId,
        CancellationToken cancellationToken)
    {
        var branchIds =
            selectedBranchId.HasValue
                ? new List<int>
                {
                    selectedBranchId.Value
                }
                : await _context.Branches
                    .AsNoTracking()
                    .Where(
                        x =>
                            !x.IsDeleted &&
                            x.IsActive &&
                            x.BranchCode != "0" &&
                            x.BranchCode != "001" &&
                            x.BranchCode != "002")
                    .OrderBy(
                        x =>
                            x.BranchCode)
                    .Select(
                        x =>
                            x.Id)
                    .ToListAsync(
                        cancellationToken);

        var detailsByBranch =
            new Dictionary<int,JsonElement>();

        foreach(
            var branchId
            in branchIds)
        {
            object? details =
                section switch
                {
                    "expenses" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Expenses.Queries.GetExpenseDetails.GetExpenseDetailsQuery(
                                financialPeriodId,
                                branchId),
                            cancellationToken),
                    "purchases" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Purchases.Queries.GetPurchaseDetails.GetPurchaseDetailsQuery(
                                financialPeriodId,
                                branchId),
                            cancellationToken),


                    "cash" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Cash.Queries.GetCashDetails.GetCashDetailsQuery(
                                financialPeriodId,
                                branchId),
                            cancellationToken),

                    "suppliers" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Suppliers.Queries.GetSupplierDetails.GetSupplierDetailsQuery(
                                financialPeriodId,
                                branchId),
                            cancellationToken),

                    "transfers" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Transfers.Queries.GetTransferDetails.GetTransferDetailsQuery(
                                financialPeriodId,
                                branchId),
                            cancellationToken),

                    "settlements" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Settlements.Queries.GetSettlementDetails.GetSettlementDetailsQuery(
                                financialPeriodId,
                                branchId),
                            cancellationToken),

                    "customers" =>
                        await _mediator.Send(
                            new InventorySystem.Application.Features.FinancialAnalysis.Customers.Queries.GetCustomerDetails.GetCustomerDetailsQuery(
                                financialPeriodId,
                                branchId),
                            cancellationToken),

                    _ =>
                        null
                };

            if(details is null)
                continue;

            var detailElement =
                JsonSerializer.SerializeToElement(
                    details,
                    details.GetType(),
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy =
                            JsonNamingPolicy.CamelCase
                    });

            detailsByBranch[branchId] =
                detailElement;
        }

        if(detailsByBranch.Count == 0)
            return serializedSection.Clone();

        return AttachBranchDetails(
            serializedSection,
            detailsByBranch);
    }

    private static JsonElement AttachBranchDetails(
        JsonElement section,
        IReadOnlyDictionary<int,JsonElement> detailsByBranch)
    {
        using var stream =
            new MemoryStream();

        using(
            var writer =
                new Utf8JsonWriter(
                    stream)
        )
        {
            if(
                section.ValueKind ==
                    JsonValueKind.Object &&
                section.TryGetProperty(
                    "branches",
                    out var branches) &&
                branches.ValueKind ==
                    JsonValueKind.Array
            )
            {
                writer.WriteStartObject();

                foreach(
                    var property
                    in section.EnumerateObject())
                {
                    if(
                        property.Name.Equals(
                            "branches",
                            StringComparison.OrdinalIgnoreCase)
                    )
                    {
                        writer.WritePropertyName(
                            property.Name);

                        writer.WriteStartArray();

                        foreach(
                            var branch
                            in branches.EnumerateArray())
                        {
                            WriteBranchWithDetails(
                                writer,
                                branch,
                                detailsByBranch);
                        }

                        writer.WriteEndArray();

                        continue;
                    }

                    property.WriteTo(
                        writer);
                }

                writer.WriteEndObject();
            }
            else
            {
                WriteBranchWithDetails(
                    writer,
                    section,
                    detailsByBranch);
            }
        }

        using var document =
            JsonDocument.Parse(
                stream.ToArray());

        return document.RootElement.Clone();
    }

    private static void WriteBranchWithDetails(
        Utf8JsonWriter writer,
        JsonElement branch,
        IReadOnlyDictionary<int,JsonElement> detailsByBranch)
    {
        if(branch.ValueKind !=
            JsonValueKind.Object)
        {
            branch.WriteTo(
                writer);

            return;
        }

        int? branchId =
            null;

        if(
            branch.TryGetProperty(
                "branchId",
                out var branchIdElement) &&
            branchIdElement.TryGetInt32(
                out var parsedBranchId)
        )
        {
            branchId =
                parsedBranchId;
        }

        writer.WriteStartObject();

        foreach(
            var property
            in branch.EnumerateObject())
        {
            if(
                property.Name.Equals(
                    "details",
                    StringComparison.OrdinalIgnoreCase)
            )
            {
                continue;
            }

            property.WriteTo(
                writer);
        }

        if(
            branchId.HasValue &&
            detailsByBranch.TryGetValue(
                branchId.Value,
                out var details)
        )
        {
            writer.WritePropertyName(
                "details");

            details.WriteTo(
                writer);
        }

        writer.WriteEndObject();
    }
    private static IReadOnlyCollection<string>
        NormalizeSections(
            IReadOnlyCollection<string> requested)
    {
        if (requested.Count == 0)
            return AllSections;

        var normalized =
            requested
                .SelectMany(
                    x =>
                        x.Split(
                            ',',
                            StringSplitOptions.RemoveEmptyEntries |
                            StringSplitOptions.TrimEntries))
                .Select(
                    NormalizeSectionName)
                .Where(
                    x =>
                        !string.IsNullOrWhiteSpace(x))
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        if (
            normalized.Count == 0 ||
            normalized.Any(
                x =>
                    string.Equals(
                        x,
                        "all",
                        StringComparison.OrdinalIgnoreCase))
        )
        {
            return AllSections;
        }

        var invalid =
            normalized
                .Where(
                    x =>
                        !AllSections.Contains(
                            x,
                            StringComparer.OrdinalIgnoreCase))
                .ToList();

        if (invalid.Count > 0)
        {
            throw new InvalidOperationException(
                "Unsupported dashboard sections: " +
                string.Join(
                    ", ",
                    invalid));
        }

        /*
         * Keep canonical dashboard order,
         * not query-string order.
         */
        return AllSections
            .Where(
                section =>
                    normalized.Contains(
                        section,
                        StringComparer.OrdinalIgnoreCase))
            .ToArray();
    }

    private static string NormalizeSectionName(
        string value)
    {
        var section =
            value
                .Trim()
                .ToLowerInvariant();

        return section switch
        {
            "profit-loss" =>
                "profit",

            "profitloss" =>
                "profit",

            "customers-employees" =>
                "customers",

            "customersemployees" =>
                "customers",

            "supplier" =>
                "suppliers",

            "transfer" =>
                "transfers",

            "settlement" =>
                "settlements",

            _ =>
                section
        };
    }

    private static JsonElement? SerializeSection(
        object? source,
        int? branchId)
    {
        if (source is null)
            return null;

        var element =
            JsonSerializer.SerializeToElement(
                source,
                source.GetType(),
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy =
                        JsonNamingPolicy.CamelCase
                });

        /*
         * ALL BRANCHES:
         *
         * Return the complete Analysis DTO:
         * totals + average + highest/lowest +
         * Branches[].
         */
        if (!branchId.HasValue)
            return element.Clone();

        /*
         * SINGLE BRANCH:
         *
         * Every Analysis summary we built returns
         * Branches[] containing all financial
         * branches.
         *
         * Dashboard extracts ONLY the selected
         * Branch DTO from that tested result.
         */
        if (
            element.ValueKind ==
                JsonValueKind.Object &&
            element.TryGetProperty(
                "branches",
                out var branches) &&
            branches.ValueKind ==
                JsonValueKind.Array
        )
        {
            foreach (var branch in branches.EnumerateArray())
            {
                if (
                    branch.ValueKind ==
                        JsonValueKind.Object &&
                    branch.TryGetProperty(
                        "branchId",
                        out var idElement) &&
                    idElement.TryGetInt32(
                        out var id) &&
                    id ==
                        branchId.Value
                )
                {
                    return branch.Clone();
                }
            }
        }

        return null;
    }
}
