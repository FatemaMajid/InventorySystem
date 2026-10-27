import apiClient from "./api/apiClient";

export const getBranches = () => apiClient.get("/api/Branches");
export const getStores = () => apiClient.get("/api/Stores");