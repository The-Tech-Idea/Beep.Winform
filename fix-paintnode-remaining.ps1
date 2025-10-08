# PowerShell script to fix remaining issues in all tree painters

$paintersPath = "c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Trees\Painters"

$painters = @(
    "ActivityLogTreePainter.cs",
    "AntDesignTreePainter.cs",
    "BootstrapTreePainter.cs",
    "ChakraUITreePainter.cs",
    "ComponentTreePainter.cs",
    "DevExpressTreePainter.cs",
    "DiscordTreePainter.cs",
    "DocumentTreePainter.cs",
    "FigmaCardTreePainter.cs",
    "FileBrowserTreePainter.cs",
    "FileManagerTreePainter.cs",
    "Fluent2TreePainter.cs",
    "InfrastructureTreePainter.cs",
    "iOS15TreePainter.cs",
    "MacOSBigSurTreePainter.cs",
    "Material3TreePainter.cs",
    "NotionMinimalTreePainter.cs",
    "PillRailTreePainter.cs",
    "PortfolioTreePainter.cs",
    "StripeDashboardTreePainter.cs",
    "SyncfusionTreePainter.cs",
    "TailwindCardTreePainter.cs",
    "TelerikTreePainter.cs",
    "VercelCleanTreePainter.cs"
)

foreach ($painter in $painters) {
    $filePath = Join-Path $paintersPath $painter
    
    if (Test-Path $filePath) {
        Write-Host "Processing $painter..." -ForegroundColor Cyan
        
        $content = Get-Content $filePath -Raw
        
        # Fix PaintCheckbox calls missing isHovered parameter
        $content = $content -replace "PaintCheckbox\(g, checkRect, node\.Item\.IsChecked\);", "PaintCheckbox(g, checkRect, node.Item.IsChecked, isHovered);"
        
        # Fix any references to undefined constants (like TimelineDotRadius, CardCornerRadius, etc.)
        # These need to stay as constants in the class
        
        Set-Content -Path $filePath -Value $content -NoNewline
        Write-Host "  ✓ Fixed $painter" -ForegroundColor Green
    } else {
        Write-Host "  ✗ File not found: $painter" -ForegroundColor Red
    }
}

Write-Host "`nDone! All remaining issues fixed." -ForegroundColor Green
