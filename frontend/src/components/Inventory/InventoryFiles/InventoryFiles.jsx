import { useLanguage } from '../../../context/LanguageContext';

import FileUpload from '../FileUpload/FileUpload';

import styles from './InventoryFiles.module.css';

function InventoryFiles({
  beforeFile,
  afterFile,
  onBeforeFileChange,
  onAfterFileChange,
}) {
  const { translations } = useLanguage();

  return (
    <section className={styles.section}>
      <div className={styles.header}>
        <h2>
          {translations.inventory.uploadFile}
        </h2>

        <p>
          {translations.inventory.uploadFileDescription}
        </p>
      </div>

      <div className={styles.grid}>
        <FileUpload
          title={
            translations.inventory.beforeInventory
          }
          description={
            translations.inventory
              .beforeInventoryDescription
          }
          file={beforeFile}
          onFileChange={onBeforeFileChange}
        />

        <FileUpload
          title={
            translations.inventory.afterInventory
          }
          description={
            translations.inventory
              .afterInventoryDescription
          }
          file={afterFile}
          onFileChange={onAfterFileChange}
        />
      </div>
    </section>
  );
}

export default InventoryFiles;