# BeepDialogManager — Fix Plan
**Date:** 2026-03-05  
**Branch:** master  
**Status:** ✅ Implemented

---

## 1. Goal

Replace the static, legacy `DialogManager` with the Beep-themed, instance-based `BeepDialogManager` + `BeepDialogModal` system. Ensure dialogs correctly show title, message, buttons, and Beep theme colors with no blank input text box appearing on non-input dialogs.

---

## 2. Legacy Code Removed

The following files were deleted — they contained the old static `DialogManager` partial class (broken async, `ShowDialog` on ThreadPool) and the painter-based `BeepDialogForm`:

| Deleted File | Reason |
|---|---|
| `DialogsManagers/DialogManager.cs` | Static legacy dialog manager — replaced by `BeepDialogManager` |
| `DialogsManagers/DialogManager.Presets.cs` | Preset helpers for the static legacy manager |
| `DialogsManagers/DialogManager.Animations.cs` | Animation helper for the static legacy manager |
| `Forms/BeepDialogForm.cs` | Old painter-based dialog form (`beepDialogBox1`) — replaced by `BeepDialogModal` |
| `Forms/BeepDialogForm.Designer.cs` | Designer file for above |
| `Forms/BeepDialogForm.resx` | Resource file for above |

No consumers of these files were found in the codebase — removal is safe.

---

## 3. Root Causes Fixed

### Bug 1 — Blank `InputTextBox` on every dialog (PRIMARY)
**File:** `Forms/BeepDialogModal.cs` — `SetInputTypeBasedonDialogType()`  
**Symptom:** An empty, focused text box appeared on every dialog (Information, Warning, Error, Question) even though those types don't need user input.  
**Cause:** The method called `SetInputVisible()` for all types except `GetInputFromList`. The `inputVisibility` dictionary was already correct (`Information/Warning/Error/Question/None → (false, false)`) but was never consulted.  
**Fix:** Rewrote `SetInputTypeBasedonDialogType()` to use the existing `inputVisibility` dictionary. Added `SetInputHidden()` — the path for all non-input types.

```csharp
// Before — wrong: showed InputTextBox on every dialog
case DialogType.Information:
    SetInputVisible();   // ← BUG: showed text box on info dialog
    break;

// After — correct: uses the inputVisibility lookup table
public void SetInputTypeBasedonDialogType()
{
    if (inputVisibility.TryGetValue(DialogType, out var vis))
    {
        if (!vis.textBox && !vis.comboBox) SetInputHidden();
        else if (vis.textBox)              SetInputVisible();
        else                               SetInputListVisible();
    }
    else SetInputHidden();
}
public void SetInputHidden()
{
    InputTextBox.Visible = false;
    SelectFromListComboBox.Visible = false;
}
```

---

### Bug 2 — Duplicate button click handlers
**File:** `Forms/BeepDialogModal.cs` — `SetButtonsDialogResultBasedonDialgType()`  
**Symptom:** Setting `DialogType` then `DialogButtons` (both in `CreateDialog()`) called `SetDialogType()` twice, accumulating `+=` handlers twice. Each button click fired the handler twice, causing double `this.Close()` calls and unpredictable `DialogResult`.  
**Fix:** Added `UnwireButtonHandlers()` private method that removes all three handlers with `-=`. This is called at the top of `SetButtonsDialogResultBasedonDialgType()` before re-wiring.

```csharp
private void UnwireButtonHandlers()
{
    LeftButton.Click  -= LeftButton_Click;
    RightButton.Click -= RightButton_Click;
    MiddleButton.Click -= LeftButton_Click;
    MiddleButton.Click -= CenterButton_Click;
}
public void SetButtonsDialogResultBasedonDialgType()
{
    UnwireButtonHandlers();  // ← clear before re-wiring
    switch (DialogType) { ... }
}
```

---

