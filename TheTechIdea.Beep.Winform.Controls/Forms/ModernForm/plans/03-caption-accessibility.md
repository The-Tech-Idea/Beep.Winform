# Stage 03 — Caption: real UIA children, tooltips, snap layouts

> **Corrected after reading the code.** The first draft said "no UIA, no keyboard, no focus ring".
> That was wrong: keyboard operation and accessible naming both exist. The defect is narrower and
> more specific — and, being narrower, it is also cheaper to fix properly.

## What already works — keep it

- `F6` / `Shift+F6` cycles caption regions; `Ctrl+F` focuses the search box; `Alt+Space` opens the
  system menu; Enter activates the focused region (`Win32.cs:739-800`).
- A keyboard focus indicator is painted (`PaintKeyboardCaptionFocusIndicator`), invalidated
  precisely on focus movement (`InvalidateKeyboardCaptionFocus`, `Core.cs:502`).
- Per-region accessible **names, roles and descriptions** are authored
  (`GetCaptionAccessibilityName/Role/Description`, `Core.cs:620-680`) — including live variation
  ("Caption restore button" vs "Caption maximize button" by `WindowState`).
- Custom caption regions carry their own `AccessibleName`/`AccessibleRole`.

## The actual defect

**There are zero `AccessibleObject`s in the folder.** The accessible vocabulary above is delivered
by *mutating the form's own identity*: on focus change the form's `AccessibleName`, `Role` and
`Description` are overwritten, with the originals stashed in `_baseAccessibleName` etc.
(`CaptureCaptionAccessibilitySnapshot`, `Core.cs:606`).

Consequences a commercial accessibility audit fails on:

1. **UIA sees one element whose identity changes.** There is no child tree, so a screen reader
   cannot enumerate the caption buttons, list them, or navigate to one directly — it only hears
   the form rename itself as focus moves.
2. **Automation cannot invoke them.** No `InvokePattern` per button; UI test frameworks
   (WinAppDriver, FlaUI, Coded UI) have nothing to click. This alone blocks enterprise adoption.
3. **The form's real identity is clobbered** while a caption region has focus — anything reading
   the form's `AccessibleName` mid-focus gets a button's name.
4. **No tooltips** on seven glyph-only buttons.
5. **No snap layouts** (F6): `WM_NCHITTEST` never returns `HTMAXBUTTON`, so hovering maximize
   shows no Win11 snap flyout.

## The work

