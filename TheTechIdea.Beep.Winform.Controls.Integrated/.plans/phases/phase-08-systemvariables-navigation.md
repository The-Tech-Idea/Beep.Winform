# Phase 08 — Delegate System Variables and Navigation to FormsManager

**Repo:** Beep.Winform  
**Scope:** `BeepDataBlock.SystemVariables.cs`, `BeepDataBlock.Navigation.cs`.  
**Depends on:** Phase 02 (`_formsManager` guaranteed non-null).  
**Build check:** `dotnet build WinFormsApp.sln` — zero errors before moving to Phase 09.

---

## What changes in this phase

| Item | Action |
|---|---|
| Local `SystemVariables` object field | Remove — property reads from FormsManager |
| `private string _currentItemName` | Keep — WinForms focus tracking, not business state |
| `private IBeepUIComponent _currentItem` | Keep — WinForms focus tracking |
| Navigation methods (`NextRecord`, `PreviousRecord`, etc.) | Rewrite as delegates to `_formsManager.Navigation.*` |
| Tab-order calculation | Delegate to `_formsManager.Items.GetTabOrder(Name)` |
| `Models/SystemVariables.cs` (WinForms copy) | Delete file |

---

## Clean Code Rules

- `SYSTEM` is a read-only property — it reads from FormsManager; it does not cache.
- WinForms focus (`_currentItemName`, `_currentItem`) is a UI concern — it stays local but gets synced to `SystemVariables.CURSOR_ITEM` via FormsManager after each navigation.
- `GoToItem()` keeps its WinForms focus logic (`component.Focus()`) but also notifies FormsManager.
- Navigation methods (`NextRecord`, `PreviousRecord`) are pure delegates — no WinForms side-effects.

---

## Target: `BeepDataBlock.SystemVariables.cs` (full rewrite)

```csharp
using TheTechIdea.Beep.Editor.Forms.Models;  // SystemVariables (FormsManager type)

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial — System Variables.
    /// `:SYSTEM.*` pseudo-variables are owned by <see cref="FormsManager"/>.
    /// This property is a read-only accessor — no local copy.
    /// Oracle Forms equivalent: :SYSTEM built-in variables.
    /// </summary>
    public partial class BeepDataBlock
    {
        /// <summary>
        /// Oracle Forms :SYSTEM variables for this block.
        /// All reads go directly to FormsManager — always current.
        /// </summary>
        public SystemVariables SYSTEM
            => _formsManager.SystemVariables.GetSystemVariables(Name);
    }
}
```

---

## Target: `BeepDataBlock.Navigation.cs` (targeted rewrite — navigation methods only)

Replace each navigation method body with a delegate. WinForms focus logic inside `GoToItem` stays.

```csharp
using System;
using TheTechIdea.Beep.Vis.Modules;   // IBeepUIComponent

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial — Navigation &amp; Focus Management.
    /// Record-level navigation is delegated to <see cref="FormsManager"/>.
    /// Item-level focus management calls FormsManager then applies WinForms focus.
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Record Navigation

        /// <summary>
        /// Moves to the next record in the result set.
        /// Oracle Forms equivalent: NEXT_RECORD built-in.
        /// </summary>
        public bool NextRecord()
            => _formsManager.Navigation.NextRecord(Name);

        /// <summary>
        /// Moves to the previous record.
        /// Oracle Forms equivalent: PREVIOUS_RECORD built-in.
        /// </summary>
        public bool PreviousRecord()
            => _formsManager.Navigation.PreviousRecord(Name);

        /// <summary>
        /// Moves to the first record.
        /// Oracle Forms equivalent: FIRST_RECORD built-in.
        /// </summary>
        public bool FirstRecord()
            => _formsManager.Navigation.FirstRecord(Name);

        /// <summary>
        /// Moves to the last record.
        /// Oracle Forms equivalent: LAST_RECORD built-in.
        /// </summary>
        public bool LastRecord()
            => _formsManager.Navigation.LastRecord(Name);

        /// <summary>
        /// Moves to the record at the given zero-based index.
        /// Oracle Forms equivalent: GO_RECORD built-in.
        /// </summary>
        public bool GoToRecord(int index)
            => _formsManager.Navigation.GoToRecord(Name, index);

        #endregion

        #region Item Navigation

        /// <summary>
        /// Moves focus to the next navigable item.
        /// Oracle Forms equivalent: NEXT_ITEM built-in.
        /// </summary>
        public bool NextItem()
        {
            var nextItemName = _formsManager.Navigation.NextItem(Name, _currentItemName);
            return !string.IsNullOrEmpty(nextItemName) && GoToItem(nextItemName);
        }

        /// <summary>
        /// Moves focus to the previous navigable item.
        /// Oracle Forms equivalent: PREVIOUS_ITEM built-in.
        /// </summary>
        public bool PreviousItem()
        {
            var prevItemName = _formsManager.Navigation.PreviousItem(Name, _currentItemName);
            return !string.IsNullOrEmpty(prevItemName) && GoToItem(prevItemName);
        }

        /// <summary>
        /// Moves focus to the named item.
        /// Oracle Forms equivalent: GO_ITEM built-in.
        /// </summary>
        public bool GoToItem(string itemName)
        {
            if (!UIComponents.TryGetValue(itemName, out var component))
                return false;

            // Notify FormsManager so :SYSTEM variables are updated.
            _formsManager.Navigation.SetCurrentItem(Name, itemName);

            // WinForms focus — UI concern only.
            _currentItemName = itemName;
            _currentItem     = component;
            component.Focus();

            return true;
        }

        #endregion
    }
}
```

---

## Files to delete

```
DataBlocks/Models/SystemVariables.cs    ← WinForms-specific copy of the model
```

Before deleting, confirm all usages use the FormsManager `SystemVariables` type:
```
grep -r "SystemVariables" --include="*.cs"
```
Any `using` pointing to the WinForms model namespace must be updated to `TheTechIdea.Beep.Editor.Forms.Models`.

---

## Checklist

- [ ] Rewrite `BeepDataBlock.SystemVariables.cs` — one-line `SYSTEM` property accessor
- [ ] Rewrite navigation methods in `BeepDataBlock.Navigation.cs` — delegates + WinForms focus
- [ ] Remove local `SystemVariables` field from `BeepDataBlock.cs`
- [ ] Update `using` directives in affected files
- [ ] Grep: `new SystemVariables\|_systemVariables\|WinForms.*SystemVariables` — zero hits
- [ ] Delete `Models/SystemVariables.cs` (WinForms version)
- [ ] `dotnet build WinFormsApp.sln` — zero errors
