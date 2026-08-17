import { useLanguage } from '../../../context/LanguageContext';

import FileUpload from '../FileUpload/FileUpload';

import styles from './InventoryFiles.module.css';

function InventoryFiles() {
  const { translations } = useLanguage();

  return (
    <section className={styles.section}>
      <div className={styles.header}>
        <h2>{translations.inventory.uploadFile}</h2>

        <p>
          {translations.inventory.uploadFileDescription}
        </p>
      </div>

      <div className={styles.grid}>
        <FileUpload
          title={translations.inventory.beforeInventory}
          description={
            translations.inventory
              .beforeInventoryDescription
          }
        />

        <FileUpload
          title={translations.inventory.afterInventory}
          description={
            translations.inventory
              .afterInventoryDescription
          }
        />
      </div>
    </section>
  );
}

export default InventoryFiles;