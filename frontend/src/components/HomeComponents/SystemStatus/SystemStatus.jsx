import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./SystemStatus.module.css";

function SystemStatus({
  api = "Connected",
  database = "Connected",
  health = "Healthy",
}) {
  const { translations, isArabic } = useLanguage();

  const t = translations.home;

  const statuses = [
    {
      key: "apiStatus",
      icon: "tick",
      value: api,
      variant:
        api === "Connected"
          ? "success"
          : "danger",
    },
    {
      key: "database",
      icon: "tick",
      value: database,
      variant:
        database === "Connected"
          ? "success"
          : "danger",
    },
    {
      key: "systemHealth",
      icon: "health",
      value: health,
      variant:
        health === "Healthy"
          ? "success"
          : "danger",
    },
  ];

  return (
    <section
      className={styles.statusCard}
      dir={isArabic ? "rtl" : "ltr"}
    >
      <div className={styles.title}>
        <div className={styles.titleIcon}>
          <Icon
            name="shield"
            size={18}
          />
        </div>

        <h2>
          {t.systemStatus}
        </h2>
      </div>

      <div className={styles.statusList}>
        {statuses.map((status) => (
          <div
            key={status.key}
            className={styles.statusItem}
          >
            <div
              className={`${styles.statusIcon} ${
                styles[status.variant]
              }`}
            >
              <Icon
                name={status.icon}
                size={17}
              />
            </div>

            <div className={styles.statusContent}>
              <span className={styles.statusLabel}>
                {t[status.key]}
              </span>

              <span
                className={`${styles.statusValue} ${
                  status.variant === "success"
                    ? styles.successText
                    : ""
                }`}
              >
                {status.value}
              </span>
            </div>
          </div>
        ))}
      </div>
    </section>
  );
}

export default SystemStatus;