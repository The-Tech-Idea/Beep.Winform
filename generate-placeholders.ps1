# Generate Placeholder SVG Icons
# Creates simple text-based SVG placeholders for missing logos

param(
    [string]$OutputDir = "TheTechIdea.Beep.Winform.Controls\GFX\Icons"
)

# List of icons that still need placeholders
$missingIcons = @(
    "H2Database",
    "Wrike",
    "SugarCRM",
    "ActiveCampaign",
    "ConstantContact",
    "Klaviyo",
    "ConvertKit",
    "MailerLite"
)

Write-Host "`n============================================" -ForegroundColor Cyan
Write-Host "  Placeholder SVG Generator" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Generating placeholders for $($missingIcons.Count) missing icons`n" -ForegroundColor White

$createdCount = 0

foreach ($icon in $missingIcons) {
    $filename = "$($icon.ToLower()).svg"
    $outputPath = Join-Path $OutputDir $filename
    
    # Skip if file already exists
    if (Test-Path $outputPath) {
        Write-Host "SKIP: $icon (already exists)" -ForegroundColor Yellow
        continue
    }
    
    # Create a simple placeholder SVG with the icon name
    $svgContent = @"
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 200 200" width="200" height="200">
  <rect width="200" height="200" fill="#f0f0f0" rx="10"/>
  <rect x="10" y="10" width="180" height="180" fill="#ffffff" stroke="#cccccc" stroke-width="2" rx="8"/>
  <text x="100" y="95" font-family="Arial, sans-serif" font-size="16" font-weight="bold" fill="#333333" text-anchor="middle">$icon</text>
  <text x="100" y="115" font-family="Arial, sans-serif" font-size="12" fill="#666666" text-anchor="middle">Icon Placeholder</text>
  <circle cx="100" cy="140" r="20" fill="none" stroke="#999999" stroke-width="2"/>
  <path d="M 100 130 L 100 145 M 93 140 L 107 140" stroke="#999999" stroke-width="2" stroke-linecap="round"/>
</svg>
"@
    
    # Write the SVG file
    $svgContent | Out-File -FilePath $outputPath -Encoding UTF8 -NoNewline
    
    Write-Host "CREATE: $filename" -ForegroundColor Green
    $createdCount++
}

Write-Host "`n============================================" -ForegroundColor Cyan
Write-Host "  Generation Complete!" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Created: $createdCount placeholder icons" -ForegroundColor Green
Write-Host "Output:  $OutputDir`n" -ForegroundColor White

Write-Host "Note: These are temporary placeholders. You can replace them later" -ForegroundColor Yellow
Write-Host "      by manually downloading the actual logos from company websites." -ForegroundColor Yellow
