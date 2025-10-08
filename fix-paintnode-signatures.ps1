# PowerShell script to fix PaintNode signatures in all tree painters
# Changes from: PaintNode(Graphics g, BeepTree owner, BeepTreeLayoutHelper layoutHelper, SimpleItem node)
# To: PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)

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
        
        # Replace the method signature
        $oldSignature = "public override void PaintNode(Graphics g, BeepTree owner, BeepTreeLayoutHelper layoutHelper, SimpleItem node)"
        $newSignature = "public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)"
        
        if ($content -match [regex]::Escape($oldSignature)) {
            $content = $content -replace [regex]::Escape($oldSignature), $newSignature
            
            # Fix the method body - first few lines that extract parameters
            # Pattern 1: if (g == null || node == null || owner == null || layoutHelper == null) return;
            $content = $content -replace "if \(g == null \|\| node == null \|\| owner == null \|\| layoutHelper == null\) return;", "if (g == null || node == null) return;"
            
            # Pattern 2: Extract nodeBounds from layoutHelper
            $content = $content -replace "var nodeBounds = layoutHelper\.TransformToViewport\(node\.ToggleRectContent\);[\r\n\s]*", ""
            
            # Pattern 3: Extract isHovered and isSelected
            $content = $content -replace "bool isHovered = owner\.LastHoveredItem == node;[\r\n\s]*", ""
            $content = $content -replace "bool isSelected = owner\.SelectedItem == node;[\r\n\s]*", ""
            
            # Pattern 4: Remove virtualization check (handled by base painter)
            $content = $content -replace "// Skip if not in viewport \(virtualization\)[\r\n\s]*if \(owner\.VirtualizeLayout && !layoutHelper\.IsNodeInViewport\(nodeBounds\)\)[\r\n\s]*return;[\r\n\s]*", ""
            
            # Pattern 5: Fix references to SimpleItem properties that are now in NodeInfo
            # node.ToggleRect -> node.ToggleRectContent (already in NodeInfo)
            # node.CheckboxRect -> node.CheckRectContent
            # node.IconRect -> node.IconRectContent
            # node.TextRect -> node.TextRectContent
            # node.DisplayText -> node.Item.Text
            # node.ImagePath -> node.Item.ImagePath
            # node.IsExpanded -> node.Item.IsExpanded
            # node.IsChecked -> node.Item.IsChecked
            # node.HasChildren -> (node.Item.Children != null && node.Item.Children.Count > 0)
            
            $content = $content -replace "node\.ToggleRect(?!Content)", "node.ToggleRectContent"
            $content = $content -replace "node\.CheckboxRect", "node.CheckRectContent"
            $content = $content -replace "node\.IconRect", "node.IconRectContent"
            $content = $content -replace "node\.TextRect", "node.TextRectContent"
            $content = $content -replace "node\.DisplayText", "node.Item.Text"
            $content = $content -replace "node\.ImagePath", "node.Item.ImagePath"
            $content = $content -replace "node\.IsExpanded", "node.Item.IsExpanded"
            $content = $content -replace "node\.IsChecked", "node.Item.IsChecked"
            $content = $content -replace "node\.HasChildren", "(node.Item.Children != null && node.Item.Children.Count > 0)"
            
            # Pattern 6: Fix TransformToViewport calls - they're already transformed!
            # Remove layoutHelper.TransformToViewport() calls since nodeBounds and sub-rects are already in viewport space
            $content = $content -replace "layoutHelper\.TransformToViewport\(([^\)]+)\)", '$1'
            
            # Pattern 7: Fix owner.Font references
            $content = $content -replace "owner\.Font", "_owner.TextFont"
            
            Set-Content -Path $filePath -Value $content -NoNewline
            Write-Host "  ✓ Fixed $painter" -ForegroundColor Green
        } else {
            Write-Host "  - $painter doesn't have old signature" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  ✗ File not found: $painter" -ForegroundColor Red
    }
}

Write-Host "`nDone! All painters have been updated." -ForegroundColor Green
