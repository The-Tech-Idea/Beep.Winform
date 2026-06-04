# BaseControl External Drawing & Built-in Features — Master Plan

> **Goal:** Transform `ControlExternalDrawingHelper` from a low-level delegate system into a rich, developer-friendly feature set that every `BaseControl`-inheriting control gets for free. Add web-standard validation indicators (icons, text, redlines), batch registration helpers, and a library of pre-built drawing handlers.

---

## Current State Assessment

### What Works
- `ControlExternalDrawingHelper` correctly manages per-child drawing function lists with `DrawingLayer` ordering
- `SvgsUIcons.Common` provides `Error`, `Success`, `Warning`, `Info`, `Close`, `Check` icons as SVG paths
- `StyledImagePainter` can render any SVG icon at any size/color
- `BaseControl` already has `ErrorText`, `HasError`, `ErrorColor`, `HelperText`, `BadgeText` properties
- `BaseControl.LabelandError.cs` has `DrawLabelAndHelperToParent` which draws labels externally

### Gaps
1. **No built-in validation icon overlay** — developers must manually create external drawing handlers
2. **No redline/underline** — web-standard validation pattern (e.g., red squiggly underline, colored bottom border) is missing
3. **No simple API** — current API requires delegate creation, manual color setup, and re-invalidation
4. **No batch icons** — showing both an error icon AND a success icon on adjacent controls requires multiple handlers
5. **No animation hooks** — external drawings can't easily fade/slide/ripple in
6. **No tooltip integration** — validation messages can't automatically link to tooltips

---

## Phase Index

| # | Phase | Description | Status |
|---|-------|-------------|--------|
| 1 | Icon Overlay System | Built-in validation icon drawing (error/success/warning/info) | ✅ |
| 2 | Redline / Underline Indicators | Colored bottom-border indicators like web form validation | ✅ |
| 3 | Simple ShowValidation API | `ShowValidation(state, message)` on BaseControl | ✅ |
| 4 | Batch Registration Helpers | Register external drawing for multiple children at once | ⬜ |
| 5 | Pre-built Drawing Handler Library | Static factory methods for common patterns | ⬜ |
| 6 | Animation Hooks | Fade-in/slide-in for external drawing elements | ✅ |
| 7 | Developer Documentation & Samples | README updates, code samples, best practices | ⬜ |
| 8 | Floating Badge UserControl System | UserControl-based badges that work for any Control, with built-in types, custom locations, lifecycle management | ✅ |

---

## Phase 1 — Icon Overlay System

### Goal
Any `BaseControl` can show a validation icon (✓, ✗, ⚠, ℹ) positioned at a corner of the control using a simple property.

### New Properties on BaseControl
```csharp
// Which validation icon to show (None, Error, Success, Warning, Info)
public ValidationIconType ValidationIcon { get; set; } = ValidationIconType.None;

// Corner position for the icon (TopRight, TopLeft, BottomRight, BottomLeft, AfterText)
public IconPosition ValidationIconPosition { get; set; } = IconPosition.TopRight;

// Size of the icon in pixels
public int ValidationIconSize { get; set; } = 16;

// Offset from the corner in pixels
public Padding ValidationIconPadding { get; set; } = new Padding(2);
```

### Implementation
- Create `ValidationIconHelper` in `BaseControl/Helpers/`
- Register an external drawing function automatically when `ValidationIcon != None`
- Read icons from `SvgsUIcons.Common.{Error|Success|Warning|Info}`
- Paint via `StyledImagePainter` with the control's `ErrorColor` / `ForeColor`

### New Types
```csharp
public enum ValidationIconType { None, Error, Success, Warning, Info }
public enum IconPosition { TopRight, TopLeft, BottomRight, BottomLeft, AfterText }
```

---

## Phase 2 — Redline / Underline Indicators

### Goal
Draw a colored indicator line (like web form validation) below a control to indicate error/success/warning state.

### Web/Figma Best Practices
- **Error:** 2px solid red line below the control, covering the full width
- **Success:** 2px solid green line
- **Warning:** 2px solid amber/orange line
- **Info:** 1px dashed blue line
- Positioning: 2-4px below the control's bottom edge

### New Properties
```csharp
// Show an indicator line below the control
public bool ShowIndicatorLine { get; set; } = false;

// Style of the indicator line
public IndicatorLineStyle IndicatorLineStyle { get; set; } = IndicatorLineStyle.Solid;

// Color derives from ValidationIcon state (Error→ErrorColor, etc.)
// Or can be overridden:
public Color IndicatorLineColor { get; set; }
```

