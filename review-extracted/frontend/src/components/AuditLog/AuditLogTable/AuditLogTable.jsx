import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./AuditLogTable.module.css";

const AuditLogTable = ({ logs, loading, onSelect }) => {
    const { translations, formatDateTime } = useLanguage();

    const getTranslatedValue = (group, value) => {
        return translations.auditLog?.[group]?.[value] || value;
    };

    return (
        <section className={styles.tableSection}>
            <div className={styles.header}>
                <div className={styles.title}>
                    <Icon name="clipboard" size={20} />
                    <span>{translations.auditLog?.table?.title}</span>
                </div>
            </div>
            <div className={styles.tableWrapper}>
                <table className={styles.table}>
                    <thead>
                        <tr>
                            <th>{translations.auditLog?.table?.date}</th>
                            <th>{translations.auditLog?.table?.user}</th>
                            <th>{translations.auditLog?.table?.action}</th>
                            <th>{translations.auditLog?.table?.entity}</th>
                            <th>{translations.auditLog?.table?.status}</th>
                            <th>{translations.auditLog?.table?.details}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {loading ? (
                            <tr>
                                <td colSpan="6" className={styles.state}>
                                    <Icon name="refresh" size={22} />
                                </td>
                            </tr>
                        ) : logs?.length ? (
                            logs.map((log) => (
                                <tr key={log.id}>
                                    <td>{formatDateTime(log.createdAt)}</td>
                                    <td>{log.userName}</td>
                                    <td>
                                        {getTranslatedValue(
                                            "actions",
                                            log.action
                                        )}
                                    </td>
                                    <td>
                                        {getTranslatedValue(
                                            "entities",
                                            log.entity
                                        )}
                                    </td>
                                    <td>
                                        <span
                                            className={`${styles.status} ${
                                                styles[log.status] || ""
                                            }`}
                                        >
                                            {getTranslatedValue(
                                                "status",
                                                log.status
                                            )}
                                        </span>
                                    </td>
                                    <td>
                                        <button
                                            type="button"
                                            className={styles.detailsButton}
                                            onClick={() => onSelect?.(log)}
                                            aria-label={
                                                translations.auditLog?.table
                                                    ?.details
                                            }
                                        >
                                            <Icon name="eye" size={18} />
                                        </button>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="6" className={styles.empty}>
                                    {translations.auditLog?.table?.empty}
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        </section>
    );
};

export default AuditLogTable;