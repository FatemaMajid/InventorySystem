import { useLanguage } from "../../../context/LanguageContext";

import styles from "./ComparisonTable.module.css";

const formatNumber = (value) => {
  if (
    value === null ||
    value === undefined
  ) {
    return "—";
  }

  return Number(value).toLocaleString("en-US");
};

const getDifferenceClass = (value) => {
  const number = Number(value || 0);

  if (number > 0) {
    return styles.positive;
  }

  if (number < 0) {
    return styles.negative;
  }

  return "";
};

const statusTranslationKey = {
  Increase: "increase",
  Decrease: "decrease",
  NoDifference: "noDifference",
  AfterOnly: "afterOnly",
  BeforeOnly: "beforeOnly",
};

function ComparisonTable({
  items = [],
  loading = false,
  error = "",
  page = 1,
  pageSize = 20,
}) {
  const { translations, direction } =
    useLanguage();

  const t =
    translations.dashboard.comparisonResults;

  const renderMessage = (message) => (
    <tr>
      <td
        colSpan="15"
        className={styles.empty}
      >
        {message}
      </td>
    </tr>
  );

  return (
    <section
      className={styles.wrapper}
      dir={direction}
    >
      <div className={styles.tableWrapper}>
        <table className={styles.table}>
          <thead>
            <tr>
              <th>#</th>
              <th>{t.itemCode}</th>
              <th>{t.itemName}</th>
              <th>{t.category}</th>
              <th>{t.unit}</th>
              <th>{t.quantityBefore}</th>
              <th>{t.quantityAfter}</th>
              <th>
                {t.quantityDifference}
              </th>
              <th>
                {t.differencePercentage}
              </th>
              <th>{t.priceBefore}</th>
              <th>{t.priceAfter}</th>
              <th>{t.beforeValue}</th>
              <th>{t.afterValue}</th>
              <th>{t.valueDifference}</th>
              <th>{t.status}</th>
            </tr>
          </thead>

          <tbody>
            {loading &&
              renderMessage(t.loading)}

            {!loading &&
              error &&
              renderMessage(error)}

            {!loading &&
              !error &&
              items.length === 0 &&
              renderMessage(t.noResults)}

            {!loading &&
              !error &&
              items.map((item, index) => {
                const rowNumber =
                  (page - 1) *
                    pageSize +
                  index +
                  1;

                const statusKey =
                  statusTranslationKey[
                    item.status
                  ];

                return (
                  <tr
                    key={
                      item.inventoryDetailId ??
                      item.itemId ??
                      item.itemCode ??
                      rowNumber
                    }
                  >
                    <td>{rowNumber}</td>

                    <td>
                      {item.itemCode || "—"}
                    </td>

                    <td>
                      {item.itemName1 || "—"}
                    </td>

                    <td>
                      {item.categoryName || "—"}
                    </td>

                    <td>
                      {item.unitName || "—"}
                    </td>

                    <td>
                      {formatNumber(
                        item.quantityBefore
                      )}
                    </td>

                    <td>
                      {formatNumber(
                        item.quantityAfter
                      )}
                    </td>

                    <td
                      className={getDifferenceClass(
                        item.quantityDifference
                      )}
                    >
                      {formatNumber(
                        item.quantityDifference
                      )}
                    </td>

                    <td
                      className={getDifferenceClass(
                        item.differencePercentage
                      )}
                    >
                      {item.differencePercentage !==
                        null &&
                      item.differencePercentage !==
                        undefined
                        ? `${Number(
                            item.differencePercentage
                          ).toFixed(2)}%`
                        : "—"}
                    </td>

                    <td>
                      {formatNumber(
                        item.consumerPriceBefore
                      )}
                    </td>

                    <td>
                      {formatNumber(
                        item.consumerPriceAfter
                      )}
                    </td>

                    <td>
                      {formatNumber(
                        item.beforeValue
                      )}
                    </td>

                    <td>
                      {formatNumber(
                        item.afterValue
                      )}
                    </td>

                    <td
                      className={getDifferenceClass(
                        item.valueDifference
                      )}
                    >
                      {formatNumber(
                        item.valueDifference
                      )}
                    </td>

                    <td>
                      <span
                        className={`${styles.status} ${
                          styles[
                            statusKey || ""
                          ] || ""
                        }`}
                      >
                        {t.statuses?.[
                          statusKey
                        ] ||
                          item.status ||
                          "—"}
                      </span>
                    </td>
                  </tr>
                );
              })}
          </tbody>
        </table>
      </div>
    </section>
  );
}

export default ComparisonTable;