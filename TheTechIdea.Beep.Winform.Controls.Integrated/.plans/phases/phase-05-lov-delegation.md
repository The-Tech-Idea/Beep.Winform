# Phase 05 — Delegate `BeepDataBlock.LOV.*` to FormsManager

**Repo:** Beep.Winform  
**Scope:** `BeepDataBlock.LOV.cs`, `BeepDataBlock.Integration.cs` (LOV section), `Dialogs/BeepLOVDialog.cs`.  
**Depends on:** Phase 02 (`_formsManager` guaranteed non-null).  
**Build check:** `dotnet build WinFormsApp.sln` — zero errors before moving to Phase 06.

---

## What changes in this phase

| Item | Action |
|---|---|
| `private Dictionary<string, BeepDataBlockLOV> _lovs` | Delete field |
| `LOVBridge` usages | Delete — no translation needed once types unified |
| All public LOV methods | Rewrite as delegates to `_formsManager.LOV.*` |
| `AttachLOVToComponent` / `DetachLOVFromComponent` | Keep — these are WinForms UI-only operations |
| `BeepLOVDialog` constructor | Change param from `BeepDataBlockLOV` → `LOVDefinition` |
| `Models/BeepDataBlockLOV.cs` | Delete file |
| `Helpers/DataBlockQueryHelper.cs` | Delete file (`QueryOperator` enum now from BeepDM) |

---

## Clean Code Rules

- `RegisterLOV` follows Pattern B (delegate + UI side-effect): call FlowManager, then optionally attach button to component.
- `ShowLOV` is async — use `Task<bool>`, never `.GetAwaiter().GetResult()`.
- `MessageBox.Show` in business-logic paths is replaced by `_notifier.ShowWarning(...)`.
- No `_lovs[...]` dictionary access anywhere after this phase.

---

## Target: `BeepDataBlock.LOV.cs` (full rewrite)

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using TheTechIdea.Beep.Editor.Forms.Models;   // LOVDefinition, LOVColumnInfo
using TheTechIdea.Beep.Vis.Modules;           // IBeepUIComponent

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial — LOV (List of Values) system.
    /// LOV state is owned by <see cref="FormsManager"/>.
    /// This partial handles registration (delegate + UI wiring)
    /// and display (delegate only).
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Registration

        /// <summary>
        /// Registers a LOV for the given item/field.
        /// Oracle Forms equivalent: attaching a LOV to an item property.
        /// </summary>
        public void RegisterLOV(string itemName, LOVDefinition lov)
        {
            _formsManager.LOV.RegisterLOV(Name, itemName, lov);

            // --- WinForms UI side-effect only ---
            if (UIComponents.TryGetValue(itemName, out var component))
                AttachLOVToComponent(itemName, component);
        }

        /// <summary>Removes the LOV attached to the given item.</summary>
        public void UnregisterLOV(string itemName)
        {
            _formsManager.LOV.UnregisterLOV(Name, itemName);
            DetachLOVFromComponent(itemName);
        }

        /// <summary>Returns true if the given item has an attached LOV.</summary>
        public bool HasLOV(string itemName)
            => _formsManager.LOV.HasLOV(Name, itemName);

        /// <summary>Returns the LOV definition attached to the given item, or null.</summary>
        public LOVDefinition GetLOV(string itemName)
            => _formsManager.LOV.GetLOV(Name, itemName);

        #endregion

        #region Display

        /// <summary>
        /// Shows the LOV window for the given item and applies the selected value.
        /// Oracle Forms equivalent: LIST_VALUES built-in.
        /// </summary>
        public Task<bool> ShowLOV(string itemName)
            => _formsManager.LOV.ShowLOVAsync(Name, itemName, Notifier);

        /// <summary>
        /// Validates that the current value for the given item is in its LOV.
        /// Oracle Forms equivalent: LOV restriction validation.
        /// </summary>
        public Task<bool> ValidateLOVValue(string itemName, object value)
            => _formsManager.LOV.ValidateValueAsync(Name, itemName, value);

        #endregion

        #region WinForms UI Wiring — private helpers

        private void AttachLOVToComponent(string itemName, IBeepUIComponent component)
        {
            // Attach a search/browse button to the component so the user can
            // trigger the LOV dialog from the keyboard (F9) or a button click.
            // This is pure WinForms layout logic — no business state.
            component.ShowLOVButton = true;
            component.LOVButtonClick -= OnLOVButtonClick;
            component.LOVButtonClick += OnLOVButtonClick;
        }

        private void DetachLOVFromComponent(string itemName)
        {
            if (!UIComponents.TryGetValue(itemName, out var component))
                return;

            component.ShowLOVButton = false;
            component.LOVButtonClick -= OnLOVButtonClick;
        }

        private async void OnLOVButtonClick(object sender, LOVButtonClickEventArgs e)
        {
            await ShowLOV(e.ItemName);
        }

        #endregion
    }
}
```

---

## `BeepDataBlock.Integration.cs` — LOV section changes

Replace direct `_items[itemName]` access and `MessageBox.Show` with delegated calls and notifier:

```csharp
// BEFORE
public async Task ShowLOVIntegrated(string itemName)
{
    if (!_items.ContainsKey(itemName))
    {
        MessageBox.Show($"Item '{itemName}' not found.", "Error");
        return;
    }
    // ...
}

// AFTER
public async Task ShowLOVIntegrated(string itemName)
{
    // Item existence is now validated inside FormsManager.LOV.ShowLOVAsync
    await ShowLOV(itemName);
}
```

---

## `BeepLOVDialog.cs` — change constructor parameter

```csharp
// BEFORE
public BeepLOVDialog(BeepDataBlockLOV lov) { ... }

// AFTER
public BeepLOVDialog(LOVDefinition lov) { ... }
```

---

## `QueryOperator` enum migration

`DataBlockQueryHelper.QueryOperator` is replaced by `TheTechIdea.Beep.Editor.Forms.Models.QueryOperator`.  
Update all references before deleting the helper file:
```
grep -r "QueryOperator\|DataBlockQueryHelper" --include="*.cs"
```

---

## Files to delete

```
DataBlocks/Models/BeepDataBlockLOV.cs
DataBlocks/Helpers/DataBlockQueryHelper.cs
```

---

## Checklist

- [ ] Rewrite `BeepDataBlock.LOV.cs` to delegation + WinForms UI wiring only
- [ ] Update `BeepDataBlock.Integration.cs` LOV section — remove direct `_items` access and `MessageBox.Show`
- [ ] Update `BeepLOVDialog` constructor — `BeepDataBlockLOV` → `LOVDefinition`
- [ ] Remove `LOVBridge` usages
- [ ] Update `using` directives
- [ ] Grep: `BeepDataBlockLOV|LOVBridge|DataBlockQueryHelper|_lovs` — zero hits
- [ ] Delete `Models/BeepDataBlockLOV.cs`
- [ ] Delete `Helpers/DataBlockQueryHelper.cs`
- [ ] `dotnet build WinFormsApp.sln` — zero errors
