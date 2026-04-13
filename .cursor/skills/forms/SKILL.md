---
name: forms
description: Entry-point guidance for FormsManager orchestration in BeepDM. Use when implementing Oracle Forms style behavior with block registration, master-detail coordination, mode transitions, navigation, CRUD, triggers, LOV, validation, audit, security, paging, multi-form communication, and performance/configuration.
---

# Forms Manager Guide

Use this skill as the top-level entry point for `FormsManager` orchestration.
All nine enhancement phases (Core → Oracle Built-ins → Multi-Form → Triggers → Audit → Security → Performance → Testing → Docs) are implemented and complete.

## Use this skill when
- Wiring a new block-based form over `UnitofWork` instances
- Coordinating master-detail behavior, form lifecycle, and navigation
- Deciding which partial class or helper owns a specific Oracle Forms behavior
- Understanding primary-key strategy rules across create/insert/commit flows

## Do not use this skill when
- The task is only about direct `UnitofWork` behavior outside forms orchestration. Use [`unitofwork`](../unitofwork/SKILL.md).
- The task is only about import/sync/ETL pipelines. Use [`importing`](../importing/SKILL.md), [`beepsync`](../beepsync/SKILL.md), or [`etl`](../etl/SKILL.md).

## Architecture — Partial-Class Surface

`FormsManager` is the coordinator class in `TheTechIdea.Beep.Editor.UOWManager`. Responsibilities are split across 18 partial files:

| File | Responsibility |
| --- | --- |
| `FormsManager.cs` | Core registration, DI wiring, fields, master/detail ownership |
| `FormsManager.Core.cs` | Shared internal utilities and initialization helpers |
| `FormsManager.Properties.cs` | Public properties surface |
| `FormsManager.Helpers.cs` | Internal helper-delegation façades |
| `FormsManager.BlockRegistration.cs` | Block register/unregister, block index |
| `FormsManager.Relationships.cs` | Master/detail registration and synchronization |
| `FormsManager.FormOperations.cs` | Open/close/commit/rollback/clear-form |
| `FormsManager.Navigation.cs` | Record navigation, block switching, navigation history |
| `FormsManager.ModeTransitions.cs` | `ENTER_QUERY` / `EXECUTE_QUERY` / CRUD transition rules |
| `FormsManager.EnhancedOperations.cs` | Insert, update, delete, duplicate, audit defaults |
| `FormsManager.BasicDataOps.cs` | Thin synchronous wrappers over common CRUD paths |
| `FormsManager.DataOperations.cs` | Undo/redo, batch commit, export/import, aggregates |
| `FormsManager.GenericOperations.cs` | Typed `RegisterBlock<T>`, `GetBlock<T>`, `ShowLOVAsync` |
| `FormsManager.BlockProperties.cs` | `SET_BLOCK_PROPERTY` / `GET_BLOCK_PROPERTY` equivalents |
| `FormsManager.Alerts.cs` | `MESSAGE`, `SHOW_ALERT`, `BELL`, confirmation helpers |
| `FormsManager.Sequences.cs` | Sequence, default-value, copy-field helpers |
| `FormsManager.Timers.cs` | `CREATE_TIMER`, `DELETE_TIMER`, `GET_TIMER` |
| `FormsManager.MultiFormNavigation.cs` | `CALL_FORM`, modeless `OPEN_FORM`, `NEW_FORM`, return-to-caller |
| `FormsManager.InterFormComm.cs` | `:GLOBAL.*`, message bus, shared blocks |
| `FormsManager.Security.cs` | Security context, block/field security, masking, violation log |
| `FormsManager.Audit.cs` | Audit capture, query, export, purge |
| `FormsManager.Performance.cs` | Paging, lazy loading, cache invalidation, fetch-ahead |
| `FormsManager.KeyTriggers.cs` | `KEY-*` trigger wrappers and default keyboard actions |
| `FormsManager.DmlTriggers.cs` | `ON-INSERT` / `ON-UPDATE` / `ON-DELETE` and `RAISE_FORM_TRIGGER` |
| `FormsManager.TriggerChaining.cs` | Trigger execution graph and dependency logging |

## Architecture — Helper Manager Properties

FormsManager exposes helper managers as properties. Access through `FormsManager` rather than constructing helpers directly.

