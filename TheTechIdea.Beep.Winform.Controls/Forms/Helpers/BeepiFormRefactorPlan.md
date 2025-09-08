# BeepiForm Refactor Plan (Helper Pattern)

Goal: Eliminate partial classes and refactor BeepiForm into a single class that composes small, focused helper components located in `Forms/Helpers/`. Helpers must be reusable so ANY Form can opt?in to modern Beep UI behavior (rounded corners, custom caption, shadow/glow, theming, snap hints, acrylic, animations, system buttons, etc.).

---
## 1. Current Responsibilities (from existing partials / code regions)
| Concern | Current Location | Notes |
|---------|------------------|-------|
| Core state fields (border, radius, theme, flags) | BeepiForm.cs / Style | Needs consolidation & ownership distribution |
| Theme application + child theming | ApplyTheme / ApplyFormStyle | Separate theme vs. style vs. runtime mutation |
| Caption bar drawing + hit test + system buttons | Caption partial (not shown here) | Should be standalone & pluggable |
| Ribbon integration | Ribbon partial | Optional module |
| Shadow & Glow & Border painting | Style partial OnPaint | Needs layered painter pipeline |
| Region (rounded corners) management | UpdateFormRegion | Must coordinate with DPI + resize + maximize |
| Hit testing (WM_NCHITTEST) | WndProc | Move logic to HitTest helper |
| Resize / drag logic | WndProc + IsInDraggableArea | Extract to Input / Drag helper |
| Snap hints / snapping visuals | Snap partial | Isolated helper |
| Animations (fade/size) | Animations partial | Keep optional |
| Acrylic / blur effect | Acrylic partial | OS capability aware |
| DPI handling | Dpi mode switch | Abstract to Dpi helper |
| Overlay painters list | Style partial | Maintain extensible painter registry |
| Layout adjustment (Padding, DisplayRectangle, AdjustControls) | BeepiForm.cs | Provide layout helper and contract |
| System buttons metrics / rectangles | Caption bar logic | Encapsulate in SystemButtons helper |
| Event aggregation (mouse handlers) | BeepiForm.cs | Input helper event bus |
| Maximize fixes (border reset) | ApplyMaximizedWindowFix | Region + Border coordination |

---
## 2. Target Helpers (Namespace: `TheTechIdea.Beep.Winform.Controls.Forms.Helpers`)
Each helper is internal (or public if reuse desired) and receives a reference to the host Form via interface `IBeepModernFormHost`.

### 2.1 Core Interface
```csharp
public interface IBeepModernFormHost
{
    Form AsForm { get; }
    IBeepTheme CurrentTheme { get; }
    int BorderRadius { get; set; }
    int BorderThickness { get; set; }
    Padding Padding { get; set; }
    bool IsInDesignMode { get; }
    void Invalidate();
    void UpdateRegion();
}
```
Additional optional capability interfaces:
- `ICaptionMetricsProvider`
- `ISystemButtonsProvider`
- `IOverlayPainterRegistry`

