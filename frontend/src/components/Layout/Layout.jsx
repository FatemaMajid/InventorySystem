import { useEffect, useState } from "react";
import { Outlet } from "react-router-dom";

import { useLanguage } from "../../context/LanguageContext";

import Sidebar from "./Sidebar";
import MainHeader from "./MainHeader";
import Footer from "./Footer";

import styles from "./Layout.module.css";

function Layout() {
  const { language, direction } = useLanguage();

  const [sidebarOpen, setSidebarOpen] = useState(false);

  useEffect(() => {
    document.documentElement.lang = language;
    document.documentElement.dir = direction;
  }, [language, direction]);

  const toggleSidebar = () => {
    setSidebarOpen((current) => !current);
  };

  const closeSidebar = () => {
    setSidebarOpen(false);
  };

  return (
    <div
      className={styles.layout}
      dir={direction}
    >
      <Sidebar
        isOpen={sidebarOpen}
        onClose={closeSidebar}
      />

      {sidebarOpen && (
        <button
          type="button"
          className={styles.overlay}
          onClick={closeSidebar}
          aria-label="Close navigation"
        />
      )}

      <div className={styles.main}>
        <MainHeader
          onMenuClick={toggleSidebar}
        />

        <div className={styles.scrollArea}>
          <main className={styles.content}>
            <Outlet />
          </main>

          <Footer />
        </div>
      </div>
    </div>
  );
}

export default Layout;