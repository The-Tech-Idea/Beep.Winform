# BeepDataBlock → FormsManager Migration — Todo Tracker

**Overall status:** In Progress  
**Started:** (fill in date)  
**Last updated:** (fill in date)

---

## How to use this file

- `[ ]` = not started  
- `[~]` = in progress  
- `[x]` = done  
- Fill in the **Completed** column when you finish a checklist item.

---

## Phase Summary

| Phase | Title | Status | Completed |
|---|---|---|---|
| 01 | BeepDM Contracts | ✅ Done | |
| 02 | Remove IsCoordinated | ✅ Done | |
| 03 | Triggers Delegation | ✅ Done | |
| 04 | Validation Delegation | ✅ Done | |
| 05 | LOV Delegation | 🔲 Not started | |
| 06 | Properties Delegation | 🔲 Not started | |
| 07 | Data Operations Delegation | 🔲 Not started | |
| 08 | SystemVariables + Navigation | 🔲 Not started | |
| 09 | Interface Consolidation | 🔲 Not started | |
| 10 | Examples + Smoke Tests | 🔲 Not started | |
| 11 | WPF + Blazor Adapters (optional) | 🔲 Not started | |

---

## Phase 01 — BeepDM Contracts

Phase doc: [`phases/phase-01-beep-dm-contracts.md`](phases/phase-01-beep-dm-contracts.md)

| # | Task | Done | Completed |
|---|---|---|---|
| 1.1 | Create `IDataBlockNotifier` in `DataManagementEngineStandard/Editor/Forms/Interfaces/` | [x] | |
| 1.2 | Create `IDataBlockController` in same folder | [x] | |
| 1.3 | Create `FieldSelectionInfo` in `DataManagementEngineStandard/Editor/Forms/Models/` | [x] | |
| 1.4 | Create `EditorTemplateInfo` in same folder | [x] | |
| 1.5 | `dotnet build BeepDM.sln` — zero errors | [x] | |

---

## Phase 02 — Remove IsCoordinated

Phase doc: [`phases/phase-02-remove-iscoordinated.md`](phases/phase-02-remove-iscoordinated.md)

| # | Task | Done | Completed |
|---|---|---|---|
| 2.1 | Rewrite `BeepDataBlock.Coordination.cs` — `_formsManager` mandatory | [x] | |
| 2.2 | Add null-guard in `FormsManager` setter (throw `ArgumentNullException`) | [x] | |
| 2.3 | Replace `BeepServiceProvider.Resolve<IFormsManager>()` in constructor/OnLoad | [x] | |
| 2.4 | Delete `IsCoordinated` property | [x] | |
| 2.5 | Grep and remove every `if (IsCoordinated)` / `if (!IsCoordinated)` guard | [x] | |
| 2.6 | `dotnet build WinFormsApp.sln` — zero errors | [x] | |

---

## Phase 03 — Triggers Delegation

Phase doc: [`phases/phase-03-triggers-delegation.md`](phases/phase-03-triggers-delegation.md)

| # | Task | Done | Completed |
|---|---|---|---|
| 3.1 | Rewrite `BeepDataBlock.Triggers.cs` — 8 one-liner delegates | [x] | |
| 3.2 | Delete `_triggers`, `_namedTriggers`, `_suppressTriggers` fields | [x] | |
| 3.3 | Delete `RegisterTriggerInternal` and `TriggerBridge` | [x] | |
| 3.4 | Update callers — `TriggerBridge.*` → FormsManager types | [x] | |
| 3.5 | Delete `Models/BeepDataBlockTrigger.cs` | [x] | |
| 3.6 | Delete `Models/TriggerContext.cs` | [x] | |
| 3.7 | Delete `Models/TriggerEnums.cs` | [x] | |
| 3.8 | Delete `Helpers/BeepDataBlockTriggerHelper.cs` | [x] | |
| 3.9 | Grep: `TriggerBridge\|_namedTriggers\|_suppressTriggers` — zero hits | [x] | |
| 3.10 | `dotnet build WinFormsApp.sln` — zero errors | [x] | |

---

## Phase 04 — Validation Delegation

Phase doc: [`phases/phase-04-validation-delegation.md`](phases/phase-04-validation-delegation.md)

