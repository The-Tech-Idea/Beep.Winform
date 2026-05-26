# Beep Docking Rewrite - Automated Copy Script
# This script copies Krypton source files and renames them for Beep

param(
    [string]$SourceDir,
    [string]$DestDir,
    [hashtable]$NameMappings,
    [string]$SourceNamespace = "Krypton.Toolkit",
    [string]$DestNamespace = "TheTechIdea.Beep.Winform.Controls.Docking"
)

function Copy-AndRenameFile {
    param(
        [string]$SourceFile,
        [string]$DestFile,
        [hashtable]$Mappings,
        [string]$SrcNamespace,
        [string]$DstNamespace
    )
    
    # Read source content
    $content = Get-Content -Path $SourceFile -Raw
    
    # Replace namespace
    $content = $content -replace [regex]::Escape($SrcNamespace), $DstNamespace
    
    # Replace all mapped names (longest first to avoid partial replacements)
    $sortedMappings = $Mappings.GetEnumerator() | Sort-Object { $_.Key.Length } -Descending
    foreach ($mapping in $sortedMappings) {
        $content = $content -replace [regex]::Escape($mapping.Key), $mapping.Value
    }
    
    # Ensure directory exists
    $destDir = Split-Path -Parent $DestFile
    if (-not (Test-Path $destDir)) {
        New-Item -ItemType Directory -Path $destDir -Force | Out-Null
    }
    
    # Write destination file
    Set-Content -Path $DestFile -Value $content -Encoding UTF8
    Write-Host "Created: $DestFile"
}

# Example usage for Phase 0:
# $mappings = @{
#     "VisualControlBase" = "BeepVisualControlBase"
#     "VisualSimpleBase" = "BeepVisualSimpleBase"
#     # ... etc
# }
# Copy-AndRenameFile -SourceFile ".../VisualControlBase.cs" -DestFile ".../BeepVisualControlBase.cs" -Mappings $mappings
