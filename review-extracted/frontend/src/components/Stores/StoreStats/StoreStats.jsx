import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import Skeleton from "../../UI/Loading/Skeleton/Skeleton";

import styles from "./StoreStats.module.css";

function StoreStats({ totalStores = 0, activeStores = 0, inactiveStores = 0, loading = false }) {
  const { translations, direction } = useLanguage();
  const t = translations.stores;

  const stats = [
    { key: "total", label: t.totalStores, value: totalStores, icon: "stores", variant: "primary" },
    { key: "active", label: t.activeStores, value: activeStores, icon: "active", variant: "teal" },
    { key: "inactive", label: t.inactiveStores, value: inactiveStores, icon: "attention", variant: "purple" },
  ];

  return (
    <div className={styles.grid} dir={direction}>
      {stats.map((stat) => (
        <article key={stat.key} className={`${styles.card} ${styles[stat.variant]}`}>
          <div className={styles.iconWrapper}><Icon name={stat.icon} size={20} /></div>
          <div className={styles.content}>
            <span className={styles.label}>{stat.label}</span>
            {loading ? <Skeleton width="65px" height="30px" borderRadius="6px" /> : <strong className={styles.value}>{Number(stat.value || 0).toLocaleString("en-US")}</strong>}
          </div>
        </article>
      ))}
    </div>
  );
}

export default StoreStats;
