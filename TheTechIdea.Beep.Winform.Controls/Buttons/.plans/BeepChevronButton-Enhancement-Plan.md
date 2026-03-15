# BeepChevronButton Enhancement Plan — Figma/Web Standards (2026)

> **Goal**: Modernize `BeepChevronButton` to align with latest Figma component conventions and modern web UI standards while preserving backward compatibility with existing WinForms usage.

> **Core Rule (Chevron Exception)**: `BeepChevronButton` is a **custom-shaped control** and should keep its own chevron silhouette painting (fill/border/clip/ripple). `BaseControl` is still used for state plumbing, theme properties, events, accessibility, scaling, and shared behaviors.

---

## Current State Assessment (from `BeepChevronButton.cs`)

### Strengths
- Supports direction (`Forward`, `Backward`, `Up`, `Down`)
- Has ripple/click animation infrastructure
- Supports optional image + text + hit area interaction
- Theme mapping exists in `ApplyTheme()`

### Gaps vs modern standards
1. **Bypasses BaseControl painting pipeline**
   - `OnPaint()` does not call `base.OnPaint(e)`
   - `DrawContent()` is effectively unused
   - Control does full custom paint in `Draw()` (fill, border, clip) which conflicts with architecture rule used in other modernized controls

2. **Hardcoded visual metrics**
   - Uses fixed numbers (`5`, `10`, `16`) instead of design tokens + DPI scaling
   - Uses `Arial` default font instead of theme typography tokens

3. **Legacy interaction model**
   - `OnClick` toggles checked state unconditionally (no configurable check behavior)
   - Cursor + image hit testing is custom but not aligned with state-layer/focus system

4. **Accessibility gaps**
   - No explicit keyboard affordance plan for direction semantics
   - No explicit focus ring strategy (keyboard-visible only)
   - No minimum hit target guarantee (48dp equivalent)

5. **Shape ownership (resolved)**
   - Chevron polygon remains the full control silhouette (custom paint owned by `BeepChevronButton`)
   - No migration to default rectangular/pill BaseControl chrome

---

## Design Decisions (Figma/Web aligned)

1. **Component model**: `BeepChevronButton` behaves as a specialized button variant, not a standalone paint engine.
2. **State model**: Use state-layer overlay approach (hover/focus/pressed/disabled) instead of hard state color swapping where possible.
3. **Tokens first**: Replace hardcoded spacing/sizes with scalable tokens.
4. **A11y default**: Keyboard-first focus visibility + clear checked/pressed semantics.
5. **Motion**: Respect reduced-motion preference.

---

## Phase 1 — Architecture Alignment (Custom-Shape Compliant)

### Objective
Keep chevron-owned painting, but align the control with BaseControl lifecycle/theming conventions and remove architectural drift.

### Tasks
1. **Keep custom paint pipeline explicit**
   - Keep chevron silhouette rendering in `Draw()` / `OnPaint` (custom-shape exception)
   - Ensure base lifecycle hooks are still respected for state updates, effects, and events

2. **Normalize ownership boundaries**
   - `BeepChevronButton` owns: polygon path, fill/border, clip region, ripple
   - `BaseControl` owns: state flags, theme plumbing, hit architecture, accessibility/event base behavior

3. **Formalize chevron shape contract**
   - Add/keep an internal helper to compute chevron polygon path for all directions
   - Ensure one canonical path function is used by: paint, hit-testing, ripple clipping, and focus rendering

4. **Deprecate `IsCustomeBorder` typo usage**
   - Preserve compatibility, but clarify that chevron border is self-painted
   - Prevent confusion with BaseControl border flags by documenting precedence

### Validation
- Control renders with chevron-owned silhouette (single source of truth for shape)
- No double border/conflicting border artifacts with BaseControl theme/state values
- Chevron visuals remain directionally correct with consistent clip/hit behavior

---

## Phase 2 — Tokenized Layout & Typography (Figma Auto Layout parity)

### Objective
Replace hardcoded metrics with token-driven, DPI-safe layout.

### Proposed Tokens (new or mapped)
- `ChevronPaddingH = 12`
- `ChevronPaddingV = 8`
- `ChevronIconSize = 16`
- `ChevronTextGap = 8`
- `ChevronMinHeight = 40` (compact) / `48` (accessible)
- `ChevronCornerRadius` (style driven unless shape override)

### Tasks
1. Add internal token constants (or shared button token class if available)
2. Replace fixed dimensions (`5/10/16`) with `Scale(token)`
3. Replace default `Arial` with theme font token fallback
4. Use consistent text layout flags with ellipsis and vertical centering
5. Ensure image/text layout follows directional flow (`Forward` vs `Backward`, vertical modes)

### Validation
- Visual consistency at 100/125/150/200% DPI
- Text/icon alignment matches Figma spacing system
- No clipping with long labels

---

## Phase 3 — Interaction & State Semantics

### Objective
Modern behavior model compatible with web buttons/toggles.

