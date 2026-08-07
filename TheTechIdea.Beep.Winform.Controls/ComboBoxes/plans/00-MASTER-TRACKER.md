# ComboBoxes — review tracker (2026-08)

36 files, ~7,900 lines. `BeepComboBox` (6 partials) + `BeepDropDownCheckBoxSelect`, a token-based
painter set (13 painters over `ComboBoxType`), helpers for layout/search/popup-model, popups routed
through `ContextMenuManager`/`BeepContextMenu`, items are `SimpleItem` throughout.

The older docs in this folder (`00-master-plan.md` … `06-animation-polish.md`, `REBUILD-COMPLETED.md`,
dated 2026-05) describe a dropdown rebuild and mark it completed. **Code is authoritative** — this
review verifies those claims by probe rather than trusting them.

## Done

### Three silent catches now report (the folder's only ones)

- `BeepComboBox.Methods.cs` — `previous.Dispose()` on the manager popup handle swallowed. A popup
  handle that will not dispose is a leaked window. Now `BeepLog.Failure`.
- `BeepDropDownCheckBoxSelect.ResolveSafeTextFont` — a disposed `TextFont` fell through silently.
  The fallback is right; the dead font is a lifetime bug upstream and must be seen. `FallbackOnce`
  (paint path).
- `BeepDropDownCheckBoxSelect` chip icon paint — `catch { }` around `StyledImagePainter.Paint`. Now
  `FailureOnce` keyed by path. Note: the painter *returns quietly* for an unresolvable path, so this
  catch only ever sees a corrupt image — the GridX/Badges lesson.

Verified: the multiline pattern finds 0 remaining in `ComboBoxes/` (and still finds them elsewhere,
so the zero is a result).

## Probe results — 15/15 pass (scratchpad ComboProbe)

All planned checks ran green on the first pass. Unusual for this repo, so stated with the evidence
that the checks discriminate: the popup placement matched a computed expectation (Top=123 vs ~123),
the re-entrancy check measured real depth (events=2, depth=2, snap-back won), the empty-list check
measured 0 opens, and the painter-distinctness check would collapse to IDENTICAL on blank renders -
passing means the five sampled types render *and* differ. Renders eyeballed: text, chevron, border
all legible (combo-*.png in %TEMP%).

1. **Selection round-trip**: `SelectedIndex` / `SelectedItem` / `SelectedValue` / `SetValue`
   agreement, events fire once, no re-entrancy when a handler sets selection (the Lovs lesson:
   child-assignment re-enters validation).
2. **Popup claims from the 2026-05 docs**: opens at the control's bottom edge, integral height,
   placement on screen. `ShowDropdown` early-returns on empty lists.
3. **Painter distinctness**: the `ComboBoxType` variants must render differently (aliased-style
   check, with a selection made — the BottomNavBars lesson).
4. **Theme responsiveness**: two different themes must produce different pixels; the 30-literal
   default block in `ComboBoxRenderState` must be a pre-theme fallback, not the live palette.
5. **Multi-select** (`BeepDropDownCheckBoxSelect`): items → select → chips render, `MaxSelection`
   honoured, `RequireAtLeastOne` blocks the last removal.

## Not examined

- The 13 painters' individual geometry (only distinctness is checked).
- Popup keyboard navigation and search filtering behaviour.
- `GetSafeFontHeight` / `MeasureTextSafe` have non-empty fallback catches that do not report;
  same disposed-font family as the fixed one. Left, recorded.

## Standing constraints

Per `CLAUDE.md`: report every catch through `BeepLog`; no stubs or legacy; nothing assigns colours;
a check must be able to fail for the reason it was written.

## Answered: which painter draws the default border, and why it reads faint

Traced, not assumed (a first attempt wrongly patched the ComboBoxType setter and was reverted):

- `BeepComboBox.DrawContent` calls `base.DrawContent` (`Drawing.cs:45`), so the border comes from
  `BaseControl`'s pipeline, never from the 13 combo field painters (`ComboBoxFieldPainterBase.DrawBorder`
  is a reserved no-op).
- Inside `ClassicBaseControlPainter.Paint` the branch is either/or
  (`ClassicBaseControlPainter.cs:211`): **styled** (`BeepStyling.PaintControl` + the
  `Styling/BorderPainters` set) only when `UseFormStylePaint && ControlStyle != None`; otherwise
  **classic** (`DrawBorders`, line 360).
- `BaseControl`'s default is `ControlStyle = BeepControlStyle.None` (`BaseControl.Properties.cs:1389`)
  → the default combo takes the CLASSIC branch. The border on screen is `DrawBorders`: 1px
  (`BorderThickness`) in the theme's `BorderColor`, `PenAlignment.Inset`, radius = `BorderRadius`(4,
  set in `BeepComboBox.Core.cs:249`; `IsRounded` defaults true).
- Why it reads faint/near-square: a 1px low-contrast gray at radius 4. And the `ComboBoxType` token
  radius (pill = 18) shapes only the field painter's interior - it never reaches the base border, so
  `RoundedPill` sits inside a radius-4 outline.

Open decision (not taken unilaterally): whether the type's token radius should flow to the base
border, or whether shaped types should set a non-None `ControlStyle` so the styled branch draws them.