### Bug 3 — Theme not applied to dialog controls
**File:** `DialogsManagers/BeepDialogManager.Core.cs` — `CreateDialog()`  
**Symptom:** Controls inside `BeepDialogModal` (buttons, label, text box) kept Designer default colors instead of the active Beep theme.  
**Cause:** `CreateDialog()` only set `dialog.Theme = theme.ThemeName` when `config.UseBeepThemeColors == true`, and never called `dialog.ApplyTheme()` at all. Without `ApplyTheme()`, the Theme string was stored but not propagated to child controls.  
**Fix:** Removed the `UseBeepThemeColors` gate — the theme is always applied from `_defaultTheme ?? BeepThemesManager.CurrentTheme`. Added an explicit `dialog.ApplyTheme()` call after the theme name assignment.

```csharp
// Before — conditional, no ApplyTheme call
if (config.UseBeepThemeColors)
{
    var theme = _defaultTheme ?? BeepThemesManager.CurrentTheme;
    dialog.Theme = theme?.ThemeName ?? string.Empty;
    // ← ApplyTheme() never called
}

// After — unconditional + explicit ApplyTheme
{
    var theme = _defaultTheme ?? ThemeManagement.BeepThemesManager.CurrentTheme;
    if (theme != null)
        dialog.Theme = theme.ThemeName;
}
dialog.ApplyTheme();   // ← propagates theme to all child controls
```

---

## 4. Current Architecture

```
BeepDialogManager (instance, singleton, implements IDialogManager)
└── ShowAsync(DialogConfig) / Show(DialogConfig)
    └── ShowDialogInternal(config)
        └── CreateDialog(config)        ← creates BeepDialogModal
            ├── sets Title, Message
            ├── sets DialogType         ← triggers SetDialogType() once
            ├── sets DialogButtons      ← triggers SetDialogType() again (deduped by UnwireButtonHandlers)
            ├── sets Theme              ← resolved from _defaultTheme or CurrentTheme
            └── calls ApplyTheme()      ← propagates to all child controls

BeepDialogModal : BeepiFormPro
├── panel1 (Dock=Top)   — header: DialogImage + TitleLabel + CloseButton
└── panel3 (Dock=Fill)  — body:
    ├── CaptionTextBox              (always visible — shows Message)
    ├── InputTextBox                (visible only for GetInputString)
    ├── SelectFromListComboBox      (visible only for GetInputFromList)
    ├── LeftButton  / MiddleButton / RightButton
    └── inputVisibility dictionary drives SetInputTypeBasedonDialogType()
```

---

## 5. Files Modified

| File | Change |
|---|---|
| `Forms/BeepDialogModal.cs` | Rewrote `SetInputTypeBasedonDialogType`, added `SetInputHidden()`, added `UnwireButtonHandlers()` |
| `DialogsManagers/BeepDialogManager.Core.cs` | `CreateDialog()` — always applies theme, adds `ApplyTheme()` call |

---

## 6. Verification Checklist

| Scenario | Expected |
|---|---|
| `BeepDialogManager.Instance.Info("Title", "Msg")` | Shows title + message + single OK button, **no InputTextBox** |
| `BeepDialogManager.Instance.Warning("W", "msg")` | Shows title + message + OK, **no InputTextBox**, warning icon |
| `BeepDialogManager.Instance.ShowAsync(DialogConfig.CreateQuestion(...))` | Shows Yes / No buttons, **no InputTextBox** |
| `InputTextAsync("title", "prompt")` | Shows InputTextBox, OK + Cancel buttons |
| `InputSelectAsync("title", "prompt", items)` | Shows SelectFromListComboBox with items populated |
| Click OK button rapidly multiple times | Handler fires exactly once, dialog closes cleanly |
| Dark/Light theme toggle | Dialog controls reflect new theme colors on next open |

---

## 7. Do NOT Use (removed)

- `DialogManager` (static class) — deleted
- `BeepDialogForm` (painter-based form) — deleted
- Any `DialogManager.ShowDialog(...)` or `DialogManager.ShowAsync(...)` calls — these no longer compile

Use `BeepDialogManager.Instance` or inject `IDialogManager` via DI instead.
