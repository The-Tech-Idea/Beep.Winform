# BeepDataBlock → FormsManager Migration Plan
**Goal:** `BeepDataBlock` becomes a pure WinForms UI front-end. Every piece of business logic lives in `IFormsManager` (BeepDM). `BeepDataBlock` methods are thin one-liners that call `_formsManager.*`.

**Architecture target:**
```
BeepDataBlock (WinForms)
  ├── UIComponents dict        ← only UI references stay here
  ├── FieldSelections / Components ← designer-time config
  ├── WinForms event handlers  ← translate WinForms events → FormsManager calls
  └── _formsManager            ← ALL logic lives here, ALWAYS required

IFormsManager (BeepDM — cross-platform)
  ├── Triggers.RegisterTrigger(blockName, ...)
  ├── Validation.RegisterRule(blockName, ...)
  ├── LOV.RegisterLOV(blockName, ...)
  ├── Items.SetItemProperty(blockName, ...)
  ├── QueryBuilder.BuildFilters(blockName, ...)
  ├── ExecuteQueryAsync(blockName, ...)
  ├── SystemVariables.GetSystemVariables(blockName)
  └── Navigation.NextRecord(blockName) ...
```

**The problem today:** BeepDataBlock owns private dictionaries (`_triggers`, `_validationRules`, `_lovs`, `_items`) as its primary state and only copies to FormsManager when `IsCoordinated == true`. That is backwards. FormsManager must be the single source of truth; the `IsCoordinated` conditional must be eliminated entirely.

**Approach:** No legacy shims, no `[Obsolete]` markers, no compatibility bridges. Each phase rewrites a partial file so every method becomes a delegate call, removes the local dictionary field, and deletes any now-unused model/helper file. The compiler confirms correctness after each phase.

---

## What stays in BeepDataBlock (WinForms only)
| Responsibility | Why it stays |
|---|---|
| `UIComponents` dict (`string → IBeepUIComponent`) | WinForms control references — no cross-platform equivalent |
| `FieldSelections` / `Components` BindingList | Designer-time configuration serialized to .resx |
| WinForms event handlers (KeyDown, LostFocus, etc.) | Translate WinForms events into FormsManager calls |
| Data binding wiring (`Binding`, `BindingSource`) | WinForms-specific plumbing |
| Layout / view mode (`ViewMode`, `_gridView`) | Pure visual concern |
| `Notifier` (MessageBox wrapper) | Implements `IDataBlockNotifier` from BeepDM, WinForms impl |
| `ConnectionName` / `EntityName` designer properties | Designer-time config |

## What moves to / stays in FormsManager
| Responsibility | FormsManager member |
|---|---|
| Trigger registration & execution | `IFormsManager.Triggers` |
| Validation rules & execution | `IFormsManager.Validation` |
| LOV registration & execution | `IFormsManager.LOV` |
| Item property get/set | `IFormsManager.Items` |
| Query building | `IFormsManager.QueryBuilder` |
| Data operations (query/insert/update/delete) | `IFormsManager.ExecuteQueryAsync(...)` etc. |
| System variables `:SYSTEM.*` | `IFormsManager.SystemVariables` |
| Navigation (next/prev record/item) | `IFormsManager.Navigation` |
| Savepoints | `IFormsManager.Savepoints` |
| Dirty state tracking | `IFormsManager.DirtyState` |
| Master-detail coordination | `IFormsManager.Relationships` |
| Performance / caching | `IFormsManager.Performance` |

---

## Current State Summary

### Already in FormsManager ✅ (BeepDM has these)
| BeepDataBlock local state | FormsManager equivalent |
|---|---|
| `_validationRules` dict | `IFormsManager.Validation` (`ValidationManager`) |
| `_triggers` / `_namedTriggers` dicts | `IFormsManager.Triggers` (`TriggerManager`) |
| `_lovs` dict | `IFormsManager.LOV` (`LOVManager`) |
| `_items` dict (`BeepDataBlockItem`) | `IFormsManager.Items` (`ItemPropertyManager`) |
| `_blockProperties` | `DataBlockInfo` registered in FormsManager |
| `QueryOperator` / filter building | `IFormsManager.QueryBuilder` (`QueryBuilderManager`) |
| `DataBlockUnitOfWorkHelper` calls | `IFormsManager.ExecuteQueryAsync / InsertRecordAsync` etc. |
| `SystemVariables` object | `IFormsManager.SystemVariables` (`SystemVariablesManager`) |

