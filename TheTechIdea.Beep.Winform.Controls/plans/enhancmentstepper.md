# Beep Steppers Commercial UI/UX Modernization Plan

**Reference direction:** Latest Figma component patterns · DevExpress WizardControl/StepProgressBar standards · Material Design 3 Stepper · Fluent UI 2 Stepper · Ant Design Steps · Radix UI Steps · Web Content Accessibility Guidelines 2.2  
**Target area:** `TheTechIdea.Beep.Winform.Controls/Steppers`  
**Last updated:** 2026-02-28

---

## 1. Goal

Elevate `BeepStepperBar` and `BeepStepperBreadCrumb` from a monolithic single-file implementation to a full commercial-grade stepper framework that equals or exceeds DevExpress WizardControl, Ant Design Steps, and Material Design 3 Stepper in:

- **Visual richness** — 12+ distinct painter styles, animated transitions, icon rendering
- **Interaction fidelity** — hover, press, focus-ring, ripple, keyboard nav, drag-reorder
- **Framework compliance** — `BeepThemesManager.ToFont`, `DpiScalingHelper`, `StyledImagePainter`, `BackgroundPainterFactory`, `BorderPainterFactory`, `ControlHitTestHelper`
- **Painter architecture** — full partial-class decomposition, distinct painters per style, no base-painter inheritance tree
- **Extensibility** — open `IStepperPainter` contract, registry-driven style lookup, optional sub-step nesting

---

## 2. Current State Assessment

### What exists

| Layer | File | Status |
|---|---|---|
| Control | `BeepSteppperBar.cs` (~1400 lines, monolithic) | ⚠️ Needs decomposition |
| Control | `BeepStepperBreadCrumb.cs` (~890 lines) | ⚠️ Needs decomposition |
| Helpers | `StepperFontHelpers.cs` | ⚠️ Uses `BeepFontManager` – must migrate to `BeepThemesManager.ToFont` |
| Helpers | `StepperThemeHelpers.cs` | ✅ Good – centralized color tokens |
| Helpers | `StepperIconHelpers.cs` | ✅ Uses `StyledImagePainter` |
| Helpers | `StepperAccessibilityHelpers.cs` | ✅ Reduced motion, high contrast |
| Helpers | `StepperStyleHelpers.cs` | ✅ ControlStyle → metrics mapping |
| Models | `StepperStyleConfig.cs` | ✅ DPI-aware metrics model |
| Models | `StepperColorConfig.cs` | ✅ Color config model |
| Painters | None | ❌ Missing entirely |
| Layout | None (inline in OnPaint) | ❌ Missing `LayoutManager` |
| Hit testing | Direct `buttonBounds` list | ⚠️ Must migrate to `ControlHitTestHelper` |
| Animations | `Timer` + `animationProgress float` | ⚠️ No easing, no ripple, no spring |

### Critical framework violations

1. **Font assignment**: `StepperFontHelpers` uses `BeepFontManager` instead of `BeepThemesManager.ToFont(TypographyStyle)`. No direct `Font = ...` or `new Font(...)` allowed.
2. **Hardcoded colors**: `completedStepColor = Color.FromArgb(34, 197, 94)` — must come from theme tokens.
3. **No painter separation**: all drawing is inline inside one OnPaint method — violates painter architecture.
4. **No `ControlHitTestHelper`**: step hit areas registered as raw `List<Rectangle>`, bypassing the framework's hit system.
5. **DPI scaling**: some values use `DpiScalingHelper.ScaleValue` but many are still hardcoded constants (e.g., `connectorLineWidth = 2`).
6. **No `BackgroundPainterFactory` / `BorderPainterFactory`** calls for container rendering.

---

## 3. Commercial Reference Standards

### DevExpress WizardControl / StepProgressBar
- Distinct visual styles: Classic, Metro, Fluent, Native, Touch
- Per-step icons, badges, validation state, optional subtitle
- Animated connector fill during step transition
- Keyboard: Tab moves focus between steps, Space/Enter activates
- Design-time smart tags for step collection

