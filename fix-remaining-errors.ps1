# Comprehensive fix for all remaining compilation errors

$rootPath = "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Dates"

Write-Host "Fixing remaining compilation errors..." -ForegroundColor Green

# 1. Fix ForegroundColor -> ForeColor in BeepDateTimePicker.Drawing.cs
Write-Host "`n1. Fixing property names in BeepDateTimePicker.Drawing.cs..." -ForegroundColor Yellow
$drawingFile = "$rootPath\BeepDateTimePicker.Drawing.cs"
$content = Get-Content $drawingFile -Raw

# Fix ForegroundColor -> ForeColor
$content = $content -replace '\.ForegroundColor\s', '.ForeColor '

# Fix ButtonBackgroundColor -> ButtonBackColor  
$content = $content -replace '\.ButtonBackgroundColor\s', '.ButtonBackColor '

Set-Content -Path $drawingFile -Value $content -NoNewline
Write-Host "  Fixed: BeepDateTimePicker.Drawing.cs" -ForegroundColor Green

Write-Host "`nAll automated fixes applied!" -ForegroundColor Green
Write-Host "`nRemaining issues require manual intervention:" -ForegroundColor Yellow
Write-Host "  - BeepDateTimePicker.Core.cs: DateTimePickerProperties missing properties" -ForegroundColor Cyan
Write-Host "    * Mode, ShowQuickButtons, AllowClear, TimeIntervalMinutes" -ForegroundColor Cyan
Write-Host "    * MinTime, MaxTime, ShowToday, ShowTomorrow, ShowYesterday" -ForegroundColor Cyan
Write-Host "    * ShowThisWeek, ShowThisMonth" -ForegroundColor Cyan
Write-Host "  - DatePickerFormat missing LongDate enum value" -ForegroundColor Cyan
Write-Host "  - Type conversions: IBeepTheme to BeepTheme" -ForegroundColor Cyan
Write-Host "  - BeepDateTimePicker missing SelectedDates property" -ForegroundColor Cyan
Write-Host "  - WeekNumbers type mismatch (bool to DatePickerWeekNumbers enum)" -ForegroundColor Cyan
