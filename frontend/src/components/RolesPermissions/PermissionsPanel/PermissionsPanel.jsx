import { useMemo } from "react";
import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./PermissionsPanel.module.css";

function PermissionsPanel({
    role,
    permissions = [],
    loading = false,
    onEdit,
}) {
    const { translations } = useLanguage();
    const t = translations.rolesPermissions || {};

    const groupedPermissions = useMemo(() => {
        return permissions.reduce((groups, permission) => {
            const [group] = permission.code.split(".");

            if (!groups[group]) {
                groups[group] = [];
            }

            groups[group].push(permission);

            return groups;
        }, {});
    }, [permissions]);

    const rolePermissionIds = useMemo(() => {
        return new Set(role?.permissionIds || []);
    }, [role]);

    const roleName =
        t.roleNames?.[role?.name] || role?.name;

    const roleDescription =
        t.roleDescriptions?.[role?.name] ||
        role?.description;

    if (loading) {
        return (
            <section className={styles.card}>
                <div className={styles.loading}>
                    <span className={styles.loader} />
                </div>
            </section>
        );
    }

    if (!role) {
        return (
            <section className={styles.card}>
                <div className={styles.empty}>
                    <div className={styles.emptyIcon}>
                        <Icon name="roles" size={22} />
                    </div>

                    <h2>{t.selectRoleTitle}</h2>
                    <p>{t.selectRoleDescription}</p>
                </div>
            </section>
        );
    }

    return (
        <section className={styles.card}>
            <div className={styles.header}>
                <div className={styles.heading}>
                    <div className={styles.iconBox}>
                        <Icon name="roles" size={20} />
                    </div>

                    <div>
                        <h2>{roleName}</h2>

                        {roleDescription && (
                            <p>{roleDescription}</p>
                        )}
                    </div>
                </div>

                <button
                    type="button"
                    className={styles.editButton}
                    onClick={onEdit}
                >
                    <Icon name="edit" size={16} />
                    <span>{t.editRole}</span>
                </button>
            </div>

            <div className={styles.summary}>
                <div className={styles.summaryItem}>
                    <Icon name="roles" size={16} />
                    <span>{t.permissions}</span>
                    <strong>
                        {rolePermissionIds.size}
                    </strong>
                </div>

                <div className={styles.summaryItem}>
                    <Icon name="users" size={16} />
                    <span>{t.users}</span>
                    <strong>{role.userCount ?? 0}</strong>
                </div>
            </div>

            <div className={styles.body}>
                <div className={styles.sectionHeader}>
                    <div>
                        <h3>{t.permissions}</h3>
                        <p>{t.permissionsDescription}</p>
                    </div>
                </div>

                <div className={styles.permissionGroups}>
                    {Object.entries(groupedPermissions).map(
                        ([group, groupPermissions]) => {
                            const selectedCount =
                                groupPermissions.filter(
                                    (permission) =>
                                        rolePermissionIds.has(
                                            permission.id
                                        )
                                ).length;

                            return (
                                <div
                                    key={group}
                                    className={styles.permissionGroup}
                                >
                                    <div
                                        className={
                                            styles.groupHeader
                                        }
                                    >
                                        <span>
                                            {t.permissionGroups?.[
                                                group
                                            ] || group}
                                        </span>

                                        <small>
                                            {selectedCount}/
                                            {
                                                groupPermissions.length
                                            }
                                        </small>
                                    </div>

                                    <div
                                        className={
                                            styles.permissionGrid
                                        }
                                    >
                                        {groupPermissions.map(
                                            (permission) => {
                                                const selected =
                                                    rolePermissionIds.has(
                                                        permission.id
                                                    );

                                                return (
                                                    <div
                                                        key={
                                                            permission.id
                                                        }
                                                        className={`${styles.permissionItem} ${selected
                                                                ? styles.selected
                                                                : ""
                                                            }`}
                                                    >
                                                        <span
                                                            className={
                                                                styles.permissionIcon
                                                            }
                                                        >
                                                            <Icon
                                                                name={
                                                                    selected
                                                                        ? "tick"
                                                                        : "lock"
                                                                }
                                                                size={14}
                                                            />
                                                        </span>

                                                        <span
                                                            className={
                                                                styles.permissionInfo
                                                            }
                                                        >
                                                            <strong>
                                                                {t.permissionNames?.[
                                                                    permission.code
                                                                ] ||
                                                                    permission.name}
                                                            </strong>

                                                            <small>
                                                                {
                                                                    permission.code
                                                                }
                                                            </small>
                                                        </span>
                                                    </div>
                                                );
                                            }
                                        )}
                                    </div>
                                </div>
                            );
                        }
                    )}
                </div>
            </div>
        </section>
    );
}

export default PermissionsPanel;