### Implementation
- Draw in the parent's coordinate space via external drawing (2px tall rectangle below control bounds)
- Register automatically via `AddChildExternalDrawing` when enabled
- Support solid, dashed, dotted styles

---

## Phase 3 — Simple `ShowValidation` API

### Goal
One call to set error/validation state.

### API
```csharp
// Set validation state
public void ShowValidation(ValidationState state, string message = null);

// Clear all validation indicators
public void ClearValidation();

// Events
public event EventHandler<ValidationStateChangedEventArgs> ValidationStateChanged;
```

### Behavior
| `ShowValidation(Error, "Invalid email")` | Shows red icon, red underline, sets `HasError=true`, sets `ErrorText` |
| `ShowValidation(Success, "")` | Shows green check icon, green underline, clears error |
| `ShowValidation(Warning, "Weak password")` | Shows amber warning icon, amber underline |
| `ShowValidation(None)` | Clears all indicators |

### New Types
```csharp
public enum ValidationState { None, Error, Success, Warning, Info }
public class ValidationStateChangedEventArgs : EventArgs
{
    public ValidationState OldState { get; }
    public ValidationState NewState { get; }
    public string Message { get; }
}
```

---

## Phase 4 — Batch Registration Helpers

### Goal
Register external drawing for multiple children with one API call.

### API
```csharp
// Register external drawing for all children matching a predicate
public void AddExternalDrawingForAll(Func<Control, bool> predicate, DrawExternalHandler handler, DrawingLayer layer);

// Register external drawing for all children of a specific type
public void AddExternalDrawingForAll<T>(DrawExternalHandler handler, DrawingLayer layer) where T : Control;

// Register external drawing for all named children
public void AddExternalDrawingForAll(IEnumerable<string> childNames, DrawExternalHandler handler, DrawingLayer layer);

// Clear all external drawing for children matching predicate
public void ClearExternalDrawingForAll(Func<Control, bool> predicate);
```

---

## Phase 5 — Pre-built Drawing Handler Library

### Goal
Static factory methods that create common external drawing handlers.

### API
```csharp
public static class BaseControlDrawingHandlers
{
    // Icon-only
    public static DrawExternalHandler IconOverlay(string svgPath, Color tint, IconPosition position, int size);

    // Red underline
    public static DrawExternalHandler Underline(Color color, int thickness, int offsetY);

    // Icon + text tooltip
    public static DrawExternalHandler ValidationIcon(string svgPath, Color tint, IconPosition pos, int size, string tooltip);

    // Badge counter (notification count, error count)
    public static DrawExternalHandler CounterBadge(int count, Color backColor, Color foreColor);

    // Ripple effect on the parent surface
    public static DrawExternalHandler RippleIndicator(Color color, Point center, float maxRadius);

    // Progress ring/arc
    public static DrawExternalHandler ProgressRing(Color color, float percentage, int thickness);

    // Pulse/focus ring
    public static DrawExternalHandler FocusRing(Color color, int thickness, int padding);
}
```

---

## Phase 6 — Animation Hooks

### Goal
External drawing elements can animate in (fade, slide) for polished UX.

### API
```csharp
// Animation types for external drawing reveal
public enum ExternalDrawingAnimation { None, FadeIn, SlideFromTop, SlideFromRight }

// Per-registration animation setting
public void AddChildExternalDrawing(Control child, DrawExternalHandler handler,
    DrawingLayer layer, ExternalDrawingAnimation animation = ExternalDrawingAnimation.FadeIn,
    int durationMs = 200);
```

### Implementation
- Store animation state per external drawing function
- Animate opacity from 0→1 over duration using `Timer`-based progressive invalidation
- For slide animations, clip the drawing rect progressively

---

## Phase 7 — Developer Documentation & Samples

### Deliverables
- Update `BaseControl/README.md` with the new feature sections
- Create `BaseControl/EXTERNAL_DRAWING_GUIDE.md` with:
  - Quick-start examples
  - Common patterns (form validation, inline errors, success confirmations)
  - Performance notes (when to use external drawing vs sub-controls)
  - Integration with `SvgsUIcons` and `StyledImagePainter`
- Create code samples in the test project

---

## Phase 8 — Floating Badge UserControl System ✅ (COMPLETED)

