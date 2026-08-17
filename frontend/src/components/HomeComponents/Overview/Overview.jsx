import { useLanguage } from '../../../context/LanguageContext';

import StatCard from '../StatCard/StatCard';

import styles from './Overview.module.css';

function Overview() {
  const { translations } = useLanguage();

  const statistics = [
    {
      id: 'active-sessions',
      icon: 'inventory',
      label: translations.home.activeSessions,
    },
    {
      id: 'branches',
      icon: 'branches',
      label: translations.home.branches,
    },
    {
      id: 'attention-items',
      icon: 'attention',
      label: translations.home.attentionItems,
    },
    {
      id: 'reports',
      icon: 'reports',
      label: translations.home.reports,
    },
  ];

  return (
    <section className={styles.section}>
      <div className={styles.header}>
        <h2>{translations.home.overview}</h2>

        <p>{translations.home.overviewSubtitle}</p>
      </div>

      <div className={styles.grid}>
        {statistics.map((stat) => (
          <StatCard
            key={stat.id}
            icon={stat.icon}
            label={stat.label}
          />
        ))}
      </div>
    </section>
  );
}

export default Overview;