### Pending — BeepDataBlock local state that must become delegate calls ❌
| Local field in BeepDataBlock | Action |
|---|---|
| `private Dictionary<TriggerType, List<BeepDataBlockTrigger>> _triggers` | Remove; `RegisterTrigger()` → `_formsManager.Triggers.RegisterTrigger(Name, ...)` |
| `private Dictionary<string, BeepDataBlockTrigger> _namedTriggers` | Remove with above |
| `private bool _suppressTriggers` | Remove; use `_formsManager.Triggers.Suspend(Name)` |
| `private Dictionary<string, List<ValidationRule>> _validationRules` | Remove; `RegisterValidationRule()` → `_formsManager.Validation.RegisterRule(...)` |
| `private Dictionary<string, BeepDataBlockLOV> _lovs` | Remove; `RegisterLOV()` → `_formsManager.LOV.RegisterLOV(Name, ...)` |
| `private Dictionary<string, BeepDataBlockItem> _items` | Remove; `RegisterItem()` → `_formsManager.Items.RegisterItem(Name, ...)` |
| `private BeepDataBlockProperties _blockProperties` | Remove; properties live in `DataBlockInfo` in FormsManager |
| `IsCoordinated` conditional pattern | Remove entirely — FormsManager is always required |

### Duplicate files to delete after delegation is complete
| File | Replaced by |
|---|---|
| `DataBlocks/Helpers/ValidationRuleHelpers.cs` | `ValidationRuleLibrary` in BeepDM |
| `DataBlocks/Helpers/BeepDataBlockPropertyHelper.cs` | `IFormsManager.Items` |
| `DataBlocks/Helpers/BeepDataBlockTriggerHelper.cs` | `IFormsManager.Triggers` |
| `DataBlocks/Helpers/DataBlockQueryHelper.cs` | `IFormsManager.QueryBuilder` |
| `DataBlocks/Helpers/DataBlockUnitOfWorkHelper.cs` | `IFormsManager` data operations |
| `DataBlocks/Models/TriggerEnums.cs` | BeepDM `TriggerEnums` |
| `DataBlocks/Models/TriggerContext.cs` | BeepDM `TriggerContext` |
| `DataBlocks/Models/BeepDataBlockTrigger.cs` | BeepDM `TriggerDefinition` |
| `DataBlocks/Models/BeepDataBlockLOV.cs` | BeepDM `LOVDefinition` |
| `DataBlocks/Models/SystemVariables.cs` | BeepDM `SystemVariablesManager` |
| `DataBlocks/Models/ValidationRule.cs` | Already empty — delete |

---

---

## Phase Documents (detailed implementation)

Each phase has a standalone document with full clean-code partial-class rewrites, file-delete checklists, caller update guides, and per-phase build checks.

| Phase | Document | Status |
|---|---|---|
| 01 | [BeepDM Contracts](phases/phase-01-beep-dm-contracts.md) | ✅ Done |
| 02 | [Remove IsCoordinated](phases/phase-02-remove-iscoordinated.md) | ✅ Done |
| 03 | [Triggers Delegation](phases/phase-03-triggers-delegation.md) | ✅ Done |
| 04 | [Validation Delegation](phases/phase-04-validation-delegation.md) | ✅ Done |
| 05 | [LOV Delegation](phases/phase-05-lov-delegation.md) | 🔲 Not started |
| 06 | [Properties Delegation](phases/phase-06-properties-delegation.md) | 🔲 Not started |
| 07 | [Data Operations Delegation](phases/phase-07-data-operations-delegation.md) | 🔲 Not started |
| 08 | [SystemVariables + Navigation](phases/phase-08-systemvariables-navigation.md) | 🔲 Not started |
| 09 | [Interface Consolidation](phases/phase-09-interface-consolidation.md) | 🔲 Not started |
| 10 | [Examples + Smoke Tests](phases/phase-10-examples-and-docs.md) | 🔲 Not started |
| 11 | [WPF + Blazor Adapters (optional)](phases/phase-11-adapters-optional.md) | 🔲 Not started |

