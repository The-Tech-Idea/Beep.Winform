# BeepTabs — Enhancement Program (Master Tracker)

Target: a commercial-grade tab control measured against DevExpress `XtraTabControl`, Telerik and
Syncfusion desktop tabs, Visual Studio / VS Code document tabs, and Material / Ant Design app tabs.

Written from a full read of `Tabs/` — **9,638 lines across 60 files**. The nine previous planning and
summary documents have been deleted; nothing here is carried over from them. Every claim cites the
code it came from.

## Ground rules for this program

These are the constraints this work is held to, not aspirations:

1. **No stubs.** A method that exists must do something. An empty body is a lie about capability.
2. **No legacy.** No back-compat shims, no "legacy overload" kept alive beside its replacement.
   When something is replaced, the old one is deleted.
3. **No swallowed exceptions.** A bare `catch { return fallback; }` converts a bug into a silent
   wrong answer. Catch what you can genuinely handle, and surface the rest.
4. **No duplication.** One implementation per concept. This repo has already paid for duplicate
   layout engines in `BeepTree` and three duplicate placement engines in `ToolTips`.
5. **Verify by measurement.** A claim about behaviour is backed by a probe run or a render, not by
   reading the code.

## How to read this

One document per feature. Each states current behaviour with evidence, what the reference products
do, the work, and how it will be verified.

| # | Feature | Doc | State | Priority |
|---|---------|-----|-------|----------|
| 1 | Painter contract | [01](01-painter-contract.md) | **done** — dead member removed, measure/draw font fixed, duplicate `PaintTab` path deleted | P0 |
| 2 | Measure / render split & "legacy" naming | [02](02-measure-render-pipeline.md) | **done** — names fixed; snapshot proven to be what is painted (Δ0px) | P0 |
| 3 | Exception policy | [03](03-exception-policy.md) | **done** — catches gone, failures rethrow, `TabError` channel added | P0 |
| 4 | Stubs & empty scaffolding | [04](04-stubs-and-scaffolding.md) | **done** — verified | P0 |
| 5 | Tab model duplication | [05](05-tab-model.md) | **done** — `Bounds` and `Content` removed; one owner per fact | P1 |
| 6 | Modes | [06](06-modes.md) | **one resolution point**; `Documents` vs `Workspace` is an open product decision | P1 |
| 7 | Overflow & header actions | [07](07-overflow-and-actions.md) | **done** — real default, selected/pinned protected; MRU ordering open | P1 |
| 8 | Keyboard, accessibility, RTL & touch | [08](08-input-and-accessibility.md) | **done** — accessible tree built, keyboard verified, RTL + high contrast fixed | P1 |
| 9 | Drag, reorder & dock | [09](09-drag-reorder-dock.md) | **done** — one reorder rule for menu and drag; tear-out undecided | P2 |
| 10 | Theming & painter parity | [10](10-theming-and-painters.md) | **done** — 7 distinct styles, all 4 header positions, adornments, DPI | P2 |
| 11 | Design-time experience | [11](11-design-time.md) | **done** — round trip verified; no defects found | P2 |
| 12 | Verification harness | [12](12-verification-harness.md) | **86 checks green** | P0 |

## The pattern worth remembering

Six times in this program, the defect was **complete, plausible code that nothing called**:

| Dead surface | What it was |
|---|---|
| `BeepTabInputPolicy` | 115 lines whose own header said to use it "instead of scattering guards" — while 20 scattered guards existed |
| `BeepTabAccessibleObjectFactory` | 228 correct lines; the control had no accessible tree at all |
| `BeepTabHeaderHost.Touch.cs` | dead *and* unsound — centred expansion on contiguous tabs makes neighbours overlap |
| `BeepTabRtlLayoutHelper` | complete and correct; `RightToLeft` did nothing measurable |
| `TabColorConfig` | a frozen second copy of `TabThemeHelpers`' colour defaults |
| `TabStyleConfig` | a frozen second copy of `TabStyleHelpers`' radius defaults |

