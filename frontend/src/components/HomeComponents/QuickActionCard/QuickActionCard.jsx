import Icon from '../../UI/Icon/Icon';

import styles from './QuickActionCard.module.css';

function QuickActionCard({
  icon,
  title,
  description,
  onClick,
}) {
  return (
    <button
      type="button"
      className={styles.card}
      onClick={onClick}
    >
      <span className={styles.icon}>
        <Icon name={icon} size={22} />
      </span>

      <span className={styles.content}>
        <strong>{title}</strong>
        <span>{description}</span>
      </span>

      <Icon
        name="arrowRight"
        size={18}
        className={styles.arrow}
      />
    </button>
  );
}

export default QuickActionCard;