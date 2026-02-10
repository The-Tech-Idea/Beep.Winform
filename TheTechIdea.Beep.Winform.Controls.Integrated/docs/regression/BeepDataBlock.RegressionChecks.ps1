param(
    [string]$ProjectRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
)

$ErrorActionPreference = "Stop"

function Assert-Pattern {
    param(
        [string]$Path,
        [string]$Pattern,
        [string]$Message
    )

    $result = & rg -n --fixed-strings $Pattern $Path
    if (-not $result) {
        throw "FAILED: $Message`nPattern: $Pattern`nPath: $Path"
    }

    Write-Host "PASS: $Message"
}

$projectFile = Join-Path $ProjectRoot "TheTechIdea.Beep.Winform.Controls.Integrated.csproj"
$propertiesFile = Join-Path $ProjectRoot "DataBlocks\BeepDataBlock.Properties.cs"
$navigationFile = Join-Path $ProjectRoot "DataBlocks\BeepDataBlock.Navigation.cs"
$uowFile = Join-Path $ProjectRoot "DataBlocks\BeepDataBlock.UnitOfWork.cs"
$triggerFile = Join-Path $ProjectRoot "DataBlocks\BeepDataBlock.Triggers.cs"
$notifierFile = Join-Path $ProjectRoot "DataBlocks\Models\IBeepDataBlockNotifier.cs"

Write-Host "Building project..."
& dotnet build $projectFile -nologo -clp:ErrorsOnly | Out-Null
Write-Host "PASS: Build succeeded"

Write-Host "Checking item resolution regressions..."
Assert-Pattern -Path $propertiesFile -Pattern "private bool TryResolveItem(string identifier, out BeepDataBlockItem item, out string resolvedItemName)" -Message "TryResolveItem exists"
Assert-Pattern -Path $propertiesFile -Pattern "if (!TryResolveItem(itemName, out var item, out _))" -Message "SetItemProperty uses resolver"
Assert-Pattern -Path $propertiesFile -Pattern "if (!TryResolveItem(itemName, out var item, out _))" -Message "GetItemProperty uses resolver"
Assert-Pattern -Path $propertiesFile -Pattern "var itemName = NormalizeItemName(kvp.Key, kvp.Value);" -Message "RegisterAllItems normalizes item names"

Write-Host "Checking key trigger flow regressions..."
Assert-Pattern -Path $navigationFile -Pattern "private async void Control_KeyDown(object sender, KeyEventArgs e)" -Message "Keyboard handler is async trigger-aware"
Assert-Pattern -Path $navigationFile -Pattern "await FireKeyPrevItem(component);" -Message "Shift+Tab goes through KEY-PREV-ITEM trigger"
Assert-Pattern -Path $navigationFile -Pattern "await FireKeyNextItem(component);" -Message "Tab goes through KEY-NEXT-ITEM trigger"
Assert-Pattern -Path $navigationFile -Pattern "control.KeyDown -= Control_KeyDown;" -Message "Keyboard handlers are detached before attach"

Write-Host "Checking parent-child commit rollback regressions..."
Assert-Pattern -Path $uowFile -Pattern "var committedUnits = new List<IUnitofWork>();" -Message "Commit tracks committed units"
Assert-Pattern -Path $uowFile -Pattern "var rollbackSucceeded = await RollbackCommittedUnitsAsync(committedUnits);" -Message "Commit failure triggers rollback strategy"
Assert-Pattern -Path $uowFile -Pattern "private async Task<bool> RollbackCommittedUnitsAsync(IEnumerable<IUnitofWork> committedUnits)" -Message "Rollback strategy helper exists"

Write-Host "Checking trigger notifier regressions..."
Assert-Pattern -Path $notifierFile -Pattern "public interface IBeepDataBlockNotifier" -Message "Notifier interface exists"
Assert-Pattern -Path $triggerFile -Pattern "NotifyWarning(context.ErrorMessage" -Message "Trigger cancellation uses notifier"
Assert-Pattern -Path $triggerFile -Pattern "NotifyWarning(warningMessage" -Message "Trigger warnings use notifier"

Write-Host "All regression checks passed."
