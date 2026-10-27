import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";

import styles from "./ComparisonFilters.module.css";

function ComparisonFilters({
  filters = {},
  onChange,
  onClear,
}) {
  const { translations, direction } =
    useLanguage();

  const t =
    translations.dashboard.comparisonResults;

  return (
    <section
      className={styles.wrapper}
      dir={direction}
    >
      <div className={styles.search}>
        <Icon
          name="search"
          size={18}
        />

        <input
          type="search"
          value={filters.search || ""}
          onChange={(event) =>
            onChange?.(
              "search",
              event.target.value
            )
          }
          placeholder={t.search}
        />
      </div>

      <select
        value={filters.status || "All"}
        onChange={(event) =>
          onChange?.(
            "status",
            event.target.value
          )
        }
        aria-label={t.status}
      >
        <option value="All">
          {t.all}
        </option>

        <option value="Increase">
          {t.statuses?.increase}
        </option>

        <option value="Decrease">
          {t.statuses?.decrease}
        </option>

        <option value="NoDifference">
          {t.statuses?.noDifference}
        </option>

        <option value="AfterOnly">
          {t.statuses?.afterOnly}
        </option>

        <option value="BeforeOnly">
          {t.statuses?.beforeOnly}
        </option>
      </select>

      <button
        type="button"
        className={styles.clear}
        onClick={onClear}
      >
        <Icon
          name="refresh"
          size={16}
        />

        <span>{t.clear}</span>
      </button>
    </section>
  );
}

export default ComparisonFilters;