import { useNavigate } from "react-router-dom";
import { useLanguage } from "../../context/LanguageContext";
import { hasPermission } from "../../services/permissionService";
import Icon from "../../components/UI/Icon/Icon";
import styles from "./SystemHome.module.css";

function SystemHome() {
  const navigate = useNavigate();
  const { translations , isArabic } = useLanguage();

  const t = translations.moduleHub;

  const canOpenInventory =
    hasPermission("Home.View");

  const canOpenFinancial =
    hasPermission("FinancialAnalysis.View") ||
    hasPermission("FinancialDashboard.View") ||
    hasPermission("FinancialPeriod.View");

  return (
    <main className={styles.page}>
      <header className={styles.hero}>
        <div>
          <span className={styles.eyebrow}>
            {t.eyebrow}
          </span>

          <h1>
            {t.title}
          </h1>

          <p>
            {t.description}
          </p>
        </div>

        <div className={styles.systemBadge}>
          <span className={styles.systemDot} />
          {t.systemReady}
        </div>
      </header>

      <section className={styles.moduleGrid}>
        <article
          className={`${styles.moduleCard} ${styles.inventoryCard}`}
        >
          <div className={styles.cardAccent} />

          <div className={styles.cardTop}>
            <div className={`${styles.moduleIcon} ${styles.inventoryIcon}`}>
              <Icon name="inventory" size={30} />
            </div>

            <span className={styles.moduleStatus}>
              {t.available}
            </span>
          </div>

          <div className={styles.cardBody}>
            <span className={styles.moduleType}>
              {t.operationalModule}
            </span>

            <h2>
              {t.inventoryTitle}
            </h2>

            <p>
              {t.inventoryDescription}
            </p>

            <div className={styles.featureList}>
              <span>{t.inventoryFeature1}</span>
              <span>{t.inventoryFeature2}</span>
              <span>{t.inventoryFeature3}</span>
            </div>
          </div>

          <div className={styles.cardFooter}>
            <button
              type="button"
              disabled={!canOpenInventory}
              onClick={() => navigate("/")}
            >
              {t.openInventory}
              <span className={styles.arrow}>
                <Icon name={isArabic ? "arrowLeft" : "arrowRight"} size={16} />
              </span>
            </button>
          </div>
        </article>

        <article
          className={`${styles.moduleCard} ${styles.financialCard}`}
        >
          <div className={styles.cardAccent} />

          <div className={styles.cardTop}>
            <div className={`${styles.moduleIcon} ${styles.financialIcon}`}>
              <Icon name="dashboard" size={30} />
            </div>

            <span className={`${styles.moduleStatus} ${styles.financialStatus}`}>
              {t.available}
            </span>
          </div>

          <div className={styles.cardBody}>
            <span className={styles.moduleType}>
              {t.analyticsModule}
            </span>

            <h2>
              {t.financialTitle}
            </h2>

            <p>
              {t.financialDescription}
            </p>

            <div className={styles.featureList}>
              <span>{t.financialFeature1}</span>
              <span>{t.financialFeature2}</span>
              <span>{t.financialFeature3}</span>
            </div>
          </div>

          <div className={styles.cardFooter}>
            <button
              type="button"
              disabled={!canOpenFinancial}
              onClick={() => navigate("/financial-analysis")}
            >
              {t.openFinancial}
              <span className={styles.arrow}>
                <Icon name={isArabic ? "arrowLeft" : "arrowRight"} size={16} />
              </span>

            </button>
          </div>
        </article>
      </section>

      <section className={styles.bottomNote}>
        <div className={styles.noteIcon}>
          <Icon name="reports" size={20} />
        </div>

        <div>
          <strong>
            {t.workspaceTitle}
          </strong>

          <p>
            {t.workspaceDescription}
          </p>
        </div>
      </section>
    </main>
  );
}

export default SystemHome;