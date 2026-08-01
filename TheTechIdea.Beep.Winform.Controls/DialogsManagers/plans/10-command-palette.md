# 10 — Command Palette

**Priority P2. Phase 5.** Not yet audited.

## What exists

`CommandPalette/BeepCommandPaletteDialog.cs` (365 lines) and `CommandAction.cs`, surfaced through
`ShowCommandPalette(IEnumerable<CommandAction>)` and `ShowQuickActions(...)` — two public entry
points whose relationship is unestablished. Given the pattern found in [01](01-api-surface.md), the
first thing to check is whether they are aliases, near-aliases, or genuinely different surfaces.

## What the reference products do

| System | Behaviour that defines it |
|---|---|
| VS Code Quick Pick | fuzzy match with **highlighted matched ranges**, MRU ordering, `>` / `@` / `#` mode prefixes, keyboard-only operation |
| Raycast / Spotlight | fuzzy match, ranked by frecency, inline actions on the selected row |
| Linear / Superhuman `Cmd-K` | scoped commands, nested menus, per-command keyboard hints shown in the row |
| Slack `Cmd-K` | recent-first, typed sections with headers |

The features that separate a command palette from a filtered list box:

1. **Fuzzy matching**, not `Contains` — "opfi" should find "Open File".
2. **Matched-range highlighting**, so the user sees *why* a row matched.
3. **Ranking by recency/frequency**, not alphabetical.
4. **Full keyboard operation** — arrows, Enter, Esc, and no mouse required at any point.
5. **Shown shortcut hints** on rows that have one.

## To establish

1. Is matching `Contains`, or fuzzy? (This is the single biggest quality difference.)
2. Are matched ranges highlighted?
3. Is there any ordering beyond input order?
4. What distinguishes `ShowCommandPalette` from `ShowQuickActions`?
5. Does `CommandAction` carry a shortcut, an icon, a group and an enabled state — the fields the
   reference products display?

## Verification

- ⬜ Probe: "opfi" matches "Open File" (fuzzy), and the matched characters are marked.
- ⬜ Probe: keyboard-only path from open to execute, with no mouse events.
- ⬜ Probe: repeated selection of one command moves it up the ranking.
- ⬜ Render: the palette at 100/150/200% DPI with long command names.
