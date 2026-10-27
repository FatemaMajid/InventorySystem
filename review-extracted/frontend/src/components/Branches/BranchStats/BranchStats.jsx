import { useLanguage } from "../../../context/LanguageContext";

import Icon from "../../UI/Icon/Icon";
import Skeleton from "../../UI/Loading/Skeleton/Skeleton";

import styles from "./BranchStats.module.css";

function BranchStats({
  totalBranches = 0,
  activeBranches = 0,
  inactiveBranches = 0,
  loading = false,
}) {
  const { translations, direction } = useLanguage();
  const t = translations.branches;

  const stats = [
    {
      key: "total",
      label: t.totalBranches,
      value: totalBranches,
      icon: "branches",
      variant: "primary",
    },
    {
      key: "active",
      label: t.activeBranches,
      value: activeBranches,
      icon: "active",
      variant: "teal",
    },
    {
      key: "inactive",
      label: t.inactiveBranches,
      value: inactiveBranches,
      icon: "attention",
      variant: "warning",
    },
  ];

  return (
    <div
      className={styles.grid}
      dir={direction}
    >
      {stats.map((stat) => (
        <article
          key={stat.key}
          className={`${styles.card} ${styles[stat.variant]}`}
        >
          <div className={styles.iconWrapper}>
            <Icon name={stat.icon} size={20} />
          </div>

          <div className={styles.content}>
            <span className={styles.label}>
              {stat.label}
            </span>

            {loading ? (
              <Skeleton
                width="65px"
                height="30px"
                borderRadius="6px"
              />
            ) : (
              <strong className={styles.value}>
                {Number(stat.value || 0).toLocaleString(
                  direction === "rtl" ?  "en-US" : "en-US"
                )}
              </strong>
            )}
          </div>
        </article>
      ))}
    </div>
  );
}

export default BranchStats;
