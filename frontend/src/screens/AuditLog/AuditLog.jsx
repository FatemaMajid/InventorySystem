import { useCallback, useEffect, useState } from "react";
import auditLogService from "../../services/auditLogService";
import AuditLogHeader from "../../components/AuditLog/AuditLogHeader/AuditLogHeader";
import AuditLogStats from "../../components/AuditLog/AuditLogStats/AuditLogStats";
import AuditLogFilters from "../../components/AuditLog/AuditLogFilters/AuditLogFilters";
import AuditLogTable from "../../components/AuditLog/AuditLogTable/AuditLogTable";
import AuditLogDetails from "../../components/AuditLog/AuditLogDetails/AuditLogDetails";
import Pagination from "../../components/UI/Pagination/Pagination";
import styles from "./AuditLog.module.css";

const AuditLog = () => {
    const [logs, setLogs] = useState([]);
    const [stats, setStats] = useState(null);
    const [loading, setLoading] = useState(false);
    const [currentPage, setCurrentPage] = useState(1);
    const [pageSize, setPageSize] = useState(20);
    const [totalItems, setTotalItems] = useState(0);
    const [totalPages, setTotalPages] = useState(1);
    const [filters, setFilters] = useState({
        user: "",
        action: "",
        entity: "",
        dateFrom: "",
        dateTo: ""
    });
    const [selectedLog, setSelectedLog] = useState(null);

    const fetchLogs = useCallback(async () => {
        setLoading(true);

        try {
            const response = await auditLogService.getAll({
                pageNumber: currentPage,
                pageSize,
                userId: filters.user,
                action: filters.action,
                entity: filters.entity,
                dateFrom: filters.dateFrom,
                dateTo: filters.dateTo
            });

            const data = response?.data ?? response;
            const items = data?.items ?? data?.Items ?? [];
            const total = data?.totalCount ?? data?.TotalCount ?? 0;
            const pages =
                data?.totalPages ??
                data?.TotalPages ??
                Math.max(1, Math.ceil(total / pageSize));

            setLogs(items);
            setTotalItems(total);
            setTotalPages(pages);

            const today = new Date();

            setStats({
                total,
                today: items.filter((log) => {
                    if (!log.createdAt) {
                        return false;
                    }

                    const createdAt = new Date(log.createdAt);

                    return (
                        createdAt.getFullYear() === today.getFullYear() &&
                        createdAt.getMonth() === today.getMonth() &&
                        createdAt.getDate() === today.getDate()
                    );
                }).length,
                success: items.filter(
                    (log) => log.status === "Success"
                ).length,
                failed: items.filter(
                    (log) => log.status === "Failed"
                ).length
            });
        } catch (error) {
            console.error("Failed to load audit logs:", error);
            setLogs([]);
            setTotalItems(0);
            setTotalPages(1);
            setStats(null);
        } finally {
            setLoading(false);
        }
    }, [currentPage, pageSize, filters]);

    useEffect(() => {
        fetchLogs();
    }, [fetchLogs]);

    const handlePageChange = (page) => {
        if (page < 1 || page > totalPages) {
            return;
        }

        setCurrentPage(page);
    };

    const handlePageSizeChange = (size) => {
        setPageSize(Number(size));
        setCurrentPage(1);
    };

    const handleFilterChange = (key, value) => {
        setFilters((current) => ({
            ...current,
            [key]: value
        }));

        setCurrentPage(1);
    };

    const handleResetFilters = () => {
        setFilters({
            user: "",
            action: "",
            entity: "",
            dateFrom: "",
            dateTo: ""
        });

        setCurrentPage(1);
    };

    return (
        <main className={styles.page}>
            <AuditLogHeader />
            <AuditLogStats stats={stats} />
            <AuditLogFilters
                filters={filters}
                logs={logs}
                onChange={handleFilterChange}
                onReset={handleResetFilters}
            />
            <AuditLogTable
                logs={logs}
                loading={loading}
                onSelect={setSelectedLog}
            />
            {!loading && totalItems > 0 && (
                <Pagination
                    currentPage={currentPage}
                    totalPages={totalPages}
                    pageSize={pageSize}
                    totalItems={totalItems}
                    onPageChange={handlePageChange}
                    onPageSizeChange={handlePageSizeChange}
                />
            )}
            <AuditLogDetails
                log={selectedLog}
                onClose={() => setSelectedLog(null)}
            />
        </main>
    );
};

export default AuditLog;