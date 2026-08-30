import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./RecentSessions.module.css";

function RecentSessions({
  sessions = [],
  onViewAll,
  onViewSession,
}) {
  const { translations, isArabic } = useLanguage();

  const t = translations.home.recentSessions;

  // Keep numbers in English in both languages.
  const formatNumber = (value) => {
    return Number(value ?? 0).toLocaleString("en-US");
  };

  // Keep date/months in English in both languages.
  // Example: 24 Aug 2026, 10:30 AM
  const formatDate = (value) => {
    if (!value) return "—";

    const date = new Date(value);

    if (Number.isNaN(date.getTime())) {
      return value;
    }

    return new Intl.DateTimeFormat("en-GB", {
      day: "2-digit",
      month: "short",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
      hour12: true,
    }).format(date);
  };

  const getStatusLabel = (status) => {
    switch (status) {
      case "Completed":
        return t.statuses.completed;

      case "InProgress":
        return t.statuses.inProgress;

      case "Cancelled":
        return t.statuses.cancelled;

      default:
        return status || "—";
    }
  };

  const getStatusClass = (status) => {
    switch (status) {
      case "Completed":
        return styles.completed;

      case "In Progress":
        return styles.inProgress;

      case "Cancelled":
        return styles.cancelled;

      default:
        return "";
    }
  };

  return (
    <section
      className={styles.wrapper}
      dir={isArabic ? "rtl" : "ltr"}
    >
      {/* Header */}
      <div className={styles.header}>
        <div className={styles.titleWrapper}>
          <div className={styles.titleIcon}>
            <Icon name="reports" size={18} />
          </div>

          <div className={styles.titleContent}>
            <h2>{t.title}</h2>

            {t.description && (
              <p>{t.description}</p>
            )}
          </div>
        </div>

        <button
          type="button"
          className={styles.viewAll}
          onClick={onViewAll}
        >
          <span>{t.viewAll}</span>

          <Icon
            name={isArabic ? "arrowLeft" : "arrowRight"}
            size={15}
          />
        </button>
      </div>

      {/* Table */}
      <div className={styles.tableWrapper}>
        <table className={styles.table}>
          <thead>
            <tr>
              <th>{t.sessionId}</th>
              <th>{t.date}</th>
              <th>{t.branch}</th>
              <th>{t.store}</th>
              <th>{t.status}</th>
              <th>{t.items}</th>
              <th>{t.actions}</th>
            </tr>
          </thead>

          <tbody>
            {sessions.length > 0 ? (
              sessions.map((session) => (
                <tr key={session.id}>
                  {/* Session ID */}
                  <td className={styles.sessionId}>
                    {session.sessionId || "—"}
                  </td>

                  {/* Date */}
                  <td className={styles.date}>
                    {formatDate(session.date)}
                  </td>

                  {/* Branch */}
                  <td>
                    {session.branch || "—"}
                  </td>

                  {/* Store / Warehouse */}
                  <td>
                    {session.store || "—"}
                  </td>

                  {/* Status */}
                  <td>
                    <span
                      className={`${styles.status} ${getStatusClass(
                        session.status
                      )}`}
                    >
                      {getStatusLabel(session.status)}
                    </span>
                  </td>

                  {/* Items */}
                  <td className={styles.items}>
                    {formatNumber(session.items)}
                  </td>

                  {/* Action */}
                  <td>
                    <button
                      type="button"
                      className={styles.viewButton}
                      onClick={() =>
                        onViewSession?.(session)
                      }
                      aria-label={t.view}
                      title={t.view}
                    >
                      <Icon
                        name="eye"
                        size={17}
                      />
                    </button>
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td
                  colSpan="7"
                  className={styles.empty}
                >
                  {t.noSessions}
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </section>
  );
}

export default RecentSessions;