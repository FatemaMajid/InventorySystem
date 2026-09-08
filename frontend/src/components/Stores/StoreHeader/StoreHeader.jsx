import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./StoreHeader.module.css";

function StoreHeader({ onNewStore }) {
  const { translations, direction } = useLanguage();
  const t = translations.stores;
  return (
    <header className={styles.header} dir={direction}>
      <div className={styles.identity}>
        <div className={styles.iconBox}><Icon name="stores" size={24} /></div>
        <div className={styles.info}>
          <div className={styles.eyebrow}>{translations.navigation?.masterData || "Master Data"}</div>
          <h1 className={styles.title}>{translations.navigation.stores}</h1>
          <p className={styles.description}>{t.description}</p>
        </div>
      </div>
      <button type="button" className={styles.newButton} onClick={onNewStore}>
        <Icon name="plus" size={18} />
        <span>{t.newStore}</span>
      </button>
    </header>
  );
}

export default StoreHeader;
