import { useLanguage } from '../../../context/LanguageContext';
import styles from './NewSessionHeader.module.css';

function NewSessionHeader() {
  const { translations, direction } = useLanguage();

  return (
    <header className={styles.header} dir={direction}>
      <div className={styles.info}>
        <h1 className={styles.title}>{translations.navigation.newInventorySession}</h1>
        <p className={styles.description}>{translations.home.newInventoryDescription}</p>
      </div>
    </header>
  );
}

export default NewSessionHeader;