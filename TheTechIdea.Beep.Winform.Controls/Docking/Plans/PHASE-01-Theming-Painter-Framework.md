# PHASE 01 — Theming & Painter Framework

**Goal:** Make every pixel of `BeepDockingManager` chrome flow through the Beep painter framework
(`BeepStyling`, `BackgroundPainterFactory`/`BorderPainterFactory`/`ShadowPainterFactory`,
`StyledImagePainter`, `SvgsUIcons`, `BeepFontManager`) and a single set of **distinct**
element renderers driven by **layout managers**. Remove all raw GDI and hard-coded fonts.

**Depends on:** 00 · **Blocks:** 02, 04, 05

---

## 1.1 Existing-code disposition (this phase)

| File | Disposition | What changes |
|------|-------------|--------------|
| `Docking/DockingThemeColors.cs` | **Reuse** | Becomes the palette source for `DockingPainterContext`. No change except possible new keys (document tab, peek). |
| `Helpers/DockingCaptionPainter.cs` | **Reuse** | Already paints SVG icons via `StyledImagePainter`+`SvgsUIcons`. Becomes the icon helper called by `CaptionRenderer`. |
| `Painters/DockingPainterAdapter.cs` | **Replace** | Delete the `SolidBrush`/`Pen`/`DrawString` body; replace with renderers below. |
| `Painters/IDockingPainter.cs` | **Refactor** | Split into element renderers + add `DockingPainterContext`. |
| `Painters/DockingPainterFactory.cs` | **Refactor** | Map `BeepControlStyle`/theme → renderer set (not just theme-name → Null). |
| `Painters/NullDockingPainter.cs` | **Reuse** | Design-time/no-op fallback. |
| `Models/DockPanel.cs` (`DrawCaption`, button rects, `new Font`) | **Refactor** | Move caption paint + hit-test into `CaptionRenderer` + `CaptionLayoutManager`. |
| `BeepDockspace.cs` (`DrawHeader`, `new Font("Segoe UI")`) | **Refactor** | Use the **same** caption/tab renderer + layout manager as `DockPanel`. |
| `Runtime/BeepDockSplitter.cs` | **Refactor** | Paint via `SplitterRenderer`; keep its drag logic. |
| `Runtime/AutoHideStrip.cs` | **Refactor** | Paint via `StripRenderer`. |
| `Runtime/DockingGuideOverlay.cs` | **Refactor (Phase 02)** | Painting moves to `GuideRenderer`; arrows from `SvgsUIcons`. |

> Two caption painters (`DockPanel.DrawCaption` + `BeepDockspace.DrawHeader`) collapse into
> **one** renderer. That dedupe is the single biggest win of this phase.

---

## 1.2 New components

```
Docking/Painters/
├── DockingPainterContext.cs     // theme, BeepControlStyle, DockingThemeColors, state flags, DPI, bounds
├── IDockingElementRenderer.cs   // common contract (Paint(ctx, g, bounds))
├── Caption/CaptionRenderer.cs   // panel/dockspace caption: bg(factory)+border(factory)+title(BeepFontManager)+icon(StyledImagePainter)+buttons
├── Tab/TabStripRenderer.cs      // tab stack for dockspace/document well
├── Splitter/SplitterRenderer.cs // splitter grip + hover/drag states
├── Guide/GuideRenderer.cs       // 5-diamond + edge guides (used Phase 02)
├── Strip/AutoHideStripRenderer.cs // collapsed auto-hide tabs
├── DockingPainterFactory.cs     // style/theme → renderer set (+ cache)
└── NullDockingPainter.cs        // reused

Docking/Layoutmanagers/
├── CaptionLayoutManager.cs      // computes title/icon/button rects; resolves hover/press/active; raises actions
├── TabStripLayoutManager.cs     // tab rects, overflow, hot/selected/close-button hit-test
├── SplitterLayoutManager.cs     // splitter hit-rects + hover (defers drag math to layout engine, Phase 03)
└── AutoHideLayoutManager.cs     // strip tab rects + hover (peek trigger, Phase 04)
```

