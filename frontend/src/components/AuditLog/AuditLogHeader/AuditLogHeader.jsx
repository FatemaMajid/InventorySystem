import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./AuditLogHeader.module.css";

const AuditLogHeader = () => {
    const { translations } = useLanguage();

    return (
        <header className={styles.header}>
            <div className={styles.heading}>
                <div className={styles.iconBox}>
                    <Icon name="history" size={24} />
                </div>
                <div className={styles.text}>
                    <h1>{translations.auditLog?.title}</h1>
                    <p>{translations.auditLog?.description}</p>
                </div>
            </div>
        </header>
    );
};

export default AuditLogHeader;