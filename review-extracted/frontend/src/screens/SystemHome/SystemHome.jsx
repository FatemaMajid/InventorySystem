import { useNavigate } from "react-router-dom";
import { useLanguage } from "../../context/LanguageContext";
import { hasPermission } from "../../services/permissionService";
import Icon from "../../components/UI/Icon/Icon";
import styles from "./SystemHome.module.css";

function SystemHome() {
    const navigate = useNavigate();
    const { translations } = useLanguage();
    const canOpenInventory = hasPermission("Home.View");

    return (
        <section className={styles.page}>
            <header className={styles.header}>
                <h1>{translations.systemHome.title}</h1>
                <p>{translations.systemHome.description}</p>
            </header>
            <div className={styles.grid}>
                <article className={styles.card}>
                    <div className={styles.icon}><Icon name="inventory" size={32} /></div>
                    <h2>{translations.systemHome.inventoryTitle}</h2>
                    <p>{translations.systemHome.inventoryDescription}</p>
                    <button type="button" disabled={!canOpenInventory} onClick={() => navigate("/home")}>
                        {translations.systemHome.openInventory}
                    </button>
                </article>
                <article className={`${styles.card} ${styles.pending}`}>
                    <div className={styles.icon}><Icon name="reports" size={32} /></div>
                    <h2>{translations.systemHome.financialTitle}</h2>
                    <p>{translations.systemHome.financialDescription}</p>
                    <button type="button" disabled>{translations.systemHome.comingSoon}</button>
                </article>
            </div>
        </section>
    );
}

export default SystemHome;
