import apiClient from "./api/apiClient";

const auditLogService = {
    getAll(params = {}) {
        const query = new URLSearchParams();

        Object.entries(params).forEach(([key, value]) => {
            if (value !== undefined && value !== null && value !== "") {
                query.set(key, value);
            }
        });

        const queryString = query.toString();

        return apiClient.get(
            `/api/AuditLogs${queryString ? `?${queryString}` : ""}`
        );
    },

    getById(id) {
        return apiClient.get(`/api/AuditLogs/${id}`);
    }
};

export default auditLogService;