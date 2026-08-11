# Lovs — review and enhancement

Master tracker for `TheTechIdea.Beep.Winform.Controls/Lovs/`.
**8 C# files, 2,158 lines: `BeepListofValuesBox` (the field) and `BeepLovPopup` (the selector).**

## What this is meant to be

An **Oracle Forms List of Values**: a field that holds a *key*, displays a *description*, and opens a
searchable multi-column selector — traditionally on **F9** — whose list comes from a query.

The shape is right. The field splits key and display value, `BeepLovPopup` is backed by `BeepGridPro`
so multi-column comes free, F9 and Alt+Down are both wired, and there is an async `ItemsLoader` for the
query-backed case. **The problem is that the query-backed case does not work.**

## The headline defect

**With an `ItemsLoader` set, the LOV cannot hold a value.**

`ValidateKey` and `UpdateDisplayValue` consult `ListItems` and nothing else. `ItemsLoader` fills the
*popup's* list and never writes back to `ListItems` — the source even carries a comment saying it
should, above code that does not. So:

- setting `SelectedKey` to a key the loader would return is **rejected and silently reverted**
- picking a row in the popup calls `SetSelectedItem`, which assigns `_keyTextBox.Text`, which fires
  `TextChanged`, which fails `ValidateKey` and **reverts the selection the user just made**

Measured, with a loader returning the classic `DEPT` rows:

```
PASS  sync:  a valid key is accepted            SelectedKey = '20'
PASS  sync:  the display value resolves         SelectedDisplayValue = 'Research'
FAIL  async: a key from the loader is accepted  REVERTED to ''
FAIL  async: the display value resolves         SelectedDisplayValue = ''
```

The synchronous pair passing is what makes the failing pair mean something: the check discriminates
between the two paths rather than being broken for both.

**An Oracle LOV is query-backed by definition.** A designer-populated `ListItems` is the demo case;
the loader is the real one, and it is the one that does not work.

## Stages

| # | Stage | Kind | Status |
|---|---|---|---|
| [01](01-loader-value-resolution.md) | The async loader cannot hold a value | **bug** | ☑ done |
| [02](02-validation-semantics.md) | Validation is inconsistent and has no off switch | **bug** | ☑ done |
| [03](03-popup-selection.md) | The popup accepts a row nobody chose | **bug** | ☑ done |
| [04](04-async-lifecycle.md) | The spinner outlives the popup | **bug** | ☑ done |
| [05](05-binding-and-return.md) | Data binding and multi-column return | enhancement | ☑ done |
| [06](06-composition.md) | The field paints itself; the popup hand-positions | refactor | ◐ popup done, field NOT composed |
| [07](07-scale.md) | Filtering and paging for real LOV sizes | enhancement | ◐ partial |
| [08](08-verification.md) | The harness | verification | ◐ partial |

Status marks: ☐ open · ◐ in progress · ☑ done

## Stages 01–06 done, 07 partial — 43 checks, 0 failures

| | |
|---|---|
| **One source of truth** | a `_known` map fed by `ListItems`, the loader's results, and accepted items |
| **`KeyResolver`** | Oracle's validate-from-list: resolve one key without loading the list |
| **Provisional acceptance** | a key that arrives before the list is kept and resolved, not thrown away |
| **`RestrictToList`** | the validated / non-validated distinction Oracle Forms makes |
| **`KeyRejected`** | a refusal is now observable, not just visible on screen |
| **`BoundProperty`** | set to `SelectedKey`, so the control can take part in data binding at all |
| **One `ApplyKey`** | typing and assignment behave identically |

| **`SearchLoader`** | typing queries the source, debounced — `SearchChanged` had no subscriber |
| **Enter no longer guesses** | it committed row 0 whenever nothing was highlighted |
| **`ReturnMappings`** | Oracle's return items — a LOV fills more than the box it sits in |
| **The field is composed** | value and badge are `BeepLabel`s; the 22 % split is one constant, not two |
| **The popup is composed** | header and footer are `TableLayoutPanel`s; no bounds arithmetic on resize |
| **`MaxRows` + virtualization** | a bounded, virtualized grid that says when it is showing a subset |

Also: `LoadItemsAsync` returns its results, the spinner stops when a load is cancelled instead of
ticking on a hidden form forever, Down moves from the search box into the list, and every failure path
reports through `BeepLog`.

**Three loaders now answer three questions** — `ItemsLoader` (what is in the list), `SearchLoader`
(what matches what I typed), `KeyResolver` (what is this one key). Only the first existed, which is
why a large LOV had to load everything to do anything.

### Three bugs in the fix, each caught by a check

All three are the same shape — **assigning the key text box runs the whole validation cycle
synchronously** — and none would have been found by reading:

1. `RejectKey`'s revert fired `TextChanged` with an empty key, which is valid, which cleared the error
   the revert had just raised.
2. The `SelectedKey` setter assigned `Text` (which validated and possibly rejected) and then repeated
   the accept sequence, clobbering the rejection.
3. Starting the lookup from inside `ValidateKey` meant a resolver returning a completed task ran its
   continuation inline, before the caller had assigned the text box.

