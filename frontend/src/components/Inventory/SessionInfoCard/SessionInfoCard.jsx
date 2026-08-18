import { useLanguage } from '../../../context/LanguageContext';

import styles from './SessionInfoCard.module.css';

function SessionInfoCard({
  value,
  onChange,
}) {
  const { translations } = useLanguage();

  const inventoryTypes = [
    {
      value: 1,
      label: translations.inventory.inventoryTypes.semiAnnual,
    },
    {
      value: 2,
      label: translations.inventory.inventoryTypes.annual,
    },
  ];

  return (
    <section className={styles.card}>
      <div className={styles.header}>
        <h2>
          {translations.inventory.sessionInformation}
        </h2>

        <p>
          {translations.inventory.sessionInformationDescription}
        </p>
      </div>

      <div className={styles.form}>
        <div className={styles.field}>
          <label htmlFor="inventoryType">
            {translations.inventory.inventoryType}
          </label>

          <select
            id="inventoryType"
            value={value}
            onChange={(event) =>
              onChange(Number(event.target.value))
            }
          >
            <option value={0} disabled>
              {translations.inventory.selectInventoryType}
            </option>

            {inventoryTypes.map((type) => (
              <option
                key={type.value}
                value={type.value}
              >
                {type.label}
              </option>
            ))}
          </select>
        </div>
      </div>
    </section>
  );
}

export default SessionInfoCard;