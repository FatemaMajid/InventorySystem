import { useEffect, useMemo, useState } from "react";
import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./UserForm.module.css";

function UserForm({
    open,
    user,
    roles = [],
    permissions = [],
    saving = false,
    error = "",
    onClose,
    onSubmit,
}) {
    const { translations, direction } = useLanguage();
    const t = translations.users;
    const [form, setForm] = useState({
        username: "",
        password: "",
        role: "",
        permissions: [],
        isActive: true,
    });

    const getRolePermissions = (role) => {
        if (!role?.permissionIds?.length) {
            return [];
        }

        return permissions
            .filter((permission) =>
                role.permissionIds.includes(permission.id)
            )
            .map((permission) => permission.code);
    };

    useEffect(() => {
        if (!open) {
            return;
        }

        const selectedRole = roles.find(
            (role) =>
                role.name?.toLowerCase() === user?.role?.toLowerCase()
        );

        const rolePermissions = getRolePermissions(selectedRole);

        setForm({
            username: user?.username || "",
            password: "",
            role: user?.role || roles[0]?.name || "",
            permissions:
                user?.permissions?.length > 0
                    ? user.permissions
                    : rolePermissions,
            isActive: user?.isActive ?? true,
        });
    }, [open, user, roles, permissions]);

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

    if (!open) {
        return null;
    }

    const change = (field, value) => {
        setForm((previous) => ({
            ...previous,
            [field]: value,
        }));
    };

    const handleRoleChange = (roleName) => {
        const selectedRole = roles.find(
            (role) =>
                role.name?.toLowerCase() === roleName?.toLowerCase()
        );

        const rolePermissions = getRolePermissions(selectedRole);

        setForm((previous) => ({
            ...previous,
            role: roleName,
            permissions: rolePermissions,
        }));
    };

    const togglePermission = (code) => {
        setForm((previous) => ({
            ...previous,
            permissions: previous.permissions.includes(code)
                ? previous.permissions.filter((item) => item !== code)
                : [...previous.permissions, code],
        }));
    };

    const toggleGroup = (groupPermissions) => {
        const codes = groupPermissions.map((item) => item.code);
        const allSelected = codes.every((code) =>
            form.permissions.includes(code)
        );

        setForm((previous) => ({
            ...previous,
            permissions: allSelected
                ? previous.permissions.filter(
                    (code) => !codes.includes(code)
                )
                : [...new Set([...previous.permissions, ...codes])],
        }));
    };

    const submit = (event) => {
        event.preventDefault();
        onSubmit?.(form);
    };

    return (
        <div
            className={styles.overlay}
            dir={direction}
            role="presentation"
        >
            <div
                className={styles.modal}
                role="dialog"
                aria-modal="true"
                aria-labelledby="user-form-title"
            >
                <div className={styles.header}>
                    <div className={styles.heading}>
                        <div className={styles.iconBox}>
                            <Icon name="users" size={20} />
                        </div>

                        <div>
                            <h2 id="user-form-title">
                                {user ? t.editUser : t.addUser}
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
                            <div className={styles.error}>
                                {error}
                            </div>
                        )}

                        <div className={styles.fields}>
                            <label>
                                <span>{t.username}</span>
                                <input
                                    value={form.username}
                                    onChange={(event) =>
                                        change(
                                            "username",
                                            event.target.value
                                        )
                                    }
                                    required
                                    maxLength={100}
                                    autoComplete="off"
                                />
                            </label>

                            <label>
                                <span>
                                    {user ? t.newPassword : t.password}
                                </span>
                                <input
                                    type="password"
                                    value={form.password}
                                    onChange={(event) =>
                                        change(
                                            "password",
                                            event.target.value
                                        )
                                    }
                                    minLength={user ? undefined : 8}
                                    maxLength={200}
                                    required={!user}
                                    autoComplete="new-password"
                                    placeholder={
                                        user
                                            ? t.passwordOptional
                                            : ""
                                    }
                                />
                            </label>

                            <label>
                                <span>{t.role}</span>
                                <select
                                    value={form.role}
                                    onChange={(event) =>
                                        handleRoleChange(
                                            event.target.value
                                        )
                                    }
                                    required
                                >
                                    <option value="">
                                        {t.selectRole}
                                    </option>

                                    {roles.map((role) => (
                                        <option
                                            key={role.id}
                                            value={role.name}
                                        >
                                            {role.name}
                                        </option>
                                    ))}
                                </select>
                            </label>

                            <label className={styles.statusField}>
                                <span>{t.status}</span>
                                <button
                                    type="button"
                                    className={`${styles.statusToggle} ${form.isActive
                                            ? styles.on
                                            : ""
                                        }`}
                                    onClick={() =>
                                        change(
                                            "isActive",
                                            !form.isActive
                                        )
                                    }
                                    aria-pressed={form.isActive}
                                >
                                    <span>
                                        {form.isActive
                                            ? t.active
                                            : t.inactive}
                                    </span>
                                </button>
                            </label>
                        </div>

                        <div className={styles.permissionsSection}>
                            <div className={styles.permissionsHeader}>
                                <div>
                                    <h3>{t.permissions}</h3>
                                    <p>{t.permissionsDescription}</p>
                                </div>

                                <span>
                                    {form.permissions.length} /{" "}
                                    {permissions.length}
                                </span>
                            </div>

                            <div className={styles.permissionGroups}>
                                {Object.entries(
                                    groupedPermissions
                                ).map(
                                    ([
                                        group,
                                        groupPermissions,
                                    ]) => {
                                        const selected =
                                            groupPermissions.filter(
                                                (item) =>
                                                    form.permissions.includes(
                                                        item.code
                                                    )
                                            ).length;

                                        return (
                                            <section
                                                key={group}
                                                className={
                                                    styles.permissionGroup
                                                }
                                            >
                                                <button
                                                    type="button"
                                                    className={
                                                        styles.groupHeader
                                                    }
                                                    onClick={() =>
                                                        toggleGroup(
                                                            groupPermissions
                                                        )
                                                    }
                                                >
                                                    <span>
                                                        {
                                                            t
                                                                .permissionGroups?.[
                                                            group
                                                            ]
                                                            || group}
                                                    </span>

                                                    <small>
                                                        {selected}/
                                                        {
                                                            groupPermissions.length
                                                        }
                                                    </small>
                                                </button>

                                                <div
                                                    className={
                                                        styles.permissionGrid
                                                    }
                                                >
                                                    {groupPermissions.map(
                                                        (
                                                            permission
                                                        ) => (
                                                            <label
                                                                key={
                                                                    permission.code
                                                                }
                                                                className={
                                                                    styles.permissionItem
                                                                }
                                                            >
                                                                <input
                                                                    type="checkbox"
                                                                    checked={form.permissions.includes(
                                                                        permission.code
                                                                    )}
                                                                    onChange={() =>
                                                                        togglePermission(
                                                                            permission.code
                                                                        )
                                                                    }
                                                                />

                                                                <span>
                                                                    <strong>
                                                                        {
                                                                            t
                                                                                .permissionNames?.[
                                                                            permission.code
                                                                            ]
                                                                            || permission.name}
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

export default UserForm;