import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";

import styles from "./ReportActions.module.css";

function ReportActions({ loading = false, onExport }) {
  const { translations, direction } = useLanguage();
  const t = translations.reports.actions;

  return (
    <div className={styles.wrapper} dir={direction}>
      <button type="button" disabled={loading} onClick={() => onExport?.("excel")}>
        <Icon name="reports" size={17} />
        <span>{t.excel}</span>
      </button>

      <button type="button" disabled={loading} onClick={() => onExport?.("pdf")}>
        <Icon name="clipboard" size={17} />
        <span>{t.pdf}</span>
      </button>

      <button type="button" disabled={loading} onClick={() => window.print()}>
        <Icon name="eye" size={17} />
        <span>{t.print}</span>
      </button>
    </div>
  );
}

export default ReportActions;
