import { useEffect, useState } from "react";
import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";

import styles from "./BranchForm.module.css";

const emptyForm = {
  branchCode: "",
  branchNameArabic: "",
  branchNameEnglish: "",
  address: "",
  phone: "",
  isActive: true,
};

function BranchForm({
  branch = null,
  loading = false,
  error = "",
  onSubmit,
  onClose,
}) {
  const { translations, direction } = useLanguage();
  const t = translations.branches;

  const [form, setForm] = useState(emptyForm);

  useEffect(() => {
    if (branch) {
      setForm({
        branchCode: branch.branchCode || "",
        branchNameArabic: branch.branchNameArabic || "",
        branchNameEnglish: branch.branchNameEnglish || "",
        address: branch.address || "",
        phone: branch.phone || "",
        isActive: branch.isActive ?? true,
      });
      return;
    }

    setForm(emptyForm);
  }, [branch]);

  const change = (field, value) => {
    setForm((current) => ({
      ...current,
      [field]: value,
    }));
  };

  const submit = (event) => {
    event.preventDefault();
    onSubmit?.(form);
  };

  return (
    <div
      className={styles.overlay}
      role="presentation"
      onMouseDown={(event) => {
        if (event.target === event.currentTarget) {
          onClose?.();
        }
      }}
    >
      <section
        className={styles.modal}
        dir={direction}
        role="dialog"
        aria-modal="true"
        aria-labelledby="branch-form-title"
      >
        <header className={styles.header}>
          <div>
            <h2
              id="branch-form-title"
              className={styles.title}
            >
              {branch ? t.editBranch : t.addBranch}
            </h2>
            <p className={styles.description}>
              {t.formDescription}
            </p>
          </div>

          <button
            type="button"
            className={styles.closeButton}
            onClick={onClose}
            aria-label={t.close}
          >
            <Icon name="close" size={18} />
          </button>
        </header>

        <form
          className={styles.form}
          onSubmit={submit}
        >
          <div className={styles.grid}>
            <label className={styles.field}>
              <span>{t.code}</span>
              <input
                value={form.branchCode}
                onChange={(event) =>
                  change("branchCode", event.target.value)
                }
                maxLength={20}
                required
                autoComplete="off"
              />
            </label>

            <label className={styles.field}>
              <span>{t.arabicName}</span>
              <input
                value={form.branchNameArabic}
                onChange={(event) =>
                  change("branchNameArabic", event.target.value)
                }
                maxLength={100}
                required
                autoComplete="off"
              />
            </label>

            <label className={styles.field}>
              <span>{t.englishName}</span>
              <input
                value={form.branchNameEnglish}
                onChange={(event) =>
                  change("branchNameEnglish", event.target.value)
                }
                maxLength={100}
                autoComplete="off"
              />
            </label>

            <label className={styles.field}>
              <span>{t.phone}</span>
              <input
                value={form.phone}
                onChange={(event) =>
                  change("phone", event.target.value)
                }
                maxLength={20}
                autoComplete="tel"
              />
            </label>

            <label className={`${styles.field} ${styles.full}`}>
              <span>{t.address}</span>
              <input
                value={form.address}
                onChange={(event) =>
                  change("address", event.target.value)
                }
                maxLength={200}
                autoComplete="street-address"
              />
            </label>
          </div>

          <label className={styles.checkbox}>
            <input
              type="checkbox"
              checked={Boolean(form.isActive)}
              onChange={(event) =>
                change("isActive", event.target.checked)
              }
            />
            <span>{t.active}</span>
          </label>

          {error && (
            <div className={styles.error}>
              {error}
            </div>
          )}

          <footer className={styles.actions}>
            <button
              type="button"
              className={styles.cancelButton}
              onClick={onClose}
              disabled={loading}
            >
              {t.cancel}
            </button>

            <button
              type="submit"
              className={styles.submitButton}
              disabled={loading}
            >
              {loading ? t.saving : t.save}
            </button>
          </footer>
        </form>
      </section>
    </div>
  );
}

export default BranchForm;
