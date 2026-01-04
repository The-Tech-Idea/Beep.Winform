# Beep WinForms controls authoring guide

This guide explains how to build new WinForms controls on top of the shared `BaseControl` helper architecture. It covers the paint pipeline, Material Design layout/sizing, hit-testing, DPI, tooltips, external drawing hooks, badges, and image rendering with `BeepImage` and `ImagePainter`.

Key concepts
- Derive from `BaseControl` (or `BeepControl` if you need legacy built-ins).
- Always base layout and child rendering on `DrawingRect` (never `ClientRectangle`). `DrawingRect` is Material-aware and already excludes borders, padding, icons, labels, etc.
- Separate responsibilities:
  - Layout: compute rectangles once per frame from `DrawingRect` in a `CalculateLayout` method.
  - Painting: override `DrawContent(Graphics g)` and draw your elements inside `DrawingRect` or sub-rects of it.
  - Hit-testing: register/update hit areas via `AddHitArea(...)` wrappers that use the same rectangles you draw.
  - Input: let `BaseControl` route events to `ControlInputHelper` and `ControlHitTestHelper`.
  - Theme: override `ApplyTheme()` and propagate the theme to child controls/images.
- DPI/scaling: use `ScaleValue`, `GetScaledFont`, `SafeApplyFont`, and size helpers.
- Images: prefer `BeepImage` for themed SVG recolor and animations; use `ImagePainter` for painter-style, non-control rendering and advanced effects.

Paint pipeline (what `BaseControl` does)
- `OnPaint` orchestrates rendering:
  1) Clear the outside background (Material-aware).
  2) Paint the inner area using `PaintInnerShape(Graphics g, Color fillColor)` which respects rounded corners and Material paths when present.
  3) `ExternalDrawing` hooks (`DrawingLayer.BeforeContent`).
  4) Call `DrawContent(g)` (virtual):
     - If `EnableMaterialStyle` = true, it uses `BaseControlMaterialHelper` to draw MD3 field (fill, border, icons, elevation/state), then supporting text.
     - Otherwise it uses `ControlPaintHelper` (`_paint.Draw(g)`) to draw non-Material background, borders, label/helper, shadows, gradients.
  5) Optionally draw hit-list UI components if `AutoDrawHitListComponents` is true.
  6) Effects overlays (ripples, etc.).

What to override
- `DrawContent(Graphics g)`: call `base.DrawContent(g)` to keep the default field/border/icons, then draw your custom content on top. If you want a fully custom look, skip calling the base once you handle everything yourself.
- `ApplyTheme()` (and optionally `SetFont()`): set Back/Fore/Border colors, then propagate `Theme`, `ParentBackColor`, and `IsChild = true` to child controls/images as applicable.
- `GetPreferredSize(Size proposedSize)` only if you must change default sizing; otherwise use the Material sizing helpers below.

Material-aware layout rule
- Use `DrawingRect` for all layout and child drawing. The framework keeps it up to date for both Material and non-Material modes:
  - When Material is enabled, `DrawingRect` is computed from the Material helper content area (excludes paddings/effects/icons/labels).
  - When Material is disabled, `DrawingRect` is the inner area after borders, padding, and shadows.
- You do not need to call `GetMaterialContentRectangle()` for normal drawing; prefer `DrawingRect`. Call the method only if you specifically need a fresh content rect before `_paint.UpdateRects()` runs.

Typical layout pattern
```
private Rectangle _contentRect;
private Rectangle _valueRect;

private void CalculateLayout()
{
    var baseRect = DrawingRect; // always base on DrawingRect
    int padding = ScaleValue(8);

    _contentRect = baseRect;
    _valueRect = new Rectangle(
        _contentRect.Left + padding,
        _contentRect.Top + padding,
        Math.Max(0, _contentRect.Width - padding * 2),
        Math.Max(0, _contentRect.Height - padding * 2));
}

protected override void DrawContent(Graphics g)
{
    base.DrawContent(g);
    CalculateLayout();

    TextRenderer.DrawText(g, Text, Font, _valueRect, ForeColor,
        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
}
```

Child component drawing contract (important)
- For any child that implements `IBeepUIComponent.Draw(Graphics g, Rectangle rect)`, the child must treat `rect` as its own `DrawingRect` and draw inside it. Example from `BeepImage`:
  - `public override void Draw(Graphics graphics, Rectangle rectangle) { DrawingRect = rectangle; DrawImage(graphics, rectangle); }`
