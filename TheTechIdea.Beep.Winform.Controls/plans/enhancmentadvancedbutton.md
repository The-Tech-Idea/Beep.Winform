# BeepAdvancedButton Enhancement Plan
**Reference direction:** Figma component quality + latest web UI/UX button standards (Material 3, Fluent 2, Open UI, WCAG 2.2)
**Target area:** `TheTechIdea.Beep.Winform.Controls/Buttons/BeepAdvancedButton`
**Last updated:** 2026-02-28

---

## Goal

Upgrade `BeepAdvancedButton` to a modern, production-grade interaction and visual system with:
- consistent visual tokens across all styles,
- complete state semantics (`normal`, `hover`, `pressed`, `focus-visible`, `disabled`, `loading`, `toggled`),
- keyboard and accessibility parity with modern web controls,
- DPI-safe rendering and theme-consistent typography/icons,
- reduced paint-path allocations and smoother animations.

---

## Current Observations

From code review of `Buttons/BeepAdvancedButton`:
- Architecture is good: painter strategy + style enum + paint context are already in place.
- State model includes `Focused`, but control flow currently resolves only `Normal/Hover/Pressed/Disabled` and has no keyboard event pipeline (`OnKeyDown`/`OnKeyUp`).
- Ripple hooks exist in painters, but context currently sends `RippleActive = false` and `RippleProgress = 0f`, so ripple is effectively disabled.
- Icon rendering still depends on `ImagePainter` mutation per draw path; this should be standardized to `StyledImagePainter` + `SvgsUI` for modern icon consistency.
- Metrics and text measurement use multiple ad-hoc patterns (`Graphics.MeasureString`, temporary bitmap graphics), which can cause DPI drift and extra allocations.
- Many painter files (especially NewsBanner variants) create fonts and drawing resources in hot paint paths, reducing scalability.

---

## UI/UX Standards to Apply

1. **Button hierarchy clarity**
   - Enforce semantic intent variants: `Primary`, `Secondary`, `Tertiary`, `Destructive`, `Success`, `Neutral`.
   - Keep style names (Solid, Outlined, Ghost, etc.) as visual modes, but map them to intent tokens.

2. **State clarity and parity**
   - Every style must clearly represent all interaction states.
   - Focus-visible must be explicit and keyboard-first (ring outside component bounds).

3. **Figma spacing rhythm**
   - Base all spacing/radii/sizes on 4/8 grid.
   - Keep icon-text alignment and baseline consistency between all button styles.

4. **Motion standards**
   - Use short and meaningful transitions (120-180ms).
   - Respect reduced motion mode and provide deterministic no-animation fallback.

5. **Accessibility baseline**
   - WCAG 2.2 contrast targets for text and icon foreground.
   - Full keyboard support (`Tab`, `Space`, `Enter`, optional arrow behavior for split/toggle variants).

---

## Implementation Constraints (Mandatory)

- **Framework integration (strict)**
  - Follow these shared system entry points during implementation:
    - `TheTechIdea.Beep.Winform.Controls/IconsManagement`
    - `TheTechIdea.Beep.Winform.Controls/FontManagement`
    - `TheTechIdea.Beep.Winform.Controls/Styling/BeepStyling.cs`
    - `TheTechIdea.Beep.Winform.Controls/Styling/BackgroundPainters/*`
    - `TheTechIdea.Beep.Winform.Controls/Styling/BorderPainters/*`
    - `TheTechIdea.Beep.Winform.Controls/ThemeManagement/BeepThemesManager.cs`
    - `TheTechIdea.Beep.Winform.Controls/Styling/ImagePainters/StyledImagePainter.cs`

- **Typography (strict)**
  - Always resolve runtime text fonts through `BeepThemesManager.ToFont(..., applyDpiScaling: true)`.
  - In paint context and painters, use `TextFont` and not control `Font` directly.
  - Do not use `context.Font` as the paint source once migration is complete.
  - Avoid creating `new Font(...)` during painting.

- **Icons and image painting (strict)**
  - Always paint images/icons through `StyledImagePainter`.
  - Resolve icon paths from `IconsManagement` constants (`SvgsUI`, `Svgs`, `SvgsDatasources`) instead of ad-hoc strings.
  - Avoid per-frame mutation of shared `ImagePainter` state where possible; prefer static `StyledImagePainter` calls.

- **Background and border rendering**
  - Align button painting with `BeepStyling` conventions and re-use `BackgroundPainters` / `BorderPainters` patterns where applicable.
  - Keep border/focus/background state decisions consistent with theme style systems.

- **Sizing and DPI**
  - Move hardcoded metrics to shared tokens and apply DPI scaling consistently.
  - Preserve existing public size presets while introducing tokenized internals.

