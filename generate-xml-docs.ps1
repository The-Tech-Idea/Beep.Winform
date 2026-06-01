param(
    [Parameter(Mandatory=$true)]
    [string]$TargetPath,
    [switch]$DryRun,
    [switch]$Recurse
)

$ErrorActionPreference = "Stop"

function Get-ChildFiles {
    if ($Recurse -and (Test-Path $TargetPath -PathType Container)) {
        return Get-ChildItem -Path $TargetPath -Recurse -Filter "*.cs" |
            Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' -and $_.Name -notmatch '\.Designer\.cs$' }
    }
    if (Test-Path $TargetPath -PathType Leaf) {
        return @(Get-Item $TargetPath)
    }
    return @()
}

$files = Get-ChildFiles
$totalAdded = 0
$totalFiles = 0

foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw
    $orig = $content

    # ── Public classes ─────────────────────────────────────────────
    $content = [regex]::Replace($content,
        '(?s)(\r?\n\h*)(\[\s*Description\s*\(\s*"([^"]+)"\s*\)\s*\])',
        { param($m)
            $indent = $m.Groups[1].Value
            $descAttr = $m.Groups[2].Value
            $desc = $m.Groups[3].Value
            if ($desc -notmatch '[.!?]$') { $desc += '.' }
            # Check if /// already above this attribute
            $before = $content.Substring(0, $m.Index)
            if ($before -match '///\s*</summary>\s*$') { return $indent + $descAttr }
            return "$indent/// <summary>`n${indent}/// $desc`n${indent}/// </summary>`n${indent}$descAttr"
        }
    )

    if ($content -ne $orig) {
        $totalFiles++
        $added = ([regex]::Matches($content, '/// <summary>')).Count - ([regex]::Matches($orig, '/// <summary>')).Count
        $totalAdded += $added
        $relPath = $file.FullName.Substring($TargetPath.Length).TrimStart('\', '/')
        if ($DryRun) {
            Write-Host "  [DRY] $relPath (+$added docs)" -ForegroundColor Gray
        } else {
            [System.IO.File]::WriteAllText($file.FullName, $content)
            Write-Host "  OK: $relPath (+$added docs)" -ForegroundColor Green
        }
    }
}

Write-Host "`n=== Done ===" -ForegroundColor Cyan
Write-Host "  Files: $totalFiles, XML docs added: $totalAdded" -ForegroundColor White
if ($DryRun) { Write-Host "  DRY RUN - no files modified" -ForegroundColor Yellow }