Three of the six were duplicates of a seam that would have fought it if connected. One was a working
implementation of a feature the control did not have. None of them were findable by reading the file
they lived in — each looked correct in isolation, which is exactly why they survived.

Two methods found all six: asking what references a type, and deleting it to see whether the compiler
objects. Neither is clever; both are mechanical, and the harness now runs the first automatically and
reports it as informational, because text matching cannot tell a partial class or a public API from a
corpse.

## Headline findings

**~~`ITabPainter` mandates a dead method.~~ — CORRECTED.** This claim was wrong. `PaintTab` is the
per-style extension point: `BaseTabPainter.PaintTabItem` calls it with **no receiver**, so a search
for `.PaintTab(` finds nothing while all seven painters override it. Deleting it, as originally
planned here, would have broken every painter. The source comment calling it a *"legacy paint
overload"* is what led there — and is now fixed. The genuinely dead member was
`ITabPainter.PaintBackground`, removed and proven by a clean compile.
See [01](01-painter-contract.md).

**Measurement and rendering live in different subsystems, and the live one is called "legacy".**
Sizes come from `BeepTabs.Layout.GetDesiredHeaderTabSizes` → `painter.MeasureTab`; the snapshot is
built by `BeepTabLayoutHelper.CreateSnapshot`; painting is `BeepTabHeaderHost.RenderLegacyHeader` →
`painter.PaintTabItem`. The method named "legacy" is the current render entry point.
See [02](02-measure-render-pipeline.md).

**Four bare `catch` blocks — REMOVED.** Three were in `TabFontHelpers`, the code that measures text:
a failure silently returned a hard-coded 16px, or re-measured with `SystemFonts.DefaultFont` — i.e.
reported a width for a *different* font than the painter draws with, which is the BeepTree
label-clipping defect exactly. Two were pure defensive noise over code that cannot throw once
`ResolveSafeFont` has run; the fourth (`ScaleTouchTarget`) could only ever hide a bug as an unscaled
touch target. The one legitimate catch, `IsFontUsable`, is now a narrow `ArgumentException` for the
disposed-font case with the reason recorded. See [03](03-exception-policy.md).

**Errors are reported only to the debugger.** `ReportError` writes to `Debug.WriteLine` and stores
`_lastError`; six `catch` blocks in `BeepTabs.HostedContent` call it and then continue as if the
operation succeeded — `AddPage`, `ClearPages` and `InsertPageAt` return `void` after a failure. A
release build shows the user nothing and tells the caller nothing. See [03](03-exception-policy.md).

