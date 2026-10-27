$ErrorActionPreference = "Stop"

$root = "D:\Projects\InventorySystem"
$frontend = Join-Path $root "frontend"
$jsxPath = Join-Path $frontend "src\modules\financial\screens\Home\FinancialHome.jsx"
$componentDir = Join-Path $frontend "src\modules\financial\components\Home"
$componentPath = Join-Path $componentDir "FinancialHeader.jsx"

if (!(Test-Path $jsxPath)) {
    throw "FinancialHome.jsx was not found."
}

if (Test-Path $componentPath) {
    throw "FinancialHeader.jsx already exists. No files were changed."
}

$jsx = [System.IO.File]::ReadAllText($jsxPath)

$headerPattern = '(?s)<header className=\{styles\.header\}>.*?</header>'
$errorPattern = '(?s)\s*\{error && \(\s*<div className=\{styles\.error\}>.*?</div>\s*\)\}'

if ([regex]::Matches($jsx, $headerPattern).Count -ne 1) {
    throw "Could not identify exactly one header block. No files were changed."
}

if ([regex]::Matches($jsx, $errorPattern).Count -ne 1) {
    throw "Could not identify exactly one error block. No files were changed."
}

if ($jsx -notmatch '(?m)^\s*import Icon from "../../../../components/UI/Icon/Icon";\s*$') {
    throw "The expected Icon import was not found. No files were changed."
}

$languageOld = @'
  const {
    translations,
    formatNumber,
  } = useLanguage();
'@

$languageNew = @'
  const {
    translations,
    formatNumber,
    isArabic,
  } = useLanguage();
'@

if (!$jsx.Contains($languageOld)) {
    throw "The expected useLanguage block was not found. No files were changed."
}

if ($jsx -match '(?m)^\s*import FinancialHeader from ') {
    throw "FinancialHeader import already exists. No files were changed."
}

$headerComponent = @'
import Icon from "../../../../components/UI/Icon/Icon";
import styles from "../../screens/Home/FinancialHome.module.css";

export default function FinancialHeader({ t, loading, onRefresh, error }) {
  return (
    <>
      <header className={styles.header}>
        <div>
          <span className={styles.eyebrow}>
            {t.eyebrow}
          </span>

          <h1>{t.homeTitle}</h1>

          <p>{t.homeDescription}</p>
        </div>

        <button
          type="button"
          className={styles.refreshButton}
          onClick={onRefresh}
          disabled={loading}
        >
          {loading ? t.loading : t.refresh}
        </button>
      </header>

      {error && (
        <div className={styles.error}>
          {error}
        </div>
      )}
    </>
  );
}
'@

$backupPath = "$jsxPath.bak-before-header-refactor-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
Copy-Item -LiteralPath $jsxPath -Destination $backupPath

try {
    $jsx = $jsx.Replace($languageOld, $languageNew)

    $jsx = [regex]::Replace(
        $jsx,
        '(?m)^import Icon from "../../../../components/UI/Icon/Icon";\s*$',
        'import Icon from "../../../../components/UI/Icon/Icon";' +
        [Environment]::NewLine +
        'import FinancialHeader from "../../components/Home/FinancialHeader";'
    )

    $jsx = [regex]::Replace(
        $jsx,
        $headerPattern,
        '<FinancialHeader t={t} loading={loading} onRefresh={loadData} error={error} />'
    )

    $jsx = [regex]::Replace($jsx, $errorPattern, '')

    New-Item -ItemType Directory -Path $componentDir -Force | Out-Null

    [System.IO.File]::WriteAllText(
        $componentPath,
        $headerComponent,
        [System.Text.UTF8Encoding]::new($false)
    )

    [System.IO.File]::WriteAllText(
        $jsxPath,
        $jsx,
        [System.Text.UTF8Encoding]::new($false)
    )

    Push-Location $frontend
    try {
        npm run build
        if ($LASTEXITCODE -ne 0) {
            throw "Frontend build failed."
        }
    }
    finally {
        Pop-Location
    }

    Write-Host ""
    Write-Host "SUCCESS: Header extracted and build passed." -ForegroundColor Green
    Write-Host "Backup: $backupPath" -ForegroundColor Cyan
}
catch {
    Copy-Item -LiteralPath $backupPath -Destination $jsxPath -Force

    if (Test-Path $componentPath) {
        Remove-Item -LiteralPath $componentPath -Force
    }

    Write-Host ""
    Write-Host "Changes rolled back because the operation failed." -ForegroundColor Yellow
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host "Backup: $backupPath" -ForegroundColor Cyan
}