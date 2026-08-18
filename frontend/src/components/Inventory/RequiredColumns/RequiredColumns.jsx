import { useLanguage } from '../../../context/LanguageContext';

import styles from './RequiredColumns.module.css';

function RequiredColumns() {
  const { translations } = useLanguage();

  const columns = [
    {
      key: 'itemCode',
      label:
        translations.inventory.requiredColumnNames.itemCode,
    },
    {
      key: 'itemName',
      label:
        translations.inventory.requiredColumnNames.itemName,
    },
    {
      key: 'category',
      label:
        translations.inventory.requiredColumnNames.category,
    },
    {
      key: 'quantity',
      label:
        translations.inventory.requiredColumnNames.quantity,
    },
    {
      key: 'price',
      label:
        translations.inventory.requiredColumnNames.price,
    },
  ];

  return (
    <section className={styles.card}>
      <div className={styles.header}>
        <h2>
          {translations.inventory.requiredColumns}
        </h2>

        <p>
          {translations.inventory.requiredColumnsDescription}
        </p>
      </div>

      <div className={styles.columns}>
        {columns.map((column) => (
          <div
            key={column.key}
            className={styles.column}
          >
            <span className={styles.dot} />

            <span>{column.label}</span>
          </div>
        ))}
      </div>
    </section>
  );
}

export default RequiredColumns;