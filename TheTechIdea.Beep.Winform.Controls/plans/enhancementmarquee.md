# Beep Marquee Enhancement Plan

## Gap Analysis vs. Commercial Standards

| Capability | Current | Target |
|---|---|---|
| Scroll direction | Horizontal only | Horizontal, Vertical, Ping-pong |
| Content model | `IBeepUIComponent` only | Text, Icon+Text, Image+Text, Badge/Pill, Custom |
| Painter pattern | None (single OnPaint) | `IMarqueeItemRenderer` + factory + per-style painters |
| Fade edges | None | Gradient fade left/right or top/bottom |
| Pause on hover | None | `PauseOnHover` property |
| Item click events | None | `ItemClicked`, `ItemHovered` with item identity |
| Easing | Linear only | EaseIn, EaseOut, Linear, Spring |
| Multiple rows | None | Up to 3 scroll rows |
| Bounce / ping-pong | None | `ScrollMode.PingPong` |
| Stock ticker | None | Symbol + price + Δ% with colour coding |
| News ticker | None | Single-item fade-transition ticker |
| Accessibility | None | Live region, keyboard stop/resume |
| RTL | None | `RightToLeft` inversion |
| Design-time items | None | Visual item editor in smart tag |

---

## Sprint 1 — Scroll Modes & Directions ✅
- `MarqueeEnums.cs`: `MarqueeScrollMode` { Continuous, PingPong, NewsTicker }, `MarqueeScrollDirection` { LeftToRight, RightToLeft, TopToBottom, BottomToTop }, `MarqueeStyle` { Default, Card, Pill, StockTicker, NewsBanner, Minimal }
- `BeepMarquee`: `ScrollMode`, `ScrollDirection` properties; `ScrollLeft` kept as alias
- `OnTimerTick()`: branch on direction; PingPong reversal; NewsTicker fade (alpha)

## Sprint 2 — Rich Item Model ✅
- `Models/MarqueeItem.cs`: Id, Text, ImagePath, BadgeText, BadgeColor, TextColor, BackgroundColor, Tag, IsVisible
- `Models/StockItem.cs`: Symbol, Price, Change, ChangePercent
- `Models/NewsItem.cs`: Category, CategoryColor
- `Models/MarqueeItemEventArgs.cs`: Item, ItemIndex, Location
- `Painters/MarqueeRenderContext.cs`: theme, font, DPI, direction, hover index, pause state
- `Painters/IMarqueeItemRenderer.cs`: Measure() + Draw()

## Sprint 3 — Painter Pattern ✅
- `Painters/MarqueePainterBase.cs`: abstract Measure/Draw + virtual DrawFadeEdges
- Painters: DefaultMarqueePainter, CardMarqueePainter, PillMarqueePainter, StockTickerPainter, NewsBannerPainter, MinimalMarqueePainter
- `Painters/MarqueePainterFactory.cs`: routes `MarqueeStyle` → painter

## Sprint 4 — Fade Edges & Pause-on-Hover ✅
- `FadeEdges` bool + `FadeWidth` int on `BeepMarquee`
- `PauseOnHover` bool; `Pause()` / `Resume()` / `IsPaused`

## Sprint 5 — Item Events & Hit-Testing ✅
- `_hitRects` populated per-frame; `OnMouseClick` → `ItemClicked`; `OnMouseMove` → `ItemHovered`

## Sprint 6 — Stock Ticker & News Ticker ✅
- StockItem, NewsItem, StockTickerPainter, NewsBannerPainter
- `AddStockItem()` helper; `ItemDisplayed` event; `NewsTransitionMs`

## Sprint 7 — Easing & RTL ✅
- `ScrollEasing` property reusing `EasingFunction` enum
- `RightToLeft.Yes` inverts scroll direction

## Sprint 8 — Accessibility, Multi-Row & Design-Time Editor
- `Helpers/MarqueeAccessibilityHelpers.cs`: WCAG contrast, accessible name
- `AccessibleRole = Marquee`; `AnnounceItems`; `Alt+M` keyboard shortcut
- `RowCount` (1–3) + independent row scroll offsets
- `BeepMarqueeItemEditor` UITypeEditor for design-time item management
