# BeepDialogForm — Null-Safety Fix Plan

## Problem Summary

`BeepDialogForm` inherits from `BeepiFormPro`, whose constructor triggers platform events
(Resize, theme sync, `ApplyFormStyle`) **before** the `BeepDialogForm` constructor body
runs. This means `_buttonPanel`, `_bodyPanel`, `_headerPanel`, and all child controls are
still `null` when those event handlers call into overridden methods such as `OnResize` and
`ApplyTheme`.

---

## Root Causes

### RC-1 — `PositionButtons()` has no null guard
`_buttonPanel` is allocated mid-constructor. If `PositionButtons()` is reached via a base
constructor event before that line executes, a `NullReferenceException` is thrown on
`_buttonPanel.ClientSize`.

**File / Line:** `BeepDialogForm.cs` → `PositionButtons()`

---

### RC-2 — `OnResize` has no null guard
The override calls `PositionButtons()` and `ReflowBodyContent()` unconditionally. The
WinForms layout engine fires `Resize` when the base constructor sets `FormBorderStyle`,
`ClientSize`, etc., long before `_buttonPanel` exists.

**File / Line:** `BeepDialogForm.cs` → `OnResize(EventArgs)`

---

### RC-3 — `ConfigureForDialogType()` has no null guard
This method is called:
- at the end of the `BeepDialogForm` constructor (safe, all fields initialized), **and**
- from the `DialogType` and `DialogButtons` property setters (called by `CreateDialog`
  in the manager **after** construction, safe), **but also**
- indirectly if a base-class theme or style event happens to set a property during init
  (unsafe — panels may not exist yet).

Guard prevents premature execution and also makes each property setter safe to call in
any order.

**File / Line:** `BeepDialogForm.cs` → `ConfigureForDialogType()`

---

### RC-4 — `SetButtonVisibilityAndCaptions()` calls `PositionButtons()` unconditionally
Every code path in `SetButtonVisibilityAndCaptions` ends by calling `PositionButtons()`.
Because `SetButtonVisibilityAndCaptions` is itself called by `ConfigureForDialogType`,
`PositionButtons` is invoked on every property-setter change — even during construction
if the guard in RC-3 is absent or bypassed.

**File / Line:** `BeepDialogForm.cs` → `SetButtonVisibilityAndCaptions()`

---

### RC-5 — `SetIcon()`, `UpdateDetailsVisibility()`, `UpdateTypedConfirmationVisibility()` have no null guards
Same construction-time risk as RC-3. `_dialogIcon`, `_detailsLabel`, `_detailsToggleButton`,
`_confirmationHintLabel`, and `_confirmationBox` are all null until the constructor
allocates them.

**File / Line:** `BeepDialogForm.cs` → the three methods above

---

### RC-6 — `ReflowBodyContent()` guards `_bodyPanel` but `PositionButtons()` does not guard `_buttonPanel`
Inconsistent defensive coding — `ReflowBodyContent` is safe, `PositionButtons` is not.

**File / Line:** `BeepDialogForm.cs` → `PositionButtons()` vs `ReflowBodyContent()`

---

### RC-7 — `CreateDialog` sets `DialogType` before `DialogButtons`
In `BeepDialogManager.Core.cs → CreateDialog()`:

```csharp
dialog.DialogType    = ...;   // triggers ConfigureForDialogType() → ApplyButtonSet → unknown _dialogButtons
dialog.DialogButtons = ...;   // triggers ConfigureForDialogType() a second time
```

The first call runs `ApplyButtonSet()` with the **default** `_dialogButtons` value, then
the second call corrects it. Double-rendering is wasteful and can expose the partial state
described in RC-1 through RC-5 if guards are not present.

**File / Line:** `BeepDialogManager.Core.cs` → `CreateDialog()`

---

### RC-8 — `SetInputVisibility()` calls `_inputBox.Focus()` during construction
`Focus()` on a control whose parent form is not yet shown/visible is a no-op at best and
a source of Win32 handle creation side-effects at worst. Focus management belongs in
`OnShown`, which already handles it correctly.

**File / Line:** `BeepDialogForm.cs` → `SetInputVisibility()`

---

## Fixes Applied

| RC | Location | Fix |
|----|----------|-----|
| RC-1 | `PositionButtons()` | Added `if (_buttonPanel == null) return;` guard at method entry |
| RC-2 | `OnResize()` | Added `if (_buttonPanel == null) return;` guard after `base.OnResize(e)` |
| RC-3 | `ConfigureForDialogType()` | Added `if (_headerPanel == null \|\| _buttonPanel == null \|\| _bodyPanel == null) return;` |
| RC-4 | `SetButtonVisibilityAndCaptions()` | Added `if (_leftButton == null \|\| ...) return;` guard |
| RC-5 | `SetIcon()` | Added `if (_dialogIcon == null) return;` guard |
| RC-5 | `UpdateDetailsVisibility()` | Added `if (_detailsLabel == null \|\| _detailsToggleButton == null) return;` |
| RC-5 | `UpdateTypedConfirmationVisibility()` | Added `if (_confirmationHintLabel == null \|\| _confirmationBox == null) return;` |
| RC-7 | `CreateDialog()` in manager | Set `DialogButtons` **before** `DialogType` so a single `ConfigureForDialogType` call sees the correct button set |
| RC-8 | `SetInputVisibility()` | Removed `_inputBox.Focus()` call; focus is handled exclusively in `OnShown` |

---

## Recommended Follow-up

- **RC-6** is resolved as a side-effect of RC-1. No separate action needed.
- Consider introducing a private `bool _initialized` flag set at the very end of the
  `BeepDialogForm` constructor as a single, authoritative guard, replacing the per-method
  null checks with `if (!_initialized) return;`.
- Add a unit/integration test that constructs `BeepDialogForm` and immediately sets
  `DialogType` and `DialogButtons` without showing the form, to act as a regression guard.
