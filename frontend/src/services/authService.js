
import apiClient from "./api/apiClient";

const TOKEN_KEY = "inventory_token";
const USER_KEY = "inventory_user";
const ROLE_CLAIM = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
const NAME_CLAIM = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

function decodeToken(token) {
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

function getRoleFromPayload(payload) {
    const role = payload?.role ?? payload?.[ROLE_CLAIM];
    return Array.isArray(role) ? role[0] ?? null : role ?? null;
}

function getExpiry(payload) {
    if (typeof payload?.exp === "number") {
        return new Date(payload.exp * 1000).toISOString();
    }
    return null;
}

export async function login(username, password) {
    const response = await apiClient.post("/api/Auth/login", {
        username,
        password,
    });

    const token =
        response?.token ||
        response?.accessToken ||
        response?.access_token;

    if (!token) {
        throw new Error("Login succeeded but JWT token was not found.");
    }

    const payload = decodeToken(token);

    if (!payload) {
        throw new Error("Login returned an invalid JWT.");
    }

    const user = {
        id:
            response?.userId ??
            response?.id ??
            payload?.sub ??
            payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ??
            null,
        username:
            response?.username ??
            payload?.unique_name ??
            payload?.[NAME_CLAIM] ??
            username,
        role: response?.role ?? getRoleFromPayload(payload) ?? "User",
        expiresAt: response?.expiresAt ?? getExpiry(payload),
    };

    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(USER_KEY, JSON.stringify(user));

    return {
        ...response,
        token,
        user,
    };
}

export function logout() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
}

export function getToken() {
    return localStorage.getItem(TOKEN_KEY);
}

export function getCurrentUser() {
    const token = getToken();
    const storedUser = localStorage.getItem(USER_KEY);

    if (!token || !storedUser) {
        return null;
    }

    try {
        const payload = decodeToken(token);
        const user = JSON.parse(storedUser);

        if (!payload) {
            logout();
            return null;
        }

        if (
            typeof payload.exp === "number" &&
            payload.exp * 1000 <= Date.now()
        ) {
            logout();
            return null;
        }

        if (user?.expiresAt) {
            const expiresAt = new Date(user.expiresAt).getTime();

            if (!Number.isNaN(expiresAt) && expiresAt <= Date.now()) {
                logout();
                return null;
            }
        }

        return user;
    } catch {
        logout();
        return null;
    }
}

export function isAuthenticated() {
    return Boolean(getCurrentUser());
}
