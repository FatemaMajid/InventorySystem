import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./DashboardHeader.module.css";

function DashboardHeader({
    session,
    onExportExcel,
    onExportPdf,
}) {
    const { translations, isArabic } = useLanguage();

    const t = translations.dashboard.header;

    return (
        <section className={styles.header}>
            <div className={styles.topRow}>

                <div className={styles.titleSection}>
                    <h1>{t.title}</h1>

                    <div className={styles.sessionInfo}>
                        <span className={styles.sessionNumber}>
                            {t.session} #{session?.number ?? ""}
                        </span>

                        <span className={styles.status}>
                            {session?.status === "Completed"
                                ? t.statuses?.completed
                                : session?.status}
                        </span>

                        <span>
                            <strong>{t.type}:</strong>{" "}
                            {session?.type === "SemiAnnual"
                                ? t.inventoryTypes?.semiAnnual
                                : session?.type === "Annual"
                                    ? t.inventoryTypes?.annual
                                    : session?.type}
                        </span>

                        <span>
                            <strong>{t.date}:</strong>{" "}
                            {session?.date ?? ""}
                        </span>

                        <span>
                            <strong>{t.branch}:</strong>{" "}
                            {session?.branch ?? ""}
                        </span>

                        <span>
                            <strong>{t.store}:</strong>{" "}
                            {session?.store ?? ""}
                        </span>
                    </div>
                </div>

                <div className={styles.exportButtons} dir={isArabic ? "rtl" : "ltr"}>

                    <button
                        type="button"
                        className={`${styles.exportButton} ${styles.excel}`}
                        onClick={onExportExcel}
                    >
                        <Icon name="reports" size={18} />
                        <span>{t.exportExcel}</span>
                        <Icon name="chevronDown" size={14} />
                    </button>

                    <button
                        type="button"
                        className={`${styles.exportButton} ${styles.pdf}`}
                        onClick={onExportPdf}
                    >
                        <Icon name="reports" size={18} />
                        <span>{t.exportPdf}</span>
                        <Icon name="chevronDown" size={14} />
                    </button>

                </div>

            </div>
        </section>
    );
}

export default DashboardHeader;