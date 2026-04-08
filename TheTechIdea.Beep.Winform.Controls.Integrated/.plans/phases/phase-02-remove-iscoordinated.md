# Phase 02 — Make `_formsManager` Mandatory; Remove `IsCoordinated`

**Repo:** Beep.Winform  
**Scope:** `BeepDataBlock.Coordination.cs`, `BeepDataBlock.cs`, and all other partials that use `IsCoordinated`.  
**Depends on:** Phase 01 (BeepDM contracts must exist).  
**Build check:** `dotnet build WinFormsApp.sln` — zero errors before moving to Phase 03.

---

## Why this phase exists

`IsCoordinated` is the root cause of duplicated state. It allows `BeepDataBlock` to run without FormsManager and causes every business-logic method to maintain a local fallback copy of state. Eliminating it means FormsManager is always present and always the single source of truth. All subsequent phases benefit from this invariant.

---

## Clean Code Rules

- `_formsManager` is a required dependency — never nullable after construction.
- Fail fast: if FormsManager cannot be resolved, throw in the constructor (not silently swallow).
- No `null` checks on `_formsManager` inside method bodies — the field is guaranteed non-null.
- `IsCoordinated` is deleted from the public API. Callers that checked it must be updated in this phase.
- Partial files keep the `#region` / `#endregion` blocks clean — remove any region that becomes empty.

---

## Files Changed

| File | Change |
|---|---|
| `DataBlocks/BeepDataBlock.Coordination.cs` | Rewrite — `_formsManager` mandatory, `IsCoordinated` deleted |
| `DataBlocks/BeepDataBlock.cs` | Remove `IsCoordinated` usage in constructor / OnLoad |
| `DataBlocks/BeepDataBlock.Triggers.cs` | Remove `if (IsCoordinated)` guards |
| `DataBlocks/BeepDataBlock.Validation.cs` | Remove `if (IsCoordinated)` guards |
| `DataBlocks/BeepDataBlock.LOV.cs` | Remove `if (IsCoordinated)` guards |
| `DataBlocks/BeepDataBlock.Properties.cs` | Remove `if (IsCoordinated)` guards |
| `DataBlocks/BeepDataBlock.UnitOfWork.cs` | Remove coordinated vs local branching |

---

## 2.1 — Rewrite `BeepDataBlock.Coordination.cs`

Remove: `_isRegisteredWithFormsManager`, `IsCoordinated` property, null-guard inside `FormManager` setter.  
Add: argument guard in setter, always-run registration.

```csharp
// BeepDataBlock.Coordination.cs
using System;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Editor.Forms.Interfaces;
using TheTechIdea.Beep.Editor.Forms.Models;
using TheTechIdea.Beep.Report;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepDataBlock
    {
        #region Fields

        private FormsManager _formsManager;

        #endregion

        #region Properties

        /// <summary>
        /// FormsManager for this block. Must not be null.
        /// Assigning a new instance automatically unregisters from the old one
        /// and registers with the new one.
        /// </summary>
        public FormsManager FormManager
        {
            get => _formsManager;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value),
                        "FormsManager is required. BeepDataBlock cannot operate without it.");

                if (ReferenceEquals(_formsManager, value))
                    return;

                if (_formsManager != null)
                    UnregisterFromFormsManager();

                _formsManager = value;
                RegisterWithFormsManager();
            }
        }

        #endregion

        #region Registration

        /// <summary>
        /// Registers this block with the current FormsManager.
        /// Called automatically when FormManager is assigned.
        /// </summary>
        public void RegisterWithFormsManager()
        {
            if (_formsManager == null)
                throw new InvalidOperationException(
                    "Cannot register: FormsManager has not been assigned.");

            if (string.IsNullOrEmpty(Name))
                Name = $"Block_{Guid.NewGuid():N}".Substring(0, 14);

            if (string.IsNullOrEmpty(FormName))
                FormName = "DefaultForm";

            var info = new DataBlockInfo
            {
                BlockName  = Name,
                FormName   = FormName,
                EntityName = EntityName,
                ConnectionName = ConnectionName
            };

            _formsManager.RegisterBlock(info);
        }

        /// <summary>
        /// Unregisters this block from the current FormsManager.
        /// Called automatically before switching to a new FormsManager instance.
        /// </summary>
        public void UnregisterFromFormsManager()
        {
            if (_formsManager == null || string.IsNullOrEmpty(Name))
                return;

            try { _formsManager.UnregisterBlock(Name); }
            catch (Exception ex)
            {
                ErrorObject?.Add(ex, "UnregisterFromFormsManager");
            }
        }

        #endregion
    }
}
```

---

## 2.2 — Update `BeepDataBlock.cs` constructor / OnLoad

Resolve FormsManager from `BeepServiceProvider` when it is not injected via the designer property.

```csharp
// Inside BeepDataBlock.cs — constructor addition
public BeepDataBlock()
{
    // ... existing init ...

    // Ensure FormsManager is always available.
    // Callers that inject it via the FormManager property will overwrite this default.
    if (_formsManager == null)
    {
        var fm = BeepServiceProvider.GetService<FormsManager>();
        if (fm != null)
            FormManager = fm;   // setter triggers RegisterWithFormsManager()
    }
}

// Inside OnLoad (or HandleCreated):
protected override void OnLoad(EventArgs e)
{
    base.OnLoad(e);

    if (_formsManager == null)
        throw new InvalidOperationException(
            $"BeepDataBlock '{Name}' requires a FormsManager. " +
            "Assign FormManager before loading the control.");

    // Registration happens in the setter; just verify it completed.
    if (!_formsManager.IsBlockRegistered(Name))
        RegisterWithFormsManager();
}
```

---

## 2.3 — Remove `if (IsCoordinated)` in each partial

**Pattern to find and remove** — grep for `if (IsCoordinated)` across all `BeepDataBlock.*.cs` files.

Each occurrence looks like:
```csharp
// BEFORE — dual-path anti-pattern
StoreLocally(data);
if (IsCoordinated)
{
    try { _formsManager.DoSomething(Name, data); }
    catch { /* local still active */ }
}
```

Replace with:
```csharp
// AFTER — single path, always delegates
_formsManager.DoSomething(Name, data);
```

The local storage line (`StoreLocally`) is also removed. That local state is fully eliminated in Phases 03–08 — this phase just removes the guards so the delegate lines are unconditional.

**Do not** add try/catch wrappers around the delegate calls here. FormsManager is guaranteed non-null; exceptions should propagate normally so bugs are visible immediately.

---

## 2.4 — Remove `IsCoordinated` from `IBeepDataBlock`

Find: `bool IsCoordinated { get; }` in `DataBlocks/Models/IBeepDataBlock.cs`  
Delete: that line.  
Add: `FormsManager FormManager { get; set; }` if not already present.

---

## Checklist

- [ ] Rewrite `BeepDataBlock.Coordination.cs` — remove `_isRegisteredWithFormsManager` and `IsCoordinated`
- [ ] Update constructor / `OnLoad` in `BeepDataBlock.cs` — always assign FormsManager
- [ ] Remove all `if (IsCoordinated)` blocks in `Triggers.cs`
- [ ] Remove all `if (IsCoordinated)` blocks in `Validation.cs`
- [ ] Remove all `if (IsCoordinated)` blocks in `LOV.cs`
- [ ] Remove all `if (IsCoordinated)` blocks in `Properties.cs`
- [ ] Remove all `if (IsCoordinated)` blocks in `UnitOfWork.cs`
- [ ] Remove `bool IsCoordinated { get; }` from `IBeepDataBlock`
- [ ] `dotnet build WinFormsApp.sln` — zero errors
