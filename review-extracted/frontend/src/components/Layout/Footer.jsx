import { useLanguage } from "../../context/LanguageContext";
import Icon from "../UI/Icon/Icon";
import styles from "./Footer.module.css";

function Footer() {
  const { translations, isArabic } = useLanguage();

  const t = translations.footer;

  return (
    <footer
      className={styles.footer}
      dir={isArabic ? "rtl" : "ltr"}
    >
      <div className={styles.footerInner}>
        <div className={styles.system}>
          <div className={styles.systemIcon}>
            <Icon
              name="system"
              size={21}
            />
          </div>

          <div className={styles.systemContent}>
            <span className={styles.systemName}>
              {t.systemName}
            </span>

            <span className={styles.systemDescription}>
              {t.systemDescription}
            </span>
          </div>
        </div>

        <div className={styles.divider} />

        <div className={styles.credit}>
          <div className={styles.creditIcon}>
            <Icon
              name="copyright"
              size={20}
            />
          </div>

          <span className={styles.copyright}>
            {t.copyright}
          </span>

          <span className={styles.version}>
            {t.version}
          </span>
        </div>

        <div className={styles.divider} />

        <div className={styles.rights}>
          <div className={styles.rightsIcon}>
            <Icon
              name="lock"
              size={19}
            />
          </div>

          <span>
            {t.allRightsReserved}
          </span>
        </div>
      </div>
    </footer>
  );
}

export default Footer;