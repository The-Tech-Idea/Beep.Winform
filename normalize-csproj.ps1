param([switch]$DryRun)
$ErrorActionPreference = "Stop"
$root = $PSScriptRoot

function Set-Property([string]$content, [string]$propName, [string]$propValue) {
    $escaped = [regex]::Escape($propName)
    $pattern = "<${escaped}>.*?</${escaped}>"
    if ($content -match $pattern) {
        if ($Matches[0] -ne "<$propName>$propValue</$propName>") {
            return $content -replace $pattern, "<$propName>$propValue</$propName>"
        }
    } else {
        return $content -replace '(<PropertyGroup>)', "`$1`n`t`t<$propName>$propValue</$propName>"
    }
    return $content
}

$skip = @(
    "$root\TheTechIdea.Beep.Winform.Controls.Tests\TheTechIdea.Beep.Winform.Controls.Tests.csproj",
    "$root\Tools\BeepBlockWizardPreviewHarness\BeepBlockWizardPreviewHarness.csproj"
)

$csprojFiles = Get-ChildItem -Path $root -Recurse -Filter "*.csproj" -Depth 3 |
    Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' -and $_.FullName -notin $skip }

Write-Host "Found $($csprojFiles.Count) .csproj files" -ForegroundColor Cyan
$total = 0

foreach ($file in $csprojFiles) {
    $rel = $file.FullName.Substring($root.Length + 1)
    $txt = Get-Content -Path $file.FullName -Raw
    $orig = $txt

    # Apply company metadata
    $txt = Set-Property $txt "Authors" "The Tech Idea"
    $txt = Set-Property $txt "Company" "The Tech Idea"
    $txt = Set-Property $txt "Copyright" "2024"
    $txt = Set-Property $txt "PackageProjectUrl" "https://github.com/The-Tech-Idea/"
    $txt = Set-Property $txt "RepositoryUrl" "https://github.com/The-Tech-Idea/Beep.Winform"
    $txt = Set-Property $txt "PackageLicenseExpression" "MIT"
    $txt = Set-Property $txt "PackageIcon" "SimpleODM.png"

    if ($txt -ne $orig) {
        $total++
        if ($DryRun) {
            Write-Host "  [DRY] $rel" -ForegroundColor Gray
        } else {
            Set-Content -Path $file.FullName -Value $txt.TrimEnd() -NoNewline
            Write-Host "  OK: $rel" -ForegroundColor Green
        }
    } else {
        Write-Host "  -- $rel" -ForegroundColor DarkGray
    }
}

Write-Host "`n=== Done ===" -ForegroundColor Cyan
Write-Host "  Total: $($csprojFiles.Count), Updated: $total" -ForegroundColor White
if ($DryRun) { Write-Host "  DRY RUN - no files modified" -ForegroundColor Yellow }