- **Painter responsibilities**
  - Control manages state/events/animation timing.
  - Painters remain render-only and stateless where possible.

- **Backward compatibility**
  - Keep existing public properties (`ButtonStyle`, `ButtonSize`, `IsToggled`, `ImagePath`) functional.
  - Add new APIs in additive form, with defaults that preserve current behavior.

---

## Enhancement Backlog

## Phase 1 - Core Interaction and State Model (P0)

### BTN-01: Complete keyboard interaction model
- Add `OnKeyDown`/`OnKeyUp` handling for `Space` and `Enter` activation.
- Add keyboard semantics for toggle and split-shape behavior.
- Ensure focus ring appears only for keyboard-origin focus (focus-visible behavior).

### BTN-02: Focus-visible rendering contract
- Promote `AdvancedButtonState.Focused` to first-class paint state.
- Add tokenized focus ring metrics (thickness, offset, color, radius expansion).
- Apply across every painter style with a shared helper path.

### BTN-03: Ripple and press animation wiring
- Add control-level animation timer/state for ripple center, progress, and fade.
- Feed real ripple values into `AdvancedButtonPaintContext`.
- Add `ReduceMotion` property and snap behavior when enabled.

### BTN-04: Unified press/hover transition pipeline
- Standardize hover/press transition easing and timing across painters.
- Prevent per-style divergence in motion speed/intensity.

---

## Phase 2 - Design Tokens and Visual Consistency (P0)

### BTN-05: Shared button token model
- Introduce token structure for spacing, radius, icon size, stroke width, focus ring, shadow depth.
- Keep existing `AdvancedButtonSize` as external API; map to internal token set.

### BTN-06: Intent + variant mapping
- Add `ButtonIntent` model (`Primary`, `Secondary`, `Tertiary`, `Destructive`, `Success`, `Neutral`).
- Map each intent to current theme colors without requiring breaking theme changes.

### BTN-07: Typography and text metrics unification
- Move text measuring to consistent method (`TextRenderer.MeasureText` or centralized helper).
- Remove temporary `Bitmap`/`Graphics` creation in repeated measure paths.
- Migrate paint context to `TextFont` and remove direct dependency on control `Font` in painter logic.
- Ensure all `TextFont` instances originate from `BeepThemesManager.ToFont(...)`.

### BTN-08: Icon system modernization
- Replace mutable per-control icon rendering with `StyledImagePainter` calls.
- Add consistent icon tint rules per state (`normal`, `hover`, `disabled`, `pressed`).
- Migrate icon path resolution to `IconsManagement` constants (`SvgsUI`/`Svgs`/`SvgsDatasources`).

---

## Phase 3 - Style-Specific UX Parity (P1)

### BTN-09: Toggle and split UX parity
- Normalize split hit areas and visual boundaries.
- Add clearer selected side semantics and optional divider affordance.

### BTN-10: Loading/async behavior contract
- Add standard loading policy: suppress duplicate clicks while loading, maintain width, preserve label context.
- Improve spinner style to match modern UI spec and ensure contrast.

### BTN-11: IconText and Chip ergonomics
- Improve icon-text spacing for long labels and RTL-safe alignment strategy.
- Add optional close affordance sizing rules and min hit target for chip close icons.

### BTN-12: Link and Ghost accessibility pass
- Ensure visible underline/focus behavior for link style.
- Ensure low-emphasis styles still meet interactive contrast targets on hover/focus.

---

## Phase 4 - NewsBanner and Advanced Styles Hardening (P1)

### BTN-13: NewsBanner painter standardization
- Apply shared typography/icon/color token usage to all NewsBanner painter variants.
- Replace ad-hoc local font creation with cached/tokenized style fonts.
- Reduce hardcoded white/black assumptions and map to theme-aware contrast rules.

### BTN-14: Neon/Contact/Chevron polish pass
- Normalize glow strength, shadow depth, and border behavior to avoid style drift.
- Add consistent disabled and focused rendering for custom shape styles.

---

## Phase 5 - Performance and Reliability (P1)

### BTN-15: Paint-path allocation reduction
- Audit and reduce repeated allocations in painters (fonts, paths, brushes, pens, measurements).
- Reuse helper caches where safe and deterministic.

### BTN-16: Repaint discipline
- Add targeted invalidation for animated states to avoid full-control redraw spikes.
- Ensure no unnecessary synchronous `Update()` usage in hot user interaction paths.

### BTN-17: DPI and resize robustness
- Validate all styles at 100/125/150/200 percent scaling.
- Verify no clipping in icon/text/loading/focus ring combinations.

---

