import { useLanguage } from "../../../../context/LanguageContext";
import Spinner from "../Spinner/Spinner";
import styles from "./LoadingState.module.css";

function LoadingState({ message }) {
  const { translations, direction } = useLanguage();

  const text = message || translations.common?.loading || "Loading...";

  return (
    <div className={styles.wrapper} dir={direction}>
      <Spinner size={28} />
      <span>{text}</span>
    </div>
  );
}

export default LoadingState;