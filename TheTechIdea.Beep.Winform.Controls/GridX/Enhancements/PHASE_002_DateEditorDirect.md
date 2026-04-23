# Phase 2: Date Editor Direct Dropdown

**Priority:** P0 | **Track:** Bug Fixes | **Status:** Pending

## Objective

When a user clicks a DateTime cell, show `BeepDateDropDown` directly with the calendar popup open — eliminating the extra click required with the current `BeepDatePicker`.

## Problem

`CreateEditorForColumn()` in `GridEditHelper.cs` creates a `BeepDatePicker` for DateTime columns. This is an inline picker that requires: click cell → editor appears → click calendar icon → popup shows. Users expect a single click to open the dropdown calendar.

## Implementation Steps

### Step 1: Change Editor Type

In `GridEditHelper.cs`, `CreateEditorForColumn()`:

```csharp
// BEFORE
BeepColumnType.DateTime => new BeepDatePicker { IsChild = true, GridMode = true },

// AFTER
BeepColumnType.DateTime => new BeepDateDropDown { IsChild = true, GridMode = true },
```

### Step 2: Seed Value and Open Popup

In `BeginEdit()`, inside the `BeginInvoke` callback, after `_editorControl.Focus()`:

```csharp
if (_currenteditorUIcomponent is BeepDateDropDown ddd)
{
    if (_originalValue is DateTime dtVal)
        ddd.SelectedDateTime = dtVal;
    else if (_originalValue is string s && DateTime.TryParse(s, out var parsed))
        ddd.SelectedDateTime = parsed;

    try { ddd.ShowPopup(); } catch { }
}
```

### Step 3: Wire DropDownClosed Event

In `BeginEdit()`, after creating the editor:

```csharp
if (_currenteditorUIcomponent is BeepDateDropDown ddd2)
    ddd2.DropDownClosed += OnDateDropDownClosed;
```

Add handler:

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

### Step 4: Detach in EndEdit

In `EndEdit()`:

```csharp
if (_currenteditorUIcomponent is BeepDateDropDown dddDetach)
    dddDetach.DropDownClosed -= OnDateDropDownClosed;
```

### Step 5: LostFocus Guard

In `OnEditorLostFocus()`:

```csharp
if (sender is BeepDateDropDown ddd && ddd._isPopupOpen) return;
```

### Step 6: GetValue Override

Verify `BeepDateDropDown` returns `SelectedDateTime` from `GetValue()`:

```csharp
public override object GetValue() => _selectedDateTime;
```

## Acceptance Criteria

- [ ] Clicking a DateTime cell opens the dropdown calendar immediately
- [ ] Selecting a date commits the value correctly
- [ ] Clicking away closes dropdown and commits
- [ ] Value type is `DateTime`, not string
- [ ] No memory leaks from event subscriptions

## Rollback Plan

Revert `CreateEditorForColumn()` to use `BeepDatePicker` and remove the `BeepDateDropDown`-specific code from `BeginEdit()`, `EndEdit()`, and `OnEditorLostFocus()`.

## Files to Modify

- `Helpers/GridEditHelper.cs`
- `Dates/BeepDateDropDown.Properties.cs` (verify/add GetValue override)
