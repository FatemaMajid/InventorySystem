import { useMemo } from "react";
import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./AuditLogFilters.module.css";

const AuditLogFilters = ({ filters, logs = [], onChange, onReset }) => {
    const { translations } = useLanguage();

    const options = useMemo(() => {
        const users = [...new Map(
            logs
                .filter((log) => log.userId !== null && log.userName)
                .map((log) => [log.userId, log.userName])
        ).entries()];

        const actions = [...new Set(
            logs
                .map((log) => log.action)
                .filter(Boolean)
        )];

        const entities = [...new Set(
            logs
                .map((log) => log.entity)
                .filter(Boolean)
        )];

        return { users, actions, entities };
    }, [logs]);

    if (!filters) {
        return null;
    }

    return (
        <section className={styles.filters}>
            <div className={styles.header}>
                <div className={styles.title}>
                    <Icon name="filter" size={20} />
                    <span>{translations.auditLog?.filters?.title}</span>
                </div>
                <button
                    type="button"
                    className={styles.reset}
                    onClick={onReset}
                >
                    {translations.auditLog?.filters?.reset}
                </button>
            </div>
            <div className={styles.fields}>
                <label className={styles.field}>
                    <span>{translations.auditLog?.filters?.user}</span>
                    <select
                        value={filters.user || ""}
                        onChange={(event) => onChange("user", event.target.value)}
                    >
                        <option value="">
                            {translations.auditLog?.filters?.all}
                        </option>
                        {options.users.map(([id, name]) => (
                            <option key={id} value={id}>
                                {name}
                            </option>
                        ))}
                    </select>
                </label>
                <label className={styles.field}>
                    <span>{translations.auditLog?.filters?.action}</span>
                    <select
                        value={filters.action || ""}
                        onChange={(event) => onChange("action", event.target.value)}
                    >
                        <option value="">
                            {translations.auditLog?.filters?.all}
                        </option>
                        {options.actions.map((action) => (
                            <option key={action} value={action}>
                                {action}
                            </option>
                        ))}
                    </select>
                </label>
                <label className={styles.field}>
                    <span>{translations.auditLog?.filters?.entity}</span>
                    <select
                        value={filters.entity || ""}
                        onChange={(event) => onChange("entity", event.target.value)}
                    >
                        <option value="">
                            {translations.auditLog?.filters?.all}
                        </option>
                        {options.entities.map((entity) => (
                            <option key={entity} value={entity}>
                                {entity}
                            </option>
                        ))}
                    </select>
                </label>
                <label className={styles.field}>
                    <span>{translations.auditLog?.filters?.dateFrom}</span>
                    <input
                        type="date"
                        value={filters.dateFrom || ""}
                        onChange={(event) => onChange("dateFrom", event.target.value)}
                    />
                </label>
                <label className={styles.field}>
                    <span>{translations.auditLog?.filters?.dateTo}</span>
                    <input
                        type="date"
                        value={filters.dateTo || ""}
                        onChange={(event) => onChange("dateTo", event.target.value)}
                    />
                </label>
            </div>
        </section>
    );
};

export default AuditLogFilters;