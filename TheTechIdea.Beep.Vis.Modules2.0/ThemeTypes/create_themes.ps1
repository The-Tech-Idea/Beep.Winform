$themes = "Modern", "Minimal", "MacOS", "Fluent", "Cartoon", "ChatBubble", "Metro", "Metro2", "GNOME", "Brutalist", "Nordic", "iOS", "Windows11", "Ubuntu", "KDE", "ArcLinux", "Dracula", "Solarized", "OneDark", "GruvBox", "Nord", "Tokyo", "Paper", "Holographic", "Custom"

foreach ($theme in $themes) {
    $folder = $theme + "Theme"
    if (Test-Path $folder) {
        Remove-Item -Path $folder -Recurse -Force
    }
    Copy-Item -Path "DefaultTheme" -Destination $folder -Recurse
    cd $folder
    Rename-Item -Path "DefaultTheme.cs" -NewName "$themeTheme.cs"
    $newName = $theme + 'Theme'
    (Get-Content "$themeTheme.cs") -replace 'DefaultTheme', $newName | Set-Content "$themeTheme.cs"
    Get-ChildItem -Path "Parts" -Filter "*.cs" | ForEach-Object { (Get-Content $_.FullName) -replace 'DefaultTheme', $newName | Set-Content $_.FullName }
    cd ..
}