import styles from "./Spinner.module.css";

function Spinner({ size = 24 }) {
  return (
    <span
      className={styles.spinner}
      style={{ width: size, height: size }}
      aria-hidden="true"
    />
  );
}

export default Spinner;