**Progress tracker:** [`todo-tracker.md`](todo-tracker.md)

---

> ⬇️ The sections below are the original inline phase summaries kept for reference.
> The canonical implementation details are in the phase documents above.

---

## Phase 1 — Add Cross-Platform Contracts to BeepDM
**Scope:** BeepDM only — all additions. Zero WinForms changes.  
**Why first:** WinForms phases depend on these types existing in BeepDM.  
**Full details:** [phases/phase-01-beep-dm-contracts.md](phases/phase-01-beep-dm-contracts.md)

### 1.1 — `IDataBlockNotifier`
**New file:** `BeepDM/.../Editor/Forms/Interfaces/IDataBlockNotifier.cs`
```csharp
public interface IDataBlockNotifier
{
    void ShowInfo(string message, string caption = "Information");
    void ShowWarning(string message, string caption = "Warning");
    void ShowError(string message, string caption = "Error");
}
```

### 1.2 — `IDataBlockController`
**New file:** `BeepDM/.../Editor/Forms/Interfaces/IDataBlockController.cs`  
Platform-neutral surface from `IBeepDataBlock` — all `IBeepUIComponent` refs replaced by `string componentId`:
```csharp
public interface IDataBlockController
{
    string Name { get; set; }
    DataBlockMode BlockMode { get; set; }
    IUnitofWork Data { get; set; }
    IEntityStructure EntityStructure { get; }
    List<EntityField> Fields { get; }
    string MasterKeyPropertyName { get; set; }
    string ForeignKeyPropertyName { get; set; }
    bool IsInQueryMode { get; }
    void SwitchBlockMode(DataBlockMode newMode);
    void HandleDataChanges();
}
```

### 1.3 — `FieldSelectionInfo`
**New file:** `BeepDM/.../Editor/Forms/Models/FieldSelectionInfo.cs`  
Pure data copy of `BeepDataBlockFieldSelection` — no `[TypeConverter]`, no `[JsonIgnore]`, no WinForms namespaces.

### 1.4 — `EditorTemplateInfo`
**New file:** `BeepDM/.../Editor/Forms/Models/EditorTemplateInfo.cs`  
Pure data copy of `BeepDataBlockEditorTemplate` — same pattern.

**Build check:** `dotnet build BeepDM.sln` — zero errors.

---

## Phase 2 — Make `_formsManager` Mandatory; Remove `IsCoordinated`
**Scope:** `BeepDataBlock.Coordination.cs` and `BeepDataBlock.cs`.  
**Why:** `IsCoordinated` is the root cause of duplicate state — it allows BeepDataBlock to operate without FormsManager. That must be ended.

### 2.1 — `_formsManager` is always set
- Remove the `IsCoordinated` property.
- Change `FormsManager` setter: assigning `null` throws `ArgumentNullException`.
- Remove the `_isRegisteredWithFormsManager` field.
- Constructor or `OnLoad`: resolve `_formsManager` from `BeepServiceProvider` if not injected.
- `RegisterWithFormsManager()` always runs at init — no guard.

### 2.2 — Remove `IsCoordinated` guards from all partials
- Replace every `if (IsCoordinated) { _formsManager.X(...) }` with a direct `_formsManager.X(...)` call.
- Remove the fallback local-state paths that existed for the non-coordinated case.

**Build check:** `dotnet build WinFormsApp.sln` — zero errors.

---

## Phase 3 — Delegate `BeepDataBlock.Triggers.*` to FormsManager
**Scope:** `BeepDataBlock.Triggers.cs`

