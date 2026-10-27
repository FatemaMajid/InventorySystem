import { useNavigate } from "react-router-dom";
import { useLanguage } from "../context/LanguageContext";
import Icon from "../components/UI/Icon/Icon";
import styles from "./SystemHome.module.css";

function SystemHome() {
  const navigate = useNavigate();
  const { language } = useLanguage();
  const isArabic = language === "ar";

  return (
    <section className={styles.page}>
      <div className={styles.header}>
        <span className={styles.eyebrow}>{isArabic ? "نظام إدارة المخزون" : "Inventory Management System"}</span>
        <h1>{isArabic ? "الرئيسية" : "Main Home"}</h1>
        <p>{isArabic ? "اختر القسم الذي تريد العمل عليه." : "Choose the module you want to work with."}</p>
      </div>

      <div className={styles.grid}>
        <button type="button" className={styles.card} onClick={() => navigate("/")}>
          <span className={styles.icon}><Icon name="inventory" size={30} /></span>
          <span className={styles.content}>
            <strong>{isArabic ? "الجرد" : "Inventory"}</strong>
            <small>{isArabic ? "الدخول إلى نظام الجرد الحالي بكل تفاصيله." : "Open the existing inventory module and all of its features."}</small>
          </span>
          <span className={styles.action}>{isArabic ? "الدخول إلى الجرد" : "Open Inventory"}</span>
        </button>

        <div className={`${styles.card} ${styles.disabledCard}`} aria-disabled="true">
          <span className={styles.icon}><Icon name="reports" size={30} /></span>
          <span className={styles.content}>
            <strong>{isArabic ? "التحليل المالي" : "Financial Analysis"}</strong>
            <small>{isArabic ? "القسم المالي الجديد لرفع وتحليل البيانات المالية." : "The new module for uploading and analyzing financial data."}</small>
          </span>
          <span className={styles.comingSoon}>{isArabic ? "قريباً" : "Coming soon"}</span>
        </div>
      </div>
    </section>
  );
}

export default SystemHome;
