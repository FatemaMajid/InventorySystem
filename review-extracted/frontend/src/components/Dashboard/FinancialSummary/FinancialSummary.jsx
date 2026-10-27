import { useLanguage } from "../../../context/LanguageContext";
import styles from "./FinancialSummary.module.css";

function FinancialSummary({
  totalValueBefore = 0,
  totalValueAfter = 0,
  totalDifference = 0,
  differencePercentage = 0,
}) {
  const { translations } = useLanguage();

  const formatNumber = (value) =>
    Number(value || 0).toLocaleString("en-US");

  const formatPercentage = (value) =>
    `${Number(value || 0).toFixed(2)}%`;

  const items = [
    {
      key: "before",
      title: translations.dashboard.financialSummary.totalValueBefore,
      value: totalValueBefore,
      className: styles.before,
    },
    {
      key: "after",
      title: translations.dashboard.financialSummary.totalValueAfter,
      value: totalValueAfter,
      className: styles.after,
    },
    {
      key: "difference",
      title: translations.dashboard.financialSummary.totalDifference,
      value: totalDifference,
      className: styles.difference,
    },
    {
      key: "percentage",
      title: translations.dashboard.financialSummary.differencePercentage,
      value: differencePercentage,
      className: styles.percentage,
      isPercentage: true,
    },
  ];

  return (
    <section className={styles.wrapper}>
      <h2 className={styles.title}>
        {translations.dashboard.financialSummary.title}
      </h2>

      <div className={styles.cards}>
        {items.map((item) => (
          <article
            key={item.key}
            className={`${styles.card} ${item.className}`}
          >
            <span className={styles.label}>
              {item.title}
            </span>

            <strong className={styles.value}>
              {item.isPercentage
                ? formatPercentage(item.value)
                : formatNumber(item.value)}
            </strong>

            {!item.isPercentage && (
              <span className={styles.currency}>
                {translations.dashboard.financialSummary.currency}
              </span>
            )}
          </article>
        ))}
      </div>
    </section>
  );
}

export default FinancialSummary;