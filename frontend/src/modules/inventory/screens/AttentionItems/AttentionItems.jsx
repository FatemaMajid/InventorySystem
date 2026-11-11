import { useCallback, useEffect, useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { useLanguage } from "../../../../context/useLanguage";
import { useInventorySession } from "../../../../context/useInventorySession";
import { getInventoryDashboard } from "../../services/dashboardService";
import { getAttentionItems } from "../../services/attentionService";
import { getInventorySessions } from "../../services/inventorySessionService";
import AttentionHeader from "../../components/AttentionItems/AttentionHeader/AttentionHeader";
import AttentionSession from "../../components/AttentionItems/AttentionSession/AttentionSession";
import AttentionSummary from "../../components/AttentionItems/AttentionSummary/AttentionSummary";
import AttentionFilters from "../../components/AttentionItems/AttentionFilters/AttentionFilters";
import AttentionTable from "../../components/AttentionItems/AttentionTable/AttentionTable";
import Pagination from "../../../../components/UI/Pagination/Pagination";
import ErrorState from "../../../../components/UI/ErrorState/ErrorState";
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
    const effectiveFilters = useMemo(() => ({
        ...appliedFilters,
        attentionType: selectedAttentionType,
    }), [appliedFilters, selectedAttentionType]);

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

    const activeSessionId = activeSession?.id;

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

            const activeId = activeSessionId;

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
    }, [activeSessionId, setActiveSession, t.error]);

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
        let active = true;
        getInventorySessions({
            pageNumber: 1,
            pageSize: 300,
            sortBy: "InventoryDate",
            descending: true,
        })
            .then((response) => {
                if (!active) return;
                const list = response?.items ?? [];
                setSessions(list);
                const preferredId = list.some((session) => session.id === activeSessionId)
                    ? activeSessionId
                    : list[0]?.id ?? "";
                setSelectedSessionId(preferredId);
                if (preferredId) {
                    const preferredSession = list.find((session) => session.id === Number(preferredId));
                    if (preferredSession) setActiveSession(preferredSession);
                }
                setSessionsError("");
            })
            .catch(() => {
                if (!active) return;
                setSessions([]);
                setSelectedSessionId("");
                setSessionsError(t.error);
            })
            .finally(() => {
                if (active) setLoadingSessions(false);
            });
        return () => { active = false; };
    }, [activeSessionId, setActiveSession, t.error]);

    useEffect(() => {
        if (!selectedSessionId) return;
        let active = true;
        const id = Number(selectedSessionId);

        getInventoryDashboard(id)
            .then((response) => {
                if (active) setSummary(response?.attention ?? null);
            })
            .catch(() => {
                if (active) setSummary(null);
            })
            .finally(() => {
                if (active) setLoadingSummary(false);
            });

        getAttentionItems({
            sessionId: id,
            pageNumber: page,
            pageSize,
            itemCode: effectiveFilters.itemCode,
            attentionType: effectiveFilters.attentionType,
        })
            .then((response) => {
                if (!active) return;
                setItems(response?.items ?? []);
                setTotalItems(response?.totalCount ?? 0);
                setItemsError("");
            })
            .catch(() => {
                if (!active) return;
                setItems([]);
                setTotalItems(0);
                setItemsError(t.error);
            })
            .finally(() => {
                if (active) setLoadingItems(false);
            });

        return () => { active = false; };
    }, [selectedSessionId, page, pageSize, effectiveFilters, t.error]);

    const handleSessionChange = (sessionId) => {
        const id = Number(sessionId);

        setLoadingSummary(true);
        setLoadingItems(true);
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
        setLoadingItems(true);
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
        setLoadingItems(true);
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
                filters={{ ...filters, attentionType: selectedAttentionType }}
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
                        effectiveFilters
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
                        onPageChange={(value) => { setLoadingItems(true); setPage(value); }}
                        onPageSizeChange={(size) => {
                            setLoadingItems(true);
                            setPageSize(Number(size));
                            setPage(1);
                        }}
                    />
                )}
        </main>
    );
}

export default AttentionItems;