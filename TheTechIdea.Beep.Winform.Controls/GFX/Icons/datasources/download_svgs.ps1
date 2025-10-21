# PowerShell script to download SVG icons for each data source
$datasourceFile = "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\GFX\Icons\datasources\datasource.txt"
$targetFolder = "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\GFX\Icons\datasources"

# Read all data source names (skip headers and empty lines)
$names = Get-Content $datasourceFile | Where-Object { $_ -and $_ -notmatch "^#" }

foreach ($name in $names) {
    # Build Bing search URL for SVG icon
    $searchUrl = "https://www.bing.com/images/search?q=$($name)+svg+icon&form=HDRSC2"
    $svgFile = Join-Path $targetFolder "$name.svg"

    # Use Invoke-WebRequest to get the first SVG image URL (requires parsing HTML)
    $html = Invoke-WebRequest -Uri $searchUrl -UseBasicParsing
    $svgUrl = ($html.Links | Where-Object { $_.href -like "*.svg*" })[0].href

    if ($svgUrl) {
        Invoke-WebRequest -Uri $svgUrl -OutFile $svgFile
        Write-Host "Downloaded $name.svg"
    } else {
        Write-Host "No SVG found for $name"
    }
}
