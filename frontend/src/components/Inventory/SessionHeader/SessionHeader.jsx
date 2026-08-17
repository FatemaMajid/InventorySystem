import { useLanguage } from '../../../context/LanguageContext';

import styles from './SessionHeader.module.css';

function SessionHeader() {
  const { translations } = useLanguage();

  return (
    <header className={styles.header}>
      <div className={styles.text}>
        <h1>{translations.navigation.newInventorySession}</h1>

        <p>
          {translations.home.newInventoryDescription}
        </p>
      </div>
    </header>
  );
}

export default SessionHeader;