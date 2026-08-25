# BeepMarquee

A rich, themed marquee/ticker control (`BeepMarquee : BaseControl`) supporting
continuous, ping-pong, and news-ticker scroll modes; multiple visual styles
(Card, Pill, StockTicker, NewsBanner, Minimal, Default); fade-edge effects;
pause-on-hover; keyboard control (Space = pause/resume, arrows = nudge,
Home/End = jump); and per-item click/hover/displayed events.

## Structure

| File | Responsibility |
|---|---|
| `BeepMarquee.cs` | Control shell: timer, scroll state, mouse/keyboard, paint orchestration, dispose |
| `MarqueeEnums.cs` | `MarqueeScrollMode`, `MarqueeScrollDirection`, `MarqueeStyle` |
| `Models/MarqueeItem.cs` | Rich data item (text, `ImagePath`, `BadgeText`, per-item colours, `Tag`) |
| `Models/NewsItem.cs` | Category-pill headline item |
| `Models/StockItem.cs` | Symbol · price · Δ% item with gain/loss colouring |
| `Models/MarqueeItemEventArgs.cs` | Click/hover event payload |
| `Painters/IMarqueeItemRenderer.cs` | Renderer contract (`Measure` / `Draw`) |
| `Painters/MarqueePainterBase.cs` | Shared rounded-rect / badge / fade-edge helpers + theme colour resolution |
| `Painters/MarqueePainterFactory.cs` | Style → renderer mapping (stateless shared instances) |
| `Painters/*Painter.cs` | One renderer per `MarqueeStyle` |
| `Helpers/*` | Theme colour, font, and style-recommendation helpers |

Rendering flows through `IMarqueeItemRenderer` (painters), and item icons render
via `StyledImagePainter.PaintWithTint` from `MarqueeItem.ImagePath`.

## Recent Fixes (2026-08)

- **Manual pause no longer overridden by hover** — pause state was a single flag
  shared by "user paused" and "hover paused", so a marquee paused via `Pause()`
  (or Space) would silently resume when the mouse left. Pause and hover-pause are
  now tracked separately (`_hoverPaused`), and `Pause()`/`Resume()` clear the
  hover flag.
- **Dead timer-tick code removed** — the tick handler computed an unused
  `vertical` flag and `dirtyMargin`, then ran two identical `Invalidate` branches;
  collapsed to a single `Invalidate()`.
- **Icon rendering wired into Card and Minimal painters** — `CardMarqueePainter`
  reserved icon space in `Measure` but never drew the icon (leaving a blank gap),
  and `MinimalMarqueePainter` drew a placeholder coloured square with a
  "replace this stub" comment. Both now render the real `ImagePath` through
  `StyledImagePainter.PaintWithTint`.

## Known / flagged (not changed — removal is the owner's call)

- **Legacy component API** — `AddMarqueeComponent` / `RemoveMarqueeComponent` and
  the `_marqueeComponents` dictionary form a second, pre-existing rendering path
  (used only when `Items` is empty). It duplicates the rich-item path and is
  labelled "legacy" in the source; removing it would be a public-API breaking
  change.
- **`ScrollLeft` alias** — a `[Browsable(false)]` compatibility alias over
  `ScrollDirection` with no in-repo call sites.
- **Unused helper surface** — `MarqueeFontHelpers.GetMarqueeFont`, all of
  `MarqueeStyleHelpers`, and `MarqueeThemeHelpers.GetMarqueeBorderColor` /
  `GetMarqueeTextColor` / `GetMarqueeShadowColor` / `GetMarqueeColors` are
  complete but currently unconsumed (style-recommended defaults never wired into
  the control).
