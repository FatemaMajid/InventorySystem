import {
    useCallback,
    useEffect,
    useState,
} from "react";

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
    const { translations, direction } =
        useLanguage();

    const { activeSession } =
        useInventorySession();

    const t =
        translations.dashboard.comparisonResults;

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

    const sessionId =
        activeSession?.id;

    const loadComparison =
        useCallback(async () => {
            if (!sessionId) {
                setItems([]);
                setTotalItems(0);
                setError("");
                setLoading(false);

                return;
            }

            try {
                setLoading(true);
                setError("");

                const response =
                    await getInventoryComparison({
                        sessionId,
                        pageNumber: currentPage,
                        pageSize,

                        itemCode:
                            filters.search,

                        itemName: "",

                        status:
                            filters.status === "All"
                                ? ""
                                : filters.status,

                        sortBy: "ItemCode",
                        descending: false,
                    });

                const data =
                    response?.data ??
                    response;

                setItems(
                    data?.items ?? []
                );

                setTotalItems(
                    Number(
                        data?.totalCount ?? 0
                    )
                );
            } catch (err) {
                console.error(
                    "Failed to load comparison results:",
                    err
                );

                setItems([]);
                setTotalItems(0);

                setError(
                    err?.message ||
                        t.loadError ||
                        ""
                );
            } finally {
                setLoading(false);
            }
        }, [
            sessionId,
            currentPage,
            pageSize,
            filters.search,
            filters.status,
            t.loadError,
        ]);

    useEffect(() => {
        loadComparison();
    }, [loadComparison]);

    useEffect(() => {
        if (!sessionId) {
            setCurrentPage(1);
            setItems([]);
            setTotalItems(0);
            setError("");
        }
    }, [sessionId]);

    const totalPages = Math.max(
        1,
        Math.ceil(
            totalItems / pageSize
        )
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
        setFilters({
            ...EMPTY_FILTERS,
        });

        setCurrentPage(1);
    };

    const handlePageChange = (
        page
    ) => {
        if (
            page < 1 ||
            page > totalPages ||
            page === currentPage
        ) {
            return;
        }

        setCurrentPage(page);
    };

    const handlePageSizeChange = (
        size
    ) => {
        setPageSize(size);
        setCurrentPage(1);
    };

    if (!activeSession) {
        return (
            <main
                className={styles.page}
                dir={direction}
            >
                <div
                    className={
                        styles.emptySession
                    }
                >
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
                onChange={
                    handleFilterChange
                }
                onClear={
                    handleClearFilters
                }
            />

            <ComparisonTable
                items={items}
                loading={loading}
                error={error}
                onRetry={loadComparison}
                page={currentPage}
                pageSize={pageSize}
            />

            {!loading &&
                !error &&
                totalItems > 0 && (
                    <Pagination
                        currentPage={
                            currentPage
                        }
                        totalPages={
                            totalPages
                        }
                        pageSize={pageSize}
                        totalItems={
                            totalItems
                        }
                        onPageChange={
                            handlePageChange
                        }
                        onPageSizeChange={
                            handlePageSizeChange
                        }
                    />
                )}
        </main>
    );
}

export default ComparisonResults;