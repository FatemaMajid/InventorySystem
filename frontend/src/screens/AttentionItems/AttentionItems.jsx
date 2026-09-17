import { useCallback, useEffect, useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { useLanguage } from "../../context/LanguageContext";
import { useInventorySession } from "../../context/InventorySessionContext";
import { getInventoryDashboard } from "../../services/dashboardService";
import { getAttentionItems } from "../../services/attentionService";
import { getInventorySessions } from "../../services/inventorySessionService";
import AttentionHeader from "../../components/AttentionItems/AttentionHeader/AttentionHeader";
import AttentionSession from "../../components/AttentionItems/AttentionSession/AttentionSession";
import AttentionSummary from "../../components/AttentionItems/AttentionSummary/AttentionSummary";
import AttentionFilters from "../../components/AttentionItems/AttentionFilters/AttentionFilters";
import AttentionTable from "../../components/AttentionItems/AttentionTable/AttentionTable";
import Pagination from "../../components/UI/Pagination/Pagination";
import ErrorState from "../../components/UI/ErrorState/ErrorState";
import styles from "./AttentionItems.module.css";

const EMPTY_FILTERS = {
    itemCode: "",
    attentionType: "All",
};

const ATTENTION_TYPES = [
    "NewlyCounted",
    "FullyDepleted",
    "UnitNotDefined",
    "PriceChanged",
];

function AttentionItems() {
    const [searchParams, setSearchParams] = useSearchParams();
    const [pageSize, setPageSize] = useState(20);

    const { direction, translations } = useLanguage();
    const { activeSession, setActiveSession } = useInventorySession();

    const t = translations.attentionItems;

    const attentionTypeParam = searchParams.get("attentionType");

    const selectedAttentionType = ATTENTION_TYPES.includes(
        attentionTypeParam
    )
        ? attentionTypeParam
        : "All";

    const [sessions, setSessions] = useState([]);
    const [selectedSessionId, setSelectedSessionId] = useState(
        activeSession?.id ?? ""
    );

    const [filters, setFilters] = useState({
        ...EMPTY_FILTERS,
        attentionType: selectedAttentionType,
    });

    const [appliedFilters, setAppliedFilters] = useState({
        ...EMPTY_FILTERS,
        attentionType: selectedAttentionType,
    });

    const [summary, setSummary] = useState(null);
    const [items, setItems] = useState([]);
    const [totalItems, setTotalItems] = useState(0);
    const [page, setPage] = useState(1);

    const [loadingSessions, setLoadingSessions] = useState(true);
    const [loadingSummary, setLoadingSummary] = useState(false);
    const [loadingItems, setLoadingItems] = useState(false);
    const [sessionsError, setSessionsError] = useState("");
    const [itemsError, setItemsError] = useState("");

    const selectedSession = useMemo(
        () =>
            sessions.find(
                (session) => session.id === Number(selectedSessionId)
            ) || activeSession,
        [sessions, selectedSessionId, activeSession]
    );

    const totalPages = Math.max(
        1,
        Math.ceil(totalItems / pageSize)
    );

    const loadSessions = useCallback(async () => {
        setLoadingSessions(true);

        try {
            setSessionsError("");

            const response = await getInventorySessions({
                pageNumber: 1,
                pageSize: 300,
                sortBy: "InventoryDate",
                descending: true,
            });

            const list = response?.items ?? [];
            setSessions(list);

            const activeId = activeSession?.id;

            const preferredId = list.some(
                (session) => session.id === activeId
            )
                ? activeId
                : list[0]?.id ?? "";

            setSelectedSessionId(preferredId);

            if (preferredId) {
                const preferredSession = list.find(
                    (session) => session.id === Number(preferredId)
                );

                if (preferredSession) {
                    setActiveSession(preferredSession);
                }
            }
        } catch {
            setSessions([]);
            setSelectedSessionId("");
            setSessionsError(t.error);
        } finally {
            setLoadingSessions(false);
        }
    }, [activeSession?.id, setActiveSession, t.error]);

    const loadSummary = useCallback(async (sessionId) => {
        if (!sessionId) {
            setSummary(null);
            return;
        }

        setLoadingSummary(true);

        try {
            const response = await getInventoryDashboard(sessionId);
            setSummary(response?.attention ?? null);
        } catch {
            setSummary(null);
        } finally {
            setLoadingSummary(false);
        }
    }, []);

    const loadItems = useCallback(
        async (sessionId, currentPage, currentFilters) => {
            if (!sessionId) {
                setItems([]);
                setTotalItems(0);
                return;
            }

            setLoadingItems(true);
            setItemsError("");

            try {
                const response = await getAttentionItems({
                    sessionId,
                    pageNumber: currentPage,
                    pageSize,
                    itemCode: currentFilters.itemCode,
                    attentionType: currentFilters.attentionType,
                });

                setItems(response?.items ?? []);
                setTotalItems(response?.totalCount ?? 0);
            } catch {
                setItems([]);
                setTotalItems(0);
                setItemsError(t.error);
            } finally {
                setLoadingItems(false);
            }
        },
        [pageSize, t.error]
    );

    useEffect(() => {
        loadSessions();
    }, [loadSessions]);

    useEffect(() => {
        setFilters((current) => ({
            ...current,
            attentionType: selectedAttentionType,
        }));

        setAppliedFilters((current) => ({
            ...current,
            attentionType: selectedAttentionType,
        }));

        setPage(1);
    }, [selectedAttentionType]);

    useEffect(() => {
        if (!selectedSessionId) return;

        loadSummary(Number(selectedSessionId));

        loadItems(
            Number(selectedSessionId),
            page,
            appliedFilters
        );
    }, [
        selectedSessionId,
        page,
        appliedFilters,
        loadSummary,
        loadItems,
    ]);

    const handleSessionChange = (sessionId) => {
        const id = Number(sessionId);

        setSelectedSessionId(id || "");
        setPage(1);

        setFilters((current) => ({
            ...current,
            itemCode: "",
        }));

        setAppliedFilters((current) => ({
            ...current,
            itemCode: "",
        }));

        const session = sessions.find(
            (item) => item.id === id
        );

        if (session) {
            setActiveSession(session);
        }
    };

    const handleFilterChange = (key, value) => {
        setFilters((current) => ({
            ...current,
            [key]: value,
        }));

        if (key === "attentionType") {
            setPage(1);

            setAppliedFilters((current) => ({
                ...current,
                attentionType: value,
            }));

            if (value === "All") {
                searchParams.delete("attentionType");
            } else {
                searchParams.set("attentionType", value);
            }

            setSearchParams(searchParams);
        }

        if (key === "itemCode") {
            setPage(1);

            setAppliedFilters((current) => ({
                ...current,
                itemCode: value,
            }));
        }
    };

    const handleClear = () => {
        setFilters(EMPTY_FILTERS);
        setAppliedFilters(EMPTY_FILTERS);
        setItemsError("");
        setPage(1);

        searchParams.delete("attentionType");
        setSearchParams(searchParams);
    };

    return (
        <main className={styles.page} dir={direction}>
            <AttentionHeader />

            <AttentionSession
                sessions={sessions}
                selectedSessionId={selectedSessionId}
                onChange={handleSessionChange}
                loading={loadingSessions}
                session={selectedSession}
            />

            <AttentionSummary
                data={summary}
                loading={loadingSummary}
            />

            <AttentionFilters
                filters={filters}
                onChange={handleFilterChange}
                onClear={handleClear}
            />

            {sessionsError && !loadingSessions && (
                <div className={styles.error}>
                    <ErrorState
                        message={sessionsError}
                        onRetry={loadSessions}
                    />
                </div>
            )}

            <AttentionTable
                items={items}
                loading={loadingItems}
                error={itemsError}
                onRetry={() =>
                    selectedSessionId &&
                    loadItems(
                        Number(selectedSessionId),
                        page,
                        appliedFilters
                    )
                }
            />

            {!loadingItems &&
                !itemsError &&
                items.length > 0 && (
                    <Pagination
                        currentPage={page}
                        totalPages={totalPages}
                        pageSize={pageSize}
                        totalItems={totalItems}
                        onPageChange={setPage}
                        onPageSizeChange={(size) => {
                            setPageSize(Number(size));
                            setPage(1);
                        }}
                    />
                )}
        </main>
    );
}

export default AttentionItems;