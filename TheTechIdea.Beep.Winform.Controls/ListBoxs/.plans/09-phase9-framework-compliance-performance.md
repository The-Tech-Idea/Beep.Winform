# Phase 9 — Framework Compliance & Performance Hardening

> Aligns BeepListBox with the **Beep WinForms Design System** enhancement checklist
> (framework compliance, no per-paint allocations, Beep-only child controls) and
> closes the outstanding **Phase 8** manual-validation gates.

Priority: **High**
Status: **Code complete (W1–W6)** — 2026-07-08. Build: 0 errors. Manual W7 (visual/DPI/HC evidence) pending.
Depends on: Phases 1–8 (feature-complete; this phase is quality hardening, not new features)

---

## Completion log (2026-07-08)

- **W1 (g.DrawString → TextRenderer):** DONE. Gate `grep DrawString ListBoxs` = 0.
  Converted base painter (empty state, group header, badge, initials, PgUp hint),
  `BeepListBox.Drawing.cs` (empty / empty-search / **search-match highlighter** with
  `NoPadding` width alignment), and 11 painters. `MeasureString` also eliminated
  (CommandListBoxPainter → `TextRenderer.MeasureText` for metric parity).
- **W2 (font caching):** DONE. Gates `BeepFontManager` = 0, `new Font(` = 0.
  Added `GetCachedFont(size, style)` on `BaseListBoxPainter` (resolves via the theme
  manager's shared cache, keyed by size+style, never disposed). Fixed a latent bug:
  the old `_cachedTrailingMetaFont?.Dispose()` disposed a shared cached font.
- **W3 (BeepContextMenu):** DONE. Gate `ContextMenuStrip` = 0. `ItemContextMenu`,
  `ListBoxContextMenuEventArgs.Menu`, `OnContextMenuOpening`, and the default
  Select/Copy/Edit/Delete menu now use `BeepContextMenu` with `SimpleItem` entries and
  `ItemClicked` dispatch.
- **W4 (GDI allocation removal):** DONE. Added cached `GetBrush(color)` / `GetPen(color,width)`
  on the base; swept the base hot paths + ~32 painters (≈66 SolidBrush + ≈68 Pen cached).
  Remaining `new SolidBrush`/`new Pen` are all legitimate: 2 base-cache constructors,
  1 once-per-paint empty-state ellipse, and capped/dashed checkmark/focus pens that
  must stay `using` (mutating a cached pen's caps would corrupt the shared instance).
  Gradient brushes correctly retained as `using`.
- **W5 (color audit):** DONE for literal-RGB fills — `DrawCheckbox` (theme + HC aware),
  selected-text color (`OnPrimaryColor`), Timeline checkmark. Alpha-of-theme overlays
  kept (sanctioned); avatar palette / shimmer white justified with comments.
- **W6 (virtualization accuracy):** DONE. PgUp/PgDn hint now uses the real
  `BeepListBox.VirtualSize.Height` (new internal accessor) instead of
  `PreferredItemHeight * count`.
- **W7 (manual validation):** PENDING — run the app, capture DPI/density/HC/keyboard
  evidence, then flip Phase 8 + this phase to fully Completed.


---

## Why this phase

Phases 1–8 delivered every feature (search, hierarchy, grouping, density, rich list
types, 47 painters). The functionality is done. What remains is **commercial-grade
polish and framework compliance** — the last column of the design-skill's
*Enhancement Checklist* that Phase 8 marked "Provisional Pass (Code)".

A static audit of `ListBoxs/**` on 2026-07-08 found concrete, countable debt against
the checklist's **CRITICAL** rules:

| Rule (design skill) | Target | Current | Where |
|---|---|---|---|
| No `g.DrawString` — use `TextRenderer.DrawText` | 0 | **31** occurrences / 13 files | Painters + `BeepListBox.Drawing.cs` |
| No `new Font()` in code-behind paint paths | 0 | **11** | `BaseListBoxPainter`, group/sub-text/badge helpers |
| No `BeepFontManager` calls — use `BeepThemesManager.ToFont(TypographyStyle)` | 0 | **45** | across painters + base |
| No plain WinForms controls — use Beep equivalents | 0 | **6** `ContextMenuStrip` refs | `BeepListBox.Properties.cs`, `.Events.cs` |
| No per-paint GDI allocations (cache brush/pen/font/path) | ~0 in hot loop | **125** `new SolidBrush` + **83** `new Pen` | Painter draw paths |
| Theme colors from `IBeepTheme`, not hardcoded RGB | fallback-only | **244** `Color.FromArgb` | Painters (subset are hardcoded RGB, not alpha overlays) |

These are not cosmetic. `new Font`/`new SolidBrush` inside `DrawItem`/`DrawSubText`
run **once per visible row per paint** — at 60 fps hover animation over a 20-row
viewport that is >1,200 GDI object allocations/second, each a finalizable handle.

---

## Goals

1. **Zero `g.DrawString`** in the ListBox subsystem — route all text through
   `TextRenderer.DrawText` (GDI, ClearType, DPI-consistent, matches the rest of Beep).
2. **Zero paint-path font allocation** — fonts resolved once via
   `BeepThemesManager.ToFont(TypographyStyle)` and cached on the painter, invalidated
   on theme/DPI change (the base painter already does this for trailing-meta — extend
   the pattern to title / sub-text / group-header / badge / separator).
3. **Beep-only child controls** — replace `System.Windows.Forms.ContextMenuStrip`
   with `BeepContextMenu` end-to-end (property type, event args, default menu builder).
4. **No per-paint brush/pen allocation in hot loops** — cache single-color brushes and
   pens keyed by (color, width), reset on theme change.
5. **Hardcoded-color audit** — every `Color.FromArgb(r,g,b)` with literal RGB must
   either become a theme token with a fallback chain, or be justified (avatar palette,
   shimmer white) with a comment.
6. **Close Phase 8 manual gates** — capture the DPI / density / high-contrast /
   keyboard evidence that Phase 8 left "Pending (Manual Capture)".

Non-goals: new painters, new list types, new public API surface. This phase changes
*how* things are drawn, not *what* is drawn.

---

## Workstreams

### W1 — Text rendering compliance (CRITICAL)

Replace all `g.DrawString(...)` with `TextRenderer.DrawText(...)`. Watch for:
- `StringFormat` centering → `TextFormatFlags.HorizontalCenter | VerticalCenter`.
- `MeasureString` → `TextRenderer.MeasureText` (already used in most measure paths).
- The search-match highlighter `DrawHighlightedText` in `BeepListBox.Drawing.cs`
  measures + draws substrings with `g.DrawString`/`MeasureString`; convert carefully so
  pixel widths still line up (TextRenderer measures differently — add the
  `TextFormatFlags.NoPadding` flag to match).

**Files:** `BaseListBoxPainter.cs` (`DrawEmptyState`, `DrawGroupHeader`, `DrawBadgePill`,
`DrawInitialsFallback`), `BeepListBox.Drawing.cs` (`DrawEmptyState`,
`DrawEmptySearchState`, `DrawHighlightedText`), plus any painter with a local
`DrawString` (grep list below).

**Done when:** `grep -r "DrawString" ListBoxs --include=*.cs` returns 0.

### W2 — Font resolution & caching (CRITICAL)

- Replace `BeepFontManager.GetFont(...)` and `new Font(...)` with
  `BeepThemesManager.ToFont(<TypographyStyle>)` using the theme's semantic roles:
  - Title → `_theme.BodyMedium` / `ListSelectedFont` per existing convention
  - Sub-text → `_theme.BodySmall` (or `CaptionMedium`)
  - Group header → `_theme.LabelMedium` bold
  - Badge → `_theme.CaptionSmall` bold
- Cache each resolved font on the painter instance (mirror the existing
  `_cachedTrailingMetaFont` pattern with a small dictionary keyed by role).
- Invalidate the cache in `Initialize()` and whenever `ApplyTheme()` re-inits painters
  (already the reinit path) — and on DPI change.
- **Do not dispose** theme-manager-owned fonts in consumers (checklist rule); only
  dispose painter-created transient fonts, which after this workstream should be none.

**Files:** `BaseListBoxPainter.cs` (add `GetRoleFont(role)` cache), `DrawSubText`,
`DrawGroupHeader`, `DrawBadgePill`, `DrawSeparatorRow`, `DrawInitialsFallback`, and the
~10 painters that call `BeepFontManager.GetFont` directly.

**Done when:** 0 `BeepFontManager` and 0 `new Font(` in `ListBoxs/**` non-designer code.

### W3 — Beep control mapping: ContextMenu (CRITICAL)

`ItemContextMenu`, `OnContextMenuOpening`, and `ListBoxContextMenuEventArgs` currently
use `System.Windows.Forms.ContextMenuStrip`. The design skill bans `ContextMenuStrip`
in favor of `BeepContextMenu`.

- Change `ItemContextMenu` property type → `BeepContextMenu`.
- Change `ListBoxContextMenuEventArgs.Menu` → `BeepContextMenu`.
- Rewrite the default menu builder (Select / Copy / Edit / Delete) to populate a
  `BeepContextMenu` with `SimpleItem` entries (consistent with the "all list data uses
  `BindingList<SimpleItem>`" rule).
- Update `BeepListBox.Events.cs` right-click handler to show `BeepContextMenu`.

**Note:** this is a public-API break on one event-arg type. It is acceptable within the
control library; grep the solution for external consumers of `ItemContextMenu` /
`ContextMenuOpening` before merging and update call sites.

**Done when:** 0 `ContextMenuStrip` in `ListBoxs/**`.

### W4 — Per-paint GDI allocation removal (HIGH, perf)

- Introduce a lightweight cached-resource holder on `BaseListBoxPainter`:
  - `GetBrush(Color)` → returns a cached `SolidBrush`, mutating `.Color` (SolidBrush.Color
    is settable) rather than allocating.
  - `GetPen(Color, float)` → cached pen keyed by width.
  - Reset/clear on theme change.
- Sweep painter draw paths (`DrawItem`, `DrawItemBackground*`, decoration helpers) to use
  the cache instead of `using (new SolidBrush(...))` / `using (new Pen(...))`.
- Keep `using` allocation only for genuinely one-shot, non-hot paths (empty state, drawn
  once per paint not per row).
- Cache `GraphicsPath` for rounded rects when the row size is unchanged (badge pills,
  selection rounding) — the base already has `GraphicsExtensions.CreateRoundedRectanglePath`;
  add a per-painter last-rect/last-radius memo.

**Done when:** no `new SolidBrush`/`new Pen`/`new GraphicsPath` inside any
per-row/per-item loop; representative scroll of 1,000 items at 150% DPI shows no GC
pressure spikes in the allocation profiler.

### W5 — Hardcoded-color audit (MEDIUM)

- Classify the 244 `Color.FromArgb` hits:
  - **Alpha overlay of a theme color** (`Color.FromArgb(alpha, theme.X)`) → keep, this is
    the sanctioned pattern.
  - **Literal RGB** (`Color.FromArgb(240,240,240)`, `Color.White`, `Color.Gray` in
    `DrawCheckbox`) → replace with theme token + fallback chain
    (`_theme?.CheckBoxBackColor ?? ...`).
  - **Avatar palette / shimmer white** → keep, add a one-line justification comment.
- Special attention: `DrawCheckbox` in `BaseListBoxPainter` uses `Color.White` /
  `Color.FromArgb(240,240,240)` / `Color.Gray` unconditionally — make it theme-aware and
  high-contrast-aware, or delegate to `BeepCheckBoxBool` rendering for visual parity with
  the rest of Beep.

**Done when:** every literal-RGB color in a painter is either tokenized or commented.

### W6 — Layout / virtualization accuracy (MEDIUM)

- `BaseListBoxPainter.DrawItems` computes the PgUp/PgDn hint visibility from a
  **fake** virtual size (`PreferredItemHeight * items.Count`), which is wrong under
  `AutoItemHeight` and rich rows. Use `_owner`'s real virtual size (already tracked by
  the layout helper via `UpdateVirtualSize`).
- Confirm the layout helper's viewport culling (already present, good) stays correct when
  search area height changes — cross-check with Phase 3's scroll-clamp fixes.

**Done when:** PgUp/PgDn hint and scrollbar thumb are correct with mixed-height rows.

### W7 — Phase 8 manual-validation closeout (HIGH)

Execute the matrix Phase 8 left pending. Capture evidence, then flip Phase 8 status to
Completed.

- [ ] Standard / Simple / Outlined screenshots in Dense, Compact, Comfortable.
- [ ] Rich-content screenshots (`Contact`, `ThreeLine`, `Notification`, `Chat`).
- [ ] Effect painter screenshots (`Neumorphic`, `GradientCard`, `Timeline`, `HeroUI`).
- [ ] Grouped keyboard traversal (Up/Down, PageUp/PageDown, Home/End).
- [ ] Hierarchy expand/collapse keyboard parity (← / →, focus visibility).
- [ ] High-contrast selection border, hover layer, focus ring.
- [ ] Disabled-item non-interactive behavior, one painter per family.
- [ ] 125% / 150% DPI baseline for `Standard`, `Contact`, `Timeline`, `Neumorphic`.

---

## Execution order

1. **W1 + W2 together** (text + font are entangled — both touch the same draw methods).
2. **W4** (allocation caching — builds on the font cache from W2).
3. **W3** (ContextMenu swap — isolated, can proceed in parallel).
4. **W5** (color audit — mechanical, after the draw methods stabilize).
5. **W6** (layout accuracy).
6. **W7** (manual evidence — last, validates all prior work visually).

Work batch-by-batch, rebuild after each painter family, keep the diff reviewable.

---

## Files affected (primary)

| File | Workstreams |
|---|---|
| `Painters/BaseListBoxPainter.cs` | W1, W2, W4, W5, W6 |
| `Painters/*.cs` (the ~15 with local `DrawString`/`BeepFontManager`/`new Font`) | W1, W2, W4, W5 |
| `BeepListBox.Drawing.cs` | W1 (empty/empty-search/highlight text) |
| `BeepListBox.Properties.cs` | W3 (`ItemContextMenu` type) |
| `BeepListBox.Events.cs` | W3 (right-click menu build/show) |
| `ListBoxEventArgs.cs` | W3 (`ListBoxContextMenuEventArgs.Menu` type) |
| `Tokens/ListBoxTokens.cs` | W2/W5 (any new font-role or color tokens if needed) |

---

## Verification checklist (Phase 9 exit criteria)

Static gates (must all be 0):
- [ ] `grep -r "DrawString" ListBoxs --include=*.cs` → 0
- [ ] `grep -r "BeepFontManager" ListBoxs --include=*.cs` → 0
- [ ] `grep -r "new Font(" ListBoxs --include=*.cs` (excluding Designer.cs) → 0
- [ ] `grep -r "ContextMenuStrip" ListBoxs --include=*.cs` → 0
- [ ] No `new SolidBrush`/`new Pen`/`new GraphicsPath` inside per-row loops

Behavioral gates:
- [ ] Build: 0 errors, 0 new warnings.
- [ ] Theme switch updates all text (title/sub/group/badge) fonts + colors live.
- [ ] Hover animation over 20 rows shows no allocation growth in the profiler.
- [ ] 1,000-item scroll smooth at 100 / 125 / 150% DPI.
- [ ] Right-click shows a themed `BeepContextMenu`, not a system menu.
- [ ] High-contrast mode: checkbox, selection, focus ring all visible.
- [ ] Phase 8 manual matrix fully captured; Phase 8 status → Completed.

---

## Notes

- This phase is deliberately **mechanical and low-risk per change** but **broad** —
  execute it incrementally by painter family, rebuild between batches.
- The base painter already models the correct patterns (`_cachedTrailingMetaFont`,
  `DrawItemBackgroundEx` contract) — Phase 9 is largely about **propagating those
  patterns** to every painter and helper that predates them.
- Track running status inline here (checkbox list) as work proceeds, mirroring the
  Phase 8 progress-log convention.
