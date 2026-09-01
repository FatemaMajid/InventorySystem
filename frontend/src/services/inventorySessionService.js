  import apiClient from "./api/apiClient";

  /* =================================
    Inventory Sessions
  ================================= */

  export async function getInventorySessions({
    pageNumber = 1,
    pageSize = 20,

    branchId,
    storeId,
    inventoryType,
    dateFrom,
    dateTo,
    status,
    sessionNumber,

    sortBy,
    descending,
  } = {}) {
    const params = new URLSearchParams();

    params.set("pageNumber", pageNumber);
    params.set("pageSize", pageSize);

    if (branchId) {
      params.set("branchId", branchId);
    }

    if (storeId) {
      params.set("storeId", storeId);
    }

    if (inventoryType) {
      params.set("inventoryType", inventoryType);
    }

    if (dateFrom) {
      params.set("dateFrom", dateFrom);
    }

    if (dateTo) {
      params.set("dateTo", dateTo);
    }

    if (status) {
      params.set("status", status);
    }

    if (sessionNumber) {
      params.set("sessionNumber", sessionNumber);
    }

    if (sortBy) {
      params.set("sortBy", sortBy);
    }

    if (descending !== undefined) {
      params.set("descending", descending);
    }

    return apiClient.get(
      `/api/InventorySessions?${params.toString()}`
    );
  }


  /* =================================
    Import - Preview
  ================================= */

  export async function previewInventoryFile(file) {
    const formData = new FormData();

    formData.append("file", file);

    return apiClient.postForm(
      "/api/InventorySessions/import/preview",
      formData
    );
  }


  /* =================================
    Import - Confirm
  ================================= */

  export async function confirmInventorySession({
    inventoryType,
    beforeFile,
    afterFile,
  }) {
    const formData = new FormData();

    formData.append(
      "beforeFile",
      beforeFile
    );

    formData.append(
      "afterFile",
      afterFile
    );

    return apiClient.postForm(
      `/api/InventorySessions/import/confirm?inventoryType=${inventoryType}`,
      formData
    );
  }