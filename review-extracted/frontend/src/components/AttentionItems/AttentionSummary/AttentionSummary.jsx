import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import Skeleton from "../../UI/Loading/Skeleton/Skeleton";

import styles from "./AttentionSummary.module.css";

const DEFAULT_DATA = {
  total: 0,
  newlyCounted: 0,
  fullyDepleted: 0,
  unitNotDefined: 0,
  priceChanged: 0,
};

function AttentionSummary({ data, loading = false }) {
  const { translations, language, direction } = useLanguage();

  const t = translations.attentionItems;
  const summary = data ?? DEFAULT_DATA;

  const items = [
    {
      id: "total",
      key: "total",
      value: summary.total,
      icon: "attention",
      type: "total",
    },
    {
      id: "newlyCounted",
      key: "newlyCounted",
      value: summary.newlyCounted,
      icon: "plus",
      type: "new",
    },
    {
      id: "fullyDepleted",
      key: "fullyDepleted",
      value: summary.fullyDepleted,
      icon: "depleted",
      type: "depleted",
    },
    {
      id: "unitNotDefined",
      key: "unitNotDefined",
      value: summary.unitNotDefined,
      icon: "attention",
      type: "unit",
    },
    {
      id: "priceChanged",
      key: "priceChanged",
      value: summary.priceChanged,
      icon: "priceChange",
      type: "price",
    },
  ];

  const formatNumber = (value) =>
    Number(value || 0).toLocaleString(
      language === "en-US"
    );

  return (
    <section className={styles.wrapper} dir={direction}>
      <div className={styles.header}>
        <div>
          <h2>{t.title}</h2>
          <p>{t.description}</p>
        </div>
      </div>

      <div className={styles.items}>
        {items.map((item) => (
          <div
            key={item.id}
            className={`${styles.item} ${styles[item.type]}`}
          >
            <div className={styles.iconBox}>
              <Icon name={item.icon} size={18} />
            </div>

            <div className={styles.content}>
              <span>{t.summary[item.key]}</span>

              <strong>
                {loading ? (
                  <Skeleton width="55px" height="24px" borderRadius="6px" />
                ) : (
                  formatNumber(item.value)
                )}
              </strong>
            </div>
          </div>
        ))}
      </div>
    </section>
  );
}

export default AttentionSummary;
