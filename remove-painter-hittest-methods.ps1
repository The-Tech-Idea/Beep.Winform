# Remove HitTest methods from all DateTimePicker painter files
# Painters should only handle visual rendering, not hit testing

$painterPath = "c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Dates\Painters"

# Get all painter files
$painterFiles = Get-ChildItem -Path $painterPath -Filter "*DateTimePickerPainter.cs" -Exclude "IDateTimePickerPainter.cs"

Write-Host "Removing HitTest methods from $($painterFiles.Count) painter files..." -ForegroundColor Green

foreach ($file in $painterFiles) {
    Write-Host "Processing: $($file.Name)" -ForegroundColor Yellow
    
    $content = Get-Content $file.FullName -Raw
    $originalLength = $content.Length
    
    # Remove HitTest method with various signature patterns
    # Pattern 1: public DateTimePickerHitTestResult HitTest(...) { ... }
    $content = $content -replace '(?s)\s*public\s+DateTimePickerHitTestResult\s+HitTest\s*\([^)]*\)\s*\{[^{}]*(?:\{[^{}]*\}[^{}]*)*\}', ''
    
    # Pattern 2: Empty HitTest methods with simple return
    $content = $content -replace '(?s)\s*public\s+DateTimePickerHitTestResult\s+HitTest\s*\([^)]*\)\s*\{\s*return\s+new\s+DateTimePickerHitTestResult\(\)\s*;\s*\}', ''
    
    # Clean up extra whitespace
    $content = $content -replace '\n\s*\n\s*\n', "`n`n"
    
    if ($content.Length -ne $originalLength) {
        Set-Content $file.FullName $content -NoNewline
        Write-Host "  ✅ Removed HitTest method from $($file.Name)" -ForegroundColor Green
    } else {
        Write-Host "  ℹ️  No HitTest method found in $($file.Name)" -ForegroundColor Cyan
    }
}

Write-Host "`nCompleted removing HitTest methods from all painter files!" -ForegroundColor Green
Write-Host "All hit testing is now handled by corresponding hit handler classes." -ForegroundColor Green