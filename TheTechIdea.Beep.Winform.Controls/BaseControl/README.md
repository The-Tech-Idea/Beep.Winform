# BaseControl architecture and painting pipeline

This document explains how `BaseControl` renders and how to extend it safely. It summarizes the helpers-first design and outlines a path to adopt the painter methodology.

## Structure
- `BaseControl` (partial): core behavior, theming, events, drawing entry points.
- `Helpers/ControlPaintHelper`: non?Material background, borders, gradients, shadows, labels/helper for classic mode.
- `Helpers/BaseControlMaterialHelper`: Material Design 3 field background, state layers, elevation, outline/underline, and icon layout/drawing.
- Other helpers: `ControlEffectHelper` (effects/ripple), `ControlHitTestHelper` (hit areas), `ControlExternalDrawingHelper` (overlays/badges), `ControlDpiHelper` (DPI/scaling), `ControlDataBindingHelper`.

## Render flow (what draws and when)
- `OnPaint` ? `DrawContent(Graphics g)` (overridable by derived controls)
  - If `EnableMaterialStyle == true`:
    1) Ensure `_materialHelper` exists ? `_materialHelper.UpdateLayout()`
    2) `_materialHelper.SetElevation(...)`
    3) `_materialHelper.DrawAll(g)` ? draws background/fill (if any), state layer, border, icons, and keeps `InnerShape`
    4) `BaseControl` draws `LabelText` and supporting text (helper/error) around the field using rectangles computed by `_materialHelper`
  - Else (classic mode): `_paint.EnsureUpdated(); _paint.Draw(g)` ? background/gradients/shadow/borders/labels in one place

Notes
- `ControlPaintHelper` never draws when Material is enabled; Material is fully handled by `BaseControlMaterialHelper` and the extra text drawing in `BaseControl` itself.
- `InnerShape` is updated by the Material helper to allow `PaintInnerShape` to fill the inner area correctly.

## Layout rectangles (Material)
- `_materialHelper.GetInputRect()` ? entire control content area
- `_materialHelper.GetFieldRect()` ? field background/outline area (excludes label/supporting text)
- `_materialHelper.GetContentRect()` ? text/content area including icon spacing
- `_materialHelper.GetAdjustedContentRect()` ? content area excluding icon hit regions
- Use `BaseControl.GetContentRect()` for drawing your inner content in Material mode.

## Icons (Material)
- Properties: `LeadingIconPath`, `TrailingIconPath`, `LeadingImagePath`, `TrailingImagePath`, `IconSize`, `IconPadding`, `ShowClearButton`
- `_materialHelper` lays out and draws icons; `BaseControl` registers hit areas via `ControlHitTestHelper` for clicks.

## Material properties (high level)
- Toggle: `EnableMaterialStyle`
- Variant: `MaterialVariant` (Outlined, Filled, Standard)
- Visual tokens: `MaterialBorderRadius`, `MaterialShowFill`, `MaterialFillColor`, `MaterialOutlineColor`, `MaterialPrimaryColor`, `ErrorText`, `ErrorColor`
- Elevation: `MaterialElevationLevel` (0–5), `MaterialUseElevation`
- Presets: `StylePreset` to quickly configure a style bundle

## For derived controls: where to draw your content
- Always draw inside `DrawContent(Graphics g)`.
- Do not draw borders/shadows manually; they are handled by helpers.
- In Material mode, get the content area from `GetContentRect()` or `_materialHelper.GetAdjustedContentRect()` if you want to ignore icon regions.
- To fill the inner area, call `PaintInnerShape(g, effectiveBackColor)`; it uses `InnerShape` when available.

## Painter methodology (strategy) – feasibility and proposal
There is an existing painter ecosystem in the repository for widgets/cards with a well-defined contract. `BaseControl` can adopt a similar pattern as a thin strategy layer on top of the existing helpers without breaking current behavior.

Goal
- Allow swapping the control skin/renderer (classic, Material, custom) using a single property, while keeping helpers as building blocks.

Minimal contract (suggested)

- Define an interface (kept small, focused on BaseControl):
  - `void Paint(Graphics g, BaseControl owner);`
  - `void UpdateLayout(BaseControl owner);`
  - Optional: `void UpdateHitAreas(BaseControl owner, Action<string, Rectangle> register);`

Integration points
- Add to `BaseControl`:
  - `IBaseControlPainter? Painter { get; set; }` and optional `PainterKind` enum + registry.
  - In `DrawContent`: if `Painter != null`, call `Painter.UpdateLayout(this)` then `Painter.Paint(g, this)`; otherwise keep current behavior.
  - Keep all existing helpers untouched; the default (null painter) preserves exact current rendering.

Built-in painters (mapping to current helpers)
- `ClassicPainter` ? thin adapter over `ControlPaintHelper`.
- `MaterialPainter` ? thin adapter over `BaseControlMaterialHelper` plus label/support text drawing already present in `BaseControl`.
- Custom painters can build on top of helpers or draw entirely bespoke visuals.

Benefits
- Unifies with repository-wide painter approach (widgets, cards, progress bars).
- Enables per-control skinning without duplicating logic.
- Backward compatible: if no painter is set, behavior is unchanged.

Caveats / constraints
- Avoid double-drawing: when a painter is assigned, it should fully own the visual (call helpers inside the painter as needed).
- Hit-test: either use `UpdateHitAreas` in the painter or let the owner keep existing icon hit areas for Material.
- Performance: painters should avoid per-frame allocations; re-use brushes/pens carefully and dispose GDI objects.

Suggested next steps
1) Add `IBaseControlPainter` in `BaseControl/Helpers/Painters/` and two adapters: `ClassicPainter`, `MaterialPainter`.
2) Add optional `Painter` property and invoke in `DrawContent` when non-null.
3) Keep `EnableMaterialStyle` functional; if a painter is explicitly set, it takes precedence over legacy flags.
4) Document painter registry and samples; provide a design-time default (null ? current behavior).

This path lets us adopt the painter methodology incrementally with zero breaking changes and aligns `BaseControl` with the broader painting architecture already used across widgets/cards.
