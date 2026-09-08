import apiClient from "./api/apiClient";

const TOKEN_KEY = "inventory_token";
const USER_KEY = "inventory_user";

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
        console.error("Login response:", response);
        throw new Error("Login succeeded but JWT token was not found.");
    }

    const user = {
        id: response?.userId ?? response?.id ?? null,
        username: response?.username ?? username,
        role: response?.role ?? "User",
        expiresAt: response?.expiresAt ?? null,
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
    const storedUser = localStorage.getItem(USER_KEY);

    if (!storedUser) {
        return null;
    }

    try {
        return JSON.parse(storedUser);
    } catch {
        localStorage.removeItem(USER_KEY);
        return null;
    }
}

export function isAuthenticated() {
    return Boolean(getToken());
}