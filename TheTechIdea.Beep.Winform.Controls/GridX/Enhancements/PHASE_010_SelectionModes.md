# Phase 10: Selection Mode Strategies

**Priority:** P1 | **Track:** Feature Additions | **Status:** Pending

## Objective

Implement the Strategy pattern for selection modes, replacing the current monolithic `GridSelectionHelper` with pluggable strategies that can be swapped at runtime.

## Problem

`SelectionMode` is exposed publicly, but the current interaction logic still behaves primarily like active-cell focus plus row-checkbox selection. Different selection modes need distinct behavior for mouse clicks, keyboard navigation, and range selection.

## Implementation Steps

### Step 1: Create Interface

```csharp
// Selection/ISelectionStrategy.cs
public interface ISelectionStrategy
{
    void OnCellClick(int rowIndex, int columnIndex, bool isCtrl, bool isShift);
    void OnHeaderClick(int columnIndex, bool isCtrl);
    void OnKeyDown(Keys key, bool isCtrl, bool isShift);
    bool CanSelectCell(int rowIndex, int columnIndex);
    bool CanSelectRow(int rowIndex);
    bool CanSelectColumn(int columnIndex);
    void ClearSelection();
    IEnumerable<int> GetSelectedRows();
    IEnumerable<int> GetSelectedColumns();
    (int row, int col)? GetAnchorPoint();
}
```

### Step 2: Implement Strategies

Create one class per selection mode:

- `CellSelectionStrategy.cs` — single cell selection
- `RowSelectionStrategy.cs` — single row selection
- `MultiCellSelectionStrategy.cs` — multi-cell range (Ctrl+click, Shift+drag)
- `MultiRowSelectionStrategy.cs` — multi-row selection
- `ColumnSelectionStrategy.cs` — column selection

### Step 3: Refactor GridSelectionHelper

Modify `GridSelectionHelper` to hold a reference to `ISelectionStrategy` and delegate all selection operations to it.

### Step 4: Wire SelectionMode Property

In `BeepGridPro.Properties.cs`, when `SelectionMode` is set, create and assign the corresponding strategy:

```csharp
public BeepGridSelectionMode SelectionMode
{
    get => _selectionMode;
    set
    {
        _selectionMode = value;
        Selection.Strategy = value switch
        {
            BeepGridSelectionMode.Cell => new CellSelectionStrategy(),
            BeepGridSelectionMode.Row => new RowSelectionStrategy(),
            BeepGridSelectionMode.MultiCell => new MultiCellSelectionStrategy(),
            BeepGridSelectionMode.MultiRow => new MultiRowSelectionStrategy(),
            BeepGridSelectionMode.Column => new ColumnSelectionStrategy(),
            _ => new CellSelectionStrategy()
        };
    }
}
```

### Step 5: Preserve Checkbox Selection

Ensure checkbox row selection (`Sel` column) works independently of the active selection strategy.

## Acceptance Criteria

- [ ] Each selection mode behaves correctly
- [ ] Switching modes at runtime works
- [ ] Checkbox selection works in all modes
- [ ] Keyboard navigation respects current mode
- [ ] Ctrl+click and Shift+click work in multi modes

## Files to Create

- `Selection/ISelectionStrategy.cs`
- `Selection/CellSelectionStrategy.cs`
- `Selection/RowSelectionStrategy.cs`
- `Selection/MultiCellSelectionStrategy.cs`
- `Selection/MultiRowSelectionStrategy.cs`
- `Selection/ColumnSelectionStrategy.cs`

## Files to Modify

- `Helpers/GridSelectionHelper.cs`
- `BeepGridPro.Properties.cs`
- `BeepGridSelectionMode.cs`
