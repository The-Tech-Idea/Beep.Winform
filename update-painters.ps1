# Script to add DrawMenuItems call to all sidebar painters
# This adds the menu item rendering code to the end of each Draw() method

$paintersPath = "c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\SideBar\Painters"

$paintersToUpdate = @(
    "ChakraUISideMenuPainter.cs",
    "DarkGlowSideMenuPainter.cs",
    "DiscordStyleSideMenuPainter.cs",
    "Fluent2SideMenuPainter.cs",
    "GradientModernSideMenuPainter.cs",
    "iOS15SideMenuPainter.cs",
    "MacOSBigSurSideMenuPainter.cs",
    "MaterialYouSideMenuPainter.cs",
    "MinimalSideMenuPainter.cs",
    "NotionMinimalSideMenuPainter.cs",
    "StripeDashboardSideMenuPainter.cs",
    "TailwindCardSideMenuPainter.cs",
    "VercelCleanSideMenuPainter.cs",
    "Windows11MicaSideMenuPainter.cs"
)

$menuItemsCode = @"

            // Draw all menu items with icons and text
            int? hoveredIndex = null;
            SimpleItem selectedItem = null;
            if (menu is BeepSideBarAdapter adapter)
            {
                hoveredIndex = adapter.HoveredItemIndex >= 0 ? adapter.HoveredItemIndex : (int?)null;
                selectedItem = adapter.SelectedItem;
            }
            DrawMenuItems(menu, g, bounds, hoveredIndex, selectedItem);
"@

foreach ($painter in $paintersToUpdate) {
    $filePath = Join-Path $paintersPath $painter
    
    if (Test-Path $filePath) {
        Write-Host "Processing $painter..."
        
        $content = Get-Content $filePath -Raw
        
        # Add Models using if not present
        if ($content -notmatch "using TheTechIdea\.Beep\.Winform\.Controls\.Models;") {
            $content = $content -replace "(using System\.Drawing\.Drawing2D;)", "`$1`nusing TheTechIdea.Beep.Winform.Controls.Models;"
        }
        
        # Find the last closing brace before the end of Draw() method
        # Look for pattern: method content followed by }
        $pattern = "(public override void Draw\(BeepSideMenu menu, Graphics g, Rectangle bounds\)[\s\S]*?)(\r?\n\s{8}\})"
        
        if ($content -match $pattern) {
            # Check if DrawMenuItems is already there
            if ($content -notmatch "DrawMenuItems\(menu, g, bounds") {
                $replacement = "`$1$menuItemsCode`$2"
                $content = $content -replace $pattern, $replacement
                
                Set-Content $filePath -Value $content -NoNewline
                Write-Host "  ✓ Updated $painter" -ForegroundColor Green
            } else {
                Write-Host "  - Skipped $painter (already has DrawMenuItems)" -ForegroundColor Yellow
            }
        } else {
            Write-Host "  ✗ Could not find Draw() method in $painter" -ForegroundColor Red
        }
    } else {
        Write-Host "  ✗ File not found: $painter" -ForegroundColor Red
    }
}

Write-Host "`nDone! Updated painters." -ForegroundColor Cyan
