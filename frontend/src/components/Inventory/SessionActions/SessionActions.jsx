import { useLanguage } from '../../../context/LanguageContext';

import styles from './SessionActions.module.css';

function SessionActions({
  disabled = true,
  loading = false,
  onSubmit,
  onCancel,
}) {
  const { translations } = useLanguage();

  return (
    <div className={styles.actions}>
      <button
        type="button"
        className={styles.cancel}
        disabled={loading}
        onClick={onCancel}
      >
        {translations.inventory.cancel}
      </button>

      <button
        type="button"
        className={styles.primary}
        disabled={disabled}
        onClick={onSubmit}
      >
        {loading
          ? translations.inventory.creatingSession
          : translations.inventory.createSession}
      </button>
    </div>
  );
}

export default SessionActions;