import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./SessionStats.module.css";

function SessionStats({
  totalSessions = 0,
  activeSessions = 0,
  completedSessions = 0,
}) {
  const { translations, direction } = useLanguage();
  const t = translations.inventorySessions;

  const stats = [
    {
      key: "total",
      label: t.totalSessions,
      value: totalSessions,
      icon: "inventory",
      variant: "primary",
    },
    {
      key: "active",
      label: t.activeSessions,
      value: activeSessions,
      icon: "active",
      variant: "teal",
    },
    {
      key: "completed",
      label: t.completedSessions,
      value: completedSessions,
      icon: "tick",
      variant: "success",
    },
  ];

  return (
    <div className={styles.grid} dir={direction}>
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

            <strong className={styles.value}>
              {stat.value}
            </strong>
          </div>
        </article>
      ))}
    </div>
  );
}

export default SessionStats;