# Phase 07 — Delegate Data Operations to FormsManager

**Repo:** Beep.Winform  
**Scope:** `BeepDataBlock.UnitOfWork.cs`, `BeepDataBlock.DataOperations.cs`.  
**Depends on:** Phase 02 (`_formsManager` guaranteed non-null).  
**Build check:** `dotnet build WinFormsApp.sln` — zero errors before moving to Phase 08.

---

## What changes in this phase

| Item | Action |
|---|---|
| `DataBlockUnitOfWorkHelper.*` call sites | Replace with `_formsManager.*Async(Name, ...)` calls |
| Coordinated vs. local branching in `ExecuteQueryWithUnitOfWorkAsync` | Remove — always delegate |
| `OnPreQuery` / `OnPostQuery` local events | Delete — replaced by FormsManager trigger types |
| `Helpers/DataBlockUnitOfWorkHelper.cs` | Delete file |

---

## Clean Code Rules

- All data operations are `async Task<bool>` — no synchronous variants.
- Pre/post hooks are FormsManager triggers (`TriggerType.PreQuery`, `TriggerType.PostQuery`, etc.) — not local .NET events.
- `IUnitofWork Data` property stays — it is the data source configured by the consumer. FormsManager reads it from `DataBlockInfo`.
- No direct calls to `Data.Units.*` inside these partials. FormsManager orchestrates that.
- Child block updates after query completion are coordinated by FormsManager relationship manager (`_formsManager.Relationships`), not local loops.

---

## Target: `BeepDataBlock.UnitOfWork.cs` (full rewrite)

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using TheTechIdea.Beep.DataBase;              // AppFilter

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial — Data Operations.
    /// All CRUD and query logic is executed by <see cref="FormsManager"/>.
    /// Every method here is a one-line delegate.
    /// Oracle Forms equivalents are noted per method.
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Query

        /// <summary>
        /// Executes a query against the block's entity.
        /// Oracle Forms equivalent: EXECUTE_QUERY built-in.
        /// </summary>
        public Task<bool> ExecuteQueryAsync(List<AppFilter> filters = null)
            => _formsManager.ExecuteQueryAsync(Name, filters);

        /// <summary>
        /// Enters query mode so the user can specify filter criteria.
        /// Oracle Forms equivalent: ENTER_QUERY built-in.
        /// </summary>
        public Task<bool> EnterQueryModeAsync()
            => _formsManager.EnterQueryModeAsync(Name);

        /// <summary>
        /// Cancels query mode without executing.
        /// Oracle Forms equivalent: EXIT_FORM with abandon changes.
        /// </summary>
        public Task CancelQueryModeAsync()
            => _formsManager.CancelQueryModeAsync(Name);

        /// <summary>
        /// Returns the number of records in the current result set.
        /// Oracle Forms equivalent: GET_BLOCK_PROPERTY(CURRENT_RECORD_ATTRIBUTE).
        /// </summary>
        public int GetRecordCount()
            => _formsManager.GetRecordCount(Name);

        #endregion

        #region Record Mutations

        /// <summary>
        /// Creates a new record buffer.
        /// Oracle Forms equivalent: CREATE_RECORD built-in.
        /// </summary>
        public Task<bool> InsertRecordAsync()
            => _formsManager.InsertRecordAsync(Name);

        /// <summary>
        /// Commits pending inserts, updates, and deletes for this block.
        /// Oracle Forms equivalent: COMMIT_FORM built-in.
        /// </summary>
        public Task<bool> SaveAsync()
            => _formsManager.SaveAsync(Name);

        /// <summary>
        /// Deletes the current record.
        /// Oracle Forms equivalent: DELETE_RECORD built-in.
        /// </summary>
        public Task<bool> DeleteCurrentRecordAsync()
            => _formsManager.DeleteCurrentRecordAsync(Name);

        /// <summary>
        /// Rolls back uncommitted changes for this block.
        /// Oracle Forms equivalent: CLEAR_BLOCK with ask-commit.
        /// </summary>
        public Task RollbackAsync()
            => _formsManager.RollbackAsync(Name);

        #endregion

        #region Savepoints

        /// <summary>Creates a named savepoint for this block.</summary>
        public Task CreateSavepointAsync(string savepointName)
            => _formsManager.Savepoints.CreateAsync(Name, savepointName);

        /// <summary>Rolls back to the named savepoint, undoing changes since it was created.</summary>
        public Task RollbackToSavepointAsync(string savepointName)
            => _formsManager.Savepoints.RollbackToAsync(Name, savepointName);

        #endregion

        #region Dirty State

        /// <summary>
        /// Returns true if this block has uncommitted changes.
        /// Oracle Forms equivalent: :SYSTEM.FORM_STATUS = 'CHANGED'.
        /// </summary>
        public bool IsDirty()
            => _formsManager.DirtyState.IsBlockDirty(Name);

        /// <summary>Marks all pending changes as clean without persisting.</summary>
        public void ClearDirtyState()
            => _formsManager.DirtyState.ClearBlock(Name);

        #endregion
    }
}
```

---

## Triggers instead of local events

Remove `OnPreQuery` and `OnPostQuery` event declarations from `BeepDataBlock.cs`.  
Consumers that previously subscribed to these events should register FormsManager triggers instead:

```csharp
// BEFORE — direct event subscription
block.OnPreQuery += (s, e) => e.Cancel = !CheckPermissions();

// AFTER — FormsManager trigger registration
block.RegisterTrigger(TriggerType.PreQuery, async ctx =>
{
    return CheckPermissions();
});
```

This works because FormsManager fires `TriggerType.PreQuery` before calling `ExecuteQueryAsync` internally.

---

## Child block refresh after query

Remove any `foreach (var childBlock in ChildBlocks) { childBlock.SetMasterRecord(...) }` loops.  
FormsManager relationship manager fires `TriggerType.WhenMasterRecordChanged` on child blocks automatically.  
Child blocks receive this trigger and re-query themselves.

To register a parent-child relationship:
```csharp
// In the form's setup code (not inside BeepDataBlock itself)
formsManager.Relationships.Register(new BlockRelationship
{
    MasterBlockName = "OrdersBlock",
    DetailBlockName = "OrderLinesBlock",
    MasterKey       = "OrderId",
    DetailKey       = "OrderId"
});
```

---

## Files to delete

```
DataBlocks/Helpers/DataBlockUnitOfWorkHelper.cs
```

Before deleting, grep:
```
grep -r "DataBlockUnitOfWorkHelper" --include="*.cs"
```

---

## Checklist

- [ ] Rewrite `BeepDataBlock.UnitOfWork.cs` to delegation-only methods
- [ ] Remove `OnPreQuery` / `OnPostQuery` event declarations from `BeepDataBlock.cs`
- [ ] Remove child-block refresh loops — replaced by `Relationships` in FormsManager
- [ ] Update all code that subscribed to `OnPreQuery`/`OnPostQuery` — switch to `RegisterTrigger`
- [ ] Grep: `DataBlockUnitOfWorkHelper|OnPreQuery|OnPostQuery|foreach.*ChildBlocks` — zero hits
- [ ] Delete `Helpers/DataBlockUnitOfWorkHelper.cs`
- [ ] `dotnet build WinFormsApp.sln` — zero errors
