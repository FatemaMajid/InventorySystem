import { useCallback, useEffect, useState } from "react";
import { useLanguage } from "../../context/LanguageContext";
import RolesPermissionsHeader from "../../components/RolesPermissions/RolesPermissionsHeader/RolesPermissionsHeader";
import RoleStats from "../../components/RolesPermissions/RoleStats/RoleStats";
import RolesList from "../../components/RolesPermissions/RolesList/RolesList";
import PermissionsPanel from "../../components/RolesPermissions/PermissionsPanel/PermissionsPanel";
import RoleForm from "../../components/RolesPermissions/RoleForm/RoleForm";
import {
    createRole,
    getPermissions,
    getRoleById,
    getRoles,
    updateRole,
} from "../../services/roleService";
import styles from "./RolesPermissions.module.css";

function RolesPermissions() {
    const { translations, direction } = useLanguage();
    const t = translations.rolesPermissions || {};

    const [roles, setRoles] = useState([]);
    const [permissions, setPermissions] = useState([]);
    const [selectedRole, setSelectedRole] = useState(null);

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [formOpen, setFormOpen] = useState(false);
    const [editingRole, setEditingRole] = useState(null);
    const [saving, setSaving] = useState(false);
    const [formError, setFormError] = useState("");

    const loadData = useCallback(async () => {
        setLoading(true);
        setError("");

        try {
            const [rolesResponse, permissionsResponse] =
                await Promise.all([
                    getRoles(),
                    getPermissions(),
                ]);

            const rolesData =
                rolesResponse?.data ?? rolesResponse;

            const permissionsData =
                permissionsResponse?.data ??
                permissionsResponse;

            const rolesList = Array.isArray(rolesData)
                ? rolesData
                : rolesData?.items ?? [];

            const permissionsList = Array.isArray(
                permissionsData
            )
                ? permissionsData
                : permissionsData?.items ?? [];

            setRoles(rolesList);
            setPermissions(permissionsList);

            if (selectedRole) {
                const exists = rolesList.some(
                    (role) => role.id === selectedRole.id
                );

                if (!exists) {
                    setSelectedRole(null);
                }
            }
        } catch (err) {
            console.error(
                "Failed to load roles and permissions:",
                err
            );

            setRoles([]);
            setPermissions([]);
            setSelectedRole(null);

            setError(
                err?.message ||
                    t.loadError ||
                    "Failed to load roles and permissions."
            );
        } finally {
            setLoading(false);
        }
    }, [selectedRole, t.loadError]);

    useEffect(() => {
        loadData();
    }, [loadData]);

    const selectRole = async (role) => {
        try {
            setError("");

            const response = await getRoleById(role.id);

            const roleData =
                response?.data ?? response;

            setSelectedRole(roleData);
        } catch (err) {
            setError(
                err?.message ||
                    t.loadDetailsError ||
                    "Failed to load role details."
            );
        }
    };

    const openCreate = () => {
        setEditingRole(null);
        setFormError("");
        setFormOpen(true);
    };

    const openEdit = async () => {
        if (!selectedRole) {
            return;
        }

        try {
            setFormError("");

            const response = await getRoleById(
                selectedRole.id
            );

            setEditingRole(
                response?.data ?? response
            );

            setFormOpen(true);
        } catch (err) {
            setFormError(
                err?.message ||
                    t.loadDetailsError ||
                    "Failed to load role details."
            );
        }
    };

    const closeForm = () => {
        if (saving) {
            return;
        }

        setFormOpen(false);
        setEditingRole(null);
        setFormError("");
    };

    const saveRole = async (form) => {
        setSaving(true);
        setFormError("");

        try {
            let response;

            if (editingRole) {
                response = await updateRole(
                    editingRole.id,
                    form
                );
            } else {
                response = await createRole(form);
            }

            const savedRole =
                response?.data ?? response;

            setFormOpen(false);
            setEditingRole(null);

            await loadData();

            if (savedRole?.id) {
                await selectRole(savedRole);
            }
        } catch (err) {
            setFormError(
                err?.message ||
                    t.saveError ||
                    "Failed to save role."
            );
        } finally {
            setSaving(false);
        }
    };

    const stats = {
        total: roles.length,
        permissions: permissions.length,
    };

    return (
        <main
            className={styles.page}
            dir={direction}
        >
            <RolesPermissionsHeader
                onAdd={openCreate}
            />

            <RoleStats
                totalRoles={stats.total}
                totalPermissions={stats.permissions}
                loading={loading}
            />

            <div className={styles.content}>
                <RolesList
                    roles={roles}
                    selectedRole={selectedRole}
                    loading={loading}
                    error={error}
                    onRetry={loadData}
                    onSelect={selectRole}
                />

                <PermissionsPanel
                    role={selectedRole}
                    permissions={permissions}
                    loading={loading}
                    onEdit={openEdit}
                />
            </div>

            <RoleForm
                open={formOpen}
                role={editingRole}
                permissions={permissions}
                saving={saving}
                error={formError}
                onClose={closeForm}
                onSubmit={saveRole}
            />
        </main>
    );
}

export default RolesPermissions;