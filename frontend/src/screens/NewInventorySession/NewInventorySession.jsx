import SessionHeader from '../../components/Inventory/SessionHeader/SessionHeader';
import SessionInfoCard from '../../components/Inventory/SessionInfoCard/SessionInfoCard';
import InventoryFiles from '../../components/Inventory/InventoryFiles/InventoryFiles';
import RequiredColumns from '../../components/Inventory/RequiredColumns/RequiredColumns';
import SessionActions from '../../components/Inventory/SessionActions/SessionActions';

import styles from './NewInventorySession.module.css';

function NewInventorySession() {
  return (
    <section className={styles.page}>
      <SessionHeader />

      <SessionInfoCard />

      <InventoryFiles />

      <RequiredColumns />

      <SessionActions />
    </section>
  );
}

export default NewInventorySession;