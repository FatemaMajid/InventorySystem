import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import EmptyState from "../../UI/EmptyState/EmptyState";
import ErrorState from "../../UI/ErrorState/ErrorState";
import LoadingState from "../../UI/Loading/LoadingState/LoadingState";
import Pagination from "../../UI/Pagination/Pagination";
import styles from "./StoresTable.module.css";

function StoresTable({
  stores = [],
  loading = false,
  error = "",
  onRetry,
  onEdit,
  onDelete,
  currentPage = 1,
  totalPages = 1,
  pageSize = 20,
  totalItems = 0,
  onPageChange,
  onPageSizeChange,
}) {
  const { translations, direction } = useLanguage();
  const t = translations.stores;

  const renderState = () => {
    if (loading) {
      return (
        <div className={styles.state}>
          <LoadingState message={t.loading} />
        </div>
      );
    }

    if (error) {
      return (
        <div className={styles.state}>
          <ErrorState message={error} onRetry={onRetry} />
        </div>
      );
    }

    return (
      <div className={styles.state}>
        <EmptyState icon="stores" message={t.noStores} />
      </div>
    );
  };

  const showState = loading || error || stores.length === 0;
  const showFooter = !loading && !error && totalItems > 0;

  return (
    <section className={styles.card} dir={direction}>
      <div className={styles.tableHeader}>
        <div className={styles.heading}>
          <div className={styles.headingIcon}>
            <Icon name="stores" size={19} />
          </div>
          <div>
            <h2 className={styles.title}>{t.storesList}</h2>
            <p className={styles.description}>{t.storesListDescription}</p>
          </div>
        </div>

        {showFooter && (
          <div className={styles.count}>{totalItems.toLocaleString("en-US")}</div>
        )}
      </div>

      {showState ? (
        renderState()
      ) : (
        <div className={styles.tableWrapper}>
          <table className={styles.table}>
            <thead>
              <tr>
                <th>{t.code}</th>
                <th>{t.name}</th>
                <th>{t.branch}</th>
                <th>{t.status}</th>
                <th className={styles.actionsHead}>{t.actions}</th>
              </tr>
            </thead>
            <tbody>
              {stores.map((store) => (
                <tr key={store.id}>
                  <td>
                    <span className={styles.code}>{store.storeCode || "—"}</span>
                  </td>
                  <td>
                    <div className={styles.nameCell}>
                      {store.storeNameArabic || store.storeNameEnglish || "—"}
                      <span>
                        {store.storeNameArabic && store.storeNameEnglish
                          ? store.storeNameEnglish
                          : ""}
                      </span>
                    </div>
                  </td>
                  <td>
                    <span className={styles.branch}>{store.branchCode || "—"}</span>
                  </td>
                  <td>
                    <span
                      className={`${styles.status} ${
                        store.isActive ? styles.active : styles.inactive
                      }`}
                    >
                      <i />
                      {store.isActive ? t.active : t.inactive}
                    </span>
                  </td>
                  <td>
                    <div className={styles.actions}>
                      <button
                        type="button"
                        className={styles.actionButton}
                        onClick={() => onEdit?.(store)}
                        aria-label={t.edit}
                        title={t.edit}
                      >
                        <Icon name="edit" size={16} />
                      </button>
                      <button
                        type="button"
                        className={`${styles.actionButton} ${styles.deleteButton}`}
                        onClick={() => onDelete?.(store)}
                        aria-label={t.delete}
                        title={t.delete}
                      >
                        <Icon name="close" size={16} />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {showFooter && (
        <Pagination
          currentPage={currentPage}
          totalPages={totalPages}
          pageSize={pageSize}
          totalItems={totalItems}
          onPageChange={onPageChange}
          onPageSizeChange={onPageSizeChange}
        />
      )}
    </section>
  );
}

export default StoresTable;