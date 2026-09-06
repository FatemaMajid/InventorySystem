import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import LoadingState from "../../UI/Loading/LoadingState/LoadingState";
import EmptyState from "../../UI/EmptyState/EmptyState";
import ErrorState from "../../UI/ErrorState/ErrorState";

import styles from "./AttentionTable.module.css";

function AttentionTable({ items = [], loading = false, error = "", onRetry }) {
  const { translations, language, direction } = useLanguage();
  const t = translations.attentionItems;
  const isArabic = language === "ar";

  const formatNumber = (value) =>
    value === null || value === undefined || value === ""
      ? "—"
      : Number(value).toLocaleString(
          isArabic ? "ar-IQ" : "en-US",
          { maximumFractionDigits: 3 }
        );

  const getName = (item) =>
    isArabic
      ? item.itemName1 || item.itemName2 || "—"
      : item.itemName2 || item.itemName1 || "—";

  const statusLabels = {
    NewlyCounted: t.types.newlyCounted,
    FullyDepleted: t.types.fullyDepleted,
    UnitNotDefined: t.types.unitNotDefined,
    PriceChanged: t.types.priceChanged,
  };

  return (
    <section className={styles.wrapper} dir={direction}>
      <div className={styles.tableHeader}>
        <div>
          <h2>{t.title}</h2>
          <p>{t.description}</p>
        </div>

        {!loading && (
          <span className={styles.count}>
            {Number(items.length).toLocaleString(
              isArabic ? "ar-IQ" : "en-US"
            )}
          </span>
        )}
      </div>

      <div className={styles.tableWrapper}>
        {loading ? (
          <div className={styles.state}>
            <LoadingState message={t.loading} />
          </div>
        ) : error ? (
          <div className={styles.state}>
            <ErrorState message={error} onRetry={onRetry} />
          </div>
        ) : items.length === 0 ? (
          <div className={styles.state}>
            <EmptyState message={t.empty} />
          </div>
        ) : (
          <table className={styles.table}>
            <thead>
              <tr>
                <th>{t.table.itemCode}</th>
                <th>{t.table.itemName}</th>
                <th>{t.table.category}</th>
                <th>{t.table.unit}</th>
                <th>{t.table.quantityBefore}</th>
                <th>{t.table.quantityAfter}</th>
                <th>{t.table.difference}</th>
                <th>{t.table.consumerPriceBefore}</th>
                <th>{t.table.consumerPriceAfter}</th>
                <th>{t.table.valueBefore}</th>
                <th>{t.table.valueAfter}</th>
                <th>{t.table.valueDifference}</th>
                <th>{t.table.attentionType}</th>
              </tr>
            </thead>
            <tbody>
              {items.map((item) => (
                <tr key={item.inventoryDetailId}>
                  <td className={styles.code}>{item.itemCode || "—"}</td>
                  <td className={styles.name}>{getName(item)}</td>
                  <td>{item.categoryName || "—"}</td>
                  <td>{item.unitName || "—"}</td>
                  <td>{formatNumber(item.quantityBefore)}</td>
                  <td>{formatNumber(item.quantityAfter)}</td>
                  <td>{formatNumber(item.quantityDifference)}</td>
                  <td>{formatNumber(item.consumerPriceBefore)}</td>
                  <td>{formatNumber(item.consumerPriceAfter)}</td>
                  <td>{formatNumber(item.beforeValue)}</td>
                  <td>{formatNumber(item.afterValue)}</td>
                  <td>{formatNumber(item.valueDifference)}</td>
                  <td>
                    <span className={`${styles.badge} ${styles[item.attentionType] || ""}`}>
                      <Icon name="attention" size={14} />
                      {statusLabels[item.attentionType] || item.attentionType || "—"}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </section>
  );
}

export default AttentionTable;
