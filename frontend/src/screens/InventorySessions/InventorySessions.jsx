import { useLanguage } from '../../context/LanguageContext';

import styles from './InventorySessions.module.css';

function InventorySessions() {
  const { translations } = useLanguage();

  return (
    <section className={styles.page}>
      <h1>{translations.navigation.inventorySessions}</h1>

      <p>
        {translations.home.inventorySessionsDescription}
      </p>
    </section>
  );
}

export default InventorySessions;