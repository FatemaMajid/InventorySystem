import { useCallback, useEffect, useMemo, useState } from "react";

import { useLanguage } from "../../../../context/LanguageContext";
import { getBranches } from "../../../../services/branchService";
import {
  exportFinancialDashboard,
  getFinancialDashboard,
  getFinancialPeriods,
} from "../../../../services/financialAnalysisService";

import LoadingState from "../../../../components/UI/Loading/LoadingState/LoadingState";
import ErrorState from "../../../../components/UI/ErrorState/ErrorState";

import styles from "./FinancialAnalysis.module.css";

const SECTION_KEYS = [
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
  "settlements",
];

const STATUS_NAMES = {
  0: "Draft",
  1: "Uploading",
  2: "Ready",
  3: "Approved",
};

const SECTION_VALUE_KEYS = {
  sales: [
    "allBranchesTotal",
    "allBranchesSales",
    "netSales",
  ],
  inventory: [
    "allBranchesInventoryChange",
    "inventoryChange",
    "allBranchesClosingInventory",
  ],
  expenses: [
    "allBranchesTotalExpenses",
    "totalExpenses",
  ],
  purchases: [
    "allBranchesTotal",
    "totalPurchases",
    "purchases",
  ],
  profit: [
    "allBranchesProfitLoss",
    "profitLoss",
    "netProfitLoss",
  ],
  customers: [
    "allBranchesCustomersTotal",
    "customersTotal",
  ],
  debt: [
    "allBranchesTotalDebt",
    "totalDebt",
  ],
  cash: [
    "allBranchesCash",
    "totalCash",
  ],
  suppliers: [
    "allBranchesSuppliers",
    "suppliersTotal",
  ],
  capital: [
    "allBranchesCapital",
    "capital",
  ],
  transfers: [
    "allBranchesTotalTransfer",
    "totalTransfer",
  ],
  settlements: [
    "allBranchesBalance",
    "totalBalance",
    "balance",
  ],
};

function unwrap(response) {
  return response?.data ?? response;
}

function getPeriodId(period) {
  return (
    period?.id ??
    period?.financialPeriodId ??
    period?.FinancialPeriodId ??
    ""
  );
}

function getBranchId(branch) {
  return (
    branch?.id ??
    branch?.branchId ??
    branch?.BranchId ??
    ""
  );
}

function getBranchCode(branch) {
  return String(
    branch?.branchCode ??
    branch?.BranchCode ??
    ""
  );
}

function getBranchName(branch, language) {
  if (language === "en") {
    return (
      branch?.branchNameEnglish ||
      branch?.nameEnglish ||
      branch?.branchNameArabic ||
      branch?.nameArabic ||
      branch?.branchName ||
      ""
    );
  }

  return (
    branch?.branchNameArabic ||
    branch?.nameArabic ||
    branch?.branchName ||
    branch?.branchNameEnglish ||
    ""
  );
}

function normalizeStatus(status) {
  if (
    status === null ||
    status === undefined ||
    status === ""
  ) {
    return "";
  }

  if (
    typeof status === "number" ||
    /^\d+$/.test(String(status))
  ) {
    return STATUS_NAMES[Number(status)] || String(status);
  }

  return String(status);
}

function extractPrimaryValue(sectionKey, data) {
  if (!data || typeof data !== "object") {
    return null;
  }

  const candidates =
    SECTION_VALUE_KEYS[sectionKey] || [];

  for (const key of candidates) {
    if (
      data[key] !== null &&
      data[key] !== undefined &&
      typeof data[key] === "number"
    ) {
      return data[key];
    }
  }

  const fallback =
    Object.entries(data).find(
      ([key, value]) =>
        typeof value === "number" &&
        ![
          "financialPeriodId",
          "activeBranches",
          "branchesWithData",
          "branchesWithoutData",
          "branchesWithAnyData",
          "branchesWithCompleteData",
          "branchesWithoutCompleteData",
          "completeBranches",
          "incompleteBranches",
          "maleRecordsCount",
          "femaleRecordsCount",
          "transactionsCount",
        ].includes(key)
    );

  return fallback?.[1] ?? null;
}

function downloadBlob(blob, fileName) {
  const url =
    window.URL.createObjectURL(blob);

  const link =
    document.createElement("a");

  link.href = url;
  link.download = fileName;

  document.body.appendChild(link);
  link.click();
  link.remove();

  window.URL.revokeObjectURL(url);
}

