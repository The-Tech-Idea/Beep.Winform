# Theme Revision Plan

Goal: Ensure consistent, accessible, and complete theme color tokens across all ThemeTypes so controls (buttons, labels, textboxes, app bar, dialogs, etc.) use the correct theme values and produce consistent visuals across themes.

Scope:
- All theme subfolders under ThemeTypes (DefaultTheme, DarkTheme, LightTheme, HighlightTheme, etc.)
- Each theme's Parts/*.cs files that define BeepTheme parts (Button, Label, TextBox, AppBar, etc.)
- Control ApplyTheme implementations that consume theme tokens

Principles:
- Define a single canonical token set per theme (Primary, Secondary, Background, Surface, TextPrimary, TextSecondary, Disabled, Error, Success, Border, Hover, Pressed, Selected, Accent, etc.).
- Map every control state to one or more tokens (e.g., ButtonBackColor -> Surface or Primary depending on variant; ButtonForeColor -> TextOnPrimary/TextOnSurface).
- Ensure contrast accessibility: text on background should meet WCAG AA where applicable.
- Keep theme part files minimal and consistent (no duplicate or conflicting tokens).

Updated approach (important):
- Do not add new interface properties at this stage. Changes going forward will be limited to color value adjustments inside each theme's Parts/*.cs files.
- Default interface additions made earlier were to provide compatibility during rollout; from now on we will avoid introducing new IBeepTheme members. We will instead align existing tokens (ButtonBackColor, ButtonForeColor, ButtonBorderColor, etc.) to each theme's intended visual style.
- This reduces cross-theme implementation work and avoids breaking changes.

Phased approach (one theme at a time):
1. Discovery & Audit
   - List all token properties present in theme (Parts/*.cs)
   - Identify missing or inconsistent color values used by controls (search for _currentTheme.ButtonBackColor etc.)
   - Produce per-theme audit report (append to PROGRESS.md or a per-theme audit file)

2. Color Fixes (no new properties)
   - Update existing color token values inside theme Parts/*.cs to be consistent and semantically correct for that theme.
   - Prefer using existing ButtonBackColor/ButtonForeColor/ButtonBorderColor and state tokens (Pressed, Hover, Selected) rather than adding new token names.
   - Ensure LabelForeColor and Core.ForeColor are consistent for readable text.

3. Control mapping verification
   - Verify controls render correctly with the theme's tokens (no code changes to controls unless they use incorrect tokens)
   - If a control relies on a different semantic (e.g., expecting a primary filled button), adjust that control's ApplyTheme mapping to use the appropriate existing tokens.

4. Visual verification
   - Run sample app in multiple DPI/monitor setups
   - Verify each control state (normal, hover, pressed, disabled, selected)
   - Check contrast and fix colors as necessary

5. Automated checks
   - Add unit tests where possible for token existence and basic contrast rules
   - Maintain per-theme screenshots in ThemeTypes/<theme>/screenshots for manual review

6. Repeat for next theme

Deliverables per theme:
- Updated Parts/*.cs with corrected color values (no new properties)
- Small changelog in PROGRESS.md with tasks and status
- Visual verification notes and screenshots (if available)

Acceptance criteria:
- No missing tokens when a control requests a token
- Buttons, Labels, Textboxes, App bar, dialogs display consistent colors with theme intent
- Contrast for primary text >= WCAG AA (4.5:1) where practical
- No runtime exceptions due to missing theme properties

Tooling & search tips:
- Search usages: `_currentTheme.`, `BeepThemesManager.GetTheme`, `ApplyTheme()` implementations
- Prefer updating existing tokens like ButtonBackColor/ButtonForeColor/TextBoxBackColor rather than introducing new tokens

Timeline and responsibilities:
- Phase per theme: 0.5-2 days (audit + color fixes + verification) depending on scope
- Start with DefaultTheme (baseline) -> DarkTheme -> LightTheme -> MaterialDesignTheme -> others

Risk & mitigation:
- Risk: breaking controls that relied on legacy token names
  - Mitigation: make minimal color-only changes; avoid renaming tokens
- Risk: mis-tuned contrast
  - Mitigation: review with accessibility tool or simple color contrast calculator

Notes:
- Changes should avoid adding manual DPI scaling; rely on Framework DPI handling (AutoScaleMode).
- Keep changes incremental and small per theme to simplify verification.

"Do one theme at a time" workflow will be tracked in PROGRESS.md (one entry per theme with tasks and statuses).