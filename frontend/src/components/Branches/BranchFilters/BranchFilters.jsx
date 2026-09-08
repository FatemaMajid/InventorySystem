import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";

import styles from "./BranchFilters.module.css";

function BranchFilters({ filters, onFiltersChange, onClear }) {
  const { translations, direction } = useLanguage();
  const t = translations.branches;

  const change = (field, value) => {
    onFiltersChange?.({
      ...filters,
      [field]: value,
    });
  };

  const hasFilters = Boolean(
    filters?.search || filters?.status
  );

  return (
    <section
      className={styles.filtersCard}
      dir={direction}
    >
      <div className={styles.searchWrapper}>
        <Icon name="search" size={18} />

        <input
          type="search"
          className={styles.searchInput}
          value={filters?.search || ""}
          onChange={(event) =>
            change("search", event.target.value)
          }
          placeholder={t.searchPlaceholder}
        />
      </div>

      <div className={styles.filters}>
        <select
          className={styles.select}
          value={filters?.status || ""}
          onChange={(event) =>
            change("status", event.target.value)
          }
          aria-label={t.status}
        >
          <option value="">{t.allStatuses}</option>
          <option value="active">{t.active}</option>
          <option value="inactive">{t.inactive}</option>
        </select>

        {hasFilters && (
          <button
            type="button"
            className={styles.clearButton}
            onClick={onClear}
          >
            <Icon name="refresh" size={16} />
            <span>{t.clear}</span>
          </button>
        )}
      </div>
    </section>
  );
}

export default BranchFilters;