- When composing children inside a parent (e.g., `BeepButton`), compute sub-rectangles from the parent’s `DrawingRect` and pass them to child `Draw`/`DrawImage` methods. Do not set child control `Location/Size` just to render; paint directly into the given rect.

Hit-testing
- Register regions using the same rectangles you paint:
  - `AddHitArea(string name, Rectangle rect, IBeepUIComponent component = null, Action hitAction = null)`
  - Add child controls via `AddHitTest(Control childControl)`.
- Query/update: `HitTest(Point location, out ControlHitTest ht)`, `ClearHitList()`, `UpdateHitTest(...)`.
- Rendering: `AutoDrawHitListComponents` lets BaseControl draw `HitList` UI components; cap via `MaxHitListDrawPerFrame`.
- Event routing: mouse/keyboard routed through `ControlInputHelper`; send custom events with `SendMouseEvent(...)`.

Tooltips
- Use `ShowToolTip(string title, string text)` and `HideToolTip()` or `ShowToolTipIfExists()` to show `ToolTipText`.

External drawing hooks and badges
- Overlays: `AddChildExternalDrawing(Control child, DrawExternalHandler handler, DrawingLayer layer)`, `ClearChildExternalDrawing(...)`.
- Badges: set `BadgeText` and colors; `UpdateRegionForBadge()` expands Region and invalidates parent badge area automatically.

DPI and fonts
- Per-monitor DPI updates on `WM_DPICHANGED`. Base invalidates rects, updates Material layout and repaints.
- Use `SafeApplyFont(Font, preserveLocation)` and helpers: `GetScaleFactor`, `GetScaledBounds`, `GetSuitableSizeForTextAndImage`, `GetScaledFont`.
- Disable scaling with `DisableDpiAndScaling` when necessary.

BeepImage quick-start and best practices
- Purpose: a control that displays images (SVG/PNG/JPG/BMP) with optional theming, clipping, transforms, and built-in animations/caching.
- Core properties:
  - `ImagePath` (or `EmbeddedImagePath`): supports file path or embedded resource name; plain filename is mapped via `ImageListHelper`.
  - `ApplyThemeOnImage`, `ImageEmbededin` (Button/Menu/AppBar/etc.), `PreserveSvgBackgrounds`.
  - Visuals: `ClipShape` (None/Circle/RoundedRect/Ellipse/Diamond/Triangle/Hexagon/Custom), `CornerRadius`, `CustomClipPath`.
  - Effects: `Opacity`, `Grayscale`.
  - Scaling: `ScaleMode` (KeepAspectRatio, KeepAspectRatioByWidth/Height, Stretch), `BaseSize`.
  - Transforms/animations: `IsSpinning` + `SpinSpeed`, `AllowManualRotation`, `ManualRotationAngle`, `IsPulsing`, `IsBouncing`, `IsFading`, `IsShaking`. Helpers: `Rotate90Clockwise/CounterClockwise/180`, `FlipHorizontal/Vertical`, `ResetTransformations()`.
- Theming:
  - When `ApplyThemeOnImage` is true, `ApplyTheme()` calls `ApplyThemeToSvg()` which recolors SVG nodes based on `ImageEmbededin` mapping using current theme (or `ForeColor`/`BackColor` by default). `OnForeColorChanged`/`OnBackColorChanged` also trigger recolor and cache invalidation.
- Rendering and caching:
  - Use `DrawImage(Graphics g, Rectangle imageRect)` to draw; control caches a rendered bitmap and regenerates only when state changes (path/rect/rotation/scale/alpha/clip/flip/theme color signature, etc.).
  - Advanced cropping: `Draw(Graphics g, Rectangle destRect, Rectangle sourceRect)`.
- Using inside `BaseControl`:
  - In `DrawContent`, get a sub-rect from `DrawingRect` and call `_beepImage.DrawImage(g, subRect)`.
  - If adding as a child control instead, set `IsChild = true`, `ParentBackColor`, and propagate `Theme`.
- Embedded resources:
  - Prefer `Svgs` constants, e.g. `beepImage.ImagePath = Svgs.Search;` The control resolves embedded resources by name and also maps simple filenames.
  - The `Svgs` class exposes helpers to enumerate and validate resources.
  - Avoid rasterizing SVGs at load unless required; keep vector via `ImagePath` over setting `Image` with a pre-rendered bitmap.

