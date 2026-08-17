import { useRef, useState } from 'react';
import { useLanguage } from '../../../context/LanguageContext';

import styles from './FileUpload.module.css';

function FileUpload({
  title,
  description,
  onFileChange,
}) {
  const { translations } = useLanguage();

  const inputRef = useRef(null);
  const [file, setFile] = useState(null);
  const [error, setError] = useState('');

  const handleFile = (selectedFile) => {
    if (!selectedFile) {
      return;
    }

    const validExtensions = ['.xlsx', '.xls'];

    const fileName = selectedFile.name.toLowerCase();

    const isExcel = validExtensions.some(
      (extension) => fileName.endsWith(extension)
    );

    if (!isExcel) {
      setFile(null);
      setError('Only Excel files are allowed.');
      return;
    }

    setError('');
    setFile(selectedFile);

    if (onFileChange) {
      onFileChange(selectedFile);
    }
  };

  const handleChange = (event) => {
    handleFile(event.target.files?.[0]);
  };

  const handleDrop = (event) => {
    event.preventDefault();

    handleFile(event.dataTransfer.files?.[0]);
  };

  const removeFile = () => {
    setFile(null);
    setError('');

    if (inputRef.current) {
      inputRef.current.value = '';
    }

    if (onFileChange) {
      onFileChange(null);
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
        onDragOver={(event) => event.preventDefault()}
        onDrop={handleDrop}
      >
        {file ? (
          <div className={styles.file}>
            <div className={styles.fileInfo}>
              <strong>{file.name}</strong>

              <span>
                {(file.size / 1024 / 1024).toFixed(2)} MB
              </span>
            </div>

            <button
              type="button"
              onClick={removeFile}
              className={styles.removeButton}
              aria-label="Remove file"
            >
              ×
            </button>
          </div>
        ) : (
          <>
            <div className={styles.uploadIcon}>
              ↑
            </div>

            <strong>
              {translations.inventory.dropFile}
            </strong>

            <span>
              {translations.inventory.or}
            </span>

            <button
              type="button"
              onClick={() => inputRef.current?.click()}
              className={styles.chooseButton}
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
        )}

        {error && (
          <span className={styles.error}>
            {error}
          </span>
        )}
      </div>
    </section>
  );
}

export default FileUpload;