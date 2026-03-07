$root = 'C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls'
$svgs = Join-Path $root 'IconsManagement\SvgsUI.cs'

$defined = Select-String -Path $svgs -Pattern 'public static readonly string\s+(\w+)\s*=' |
    ForEach-Object { $_.Matches[0].Groups[1].Value } |
    Sort-Object -Unique

$definedSet = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
$defined | ForEach-Object { [void]$definedSet.Add($_) }

$map = [ordered]@{
    'Activity'='Analyze'; 'ArrowDown'='CircleArrowDown'; 'ArrowRight'='CircleArrowRight'; 'ArrowUp'='CircleArrowUp'
    'Atom'='AtomN2'; 'BarChart'='ChartDots'; 'BarChart2'='ChartDotsN2'; 'BellOff'='BellX'; 'Box'='BoxMultiple'
    'Brand'='BrandTabler'; 'Building'='BuildingBroadcastTower'; 'Cart'='ShoppingCart'; 'Chart'='ChartDots'
    'CheckCircle'='CircleCheck'; 'ChevronLeft'='CircleChevronLeft'; 'ChevronUp'='CircleChevronUp'; 'Code'='CodeCircle'
    'Company'='BuildingBroadcastTower'; 'CornerDownRight'='CircleArrowDownRight'; 'Currency'='CashBanknote'
    'Dollar'='CashBanknote'; 'DollarSign'='CashBanknote'; 'Dot'='CircleDot'; 'Ellipsis'='Dots'; 'EyeOff'='Eye'
    'Facebook'='BrandFacebook'; 'FolderPlus'='FolderOpen'; 'Frown'='MoodSad'; 'Github'='BrandGithub'; 'Google'='BrandGoogle'
    'Grid'='LayoutGrid'; 'Happy'='MoodHappy'; 'HardDrive'='DeviceDesktop'; 'Hash'='Hash'; 'HeartOff'='HeartBroken'
    'IdCard'='Id'; 'Image'='Photo'; 'Info'='InfoCircle'; 'Loader'='CircleArrowRight'; 'LogIn'='Login'; 'Logo'='BrandTabler'
    'Menu'='MenuN2'; 'MessageSquare'='MessageCircle'; 'Mic'='Microphone'; 'MicOff'='MicrophoneOff'; 'Microsoft'='BrandWindows'
    'Minus'='SquareMinus'; 'Monitor'='DeviceDesktop'; 'More'='Dots'; 'Package'='BoxMultiple'; 'Pause'='PlayerPause'
    'Percent'='Percentage'; 'Person'='User'; 'PieChart'='ChartPie'; 'Play'='PlayerPlay'; 'Power'='Bolt'; 'PowerOff'='BoltOff'
    'Sad'='MoodSad'; 'ShieldOff'='ShieldHalf'; 'Shopping'='ShoppingCart'; 'Signal'='Wifi'; 'SignalOff'='WifiOff'
    'Sliders'='Adjustments'; 'Smile'='MoodSmile'; 'Sound'='Speakerphone'; 'Thermometer'='Temperature'
    'ThumbsDown'='ThumbDown'; 'ThumbsUp'='ThumbUp'; 'Tool'='Settings'; 'TrendingDown'='ArrowBadgeDown'
    'TrendingUp'='ArrowBadgeUp'; 'Twitter'='BrandTwitter'; 'Unlock'='LockOpen'; 'UserCircle'='User'; 'Users'='User'
    'Volume'='Volume'; 'VolumeX'='VolumeOff'; 'Wifi'='Wifi'; 'WifiOff'='WifiOff'; 'XCircle'='CircleX'; 'ZapOff'='BoltOff'
}

$resolved = [ordered]@{}
foreach ($k in $map.Keys) {
    $target = $map[$k]
    if (-not $definedSet.Contains($target)) {
        $target = 'Apps'
    }
    $resolved[$k] = $target
}

$files = Get-ChildItem $root -Filter '*.cs' -Recurse |
    Where-Object { $_.FullName -notlike '*\IconsManagement\SvgsUI.cs' }

$changedFiles = 0
$totalReplacements = 0

foreach ($f in $files) {
    $content = Get-Content $f.FullName -Raw
    if ($null -eq $content) { continue }

    $original = $content

    foreach ($old in $resolved.Keys) {
        $pattern = 'SvgsUI\.' + [regex]::Escape($old) + '(?![A-Za-z0-9_])'
        $replacement = 'SvgsUI.' + $resolved[$old]
        $count = [regex]::Matches($content, $pattern).Count
        if ($count -gt 0) {
            $content = [regex]::Replace($content, $pattern, $replacement)
            $totalReplacements += $count
        }
    }

    if ($content -ne $original) {
        Set-Content -Path $f.FullName -Value $content -Encoding UTF8
        $changedFiles++
    }
}

$mapLog = Join-Path $root 'oldiconx_mapping_applied.txt'
@(
    'Applied SvgsUI remap',
    ('Generated: ' + (Get-Date -Format 'yyyy-MM-dd HH:mm:ss')),
    ('Changed files: ' + $changedFiles),
    ('Total replacements: ' + $totalReplacements),
    ''
) | Set-Content $mapLog -Encoding UTF8

$resolved.GetEnumerator() |
    ForEach-Object { '{0} => {1}' -f $_.Key, $_.Value } |
    Add-Content $mapLog

Write-Output ('CHANGED_FILES=' + $changedFiles)
Write-Output ('TOTAL_REPLACEMENTS=' + $totalReplacements)
Write-Output ('MAP_LOG=' + $mapLog)
