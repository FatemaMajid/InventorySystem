import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./RolesList.module.css";

function RolesList({
    roles = [],
    selectedRole,
    loading = false,
    error = "",
    onRetry,
    onSelect,
}) {
    const { translations } = useLanguage();
    const t = translations.rolesPermissions || {};

    const getRoleName = (name) =>
        t.roleNames?.[name] || name;

    const getRoleDescription = (name, description) =>
        t.roleDescriptions?.[name] || description;

    if (loading) {
        return (
            <section className={styles.card}>
                <div className={styles.header}>
                    <div>
                        <h2>{t.roles}</h2>
                        <p>{t.rolesDescription}</p>
                    </div>
                </div>

                <div className={styles.loading}>
                    <span className={styles.loader} />
                </div>
            </section>
        );
    }

    if (error) {
        return (
            <section className={styles.card}>
                <div className={styles.header}>
                    <div>
                        <h2>{t.roles}</h2>
                        <p>{t.rolesDescription}</p>
                    </div>
                </div>

                <div className={styles.error}>
                    <div className={styles.errorIcon}>
                        <Icon name="alert" size={20} />
                    </div>

                    <p>{error}</p>

                    <button
                        type="button"
                        className={styles.retryButton}
                        onClick={onRetry}
                    >
                        <Icon name="refresh" size={16} />
                        <span>{t.retry}</span>
                    </button>
                </div>
            </section>
        );
    }

    return (
        <section className={styles.card}>
            <div className={styles.header}>
                <div>
                    <h2>{t.roles}</h2>
                    <p>{t.rolesDescription}</p>
                </div>

                <span className={styles.count}>
                    {roles.length}
                </span>
            </div>

            <div className={styles.list}>
                {roles.map((role) => {
                    const isSelected =
                        selectedRole?.id === role.id;

                    const roleName = getRoleName(role.name);
                    const roleDescription =
                        getRoleDescription(
                            role.name,
                            role.description
                        );

                    return (
                        <button
                            key={role.id}
                            type="button"
                            className={`${styles.roleItem} ${
                                isSelected
                                    ? styles.selected
                                    : ""
                            }`}
                            onClick={() => onSelect(role)}
                        >
                            <span className={styles.roleIcon}>
                                <Icon
                                    name="roles"
                                    size={18}
                                />
                            </span>

                            <span className={styles.roleInfo}>
                                <strong>{roleName}</strong>

                                {roleDescription && (
                                    <small>
                                        {roleDescription}
                                    </small>
                                )}
                            </span>

                            <span className={styles.roleMeta}>
                                <span>
                                    {role.permissionCount}
                                </span>

                                <Icon
                                    name="roles"
                                    size={15}
                                />
                            </span>
                        </button>
                    );
                })}
            </div>
        </section>
    );
}

export default RolesList;