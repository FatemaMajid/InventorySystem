import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import LoadingState from "../../UI/Loading/LoadingState/LoadingState";
import EmptyState from "../../UI/EmptyState/EmptyState";
import ErrorState from "../../UI/ErrorState/ErrorState";
import Pagination from "../../UI/Pagination/Pagination";
import styles from "./UsersTable.module.css";

function UsersTable({ users = [], loading = false, error = "", onRetry, onEdit, onDelete, currentPage = 1, totalPages = 1, pageSize = 20, totalItems = 0, onPageChange, onPageSizeChange }) {
    const { translations, direction } = useLanguage();
    const t = translations.users;

    const renderState = () => {
        if (loading) return <div className={styles.state}><LoadingState message={t.loading} /></div>;
        if (error) return <div className={styles.state}><ErrorState message={error} onRetry={onRetry} /></div>;
        return <div className={styles.state}><EmptyState icon="users" message={t.empty} /></div>;
    };

    return (
        <section className={styles.card} dir={direction}>
            <div className={styles.header}>
                <div className={styles.heading}>
                    <div className={styles.headingIcon}><Icon name="users" size={19} /></div>
                    <div>
                        <h2 className={styles.title}>{t.listTitle}</h2>
                        <p className={styles.description}>{t.listDescription}</p>
                    </div>
                </div>
                {!loading && !error && totalItems > 0 && <span className={styles.count}>{totalItems.toLocaleString(direction === "rtl" ? "ar-IQ" : "en-US")}</span>}
            </div>
            {loading || error || users.length === 0 ? renderState() : (
                <div className={styles.tableWrapper}>
                    <table className={styles.table}>
                        <thead>
                            <tr>
                                <th>{t.username}</th>
                                <th>{t.role}</th>
                                <th>{t.status}</th>
                                <th>{t.createdAt}</th>
                                <th>{t.updatedAt}</th>
                                <th className={styles.actionsHead}>{t.actions}</th>
                            </tr>
                        </thead>
                        <tbody>
                            {users.map((user) => (
                                <tr key={user.id}>
                                    <td><span className={styles.username}>{user.username}</span></td>
                                    <td><span className={styles.role}>{user.role || "—"}</span></td>
                                    <td><span className={`${styles.status} ${user.isActive ? styles.active : styles.inactive}`}><i />{user.isActive ? t.active : t.inactive}</span></td>
                                    <td>{user.createdAt ? new Date(user.createdAt).toLocaleDateString(direction === "rtl" ? "ar-IQ" : "en-GB") : "—"}</td>
                                    <td>{user.updatedAt ? new Date(user.updatedAt).toLocaleDateString(direction === "rtl" ? "ar-IQ" : "en-GB") : "—"}</td>
                                    <td>
                                        <div className={styles.actions}>
                                            <button type="button" className={styles.actionButton} onClick={() => onEdit?.(user)} aria-label={t.edit} title={t.edit}><Icon name="edit" size={16} /></button>
                                            <button type="button" className={`${styles.actionButton} ${styles.deleteButton}`} onClick={() => onDelete?.(user)} aria-label={t.delete} title={t.delete}><Icon name="close" size={16} /></button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
            {!loading && !error && totalItems > 0 && <Pagination currentPage={currentPage} totalPages={totalPages} pageSize={pageSize} totalItems={totalItems} onPageChange={onPageChange} onPageSizeChange={onPageSizeChange} />}
        </section>
    );
}

export default UsersTable;
