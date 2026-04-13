---
name: forms-helper-managers
description: Detailed guidance for FormsManager helper architecture in BeepDM. Use when extending or customizing any of the 25 helper managers behind FormsManager orchestration — validation, triggers, LOV, savepoints, locking, paging, audit, security, multi-form registry, message bus, or item properties.
---

# Forms Helper Managers

Use this skill when touching helper classes under `Editor/Forms/Helpers`. All helpers are exposed as typed properties on `FormsManager`; construct them directly only in unit tests or custom DI wire-up.

## File Locations (all under `DataManagementEngineStandard/Editor/Forms/Helpers/`)

### Core orchestration helpers
- `DirtyStateManager.cs` — `DirtyStateManager` property (`IDirtyStateManager`)
- `EventManager.cs` — `EventManager` property (`IEventManager`)
- `BlockFactory.cs` — `BlockFactory` property (`IBlockFactory`)
- `BlockErrorLog.cs` — `ErrorLog` property (`IBlockErrorLog`)

### Relationship and data-shaping helpers
- `LOVManager.cs` — `LOV` property (`ILOVManager`)
- `QueryBuilderManager.cs` — `QueryBuilder` property (`IQueryBuilderManager`)
- `PagingManager.cs` — `Paging` property (`IPagingManager`)
- `MasterDetailKeyResolver.cs` — internal; resolves FK assignment timing

### Validation, triggers, and item interaction
- `ValidationManager.cs` — `Validation` property (`IValidationManager`)
- `TriggerManager.cs` — `Triggers` property (`ITriggerManager`)
- `TriggerLibrary.cs` — common trigger factories (audit stamp, auto-number, cascade delete, field formatting)
- `TriggerDependencyManager.cs` — `TriggerDependency` property; DFS cycle detection
- `TriggerExecutionLog.cs` — execution statistics available via `Triggers`
- `ItemPropertyManager.cs` — `ItemProperties` property (`IItemPropertyManager`)
- `BlockPropertyManager.cs` — `BlockProperties` property (`IBlockPropertyManager`)
- `MessageQueueManager.cs` — `Messages` property (`IMessageQueueManager`)
- `TimerManager.cs` — `Timers` property (`ITimerManager`)

### Transactional state helpers
- `SavepointManager.cs` — `Savepoints` property (`ISavepointManager`)
- `LockManager.cs` — `Locking` property (`ILockManager`)
- `CrossBlockValidationManager.cs` — `CrossBlockValidation` property
- `NavigationHistoryManager.cs` — internal; backs `NavigationHistory` on FormsManager

### Cross-form and platform helpers
- `FormRegistry.cs` — `Registry` property (`IFormRegistry`)
- `FormMessageBus.cs` — `MessageBus` property (`IFormMessageBus`)
- `SharedBlockManager.cs` — `SharedBlocks` property (`ISharedBlockManager`)
- `DefaultAlertProvider.cs` — `AlertProvider` property (`IAlertProvider`)
- `SequenceProvider.cs` — `Sequences` property (`ISequenceProvider`)
- `SystemVariablesManager.cs` — `SystemVariables` property (`ISystemVariablesManager`)

### Audit and security helpers
- `AuditManager.cs` — `AuditManager` property (`IAuditManager`)
- `InMemoryAuditStore.cs` — default backing store for `AuditManager`
- `FileAuditStore.cs` — JSON-backed audit storage; inject via `IAuditStore`
- `SecurityManager.cs` — `Security` property (`ISecurityManager`)
- `TypeBridgeAdapters.cs` — runtime type adaptation utilities for reflection-free field access

## Key Responsibilities

| Helper | Key behavior |
| --- | --- |
| `DirtyStateManager` | Collect dirty blocks; suggest save or rollback |
| `EventManager` | Subscribe/unsubscribe UoW event streams into block/record/field/error callbacks |
| `LOVManager` | Register LOV definition → load from datasource → cache → filter → validate → auto-populate related fields |
| `ValidationManager` | Fluent rule registration; fires at item/record/block/form levels; wired by FormsManager automatically |
| `TriggerManager` | Registration, ordering, async execution, scope-aware lookup, execution stats |
| `TriggerLibrary` | Common trigger factories — compose into trigger registration rather than write boilerplate |
| `SavepointManager` | Named record snapshots; restore to named point without full rollback |
| `LockManager` | Client-side current-record locking; auto-lock on enter-CRUD; release on commit/rollback |
| `PagingManager` | Tracks page state; does NOT load data itself; FormsManager navigates UoW cursor on page load |
| `AuditManager` | Captures field changes from `OnBlockFieldChanged`; groups into `AuditEntry`; flush to in-memory or file store |
| `SecurityManager` | Pushes effective flags into `DataBlockInfo` and `ItemPropertyManager`; runtime CRUD enforcement |
| `FormRegistry` | Active-form registry for modal/modeless multi-form navigation |
| `FormMessageBus` | Typed pub/sub between running form instances |

## Working Rules
1. Register blocks before relationship creation.
2. Centralize event subscription/unsubscription and remove subscriptions on form close.
3. Dirty-state checks belong in navigation and mode-transition code paths, not scattered across callers.
4. `TriggerLibrary` factories cover ~80% of common trigger scenarios; use them before writing custom trigger lambdas.
5. Security updates both metadata (DataBlockInfo, ItemPropertyManager) and runtime enforcement in CRUD entry points; UI disabling alone is not the enforcement boundary.
6. Audit capture is driven from `OnBlockFieldChanged`; bypassing FormsManager mutation paths also bypasses audit unless compensated by the UoW.
7. `PagingManager` computes page positions; it does not execute datasource queries. Set total-record count after each query.

## Related Skills
- [`forms`](../forms/SKILL.md)
- [`forms-operations-navigation`](../forms-operations-navigation/SKILL.md)
- [`forms-performance-configuration`](../forms-performance-configuration/SKILL.md)

## Detailed Reference
Use [`reference.md`](./reference.md) for helper responsibilities, trigger patterns, LOV flows, and pitfalls.
