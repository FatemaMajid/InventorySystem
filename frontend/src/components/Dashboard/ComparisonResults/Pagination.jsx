import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./Pagination.module.css";

function Pagination({
    page,
    totalPages,
    totalItems,
    pageSize,
    onPageChange,
}) {
    const { translations, isArabic, direction } = useLanguage();

    const t = translations.dashboard.comparisonResults;

    const start =
        totalItems === 0
            ? 0
            : (page - 1) * pageSize + 1;

    const end = Math.min(
        page * pageSize,
        totalItems
    );

    const getPages = () => {
        if (totalPages <= 5) {
            return Array.from(
                { length: totalPages },
                (_, index) => index + 1
            );
        }

        if (page <= 3) {
            return [1, 2, 3, 4, 5, "...", totalPages];
        }

        if (page >= totalPages - 2) {
            return [
                1,
                "...",
                totalPages - 4,
                totalPages - 3,
                totalPages - 2,
                totalPages - 1,
                totalPages,
            ];
        }

        return [
            1,
            "...",
            page - 1,
            page,
            page + 1,
            "...",
            totalPages,
        ];
    };

    const previousIcon = isArabic
        ? "arrowRight"
        : "arrowLeft";

    const nextIcon = isArabic
        ? "arrowLeft"
        : "arrowRight";

    return (
        <div
            className={styles.pagination}
            dir={direction}
        >
            {/* RANGE */}
            <span className={styles.paginationInfo}>
                {start}-{end} {t.of} {totalItems}
            </span>

            <div className={styles.paginationControls}>

                {/* PREVIOUS */}
                <button
                    type="button"
                    className={styles.pageButton}
                    disabled={page === 1}
                    onClick={() =>
                        onPageChange(page - 1)
                    }
                    aria-label={t.previous}
                >
                    <Icon
                        name={previousIcon}
                        size={14}
                    />
                </button>

                {/* PAGES */}
                {getPages().map((pageNumber, index) => {
                    if (pageNumber === "...") {
                        return (
                            <span
                                key={`ellipsis-${index}`}
                                className={styles.ellipsis}
                            >
                                ...
                            </span>
                        );
                    }

                    return (
                        <button
                            key={pageNumber}
                            type="button"
                            className={`${styles.pageButton} ${
                                pageNumber === page
                                    ? styles.activePage
                                    : ""
                            }`}
                            onClick={() =>
                                onPageChange(pageNumber)
                            }
                        >
                            {pageNumber}
                        </button>
                    );
                })}

                {/* NEXT */}
                <button
                    type="button"
                    className={styles.pageButton}
                    disabled={page === totalPages}
                    onClick={() =>
                        onPageChange(page + 1)
                    }
                    aria-label={t.next}
                >
                    <Icon
                        name={nextIcon}
                        size={14}
                    />
                </button>
            </div>
        </div>
    );
}

export default Pagination;