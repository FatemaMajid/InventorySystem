import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./AttentionHeader.module.css";

function AttentionHeader() {
  const { translations, direction } = useLanguage();
  const t = translations.attentionItems;

  return (
    <header className={styles.wrapper} dir={direction}>
      <div className={styles.iconBox}>
        <Icon name="attention" size={24} />
      </div>

      <div className={styles.content}>
        <h1>{t.title}</h1>
        <p>{t.description}</p>
      </div>
    </header>
  );
}

export default AttentionHeader;