### Goal
Replace the legacy external-drawing badge (`CreateBadgeDrawingHandler`) with a proper `UserControl`-based `BeepFloatingBadge` system. Works for **any `Control`**, not just `BaseControl`. Includes 6 built-in badge types, a user-extensible factory, CSS-style anchor system, and lifecycle management via `BeepBadgeManager`.

### Architecture
```
Extension Methods (ShowCounterBadge, ShowDotBadge, etc.)
            │
            ▼
┌──────────────────────────────────────────────────────┐
│  BeepBadgeManager (static, per-parent singleton)     │
│    - tracks Control → List<IBeepBadge>               │
│    - auto-disposes badges on parent/child dispose    │
│    - auto-repositions badges on resize/move          │
│    - BringToFront on parent Paint event              │
└──────────────────────────────────────────────────────┘
            │
            ▼
┌──────────────────────────────────────────────────────┐
│  BeepFloatingBadge : UserControl, IBeepBadge         │
│    - anti-aliased GraphicsPath (circle/pill/diamond) │
│    - drop shadow, border, anchor, offset             │
│    - BadgeLocation.ComputeBounds() for position      │
│    - events: BadgeClick, BadgeOpened, BadgeClosed    │
└──────────────────────────────────────────────────────┘
            │
    ┌───────┼───────┬───────────┬──────────┐
    ▼       ▼       ▼           ▼          ▼
 Counter  Dot    Icon       Text       Validation  Notification
("5",     (red   (SVG)      ("NEW",    (error/     (counter +
 "99+")   dot)              "PRO")     success)     pulse anim)
```

### BaseControl Integration
- `BaseControl.Badge` property (`IBeepBadge?`) — setter auto-binds to parent
- `BaseControl.SetBadge(badge)` / `BaseControl.ClearBadge()` methods
- `RegisterFloatingBadge()` called from `OnParentChanged` — badge re-attaches on parent change
- `Dispose(bool)` detaches badge properly

### Built-in Badge Types

| Class | Use | Key Props |
|-------|-----|-----------|
| `BeepCounterBadge` | Numeric counter | `DisplayText`, `MaxDisplay`, `ShowPlus` |
| `BeepDotBadge` | Status dot | `BadgeDiameter=10`, no text |
| `BeepIconBadge` | SVG icon overlay | `SvgPath`, `SetIcon(svg)` |
| `BeepTextBadge` | Text label | `DisplayText`, pill shape |
| `BeepValidationBadge` | Form validation | `State` (Error/Success/Warning/Info/None), auto-SVG+color |
| `BeepNotificationBadge` | Live notification | extends `BeepCounterBadge` + pulse animation |

### Location System
- `BadgeAnchor`: TopLeft, TopRight, TopCenter, BottomLeft, BottomRight, BottomCenter, MiddleLeft, MiddleRight, MiddleCenter, Custom
- `BadgeSide` + `BadgeAlignment`: CSS-style (Top+End = top-right)
- `BadgeLocation.ComputeBounds(ownerBounds, badgeSize)`: unified rectangle computation, supports anchor + offset + relative position + custom `BoundsProvider`
- `BadgeLocations` static presets: `TopRight(nudge)`, `Relative(fx, fy)`, `Css(side, alignment)`, `Custom(provider)`
- Fluent setters: `.SetText("5").At(0.9f, 0.1f)`, `.With(Right, Start)`

### User-Defined Badge Types
```csharp
// Register custom type
BeepBadgeFactory.Register<MyCustomBadge>("MyBadge");
// Or with factory
BeepBadgeFactory.Register("MyBadge", () => new MyCustomBadge("hi"));

// Use it
myButton.ShowBadge(BeepBadgeFactory.Create("MyBadge"));
```

### Files Created

