import { useEffect, useState } from "react";
import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./StoreForm.module.css";

const emptyForm = {
  storeCode: "",
  storeNameArabic: "",
  storeNameEnglish: "",
  branchCode: "",
  isActive: true,
};

function StoreForm({
  open,
  store,
  branches = [],
  saving = false,
  error = "",
  onClose,
  onSubmit,
}) {
  const { translations, direction } = useLanguage();
  const t = translations.stores;
  const [form, setForm] = useState(emptyForm);

  useEffect(() => {
    if (!open) return;

    setForm(
      store
        ? {
            storeCode: store.storeCode ?? "",
            storeNameArabic: store.storeNameArabic ?? "",
            storeNameEnglish: store.storeNameEnglish ?? "",
            branchCode: store.branchCode ?? "",
            isActive: store.isActive !== false,
          }
        : emptyForm
    );
  }, [open, store]);

  if (!open) return null;

  const update = (key, value) => setForm((current) => ({ ...current, [key]: value }));

  const handleOverlayMouseDown = (event) => {
    if (event.target === event.currentTarget && !saving) onClose();
  };

  const handleSubmit = (event) => {
    event.preventDefault();
    onSubmit(form);
  };

  return (
    <div className={styles.overlay} role="presentation" onMouseDown={handleOverlayMouseDown}>
      <section className={styles.modal} dir={direction} role="dialog" aria-modal="true">
        <header className={styles.header}>
          <div>
            <h2>{store ? t.editStore : t.newStore}</h2>
            <p>{store ? t.editDescription : t.formDescription}</p>
          </div>
          <button
            type="button"
            className={styles.closeButton}
            onClick={onClose}
            disabled={saving}
            aria-label={t.close}
          >
            <Icon name="close" size={18} />
          </button>
        </header>

        <form onSubmit={handleSubmit}>
          <div className={styles.grid}>
            <label>
              <span>{t.code}</span>
              <input
                value={form.storeCode}
                onChange={(e) => update("storeCode", e.target.value)}
                maxLength={20}
                required
              />
            </label>

            <label>
              <span>{t.arabicName}</span>
              <input
                value={form.storeNameArabic}
                onChange={(e) => update("storeNameArabic", e.target.value)}
                maxLength={100}
                required
              />
            </label>

            <label>
              <span>{t.englishName}</span>
              <input
                value={form.storeNameEnglish}
                onChange={(e) => update("storeNameEnglish", e.target.value)}
                maxLength={100}
              />
            </label>

            <label>
              <span>{t.branch}</span>
              <select
                value={form.branchCode}
                onChange={(e) => update("branchCode", e.target.value)}
                required
              >
                <option value="">{t.selectBranch}</option>
                {branches.map((branch) => (
                  <option key={branch.id ?? branch.branchCode} value={branch.branchCode}>
                    {branch.branchNameArabic || branch.branchNameEnglish || branch.branchCode}
                  </option>
                ))}
              </select>
            </label>

            <label className={styles.checkbox}>
              <input
                type="checkbox"
                checked={form.isActive}
                onChange={(e) => update("isActive", e.target.checked)}
              />
              <span>{t.active}</span>
            </label>
          </div>

          {error ? <p className={styles.error}>{error}</p> : null}

          <footer className={styles.footer}>
            <button type="button" className={styles.secondary} onClick={onClose} disabled={saving}>
              {t.cancel}
            </button>
            <button type="submit" className={styles.primary} disabled={saving}>
              {saving ? t.saving : t.save}
            </button>
          </footer>
        </form>
      </section>
    </div>
  );
}

export default StoreForm;