import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

import HomeHero from "../../components/HomeComponents/HomeHero/HomeHero";
import QuickActions from "../../components/HomeComponents/QuickActions/QuickActions";
import RecentSessions from "../../components/HomeComponents/RecentSessions/RecentSessions";
import Overview from "../../components/HomeComponents/Overview/Overview";
import SystemStatus from "../../components/HomeComponents/SystemStatus/SystemStatus";

import { getHomeDashboard } from "../../services/homeService";

import styles from "./Home.module.css";

function Home() {
  const navigate = useNavigate();

  const [dashboard, setDashboard] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
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
  }, []);

  const statistics = dashboard?.statistics ?? {};

  const recentSessions = (
    dashboard?.recentSessions ?? []
  ).map((session) => ({
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

  return (
    <section className={styles.home}>
      <HomeHero
        activeSessions={statistics.activeSessions ?? 0}
        totalSessions={statistics.totalSessions ?? 0}
        attentionItems={statistics.attentionItems ?? 0}
      />

      <QuickActions />

      <div className={styles.bottomGrid}>
        <RecentSessions
          sessions={recentSessions}
          loading={loading}
          error={error}
          onViewAll={() =>
            navigate("/inventory-sessions")
          }
          onViewSession={(session) =>
            navigate(
              `/inventory-sessions/${session.id}`
            )
          }
        />

        <Overview
          branches={overview.branches ?? 0}
          stores={overview.stores ?? 0}
          itemCategories={overview.categories ?? 0}
          items={overview.items ?? 0}
        />
      </div>

      <SystemStatus
        api={systemStatus.api}
        database={systemStatus.database}
        health={systemStatus.health}
      />
    </section>
  );
}

export default Home;