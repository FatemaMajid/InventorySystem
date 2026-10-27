import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import LoadingState from "../../UI/Loading/LoadingState/LoadingState";
import EmptyState from "../../UI/EmptyState/EmptyState";
import ErrorState from "../../UI/ErrorState/ErrorState";
import Pagination from "../../UI/Pagination/Pagination";
import styles from "./BranchesTable.module.css";

function BranchesTable({
    branches = [],
    loading = false,
    error = "",
    onRetry,
    onEdit,
    onDelete,
    canEdit = false,
    currentPage = 1,
    totalPages = 1,
    pageSize = 20,
    totalItems = 0,
    onPageChange,
    onPageSizeChange
}) {
    const { translations, direction } = useLanguage();
    const t = translations.branches;

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
                    <ErrorState
                        message={error}
                        onRetry={onRetry}
                    />
                </div>
            );
        }

        return (
            <div className={styles.state}>
                <EmptyState
                    icon="branches"
                    message={t.empty}
                />
            </div>
        );
    };

    return (
        <section
            className={styles.card}
            dir={direction}
        >
            <div className={styles.tableHeader}>
                <div className={styles.heading}>
                    <div className={styles.headingIcon}>
                        <Icon
                            name="branches"
                            size={19}
                        />
                    </div>

                    <div>
                        <h2 className={styles.title}>
                            {t.listTitle}
                        </h2>

                        <p className={styles.description}>
                            {t.listDescription}
                        </p>
                    </div>
                </div>

                {!loading && !error && totalItems > 0 && (
                    <div className={styles.count}>
                        {totalItems.toLocaleString("en-US")}
                    </div>
                )}
            </div>

            {loading || error || branches.length === 0 ? (
                renderState()
            ) : (
                <div className={styles.tableWrapper}>
                    <table className={styles.table}>
                        <thead>
                            <tr>
                                <th>{t.code}</th>
                                <th>{t.arabicName}</th>
                                <th>{t.englishName}</th>
                                <th>{t.address}</th>
                                <th>{t.phone}</th>
                                <th>{t.status}</th>
                                {canEdit && (
                                    <th className={styles.actionsHead}>
                                        {t.actions}
                                    </th>
                                )}
                            </tr>
                        </thead>

                        <tbody>
                            {branches.map((branch) => (
                                <tr key={branch.id}>
                                    <td>
                                        <span className={styles.code}>
                                            {branch.branchCode || "—"}
                                        </span>
                                    </td>

                                    <td className={styles.primaryText}>
                                        {branch.branchNameArabic || "—"}
                                    </td>

                                    <td>
                                        {branch.branchNameEnglish || "—"}
                                    </td>

                                    <td>
                                        {branch.address || "—"}
                                    </td>

                                    <td>
                                        {branch.phone || "—"}
                                    </td>

                                    <td>
                                        <span
                                            className={`${styles.status} ${
                                                branch.isActive
                                                    ? styles.active
                                                    : styles.inactive
                                            }`}
                                        >
                                            <i />
                                            {branch.isActive
                                                ? t.active
                                                : t.inactive}
                                        </span>
                                    </td>

                                    {canEdit && (
                                        <td>
                                            <div className={styles.actions}>
                                                <button
                                                    type="button"
                                                    className={styles.actionButton}
                                                    onClick={() =>
                                                        onEdit?.(branch)
                                                    }
                                                    aria-label={t.edit}
                                                    title={t.edit}
                                                >
                                                    <Icon
                                                        name="edit"
                                                        size={16}
                                                    />
                                                </button>

                                                <button
                                                    type="button"
                                                    className={`${styles.actionButton} ${styles.deleteButton}`}
                                                    onClick={() =>
                                                        onDelete?.(branch)
                                                    }
                                                    aria-label={t.delete}
                                                    title={t.delete}
                                                >
                                                    <Icon
                                                        name="close"
                                                        size={16}
                                                    />
                                                </button>
                                            </div>
                                        </td>
                                    )}
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}

            {!loading && !error && totalItems > 0 && (
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

export default BranchesTable;