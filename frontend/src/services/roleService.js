import apiClient from "./api/apiClient";

export const getRoles = () =>
    apiClient.get("/api/Roles");

export const getRoleById = (id) =>
    apiClient.get(`/api/Roles/${id}`);

export const getPermissions = () =>
    apiClient.get("/api/Roles/permissions");

export const createRole = (role) =>
    apiClient.post("/api/Roles", role);

export const updateRole = (id, role) =>
    apiClient.put(`/api/Roles/${id}`, {
        ...role,
        id,
    });