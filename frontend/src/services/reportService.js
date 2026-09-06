import API_BASE_URL from "./api/apiConfig";

async function downloadReport(endpoint) {
  const token = localStorage.getItem("inventory_token");

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    headers: token ? { Authorization: `Bearer ${token}` } : {},
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || `Request failed with status ${response.status}.`);
  }

  const blob = await response.blob();
  const disposition = response.headers.get("content-disposition") || "";
  const match = disposition.match(/filename\*?=(?:UTF-8''|\")?([^;\"]+)/i);
  const fileName = match?.[1]
    ? decodeURIComponent(match[1].replaceAll('"', ""))
    : "Inventory_Report";

  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = fileName;
  document.body.appendChild(link);
  link.click();
  link.remove();
  URL.revokeObjectURL(url);
}

export function exportReport(sessionId, format, language = "ar") {
  if (!sessionId) {
    throw new Error("Session ID is required.");
  }

  const normalizedLanguage = language === "en" ? "en" : "ar";
  const endpoint =
    format === "pdf"
      ? `/api/InventorySessions/${sessionId}/dashboard/export/pdf?language=${normalizedLanguage}`
      : `/api/InventorySessions/${sessionId}/dashboard/export/excel?language=${normalizedLanguage}`;

  return downloadReport(endpoint);
}
