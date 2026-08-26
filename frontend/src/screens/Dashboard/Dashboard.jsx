import { useEffect, useState } from "react";

import DashboardHeader from "../../components/Dashboard/DashboardHeader/DashboardHeader";
import KpiCard from "../../components/Dashboard/KpiCard/KpiCard";
import FinancialSummary from "../../components/Dashboard/FinancialSummary/FinancialSummary";
import StatusChart from "../../components/Dashboard/StatusChart/StatusChart";
import ValueChart from "../../components/Dashboard/ValueChart/ValueChart";
import TopDifferences from "../../components/Dashboard/TopDifferences/TopDifferences";
import AttentionSummary from "../../components/Dashboard/AttentionSummary/AttentionSummary";
import ComparisonResults from "../../components/Dashboard/ComparisonResults/ComparisonResults";

import { getInventoryDashboard } from "../../services/dashboardService";

import styles from "./Dashboard.module.css";

function Dashboard() {
  const sessionId = 5;

  const [dashboardData, setDashboardData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let cancelled = false;

    const loadDashboard = async () => {
      try {
        setLoading(true);
        setError("");

        const response = await getInventoryDashboard(sessionId);

        if (!cancelled) {
          setDashboardData(response);
        }
      } catch (err) {
        console.error("Failed to load dashboard:", err);

        if (!cancelled) {
          setError(
            err?.message || "Failed to load dashboard data."
          );
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    };

    loadDashboard();

    return () => {
      cancelled = true;
    };
  }, []);

  if (loading) {
    return (
      <main className={styles.dashboard}>
        <div>Loading...</div>
      </main>
    );
  }

  if (error) {
    return (
      <main className={styles.dashboard}>
        <div>{error}</div>
      </main>
    );
  }

  if (!dashboardData) {
    return null;
  }

  const { session, summary, financial, statuses, topValueDifferences, attention } =
    dashboardData;

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
      />

      <section className={styles.kpiSection}>
        <KpiCard
          cards={cards}
          loading={false}
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
          increase={statusMap.Increase || summary.increase}
          decrease={statusMap.Decrease || summary.decrease}
          match={statusMap.Match || summary.match}
          newlyCounted={
            statusMap.NewlyCounted || summary.newlyCounted
          }
          fullyDepleted={
            statusMap.FullyDepleted || summary.fullyDepleted
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
          data={dashboardData.attention}
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