import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./AuditLogStats.module.css";

const AuditLogStats = ({ stats }) => {
    const { translations } = useLanguage();

    if (!stats) {
        return null;
    }

    const items = [
        {
            key: "total",
            value: stats.total,
            icon: "history",
            className: styles.total
        },
        {
            key: "today",
            value: stats.today,
            icon: "calendar",
            className: styles.today
        },
        {
            key: "success",
            value: stats.success,
            icon: "tick",
            className: styles.success
        },
        {
            key: "failed",
            value: stats.failed,
            icon: "close",
            className: styles.failed
        }
    ];

    return (
        <section className={styles.stats}>
            {items.map((item) => (
                <div key={item.key} className={`${styles.card} ${item.className}`}>
                    <div className={styles.icon}>
                        <Icon name={item.icon} size={22} />
                    </div>
                    <div className={styles.content}>
                        <span className={styles.label}>
                            {translations.auditLog?.stats?.[item.key]}
                        </span>
                        <strong className={styles.value}>
                            {item.value ?? 0}
                        </strong>
                    </div>
                </div>
            ))}
        </section>
    );
};

export default AuditLogStats;