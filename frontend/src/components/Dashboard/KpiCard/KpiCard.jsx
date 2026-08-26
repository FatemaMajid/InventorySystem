import Icon from "../../UI/Icon/Icon";
import { useLanguage } from "../../../context/LanguageContext";
import styles from "./KpiCard.module.css";

const DEFAULT_CARDS = [
    {
        id: "total",
        key: "totalItems",
        value: 0,
        icon: "totalItems",
        type: "total",
    },
    {
        id: "increase",
        key: "increase",
        value: 0,
        icon: "increase",
        type: "increase",
    },
    {
        id: "decrease",
        key: "decrease",
        value: 0,
        icon: "decrease",
        type: "decrease",
    },
    {
        id: "match",
        key: "match",
        value: 0,
        icon: "tick",
        type: "match",
    },
    {
        id: "newly-counted",
        key: "newlyCounted",
        value: 0,
        icon: "plus",
        type: "new",
    },
    {
        id: "fully-depleted",
        key: "fullyDepleted",
        value: 0,
        icon: "depleted",
        type: "depleted",
    },
    {
        id: "price-changed",
        key: "priceChanged",
        value: 0,
        icon: "priceChange",
        type: "price",
    },
    {
        id: "unit-not-defined",
        key: "unitNotDefined",
        value: 0,
        icon: "changeUnit",
        type: "unit",
    },
];

function KpiCard({
    cards = DEFAULT_CARDS,
    loading = false,
    onCardClick,
}) {
    const { translations, language } = useLanguage();

    const t = translations.dashboard.kpiCards;

    return (
        <section className={styles.wrapper}>
            <div className={styles.cards}>
                {cards.map((card) => (
                    <article
                        key={card.id}
                        className={`${styles.card} ${
                            styles[card.type] || ""
                        } ${
                            onCardClick ? styles.clickable : ""
                        }`}
                        onClick={() => onCardClick?.(card)}
                    >
                        <div className={styles.cardTop}>
                            <div className={styles.iconBox}>
                                <Icon
                                    name={card.icon}
                                    size={22}
                                    className={styles.icon}
                                />
                            </div>

                            {card.change !== undefined && (
                                <span
                                    className={`${styles.change} ${
                                        card.change >= 0
                                            ? styles.positive
                                            : styles.negative
                                    }`}
                                >
                                    {card.change >= 0 ? "+" : ""}
                                    {card.change}%
                                </span>
                            )}
                        </div>

                        <div className={styles.content}>
                            <span className={styles.title}>
                                {t[card.key]}
                            </span>

                            {loading ? (
                                <div className={styles.skeletonValue} />
                            ) : (
                                <strong className={styles.value}>
                                     {Number(card.value || 0).toLocaleString("en-US")}

                                </strong>
                            )}
                        </div>
                    </article>
                ))}
            </div>
        </section>
    );
}

export default KpiCard;