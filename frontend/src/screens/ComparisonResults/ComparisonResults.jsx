import { useEffect, useState } from "react";

import { useLanguage } from "../../context/LanguageContext";
import { useInventorySession } from "../../context/InventorySessionContext";

import {
  getInventoryComparison,
} from "../../services/dashboardService";

import ComparisonHeader from "../../components/ComparisonResults/ComparisonHeader/ComparisonHeader";
import ComparisonSummary from "../../components/ComparisonResults/ComparisonSummary/ComparisonSummary";
import ComparisonFilters from "../../components/ComparisonResults/ComparisonFilters/ComparisonFilters";
import ComparisonTable from "../../components/ComparisonResults/ComparisonTable/ComparisonTable";

import Pagination from "../../components/UI/Pagination/Pagination";

import styles from "./ComparisonResults.module.css";

const DEFAULT_PAGE_SIZE = 20;

const EMPTY_FILTERS = {
  search: "",
  status: "All",
};

function ComparisonResults() {
  const { translations, direction } = useLanguage();
  const { activeSession } = useInventorySession();

  const t = translations.dashboard.comparisonResults;

  const [items, setItems] = useState([]);

  const [filters, setFilters] =
    useState(EMPTY_FILTERS);

  const [currentPage, setCurrentPage] =
    useState(1);

  const [pageSize, setPageSize] =
    useState(DEFAULT_PAGE_SIZE);

  const [totalItems, setTotalItems] =
    useState(0);

  const [loading, setLoading] =
    useState(false);

  const [error, setError] =
    useState("");

  const sessionId = activeSession?.id;

  useEffect(() => {
    if (!sessionId) {
      setItems([]);
      setTotalItems(0);
      return;
    }

    let cancelled = false;

    const loadComparison = async () => {
      try {
        setLoading(true);
        setError("");

        const response =
          await getInventoryComparison({
            sessionId,
            pageNumber: currentPage,
            pageSize,

            /*
             * The current API exposes
             * itemCode and itemName separately.
             *
             * Search is currently sent as itemCode.
             */
            itemCode: filters.search,
            itemName: "",

            status:
              filters.status === "All"
                ? ""
                : filters.status,

            sortBy: "ItemCode",
            descending: false,
          });

        if (cancelled) {
          return;
        }

        setItems(response?.items ?? []);

        setTotalItems(
          Number(response?.totalCount ?? 0)
        );
      } catch (err) {
        if (cancelled) {
          return;
        }

        console.error(
          "Failed to load comparison results:",
          err
        );

        setItems([]);
        setTotalItems(0);

        setError(
          err?.message ||
            t.loadError ||
            "Failed to load comparison results."
        );
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    };

    loadComparison();

    return () => {
      cancelled = true;
    };
  }, [
    sessionId,
    currentPage,
    pageSize,
    filters.search,
    filters.status,
    t.loadError,
  ]);

  const totalPages = Math.max(
    1,
    Math.ceil(totalItems / pageSize)
  );

  const handleFilterChange = (
    field,
    value
  ) => {
    setFilters((previous) => ({
      ...previous,
      [field]: value,
    }));

    setCurrentPage(1);
  };

  const handleClearFilters = () => {
    setFilters(EMPTY_FILTERS);
    setCurrentPage(1);
  };

  const handlePageChange = (page) => {
    if (
      page < 1 ||
      page > totalPages ||
      page === currentPage
    ) {
      return;
    }

    setCurrentPage(page);
  };

  const handlePageSizeChange = (size) => {
    setPageSize(size);
    setCurrentPage(1);
  };

  if (!activeSession) {
    return (
      <main
        className={styles.page}
        dir={direction}
      >
        <div className={styles.emptySession}>
          {t.noActiveSession}
        </div>
      </main>
    );
  }

  return (
    <main
      className={styles.page}
      dir={direction}
    >
      <ComparisonHeader />

      <ComparisonSummary />

      <ComparisonFilters
        filters={filters}
        onChange={handleFilterChange}
        onClear={handleClearFilters}
      />

      <ComparisonTable
        items={items}
        loading={loading}
        error={error}
        page={currentPage}
        pageSize={pageSize}
      />

      {!loading &&
        !error &&
        totalItems > 0 && (
          <Pagination
            currentPage={currentPage}
            totalPages={totalPages}
            pageSize={pageSize}
            totalItems={totalItems}
            onPageChange={handlePageChange}
            onPageSizeChange={
              handlePageSizeChange
            }
          />
        )}
    </main>
  );
}

export default ComparisonResults;