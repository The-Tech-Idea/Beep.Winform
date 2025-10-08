# PowerShell script to fix NodeInfo null checks (structs can't be null)

$paintersPath = "c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Trees\Painters"

$painters = @(
    "BaseTreePainter.cs",
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
    "StandardTreePainter.cs",
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
        
        # Fix null check for NodeInfo struct (can't be null, check Item instead)
        $content = $content -replace "if \(g == null \|\| node == null\) return;", "if (g == null || node.Item == null) return;"
        
        Set-Content -Path $filePath -Value $content -NoNewline
        Write-Host "  ✓ Fixed $painter" -ForegroundColor Green
    } else {
        Write-Host "  ✗ File not found: $painter" -ForegroundColor Red
    }
}

Write-Host "`nDone! All NodeInfo null checks fixed." -ForegroundColor Green