## File-by-File Implementation Map

- `Buttons/BeepAdvancedButton/BeepAdvancedButton.cs`
  - Keyboard interaction, focus-visible state plumbing, ripple runtime state, transition timer orchestration.
  - New additive properties (for example: `ReduceMotion`, `Intent`, `ShowFocusRing`).
  - Build and pass `TextFont` from `BeepThemesManager.ToFont(...)` into paint context.

- `Buttons/BeepAdvancedButton/Models/AdvancedButtonModels.cs`
  - Extend paint context for animation, focus ring, and intent/token inputs.
  - Replace `Font` paint property with `TextFont` (keep compatibility shim only if needed during migration).
  - Add strongly typed token payload model if needed.

- `Buttons/BeepAdvancedButton/Enums/AdvancedButtonEnums.cs`
  - Add optional `ButtonIntent` enum (additive).

- `Buttons/BeepAdvancedButton/Helpers/BeepAdvancedButtonHelper.cs`
  - Centralized text measurement and layout helpers.
  - Replace ad-hoc measure allocations with shared utility methods.

- `ThemeManagement/BeepThemesManager.cs`
  - Use `BeepThemesManager.ToFont(...)` as the single source for button text font resolution.

- `FontManagement/*`
  - Ensure font family and fallback behavior stay aligned with `BeepFontManager` and theme typography.

- `IconsManagement/*`
  - Ensure button icon references use centralized SVG constants.

- `Styling/BeepStyling.cs`
  - Align button background/border/text rendering decisions with shared style conventions.

- `Buttons/BeepAdvancedButton/Painters/BaseButtonPainter.cs`
  - Shared focus ring, icon render (`StyledImagePainter`), and token-based drawing helpers.
  - Consolidated color and transition helpers.

- `Buttons/BeepAdvancedButton/Painters/*Painter.cs`
  - Apply consistent state semantics and token usage style-by-style.
  - Align hover/pressed/disabled/focused visuals.
  - Consume `TextFont` only, and route image painting through `StyledImagePainter` only.

- `Styling/BackgroundPainters/*`
  - Reference patterns for stateful surface rendering consistency.

- `Styling/BorderPainters/*`
  - Reference patterns for focused/hovered border behavior consistency.

- `Buttons/BeepAdvancedButton/Painters/NewsBanner/*.cs`
  - Normalize typography, spacing, contrast, and resource usage across all variants.

- `Buttons/BeepAdvancedButton/Painters/ButtonPainterFactory.cs`
  - Keep style routing stable; support any new additive painter variants if introduced.

- `Buttons/BeepAdvancedButton/Readme.md`
  - Document new intent/state model, keyboard behavior, reduced motion, and accessibility updates.

---

## Acceptance Criteria

- Every button style supports and visually differentiates: `normal`, `hover`, `pressed`, `focus-visible`, `disabled`, and `loading`.
- `Space` and `Enter` activate button semantics correctly for standard, toggle, and split variants.
- Ripple and hover animations run smoothly when motion is enabled, and fully respect `ReduceMotion`.
- Sizing and layout are stable under DPI scaling and no style clips text/icons/focus ring.
- Paint paths avoid repeated font creation and unnecessary per-frame measurement allocations.
- All text rendering uses `TextFont` from `BeepThemesManager.ToFont(...)`, not control `Font`.
- Icon rendering is consistent and theme-aware across all painter styles using `StyledImagePainter` only.
- NewsBanner variants follow shared token and contrast rules with no style-specific regressions.

---

## Validation Checklist

- Visual validation
  - All styles (`Solid`, `Outlined`, `Icon`, `Text`, `Toggle`, `FAB`, `Ghost`, `Link`, `Gradient`, `IconText`, `Chip`, `Contact`, `NavigationChevron`, `NeonGlow`, `NewsBanner`) in light/dark/high-contrast themes.
  - Focus ring visibility and contrast checks against each background.

- Interaction validation
  - Mouse hover/press/release semantics.
  - Keyboard tab navigation + `Space`/`Enter` activation.
  - Toggle and split-area click routing correctness.

- Accessibility validation
  - Foreground/background contrast checks for all semantic intents.
  - Loading and disabled states remain understandable and non-ambiguous.

- Performance validation
  - No visible flicker under rapid hover and click.
  - Stable frame pacing for ripple/loading animation on repeated interactions.

---

## Suggested Execution Order

1. BTN-01, BTN-02, BTN-03, BTN-04
2. BTN-05, BTN-06, BTN-07, BTN-08
3. BTN-09, BTN-10, BTN-11, BTN-12
4. BTN-13, BTN-14
5. BTN-15, BTN-16, BTN-17
