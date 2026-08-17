import WelcomeCard from '../../components/HomeComponents/WelcomeCard/WelcomeCard';
import QuickActions from '../../components/HomeComponents/QuickActions/QuickActions';
import Overview from '../../components/HomeComponents/Overview/Overview';

import styles from './Home.module.css';

function Home() {
  return (
    <section className={styles.home}>
      <WelcomeCard />
      <QuickActions />
      <Overview />
    </section>
  );
}

export default Home;