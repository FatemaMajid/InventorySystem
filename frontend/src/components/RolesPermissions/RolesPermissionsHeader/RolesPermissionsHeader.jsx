import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./RolesPermissionsHeader.module.css";

function RolesPermissionsHeader({ onAdd, canEdit = false }) {
    const { translations } = useLanguage();
    const t = translations.rolesPermissions || {};

    return (
        <header className={styles.header}>
            <div className={styles.heading}>
                <div className={styles.iconBox}>
                    <Icon name="roles" size={22} />
                </div>

                <div>
                    <h1>{t.title}</h1>
                    <p>{t.description}</p>
                </div>
            </div>

            {canEdit && (
                <button
                    type="button"
                    className={styles.addButton}
                    onClick={onAdd}
                >
                    <Icon name="plus" size={18} />
                    <span>{t.addRole}</span>
                </button>
            )}
        </header>
    );
}

export default RolesPermissionsHeader;