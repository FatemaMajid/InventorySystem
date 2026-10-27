import { useState } from "react";
import { useLanguage } from "../../context/LanguageContext";
import { useTheme } from "../../context/ThemeContext";
import { useAuth } from "../../context/AuthContext";
import Icon from "../UI/Icon/Icon";
import styles from "./MainHeader.module.css";

function MainHeader({ onMenuClick }) {
    const {
        language,
        setLanguage,
        translations,
    } = useLanguage();

    const {
        isDark,
        toggleTheme,
    } = useTheme();

    const {
        user,
    } = useAuth();

    const [changingLanguage, setChangingLanguage] = useState(false);

    const nextLanguage = language === "ar" ? "en" : "ar";

    const username =
        user?.username ||
        translations.common.user;

    const role =
        user?.role ||
        translations.common.inventoryUser;

    const avatar =
        username?.trim()?.charAt(0)?.toUpperCase() ||
        "U";

    const handleLanguageChange = () => {
        if (changingLanguage) {
            return;
        }

        setChangingLanguage(true);

        setTimeout(() => {
            setLanguage(nextLanguage);
            setChangingLanguage(false);
        }, 250);
    };

    return (
        <header className={styles.header}>
            <div className={styles.titleSection}>
                <button
                    type="button"
                    className={styles.menuButton}
                    onClick={onMenuClick}
                    aria-label={translations.common.menu}
                >
                    <Icon
                        name="menu"
                        size={20}
                    />
                </button>

                <h1 className={styles.title}>
                    {translations.common.systemName}
                </h1>
            </div>

            <div className={styles.actions}>
                <button
                    type="button"
                    className={styles.iconButton}
                    onClick={toggleTheme}
                    aria-label={
                        isDark
                            ? translations.common.lightMode
                            : translations.common.darkMode
                    }
                >
                    <Icon
                        name={isDark ? "sun" : "moon"}
                        size={19}
                    />
                </button>

                <button
                    type="button"
                    className={styles.iconButton}
                    aria-label={translations.common.notifications}
                >
                    <Icon
                        name="notification"
                        size={19}
                    />
                </button>

                <button
                    type="button"
                    className={styles.languageButton}
                    onClick={handleLanguageChange}
                    disabled={changingLanguage}
                    aria-label={
                        language === "ar"
                            ? translations.common.changeLanguageToEnglish
                            : translations.common.changeLanguageToArabic
                    }
                >
                    {changingLanguage ? (
                        <span className={styles.languageLoader} />
                    ) : (
                        <>
                            <Icon
                                name="language"
                                size={18}
                            />

                            <span>
                                {language === "ar"
                                    ? translations.common.english
                                    : translations.common.arabic}
                            </span>
                        </>
                    )}
                </button>

                <div className={styles.divider} />

                <button
                    type="button"
                    className={styles.userButton}
                >
                    <div className={styles.avatar}>
                        {avatar}
                    </div>

                    <div className={styles.userInfo}>
                        <span className={styles.userName}>
                            {username}
                        </span>

                        <span className={styles.userRole}>
                            {role}
                        </span>
                    </div>
                </button>
            </div >
        </header >
    );
}

export default MainHeader;