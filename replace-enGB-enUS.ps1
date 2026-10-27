$ErrorActionPreference = "Stop"

$root = "D:\Projects\InventorySystem"
$oldText = "en-US"
$newText = "en-US"
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$backupRoot = Join-Path $root "_backup-enGB-to-enUS-$timestamp"

$extensions = @(
    ".js", ".jsx", ".ts", ".tsx",
    ".json", ".jsonc", ".html", ".htm",
    ".css", ".scss", ".sass", ".less",
    ".md", ".mdx", ".txt", ".xml", ".resx",
    ".cs", ".cshtml", ".razor", ".csproj",
    ".sln", ".props", ".targets", ".config",
    ".yml", ".yaml", ".toml", ".ini",
    ".sql", ".ps1", ".sh", ".bat", ".cmd",
    ".svg", ".vue", ".svelte", ".properties"
)

$fileNames = @(
    ".env", ".env.local", ".env.development",
    ".env.production", ".gitignore", ".editorconfig"
)

if (!(Test-Path -LiteralPath $root)) {
    throw "Project directory not found."
}

$files = @(
    Get-ChildItem -LiteralPath $root -Recurse -File |
        Where-Object {
            $_.FullName -notmatch '[\\/](node_modules|\.git|dist|bin|obj|coverage|\.next)[\\/]'
        } |
        Where-Object {
            $extensions -contains $_.Extension.ToLowerInvariant() -or
            $fileNames -contains $_.Name.ToLowerInvariant()
        }
)

$changedFiles = 0
$totalReplacements = 0
$results = @()

foreach ($file in $files) {
    $content = [System.IO.File]::ReadAllText($file.FullName)

    $matches = [regex]::Matches(
        $content,
        [regex]::Escape($oldText)
    ).Count

    if ($matches -eq 0) {
        continue
    }

    $relativePath = $file.FullName.Substring($root.Length).TrimStart('\', '/')
    $backupPath = Join-Path $backupRoot $relativePath
    $backupDirectory = Split-Path -Parent $backupPath

    New-Item -ItemType Directory -Path $backupDirectory -Force |
        Out-Null

    Copy-Item -LiteralPath $file.FullName -Destination $backupPath

    $bytes = [System.IO.File]::ReadAllBytes($file.FullName)

    if (
        $bytes.Length -ge 3 -and
        $bytes[0] -eq 0xEF -and
        $bytes[1] -eq 0xBB -and
        $bytes[2] -eq 0xBF
    ) {
        $encoding = [System.Text.UTF8Encoding]::new($true)
    }
    elseif (
        $bytes.Length -ge 2 -and
        $bytes[0] -eq 0xFF -and
        $bytes[1] -eq 0xFE
    ) {
        $encoding = [System.Text.Encoding]::Unicode
    }
    elseif (
        $bytes.Length -ge 2 -and
        $bytes[0] -eq 0xFE -and
        $bytes[1] -eq 0xFF
    ) {
        $encoding = [System.Text.Encoding]::BigEndianUnicode
    }
    else {
        $encoding = [System.Text.UTF8Encoding]::new($false)
    }

    $updatedContent = $content.Replace($oldText, $newText)

    [System.IO.File]::WriteAllText(
        $file.FullName,
        $updatedContent,
        $encoding
    )

    $changedFiles++
    $totalReplacements += $matches

    $results += [PSCustomObject]@{
        File = $relativePath
        Replacements = $matches
    }
}

Write-Host ""
Write-Host "Replacement completed." -ForegroundColor Green
Write-Host "Files changed: $changedFiles"
Write-Host "Total replacements: $totalReplacements"

if ($changedFiles -gt 0) {
    Write-Host "Backup directory: $backupRoot" -ForegroundColor Cyan
    $results | Format-Table -AutoSize
}
else {
    Write-Host "No exact en-US occurrences found in the scanned files." -ForegroundColor Yellow
}