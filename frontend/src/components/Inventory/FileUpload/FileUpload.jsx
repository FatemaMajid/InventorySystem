import { useRef, useState } from 'react';
import { useLanguage } from '../../../context/LanguageContext';

import styles from './FileUpload.module.css';

function FileUpload({
  title,
  description,
  file,
  onFileChange,
  preview = null,
  previewing = false,
}) {
  const { translations } = useLanguage();

  const inputRef = useRef(null);

  const [error, setError] = useState('');

  const handleFile = (selectedFile) => {
    if (!selectedFile) {
      return;
    }

    const fileName =
      selectedFile.name.toLowerCase();

    const isExcel =
      fileName.endsWith('.xlsx') ||
      fileName.endsWith('.xls');

    if (!isExcel) {
      setError(
        translations.inventory.invalidExcelFile
      );

      onFileChange(null);

      if (inputRef.current) {
        inputRef.current.value = '';
      }

      return;
    }

    setError('');

    onFileChange(selectedFile);
  };

  const handleChange = (event) => {
    handleFile(event.target.files?.[0]);
  };

  const handleDrop = (event) => {
    event.preventDefault();

    handleFile(
      event.dataTransfer.files?.[0]
    );
  };

  const removeFile = () => {
    onFileChange(null);
    setError('');

    if (inputRef.current) {
      inputRef.current.value = '';
    }
  };

  return (
    <section className={styles.card}>
      <div className={styles.header}>
        <h2>{title}</h2>

        <p>{description}</p>
      </div>

      <div
        className={styles.dropZone}
        onDragOver={(event) =>
          event.preventDefault()
        }
        onDrop={handleDrop}
      >
        {!file ? (
          <>
            <div className={styles.uploadIcon}>
              ↑
            </div>

            <strong>
              {translations.inventory.dropFile}
            </strong>

            <span className={styles.or}>
              {translations.inventory.or}
            </span>

            <button
              type="button"
              className={styles.chooseButton}
              onClick={() =>
                inputRef.current?.click()
              }
            >
              {translations.inventory.chooseFile}
            </button>

            <input
              ref={inputRef}
              type="file"
              accept=".xlsx,.xls"
              hidden
              onChange={handleChange}
            />
          </>
        ) : (
          <div className={styles.selectedFile}>
            <div className={styles.fileIcon}>
              XLS
            </div>

            <div className={styles.fileInfo}>
              <strong>
                {file.name}
              </strong>

              <span>
                {(
                  file.size /
                  1024 /
                  1024
                ).toFixed(2)} MB
              </span>
            </div>

            <button
              type="button"
              className={styles.removeButton}
              onClick={removeFile}
              disabled={previewing}
              aria-label={
                translations.inventory.removeFile
              }
            >
              ×
            </button>
          </div>
        )}

        {previewing && (
          <div className={styles.previewLoading}>
            <span className={styles.spinner} />

            <span>
              {
                translations.inventory
                  .checkingFile
              }
            </span>
          </div>
        )}

        {!previewing &&
          preview && (
            <div className={styles.previewResult}>
              <div className={styles.resultHeader}>
                <span className={styles.successIcon}>
                  ✓
                </span>

                <strong>
                  {
                    translations.inventory
                      .fileValidated
                  }
                </strong>
              </div>

              <div className={styles.resultStats}>
                <div>
                  <span>
                    {
                      translations.inventory
                        .totalRows
                    }
                  </span>

                  <strong>
                    {preview.totalRows ?? 0}
                  </strong>
                </div>

                <div>
                  <span>
                    {
                      translations.inventory
                        .validRows
                    }
                  </span>

                  <strong>
                    {preview.validRows ?? 0}
                  </strong>
                </div>

                <div>
                  <span>
                    {
                      translations.inventory
                        .errorRows
                    }
                  </span>

                  <strong>
                    {preview.errorRows ?? 0}
                  </strong>
                </div>
              </div>
            </div>
          )}

        {error && (
          <div className={styles.error}>
            {error}
          </div>
        )}
      </div>
    </section>
  );
}

export default FileUpload;