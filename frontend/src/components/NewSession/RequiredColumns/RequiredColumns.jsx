import { useLanguage } from '../../../context/LanguageContext';
import styles from './RequiredColumns.module.css';

function RequiredColumns() {
  const { translations } = useLanguage();
  const t = translations.inventory;

  const columns = [
    t.requiredColumnNames.itemCode,
    t.requiredColumnNames.itemName,
    t.requiredColumnNames.category,
    t.requiredColumnNames.quantity,
    t.requiredColumnNames.price,
  ];

  return (
    <section className={styles.card}>
      <div className={styles.header}>
        <h2>{t.requiredColumns}</h2>
        <p>{t.requiredColumnsDescription}</p>
      </div>

      <div className={styles.columns}>
        {columns.map((column) => (
          <div key={column} className={styles.column}>
            <span className={styles.dot} />
            <span>{column}</span>
          </div>
        ))}
      </div>
    </section>
  );
}

export default RequiredColumns;