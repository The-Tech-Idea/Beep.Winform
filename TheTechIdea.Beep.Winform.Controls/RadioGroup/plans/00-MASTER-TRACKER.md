# RadioGroup — review & enhancement plan (2026-08)

32 files, ~10,700 lines. `BeepRadioGroup` + `BeepHierarchicalRadioGroup` over `BeepRadioGroupBase`,
13 style renderers (Material/Card/Chip/Pill/Tile/Toggle/Segmented/Circular/Flat/Button/Checkbox/…)
over `IRadioGroupRenderer` with a shared `BaseRadioRenderer`, an MD3 **token model**
(`RadioGroupColorTokens`) as the colour authority, layout/hit-test/state/theme/icon helpers.

## Findings (static pass)

### F1 — the token model is the right shape, wrongly sourced

`RadioGroupColorTokens.FromTheme` already resolves **high contrast per paint** (the settled
Steppers/BreadCrumbs end-state — good). But: `useThemeColors=false` or null theme drops into
`FromStyleColors`, a whole parallel themeless palette; and half the tokens are **derived** via
Blend/Lighten/Darken (banned — "no blends, no derived palettes") when the theme has a real slot
for every single role:

| token | slot (exists, verified) |
|---|---|
| Surface / SurfaceVariant / SurfaceContainer | `SurfaceColor` / `PanelBackColor` / `CardBackColor` |
| OnSurface / OnSurfaceVariant | `ForeColor` / `SecondaryTextColor` |
| Outline / OutlineVariant | `BorderColor` / `InactiveBorderColor` |
| Primary / OnPrimary | `PrimaryColor` / `OnPrimaryColor` |
| PrimaryContainer / OnPrimaryContainer | `ButtonSelectedBackColor` / `ButtonSelectedForeColor` |
| Error / Disabled / DisabledContainer | `ErrorColor` / `DisabledForeColor` / `DisabledBackColor` |
| Hover/Focus/Press state layers | `FromArgb(8/12/12, PrimaryColor)` — alpha veil of a slot, KEEP |

Fix: `FromTheme(theme)` = theme ?? CurrentTheme, slot per role, no flag, no style param; delete
`FromStyleColors` + Blend/Lighten/Darken/Luminance (zero external callers).

### F2 — `RadioGroupThemeHelpers`: everything but one forwarder is dead

Only `ResolveTokens` (a one-line forward to `FromTheme`) is ever called. The other 8 getters —
the full useThemeColors/Empty-guard/literal anti-pattern PLUS `ShiftLuminance` — have **zero
callers** and carry 47 of the folder's 74 literals. Delete the file; `BaseRadioRenderer` calls
`FromTheme` directly.

### F3 — `ColorProfile` is a lying stub across all three controls

`RadioGroupColorConfig`: 15-literal Material palette exposed as a designer surface on
`BeepRadioGroupBase`, `BeepRadioGroup`, `BeepHierarchicalRadioGroup` — and `ApplyColorProfile`
ignores the profile entirely (`if (profile == null || _useThemeColors) return; SafeInvalidate();`).
Every colour a consumer sets does nothing. Rule 2: no stubs — delete the surface (public members
removed, decision recorded here). Per-instance colour overrides, if ever wanted, are a feature
to design, not a stub to keep.

### F4 — 4 swallows, 0 BeepLog

- `BeepHierarchicalRadioGroup.cs:845` + `BeepRadioGroup.cs:387` — search-box "polish" catches
  that ALSO wrap an explicit `_searchBox.ApplyTheme()` — a rule-3 violation being silenced
  (children theme themselves). Drop the child ApplyTheme; report the catch.
- `RadioGroupLayoutHelper.cs:625` (ItemMeasurer catch → default size) and `:633`
  (CreateGraphics catch → null) — degraded paths that must report (FallbackOnce).

### F5 — `useThemeColors` flag threaded through 18 files

Base renderer property + `IRadioGroupRenderer` + all 13 renderers + control properties. Dies
with F1/F2.

### F6 — ~25 literals outside the dying files

Icon helpers (7), Flat/Card/Segmented renderers, style helpers, Drawing partial, hierarchical
control. Sweep to tokens/slots.

### F7 — no probe

Planned (RadioProbe): all 13 renderers render with 4 items (one selected, one disabled) wide +
narrow, blank-guard + cross-renderer distinctness; theme responsiveness; click → SelectedItem +
event through the real hit path (cursor parked); keyboard navigation; hierarchical control
renders; EVERY render eyeballed (the Steppers one-node lesson).

