import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../Icon/Icon";

import styles from "./Pagination.module.css";

function Pagination({
    currentPage = 1,
    totalPages = 1,
    pageSize = 20,
    totalItems = 0,
    onPageChange,
    onPageSizeChange,
}) {
    const { translations, direction } = useLanguage();

    const t = translations.pagination;

    const startItem =
        totalItems === 0
            ? 0
            : (currentPage - 1) * pageSize + 1;

    const endItem = Math.min(
        currentPage * pageSize,
        totalItems
    );

    const canGoPrevious = currentPage > 1;
    const canGoNext = currentPage < totalPages;

    const handlePageSizeChange = (event) => {
        onPageSizeChange?.(Number(event.target.value));
    };

    return (
        <div
            className={styles.pagination}
            dir={direction}
        >
            {/* Results info */}
            <div className={styles.info}>
                <span>{t.showing}</span>

                <strong>{startItem}</strong>

                <span>-</span>

                <strong>{endItem}</strong>

                <span>{t.of}</span>

                <strong>{totalItems}</strong>
            </div>

            {/* Controls */}
            <div className={styles.controls}>
                <label className={styles.pageSize}>
                    <span>{t.rowsPerPage}</span>

                    <select
                        value={pageSize}
                        onChange={handlePageSizeChange}
                        aria-label={t.rowsPerPage}
                    >
                        <option value={10}>10</option>
                        <option value={20}>20</option>
                        <option value={50}>50</option>
                        <option value={100}>100</option>
                    </select>
                </label>

                <div className={styles.navigation}>
                    <button
                        type="button"
                        className={styles.navButton}
                        disabled={!canGoPrevious}
                        onClick={() =>
                            onPageChange?.(currentPage - 1)
                        }
                        aria-label={t.previous}
                    >
                        <Icon name={direction === "rtl" ? "arrowRight" : "arrowLeft"}
                            size={17} />
                    </button>

                    <span className={styles.currentPage}>
                        {currentPage}
                    </span>

                    <button
                        type="button"
                        className={styles.navButton}
                        disabled={!canGoNext}
                        onClick={() =>
                            onPageChange?.(currentPage + 1)
                        }
                        aria-label={t.next}
                    >
                        <Icon name={direction === "rtl" ? "arrowLeft" : "arrowRight"}
                            size={17} />
                    </button>
                </div>
            </div>
        </div>
    );
}

export default Pagination;