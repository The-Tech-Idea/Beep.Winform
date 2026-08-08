# Steppers — review & enhancement plan (2026-08)

43 files, ~8,000 lines. `BeepStepperBar : BaseControl` (in `BeepSteppperBar.cs` — note the
long-standing filename typo) + `BeepStepperBreadCrumb`, 14 style painters + NoOp over
`IStepperPainter` with a registry, helpers for theme/font/icon/style/accessibility, a
`StepperColorConfig` custom-override model.

Entry motive: the user reports **alignment issues in stepper styles** seen via the wizard forms —
this folder's painters are the likely source, so the probe leads with per-painter renders and
alignment checks, not just distinctness.

## Findings (static pass)

### F1 — `StepperThemeHelpers` has the full anti-pattern, third folder running

`useThemeColors` flag, `!= Color.Empty` guards, literal fallbacks — and worse than the previous two:
**reflection-based theme property lookup** (`GetProperty(...).GetValue(theme)`) per colour, per
paint. Same fix as Calendar/VerticalTables (the settled end-state): theme = supplied ??
`BeepThemesManager.CurrentTheme`, one slot one return, no flag, no guards, no reflection.

### F2 — ~46 literal colours

Helpers (17), painters (~23 across 10 files), `BeepSteppperBar` (3), `StepperColorConfig` (3 — check
whether these are custom-override *defaults*, which should be `Color.Empty`, not a palette).

### F3 — 4 empty swallows

`StepperAccessibilityHelpers` ×2, `StepperFontHelpers` ×1, `StepperThemeHelpers` ×1 → BeepLog per
site.

### F4 — no probe; alignment unverified (the user's actual complaint)

Planned probe (StepperProbe):
1. Render ALL 14 painters at two widths (wide + narrow) with 4 steps, one completed/current/pending/
   error; blank-guard + aliased-style distinctness.
2. **Alignment checks, mechanical**: for each painter, every drawn text/glyph pixel must lie inside
   the control bounds (no off-edge bleed — the wizard Cards/Horizontal lesson); node centres evenly
   spaced (max deviation ≤2px); label column x-alignment for vertical painters.
3. Theme responsiveness: one painter under two themes → different pixels.
4. State round-trip: CurrentStep set → StepChanged raised once; click a node → navigation (if the
   control supports it) via the real pipeline (cursor parked — the VerticalTables lesson).
5. Eyeball EVERY painter render individually — this is the complaint; distinctness checks alone
   don't see misalignment.

### F5 — spacing, sizing and alignment revision, ALL 14 painters (user directive)

Not just probe checks — a per-painter geometry pass with the rules that fixed the wizard forms:

1. **Every text rect sized from its font** (`font.Height + pad`), never a constant. A constant rect
   clips any taller font silently — DrawText clips to its rect (Minimal title, HStepper labels).
2. **Bands sized from their content stack**, top-anchored: sum of rows (glyph row + label row +
   description row + gaps), not a guessed height that content overruns (HStepper 100→112).
