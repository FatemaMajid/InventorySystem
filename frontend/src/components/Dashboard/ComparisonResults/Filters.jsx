import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./Filters.module.css";

const Filters = ({
    filters,
    onChange,
    onApply,
    onClear,
    categories = [],
    units = [],
}) => {
    const { translations, direction } = useLanguage();

    const t = translations.dashboard.comparisonResults;

    const statusOptions = [
    { value: "All", label: t.all },
    { value: "Increase", label: t.statuses.increase },
    { value: "Decrease", label: t.statuses.decrease },
    { value: "NoDifference", label: t.statuses.noDifference },
    { value: "AfterOnly", label: t.statuses.afterOnly },
    { value: "BeforeOnly", label: t.statuses.beforeOnly },
];

    return (
        <aside
            className={styles.filters}
            dir={direction}
        >
            <div className={styles.header}>
                <div className={styles.title}>
                    <Icon name="filter" size={16} />
                    <span>{t.filters}</span>
                </div>

                <button
                    type="button"
                    className={styles.clear}
                    onClick={onClear}
                >
                    {t.clear}
                </button>
            </div>

            {/* STATUS */}
            <div className={styles.field}>
                <label>{t.status}</label>

                <select
                    value={filters.status}
                    onChange={(e) =>
                        onChange("status", e.target.value)
                    }
                >
                    {statusOptions.map((option) => (
                        <option
                            key={option.value}
                            value={option.value}
                        >
                            {option.label}
                        </option>
                    ))}
                </select>
            </div>

            {/* SEARCH */}
            <div className={styles.field}>
                <label>{t.search}</label>

                <div className={styles.searchBox}>
                    <input
                        type="text"
                        value={filters.search}
                        placeholder={t.searchPlaceholder}
                        onChange={(e) =>
                            onChange("search", e.target.value)
                        }
                    />

                    {/* <Icon name="search" size={16} /> */}
                </div>
            </div>

            {/* CATEGORY */}
            <div className={styles.field}>
                <label>{t.category}</label>

                <select
                    value={filters.category}
                    onChange={(e) =>
                        onChange(
                            "category",
                            e.target.value
                        )
                    }
                >
                    <option value="All">
                        {t.all}
                    </option>

                    {categories.map((category) => (
                        <option
                            key={category.id}
                            value={category.id}
                        >
                            {category.name}
                        </option>
                    ))}
                </select>
            </div>

            {/* UNIT */}
            <div className={styles.field}>
                <label>{t.unit}</label>

                <select
                    value={filters.unit}
                    onChange={(e) =>
                        onChange(
                            "unit",
                            e.target.value
                        )
                    }
                >
                    <option value="All">
                        {t.all}
                    </option>

                    {units.map((unit) => (
                        <option
                            key={unit.id}
                            value={unit.id}
                        >
                            {unit.name}
                        </option>
                    ))}
                </select>
            </div>
            <button
                type="button"
                className={styles.apply}
                onClick={onApply}
            >
                {t.applyFilters}
            </button>
        </aside>
    );
};

export default Filters;