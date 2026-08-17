import { useNavigate } from 'react-router-dom';
import { useLanguage } from '../../../context/LanguageContext';

import QuickActionCard from '../QuickActionCard/QuickActionCard';

import styles from './QuickActions.module.css';

function QuickActions() {
  const { translations } = useLanguage();
  const navigate = useNavigate();

  const actions = [
    {
      id: 'new-inventory',
      icon: 'plus',
      title: translations.home.newInventory,
      description: translations.home.newInventoryDescription,
      path: '/new-inventory-session',
    },
    {
      id: 'inventory-sessions',
      icon: 'inventory',
      title: translations.home.inventorySessions,
      description: translations.home.inventorySessionsDescription,
      path: '/inventory-sessions',
    },
    {
      id: 'reports',
      icon: 'reports',
      title: translations.home.reports,
      description: translations.home.reportsDescription,
      path: '/reports',
    },
  ];

  return (
    <section className={styles.section}>
      <div className={styles.header}>
        <h2>{translations.home.quickActions}</h2>

        <p>{translations.home.quickActionsSubtitle}</p>
      </div>

      <div className={styles.grid}>
        {actions.map((action) => (
          <QuickActionCard
            key={action.id}
            icon={action.icon}
            title={action.title}
            description={action.description}
            onClick={() => navigate(action.path)}
          />
        ))}
      </div>
    </section>
  );
}

export default QuickActions;