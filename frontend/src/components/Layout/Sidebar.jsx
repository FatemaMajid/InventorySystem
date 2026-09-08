import { NavLink } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { useLanguage } from "../../context/LanguageContext";
import { navigationGroups } from "../Navigation/navigationItems";
import Icon from "../UI/Icon/Icon";
import logo from "../../assets/logo/logo2.png";
import styles from "./Sidebar.module.css";

function Sidebar({
    isOpen = false,
    onClose,
}) {
    const { translations } = useLanguage();
    const { user, signOut } = useAuth();

    const getPermissions = () => {
        const token = localStorage.getItem("inventory_token");

        if (!token) {
            return [];
        }

        try {
            const payload = JSON.parse(atob(token.split(".")[1]));
            const permissions = payload.permission || [];

            return Array.isArray(permissions)
                ? permissions
                : [permissions];
        } catch {
            return [];
        }
    };

    const permissions = getPermissions();

    const hasPermission = (permission) => {
        if (!permission) {
            return true;
        }

        return permissions.some(
            (item) =>
                item.toLowerCase() === permission.toLowerCase()
        );
    };

    const isUserRole =
        user?.role?.toLowerCase() === "user";

    const visibleGroups = navigationGroups
        .filter(
            (group) =>
                !(isUserRole && group.key === "masterData")
        )
        .map((group) => ({
            ...group,
            items: group.items.filter(
                (item) =>
                    item.key === "home" ||
                    hasPermission(item.permission)
            ),
        }))
        .filter((group) => group.items.length > 0);

    const handleLogout = () => {
        signOut();
        onClose?.();
        window.location.href = "/login";
    };

    return (
        <aside
            className={`${styles.sidebar} ${
                isOpen ? styles.open : ""
            }`}
        >
            <div className={styles.logo}>
                <img
                    src={logo}
                    alt=""
                    className={styles.logoImage}
                />
            </div>

            <nav className={styles.navigation}>
                {visibleGroups.map((group) => (
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
                                            isActive
                                                ? styles.active
                                                : ""
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
                    onClick={handleLogout}
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