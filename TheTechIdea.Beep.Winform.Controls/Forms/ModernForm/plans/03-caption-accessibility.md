# Stage 03 — Caption bar: accessibility, keyboard, tooltips, snap layouts

## The gap

The caption bar is painted, and its buttons — close, min/max, theme, style, profile, custom
action, search (`Core.cs:802-1240`) — are hit-area rectangles in
`BeepiFormProHitAreaManager`, not controls. That buys pixel freedom and costs everything a
commercial product is audited for:

- **No UIA presence.** A screen reader sees a form with no close button. Narrator/NVDA users
  cannot operate the chrome at all. DevExpress ships `AccessibleObject`s for every caption glyph.
- **No keyboard path.** Painted buttons cannot take focus; there is no visible focus ring and no
  Enter/Space activation.
- **No tooltips.** Seven glyph-only buttons, none of them named on hover.
- **No Win11 snap layouts** (`WM_NCHITTEST` never returns `HTMAXBUTTON` — F6): hovering maximize
  shows no snap flyout, instantly marking the window as non-native.
- System behaviours to verify and fill where absent: caption double-click maximizes, right-click
  shows the system menu, `Alt+Space` opens it, drag from maximized restores-and-drags.

## The work

1. **Accessible objects over the painted buttons.** Keep the painting; add an
   `AccessibleObject` tree (`ControlAccessibleObject` children, role `PushButton`, names from
   localizable resources, `Bounds` fed from the existing hit areas, `DoDefaultAction` invoking the
   same handlers the mouse uses). This is the standard pattern for owner-drawn chrome and does not
   require composing real controls.
2. **Keyboard.** A focus cycle across visible caption buttons (F6 or Ctrl+Tab section-style),
   painted focus ring from the theme's focus slot, Enter/Space activation, Esc leaves the cycle.
3. **Tooltips** through `ToolTipManager` with the shaped `CustomToolTip` — the captions come from
   the same resource strings UIA uses, so the two can never disagree.
4. **Snap layouts:** return `HTMAXBUTTON` from `WM_NCHITTEST` over the maximize hit area on
   Win11, and handle `WM_NCLBUTTONDOWN`/`UP` there so the flyout's own click-through works.
5. **System-menu parity:** double-click, right-click menu, `Alt+Space`, drag-restore. Each gets a
   check; any already working simply gets its check recorded green.

## Verification

- UIA: walk the form's `AccessibleObject` children; assert one per *visible* caption button with
  a non-empty localized name; toggle `ShowThemeButton` and assert the child count follows.
  `DoDefaultAction` on the close child must raise the same closing path as a mouse click
  (`_isForcedClose` observable).
- Keyboard: drive the focus cycle programmatically; assert the focus ring paints (differential
  render: focused vs not) and Enter fires the button's action.
- Snap: send a synthetic `WM_NCHITTEST` at the maximize hit-point; assert `HTMAXBUTTON` on Win11
  builds and the legacy value elsewhere.
- Tooltip: hover-register a caption button; assert `ToolTipManager` holds a config whose text
  equals the UIA name.
- Break-it-first: hide a button and confirm the UIA-count check fails if the accessible child
  fails to disappear; blank a resource string and confirm the name check goes red.

## Exit criteria

Every visible caption button reachable by screen reader and keyboard with a themed focus ring;
snap layouts on Win11; tooltips named identically to UIA; checks + break-tests recorded. Any new
public surface (e.g. localization hooks) recorded in the tracker.