**A no-op method presented as functionality — DELETED.**
`TabFontHelpers.ApplyFontTheme(BeepControlStyle)` had an empty body documented as *"no-op"*. It had
zero callers (the many `ApplyFontTheme` hits elsewhere belong to other controls' own helpers), so it
was removed outright. See [04](04-stubs-and-scaffolding.md).

**Empty scaffolding.** `Tabs/Adapters/` contained no files at all — the previous plan described
adapters as "temporary internal seams", and the folder outlived the idea. Removed as part of
writing this plan.

**A shipped crash: painters disposed shared cached GDI objects — FIXED.** Reported from a real
session as `ArgumentException: Parameter is not valid` out of `Graphics.FillPath` in
`CardTabPainter.PaintTabItem`.

`PaintersFactory.GetSolidBrush`/`GetPen` return a **process-wide cached** instance. `CardTabPainter`
and `ButtonTabPainter` wrapped them in `using`, so the first paint disposed the object the cache was
still handing out. The damage is permanent and global: the factory's self-healing probe
(`_ = brush.Color`) **cannot detect it**, because `SolidBrush.Color` returns a cached field and never
touches the native handle. That colour is then poisoned for the lifetime of the process, and every
later `FillPath` with it throws — in any control, not just tabs.

Fixed at 10 sites in the tab painters and 5 more in `Trees/Painters` (`FigmaCard`, `FileManager`,
`iOS15`, `TailwindCard`). The 23 background painters that matched a blanket search were **not**
offenders: they call `CreateLinearGradientBrush`, which transfers ownership, so their `using` is
correct and removing it would have traded a crash for a GDI handle leak. `PaintersFactory` now
documents the ownership split explicitly, since not stating it is what allowed the misuse.

**The harness had passed this crash as green.** The first contact sheet reported all 21 renders fine
while the paint was throwing: `DrawToBitmap` routes through `Control.PaintWithErrorHandling`, which
never lets the exception reach the caller, so a half-painted tab still produced a bitmap with plenty
of colour in it and satisfied both the "not blank" and "renders distinctly" checks. Pixel assertions
cannot see a paint that aborted. The sheet now installs a `FirstChanceException` hook filtered to the
tab painters and asserts no paint threw, **before** trusting any pixel comparison — and the first
finding it produced under the old order ("Underline and Minimal render identically under
MaterialDesignTheme") turned out to be an artifact of the crash, not a real duplication.

**Every tab was measured in one font and painted in another — FIXED.** The defect this program
predicted from `BeepTree` was present in `BaseTabPainter`, the base class all seven painters inherit.
`MeasureTab` sized each tab with `TabFontHelpers.GetTabFont(Theme, item.IsSelected)` — the theme font,
bold when selected — while `DrawTextInBounds` painted the title with a hardcoded
`SystemFonts.DefaultFont`. Theme fonts therefore never reached a drawn tab title, and a selected tab
was measured bold but drawn regular. The correctly-resolved font was already sitting in a local
variable one line above the call and simply was not passed. The subtext directly below it did it
right, which is what made the bug survive review. See [01](01-painter-contract.md).

**High contrast was implemented, documented, and never called — FIXED.**
`BeepTabHeaderHost.HighContrast.cs` held a complete 156-line high-contrast paint pass whose XML doc
said *"Called from OnPaint when IsHighContrast is true"*. Nothing called it: `RenderHeader` went
straight to the theme painters, so Windows High Contrast mode did nothing at all except for focus
rings. It was also a **second implementation of tab geometry** — its own text bounds, close glyph and
dirty marker — and a lossy one, drawing no icons, badges or subtext. Rather than wire up a reduced
parallel renderer, high contrast became what it actually is, a colour concern: `TabThemeHelpers` now
resolves system colours when `IsHighContrast`, so the single painter pipeline is correct in both
modes. The duplicate file is deleted. See [08](08-input-and-accessibility.md).

**Two functions answered "what colour is the close glyph".** `TabIconHelpers.GetCloseIconColor` (live)
and `TabThemeHelpers.GetCloseButtonColor` (zero callers) were near-identical and already disagreed on
the hover fallback. The orphan was deleted rather than left to drift further.

**The contact sheet found four more defects — see [10](10-theming-and-painters.md).** A permanently
stuck style transition that made every tab paint twice by two painters forever; Underline and Minimal
being the same style; the selected tab's label rendered white-on-white in both of them; and every
close button drawn as a solid dark square because the default "close icon" is a red badge graphic,
not a glyph. All four are fixed and all seven painters now render distinctly under three themes.

**The seven painters barely differ.** Each overrides exactly two members of `BaseTabPainter`. That
may be correct — or it may be the same "styles that render identically" problem found in `BeepTree`,
where four painter groups produced identical output. It has never been rendered side by side.
See [10](10-theming-and-painters.md).

## Prior art in this repo

Three defect classes have recurred across `BeepGridPro`, `BeepTree` and `ToolTips`, and all three
are plausible here. The harness in [12](12-verification-harness.md) tests for them explicitly:

- **Measure with one font, draw with another** — clipped every label in `BeepTree`'s 25 painters and
  clipped the Glass tooltip twice.
- **Two implementations of one geometry** — `BeepTree` had two layout engines 4px apart;
  `ToolTips` had three placement engines.
- **A declared property nothing reads** — `ToolTips` had six.
