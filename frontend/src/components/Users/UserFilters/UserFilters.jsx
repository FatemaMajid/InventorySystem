import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./UserFilters.module.css";

function UserFilters({ filters, roles = [], onFiltersChange, onClear }) {
    const { translations, direction } = useLanguage();
    const t = translations.users;
    const change = (field, value) => {
        onFiltersChange?.({ ...filters, [field]: value });
    };
    const hasFilters = Boolean(filters?.search || filters?.status || filters?.role);

    return (
        <section className={styles.card} dir={direction}>
            <div className={styles.searchBox}>
                <Icon name="search" size={18} />
                <input
                    type="search"
                    value={filters?.search || ""}
                    onChange={(event) => change("search", event.target.value)}
                    placeholder={t.searchPlaceholder}
                    aria-label={t.search}
                />
            </div>
            <div className={styles.controls}>
                <select value={filters?.status || ""} onChange={(event) => change("status", event.target.value)} aria-label={t.status}>
                    <option value="">{t.allStatuses}</option>
                    <option value="active">{t.active}</option>
                    <option value="inactive">{t.inactive}</option>
                </select>
                <select value={filters?.role || ""} onChange={(event) => change("role", event.target.value)} aria-label={t.role}>
                    <option value="">{t.allRoles}</option>
                    {roles.map((role) => <option key={role.id} value={role.name}>{role.name}</option>)}
                </select>
                {hasFilters && (
                    <button type="button" className={styles.clearButton} onClick={onClear}>
                        <Icon name="refresh" size={16} />
                        <span>{t.clear}</span>
                    </button>
                )}
            </div>
        </section>
    );
}

export default UserFilters;
