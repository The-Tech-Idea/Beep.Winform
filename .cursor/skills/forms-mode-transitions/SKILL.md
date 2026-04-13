---
name: forms-mode-transitions
description: Detailed guidance for FormsManager mode transitions in BeepDM. Use when working with EnterQueryModeAsync, ExecuteQueryAndEnterCrudModeAsync, EnterCrudModeForNewRecordAsync, trigger wiring around transitions, LOV validation on field change, DML trigger hooks, key trigger defaults, and trigger chaining/dependency rules.
---

# Forms Mode Transitions

Use this skill when implementing or debugging mode-aware behavior in `FormsManager`, including all Oracle Forms trigger hooks that fire as a side effect of mode changes.

## File Locations
- `DataManagementEngineStandard/Editor/Forms/FormsManager.ModeTransitions.cs`
- `DataManagementEngineStandard/Editor/Forms/FormsManager.EnhancedOperations.cs`
- `DataManagementEngineStandard/Editor/Forms/FormsManager.KeyTriggers.cs`
- `DataManagementEngineStandard/Editor/Forms/FormsManager.DmlTriggers.cs`
- `DataManagementEngineStandard/Editor/Forms/FormsManager.TriggerChaining.cs`
- `DataManagementEngineStandard/Editor/Forms/Helpers/DirtyStateManager.cs`
- `DataManagementEngineStandard/Editor/Forms/Helpers/TriggerManager.cs`
- `DataManagementEngineStandard/Editor/Forms/Helpers/TriggerDependencyManager.cs`

## Core Mode-Transition APIs
- `EnterQueryModeAsync(blockName)` — fires `PreQuery`; resets current record
- `ExecuteQueryAndEnterCrudModeAsync(blockName, filters)` — fires `PreQuery`/`PostQuery`; enters CRUD
- `EnterCrudModeForNewRecordAsync(blockName)` — resolves unsaved changes, then enters CRUD for new record
- `CreateNewRecordInMasterBlockAsync(blockName)` — convenience entry into a new master record
- `ValidateAllBlocksForModeTransitionAsync()` — cross-block readiness check before global transition
- `GetBlockMode(blockName)` → `BlockMode` enum
- `GetAllBlockModeInfo()` — snapshot of all block modes
- `IsFormReadyForModeTransitionAsync()` — form-level readiness check

## Trigger Wiring Around Transitions
- `PreInsert`/`PostInsert` fire in `InsertRecordEnhancedAsync`
- `PreUpdate`/`PostUpdate` fire in `UpdateCurrentRecordAsync`
- `PreDelete`/`PostDelete` fire in `DeleteCurrentRecordAsync`
- `PreQuery`/`PostQuery` fire in `ExecuteQueryEnhancedAsync`
- `PreCommit`/`PostCommit` fire in `CommitFormAsync`
- `WhenNewBlockInstance` fires in `SwitchToBlockAsync`
- `WhenCreateRecord` fires in `CreateNewRecord`
- `WhenRemoveRecord` fires in delete operations
- `OnPopulateDetails` fires in `SynchronizeDetailBlocksAsync`
- Returning `TriggerResult.Cancelled` from any trigger aborts the triggering operation

## Validation Integration On Mode Change
- `ValidateItem` fires automatically via `OnBlockFieldChanged` (LOV validation included)
- `ValidateRecord` fires before per-record commit
- `CrossBlockValidation.Validate` fires in `CommitFormAsync`
- `ValidateBlock` fires in post-query transition
- `ValidateBeforeNavigation` config option gates record navigation

## Key Trigger APIs (KeyTriggers)
- `RegisterKeyTrigger(blockName, key, handler)`
- `RegisterKeyTriggerAsync(blockName, key, asyncHandler)`
- `FireKeyTriggerAsync(blockName, key)` — falls through to `ExecuteKeyDefaultActionAsync` if unregistered
- `ExecuteKeyDefaultActionAsync(blockName, key)` — built-in default for NEXT-ITEM, PREV-ITEM, etc.

## DML Trigger APIs (DmlTriggers)
- `FireOnInsertAsync(blockName, record)` — `ON-INSERT` (type 40)
- `FireOnUpdateAsync(blockName, record)` — `ON-UPDATE` (type 41)
- `FireOnDeleteAsync(blockName, record)` — `ON-DELETE` (type 42)
- `RaiseFormTriggerAsync(triggerName)` — programmatic `RAISE_FORM_TRIGGER`

## Trigger Chaining and Dependency (TriggerChaining)
- Dependency graph prevents cycles (DFS in `TriggerDependencyManager.FindCycle`)
- `TriggerExecutionLog` collects execution statistics per trigger per block
- Chained triggers execute in dependency order; circular dependencies throw at registration time

## Working Rules
1. Never force mode changes by mutating block state externally.
2. Always inspect returned `IErrorsInfo`; treat `Cancelled` as a user or business-rule halt, not an error.
3. Resolve unsaved changes and parent context before entering detail CRUD flows.
4. Use `TriggerResult.Cancelled` to cancel an operation from inside a trigger; do not throw.
5. Preserve LOV field population after validation — it runs in the same `OnBlockFieldChanged` pipeline.

## Related Skills
- [`forms`](../forms/SKILL.md)
- [`forms-enhanced-data-operations`](../forms-enhanced-data-operations/SKILL.md)
- [`forms-helper-managers`](../forms-helper-managers/SKILL.md)

## Detailed Reference
Use [`reference.md`](./reference.md) for transition flows, failure patterns, and debug checks.
