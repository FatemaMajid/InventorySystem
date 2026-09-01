import { useLanguage } from '../../../context/LanguageContext';
import Icon from '../../UI/Icon/Icon';
import styles from './NewSessionActions.module.css';

function NewSessionActions({ disabled, loading, onSubmit, onCancel }) {
  const { translations, direction } = useLanguage();
  const t = translations.inventory;

  return (
    <div className={styles.actions} dir={direction}>
      <button type="button" className={styles.cancel} onClick={onCancel} disabled={loading}>
        {t.cancel}
      </button>

      <button type="button" className={styles.submit} onClick={onSubmit} disabled={disabled || loading}>
        {loading ? (
          <span>{t.creatingSession}</span>
        ) : (
          <>
            <Icon name="plus" size={18} />
            <span>{t.createSession}</span>
          </>
        )}
      </button>
    </div>
  );
}

export default NewSessionActions;