### Tasks
1. **Checked behavior policy**
   - Add `IsCheckable` (default `false` for button parity, `true` for toggle variants)
   - Only toggle `IsChecked` when `IsCheckable == true`

2. **State layers**
   - Hover/focus/pressed overlays using alpha-based state layers
   - Keep ripple as optional pressed effect layered above content

3. **Keyboard behavior**
   - `Space/Enter` activate
   - Focus-visible ring on keyboard focus
   - Arrow-key behavior optional for direction cycling (opt-in)

4. **Image hit area semantics**
   - Keep optional image click action but unify with command/click handling

### Validation
- Behavior matches modern web button/toggle patterns
- Ripple + pressed state remain coherent
- Checked visual state clear and deterministic

---

## Phase 4 — Accessibility & UX Compliance

### Objective
Meet practical WCAG-aligned desktop accessibility expectations.

### Tasks
1. Set/verify:
   - `AccessibleRole = PushButton` (or `CheckButton` when checkable)
   - `AccessibleName` fallback from `Text`
   - `AccessibleDescription` for direction semantics

2. Guarantee minimum target size
   - Clamp to scaled minimum height/width (48dp equivalent where possible)

3. Contrast and focus
   - Ensure focus ring has sufficient contrast
   - Disabled state text/background contrast remains readable

4. Reduced motion
   - Add `ReducedMotion` option to disable ripple animation and shorten transitions

### Validation
- Keyboard-only navigation fully usable
- Focus clearly visible
- Screen reader announces role + label + checked state

---

## Phase 5 — Theming & Visual Variants

### Objective
Support modern variants without custom painting fragmentation.

### Tasks
1. Add/align variant properties:
   - `Variant` (`Filled`, `Outlined`, `Tonal`, `Text`)
   - `Size` (`Sm`, `Md`, `Lg`)
   - `Direction`

2. Map variants to BaseControl style properties
   - Border thickness/color, background, foreground via theme

3. Ensure compatibility with `ControlStyle` ecosystem
   - Material3, Fluent2, Shadcn, etc. should style chevron button consistently

### Validation
- Variant matrix renders consistently across styles/themes
- No style-specific regressions

---

## Phase 6 — API Cleanup & Backward Compatibility

### Objective
Modernize API without breaking existing code.

### Tasks
1. Keep existing properties but mark legacy behavior in XML docs
2. Introduce new properties with clear precedence rules:
   - `IsCheckable` + `IsChecked`
   - `ShowRipple`
   - `ReducedMotion`
   - `ChevronContentAlignment`

3. Maintain `ImagePath` contract (string path), use `StyledImagePainter`
4. Add migration notes in button docs

### Validation
- Existing forms compile unchanged
- New APIs available for modern behavior

---

## Phase 7 — Documentation & Samples (Required)

### Files to update
- `TheTechIdea.Beep.Winform.Controls/Readme.md`
- `TheTechIdea.Beep.Winform.Controls/BaseControl/Readme.md` (if shape/paint contract references are updated)
- `TheTechIdea.Beep.Winform.Controls/Styling/Readme.md` (if token additions)
- `TheTechIdea.Beep.Winform.Controls/ThemeManagement/Readme.md` (if theme token additions)
- `TheTechIdea.Beep.Winform.Controls/Buttons/Readme.md` (or per-control readme path used in repo)

### Sample requirements
- Add at least one sample per direction
- Demonstrate: checkable vs non-checkable, ripple on/off, reduced motion, variant/size matrix

---

## Implementation Order (Recommended)

1. Phase 1 (architecture)  
2. Phase 2 (tokens/layout)  
3. Phase 3 (interaction semantics)  
4. Phase 4 (accessibility)  
5. Phase 5 (variants/theme)  
6. Phase 6 (API cleanup)  
7. Phase 7 (docs/samples)

---

## Risks & Mitigations

1. **Risk**: Breaking existing visual expectations where chevron shape was outer silhouette  
   **Mitigation**: Keep optional advanced `Chevron` shape override mode, default to BaseControl chrome

2. **Risk**: Behavior change from auto-toggle on click  
   **Mitigation**: Preserve current behavior behind `IsCheckable=true` migration mode for existing instances if needed

3. **Risk**: Theme regressions across style matrix  
   **Mitigation**: Validate against representative style set (Material3, Fluent2, Retro, Terminal, Shadcn)

---

## Acceptance Criteria

- Uses BaseControl painting pipeline (`base.OnPaint` + `DrawContent` responsibilities)
- No local border/background conflicts
- Directional chevron visuals remain correct
- DPI/token-based layout with no hardcoded magic numbers in paint/layout logic
- Keyboard, focus, and accessibility behavior validated
- Docs and samples updated

---

## Optional Future Enhancements

- Chevron path morph animation on direction change
- Micro-interactions for hover elevation
- Icon slot + badge slot support
- Command binding abstraction for image-click area