## What the probe established

| finding | evidence |
|---|---|
| **The loader path cannot hold a value** | `SelectedKey = "20"` reverts to `""`; display stays empty |
| **`BoundProperty` is never set** | empty — the control cannot participate in data binding |
| **No restrict-to-list switch** | entry is always forced to match; free-text LOVs are impossible |
| **A rejected key reports nothing** | `HasError` stays false on the property path — a silent revert |

That last one is worth stating carefully. It **passed** the check as written ("no false validation
error") and is still a defect: the property setter reverts silently while the typed path sets
`ErrorText` and shows a notification. Same rejection, two different behaviours, and the quieter one is
the one a data-binding caller hits. See [02](02-validation-semantics.md).

## Read before changing anything

- **`SetSelectedItem` writes through `_keyTextBox.Text`**, so it re-enters `KeyTextBox_TextChanged`
  synchronously. Any fix to validation has to account for programmatic assignment looking exactly like
  typing. This is the mechanism behind the headline defect.
- **The 22 % / 78 % key/value split is computed twice** — in `AdjustLayout` and again in
  `PaintValueArea`. They agree today by coincidence of identical arithmetic.
- **`SearchChanged` exists on the popup and nothing subscribes to it.** The comment says it is "useful
  for server-side filtering", which is exactly what a large LOV needs; see [07](07-scale.md).

## Standing constraints

Per `CLAUDE.md`: report every catch through `BeepLog`; no stubs or legacy paths; nothing assigns
colours; compose from Beep controls; a check must be able to fail for the reason it was written.

---

## Batch: the theming layer

The behaviour stages (01–06) left the theming layer untouched. A census found the whole disease
set: 32 `useThemeColors` sites, 18 `Color.Empty` guards, 19 literal palettes (Windows blue,
Bootstrap danger red, assorted greys), a literal `new Font("Segoe UI", 11f)`, two luminance
shifts, and no DPI scaling anywhere.

### Most of it was dead

`LovThemeHelpers`, `LovIconHelpers`, `LovStyleHelpers`, `LovColorConfig` and `LovStyleConfig` —
**577 lines, five files — had no consumer anywhere in the repo.** Deleting them all compiled with
zero errors, which is the proof. They accounted for nearly every literal and every `Color.Empty`
guard in the census. `LovFontHelpers` is the only one that was live.

The `GetErrorColor` / `GetButtonIconColor` hits a naive grep reports are the identically-named
methods in the Numeric and Wizard helpers, not these.

### Public members removed

| member | why |
|---|---|
| `LovThemeHelpers` (whole class) | no consumer |
| `LovIconHelpers` (whole class) | no consumer |
| `LovStyleHelpers` (whole class) | no consumer |
| `LovColorConfig`, `LovStyleConfig` (models) | no consumer |
| `BeepLovPopup.LovTheme` (string), `BeepLovPopup.UseThemeColors` (bool) | forwarded into children alongside an `ApplyTheme()` call on each — see below |

### The live defects

- **`ApplyLovTheme` themed its children from the outside.** It pushed a theme *name* and a
  `UseThemeColors` flag into the search box, three buttons, the count label, the chips and the
  grid, then called `ApplyTheme()` on each. Every one of those is a `BaseControl` that subscribes
  to `ThemeChanged` and re-applies itself; walking children re-enters theming, which CLAUDE.md
  rule 4 forbids. It now themes only the plain WinForms containers it actually owns.
- **`BeepLovPopup` is a plain `Form` and nothing re-themed it on a theme change.** It only
  followed if the field happened to push a theme in. It now subscribes to `ThemeChanged` itself
  and unsubscribes on dispose (the event is static and would otherwise hold every popup opened).
- **`BeepListofValuesBox` pushed theme + flag into `_keyTextBox` and called `ApplyTheme()` on it.**
  Same rule, same fix. Only the font is still set from the parent, because typography is this
  control's decision rather than the child's.
- **The header used an `Empty`-guard detour** (`GridHeaderBackColor != Color.Empty ? … :
  PanelBackColor`). One slot, one return; an unfilled slot is the theme's bug to fix in its part
  file.
- **The value area fell back to `SystemColors`** via `_currentTheme?.Slot ?? SystemColors.X`, so a
  null theme silently produced Windows' palette inside a themed control. There is always a theme.
- **The key badge picked literal `(30,30,30)` / `White`** by BT.601 luminance, ignoring the theme.
- **Badge padding and corner radius were raw pixels**; they scale through `DpiScalingHelper` now.

### A wrong fix, caught by making the check stricter

The badge ink first used `OnPrimaryColor` as the "ink on a fill" slot. The check asserted only
that the chosen slot beat the alternative — which **passed on DarkTheme at 1.25 vs 1.25**, both
candidates being equally illegible. Adding the WCAG AA floor turned it red and named the numbers:
white ink on a cyan accent.

That looked like a theme bug and was not. `OnPrimaryColor` means *ink on `PrimaryColor`*, and
DarkTheme's `PrimaryColor` is (18,18,18) — white on it is correct. The badge is filled with
`AccentColor`, so the slot was simply the wrong candidate. The candidates are now `ForeColor` and
`PanelBackColor`, the two ends of the theme's own contrast range. DarkTheme went **1.25 → 13.30**;
LightTheme stayed at 9.88.

Had the check kept its weaker form, a near-invisible badge would have shipped with a green tick.

### Verified — 56 pass, 5 fail

New checks: the two themes differ (blindness guard), the popup follows a *live* theme change, it
resolves the manager's current theme, the header uses the grid-header slot, and badge ink clears
WCAG AA on both themes while remaining a theme slot.

**The probe was hanging, and it mattered.** An orphaned `LovProbe` from an earlier run held its
own `.exe` open, so **four consecutive builds failed to copy it and every run was the stale
binary** — checks that existed in source appeared not to run. The probe now flushes and hard-exits.
`Hide()` on the last visible form was ending the message loop; a minimal Show/Hide repro proved
`Hide()` itself is fine, so this is a probe-shape issue, not a library defect.

### Still failing — stage 06 is not what this tracker claims

Five checks fail, all pre-existing, and they contradict the stage-06 row above:

```
FAIL  compose: the field holds a TableLayoutPanel: 1 child controls: BeepTextBox
FAIL  compose: the display value is carried by a control: still painted
FAIL  compose: the key badge is its own label: 0 visible label(s)
FAIL  compose: ShowKeyBadge=true brings it back: 0 visible label(s)
FAIL  compose: an empty field shows a placeholder, not a value
```

`BeepListofValuesBox.ValueArea.cs` paints the value and the badge with `TextRenderer.DrawText` and
`FillPath`. **The field is not composed.** Stage 06 is marked done and says "value and badge are
`BeepLabel`s"; the code says otherwise, and per CLAUDE.md the code is authoritative. The row is
wrong and the work is outstanding — the popup half of stage 06 (header/footer `TableLayoutPanel`s)
did land and passes.

This batch deliberately fixed the painted path rather than replacing it, so the theming is correct
either way; composing the field remains open.

---

## Batch: tooltips removed, popup sized to its content

### Tooltips are gone from the LOV

Two sources, both removed:

- `BeepListofValuesBox` called `ShowNotification(...)` on a rejected key, which pops a
  `CustomToolTip` window over the form. `RejectKey` already sets `ErrorText` and raises
  `KeyRejected`, so this was a third copy of the same message — and the only one that opened a
  window.
- Recent chips set `ToolTipText`, which is precisely what makes `BaseControl` register a rich
  tooltip with `ToolTipManager`. Nothing else in Lovs sets tooltip text, and `UpdateTooltip`
  returns early when it is empty, so the LOV now registers no tooltips at all.

### Sizing

- **`MaxPopupHeight` did nothing.** A public property on `BeepListofValuesBox` *and* on
  `BeepLovPopup`, pushed from one to the other on every open, and read by neither. It is the cap
  now.
- **The popup sized to `VisibleRows` (10) regardless of how many rows existed.** A four-row result
  opened a ten-row window, so over half the dropdown was empty space. It sizes to the rows there
  are, capped by `VisibleRows` and then by `MaxPopupHeight`. 420x569 -> 420x322 for four rows.
- **`FitColumns` ran before the popup had its real width.** It scales columns to the grid's current
  client width but is called from `RebindGrid`, which runs *before* `ApplyFixedSize` sets the
  width — so the columns were fitted to the wrong number. It is called again after the resize.
- **Every layout constant was raw pixels** (`HeaderH`, `FooterH`, `RecentPanelH`, `GridChromeH`).
  They scale through `DpiScalingHelper` now.
- **Recent chips measured with `SystemFonts.DefaultFont`** and were then drawn in the theme's font,
  so every chip was sized for a font it never used. One font measures and draws now, and the chip
  height scales.

### Fixed — and the fault was here, not in BeepGridPro

**The spurious horizontal scrollbar was `FitColumns` measuring against the wrong width.**
`GridScrollBarsHelper` compares the total column width against
`Layout.RowsRect.Width - ScrollbarWidth` whenever a vertical scrollbar is needed. `FitColumns`
scaled the columns to `_grid.ClientSize.Width` — a different, larger number, with (per its own
comment) "no reserve of any kind" — so the columns overflowed by exactly the scrollbar's width and
the grid correctly raised a horizontal scrollbar, which then ate the height that the last row
needed.

It now fits against the grid's own `Layout.RowsRect.Width` with the vertical scrollbar's width
reserved unconditionally. The old comment rejected a *conditional* reserve because it made column
widths depend on how many rows happened to be loaded — which is true, and is an argument for
reserving always rather than never. No change to `BeepGridPro` was needed: its scrollbar logic was
right and the caller was wrong.

One allowance remains and is honest about itself: `GridChromeH` (18px) does not cover everything
the grid puts around its rows, so the height adds a scrollbar's worth on top. That figure is
measured, not assumed — removing it clips "Operations" off a four-row list and restoring it shows
all four.

Verified by rendering the popup populated in both themes and eyeballing, plus 62 passing checks.
The 5 failures are still the pre-existing composition ones recorded above.
