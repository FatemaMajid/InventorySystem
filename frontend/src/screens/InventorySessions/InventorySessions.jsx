import {
  useCallback,
  useEffect,
  useState,
} from "react";

import { useLanguage } from "../../context/LanguageContext";

import {
  getInventorySessions,
} from "../../services/inventorySessionService";

import {
  getBranches,
  getStores,
} from "../../services/masterDataService";

import SessionHeader from "../../components/InventorySessions/SessionHeader/SessionHeader";
import SessionStats from "../../components/InventorySessions/SessionStats/SessionStats";
import SessionFilters from "../../components/InventorySessions/SessionFilters/SessionFilters";
import SessionsTable from "../../components/InventorySessions/SessionsTable/SessionsTable";
import Pagination from "../../components/UI/Pagination/Pagination";
import ErrorState from "../../components/UI/ErrorState/ErrorState";

import styles from "./InventorySessions.module.css";

function InventorySessions() {
  const { translations, direction } =
    useLanguage();

  const t =
    translations.inventorySessions;

  const [sessions, setSessions] =
    useState([]);

  const [branches, setBranches] =
    useState([]);

  const [stores, setStores] =
    useState([]);

  const [stats, setStats] =
    useState({
      total: 0,
      active: 0,
      completed: 0,
    });

  const [filters, setFilters] =
    useState({
      branchId: "",
      storeId: "",
      status: "",
      sessionNumber: "",
    });

  const [page, setPage] =
    useState(1);

  const [pageSize, setPageSize] =
    useState(20);

  const [totalItems, setTotalItems] =
    useState(0);

  const [totalPages, setTotalPages] =
    useState(1);

  const [loadingSessions, setLoadingSessions] =
    useState(false);

  const [loadingStats, setLoadingStats] =
    useState(false);

  const [loadingMasterData, setLoadingMasterData] =
    useState(false);

  const [sessionsError, setSessionsError] =
    useState("");

  const [masterDataError, setMasterDataError] =
    useState("");

  const fetchSessions =
    useCallback(async () => {
      try {
        setLoadingSessions(true);
        setSessionsError("");

        const response =
          await getInventorySessions({
            pageNumber: page,
            pageSize,
            ...filters,
          });

        const data =
          response?.data ??
          response;

        const items =
          data?.items ?? [];

        const count =
          Number(
            data?.totalCount ?? 0
          );

        setSessions(items);
        setTotalItems(count);

        setTotalPages(
          data?.totalPages ??
            Math.max(
              1,
              Math.ceil(
                count / pageSize
              )
            )
        );
      } catch (error) {
        console.error(
          "Failed to load inventory sessions:",
          error
        );

        setSessions([]);
        setTotalItems(0);
        setTotalPages(1);

        setSessionsError(
          error?.message ||
            t.loadError
        );
      } finally {
        setLoadingSessions(false);
      }
    }, [
      page,
      pageSize,
      filters,
      t.loadError,
    ]);

  const fetchStats =
    useCallback(async () => {
      try {
        setLoadingStats(true);

        const response =
          await getInventorySessions({
            pageNumber: 1,
            pageSize: 10000,
          });

        const data =
          response?.data ??
          response;

        const all =
          data?.items ?? [];

        const limit =
          new Date();

        limit.setDate(
          limit.getDate() - 7
        );

        setStats({
          total:
            Number(
              data?.totalCount ??
                all.length
            ),

          active:
            all.filter(
              (session) =>
                session.inventoryDate &&
                new Date(
                  session.inventoryDate
                ) >= limit
            ).length,

          completed:
            all.filter(
              (session) =>
                session.status ===
                "Completed"
            ).length,
        });
      } catch (error) {
        console.error(
          "Failed to load inventory session statistics:",
          error
        );
      } finally {
        setLoadingStats(false);
      }
    }, []);

  const fetchMasterData =
    useCallback(async () => {
      try {
        setLoadingMasterData(true);
        setMasterDataError("");

        const [
          branchesResponse,
          storesResponse,
        ] = await Promise.all([
          getBranches(),
          getStores(),
        ]);

        const branchData =
          branchesResponse?.data ??
          branchesResponse;

        const storeData =
          storesResponse?.data ??
          storesResponse;

        setBranches(
          Array.isArray(branchData)
            ? branchData
            : branchData?.items ?? []
        );

        setStores(
          Array.isArray(storeData)
            ? storeData
            : storeData?.items ?? []
        );
      } catch (error) {
        console.error(
          "Failed to load inventory session filters:",
          error
        );

        setBranches([]);
        setStores([]);

        setMasterDataError(
          error?.message ||
            ""
        );
      } finally {
        setLoadingMasterData(false);
      }
    }, []);

  useEffect(() => {
    fetchSessions();
  }, [fetchSessions]);

  useEffect(() => {
    fetchStats();
  }, [fetchStats]);

  useEffect(() => {
    fetchMasterData();
  }, [fetchMasterData]);

  const handleFiltersChange = (
    changes
  ) => {
    setFilters((previous) => ({
      ...previous,
      ...changes,
    }));

    setPage(1);
  };

  const handlePageSizeChange = (
    size
  ) => {
    setPageSize(Number(size));
    setPage(1);
  };

  return (
    <main
      className={styles.page}
      dir={direction}
    >
      <SessionHeader />

      <SessionStats
        totalSessions={stats.total}
        activeSessions={stats.active}
        completedSessions={
          stats.completed
        }
        loading={loadingStats}
      />

      <SessionFilters
        filters={filters}
        onFiltersChange={
          handleFiltersChange
        }
        branches={branches}
        stores={stores}
      />

      {masterDataError && (
        <div className={styles.masterDataError}>
          <ErrorState
            message={masterDataError}
            onRetry={fetchMasterData}
          />
        </div>
      )}

      <section
        className={
          styles.sessionsSection
        }
      >
        <SessionsTable
          sessions={sessions}
          loading={loadingSessions}
          error={sessionsError}
          onRetry={fetchSessions}
        />

        {!loadingSessions &&
          !sessionsError &&
          totalItems > 0 && (
            <Pagination
              currentPage={page}
              totalPages={totalPages}
              pageSize={pageSize}
              totalItems={totalItems}
              onPageChange={setPage}
              onPageSizeChange={
                handlePageSizeChange
              }
            />
          )}
      </section>
    </main>
  );
}

export default InventorySessions;