**Core orchestration**
- `DirtyStateManager` — unsaved change tracking
- `EventManager` — UoW event subscription routing
- `BlockFactory` — create UoW + EntityStructure from connection
- `ErrorLog` (`IBlockErrorLog`) — per-block error log

**Relationship and data shaping**
- `LOV` (`ILOVManager`) — register, load, cache, validate, cascade, auto-complete
- `QueryBuilder` (`IQueryBuilderManager`) — reusable filter/query definitions
- `Paging` (`IPagingManager`) — page size, current page, total-record count, fetch-ahead

**Validation, triggers, and interaction**
- `Validation` (`IValidationManager`) — item/record/block/form rules, fluent registration
- `Triggers` (`ITriggerManager`) — registration, ordering, async execution, statistics
- `ItemProperties` (`IItemPropertyManager`) — enabled, visible, read-only, tab-order
- `Messages` (`IMessageQueueManager`) — status message queue for UI consumers
- `Timers` (`ITimerManager`) — named timers → `WHEN-TIMER-EXPIRED`

**Transactional state**
- `Savepoints` (`ISavepointManager`) — named snapshots and rollback targets
- `Locking` (`ILockManager`) — client-side record locking and auto-lock
- `CrossBlockValidation` (`CrossBlockValidationManager`) — commit-time multi-block rules

**Cross-form and platform**
- `Registry` (`IFormRegistry`) — active-form registry for multi-form navigation
- `MessageBus` (`IFormMessageBus`) — pub/sub and broadcast form messages
- `SharedBlocks` (`ISharedBlockManager`) — shared UoW-backed blocks across form instances
- `AlertProvider` (`IAlertProvider`) — `ShowAlertAsync` UI injection
- `Sequences` (`ISequenceProvider`) — Oracle-style in-memory sequences

**Audit and security**
- `AuditManager` (`IAuditManager`) — field-level change capture from `OnBlockFieldChanged`
- `Security` (`ISecurityManager`) — block/field security, masking, CRUD enforcement

**Shared simulation**
- `SystemVariables` (`ISystemVariablesManager`) — `:SYSTEM.*` emulation
- `BlockProperties` (`IBlockPropertyManager`) — `SET/GET_BLOCK_PROPERTY`

## File Locations
- `DataManagementEngineStandard/Editor/Forms/FormsManager*.cs` (25 partials)
- `DataManagementEngineStandard/Editor/Forms/Helpers/` (25 helper implementations)
- `DataManagementEngineStandard/Editor/Forms/Configuration/`
- `DataManagementEngineStandard/Editor/Forms/Models/`
- `DataManagementEngineStandard/Editor/Forms/Interfaces/`

## Primary-Key Handling Rules
1. If the caller or trigger already supplied a valid key, preserve it.
2. If the field is datasource-managed identity/auto-increment, leave it unset and refresh after insert/commit.
3. If a real sequence exists, prefer `IUnitofWork.GetSeq(...)` before the in-memory `ISequenceProvider`.
4. Use `ISequenceProvider` for Oracle-style built-ins, deterministic tests, or non-database-backed scenarios.
5. For GUID or custom keys, use item defaults or triggers explicitly — do not guess.
6. Never auto-number composite keys blindly.
7. Never consume sequence values during query, paging, navigation, or cache prefetch.

## Fast Workflow
1. Create `FormsManager(editor)`.
2. Register each block with `RegisterBlock<T>(...)` (typed) or `RegisterBlock(...)` (untyped).
3. Create relationships with `CreateMasterDetailRelation(...)`.
4. Set security context and audit user if needed.
5. Open the form and enter the right mode for the target block.
6. Use form/navigation/CRUD APIs through `FormsManager`, not direct ad-hoc block mutations.
7. Commit or roll back through form-level APIs.

## Specialized Skills
- [`forms-mode-transitions`](../forms-mode-transitions/SKILL.md) — ENTER_QUERY, EXECUTE_QUERY, triggers, validation
- [`forms-operations-navigation`](../forms-operations-navigation/SKILL.md) — lifecycle, record navigation, multi-form
- [`forms-enhanced-data-operations`](../forms-enhanced-data-operations/SKILL.md) — CRUD, LOV, sequences, undo/redo, export
- [`forms-helper-managers`](../forms-helper-managers/SKILL.md) — all 25 helper managers
- [`forms-performance-configuration`](../forms-performance-configuration/SKILL.md) — paging, audit, security, config

## Detailed Reference
Use [`reference.md`](./reference.md) for scenarios and examples.