### 2.2 Helper List
| Helper | Purpose | Key Public Members |
|--------|---------|--------------------|
| FormStateStore | Holds mutable state previously scattered (flags, cached values) | Properties for IsDragging, InMoveOrResize, SavedRadius, SavedThickness |
| FormDpiHelper | Optional manual DPI behaviors | OnDpiChanged, TranslateSize/Point |
| FormThemeHelper | Loads & applies theme + child propagation | ApplyTheme(), ApplyChildTheme(Control) |
| FormStyleHelper | Applies style presets (Modern, Material, etc.) separate from theme | ApplyStyle(BeepFormStyle style) |
| FormRegionHelper | Manages rounded region & reacts to resize/maximize | Update(), Clear(), ComputePath(Size) |
| FormShadowGlowPainter | Pre-paint shadow & glow layers | Paint(Graphics g, Rectangle bounds) |
| FormBackgroundPainter | Fills background & optional gradient/acrylic fallback | PaintBackground(Graphics g, Rectangle bounds) |
| FormBorderPainter | Draws border respecting radius & thickness | PaintBorder(Graphics g, Rectangle bounds) |
| FormCaptionBarHelper | Metrics + drawing + hit test for caption & drag area | Layout(), Paint(), HitTest(Point) |
| FormSystemButtonsHelper | Close/Max/Min rectangles, states & painting | UpdateLayout(Rectangle captionArea), Paint(), HitTest(Point), ProcessClick(ButtonType) |
| FormHitTestHelper | Central WM_NCHITTEST logic (edges + caption + buttons) | HitTest(Point client) -> HT* codes |
| FormInputHandlerHelper | Mouse down/move/up for dragging & state flags | BeginDrag(Point), UpdateDrag(Point), EndDrag() |
| FormSnapHelper | Handles snap hints (visual overlays) | OnDragMove(), ShowHint(), Clear() |
| FormAnimationHelper | Basic show/hide / transition animations | Play(AnimationType) |
| FormAcrylicHelper | OS dependent blur / mica | Enable(), Disable(), Reapply() |
| FormOverlayPainterRegistry | Maintains ordered list of overlay painters | Add(Action<Graphics>), InvokeAll(Graphics) |
| FormLayoutHelper | Computes AdjustedClientRectangle & DisplayRectangle unified with painting bounds | GetClientArea(), GetContentArea() |
| FormSystemMenuHelper (optional) | Provides system menu on right-click | ShowMenu(Point screen) |

### 2.3 Painter Pipeline
Order of invocation inside Form.OnPaint:
1. Region update (if dirty) – RegionHelper
2. Shadow/Glow – ShadowGlowPainter (behind main shape)
3. Background – BackgroundPainter
4. Caption Bar (if enabled) – CaptionBarHelper
5. Border – BorderPainter
6. System Buttons – SystemButtonsHelper
7. Overlay painters – OverlayPainterRegistry

All painting helpers receive a consistent `Rectangle bounds = layoutHelper.GetPaintBounds()`.

### 2.4 Event / Message Flow
- BeepiForm overrides WndProc and delegates:
  - WM_NCHITTEST -> HitTestHelper
  - WM_DPICHANGED -> DpiHelper
  - WM_ENTERSIZEMOVE / WM_EXITSIZEMOVE -> StateStore + RegionHelper
  - WM_GETMINMAXINFO -> (optional future) LayoutHelper

- Mouse events delegate to InputHandlerHelper (which can in turn notify SnapHelper).

### 2.5 Public API Preservation
We keep existing public properties (BorderRadius, BorderThickness, Theme, FormStyle, etc.). Internally they delegate to helpers.
No breaking changes for consumers.

---
## 3. Refactor Phases

### Phase 0 – Scaffold
1. Create interfaces + base contracts (IBeepModernFormHost, IOverlayPainterRegistry, etc.).
2. Implement minimal FormStateStore.
3. Inject helpers into BeepiForm (composition root).
4. Add a feature flag `bool UseHelperInfrastructure = true;` (default true) for incremental switch.

### Phase 1 – Non-visual Core
1. Move region logic to FormRegionHelper.
2. Move state flags & maximize fix to FormStateStore + RegionHelper.
3. Replace direct field access with properties that wrap FormStateStore where appropriate.

### Phase 2 – Layout & Hit Testing
1. Introduce FormLayoutHelper and consolidate GetAdjustedClientRectangle & DisplayRectangle.
2. Extract WM_NCHITTEST logic to HitTestHelper.
3. InputHandlerHelper handles drag detection logic.

### Phase 3 – Painting Pipeline
1. Move OnPaint body to orchestrator pattern in BeepiForm calling: shadow/glow -> background -> caption -> border -> overlays.
2. Extract each concern to its painter/helper.
3. Provide extension points: `RegisterOverlayPainter(Action<Graphics>)` now backed by OverlayPainterRegistry.

### Phase 4 – Caption & System Buttons
1. Implement CaptionBarHelper + SystemButtonsHelper, remove caption partial code.
2. HitTestHelper queries these helpers.

