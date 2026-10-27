import { useLanguage } from "../../../../context/LanguageContext";
import styles from "./FinancialSection.module.css";

function FinancialSection({
  sectionKey,
  tone = "blue",
}) {
  const { translations } =
    useLanguage();

  const t =
    translations.financialModule;

  const title =
    t.sections[sectionKey] ||
    sectionKey;

  return (
    <main className={styles.page}>
      <header className={styles.header}>
        <span
          className={`${styles.marker} ${styles[tone]}`}
        />

        <div>
          <span className={styles.eyebrow}>
            {t.eyebrow}
          </span>

          <h1>
            {title}
          </h1>

          <p>
            {t.sectionWorkspaceDescription}
          </p>
        </div>
      </header>

      <section className={styles.workspace}>
        <article className={styles.workspaceCard}>
          <span className={styles.step}>
            01
          </span>

          <strong>
            {t.sectionStructure.overview}
          </strong>

          <p>
            {t.sectionStructure.overviewDescription}
          </p>
        </article>

        <article className={styles.workspaceCard}>
          <span className={styles.step}>
            02
          </span>

          <strong>
            {t.sectionStructure.comparison}
          </strong>

          <p>
            {t.sectionStructure.comparisonDescription}
          </p>
        </article>

        <article className={styles.workspaceCard}>
          <span className={styles.step}>
            03
          </span>

          <strong>
            {t.sectionStructure.details}
          </strong>

          <p>
            {t.sectionStructure.detailsDescription}
          </p>
        </article>
      </section>

      <div className={styles.note}>
        {t.sectionImplementationNote}
      </div>
    </main>
  );
}

export default FinancialSection;