### Ant Design Steps
- Horizontal, vertical, inline, dot, navigation styles
- Step subtitle support
- Error state with red icon + tooltip
- Responsive collapse to "mini" on small containers
- Progress-filled connector line proportional to sub-step progress

### Material Design 3 Stepper
- Filled, outlined, tonal variants for node
- Focus ring: 3px offset, theme `outline` token
- Active step uses `primary` fill, check uses `onPrimary`
- Connector: 1px `outlineVariant` inactive, 2px `primary` completed
- Spring-based easing for node scale (200ms cubic-bezier)

### Figma / Web Standards
- 4/8/12/16/24 spacing rhythm for all gaps
- States: default, hover (+2dp elevation token), focused (focus ring), pressed (ripple), disabled (38% opacity)
- Accessible name: `role="tab"`, `aria-selected`, `aria-disabled`
- Touch target: minimum 44×44px always enforced

---

## 4. Target Architecture

### 4.1 File Structure After Enhancement

```
Steppers/
├── BeepStepperBar.cs                       ← properties, events, constructor
├── BeepStepperBar.Layout.cs               ← ComputeLayout(), StepBounds, connector rects
├── BeepStepperBar.Painters.cs             ← painter wiring, IStepperPainter dispatch
├── BeepStepperBar.HitTest.cs              ← ControlHitTestHelper registration, mouse/key events
├── BeepStepperBar.Interactions.cs         ← step selection, Next/Prev, keyboard nav
├── BeepStepperBar.Animations.cs           ← timer-based easing, ripple, connector fill
├── BeepStepperBar.DesignTime.cs           ← design-mode sample data, smart tags
├── BeepStepperBreadCrumb.cs               ← chevron breadcrumb (parallel decomposition)
├── BeepStepperBreadCrumb.Layout.cs
├── BeepStepperBreadCrumb.Painters.cs
├── BeepStepperBreadCrumb.HitTest.cs
├── BeepStepperBreadCrumb.Interactions.cs
├── Helpers/
│   ├── StepperFontHelpers.cs              ← migrate to BeepThemesManager.ToFont
│   ├── StepperThemeHelpers.cs             ← ✅ keep, extend
│   ├── StepperIconHelpers.cs              ← ✅ keep, extend
│   ├── StepperAccessibilityHelpers.cs     ← ✅ keep, extend
│   ├── StepperStyleHelpers.cs             ← ✅ keep, extend
│   ├── StepperLayoutHelper.cs             ← NEW: pure geometry, no Graphics
│   └── StepperAnimationEasing.cs         ← NEW: easing functions (Spring, EaseOut, Linear)
├── Models/
│   ├── StepperStyleConfig.cs              ← ✅ extend
│   ├── StepperColorConfig.cs              ← ✅ extend
│   ├── StepModel.cs                       ← NEW: per-step data model (replaces SimpleItem)
│   ├── StepPainterContext.cs              ← NEW: snapshot passed to painter per frame
│   └── StepAnimationState.cs             ← NEW: per-step animation state (progress, ripple)
├── Painters/
│   ├── IStepperPainter.cs                 ← NEW: painter contract
│   ├── StepperPainterRegistry.cs         ← NEW: name → painter lookup
│   ├── CircularNodeStepperPainter.cs      ← NEW: Style 1
│   ├── ChevronBreadcrumbStepperPainter.cs ← NEW: Style 2
│   ├── SquareDashedStepperPainter.cs      ← NEW: Style 3
│   ├── SegmentedTabStepperPainter.cs      ← NEW: Style 4
│   ├── ProgressBarStepperPainter.cs       ← NEW: Style 5
│   ├── DotsStepperPainter.cs              ← NEW: Style 6
│   ├── IconTimelineStepperPainter.cs      ← NEW: Style 7
│   ├── VerticalTimelineStepperPainter.cs  ← NEW: Style 8
│   ├── GradientMaterialStepperPainter.cs  ← NEW: Style 9
│   ├── BadgeStatusStepperPainter.cs       ← NEW: Style 10
│   ├── AlternatingTimelineStepperPainter.cs ← NEW: Style 11
│   └── CompactInlineStepperPainter.cs     ← NEW: Style 12
```

