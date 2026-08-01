# BeepTabs

Custom tab control with owned header rendering, page hosting, document/workspace modes and seven
painter styles. Replaces `System.Windows.Forms.TabControl`.

Written from the code as it stands (9,638 lines / 60 files). For planned work see
[`plans/`](plans/README.md).

## Layout

```
Tabs/
├── BeepTabs.cs                     the control; ContainerControl facade
├── BeepTabs.Actions.cs             commands + TabError event/ReportError/_lastError
├── BeepTabs.Animation.cs           style-transition animation
├── BeepTabs.Appearance.cs          style, overflow policy, header options
├── BeepTabs.ClosedTabHistory.cs    reopen-last-closed (Ctrl+Shift+T)
├── BeepTabs.ContextMenu.cs         header context menu
├── BeepTabs.Drawing.cs             paint entry; owns the BeepTabHeaderHost
├── BeepTabs.HostedContent.cs       page model <-> host projection (largest file, 1048 lines)
├── BeepTabs.Initialization.cs      wiring
├── BeepTabs.Interaction.cs         pointer/selection behaviour
├── BeepTabs.Keyboard.cs            key handling
├── BeepTabs.Layout.cs              header measurement + rects
├── BeepTabs.Metadata.cs            per-tab metadata
├── BeepTabs.WorkspaceCommands.cs   workspace verbs
├── BeepTabs.WorkspaceMru.cs        MRU ordering
├── BeepTabPage.cs                  page control; owns content + Tab* document state
├── BeepTabQuickSwitch.cs           Ctrl+Tab style switcher with filter
├── TabStyles.cs                    TabStyle, TabLabelVisibility enums
├── Hosts/                          BeepTabHeaderHost (12 partials) + BeepTabContentHost
├── Helpers/                        layout, overflow, hit-test, MRU, a11y, RTL, fonts, icons, theme
├── Models/                         BeepTabItem, layout snapshots, state, configs
└── Painters/                       ITabPainter + Base + 7 styles
```

## How a header is drawn

1. `BeepTabs.Layout.GetDesiredHeaderTabSizes(graphics)` measures via `painter.MeasureTab`.
2. `BeepTabs.Layout.GetCurrentHeaderTabRects(graphics)` produces the rects.
3. `BeepTabLayoutHelper.CreateSnapshot(...)` builds a `BeepTabHeaderLayoutSnapshot`.
4. `BeepTabHeaderHost.SyncSnapshot()` takes it.
5. `BeepTabs.Drawing` calls `_headerHost.RenderHeader(graphics, CreateHeaderRenderRequest())`,
   which calls `painter.PaintTabItem(...)` per item.

Measurement lives on `BeepTabs`; rendering lives on the host. See
[plans/02](plans/02-measure-render-pipeline.md) — that seam is the main architectural question.

## Three representations of a tab

| Type | Kind | Owns |
|---|---|---|
| `BeepTabPage` | a `Control` | content, and document state (`TabIsPinned`, `TabIsDirty`, `TabIsPreview`, `TabCanClose`, …) |
| `BeepTabItem` | plain data | render snapshot + interaction state (`IsSelected`, `IsHovered`, `IsPressed`, …) |
| `BeepTabHeaderItemLayout` | geometry | resolved rects the painter draws into |

`BeepTabs.HostedContent` projects between them.

## Known behaviour worth knowing before you edit

Detailed in [`plans/`](plans/README.md).

### Still open

- **Tear-out (dragging a tab into its own window) is undecided.**
  — [plans/09](plans/09-drag-reorder-dock.md)
- **Tear-out (dragging a tab into its own window) is undecided.**
  — [plans/09](plans/09-drag-reorder-dock.md)
- **`Documents` and `Workspace` modes are behaviourally identical** — the enum declares three modes
  and the control implements two. — [plans/06](plans/06-modes.md)
- **The overflow menu is not MRU-ordered**, and the visible run can have positional gaps
  (`0, 10, 11`) rather than being a contiguous window around the selection.
  — [plans/07](plans/07-overflow-and-actions.md)
- **Overflow, modes, tab-model ownership, drag/reorder and design-time round trip** are still
  unverified. — [plans/](plans/README.md)
- **`BeepTabItem.Index` sits on a type described as a snapshot** — load-bearing across overflow,
  layout and hit-test. — [plans/05](plans/05-tab-model.md)
- **High contrast is sourced correctly but never rendered** — `SystemInformation.HighContrast` is an
  OS setting the harness cannot switch, so the colour mapping is verified by construction only.
  — [plans/08](plans/08-input-and-accessibility.md)
- **Measurement lives on `BeepTabs.Layout`, rendering on `BeepTabHeaderHost`**, joined by a snapshot
  that helpers bypass by calling back into the owner.
  — [plans/02](plans/02-measure-render-pipeline.md)

### Fixed — recorded because the reasoning matters

- **`ITabPainter.PaintTab` is *not* dead.** An earlier version of these notes said it was. It is the
  per-style extension point, called with no receiver from `BaseTabPainter.PaintTabItem`, so a search
  for `.PaintTab(` finds nothing while all seven painters override it. Deleting it, as the plan
  originally instructed, would have broken every painter. The genuinely dead member was
  `PaintBackground`. — [plans/01](plans/01-painter-contract.md)
- **Tabs were measured in one font and drawn in another.** `MeasureTab` used the theme font (bold
  when selected); `DrawTextInBounds` hardcoded `SystemFonts.DefaultFont`. Theme fonts never reached a
  drawn title. — [plans/01](plans/01-painter-contract.md)
- **High contrast never ran.** A complete 156-line paint pass existed, documented as called from
  `OnPaint`, that nothing called. It is now a colour concern resolved in `TabThemeHelpers`, and the
  duplicate file is deleted. — [plans/08](plans/08-input-and-accessibility.md)
- **`RenderHeader` is the live render entry point**, formerly named `RenderLegacyHeader` despite
  being the only render path there is. — [plans/02](plans/02-measure-render-pipeline.md)
- **Four bare `catch` blocks**, three in the text-measurement helper, and operations that reported a
  failure then returned as if it had succeeded. Failures now rethrow and a public `TabError` event
  carries them in every build configuration. — [plans/03](plans/03-exception-policy.md)
- **`TabFontHelpers.ApplyFontTheme` was an empty method** with zero callers; deleted.
  — [plans/04](plans/04-stubs-and-scaffolding.md)
- **RTL did nothing.** `BeepTabRtlLayoutHelper` was complete but referenced only by itself; now
  called from `SyncSnapshot` and proven by measurement. — [plans/08](plans/08-input-and-accessibility.md)
- **Painters disposed shared cached GDI objects**, poisoning a colour process-wide and crashing
  `FillPath`. — [plans/10](plans/10-theming-and-painters.md)
- **The style transition never ended**, so every tab painted twice by two painters forever.
  — [plans/10](plans/10-theming-and-painters.md)
- **Underline and Minimal were one style**; the selected label was white-on-white in both; every
  close button was a solid dark square. — [plans/10](plans/10-theming-and-painters.md)
- **Overflow dropped the selected and pinned tabs**, and three of five overflow policies were
  declared but never implemented. — [plans/07](plans/07-overflow-and-actions.md)

## Conventions

- Measure and draw with the same font and metrics. Diverging on this clipped every label in
  `BeepTree` and twice in `ToolTips`.
- One implementation per geometry. Two have never coexisted in this repo without disagreeing.
- No bare `catch`. No empty method bodies. No back-compat shims — delete the old thing.
- `InitializeComponent` must contain no loops or conditionals; it breaks the designer parser.
