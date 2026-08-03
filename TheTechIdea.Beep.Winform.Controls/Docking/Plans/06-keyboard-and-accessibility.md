# 06 — Keyboard docking and accessibility

## What is missing

`grep -rn "AccessibleRole\|CreateAccessibilityInstance"` across `Docking/` returns **0**.

A docking system with no accessible surface is unusable with a screen reader, and a docking system
that can only be rearranged by dragging is unusable without a pointer. Both are true here today.

This is not a small feature bolted on at the end — it is why the tracker recommends threading it
through rather than scheduling it last. Accessibility retrofitted after seven features is
accessibility done twice.

## Reference behaviour

| product | keyboard docking |
|---|---|
| VS Code | `Ctrl+K Ctrl+←/→` move group focus; `Ctrl+K ←/→` split; `Ctrl+W` close; entire layout keyboard-drivable |
| Visual Studio | `Alt+F7` / `Alt+Shift+F7` cycle tool windows; `Ctrl+Alt+←/→` move within a group |
| JetBrains Rider | `Alt+1..9` focus tool windows; `Shift+Esc` hide; `Ctrl+Shift+←/→` resize |

## Design

**Accessible tree.** The docking host is a container of regions. Report:

- host → `AccessibleRole.Client`, children one per dock region and one per floating window
- each panel → `AccessibleRole.Pane`, named by its caption, with `Bounds` and a `Focused` state
- each tab strip → `AccessibleRole.PageTabList` with `PageTab` children

`DisplayContainers` and `Filtering` both needed exactly this and the pattern is established: override
`CreateAccessibilityInstance`, and override `GetChildCount`/`GetChild` — **the defaults return -1 and
MSAA walks the window hierarchy instead**, which is why a traversal check on an un-overridden control
measures nothing and proves nothing.

**Keyboard docking.** Every drag-reachable operation needs a command:

- focus a panel by index; cycle panels forward/back
- move the focused panel to a dock position, or into a neighbouring group
- split, maximise ([04](04-maximise-and-zen.md)), close, float, re-dock
- resize a splitter by keyboard, in a defined increment

**One key handler.** Features 01, 02 and 04 each want bindings. They route through this one, or the
folder acquires four competing `ProcessCmdKey` overrides that shadow each other.

## Work

- [ ] `CreateAccessibilityInstance` on the docking host, panels and tab strips
- [ ] Focus indicator on the focused panel — check whether one already exists before adding it; a
      complete focus ring that nothing enabled has already been found twice in this codebase
- [ ] Command surface for every drag-reachable operation
- [ ] A single key-binding table, with features 01/02/04 registering into it
- [ ] Splitter resize by keyboard
- [ ] High-contrast legibility for captions, guides and rails

## Verification

- Accessible **names** per panel, not a tree walk. Compare an empty host against a populated one so
  the child count is known to be real rather than constant
- Every operation reachable by driving keys, asserted by sending keys — not by reading the handler
- Focus indicator visibly distinct from both hover and active
- Guides and captions legible under `SystemInformation.HighContrast`

---

## Outcome

### What was actually missing

Accessibility was genuinely **0** — the audit's grep was right this time. Keyboard handling was
**not**: `BeepDockingManager.Navigation.cs` already had a single host-form handler with Ctrl+Tab,
Ctrl+F4/W, Ctrl+Shift+arrows and Escape, and `hostForm.KeyPreview = true` was already set, so it
genuinely fires. The "one key handler" this document asked for existed; what it lacked was commands.

Two things this document listed as work were already done, and one it warned about was real:

- **`Esc` cancels an in-flight drag** — listed as open in [05](05-drop-guides-and-preview.md); the
  Escape branch already called `_dragController.Cancel()`.
- **A complete focus surface that nothing enabled** — `DockingPainterContext.IsFocused` was declared
  and reset in `Update()`, but never assigned `true` and never read. This document's instruction to
  *"check whether one already exists before adding it"* was the reason it was found rather than
  duplicated.

### Accessible tree

`DockPanel` → `AccessibleRole.Pane`, named by `Title`. That matters: the caption lives in `Title`,
not `Control.Text`, so the default reported **no name at all**. It also surfaces `Focused`, and
`Invisible` for a panel concealed by a maximise.

`BeepDockspace` → `PageTabList` with a `PageTab` per tab, named by title, screen bounds taken from
the same `_captionLayout.TabRects` the painter and hit-testing use, plus `Select`/`DoDefaultAction`.
With `HeaderPosition.None` — which zen mode sets — it reports `Pane` and no children rather than an
empty tab list.

