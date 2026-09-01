import { useLanguage } from "../../../context/LanguageContext";
import { useInventorySession } from "../../../context/InventorySessionContext";
import { useNavigate } from "react-router-dom";
import Icon from "../../UI/Icon/Icon";
import styles from "./SessionsTable.module.css";

function SessionsTable({
  sessions = [],
  loading = false,
}) {
  const { translations, direction } = useLanguage();
  const { setActiveSession } = useInventorySession();
  const navigate = useNavigate();

  const t = translations.inventorySessions;

  const openSession = (session) => {
    setActiveSession(session);
    navigate("/dashboard");
  };

  const statusLabel = (status) => {
    if (status === "Completed") return t.completed;
    if (status === "InProgress") return t.inProgress;
    if (status === "Cancelled") return t.cancelled;
    return status || "—";
  };

  const formatDate = (date) => {
    if (!date) return "—";

    const utcDate = new Date(
      date.endsWith("Z") ? date : `${date}Z`
    );

    return utcDate.toLocaleString(
      direction === "rtl" ? "ar-IQ" : "en-US",
      {
        year: "numeric",
        month: "short",
        day: "2-digit",
        hour: "2-digit",
        minute: "2-digit",
      }
    );
  };
  return (
    <section className={styles.card} dir={direction}>
      <div className={styles.header}>
        <div>
          <h2 className={styles.title}>
            {t.sessionsList}
          </h2>

          <p className={styles.description}>
            {t.sessionsListDescription}
          </p>
        </div>
      </div>

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
            {loading ? (
              <tr>
                <td colSpan={7} className={styles.empty}>
                  {t.loading}
                </td>
              </tr>
            ) : sessions.length === 0 ? (
              <tr>
                <td colSpan={7} className={styles.empty}>
                  {t.noSessions}
                </td>
              </tr>
            ) : (
              sessions.map((session) => (
                <tr key={session.id}>
                  <td className={styles.sessionId}>
                    {session.sessionNumber}
                  </td>

                  <td>
                    {formatDate(session.inventoryDate)}
                  </td>

                  <td>{session.branchName || "—"}</td>

                  <td>{session.storeName || "—"}</td>

                  <td>
                    <span
                      className={`${styles.status} ${styles[session.status]
                        }`}
                    >
                      {statusLabel(session.status)}
                    </span>
                  </td>

                  <td>
                    {Number(
                      session.totalItems || 0
                    ).toLocaleString(
                      direction === "rtl" ? "ar-IQ" : "en-US"
                    )}
                  </td>

                  <td>
                    <button
                      type="button"
                      className={styles.actionButton}
                      onClick={() => openSession(session)}
                      title={t.view}
                      aria-label={t.view}
                    >
                      <Icon name="eye" size={17} />
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </section>
  );
}

export default SessionsTable;