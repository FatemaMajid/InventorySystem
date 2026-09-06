import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";

import styles from "./ReportTabs.module.css";

function ReportTabs({ activeTab, onChange }) {
  const { translations, direction } = useLanguage();
  const t = translations.reports.tabs;

  const tabs = [
    { key: "overview", label: t.overview, icon: "dashboard" },
    { key: "comparison", label: t.comparison, icon: "comparison" },
  ];

  return (
    <div className={styles.wrapper} dir={direction}>
      {tabs.map((tab) => (
        <button
          key={tab.key}
          type="button"
          className={activeTab === tab.key ? styles.active : ""}
          onClick={() => onChange?.(tab.key)}
        >
          <Icon name={tab.icon} size={17} />
          <span>{tab.label}</span>
        </button>
      ))}
    </div>
  );
}

export default ReportTabs;
