import { useLanguage } from "../../../context/LanguageContext";
import { useNavigate } from "react-router-dom";
import Icon from "../../UI/Icon/Icon";
import styles from "./HomeHero.module.css";

function HomeHero({
  activeSessions = 0,
  totalSessions = 0,
  attentionItems = 0,
  canCreateInventory = false,
}) {
  const { translations, isArabic } = useLanguage();
  const navigate = useNavigate();
  const t = translations.home;

  const formatNumber = (value) =>
    new Intl.NumberFormat("en-US", {
      useGrouping: true,
    }).format(Number(value) || 0);

  const stats = [
    {
      key: "activeSessions",
      value: activeSessions,
      description: t.activeSessionsDescription,
      icon: "active",
      variant: "blue",
    },
    {
      key: "totalSessions",
      value: totalSessions,
      description: t.totalSessionsDescription,
      icon: "reports",
      variant: "green",
    },
    {
      key: "attentionItems",
      value: attentionItems,
      description: t.attentionItemsDescription,
      icon: "attention",
      variant: "orange",
    },
  ];

  return (
    <section
      className={styles.hero}
      dir={isArabic ? "rtl" : "ltr"}
    >
      <div className={styles.heroContent}>
        <div className={styles.greeting}>
          <span className={styles.greetingLabel}>
            {t.welcomeLabel}
          </span>
          <span className={styles.greetingUser}>
            {t.userName}
          </span>
          <Icon name="wave" size={15} />
        </div>

        <h1 className={styles.title}>
          {t.welcomeBack}
        </h1>

        <p className={styles.subtitle}>
          {t.subtitle}
        </p>

        <div className={styles.stats}>
          {stats.map((stat) => (
            <div
              key={stat.key}
              className={`${styles.statCard} ${styles[stat.variant]}`}
            >
              <div className={styles.statIcon}>
                <Icon name={stat.icon} size={25} />
              </div>
              <div className={styles.statContent}>
                <span className={styles.statLabel}>
                  {t[stat.key]}
                </span>
                <strong className={styles.statValue}>
                  {formatNumber(stat.value)}
                </strong>
                <span className={styles.statDescription}>
                  {stat.description}
                </span>
              </div>
            </div>
          ))}
        </div>
      </div>

      <div className={styles.visual}>
        <div className={styles.wave} />

        <div className={styles.illustration}>
          <div className={styles.clipboard}>
            <div className={styles.clip} />
            <div className={styles.paper}>
              <div className={styles.line}>
                <span />
                <i />
              </div>
              <div className={styles.line}>
                <span />
                <i />
              </div>
              <div className={styles.line}>
                <span />
                <i />
              </div>
              <div className={styles.line}>
                <span />
                <i />
              </div>
            </div>
          </div>

          <div className={`${styles.box} ${styles.boxOne}`} />
          <div className={`${styles.box} ${styles.boxTwo}`} />

          <div className={styles.plant}>
            <div className={styles.stem} />
            <div className={`${styles.leaf} ${styles.leafOne}`} />
            <div className={`${styles.leaf} ${styles.leafTwo}`} />
            <div className={`${styles.leaf} ${styles.leafThree}`} />
          </div>
        </div>

        {canCreateInventory && (
          <button
            type="button"
            className={styles.newInventoryButton}
            onClick={() =>
              navigate("/new-inventory-session")
            }
          >
            <Icon name="plus" size={17} />
            <span>{t.newInventory}</span>
            <span className={styles.arrow}>
              <Icon
                name={isArabic ? "arrowLeft" : "arrowRight"}
                size={17}
              />
            </span>
          </button>
        )}
      </div>
    </section>
  );
}

export default HomeHero;