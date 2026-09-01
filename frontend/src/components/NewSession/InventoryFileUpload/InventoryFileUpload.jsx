import { useRef, useState } from 'react';
import { useLanguage } from '../../../context/LanguageContext';
import Icon from '../../UI/Icon/Icon';
import styles from './InventoryFileUpload.module.css';

function InventoryFileUpload({ title, description, file, onFileChange, preview, previewing }) {
  const { translations } = useLanguage();
  const t = translations.inventory;
  const inputRef = useRef(null);
  const [error, setError] = useState('');

  const handleFile = (selectedFile) => {
    if (!selectedFile) return;

    const fileName = selectedFile.name.toLowerCase();
    const validExtension = fileName.endsWith('.xlsx') || fileName.endsWith('.xls');

    if (!validExtension) {
      setError(t.invalidExcelFile);
      return;
    }

    setError('');
    onFileChange(selectedFile);
  };

  const handleInputChange = (event) => {
    handleFile(event.target.files?.[0]);
  };

  const handleDrop = (event) => {
    event.preventDefault();
    handleFile(event.dataTransfer.files?.[0]);
  };

  const handleRemove = () => {
    setError('');
    onFileChange(null);

    if (inputRef.current) {
      inputRef.current.value = '';
    }
  };

  return (
    <section className={styles.card}>
      <div className={styles.header}>
        <h3>{title}</h3>
        <p>{description}</p>
      </div>

      <div className={styles.dropZone} onDragOver={(event) => event.preventDefault()} onDrop={handleDrop}>
        {!file ? (
          <>
            <div className={styles.uploadIcon}>
              <Icon name="arrowUp" size={22} />
            </div>
            <strong>{t.dropFile}</strong>
            <span>{t.or}</span>
            <button type="button" className={styles.chooseButton} onClick={() => inputRef.current?.click()}>
              {t.chooseFile}
            </button>
            <input ref={inputRef} type="file" accept=".xlsx,.xls" hidden onChange={handleInputChange} />
          </>
        ) : (
          <div className={styles.selectedFile}>
            <div className={styles.fileIcon}>
              <Icon name="clipboard" size={20} />
            </div>

            <div className={styles.fileInfo}>
              <strong>{file.name}</strong>
              <span>{(file.size / 1024 / 1024).toFixed(2)} MB</span>
            </div>

            <button type="button" className={styles.removeButton} onClick={handleRemove} disabled={previewing} aria-label={t.removeFile}>
              <Icon name="close" size={17} />
            </button>
          </div>
        )}

        {previewing && (
          <div className={styles.previewLoading}>
            <span className={styles.spinner} />
            <span>{t.checkingFile}</span>
          </div>
        )}

        {!previewing && preview && (
          <div className={styles.preview}>
            <div className={styles.previewHeader}>
              <span className={styles.successIcon}>
                <Icon name="tick" size={13} />
              </span>
              <strong>{t.fileValidated}</strong>
            </div>

            <div className={styles.stats}>
              <div>
                <span>{t.totalRows}</span>
                <strong>{preview.totalRows ?? 0}</strong>
              </div>
              <div>
                <span>{t.validRows}</span>
                <strong>{preview.validRows ?? 0}</strong>
              </div>
              <div>
                <span>{t.errorRows}</span>
                <strong>{preview.errorRows ?? 0}</strong>
              </div>
            </div>
          </div>
        )}

        {error && <div className={styles.error}>{error}</div>}
      </div>
    </section>
  );
}

export default InventoryFileUpload;