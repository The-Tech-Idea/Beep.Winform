# Phase 08 — Design-Time Wizard Polish (Theming & Accessibility)

## Phase Metadata
- Phase: `08`
- Program: `DocumentHost MDI Commercial-Grade Enhancement`
- Tracking Prefix: `DOCMDI-P8-`
- Scope:
  - `TheTechIdea.Beep.Winform.Controls.Design.Server/Dialogs/DocumentSetupWizardDialog.cs`
  - `TheTechIdea.Beep.Winform.Controls.Design.Server/Dialogs/WizardPalette.cs`
- Status: `Shipped — Theming + Accessibility Wave 1`
- Owner: Design-time UX
- Predecessors: Phase 07 (`DOCMDI-P7-*` design-time first slice).

## Goal
Make the `DocumentSetupWizardDialog` (the first-drop wizard that auto-wires a
`BeepDocumentHost` + view) feel like the rest of the Beep design language —
colours, typography, and accessibility — by pulling everything through the
existing `BeepThemesManager` / `IBeepTheme` system rather than hard-coding
chrome colours.

## Design Decision — Why a Palette Adapter, Not Direct Theme Access
The wizard is a `Form` (not a `BaseControl`), so the standard `ApplyTheme`
override pattern doesn't apply. Instead we use a one-shot **adapter**:

1. `WizardPalette` constructs once per dialog open.
2. It calls `BeepThemesManager.CurrentTheme ?? BeepThemesManager.GetDefaultTheme()`
   and projects `IBeepTheme` tokens (`BackColor`, `PanelBackColor`,
   `AccentColor`, `BorderColor`, `SurfaceColor`, etc.) onto a small,
   wizard-specific named-colour set (chrome, tile, preview, footer).
3. Derived shades (hover, idle-tab, MDI surface) are computed via `MixColor`
   so we don't bake a second palette — they always track the active theme.
4. Fallback to `SystemColors` only when:
   - OS High Contrast is on (accessibility request — honour it over custom theme).
   - Theme discovery fails (rare design-time path with no theme assembly loaded).

This matches commercial wizards (DevExpress Template Gallery, VS New Project
dialog) — palette is sampled once at open time, no mid-dialog hot-swap.

## TODO Checklist

### Theming
- [x] `DOCMDI-P8-001` Create `WizardPalette` adapter over `BeepThemesManager.CurrentTheme`.
- [x] `DOCMDI-P8-002` Wire form / header / tiles container / options strip / preview / footer through `_palette`.
- [x] `DOCMDI-P8-003` Pass `WizardPalette` into nested `PreviewPanel` and repaint Tabbed / Browser / MDI previews from palette tokens.
- [x] `DOCMDI-P8-004` Wire `ModeTile` background, hover, selected, border, title, description, and badge colours from palette.
- [x] `DOCMDI-P8-005` Wire Apply / Configure-Later buttons to `_palette.Accent` / `AccentFore` / `FormSurface` / `Separator`.
- [x] `DOCMDI-P8-006` Wire warning host-picker strip + warning label to `_palette.WarningBack` / `WarningFore`.
- [x] `DOCMDI-P8-007` OS High Contrast: detect via `SystemInformation.HighContrast`, skip Beep theme and use `SystemColors`.

### Accessibility (Wave 1)
- [x] `DOCMDI-P8-010` Dialog itself: `AccessibleName`, `AccessibleDescription`, `AccessibleRole.Dialog`.
- [x] `DOCMDI-P8-011` Header label: `AccessibleName` + `AccessibleRole.StaticText` (no `Heading` role exists in WinForms `AccessibleRole`).
- [x] `DOCMDI-P8-012` Tiles container: `AccessibleName` + `AccessibleRole.Grouping`.
- [x] `DOCMDI-P8-013` Each `ModeTile`: `AccessibleName` (with "(recommended)" suffix where applicable), `AccessibleDescription`, `AccessibleRole.RadioButton`.
- [x] `DOCMDI-P8-014` Options strip + each checkbox / spinner / combo: `AccessibleName`.
- [x] `DOCMDI-P8-015` Host-picker strip + combo: `AccessibleName`, `AccessibleDescription`.
- [x] `DOCMDI-P8-016` Preview panel: `AccessibleName`, `AccessibleRole.Graphic`.
- [x] `DOCMDI-P8-017` Footer status label: `AccessibleName`, `AccessibleRole.StaticText`.
- [x] `DOCMDI-P8-018` Apply / Configure-Later buttons + "Don't show again" checkbox: `AccessibleName`.

