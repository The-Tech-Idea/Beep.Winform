# PowerShell script to properly fix CalendarHoverBackColor references

$paintersPath = "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Dates\Painters"

# Get all painter files
$painterFiles = Get-ChildItem -Path $paintersPath -Filter "*Painter.cs" -Recurse

Write-Host "Found $($painterFiles.Count) painter files to fix..." -ForegroundColor Cyan

foreach ($file in $painterFiles) {
    Write-Host "Processing: $($file.Name)" -ForegroundColor Yellow
    
    $content = Get-Content -Path $file.FullName -Raw
    $originalContent = $content
    
    # Fix CalendarHoverBackColor that's missing _theme?. prefix
    # Look for patterns where CalendarHoverBackColor appears without _theme?.
    $content = $content -replace '(?<![_\w])CalendarHoverBackColor\s*\?\?', '_theme?.CalendarHoverBackColor ??'
    
    # Also handle cases where it might be standalone
    $content = $content -replace 'var\s+(\w+)\s*=\s*CalendarHoverBackColor\s*\?\?', 'var $1 = _theme?.CalendarHoverBackColor ??'
    
    # Only save if content changed
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  Fixed: $($file.Name)" -ForegroundColor Green
    } else {
        Write-Host "  No changes: $($file.Name)" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "Done! All painter files processed." -ForegroundColor Cyan
