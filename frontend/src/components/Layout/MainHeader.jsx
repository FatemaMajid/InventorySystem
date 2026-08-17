import { useState } from 'react';
import { useLanguage } from '../../context/LanguageContext';
import { useTheme } from '../../context/ThemeContext';
import Icon from '../UI/Icon/Icon';
import styles from './MainHeader.module.css';

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

  const [changingLanguage, setChangingLanguage] =
    useState(false);

  const nextLanguage =
    language === 'ar' ? 'en' : 'ar';

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
      {/* Title */}
      <div className={styles.titleSection}>
        <button
          type="button"
          className={styles.menuButton}
          onClick={onMenuClick}
          aria-label="Open navigation"
        >
          ☰
        </button>

        <h1 className={styles.title}>
          {translations.common.systemName}
        </h1>
      </div>

      {/* Actions */}
      <div className={styles.actions}>
        {/* Theme */}
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
            name={isDark ? 'sun' : 'moon'}
            size={19}
          />
        </button>

        {/* Notifications */}
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

        {/* Language */}
        <button
          type="button"
          className={styles.languageButton}
          onClick={handleLanguageChange}
          disabled={changingLanguage}
          aria-label={
            language === 'ar'
              ? 'Change language to English'
              : 'تغيير اللغة إلى العربية'
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
                {language === 'ar'
                  ? 'English'
                  : 'العربية'}
              </span>
            </>
          )}
        </button>

        {/* Divider */}
        <div className={styles.divider} />

        {/* User */}
        <button
          type="button"
          className={styles.userButton}
        >
          <div className={styles.avatar}>
            U
          </div>

          <div className={styles.userInfo}>
            <span className={styles.userName}>
              {translations.common.user}
            </span>

            <span className={styles.userRole}>
              {translations.common.inventoryUser}
            </span>
          </div>
        </button>
      </div>
    </header>
  );
}

export default MainHeader;