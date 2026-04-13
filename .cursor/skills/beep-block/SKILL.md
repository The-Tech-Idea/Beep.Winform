---
name: beep-block
description: Guidance for BeepBlock — the WinForms block-level UI surface in the integrated controls path. Use when implementing or extending a BeepBlock, BeepBlockDefinition, BeepFieldDefinition, BeepBlockEntityDefinition, BeepFieldControlTypeRegistry, BeepBlockPresenterRegistry, or field-presenter pipeline. Covers record mode, grid mode, query mode, LOV popup, navigation bar, validation summary, designer smart-tag workflow, and field-control-type policy.
---

# BeepBlock

`BeepBlock` is the block-level UI surface in the integrated controls path. Each instance maps to one logical `FormsManager` block and is hosted by `BeepForms` (`IBeepFormsHost`). It inherits from `BaseControl` and implements `IBeepBlockView`.

## Use this skill when
- Creating or modifying a `BeepBlock` usage in a form
- Setting up `BeepBlockDefinition`, `BeepFieldDefinition`, or `BeepBlockEntityDefinition` for a block
- Customizing field editor keys, control types, or binding properties
- Working with the field-control-type policy (`BeepFieldControlTypeRegistry`, `BeepFieldControlTypePolicy`)
- Adding or registering a custom field presenter (`IBeepFieldPresenter`)
- Implementing query-mode criteria capture, LOV popup, or grid mode
- Authoring design-time setup through the BeepBlock smart tag or modal editors

## Do not use this skill when
- The task is about `FormsManager` orchestration only — use [`forms`](../forms/SKILL.md)
- The task is about the surrounding `BeepForms` host — use [`beep-forms-integrated`](../beep-forms-integrated/SKILL.md)

---

## Architecture — Partial-Class Structure

| File | Responsibility |
| --- | --- |
| `BeepBlock.cs` | Root control, `BlockName`, `Definition`, `Entity` properties, host binding, presenter registry |
| `BeepBlock.Layout.cs` | Caption/header, workflow strip, validation summary, record-host and grid-host regions |
| `BeepBlock.RecordMode.cs` | Definition-driven record-editor row scaffold, shared `BindingSource` wire-up |
| `BeepBlock.GridMode.cs` | `BeepGridPro` column configuration from runtime field/entity metadata, grid `BindingSource` |
| `BeepBlock.QueryMode.cs` | Typed criteria-entry rows, operator/value state, range/list/no-value widgets, `AppFilter` packaging |
| `BeepBlock.Binding.cs` | `BindingSource` lifecycle, `Position` ↔ `UnitOfWork.CurrentItem` synchronization |
| `BeepBlock.Lov.cs` | `BeepLovPopup` integration, F9 keyboard entry, related-field map-back, debounced manager LOV reload |
| `BeepBlock.Navigation.cs` | `BeepBlockNavigationBar` command availability from manager state + `Navigation` definition overrides |
| `BeepBlock.Validation.cs` | Field highlighting, severity badges, block-level headline+detail validation summary, semantic tooltips |
| `BeepBlock.Metadata.cs` | Design-time typed suggestions, entity snapshot capture from `FormsManager`, metadata helpers |

---

## Model Types

### `BeepBlockDefinition`
Top-level definition persisted in `Designer.cs`. Serialized as `DesignerSerializationVisibility.Content`.

| Property | Type | Purpose |
| --- | --- | --- |
| `Id` | string | unique stable identifier |
| `BlockName` | string | matches `BeepBlock.BlockName` |
| `ManagerBlockName` | string | overrides manager block name when different from UI name |
| `Caption` | string | header label |
| `PresentationMode` | `BeepBlockPresentationMode` | `Record` or `Grid` |
| `Fields` | `List<BeepFieldDefinition>` | ordered field editors |
| `Entity` | `BeepBlockEntityDefinition` | connection/entity metadata snapshot |
| `Navigation` | `BeepBlockNavigationDefinition` | navigation bar overrides |
| `Metadata` | `Dictionary<string,string>` | legacy key/value bag |

### `BeepFieldDefinition`
Per-field editor descriptor inside `BeepBlockDefinition.Fields`.