| File | Purpose |
|------|---------|
| `Badges\BadgeAnchor.cs` | 10-value anchor enum |
| `Badges\BadgeShape.cs` | 6-value shape enum (Circle, Pill, Diamond, etc.) |
| `Badges\BadgeSide.cs` | Top/Right/Bottom/Left enum |
| `Badges\BadgeAlignment.cs` | Start/Center/End enum |
| `Badges\BadgeLocation.cs` | Unified position with `ComputeBounds()` |
| `Badges\BadgeLocations.cs` | Preset factory methods |
| `Badges\ValidationState.cs` | None/Error/Success/Warning/Info enum |
| `Badges\IBeepBadge.cs` | Interface: Attach, Detach, Reposition, Location |
| `Badges\BeepFloatingBadge.cs` | Base UserControl (376 lines) |
| `Badges\BeepBadgeManager.cs` | Lifecycle manager (static, per-parent) |
| `Badges\BeepBadgeFactory.cs` | User-defined type registry |
| `Badges\BeepBadgeExtensions.cs` | Extension methods for any Control |
| `Badges\Builtin\BeepCounterBadge.cs` | Numeric counter |
| `Badges\Builtin\BeepDotBadge.cs` | Solid dot |
| `Badges\Builtin\BeepIconBadge.cs` | SVG icon (uses StyledImagePainter) |
| `Badges\Builtin\BeepTextBadge.cs` | Short text |
| `Badges\Builtin\BeepValidationBadge.cs` | Form validation (uses SvgsUIcons) |
| `Badges\Builtin\BeepNotificationBadge.cs` | Counter + pulse animation |

### Files Modified
- `BaseControl\BaseControl.Properties.cs` — added `Badge` property
- `BaseControl\BaseControl.Methods.cs` — added `SetBadge()`, `ClearBadge()`
- `BaseControl\BaseControl.Events.cs` — added `RegisterFloatingBadge()`, called from `OnParentChanged`
- `BaseControl\BaseControl.cs` — added badge cleanup in `Dispose(bool)`

### Key Design Decisions
- **Separate UserControl per badge** (not external drawing) — enables click, focus, animation, z-order
- **Per-parent static manager** — handles lifecycle without each badge needing to track parent events
- **BringBadgesToFront on parent Paint** — cheaper than subclassing WndProc
- **BOTH badge systems coexist** — legacy `BadgeText` + external drawing still works; new `IBeepBadge Badge` property for UserControl approach
- **No namespace collision** — new `BadgeShape` in `Badges` namespace doesn't conflict with old `BadgeShape` from `Beep.Vis.Modules` because they're in different namespace hierarchies; resolution is correct via C# parent-namespace priority

---

## Master TODO Tracker

