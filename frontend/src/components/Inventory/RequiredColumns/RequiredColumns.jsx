import { useLanguage } from '../../../context/LanguageContext';
import styles from './RequiredColumns.module.css';

function RequiredColumns() {
  const { translations } = useLanguage();

  const columns = [
    'Item Code',
    'Item Name',
    'Unit',
    'Quantity',
  ];

  return (
    <section className={styles.card}>
      <div className={styles.header}>
        <h2>{translations.inventory.requiredColumns}</h2>

        <p>
          {translations.inventory.requiredColumnsDescription}
        </p>
      </div>

      <div className={styles.columns}>
        {columns.map((column) => (
          <div
            key={column}
            className={styles.column}
          >
            <span className={styles.dot} />
            <span>{column}</span>
          </div>
        ))}
      </div>
    </section>
  );
}

export default RequiredColumns;