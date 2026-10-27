import { useLanguage } from "../../../context/LanguageContext";
import styles from "./StatusChart.module.css";

function StatusChart({
  increase = 0,
  decrease = 0,
  match = 0,
  newlyCounted = 0,
  fullyDepleted = 0,
}) {
  const { translations, language } = useLanguage();

  const t = translations.dashboard.kpiCards;

  const statusColors = {
    increase: "var(--color-success)",
    decrease: "var(--color-danger)",
    match: "var(--color-primary)",
    newlyCounted: "var(--color-warning)",
    fullyDepleted: "var(--color-purple)",
  };

  const items = [
    {
      key: "increase",
      value: Number(increase || 0),
      label: t.increase,
    },
    {
      key: "decrease",
      value: Number(decrease || 0),
      label: t.decrease,
    },
    {
      key: "match",
      value: Number(match || 0),
      label: t.match,
    },
    {
      key: "newlyCounted",
      value: Number(newlyCounted || 0),
      label: t.newlyCounted,
    },
    {
      key: "fullyDepleted",
      value: Number(fullyDepleted || 0),
      label: t.fullyDepleted,
    },
  ];

  const total = items.reduce(
    (sum, item) => sum + item.value,
    0
  );

  let current = 0;

  const segments = items.map((item) => {
    const percentage =
      total > 0 ? (item.value / total) * 100 : 0;

    const start = current;
    current += percentage;

    return {
      ...item,
      percentage,
      start,
      end: current,
    };
  });

  const gradient =
    total > 0
      ? `conic-gradient(${segments
          .map(
            (item) =>
              `${statusColors[item.key]} ${item.start}% ${item.end}%`
          )
          .join(", ")})`
      : "conic-gradient(var(--color-border) 0% 100%)";

  const formatNumber = (value) => Number(value || 0).toLocaleString("en-US");

  return (
    <section className={styles.wrapper}>
      <h2 className={styles.title}>
        {t.title}
      </h2>

      <div className={styles.content}>
        <div className={styles.chartContainer}>
          <div
            className={styles.donut}
            style={{ background: gradient }}
          >
            <div className={styles.donutInner}>
              <strong className={styles.total}>
                {formatNumber(total)}
              </strong>

              <span className={styles.totalLabel}>
                {t.total}
              </span>
            </div>
          </div>
        </div>

        <div className={styles.legend}>
          {segments.map((item) => (
            <div
              key={item.key}
              className={styles.legendItem}
            >
              <span
                className={styles.legendDot}
                style={{
                  backgroundColor:
                    statusColors[item.key],
                }}
              />

              <span className={styles.legendLabel}>
                {item.label}
              </span>

              <span className={styles.legendValue}>
                {formatNumber(item.value)}
              </span>

              <span className={styles.legendPercentage}>
                {item.percentage.toFixed(2)}%
              </span>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}

export default StatusChart;