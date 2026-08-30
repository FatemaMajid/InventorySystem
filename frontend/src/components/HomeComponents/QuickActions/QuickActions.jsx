import { useLanguage } from "../../../context/LanguageContext";
import { useNavigate } from "react-router-dom";
import Icon from "../../UI/Icon/Icon";
import styles from "./QuickActions.module.css";

function QuickActions() {
  const { translations, isArabic } = useLanguage();
  const navigate = useNavigate();

  const t = translations.home;

  const actions = [
    {
      key: "newInventory",
      title: t.newInventory,
      description: t.newInventoryDescription,
      icon: "plus",
      path: "/new-inventory-session",
      className: styles.blue,
    },
    {
      key: "inventorySessions",
      title: t.inventorySessions,
      description: t.inventorySessionsDescription,
      icon: "inventory",
      path: "/inventory-sessions",
      className: styles.purple,
    },
    {
      key: "reports",
      title: t.reports,
      description: t.reportsDescription,
      icon: "reports",
      path: "/reports",
      className: styles.green,
    },
  ];

  return (
    <section
      className={styles.wrapper}
      dir={isArabic ? "rtl" : "ltr"}
    >
      <div className={styles.cards}>
        {actions.map((action) => (
          <button
            key={action.key}
            type="button"
            className={styles.card}
            onClick={() => navigate(action.path)}
          >
            <div
              className={`${styles.iconBox} ${action.className}`}
            >
              <Icon
                name={action.icon}
                size={23}
              />
            </div>

            <div className={styles.content}>
              <h3>{action.title}</h3>

              <p>{action.description}</p>
            </div>

            <div className={styles.arrow}>
              <Icon
                name={isArabic ? "arrowLeft" : "arrowRight"}
                size={19}
              />
            </div>
          </button>
        ))}
      </div>
    </section>
  );
}

export default QuickActions;