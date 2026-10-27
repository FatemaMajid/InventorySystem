$ErrorActionPreference = "Stop"

$root = "D:\Projects\InventorySystem"
$frontend = Join-Path $root "frontend"
$jsxPath = Join-Path $frontend "src\modules\financial\screens\Home\FinancialHome.jsx"
$componentDir = Join-Path $frontend "src\modules\financial\components\Home"
$componentPath = Join-Path $componentDir "FinancialHeader.jsx"

if (!(Test-Path $jsxPath)) {
    throw "FinancialHome.jsx not found."
}

if (Test-Path $componentPath) {
    throw "FinancialHeader.jsx already exists. No files changed."
}

$jsx = [System.IO.File]::ReadAllText($jsxPath)

$headerRegex = [regex]::new('(?s)<header className=\{styles\.header\}>.*?</header>')
$errorRegex = [regex]::new('(?s)\s*\{error && \(\s*<div className=\{styles\.error\}>.*?</div>\s*\)\}')
$iconImportRegex = [regex]::new('(?m)^import Icon from "../../../../components/UI/Icon/Icon";\s*$')

if ($headerRegex.Matches($jsx).Count -ne 1) {
    throw "Expected exactly one header block. No files changed."
}

if ($errorRegex.Matches($jsx).Count -ne 1) {
    throw "Expected exactly one error block. No files changed."
}

if ($iconImportRegex.Matches($jsx).Count -ne 1) {
    throw "Expected Icon import not found. No files changed."
}

if ($jsx -match 'import FinancialHeader from ') {
    throw "FinancialHeader import already exists. No files changed."
}

$componentCode = @'
import Icon from "../../../../components/UI/Icon/Icon";
import styles from "../../screens/Home/FinancialHome.module.css";

export default function FinancialHeader({ t, loading, onRefresh, error }) {
  return (
    <>
      <header className={styles.header}>
        <div>
          <span className={styles.eyebrow}>{t.eyebrow}</span>
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

$backupPath = "$jsxPath.bak-header-refactor-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
Copy-Item -LiteralPath $jsxPath -Destination $backupPath

try {
    $jsx = $headerRegex.Replace(
        $jsx,
        '<FinancialHeader t={t} loading={loading} onRefresh={loadData} error={error} />',
        1
    )

    $jsx = $errorRegex.Replace($jsx, '', 1)

    $jsx = $iconImportRegex.Replace(
        $jsx,
        'import Icon from "../../../../components/UI/Icon/Icon";' +
        [Environment]::NewLine +
        'import FinancialHeader from "../../components/Home/FinancialHeader";',
        1
    )

    New-Item -ItemType Directory -Path $componentDir -Force | Out-Null

    [System.IO.File]::WriteAllText(
        $componentPath,
        $componentCode,
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

    Write-Host "SUCCESS: Financial header extracted. Build passed." -ForegroundColor Green
    Write-Host "Backup: $backupPath" -ForegroundColor Cyan
}
catch {
    Copy-Item -LiteralPath $backupPath -Destination $jsxPath -Force

    if (Test-Path $componentPath) {
        Remove-Item -LiteralPath $componentPath -Force
    }

    Write-Host "Changes rolled back." -ForegroundColor Yellow
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host "Backup: $backupPath" -ForegroundColor Cyan
}