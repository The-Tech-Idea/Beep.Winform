# BeepDatePicker — Theme, Font & Skill Compliance Plan

**Target file:** `TheTechIdea.Beep.Winform.Controls/Dates/BeepDatePicker.cs`
**Rules source:** `.cursor/skills/beep-winform/SKILL.md` + `reference.md`
**Status:** Non-compliant in 7 areas — detailed below with fix steps.

---

## 1. Violations Found

### 1.1 Font Violations (SKILL Rule 2)

| Location | Violation | Rule |
|---|---|---|
| `ApplyTheme()` | `catch { _textFont = this.Font; }` — fallback to `this.Font` is forbidden | Rule 2.3: Never use `this.Font` |
| `ApplyTheme()` | Same — outer catch branch also assigns `this.Font` | Rule 2.3 |
| `DrawDropdownArrow()` | Arrow drawn with no font context (minor, but the arrow color uses raw `ForeColor` rather than a theme token) | Rule 2 (font/colour must come from theme) |
| Field default | `private Font _textFont;` — field is uninitialized (null until `ApplyTheme`) | Rule 2.4: Resolve in `ApplyTheme()` only; guard all paint paths |

**Root problem:** When `BeepThemesManager.ToFont(_currentTheme.DateFont)` throws or returns null, the code silently falls back to `this.Font` which is a WinForms auto-scaled font — violating skill Rule 2 and potentially double-scaling the font (DPI issue from the DPI fix plan).

**Correct pattern:**
```csharp
// CORRECT — per reference.md ApplyTheme pattern
if (_currentTheme != null)
{
    _textFont = BeepThemesManager.ToFont(_currentTheme.DateFont)
             ?? BeepThemesManager.ToFont(_currentTheme.BodyMedium)
             ?? SystemFonts.DefaultFont;
}
```

- `this.Font` and `Control.Font` must never appear as font fallbacks.
- The safe fallback chain is: `DateFont → BodyMedium → SystemFonts.DefaultFont`.

---

### 1.2 Color Violations (SKILL Rule 2 / reference.md UseThemeColors Pattern)

| Location | Violation | Correct token |
|---|---|---|
| `Paint()` text drawing | Uses `_currentTheme.ComboBoxForeColor` | Should use `_currentTheme.CalendarForeColor` |
| `Paint()` placeholder drawing | Mixed: `_currentTheme.TextBoxPlaceholderColor` fallback to `BeepStyling.CurrentTheme?.TextBoxPlaceholderColor` | Should use dedicated CalendarForeColor + alpha dimming |
| `Paint()` divider line | Uses `_currentTheme.ComboBoxBorderColor` | Should use `_currentTheme.CalendarBorderColor` |
| `DrawDropdownArrow()` arrow | `new SolidBrush(ForeColor)` — raw WinForms property | Should use `_currentTheme.CalendarForeColor` or `_currentTheme.CalendarTitleForColor` |
| `DrawCalendarIcon()` icon tint | Uses `_currentTheme.CalendarTitleForColor` — correct, but missing `UseThemeColors` guard | Must check `UseThemeColors` before using theme |

**Missing `UseThemeColors` guard:** Every painter in the skill must follow the mandatory `UseThemeColors` switch pattern from `reference.md`:

```csharp
// MANDATORY pattern — reference.md §UseThemeColors Pattern
Color foreColor, borderColor, placeholderColor;
if (UseThemeColors && _currentTheme != null)
{
    foreColor       = _currentTheme.CalendarForeColor;
    borderColor     = _currentTheme.CalendarBorderColor;
    placeholderColor = Color.FromArgb(128, _currentTheme.CalendarForeColor);
}
else
{
    // Style-identity fallback (preserve visual identity when no theme)
    foreColor       = Color.FromArgb(230, 235, 241);
    borderColor     = Color.FromArgb(64, 69, 82);
    placeholderColor = Color.FromArgb(128, 230, 235, 241);
}
```

**Full colour token map for BeepDatePicker:**