| # | Task | Done | Completed |
|---|---|---|---|
| 4.1 | Rewrite `BeepDataBlock.Validation.cs` — 5 one-liner delegates | [x] | |
| 4.2 | Move `GetCurrentRecordValues()` to `BeepDataBlock.cs` or `BeepDataBlock.Helpers.cs` | [ ] | |
| 4.3 | Delete `_validationRules` field | [x] | |
| 4.4 | Delete `ValidationBridge` usages | [x] | |
| 4.5 | Update callers — `ValidationRuleHelpers.*` → `ValidationRuleLibrary.*` | [ ] | |
| 4.6 | Delete `Models/ValidationRule.cs` (WinForms copy) | [ ] | |
| 4.7 | Delete `Helpers/ValidationRuleHelpers.cs` | [ ] | |
| 4.8 | Grep: `ValidationBridge\|_validationRules\|ValidationRule\.cs` — zero hits | [ ] | |
| 4.9 | `dotnet build WinFormsApp.sln` — zero errors | [ ] | |

---

## Phase 05 — LOV Delegation

Phase doc: [`phases/phase-05-lov-delegation.md`](phases/phase-05-lov-delegation.md)

| # | Task | Done | Completed |
|---|---|---|---|
| 5.1 | Rewrite `BeepDataBlock.LOV.cs` — delegates + WinForms UI wiring | [ ] | |
| 5.2 | Update `BeepDataBlock.Integration.cs` LOV section | [ ] | |
| 5.3 | Update `BeepLOVDialog` constructor — `BeepDataBlockLOV` → `LOVDefinition` | [ ] | |
| 5.4 | Remove `LOVBridge` usages | [ ] | |
| 5.5 | Delete `_lovs` field | [ ] | |
| 5.6 | Delete `Models/BeepDataBlockLOV.cs` | [ ] | |
| 5.7 | Delete `Helpers/DataBlockQueryHelper.cs` | [ ] | |
| 5.8 | Grep: `LOVBridge\|BeepDataBlockLOV\|_lovs\[` — zero hits | [ ] | |
| 5.9 | `dotnet build WinFormsApp.sln` — zero errors | [ ] | |

---

## Phase 06 — Properties Delegation

Phase doc: [`phases/phase-06-properties-delegation.md`](phases/phase-06-properties-delegation.md)

| # | Task | Done | Completed |
|---|---|---|---|
| 6.1 | Rewrite `BeepDataBlock.Properties.cs` — delegates + `UIComponents` wiring | [ ] | |
| 6.2 | Delete `_items` field | [ ] | |
| 6.3 | Delete `_blockProperties` field — replace with `SyncBlockInfoToFormsManager()` | [ ] | |
| 6.4 | Slim `BeepDataBlockItem` — extend `ItemInfo` | [ ] | |
| 6.5 | Delete `Helpers/BeepDataBlockPropertyHelper.cs` | [ ] | |
| 6.6 | Grep: `BeepDataBlockPropertyHelper\|_items\[\|_blockProperties` — zero hits | [ ] | |
| 6.7 | `dotnet build WinFormsApp.sln` — zero errors | [ ] | |

---

## Phase 07 — Data Operations Delegation

Phase doc: [`phases/phase-07-data-operations-delegation.md`](phases/phase-07-data-operations-delegation.md)

| # | Task | Done | Completed |
|---|---|---|---|
| 7.1 | Rewrite `BeepDataBlock.UnitOfWork.cs` — pure delegation | [ ] | |
| 7.2 | Remove `OnPreQuery` / `OnPostQuery` event declarations | [ ] | |
| 7.3 | Remove child-block refresh loops | [ ] | |
| 7.4 | Update callers — subscribe to FormsManager triggers instead | [ ] | |
| 7.5 | Delete `Helpers/DataBlockUnitOfWorkHelper.cs` | [ ] | |
| 7.6 | Grep: `DataBlockUnitOfWorkHelper\|OnPreQuery\|OnPostQuery` — zero hits | [ ] | |
| 7.7 | `dotnet build WinFormsApp.sln` — zero errors | [ ] | |

---

## Phase 08 — SystemVariables + Navigation

Phase doc: [`phases/phase-08-systemvariables-navigation.md`](phases/phase-08-systemvariables-navigation.md)