### Phase 1 — Icon Overlay System ⬜
- [ ] Create `ValidationIconType` and `IconPosition` enums in `BaseControl\Models\`
- [ ] Add `ValidationIcon`, `ValidationIconPosition`, `ValidationIconSize`, `ValidationIconPadding` properties to `BaseControl.Properties.cs`
- [ ] Implement `ApplyValidationIcon()` in `BaseControl.Methods.cs` — registers/updates external drawing
- [ ] Create auto-subscription: when `ValidationIcon` changes, auto-register/clear drawing
- [ ] Test: set `ValidationIcon = Error` on a textbox, verify icon appears

### Phase 2 — Redline / Underline Indicators ⬜
- [ ] Create `IndicatorLineStyle` enum in `BaseControl\Models\`
- [ ] Add `ShowIndicatorLine`, `IndicatorLineStyle`, `IndicatorLineColor` properties
- [ ] Implement `ApplyIndicatorLine()` — draws colored line below control
- [ ] Coordinate with validation state: error→red, success→green, etc.
- [ ] Test: set `ShowIndicatorLine = true` with error state

### Phase 3 — Simple ShowValidation API ⬜
- [ ] Create `ValidationState` enum and `ValidationStateChangedEventArgs`
- [ ] Add `ShowValidation(state, message)` and `ClearValidation()` methods
- [ ] Wire to existing `HasError`, `ErrorText`, `ErrorColor` properties
- [ ] Wire to icon overlay and indicator line
- [ ] Fire `ValidationStateChanged` event
- [ ] Test: `ShowValidation(Error, "Required field")` on input control

### Phase 4 — Batch Registration Helpers ⬜
- [ ] Add `AddExternalDrawingForAll(predicate, ...)` overloads
- [ ] Add `AddExternalDrawingForAll<T>(...)` generic overload
- [ ] Add `ClearExternalDrawingForAll(predicate)`
- [ ] Test: register drawing for all TextBox children in a form

### Phase 5 — Pre-built Drawing Handler Library ⬜
- [ ] Create `BaseControlDrawingHandlers` static class
- [ ] Implement `IconOverlay` — SVG icon with position/size/tint
- [ ] Implement `Underline` — colored line at specified offset
- [ ] Implement `CounterBadge` — numeric badge with background
- [ ] Implement `FocusRing` — animated focus indicator
- [ ] Add XML docs for all factory methods
- [ ] Test: each handler renders correctly at different DPI scales

### Phase 6 — Animation Hooks ⬜
- [ ] Add `ExternalDrawingAnimation` enum
- [ ] Extend `ExternalDrawingFunction` with animation state (opacity, progress, timer)
- [ ] Add `AddChildExternalDrawing(... animation)` overload
- [ ] Implement fade-in animation (0→1 opacity over duration, progressive invalidation)
- [ ] Implement slide-in animation (clip-rect expansion)
- [ ] Test: smooth animation without jank

### Phase 7 — Documentation & Samples ⬜
- [ ] Update `BaseControl/README.md` — new feature sections
- [ ] Create `BaseControl/EXTERNAL_DRAWING_GUIDE.md` — quick-start, patterns, performance
- [ ] Create sample form in test project demonstrating all features
- [ ] Add code snippets for common use cases (form validation, inline errors, success toast)

### Phase 8 — Floating Badge UserControl System ✅ (COMPLETED)
- [x] Create `Badges\` directory structure with `Builtin\` subdirectory
- [x] Create `BadgeAnchor` (10 presets), `BadgeShape` (6 shapes), `BadgeSide`, `BadgeAlignment` enums
- [x] Create `ValidationState` enum (None/Error/Success/Warning/Info)
- [x] Create `BadgeLocation` class with `ComputeBounds(ownerBounds, badgeSize)`
- [x] Create `BadgeLocations` static preset factory
- [x] Create `IBeepBadge` interface (Attach, Detach, Reposition, Location, Target)
- [x] Create `BeepFloatingBadge` base UserControl (anti-aliased GraphicsPath, drop shadow, border)
- [x] Create `BeepBadgeManager` lifecycle manager (per-parent, BringToFront, auto-dispose)
- [x] Create `BeepBadgeFactory` with `Register<T>(name)` and `Create(name)`
- [x] Create `BeepBadgeExtensions` extension methods (ShowBadge, ShowCounterBadge, ShowDotBadge, etc.)
- [x] Create built-in badges: Counter, Dot, Icon, Text, Validation, Notification
- [x] Add `Badge` property to `BaseControl` + `SetBadge`/`ClearBadge` methods
- [x] Hook `OnParentChanged` → `RegisterFloatingBadge()`
- [x] Hook badge cleanup in `BaseControl.Dispose(bool)`
- [x] Build verified: 0 errors
- [ ] Test: badge follows control on move, resize, parent change, dispose
- [ ] Test: user-defined `MyCustomBadge` registers and renders
- [ ] Test: all 10 anchor presets + offset + relative + custom BoundsProvider
- [ ] Test: pulse animation on NotificationBadge
- [ ] Sample form demonstrating all built-in badge types
- [ ] Update `BaseControl\README.md` with badge mechanism section

---

## File Plan

| New File | Location |
|----------|----------|
| `ValidationIconType.cs` (enum) | `BaseControl\Models\` (inline in existing Models) |
| `IconPosition.cs` (enum) | `BaseControl\Models\` |
| `IndicatorLineStyle.cs` (enum) | `BaseControl\Models\` |
| `ExternalDrawingAnimation.cs` (enum) | `BaseControl\Models\` |
| `ValidationState.cs` (enum) | `BaseControl\Models\` |
| `ValidationStateChangedEventArgs.cs` | `BaseControl\Models\` |
| `BaseControlDrawingHandlers.cs` | `BaseControl\Helpers\` |
| `EXTERNAL_DRAWING_GUIDE.md` | `BaseControl\` |

| Modified File | Changes |
|---------------|---------|
| `BaseControl.Properties.cs` | Add icon overlay + indicator line properties |
| `BaseControl.Methods.cs` | Add `ShowValidation`, `ClearValidation`, icon overlay registration |
| `BaseControl.LabelandError.cs` | Wire validation icon into existing label/error drawing |
| `BaseControl.Events.cs` | Wire `ValidationStateChanged` event |
| `ControlExternalDrawingHelper.cs` | Add animation support, batch registration |
| `BaseControl\README.md` | Document new features |

---

## Success Criteria
- Developer can write `myTextbox.ShowValidation(ValidationState.Error, "Invalid format")` and get an error icon + red underline automatically
- Zero manual delegate creation needed for 90% of use cases
- All icons from `SvgsUIcons` render correctly at 96/144/192 DPI
- Performance: external drawing adds <1ms per control per paint frame
- Backward compatible: existing code using `AddChildExternalDrawing` continues to work
