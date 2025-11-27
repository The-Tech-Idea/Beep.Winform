## Theme Fix & Enhancement Plan

This file tracks how themes should be refactored and enhanced so they are all:

- **Palette‑driven**: Each theme must get all base colors from its `Parts/BeepTheme.ColorPalette.cs` file.
- **Consistent with Style system**: Colors should line up with `StyleColors` / painters for the corresponding `BeepControlStyle`.
- **Modular**: All other partials (`Core`, `Buttons`, `Card`, etc.) must reference palette properties, not hard‑coded ARGB for role colors.

---

### 1. General refactor rules

- **Color source of truth**
  - All theme‑wide colors (background, surface, primary, secondary, accent, error, warning, success, on‑colors, focus/indicator) live in `BeepTheme.ColorPalette.cs`.
  - Other parts should reference these properties, e.g.:
    - `PrimaryColor`, `SecondaryColor`, `AccentColor`
    - `BackColor`, `BackgroundColor`, `SurfaceColor`
    - `ForeColor`, `OnPrimaryColor`, `OnBackgroundColor`
    - `ErrorColor`, `WarningColor`, `SuccessColor`
    - `FocusIndicatorColor`, `BorderColor`, `ActiveBorderColor`, etc.

- **Avoid hard‑coded `Color.FromArgb` in feature parts** for role colors:
  - OK for very local visual tweaks (e.g. overlay alpha, tiny light/dark adjustments).
  - Not OK for main roles that should be shared across the theme.

- **Align with `StyleColors` and painters**:
  - For each `BeepControlStyle` associated with the theme (e.g. `BeepControlStyle.Neon` ↔ `NeonTheme`):
    - `StyleColors.GetBackground(style)` should roughly match `theme.BackgroundColor` / `BackColor`.
    - `StyleColors.GetPrimary(style)` should roughly match `theme.PrimaryColor` / `AccentColor`.
    - `StyleColors.GetSecondary(style)` / `GetSurface(style)` should roughly match `theme.SurfaceColor` / card surfaces.
  - If there is a mismatch, adjust either the theme palette or `StyleColors` minimally so backgrounds, borders, shadows, and path painters look coherent.

---

### 2. Per‑theme checklist

For each theme directory under `Themes` (e.g. `NeonTheme`, `MinimalTheme`, `NordTheme`, etc.):

1. **Color palette**
   - [ ] Open `Parts/BeepTheme.ColorPalette.cs`.
   - [ ] Verify all base fields are present and sensible:
     - [ ] `ForeColor`, `BackColor`, `BackgroundColor`, `SurfaceColor`
     - [ ] `PrimaryColor`, `SecondaryColor`, `AccentColor`
     - [ ] `BorderColor`, `ActiveBorderColor`, `InactiveBorderColor` (if used)
     - [ ] `ErrorColor`, `WarningColor`, `SuccessColor`
     - [ ] `OnPrimaryColor`, `OnBackgroundColor`, `FocusIndicatorColor`
   - [ ] Ensure palette matches the intended design system / brand.

2. **Core + key parts**
   - [ ] Open `Parts/BeepTheme.Core.cs`, `Buttons.cs`, `Card.cs`, `TextBox.cs`, etc.
   - [ ] Replace any repeated ARGB role colors with references to palette properties.
   - [ ] Ensure disabled/hover/pressed states are derived from palette colors via lightening/darkening, not unrelated constants.

3. **Integration with style painters**
   - [ ] Map from this theme to its primary `BeepControlStyle`(s).
   - [ ] Inspect `StyleColors` entry for that style and align background/primary/secondary if needed.
   - [ ] Visually confirm the theme + backgrounds/borders/shadows + path painters look consistent.

4. **Accessibility / contrast (optional but recommended)**
   - [ ] Check primary text on background meets reasonable contrast (refer to `plansfixtheme_contrast_report.md` as needed).
   - [ ] Adjust `ForeColor`/`BackgroundColor` or `On*` colors if necessary.

---

### 3. Suggested theme order

Start with styles that already have strong painter + color definitions, then propagate patterns:

1. **NeonTheme** (done for palette + painters; use as reference)
2. **MinimalTheme**
3. **NordTheme / NordicTheme**
4. **SolarizedTheme / GruvBoxTheme / OneDarkTheme / DraculaTheme**
5. **MetroTheme / Metro2Theme / KDETheme / GNOMETheme / UbuntuTheme**
6. **PaperTheme / HolographicTheme / CyberpunkTheme / ArcLinuxTheme / BrutalistTheme**
7. **iOSTheme / MacOSTheme / GlassTheme / ChatBubbleTheme / CartoonTheme / FluentTheme**
8. **Any remaining custom themes**

For each, apply the checklist above and update this file by marking items as completed if you want to track progress.


