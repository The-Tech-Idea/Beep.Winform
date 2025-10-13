# PowerShell script to fix all painter files with correct BeepTheme properties

$paintersPath = "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Dates\Painters"

# Get all painter files
$painterFiles = Get-ChildItem -Path $paintersPath -Filter "*Painter.cs" -Recurse

Write-Host "Found $($painterFiles.Count) painter files to fix..." -ForegroundColor Cyan

foreach ($file in $painterFiles) {
    Write-Host "Processing: $($file.Name)" -ForegroundColor Yellow
    
    $content = Get-Content -Path $file.FullName -Raw
    $originalContent = $content
    
    # Fix TextColor -> ForeColor
    $content = $content -replace '(_theme\?\.)TextColor', '$1ForeColor'
    
    # Fix HoverBackColor -> Calendar-specific or hardcoded
    $content = $content -replace '(_theme\?\.)HoverBackColor', 'CalendarHoverBackColor'
    
    # Fix RegularFont -> new Font with FontName
    $content = $content -replace '(_theme\?\.)RegularFont\s*\?\?\s*new\s+Font\("([^"]+)",\s*([0-9.]+)f?\)', 'new Font($1FontName ?? "$2", $3f)'
    $content = $content -replace '(_theme\?\.)RegularFont', 'new Font($1FontName ?? "Segoe UI", 10f)'
    
    # Fix BoldFont -> new Font with FontName and FontStyle.Bold
    $content = $content -replace '(_theme\?\.)BoldFont\s*\?\?\s*new\s+Font\("([^"]+)",\s*([0-9.]+)f?,\s*FontStyle\.Bold\)', 'new Font($1FontName ?? "$2", $3f, FontStyle.Bold)'
    $content = $content -replace '(_theme\?\.)BoldFont', 'new Font($1FontName ?? "Segoe UI", 10f, FontStyle.Bold)'
    
    # Only save if content changed
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  ✓ Fixed: $($file.Name)" -ForegroundColor Green
    } else {
        Write-Host "  - No changes: $($file.Name)" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "Done! All painter files processed." -ForegroundColor Cyan
