import { useCallback, useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";

import { useLanguage } from "../../../../context/LanguageContext";
import { hasPermission } from "../../../../services/permissionService";
import {
  getFinancialDashboard,
  getFinancialPeriods,
} from "../../../../services/financialAnalysisService";

import Icon from "../../../../components/UI/Icon/Icon";
import styles from "./FinancialHome.module.css";

const STATUS_NAMES = {
  0: "Draft",
  1: "Uploading",
  2: "Ready",
  3: "Approved",
};

function unwrap(response) {
  return response?.data ?? response;
}

function periodId(period) {
  return (
    period?.id ??
    period?.financialPeriodId ??
    ""
  );
}

function normalizeStatus(status) {
  if (
    status === null ||
    status === undefined
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

function FinancialHome() {
  const navigate = useNavigate();

  const {
    translations,
    formatNumber,
  } = useLanguage();

  const t = translations.financialModule;

  const [periods, setPeriods] =
    useState([]);

  const [dashboard, setDashboard] =
    useState(null);

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  const latestPeriod =
    useMemo(() => {
      if (!periods.length) {
        return null;
      }

      return [...periods].sort(
        (a, b) =>
          Number(periodId(b)) -
          Number(periodId(a))
      )[0];
    }, [periods]);

  const loadData =
    useCallback(async () => {
      setLoading(true);
      setError("");

      try {
        const periodsResponse =
          await getFinancialPeriods();

        const list =
          unwrap(periodsResponse) || [];

        const normalized =
          Array.isArray(list)
            ? list
            : [];

        setPeriods(normalized);

        if (normalized.length > 0) {
          const current =
            [...normalized].sort(
              (a, b) =>
                Number(periodId(b)) -
                Number(periodId(a))
            )[0];

          const dashboardResponse =
            await getFinancialDashboard({
              periodId: periodId(current),
              branchId: "all",
              sections: "sales,profit,cash,debt",
            });

          setDashboard(
            unwrap(dashboardResponse)
          );
        }
      } catch (err) {
        setError(
          err?.message ||
          t.loadError
        );
      } finally {
        setLoading(false);
      }
    }, [t.loadError]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const status =
    normalizeStatus(
      latestPeriod?.status ??
      dashboard?.status
    );

  const quickActions = [
    {
      key: "dashboard",
      icon: "dashboard",
      path: "/financial-analysis/dashboard",
      permission: "FinancialDashboard.View",
      tone: "blue",
    },
    {
      key: "periods",
      icon: "reports",
      path: "/financial-analysis/periods",
      permission: "FinancialPeriod.View",
      tone: "amber",
    },
    {
      key: "uploads",
      icon: "plus",
      path: "/financial-analysis/uploads",
      permission: "FinancialPeriod.View",
      tone: "teal",
    },
  ].filter(
    (item) =>
      hasPermission(item.permission)
  );

  const groups = [
    {
      key: "incomeStatement",
      tone: "blue",
      items: [
        ["sales", "/financial-analysis/sales", "FinancialSales.View"],
        ["inventory", "/financial-analysis/inventory", "FinancialInventory.View"],
        ["expenses", "/financial-analysis/expenses", "FinancialExpenses.View"],
        ["purchases", "/financial-analysis/purchases", "FinancialPurchases.View"],
        ["profitLoss", "/financial-analysis/profit-loss", "FinancialProfitLoss.View"],
      ],
    },
    {
      key: "financialPosition",
      tone: "purple",
      items: [
        ["customersEmployees", "/financial-analysis/customers-employees", "FinancialCustomers.View"],
        ["debt", "/financial-analysis/debt", "FinancialDebt.View"],
        ["cash", "/financial-analysis/cash", "FinancialCash.View"],
        ["suppliers", "/financial-analysis/suppliers", "FinancialSuppliers.View"],
        ["capital", "/financial-analysis/capital", "FinancialCapital.View"],
      ],
    },
    {
      key: "movements",
      tone: "teal",
      items: [
        ["transfers", "/financial-analysis/transfers", "FinancialTransfers.View"],
        ["settlements", "/financial-analysis/settlements", "FinancialSettlements.View"],
      ],
    },
  ];

  return (
    <main className={styles.page}>
      <header className={styles.header}>
        <div>
          <span className={styles.eyebrow}>
            {t.eyebrow}
          </span>

          <h1>
            {t.homeTitle}
          </h1>

          <p>
            {t.homeDescription}
          </p>
        </div>

        <button
          type="button"
          className={styles.refreshButton}
          onClick={loadData}
          disabled={loading}
        >
          {loading
            ? t.loading
            : t.refresh}
        </button>
      </header>

      {error && (
        <div className={styles.error}>
          {error}
        </div>
      )}

      <section className={styles.snapshot}>
        <article className={`${styles.metricCard} ${styles.blueMetric}`}>
          <span>{t.currentPeriod}</span>

          <strong>
            {latestPeriod
              ? `#${periodId(latestPeriod)}`
              : "—"}
          </strong>

          <small>
            {latestPeriod?.startDate
              ? new Date(latestPeriod.startDate).toLocaleDateString("en-US")
              : "—"}
            {" → "}
            {latestPeriod?.endDate
              ? new Date(latestPeriod.endDate).toLocaleDateString("en-US")
              : "—"}
          </small>
        </article>

        <article className={`${styles.metricCard} ${styles.greenMetric}`}>
          <span>{t.status}</span>

          <strong>
            {t.statuses?.[status] ||
              status ||
              "—"}
          </strong>

          <small>
            {t.periodStatus}
          </small>
        </article>

        <article className={`${styles.metricCard} ${styles.purpleMetric}`}>
          <span>{t.activeBranches}</span>

          <strong>
            {formatNumber(
              dashboard?.activeBranches ?? 0
            )}
          </strong>

          <small>
            {t.financialBranches}
          </small>
        </article>

        <article className={`${styles.metricCard} ${styles.amberMetric}`}>
          <span>{t.fileHealth}</span>

          <strong>
            {formatNumber(
              dashboard?.validFiles ?? 0
            )}
          </strong>

          <small>
            {t.validFilesLabel} · {formatNumber(
              dashboard?.missingFiles ?? 0
            )} {t.missingFilesLabel}
          </small>
        </article>
      </section>

      <section className={styles.contentGrid}>
        <div className={styles.quickPanel}>
          <div className={styles.sectionHeading}>
            <div>
              <h2>
                {t.quickActions}
              </h2>

              <p>
                {t.quickActionsDescription}
              </p>
            </div>
          </div>

          <div className={styles.quickGrid}>
            {quickActions.map(
              (action) => (
                <button
                  key={action.key}
                  type="button"
                  className={`${styles.quickAction} ${styles[action.tone]}`}
                  onClick={() =>
                    navigate(action.path)
                  }
                >
                  <span className={styles.quickIcon}>
                    <Icon
                      name={action.icon}
                      size={21}
                    />
                  </span>

                  <span>
                    <strong>
                      {t.quick[action.key]}
                    </strong>

                    <small>
                      {t.quickDescriptions[action.key]}
                    </small>
                  </span>
                </button>
              )
            )}
          </div>
        </div>

        <aside className={styles.periodPanel}>
          <span className={styles.panelLabel}>
            {t.dataHealth}
          </span>

          <div className={styles.healthRow}>
            <span>{t.expectedFiles}</span>
            <strong>
              {formatNumber(
                dashboard?.expectedFiles ?? 0
              )}
            </strong>
          </div>

          <div className={styles.healthRow}>
            <span>{t.validFiles}</span>
            <strong className={styles.good}>
              {formatNumber(
                dashboard?.validFiles ?? 0
              )}
            </strong>
          </div>

          <div className={styles.healthRow}>
            <span>{t.missingFiles}</span>
            <strong className={styles.warning}>
              {formatNumber(
                dashboard?.missingFiles ?? 0
              )}
            </strong>
          </div>
        </aside>
      </section>

      <section className={styles.analysisSection}>
        <div className={styles.sectionHeading}>
          <div>
            <h2>
              {t.analysisAreas}
            </h2>

            <p>
              {t.analysisAreasDescription}
            </p>
          </div>
        </div>

        <div className={styles.groupGrid}>
          {groups.map((group) => {
            const visibleItems =
              group.items.filter(
                ([, , permission]) =>
                  hasPermission(permission)
              );

            if (!visibleItems.length) {
              return null;
            }

            return (
              <article
                key={group.key}
                className={`${styles.groupCard} ${styles[group.tone]}`}
              >
                <div className={styles.groupTop}>
                  <span className={styles.groupMarker} />

                  <div>
                    <h3>
                      {t.groups[group.key]}
                    </h3>

                    <p>
                      {t.groupDescriptions[group.key]}
                    </p>
                  </div>
                </div>

                <div className={styles.groupLinks}>
                  {visibleItems.map(
                    ([key, path]) => (
                      <button
                        key={key}
                        type="button"
                        onClick={() =>
                          navigate(path)
                        }
                      >
                        <span>
                          {t.sections[key]}
                        </span>

                        <span className={styles.linkArrow}>
                          →
                        </span>
                      </button>
                    )
                  )}
                </div>
              </article>
            );
          })}
        </div>
      </section>
    </main>
  );
}

export default FinancialHome;