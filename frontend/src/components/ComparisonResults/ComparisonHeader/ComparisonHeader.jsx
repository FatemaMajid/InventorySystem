import { useLanguage } from "../../../context/LanguageContext";
import { useInventorySession } from "../../../context/InventorySessionContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./ComparisonHeader.module.css";

function ComparisonHeader() {
  const { translations, direction } = useLanguage();
  const { activeSession } = useInventorySession();

  const t = translations.dashboard.comparisonResults;

  return (
    <section
      className={styles.wrapper}
      dir={direction}
    >
      <div className={styles.titleWrapper}>
        <div className={styles.icon}>
          <Icon name="comparison" size={22} />
        </div>

        <div>
          <h1>{t.title}</h1>
          <p>{t.description}</p>
        </div>
      </div>

      <div className={styles.session}>
        <span>{activeSession?.sessionNumber || "—"}</span>
      </div>
    </section>
  );
}

export default ComparisonHeader;