### What changes
- Remove `private Dictionary<TriggerType, List<BeepDataBlockTrigger>> _triggers`.
- Remove `private Dictionary<string, BeepDataBlockTrigger> _namedTriggers`.
- Remove `private bool _suppressTriggers`.
- Rewrite every public method as a one-line delegate:
  ```csharp
  public void RegisterTrigger(TriggerType type, Func<TriggerContext, Task<bool>> handler, int order = 0)
      => _formsManager.Triggers.RegisterTrigger(Name, (TriggerType)type, handler, order);

  public void SuspendTriggers() => _formsManager.Triggers.Suspend(Name);
  public void ResumeTriggers()  => _formsManager.Triggers.Resume(Name);
  public List<TriggerDefinition> GetAllTriggers() => _formsManager.Triggers.GetBlockTriggers(Name);
  ```
- Internal `ExecuteTrigger(type, context)` calls → `await _formsManager.Triggers.ExecuteAsync(Name, type, context)`.
- Remove `BeepDataBlockTrigger`, `TriggerContext`, `TriggerEnums` imports; add BeepDM equivalents.

### Delete
- `DataBlocks/Models/BeepDataBlockTrigger.cs`
- `DataBlocks/Models/TriggerContext.cs`
- `DataBlocks/Models/TriggerEnums.cs`
- `DataBlocks/Helpers/BeepDataBlockTriggerHelper.cs`

**Build check:** `dotnet build WinFormsApp.sln` — zero errors.

---

## Phase 4 — Delegate `BeepDataBlock.Validation.*` to FormsManager
**Scope:** `BeepDataBlock.Validation.cs`

### What changes
- Remove `private Dictionary<string, List<ValidationRule>> _validationRules`.
- Rewrite every public method:
  ```csharp
  public void RegisterValidationRule(string fieldName, ValidationRule rule)
      => _formsManager.Validation.RegisterRule(Name, fieldName, rule);

  public async Task<IErrorsInfo> ValidateField(string fieldName, object value)
      => await _formsManager.Validation.ValidateItemAsync(Name, fieldName, value);

  public async Task<IErrorsInfo> ValidateRecord()
      => await _formsManager.Validation.ValidateRecordAsync(Name, GetCurrentRecordValues());
  ```
- Remove `ValidationBridge` adapter — no longer needed once FormsManager owns the rules.

### Delete
- `DataBlocks/Models/ValidationRule.cs` (already empty)
- `DataBlocks/Helpers/ValidationRuleHelpers.cs`

**Build check:** `dotnet build WinFormsApp.sln` — zero errors.

---

## Phase 5 — Delegate `BeepDataBlock.LOV.*` to FormsManager
**Scope:** `BeepDataBlock.LOV.cs`, `BeepDataBlock.Integration.cs` (LOV section)

### What changes
- Remove `private Dictionary<string, BeepDataBlockLOV> _lovs`.
- Rewrite every public method:
  ```csharp
  public void RegisterLOV(string itemName, LOVDefinition lov)
      => _formsManager.LOV.RegisterLOV(Name, itemName, lov);

  public async Task<bool> ShowLOV(string itemName)
      => await _formsManager.LOV.ShowLOVAsync(Name, itemName, _notifier);
  ```
- `BeepLOVDialog` constructor: change parameter from `BeepDataBlockLOV` to `LOVDefinition`.
- Remove `LOVBridge` adapter.

### Delete
- `DataBlocks/Models/BeepDataBlockLOV.cs`
- `DataBlocks/Helpers/DataBlockQueryHelper.cs` (query operator enum now from BeepDM)

**Build check:** `dotnet build WinFormsApp.sln` — zero errors.

---

## Phase 6 — Delegate `BeepDataBlock.Properties.*` to FormsManager
**Scope:** `BeepDataBlock.Properties.cs`

