# BeepCheckBox Enhancement Summary

## Overview

`BeepCheckBox<T>` is a painter-driven checkbox family. Rendering is delegated to one of eight
stateless painters selected by `CheckBoxStyle`, each of which receives state and options through
`CheckBoxRenderOptions` and `CheckBoxItemState` on every call.

## Architecture

- **Capability matrix** — `CheckBoxPainterCapabilities` declares, per painter, whether it
  supports ThreeState, hover highlight, focus ring, disabled dimming, and custom check icons,
  plus an intentional-divergence note. `CheckBoxPainterBase.GetCapabilities()` supplies the
  standard defaults; `SwitchCheckBoxPainter` and `ButtonCheckBoxPainter` override it.
- **Single geometry path** — `CheckBoxLayoutCalculator.Compute` / `ComputeGrid` produce the
  glyph and text rectangles used by both the draw path and glyph hit-testing, so the two can
  never drift apart.
- **Shared helpers** — `CheckBoxThemeHelpers`, `CheckBoxFontHelpers`, `CheckBoxIconHelpers`,
  and `CheckBoxStyleHelpers` centralize color, typography, icon, and metric resolution.
- **Painter factory caching** — `CheckBoxPainterFactory` caches stateless painters per style.
- **Diagnostics** — `BeepCheckBoxDiagnostics` provides DEBUG-only counters for invalidation,
  toggle, cache rebuild, and painter fetch, with zero cost in Release builds.
- **Grid mode** — `DrawForGrid` renders a simplified glyph path for grid-hosted cells.

## Recent Fixes

- **Luminance math unified.** `CheckBoxPainterBase` previously carried its own additive
  RGB-shift `ShiftLuminance` whose sign-flip inverted pressed/hover feedback on light
  surfaces (pressing a light glyph made it lighter). It now delegates to
  `ColorUtils.ShiftLuminance` (true HSL) while preserving the input alpha, so hover lightens
  and pressed darkens consistently — matching `ModernCheckBoxPainter`, `SwitchCheckBoxPainter`,
  `ButtonCheckBoxPainter`, and the grid path, which already used `ColorUtils`.
- **Switch capability corrected.** `SwitchCheckBoxPainter` renders the indeterminate position
  (mid-toned track, center-left thumb, dash inside the thumb), so its capability matrix now
  advertises `SupportsThreeState = true`; the stale note claiming indeterminate "falls through
  to unchecked" was removed.
- **Button painter cleanup.** Removed a dead `SolidBrush` construction that was immediately
  overwritten with the computed effective background.
- **No more silent value drops.** `SetValue` coercion failures (e.g. a non-numeric string
  bound to a bool checkbox) are now reported to the debugger with context instead of being
  swallowed by empty `catch` blocks. The control still keeps its previous value on failure.
