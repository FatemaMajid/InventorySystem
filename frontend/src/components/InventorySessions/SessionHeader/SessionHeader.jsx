import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";

import styles from "./SessionHeader.module.css";

function SessionHeader() {
  const { translations, direction } = useLanguage();

  return (
    <header
      className={styles.header}
      dir={direction}
    >
      <div className={styles.info}>
        <h1 className={styles.title}>
          {translations.navigation.inventorySessions}
        </h1>

        <p className={styles.description}>
          {translations.home.inventorySessionsDescription}
        </p>
      </div>

      <button
        type="button"
        className={styles.newButton}
      >
        <Icon
          name="plus"
          size={18}
        />

        <span>
          {translations.inventorySessions?.newSession ||
            "New Inventory Session"}
        </span>
      </button>
    </header>
  );
}

export default SessionHeader;