---
name: forms-operations-navigation
description: Detailed guidance for FormsManager form lifecycle, record navigation, block navigation, navigation-history, savepoints, record locking, and multi-form communication in BeepDM. Use when implementing OpenFormAsync, CloseFormAsync, CommitFormAsync, RollbackFormAsync, record/block movement, CallFormAsync, ReturnToCallerAsync, savepoint snapshots, or inter-form messaging.
---

# Forms Operations And Navigation

Use this skill for end-user form lifecycle behavior, record navigation, and all Phase 2–3 Oracle Forms built-in navigation and multi-form features.

## File Locations
- `DataManagementEngineStandard/Editor/Forms/FormsManager.FormOperations.cs`
- `DataManagementEngineStandard/Editor/Forms/FormsManager.Navigation.cs`
- `DataManagementEngineStandard/Editor/Forms/FormsManager.MultiFormNavigation.cs`
- `DataManagementEngineStandard/Editor/Forms/FormsManager.InterFormComm.cs`
- `DataManagementEngineStandard/Editor/Forms/Helpers/FormRegistry.cs`
- `DataManagementEngineStandard/Editor/Forms/Helpers/FormMessageBus.cs`
- `DataManagementEngineStandard/Editor/Forms/Helpers/SharedBlockManager.cs`
- `DataManagementEngineStandard/Editor/Forms/Helpers/SavepointManager.cs`
- `DataManagementEngineStandard/Editor/Forms/Helpers/LockManager.cs`
- `DataManagementEngineStandard/Editor/Forms/Helpers/NavigationHistoryManager.cs`

## Form Lifecycle APIs (FormOperations)
- `OpenFormAsync(formName)` — initializes form; fires `WhenNewFormInstance`
- `CloseFormAsync()` — fires `PostForm` triggers; clears cache per config
- `CommitFormAsync()` — fires `PreCommit`/crossblock-validation/UoW commit/`PostCommit`
- `RollbackFormAsync()` — reverts all blocks to last commit/savepoint
- `ClearAllBlocksAsync()` — `CLEAR_FORM` equivalent
- `ClearBlockAsync(blockName)` — `CLEAR_BLOCK` equivalent
- `ValidateForm()` — form-level validation; returns `IErrorsInfo`

## Record Navigation APIs (Navigation)
- `FirstRecordAsync(blockName)` / `LastRecordAsync(blockName)`
- `NextRecordAsync(blockName)` / `PreviousRecordAsync(blockName)`
- `NavigateToRecordAsync(blockName, index)`
- `SwitchToBlockAsync(blockName)` — fires `WhenNewBlockInstance`
- `GetCurrentRecordInfo(blockName)` → index, record count, dirty flag
- `GetAllNavigationInfo()` — snapshot across all blocks

## Navigation Built-ins (Phase 2)
- `GoBlock(blockName)` — direct block switch
- `GoItem(blockName, itemName)` — set focus to an item
- `GoRecord(blockName, recordIndex)` — direct record jump
- `NextBlock()` / `PreviousBlock()` — forward/back in block tab order
- `NextItem(blockName)` / `PreviousItem(blockName)` — item tab traversal

## Savepoints (SavepointManager)
- `CreateSavepoint(blockName, savepointName)` — snapshot named record state
- `RestoreSavepoint(blockName, savepointName)` — roll back to named snapshot
- `ClearSavepoints(blockName)` — discard all named snapshots for a block

## Record Locking (LockManager)
- `LockCurrentRecord(blockName)` — client-side lock on current record
- `UnlockCurrentRecord(blockName)` — release current-record lock
- `IsRecordLocked(blockName)` — check lock state
- Auto-lock on enter-CRUD; auto-release on commit/rollback (configured via `DataBlockInfo`)

## Multi-Form Navigation (MultiFormNavigation)
- `CallFormAsync(formName, parameters, mode)` — suspend this form; activate target (`CALL_FORM`)
- `OpenFormAsync(formName, parameters)` — open independently in modeless mode (`OPEN_FORM`)
- `NewFormAsync(formName, parameters)` — close this form and open a new one (`NEW_FORM`)
- `ReturnToCallerAsync(returnValue)` — close current form and resume parent caller
- `GetFormParameter(paramName)` — retrieve passed parameter value

## Inter-Form Communication (InterFormComm)
- `SetGlobalVariable(name, value)` / `GetGlobalVariable(name)` — `:GLOBAL.*` emulation
- `SendParameterToForm(formName, paramName, value)` — pre-open parameter injection
- `PostMessageAsync(targetFormName, messageType, payload)` — fire-and-forget form message
- `OnFormMessage` event — subscribe to incoming form messages
- `CreateSharedBlock(blockName, uow)` / `GetSharedBlock(blockName)` — cross-form shared UoW block

## Navigation History
- `NavigationHistory` property → `NavigationHistoryManager`
- Records block/record navigation steps; supports back-navigation to previous positions

## Working Rules
1. Switch block explicitly before block-scoped operations.
2. Assume navigation can be cancelled by validation or unsaved-change policy (return `NavigationResult.Cancelled`).
3. Commit/rollback through form-level APIs, not per-block manual calls.
4. Detail-block synchronization after navigation is automatic when relationship is registered.
5. `CallFormAsync` suspends the current form; do not call it inside a navigation event handler.
6. Savepoints are record-level snapshots; full UoW rollback uses `RollbackFormAsync`.

## Related Skills
- [`forms`](../forms/SKILL.md)
- [`forms-helper-managers`](../forms-helper-managers/SKILL.md)
- [`forms-mode-transitions`](../forms-mode-transitions/SKILL.md)

## Detailed Reference
Use [`reference.md`](./reference.md) for safe lifecycle patterns, diagnostics, and pitfalls.
