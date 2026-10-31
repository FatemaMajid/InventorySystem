
import { getToken } from "./authService";

function getPayload() {
    const token = getToken();
    if (!token) return null;
    try {
        const parts = token.split(".");
        if (parts.length !== 3) return null;
        const base64 = parts[1].replace(/-/g, "+").replace(/_/g, "/");
        const padded = base64.padEnd(Math.ceil(base64.length / 4) * 4, "=");
        const binary = atob(padded);
        const bytes = Uint8Array.from(binary, (char) => char.charCodeAt(0));
        return JSON.parse(new TextDecoder().decode(bytes));
    } catch {
        return null;
    }
}

function getPermissions() {
    const payload = getPayload();
    const permissions = payload?.permission ?? [];
    return Array.isArray(permissions) ? permissions : [permissions];
}

function getRole() {
    const payload = getPayload();
    const role =
        payload?.role ??
        payload?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ??
        payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role"];
    return Array.isArray(role) ? role[0] ?? null : role ?? null;
}

export function hasPermission(permission) {
    if (!permission) return true;
    if (String(getRole()).toLowerCase() === "manager") return true;
    return getPermissions().some(
        (userPermission) =>
            String(userPermission).toLowerCase() === String(permission).toLowerCase()
    );
}

export function hasAnyPermission(permissions = []) {
    return permissions.some((permission) => hasPermission(permission));
}
