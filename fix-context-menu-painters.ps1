# PowerShell script to fix property names in all context menu painters

$painters = @(
    "MaterialContextMenuPainter.cs",
    "MinimalContextMenuPainter.cs",
    "FlatContextMenuPainter.cs",
    "OfficeContextMenuPainter.cs"
)

$basePath = "c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\ContextMenus\Painters\"

foreach ($painter in $painters) {
    $filePath = Join-Path $basePath $painter
    
    if (Test-Path $filePath) {
        Write-Host "Fixing $painter..." -ForegroundColor Yellow
        
        $content = Get-Content $filePath -Raw
        
        # Fix SimpleItem properties
        $content = $content -replace 'item\.Disabled', '!item.IsEnabled'
        $content = $content -replace 'item\.Checked', 'item.IsChecked'
        $content = $content -replace 'item\.Shortcut', 'item.KeyCombination'
        
        # Fix Theme color properties
        $content = $content -replace 'theme\.BackgroundColor', 'theme.MenuBackColor'
        $content = $content -replace 'theme\.ForegroundColor', 'theme.MenuItemForeColor'
        $content = $content -replace 'theme\.HoverBackColor', 'theme.MenuItemHoverBackColor'
        $content = $content -replace 'theme\.SelectedBackColor', 'theme.MenuItemSelectedBackColor'
        $content = $content -replace 'theme\.BorderColor', 'theme.MenuBorderColor'
        $content = $content -replace 'theme\.PrimaryColor', 'theme.MenuItemSelectedBackColor'
        
        # Add hover/selected text colors where appropriate
        # This is a simple replacement - manual review may be needed for complex cases
        
        Set-Content $filePath -Value $content -NoNewline
        
        Write-Host "  ✓ Fixed $painter" -ForegroundColor Green
    } else {
        Write-Host "  ✗ File not found: $painter" -ForegroundColor Red
    }
}

Write-Host "`nAll painters fixed! Please review the changes." -ForegroundColor Cyan
