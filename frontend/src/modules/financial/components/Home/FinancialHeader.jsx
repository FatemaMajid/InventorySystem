import Icon from "../../../../components/UI/Icon/Icon";
import styles from "../../screens/Home/FinancialHome.module.css";

export default function FinancialHeader({ t, loading, onRefresh, error }) {
  return (
    <>
      <header className={styles.header}>
        <div>
          <span className={styles.eyebrow}>{t.eyebrow}</span>
          <h1>{t.homeTitle}</h1>
          <p>{t.homeDescription}</p>
        </div>

        <button
          type="button"
          className={styles.refreshButton}
          onClick={onRefresh}
          disabled={loading}
        >
          {loading ? t.loading : t.refresh}
        </button>
      </header>

      {error && (
        <div className={styles.error}>
          {error}
        </div>
      )}
    </>
  );
}