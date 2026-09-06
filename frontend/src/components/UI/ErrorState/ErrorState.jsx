import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../Icon/Icon";
import styles from "./ErrorState.module.css";

function ErrorState({
  message,
  onRetry,
}) {
  const { translations, direction } = useLanguage();

  const text =
    message ||
    translations.common?.error ||
    "Something went wrong.";

  const retryText =
    translations.common?.retry ||
    "Retry";

  return (
    <div className={styles.wrapper} dir={direction}>
      <div className={styles.icon}>
        <Icon name="attention" size={28} />
      </div>

      <span>{text}</span>

      {onRetry && (
        <button
          type="button"
          className={styles.retry}
          onClick={onRetry}
        >
          {retryText}
        </button>
      )}
    </div>
  );
}

export default ErrorState;