# Download Website Logos Script
# This script downloads logos directly from company websites
# Handles both SVG and PNG formats

param(
    [string]$InputFile = "TheTechIdea.Beep.Winform.Controls\GFX\Icons\datasources\website-sources.txt",
    [string]$OutputDir = "TheTechIdea.Beep.Winform.Controls\GFX\Icons",
    [switch]$Force
)

# Track statistics
$downloadedCount = 0
$skippedCount = 0
$failedCount = 0
$failedDownloads = @()

# Retry configuration
$maxRetries = 3
$retryDelay = 3000  # milliseconds - longer delay for website requests

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
Write-Host "  Website Logo Downloader" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Input:  $InputFile" -ForegroundColor White
Write-Host "Output: $OutputDir" -ForegroundColor White
Write-Host "Total:  $totalLines logos`n" -ForegroundColor White

$currentLine = 0

foreach ($line in $lines) {
    $currentLine++
    $percentage = [math]::Round(($currentLine / $totalLines) * 100)
    
    # Parse the line
    if ($line -match '^([^|]+)\|(.+)$') {
        $key = $Matches[1].Trim()
        $url = $Matches[2].Trim()
        
        # Determine file extension from URL
        $urlExtension = [System.IO.Path]::GetExtension($url).ToLower()
        if ($urlExtension -eq "") {
            $urlExtension = ".svg"  # Default to SVG
        }
        
        # We always want SVG output
        $filename = "$($key.ToLower()).svg"
        $outputPath = Join-Path $OutputDir $filename
        
        # For PNG downloads, use temporary path
        $tempPath = $outputPath
        if ($urlExtension -eq ".png") {
            $tempPath = Join-Path $OutputDir "$($key.ToLower()).png"
        }
        
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
                $webClient.Headers.Add("Accept", "image/svg+xml,image/*,*/*")
                $webClient.Headers.Add("Referer", $url)
                
                $webClient.DownloadFile($url, $tempPath)
                
                # If it was a PNG, note that conversion would be needed
                if ($urlExtension -eq ".png") {
                    Write-Host "SUCCESS (PNG - manual conversion needed)" -ForegroundColor Yellow
                    # Rename to indicate it needs conversion
                    if (Test-Path $outputPath) {
                        Remove-Item $outputPath -Force
                    }
                    Rename-Item $tempPath $outputPath
                }
                else {
                    Write-Host "SUCCESS" -ForegroundColor Green
                }
                
                $downloadSuccess = $true
                $downloadedCount++
                
            }
            catch {
                $lastError = $_.Exception.Message
                
                # Check if it's a rate limit or server error
                if ($lastError -match "429|503|Too Many Requests|Service Unavailable") {
                    if ($attempt -lt $maxRetries) {
                        Write-Host "(Server busy, retry $attempt/$maxRetries...) " -NoNewline -ForegroundColor Yellow
                        Start-Sleep -Milliseconds $retryDelay
                    }
                }
                elseif ($lastError -match "403|Forbidden") {
                    if ($attempt -lt $maxRetries) {
                        Write-Host "(Access denied, retry $attempt/$maxRetries...) " -NoNewline -ForegroundColor Yellow
                        Start-Sleep -Milliseconds ($retryDelay * 2)
                    }
                }
                else {
                    # For other errors, don't retry
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
            if ($lastError -match "404|Not Found") {
                $errorType = "404 Not Found"
            }
            elseif ($lastError -match "403|Forbidden") {
                $errorType = "403 Forbidden"
            }
            elseif ($lastError -match "429|Too Many Requests") {
                $errorType = "429 Too Many Requests"
            }
            elseif ($lastError -match "503|Service Unavailable") {
                $errorType = "503 Service Unavailable"
            }
            elseif ($lastError -match "SSL|TLS|Certificate") {
                $errorType = "SSL/Certificate Error"
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

        # Add a delay to be respectful to the websites
        Start-Sleep -Milliseconds 500
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
    Write-Host "Logo files saved to: $OutputDir" -ForegroundColor Cyan
}

# Write failed downloads to log file
if ($failedDownloads.Count -gt 0) {
    $logFile = Join-Path (Split-Path $OutputDir) "website-failed-downloads.log"
    
    Write-Host "`nWriting failed downloads to log file..." -ForegroundColor Yellow
    
    # Create log content
    $logContent = @"
# Failed Website Logo Downloads Log
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
    Write-Host "`nNote: Some company websites may block automated downloads." -ForegroundColor Yellow
    Write-Host "You may need to manually download these logos from their press/brand pages." -ForegroundColor Yellow
}

Write-Host "`nWebsite logo download complete!" -ForegroundColor Green
