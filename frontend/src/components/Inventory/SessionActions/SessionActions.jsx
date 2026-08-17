import { useLanguage } from '../../../context/LanguageContext';

import styles from './SessionActions.module.css';

function SessionActions() {
  const { translations } = useLanguage();

  return (
    <div className={styles.actions}>
      <button
        type="button"
        className={styles.cancel}
      >
        {translations.inventory.cancel}
      </button>

      <button
        type="button"
        className={styles.primary}
      >
        {translations.inventory.createSession}
      </button>
    </div>
  );
}

export default SessionActions;