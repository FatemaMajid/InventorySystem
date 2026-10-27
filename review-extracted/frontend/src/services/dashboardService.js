import apiClient from "./api/apiClient";

export async function getInventoryDashboard(sessionId) {
  if (!sessionId) {
    throw new Error("Session ID is required.");
  }

  return apiClient.get(
    `/api/InventorySessions/${sessionId}/dashboard`
  );
}

export async function exportDashboardExcel(sessionId, language = "ar") {
  if (!sessionId) {
    throw new Error("Session ID is required.");
  }

  return apiClient.get(
    `/api/InventorySessions/${sessionId}/dashboard/export/excel?language=${language}`,
    { responseType: "blob" }
  );
}

export async function exportDashboardPdf(sessionId, language = "ar") {
  if (!sessionId) {
    throw new Error("Session ID is required.");
  }

  return apiClient.get(
    `/api/InventorySessions/${sessionId}/dashboard/export/pdf?language=${language}`,
    { responseType: "blob" }
  );
}

export async function getInventoryComparison({
  sessionId,
  pageNumber = 1,
  pageSize = 20,
  itemCode = "",
  itemName = "",
  categoryId = null,
  unitId = null,
  status = "",
  sortBy = "ItemCode",
  descending = false,
}) {
  if (!sessionId) {
    throw new Error("Session ID is required.");
  }

  const params = new URLSearchParams();

  params.set("pageNumber", pageNumber);
  params.set("pageSize", pageSize);
  params.set("sortBy", sortBy);
  params.set("descending", descending);

  if (itemCode?.trim()) {
    params.set("itemCode", itemCode.trim());
  }

  if (itemName?.trim()) {
    params.set("itemName", itemName.trim());
  }

  if (
    categoryId !== null &&
    categoryId !== undefined &&
    categoryId !== ""
  ) {
    params.set("categoryId", categoryId);
  }

  if (
    unitId !== null &&
    unitId !== undefined &&
    unitId !== ""
  ) {
    params.set("unitId", unitId);
  }

  if (status && status !== "All") {
    params.set("status", status);
  }

  return apiClient.get(
    `/api/InventorySessions/${sessionId}/comparison?${params.toString()}`
  );
}

