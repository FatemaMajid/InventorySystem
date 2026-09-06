import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../Icon/Icon";
import styles from "./EmptyState.module.css";

function EmptyState({
  icon = "search",
  message,
}) {
  const { translations, direction } = useLanguage();

  const text = message || translations.common?.noResults || "No results found.";

  return (
    <div className={styles.wrapper} dir={direction}>
      <div className={styles.icon}>
        <Icon name={icon} size={28} />
      </div>

      <span>{text}</span>
    </div>
  );
}

export default EmptyState;