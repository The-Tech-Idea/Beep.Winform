# Phase 4 — Painter Expansion & Visual Polish

**Status:** `[ ]` | **Tasks:** 8 | **Complexity:** High | **Depends on:** Phase 3 (navigation infra for connector animation)

---

## Objective

Create `CardsPainter` to fix the architectural inconsistency (CardsWizardForm is the only style without a dedicated painter). Add animated connector fills, hover effects, SVG icon rendering, and centralized theme color resolution.

## Background

- `CardsWizardForm.StepCard_Paint()` is ~100 lines of inline GDI+ code — should be a dedicated `CardsPainter` like the other 3 styles
- Connecting lines between steps are static-colored with no animation
- Error state drawing exists (ErrorX icon) but connectors don't reflect errors
- `WizardStep.Icon` (SVG path) exists but is not rendered by any painter
- `IWizardPainter` interface lacks a uniform `PaintStepIndicators` method — painters have ad-hoc paint methods
- Each painter duplicates Initialize logic for theme color resolution

## Tasks

### WZ-29 — Create CardsPainter (High)
Extract all `StepCard_Paint` logic into `CardsPainter : IWizardPainter`:
- Move circle drawing, checkmark, error X, number, title, description, "Optional" badge
- Move accent bar and background painting
- CardsWizardForm delegates to `_painter.PaintStepIndicators(...)`
- Preserve exact pixel-perfect rendering (then DPI-scale in Phase 2)

**Files:** Create `Painters/CardsPainter.cs`; modify `Forms/CardsWizardForm.cs`

### WZ-30 — Animated Connector Fill (Medium)
For horizontal and vertical painters, animate the connector line fill from 0%→100% during step transitions:
- Use a secondary color overlay that slides in from left/top
- Integrate with `TransitionDurationMs` and selected easing
- Track "animation in progress" per connector segment

**Files:** `Painters/HorizontalStepperPainter.cs`, `Painters/VerticalStepperPainter.cs`, `Helpers/WizardAnimationEngine.cs`

### WZ-31 — Error-State Connector Coloring (Low)
When a step is in `StepState.Error`, color the connecting line leading to it in red. Check step state in painter color logic.

**Files:** `Painters/HorizontalStepperPainter.cs`, `Painters/VerticalStepperPainter.cs`, `Painters/CardsPainter.cs`

### WZ-32 — Standardize IWizardPainter Interface (Medium)
Add formal `PaintStepIndicators` method to `IWizardPainter`:
```csharp
void PaintStepIndicators(Graphics g, Rectangle bounds, int currentIndex, int totalSteps, IList<WizardStep> steps);
```
Make all 4 painters implement this uniformly. Forms call `_painter.PaintStepIndicators(...)` without knowing painter type.

**Files:** All 4 painters, `Forms/WizardFormBase.cs`

### WZ-33 — Radial Progress for Minimal Style (Medium)
Add `WizardConfig.ShowRadialProgress` option. When enabled, MinimalPainter draws a circular arc progress indicator instead of dots. Arc fills clockwise as progress increases. Arc uses `Graphics.DrawArc` with thick pen.

**Files:** `Core/WizardModels.cs`, `Painters/MinimalPainter.cs`

### WZ-34 — Themed Gradient + Shadow on Step Circles (Medium)
Add optional gradient fill (top-to-bottom lighten) and drop shadow (blurred ellipse beneath) on step circles for the current step. Add `IWizardPainter` configuration for flat vs elevated rendering.

**Files:** All 4 painters

### WZ-35 — SVG Icon Rendering (Medium)
When `WizardStep.Icon` is set (SVG path string), render the icon inside step circles instead of numbers:
- Create `WizardIconRenderer` using `StyledImagePainter.PaintWithTint`
- Resolve icon path via `StepperIconHelpers` or direct `SvgsUI` constants
- Fall back to step numbers if no icon or icon fails to load
- Scale icon to fit within circle with padding

**Files:** Create `Helpers/WizardIconRenderer.cs`; modify all 4 painters

### WZ-36 — Step Description in HorizontalStepper (Low)
`WizardStep.Description` is rendered in VerticalStepper (for current step) but not in HorizontalStepper. Add a second line of smaller text below the title for the current step only.

**File:** `Painters/HorizontalStepperPainter.cs`

## Acceptance Criteria
- [ ] All 4 styles have dedicated painter classes
- [ ] `IWizardPainter` exposes uniform `PaintStepIndicators` method
- [ ] Connector lines animate fill during transitions
- [ ] Error steps show red connectors
- [ ] Minimal style has optional radial progress mode
- [ ] SVG icons render in step circles when `Icon` is set
- [ ] Build with 0 errors
