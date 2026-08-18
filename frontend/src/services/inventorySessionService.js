import apiClient from './api/apiClient';

export async function getInventorySessions({
  pageNumber = 1,
  pageSize = 20,
} = {}) {
  return apiClient.get(
    `/api/InventorySessions?pageNumber=${pageNumber}&pageSize=${pageSize}`
  );
}

export async function previewInventoryFile(file) {
  const formData = new FormData();

  formData.append('file', file);

  return apiClient.postForm(
    '/api/InventorySessions/import/preview',
    formData
  );
}

export async function confirmInventorySession({
  inventoryType,
  beforeFile,
  afterFile,
}) {
  const formData = new FormData();

  formData.append(
    'beforeFile',
    beforeFile
  );

  formData.append(
    'afterFile',
    afterFile
  );

  return apiClient.postForm(
    `/api/InventorySessions/import/confirm?inventoryType=${inventoryType}`,
    formData
  );
}