| # | Task | Done | Completed |
|---|---|---|---|
| 8.1 | Rewrite `BeepDataBlock.SystemVariables.cs` — one-line property accessor | [ ] | |
| 8.2 | Rewrite navigation methods in `BeepDataBlock.Navigation.cs` | [ ] | |
| 8.3 | Remove local `SystemVariables` field from `BeepDataBlock.cs` | [ ] | |
| 8.4 | Delete `Models/SystemVariables.cs` (WinForms version) | [ ] | |
| 8.5 | Grep: `new SystemVariables\|_systemVariables` — zero hits | [ ] | |
| 8.6 | `dotnet build WinFormsApp.sln` — zero errors | [ ] | |

---

## Phase 09 — Interface Consolidation

Phase doc: [`phases/phase-09-interface-consolidation.md`](phases/phase-09-interface-consolidation.md)

| # | Task | Done | Completed |
|---|---|---|---|
| 9.1 | Update `IBeepDataBlock` — extend `IDataBlockController`, remove duplicates | [ ] | |
| 9.2 | Update `IBeepDataBlockNotifier` — extend `IDataBlockNotifier` | [ ] | |
| 9.3 | Slim `BeepDataBlockFieldSelection` — inherit `FieldSelectionInfo` | [ ] | |
| 9.4 | Slim `BeepDataBlockEditorTemplate` — inherit `EditorTemplateInfo` | [ ] | |
| 9.5 | Add `IBeepDataBlockNotifier` to `MessageBoxBeepDataBlockNotifier` implements clause | [ ] | |
| 9.6 | Grep BeepDM source for `IBeepDataBlock` — replace with `IDataBlockController` | [ ] | |
| 9.7 | `dotnet build BeepDM.sln` — zero errors | [ ] | |
| 9.8 | `dotnet build WinFormsApp.sln` — zero errors | [ ] | |

---

## Phase 10 — Examples + Smoke Tests

Phase doc: [`phases/phase-10-examples-and-docs.md`](phases/phase-10-examples-and-docs.md)

| # | Task | Done | Completed |
|---|---|---|---|
| 10.1 | Update all `Beep.Sample.*` example projects — remove bridge classes | [ ] | |
| 10.2 | Add full `OrdersFormController` example | [ ] | |
| 10.3 | Add master-detail relationship example | [ ] | |
| 10.4 | Smoke test A — Block Setup | [ ] | |
| 10.5 | Smoke test B — LOV | [ ] | |
| 10.6 | Smoke test C — Validation | [ ] | |
| 10.7 | Smoke test D — Triggers | [ ] | |
| 10.8 | Smoke test E — Navigation | [ ] | |
| 10.9 | Smoke test F — Master-Detail | [ ] | |
| 10.10 | Smoke test G — Data Operations | [ ] | |
| 10.11 | Smoke test H — System Variables | [ ] | |
| 10.12 | `dotnet build Beep.Sample.sln` — zero errors | [ ] | |

---

## Phase 11 — WPF + Blazor Adapters (optional)

Phase doc: [`phases/phase-11-adapters-optional.md`](phases/phase-11-adapters-optional.md)

| # | Task | Done | Completed |
|---|---|---|---|
| 11.1 | Create `TheTechIdea.Beep.WPF.Controls.Integrated` project | [ ] | |
| 11.2 | Implement `BeepWpfDataBlock : UserControl, IDataBlockController` | [ ] | |
| 11.3 | Create `TheTechIdea.Beep.Blazor.Integrated` project | [ ] | |
| 11.4 | Implement `BeepBlazorDataBlock : ComponentBase, IDataBlockController` | [ ] | |
| 11.5 | Register both in a spike test with a shared `FormsManager` | [ ] | |
| 11.6 | `dotnet build` both projects — zero errors | [ ] | |

---

## Deleted Files Log

Record each file deletion here for audit trail.

| File | Deleted in Phase | Date Deleted |
|---|---|---|
| `Models/BeepDataBlockTrigger.cs` | 03 | |
| `Models/TriggerContext.cs` | 03 | |
| `Models/TriggerEnums.cs` | 03 | |
| `Helpers/BeepDataBlockTriggerHelper.cs` | 03 | |
| `Models/ValidationRule.cs` | 04 | |
| `Helpers/ValidationRuleHelpers.cs` | 04 | |
| `Models/BeepDataBlockLOV.cs` | 05 | |
| `Helpers/DataBlockQueryHelper.cs` | 05 | |
| `Helpers/BeepDataBlockPropertyHelper.cs` | 06 | |
| `Helpers/DataBlockUnitOfWorkHelper.cs` | 07 | |
| `Models/SystemVariables.cs` (WinForms copy) | 08 | |