ImagePainter quick-start (painter, not a control)
- Purpose: draw images from code in any `Graphics` using the same features as `BeepImage` without creating a child control.
- Typical use:
```
using var painter = new ImagePainter();
painter.ImagePath = Svgs.Cancel; // or a file path
painter.ScaleMode = ImageScaleMode.KeepAspectRatio;
painter.ClipShape = ImageClipShape.Circle;
painter.DrawImage(graphics, bounds); // bounds normally comes from your control's DrawingRect or a sub-rect
```
- Theming and context:
  - Set `CurrentTheme`, `ImageEmbededin`, and `ApplyThemeOnImage = true` to recolor SVGs. `ApplyThemeToSvg()` updates all nodes and flushes styles.
  - `DrawSvg(Graphics g, string iconKeyOrPath, Rectangle destRect, Color color, float opacity = 1.0f)` is a convenience that resolves keys via `ImageListHelper` and draws with color/opacity.
- Transforms and effects:
  - `ManualRotationAngle`, `PulseScale`, `FadeAlpha`, `Grayscale`, `FlipX/FlipY`; call `ResetTransformations()` to clear.
  - No internal timer; use a `Timer` in your control to animate (see `ImagePainterExample.CustomImageControl`).
- Performance:
  - Images loaded from file are cloned to avoid file locks. Internal caching regenerates only on state changes; call property setters to invalidate cache.
  - Use `BatchConfiguration(ImagePainter[] painters, IBeepTheme theme)` pattern to configure many painters uniformly.

Using Svgs and extensions
- `Svgs` contains strongly-typed embedded SVG resource paths: e.g., `Svgs.Search`, `Svgs.NavUser`, etc. Use these strings with `BeepImage.ImagePath` or `ImagePainter.ImagePath`.
- Utilities:
  - `Svgs.ResourceExists(string path)`, `Svgs.GetAvailableResources()`, `Svgs.GetAllPaths()`.
  - Convenience extensions in `Svgs` (e.g., `SetSearchIcon`, `SetUserIcon`) call `beepImage.ImagePath = Svgs.*`.
- Note: `BeepSvgPathsExtensions.SetSvgFromResource` uses a `resource://` scheme and `LoadSvgFromEmbeddedResource` rasterizes the SVG via `ImageLoader`. Prefer setting `ImagePath = Svgs.*` to keep vector quality and enable full theming.

Performance tips
- BaseControl is double-buffered. Prefer `EnableHighQualityRendering = true` for quality; turn it off for raw speed.
- If you render many hit areas/components, consider disabling `AutoDrawHitListComponents` and draw only what’s visible.
- Use `UseExternalBufferedGraphics` only after measuring a benefit.
- Avoid allocating in tight loops; reuse `Rectangle` fields computed in `CalculateLayout()`.

Design-time safety
- Guard dynamic logic with `if (DesignMode) return;` if needed.
- Create/measure fonts/graphics inside try/catch blocks where design-time may throw, similar to the core helpers.

Checklist for a new control
- [ ] Derives from `BaseControl` (or `BeepControl`).
- [ ] All layout and child drawing use `DrawingRect` (no `ClientRectangle`).
- [ ] Fields for parts (images/labels/buttons) created in an init method.
- [ ] `CalculateLayout` derives from `DrawingRect` and uses `ScaleValue` for sizes.
- [ ] `DrawContent` calls `base.DrawContent(g)` then draws custom foreground; images use `BeepImage.DrawImage` or an `ImagePainter` with rects taken from `DrawingRect`.
- [ ] Hit areas added/updated after layout with the same rectangles you draw; mouse routes through built-in helper.
- [ ] `ApplyTheme` sets colors and propagates to all parts; call `ApplyThemeToSvg()` for SVG recolor when `ApplyThemeOnImage` is true.
- [ ] If using `ImagePainter`, dispose it in `Dispose`.
- [ ] External overlays wired via `AddChildExternalDrawing` when needed.
- [ ] For Material: rely on sizing helpers or call `ApplyMaterialSizeCompensation()`.

Troubleshooting
- Hover doesn’t update: ensure hit areas refresh after layout; invalidate on state change.
- Click not firing: verify `AddHitArea` is called after layout and the `TargetRect` matches the painted area.
- Content overlaps Material icons: `DrawingRect` already excludes icon areas; if you see overlap, ensure you aren’t using `ClientRectangle`.
- SVG doesn’t recolor: ensure `ApplyThemeOnImage = true` and a valid `CurrentTheme`/`Theme` + appropriate `ImageEmbededin`.
- Layout shifts on DPI: compute sizes via `ScaleValue`/helpers and avoid absolute pixels.
- Helper/error text clipped: ensure Material minimum height (`GetMaterialMinimumHeight()`) or call size compensation.
