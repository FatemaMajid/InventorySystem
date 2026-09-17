import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import Skeleton from "../../UI/Loading/Skeleton/Skeleton";
import styles from "./UserStats.module.css";

function UserStats({ totalUsers = 0, activeUsers = 0, inactiveUsers = 0, loading = false }) {
    const { translations, direction } = useLanguage();
    const t = translations.users;
    const stats = [
        { key: "total", label: t.totalUsers, value: totalUsers, icon: "users", variant: "primary" },
        { key: "active", label: t.activeUsers, value: activeUsers, icon: "active", variant: "teal" },
        { key: "inactive", label: t.inactiveUsers, value: inactiveUsers, icon: "attention", variant: "warning" },
    ];

    return (
        <div className={styles.grid} dir={direction}>
            {stats.map((stat) => (
                <article key={stat.key} className={`${styles.card} ${styles[stat.variant]}`}>
                    <div className={styles.iconWrapper}>
                        <Icon name={stat.icon} size={20} />
                    </div>
                    <div className={styles.content}>
                        <span className={styles.label}>{stat.label}</span>
                        {loading ? (
                            <Skeleton width="65px" height="30px" borderRadius="6px" />
                        ) : (
                            <strong className={styles.value}>
                                {Number(stat.value || 0).toLocaleString(direction === "rtl" ? "en-US" : "en-US")}
                            </strong>
                        )}
                    </div>
                </article>
            ))}
        </div>
    );
}

export default UserStats;