---

## 5. IStepperPainter Contract

```csharp
// Steppers/Painters/IStepperPainter.cs
public interface IStepperPainter
{
    string Name { get; }

    // Called once when painter is assigned or theme changes
    void Initialize(BaseControl owner, IBeepTheme theme,
                    Font stepFont, Font labelFont, Font numberFont);

    // Called during layout invalidation (no Graphics needed)
    StepperLayoutResult ComputeLayout(Rectangle clientRect,
                                      IReadOnlyList<StepModel> steps,
                                      Orientation orientation,
                                      StepperStyleConfig styleConfig);

    // Main paint entry point
    void Paint(Graphics g, StepPainterContext ctx);

    // Per-step painting (called by Paint for each step)
    void PaintStep(Graphics g, StepPainterContext ctx, int stepIndex, Rectangle stepRect);

    // Connector between step[i] and step[i+1]
    void PaintConnector(Graphics g, StepPainterContext ctx, int fromIndex, Rectangle connectorRect);
}
```

### StepPainterContext

```csharp
public sealed class StepPainterContext
{
    public Graphics                    Graphics       { get; init; }
    public Rectangle                   DrawingRect    { get; init; }
    public IBeepTheme                  Theme          { get; init; }
    public bool                        UseThemeColors { get; init; }
    public IReadOnlyList<StepModel>    Steps          { get; init; }
    public IReadOnlyList<Rectangle>    StepRects      { get; init; }
    public IReadOnlyList<Rectangle>    ConnectorRects { get; init; }
    public int                         SelectedIndex  { get; init; }
    public int                         HoveredIndex   { get; init; }
    public int                         PressedIndex   { get; init; }
    public int                         FocusedIndex   { get; init; }
    public Orientation                 Orientation    { get; init; }
    public StepperStyleConfig          StyleConfig    { get; init; }
    public IReadOnlyList<StepAnimationState> AnimStates { get; init; }
    public Font                        StepFont       { get; init; }
    public Font                        LabelFont      { get; init; }
    public Font                        NumberFont     { get; init; }
}
```

### StepModel

```csharp
public sealed class StepModel
{
    public string   Text        { get; set; }
    public string   Subtitle    { get; set; }
    public string   Tooltip     { get; set; }
    public string   ImagePath   { get; set; }  // resolved via StepperIconHelpers
    public StepState State      { get; set; } = StepState.Pending;
    public bool     IsEnabled   { get; set; } = true;
    public object   Tag         { get; set; }
    public int      BadgeCount  { get; set; }   // Style 10 badge
    public bool     HasSubSteps { get; set; }
    public IReadOnlyList<StepModel> SubSteps { get; set; }
}
```

---

## 6. Painter Style Catalog