Overriding `GetChildCount`/`GetChild` is what publishes the tabs, exactly as this document said.

### Commands, all through the existing handler

| binding | command | reference |
|---|---|---|
| `Alt+1..9` | focus panel by index | Rider |
| `Ctrl+Shift+F12` | toggle maximise | Rider |
| `Ctrl+Alt+Z` | toggle zen | VS Code's `Ctrl+K Z` chord, single-stroke |
| `Ctrl+Alt+Right/Down` | split right / down | Rider |
| `Ctrl+Alt+Shift+arrows` | move an edge divider | VS / Rider |
| `Escape` | restore a maximise, after navigator and drag |  |

`FocusPanelByIndex` orders by dock position then key, not registration order, so a keystroke reaches
the same panel every time instead of shuffling as panels are closed and reopened.

### Three defects found by verifying rather than reading

**1. The guards were not mutually exclusive.** The pre-existing `Ctrl+Shift+Left/Right` branches did
not exclude `Alt`, so in an `else if` chain they matched `Ctrl+Alt+Shift+Right`, declined to act, and
left the resize branch unreachable. This document warned about "four competing `ProcessCmdKey`
overrides that shadow each other"; the same shadowing happens inside *one* handler when the guards
overlap.

**2. Splitter drags overshot by a factor of ~3.6.** `DragSplitter` measured the ratio against the
edge group's own laid-out rectangle, while `BuildLayout` applies that ratio to the space still
available. A 16px drag on a 249px edge of a 900px host moved it **57px**, and the inverse drag did
not return. `BuildLayout` now records the extent it used and `DragSplitter` reads it — single-sourced
rather than re-derived. **This was never keyboard-specific: mouse dragging took the same path.**

**3. Painting a dockspace header threw.** Found while making the tab-strip check deterministic —
written up in full in [09](09-dead-surface.md); the short version is that all 32 renderer call sites
wrapped `PaintResourceCache`'s shared brushes and pens in `using`.

### The focus ring, and why reading the painter would have shipped it broken

`IsFocused` is now set from `ContainsFocus` (focus usually sits on a control *inside* the active
panel) and drawn once in `CaptionRenderer.Paint` for all five tab styles, rather than five times.
`OnEnter`/`OnLeave` invalidate the header, without which the ring is painted correctly and appears
only when something unrelated happens to repaint.

The first version drew the ring in `AccentColor` — and changed **not one pixel**. In this theme the
active tab's background *is* the accent, so an accent ring on the active tab is invisible. The
tracing showed `drawing {X=2,Y=2,Width=156,Height=22} colour=accent` while the bitmap was byte-identical
to the unfocused one. It now uses `ActiveTabForeColor`, which is legible against that background by
construction — it is what the tab's own text uses.

Worse, the check had **passed for the wrong reason** on an earlier run: "focus is distinguishable
from hover" went green while focus drew nothing, because it was measuring the *hover* difference.
Only comparing focused against **plain** exposed it.

### Measured

```
plain vs focused:            0.44% of pixels differ
focused vs hovered:          2.14%
hovered vs hovered+focused:  0.44%   (focus stays visible under hover)
two renders of one state:    0.00%   (so the deltas are the flag)
```

Keyboard, driven by sending keys through the form's `OnKeyDown`, with `KeyPreview` asserted
separately and an unbound-combination baseline:

```
explorer width 249 -> 265 after Ctrl+Alt+Shift+Right   (exactly the 16px step, and it returns)
Alt+1 -> explorer, Alt+2 -> editor, Alt+1 -> explorer  (stable)
Alt+9 with four panels: no change
```

Accessibility, every assertion paired against a stock `Panel` measured the same way
(`role=None, childCount=-1`), and an empty manager compared against a populated one (0 vs 3 named).

`DockProbe`: **98 passed, 0 failed**. Solution builds with 0 errors.

### Remaining

- [ ] High-contrast legibility under `SystemInformation.HighContrast` — needs a themed pass over
      guides, captions and rails, and a way to drive the setting headlessly
- [ ] `IsPressed`, `CanClose`, `CanFloat`, `CanAutoHide`, `CanPin` on `DockingPainterContext` have
      **zero readers** — dead surface [09](09-dead-surface.md) missed. Implement or remove
- [ ] Move-between-groups by keyboard; `MoveActivePanel` reorders within a stack only

