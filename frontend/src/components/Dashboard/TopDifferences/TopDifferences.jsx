import { useLanguage } from "../../../context/LanguageContext";
import styles from "./TopDifferences.module.css";

function TopDifferences({ items = [] }) {
  const { translations } = useLanguage();


  const maxPositive = 2450000;
  const maxNegative = 2100000;

  const formatValue = (value) => {
    const abs = Math.abs(value);

    if (abs >= 1000000) {
      return `${value < 0 ? "-" : "+"}${(abs / 1000).toLocaleString()}K`;
    }

    return `${value < 0 ? "-" : "+"}${(abs / 1000).toLocaleString()}K`;
  };

  return (
    <section className={styles.wrapper}>
      <h2 className={styles.title}>
        {translations.dashboard.kpiCards.topDifferences.title}
      </h2>

      <div className={styles.chart}>
        {items.map((item) => {
          const positive = item.valueDifference >= 0;

          const width = positive
            ? (Math.abs(item.valueDifference) / maxPositive) * 100
            : (Math.abs(item.valueDifference) / maxNegative) * 100;

          return (
            <div className={styles.row} key={item.itemCode}>

              {/* ITEM CODE */}
              <div className={styles.code}>
                {item.itemCode}
              </div>

              {/* BAR AREA */}
              <div className={styles.barArea}>

                <div
                  className={`${styles.bar} ${
                    positive
                      ? styles.barPositive
                      : styles.barNegative
                  }`}
                  style={{ width: `${width}%` }}
                />

              </div>

              {/* VALUE */}
              <div
                className={`${styles.value} ${
                  positive
                    ? styles.valuePositive
                    : styles.valueNegative
                }`}
              >
                {formatValue(item.valueDifference)}
              </div>

            </div>
          );
        })}
      </div>
    </section>
  );
}

export default TopDifferences;