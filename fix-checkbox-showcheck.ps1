# Fix all painters to check ShowCheckBox before calling PaintCheckbox
# Simple approach: wrap the content inside if (node.CheckRectContent != Rectangle.Empty) with ShowCheckBox check

$painterFiles = Get-ChildItem -Path "TheTechIdea.Beep.Winform.Controls\Trees\Painters" -Filter "*TreePainter.cs" -Recurse | 
    Where-Object { $_.Name -ne "BaseTreePainter.cs" -and $_.Name -ne "ITreePainter.cs" }

$fixCount = 0

foreach ($file in $painterFiles) {
    $content = Get-Content $file.FullName -Raw
    
    # Check if already fixed
    if ($content -match 'if\s*\(\s*_owner\.ShowCheckBox\s*\).*?PaintCheckbox') {
        Write-Host "Skipped (already fixed): $($file.Name)" -ForegroundColor Yellow
        continue
    }
    
    # Pattern: Find the checkbox block and wrap its content
    $pattern = '(if\s*\(\s*node\.CheckRectContent\s*!=\s*Rectangle\.Empty\s*\)\r?\n\s*\{\r?\n)(\s+)(var\s+checkRect\s*=\s*node\.CheckRectContent;\r?\n\s+PaintCheckbox\(g,\s*checkRect,\s*node\.Item\.IsChecked,\s*isHovered\);)(\r?\n\s*\})'
    
    if ($content -match $pattern) {
        $replacement = {
            param($match)
            $intro = $match.Groups[1].Value
            $indent = $match.Groups[2].Value
            $body = $match.Groups[3].Value
            $closing = $match.Groups[4].Value
            
            # Add ShowCheckBox check
            "$intro$indent    if (_owner.ShowCheckBox)`r`n$indent    {`r`n$indent    $body`r`n$indent    }$closing"
        }
        
        $newContent = $content -replace $pattern, $replacement
        $newContent | Set-Content -Path $file.FullName -NoNewline
        Write-Host "Fixed: $($file.Name)" -ForegroundColor Green
        $fixCount++
    }
    else {
        Write-Host "Pattern not found in: $($file.Name)" -ForegroundColor Magenta
    }
}

Write-Host "`nFixed $fixCount painter files." -ForegroundColor Cyan
