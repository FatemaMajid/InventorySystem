import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";

import styles from "./ReportSession.module.css";

function ReportSession({ data }) {
  const { translations, language, direction } = useLanguage();
  const t = translations.reports;

  if (!data) return null;

  const session = data.session ?? {};
  const date = session.inventoryDate
    ? new Date(session.inventoryDate).toLocaleDateString( "en-US")
    : "—";

  return (
    <section className={styles.wrapper} dir={direction}>
      <div className={styles.item}>
        <Icon name="inventory" size={18} />
        <div>
          <span>{t.session}</span>
          <strong>{session.sessionNumber || "—"}</strong>
        </div>
      </div>

      <div className={styles.item}>
        <Icon name="calendar" size={18} />
        <div>
          <span>{t.date}</span>
          <strong>{date}</strong>
        </div>
      </div>

      <div className={styles.item}>
        <Icon name="branches" size={18} />
        <div>
          <span>{t.branch}</span>
          <strong>{session.branchName || "—"}</strong>
        </div>
      </div>

      <div className={styles.item}>
        <Icon name="stores" size={18} />
        <div>
          <span>{t.store}</span>
          <strong>{session.storeName || "—"}</strong>
        </div>
      </div>

      <span className={styles.status}>{session.status || "—"}</span>
    </section>
  );
}

export default ReportSession;