### What changes
- Remove `private Dictionary<string, BeepDataBlockItem> _items`.
- Remove `private BeepDataBlockProperties _blockProperties`.
- Rewrite every public method:
  ```csharp
  public void RegisterItem(string itemName, IBeepUIComponent component)
  {
      _formsManager.Items.RegisterItem(Name, itemName, new ItemInfo { ... });
      UIComponents[itemName] = component;   // UI ref stays local
  }

  public void SetItemProperty(string itemName, string prop, object value)
      => _formsManager.Items.SetItemProperty(Name, itemName, prop, value);

  public object GetItemProperty(string itemName, string prop)
      => _formsManager.Items.GetItemProperty(Name, itemName, prop);
  ```
- `BeepDataBlockItem` stays in WinForms only as a thin wrapper that adds `IBeepUIComponent Component` over `ItemInfo`.

### Delete
- `DataBlocks/Helpers/BeepDataBlockPropertyHelper.cs`

**Build check:** `dotnet build WinFormsApp.sln` — zero errors.

---

## Phase 7 — Delegate Data Operations to FormsManager
**Scope:** `BeepDataBlock.UnitOfWork.cs`, `BeepDataBlock.DataOperations.cs`

### What changes
- Remove direct `DataBlockUnitOfWorkHelper.*` calls.
- Rewrite data-op methods:
  ```csharp
  public async Task<bool> ExecuteQueryAsync(List<AppFilter> filters = null)
      => await _formsManager.ExecuteQueryAsync(Name, filters);

  public async Task<bool> InsertRecordAsync()
      => await _formsManager.InsertRecordAsync(Name);

  public async Task<bool> SaveAsync()
      => await _formsManager.SaveAsync(Name);

  public async Task<bool> DeleteCurrentRecordAsync()
      => await _formsManager.DeleteCurrentRecordAsync(Name);
  ```
- Remove the "coordinated vs. local" branching in `ExecuteQueryWithUnitOfWorkAsync`.
- Remove `OnPreQuery` / `OnPostQuery` local events — these are now FormsManager trigger callbacks (`TriggerType.PreQuery`, `TriggerType.PostQuery`).

### Delete
- `DataBlocks/Helpers/DataBlockUnitOfWorkHelper.cs`

**Build check:** `dotnet build WinFormsApp.sln` — zero errors.

---

## Phase 8 — Delegate System Variables and Navigation to FormsManager
**Scope:** `BeepDataBlock.SystemVariables.cs`, `BeepDataBlock.Navigation.cs`

### What changes
- `SYSTEM` property: `public SystemVariables SYSTEM => _formsManager.SystemVariables.GetSystemVariables(Name);`
- Remove local `SystemVariables` object field.
- Navigation methods delegate:
  ```csharp
  public bool NextRecord() => _formsManager.Navigation.NextRecord(Name);
  public bool PreviousRecord() => _formsManager.Navigation.PreviousRecord(Name);
  public bool NextItem() => _formsManager.Navigation.NextItem(Name);
  public bool PreviousItem() => _formsManager.Navigation.PreviousItem(Name);
  ```
- Tab order, navigable items list → `_formsManager.Items.GetTabOrder(Name)`.

### Delete
- `DataBlocks/Models/SystemVariables.cs` (WinForms version)

**Build check:** `dotnet build WinFormsApp.sln` — zero errors.

---

## Phase 9 — Interface Consolidation
**Scope:** `IBeepDataBlock`, `IBeepDataBlockNotifier`, `BeepDataBlock.cs` wiring.

### 9.1 — `IBeepDataBlock` extends `IDataBlockController`
- `IDataBlockController` (Phase 1.2) is the cross-platform surface.
- `IBeepDataBlock` extends it and adds only WinForms-specific members: `UIComponents`, `SetMasterRecord`, `RemoveChildBlock`.
- Any FormsManager code that needed `IBeepDataBlock` switches to `IDataBlockController`.

### 9.2 — `IBeepDataBlockNotifier` extends `IDataBlockNotifier`
- `IBeepDataBlockNotifier` inherits `IDataBlockNotifier` (Phase 1.1).
- `MessageBoxBeepDataBlockNotifier` stays in WinForms — no body changes.