| Property | Purpose |
| --- | --- |
| `FieldName` | entity field name (binding key) |
| `Label` | display label |
| `EditorKey` | presenter key: `text`, `numeric`, `date`, `checkbox`, `combo`, `lov` |
| `ControlType` | full CLR type name for explicit control override |
| `BindingProperty` | control property used for data binding (e.g., `Text`, `Value`, `SelectedValue`) |
| `Order` | column/row order |
| `Width` | preferred editor width |
| `IsVisible` | include in scaffold |
| `IsReadOnly` | render read-only |
| `Options` | `List<BeepFieldOptionDefinition>` — static combo/LOV items |

### `BeepBlockEntityDefinition`
UI-side entity snapshot. Preseed `EditorKey`, `ControlType`, `BindingProperty` to override generated field defaults before `BeepFieldDefinition` rows are emitted.

| Property | Purpose |
| --- | --- |
| `ConnectionName` | `BeepDataConnection` connection name |
| `EntityName` | logical entity name |
| `DatasourceEntityName` | datasource-specific entity name |
| `IsMasterBlock` | true for master blocks in master/detail |
| `MasterBlockName` | name of the master block for detail |
| `MasterKeyField` | primary key field on master |
| `ForeignKeyField` | foreign key field on this entity |
| `Fields` | `List<BeepBlockEntityFieldDefinition>` — per-field control/binding overrides |

### `BeepBlockEntityFieldDefinition` — full `EntityField` snapshot

This is a **UI-side snapshot** of the corresponding `EntityField` from `EntityStructure.Fields`. It is populated by `CreateEntityDefinition()` in `BeepBlock.Metadata.cs`. **`EntityField` / `EntityStructure` are never stored directly in UI controls.**

| Property | Source in `EntityField` | UI impact |
| --- | --- | --- |
| `FieldName` | `FieldName` | binding key |
| `Label` | `Caption` (fallback `FieldName`) | display label |
| `DataType` | `Fieldtype` (.NET type string) | editor type selection |
| `Category` | `FieldCategory` (`DbFieldCategory`) | control type resolution |
| `Order` | `OrdinalPosition` | column/row order |
| `Size` | `Size` | editor width hint |
| `NumericPrecision` | `NumericPrecision` | numeric format |
| `NumericScale` | `NumericScale` | numeric format |
| `IsRequired` | `IsRequired` | validation rule |
| `AllowDBNull` | `AllowDBNull` | validation rule |
| `IsPrimaryKey` | `IsKey` ∪ `PrimaryKeys` membership | display badge; excluded from WHERE updates |
| `IsUnique` | `IsUnique` | validation rule |
| `IsIndexed` | `IsIndexed` | metadata only |
| `IsAutoIncrement` | `IsAutoIncrement` | sets `IsReadOnly = true` in `ToFieldDefinition()` |
| `IsReadOnly` | `IsReadOnly` | propagated to `BeepFieldDefinition` |
| `IsCheck` | `IsCheck` | forces `checkbox` editor |
| `IsIdentity` | `IsIdentity` | server-generated; sets `IsReadOnly = true`; excluded from INSERT |
| `IsHidden` | `IsHidden` | sets `IsVisible = false`; field absent from rendered scaffold |
| `IsLong` | `IsLong` | LOB/CLOB/TEXT; forces multi-line (`BeepRichTextBox`) editor |
| `IsRowVersion` | `IsRowVersion` | concurrency stamp; sets `IsReadOnly = true` |
| `DefaultValue` | `DefaultValue` | provider default; passed to editor as placeholder hint |

**Phase 7A adds:** `IsIdentity`, `IsHidden`, `IsLong`, `IsRowVersion`, `DefaultValue`.  Until Phase 7A is complete the 5 fields are absent and auto-increment/identity columns will appear editable.

### `BeepBlockNavigationDefinition`
Typed overrides for the `BeepBlockNavigationBar`. Null properties fall back to manager state.

---

## Services

### `BeepFieldControlTypeRegistry` (static)
Resolves the default editor key, control type, and binding property for a field given its `DbFieldCategory` and CLR data type. Combines built-in rules with an optional persisted policy file.

```csharp
// Resolve defaults for a date field
var resolution = BeepFieldControlTypeRegistry.ResolveDefaultFieldSettings(
    DbFieldCategory.Date, "DateTime");
// resolution.EditorKey    → "date"
// resolution.ControlType  → fully qualified BeepDateTimePicker CLR name
// resolution.BindingProperty → "Value"
```