### Phase 5 – Theme & Style
1. ThemeHelper handles ApplyTheme (color resolution).
2. StyleHelper sets metrics & delegates to RegionHelper / ShadowGlowPainter.

### Phase 6 – Advanced Features
1. AcrylicHelper (optional, only if previously available code).
2. SnapHelper for docking hints.
3. AnimationHelper for show/hide transitions.

### Phase 7 – Cleanup
1. Remove all partial class files (merge into single BeepiForm.cs or keep helpers only outside).
2. Mark deprecated internal fields with `[Obsolete]` if external libs may reflect.
3. Ensure Region disposed properly.
4. Add XML documentation to new helpers.

### Phase 8 – Generalization
1. Provide a static `ModernFormActivator.Enable(Form form)` that attaches helpers dynamically to ANY Form (creates adapter implementing IBeepModernFormHost via composition).
2. Publish minimal usage snippet.

---
## 4. File/Type Naming
```
Forms/Helpers/
  IBeepModernFormHost.cs
  FormStateStore.cs
  FormDpiHelper.cs
  FormThemeHelper.cs
  FormStyleHelper.cs
  FormRegionHelper.cs
  FormLayoutHelper.cs
  FormShadowGlowPainter.cs
  FormBackgroundPainter.cs
  FormBorderPainter.cs
  FormCaptionBarHelper.cs
  FormSystemButtonsHelper.cs
  FormHitTestHelper.cs
  FormInputHandlerHelper.cs
  FormSnapHelper.cs
  FormAnimationHelper.cs
  FormAcrylicHelper.cs
  FormOverlayPainterRegistry.cs
  ModernFormActivator.cs (optional)
```

---
## 5. Responsibilities Mapping Example
| Existing Field | New Owner |
| `_borderRadius` | FormStateStore (exposed via BeepiForm.BorderRadius property) |
| `_shadowDepth` | FormShadowGlowPainter (configured by StyleHelper) |
| `_enableGlow/_glowColor/_glowSpread` | FormShadowGlowPainter |
| `_showCaptionBar`, `_captionHeight` | FormCaptionBarHelper |
| `_btnClose/_btnMax/_btnMin` rects | FormSystemButtonsHelper |
| `_inMoveOrResize` | FormStateStore |

---
## 6. Incremental Migration Strategy
1. Introduce helpers without removing old code (shadowed but unused when UseHelperInfrastructure = true).
2. Validate painting equivalence (visual regression check manually).
3. Remove legacy partials after parity.

---
## 7. Extensibility Considerations
- All painters accept injection of optional decorators (e.g. custom watermark painter).
- OverlayPainterRegistry maintains ordered list; consumers can insert at specific index (future enhancement).
- Future: add accessibility hooks (focus ring, high contrast adaptation) through a dedicated helper.

---
## 8. Risks & Mitigations
| Risk | Mitigation |
|------|------------|
| Subtle layout drift (off-by-1 px) | Centralize rectangles in LayoutHelper; unit test relative relationships |
| Region flicker during resize | Defer region update until resize completed (listen to WM_ENTERSIZEMOVE/WM_EXITSIZEMOVE) |
| Theme timing (ApplyTheme before helpers ready) | Initialize helpers first in constructor before calling ApplyTheme |
| Backwards compatibility | Keep public API unchanged; add obsolete attributes only internally |

---
## 9. Acceptance Criteria
- No visual right-side or bottom artifact.
- Shadow/glow identical (or improved) vs. previous implementation.
- Resizing + maximize restores borders & radius properly.
- Hit test edges accurate for all DPI scales.
- Caption bar draggable only in defined area; system buttons respond.
- Form can be instantiated in designer (helpers must be design-time safe: guard Handle access & license mode checks).

---
## 10. Next Step
If you approve this plan: I will scaffold the interfaces + 3 core helpers first (State, Region, Layout) and rewire BeepiForm minimally, then proceed phase-by-phase.

Let me know to proceed or adjust before coding.