### 9.3 — Slim `BeepDataBlockEditorTemplate` and `BeepDataBlockFieldSelection`
- Inherit `EditorTemplateInfo` / `FieldSelectionInfo` from BeepDM (Phases 1.3–1.4).
- Remove duplicated data properties; keep only WinForms attribute decorators and the `AppDomain` computed properties.

**Build check:** `dotnet build WinFormsApp.sln` + `dotnet build BeepDM.sln` — zero errors.

---

## Phase 10 — Examples and Documentation
**Scope:** `Examples/` folder and doc files — no production code changes.

### 10.1 — Update Examples
| File | Change |
|---|---|
| `OracleFormsValidationExamples.cs` | `ValidationRuleHelpers.*` → `ValidationRuleLibrary.*`; rules registered via `block.RegisterValidationRule(...)` which now delegates |
| `OracleFormsTriggerExamples.cs` | `BeepDataBlockTrigger` → `TriggerDefinition`; `new TriggerContext(...)` → FormsManager context |
| `OracleFormsLOVExamples.cs` | `BeepDataBlockLOV` → `LOVDefinition` |
| `OracleFormsItemPropertiesExamples.cs` | Static helper calls → `block.SetItemProperty(...)` delegate calls |

### 10.2 — Final Build and Smoke Test
- `dotnet build BeepDM.sln` — zero errors.
- `dotnet build WinFormsApp.sln` — zero errors.
- WinForms designer smoke test: open BeepDataBlock, no broken properties.
- Runtime smoke test: connect data source, CRUD, trigger fires, LOV opens, validation blocks save.

---

## Phase 11 — Other UI Adapters (Optional / Future)
**Scope:** Prove FormsManager is truly cross-platform.

### 11.1 — WPF Adapter
- `Beep.Desktop.Wpf/DataBlocks/WpfDataBlock.cs` implements `IDataBlockController`.
- Backed by the same `IFormsManager`. Zero business logic in the class.

### 11.2 — Blazor Adapter
- `MyWebSite/.../BlazorDataBlock.razor` with `@implements IDataBlockController`.
- Inject `IFormsManager` via DI; delegate all calls.

---

## Delegation Pattern Reference

Every method in BeepDataBlock partials follows one of two patterns after migration:

**Pattern A — Pure delegate (no UI side-effect):**
```csharp
public void RegisterValidationRule(string field, ValidationRule rule)
    => _formsManager.Validation.RegisterRule(Name, field, rule);
```

**Pattern B — Delegate + UI side-effect:**
```csharp
public void RegisterLOV(string itemName, LOVDefinition lov)
{
    _formsManager.LOV.RegisterLOV(Name, itemName, lov);
    if (UIComponents.TryGetValue(itemName, out var component))
        AttachLOVButtonToComponent(component);   // WinForms UI only
}
```

Nothing else belongs in a BeepDataBlock partial method.

---

## File Change Summary per Phase

| Phase | BeepDM files added | WinForms files deleted | WinForms files modified |
|---|---|---|---|
| 1 | 4 | 0 | 0 |
| 2 | 0 | 0 | `Coordination.cs`, `BeepDataBlock.cs` |
| 3 | 0 | 3 model + 1 helper | `Triggers.cs` |
| 4 | 0 | 1 model + 1 helper | `Validation.cs` |
| 5 | 0 | 1 model + 1 helper | `LOV.cs`, `Integration.cs` |
| 6 | 0 | 1 helper | `Properties.cs` |
| 7 | 0 | 1 helper | `UnitOfWork.cs` |
| 8 | 0 | 1 model | `SystemVariables.cs`, `Navigation.cs` |
| 9 | 0 | 0 | 3 interface / model files |
| 10 | 0 | 0 | 4 examples + docs |
| 11 | 2 (optional) | 0 | 0 |

**Total WinForms files deleted:** 11.  
**Total BeepDM files added:** 4–6.  
**End state:** `BeepDataBlock` contains zero private business-logic dictionaries. Every non-UI method is a one- or two-liner delegate to `_formsManager`.
