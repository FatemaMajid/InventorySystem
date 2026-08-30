import { NavLink } from "react-router-dom";
import { useLanguage } from "../../context/LanguageContext";
import { navigationGroups } from "../Navigation/navigationItems";
import Icon from "../UI/Icon/Icon";
import styles from "./Sidebar.module.css";

function Sidebar({
  isOpen = false,
  onClose,
}) {
  const { translations } = useLanguage();

  return (
    <aside
      className={`${styles.sidebar} ${
        isOpen ? styles.open : ""
      }`}
    >
      <div className={styles.logo}>
        <div className={styles.logoIcon}>
          <span>
            {translations.common.systemName
              .split(" ")
              .map((word) => word.charAt(0))
              .slice(0, 2)
              .join("")}
          </span>
        </div>

        <div className={styles.logoText}>
          {translations.common.systemName}
        </div>
      </div>

      <nav className={styles.navigation}>
        {navigationGroups.map((group) => (
          <div
            key={group.key}
            className={styles.group}
          >
            <div className={styles.groupTitle}>
              {translations.navigationGroups[group.key]}
            </div>

            <div className={styles.groupItems}>
              {group.items.map((item) => (
                <NavLink
                  key={item.key}
                  to={item.path}
                  onClick={onClose}
                  className={({ isActive }) =>
                    `${styles.navItem} ${
                      isActive ? styles.active : ""
                    }`
                  }
                >
                  <span className={styles.navIcon}>
                    <Icon
                      name={item.icon}
                      size={18}
                    />
                  </span>

                  <span className={styles.navLabel}>
                    {translations.navigation[item.key]}
                  </span>
                </NavLink>
              ))}
            </div>
          </div>
        ))}
      </nav>

      <div className={styles.footer}>
        <button
          type="button"
          className={styles.logout}
        >
          <span className={styles.logoutIcon}>
            <Icon
              name="logout"
              size={18}
            />
          </span>

          <span className={styles.logoutLabel}>
            {translations.common.logout}
          </span>
        </button>
      </div>
    </aside>
  );
}

export default Sidebar;