**Policy file location:** `%LocalAppData%\TheTechIdea\Beep.Winform\field-control-defaults.json`

Editable through:
- The BeepBlock smart-tag setup wizard
- The `BeepFieldControlTypePolicy` dedicated editor surface
- Per-entity field snapshots in `BeepBlockEntityDefinition.Fields`

### `BeepBlockPresenterRegistry`
Holds `IBeepFieldPresenter` instances keyed by `EditorKey`. Resolved per `BeepFieldDefinition` at scaffold time.

Default registered keys: `text`, `numeric`, `date`, `checkbox`, `combo` (aliased also as `lov`, `option`).

Fallback for explicit `ControlType`: `ReflectiveControlBeepFieldPresenter` — instantiates the named CLR type, binds via `BindingProperty`.

```csharp
// Register a custom presenter
block.PresenterRegistry.Register(new MyColorPickerPresenter());
```

### `IBeepFieldPresenter`
```csharp
public interface IBeepFieldPresenter
{
    string Key { get; }
    bool CanPresent(BeepFieldDefinition fieldDefinition);
    Control CreateEditor(BeepFieldDefinition fieldDefinition);
}
```

## Key Runtime Behaviors

### EntityStructure → BeepBlock data flow

```
IDataSource.GetEntityStructure(entityName)
  → EntityStructure { Fields: List<EntityField>, PrimaryKeys, Relations }
       ↓  (via FormsManager or direct feed)
  BeepBlock.Metadata.CreateEntityDefinition()
  → BeepBlockEntityFieldDefinition[]   ← lossless snapshot of EntityField
       ↓
  BeepBlockEntityFieldDefinition.ToFieldDefinition()
  → BeepFieldDefinition[]   ← editor descriptors (ControlType, IsVisible, IsReadOnly …)
       ↓
  BeepFieldControlTypeRegistry.ResolveDefaultFieldSettings(Category, DataType, IsLong)
  → ControlType, EditorKey, BindingProperty   ← correct editor per field type
       ↓
  BeepBlockPresenterRegistry.Resolve(fieldDef)
  → IBeepFieldPresenter   ← creates the actual WinForms control
```

> **Architecture boundary:** `BeepBlock` is UI only. It never calls `IDataSource` and never accepts `EntityStructure` as a parameter. The only path for schema data into `BeepBlock` is `SyncFromManager()` which reads `DataBlockInfo` that **FormsManager** provides. To set up a block from a connection/entity name, call `FormsManager.SetupBlockAsync(blockName, connectionName, entityName)` — FormsManager does all the datasource work internally.

### `DbFieldCategory` → editor defaults

| `DbFieldCategory` | `IsLong` | Editor control | Key |
| --- | --- | --- | --- |
| `String` | false | `BeepTextBox` | `text` |
| `String` / `LongString` | true | `BeepRichTextBox` | `memo` |
| `Integer` | — | `BeepNumericBox` | `integer` |
| `Decimal` / `Float` | — | `BeepNumericBox` (with format) | `decimal` |
| `Date` | — | `BeepDatePicker` | `date` |
| `DateTime` | — | `BeepDateTimePicker` | `datetime` |
| `Boolean` | — | `BeepCheckBox` | `checkbox` |
| `Binary` | — | `BeepBlobViewer` | `blob` |
| `Enum` | — | `BeepComboBox` | `combobox` |

`IsIdentity = true` / `IsRowVersion = true` → `IsReadOnly` regardless of category.  
`IsHidden = true` → `IsVisible = false`; field absent from rendered scaffold.  
(Phase 7C finalises this mapping table.)

### Binding contract
- Record editors bind to a shared block-level `BindingSource` over `FormsManager.GetUnitOfWork(blockName).Units`.
- `BindingSource.Position` and `UnitOfWork.CurrentItem` stay synchronized.
- Grid mode also uses the same `BindingSource`; switching presentation mode does not break position.
- Records are `List<object>` items where each item is a class instance with public properties — not `DataRow` or dictionaries.

### Field scaffold resolution order
1. Explicit `BeepFieldDefinition` list in `Definition.Fields` (design-time authored).
2. `Definition.Entity.Fields` → `CreateFieldDefinitions()` if no explicit list.
3. Runtime `FormsManager.GetBlock(blockName).UnitOfWork.EntityStructure` field capture as final fallback.

