# Phase 17: Editor Framework Refactor

**Priority:** P3 | **Track:** Polish & Quality | **Status:** Pending

## Objective

Extract editor logic from `GridEditHelper` into composable, testable editor components. Each editor implements `IGridEditor` and is created by `GridEditorFactory`.

## Implementation Steps

### Step 1: Create Editor Interface

```csharp
// Editors/IGridEditor.cs
public interface IGridEditor
{
    Control EditorControl { get; }
    void SetValue(object? value);
    object? GetValue();
    void BeginEdit();
    void EndEdit();
    void CancelEdit();
    event EventHandler? ValueChanged;
    event EventHandler? EditCompleted;
    event EventHandler? EditCancelled;
}
```

### Step 2: Create Editor Factory

```csharp
// Editors/GridEditorFactory.cs
public class GridEditorFactory
{
    private readonly Dictionary<BeepColumnType, Func<IGridEditor>> _editors = new();

    public void RegisterEditor(BeepColumnType type, Func<IGridEditor> factory);
    public IGridEditor CreateEditor(BeepColumnType type, BeepColumnConfig column);
    public void RegisterDefaultEditor(Func<IGridEditor> factory);
}
```

### Step 3: Implement Editor Classes

- `BeepGridDateDropDownEditor.cs` — wraps `BeepDateDropDown`
- `BeepGridComboBoxEditor.cs` — wraps `BeepComboBox`
- `BeepGridNumericEditor.cs` — wraps numeric input control
- `BeepGridMaskedEditor.cs` — wraps masked input control

Each editor implements `IGridEditor` and encapsulates its specific lifecycle.

### Step 4: Refactor GridEditHelper

Modify `GridEditHelper` to:
- Use `GridEditorFactory` to create editors
- Delegate editor lifecycle to `IGridEditor` implementations
- Remove type-specific switch statements
- Keep only orchestration logic (positioning, focus, commit)

### Step 5: Support Custom Editors

Allow users to register custom editors:

```csharp
grid.Edit.Factory.RegisterEditor(BeepColumnType.Custom, () => new MyCustomEditor());
```

## Acceptance Criteria

- [ ] Each editor type works in grid cell
- [ ] Custom editor registration works
- [ ] Editor lifecycle (begin/edit/end/cancel) works correctly
- [ ] Value conversion works for all editor types
- [ ] No regression in existing editor behavior
- [ ] `GridEditHelper` is simplified and more maintainable

## Files to Create

- `Editors/IGridEditor.cs`
- `Editors/GridEditorFactory.cs`
- `Editors/BeepGridDateDropDownEditor.cs`
- `Editors/BeepGridComboBoxEditor.cs`
- `Editors/BeepGridNumericEditor.cs`
- `Editors/BeepGridMaskedEditor.cs`

## Files to Modify

- `Helpers/GridEditHelper.cs`