| # | Painter Class | Visual Description | Connector | Node Shape |
|---|---|---|---|---|
| 1 | `CircularNodeStepperPainter` | Numbered circles + center labels, thin line | Solid line, fills on complete | Circle |
| 2 | `ChevronBreadcrumbStepperPainter` | Polygon chevrons, gradient fill active | None (overlapping) | Chevron polygon |
| 3 | `SquareDashedStepperPainter` | Square nodes, dashed connector, minimal | Dashed, 4px dash | Square, rounded |
| 4 | `SegmentedTabStepperPainter` | Rectangular tabs flush, filled active | Flush edge | Rectangle |
| 5 | `ProgressBarStepperPainter` | Label top, thin bar, Prev/Next CTAs | Bar fill % | None |
| 6 | `DotsStepperPainter` | Small dots, active dot larger + pulse | Thin line | Circle (3–8px) |
| 7 | `IconTimelineStepperPainter` | Pill/circle with icon inside, partial track | Track with fill | RoundedRect/Circle |
| 8 | `VerticalTimelineStepperPainter` | Vertical, big icon, card on side | Vertical line | Large circle |
| 9 | `GradientMaterialStepperPainter` | Gradient chevrons L→R, palette-driven | Chevron overlap | Chevron + gradient |
| 10 | `BadgeStatusStepperPainter` | Circle + numeric badge overlay, status ring | Dotted line | Circle + badge |
| 11 | `AlternatingTimelineStepperPainter` | Vertical, L/R alternating nodes, center line | Center vertical line | Circle |
| 12 | `CompactInlineStepperPainter` | Minimal inline: text/number + separator | Arrow `›` or slash | None or tiny dot |

---

## 7. Framework Compliance Requirements (Mandatory)

### 7.1 Font Rules
- **NEVER** `new Font(...)`, **NEVER** `control.Font`, **NEVER** assign `Font = ...`
- Resolve ALL fonts in `ApplyTheme()` using `BeepThemesManager.ToFont(TypographyStyle)`
- Pass resolved fonts into `IStepperPainter.Initialize(...)` — painters must NOT resolve fonts
- `StepperFontHelpers.cs` must be updated to delegate to `BeepThemesManager.ToFont`

```csharp
// In BeepStepperBar.ApplyTheme():
_stepFont   = BeepThemesManager.ToFont(CurrentTheme?.LabelSmall)  ?? SystemFonts.DefaultFont;
_labelFont  = BeepThemesManager.ToFont(CurrentTheme?.BodySmall)   ?? SystemFonts.DefaultFont;
_numberFont = BeepThemesManager.ToFont(CurrentTheme?.LabelMedium) ?? SystemFonts.DefaultFont;
_activePainter?.Initialize(this, CurrentTheme, _stepFont, _labelFont, _numberFont);
```

### 7.2 DPI Rules
- ALL pixel constants replaced with `DpiScalingHelper.ScaleValue(value, this)` at paint/layout time
- `StepperStyleConfig.ScaleForDpi()` already exists — call it in `ComputeLayout`, never cache the result
- Pass `owner` control reference into helpers where DPI is needed; do not cache the scale factor

### 7.3 Color Rules
- Remove ALL hardcoded `Color.FromArgb(...)` field initializers (e.g., `completedStepColor`)
- All colors come from `StepperThemeHelpers` → `IBeepTheme` token properties
- Expose only semantic override properties on the control (e.g., `CustomCompletedColor`, default `Color.Empty`)

### 7.4 Image / Icon Rules
- All icon rendering via `StyledImagePainter.PaintIcon(g, rect, path, color)` or `PaintWithTint`
- Icon paths via `SvgsUI` or `Svgs` constants only (e.g., `SvgsUI.CheckCircle`, `SvgsUI.WarningCircle`)
- `StepperIconHelpers` already encapsulates this — painters call through helpers, never direct

### 7.5 Background / Border / Shadow
- Container background: `BackgroundPainterFactory.GetPainter(FormStyle).Paint(g, bounds)`
- Container border: `BorderPainterFactory.GetPainter(ControlStyle).Paint(g, bounds)`
- Shadow (when elevated): `ShadowPainterFactory.GetPainter(ControlStyle).Paint(g, bounds)`

### 7.6 Hit Testing
- **All** step hit areas registered via `ControlHitTestHelper.AddHitArea(name, rect, null, callback)`
- `ControlHitTestHelper.HandleMouseMove/Down/Up/Click` replaces raw `buttonBounds` iteration
- Hover state change detected in `ControlHitTestHelper` callbacks, stored as `_hoveredStepIndex`
- Keyboard: `ProcessDialogKey` dispatches Left/Right/Up/Down → `_interactionManager.MoveFocus`