| Purpose | Theme Token | Fallback |
|---|---|---|
| Input text | `_currentTheme.CalendarForeColor` | `ForeColor` |
| Placeholder text | `Color.FromArgb(128, CalendarForeColor)` | `Color.FromArgb(128, ForeColor)` |
| Background | `_currentTheme.CalendarBackColor` | `BackColor` |
| Border | `_currentTheme.CalendarBorderColor` | `BorderColor` |
| Divider line | `_currentTheme.CalendarBorderColor` | `BorderColor` |
| Calendar icon tint | `_currentTheme.CalendarTitleForColor` | `ForeColor` |
| Dropdown arrow | `_currentTheme.CalendarTitleForColor` | `ForeColor` |
| Hover background | `_currentTheme.CalendarHoverBackColor` | `ThemeUtil.Lighten(BackColor, 0.08)` |
| Selected date | `_currentTheme.CalendarSelectedDateBackColor` | `PrimaryColor` |
| Selected date text | `_currentTheme.CalendarSelectedDateForColor` | `OnPrimaryColor` |
| Today highlight | `_currentTheme.CalendarTodayForeColor` | `AccentColor` |
| Focus ring | `_currentTheme.FocusIndicatorColor` | `ActiveBorderColor` |
| Error border | `_currentTheme.ErrorColor` | `Color.Red` |

> All these tokens are properly set by `ArcLinuxTheme.ApplyCalendar()` (and all other themes that follow the same pattern). The control must consume them exclusively.

---

### 1.3 DPI / Layout Violations (SKILL Rule 5)

| Location | Violation | Fix |
|---|---|---|
| `private int _buttonWidth => 24;` | Hardcoded pixel constant | `DpiScalingHelper.ScaleValue(24, this)` |
| `private int _padding => 3;` | Hardcoded pixel constant | `DpiScalingHelper.ScaleValue(3, this)` |
| `DrawDropdownArrow()` | `arrowVisualSize = Math.Min(12, ...)` — hardcoded 12 | `DpiScalingHelper.ScaleValue(12, this)` |
| `GetButtonRect()` | `BorderThickness` added raw without scaling | Already scaled by `BaseControl`; confirm no double-scale |

**Correct pattern:**
```csharp
// CORRECT — SKILL Rule 5.1
private int ButtonWidth => DpiScalingHelper.ScaleValue(24, this);
private int Padding     => DpiScalingHelper.ScaleValue(3, this);
```

Use expression-body `=>` properties so the scale is recalculated fresh on each layout call (never cached as a field).

---

### 1.4 Image / Icon Violations (SKILL Rule 4)

| Location | Violation | Fix |
|---|---|---|
| `DrawCalendarIcon()` | Falls back to `calendarIcon.Draw(g, iconRect)` which bypasses `StyledImagePainter` | Always use `StyledImagePainter.PaintWithTint(...)` as the primary path |
| `calendarIcon` (BeepImage) | `BeepImage.Draw(g, rect)` is used directly | `StyledImagePainter` is the mandatory image rendering API (SKILL Rule 4.1) |
| Icon path | Resolved via `calendarIcon?.ImagePath` | Correct — string path required (Rule 4.3) |

**Correct `DrawCalendarIcon` pattern:**
```csharp
private void DrawCalendarIcon(Graphics g, Rectangle buttonRect)
{
    int pad = Math.Max(1, Padding);
    var iconRect = new Rectangle(
        buttonRect.X + pad, buttonRect.Y + pad,
        Math.Max(8, buttonRect.Width - 2 * pad),
        Math.Max(8, buttonRect.Height - 2 * pad));

    string imagePath = calendarIcon?.ImagePath;
    if (string.IsNullOrEmpty(imagePath)) { DrawDropdownArrow(g, buttonRect); return; }

    Color iconColor = (UseThemeColors && _currentTheme != null)
        ? _currentTheme.CalendarTitleForColor
        : ForeColor;

    int cornerRadius = Math.Max(0, BorderRadius);
    StyledImagePainter.PaintWithTint(g, iconRect, imagePath, iconColor, 1f, cornerRadius);
}
```

- Remove the `BeepImage.Draw()` fallback code path entirely.
- Remove the `try/catch` that silently swallows `StyledImagePainter` errors (log them instead).

---

### 1.5 ApplyTheme Pattern Violations (SKILL Rule 2.5 / reference.md)