### Verification
- [x] `DOCMDI-P8-020` `dotnet build TheTechIdea.Beep.Winform.Controls.Design.Server.csproj` succeeds (0 errors, pre-existing warnings only).
- [x] `DOCMDI-P8-021` No remaining hard-coded chrome colours in `DocumentSetupWizardDialog.cs` outside the static brand glyph tile-icon painters.
- [x] `DOCMDI-P8-022` Linter shows no new diagnostics on the changed files.

## Files Touched
- `TheTechIdea.Beep.Winform.Controls.Design.Server/Dialogs/WizardPalette.cs` — new adapter.
- `TheTechIdea.Beep.Winform.Controls.Design.Server/Dialogs/DocumentSetupWizardDialog.cs` — palette wiring + accessibility metadata.
- `.plans/MASTER-TODO-TRACKER.md` — added `DOCMDI-NEXT-017`.

## Deferred / Backlog
These items are intentionally left for a follow-up Wave 2 to keep this slice
reviewable:

- [ ] `DOCMDI-P8-030` Tile brand-glyph painters (`PaintTabbedIcon`, `PaintBrowserIcon`, `PaintMdiIcon`) — keep current illustrated palette; revisit only if visually clashes under a custom Beep theme.
- [ ] `DOCMDI-P8-031` Live theme hot-swap (`BeepThemesManager.ThemeChanged` -> rebuild palette + invalidate) — not commercial-wizard pattern, treat as opt-in.
- [x] `DOCMDI-P8-032` Screen-reader live-region announcement on tile selection — `ModeTile.RaiseSelectionAccessibilityEvent()` fires `AccessibleEvents.SelectionAdd` + `StateChange`, and the status label now lives behind a small `AnnouncingLabel` subclass that re-broadcasts `NameChange` whenever the mode prose changes. Shipped alongside Phase 09 (`DOCMDI-NEXT-018`).
- [ ] `DOCMDI-P8-033` Localisation pass (English-only strings).
- [ ] `DOCMDI-P8-034` Telemetry on tile selection / "Configure Later" / "Don't show again".
- [ ] `DOCMDI-P8-035` Embed the wizard inline in the smart-tag for "no-dialog" repeat-configure.
- [ ] `DOCMDI-P8-036` Layout template catalogue beyond the four built-in templates.

## Verification Criteria
1. Switching `BeepThemesManager.CurrentTheme` (Light / Dark / Material You) before
   opening the wizard produces visually correct chrome, tiles, preview, and
   footer without re-introducing white-on-white or low-contrast pairings.
2. With OS High Contrast on, the wizard renders with system high-contrast
   colours regardless of Beep theme.
3. Narrator (or any UIA inspector) reports a meaningful `Name` /
   `Description` / `Role` on the dialog itself, the heading, the three mode
   tiles (including the "recommended" marker), the starter-content checkboxes
   and spinner, the host-picker combo, the preview, the status label, the
   "Don't show again" checkbox, and both footer buttons.

## Notes
- The wizard is design-time only; it never participates in the runtime
  `ApplyTheme(IBeepTheme)` lifecycle that `BaseControl` uses. We deliberately
  do not subscribe to `BeepThemesManager.ThemeChanged` because re-painting a
  modal mid-flow is not the commercial-wizard pattern.
- The brand-glyph tile icons (the small illustrative drawings inside each
  mode tile) are intentionally not themed — they're miniature schematic
  artwork meant to read at a glance, similar to the screenshots in the VS
  "New Project" dialog. If a future Beep theme makes them clash, we revisit
  via `DOCMDI-P8-030`.
