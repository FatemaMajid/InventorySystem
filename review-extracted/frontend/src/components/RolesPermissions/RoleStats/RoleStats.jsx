import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./RoleStats.module.css";

function RoleStats({
    totalRoles = 0,
    totalPermissions = 0,
    loading = false,
}) {
    const { translations } = useLanguage();
    const t = translations.rolesPermissions || {};

    const stats = [
        {
            key: "roles",
            label: t.totalRoles,
            value: totalRoles,
            icon: "roles",
        },
        {
            key: "permissions",
            label: t.totalPermissions,
            value: totalPermissions,
            icon: "roles",
        },
    ];

    return (
        <section className={styles.stats}>
            {stats.map((stat) => (
                <div
                    key={stat.key}
                    className={styles.card}
                >
                    <div className={styles.iconBox}>
                        <Icon
                            name={stat.icon}
                            size={20}
                        />
                    </div>

                    <div className={styles.content}>
                        <span className={styles.label}>
                            {stat.label}
                        </span>

                        <strong className={styles.value}>
                            {loading ? "—" : stat.value}
                        </strong>
                    </div>
                </div>
            ))}
        </section>
    );
}

export default RoleStats;