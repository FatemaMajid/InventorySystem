import { useEffect, useMemo, useState } from "react";
import { useLanguage } from "../../../context/LanguageContext";

// import Icon from "../../UI/Icon/Icon";
import Filters from "./Filters";
import Pagination from "./Pagination";

import { getInventoryComparison } from "../../../services/dashboardService";

import styles from "./ComparisonResults.module.css";

const PAGE_SIZE = 10;

const EMPTY_FILTERS = {
  status: "All",
  search: "",
  category: "All",
  unit: "All",
};

const formatNumber = (value) => {
  if (value === null || value === undefined) {
    return "—";
  }

  return Number(value).toLocaleString("en-US");
};

const getDifferenceClass = (value) => {
  const number = Number(value || 0);

  if (number > 0) {
    return styles.positive;
  }

  if (number < 0) {
    return styles.negative;
  }

  return "";
};

function ComparisonResults({ sessionId }) {
  const { translations } = useLanguage();

  const t = translations.dashboard.comparisonResults;

  const [data, setData] = useState([]);

  const [filters, setFilters] = useState(EMPTY_FILTERS);

  const [currentPage, setCurrentPage] = useState(1);

  const [totalItems, setTotalItems] = useState(0);

  const [loading, setLoading] = useState(false);

  const [error, setError] = useState("");

  /*
   * FILTER OPTIONS
   *
   * Category / Unit IDs come directly from
   * the comparison API response.
   */
  const categories = useMemo(() => {
    const map = new Map();

    data.forEach((item) => {
      if (
        item.categoryId !== null &&
        item.categoryId !== undefined &&
        item.categoryName
      ) {
        map.set(
          String(item.categoryId),
          item.categoryName
        );
      }
    });

    return Array.from(map.entries()).map(
      ([id, name]) => ({
        id,
        name,
      })
    );
  }, [data]);

  const units = useMemo(() => {
    const map = new Map();

    data.forEach((item) => {
      if (
        item.unitId !== null &&
        item.unitId !== undefined &&
        item.unitName
      ) {
        map.set(
          String(item.unitId),
          item.unitName
        );
      }
    });

    return Array.from(map.entries()).map(
      ([id, name]) => ({
        id,
        name,
      })
    );
  }, [data]);

  /*
   * LOAD DATA
   */
  useEffect(() => {
    if (!sessionId) {
      setData([]);
      setTotalItems(0);
      return;
    }

    let cancelled = false;

    const loadComparison = async () => {
      try {
        setLoading(true);
        setError("");

        const response =
          await getInventoryComparison({
            sessionId,
            pageNumber: currentPage,
            pageSize: PAGE_SIZE,

            itemCode: filters.search,
            itemName: filters.search,

            categoryId:
              filters.category === "All"
                ? null
                : filters.category,

            unitId:
              filters.unit === "All"
                ? null
                : filters.unit,

            status:
              filters.status === "All"
                ? ""
                : filters.status,

            sortBy: "ItemCode",
            descending: false,
          });

        if (cancelled) {
          return;
        }

        setData(response?.items || []);

        setTotalItems(
          Number(response?.totalCount || 0)
        );
      } catch (err) {
        if (cancelled) {
          return;
        }

        setData([]);
        setTotalItems(0);

        setError(
          err?.message ||
          t.error ||
          "Failed to load comparison results."
        );
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    };

    loadComparison();

    return () => {
      cancelled = true;
    };
  }, [
    sessionId,
    currentPage,
    filters.status,
    filters.search,
    filters.category,
    filters.unit,
    t.error,
  ]);

  /*
   * PAGINATION
   */
  const totalPages = Math.max(
    1,
    Math.ceil(totalItems / PAGE_SIZE)
  );

  const safePage = Math.min(
    currentPage,
    totalPages
  );

  /*
   * HANDLERS
   */
  const handleFilterChange = (
    field,
    value
  ) => {
    setFilters((prev) => ({
      ...prev,
      [field]: value,
    }));

    setCurrentPage(1);
  };

  const handleApplyFilters = () => {
    setCurrentPage(1);
  };

  const handleClearFilters = () => {
    setFilters(EMPTY_FILTERS);
    setCurrentPage(1);
  };

  const handlePageChange = (page) => {
    if (
      page < 1 ||
      page > totalPages ||
      page === currentPage
    ) {
      return;
    }

    setCurrentPage(page);
  };

  /*
   * RENDER
   */
  return (
    <section
      className={styles.wrapper}
      dir="inherit"
    >
      {/* HEADER */}
      <div className={styles.header}>
        <div className={styles.headerInfo}>
          <div className={styles.titleRow}>
            <h2 className={styles.title}>
              {t.title}
            </h2>

            <span className={styles.count}>
              {totalItems.toLocaleString("en-US")}{" "}
              {t.items}
            </span>
          </div>
        </div>
      </div>

      {/* CONTENT */}
      <div className={styles.contentLayout}>

        {/* FILTERS */}
        <div className={styles.filtersPanel}>
          <Filters
            filters={filters}
            categories={categories}
            units={units}
            onChange={handleFilterChange}
            onApply={handleApplyFilters}
            onClear={handleClearFilters}
          />
        </div>

        {/* RESULTS */}
        <div className={styles.resultsPanel}>

          {/* TABLE */}
          <div className={styles.tableWrapper}>
            <table className={styles.table}>

              <thead>
                <tr>
                  <th>#</th>

                  <th>
                    {t.itemCode}
                  </th>

                  <th>
                    {t.itemName}
                  </th>

                  <th>
                    {t.category}
                  </th>

                  <th>
                    {t.unit}
                  </th>

                  <th>
                    {t.quantityBefore}
                  </th>

                  <th>
                    {t.quantityAfter}
                  </th>

                  <th>
                    {t.quantityDifference}
                  </th>

                  <th>
                    {t.differencePercentage}
                  </th>

                  <th>
                    {t.priceBefore}
                  </th>

                  <th>
                    {t.priceAfter}
                  </th>

                  <th>
                    {t.beforeValue}
                  </th>

                  <th>
                    {t.afterValue}
                  </th>

                  <th>
                    {t.valueDifference}
                  </th>

                  <th>
                    {t.status}
                  </th>
                </tr>
              </thead>

              <tbody>

                {/* ERROR */}
                {!loading && error && (
                  <tr>
                    <td
                      colSpan="15"
                      className={styles.empty}
                    >
                      {error}
                    </td>
                  </tr>
                )}

                {/* LOADING */}
                {loading && (
                  <tr>
                    <td
                      colSpan="15"
                      className={styles.empty}
                    >
                      {t.loading}
                    </td>
                  </tr>
                )}

                {/* EMPTY */}
                {!loading &&
                  !error &&
                  data.length === 0 && (
                    <tr>
                      <td
                        colSpan="15"
                        className={styles.empty}
                      >
                        {t.noResults}
                      </td>
                    </tr>
                  )}

                {/* DATA */}
                {!loading &&
                  !error &&
                  data.map(
                    (item, index) => {
                      const rowNumber =
                        (safePage - 1) *
                        PAGE_SIZE +
                        index +
                        1;

                      const statusTranslationKey = {
                        Increase: "increase",
                        Decrease: "decrease",
                        NoDifference: "noDifference",
                        AfterOnly: "afterOnly",
                        BeforeOnly: "beforeOnly",
                      };

                      return (
                        <tr
                          key={
                            item.inventoryDetailId ??
                            item.itemId ??
                            item.itemCode ??
                            rowNumber
                          }
                        >
                          <td>
                            {rowNumber}
                          </td>

                          <td>
                            {item.itemCode}
                          </td>

                          <td>
                            {item.itemName1 || "—"}
                          </td>

                          <td>
                            {item.categoryName || "—"}
                          </td>

                          <td>
                            {item.unitName || "—"}
                          </td>

                          <td>
                            {formatNumber(
                              item.quantityBefore
                            )}
                          </td>

                          <td>
                            {formatNumber(
                              item.quantityAfter
                            )}
                          </td>

                          <td
                            className={getDifferenceClass(
                              item.quantityDifference
                            )}
                          >
                            {formatNumber(
                              item.quantityDifference
                            )}
                          </td>

                          <td
                            className={getDifferenceClass(
                              item.differencePercentage
                            )}
                          >
                            {item.differencePercentage !==
                              null &&
                              item.differencePercentage !==
                              undefined
                              ? `${Number(
                                item.differencePercentage
                              ).toFixed(2)}%`
                              : "—"}
                          </td>

                          <td>
                            {formatNumber(
                              item.consumerPriceBefore
                            )}
                          </td>

                          <td>
                            {formatNumber(
                              item.consumerPriceAfter
                            )}
                          </td>

                          <td>
                            {formatNumber(
                              item.beforeValue
                            )}
                          </td>

                          <td>
                            {formatNumber(
                              item.afterValue
                            )}
                          </td>

                          <td
                            className={getDifferenceClass(
                              item.valueDifference
                            )}
                          >
                            {formatNumber(
                              item.valueDifference
                            )}
                          </td>

                          <td>
                            <span
                              className={`${styles.status} ${styles[
                                item.status
                              ] || ""
                                }`}
                            >
                              {t.statuses?.[
                                statusTranslationKey[item.status]
                              ] || item.status}
                            </span>
                          </td>
                        </tr>
                      );
                    }
                  )}
              </tbody>
            </table>
          </div>

          {/* PAGINATION */}
          {!loading &&
            !error &&
            totalItems > 0 && (
              <Pagination
                page={safePage}
                totalPages={totalPages}
                totalItems={totalItems}
                pageSize={PAGE_SIZE}
                onPageChange={
                  handlePageChange
                }
              />
            )}

        </div>
      </div>
    </section>
  );
}

export default ComparisonResults;