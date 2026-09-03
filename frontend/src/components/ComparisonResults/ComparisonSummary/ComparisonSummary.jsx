import { useLanguage } from "../../../context/LanguageContext";
import { useInventorySession } from "../../../context/InventorySessionContext";
import styles from "./ComparisonSummary.module.css";

function ComparisonSummary() {
  const { translations, direction } = useLanguage();
  const { activeSession } = useInventorySession();

  const t = translations.dashboard.comparisonResults;

  const formatNumber = (value) =>
    Number(value ?? 0).toLocaleString("en-US");

  return (
    <section
      className={styles.wrapper}
      dir={direction}
    >
      <div className={styles.card}>
        <span>{t.totalItems}</span>
        <strong>
          {formatNumber(activeSession?.totalItems)}
        </strong>
      </div>

      <div className={styles.card}>
        <span>{t.branch}</span>
        <strong>
          {activeSession?.branchName || "—"}
        </strong>
      </div>

      <div className={styles.card}>
        <span>{t.store}</span>
        <strong>
          {activeSession?.storeName || "—"}
        </strong>
      </div>

      <div className={styles.card}>
        <span>{t.inventoryType}</span>
        <strong>
          {activeSession?.inventoryType || "—"}
        </strong>
      </div>
    </section>
  );
}

export default ComparisonSummary;