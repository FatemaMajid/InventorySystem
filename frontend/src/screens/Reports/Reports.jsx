import { useLanguage } from '../../context/LanguageContext';

import styles from './Reports.module.css';

function Reports() {
  const { translations } = useLanguage();

  return (
    <section className={styles.page}>
      <h1>{translations.navigation.reports}</h1>

      <p>
        {translations.home.reportsDescription}
      </p>
    </section>
  );
}

export default Reports;