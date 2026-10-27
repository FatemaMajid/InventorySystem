import apiClient from "./api/apiClient";

export async function getAttentionItems({
  sessionId,
  pageNumber = 1,
  pageSize = 20,
  itemCode = "",
  attentionType = "",
  categoryId = null,
  unitId = null,
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

  if (attentionType && attentionType !== "All") {
    params.set("attentionType", attentionType);
  }

  if (categoryId !== null && categoryId !== undefined && categoryId !== "") {
    params.set("categoryId", categoryId);
  }

  if (unitId !== null && unitId !== undefined && unitId !== "") {
    params.set("unitId", unitId);
  }

  return apiClient.get(
    `/api/InventorySessions/${sessionId}/attention?${params.toString()}`
  );
}
