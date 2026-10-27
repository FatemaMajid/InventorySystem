import { getToken } from "./authService";

function getPermissions() {
    const token = getToken();

    if (!token) {
        return [];
    }

    try {
        const payload = JSON.parse(
            atob(token.split(".")[1])
        );

        const permissions = payload.permission || [];

        return Array.isArray(permissions)
            ? permissions
            : [permissions];
    } catch {
        return [];
    }
}

function getRole() {
    const token = getToken();

    if (!token) {
        return null;
    }

    try {
        const payload = JSON.parse(
            atob(token.split(".")[1])
        );

        const role = payload.role;

        if (Array.isArray(role)) {
            return role[0] || null;
        }

        return role || null;
    } catch {
        return null;
    }
}

export function hasPermission(permission) {
    if (!permission) {
        return true;
    }

    if (String(getRole()).toLowerCase() === "manager") {
        return true;
    }

    const permissions = getPermissions();

    return permissions.some(
        (userPermission) =>
            String(userPermission).toLowerCase() ===
            permission.toLowerCase()
    );
}

export function hasAnyPermission(permissions = []) {
    return permissions.some((permission) =>
        hasPermission(permission)
    );
}