**Rule (house style):** layout managers **detect** (hit-test, hover, press, active);
renderers **only paint** the resolved state. No mouse logic inside renderers.

---

## 1.3 Painter context

`DockingPainterContext` carries everything a renderer needs so renderers stay stateless:
- `IBeepTheme Theme`, `DockingThemeColors Colors`
- `BeepControlStyle Style`, `bool UseThemeColors`
- state flags: `IsActive`, `IsHover`, `IsPressed`, `IsFocused`, `IsDragging`
- `Rectangle Bounds`, `float DpiScale`, button-visibility flags (`CanClose/Float/AutoHide/Pin`).

Background/border/shadow obtained from the factories using `Style` so docking chrome matches
form/control styling exactly.

---

## 1.4 Implementation steps

1. **Context + contract.** Add `DockingPainterContext` and `IDockingElementRenderer`.
2. **CaptionRenderer + CaptionLayoutManager.** Port logic from `DockPanel.DrawCaption` and
   `BeepDockspace.DrawHeader` into one place. Background/border via factories; title via
   `BeepFontManager`; icon + caption buttons (close/float/autohide/pin) via
   `StyledImagePainter` + `SvgsUIcons`. Hover/press/active resolved by the layout manager.
3. **TabStripRenderer + TabStripLayoutManager.** Tab backgrounds, selected/hot states,
   per-tab close glyph, overflow chevron. Used by `BeepDockspace` now and the document well
   in Phase 05.
4. **SplitterRenderer.** Replace `BeepDockSplitter.OnPaint` GDI with renderer; keep drag.
5. **AutoHideStripRenderer.** Replace strip GDI.
6. **Factory.** `DockingPainterFactory.GetRenderers(style, theme)` returns the renderer set;
   cache by `(style, themeName)`; default to `NullDockingPainter` at design time if needed.
7. **Wire `BeepDockingManager`.** `DockPanel`/`BeepDockspace`/`BeepDockSplitter`/`AutoHideStrip`
   call the factory; remove their own GDI.
8. **Fonts.** Remove every `new Font("Segoe UI")`; use `BeepFontManager`.
9. **`ApplyTheme()`.** Override on `BeepDockingManager` and each surface to push palette + invalidate.
10. **`Style` property.** Add `BeepControlStyle Style` to `BeepDockingManager`; propagate to context.

---

## 1.5 TODO checklist

- [x] `DockingPainterContext`, `IDockingElementRenderer`.
- [x] `CaptionRenderer` + `CaptionLayoutManager` (replaces both caption painters).
- [ ] `TabStripRenderer` + `TabStripLayoutManager`.
- [x] `SplitterRenderer`; refactor `BeepDockSplitter` paint.
- [x] `AutoHideStripRenderer` + `AutoHideStripLayoutManager`; `AutoHideStrip` paint/hit-test now delegate to them.
- [x] Rewrite `DockingPainterFactory` (now vends per-style renderer set); `DockingPainterAdapter` GDI body removed (now theme/metrics provider only).
- [x] Remove all `new Font(...)`; route via `BeepFontManager`.
- [x] `ApplyTheme()` on `BeepDockingManager` + surfaces; add `Style` property + propagation.
- [x] Keep `DockingThemeColors` + `DockingCaptionPainter` (extend keys only as needed).

---

## 1.6 Verification criteria

- No `SolidBrush`/`Pen`/`DrawString`/`new Font` remain in docking chrome (search clean).
- Switching theme or `Style` restyles captions, tabs, splitters, strips consistently.
- Caption is painted by exactly **one** renderer for both `DockPanel` and `BeepDockspace`.
- Hover/press/active visuals come from layout-manager state, not renderer-internal mouse code.
- High-DPI: icons/text scale via `StyledImagePainter`/`BeepFontManager`.
