import { useCallback, useEffect, useMemo, useState } from "react";
import { useLanguage } from "../../context/LanguageContext";
import { hasPermission } from "../../services/permissionService";
import BranchHeader from "../../components/Branches/BranchHeader/BranchHeader";
import BranchStats from "../../components/Branches/BranchStats/BranchStats";
import BranchFilters from "../../components/Branches/BranchFilters/BranchFilters";
import BranchesTable from "../../components/Branches/BranchesTable/BranchesTable";
import BranchForm from "../../components/Branches/BranchForm/BranchForm";
import {
  createBranch,
  deleteBranch,
  getBranches,
  getBranchById,
  updateBranch,
} from "../../services/branchService";
import styles from "./Branches.module.css";

function Branches() {
  const { translations, direction } = useLanguage();
  const t = translations.branches;

  const canCreate = hasPermission("Branch.Create");
  const canEdit = hasPermission("Branch.Edit");

  const [branches, setBranches] = useState([]);
  const [filters, setFilters] = useState({
    search: "",
    status: "",
  });
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [loading, setLoading] = useState(true);
  const [formLoading, setFormLoading] = useState(false);
  const [error, setError] = useState("");
  const [formError, setFormError] = useState("");
  const [formOpen, setFormOpen] = useState(false);
  const [editingBranch, setEditingBranch] = useState(null);

  const loadBranches = useCallback(async () => {
    try {
      setLoading(true);
      setError("");

      const response = await getBranches();
      const data = response?.data ?? response;

      setBranches(
        Array.isArray(data)
          ? data
          : data?.items ?? []
      );
    } catch (err) {
      console.error("Failed to load branches:", err);
      setBranches([]);
      setError(
        err?.message || t.loadError
      );
    } finally {
      setLoading(false);
    }
  }, [t.loadError]);

  useEffect(() => {
    loadBranches();
  }, [loadBranches]);

  const filteredBranches = useMemo(() => {
    const search = filters.search.trim().toLowerCase();

    return branches
      .filter((branch) => {
        const matchesSearch = !search || [
          branch.branchCode,
          branch.branchNameArabic,
          branch.branchNameEnglish,
          branch.address,
          branch.phone,
        ].some((value) =>
          String(value || "")
            .toLowerCase()
            .includes(search)
        );

        const matchesStatus =
          !filters.status ||
          (filters.status === "active"
            ? branch.isActive
            : !branch.isActive);

        return matchesSearch && matchesStatus;
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
  }, [branches, filters]);

  const totalItems = filteredBranches.length;
  const totalPages = Math.max(
    1,
    Math.ceil(totalItems / pageSize)
  );

  useEffect(() => {
    if (page > totalPages) {
      setPage(totalPages);
    }
  }, [page, totalPages]);

  const paginatedBranches = useMemo(() => {
    const start = (page - 1) * pageSize;

    return filteredBranches.slice(
      start,
      start + pageSize
    );
  }, [filteredBranches, page, pageSize]);

  const stats = useMemo(() => {
    const active = branches.filter(
      (branch) => branch.isActive
    ).length;

    return {
      total: branches.length,
      active,
      inactive: branches.length - active,
    };
  }, [branches]);

  const openCreate = () => {
    if (!canCreate) return;

    setEditingBranch(null);
    setFormError("");
    setFormOpen(true);
  };

  const openEdit = async (branch) => {
    if (!canEdit) return;

    try {
      setFormError("");
      setFormLoading(true);

      const response = await getBranchById(branch.id);
      const data = response?.data ?? response;

      setEditingBranch(data);
      setFormOpen(true);
    } catch (err) {
      console.error("Failed to load branch:", err);
      setFormOpen(false);
      setFormError(
        err?.message || t.loadDetailsError
      );
    } finally {
      setFormLoading(false);
    }
  };

  const closeForm = () => {
    setFormOpen(false);
    setEditingBranch(null);
    setFormError("");
  };

  const saveBranch = async (form) => {
    if (editingBranch && !canEdit) return;
    if (!editingBranch && !canCreate) return;

    try {
      setFormLoading(true);
      setFormError("");

      if (editingBranch) {
        await updateBranch(
          editingBranch.id,
          form
        );
      } else {
        await createBranch(form);
      }

      closeForm();
      await loadBranches();
    } catch (err) {
      console.error("Failed to save branch:", err);
      setFormError(
        err?.message || t.saveError
      );
    } finally {
      setFormLoading(false);
    }
  };

  const removeBranch = async (branch) => {
    if (!canEdit) return;

    const confirmed = window.confirm(
      t.deleteConfirmation.replace(
        "{name}",
        branch.branchNameArabic || branch.branchCode
      )
    );

    if (!confirmed) return;

    try {
      setError("");
      await deleteBranch(branch.id);
      await loadBranches();
    } catch (err) {
      console.error("Failed to delete branch:", err);
      setError(
        err?.message || t.deleteError
      );
    }
  };

  const changeFilters = (nextFilters) => {
    setFilters(nextFilters);
    setPage(1);
  };

  const changePageSize = (value) => {
    setPageSize(value);
    setPage(1);
  };

  return (
    <main
      className={styles.page}
      dir={direction}
    >
      <BranchHeader
        onAdd={openCreate}
        canCreate={canCreate}
      />

      <BranchStats
        totalBranches={stats.total}
        activeBranches={stats.active}
        inactiveBranches={stats.inactive}
        loading={loading}
      />

      <BranchFilters
        filters={filters}
        onFiltersChange={changeFilters}
        onClear={() =>
          changeFilters({
            search: "",
            status: "",
          })
        }
      />

      <BranchesTable
        branches={paginatedBranches}
        loading={loading}
        error={error}
        onRetry={loadBranches}
        onEdit={openEdit}
        onDelete={removeBranch}
        canEdit={canEdit}
        currentPage={page}
        totalPages={totalPages}
        pageSize={pageSize}
        totalItems={totalItems}
        onPageChange={setPage}
        onPageSizeChange={changePageSize}
      />

      {formOpen && (
        <BranchForm
          branch={editingBranch}
          loading={formLoading}
          error={formError}
          onSubmit={saveBranch}
          onClose={closeForm}
        />
      )}
    </main>
  );
}

export default Branches;