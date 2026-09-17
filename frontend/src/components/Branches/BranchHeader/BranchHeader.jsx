import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./BranchHeader.module.css";

function BranchHeader({ onAdd, canCreate = false }) {
  const { translations, direction } = useLanguage();
  const t = translations.branches;

  return (
    <header className={styles.header} dir={direction}>
      <div className={styles.identity}>
        <div className={styles.iconBox}>
          <Icon name="branches" size={24} />
        </div>

        <div className={styles.info}>
          <div className={styles.eyebrow}>
            {translations.navigation?.masterData || "Master Data"}
          </div>

          <h1 className={styles.title}>
            {translations.navigation.branches}
          </h1>

          <p className={styles.description}>
            {t.description}
          </p>
        </div>
      </div>

      {canCreate && (
        <button
          type="button"
          className={styles.newButton}
          onClick={onAdd}
        >
          <Icon name="plus" size={18} />
          <span>{t.addBranch}</span>
        </button>
      )}
    </header>
  );
}

export default BranchHeader;