import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";

import styles from "./AttentionSession.module.css";

function AttentionSession({
  sessions = [],
  selectedSessionId = "",
  onChange,
  loading = false,
  session = null,
}) {
  const { translations, language, direction } = useLanguage();
  const t = translations.attentionItems;
  const isArabic = language === "ar";

  const getDate = (value) => {
    if (!value) {
      return "—";
    }

    const date = new Date(value);

    if (Number.isNaN(date.getTime())) {
      return "—";
    }

    return date.toLocaleDateString(
      isArabic ? "ar-IQ" : "en-US",
      {
        year: "numeric",
        month: "short",
        day: "numeric",
      }
    );
  };

  const getLocalizedValue = (
    arabic,
    english,
    fallback = "—"
  ) =>
    isArabic
      ? arabic || english || fallback
      : english || arabic || fallback;

  const branchName = getLocalizedValue(
    session?.branchNameArabic,
    session?.branchNameEnglish,
    session?.branchName
  );

  const storeName = getLocalizedValue(
    session?.storeNameArabic,
    session?.storeNameEnglish,
    session?.storeName
  );

  const inventoryType =
    session?.inventoryTypeName ||
    session?.inventoryType ||
    "—";

  return (
    <section className={styles.wrapper} dir={direction}>
      <div className={styles.selectRow}>
        <div className={styles.labelGroup}>
          <div className={styles.iconBox}>
            <Icon name="inventory" size={19} />
          </div>

          <div>
            <span className={styles.label}>
              {t.session}
            </span>

            <strong className={styles.sessionNumber}>
              {session?.sessionNumber || "—"}
            </strong>
          </div>
        </div>

        <div className={styles.selectContainer}>
          <span className={styles.selectLabel}>
            {t.session}
          </span>

          <select
            value={selectedSessionId}
            onChange={(event) =>
              onChange?.(event.target.value)
            }
            disabled={loading || sessions.length === 0}
            aria-label={t.session}
          >
            {!selectedSessionId && (
              <option value="">
                {t.session}
              </option>
            )}

            {sessions.map((item) => (
              <option key={item.id} value={item.id}>
                {item.sessionNumber}
              </option>
            ))}
          </select>
        </div>
      </div>

      {session && (
        <div className={styles.meta}>
          <div>
            <span>{t.sessionDate}</span>
            <strong>
              {getDate(session.inventoryDate)}
            </strong>
          </div>

          <div>
            <span>{t.branch}</span>
            <strong>{branchName}</strong>
          </div>

          <div>
            <span>{t.store}</span>
            <strong>{storeName}</strong>
          </div>

          <div>
            <span>{t.inventoryType}</span>
            <strong>{inventoryType}</strong>
          </div>
        </div>
      )}
    </section>
  );
}

export default AttentionSession;
