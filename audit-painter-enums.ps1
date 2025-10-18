# Audit all Painter/Handler enum usage
$painterPath = "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Dates\Painters"
$handlerPath = "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Dates\HitHandlers"

# Get all painter files
$painters = Get-ChildItem "$painterPath\*DateTimePickerPainter.cs" | Where-Object { $_.Name -ne "IDateTimePickerPainter.cs" }

$report = @()

foreach ($painter in $painters) {
    $baseName = $painter.Name -replace "Painter\.cs$", "Handler.cs"
    $handlerFile = Join-Path $handlerPath $baseName
    
    if (Test-Path $handlerFile) {
        Write-Host "`n=== $($painter.BaseName) ===" -ForegroundColor Cyan
        
        # Extract enum usage from painter (IsAreaHovered/IsAreaPressed calls)
        $painterContent = Get-Content $painter.FullName -Raw
        $painterEnums = [regex]::Matches($painterContent, 'IsArea(?:Hovered|Pressed)\(DateTimePickerHitArea\.(\w+)\)') | 
            ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
        
        # Extract enum usage from handler (result.HitArea = assignments)
        $handlerContent = Get-Content $handlerFile -Raw
        $handlerEnums = [regex]::Matches($handlerContent, 'result\.HitArea\s*=\s*DateTimePickerHitArea\.(\w+)') | 
            ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
        
        Write-Host "Painter checks: $($painterEnums -join ', ')" -ForegroundColor Yellow
        Write-Host "Handler registers: $($handlerEnums -join ', ')" -ForegroundColor Green
        
        # Find missing
        $missing = $handlerEnums | Where-Object { $_ -notin $painterEnums }
        if ($missing) {
            Write-Host "MISSING in painter: $($missing -join ', ')" -ForegroundColor Red
        } else {
            Write-Host "OK - All enums checked" -ForegroundColor Green
        }
        
        $report += [PSCustomObject]@{
            Painter = $painter.BaseName
            Handler = $baseName -replace "\.cs$", ""
            PainterChecks = ($painterEnums -join ', ')
            HandlerRegisters = ($handlerEnums -join ', ')
            Missing = ($missing -join ', ')
            Status = if ($missing) { "INCOMPLETE" } else { "COMPLETE" }
        }
    }
}

Write-Host "`n`n=== SUMMARY ===" -ForegroundColor Cyan
$report | Format-Table -AutoSize
$report | Export-Csv "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\PAINTER_AUDIT.csv" -NoTypeInformation
Write-Host "`nReport saved to PAINTER_AUDIT.csv" -ForegroundColor Green
