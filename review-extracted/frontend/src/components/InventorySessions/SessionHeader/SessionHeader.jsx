import { useLanguage } from "../../../context/LanguageContext";
import { hasPermission } from "../../../services/permissionService";
import { useNavigate } from "react-router-dom";
import Icon from "../../UI/Icon/Icon";
import styles from "./SessionHeader.module.css";

function SessionHeader() {
    const { translations, direction } = useLanguage();
    const navigate = useNavigate();
    const canCreateInventory = hasPermission("InventorySession.Create");

    return (
        <header className={styles.header} dir={direction}>
            <div className={styles.info}>
                <h1 className={styles.title}>
                    {translations.navigation.inventorySessions}
                </h1>
                <p className={styles.description}>
                    {translations.home.inventorySessionsDescription}
                </p>
            </div>

            {canCreateInventory && (
                <button
                    type="button"
                    className={styles.newButton}
                    onClick={() => navigate("/new-inventory-session")}
                >
                    <Icon name="plus" size={18} />
                    <span>
                        {translations.inventorySessions?.newSession}
                    </span>
                </button>
            )}
        </header>
    );
}

export default SessionHeader;