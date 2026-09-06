import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";

import styles from "./ReportHeader.module.css";

function ReportHeader() {
  const { translations, direction } = useLanguage();
  const t = translations.reports;

  return (
    <header className={styles.wrapper} dir={direction}>
      <div className={styles.titleWrapper}>
        <div className={styles.icon}>
          <Icon name="reports" size={24} />
        </div>

        <div>
          <h1>{t.title}</h1>
          <p>{t.description}</p>
        </div>
      </div>
    </header>
  );
}

export default ReportHeader;
