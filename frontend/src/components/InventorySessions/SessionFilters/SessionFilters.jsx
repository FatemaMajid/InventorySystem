import { useState } from "react";
import { useLanguage } from "../../../context/LanguageContext";
import Icon from "../../UI/Icon/Icon";
import styles from "./SessionFilters.module.css";

function SessionFilters({
  filters = {},
  onFiltersChange,
  branches = [],
  stores = [],
}) {
  const { translations, direction } = useLanguage();
  const t = translations.inventorySessions;
  const [open, setOpen] = useState(null);

  const isActive = (item) =>
    item?.isActive === true || item?.IsActive === true;

  const getId = (item) =>
    item?.id ??
    item?.Id ??
    item?.branchId ??
    item?.BranchId;

  const getBranchCode = (item) =>
    item?.branchCode ??
    item?.BranchCode;

  const getStoreBranchId = (store) =>
    store?.branchId ??
    store?.BranchId ??
    store?.branch?.id ??
    store?.Branch?.id;

  const getStoreBranchCode = (store) =>
    store?.branchCode ??
    store?.BranchCode ??
    store?.branch?.code ??
    store?.Branch?.code;

  const getBranchName = (item) =>
    direction === "rtl"
      ? item?.branchNameArabic ??
        item?.BranchNameArabic ??
        item?.nameArabic ??
        item?.NameArabic ??
        item?.branchName ??
        item?.BranchName ??
        item?.name ??
        item?.Name ??
        getBranchCode(item)
      : item?.branchNameEnglish ??
        item?.BranchNameEnglish ??
        item?.nameEnglish ??
        item?.NameEnglish ??
        item?.branchName ??
        item?.BranchName ??
        item?.name ??
        item?.Name ??
        getBranchCode(item);

  const getStoreName = (store) =>
    direction === "rtl"
      ? store?.storeNameArabic ??
        store?.StoreNameArabic ??
        store?.nameArabic ??
        store?.NameArabic ??
        store?.storeName ??
        store?.StoreName ??
        store?.name ??
        store?.Name ??
        store?.storeCode ??
        store?.StoreCode
      : store?.storeNameEnglish ??
        store?.StoreNameEnglish ??
        store?.nameEnglish ??
        store?.NameEnglish ??
        store?.storeName ??
        store?.StoreName ??
        store?.name ??
        store?.Name ??
        store?.storeCode ??
        store?.StoreCode;

  const activeBranches = branches.filter(isActive);
  const activeStores = stores.filter(isActive);

  const selectedBranch = activeBranches.find(
    (branch) =>
      String(getId(branch)) ===
      String(filters.branchId)
  );

  const selectedStore = activeStores.find(
    (store) =>
      String(getId(store)) ===
      String(filters.storeId)
  );

  const visibleStores = filters.branchId
    ? activeStores.filter((store) => {
        const branch = selectedBranch;

        if (!branch) return false;

        const storeBranchId =
          getStoreBranchId(store);

        const storeBranchCode =
          getStoreBranchCode(store);

        const branchId = getId(branch);
        const branchCode =
          getBranchCode(branch);

        return (
          (storeBranchId != null &&
            String(storeBranchId) ===
              String(branchId)) ||
          (storeBranchCode != null &&
            branchCode != null &&
            String(storeBranchCode) ===
              String(branchCode))
        );
      })
    : activeStores;

  const change = (field, value) => {
    if (field === "branchId") {
      onFiltersChange?.({
        branchId: value,
        storeId: "",
      });
    } else {
      onFiltersChange?.({
        [field]: value,
      });
    }

    setOpen(null);
  };

  const clear = () => {
    onFiltersChange?.({
      branchId: "",
      storeId: "",
      status: "",
      sessionNumber: "",
    });

    setOpen(null);
  };

  const selectedStatus =
    {
      active: t.active,
      completed: t.completed,
    }[filters.status] || t.allStatuses;

  const dropdown = (
    key,
    label,
    options,
    onSelect
  ) => (
    <div className={styles.dropdown}>
      <button
        type="button"
        className={styles.select}
        onClick={() =>
          setOpen(
            open === key ? null : key
          )
        }
      >
        <span>{label}</span>

        <Icon
          name="chevronDown"
          size={16}
        />
      </button>

      {open === key && (
        <div className={styles.menu}>
          {options.length ? (
            options.map((option) => (
              <button
                type="button"
                key={String(option.value)}
                className={styles.option}
                onClick={() =>
                  onSelect(option.value)
                }
              >
                {option.label}
              </button>
            ))
          ) : (
            <div className={styles.emptyOption}>
              {t.noOptions}
            </div>
          )}
        </div>
      )}
    </div>
  );

  return (
    <section
      className={styles.filtersCard}
      dir={direction}
    >
      <div className={styles.searchWrapper}>
        <Icon
          name="search"
          size={18}
        />

        <input
          type="search"
          className={styles.searchInput}
          value={filters.sessionNumber || ""}
          onChange={(e) =>
            change(
              "sessionNumber",
              e.target.value
            )
          }
          placeholder={
            t.searchPlaceholder
          }
        />
      </div>

      <div className={styles.filters}>
        {dropdown(
          "status",
          selectedStatus,
          [
            {
              value: "",
              label: t.allStatuses,
            },
            {
              value: "active",
              label: t.active,
            },
            {
              value: "completed",
              label: t.completed,
            },
          ],
          (value) =>
            change("status", value)
        )}

        {dropdown(
          "branch",
          selectedBranch
            ? getBranchName(
                selectedBranch
              )
            : t.allBranches,
          [
            {
              value: "",
              label: t.allBranches,
            },
            ...activeBranches.map(
              (branch) => ({
                value: getId(branch),
                label: getBranchName(
                  branch
                ),
              })
            ),
          ],
          (value) =>
            change("branchId", value)
        )}

        {dropdown(
          "store",
          selectedStore
            ? getStoreName(
                selectedStore
              )
            : t.allStores,
          [
            {
              value: "",
              label: t.allStores,
            },
            ...visibleStores.map(
              (store) => ({
                value: getId(store),
                label: getStoreName(
                  store
                ),
              })
            ),
          ],
          (value) =>
            change("storeId", value)
        )}

        <button
          type="button"
          className={styles.clearButton}
          onClick={clear}
        >
          <Icon
            name="refresh"
            size={17}
          />

          <span>
            {t.clearFilters}
          </span>
        </button>
      </div>
    </section>
  );
}

export default SessionFilters;