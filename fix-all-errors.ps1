# Fix all compilation errors in painters and related files

$rootPath = "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls"

Write-Host "Starting comprehensive error fixes..." -ForegroundColor Green

# 1. Fix double _theme references (_theme?._theme? -> _theme?)
Write-Host "`n1. Fixing double _theme references..." -ForegroundColor Yellow
$painterFiles = Get-ChildItem -Path "$rootPath\Dates\Painters\*.cs" -File
foreach ($file in $painterFiles) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    # Fix _theme?._theme? -> _theme?
    $content = $content -replace '_theme\?\._theme\?\.', '_theme?.'
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  Fixed: $($file.Name)" -ForegroundColor Green
    }
}

# 2. Fix DefaultBeepTheme.cs stray _owner reference
Write-Host "`n2. Fixing DefaultBeepTheme.cs..." -ForegroundColor Yellow
$themeFile = "$rootPath\Themes\DefaultBeepTheme.cs"
$content = Get-Content $themeFile -Raw
$originalContent = $content

# Fix: _owner._currentTheme.CalendarHoverBackColor= -> CalendarHoverBackColor =
$content = $content -replace '\s*_owner\._currentTheme\.CalendarHoverBackColor\s*=', '                CalendarHoverBackColor ='

if ($content -ne $originalContent) {
    Set-Content -Path $themeFile -Value $content -NoNewline
    Write-Host "  Fixed: DefaultBeepTheme.cs" -ForegroundColor Green
}

Write-Host "`nAll fixes applied!" -ForegroundColor Green
Write-Host "`nNote: Some errors require additional investigation:" -ForegroundColor Yellow
Write-Host "  - DateTimePickerProperties missing properties (Mode, ShowToday, TimeIntervalMinutes, etc.)" -ForegroundColor Cyan
Write-Host "  - IBeepTheme property mismatches (ForegroundColor, ButtonBackgroundColor)" -ForegroundColor Cyan
Write-Host "  - BeepDateTimePicker missing SelectedDates property" -ForegroundColor Cyan
Write-Host "  - DatePickerFormat missing LongDate enum value" -ForegroundColor Cyan
Write-Host "  - Type conversion issues (IBeepTheme to BeepTheme)" -ForegroundColor Cyan