---

## 8. Interaction Model

### 8.1 States Per Step
Every step must cleanly differentiate these states both visually and semantically:

| State | Visual Cue |
|---|---|
| `Default` (pending) | Outline circle/shape, grey connector |
| `Hover` | Fill lightened, cursor pointer, tooltip shown |
| `Focused` | 2px offset focus ring (theme `OutlineColor`) |
| `Pressed` | Ripple from click point, scale 0.95 |
| `Active` (current) | Primary fill, white/onPrimary number |
| `Completed` | Success fill + check icon |
| `Error` | Error fill + X icon, connector red |
| `Warning` | Warning fill + `!` icon |
| `Disabled` | 38% opacity, no interaction |

### 8.2 Animations
- **Connector fill**: animated from 0→100% over 300ms ease-out when a step completes
- **Node scale**: active node pulses 1.0→1.08→1.0 on selection (100ms spring)
- **Ripple**: 300ms circular ripple from click center, theme primary 20% alpha
- **Focus ring**: instant show/hide, no animation (WCAG requirement)
- **Reduced motion**: all animations disabled when `StepperAccessibilityHelpers.IsReducedMotionEnabled()`

### 8.3 Keyboard Navigation
- `Tab` / `Shift+Tab`: moves focus between steps
- `Left` / `Up`: focus previous enabled step
- `Right` / `Down`: focus next enabled step
- `Enter` / `Space`: activate focused step (if `AllowStepNavigation`)
- `Home`: focus first step
- `End`: focus last step

### 8.4 Tooltip Behavior
- Show step `Tooltip` text via `BaseControl` tooltip manager on hover
- Auto-generate tooltip from `StepModel.Text + State` if no explicit tooltip set
- Tooltip appears after 400ms hover delay, disappears immediately on mouse leave

---

## 9. New Properties to Add

```csharp
// Painter selection
[Browsable(true), Category("Appearance")]
public string PainterName { get; set; } = "CircularNode";

// Step data model (replaces parallel Dictionaries)
[Browsable(false)]
public IList<StepModel> StepModels { get; set; }

// Navigation
[Browsable(true), Category("Behavior")]
public bool ShowNextPrevButtons { get; set; } = false;

[Browsable(true), Category("Behavior")]
public bool AllowClickNavigation { get; set; } = true;

[Browsable(true), Category("Behavior")]
public bool AllowSkipSteps { get; set; } = false;

// Animation
[Browsable(true), Category("Animation")]
public int TransitionDurationMs { get; set; } = 300;

[Browsable(true), Category("Animation")]
public bool EnableRipple { get; set; } = true;

// Connector
[Browsable(true), Category("Appearance")]
public ConnectorStyle ConnectorStyle { get; set; } = ConnectorStyle.Solid;

// Accessibility
[Browsable(true), Category("Accessibility")]
public bool ShowStepNumbers { get; set; } = true;

[Browsable(true), Category("Accessibility")]
public bool ShowLabels { get; set; } = true;
```

---

## 10. Implementation Phases

### Phase 1 — Framework Compliance & Decomposition (Priority: CRITICAL)

**ST-01** `StepperFontHelpers.cs` — migrate all font resolution to `BeepThemesManager.ToFont(TypographyStyle)`. Remove all `BeepFontManager` calls and any `new Font(...)`.

**ST-02** `BeepStepperBar.cs` — remove hardcoded color field initializers (`Color.FromArgb`). Move color resolution to `ApplyTheme()` via `StepperThemeHelpers`.

**ST-03** `BeepStepperBar.cs` / `BeepStepperBreadCrumb.cs` — replace raw `buttonBounds` hit testing with `ControlHitTestHelper.AddHitArea/HandleMouseMove/HandleMouseDown/HandleMouseUp/HandleClick`.

