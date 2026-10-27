$ErrorActionPreference = "Stop"

$keysPath = "src\InventorySystem.Application\Common\Localization\LocalizationKeys.cs"
$arPath = "src\InventorySystem.Application\Common\Localization\ArabicResources.cs"

if(-not (Test-Path $keysPath)){ throw "Missing: $keysPath" }
if(-not (Test-Path $arPath)){ throw "Missing: $arPath" }

$translations = [ordered]@{
Title="التحليل المالي"
Dashboard="لوحة التحليل المالي"
FinancialPeriod="الفترة المالية"
PeriodFrom="من تاريخ"
PeriodTo="إلى تاريخ"
GeneratedAt="تاريخ إنشاء التقرير"
Status="الحالة"
Scope="النطاق"
AllBranches="جميع الفروع"
SelectedBranch="الفرع المحدد"
Branch="الفرع"
BranchCode="رمز الفرع"
BranchName="اسم الفرع"
Sales="المبيعات"
Inventory="المخزون"
Expenses="المصاريف"
Purchases="المشتريات"
ProfitLoss="الأرباح والخسائر"
CustomersEmployees="الزبائن والموظفون"
Customers="الزبائن"
Employees="الموظفون"
BranchDebt="مديونية الفرع"
Cash="النقد"
Suppliers="الموردون"
Capital="رأس مال الفرع"
Transfers="المناقلات"
Settlements="التسديدات"
Male="رجالي"
Female="نسائي"
Total="الإجمالي"
Average="المتوسط"
Highest="أعلى فرع"
Lowest="أدنى فرع"
Value="القيمة"
Details="التفاصيل"
Debit="مدين"
Credit="دائن"
Balance="الرصيد"
Opening="الافتتاحي"
Closing="الختامي"
Change="التغير"
Profit="ربح"
Loss="خسارة"
BreakEven="تعادل"
Complete="مكتمل"
Incomplete="غير مكتمل"
Missing="ناقص"
Valid="صالح"
Invalid="غير صالح"
Draft="مسودة"
Uploading="جاري رفع الملفات"
Ready="جاهزة للاعتماد"
Approved="معتمدة"
ActiveBranches="الفروع الفعالة"
ExpectedFiles="الملفات المطلوبة"
ValidFiles="الملفات الصالحة"
MissingFiles="الملفات الناقصة"
CompletionPercentage="نسبة الاكتمال"
SalesReturns="مردودات المبيعات"
DiscountAllowed="الخصم المسموح"
BranchDiscountAllowed="خصم الفرع المسموح"
NetSales="صافي المبيعات"
OpeningInventory="المخزون الافتتاحي"
ClosingInventory="المخزون الختامي"
InventoryChange="تغير المخزون"
TotalExpenses="إجمالي المصاريف"
TotalPurchases="إجمالي المشتريات"
CustomerBalance="رصيد الزبائن"
EmployeeBalance="رصيد الموظفين"
MaleDebt="مديونية الرجالي"
FemaleDebt="مديونية النسائي"
TotalDebt="إجمالي المديونية"
TotalCash="إجمالي النقد"
CashboxesCount="عدد الصناديق"
SuppliersTotal="إجمالي الموردين"
CapitalValue="قيمة رأس المال"
MaleTransfer="مناقلات الرجالي"
FemaleTransfer="مناقلات النسائي"
TotalTransfer="إجمالي المناقلات"
PeriodDebit="مدين الفترة"
PeriodCredit="دائن الفترة"
PeriodBalance="رصيد الفترة"
IncludedSections="الأقسام المشمولة"
SectionSummary="ملخص القسم"
BranchDetails="تفاصيل الفروع"
NoData="لا توجد بيانات"
WithData="توجد بيانات"
WithoutData="بدون بيانات"
BranchesWithData="الفروع التي تحتوي بيانات"
BranchesWithoutData="الفروع بدون بيانات"
CompleteBranches="الفروع المكتملة"
IncompleteBranches="الفروع غير المكتملة"
ProfitableBranches="الفروع الرابحة"
LossBranches="الفروع الخاسرة"
BreakEvenBranches="فروع التعادل"
TotalProfit="إجمالي الأرباح"
TotalLoss="إجمالي الخسائر"
OverallResult="النتيجة الإجمالية"
MaleFiles="ملفات الرجالي"
FemaleFiles="ملفات النسائي"
Records="عدد السجلات"
Transactions="عدد الحركات"
Yes="نعم"
No="لا"
ResultType="نوع النتيجة"
ResultValue="قيمة النتيجة"
SignedValue="القيمة الصافية"
Assets="الموجودات"
Liabilities="المطلوبات"
AccountCode="رمز الحساب"
AccountName="اسم الحساب"
FileAvailable="الملف متوفر"
ExportExcel="تصدير Excel"
ExportPdf="تصدير PDF"
Print="طباعة"
InvalidBranchId="معرّف الفرع غير صالح."
BranchNotFound="الفرع المالي المحدد غير موجود."
UnsupportedSection="قسم التحليل المالي غير مدعوم."
UnsupportedSections="توجد أقسام تحليل مالي غير مدعومة."
}

