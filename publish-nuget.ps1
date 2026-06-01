param(
    [string]$NuGetApiKey = "YOUR_NUGET_API_KEY_HERE",
    [string]$NuGetSource = "https://api.nuget.org/v3/index.json",
    [switch]$DryRun,
    [switch]$SkipPush
)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot

# ── Validate API key ──────────────────────────────────────────────
if (-not $SkipPush -and -not $NuGetApiKey) {
    $NuGetApiKey = Read-Host "Enter NuGet API key for $NuGetSource"
}
if (-not $SkipPush -and -not $NuGetApiKey) {
    Write-Error "NuGet API key required. Set -NuGetApiKey or edit the script."
    exit 1
}

# ── Detect changed projects via git ───────────────────────────────
Write-Host "Detecting changed projects..." -ForegroundColor Cyan

$changedFiles = git -C $root diff --name-only HEAD 2>$null
if (-not $changedFiles) {
    $changedFiles = git -C $root diff --name-only --cached 2>$null
}
if (-not $changedFiles) {
    Write-Host "No staged or unstaged changes. Checking last commit..." -ForegroundColor Yellow
    $changedFiles = git -C $root diff --name-only HEAD~1 HEAD 2>$null
}
if (-not $changedFiles) {
    Write-Host "No changes detected. Exiting." -ForegroundColor Yellow
    exit 0
}

Write-Host "Changed files:" -ForegroundColor Gray
$changedFiles | ForEach-Object { Write-Host "  $_" }

# ── Load skip list ────────────────────────────────────────────────
$skipFile = Join-Path $root "unfinished-projects.txt"
$skipDirs = @()
if (Test-Path $skipFile) {
    $skipDirs = Get-Content $skipFile | Where-Object { $_ -notmatch '^\s*(#|$)' } | ForEach-Object { $_.Trim() }
    Write-Host "`nSkipping $($skipDirs.Count) unfinished/utility projects." -ForegroundColor DarkGray
}

# ── Map changed files to .csproj files ────────────────────────────
$projects = @{}
foreach ($file in $changedFiles) {
    $dir = Split-Path -Parent (Join-Path $root $file)
    while ($dir -and $dir.StartsWith($root) -and $dir.Length -gt $root.Length) {
        $relDir = $dir.Substring($root.Length + 1)
        if ($skipDirs -contains $relDir) {
            Write-Host "  Skipping $relDir (excluded)" -ForegroundColor DarkYellow
            break
        }
        $csproj = Get-ChildItem -Path $dir -Filter "*.csproj" -Depth 0 -EA SilentlyContinue | Select-Object -First 1
        if ($csproj) {
            $projects[$csproj.FullName] = $true
            break
        }
        $dir = Split-Path -Parent $dir
    }
}

$csprojFiles = $projects.Keys | Sort-Object
if (-not $csprojFiles) {
    Write-Host "No matching .csproj files found. Exiting." -ForegroundColor Yellow
    exit 0
}

Write-Host "`nProjects to package:" -ForegroundColor Cyan
foreach ($f in $csprojFiles) {
    $rel = $f.Substring($root.Length + 1)
    $content = Get-Content -Path $f -Raw
    $ver = ''
    $pid = ''
    if ($content -match '<Version>(.*?)</Version>') { $ver = $Matches[1] }
    if ($content -match '<PackageId>(.*?)</PackageId>') { $pid = $Matches[1] }
    Write-Host "  $rel  |  $pid v$ver" -ForegroundColor Gray
}

# ── Bump versions ──────────────────────────────────────────────────
Write-Host "`nBumping versions..." -ForegroundColor Cyan

foreach ($csproj in $csprojFiles) {
    $content = Get-Content -Path $csproj -Raw
    if ($content -match '<Version>(.*?)</Version>') {
        $oldVer = $Matches[1]
        $parts = $oldVer -split '\.'
        if ($parts.Length -ge 3) {
            $newVer = "$($parts[0]).$($parts[1]).$([int]$parts[2] + 1)"
        } else {
            $newVer = "$oldVer.1"
        }
        $content = $content -replace "<Version>$oldVer</Version>", "<Version>$newVer</Version>"
        $content = $content -replace "<PackageVersion>$oldVer</PackageVersion>", "<PackageVersion>$newVer</PackageVersion>"

        if ($DryRun) {
            Write-Host "  [DRY] $(Split-Path -Leaf $csproj): $oldVer -> $newVer" -ForegroundColor Gray
        } else {
            Set-Content -Path $csproj -Value $content.TrimEnd() -NoNewline
            Write-Host "  $(Split-Path -Leaf $csproj): $oldVer -> $newVer" -ForegroundColor Green
        }
    }
}

if ($DryRun) {
    Write-Host "`nDry run complete." -ForegroundColor Yellow
    exit 0
}

# ── Build and pack ─────────────────────────────────────────────────
Write-Host "`nBuilding and packing..." -ForegroundColor Cyan

$failed = @()
foreach ($csproj in $csprojFiles) {
    $name = Split-Path -Leaf (Split-Path -Parent $csproj)
    Write-Host "  $name..." -ForegroundColor Gray -NoNewline
    $output = dotnet build $csproj -c Release 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host " OK" -ForegroundColor Green
    } else {
        Write-Host " FAILED" -ForegroundColor Red
        $failed += (Split-Path -Leaf $csproj)
    }
}

# ── Push to NuGet ──────────────────────────────────────────────────
if (-not $SkipPush -and $failed.Count -eq 0) {
    Write-Host "`nPushing to $NuGetSource..." -ForegroundColor Cyan

    $nupkgs = Get-ChildItem -Path $root -Recurse -Filter "*.nupkg" -Depth 5 |
        Where-Object { $_.FullName -match '[\\/]Release[\\/]' -and -not $_.Name.EndsWith('.snupkg') }

    foreach ($pkg in $nupkgs) {
        Write-Host "  $($pkg.Name)..." -ForegroundColor Gray -NoNewline
        dotnet nuget push $pkg.FullName --api-key $NuGetApiKey --source $NuGetSource --skip-duplicate 2>&1 | Out-Null
        if ($LASTEXITCODE -eq 0) {
            Write-Host " OK" -ForegroundColor Green
        } else {
            Write-Host " FAILED" -ForegroundColor Red
        }
    }
}

# ── Summary ────────────────────────────────────────────────────────
Write-Host "`n=== Summary ===" -ForegroundColor Cyan
Write-Host "  Projects: $($csprojFiles.Count)" -ForegroundColor White
if ($failed.Count -gt 0) {
    Write-Host "  Failed: $($failed.Count)" -ForegroundColor Red
    $failed | ForEach-Object { Write-Host "    - $_" -ForegroundColor Red }
}
Write-Host "Done." -ForegroundColor Green
