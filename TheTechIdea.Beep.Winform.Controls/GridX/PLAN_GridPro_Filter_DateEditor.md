# BeepGridPro — Fix Plan
## Issue 1: Filter Not Working  |  Issue 2: Date Editor Direct Dropdown

---

## Issue 1 — Filter Not Working

### Root Cause (confirmed by code analysis)

`ApplyLocalVisibilityFilter()` in `GridSortFilterHelper.cs` correctly sets `row.IsVisible = false`
on rows that don't match the filter. However, `DrawRows()` in
`GridRenderHelper.Rendering.cs` **completely ignores `row.IsVisible`** — it
iterates every row in `[visibleRowStart … visibleRowEnd]` without checking the flag,
so hidden rows are painted as normal and occupy vertical space.

Three specific places are broken:

| Location | Problem |
|---|---|
| `GridRenderHelper.Rendering.cs` — `DrawRows()` loop | Does not skip `row.IsVisible == false`; draws every row |
| `GridRenderHelper.Rendering.cs` — height-accumulation loop (sticky section) | Adds height for invisible rows, displacing `currentY` |
| `GridRenderHelper.Rendering.cs` — `GetVisibleRowCount()` helper | Does not skip invisible rows when counting how many rows fit in viewport |

### Fix Plan

#### Step 1 — `DrawRows()`: skip invisible rows

In the main scrolling draw loop:
```csharp
// BEFORE (line ~516)
for (int r = visibleRowStart; r <= visibleRowEnd && r < _grid.Data.Rows.Count; r++)
{
    var row = _grid.Data.Rows[r];
    int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
    // draws unconditionally
```
**After fix:**
```csharp
for (int r = visibleRowStart; r <= visibleRowEnd && r < _grid.Data.Rows.Count; r++)
{
    var row = _grid.Data.Rows[r];
    if (!row.IsVisible) continue;          // ← ADD THIS
    int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
    // ...draw...
    drawY += rowHeight;                    // only advance Y for visible rows
}
```
The same guard must be applied in the **sticky columns** draw loop (if one exists
for stickied cols — same file).

#### Step 2 — `currentY` height accumulation: skip invisible rows

Before the main draw loop, `currentY` is computed by summing row heights up to
`firstVisibleRowIndex`. That loop must also skip invisible rows:
```csharp
// BEFORE
for (int i = 0; i < firstVisibleRowIndex && i < _grid.Data.Rows.Count; i++)
{
    var row = _grid.Data.Rows[i];
    totalRowsHeight += row.Height > 0 ? row.Height : _grid.RowHeight;
}
```
**After fix:**
```csharp
for (int i = 0; i < firstVisibleRowIndex && i < _grid.Data.Rows.Count; i++)
{
    var row = _grid.Data.Rows[i];
    if (!row.IsVisible) continue;          // ← ADD THIS
    totalRowsHeight += row.Height > 0 ? row.Height : _grid.RowHeight;
}
```

#### Step 3 — `GetVisibleRowCount()`: skip invisible rows

Find `GetVisibleRowCount()` in the same file and change the height accumulation
inside it to skip rows where `IsVisible == false`.

#### Step 4 — Scroll bar update after filter

After `ApplyLocalVisibilityFilter()` calls `RecalculateAndInvalidate()`, the
scroll bars must recalculate based on total **visible** row height, not total
row count. Verify `ScrollBars.UpdateBars()` uses `Rows.Count(r => r.IsVisible)`
(or the equivalent visible-row-height sum) — if it uses `_grid.Data.Rows.Count`
fix it as well (check `GridScrollHelper`).

#### Step 5 — Verify filter row calls `SortFilter.Filter()`

Check that the inline filter row text boxes (in header) call
`_grid.SortFilter.Filter(columnName, text)` on `TextChanged`. If this call is
missing or the column name does not match `BeepRowConfig.Cells[i].ColumnName`
the `_containsFilters` dictionary is never populated. Add a quick trace log.

---

## Issue 2 — Date Field: Show `BeepDateDropDown` Directly on Cell Click

### Current Behaviour (confirmed by code analysis)

`CreateEditorForColumn()` in `GridEditHelper.cs`:
```csharp
BeepColumnType.DateTime => new BeepDatePicker { IsChild = true, GridMode = true },
```
`BeepDatePicker` is an inline picker control (always-visible calendar region).
The user must click the cell → editor appears → click the calendar icon → popup shows.

### Desired Behaviour

