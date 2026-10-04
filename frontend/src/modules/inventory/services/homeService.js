import apiClient from "../../../services/api/apiClient";

export async function getHomeDashboard() {
  return apiClient.get("/api/Home/dashboard");
}