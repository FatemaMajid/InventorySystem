import { useEffect, useState } from "react";
import { useLanguage } from "../../../context/LanguageContext";
import auditLogService from "../../../services/auditLogService";
import Icon from "../../UI/Icon/Icon";
import styles from "./AuditLogDetails.module.css";

const AuditLogDetails = ({ log, onClose }) => {
    const { language, translations, formatDateTime } = useLanguage();
    const [details, setDetails] = useState(log);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (!log?.id) {
            setDetails(null);
            return;
        }

        const loadDetails = async () => {
            setLoading(true);

            try {
                const response = await auditLogService.getById(log.id);
                setDetails(response?.data ?? response ?? log);
            } catch (error) {
                console.error("Failed to load audit log details:", error);
                setDetails(log);
            } finally {
                setLoading(false);
            }
        };

        loadDetails();
    }, [log]);

    if (!log) {
        return null;
    }

    const parseDetails = (value) => {
        if (!value) {
            return null;
        }

        if (typeof value === "object") {
            return value;
        }

        try {
            return JSON.parse(value);
        } catch {
            return null;
        }
    };

    const parsedDetails = parseDetails(details?.details);
    const isArabic = language === "ar";

    const getLocalizedName = (arabicName, englishName) => {
        return isArabic
            ? arabicName || englishName
            : englishName || arabicName;
    };

    const renderEntityDetails = () => {
        if (!parsedDetails) {
            return null;
        }

        if (details?.entity === "Branch") {
            return (
                <>
                    <div className={styles.item}>
                        <span>{translations.auditLog?.details?.name}</span>
                        <strong>
                            {getLocalizedName(
                                parsedDetails.branchNameArabic,
                                parsedDetails.branchNameEnglish
                            )}
                        </strong>
                    </div>
                    <div className={styles.item}>
                        <span>{translations.auditLog?.details?.code}</span>
                        <strong>{parsedDetails.branchCode}</strong>
                    </div>
                </>
            );
        }

        if (details?.entity === "Store") {
            return (
                <>
                    <div className={styles.item}>
                        <span>{translations.auditLog?.details?.name}</span>
                        <strong>
                            {getLocalizedName(
                                parsedDetails.storeNameArabic,
                                parsedDetails.storeNameEnglish
                            )}
                        </strong>
                    </div>
                    <div className={styles.item}>
                        <span>{translations.auditLog?.details?.code}</span>
                        <strong>{parsedDetails.storeCode}</strong>
                    </div>
                    <div className={styles.item}>
                        <span>
                            {translations.auditLog?.details?.branchCode}
                        </span>
                        <strong>{parsedDetails.branchCode}</strong>
                    </div>
                </>
            );
        }

        return null;
    };

    return (
        <div className={styles.overlay} role="presentation" onClick={onClose}>
            <section
                className={styles.panel}
                role="dialog"
                aria-modal="true"
                onClick={(event) => event.stopPropagation()}
            >
                <div className={styles.header}>
                    <div className={styles.title}>
                        <div className={styles.iconBox}>
                            <Icon name="eye" size={20} />
                        </div>
                        <span>{translations.auditLog?.details?.title}</span>
                    </div>
                    <button
                        type="button"
                        className={styles.close}
                        onClick={onClose}
                        aria-label={translations.auditLog?.details?.close}
                    >
                        <Icon name="close" size={20} />
                    </button>
                </div>
                <div className={styles.content}>
                    {loading ? (
                        <div className={styles.item}>
                            <span>{translations.auditLog?.table?.status}</span>
                            <strong>
                                {translations.auditLog?.table?.loading}
                            </strong>
                        </div>
                    ) : (
                        <>
                            <div className={styles.item}>
                                <span>{translations.auditLog?.table?.date}</span>
                                <strong>
                                    {formatDateTime(details?.createdAt)}
                                </strong>
                            </div>
                            <div className={styles.item}>
                                <span>{translations.auditLog?.table?.user}</span>
                                <strong>{details?.userName}</strong>
                            </div>
                            <div className={styles.item}>
                                <span>{translations.auditLog?.table?.action}</span>
                                <strong>
                                    {translations.auditLog?.actions?.[
                                        details?.action
                                    ] || details?.action}
                                </strong>
                            </div>
                            <div className={styles.item}>
                                <span>{translations.auditLog?.table?.entity}</span>
                                <strong>
                                    {translations.auditLog?.entities?.[
                                        details?.entity
                                    ] || details?.entity}
                                </strong>
                            </div>
                            <div className={styles.item}>
                                <span>{translations.auditLog?.table?.status}</span>
                                <strong>
                                    {translations.auditLog?.status?.[
                                        details?.status
                                    ] || details?.status}
                                </strong>
                            </div>
                            {renderEntityDetails()}
                        </>
                    )}
                </div>
            </section>
        </div>
    );
};

export default AuditLogDetails;