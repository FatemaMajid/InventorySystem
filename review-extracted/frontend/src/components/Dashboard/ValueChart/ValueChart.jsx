import { useLanguage } from "../../../context/LanguageContext";
import styles from "./ValueChart.module.css";

function ValueChart({
  beforeValue = 0,
  afterValue = 0,
}) {
  const { translations } = useLanguage();

  const t = translations.dashboard.valueChart;

  const before = Number(beforeValue || 0);
  const after = Number(afterValue || 0);

  const maxValue = Math.max(before, after, 1);

  const beforeWidth = (before / maxValue) * 100;
  const afterWidth = (after / maxValue) * 100;

  const difference = after - before;

  const differencePercentage =
    before > 0 ? (difference / before) * 100 : 0;

  const isIncrease = difference >= 0;

  const formatNumber = (value) => Number(value || 0).toLocaleString("en-US");

  return (
    <section className={styles.wrapper}>
      <div className={styles.header}>
        <div className={styles.heading}>
          <h2 className={styles.title}>
            {t.title}
          </h2>

          <p className={styles.subtitle}>
            {t.subtitle}
          </p>
        </div>

        <div
          className={`${styles.difference} ${
            isIncrease
              ? styles.positive
              : styles.negative
          }`}
        >
          <span className={styles.differenceValue}>
            {isIncrease ? "+" : ""}
            {formatNumber(difference)}
          </span>

          <span className={styles.differencePercentage}>
            {isIncrease ? "+" : ""}
            {differencePercentage.toFixed(2)}%
          </span>
        </div>
      </div>

      <div className={styles.chart}>
        <div className={styles.row}>
          <div className={styles.rowHeader}>
            <span className={styles.label}>
              {t.before}
            </span>

            <strong className={styles.value}>
              {formatNumber(before)}
              <span className={styles.currency}>
                IQD
              </span>
            </strong>
          </div>

          <div className={styles.track}>
            <div
              className={`${styles.bar} ${styles.beforeBar}`}
              style={{ width: `${beforeWidth}%` }}
            />
          </div>
        </div>

        <div className={styles.row}>
          <div className={styles.rowHeader}>
            <span className={styles.label}>
              {t.after}
            </span>

            <strong className={styles.value}>
              {formatNumber(after)}
              <span className={styles.currency}>
                IQD
              </span>
            </strong>
          </div>

          <div className={styles.track}>
            <div
              className={`${styles.bar} ${styles.afterBar}`}
              style={{ width: `${afterWidth}%` }}
            />
          </div>
        </div>
      </div>
    </section>
  );
}

export default ValueChart;