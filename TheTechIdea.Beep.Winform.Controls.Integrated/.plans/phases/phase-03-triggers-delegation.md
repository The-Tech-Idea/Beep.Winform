# Phase 03 — Delegate `BeepDataBlock.Triggers.*` to FormsManager

**Repo:** Beep.Winform  
**Scope:** `BeepDataBlock.Triggers.cs` + model/helper deletion.  
**Depends on:** Phase 02 (`_formsManager` is guaranteed non-null, `IsCoordinated` is gone).  
**Build check:** `dotnet build WinFormsApp.sln` — zero errors before moving to Phase 04.

---

## What changes in this phase

| Item | Action |
|---|---|
| `private Dictionary<TriggerType, List<BeepDataBlockTrigger>> _triggers` | Delete field |
| `private Dictionary<string, BeepDataBlockTrigger> _namedTriggers` | Delete field |
| `private bool _suppressTriggers` | Delete field |
| Every public method body | Replace with one-line delegate to `_formsManager.Triggers.*` |
| `RegisterTriggerInternal()` | Delete — was only used to split local vs. FormsManager logic |
| `TriggerBridge` usages | Delete — no translation needed once types are unified |
| `Models/BeepDataBlockTrigger.cs` | Delete file |
| `Models/TriggerContext.cs` | Delete file |
| `Models/TriggerEnums.cs` | Delete file |
| `Helpers/BeepDataBlockTriggerHelper.cs` | Delete file |

---

## Clean Code Rules

- Every method in `BeepDataBlock.Triggers.cs` is exactly 1–3 lines after this phase.
- No try/catch around FormsManager calls — let exceptions propagate.
- No `// TODO` comments left in production code — fix or remove.
- Imports must not reference any deleted model.
- `private` helper methods that are now empty are deleted.
- Keep `#region`/`#endregion` only if the region contains 2+ members.

---

## Target: `BeepDataBlock.Triggers.cs` (full rewrite)

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheTechIdea.Beep.Editor.Forms.Models;   // TriggerType, TriggerContext, TriggerDefinition

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial — Trigger System.
    /// All trigger state is owned by <see cref="FormsManager"/>.
    /// Every method here is a one-line delegate.
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Registration

        /// <summary>
        /// Registers an anonymous trigger of the given type.
        /// Oracle Forms equivalent: writing a WHEN-xxx trigger.
        /// </summary>
        public void RegisterTrigger(
            TriggerType type,
            Func<TriggerContext, Task<bool>> handler,
            int executionOrder = 0)
            => _formsManager.Triggers.RegisterTrigger(Name, type, handler, executionOrder);

        /// <summary>
        /// Registers a named trigger so it can be enabled/disabled by name later.
        /// </summary>
        public void RegisterTrigger(
            string triggerName,
            TriggerType type,
            Func<TriggerContext, Task<bool>> handler,
            string description = null)
            => _formsManager.Triggers.RegisterTrigger(Name, triggerName, type, handler, description);

        /// <summary>Removes the named trigger from this block.</summary>
        public void UnregisterTrigger(string triggerName)
            => _formsManager.Triggers.UnregisterTrigger(Name, triggerName);

        #endregion

        #region Enable / Disable

        /// <summary>Enables the named trigger.</summary>
        public void EnableTrigger(string triggerName)
            => _formsManager.Triggers.EnableTrigger(Name, triggerName);

        /// <summary>Disables the named trigger (does not remove it).</summary>
        public void DisableTrigger(string triggerName)
            => _formsManager.Triggers.DisableTrigger(Name, triggerName);

        /// <summary>Suspends all triggers for this block.</summary>
        public void SuspendTriggers()
            => _formsManager.Triggers.Suspend(Name);

        /// <summary>Resumes all triggers for this block.</summary>
        public void ResumeTriggers()
            => _formsManager.Triggers.Resume(Name);

        #endregion

        #region Execution

        /// <summary>
        /// Executes all registered triggers of the given type.
        /// Returns false if any trigger cancels the operation.
        /// </summary>
        public Task<bool> ExecuteTrigger(TriggerType type, TriggerContext context)
            => _formsManager.Triggers.ExecuteAsync(Name, type, context);

        #endregion

        #region Query

        /// <summary>Returns all trigger definitions registered for this block.</summary>
        public List<TriggerDefinition> GetAllTriggers()
            => _formsManager.Triggers.GetBlockTriggers(Name);

        /// <summary>Returns trigger definitions of the given type.</summary>
        public List<TriggerDefinition> GetTriggers(TriggerType type)
            => _formsManager.Triggers.GetBlockTriggers(Name, type);

        /// <summary>Returns the named trigger, or null if not found.</summary>
        public TriggerDefinition GetTrigger(string triggerName)
            => _formsManager.Triggers.GetTrigger(Name, triggerName);

        #endregion
    }
}
```

---

## Using references to update

Remove from top of `BeepDataBlock.Triggers.cs`:
```csharp
// DELETE these
using TheTechIdea.Beep.Editor.Forms.Helpers;       // TriggerBridge
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;  // BeepDataBlockTrigger, TriggerContext, TriggerEnums
```

Add:
```csharp
using TheTechIdea.Beep.Editor.Forms.Models;   // TriggerType, TriggerContext, TriggerDefinition
```

---

## Files to delete

```
DataBlocks/Models/BeepDataBlockTrigger.cs
DataBlocks/Models/TriggerContext.cs
DataBlocks/Models/TriggerEnums.cs
DataBlocks/Helpers/BeepDataBlockTriggerHelper.cs
```

Before deleting, grep for usages outside the Triggers partial:
```
grep -r "BeepDataBlockTrigger\|TriggerBridge\|BeepDataBlockTriggerHelper" --include="*.cs"
```
Any file still referencing these must be updated to use the FormsManager equivalents first.

---

## Caller update guide

| Old call | New call |
|---|---|
| `new BeepDataBlockTrigger { TriggerType = TriggerType.WhenNewRecord, Handler = ... }` | `block.RegisterTrigger(TriggerType.WhenNewRecord, handler)` |
| `BeepDataBlockTriggerHelper.GetTriggerStatistics(block)` | `block.FormManager.Triggers.GetBlockTriggers(block.Name)` |
| `block._suppressTriggers = true` | `block.SuspendTriggers()` |
| `TriggerContext ctx = new TriggerContext { Block = block }` | `new TriggerContext { BlockName = block.Name }` *(FormsManager type)* |

---

## Checklist

- [ ] Grep for all `IsCoordinated` blocks removed in Phase 02 — confirm none left in `Triggers.cs`
- [ ] Rewrite `BeepDataBlock.Triggers.cs` to delegation-only methods
- [ ] Update `using` directives — remove old models, add FormsManager models
- [ ] Grep: `BeepDataBlockTrigger|TriggerBridge|BeepDataBlockTriggerHelper` — zero hits before deleting
- [ ] Delete `Models/BeepDataBlockTrigger.cs`
- [ ] Delete `Models/TriggerContext.cs`
- [ ] Delete `Models/TriggerEnums.cs`
- [ ] Delete `Helpers/BeepDataBlockTriggerHelper.cs`
- [ ] `dotnet build WinFormsApp.sln` — zero errors
