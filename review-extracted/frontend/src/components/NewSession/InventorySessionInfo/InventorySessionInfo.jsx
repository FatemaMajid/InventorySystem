import { useLanguage } from '../../../context/LanguageContext';
import Icon from '../../UI/Icon/Icon';
import styles from './InventorySessionInfo.module.css';

function InventorySessionInfo({
  inventoryType,
  onInventoryTypeChange,
  branchId,
  onBranchChange,
  storeId,
  onStoreChange,
  branches = [],
  stores = [],
  loading = false,
}) {
  const { translations, direction } = useLanguage();
  const t = translations.inventory;

  const isActive = (item) => item?.isActive === true || item?.IsActive === true;

  const getId = (item) => item?.id ?? item?.Id ?? item?.branchId ?? item?.BranchId;

  const getBranchCode = (item) => item?.branchCode ?? item?.BranchCode;

  const getStoreBranchId = (item) => item?.branchId ?? item?.BranchId ?? item?.branch?.id ?? item?.Branch?.id;

  const getStoreBranchCode = (item) => item?.branchCode ?? item?.BranchCode ?? item?.branch?.code ?? item?.Branch?.code;

  const getBranchName = (branch) =>
    direction === 'rtl'
      ? branch?.branchNameArabic ?? branch?.BranchNameArabic ?? branch?.nameArabic ?? branch?.NameArabic ?? branch?.branchName ?? branch?.BranchName ?? branch?.name ?? branch?.Name ?? getBranchCode(branch)
      : branch?.branchNameEnglish ?? branch?.BranchNameEnglish ?? branch?.nameEnglish ?? branch?.NameEnglish ?? branch?.branchName ?? branch?.BranchName ?? branch?.name ?? branch?.Name ?? getBranchCode(branch);

  const getStoreName = (store) =>
    direction === 'rtl'
      ? store?.storeNameArabic ?? store?.StoreNameArabic ?? store?.nameArabic ?? store?.NameArabic ?? store?.storeName ?? store?.StoreName ?? store?.name ?? store?.Name ?? store?.storeCode ?? store?.StoreCode
      : store?.storeNameEnglish ?? store?.StoreNameEnglish ?? store?.nameEnglish ?? store?.NameEnglish ?? store?.storeName ?? store?.StoreName ?? store?.name ?? store?.Name ?? store?.storeCode ?? store?.StoreCode;

  const activeBranches = branches
    .filter(isActive)
    .sort((a, b) =>
      String(getBranchCode(a) ?? '').localeCompare(
        String(getBranchCode(b) ?? ''),
        undefined,
        {
          numeric: true,
          sensitivity: 'base',
        }
      )
    );

  const activeStores = stores.filter(isActive);

  const selectedBranch = activeBranches.find((branch) => String(getId(branch)) === String(branchId));

  const visibleStores = selectedBranch
    ? activeStores.filter((store) => {
        const storeBranchId = getStoreBranchId(store);
        const storeBranchCode = getStoreBranchCode(store);
        const selectedBranchId = getId(selectedBranch);
        const selectedBranchCode = getBranchCode(selectedBranch);

        return (
          (storeBranchId != null && String(storeBranchId) === String(selectedBranchId)) ||
          (storeBranchCode != null && selectedBranchCode != null && String(storeBranchCode) === String(selectedBranchCode))
        );
      })
    : [];

  return (
    <section className={styles.card} dir={direction}>
      <div className={styles.header}>
        <div>
          <h2>{t.sessionInformation}</h2>
          <p>{t.sessionInformationDescription}</p>
        </div>
      </div>

      <div className={styles.form}>
        <div className={styles.field}>
          <label htmlFor="inventory-type">{t.inventoryType}</label>
          <div className={styles.selectWrapper}>
            <select id="inventory-type" value={inventoryType} onChange={(event) => onInventoryTypeChange(Number(event.target.value))}>
              <option value={0}>{t.selectInventoryType}</option>
              <option value={1}>{t.inventoryTypes.semiAnnual}</option>
              <option value={2}>{t.inventoryTypes.annual}</option>
            </select>
            <Icon name="chevronDown" size={16} className={styles.chevron} />
          </div>
        </div>

        <div className={styles.field}>
          <label htmlFor="branch">{t.selectBranch}</label>
          <div className={styles.selectWrapper}>
            <select id="branch" value={branchId} onChange={(event) => onBranchChange(event.target.value)} disabled={loading}>
              <option value="">{t.selectBranch}</option>
              {activeBranches.map((branch) => (
                <option key={String(getId(branch))} value={getId(branch)}>
                  {getBranchName(branch)}
                </option>
              ))}
            </select>
            <Icon name="chevronDown" size={16} className={styles.chevron} />
          </div>
        </div>

        <div className={styles.field}>
          <label htmlFor="store">{t.selectStore}</label>
          <div className={styles.selectWrapper}>
            <select id="store" value={storeId} onChange={(event) => onStoreChange(event.target.value)} disabled={loading || !branchId}>
              <option value="">{t.selectStore}</option>
              {visibleStores.map((store) => (
                <option key={String(getId(store))} value={getId(store)}>
                  {getStoreName(store)}
                </option>
              ))}
            </select>
            <Icon name="chevronDown" size={16} className={styles.chevron} />
          </div>
        </div>
      </div>
    </section>
  );
}

export default InventorySessionInfo;