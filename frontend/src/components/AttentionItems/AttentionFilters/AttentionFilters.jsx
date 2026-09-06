import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./AttentionFilters.module.css";

function AttentionFilters({
  filters = {},
  onChange,
  onClear,
}) {
  const { translations, direction } = useLanguage();
  const t = translations.attentionItems;



  return (
    <section className={styles.wrapper} dir={direction}>
      <div className={styles.heading}>
        <Icon name="filter" size={18} />
        <span>{t.filters.title}</span>
      </div>

      <div className={styles.fields}>
        <label className={styles.field}>
          <span>{t.filters.search}</span>
          <div className={styles.inputWrapper}>
            <Icon name="search" size={17} />
            <input
              type="text"
              value={filters.itemCode || ""}
              onChange={(event) => onChange?.("itemCode", event.target.value)}
              placeholder={t.filters.search}
            />
          </div>
        </label>

        <label className={styles.field}>
          <span>{t.filters.attentionType}</span>
          <select
            value={filters.attentionType || "All"}
            onChange={(event) => onChange?.("attentionType", event.target.value)}
          >
            <option value="All">{t.filters.all}</option>
            <option value="NewlyCounted">{t.types.newlyCounted}</option>
            <option value="FullyDepleted">{t.types.fullyDepleted}</option>
            <option value="UnitNotDefined">{t.types.unitNotDefined}</option>
            <option value="PriceChanged">{t.types.priceChanged}</option>
          </select>
        </label>


      </div>

      <div className={styles.actions}>
        <button type="button" className={styles.clear} onClick={onClear}>
          {t.filters.clear}
        </button>
      </div>
    </section>
  );
}

export default AttentionFilters;
