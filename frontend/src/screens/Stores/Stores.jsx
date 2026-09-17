import { useCallback, useEffect, useMemo, useState } from "react";
import { useLanguage } from "../../context/LanguageContext";
import { hasPermission } from "../../services/permissionService";
import StoreHeader from "../../components/Stores/StoreHeader/StoreHeader";
import StoreStats from "../../components/Stores/StoreStats/StoreStats";
import StoreFilters from "../../components/Stores/StoreFilters/StoreFilters";
import StoresTable from "../../components/Stores/StoresTable/StoresTable";
import StoreForm from "../../components/Stores/StoreForm/StoreForm";
import { createStore, deleteStore, getStores, updateStore } from "../../services/storeService";
import { getBranches } from "../../services/masterDataService";
import styles from "./Stores.module.css";

function Stores() {
  const { translations, direction } = useLanguage();
  const t = translations.stores;
  const canCreate = hasPermission("Store.Create");
  const canEdit = hasPermission("Store.Edit");
  const [stores, setStores] = useState([]);
  const [branches, setBranches] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [filters, setFilters] = useState({ search: "", status: "all", branchCode: "" });
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [formOpen, setFormOpen] = useState(false);
  const [editingStore, setEditingStore] = useState(null);
  const [saving, setSaving] = useState(false);
  const [formError, setFormError] = useState("");

  const loadData = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const [storesResponse, branchesResponse] = await Promise.all([getStores(), getBranches()]);
      const storesData = storesResponse?.data ?? storesResponse;
      const branchesData = branchesResponse?.data ?? branchesResponse;
      setStores(Array.isArray(storesData) ? storesData : storesData?.items ?? []);
      setBranches(
        (Array.isArray(branchesData) ? branchesData : branchesData?.items ?? []).sort((a, b) =>
          String(a.branchCode ?? "").localeCompare(
            String(b.branchCode ?? ""),
            undefined,
            { numeric: true, sensitivity: "base" }
          )
        )
      );
    } catch (err) {
      console.error("Failed to load stores:", err);
      setError(err?.message || t.loadError);
    } finally {
      setLoading(false);
    }
  }, [t.loadError]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const filteredStores = useMemo(() => {
    const query = filters.search.trim().toLowerCase();

    return stores
      .filter((store) => {
        const matchesSearch =
          !query ||
          [store.storeCode, store.storeNameArabic, store.storeNameEnglish, store.branchCode].some(
            (value) => String(value ?? "").toLowerCase().includes(query)
          );

        const matchesStatus =
          filters.status === "all" ||
          (filters.status === "active" ? store.isActive : !store.isActive);

        const matchesBranch =
          !filters.branchCode ||
          String(store.branchCode ?? "") === String(filters.branchCode);

        return matchesSearch && matchesStatus && matchesBranch;
      })
      .sort((a, b) =>
        String(a.branchCode ?? "").localeCompare(
          String(b.branchCode ?? ""),
          undefined,
          {
            numeric: true,
            sensitivity: "base",
          }
        )
      );
  }, [stores, filters]);

  const totalItems = filteredStores.length;
  const totalPages = Math.max(1, Math.ceil(totalItems / pageSize));

  const paginatedStores = useMemo(() => {
    const start = (currentPage - 1) * pageSize;
    return filteredStores.slice(start, start + pageSize);
  }, [filteredStores, currentPage, pageSize]);

  useEffect(() => {
    if (currentPage > totalPages) setCurrentPage(totalPages);
  }, [currentPage, totalPages]);

  const handleFiltersChange = (nextFilters) => {
    setFilters((current) => ({ ...current, ...nextFilters }));
    setCurrentPage(1);
  };

  const handlePageSizeChange = (value) => {
    setPageSize(Number(value));
    setCurrentPage(1);
  };

  const openCreate = () => {
    if (!canCreate) return;
    setEditingStore(null);
    setFormError("");
    setFormOpen(true);
  };

  const openEdit = (store) => {
    if (!canEdit) return;
    setEditingStore(store);
    setFormError("");
    setFormOpen(true);
  };

  const closeForm = () => {
    if (saving) return;
    setFormOpen(false);
    setEditingStore(null);
    setFormError("");
  };

  const handleSubmit = async (form) => {
    if (editingStore && !canEdit) return;
    if (!editingStore && !canCreate) return;

    setSaving(true);
    setFormError("");

    try {
      if (editingStore) {
        await updateStore(editingStore.id, form);
      } else {
        await createStore(form);
      }

      setFormOpen(false);
      setEditingStore(null);
      await loadData();
    } catch (err) {
      console.error("Failed to save store:", err);
      setFormError(err?.message || t.saveError);
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (store) => {
    if (!canEdit) return;

    const name =
      store.storeNameArabic ||
      store.storeNameEnglish ||
      store.storeCode;

    const confirmed = window.confirm(
      t.deleteConfirmation.replace("{name}", name)
    );

    if (!confirmed) return;

    try {
      setError("");
      await deleteStore(store.id);
      await loadData();
    } catch (err) {
      console.error("Failed to delete store:", err);
      setError(err?.message || t.deleteError);
    }
  };

  const totalStores = stores.length;
  const activeStores = stores.filter((store) => store.isActive).length;
  const inactiveStores = totalStores - activeStores;

  return (
    <main className={styles.page} dir={direction}>
      <StoreHeader
        onNewStore={openCreate}
        canCreate={canCreate}
      />

      <StoreStats
        totalStores={totalStores}
        activeStores={activeStores}
        inactiveStores={inactiveStores}
        loading={loading}
      />

      <StoreFilters
        search={filters.search}
        status={filters.status}
        branchCode={filters.branchCode}
        branches={branches}
        onSearchChange={(search) =>
          handleFiltersChange({ search })
        }
        onStatusChange={(status) =>
          handleFiltersChange({ status })
        }
        onBranchChange={(branchCode) =>
          handleFiltersChange({ branchCode })
        }
        onReset={() =>
          handleFiltersChange({
            search: "",
            status: "all",
            branchCode: "",
          })
        }
      />

      <StoresTable
        stores={paginatedStores}
        loading={loading}
        error={error}
        onRetry={loadData}
        onEdit={openEdit}
        onDelete={handleDelete}
        canEdit={canEdit}
        currentPage={currentPage}
        totalPages={totalPages}
        pageSize={pageSize}
        totalItems={totalItems}
        onPageChange={setCurrentPage}
        onPageSizeChange={handlePageSizeChange}
      />

      <StoreForm
        open={formOpen}
        store={editingStore}
        branches={branches}
        saving={saving}
        error={formError}
        onClose={closeForm}
        onSubmit={handleSubmit}
      />
    </main>
  );
}

export default Stores;