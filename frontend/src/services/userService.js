import apiClient from "./api/apiClient";

export const getUsers = () =>
    apiClient.get("/api/Users");

export const getUserById = (id) =>
    apiClient.get(`/api/Users/${id}`);

export const getUserOptions = () =>
    apiClient.get("/api/Users/options");

export const createUser = (user) =>
    apiClient.post("/api/Users", user);

export const updateUser = (id, user) =>
    apiClient.put(`/api/Users/${id}`, {
        ...user,
        id,
    });

export const deleteUser = (id) =>
    apiClient.delete(`/api/Users/${id}`);