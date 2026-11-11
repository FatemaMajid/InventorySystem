import { useCallback, useEffect, useState } from "react";

import { useLanguage } from "../../../../context/useLanguage";
import { useInventorySession } from "../../../../context/useInventorySession";

import { getInventoryDashboard, getInventoryComparison } from "../../services/dashboardService";
import { exportReport } from "../../services/reportService";

import ReportHeader from "../../components/Reports/ReportHeader/ReportHeader";
import ReportSession from "../../components/Reports/ReportSession/ReportSession";
import ReportSummary from "../../components/Reports/ReportSummary/ReportSummary";
import ReportOverview from "../../components/Reports/ReportOverview/ReportOverview";
import ReportActions from "../../components/Reports/ReportActions/ReportActions";
import ReportTabs from "../../components/Reports/ReportTabs/ReportTabs";

import ComparisonFilters from "../../components/ComparisonResults/ComparisonFilters/ComparisonFilters";
import ComparisonTable from "../../components/ComparisonResults/ComparisonTable/ComparisonTable";
import Pagination from "../../../../components/UI/Pagination/Pagination";

import ErrorState from "../../../../components/UI/ErrorState/ErrorState";

import styles from "./Reports.module.css";

const DEFAULT_PAGE_SIZE = 20;
const EMPTY_FILTERS = { search: "", status: "All" };

function ReportsContent({ activeSession }) {
  const { translations, language, direction } = useLanguage();

  const t = translations.reports;
  const sessionId = activeSession?.id;

  const [activeTab, setActiveTab] = useState("overview");
  const [dashboard, setDashboard] = useState(null);
  const [comparison, setComparison] = useState([]);
  const [totalItems, setTotalItems] = useState(0);
  const [filters, setFilters] = useState(EMPTY_FILTERS);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [loadingDashboard, setLoadingDashboard] = useState(Boolean(sessionId));
  const [loadingComparison, setLoadingComparison] = useState(false);
  const [exporting, setExporting] = useState(false);
  const [error, setError] = useState("");
  const [comparisonError, setComparisonError] = useState("");

  const loadDashboard = useCallback(async () => {
    if (!sessionId) {
      setDashboard(null);
      return;
    }

    setLoadingDashboard(true);
    setError("");

    try {
      const response = await getInventoryDashboard(sessionId);
      setDashboard(response?.data ?? response);
    } catch (err) {
      setDashboard(null);
      setError(err?.message || t.error);
    } finally {
      setLoadingDashboard(false);
    }
  }, [sessionId, t.error]);

  const loadComparison = useCallback(async () => {
    if (!sessionId) {
      setComparison([]);
      setTotalItems(0);
      return;
    }

    setLoadingComparison(true);
    setComparisonError("");

    try {
      const response = await getInventoryComparison({
        sessionId,
        pageNumber: currentPage,
        pageSize,
        itemCode: filters.search,
        status: filters.status === "All" ? "" : filters.status,
        sortBy: "ItemCode",
        descending: false,
      });

      const data = response?.data ?? response;
      setComparison(data?.items ?? []);
      setTotalItems(Number(data?.totalCount ?? 0));
    } catch (err) {
      setComparison([]);
      setTotalItems(0);
      setComparisonError(err?.message || t.error);
    } finally {
      setLoadingComparison(false);
    }
  }, [sessionId, currentPage, pageSize, filters.search, filters.status, t.error]);

  useEffect(() => {
    if (!sessionId) return;
    let active = true;

    getInventoryDashboard(sessionId)
      .then((response) => {
        if (!active) return;
        setDashboard(response?.data ?? response);
        setError("");
      })
      .catch((err) => {
        if (!active) return;
        setDashboard(null);
        setError(err?.message || t.error);
      })
      .finally(() => {
        if (active) setLoadingDashboard(false);
      });

    return () => { active = false; };
  }, [sessionId, t.error]);

  useEffect(() => {
    if (activeTab !== "comparison" || !sessionId) return;
    let active = true;

    getInventoryComparison({
      sessionId,
      pageNumber: currentPage,
      pageSize,
      itemCode: filters.search,
      status: filters.status === "All" ? "" : filters.status,
      sortBy: "ItemCode",
      descending: false,
    })
      .then((response) => {
        if (!active) return;
        const data = response?.data ?? response;
        setComparison(data?.items ?? []);
        setTotalItems(Number(data?.totalCount ?? 0));
        setComparisonError("");
      })
      .catch((err) => {
        if (!active) return;
        setComparison([]);
        setTotalItems(0);
        setComparisonError(err?.message || t.error);
      })
      .finally(() => {
        if (active) setLoadingComparison(false);
      });

    return () => { active = false; };
  }, [activeTab, sessionId, currentPage, pageSize, filters.search, filters.status, t.error]);

  const handleFilterChange = (field, value) => {
    setLoadingComparison(true);
    setFilters((current) => ({ ...current, [field]: value }));
    setCurrentPage(1);
  };

  const handleClearFilters = () => {
    setLoadingComparison(true);
    setFilters(EMPTY_FILTERS);
    setCurrentPage(1);
  };

  const handleExport = async (format) => {
    if (!sessionId || exporting) return;

    setExporting(true);

    try {
      await exportReport(sessionId, format, language);
    } catch (err) {
      setError(err?.message || t.exportError);
    } finally {
      setExporting(false);
    }
  };

  if (!activeSession) {
    return (
      <main className={styles.page} dir={direction}>
        <ReportHeader />
        <div className={styles.emptySession}>
          {t.noActiveSession}
        </div>
      </main>
    );
  }

  const totalPages = Math.max(1, Math.ceil(totalItems / pageSize));

  return (
    <main className={styles.page} dir={direction}>
      <div className={styles.topRow}>
        <ReportHeader />
        <ReportActions loading={exporting} onExport={handleExport} />
      </div>

      <ReportSession data={dashboard} />

      <ReportSummary data={dashboard} loading={loadingDashboard} />

      {error && !loadingDashboard && (
        <div className={styles.error}>
          <ErrorState message={error} onRetry={loadDashboard} />
        </div>
      )}

      <ReportTabs activeTab={activeTab} onChange={(value) => { if (value === "comparison") setLoadingComparison(true); setActiveTab(value); }} />

      {activeTab === "overview" ? (
        <ReportOverview data={dashboard} loading={loadingDashboard} />
      ) : (
        <section className={styles.comparisonSection}>
          <ComparisonFilters
            filters={filters}
            onChange={handleFilterChange}
            onClear={handleClearFilters}
          />

          <ComparisonTable
            items={comparison}
            loading={loadingComparison}
            error={comparisonError}
            onRetry={loadComparison}
            page={currentPage}
            pageSize={pageSize}
          />

          {!loadingComparison && !comparisonError && totalItems > 0 && (
            <Pagination
              currentPage={currentPage}
              totalPages={totalPages}
              pageSize={pageSize}
              totalItems={totalItems}
              onPageChange={(value) => { setLoadingComparison(true); setCurrentPage(value); }}
              onPageSizeChange={(size) => {
                setPageSize(Number(size));
                setCurrentPage(1);
              }}
            />
          )}
        </section>
      )}
    </main>
  );
}

function Reports() {
  const { activeSession } = useInventorySession();
  return <ReportsContent key={activeSession?.id ?? "no-session"} activeSession={activeSession} />;
}

export default Reports;
