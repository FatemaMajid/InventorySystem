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

import styles from "./InventorySessions.module.css";

function InventorySessions() {
  const { translations, direction } =
    useLanguage();

  const t = translations.inventorySessions;

  const [sessions, setSessions] =
    useState([]);

  const [branches, setBranches] =
    useState([]);

  const [stores, setStores] =
    useState([]);

  const [stats, setStats] = useState({
    total: 0,
    active: 0,
    completed: 0,
  });

  const [filters, setFilters] = useState({
    branchId: "",
    storeId: "",
    status: "",
    sessionNumber: "",
  });

  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] =
    useState(20);

  const [totalItems, setTotalItems] =
    useState(0);

  const [totalPages, setTotalPages] =
    useState(1);

  const [loading, setLoading] =
    useState(false);

  const fetchSessions = useCallback(
    async () => {
      setLoading(true);

      try {
        const response =
          await getInventorySessions({
            pageNumber: page,
            pageSize,
            ...filters,
          });

        const data =
          response?.data ?? response;

        setSessions(data?.items ?? []);
        setTotalItems(
          data?.totalCount ?? 0
        );

        setTotalPages(
          data?.totalPages ??
            Math.max(
              1,
              Math.ceil(
                (data?.totalCount ?? 0) /
                  pageSize
              )
            )
        );
      } catch (error) {
        console.error(error);
        setSessions([]);
      } finally {
        setLoading(false);
      }
    },
    [page, pageSize, filters]
  );

  const fetchStats = useCallback(
    async () => {
      try {
        const response =
          await getInventorySessions({
            pageNumber: 1,
            pageSize: 10000,
          });

        const data =
          response?.data ?? response;

        const all =
          data?.items ?? [];

        const limit = new Date();
        limit.setDate(
          limit.getDate() - 7
        );

        setStats({
          total:
            data?.totalCount ??
            all.length,

          active: all.filter(
            (session) =>
              session.inventoryDate &&
              new Date(
                session.inventoryDate
              ) >= limit
          ).length,

          completed: all.filter(
            (session) =>
              session.status ===
              "Completed"
          ).length,
        });
      } catch (error) {
        console.error(error);
      }
    },
    []
  );

  useEffect(() => {
    fetchSessions();
  }, [fetchSessions]);

  useEffect(() => {
    fetchStats();
  }, [fetchStats]);

  useEffect(() => {
    Promise.all([
      getBranches(),
      getStores(),
    ]).then(
      ([branchesRes, storesRes]) => {
        const branchData =
          branchesRes?.data ??
          branchesRes;

        const storeData =
          storesRes?.data ??
          storesRes;

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
      }
    ).catch(console.error);
  }, []);

  const handleFiltersChange = (
    changes
  ) => {
    setFilters((prev) => ({
      ...prev,
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
        completedSessions={stats.completed}
      />

      <SessionFilters
        filters={filters}
        onFiltersChange={
          handleFiltersChange
        }
        branches={branches}
        stores={stores}
      />

      <section
        className={
          styles.sessionsSection
        }
      >
        <SessionsTable
          sessions={sessions}
          loading={loading}
        />

        {!loading &&
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