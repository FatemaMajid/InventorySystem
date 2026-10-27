import { useEffect, useState } from "react";
import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./RoleForm.module.css";

function RoleForm({
    open,
    role,
    permissions = [],
    saving = false,
    error = "",
    onClose,
    onSubmit,
}) {
    const { translations, direction } = useLanguage();
    const t = translations.rolesPermissions || {};

    const [form, setForm] = useState({
        name: "",
        description: "",
        permissionIds: [],
    });

    useEffect(() => {
        if (!open) {
            return;
        }

        setForm({
            name: role?.name || "",
            description: role?.description || "",
            permissionIds: role?.permissionIds || [],
        });
    }, [open, role]);

    if (!open) {
        return null;
    }

    const change = (field, value) => {
        setForm((previous) => ({
            ...previous,
            [field]: value,
        }));
    };

    const togglePermission = (id) => {
        setForm((previous) => ({
            ...previous,
            permissionIds: previous.permissionIds.includes(id)
                ? previous.permissionIds.filter((item) => item !== id)
                : [...previous.permissionIds, id],
        }));
    };

    const toggleGroup = (groupPermissions) => {
        const ids = groupPermissions.map((permission) => permission.id);
        const allSelected = ids.every((id) =>
            form.permissionIds.includes(id)
        );

        setForm((previous) => ({
            ...previous,
            permissionIds: allSelected
                ? previous.permissionIds.filter(
                      (id) => !ids.includes(id)
                  )
                : [...new Set([...previous.permissionIds, ...ids])],
        }));
    };

    const groupedPermissions = permissions.reduce((groups, permission) => {
        const [group] = permission.code.split(".");

        if (!groups[group]) {
            groups[group] = [];
        }

        groups[group].push(permission);
        return groups;
    }, {});

    const submit = (event) => {
        event.preventDefault();
        onSubmit?.(form);
    };

    return (
        <div className={styles.overlay} dir={direction} role="presentation">
            <div
                className={styles.modal}
                role="dialog"
                aria-modal="true"
                aria-labelledby="role-form-title"
            >
                <div className={styles.header}>
                    <div className={styles.heading}>
                        <div className={styles.iconBox}>
                            <Icon name="roles" size={20} />
                        </div>
                        <div>
                            <h2 id="role-form-title">
                                {role ? t.editRole : t.addRole}
                            </h2>
                            <p>{t.formDescription}</p>
                        </div>
                    </div>
                    <button
                        type="button"
                        className={styles.closeButton}
                        onClick={onClose}
                        disabled={saving}
                        aria-label={t.close}
                    >
                        <Icon name="close" size={18} />
                    </button>
                </div>

                <form onSubmit={submit}>
                    <div className={styles.body}>
                        {error && (
                            <div className={styles.error}>{error}</div>
                        )}

                        <div className={styles.fields}>
                            <label>
                                <span>{t.roleName}</span>
                                <input
                                    value={form.name}
                                    onChange={(event) =>
                                        change("name", event.target.value)
                                    }
                                    required
                                    maxLength={100}
                                />
                            </label>

                            <label>
                                <span>{t.roleDescription}</span>
                                <textarea
                                    value={form.description}
                                    onChange={(event) =>
                                        change(
                                            "description",
                                            event.target.value
                                        )
                                    }
                                    maxLength={500}
                                    rows={3}
                                />
                            </label>
                        </div>

                        <div className={styles.permissionsSection}>
                            <div className={styles.permissionsHeader}>
                                <div>
                                    <h3>{t.permissions}</h3>
                                    <p>{t.permissionsDescription}</p>
                                </div>
                                <span>
                                    {form.permissionIds.length} /{" "}
                                    {permissions.length}
                                </span>
                            </div>

                            <div className={styles.permissionGroups}>
                                {Object.entries(groupedPermissions).map(
                                    ([group, groupPermissions]) => {
                                        const selected = groupPermissions.filter(
                                            (permission) =>
                                                form.permissionIds.includes(
                                                    permission.id
                                                )
                                        ).length;

                                        return (
                                            <section
                                                key={group}
                                                className={styles.permissionGroup}
                                            >
                                                <button
                                                    type="button"
                                                    className={styles.groupHeader}
                                                    onClick={() =>
                                                        toggleGroup(
                                                            groupPermissions
                                                        )
                                                    }
                                                >
                                                    <span>
                                                        {t.permissionGroups?.[
                                                            group
                                                        ] || group}
                                                    </span>
                                                    <small>
                                                        {selected}/
                                                        {groupPermissions.length}
                                                    </small>
                                                </button>

                                                <div
                                                    className={
                                                        styles.permissionGrid
                                                    }
                                                >
                                                    {groupPermissions.map(
                                                        (permission) => (
                                                            <label
                                                                key={
                                                                    permission.id
                                                                }
                                                                className={
                                                                    styles.permissionItem
                                                                }
                                                            >
                                                                <input
                                                                    type="checkbox"
                                                                    checked={form.permissionIds.includes(
                                                                        permission.id
                                                                    )}
                                                                    onChange={() =>
                                                                        togglePermission(
                                                                            permission.id
                                                                        )
                                                                    }
                                                                />
                                                                <span>
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
                                                            </label>
                                                        )
                                                    )}
                                                </div>
                                            </section>
                                        );
                                    }
                                )}
                            </div>
                        </div>
                    </div>

                    <div className={styles.footer}>
                        <button
                            type="button"
                            className={styles.cancelButton}
                            onClick={onClose}
                            disabled={saving}
                        >
                            {t.cancel}
                        </button>
                        <button
                            type="submit"
                            className={styles.saveButton}
                            disabled={saving}
                        >
                            <Icon
                                name={saving ? "refresh" : "tick"}
                                size={17}
                            />
                            <span>
                                {saving ? t.saving : t.save}
                            </span>
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}

export default RoleForm;