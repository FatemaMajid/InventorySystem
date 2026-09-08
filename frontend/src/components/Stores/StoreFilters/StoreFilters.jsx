import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";

import styles from "./StoreFilters.module.css";

function StoreFilters({
  search = "",
  status = "all",
  branchCode = "",
  branches = [],
  onSearchChange,
  onStatusChange,
  onBranchChange,
  onReset,
}) {
  const { translations, direction } = useLanguage();
  const t = translations.stores;

  return (
    <section className={styles.card} dir={direction}>
      <div className={styles.searchWrapper}>
        <Icon name="search" size={18} />
        <input
          value={search}
          onChange={(event) => onSearchChange(event.target.value)}
          placeholder={t.searchPlaceholder}
          aria-label={t.searchPlaceholder}
        />
      </div>

      <select
        value={branchCode}
        onChange={(event) => onBranchChange(event.target.value)}
        aria-label={t.branch}
      >
        <option value="">{t.allBranches}</option>
        {branches.map((branch) => (
          <option key={branch.id ?? branch.branchCode} value={branch.branchCode}>
            {branch.branchNameArabic || branch.branchNameEnglish || branch.branchCode}
          </option>
        ))}
      </select>

      <select
        value={status}
        onChange={(event) => onStatusChange(event.target.value)}
        aria-label={t.status}
      >
        <option value="all">{t.allStatuses}</option>
        <option value="active">{t.active}</option>
        <option value="inactive">{t.inactive}</option>
      </select>

      <button type="button" className={styles.resetButton} onClick={onReset}>
        {t.reset}
      </button>
    </section>
  );
}

export default StoreFilters;