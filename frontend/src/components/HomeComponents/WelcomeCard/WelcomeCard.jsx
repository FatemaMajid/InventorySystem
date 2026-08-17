import { useLanguage } from '../../../context/LanguageContext';
import Icon from '../../UI/Icon/Icon';

import styles from './WelcomeCard.module.css';

function WelcomeCard() {
  const { translations } = useLanguage();

  return (
    <section className={styles.card}>
      <div className={styles.content}>
        <span className={styles.eyebrow}>
          {translations.home.welcomeLabel}
        </span>

        <h1 className={styles.title}>
          {translations.home.title}
        </h1>

        <p className={styles.subtitle}>
          {translations.home.subtitle}
        </p>
      </div>

      <div className={styles.icon}>
        <Icon
          name="inventory"
          size={34}
        />
      </div>
    </section>
  );
}

export default WelcomeCard;