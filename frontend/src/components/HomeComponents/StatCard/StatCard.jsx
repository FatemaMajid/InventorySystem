import Icon from '../../UI/Icon/Icon';

import styles from './StatCard.module.css';

function StatCard({
  icon,
  label,
  value = '—',
}) {
  return (
    <article className={styles.card}>
      <div className={styles.icon}>
        <Icon
          name={icon}
          size={22}
        />
      </div>

      <span className={styles.label}>
        {label}
      </span>

      <strong className={styles.value}>
        {value}
      </strong>
    </article>
  );
}

export default StatCard;