### LOV flow
1. Field with `EditorKey = "lov"` or a bound manager LOV registration triggers the LOV surface.
2. Record-mode LOV fields render a picker button + F9 keyboard entry → opens `BeepLovPopup`.
3. Popup pre-seeded from manager-backed LOV load; popup search box debounces into manager LOV reloads.
4. After selection: related-field mappings applied to current record; sibling editors refreshed.

### Query mode
- Per-field operator/value rows; supported operators include `=`, `<>`, `<`, `>`, `<=`, `>=`, `like`, `not like`, `between`, `not between`, `in`, `not in`, `is null`, `is not null`.
- Range fields (`between`/`not between`) show two value editors.
- List fields (`in`/`not in`) show a dedicated list-entry widget (free-text or multi-pick from static options/LOV).
- No-value operators (`is null`/`is not null`) hide unused editors.
- `packageFiltersForManager()` produces `List<AppFilter>` with `FilterValue1` populated.

### Validation summary
- Driven by manager `ValidationManager` events and field-change feedback.
- Severity: `Error` → headline + detail + badge; `Warning` → subdued badge + tooltip; `Info` → tooltip only.
- Suppressed while query mode is active (block acts as clean criteria-entry surface).

---

## Design-Time Workflow (Smart Tag / Modal Editors)
1. Drag `BeepBlock` onto the form.
2. Open the smart tag → **Setup Wizard**: select connection → entity → visible fields → presentation mode.
3. Tune field order, labels, editor keys, and control types in the **Field Editor** modal.
4. Set navigation overrides in the **Navigation Definition Editor**.
5. All settings persist to `Designer.cs` as `BeepBlockDefinition` content.

---

## File Locations
```
TheTechIdea.Beep.Winform.Controls.Integrated/
  Blocks/
    BeepBlock/
      BeepBlock.cs                   ← root, IBeepBlockView
      BeepBlock.Binding.cs
      BeepBlock.GridMode.cs
      BeepBlock.Layout.cs
      BeepBlock.Lov.cs
      BeepBlock.Metadata.cs
      BeepBlock.Navigation.cs
      BeepBlock.QueryMode.cs
      BeepBlock.RecordMode.cs
      BeepBlock.Validation.cs
    Contracts/
      IBeepBlockView.cs
      IBeepFieldPresenter.cs
    Models/
      BeepBlockDefinition.cs         ← BeepBlockDefinition, BeepBlockNavigationDefinition
      BeepBlockEntityDefinition.cs   ← BeepBlockEntityDefinition, BeepBlockEntityFieldDefinition
      BeepBlockViewState.cs
      BeepFieldDefinition.cs         ← BeepFieldDefinition, BeepFieldOptionDefinition
    Navigation/
      BeepBlockNavigationBar.cs
    Services/
      BeepBlockPresenterRegistry.cs
      BeepFieldControlTypePolicy.cs
      BeepFieldControlTypeRegistry.cs
      Presenters/
        CheckboxBeepFieldPresenter.cs
        ComboBeepFieldPresenter.cs
        DateBeepFieldPresenter.cs
        NumericBeepFieldPresenter.cs
        ReflectiveControlBeepFieldPresenter.cs
        TextBeepFieldPresenter.cs
```

---

## ⚠ Bootstrap Gap (Phase 7 — Not Yet Implemented)

`BeepBlock.SyncFromManager()` correctly reads `manager.GetBlock(blockName).UnitOfWork.EntityStructure` and builds runtime field definitions. **This works correctly IF FormsManager already has the block registered.**

The gap is upstream: `BeepForms` does not yet call FormsManager to register blocks from `BeepFormsDefinition.Blocks[].Entity` data. Until Phase 7 is implemented, a `BeepBlock` bound to an unregistered manager block name will fall through `manager.BlockExists()` → all viewstate cleared → no fields rendered.

**Workaround (current):** register the block with FormsManager before assigning `BeepForms.FormsManager`.

See `.plans/07-phase7-formsmanager-bootstrap.md`.

---

## Related Skills
- [`beep-forms-integrated`](../beep-forms-integrated/SKILL.md) — host that contains BeepBlock instances
- [`forms`](../forms/SKILL.md) — FormsManager orchestration layer
- [`beep-data-connection`](../beep-data-connection/SKILL.md) — connection management for entity metadata

## Detailed Reference
Use [`reference.md`](./reference.md) for usage scenarios and code examples.
