import { useCallback, useEffect, useMemo, useState } from "react";

import { useLanguage } from "../../context/LanguageContext";
import { hasPermission } from "../../services/permissionService";

import UserHeader from "../../components/Users/UserHeader/UserHeader";
import UserStats from "../../components/Users/UserStats/UserStats";
import UserFilters from "../../components/Users/UserFilters/UserFilters";
import UsersTable from "../../components/Users/UsersTable/UsersTable";
import UserForm from "../../components/Users/UserForm/UserForm";

import {
    createUser,
    deleteUser,
    getUserById,
    getUserOptions,
    getUsers,
    updateUser,
} from "../../services/userService";

import styles from "./Users.module.css";

function Users() {
    const { translations, direction } = useLanguage();
    const t = translations.users || {};

    const canCreate = hasPermission("User.Create");
    const canEdit = hasPermission("User.Edit");
    const canDeactivate = hasPermission("User.Deactivate");

    const [users, setUsers] = useState([]);
    const [roles, setRoles] = useState([]);
    const [permissions, setPermissions] = useState([]);

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [saving, setSaving] = useState(false);
    const [formError, setFormError] = useState("");

    const [filters, setFilters] = useState({
        search: "",
        status: "",
        role: "",
    });

    const [currentPage, setCurrentPage] = useState(1);
    const [pageSize, setPageSize] = useState(20);

    const [formOpen, setFormOpen] = useState(false);
    const [editingUser, setEditingUser] = useState(null);

    const loadData = useCallback(async () => {
        setLoading(true);
        setError("");

        try {
            const [usersResponse, optionsResponse] =
                await Promise.all([
                    getUsers(),
                    getUserOptions(),
                ]);

            const usersData =
                usersResponse?.data ?? usersResponse;

            const optionsData =
                optionsResponse?.data ?? optionsResponse;

            setUsers(
                Array.isArray(usersData)
                    ? usersData
                    : usersData?.items ?? []
            );

            setRoles(optionsData?.roles ?? []);
            setPermissions(optionsData?.permissions ?? []);
        } catch (err) {
            console.error(
                "Failed to load users:",
                err
            );

            setUsers([]);

            setError(
                err?.message ||
                    t.loadError ||
                    "Failed to load users."
            );
        } finally {
            setLoading(false);
        }
    }, [t.loadError]);

    useEffect(() => {
        loadData();
    }, [loadData]);

    const filteredUsers = useMemo(() => {
        const search =
            filters.search.trim().toLowerCase();

        return users
            .filter((user) => {
                const matchesSearch =
                    !search ||
                    [
                        user.username,
                        user.role,
                    ].some((value) =>
                        String(value ?? "")
                            .toLowerCase()
                            .includes(search)
                    );

                const matchesStatus =
                    !filters.status ||
                    (
                        filters.status === "active" &&
                        user.isActive
                    ) ||
                    (
                        filters.status === "inactive" &&
                        !user.isActive
                    );

                const matchesRole =
                    !filters.role ||
                    String(user.role ?? "") ===
                        String(filters.role);

                return (
                    matchesSearch &&
                    matchesStatus &&
                    matchesRole
                );
            })
            .sort((a, b) =>
                String(a.username ?? "").localeCompare(
                    String(b.username ?? ""),
                    undefined,
                    {
                        sensitivity: "base",
                    }
                )
            );
    }, [users, filters]);

    const totalItems = filteredUsers.length;

    const totalPages = Math.max(
        1,
        Math.ceil(totalItems / pageSize)
    );

    useEffect(() => {
        if (currentPage > totalPages) {
            setCurrentPage(totalPages);
        }
    }, [currentPage, totalPages]);

    const paginatedUsers = useMemo(() => {
        const start =
            (currentPage - 1) * pageSize;

        return filteredUsers.slice(
            start,
            start + pageSize
        );
    }, [
        filteredUsers,
        currentPage,
        pageSize,
    ]);

    const stats = useMemo(() => {
        const active = users.filter(
            (user) => user.isActive
        ).length;

        return {
            total: users.length,
            active,
            inactive: users.length - active,
        };
    }, [users]);

    const changeFilters = (nextFilters) => {
        setFilters(nextFilters);
        setCurrentPage(1);
    };

    const openCreate = () => {
        if (!canCreate) {
            return;
        }

        setEditingUser(null);
        setFormError("");
        setFormOpen(true);
    };

    const openEdit = async (user) => {
        if (!canEdit) {
            return;
        }

        try {
            setFormError("");

            const response =
                await getUserById(user.id);

            setEditingUser(
                response?.data ?? response
            );

            setFormOpen(true);
        } catch (err) {
            setFormError(
                err?.message ||
                    t.loadDetailsError ||
                    "Failed to load user details."
            );
        }
    };

    const closeForm = () => {
        if (saving) {
            return;
        }

        setFormOpen(false);
        setEditingUser(null);
        setFormError("");
    };

    const saveUser = async (form) => {
        if (editingUser && !canEdit) {
            return;
        }

        if (!editingUser && !canCreate) {
            return;
        }

        setSaving(true);
        setFormError("");

        try {
            if (editingUser) {
                await updateUser(
                    editingUser.id,
                    form
                );
            } else {
                await createUser(form);
            }

            closeForm();
            await loadData();
        } catch (err) {
            setFormError(
                err?.message ||
                    t.saveError ||
                    "Failed to save user."
            );
        } finally {
            setSaving(false);
        }
    };

    const removeUser = async (user) => {
        if (!canDeactivate) {
            return;
        }

        const confirmed = window.confirm(
            (
                t.deleteConfirmation ||
                "Are you sure you want to delete {name}?"
            ).replace(
                "{name}",
                user.username
            )
        );

        if (!confirmed) {
            return;
        }

        try {
            setError("");

            await deleteUser(user.id);
            await loadData();
        } catch (err) {
            setError(
                err?.message ||
                    t.deleteError ||
                    "Failed to delete user."
            );
        }
    };

    return (
        <main
            className={styles.page}
            dir={direction}
        >
            <UserHeader
                onAdd={openCreate}
                canCreate={canCreate}
            />

            <UserStats
                totalUsers={stats.total}
                activeUsers={stats.active}
                inactiveUsers={stats.inactive}
                loading={loading}
            />

            <UserFilters
                filters={filters}
                roles={roles}
                onFiltersChange={changeFilters}
                onClear={() =>
                    changeFilters({
                        search: "",
                        status: "",
                        role: "",
                    })
                }
            />

            <UsersTable
                users={paginatedUsers}
                loading={loading}
                error={error}
                onRetry={loadData}
                onEdit={openEdit}
                onDelete={removeUser}
                canEdit={canEdit}
                canDeactivate={canDeactivate}
                currentPage={currentPage}
                totalPages={totalPages}
                pageSize={pageSize}
                totalItems={totalItems}
                onPageChange={setCurrentPage}
                onPageSizeChange={(size) => {
                    setPageSize(Number(size));
                    setCurrentPage(1);
                }}
            />

            <UserForm
                open={formOpen}
                user={editingUser}
                roles={roles}
                permissions={permissions}
                saving={saving}
                error={formError}
                onClose={closeForm}
                onSubmit={saveUser}
            />
        </main>
    );
}

export default Users;