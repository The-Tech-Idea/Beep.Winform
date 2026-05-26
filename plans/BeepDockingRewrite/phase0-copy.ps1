# Phase 0 - Base Infrastructure Copy Script
# Copies Krypton base classes and renames them for Beep

$ErrorActionPreference = "Stop"

# Base paths
$kryptonBase = "C:/Users/f_ald/source/repos/The-Tech-Idea/Standard-Toolkit-master/Standard-Toolkit-master/Source/Krypton Components/Krypton.Toolkit"
$beepBase = "C:/Users/f_ald/source/repos/The-Tech-Idea/Beep.Winform/TheTechIdea.Beep.Winform.Controls/Docking"

# Name mappings for Phase 0
$globalMappings = @{
    # Control Base Classes
    "VisualControlBase" = "BeepVisualControlBase"
    "VisualControlContainment" = "BeepVisualControlContainment"
    "VisualSimpleBase" = "BeepVisualSimpleBase"
    "VisualForm" = "BeepVisualForm"
    "VisualPanel" = "BeepVisualPanel"
    "VisualControl" = "BeepVisualControl"
    "VisualContainerControl" = "BeepVisualContainerControl"
    "VisualContainerControlBase" = "BeepVisualContainerControlBase"
    
    # View System
    "ViewBase" = "BeepViewBase"
    "ViewComposite" = "BeepViewComposite"
    "ViewDrawPanel" = "BeepViewDrawPanel"
    "ViewManager" = "BeepViewManager"
    "ViewLayoutDocker" = "BeepViewLayoutDocker"
    "ViewLayoutFill" = "BeepViewLayoutFill"
    "ViewDrawCanvas" = "BeepViewDrawCanvas"
    "ViewDrawDocker" = "BeepViewDrawDocker"
    "ViewDrawButton" = "BeepViewDrawButton"
    "ViewLayoutSeparator" = "BeepViewLayoutSeparator"
    "ViewDrawWorkspaceSeparator" = "BeepViewDrawWorkspaceSeparator"
    
    # Palette System
    "PaletteBase" = "BeepPaletteBase"
    "PaletteRedirect" = "BeepPaletteRedirect"
    "PaletteDoubleRedirect" = "BeepPaletteDoubleRedirect"
    "PaletteBack" = "BeepPaletteBack"
    "PaletteBorder" = "BeepPaletteBorder"
    "PaletteContent" = "BeepPaletteContent"
    "PaletteDragDrop" = "BeepPaletteDragDrop"
    "IPaletteDragDrop" = "IBeepPaletteDragDrop"
    "IRenderer" = "IBeepRenderer"
    "RenderBase" = "BeepRenderBase"
    "RenderStandard" = "BeepRenderStandard"
    
    # Utilities
    "TypedCollection" = "BeepTypedCollection"
    "BoolFlags31" = "BeepBoolFlags31"
    "CommonHelper" = "BeepCommonHelper"
    
    # Delegates and Events
    "NeedPaintHandler" = "BeepNeedPaintHandler"
    "NeedPaintPaletteHandler" = "BeepNeedPaintPaletteHandler"
    
    # Other common types
    "KryptonContextMenu" = "BeepContextMenu"
    "KryptonCustomPaletteBase" = "BeepCustomPaletteBase"
    "PaletteMode" = "BeepPaletteMode"
    "SimpleCall" = "BeepSimpleCall"
    "ToolTipManager" = "BeepToolTipManager"
    "ToolTipValues" = "BeepToolTipValues"
    "VisualPopupToolTip" = "BeepVisualPopupToolTip"
    "IKryptonDebug" = "IBeepDebug"
    "KryptonManager" = "BeepManager"
    "Redirector" = "BeepRedirector"
    "KryptonContextMenuStrip" = "BeepContextMenuStrip"
}

function Copy-BeepFile {
    param(
        [string]$SourcePath,
        [string]$DestPath,
        [string]$SubNamespace = ""
    )
    
    if (-not (Test-Path $SourcePath)) {
        Write-Warning "Source file not found: $SourcePath"
        return
    }
    
    $content = Get-Content -Path $SourcePath -Raw
    
    # Replace namespace
    $destNamespace = "TheTechIdea.Beep.Winform.Controls.Docking"
    if ($SubNamespace) {
        $destNamespace += ".$SubNamespace"
    }
    $content = $content -replace "namespace Krypton.Toolkit;", "namespace $destNamespace;"
    $content = $content -replace "namespace Krypton.Toolkit", "namespace $destNamespace"
    
    # Replace class names (longest first to avoid partial replacements)
    $sortedMappings = $globalMappings.GetEnumerator() | Sort-Object { $_.Key.Length } -Descending
    foreach ($mapping in $sortedMappings) {
        # Whole word replacements to avoid partial matches
        $pattern = '\b' + [regex]::Escape($mapping.Key) + '\b'
        $content = $content -replace $pattern, $mapping.Value
    }
    
    # Ensure directory exists
    $destDir = Split-Path -Parent $DestPath
    if (-not (Test-Path $destDir)) {
        New-Item -ItemType Directory -Path $destDir -Force | Out-Null
    }
    
    Set-Content -Path $DestPath -Value $content -Encoding UTF8
    Write-Host "  Created: $DestPath"
}

Write-Host "`n=== Phase 0: Base Infrastructure ===" -ForegroundColor Cyan

# Task 0.1 - Control Base Classes
Write-Host "`nTask 0.1 - Control Base Classes..." -ForegroundColor Yellow
$controlFiles = @(
    @{ Src = "Controls Visuals/VisualControlBase.cs"; Dest = "Base/BeepVisualControlBase.cs"; Ns = "Base" },
    @{ Src = "Controls Visuals/VisualControlContainment.cs"; Dest = "Base/BeepVisualControlContainment.cs"; Ns = "Base" },
    @{ Src = "Controls Visuals/VisualSimpleBase.cs"; Dest = "Base/BeepVisualSimpleBase.cs"; Ns = "Base" },
    @{ Src = "Controls Visuals/VisualForm.cs"; Dest = "Base/BeepVisualForm.cs"; Ns = "Base" },
    @{ Src = "Controls Visuals/VisualPanel.cs"; Dest = "Base/BeepVisualPanel.cs"; Ns = "Base" },
    @{ Src = "Controls Visuals/VisualControl.cs"; Dest = "Base/BeepVisualControl.cs"; Ns = "Base" },
    @{ Src = "Controls Visuals/VisualContainerControl.cs"; Dest = "Base/BeepVisualContainerControl.cs"; Ns = "Base" },
    @{ Src = "Controls Visuals/VisualContainerControlBase.cs"; Dest = "Base/BeepVisualContainerControlBase.cs"; Ns = "Base" }
)

foreach ($file in $controlFiles) {
    $srcPath = Join-Path $kryptonBase $file.Src
    $destPath = Join-Path $beepBase $file.Dest
    Copy-BeepFile -SourcePath $srcPath -DestPath $destPath -SubNamespace $file.Ns
}

Write-Host "`nPhase 0 files created successfully!" -ForegroundColor Green
Write-Host "Next: Run 'dotnet build' to check for missing dependencies." -ForegroundColor Cyan
