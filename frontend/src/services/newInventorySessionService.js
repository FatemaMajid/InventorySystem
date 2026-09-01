    import apiClient from "./api/apiClient";

    export const getNewSessionBranches = () =>
    apiClient.get("/api/Branches");

    export const getNewSessionStores = () =>
    apiClient.get("/api/Stores");

    export const previewInventoryFile = (file) => {
    const formData = new FormData();
    formData.append("file", file);

    return apiClient.postForm(
        "/api/InventorySessions/import/preview",
        formData
    );
    };

    export const confirmInventorySession = ({
    inventoryType,
    beforeFile,
    afterFile,
    }) => {
    const formData = new FormData();

    formData.append("beforeFile", beforeFile);
    formData.append("afterFile", afterFile);

    return apiClient.postForm(
        `/api/InventorySessions/import/confirm?inventoryType=${inventoryType}`,
        formData
    );
    };