| Violation | Fix |
|---|---|
| `catch { _textFont = this.Font; }` | Replace with safe chain (see §1.1) |
| No propagation to `BeepDatePickerView` (the popup calendar) | Propagate `CurrentTheme` + call `ApplyTheme()` on `_calendarView` in `ApplyTheme()` |
| `Invalidate()` called directly at end of `ApplyTheme()` | Replace with `InvalidateOnce()` (from DPI fix plan) |
| `calendarIcon.ApplyThemeOnImage = true` but `calendarIcon.CurrentTheme` not set | Set `calendarIcon.CurrentTheme = CurrentTheme` before setting flag (already correct but add `calendarIcon.Theme = Theme` consistency check) |

**Correct `ApplyTheme()` skeleton:**
```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();

    // 1. Calendar icon
    if (calendarIcon != null)
    {
        calendarIcon.CurrentTheme = CurrentTheme;
        calendarIcon.Theme = Theme;
        calendarIcon.ApplyThemeOnImage = true;
        calendarIcon.ApplyTheme();
    }

    // 2. Fonts — per SKILL Rule 2, never use this.Font
    if (_currentTheme != null)
    {
        _textFont = BeepThemesManager.ToFont(_currentTheme.DateFont)
                 ?? BeepThemesManager.ToFont(_currentTheme.BodyMedium)
                 ?? SystemFonts.DefaultFont;
    }

    // 3. Visual colours from Calendar tokens
    if (_currentTheme != null)
    {
        if (_currentTheme.CalendarBackColor   != Color.Empty) BackColor   = _currentTheme.CalendarBackColor;
        if (_currentTheme.CalendarForeColor   != Color.Empty) ForeColor   = _currentTheme.CalendarForeColor;
        if (_currentTheme.CalendarBorderColor != Color.Empty) BorderColor = _currentTheme.CalendarBorderColor;
    }

    // 4. Propagate to popup calendar view
    if (_calendarView != null)
    {
        _calendarView.CurrentTheme = CurrentTheme;
        _calendarView.Theme = Theme;
        _calendarView.ApplyTheme();
    }

    // 5. Pre-render icon cache (keep existing Task.Run block)
    // ... existing icon prerender code ...

    // 6. Invalidate once (batched — no cascade redraws)
    InvalidateOnce();
}
```

---

### 1.6 Paint Method Violations (SKILL Rule 6 / reference.md DrawContent Pattern)

| Violation | Fix |
|---|---|
| `Paint(g, bounds)` called from both `DrawContent()` and `Draw()` — two entry points diverge | Acceptable; but ensure both always resolve `contentRect` identically |
| `TextRenderer.DrawText` uses `_currentTheme.ComboBoxForeColor` | Use `CalendarForeColor` (see §1.2 colour map) |
| Divider pen uses `ComboBoxBorderColor` | Use `CalendarBorderColor` |
| Arrow brush uses raw `ForeColor` | Use `CalendarTitleForColor` from theme (guarded by `UseThemeColors`) |
| `_currentTheme == null` is the only guard — no `UseThemeColors` guard | Add full `UseThemeColors` guard (see §1.2) |

---

### 1.7 BeepDatePickerView (Popup Calendar) — Theme Propagation Gap

The popup calendar (`BeepDatePickerView`, inside `_calendarView`) contains its own drawing logic for:
- Day cells (selected, today, hover, disabled, highlighted)
- Week header row
- Month/year navigation header
- Footer

All of these must use the dedicated `IBeepTheme` calendar tokens — not hard-coded colours, not re-read from `SystemColors`, and not inherited from WinForms ambient properties.

**Required audit checklist for `BeepDatePickerView`:**

- [ ] Day cell background → `CalendarBackColor`
- [ ] Day cell text → `CalendarForeColor`
- [ ] Selected day background → `CalendarSelectedDateBackColor`
- [ ] Selected day foreground → `CalendarSelectedDateForColor`
- [ ] Today ring/highlight → `CalendarTodayForeColor`
- [ ] Hover day overlay → `CalendarHoverBackColor`
- [ ] Header background → `CalendarBackColor` (or `SurfaceColor`)
- [ ] Header text → `CalendarTitleForColor`
- [ ] Day column headers (Mon–Sun row) → `CalendarDaysHeaderForColor`
- [ ] Footer / nav arrows → `CalendarFooterColor`
- [ ] Border → `CalendarBorderColor`
- [ ] Navigation arrows icon → `StyledImagePainter.PaintWithTint(...)` with `CalendarTitleForColor`
- [ ] All fonts resolved via `BeepThemesManager.ToFont(theme.XxxStyle)` — never inline `new Font()`