---

## Outcome — high contrast, and the header appearance work it led into

### High contrast strengthens the theme rather than replacing it

The first attempt swapped in a `SystemColors` palette. That was wrong for this library: the theme and
the control style are the source of the look, so high contrast now takes the **theme's own colours**
and pushes any pair that is too close together until it is legible. Backgrounds are never moved —
they carry the theme's identity, and a high-contrast dock that looked like nothing else in the
application would be a different product, not an accessible one.

`BeepDockingManager.HighContrast` is a `bool?` overriding `SystemInformation.HighContrast`, for the
same reason the display set is an input: otherwise none of it is testable without changing a
machine-wide Windows setting.

### Two findings

**`ColorUtils.GetContrastColor` chooses the wrong colour.** It picks black or white from a
*brightness* threshold rather than from the contrast each actually achieves. Measured on this theme's
header: `#2196F3` sits at brightness 0.49, so it returned white at **3.12:1** where black gives
**6.72:1** — below WCAG AA in a default theme. Docking now resolves by ratio. The shared helper is
left alone deliberately: it is used across the library, and correcting it would change text colour on
mid-tone surfaces everywhere, which is a decision to take deliberately rather than as a side effect.

**AAA is not always reachable.** With the background fixed, the best any foreground can do is black
or white; against a mid-tone surface that caps below 7:1 — `#2196F3` tops out at 6.72, mid-grey at
5.32. The requirement is therefore the achievable ceiling, not a flat number that would only be met
by repainting the theme.

```
normal  header 6.72:1   panel 14.77:1   active tab 14.94:1   inactive tab 10.88:1
high    header 6.72:1 (ceiling 6.72)    panel 14.77 (19.26)  inactive 10.88 (14.03)
```

### The header, looked at rather than reasoned about

Rendering the strip at 3x and reading the image found three things no amount of code review would
have:

**Every tab was the same width.** `LayoutVisibleHorizontalTabs` used `tabsWidth / count` clamped to
`MaxTabWidth`, so a three-letter title occupied the same 160px as a long one, each carrying a block
of dead space. Tabs now size to their content and compress proportionally only when the strip runs
out of room, which is what VS Code, Visual Studio and Rider all do.

**Sizing to content immediately produced "Expl…", "Out…", "Proble…".** The measurement used
`TextRenderer` (GDI) while the renderer draws with `Graphics.DrawString` (GDI+), and GDI+ needs more
width for the same string — so every tab was laid out fractionally too small and ellipsised its own
title. Measure-here/draw-there, caught only because the render was inspected. The font **and** the
`StringFormat` are now single-sourced on `CaptionLayoutManager` and consumed by the renderer;
trimming and wrapping both change how much width a string needs, so sharing one is not optional.

**The auto-hide pin was a lollipop.** `PaintPinFallback` drew a filled circle with a line rising out
of it, and `CaptionIcons.Pin` pointed at `fi-tr-map-pin.svg` — a location marker. "Pin this panel"
means a thumbtack; a map pin means "a place". Now drawn as a thumbtack seen side-on: a head bar, a
markedly narrower parallel shaft, and a needle. The step between head and shaft is what makes it read
as a pin rather than a funnel — the first attempt tapered and looked like one.

The icon set has no plain thumbtack, only `fi-tr-thumbtack-slash` (the *unpin* state), which is why
it is drawn rather than referenced.

`DockProbe`: **203 passed, 0 failed**. Docking suite 48/48. Solution 0 errors.

### Remaining

### The active tab — changed, on approval

It was a filled accent block. Now a **raised surface with the accent carried by its underline**.

The reason it mattered beyond taste: the active-tab accent bar and the keyboard focus ring are both
painted in the accent colour, so on an accent background **neither could be seen**. The focus ring
drew its rectangle and changed not one pixel — that is how it was found. Making the tab a surface
makes both visible without touching either painter.

Inactive tab text also dropped from pure white to a dimmed grey, so the active tab reads as selected
rather than merely differently shaped.

The blue block was coming from the `Default` palette — the one used when no Beep theme is in force.
Themed paths already resolved the active tab to a surface colour (`WhiteSmoke` in the theme measured),
because `FirstValid` tries the theme's colours first and only falls back to `Default`. So this was a
defect in the unthemed fallback, not in the theming.

Verified across all four tab styles: `Default` and `VsIde2022` show the accent as an underline,
`JetBrains` on top, `VsCode` as a pill — each style's own convention, all now visible.
