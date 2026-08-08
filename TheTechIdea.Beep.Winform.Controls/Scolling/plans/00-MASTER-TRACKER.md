# Scolling — review & enhancement plan (2026-08)

2 files, 1,410 lines (fourteenth folder; the folder name itself carries the typo — the
BeepSteppperBar precedent says renaming is the user's call, recorded here).
`BeepScrollBar : BaseControl` (custom scrollbar; consumed by ContextMenus, ListBoxs,
Trees) and `BeepScrollList : BeepPanel` (carousel list, hit-area pipeline). Census is
almost clean (0 flags, 0 guards, 1 literal, 0 catches) — the diseases here are stamping,
dead members, and triplicated layout math.

## Findings (static pass)

### F1 — BeepScrollBar: ApplyTheme STAMPS theme colours into the custom-colour properties

`TrackColor/ThumbColor/ThumbColorHover/ThumbColorActive` are caller-facing custom
properties, and `ApplyTheme` writes the ScrollBar* slots into them — after the first
theme apply, an explicit caller colour and a themed one are indistinguishable, and the
CLAUDE.md rule ("a colour the caller set explicitly must survive a theme change") cannot
hold. Settled shape: fields default `Color.Empty`, DrawContent resolves per paint
(custom-else-slot) from the ScrollBar* family (`ScrollBarBackColor/TrackColor/
ThumbColor/HoverThumbColor/ActiveThumbColor`), HC per-paint branch. ApplyTheme override
deleted. No consumer sets these explicitly (verified), so behaviour only improves.

### F2 — BeepScrollBar: DesignMode guards BREAK the designer; dead fills; static brush cache

- `if (DesignMode) return;` in the Minimum/Maximum/Value/LargeChange/SmallChange setters
  makes those properties UNSETTABLE at design time (and the ctor's `!DesignMode` around
  SetStyle is inert — DesignMode is always false before siting). All deleted.
- DrawContent fills the same rect twice (BackColor then TrackColor) — first fill dead.
- Both files carry a never-disposed static `Dictionary<int, SolidBrush>` cache; painting
  three rects does not need one. Replaced with using-scoped brushes.

### F3 — BeepScrollList: two dead composed helpers, themed and disposed but never drawn

`_button` (BeepButton) and `_label` (BeepLabel) are created, fully themed in ApplyTheme
(List* slots stamped into them), and disposed — but no draw call ever uses them; item
rendering is direct FillRectangle + TextRenderer. Deleted (ctor/ApplyTheme/Dispose
blocks). `_image` IS used (item icons) and stays.

### F4 — BeepScrollList: item layout triplicated, clicks fire twice

Three copies of the item-rect math: `UpdateItemPositions` (CENTERED layout — disagrees
with what is drawn), `DrawContent` (row layout + re-registers hit areas every paint),
and `OnMouseClick` (third copy, calls `ItemClicked` directly). Consequences: the
pre-paint hit rects never match the drawn rows until the next paint overwrites them, and
a click fires `ItemClicked` TWICE (once via the BaseControl hit-area callback, once via
OnMouseClick) — `ItemSelected` raised twice per click. Fix: one `GetItemRect(i)`
authority; hit areas rebuilt from it in one place; OnMouseClick deleted (the hit-area
pipeline IS the click path); `ItemClicked` routes through the `SelectedIndex` setter
(one method decides — the Lovs lesson).

### F5 — BeepScrollList colours: generic slots instead of the List* family; literal indicator

Items paint with control-level `BackColor/ForeColor/Selected*/Hover*`; the dedicated
family (`ListBackColor`, `ListItemForeColor`, `ListItemHover*`, `ListItemSelected*`) was
only ever stamped into the dead `_button`. Items now resolve the List* slots per paint.
`ScrollIndicatorColor` (`= Color.Gray`) becomes Empty-default custom passthrough over
`ScrollBarThumbColor`.

### F6 — minor mechanics

Drag overscroll: `OnMouseMove` assigns `_scrollOffset = _targetScrollOffset` BEFORE
clamping. Empty `OnPaint` override (calls base only) deleted.

### F7 — no probe

Planned (ScrollProbe): scrollbar track+thumb render, thumb moves with Value, page-click
and wheel change Value + raise ValueChanged, live theme change re-renders, custom colour
survives a theme change (the F1 proof); list renders items, selected differs from
unselected, click selects + raises ItemSelected ONCE (the F4 proof), keyboard nav,
indicator appears when content overflows, live theme change. Eyeball everything.

## Order

1. F1–F6 in one batch (small folder) — build + commit
2. F7 probe + eyeball — commit fixes

## Standing constraints

There is ALWAYS a theme — slot per role from the control's OWN slot family (ScrollBar*/
List*), custom overrides as Empty-passthrough, no flags/guards/blends, HC per paint. A
check must be able to fail; renders get eyeballed. Commit to master only.

Observations (not in scope): `ControlPaint.DrawCheckBox` renders the system checkbox
(unthemed); the 60fps animation timer never stops when idle; the folder-name typo.
