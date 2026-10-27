import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import Skeleton from "../../UI/Loading/Skeleton/Skeleton";

import styles from "./ReportSummary.module.css";

function ReportSummary({ data, loading = false }) {
  const { translations, language, direction } = useLanguage();
  const t = translations.reports;
  const summary = data?.summary ?? {};
  const attention = data?.attention ?? {};

  const cards = [
    { key: "totalItems", value: summary.totalItems, icon: "totalItems", type: "total" },
    { key: "match", value: summary.match, icon: "tick", type: "match" },
    { key: "differences", value: Math.max(0, (summary.totalItems ?? 0) - (summary.match ?? 0)), icon: "comparison", type: "difference" },
    { key: "attention", value: attention.total, icon: "attention", type: "attention" },
  ];

  const formatNumber = (value) =>
    Number(value ?? 0).toLocaleString("en-US");

  return (
    <section className={styles.wrapper} dir={direction}>
      <div className={styles.cards}>
        {cards.map((card) => (
          <article key={card.key} className={`${styles.card} ${styles[card.type]}`}>
            <div className={styles.iconBox}>
              <Icon name={card.icon} size={20} />
            </div>

            <div className={styles.content}>
              <span>{t.summary[card.key]}</span>
              {loading ? (
                <Skeleton width="65px" height="28px" borderRadius="6px" />
              ) : (
                <strong>{formatNumber(card.value)}</strong>
              )}
            </div>
          </article>
        ))}
      </div>

      <div className={styles.financial}>
        <div>
          <span>{t.financial.before}</span>
          <strong>
            {loading ? <Skeleton width="90px" height="22px" /> : formatNumber(data?.financial?.totalValueBefore)}
          </strong>
        </div>
        <div>
          <span>{t.financial.after}</span>
          <strong>
            {loading ? <Skeleton width="90px" height="22px" /> : formatNumber(data?.financial?.totalValueAfter)}
          </strong>
        </div>
        <div>
          <span>{t.financial.difference}</span>
          <strong className={data?.financial?.totalDifference < 0 ? styles.negative : styles.positive}>
            {loading ? <Skeleton width="90px" height="22px" /> : formatNumber(data?.financial?.totalDifference)}
          </strong>
        </div>
        <div>
          <span>{t.financial.percentage}</span>
          <strong>
            {loading ? <Skeleton width="65px" height="22px" /> : `${Number(data?.financial?.differencePercentage ?? 0).toFixed(2)}%`}
          </strong>
        </div>
      </div>
    </section>
  );
}

export default ReportSummary;
