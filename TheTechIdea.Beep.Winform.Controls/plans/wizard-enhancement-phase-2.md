# Phase 2 — DPI Scaling & Responsive Layout

**Status:** `[ ]` | **Tasks:** 10 | **Complexity:** Medium | **Depends on:** Phase 1 (WZ-08 refactor touches all forms)

---

## Objective

Eliminate all hardcoded pixel constants across painters, layout managers, templates, and forms. Create `WizardFormBase` to eliminate ~300 lines of duplicated code across the 4 form files. DPI-scale everything to render correctly at 96/125/150/200%.

## Background (from audit)

- **P0 fix needed:** `VerticalStepperPainter.cs` and `MinimalPainter.cs` have ZERO DPI scaling
- **P1 fix needed:** All 4 forms hardcode panel heights, button sizes, padding
- **Dead code:** `WizardLayoutManagers.cs` (210 lines) is never called by any form — forms use Dock layout directly
- **Duplication:** ~300 lines of identical code across 4 forms (button handling, validation display, animation dispatch, theme, keyboard)
- `WizardStepTemplates.cs` hardcodes Padding, icon sizes, and font sizes; also has a font disposal issue (disposing managed fonts from `BeepThemesManager`)
- `CardsWizardForm.cs` has inline GDI+ painting with hardcoded pixel values

## Tasks

### WZ-09 — DPI-Scale VerticalStepperPainter (Medium)
Apply `DpiScalingHelper.ScaleValue(value, _host)` to all hardcoded values following the pattern already established in `HorizontalStepperPainter`:
- `circleSize = 32` → scale
- `leftMargin = 30`, `topMargin = 40` → scale
- `itemHeight = Math.Min(80, ...)` → scale base value 80
- `textOffset = 15`, `size = 6` (checkmark), `size = 5` (error X) → scale
- Pen widths `2f` → scale

**File:** `Painters/VerticalStepperPainter.cs`

### WZ-10 — DPI-Scale MinimalPainter (Low)
Apply `DpiScalingHelper.ScaleValue` to:
- `dotSize = 10`, `dotSpacing = 24`
- `centerY = 25` in bounds calculation
- Ring expansion offsets `3`, `6`
- Title rect offset `15`, `25`

**File:** `Painters/MinimalPainter.cs`

### WZ-11 — DPI-Scale WizardLayoutManagers (Medium)
Replace all `const int` declarations with `protected virtual int` properties that call `DpiScalingHelper.ScaleValue`:
- `HorizontalStepperLayout`: StepIndicatorHeight(100), ButtonPanelHeight(70), SidePadding(30)
- `VerticalStepperLayout`: SidePanelWidth(280), ButtonPanelHeight(70), ContentPadding(30)
- `MinimalLayout`: HeaderHeight(60), ButtonPanelHeight(60), SidePadding(40)
- Add `CardsLayout` class for card-based layouts
- Add `Control Host` parameter to `Initialize()`

**File:** `Layout/WizardLayoutManagers.cs`

### WZ-12 — Wire Forms to Layout Managers (Medium)
Currently layout managers are dead code (never called). Wire them up:
- Each form creates appropriate `WizardLayoutManager` subclass
- `GetStepIndicatorBounds().Height` used for panel heights instead of hardcoded values
- `GetContentBounds()` used for content panel
- `GetButtonPanelBounds()` used for button panel positioning
- `HitTestStep(Point)` wired to mouse click for step navigation

**Files:** All 4 forms

### WZ-13 — Create WizardFormBase (High)
Extract shared code from all 4 forms into an abstract base class:
- Fields: `_activeAnimationTimers`, `_previousStepIndex`, `_cachedPages`, `_painter`, `_layout`
- Panels: `_contentPanel`, `_errorPanel`, `_buttonPanel`
- Buttons: `_btnNext`, `_btnBack`, `_btnCancel`, `_btnSkip`, `_btnHelp`
- Common methods: `InitializeControls()`, `LayoutControls()`, `SetupEventHandlers()`
- `UpdateUI()` core logic (page caching, animation, validation state)
- `ShowValidationError()`, `HideValidationError()`, `GetContentPanel()`
- Button click handlers (`BtnNext_Click`, `BtnBack_Click`, `BtnCancel_Click`, etc.)
- `Form_KeyDown`, `ApplyTheme()`, `OnFormClosing` cleanup
- Leave abstract/virtual: `InitializeStyleSpecific()`, `GetStylePainter()`
- Standardize button sizes: Next=120x40, Back=100x40, Cancel=100x40, Skip=100x40, Help=80x40

**Files:** Create `Forms/WizardFormBase.cs`, modify all 4 forms to inherit from it

### WZ-14 — Responsive Thresholds (Medium)
Add to `WizardLayoutManager`:
- `GetRecommendedMinimumSize()` → `Size`
- For HorizontalStepper: min width 600px
- For VerticalStepper: min width 500px (collapse side panel to icons-only below this)
- Forms enforce `MinimumSize` in constructor using scaled values

**File:** `Layout/WizardLayoutManagers.cs`, all 4 forms

### WZ-15 — DPI-Scale CardsWizardForm (Medium)
Scale all card painting values in `StepCard_Paint`:
- `_cardPanel.Width = 220` → scale
- Card `Height = 70`, `Margin(0,0,0,6)`, `Padding(10)` → scale
- `circleSize = 30`, `circleX = 14` → scale
- Text offset `12`, `10` → scale
- Accent bar offset `8`, size `16` → scale
- Pen widths `2f` → scale
- Font sizes: `13f`, `10f`, `8.5f`, `12f`, `7f` → scale

**File:** `Forms/CardsWizardForm.cs`

### WZ-16 — DPI-Scale WizardStepTemplates (Medium)
Scale all hardcoded values in the 4 templates:
- Welcome: panel `Height=100`, label `Height=50/100`, `Padding(40,40,40,40)`, fonts `24f/12f`
- Summary: label `Height=40`, `Padding(20/10)`, fonts `16f/10f`
- Completion/Error: panel `Height=80`, label `Height=50/60`, `Padding(40,60,40,40)`, fonts `22f/11f`, icon `size=60`, `y=10`, pen `4f`

**File:** `Templates/WizardStepTemplates.cs`

### WZ-17 — Fix Font Disposal Pattern (Low)
Templates call `?.Font?.Dispose()` on fonts from `WizardHelpers.GetFont()`, which returns managed fonts from `BeepThemesManager`. These should NOT be disposed by the consumer. Replace with nulling or remove Dispose calls. Also fix `SummaryStepTemplate.LoadData()` — creates a new `BeepLabel` on every call without clearing old labels (control leak).

**File:** `Templates/WizardStepTemplates.cs`

### WZ-18 — OnResize Recalculation (Low)
Add `OnResize` override to `WizardFormBase` that recalculates layout and invalidates step indicators. Use throttling to avoid excessive repaints during window resize drags.

**File:** `Forms/WizardFormBase.cs`

## Acceptance Criteria
- [ ] ZERO hardcoded `const int` pixel values in any painter or layout manager
- [ ] Wizards render correctly at 96, 125, 150, 200% DPI
- [ ] All touch targets ≥44px at 96 DPI
- [ ] `WizardFormBase` reduces form code by ~60%
- [ ] Layout managers are wired and functional (no longer dead code)
- [ ] Templates don't leak controls on repeated LoadData calls
- [ ] Build with 0 errors
