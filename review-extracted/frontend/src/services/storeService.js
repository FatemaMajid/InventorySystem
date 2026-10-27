import apiClient from "./api/apiClient";

/* =================================
   Stores
================================= */

export async function getStores() {
  return apiClient.get("/api/Stores");
}

export async function getStoreById(id) {
  return apiClient.get(`/api/Stores/${id}`);
}

export async function createStore(store) {
  return apiClient.post("/api/Stores", store);
}

export async function updateStore(id, store) {
  return apiClient.put(`/api/Stores/${id}`, store);
}

export async function deleteStore(id) {
  return apiClient.delete(`/api/Stores/${id}`);
}
