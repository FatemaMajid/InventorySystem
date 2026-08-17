import iconMap from './iconMap';
import styles from './Icon.module.css';

function Icon({
  name,
  size = 20,
  className = '',
  title,
}) {
  const icon = iconMap[name];

  if (!icon) {
    console.warn(`Icon "${name}" was not found.`);
    return null;
  }

  return (
    <span
      className={`${styles.icon} ${className}`}
      style={{
        width: `${size}px`,
        height: `${size}px`,
        '--icon-url': `url("${icon}")`,
      }}
      title={title}
      aria-hidden={!title}
    />
  );
}

export default Icon;