import { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useLanguage } from "../../context/LanguageContext";
import { useInventorySession } from "../../context/InventorySessionContext";
import DashboardHeader from "../../components/Dashboard/DashboardHeader/DashboardHeader";
import KpiCard from "../../components/Dashboard/KpiCard/KpiCard";
import FinancialSummary from "../../components/Dashboard/FinancialSummary/FinancialSummary";
import StatusChart from "../../components/Dashboard/StatusChart/StatusChart";
import ValueChart from "../../components/Dashboard/ValueChart/ValueChart";
import TopDifferences from "../../components/Dashboard/TopDifferences/TopDifferences";
import AttentionSummary from "../../components/Dashboard/AttentionSummary/AttentionSummary";
import ComparisonResults from "../../components/Dashboard/ComparisonResults/ComparisonResults";
import LoadingState from "../../components/UI/Loading/LoadingState/LoadingState";
import ErrorState from "../../components/UI/ErrorState/ErrorState";
import Icon from "../../components/UI/Icon/Icon";
import InventoryQuantitySummary from "../../components/Dashboard/InventoryQuantitySummary/InventoryQuantitySummary";
import {
    getInventoryDashboard,
    exportDashboardExcel,
    exportDashboardPdf,
} from "../../services/dashboardService";
import styles from "./Dashboard.module.css";