3. **Node spacing computed from available width**: first/last nodes inset by half a node + label
   half-width so edge labels never bleed past control bounds; centres evenly spaced; label rects
   clamped to bounds (HStepper's label ran 10px off the form edge).
4. **No collisions between fixed chrome and flowing content** — counters/badges own their row or are
   anchored opposite the flow (the chip-on-circle collision, twice).
5. **Vertical painters**: one shared left gutter width for node column, labels x-aligned to a single
   column edge; connector lines centred on node centres, not rect edges.
6. **DPI**: every constant through DpiScalingHelper; no raw pixel offsets.
7. Paddings consistent across painters (same outer inset per orientation) so switching styles does
   not shift content.

Each painter gets: geometry read → rules applied → render at wide/narrow → eyeball → next. The probe's
mechanical checks (F4.2) then hold the line against regressions.

## Order

1. F3 swallows + F1 helpers rewrite (mechanical, settled pattern) — build + commit
2. F2 painter sweep to slots — build + commit
3. F4 probe + F5 geometry pass painter by painter; render-eyeball all 14 — commit per fix batch
4. Filename typo `BeepSteppperBar.cs` → rename only if the user wants the churn (git mv, all refs)

## Batch 1 done — swallows + helpers rewrite (build 0 errors)

F3: the 4 empty swallows report (StepperAccessibilityHelpers ×2 WarnOnce a11yColors/a11yName,
StepperFontHelpers WarnOnce fontReflect, StepperThemeHelpers — see below, its swallow died with the
machinery).

F1: `StepperThemeHelpers` rewritten to the settled end-state — 370 lines → 92. The reflection probes
targeted **phantom property names** (`StepperCompletedColor`, `StepperConnectorPendingColor`, …) that
do not exist on `IBeepTheme`; they had never hit once, and every call fell through to the hardcoded
Tailwind palette. Mapping now (real `Stepper*` family, one slot one return, `theme ??
BeepThemesManager.CurrentTheme`, no flag):

| getter | slot |
|---|---|
| Completed fill | `StepperItemCheckedBoxBackColor` |
| Active fill | `StepperItemSelectedBackColor` |
| Pending fill | `DisabledBackColor` |
| Error / Warning fill | `ErrorColor` / `WarningColor` |
| Connector | completed → CheckedBoxBack, else `StepperBorderColor` |
| Step text (on-node ink) | active → SelectedFore, completed → CheckedBoxFore, else `StepperItemForeColor` |
| Label | active → `StepperForeColor`, else `DisabledForeColor` |
| Background | `StepperBackColor` |
| Border | active → SelectedBorder, error → `ErrorColor`, else `StepperItemBorderColor` |

`customColor` stays: an explicit caller override is data (`Color.Empty` falls through to the slot).
144 callsites swept to the flag-less signatures across both controls, 3 partial files, 14 painters.

**Deleted, not kept** (rule 2): `ApplyThemeColors(dynamic)` + its `HasProperty`/`GetPropertyValue`
reflection — it wrote theme colours INTO `BeepSteppperBar`'s custom-override fields, so after one
application every themed value looked like an explicit override and no later theme change could land.
The fields stay `Color.Empty` and per-paint resolution (which already existed at all paint sites) is
the only path. `GetThemeColors` tuple had zero callers — deleted.

Not verified yet: renders (batch 3's probe); `UseThemeColors` still gates high-contrast accessibility
application in `BeepSteppperBar` — F2 decides.

## Batch 2 done — literal sweep to slots (build 0 errors)

All ~44 literals resolved; census after the sweep finds **zero** (one survivor: the WCAG
luminance contrast pick Black/White in `StepperAccessibilityHelpers:385` — the accepted
contrast-ink idiom, same as Calendar's `GetEventInk`).

The worst find: painters passed `Color.White` **as the `customColor` override** to the theme
helpers (10 sites) — the override always wins, so those calls had been hardcoding White through
the override channel and the slot never resolved once. All dropped; the slot decides.

- Selected/active ink ternaries (`selected ? Color.White : …`) → `GetStepTextColor(theme,
  Active)` (3 painters); checkmark on completed fill → CheckedBoxFore ink; gradient chevron
  text → state ink.
- Hover veils `FromArgb(α, White)` → `FromArgb(α, StepperItemHoverBackColor)` — same alpha,
  theme-driven (10 painters).
- Error badge: ring → `StepperBackColor` (separates badge from node in the control's own
  background), count ink → `OnPrimaryColor`.
- `?? Color.Gray` / `?? Color.White` null-theme guards dropped (3 sites) — always a theme.
- `GetHighContrastColors()` non-HC branch returned the Tailwind palette → now theme slots; the
  HC branch keeps SystemColors (the OS accessibility palette is correct there).
- `GetIconColor` rewritten flag-less (was the same anti-pattern + both callers passed the
  always-winning White override).

**Deleted** (rule 2): `StepperColorConfig` (zero consumers anywhere — dead model);
`PaintStepIcon` (zero callers); `StepPainterContext.UseThemeColors` (zero readers after the
sweep); `ApplyHighContrastAdjustments`' never-read theme/flag params; and
`BeepStepperBreadCrumb`'s **shadow** `UseThemeColors { get; private set; }` — stuck false
forever, it hid `BaseControl`'s real property and made the breadcrumb's themed path unreachable
for its whole life.

Known residue for batch 3's probe to watch: high-contrast application writes system colours into
the custom-override fields, so *leaving* HC mode won't restore themed colours until restart —
noted, not redesigned here.

## Batch 3 done — probe 70/70, geometry + defects found by render (the eyeball earned its keep)

StepperProbe (scratchpad): all 12 painters wide+narrow, blank-guard + cross-painter distinctness,
mechanical alignment (node centres evenly spaced ≤2px, rects in bounds, top/bottom edge-bleed rows),
vertical orientation column alignment, theme responsiveness, PainterName switch repaint, CurrentStep
→ StepChanged, pipeline click navigation, StepValidating.Cancel gate, breadcrumb with real items.

**The colour-count checks passed while every painter rendered ONE node** — only the eyeball caught
it. Root cause, the repo's documented re-entrant sync disease: `StepCount = 4` → InitializeSteps →
SyncStepsWithListItems rebuilds ListItems, the FIRST Add fires ListChanged synchronously →
SyncListItemsWithSteps adopts `stepCount = ListItems.Count` (=1) → the rebuild loop's own bound
collapses. Every stepper constructed via StepCount ended up with one step; CurrentStep's guard then
rejected every set, so StepChanged never fired and my first "click navigates" PASS was vacuous
(CurrentStep was 0 all along). Fix: `_syncingListItems` guard — our own writes don't re-enter
(SetStepState's IsChecked write guarded too).

Defects found by render + fixed:
- **PainterName was an auto-property** — assigning it after construction painted the old style until
  something else re-ran InitializePainter. Both controls: setter now re-initializes (attributes kept
  on the property, not the backing field).
- **VerticalTimeline + AlternatingTimeline overflowed the band**: fixed 52px pitch centred in a
  140px control put node 1 at Y=-24 (clipped AND unclickable). Now: inset + spacing compresses +
  nodes shrink when even min spacing cannot fit; cards clamped to the content rect.
- **Completed check never painted**: GetCheckIconPath's "try paths" loop returned the first
  non-empty STRING ("check.svg") — never a resolvable resource; StyledImagePainter silently no-oped
  and every completed node was an empty circle. All five icon getters now return SvgsUI constants.
- **On-fill ink**: GetStepTextColor gained Error/Warning → OnPrimaryColor (the "4" marker was
  dark-red-on-red in 8 painters); chevron segment text, breadcrumb segment text and both timeline
  cards moved from label ink (pairs with control bg) to on-fill ink. GetStepOnFillColor briefly
  existed and was folded back in — one concept, one name.
- **Distribution (F5 rule 3)**: CircularNode/ProgressBar/IconTimeline/SquareDashed/BadgeStatus
  clustered a 4-step bar in the middle of a 900px control at the recommended ~20px pitch, labels
  touching. Horizontal layouts now distribute across available width with a label-safe edge inset
  (56 scaled) and an 8px floor. Dots stays compact deliberately — pagination idiom. Owner plumbed
  into the three painters that discarded it (DPI rule 6).
- **CompactInline underline** sat at the control's bottom edge, a band-height away from the label it
  marks — now under the label, sized to it.
- **Breadcrumb doubled every label** ("Account"/"Account"): Name and Text carry the same string by
  SimpleItem convention here; equal Subtitle is now dropped (painter path + legacy path).
- 9 `?? Color.DodgerBlue` focus-pen fallbacks the batch-2 census regex missed (named-colour census
  now clean); 3 Debug.WriteLine → BeepLog.FailureOnce.

Not done / open: vertical-orientation distribution for the node painters (horizontal only was
in-scope); high-contrast override-field freeze from batch 2 still noted; `BeepSteppperBar.cs`
filename typo still pending user's call on churn.

## Standing constraints

There is ALWAYS a theme — assign slots directly, never guard, never blend, never literal (semantic
states from semantic slots). Text rects sized from fonts, not constants. A check must be able to
fail. Commit to master only.