**ST-04** Decompose `BeepStepperBar.cs` into partial files:
  - `BeepStepperBar.cs` — public API, events, constructor
  - `BeepStepperBar.Layout.cs` — `ComputeLayout()`, `StepBounds`, `ConnectorRects`
  - `BeepStepperBar.Painters.cs` — painter dispatch, `InitializePainter()`, `DrawContent` override
  - `BeepStepperBar.HitTest.cs` — `ControlHitTestHelper` wiring, mouse/key overrides
  - `BeepStepperBar.Interactions.cs` — step selection, `GoNext()`, `GoPrevious()`, validation
  - `BeepStepperBar.Animations.cs` — timer, easing, `StepAnimationState` per step
  - `BeepStepperBar.DesignTime.cs` — `DesignMode` sample data

**ST-05** Decompose `BeepStepperBreadCrumb.cs` in parallel partial structure.

**ST-06** Create `StepModel.cs`, `StepPainterContext.cs`, `StepAnimationState.cs` in `Models/`.

**ST-07** Create `StepperLayoutHelper.cs` — pure geometry helper (no Graphics), computes step center points, connector rectangles, responsive auto-spacing.

### Phase 2 — Painter Contract & Core Painters (Priority: HIGH)

**ST-08** Create `IStepperPainter.cs` with the full contract defined in Section 5.

**ST-09** Create `StepperPainterRegistry.cs` — `Dictionary<string, IStepperPainter>`, auto-discovers painters in assembly via reflection, fallback to `CircularNode`.

**ST-10** `CircularNodeStepperPainter.cs` (Style 1) — numbered circles, animated connector fill, check icon for completed steps, full state expressiveness.

**ST-11** `ChevronBreadcrumbStepperPainter.cs` (Style 2) — polygon chevron shapes, gradient active fill, overlapping layout, replaces current `BeepStepperBreadCrumb` drawing logic.

**ST-12** `ProgressBarStepperPainter.cs` (Style 5) — top-label bar with Prev/Next button hints (CTA areas), animated progress bar fill between steps.

**ST-13** `DotsStepperPainter.cs` (Style 6) — small dots, active dot scale-pulse, click on any dot navigates step.

### Phase 3 — Extended Painter Library (Priority: MEDIUM)

**ST-14** `SegmentedTabStepperPainter.cs` (Style 4) — flush rectangular tab segments, filled active section.

**ST-15** `SquareDashedStepperPainter.cs` (Style 3) — square/rounded nodes, dashed connector, minimal modern.

**ST-16** `IconTimelineStepperPainter.cs` (Style 7) — icons inside pill/circle shapes, partial track fill, supports custom `StepModel.ImagePath`.

**ST-17** `VerticalTimelineStepperPainter.cs` (Style 8) — vertical layout, large icons, right-side label/subtitle card, vertical connector line.

**ST-18** `GradientMaterialStepperPainter.cs` (Style 9) — large gradient chevrons left-to-right, palette-driven, fills with completion gradient.

**ST-19** `BadgeStatusStepperPainter.cs` (Style 10) — circle + numeric badge overlay from `StepModel.BadgeCount`, status ring color from `StepState`.

**ST-20** `AlternatingTimelineStepperPainter.cs` (Style 11) — vertical only, alternating left/right nodes on center line, date/subtitle support via `StepModel.Subtitle`.

**ST-21** `CompactInlineStepperPainter.cs` (Style 12) — minimal inline breadcrumb, text separator (`›` or `/`), underline on active, fits in narrow containers.

### Phase 4 — Interaction & Animation Polish (Priority: MEDIUM)

**ST-22** `StepperAnimationEasing.cs` — implement `SpringEaseOut`, `CubicEaseOut`, `Linear` static easing functions. Used by `BeepStepperBar.Animations.cs`.

**ST-23** Connector fill animation — animated `float connectorFillProgress[i]` per step pair, transitions 0→1 over `TransitionDurationMs` when step `i` completes.

