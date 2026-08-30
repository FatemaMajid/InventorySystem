import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./AttentionSummary.module.css";

const DEFAULT_DATA = {
    total: 0,
    newlyCounted: 0,
    fullyDepleted: 0,
    unitNotDefined: 0,
    priceChanged: 0,
};

function AttentionSummary({
    data = DEFAULT_DATA,
    onViewAll,
}) {
    const { translations, language, direction } = useLanguage();

    const isArabic = language === "ar";
    const t = translations.dashboard.attention;

    const items = [
        {
            id: "newlyCounted",
            key: "newlyCounted",
            value: data.newlyCounted,
            icon: "plus",
            type: "new",
        },
        {
            id: "fullyDepleted",
            key: "fullyDepleted",
            value: data.fullyDepleted,
            icon: "depleted",
            type: "depleted",
        },
        {
            id: "unitNotDefined",
            key: "unitNotDefined",
            value: data.unitNotDefined,
            icon: "attention",
            type: "unit",
        },
        {
            id: "priceChanged",
            key: "priceChanged",
            value: data.priceChanged,
            icon: "priceChange",
            type: "price",
        },
    ];

    return (
        <section
            className={styles.wrapper}
            dir={direction}
        >
            <div className={styles.header}>
                <div className={styles.titleGroup}>
                    <div className={styles.iconBox}>
                        <Icon
                            name="attention"
                            size={20}
                        />
                    </div>

                    <div className={styles.titleContent}>
                        <h2 className={styles.title}>
                            {t.title}
                            <span className={styles.total}>
                                {" "}
                                (
                                {Number(data.value || 0).toLocaleString("en-US")}
                                )
                            </span>
                        </h2>

                        <p className={styles.description}>
                            {t.description}
                        </p>
                    </div>
                </div>

                <button
                    type="button"
                    className={styles.viewAll}
                    onClick={onViewAll}
                >
                    <span>{t.viewAll}</span>

                    <Icon
                        name={isArabic ? "arrowLeft" : "arrowRight"}
                        size={16}
                    />
                </button>
            </div>

            <div className={styles.items}>
                {items.map((item) => (
                    <div
                        key={item.id}
                        className={`${styles.item} ${styles[item.type]}`}
                    >
                        <div className={styles.itemInfo}>
                            <span className={styles.itemTitle}>
                                {t[item.key]}
                            </span>

                            <strong className={styles.itemValue}>
                                {Number(item.value || 0).toLocaleString("en-US")}

                            </strong>
                        </div>

                        <button
                            type="button"
                            className={styles.itemAction}
                            onClick={() => onViewAll?.(item.key)}
                        >
                            <span>{t.view}</span>

                            <Icon
                                name={
                                    isArabic
                                        ? "arrowLeft"
                                        : "arrowRight"
                                }
                                size={14}
                            />
                        </button>
                    </div>
                ))}
            </div>
        </section>
    );
}

export default AttentionSummary;