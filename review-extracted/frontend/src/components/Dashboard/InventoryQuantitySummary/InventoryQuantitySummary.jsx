import Icon from "../../UI/Icon/Icon";
import { useLanguage } from "../../../context/LanguageContext";
import styles from "./InventoryQuantitySummary.module.css";

function InventoryQuantitySummary({
    totalQuantityBefore = 0,
    totalQuantityAfter = 0,
    quantityDifference = 0,
    quantityIncrease = 0,
    quantityDecrease = 0,
}) {
    const { translations, language } = useLanguage();
    const t = translations.dashboard.inventoryQuantity;

    const formatNumber = (value) =>
    Number(value || 0).toLocaleString("en-US", {
        maximumFractionDigits: 2,
    });

    const items = [
        {
            id: "before",
            label: t.totalQuantityBefore,
            value: totalQuantityBefore,
            icon: "inventory",
            type: "before",
        },
        {
            id: "after",
            label: t.totalQuantityAfter,
            value: totalQuantityAfter,
            icon: "inventory",
            type: "after",
        },
        {
            id: "difference",
            label: t.quantityDifference,
            value: quantityDifference,
            icon: "priceChange",
            type: quantityDifference >= 0 ? "increase" : "decrease",
        },
        {
            id: "increase",
            label: t.quantityIncrease,
            value: quantityIncrease,
            icon: "increase",
            type: "increase",
        },
        {
            id: "decrease",
            label: t.quantityDecrease,
            value: quantityDecrease,
            icon: "decrease",
            type: "decrease",
        },
    ];

    return (
        <section className={styles.wrapper}>
            <div className={styles.header}>
                <div className={styles.headerIcon}>
                    <Icon name="inventory" size={20} />
                </div>
                <h2>{t.title}</h2>
            </div>

            <div className={styles.cards}>
                {items.map((item) => (
                    <article
                        key={item.id}
                        className={`${styles.card} ${styles[item.type]}`}
                    >
                        <div className={styles.iconBox}>
                            <Icon name={item.icon} size={20} />
                        </div>
                        <div className={styles.content}>
                            <span className={styles.label}>{item.label}</span>
                            <strong className={styles.value}>
                                {formatNumber(item.value)}
                            </strong>
                        </div>
                    </article>
                ))}
            </div>
        </section>
    );
}

export default InventoryQuantitySummary;