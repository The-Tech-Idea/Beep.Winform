---
name: forms-enhanced-data-operations
description: Detailed guidance for FormsManager enhanced CRUD, LOV, sequences, undo/redo, export, batch commit, aggregates, and DML trigger operations in BeepDM. Use when implementing CreateNewRecord, InsertRecordEnhancedAsync, UpdateCurrentRecordAsync, ExecuteQueryEnhancedAsync, ShowLOVAsync, UndoBlock, BatchCommitAsync, or block data aggregates.
---

# Forms Enhanced Data Operations

Use this skill when operating on data through `FormsManager.EnhancedOperations`, `FormsManager.DataOperations`, `FormsManager.GenericOperations`, `FormsManager.Sequences`, or `FormsManager.DmlTriggers`.

## File Locations
- `DataManagementEngineStandard/Editor/Forms/FormsManager.EnhancedOperations.cs`
- `DataManagementEngineStandard/Editor/Forms/FormsManager.BasicDataOps.cs`
- `DataManagementEngineStandard/Editor/Forms/FormsManager.DataOperations.cs`
- `DataManagementEngineStandard/Editor/Forms/FormsManager.GenericOperations.cs`
- `DataManagementEngineStandard/Editor/Forms/FormsManager.Sequences.cs`
- `DataManagementEngineStandard/Editor/Forms/FormsManager.DmlTriggers.cs`
- `DataManagementEngineStandard/Editor/Forms/Helpers/TriggerManager.cs`
- `DataManagementEngineStandard/Editor/Forms/Helpers/LOVManager.cs`

## Core CRUD APIs (EnhancedOperations)
- `CreateNewRecord(blockName)` — typed record creation using registered `EntityType`
- `InsertRecordEnhancedAsync(blockName, record)` — insert with validation, pre/post trigger wiring
- `UpdateCurrentRecordAsync(blockName)` — update with pre/post trigger and relationship sync
- `DeleteCurrentRecordAsync(blockName)` — delete with `WhenRemoveRecord` / pre/post delete triggers
- `DuplicateCurrentRecordAsync(blockName)` — clone via `CopyFields` + new key
- `ExecuteQueryEnhancedAsync(blockName, filters)` — query with `PreQuery`/`PostQuery` triggers
- `GetCurrentRecord(blockName)` — typed current record accessor
- `GetRecordCount(blockName)` — loaded-record count
- `CopyFields(source, target, fieldNames[])` — safe field copy by explicit list
- `ApplyAuditDefaults(record, userName)` — stamp created/modified metadata

## Typed Registration (GenericOperations)
- `RegisterBlock<T>(blockName, uow, entityStructure, dataSourceName, isMasterBlock)` — stamps `EntityType` on `DataBlockInfo`
- `GetBlock<T>(blockName)` — verified typed block accessor
- `InsertRecordAsync<T>(blockName, record)` — create + insert shorthand
- `ShowLOVAsync(blockName, fieldName)` — display LOV and apply selected row + related fields

## Undo / Redo (DataOperations)
- `SetBlockUndoEnabled(blockName, enable, maxDepth)` — requires `IUndoable` UoW
- `UndoBlock(blockName)` / `RedoBlock(blockName)`
- `CanUndoBlock(blockName)` / `CanRedoBlock(blockName)`
- `GetBlockChangeSummary(blockName)` → `ChangeSummary`
- `GetFormChangeSummary()` → `IReadOnlyDictionary<string, ChangeSummary>`

## Batch Commit + Export / Import (DataOperations)
- `BatchCommitAsync(blockNames[], progress)` — commit multiple blocks with progress reporting
- `ExportBlockDataAsync(blockName, format, filePath)` — JSON, CSV, DataTable export
- `ImportBlockDataAsync(blockName, format, filePath)` — import from file into block records
- `GetBlockAggregate(blockName, fieldName, operation)` — Sum, Average, Count aggregates

## Sequences and Default Values (Sequences)
- `GetNextSequence(sequenceName)` — uses `ISequenceProvider`; prefer datasource sequence when real sequence exists
- `SetItemDefault(blockName, itemName, defaultFactory)` — factory called on `CreateNewRecord`
- `CopyFieldValue(srcBlock, srcField, destBlock, destField)` — single-field cross-block copy
- `PopulateGroupFromBlock(blockName, groupName)` — bulk copy to named group

## DML Triggers (DmlTriggers)
- `FireOnInsertAsync(blockName, record)` — fires `ON-INSERT` (TriggerType 40)
- `FireOnUpdateAsync(blockName, record)` — fires `ON-UPDATE` (TriggerType 41)
- `FireOnDeleteAsync(blockName, record)` — fires `ON-DELETE` (TriggerType 42)
- `RaiseFormTriggerAsync(triggerName)` — programmatic `RAISE_FORM_TRIGGER`

## Working Rules
1. Prefer enhanced methods over ad-hoc reflection in callers.
2. Always enter query/CRUD mode via `FormsManager` before CRUD operations.
3. Inspect returned `IErrorsInfo`; treat `Warning` distinctly from `Failed`.
4. Use explicit field lists in `CopyFields` — never rely on position-based copy.
5. Undo is only available when the backing UoW implements `IUndoable`.
6. Never consume sequences during query, paging, navigation, or cache prefetch.
7. DML trigger fire is in addition to the pre/post triggers wired in `EnhancedOperations` — do not double-fire.

## Related Skills
- [`forms`](../forms/SKILL.md)
- [`forms-mode-transitions`](../forms-mode-transitions/SKILL.md)
- [`forms-helper-managers`](../forms-helper-managers/SKILL.md)

## Detailed Reference
Use [`reference.md`](./reference.md) for flow examples, pitfalls, and verification checks.