1. **`CreateAccessibilityInstance` → a `ControlAccessibleObject` with children.** One child per
   *visible* caption region, `Role`/`Name`/`Description` fed by the existing
   `GetCaptionAccessibility*` methods (they become the child's data source — no new vocabulary),
   `Bounds` from the hit-area manager (screen coords), `DoDefaultAction` calling the same
   `OnRegionClicked` path the mouse and Enter use. `GetChildCount`/`GetChild` follow visibility.
2. **Stop mutating the form's identity.** Delete the snapshot/restore machinery once children
   exist; the form keeps its own name, and focus is expressed through the child's `State`
   (`Focused`/`Focusable`) instead.
3. **Tooltips** via `ToolTipManager` + the shaped `CustomToolTip`, text drawn from the *same*
   `GetCaptionAccessibilityName` source, so tooltip and screen reader can never disagree.
4. **Snap layouts**: return `HTMAXBUTTON` over the maximize hit area on Win11 (build ≥ 22000) and
   handle `WM_NCLBUTTONDOWN`/`WM_NCLBUTTONUP` there so the flyout's click-through works.
5. **System-menu parity check**: caption double-click maximizes, right-click shows the system
   menu, drag-from-maximized restores. Where already correct, record the check green.

## Outcome — ☑ done (items 1–4); item 5 not verified

### Measured before the fix

```
FAIL  the caption exposes one accessible child per visible region: GetChildCount()=-1, regions=3
FAIL  every accessible child has a non-empty name: no children to name
FAIL  the close child actually closes the form when invoked: no close child exists to invoke
FAIL  the form's own accessible name survives caption focus:
        became: Caption close button -> Caption maximize button -> Caption minimize button
FAIL  hovering maximize reports HTMAXBUTTON: WM_NCHITTEST returned 1 (HTMAXBUTTON=9)
```

`GetChildCount()` returning **-1** is the sharpest number here: not "an empty tree" but *no tree at
all*. And a form explicitly named "My Application Window" reported itself as "Caption close button"
the moment F6 reached the close button — this document's claim, reproduced verbatim.

### What changed

1. **`BeepiFormPro.Accessibility.cs`** (new). `CreateAccessibilityInstance` returns a
   `ControlAccessibleObject` with one `CaptionRegionAccessibleObject` child per *visible* caption
   region. `Name`/`Description`/`DefaultAction`/`Role` **read** the existing
   `GetCaptionAccessibility*` methods rather than copying them onto the form; `Bounds` are screen
   coordinates (a client rect would misplace a screen reader's highlight on every monitor but the
   primary); `DoDefaultAction` calls the same `OnRegionClicked` the mouse and Enter use; `State`
   carries `Focused`. `HitTest` and `GetFocused` are overridden so a reader landing on the caption
   by cursor or by focus finds the child, not the form.
2. **The identity mutation is deleted** — `CaptureCaptionAccessibilitySnapshot`, the
   `UpdateCaptionAccessibilityFocus` body, `RestoreCaptionAccessibilityFocus`'s body and five
   `_baseAccessible*` fields. What remains is the notification
   (`AccessibilityNotifyClients(Focus, index + 1)` — MSAA child IDs are 1-based).
3. **Tooltips** (`BeepiFormPro.CaptionToolTips.cs`, new; `ShowCaptionToolTips`, default true) on the
   glyph-only buttons, text from `GetCaptionAccessibilityName`. One source is the point: a tooltip
   and a screen reader that disagree is worse than either alone, and sharing the method makes
   disagreement impossible rather than merely unlikely. It also means the tooltip already says
   "restore" when the window is maximized, for free.
4. **Snap layouts** — `WM_NCHITTEST` returns `HTMAXBUTTON` over the maximize button on Win11.

### Two bugs found in passing, both real

- **`WM_NCMOVE` is not a Windows message.** The constant declared `WM_NCMOVE = 0x00A0`, which is
  **`WM_NCMOUSEMOVE`**, and `case WM_NCMOVE:` was therefore quietly swallowing every non-client
  mouse move. Latent while the case only `break`s — but it made `WM_NCMOUSEMOVE` unusable, which is
  exactly the message the maximize button's hover state needed. The fiction was deleted, not
  renumbered.
- **Reporting `HTMAXBUTTON` moves the button out of the client-mouse world.** Its click then arrives
  as `WM_NCLBUTTONUP` and its hover as `WM_NCMOUSEMOVE`, neither of which the form handled. Left
  alone, this stage would have traded a snap flyout for a maximize button that no longer maximizes
  and no longer highlights. Both are now forwarded to the same interaction path the client-area
  buttons use, and `TrackMouseEvent(TME_LEAVE | TME_NONCLIENT)` asks for the leave message — without
  it the hover highlight sticks after the cursor leaves, because NC mouse messages simply stop
  arriving and there is nothing to infer "moved away" from.

### Break-it-first — seven breaks, all run

| break | result |
|---|---|
| A. `GetChildCount()` returns -1 again | **red** — 4 checks |
| B. child list hardcoded to three | **red** — the visibility differential only |
| C. `DoDefaultAction` inert | **red** — invoke check |
| D. focus renames the form again | **red** — identity check |
| E. never report `HTMAXBUTTON` | **red** — snap check |
| F. tooltip text given its own source | **red** — tooltip/UIA agreement |
| G. `WM_NCLBUTTONUP` no longer maximizes | **red** — the click-still-works check |
| all restored | green (12 checks) |

**B is the one worth keeping.** Three children and three visible regions could both be hardcoded and
the count check would pass on a coincidence. Only the differential — toggle `ShowThemeButton`, the
count must move by exactly one — separates a tree that follows visibility from one that does not,
and B is red on *that check alone*.

### Verified

- 12 checks green; seven breaks each seen red; all green restored.
- Renders eyeballed: `03-caption-focus-close.png` / `03-caption-focus-minimize.png` — the focus ring
  is drawn around the close button in the first and the minimize button in the second, so it both
  renders and tracks.
- Full library builds clean.

### Public / protected surface

| member | change |
|---|---|
| `protected override AccessibleObject CreateAccessibilityInstance()` | **added** |
| `public bool ShowCaptionToolTips` | **added** (default `true`) |
| `internal BeepiFormProAccessibleObject`, `internal CaptionRegionAccessibleObject` | **added** |
| `internal string KeyboardFocusedCaptionArea` | **added** |
| `private const int WM_NCMOVE` | **removed** — not a Windows message; see above |
| `CaptureCaptionAccessibilitySnapshot`, 5 × `_baseAccessible*` fields | **removed** (private) |

`RestoreCaptionAccessibilityFocus` is kept as an empty method, deliberately: it is the one place
"focus left the caption" is expressed and its callers read naturally. It is documented as such
rather than left looking like an oversight.

### NOT verified — item 5, system-menu parity

Caption double-click to maximize, right-click for the system menu, and drag-from-maximized to
restore are **not checked**. They were not touched by this stage, but "not touched" is not "still
works" — and this stage moved the maximize button into the non-client world, which is adjacent
enough to warrant a look. A probe cannot drive a real double-click or a drag; this needs a human.

Recorded as open rather than assumed green.

## Verification

- **UIA tree**: `AccessibilityObject.GetChildCount()` equals the number of *visible* caption
  regions; toggling `ShowThemeButton` changes it by exactly one; each child has a non-empty name
  matching `GetCaptionAccessibilityName`.
- **Invoke**: `GetChild(closeIndex).DoDefaultAction()` reaches the same closing path as a click
  (observe `_isForcedClose` / the `FormClosing` sequence).
- **Identity**: move keyboard focus across every region and assert the *form's* `AccessibleName`
  never changes (this is the regression the current design would fail).
- **Focus ring**: differential render, focused vs not (blindness guard: two unfocused renders
  identical first).
- **Snap**: synthetic `WM_NCHITTEST` at the maximize hit-point returns `HTMAXBUTTON` on Win11,
  legacy value elsewhere.
- **Tooltip == UIA name** for every caption region.
- **Break-it-first**: hide a button and confirm the child-count check fails if the child does not
  disappear; blank one name resource and confirm the name check goes red; restore the identity
  mutation and confirm the identity check goes red.

## Exit criteria

A real UIA child per visible caption button, invokable by automation; the form's own identity
stable; tooltips sourced from the accessibility names; snap layouts on Win11; all checks with
their break-tests recorded. Public/protected surface changes (e.g. `CreateAccessibilityInstance`
override, any exposed region descriptor) recorded in the tracker.