# ============================================================
# ENSURE LocalizationKeys.FinancialAnalysis
# ============================================================

$keys = Get-Content $keysPath -Raw

$marker = "public static class FinancialAnalysis"
$start = $keys.IndexOf($marker)

if($start -lt 0){
    throw "FinancialAnalysis class not found in LocalizationKeys.cs"
}

$open = $keys.IndexOf("{",$start)

if($open -lt 0){
    throw "FinancialAnalysis opening brace not found."
}

$depth = 0
$close = -1

for($i=$open;$i -lt $keys.Length;$i++){
    if($keys[$i] -eq '{'){
        $depth++
    }
    elseif($keys[$i] -eq '}'){
        $depth--
        if($depth -eq 0){
            $close = $i
            break
        }
    }
}

if($close -lt 0){
    throw "FinancialAnalysis closing brace not found."
}

$classText = $keys.Substring($open,$close-$open+1)
$missingLines = New-Object System.Collections.Generic.List[string]

foreach($name in $translations.Keys){
    $pattern = "public\s+const\s+string\s+$([regex]::Escape($name))\s*="
    if(-not [regex]::IsMatch($classText,$pattern)){
        $missingLines.Add("        public const string $name = `"FinancialAnalysis.$name`";")
    }
}

if($missingLines.Count -gt 0){
    $block = "`r`n" + ($missingLines -join "`r`n") + "`r`n"
    $keys = $keys.Insert($open+1,$block)
    Set-Content -Encoding UTF8 $keysPath $keys
    Write-Host "Added missing keys: $($missingLines.Count)" -ForegroundColor Green
}
else{
    Write-Host "FinancialAnalysis keys already complete." -ForegroundColor Green
}

# ============================================================
# REBUILD ARABIC FinancialAnalysis RESOURCE ENTRIES
# ============================================================

$ar = Get-Content $arPath -Raw

# Remove any previous/incomplete FinancialAnalysis entries only.
$ar = [regex]::Replace(
    $ar,
    '(?m)^\s*\[LocalizationKeys\.FinancialAnalysis\.[A-Za-z0-9_]+\]\s*=\s*".*?",\s*\r?\n?',
    ''
)

$dictMarker = "public static readonly Dictionary<string, string> Values"
$dictStart = $ar.IndexOf($dictMarker)

if($dictStart -lt 0){
    throw "ArabicResources.Values dictionary not found."
}

$dictOpen = $ar.IndexOf("{",$dictStart)

if($dictOpen -lt 0){
    throw "ArabicResources.Values opening brace not found."
}

$depth = 0
$dictClose = -1

for($i=$dictOpen;$i -lt $ar.Length;$i++){
    if($ar[$i] -eq '{'){
        $depth++
    }
    elseif($ar[$i] -eq '}'){
        $depth--
        if($depth -eq 0){
            $dictClose = $i
            break
        }
    }
}

if($dictClose -lt 0){
    throw "ArabicResources.Values closing brace not found."
}

$resourceLines = New-Object System.Collections.Generic.List[string]
$resourceLines.Add("")
$resourceLines.Add("        // Financial Analysis")

foreach($name in $translations.Keys){
    $value = $translations[$name]
    $value = $value.Replace('\','\\')
    $value = $value.Replace('"','\"')

    $resourceLines.Add(
        "        [LocalizationKeys.FinancialAnalysis.$name] = `"$value`","
    )
}

$resourceLines.Add("")

$resourceBlock = $resourceLines -join "`r`n"

$ar = $ar.Insert($dictClose,$resourceBlock)

Set-Content -Encoding UTF8 $arPath $ar

# ============================================================
# VALIDATE
# ============================================================

$ar = Get-Content $arPath -Raw
$missing = New-Object System.Collections.Generic.List[string]

foreach($name in $translations.Keys){
    $needle = "[LocalizationKeys.FinancialAnalysis.$name]"
    if(-not $ar.Contains($needle)){
        $missing.Add($name)
    }
}

if($missing.Count -gt 0){
    throw "Still missing: $($missing -join ', ')"
}

$names =
    Select-String `
        -Path $arPath `
        -Pattern 'LocalizationKeys\.FinancialAnalysis\.([A-Za-z0-9_]+)' `
        -AllMatches |
    ForEach-Object {
        foreach($match in $_.Matches){
            $match.Groups[1].Value
        }
    }

$duplicates =
    $names |
    Group-Object |
    Where-Object { $_.Count -gt 1 }

if($duplicates){
    $duplicates | Format-Table Name,Count
    throw "Duplicate FinancialAnalysis translations detected."
}

Write-Host ""
Write-Host "Arabic FinancialAnalysis entries repaired successfully." -ForegroundColor Green
Write-Host "Count: $($translations.Count)" -ForegroundColor Green
Write-Host ""

Select-String `
    -Path $arPath `
    -Pattern 'FinancialAnalysis.Dashboard|FinancialAnalysis.FinancialPeriod|FinancialAnalysis.ProfitLoss|FinancialAnalysis.Highest|FinancialAnalysis.Lowest|FinancialAnalysis.OverallResult'

Write-Host "`n===== BUILD =====" -ForegroundColor Cyan
dotnet build