## Order

1. F1 tokens rewrite + F2 delete dead helpers + F3 delete ColorProfile stub + F4 swallows +
   F5 flag removal — one coherent change: build + commit
2. F6 literal sweep — build + commit
3. F7 probe + geometry eyeball — commit per fix batch

## Batch 1 done — tokens slot-direct, dead systems deleted (build 0 errors)

F1: `RadioGroupColorTokens.FromTheme(theme)` — one slot per role exactly as tabled above; HC
branch kept (was already the settled per-paint shape; its `Color.Red` error literal now
`SystemColors.MenuHighlight`, aligned with Steppers); state layers stay alpha veils of Primary.
Deleted: `FromStyleColors` (the themeless parallel palette), Blend/Lighten/Darken/Luminance/
Fallback colour math — zero external callers.

F2: `RadioGroupThemeHelpers.cs` deleted (312 lines, 47 literals, `ShiftLuminance`) — only its
one-line `ResolveTokens` forward was ever called; `BaseRadioRenderer` now calls `FromTheme`
directly.

F3: the `ColorProfile` stub surface deleted everywhere — `RadioGroupColorConfig` model, the
three controls' properties/fields, `ApplyColorProfile` in all three (the base one ignored the
profile entirely; the two control ones only stamped Back/ForeColor when the flag was off).
Public members removed per rule 2 — recorded here as the decision.

F4: all 4 swallows report; the two search-box catches also dropped their `_searchBox.ApplyTheme()`
(rule 3 — parented children theme themselves; the catch existed to silence that call).

F5: `UseThemeColors` gone from `IRadioGroupRenderer`, base renderer, both wrapper renderers'
forwards, the three controls' shadow properties and all propagation. BaseControl's own property
remains (unwired here, harmless). Bonus: the group background cleared with **SideMenuBackColor**
— wrong slot — now `SurfaceColor`, single path.

Not verified yet: renders (batch 3). Restored during the sweep: an over-broad cut briefly took
the `Style` and `TextFont` properties with it — caught by build, restored verbatim.

## Batch 2 done — literal sweep (commit 5e6605a0)

Shadow token added (transparent in HC — no fake depth); card/segmented shadows alpha the slot;
the selected card's −20 RGB gradient derivation replaced by a solid tokens fill; design-time
placeholder + hierarchical connector/expander on slots; dead `GetIconColor`/`GetShadowColor`
deleted. Census: zero literals.

## Batch 3 done — probe 31/31, three real defects found by render

RadioProbe (scratchpad): 12 styles × normal+narrow render, cross-style distinctness, theme
responsiveness, hit-path click round-trip (SelectedValue + SelectionChanged + ItemClicked),
keyboard Down+Space, hierarchical render. Every render eyeballed.

Defects found and fixed:
- **Programmatic selection before Show() was a silent no-op** — `SelectValue` validates against
  the state helper's item list, but item propagation lived behind `IsHandleCreated`, so
  `new BeepRadioGroup { Items = ..., SelectedValue = "X" }` (the standard initializer shape)
  lost the selection and every style rendered UNSELECTED. The eyeball caught it; the colour
  counters had passed. Items now propagate to the state helper unconditionally; only layout
  waits for the handle.
- **Checkbox rendered pixel-identical to Flat** (circles, never a checkbox): the wrapper's
  documented job was "forces multi-selection visual semantics" but it forwarded the flag
  instead of forcing it. `AllowMultipleSelection` is now hard-wired true on the inner Flat —
  that is what draws square check boxes.
- **Button rendered pixel-identical to Card**: pure pass-through with no identity. It now
  paints a real button group — filled Primary button with OnPrimary centred text for the
  selected item, SurfaceContainer buttons otherwise; layout/measure still delegate to Card.

By design, recorded: `RadioGroupRenderStyle.Custom` is the `RegisterRenderer` slot — setting
it without a registered renderer is a guarded no-op (keeps the current renderer); excluded
from the probe's distinctness check with a comment.

Not verified: HC token branch (system HC not toggleable from a probe — same caveat as
Steppers/BreadCrumbs); hierarchical expand/collapse interaction unprobed (renders only).

## Standing constraints

There is ALWAYS a theme — slot per role, no flag, no guards, no blends/luminance derivation
(alpha veils of a slot are fine); semantic states from semantic slots. A check must be able to
fail, and renders get eyeballed, not just counted. Commit to master only.
