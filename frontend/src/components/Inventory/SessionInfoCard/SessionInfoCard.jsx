import { useLanguage } from '../../../context/LanguageContext';

import styles from './SessionInfoCard.module.css';

function SessionInfoCard() {
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
        <div>
          <h2>
            {translations.inventory.sessionInformation}
          </h2>

          <p>
            {translations.inventory.sessionInformationDescription}
          </p>
        </div>
      </div>

      <div className={styles.form}>
        {/* Inventory Type */}
        <div className={styles.field}>
          <label htmlFor="inventoryType">
            {translations.inventory.inventoryType}
          </label>

          <select id="inventoryType" defaultValue="">
            <option value="" disabled>
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

        {/* Branch */}
        <div className={styles.field}>
          <label htmlFor="branch">
            {translations.navigation.branches}
          </label>

          <select id="branch" defaultValue="">
            <option value="" disabled>
              {translations.inventory.selectBranch}
            </option>
          </select>
        </div>

        {/* Store */}
        <div className={styles.field}>
          <label htmlFor="store">
            {translations.navigation.stores}
          </label>

          <select id="store" defaultValue="">
            <option value="" disabled>
              {translations.inventory.selectStore}
            </option>
          </select>
        </div>

        {/* Inventory Date */}
        <div className={styles.field}>
          <label htmlFor="inventoryDate">
            {translations.inventory.inventoryDate}
          </label>

          <input
            id="inventoryDate"
            type="date"
          />
        </div>

        {/* Notes */}
        <div
          className={`${styles.field} ${styles.fullWidth}`}
        >
          <label htmlFor="notes">
            {translations.inventory.notes}
          </label>

          <textarea
            id="notes"
            rows="4"
            placeholder={
              translations.inventory.notesPlaceholder
            }
          />
        </div>
      </div>
    </section>
  );
}

export default SessionInfoCard;