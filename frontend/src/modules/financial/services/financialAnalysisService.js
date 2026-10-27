import apiClient from "../../../services/api/apiClient";

const FINANCIAL_BASE = "/api/financial-analysis";

function buildDashboardParams({
  periodId,
  branchId = "all",
  sections = "all",
  language = "ar",
}) {
  const params = new URLSearchParams();

  params.set("periodId", periodId);
  params.set("branchId", branchId || "all");
  params.set("sections", sections || "all");
  params.set("language", language === "en" ? "en" : "ar");

  return params.toString();
}

export async function getFinancialPeriods() {
  return apiClient.get(`${FINANCIAL_BASE}/periods`);
}

export async function getFinancialDashboard({
  periodId,
  branchId = "all",
  sections = "all",
}) {
  if (!periodId) {
    throw new Error("Financial period ID is required.");
  }

  const params = new URLSearchParams();

  params.set("periodId", periodId);
  params.set("branchId", branchId || "all");
  params.set("sections", sections || "all");

  return apiClient.get(
    `${FINANCIAL_BASE}/dashboard?${params.toString()}`
  );
}

export async function exportFinancialDashboard({
  periodId,
  branchId = "all",
  sections = "all",
  language = "ar",
  format = "pdf",
}) {
  if (!periodId) {
    throw new Error("Financial period ID is required.");
  }

  const normalizedFormat =
    format === "excel"
      ? "excel"
      : "pdf";

  const params = buildDashboardParams({
    periodId,
    branchId,
    sections,
    language,
  });

  return apiClient.get(
    `${FINANCIAL_BASE}/dashboard/export/${normalizedFormat}?${params}`,
    {
      responseType: "blob",
    }
  );
}
