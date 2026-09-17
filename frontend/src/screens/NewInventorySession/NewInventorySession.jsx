import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useLanguage } from "../../context/LanguageContext";
import { useInventorySession } from "../../context/InventorySessionContext";
import { hasPermission } from "../../services/permissionService";
import { getBranches, getStores } from "../../services/masterDataService";
import { previewInventoryFile, confirmInventorySession } from "../../services/inventorySessionService";
import NewSessionHeader from "../../components/NewSession/NewSessionHeader/NewSessionHeader";
import InventorySessionInfo from "../../components/NewSession/InventorySessionInfo/InventorySessionInfo";
import InventoryFileUpload from "../../components/NewSession/InventoryFileUpload/InventoryFileUpload";
import RequiredColumns from "../../components/NewSession/RequiredColumns/RequiredColumns";
import NewSessionActions from "../../components/NewSession/NewSessionActions/NewSessionActions";
import styles from "./NewInventorySession.module.css";

function NewInventorySession() {
  const navigate = useNavigate();
  const { translations, direction } = useLanguage();
  const { setActiveSession } = useInventorySession();
  const t = translations.inventory;
  const canCreateInventory = hasPermission("InventorySession.Create");

  const [inventoryType, setInventoryType] = useState(0);
  const [branchId, setBranchId] = useState("");
  const [storeId, setStoreId] = useState("");
  const [branches, setBranches] = useState([]);
  const [stores, setStores] = useState([]);
  const [beforeFile, setBeforeFile] = useState(null);
  const [afterFile, setAfterFile] = useState(null);
  const [beforePreview, setBeforePreview] = useState(null);
  const [afterPreview, setAfterPreview] = useState(null);
  const [previewingBefore, setPreviewingBefore] = useState(false);
  const [previewingAfter, setPreviewingAfter] = useState(false);
  const [loadingMasterData, setLoadingMasterData] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!canCreateInventory) {
      navigate("/inventory-sessions", { replace: true });
    }
  }, [canCreateInventory, navigate]);

  useEffect(() => {
    let cancelled = false;

    const loadMasterData = async () => {
      try {
        setLoadingMasterData(true);

        const [branchesResponse, storesResponse] = await Promise.all([
          getBranches(),
          getStores(),
        ]);

        if (cancelled) return;

        const branchesData = branchesResponse?.data ?? branchesResponse;
        const storesData = storesResponse?.data ?? storesResponse;

        const branchItems = Array.isArray(branchesData)
          ? branchesData
          : branchesData?.items ?? [];

        const storeItems = Array.isArray(storesData)
          ? storesData
          : storesData?.items ?? [];

        setBranches(
          branchItems
            .filter(
              (branch) =>
                branch?.isActive === true ||
                branch?.IsActive === true
            )
            .sort((a, b) =>
              String(
                a?.branchCode ??
                a?.BranchCode ??
                ""
              ).localeCompare(
                String(
                  b?.branchCode ??
                  b?.BranchCode ??
                  ""
                ),
                undefined,
                {
                  numeric: true,
                  sensitivity: "base",
                }
              )
            )
        );

        setStores(
          storeItems.filter(
            (store) =>
              store?.isActive === true ||
              store?.IsActive === true
          )
        );
      } catch (err) {
        if (!cancelled) {
          console.error("Failed to load master data:", err);
          setError(t.loadMasterDataError);
        }
      } finally {
        if (!cancelled) {
          setLoadingMasterData(false);
        }
      }
    };

    loadMasterData();

    return () => {
      cancelled = true;
    };
  }, [t, canCreateInventory]);

  const handleBranchChange = (value) => {
    setBranchId(value);
    setStoreId("");
  };

  const handleBeforeFileChange = async (file) => {
    setBeforeFile(file);
    setBeforePreview(null);
    setError("");

    if (!file) return;

    setPreviewingBefore(true);

    try {
      const response = await previewInventoryFile(file);
      setBeforePreview(response?.data ?? response);
    } catch (err) {
      console.error("Before file preview failed:", err);
      setBeforeFile(null);
      setError(err?.message || t.previewError);
    } finally {
      setPreviewingBefore(false);
    }
  };

  const handleAfterFileChange = async (file) => {
    setAfterFile(file);
    setAfterPreview(null);
    setError("");

    if (!file) return;

    setPreviewingAfter(true);

    try {
      const response = await previewInventoryFile(file);
      setAfterPreview(response?.data ?? response);
    } catch (err) {
      console.error("After file preview failed:", err);
      setAfterFile(null);
      setError(err?.message || t.previewError);
    } finally {
      setPreviewingAfter(false);
    }
  };

  const canSubmit =
    canCreateInventory &&
    inventoryType !== 0 &&
    branchId !== "" &&
    storeId !== "" &&
    beforeFile !== null &&
    afterFile !== null &&
    beforePreview !== null &&
    afterPreview !== null &&
    !previewingBefore &&
    !previewingAfter &&
    !isSubmitting;

  const handleCreateSession = async () => {
    if (!canSubmit) return;

    setIsSubmitting(true);
    setError("");

    try {
      const response = await confirmInventorySession({
        inventoryType,
        beforeFile,
        afterFile,
      });

      const session = response?.data ?? response;

      setActiveSession({
        ...session,
        id:
          session?.id ??
          session?.sessionId ??
          session?.inventorySessionId,
        branchId,
        storeId,
        inventoryType,
      });

      navigate("/dashboard");
    } catch (err) {
      console.error("Failed to create inventory session:", err);
      setError(err?.message || t.createSessionError);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleCancel = () => {
    if (isSubmitting) return;
    navigate("/inventory-sessions");
  };

  if (!canCreateInventory) {
    return null;
  }

  return (
    <main className={styles.page} dir={direction}>
      <NewSessionHeader />
      <InventorySessionInfo
        inventoryType={inventoryType}
        onInventoryTypeChange={setInventoryType}
        branchId={branchId}
        onBranchChange={handleBranchChange}
        storeId={storeId}
        onStoreChange={setStoreId}
        branches={branches}
        stores={stores}
        loading={loadingMasterData}
      />
      <section className={styles.uploadSection}>
        <div className={styles.sectionHeader}>
          <h2>{t.uploadFile}</h2>
          <p>{t.uploadFileDescription}</p>
        </div>
        <div className={styles.uploadGrid}>
          <InventoryFileUpload
            title={t.beforeInventory}
            description={t.beforeInventoryDescription}
            file={beforeFile}
            onFileChange={handleBeforeFileChange}
            preview={beforePreview}
            previewing={previewingBefore}
          />
          <InventoryFileUpload
            title={t.afterInventory}
            description={t.afterInventoryDescription}
            file={afterFile}
            onFileChange={handleAfterFileChange}
            preview={afterPreview}
            previewing={previewingAfter}
          />
        </div>
      </section>
      <RequiredColumns />
      {error && (
        <div className={styles.error} role="alert">
          {error}
        </div>
      )}
      <NewSessionActions
        disabled={!canSubmit}
        loading={isSubmitting}
        onSubmit={handleCreateSession}
        onCancel={handleCancel}
      />
    </main>
  );
}

export default NewInventorySession;