function FinancialAnalysis() {
  const {
    translations,
    language,
    direction,
    formatNumber,
  } = useLanguage();

  const t =
    translations.financialAnalysis;

  const [periods, setPeriods] =
    useState([]);

  const [branches, setBranches] =
    useState([]);

  const [periodId, setPeriodId] =
    useState("");

  const [branchId, setBranchId] =
    useState("all");

  const [dashboard, setDashboard] =
    useState(null);

  const [loadingOptions, setLoadingOptions] =
    useState(true);

  const [loadingDashboard, setLoadingDashboard] =
    useState(false);

  const [exporting, setExporting] =
    useState("");

  const [error, setError] =
    useState("");

  const financialBranches =
    useMemo(
      () =>
        branches
          .filter((branch) => {
            const code =
              getBranchCode(branch);

            const active =
              branch?.isActive ??
              branch?.active ??
              true;

            return (
              active !== false &&
              code !== "0" &&
              code !== "001" &&
              code !== "002"
            );
          })
          .sort((a, b) =>
            getBranchCode(a).localeCompare(
              getBranchCode(b),
              "en",
              {
                numeric: true,
              }
            )
          ),
      [branches]
    );

  const loadOptions =
    useCallback(async () => {
      setLoadingOptions(true);
      setError("");

      try {
        const [
          periodsResponse,
          branchesResponse,
        ] = await Promise.all([
          getFinancialPeriods(),
          getBranches(),
        ]);

        const periodData =
          unwrap(periodsResponse) || [];

        const branchData =
          unwrap(branchesResponse) || [];

        const normalizedPeriods =
          Array.isArray(periodData)
            ? [...periodData].sort(
                (a, b) =>
                  Number(getPeriodId(b)) -
                  Number(getPeriodId(a))
              )
            : [];

        setPeriods(normalizedPeriods);

        setBranches(
          Array.isArray(branchData)
            ? branchData
            : []
        );

        if (
          !periodId &&
          normalizedPeriods.length > 0
        ) {
          setPeriodId(
            String(
              getPeriodId(
                normalizedPeriods[0]
              )
            )
          );
        }
      } catch (err) {
        setError(
          err?.message ||
          t.errors.loadOptions
        );
      } finally {
        setLoadingOptions(false);
      }
    }, [periodId, t.errors.loadOptions]);

  const loadDashboard =
    useCallback(async () => {
      if (!periodId) {
        setDashboard(null);
        return;
      }

      setLoadingDashboard(true);
      setError("");

      try {
        const response =
          await getFinancialDashboard({
            periodId,
            branchId,
            sections: "all",
          });

        setDashboard(
          unwrap(response)
        );
      } catch (err) {
        setDashboard(null);

        setError(
          err?.message ||
          t.errors.loadDashboard
        );
      } finally {
        setLoadingDashboard(false);
      }
    }, [
      periodId,
      branchId,
      t.errors.loadDashboard,
    ]);

  useEffect(() => {
    loadOptions();
  }, [loadOptions]);

  useEffect(() => {
    loadDashboard();
  }, [loadDashboard]);

  const handleExport =
    async (format) => {
      if (
        !periodId ||
        exporting
      ) {
        return;
      }

      setExporting(format);
      setError("");

      try {
        const response =
          await exportFinancialDashboard({
            periodId,
            branchId,
            sections: "all",
            language,
            format,
          });

        const blob =
          response instanceof Blob
            ? response
            : response?.data instanceof Blob
              ? response.data
              : response;

        if (!(blob instanceof Blob)) {
          throw new Error(
            t.errors.export
          );
        }

        const extension =
          format === "excel"
            ? "xlsx"
            : "pdf";

        const scope =
          branchId === "all"
            ? "All"
            : branchId;

        downloadBlob(
          blob,
          `Financial_Dashboard_Period_${periodId}_${scope}_${language.toUpperCase()}.${extension}`
        );
      } catch (err) {
        setError(
          err?.message ||
          t.errors.export
        );
      } finally {
        setExporting("");
      }
    };

  const sections =
    dashboard?.sections || {};

  const status =
    normalizeStatus(
      dashboard?.status
    );

  if (loadingOptions) {
    return (
      <main
        className={styles.page}
        dir={direction}
      >
        <LoadingState />
      </main>
    );
  }

  return (
    <main
      className={styles.page}
      dir={direction}
    >
      <section className={styles.header}>
        <div>
          <p className={styles.eyebrow}>
            {t.eyebrow}
          </p>

          <h1>
            {t.title}
          </h1>

          <p className={styles.description}>
            {t.description}
          </p>
        </div>

        <div className={styles.actions}>
          <button
            type="button"
            className={styles.secondaryButton}
            disabled={
              !periodId ||
              Boolean(exporting)
            }
            onClick={() =>
              handleExport("excel")
            }
          >
            {exporting === "excel"
              ? t.exporting
              : t.exportExcel}
          </button>

          <button
            type="button"
            className={styles.primaryButton}
            disabled={
              !periodId ||
              Boolean(exporting)
            }
            onClick={() =>
              handleExport("pdf")
            }
          >
            {exporting === "pdf"
              ? t.exporting
              : t.exportPdf}
          </button>
        </div>
      </section>

      <section className={styles.filters}>
        <div className={styles.field}>
          <label htmlFor="financial-period">
            {t.period}
          </label>

          <select
            id="financial-period"
            value={periodId}
            onChange={(event) => {
              setPeriodId(
                event.target.value
              );
            }}
          >
            {periods.length === 0 && (
              <option value="">
                {t.noPeriods}
              </option>
            )}

            {periods.map((period) => {
              const id =
                getPeriodId(period);

              const start =
                period?.startDate
                  ? new Date(
                      period.startDate
                    ).toLocaleDateString(
                      "en-GB"
                    )
                  : "";

              const end =
                period?.endDate
                  ? new Date(
                      period.endDate
                    ).toLocaleDateString(
                      "en-GB"
                    )
                  : "";

              return (
                <option
                  key={id}
                  value={id}
                >
                  {`#${id} — ${start} → ${end}`}
                </option>
              );
            })}
          </select>
        </div>

        <div className={styles.field}>
          <label htmlFor="financial-branch">
            {t.branch}
          </label>

          <select
            id="financial-branch"
            value={branchId}
            onChange={(event) =>
              setBranchId(
                event.target.value
              )
            }
          >
            <option value="all">
              {t.allBranches}
            </option>

            {financialBranches.map(
              (branch) => {
                const id =
                  getBranchId(branch);

                const code =
                  getBranchCode(branch);

                const name =
                  getBranchName(
                    branch,
                    language
                  );

                return (
                  <option
                    key={id}
                    value={id}
                  >
                    {code
                      ? `${code} - ${name}`
                      : name}
                  </option>
                );
              }
            )}
          </select>
        </div>
      </section>

      {error && (
        <section className={styles.error}>
          <ErrorState
            message={error}
            onRetry={
              periodId
                ? loadDashboard
                : loadOptions
            }
          />
        </section>
      )}

      {periods.length === 0 && !error && (
        <section className={styles.empty}>
          <h2>
            {t.noPeriodsTitle}
          </h2>

          <p>
            {t.noPeriodsDescription}
          </p>
        </section>
      )}

      {periodId && loadingDashboard && (
        <section className={styles.loading}>
          <LoadingState />
        </section>
      )}

      {dashboard && !loadingDashboard && (
        <>
          <section className={styles.summaryGrid}>
            <article className={styles.summaryCard}>
              <span>
                {t.period}
              </span>

              <strong>
                {dashboard.startDate
                  ? new Date(
                      dashboard.startDate
                    ).toLocaleDateString(
                      "en-GB"
                    )
                  : "—"}
                {" — "}
                {dashboard.endDate
                  ? new Date(
                      dashboard.endDate
                    ).toLocaleDateString(
                      "en-GB"
                    )
                  : "—"}
              </strong>
            </article>

            <article className={styles.summaryCard}>
              <span>
                {t.status}
              </span>

              <strong>
                {t.statuses[status] ||
                  status ||
                  "—"}
              </strong>
            </article>

            <article className={styles.summaryCard}>
              <span>
                {t.scope}
              </span>

              <strong>
                {dashboard.isAllBranches
                  ? t.allBranches
                  : dashboard.selectedBranch
                    ? `${dashboard.selectedBranch.branchCode} - ${dashboard.selectedBranch.branchName}`
                    : t.selectedBranch}
              </strong>
            </article>

            <article className={styles.summaryCard}>
              <span>
                {t.activeBranches}
              </span>

              <strong>
                {formatNumber(
                  dashboard.activeBranches ?? 0
                )}
              </strong>
            </article>
          </section>

          <section className={styles.sectionBlock}>
            <div className={styles.sectionHeader}>
              <div>
                <h2>
                  {t.sectionsTitle}
                </h2>

                <p>
                  {t.sectionsDescription}
                </p>
              </div>
            </div>

            <div className={styles.sectionsGrid}>
              {SECTION_KEYS.map(
                (sectionKey) => {
                  const data =
                    sections[sectionKey];

                  const value =
                    extractPrimaryValue(
                      sectionKey,
                      data
                    );

                  return (
                    <article
                      key={sectionKey}
                      className={styles.analysisCard}
                    >
                      <div
                        className={
                          styles.analysisCardTop
                        }
                      >
                        <span
                          className={
                            styles.sectionDot
                          }
                        />

                        <h3>
                          {t.sections[
                            sectionKey
                          ]}
                        </h3>
                      </div>

                      <div
                        className={
                          styles.analysisValue
                        }
                      >
                        {value === null
                          ? t.noData
                          : formatNumber(
                              value,
                              {
                                maximumFractionDigits: 2,
                              }
                            )}
                      </div>

                      <p>
                        {data
                          ? t.available
                          : t.noData}
                      </p>
                    </article>
                  );
                }
              )}
            </div>
          </section>
        </>
      )}
    </main>
  );
}

export default FinancialAnalysis;