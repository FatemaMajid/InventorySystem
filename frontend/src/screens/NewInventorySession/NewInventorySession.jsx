import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useLanguage } from '../../context/LanguageContext';

import SessionHeader from '../../components/Inventory/SessionHeader/SessionHeader';
import SessionInfoCard from '../../components/Inventory/SessionInfoCard/SessionInfoCard';
import FileUpload from '../../components/Inventory/FileUpload/FileUpload';
import RequiredColumns from '../../components/Inventory/RequiredColumns/RequiredColumns';
import SessionActions from '../../components/Inventory/SessionActions/SessionActions';

import {
  previewInventoryFile,
  confirmInventorySession,
} from '../../services/inventorySessionService';

import styles from './NewInventorySession.module.css';

function NewInventorySession() {
  const navigate = useNavigate();
  const { translations } = useLanguage();

  const [inventoryType, setInventoryType] =
    useState(0);

  const [beforeFile, setBeforeFile] =
    useState(null);

  const [afterFile, setAfterFile] =
    useState(null);

  const [beforePreview, setBeforePreview] =
    useState(null);

  const [afterPreview, setAfterPreview] =
    useState(null);

  const [previewingBefore, setPreviewingBefore] =
    useState(false);

  const [previewingAfter, setPreviewingAfter] =
    useState(false);

  const [isSubmitting, setIsSubmitting] =
    useState(false);

  const [error, setError] =
    useState('');

  const canSubmit =
    inventoryType !== 0 &&
    beforeFile !== null &&
    afterFile !== null &&
    beforePreview !== null &&
    afterPreview !== null &&
    !previewingBefore &&
    !previewingAfter &&
    !isSubmitting;

  const handleBeforeFileChange = async (
    file
  ) => {
    setBeforeFile(file);
    setBeforePreview(null);
    setError('');

    if (!file) {
      return;
    }

    setPreviewingBefore(true);

    try {
      const result =
        await previewInventoryFile(file);

      setBeforePreview(result);
    } catch (err) {
      setBeforeFile(null);

      setError(
        err.message ||
          translations.inventory
            .previewError
      );
    } finally {
      setPreviewingBefore(false);
    }
  };

  const handleAfterFileChange = async (
    file
  ) => {
    setAfterFile(file);
    setAfterPreview(null);
    setError('');

    if (!file) {
      return;
    }

    setPreviewingAfter(true);

    try {
      const result =
        await previewInventoryFile(file);

      setAfterPreview(result);
    } catch (err) {
      setAfterFile(null);

      setError(
        err.message ||
          translations.inventory
            .previewError
      );
    } finally {
      setPreviewingAfter(false);
    }
  };

  const handleCreateSession = async () => {
    if (!canSubmit) {
      return;
    }

    setIsSubmitting(true);
    setError('');

    try {
      const result =
        await confirmInventorySession({
          inventoryType,
          beforeFile,
          afterFile,
        });

      console.log(
        'Inventory session created:',
        result
      );

      navigate('/inventory-sessions');
    } catch (err) {
      setError(
        err.message ||
          translations.inventory
            .createSessionError
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleCancel = () => {
    if (isSubmitting) {
      return;
    }

    navigate('/inventory-sessions');
  };

  return (
    <section className={styles.page}>
      <SessionHeader />

      <SessionInfoCard
        value={inventoryType}
        onChange={setInventoryType}
      />

      <section className={styles.uploadSection}>
        <div className={styles.sectionHeader}>
          <h2>
            {translations.inventory.uploadFile}
          </h2>

          <p>
            {
              translations.inventory
                .uploadFileDescription
            }
          </p>
        </div>

        <div className={styles.uploadGrid}>
          <FileUpload
            title={
              translations.inventory
                .beforeInventory
            }
            description={
              translations.inventory
                .beforeInventoryDescription
            }
            file={beforeFile}
            onFileChange={
              handleBeforeFileChange
            }
            preview={beforePreview}
            previewing={previewingBefore}
          />

          <FileUpload
            title={
              translations.inventory
                .afterInventory
            }
            description={
              translations.inventory
                .afterInventoryDescription
            }
            file={afterFile}
            onFileChange={
              handleAfterFileChange
            }
            preview={afterPreview}
            previewing={previewingAfter}
          />
        </div>
      </section>

      <RequiredColumns />

      {error && (
        <div className={styles.error}>
          {error}
        </div>
      )}

      <SessionActions
        disabled={!canSubmit}
        loading={isSubmitting}
        onSubmit={handleCreateSession}
        onCancel={handleCancel}
      />
    </section>
  );
}

export default NewInventorySession;