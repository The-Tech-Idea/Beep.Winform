# PowerShell script to update all Form Painters with ShowCaptionBar, ShowCloseButton, and ShowMinMaxButtons support
# This script adds the visibility checks to CalculateLayoutAndHitAreas methods

$paintersPath = "c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Forms\ModernForm\Painters"

# List of painters that still need updating (excluding already updated ones)
$paintersToUpdate = @(
    "ChatBubbleFormPainter.cs",
    "CartoonFormPainter.cs",
    "BrutalistFormPainter.cs",
    "ArcLinuxFormPainter.cs",
    "CyberpunkFormPainter.cs",
    "DraculaFormPainter.cs",
    "GNOMEFormPainter.cs",
    "KDEFormPainter.cs",
    "NeonFormPainter.cs",
    "NeoMorphismFormPainter.cs",
    "GruvBoxFormPainter.cs",
    "HolographicFormPainter.cs",
    "GlassmorphismFormPainter.cs",
    "RetroFormPainter.cs",
    "UbuntuFormPainter.cs",
    "SolarizedFormPainter.cs",
    "TokyoFormPainter.cs",
    "OneDarkFormPainter.cs",
    "PaperFormPainter.cs",
    "NordFormPainter.cs",
    "NordicFormPainter.cs",
    "CustomFormPainter.cs"
)

Write-Host "Starting painter updates..." -ForegroundColor Cyan
Write-Host "Total painters to update: $($paintersToUpdate.Count)" -ForegroundColor Yellow

$updatedCount = 0
$errorCount = 0

foreach ($painterFile in $paintersToUpdate) {
    $filePath = Join-Path $paintersPath $painterFile
    
    if (-not (Test-Path $filePath)) {
        Write-Host "  [SKIP] File not found: $painterFile" -ForegroundColor DarkGray
        continue
    }
    
    Write-Host "`nProcessing: $painterFile" -ForegroundColor White
    
    try {
        $content = Get-Content $filePath -Raw
        $originalContent = $content
        
        # Pattern 1: Standard button layout (buttonX pattern)
        $pattern1 = '(public void CalculateLayoutAndHitAreas\(BeepiFormPro owner\)\s*\{\s*var layout = new PainterLayoutInfo\(\);)\s*(?!.*if \(!owner\.ShowCaptionBar\))'
        $replacement1 = @'
$1
            
            // If caption bar is hidden, skip button layout
            if (!owner.ShowCaptionBar)
            {
                layout.CaptionRect = Rectangle.Empty;
                layout.ContentRect = new Rectangle(0, 0, owner.ClientSize.Width, owner.ClientSize.Height);
                owner.CurrentLayout = layout;
                return;
            }
'@
        
        if ($content -match $pattern1) {
            $content = $content -replace $pattern1, $replacement1
            Write-Host "  [OK] Added ShowCaptionBar check" -ForegroundColor Green
        }
        
        # Pattern 2: Close button (buttonX or buttonWidth pattern)
        $closePattern = '(\s*)(layout\.CloseButtonRect = new Rectangle\(buttonX.*?\);)\s*(owner\._hits\.RegisterHitArea\("close".*?\);)\s*(buttonX -= .*?;)'
        $closeReplacement = @'
$1if (owner.ShowCloseButton)
$1{
$1    $2
$1    $3
$1    $4
$1}
'@
        
        if ($content -match $closePattern) {
            $content = $content -replace $closePattern, $closeReplacement
            Write-Host "  [OK] Wrapped close button with ShowCloseButton check" -ForegroundColor Green
        }
        
        # Pattern 3: Maximize button (must come before minimize in replacement)
        $maxPattern = '(\s*)(layout\.MaximizeButtonRect = new Rectangle\(button.*?\);)\s*(owner\._hits\.RegisterHitArea\("maximize".*?\);)\s*(buttonX -= .*?;)\s*\n\s*// Minimize button\s*\n\s*(layout\.MinimizeButtonRect = new Rectangle\(button.*?\);)\s*(owner\._hits\.RegisterHitArea\("minimize".*?\);)\s*(buttonX -= .*?;)'
        $maxReplacement = @'
$1// Maximize/Minimize buttons
$1if (owner.ShowMinMaxButtons)
$1{
$1    $2
$1    $3
$1    $4
$1    
$1    $5
$1    $6
$1    $7
$1}
'@
        
        if ($content -match $maxPattern) {
            $content = $content -replace $maxPattern, $maxReplacement
            Write-Host "  [OK] Wrapped min/max buttons with ShowMinMaxButtons check" -ForegroundColor Green
        }
        else {
            # Alternative pattern without comments
            $maxPattern2 = '(\s*)(layout\.MaximizeButtonRect = new Rectangle\(button.*?\);)\s*(owner\._hits\.RegisterHitArea\("maximize".*?\);)\s*(buttonX -= .*?;)\s*\n\s*(layout\.MinimizeButtonRect = new Rectangle\(button.*?\);)\s*(owner\._hits\.RegisterHitArea\("minimize".*?\);)\s*(buttonX -= .*?;)'
            $maxReplacement2 = @'
$1// Maximize/Minimize buttons
$1if (owner.ShowMinMaxButtons)
$1{
$1    $2
$1    $3
$1    $4
$1    
$1    $5
$1    $6
$1    $7
$1}
'@
            
            if ($content -match $maxPattern2) {
                $content = $content -replace $maxPattern2, $maxReplacement2
                Write-Host "  [OK] Wrapped min/max buttons with ShowMinMaxButtons check (alt pattern)" -ForegroundColor Green
            }
        }
        
        # Check if anything changed
        if ($content -ne $originalContent) {
            Set-Content -Path $filePath -Value $content -NoNewline
            Write-Host "  [SUCCESS] Updated $painterFile" -ForegroundColor Green
            $updatedCount++
        }
        else {
            Write-Host "  [INFO] No changes needed or patterns not matched in $painterFile" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "  [ERROR] Failed to process $painterFile : $_" -ForegroundColor Red
        $errorCount++
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Update Summary:" -ForegroundColor Cyan
Write-Host "  Total files processed: $($paintersToUpdate.Count)" -ForegroundColor White
Write-Host "  Successfully updated: $updatedCount" -ForegroundColor Green
Write-Host "  Errors: $errorCount" -ForegroundColor $(if ($errorCount -eq 0) { "Green" } else { "Red" })
Write-Host "========================================`n" -ForegroundColor Cyan

if ($updatedCount -gt 0) {
    Write-Host "Please rebuild the project to verify all changes compile successfully." -ForegroundColor Yellow
}
