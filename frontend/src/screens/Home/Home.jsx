import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import HomeHero from "../../components/HomeComponents/HomeHero/HomeHero";
import QuickActions from "../../components/HomeComponents/QuickActions/QuickActions";
import RecentSessions from "../../components/HomeComponents/RecentSessions/RecentSessions";
import Overview from "../../components/HomeComponents/Overview/Overview";
import SystemStatus from "../../components/HomeComponents/SystemStatus/SystemStatus";
import { getHomeDashboard } from "../../services/homeService";
import { hasPermission } from "../../services/permissionService";
import { getCurrentUser } from "../../services/authService";
import { useInventorySession } from "../../context/InventorySessionContext";
import styles from "./Home.module.css";

function Home() {
    const navigate = useNavigate();
    const { setActiveSession } = useInventorySession();
    const [dashboard, setDashboard] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const canViewDashboard = hasPermission("Dashboard.View");
    const canCreateInventory = hasPermission("InventorySession.Create");
    const canViewInventorySessions = hasPermission("InventorySession.View");
    const canViewReports = hasPermission("Report.View");

    const currentUser = getCurrentUser();
    const canViewOverview =
        currentUser?.role?.toLowerCase() === "admin" ||
        currentUser?.role?.toLowerCase() === "manager";

    useEffect(() => {
        if (!canViewDashboard) {
            setLoading(false);
            return;
        }

        let mounted = true;

        async function loadDashboard() {
            try {
                setLoading(true);
                setError(null);

                const response = await getHomeDashboard();

                if (!mounted) return;

                setDashboard(response);
            } catch (err) {
                console.error("Failed to load home dashboard:", err);

                if (!mounted) return;

                setError(err);
            } finally {
                if (mounted) {
                    setLoading(false);
                }
            }
        }

        loadDashboard();

        return () => {
            mounted = false;
        };
    }, [canViewDashboard]);

    const statistics = dashboard?.statistics ?? {};

    const recentSessions = (dashboard?.recentSessions ?? []).map((session) => ({
        ...session,
        id: session.id,
        sessionId: session.sessionNumber,
        date: session.inventoryDate,
        branch: session.branchName,
        store: session.storeName,
        status: session.status,
        items: session.totalItems,
    }));

    const overview = dashboard?.overview ?? {};
    const systemStatus = dashboard?.systemStatus ?? {};

    const handleViewSession = (session) => {
        if (!session?.id) return;

        setActiveSession(session);
        navigate("/dashboard");
    };

    return (
        <section className={styles.home}>
            {canViewDashboard && (
                <HomeHero
                    activeSessions={statistics.activeSessions ?? 0}
                    totalSessions={statistics.totalSessions ?? 0}
                    attentionItems={statistics.attentionItems ?? 0}
                    canCreateInventory={canCreateInventory}
                />
            )}

            <QuickActions
                canCreateInventory={canCreateInventory}
                canViewInventorySessions={canViewInventorySessions}
                canViewReports={canViewReports}
            />

            <div className={styles.bottomGrid}>
                {canViewInventorySessions && (
                    <RecentSessions
                        sessions={recentSessions}
                        loading={loading}
                        error={error}
                        onViewAll={() => navigate("/inventory-sessions")}
                        onViewSession={handleViewSession}
                    />
                )}

                {canViewOverview && (
                    <Overview
                        branches={overview.branches ?? 0}
                        stores={overview.stores ?? 0}
                        itemCategories={overview.categories ?? 0}
                        items={overview.items ?? 0}
                    />
                )}
            </div>

            {canViewDashboard && (
                <SystemStatus
                    api={systemStatus.api}
                    database={systemStatus.database}
                    health={systemStatus.health}
                />
            )}
        </section>
    );
}

export default Home;