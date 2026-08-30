import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./Overview.module.css";

function Overview({
  branches = 12,
  stores = 28,
  itemCategories = 54,
  items = 1842,
}) {
  const { translations, isArabic } = useLanguage();

  const t = translations.home;

  const overviewItems = [
    {
      key: "branches",
      value: branches,
      description: t.totalBranches,
      icon: "branches",
      variant: "blue",
    },
    {
      key: "stores",
      value: stores,
      description: t.totalStores,
      icon: "stores",
      variant: "purple",
    },
    {
      key: "itemCategories",
      value: itemCategories,
      description: t.totalCategories,
      icon: "category",
      variant: "orange",
    },
    {
      key: "totalItems",
      value: items,
      description: t.totalItems,
      icon: "totalItems",
      variant: "teal",
    },
  ];

  const formatNumber = (value) =>
    new Intl.NumberFormat("en-US").format(Number(value) || 0);

  return (
    <section
      className={styles.wrapper}
      dir={isArabic ? "rtl" : "ltr"}
    >
      <div className={styles.header}>
        <div className={styles.titleWrapper}>
          <div className={styles.titleIcon}>
            <Icon name="reports" size={18} />
          </div>

          <div className={styles.titleContent}>
            <h2>{t.overview}</h2>
            <p>{t.overviewSubtitle}</p>
          </div>
        </div>
      </div>

      <div className={styles.grid}>
        {overviewItems.map((item) => (
          <div
            key={item.key}
            className={`${styles.card} ${styles[item.variant]}`}
          >
            <div className={styles.iconWrapper}>
              <Icon
                name={item.icon}
                size={22}
              />
            </div>

            <div className={styles.content}>
              <span className={styles.label}>
                {t[item.key]}
              </span>

              <strong className={styles.value}>
                {formatNumber(item.value)}
              </strong>

              <span className={styles.description}>
                {item.description}
              </span>
            </div>
          </div>
        ))}
      </div>
    </section>
  );
}

export default Overview;