**ST-24** Ripple effect — `StepAnimationState.RippleCenter`, `RippleRadius`, `RippleAlpha` per step, renders in painter's `PaintStep` using `StepPainterContext.AnimStates[i]`.

**ST-25** Active node scale pulse — node scale-spring on step selection (1.0 → 1.08 → 1.0, 100ms), stored in `StepAnimationState.NodeScale`.

**ST-26** Focus ring — painted as 2px `theme.OutlineColor` ring offset by 2px, no animation, instant on/off for WCAG compliance.

**ST-27** Keyboard navigation in `BeepStepperBar.Interactions.cs` — Left/Right/Up/Down/Home/End/Enter/Space with `AllowStepNavigation` guard.

### Phase 5 — Sub-Step & Validation Support (Priority: LOW)

**ST-28** Sub-step nesting — when `StepModel.HasSubSteps`, painters that support it (e.g., `VerticalTimeline`) render nested steps indented with a smaller connector.

**ST-29** Step validation — `StepValidating` event raised before `CurrentStep` changes, `StepValidatingEventArgs.Cancel = true` prevents navigation.

**ST-30** `ShowNextPrevButtons` layout — when true, reserve bottom area in `StepperLayoutHelper` and render Prev/Next `BeepButton` controls (instantiated as child controls, not painted).

### Phase 6 — Design-Time & Accessibility (Priority: LOW)

**ST-31** `BeepStepperBar.DesignTime.cs` — populate `StepModels` with 4 sample steps in design mode, display first painter style.

**ST-32** Extend `StepperAccessibilityHelpers` — add `SetAccessibleStep(Control control, int stepIndex, int totalSteps, StepState state)` for live-region announcements.

**ST-33** Design-time smart tags — `DesignerActionList` for `PainterName`, `StepCount`, `Orientation`, `ShowLabels` quick-set.

---

## 11. Files to Create (New)

| File | Purpose |
|---|---|
| `Models/StepModel.cs` | Per-step data model |
| `Models/StepPainterContext.cs` | Painter call context snapshot |
| `Models/StepAnimationState.cs` | Per-step animation state |
| `Helpers/StepperLayoutHelper.cs` | Pure geometry, computes rects |
| `Helpers/StepperAnimationEasing.cs` | Easing functions |
| `Painters/IStepperPainter.cs` | Painter contract |
| `Painters/StepperPainterRegistry.cs` | Name → painter registry |
| `Painters/CircularNodeStepperPainter.cs` | Style 1 |
| `Painters/ChevronBreadcrumbStepperPainter.cs` | Style 2 |
| `Painters/SquareDashedStepperPainter.cs` | Style 3 |
| `Painters/SegmentedTabStepperPainter.cs` | Style 4 |
| `Painters/ProgressBarStepperPainter.cs` | Style 5 |
| `Painters/DotsStepperPainter.cs` | Style 6 |
| `Painters/IconTimelineStepperPainter.cs` | Style 7 |
| `Painters/VerticalTimelineStepperPainter.cs` | Style 8 |
| `Painters/GradientMaterialStepperPainter.cs` | Style 9 |
| `Painters/BadgeStatusStepperPainter.cs` | Style 10 |
| `Painters/AlternatingTimelineStepperPainter.cs` | Style 11 |
| `Painters/CompactInlineStepperPainter.cs` | Style 12 |
| `BeepStepperBar.Layout.cs` | Partial: layout |
| `BeepStepperBar.Painters.cs` | Partial: painter dispatch |
| `BeepStepperBar.HitTest.cs` | Partial: hit testing |
| `BeepStepperBar.Interactions.cs` | Partial: step selection/nav |
| `BeepStepperBar.Animations.cs` | Partial: animation timer |
| `BeepStepperBar.DesignTime.cs` | Partial: design mode |
| `BeepStepperBreadCrumb.Layout.cs` | Breadcrumb partial |
| `BeepStepperBreadCrumb.Painters.cs` | Breadcrumb partial |
| `BeepStepperBreadCrumb.HitTest.cs` | Breadcrumb partial |
| `BeepStepperBreadCrumb.Interactions.cs` | Breadcrumb partial |

