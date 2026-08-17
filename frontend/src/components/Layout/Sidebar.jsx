import { NavLink } from 'react-router-dom';
import { useLanguage } from '../../context/LanguageContext';
import { navigationGroups } from '../Navigation/navigationItems';
import Icon from '../UI/Icon/Icon';
import styles from './Sidebar.module.css';

function Sidebar({
  isOpen = false,
  onClose,
}) {
  const { translations } = useLanguage();

  return (
    <aside
      className={`${styles.sidebar} ${
        isOpen ? styles.open : ''
      }`}
    >
      <div className={styles.logo}>
        <div className={styles.logoIcon}>
          IS
        </div>

        <span>
          {translations.common.systemName}
        </span>
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
                      isActive ? styles.active : ''
                    }`
                  }
                >
                  <Icon
                    name={item.icon}
                    size={19}
                  />

                  <span>
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
          <Icon
            name="logout"
            size={19}
          />

          <span>
            {translations.common.logout}
          </span>
        </button>
      </div>
    </aside>
  );
}

export default Sidebar;