---

## 2. Implementation Steps

### Step 1 — Fix fonts in `ApplyTheme()` [ BeepDatePicker.cs ]
- Remove `catch { _textFont = this.Font; }` blocks.
- Replace with safe fallback chain: `DateFont → BodyMedium → SystemFonts.DefaultFont`.
- Initialize `_textFont = SystemFonts.DefaultFont;` as field default.

### Step 2 — Fix colour resolution in `Paint()` [ BeepDatePicker.cs ]
- Add `UseThemeColors` guard at top of `Paint()`.
- Replace all `ComboBoxForeColor` / `ComboBoxBorderColor` references with Calendar tokens.
- Replace placeholder colour fallback chain.

### Step 3 — Fix arrow colour [ BeepDatePicker.cs — `DrawDropdownArrow()` ]
- Replace `new SolidBrush(ForeColor)` with `new SolidBrush(CalendarTitleForColor from resolved colours)`.

### Step 4 — Fix `DrawCalendarIcon()` [ BeepDatePicker.cs ]
- Remove `BeepImage.Draw()` fallback.
- All icon paint via `StyledImagePainter.PaintWithTint()` only.
- Guard icon colour with `UseThemeColors`.

### Step 5 — Fix DPI constants [ BeepDatePicker.cs ]
- Replace `private int _buttonWidth => 24;` with `private int ButtonWidth => DpiScalingHelper.ScaleValue(24, this);`
- Replace `private int _padding => 3;` with `private int Padding => DpiScalingHelper.ScaleValue(3, this);`
- Replace all `_buttonWidth` / `_padding` usages with `ButtonWidth` / `Padding`.
- Replace hardcoded `12` in `DrawDropdownArrow()` with `DpiScalingHelper.ScaleValue(12, this)`.

### Step 6 — Fix `ApplyTheme()` end call [ BeepDatePicker.cs ]
- Replace `Invalidate()` with `InvalidateOnce()`.
- Add calendar view propagation (see §1.5 skeleton).

### Step 7 — Audit BeepDatePickerView [ BeepDatePickerView.cs ]
- Review each drawing section against the audit checklist in §1.7.
- Resolve all fonts via `BeepThemesManager.ToFont(...)`.
- Replace raw colour literals with theme token reads.
- Propagate `CurrentTheme` from `BeepDatePicker.ApplyTheme()` → `_calendarView.ApplyTheme()`.

---

## 3. Quick-Reference: Skill Rules Applied

| Rule | Requirement | Applied Here |
|---|---|---|
| SKILL 2.1 | Never assign `Font` directly on controls | Steps 1, 7 |
| SKILL 2.2 | Never create inline `new Font(...)` | Steps 1, 7 |
| SKILL 2.3 | Never use `this.Font` / `Control.Font` | Step 1 |
| SKILL 2.4 | Fonts from theme typography via `BeepThemesManager.ToFont()` | Steps 1, 7 |
| SKILL 2.5 | Resolve fonts in `ApplyTheme()`, store in `_textFont` | Step 1 |
| SKILL 2.6 | Re-resolve fonts on theme change | Step 6 (via InvalidateOnce) |
| SKILL 4.1 | Use `StyledImagePainter.Paint/PaintWithTint/PaintDisabled` | Step 4 |
| SKILL 4.3 | Icon as string path, not raw Image object | Already compliant |
| SKILL 5.1 | Scale all hardcoded dimensions via `DpiScalingHelper.ScaleValue` | Step 5 |
| ref UseThemeColors | Guard ALL colour reads with `UseThemeColors && CurrentTheme != null` | Step 2 |
| ref ApplyTheme | Propagate theme to all child controls | Step 6 |

---

## 4. File Change Summary

| File | Steps | Impact |
|---|---|---|
| `Dates/BeepDatePicker.cs` | 1–6 | Font safety, colour correctness, DPI compliance, icon rendering |
| `Dates/BeepDatePickerView.cs` | 7 | Full theme token audit for calendar popup |

---

*End of plan.*
