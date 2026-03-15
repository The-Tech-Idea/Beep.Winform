# BeepButton Enhancement Plan — Modern Figma/Web Standards

> **Goal**: Fix the border-drawing conflict with BaseControl, then refactor BeepButton to align with current **Figma Design System**, **Material Design 3**, and **modern web component** standards while preserving full backward compatibility.

> **Core Rule**: Border, shadow, and background painting is **always** handled by `ClassicBaseControlPainter` (via `BaseControl`'s `ControlStyle` / `FormStyle` system). BeepButton **never** draws its own borders or background — it only paints custom content (image, text, splash, loading indicator). The only exception is if a truly custom shape is needed that `ClassicBaseControlPainter` cannot produce.

---

## Phase 0 — BorderPath / BorderPainter Consistency Fix ✅ COMPLETED

**Prerequisite fix**: `CreateControlStylePath()` had only 5 custom shape cases while `BorderPainterFactory` had 61 custom painters. Two painters (Terminal, Retro) ignored the passed `GraphicsPath` and drew their own geometry, causing Region clip / border / background mismatches.

### What was done:
1. **Created `CreateTerminalPath()`** in PathPainterHelpers — chamfered rectangle (4-12px 45° corners) for CRT/ASCII terminal aesthetic
2. **Redesigned `CreateRetroPath()`** — Win95-compatible sharp rect with small corner bevels (replaces the old octagonal shape)
3. **Added missing cases to `CreateControlStylePath()`** — Terminal, Cyberpunk, ChatBubble now route to custom paths
4. **Rewrote `TerminalBorderPainter`** — now uses `g.DrawPath(pen, insetPath)` following the chamfered path + additive corner accent overlays
5. **Rewrote `RetroBorderPainter`** — now uses double-stroke `g.DrawPath` at different insets (light outer + dark inner) for path-aware bevel; pressed state inverts bevel (sunken effect)
6. **Full audit**: 59/61 painters were already path-respecting. 6 spot-checked across all categories. `GetRadius()` values verified for all 60+ styles.
7. **Convention documented** in `Styling/BorderPainters/README.md`: all painters MUST use the passed GraphicsPath, never extract bounds and draw independently.

---

## Root Cause Analysis — Border Conflict

### Current Painting Pipeline

```
OnPaint (BaseControl.Events.cs)
  └─ SafeDraw
       ├─ ClearDrawingSurface          ← clears to BackColor
       ├─ SafeExternalDrawing(BeforeContent)
       ├─ DrawContent (virtual)
       │    ├─ BaseControl.DrawContent  ← calls _painter.UpdateLayout() + _painter.Paint()
       │    │    └─ ClassicBaseControlPainter.Paint()
       │    │         ├─ STYLED PATH: BeepStyling.PaintControl() → Shadow + Background + Border
       │    │         └─ CLASSIC PATH: FillPath(background) + DrawBorders()
       │    └─ BeepButton.DrawContent (override)
       │         ├─ base.DrawContent()  ← triggers painter → draws borders!
       │         ├─ UpdateDrawingRect()
       │         ├─ DrawImageAndText()
       │         ├─ DrawSplashEffect()
       │         └─ DrawLoadingIndicator()
       ├─ DrawLabelAndHelperUniversal
       ├─ SafeExternalDrawing(AfterAll)
       └─ SafeDrawEffects
```

### The Conflict

| Source | What | Effect |
|--------|------|--------|
| `BaseControl._borderThickness` | Defaults to `1` | Enables border drawing in ClassicBaseControlPainter |
| `BeepButton.ShowAllBorders = false` | Set in `InitializeComponents()` | Tries to suppress borders |
| `ClassicBaseControlPainter.shouldDrawBorders` | `!IsFrameless && (ShowAllBorders \|\| BorderThickness > 0)` | **Still TRUE** because `BorderThickness=1` |
| `BeepButton.ApplyTheme()` | `BorderColor = _currentTheme.ButtonBorderColor` | Sets base border color (green from theme) |
| `BeepButton.borderSize` / `borderColor` | Local fields (lines 66-78) | **Dead code** — never used in any draw path |
| **Styled Path** | `BeepStyling.PaintControl()` | Draws border via `BorderPainterFactory` using `StyleBorders.GetBorderWidth(style)` |
| **Classic Path** | `DrawBorders()` | Draws border using `owner.BorderColor` + `owner.BorderThickness` |

**Result**: The painter draws a border (because `BorderThickness > 0`), and the styled/classic path both apply their own border logic. The "double border" artifact occurs when the base painter's border and the styled border conflict in color, width, or inset position.

### Fix Required

1. Remove dead `borderSize`/`borderColor`/`selectedBorderColor` fields from BeepButton
2. Let BeepButton **fully delegate** border, shadow, and background drawing to `ClassicBaseControlPainter`
3. BeepButton only configures BaseControl properties (`ShowAllBorders`, `BorderThickness`, `BorderColor`, `IsFrameless`, `ShowShadow`, etc.) — it never draws them itself
4. Properly coordinate constructor defaults so the painter produces the correct visual

---

## Phase 1 — Border Conflict Fix & Property Cleanup

**Priority**: Critical — fixes the visual bug  
**Estimated Changes**: BeepButton.cs only

### Tasks

1. **Remove dead border fields**
   - Delete `private int borderSize = 1;` (line 66)
   - Delete `private Color borderColor = Color.Black;` (line 78)  
   - Delete `private Color selectedBorderColor = Color.Blue;` (line 79)
   - Delete `public int BorderSize { get/set }` property (lines 382-387) — it shadows base functionality
   - Any code referencing these fields → redirect to `base.BorderThickness` / `base.BorderColor`

2. **Fix constructor border setup**
   - In `InitializeComponents()`: configure BaseControl border properties correctly
   - Set `BorderThickness = 0` (or `IsFrameless = true`) for default borderless buttons
   - Keep `ShowAllBorders = false` as-is
   - All border/shadow/background appearance is controlled by setting BaseControl properties — `ClassicBaseControlPainter` reads them and paints accordingly

3. **Update `ApplyTheme()`**
   - Only set `BorderColor` from theme when borders are actually enabled (`ShowAllBorders == true` or individual borders are on)
   - Let BaseControl properties drive what the painter draws — no button-local border logic
   - Map theme tokens to standard BaseControl color properties (`BackColor`, `ForeColor`, `BorderColor`, etc.)

4. **Clean up `DrawContent()` override**
   - `base.DrawContent(g)` calls the painter for border/shadow/background — this is correct, keep it
   - BeepButton's override only adds: `DrawImageAndText()`, `DrawSplashEffect()`, `DrawLoadingIndicator()`
   - Remove any remnant border/background drawing code from BeepButton
   - `GetButtonClipPath()` should use `BorderPath` from the painter (already does)

### Validation
- Save/Cancel buttons show ZERO unwanted border (painter respects `BorderThickness=0`)
- Setting `ShowAllBorders = true` + `BorderThickness = 1` on a button shows exactly ONE clean border via the painter
- No compile errors, no behavior changes for existing code

---

## Phase 2 — State Layer System (Figma/M3 Pattern)

**Priority**: High — aligns with modern interaction design  
**Reference**: Figma Auto Layout + Material Design 3 state layers

### Concept

Modern design systems use a **state layer** (semi-transparent overlay) on top of the background to indicate hover/press/focus/disabled states, rather than swapping entire background colors. This provides:
- Consistent visual grammar across all button styles (controlled by ControlStyle/FormStyle)
- Theme-independent relative contrast
- Smoother state transitions

> **Note**: Background fill for each state is still handled by `ClassicBaseControlPainter` via `GetEffectiveBackColor()`. The state layer is an _additional_ overlay drawn by BeepButton on top of the painter's background, clipped to the painter's `BorderPath`. This is a valid case for custom painting in BeepButton since it's content-layer visual feedback, not border/background painting.

### Tasks

1. **Add state-layer opacity tokens** (aligned to M3 spec):
   ```
   Hover    = 8% opacity of OnSurface (or state-layer color)
   Focus    = 10% opacity
   Pressed  = 10% opacity  
   Dragged  = 16% opacity
   Disabled = 12% opacity on container, 38% on content
   ```

2. **Refactor `DrawStateOverlays()`** (currently commented out in DrawContent):
   - Re-enable with correct opacity values
   - Use `OnSurface` color (foreground) as overlay tint, NOT the hover/pressed BackColor
   - Apply AFTER background, BEFORE content (image/text)
   - Clip to button shape path

3. **Simplify color properties**:
   - Keep `HoverBackColor` / `PressedBackColor` as overrides for manual mode
   - Add `UseStateLayerSystem = true` (default) — when true, ignores individual state colors and uses the opacity-based overlay approach
   - When `UseStateLayerSystem = false`, fall back to legacy explicit color per state

4. **Ripple refinement**:
   - Splash/ripple should be a **pressed** state layer expanding from click point
   - Align splash opacity with M3 pressed state (10% of OnSurface)
   - Ripple dissipates into a full state layer overlay

### Validation
- Hover shows subtle 8% tint overlay (on top of painter-drawn background)
- Press shows 10% overlay + optional ripple
- Focus shows 10% overlay + optional focus ring
- Disabled dims content (38%) — background handled by painter via `GetEffectiveBackColor()`

---

## Phase 3 — Typography & Icon Layout (Figma Auto-Layout)

**Priority**: Medium — improves visual fidelity and DPI compliance

### Tasks

1. **Standardize internal padding** (M3 button specs):
   ```
   Default:               24dp left, 24dp right, centered vertically
   With leading icon:     16dp left, 24dp right
   With trailing icon:    24dp left, 16dp right  
   Icon-only:            equal padding all sides (square)
   Frameless/Text-style:  12dp horizontal
   ```

2. **Icon sizing tokens**:
   ```
   Default icon size:  18dp (M3 standard)
   Large icon:         24dp
   Button icon spacing: 8dp between icon and text
   ```

3. **Refactor `CalculateLayout()`**:
   - Use token-based padding instead of hardcoded `-2` inflate
   - Support `Gap` property (Figma concept) for spacing between icon and text
   - Scale all measurements via `Scale()` for DPI awareness

4. **Font handling**:
   - Default to `LabelLarge` typography token from M3 (14sp, Medium weight, 0.1sp tracking)
   - Remove `UseScaledFont` — handle DPI in one place via `Scale()`
   - Use `TextRenderer.DrawText` with proper `TextFormatFlags` consistently

5. **Min-width / min-height tokens**:
   ```
   Minimum touch target: 48dp x 48dp (accessibility)
   Minimum button width:  64dp (with text)
   Minimum button height: 40dp (M3), 36dp current compact
   ```

### Validation
- Buttons render identically at 100%, 125%, 150%, 200% DPI
- Icon + text spacing is consistent across all TextImageRelation values
- Touch targets meet accessibility minimums

---

## Phase 4 — Motion & Animation (Web Standards)

**Priority**: Medium — enhances perceived performance and polish

### Tasks

1. **State transitions** (CSS-like easing):
   - Hover enter/exit: 150ms ease-in-out for state layer opacity
   - Press: instant state layer + 150ms ripple expand  
   - Focus: 150ms ease-in for focus ring 
   - Disabled transition: 150ms fade

2. **Refine splash/ripple**:
   - Use `cubic-bezier(0.2, 0, 0, 1)` (M3 standard easing)
   - Splash radius should be sqrt(width² + height²) / 2 (covers full button)
   - Opacity: start at 10%, fade over 300ms after release
   - Support `ReducedMotion` flag (accessibility) — instant state changes, no ripple

3. **Loading state animation**:
   - Replace spinner with indeterminate progress bar inside button bounds
   - OR use circular indicator that respects button height
   - Smooth rotation with 60fps timer already in place

4. **Elevation transitions** (when `ShowShadow = true` on BaseControl):
   - Rest: elevation 1 (subtle shadow)
   - Hover: elevation 2 (increased shadow)
   - Press: elevation 1 (reduces back)
   - Shadow drawing is handled by `ClassicBaseControlPainter.DrawShadow()` — BeepButton only toggles `ShowShadow` / `ShadowOpacity` properties

### Validation
- Hover/press state transitions are smooth (no flicker)
- Ripple expands naturally from click point
- `ReducedMotion = true` disables all animations
- Loading indicator is visible and smooth

---

## Phase 5 — Token System Integration

**Priority**: Medium-High — aligns with design-system token architecture

### Tasks

1. **Map M3 tokens to existing BeepTheme properties**:
   ```
   md.sys.color.primary       → ButtonBackColor
   md.sys.color.on-primary    → ButtonForeColor
   md.sys.color.outline       → ButtonBorderColor
   md.sys.color.on-surface    → State layer tint color
   md.sys.elevation.level1    → Shadow (via ShowShadow + ShadowOpacity on BaseControl)
   ```

2. **Extend `IBeepTheme` if needed** for missing tokens:
   - `StateLayerColor` (typically OnSurface)
   - `StateLayerHoverOpacity`, `StateLayerPressedOpacity`, etc.
   - These are button content-layer tokens, not border/background tokens

3. **Simplify `ApplyTheme()`**:
   - Set standard BaseControl properties from theme (`BackColor`, `ForeColor`, `BorderColor`, hover/press colors)
   - `ClassicBaseControlPainter` reads these properties and paints border/shadow/background
   - BeepButton only maps theme tokens to BaseControl properties — no custom paint logic for chrome
   ```csharp
   // ApplyTheme sets BaseControl properties → painter reads them
   BackColor = theme.ButtonBackColor;
   ForeColor = theme.ButtonForeColor;
   BorderColor = theme.ButtonBorderColor;
   HoverBackColor = theme.ButtonHoverBackColor;
   // ... etc — painter handles the rest
   ```

### Validation
- Theme switching updates all button properties → painter renders correctly
- Custom themes work without breaking rendering
- Token fallbacks exist for themes missing new properties

---

## Phase 6 — Accessibility & Focus Management

**Priority**: High — regulatory and UX requirement

### Tasks

1. **Focus ring** (WCAG 2.1 / Figma convention):
   - 2dp offset from button edge
   - 2dp width
   - Color: theme `FocusIndicatorColor` or system highlight
   - Only visible on keyboard focus (not mouse click)
   - Draw in painter layer, AFTER border

2. **Contrast validation**:
   - Ensure 4.5:1 text contrast ratio for all variants and states
   - Ensure 3:1 border contrast for bordered buttons against background
   - Add `EnsureAccessibleContrast()` utility

3. **Keyboard navigation** (already partially implemented):
   - Verify Enter/Space trigger Click ✓
   - Add `AccessibleRole = PushButton` ✓
   - Add proper `AccessibleName` and `AccessibleDescription` defaults
   - Support `Escape` to cancel if in dialog context

4. **Screen reader**:
   - Announce button state to screen readers
   - Badge text should be in accessible name

5. **Touch targets**:
   - Minimum 48x48dp hit area regardless of visual size
   - Use invisible padding extension if visual button is smaller

### Validation
- Tab navigation cycles through buttons correctly
- Focus ring appears on Tab, disappears on mouse click
- Screen reader announces "Save button" not just "button"
- Color contrast passes WCAG 2.1 AA on all themes

---

## Phase 7 — Shape System & Advanced Visuals

**Priority**: Low — optional polish

### Tasks

1. **Shape token support** (M3 shape scale):
   ```
   None        = 0dp radius
   ExtraSmall  = 4dp
   Small       = 8dp  (M3 button default)
   Medium      = 12dp
   Large       = 16dp
   ExtraLarge  = 28dp
   Full        = height/2 (pill shape)
   ```

2. **Gradient support cleanup**:
   - Keep `UseGradientBackground` as opt-in for power users
   - Remove gradient from default button rendering
   - Gradient should be compatible with state layers (apply state layer on top)

3. **Icon button shapes**:
   - Circle shape for icon-only buttons (FAB-like)
   - Square with radius for icon-only standard buttons
   - Auto-detect: if `IsImageOnly && ButtonType == Normal` → use equal-dimension shape

4. **Badge positioning**:
   - Align badge to top-right corner of button bounds
   - Respect badge overflow into parent clip

### Validation
- Border radius tokens produce correct shapes (driven by `ControlStyle` / `BorderRadius` on BaseControl)
- Icon-only buttons are properly shaped (circle or rounded square) via `ShapeType` on BaseControl
- Gradients still work when enabled, don't conflict with state layers
- All shape/radius rendering done by `ClassicBaseControlPainter` — BeepButton only sets properties

---

## Phase 8 — Documentation & README Updates

**Priority**: Required after each phase

### Tasks

1. Update [Buttons/README.md](../BeepButton.cs) (or create if missing):
   - Document how BeepButton delegates border/shadow/background to BaseControl painter
   - Document state layer system opt-in/opt-out
   - Document which BaseControl properties control appearance (ShowAllBorders, BorderThickness, ShowShadow, ControlStyle, etc.)
   - Code examples for configuring bordered vs borderless buttons

2. Update `BaseControl/Readme.md`:
   - Document border delegation pattern
   - Note that derived controls should NOT add their own border fields

3. Update `Styling/Readme.md`:
   - Document `BeepStyling.PaintControl` interaction with `IsFrameless` / `ShowAllBorders`

4. Update `ThemeManagement/Readme.md`:
   - Document new token properties if added to `IBeepTheme`

---

## Implementation Order

| Order | Phase | Blocking? | Notes |
|-------|-------|-----------|-------|
| 1st | **Phase 1** — Border fix | Yes | Must fix before other phases |
| 2nd | **Phase 5** — Tokens | Yes | Needed by Phase 2 |
| 3rd | **Phase 2** — State layers | Yes | Core interaction model |
| 4th | **Phase 6** — Accessibility | Parallel | Can develop alongside Phase 3 |
| 5th | **Phase 3** — Layout/typography | No | Independent refinement |
| 6th | **Phase 4** — Motion | No | Polish layer |
| 7th | **Phase 7** — Shapes | No | Optional |
| 8th | **Phase 8** — Docs | Ongoing | After each phase |

---

## Breaking Change Policy

- **No existing public API removed** — deprecated fields will get `[Obsolete]` first
- **Default behavior preserved** — default button looks the same as before after fix
- **Opt-in for new features** — `UseStateLayerSystem`, new tokens are additive
- `BorderSize` property on BeepButton will be marked `[Obsolete]` pointing to `base.BorderThickness`
- **Border/shadow/background always owned by ClassicBaseControlPainter** — BeepButton configures BaseControl properties, painter reads and renders them
