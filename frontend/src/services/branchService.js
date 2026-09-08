import apiClient from "./api/apiClient";

export const getBranches = () =>
  apiClient.get("/api/Branches");

export const getBranchById = (id) =>
  apiClient.get(`/api/Branches/${id}`);

export const createBranch = (branch) =>
  apiClient.post("/api/Branches", branch);

export const updateBranch = (id, branch) =>
  apiClient.put(`/api/Branches/${id}`, branch);

export const deleteBranch = (id) =>
  apiClient.delete(`/api/Branches/${id}`);
