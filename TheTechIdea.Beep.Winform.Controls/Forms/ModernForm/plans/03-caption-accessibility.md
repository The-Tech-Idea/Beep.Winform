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
