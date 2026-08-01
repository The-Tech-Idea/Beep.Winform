# Beep ToolTips

Custom tooltip, popover, preview and guided-tour framework. Replaces
`System.Windows.Forms.ToolTip` with a themed, painter-based, owner-drawn system.

Written from the code as it stands. For planned work see [`plans/`](plans/README.md).

## Layout

```
ToolTips/
├── ToolTipManager.cs          singleton orchestrator: show/hide/update, control attachment
├── ToolTipInstance.cs         one live tooltip — owns a CustomToolTip window
├── ToolTipConfig.cs           everything configurable about a tooltip (+ ShortcutKeyItem, ToolTipContentItem)
├── ToolTipEnums.cs            ToolTipType, Placement, Animation, ArrowStyle, TriggerMode, Easing, Section, LayoutVariant
├── ToolTipExtensions.cs       Control extension methods + ToolTipBuilder fluent API
├── CustomToolTip.*.cs         the tooltip window: Core / Drawing / Positioning / Animation / Accessibility / Methods
├── BeepPopover.cs             click-triggered popover built on the same config
├── BeepPinnedTooltip.cs       standalone draggable pinned tooltip (not wired to ToolTipConfig.Pinnable)
├── BeepTour*.cs               guided tour: Manager, Builder, Step
├── OutsideClickMessageFilter  process-wide IMessageFilter for click-outside dismissal
├── IToolTipHost.cs            host abstraction
├── Models/ToolTipStyleConfig  per-style visual constants
├── Helpers/                   positioning, layout, markup, animation, arrow, shortcut badges, a11y
└── Painters/                  IToolTipPainter + Base, BeepStyled, Glass, Preview, Tour, and the factory
```

## How a tooltip is shown

1. A caller uses `control.SetTooltip(...)`, `ToolTipManager.Instance.ShowTooltipAsync(config)`, or
   the `ToolTipBuilder`.
2. `ToolTipManager` assigns `config.Key` (generating a GUID if absent), cancels any live tooltip with
   the same key, and creates a `ToolTipInstance`.
3. `ToolTipInstance` constructs a `CustomToolTip` window and shows it.
4. `CustomToolTip` measures content, asks `ToolTipPositioningHelpers` for a placement, applies its
   own offset maths, constrains to the screen, and paints via the painter chosen by
   `ToolTipPainterFactory` from `config.LayoutVariant`.
5. Hide happens on mouse-leave, the `Duration` timer, Escape, outside click, or an explicit call.

For control-attached tooltips, `SetTooltip` stores named `MouseEnter`/`MouseLeave`/`MouseMove`
handlers in `_attachedHandlers` so `RemoveTooltip` can detach them cleanly. Do not replace these with
anonymous lambdas — that leaked a handler on every reassignment and the named-delegate registry is
the fix.

## Two axes of appearance

Deliberately orthogonal, and worth preserving:

- **`ToolTipType`** — semantic intent (Success, Warning, Error, Info, Help, Tutorial, Preview…).
- **`BeepControlStyle`** — visual design language (Material3, Fluent2, iOS15…).

`ToolTipLayoutVariant` is a third axis (Simple/Rich/Card/Preview/Tour/Shortcut/Glass) selecting the
painter — though today only Preview, Tour and Glass map to their own painter; see
[plans/07](plans/07-content-pipeline.md).

## Known behaviour worth knowing before you edit

These are documented in detail under [`plans/`](plans/README.md); the short version:

- The anchor passed to positioning is a **1×1 rectangle**, so `*Start` / `*End` alignments do not
  meaningfully differ. — [plans/01](plans/01-anchor-and-placement.md)
- Placement is chosen by `ToolTipPositioningHelpers` and applied by
  `CustomToolTip.AdjustPositionForPlacement`, which use **different offset maths**. —
  [plans/01](plans/01-anchor-and-placement.md)
- The arrow does not track the anchor after the tooltip is shifted to fit the screen. —
  [plans/02](plans/02-arrow-tracking.md)
- Nothing repositions a visible tooltip when its anchor moves, scrolls or changes monitor. —
  [plans/03](plans/03-auto-update.md)
- `PersistOnHover`, `Pinnable` and `LoadPreviewAsync` are declared and documented but **never read
  by any code**. — [plans/04](plans/04-interactive-hover.md), [10](plans/10-pinning.md),
  [07](plans/07-content-pipeline.md)
- Each show creates a new top-level window. — [plans/12](plans/12-lifecycle-and-performance.md)

## Conventions

- Painters are stateless and pooled by `ToolTipPainterFactory`; do not store per-tooltip state in one.
- Measure and draw text with the same font and the same `TextFormatFlags`. Diverging on this is what
  clipped every label in `BeepTree`.
- Theme colours are resolved through `BeepThemesManager`; never hard-code a colour in a painter.
- `ToolTipManager` subscribes to `BeepThemesManager.ThemeChanged` — live tooltips must repaint on a
  theme switch, so do not cache resolved colours across paints without invalidating them there.
