# PowerShell script to download SVG icons from datasource.txt and rename them
# Usage: .\download-svg-icons.ps1

param(
    [string]$InputFile = "TheTechIdea.Beep.Winform.Controls\GFX\Icons\datasources\datasource.txt",
    [string]$OutputDir = "TheTechIdea.Beep.Winform.Controls\GFX\Icons"
)

# Read the datasource.txt file
$content = Get-Content $InputFile -Raw

# Split content into lines
$lines = $content -split '\r?\n'

$downloadedCount = 0
$skippedCount = 0
$failedCount = 0
$totalLines = 0
$failedDownloads = @()
$maxRetries = 3
$retryDelay = 2000  # milliseconds

# Count total entries first
foreach ($line in $lines) {
    if ($line.Trim() -ne '' -and !$line.Trim().StartsWith('#') -and $line -match '\|') {
        $totalLines++
    }
}

Write-Host "`n============================================" -ForegroundColor Cyan
Write-Host "  SVG Icon Downloader" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Input:  $InputFile" -ForegroundColor Gray
Write-Host "Output: $OutputDir" -ForegroundColor Gray
Write-Host "Total:  $totalLines icons`n" -ForegroundColor Gray

# Process each line
$currentLine = 0

foreach ($line in $lines) {
    # Skip lines that start with # or are empty or don't contain |
    if ($line.Trim().StartsWith('#') -or $line.Trim() -eq '' -or $line -notmatch '\|') {
        continue
    }

    # Parse key|URL format
    if ($line -match '^([^|]+)\|(.+)$') {
        $key = $Matches[1].Trim()
        $url = $Matches[2].Trim()
        $currentLine++

        # Convert key to lowercase for filename
        $filename = "$($key.ToLower()).svg"
        $outputPath = Join-Path $OutputDir $filename

        # Progress indicator
        $progress = [math]::Round(($currentLine / $totalLines) * 100)
        Write-Host "[$progress%] " -NoNewline -ForegroundColor Cyan

        # Check if file already exists
        if (Test-Path $outputPath) {
            Write-Host "SKIP: $key -> $filename" -ForegroundColor Yellow
            $skippedCount++
            continue
        }

        try {
            Write-Host "GET:  $key" -NoNewline -ForegroundColor Green
            
            $downloaded = $false
            $attempt = 0
            
            while (!$downloaded -and $attempt -lt $maxRetries) {
                $attempt++
                
                try {
                    # Download the SVG file with user agent
                    $webClient = New-Object System.Net.WebClient
                    $webClient.Headers.Add("User-Agent", "Mozilla/5.0")
                    $webClient.DownloadFile($url, $outputPath)
                    $webClient.Dispose()
                    
                    $downloaded = $true
                    Write-Host " -> $filename" -ForegroundColor Gray
                    $downloadedCount++
                }
                catch {
                    $errorMsg = $_.Exception.Message
                    
                    # Check if it's a rate limit error (429)
                    if ($errorMsg -match "429|Too Many Requests") {
                        if ($attempt -lt $maxRetries) {
                            Write-Host " (Rate limited, retry $attempt/$maxRetries...)" -NoNewline -ForegroundColor Yellow
                            Start-Sleep -Milliseconds $retryDelay
                        }
                        else {
                            throw
                        }
                    }
                    else {
                        # Not a rate limit error, throw immediately
                        throw
                    }
                }
            }
        }
        catch {
            Write-Host " FAILED" -ForegroundColor Red
            $errorMsg = $_.Exception.Message
            
            # Extract error type
            $errorType = "Unknown"
            if ($errorMsg -match "404|Not Found") {
                $errorType = "404 Not Found"
            }
            elseif ($errorMsg -match "429|Too Many Requests") {
                $errorType = "429 Too Many Requests"
            }
            
            Write-Host "      Error: $errorType" -ForegroundColor DarkRed
            $failedCount++
            
            # Add to failed downloads list
            $failedDownloads += [PSCustomObject]@{
                Key = $key
                URL = $url
                Filename = $filename
                ErrorType = $errorType
                ErrorMessage = $errorMsg
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
Write-Host "  Total:       $($downloadedCount + $skippedCount + $failedCount) entries`n" -ForegroundColor White

if ($downloadedCount -gt 0) {
    Write-Host "SVG files saved to: $OutputDir" -ForegroundColor Cyan
}

# Write failed downloads to log file
if ($failedDownloads.Count -gt 0) {
    $logFile = Join-Path (Split-Path $OutputDir) "failed-downloads.log"
    
    Write-Host "`nWriting failed downloads to log file..." -ForegroundColor Yellow
    
    # Create log content
    $logContent = @"
# Failed SVG Downloads Log
# Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
# Total Failed: $($failedDownloads.Count)

"@
    
    # Group by error type
    $grouped = $failedDownloads | Group-Object -Property ErrorType
    
    foreach ($group in $grouped) {
        $logContent += "`n## $($group.Name) ($($group.Count) files)`n"
        foreach ($item in $group.Group) {
            $logContent += "$($item.Key)|$($item.URL)`n"
        }
    }
    
    # Write to file
    $logContent | Out-File -FilePath $logFile -Encoding UTF8
    
    Write-Host "Failed downloads logged to: $logFile" -ForegroundColor Yellow
    Write-Host "`nYou can retry failed downloads by running the script again." -ForegroundColor Yellow
}