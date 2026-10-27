import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./UserHeader.module.css";

function UserHeader({ onAdd, canCreate = false }) {
    const { translations, direction } = useLanguage();
    const t = translations.users;

    return (
        <header className={styles.header} dir={direction}>
            <div className={styles.identity}>
                <div className={styles.iconBox}>
                    <Icon name="users" size={25} />
                </div>
                <div className={styles.info}>
                    <span className={styles.eyebrow}>
                        {translations.navigationGroups?.administration || "Administration"}
                    </span>
                    <h1 className={styles.title}>
                        {translations.navigation.users}
                    </h1>
                    <p className={styles.description}>
                        {t.description}
                    </p>
                </div>
            </div>

            {canCreate && (
                <button
                    type="button"
                    className={styles.addButton}
                    onClick={onAdd}
                >
                    <Icon name="plus" size={18} />
                    <span>{t.addUser}</span>
                </button>
            )}
        </header>
    );
}

export default UserHeader;