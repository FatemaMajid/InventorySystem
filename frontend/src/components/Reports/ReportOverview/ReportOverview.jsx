import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";

import styles from "./ReportOverview.module.css";

function ReportOverview({ data, loading = false }) {
  const { translations, language, direction } = useLanguage();
  const t = translations.reports;

  if (loading || !data) return null;

  const statuses = data.statuses ?? [];
  const topDifferences = data.topValueDifferences ?? [];

  const formatNumber = (value) =>
    Number(value ?? 0).toLocaleString(language === "ar" ? "ar-IQ" : "en-US");

  return (
    <section className={styles.grid} dir={direction}>
      <article className={styles.card}>
        <div className={styles.cardHeader}>
          <div>
            <h2>{t.statusOverview.title}</h2>
            <p>{t.statusOverview.description}</p>
          </div>
          <Icon name="comparison" size={20} />
        </div>

        <div className={styles.statusList}>
          {statuses.map((status) => (
            <div key={status.status} className={styles.statusRow}>
              <span>{t.statuses[status.status] || status.status}</span>
              <strong>{formatNumber(status.count)}</strong>
              <span className={styles.percentage}>{Number(status.percentage ?? 0).toFixed(1)}%</span>
            </div>
          ))}
        </div>
      </article>

      <article className={styles.card}>
        <div className={styles.cardHeader}>
          <div>
            <h2>{t.topDifferences.title}</h2>
            <p>{t.topDifferences.description}</p>
          </div>
          <Icon name="priceChange" size={20} />
        </div>

        <div className={styles.differenceList}>
          {topDifferences.length === 0 ? (
            <span className={styles.empty}>{t.topDifferences.empty}</span>
          ) : (
            topDifferences.map((item) => (
              <div key={item.itemCode} className={styles.differenceRow}>
                <div>
                  <strong>{item.itemCode}</strong>
                  <span>{item.itemName || "—"}</span>
                </div>
                <b className={item.valueDifference < 0 ? styles.negative : styles.positive}>
                  {formatNumber(item.valueDifference)}
                </b>
              </div>
            ))
          )}
        </div>
      </article>
    </section>
  );
}

export default ReportOverview;