function Dashboard() {
    const navigate = useNavigate();
    const { translations, language } = useLanguage();
    const { activeSession } = useInventorySession();
    const sessionId = activeSession?.id;
    const t = translations.dashboard;
    const [dashboardData, setDashboardData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [exporting, setExporting] = useState("");

    const loadDashboard = useCallback(async () => {
        if (!sessionId) {
            setDashboardData(null);
            setLoading(false);
            setError("");
            return;
        }

        try {
            setLoading(true);
            setError("");
            const response = await getInventoryDashboard(sessionId);
            setDashboardData(response?.data ?? response);
        } catch (err) {
            console.error("Failed to load dashboard:", err);
            setDashboardData(null);
            setError(
                err?.message ||
                t.errors?.loadDashboard ||
                translations.common.error
            );
        } finally {
            setLoading(false);
        }
    }, [sessionId, t.errors, translations.common.error]);

    useEffect(() => {
        loadDashboard();
    }, [loadDashboard]);

    const downloadFile = (blob, fileName) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement("a");

        link.href = url;
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        link.remove();
        window.URL.revokeObjectURL(url);
    };

    const handleExportExcel = async () => {
        if (!sessionId || exporting) return;

        try {
            setExporting("excel");

            const file = await exportDashboardExcel(
                sessionId,
                language
            );

            downloadFile(
                file,
                language === "ar"
                    ? `تقرير_الجرد_${sessionId}.xlsx`
                    : `Inventory_Report_${sessionId}.xlsx`
            );
        } catch (err) {
            console.error("Failed to export Excel:", err);
            setError(
                err?.message ||
                t.errors?.exportExcel ||
                translations.common.error
            );
        } finally {
            setExporting("");
        }
    };

    const handleExportPdf = async () => {
        if (!sessionId || exporting) return;

        try {
            setExporting("pdf");

            const file = await exportDashboardPdf(
                sessionId,
                language
            );

            downloadFile(
                file,
                language === "ar"
                    ? `تقرير_الجرد_${sessionId}.pdf`
                    : `Inventory_Report_${sessionId}.pdf`
            );
        } catch (err) {
            console.error("Failed to export PDF:", err);
            setError(
                err?.message ||
                t.errors?.exportPdf ||
                translations.common.error
            );
        } finally {
            setExporting("");
        }
    };

    const handleAttentionView = (attentionType) => {
        if (!attentionType) {
            navigate("/attention-items");
            return;
        }

        navigate(
            `/attention-items?attentionType=${encodeURIComponent(attentionType)}`
        );
    };

    if (loading) {
        return (
            <main className={styles.dashboard}>
                <LoadingState />
            </main>
        );
    }

    if (error) {
        return (
            <main className={styles.dashboard}>
                <ErrorState
                    message={error}
                    onRetry={loadDashboard}
                />
            </main>
        );
    }

    if (!dashboardData) {
        return (
            <main className={styles.dashboard}>
                <section className={styles.emptyState}>
                    <div className={styles.emptyStateIcon}>
                        <Icon name="inventory" size={30} />
                    </div>
                    <h1>{t.emptyState?.title}</h1>
                    <p>{t.emptyState?.description}</p>
                    <button
                        type="button"
                        className={styles.emptyStateButton}
                        onClick={() => navigate("/inventory-sessions")}
                    >
                        <Icon name="inventory" size={18} />
                        <span>{t.emptyState?.selectSession}</span>
                    </button>
                </section>
            </main>
        );
    }

    const {
        session,
        summary,
        inventoryQuantity,
        financial,
        statuses,
        topValueDifferences,
        attention,
    } = dashboardData;

    const statusMap = Object.fromEntries(
        (statuses || []).map((item) => [
            item.status,
            item.count,
        ])
    );

    const cards = [
        {
            id: "total",
            key: "totalItems",
            value: summary.totalItems,
            icon: "totalItems",
            type: "total",
        },
        {
            id: "increase",
            key: "increase",
            value: summary.increase,
            icon: "increase",
            type: "increase",
        },
        {
            id: "decrease",
            key: "decrease",
            value: summary.decrease,
            icon: "decrease",
            type: "decrease",
        },
        {
            id: "match",
            key: "match",
            value: summary.match,
            icon: "tick",
            type: "match",
        },
        {
            id: "newly-counted",
            key: "newlyCounted",
            value: summary.newlyCounted,
            icon: "plus",
            type: "new",
        },
        {
            id: "fully-depleted",
            key: "fullyDepleted",
            value: summary.fullyDepleted,
            icon: "depleted",
            type: "depleted",
        },
        {
            id: "price-changed",
            key: "priceChanged",
            value: summary.priceChanged,
            icon: "priceChange",
            type: "price",
        },
        {
            id: "unit-not-defined",
            key: "unitNotDefined",
            value: summary.unitNotDefined,
            icon: "changeUnit",
            type: "unit",
        },
        {
            id: "quantity-before",
            key: "totalQuantityBefore",
            value: inventoryQuantity?.totalQuantityBefore,
            icon: "inventory",
            type: "total",
        },
        {
            id: "quantity-after",
            key: "totalQuantityAfter",
            value: inventoryQuantity?.totalQuantityAfter,
            icon: "inventory",
            type: "total",
        },
    ];

    return (
        <main className={styles.dashboard}>
            <DashboardHeader
                session={{
                    number: session.sessionNumber,
                    status: session.status,
                    type: session.inventoryType,
                    date: session.inventoryDate,
                    branch: session.branchName,
                    store: session.storeName,
                }}
                onExportExcel={handleExportExcel}
                onExportPdf={handleExportPdf}
            />

            <section className={styles.kpiSection}>
                <KpiCard
                    cards={cards}
                    loading={false}
                />
            </section>

            <section className={styles.inventoryQuantitySection}>
                <InventoryQuantitySummary
                    totalQuantityBefore={inventoryQuantity?.totalQuantityBefore}
                    totalQuantityAfter={inventoryQuantity?.totalQuantityAfter}
                    quantityDifference={inventoryQuantity?.quantityDifference}
                    quantityIncrease={inventoryQuantity?.quantityIncrease}
                    quantityDecrease={inventoryQuantity?.quantityDecrease}
                />
            </section>

            <section className={styles.financialSummarySection}>
                <FinancialSummary
                    totalValueBefore={financial.totalValueBefore}
                    totalValueAfter={financial.totalValueAfter}
                    totalDifference={financial.totalDifference}
                    differencePercentage={financial.differencePercentage}
                />
            </section>

            <section className={styles.chartSection}>
                <StatusChart
                    increase={
                        statusMap.Increase ??
                        summary.increase
                    }
                    decrease={
                        statusMap.Decrease ??
                        summary.decrease
                    }
                    match={
                        statusMap.Match ??
                        summary.match
                    }
                    newlyCounted={
                        statusMap.NewlyCounted ??
                        summary.newlyCounted
                    }
                    fullyDepleted={
                        statusMap.FullyDepleted ??
                        summary.fullyDepleted
                    }
                />

                <ValueChart
                    beforeValue={financial.totalValueBefore}
                    afterValue={financial.totalValueAfter}
                />

                <TopDifferences
                    items={topValueDifferences}
                />
            </section>

            <section className={styles.attentionSection}>
                <AttentionSummary
                    data={attention}
                    onViewAll={handleAttentionView}
                />
            </section>

            <section className={styles.ComparisonResults}>
                <ComparisonResults
                    sessionId={session.sessionId}
                />
            </section>
        </main>
    );
}

export default Dashboard;