---

## 12. Files to Modify (Existing)

| File | Change |
|---|---|
| `BeepStepperBar.cs` | Extract partials, remove inline drawing, resolve fonts in `ApplyTheme`, remove hardcoded colors |
| `BeepStepperBreadCrumb.cs` | Extract partials, delegate drawing to painter |
| `Helpers/StepperFontHelpers.cs` | Replace `BeepFontManager` with `BeepThemesManager.ToFont(TypographyStyle)` |
| `Helpers/StepperThemeHelpers.cs` | Extend with error/warning/disabled semantic token helpers |
| `Helpers/StepperIconHelpers.cs` | Add `SvgsUI` constants for common step icons (check, error, warning, info) |
| `Models/StepperStyleConfig.cs` | Add `ConnectorStyle`, `NodeShape`, `ShowSubtitle` fields |

---

## 13. Guardrails (Mandatory — Do Not Violate)

1. **No `new Font(...)`** anywhere in Steppers code
2. **No `Font = ...`** assignments — fonts come from `BeepThemesManager.ToFont`
3. **No `Color.FromArgb(...)`** field initializers — all from `IBeepTheme` tokens via `StepperThemeHelpers`
4. **No `control.Font`** used for measurement — use the `Font` instances from `ApplyTheme`
5. **Painters are render-only** — no state mutation inside painter methods
6. **`ControlHitTestHelper`** is the ONLY hit mechanism — remove all direct `buttonBounds` checks
7. **`DpiScalingHelper.ScaleValue(value, this)`** for every pixel constant in layout and paint
8. **`StyledImagePainter`** for every icon/image render — no `g.DrawImage(...)` directly on resource streams
9. **`BackgroundPainterFactory` / `BorderPainterFactory`** for container background and border
10. **Event detection in control/hit-test layer only** — painters receive state, do not detect events

---

## 14. Acceptance Criteria

- [ ] All 12 painter styles render correctly with theme colors, DPI scaling, and no font violations
- [ ] `ControlHitTestHelper` used for all step hit areas; hover/click/focus dispatch correctly
- [ ] Keyboard navigation: Left/Right/Home/End/Enter/Space work per spec
- [ ] `StepValidating` event fires before step change and `Cancel = true` blocks navigation
- [ ] Ripple, connector-fill, and node-scale animations respect `IsReducedMotionEnabled()`
- [ ] Focus ring is visible in all themes (WCAG 2.2 Focus Appearance)
- [ ] No `new Font(...)`, no `Color.FromArgb(...)` field initializer, no `control.Font` in any stepper file
- [ ] `BeepStepperBar` and `BeepStepperBreadCrumb` build with zero errors and zero warnings
- [ ] Design-mode shows 4 sample steps with the default painter style
- [ ] All painters support `Orientation.Horizontal` and `Orientation.Vertical` (or explicitly state vertical is not supported)

---

## 15. Implementation Priority Order

```
Phase 1 (ST-01 → ST-07)  — Framework compliance, must be done first
Phase 2 (ST-08 → ST-13)  — Core painters, unblocks visual QA
Phase 3 (ST-14 → ST-21)  — Extended palette
Phase 4 (ST-22 → ST-27)  — Animation polish
Phase 5 (ST-28 → ST-30)  — Advanced features
Phase 6 (ST-31 → ST-33)  — Design-time
```

---

*Plan created: 2026-02-28*  
*Reference sources: Material Design 3 Stepper, Ant Design Steps, DevExpress WizardControl/StepProgressBar, Fluent UI 2 Stepper, Figma Community Stepper components, WCAG 2.2 Success Criterion 2.4.11 Focus Appearance*