On first cell click: `BeepDateDropDown` is placed in the cell **and its calendar popup
opens immediately** — exactly like how a `BeepComboBox` could auto-open its list.

### Fix Plan

#### Step 1 — `CreateEditorForColumn()`: change DateTime editor

```csharp
// GridEditHelper.cs — inside CreateEditorForColumn() switch
BeepColumnType.DateTime => new BeepDateDropDown { IsChild = true, GridMode = true },
```

#### Step 2 — `BeginEdit()`: seed value + open popup immediately

In the `BeginInvoke` callback inside `BeginEdit()`, add a block for
`BeepDateDropDown` after the existing `BeepComboBox` block:

```csharp
// After: _editorControl.Focus();
if (_currenteditorUIcomponent is BeepDateDropDown ddd)
{
    // Seed current cell value into the picker
    if (_originalValue is DateTime dtVal)
        ddd.SelectedDateTime = dtVal;
    else if (_originalValue is string s && DateTime.TryParse(s, out var parsed))
        ddd.SelectedDateTime = parsed;

    // Open calendar popup immediately (first click = popup visible)
    try { ddd.ShowPopup(); } catch { }
}
```

#### Step 3 — Wire `DropDownClosed` → `EndEdit(true)` (like `OnComboPopupClosed`)

In `BeginEdit()`, subscribe to `DropDownClosed` right after creating the editor
(same pattern as `combo.PopupClosed += OnComboPopupClosed`):

```csharp
if (_currenteditorUIcomponent is BeepDateDropDown ddd2)
    ddd2.DropDownClosed += OnDateDropDownClosed;
```

Add the handler:
```csharp
private void OnDateDropDownClosed(object sender, EventArgs e)
{
    if (!_isEndingEdit && _currenteditorUIcomponent is BeepDateDropDown ddd)
    {
        if (_grid != null && !_grid.IsDisposed && _grid.IsHandleCreated)
        {
            _grid.BeginInvoke(new Action(() =>
            {
                if (!_isEndingEdit && ddd != null && !ddd.IsDisposed)
                    EndEdit(true);
            }));
        }
        else
        {
            if (!_isEndingEdit && ddd != null && !ddd.IsDisposed)
                EndEdit(true);
        }
    }
}
```

#### Step 4 — Detach `DropDownClosed` in `EndEdit()`

In `EndEdit()`, add the detach alongside the existing combo detach:
```csharp
if (_currenteditorUIcomponent is BeepDateDropDown dddDetach)
    dddDetach.DropDownClosed -= OnDateDropDownClosed;
```

#### Step 5 — `LostFocus` guard for BeepDateDropDown

In `OnEditorLostFocus()`, add `BeepDateDropDown` to the early-return guard
(same as `BeepComboBox`) so moving focus to the popup doesn't prematurely commit:
```csharp
private void OnEditorLostFocus(object sender, EventArgs e)
{
    if (_suppressLostFocus || _isEndingEdit) return;
    if (sender is BeepComboBox) return;
    if (sender is BeepDateDropDown ddd && ddd._isPopupOpen) return;  // ← ADD
    EndEdit(true);
}
```

#### Step 6 — `GetValue()` override in `BeepDateDropDown`

Verify `BeepDateDropDown` returns `SelectedDateTime` (not the text string) from
`IBeepUIComponent.GetValue()`. Currently it inherits `BeepTextBox.GetValue()` which
returns the text. Add or confirm an override in `BeepDateDropDown.Properties.cs`:

```csharp
public override object GetValue() => _selectedDateTime;
```

This ensures `NormalizeEditorValue()` in `GridEditHelper` receives a `DateTime`
object and `ConvertValue()` maps it correctly without needing to parse text.

---

## Files to Modify

| File | Change |
|---|---|
| `GridX/Helpers/GridRenderHelper.Rendering.cs` | Add `if (!row.IsVisible) continue;` in `DrawRows()` and height loops |
| `GridX/Helpers/GridEditHelper.cs` | `BeepDatePicker` → `BeepDateDropDown`, add `BeginEdit` seed + popup, add `OnDateDropDownClosed`, update `EndEdit` + `OnEditorLostFocus` |
| `Dates/BeepDateDropDown.Properties.cs` | Add `GetValue()` override if missing |

---

## Implementation Order

1. `GridRenderHelper.Rendering.cs` — filter rendering fix (Issues immediately visible)
2. `GridEditHelper.cs` — date editor changes (Step 1–5)
3. `BeepDateDropDown.Properties.cs` — `GetValue()` override (Step 6)
