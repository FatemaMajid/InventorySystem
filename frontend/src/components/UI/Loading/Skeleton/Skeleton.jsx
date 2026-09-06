import styles from "./Skeleton.module.css";

function Skeleton({
  width = "100%",
  height = "1rem",
  borderRadius = "var(--radius-md)",
  className = "",
}) {
  return (
    <span
      className={`${styles.skeleton} ${className}`}
      style={{
        width,
        height,
        borderRadius,
      }}
      aria-hidden="true"
    />
  );
}

export default Skeleton;