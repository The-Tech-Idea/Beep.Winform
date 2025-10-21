# Download Alternative SVG Icons Script
# This script downloads icons from alternative sources (jsdelivr, unpkg, GitHub) for previously failed downloads

param(
    [string]$InputFile = "TheTechIdea.Beep.Winform.Controls\GFX\Icons\datasources\alternative-sources.txt",
    [string]$OutputDir = "TheTechIdea.Beep.Winform.Controls\GFX\Icons",
    [switch]$Force  # Force re-download even if file exists
)

# Track statistics
$downloadedCount = 0
$skippedCount = 0
$failedCount = 0
$failedDownloads = @()

# Retry configuration
$maxRetries = 3
$retryDelay = 2000  # milliseconds

# Ensure output directory exists
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

# Read the input file
$lines = Get-Content $InputFile | Where-Object { 
    $_.Trim() -ne "" -and 
    -not $_.StartsWith("#") -and
    $_ -match "\|"
}

$totalLines = $lines.Count

Write-Host "`n============================================" -ForegroundColor Cyan
Write-Host "  Alternative SVG Icon Downloader" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Input:  $InputFile" -ForegroundColor White
Write-Host "Output: $OutputDir" -ForegroundColor White
Write-Host "Total:  $totalLines icons`n" -ForegroundColor White

$currentLine = 0

foreach ($line in $lines) {
    $currentLine++
    $percentage = [math]::Round(($currentLine / $totalLines) * 100)
    
    # Parse the line
    if ($line -match '^([^|]+)\|(.+)$') {
        $key = $Matches[1].Trim()
        $url = $Matches[2].Trim()
        $filename = "$($key.ToLower()).svg"
        $outputPath = Join-Path $OutputDir $filename
        
        # Check if file already exists
        if ((Test-Path $outputPath) -and -not $Force) {
            $skippedCount++
            Write-Host "[$percentage%] SKIP: $key -> $filename (already exists)" -ForegroundColor Yellow
            continue
        }
        
        if ((Test-Path $outputPath) -and $Force) {
            Write-Host "[$percentage%] REPLACE: $key " -NoNewline -ForegroundColor Cyan
        }
        else {
            Write-Host "[$percentage%] GET:  $key " -NoNewline -ForegroundColor Green
        }
        
        # Attempt download with retry logic
        $attempt = 0
        $downloadSuccess = $false
        $lastError = $null
        
        while ($attempt -lt $maxRetries -and -not $downloadSuccess) {
            $attempt++
            
            try {
                $webClient = New-Object System.Net.WebClient
                $webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36")
                $webClient.DownloadFile($url, $outputPath)
                
                $downloadSuccess = $true
                $downloadedCount++
                Write-Host "SUCCESS" -ForegroundColor Green
                
            }
            catch {
                $lastError = $_.Exception.Message
                
                # Check if it's a rate limit error
                if ($lastError -match "429" -or $lastError -match "Too Many Requests") {
                    if ($attempt -lt $maxRetries) {
                        Write-Host "(Rate limited, retry $attempt/$maxRetries...) " -NoNewline -ForegroundColor Yellow
                        Start-Sleep -Milliseconds $retryDelay
                    }
                }
                else {
                    # For non-rate-limit errors, don't retry
                    break
                }
            }
            finally {
                if ($null -ne $webClient) {
                    $webClient.Dispose()
                }
            }
        }
        
        # If download failed after all retries
        if (-not $downloadSuccess) {
            $failedCount++
            
            # Determine error type
            $errorType = "Unknown"
            if ($lastError -match "404" -or $lastError -match "Not Found") {
                $errorType = "404 Not Found"
            }
            elseif ($lastError -match "429" -or $lastError -match "Too Many Requests") {
                $errorType = "429 Too Many Requests"
            }
            elseif ($lastError -match "403" -or $lastError -match "Forbidden") {
                $errorType = "403 Forbidden"
            }
            
            Write-Host "FAILED" -ForegroundColor Red
            Write-Host "      Error: $errorType" -ForegroundColor Red
            
            # Track failed download
            $failedDownloads += [PSCustomObject]@{
                Key = $key
                URL = $url
                Filename = $filename
                ErrorType = $errorType
                ErrorMessage = $lastError
            }
        }

        # Add a small delay to be respectful to the servers
        Start-Sleep -Milliseconds 150
    }
}

Write-Host "`n============================================" -ForegroundColor Cyan
Write-Host "  Download Complete!" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "  Downloaded:  $downloadedCount files" -ForegroundColor Green
Write-Host "  Skipped:     $skippedCount files (already exist)" -ForegroundColor Yellow
Write-Host "  Failed:      $failedCount files" -ForegroundColor Red
Write-Host "  Total:       $totalLines entries`n" -ForegroundColor White

if ($downloadedCount -gt 0) {
    Write-Host "SVG files saved to: $OutputDir" -ForegroundColor Cyan
}

# Write failed downloads to log file
if ($failedDownloads.Count -gt 0) {
    $logFile = Join-Path (Split-Path $OutputDir) "alternative-failed-downloads.log"
    
    Write-Host "`nWriting failed downloads to log file..." -ForegroundColor Yellow
    
    # Create log content
    $logContent = @"
# Failed Alternative SVG Downloads Log
# Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
# Total Failed: $($failedDownloads.Count)

"@
    
    # Group by error type
    $grouped = $failedDownloads | Group-Object -Property ErrorType
    
    foreach ($group in $grouped) {
        $logContent += "`n## $($group.Name) ($($group.Count) files)`n"
        foreach ($item in $group.Group) {
            $logContent += "$($item.Key)|$($item.URL)`n"
            $logContent += "  Error: $($item.ErrorMessage)`n`n"
        }
    }
    
    # Write to file
    $logContent | Out-File -FilePath $logFile -Encoding UTF8
    
    Write-Host "Failed downloads logged to: $logFile" -ForegroundColor Yellow
}

Write-Host "`nAlternative icon download complete!" -ForegroundColor Green
