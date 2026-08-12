# Stage 04 — Per-monitor DPI: one scaling story

## The gap

- **No `WM_DPICHANGED` handling anywhere** in the folder. Dragging the form between a 100% and a
  150% monitor leaves the chrome at the old scale: caption height, button glyphs, border widths
  and corner radii all wrong until something else forces a relayout. Commercial suites are
  PerMonitorV2-clean; this is where the difference is most visible on modern multi-monitor setups.
- **Two AutoScale owners.** The ctor writes `AutoScaleDimensions = (96,96)` under a comment that
  says to remove it (`BeepiFormPro.cs:70-73`), while every derived form's designer file
  serializes its own. Depending on construction order this double-applies or no-ops scaling.
  (Deleting the ctor write is stage 01's item; this stage owns proving the end-to-end story.)
- **⚠︎ Corrected after reading the code.** The first draft said metrics "mix raw constants with
  per-site calls, unknowable by reading". That was wrong: `FormPainterMetrics.cs:1420-1450`
  **does** scale centrally — caption height, button widths, icon size, spacing, search box and
  paddings, in one place, guarded by `AreScaleFactorsEqual`.
  The residual hole is narrower and therefore fixable: **6 painters call `DpiScalingHelper`
  directly** (`Linear`, `MaterialYou`, `Modern`, `NextJS`, `RadixUI`, +1). Those are the sites
  where a value can be scaled twice — the exact defect already paid for in
  `CommandListBoxPainter`, where `ScaleValue(MeasureText(...).Width + gap)` inflated a chip at
  150% DPI.

## The work

1. **Handle `WM_DPICHANGED`:** accept the suggested rect, invalidate layout, drop cached
   geometry (border path cache keys on size+style today — add DPI), re-fetch fonts (cache-owned,
   never disposed), repaint. `WM_GETDPISCALEDSIZE` for a clean size negotiation.
2. **Close the painter-side hole.** Metrics already leave `FormPainterMetrics` pre-scaled;
   make that the *only* seam by removing the 6 painters' direct `DpiScalingHelper` calls (each
   either takes a pre-scaled metric or gains one). Then double-scaling is structurally impossible
   rather than individually audited — and the fence below keeps it that way.
3. **Fonts across DPI:** caption/title fonts re-resolved on DPI change through the font cache
   (rule 2: never dispose them).
4. **The AutoScale decision recorded:** designer file owns `AutoScaleDimensions`; base sets only
   `AutoScaleMode.Dpi`; derived forms audited for contradicting serialized values.

## Outcome — ☑ core done; two items recorded open

### The plan's premise was wrong, and the truth was worse

This document's ⚠︎ correction said `FormPainterMetrics` "**does** scale centrally" and that the
residual hole was 6 painters that might scale a second time. Measured:

- `FormPainterMetrics.DefaultFor(style, theme, dpiScaleFactor)` and its cached twin — the complete,
  guarded, 20-line central scaler — had **zero callers anywhere in the repository**. Every real call
  site used `DefaultForCached(style, theme)`, the overload with no DPI in it.
- So metrics were **never** DPI-scaled. The 6 painters calling `DpiScalingHelper` directly were not
  a double-scaling hazard — they were the only painters that scaled **at all**. The other 32 ignored
  DPI completely, and two skins on the same 150% monitor disagreed about whether DPI exists.
- Dead DPI-aware overloads are worse than none: they make the pipeline read as correct.

Routing the form's own 9 metrics fetches through the DPI overload changed **nothing a painter
draws**, because each of the 38 painters had its own `GetMetrics` calling the unscaled overload.
Thirty-eight copies of one lookup is also why the hole stayed invisible: there was no single place
to be wrong in.

### What changed

1. **`BeepiFormPro.Dpi.cs`** (new). `protected virtual float DpiScaleFactor` — one answer to "what
   scale are we at". It is **virtual out of necessity**: `Control.DeviceDpi` is neither virtual nor
   settable (its backing store is a `readonly static` in this runtime), so without this seam the
   scaling pipeline cannot be exercised by any probe — it would be verifiable only by dragging a
   window between two physical monitors by hand.
2. **`internal FormPainterMetrics MetricsFor(FormStyle)`** — the one metrics lookup, DPI-aware.
   All **38 painters** now route through it (31 + 7 of a second signature); zero call
   `DefaultForCached` directly.
3. **`WM_DPICHANGED` handled**: adopt the suggested rect, drop metrics measured at the old scale,
   rebuild layout and window shape, repaint.

### Verified

```
PASS  the metrics a painter receives are scaled for the form's DPI:
        caption 32->48, button 32->48, radius 6->9 at 1.0x -> 1.5x
PASS  a metrics-driven skin's caption responds to DPI: Dracula 40px -> 48px (x1.20)
PASS  WM_DPICHANGED discards metrics measured at the old scale: cache 6 -> 1 entries
```

**No skin's appearance changed.** Caption height was captured for all **37 styles** before and after;
the tables are byte-identical. At this machine's scale `DefaultForCached(…, 1.0f, …)` falls through
to the unscaled path, so the change is inert at 96 DPI by construction — and now measured to be.

### Break-it-first — four breaks

| break | result |
|---|---|
| A. `MetricsFor` drops the DPI argument | **red** — 3 checks |
| B. `WM_DPICHANGED` no longer handled | **red** — the cache check |
| C. one painter bypasses the seam | **red** — that skin stops responding |
| D. a painter re-scales a metric | **STILL GREEN — see below** |
| all restored | green (10 checks) |

**B only became a real check after a first attempt failed to fail.** It originally asserted that the
window adopted the suggested rect — which WinForms' own `base.WndProc` does. Gutting
`HandleDpiChanged` entirely left it green: it was testing the framework, not this form. It now
asserts the one thing only our handler does — discarding metrics measured at the old scale.

### ⚠︎ The double-scale fence is UNPROVEN

`no painter re-scales a value that came from pre-scaled metrics` reports 0 sites and passes. Break D
inserted five genuine `ScaleValue(metrics.ButtonWidth, …)` calls into `DraculaFormPainter` and the
check **stayed green**. The build succeeded and the probe ran, so the fence is not seeing what it
claims to scan; the cause was not found before this stage was written up.

Treat this check as decorative until it has been seen red. It is recorded here rather than removed
because the invariant it encodes is the right one — banning `DpiScalingHelper` outright would also
ban correct code (a painter scaling its **own literal** once is fine; scaling a value that already
came from pre-scaled metrics is the defect, as in `CommandListBoxPainter`).

### NOT done — recorded, with counts

- **16 painters compute `owner.Font.Height + <literal>`** for caption height. The font grows with
  DPI; the literal does not — so those captions are *partially* scaled. Converting them changes each
  skin's appearance wherever the literal and `metrics.CaptionHeight` disagree, so it needs its own
  before/after table across 37 styles. 21 painters take caption height from pre-scaled metrics and
  are correct.
- **6 painters still scale their own literals** via `DpiScalingHelper` (Linear, MaterialYou, Modern,
  NextJS, RadixUI, Shadcn). This is *correct* — scaled once — but it answers to the real monitor
  rather than to `DpiScaleFactor`, so those skins are outside the seam and invisible to any probe.
- **`AutoScaleDimensions` (F5, inherited from stage 01)** — still untouched. It needs an audit of
  every derived form's serialized value, and getting it wrong silently rescales existing forms.
- **A real monitor drag** has not been performed. Everything above is measured through the seam.

## Verification

Real monitor-dragging cannot run in a probe, so the checks target the seams:

- Simulate `WM_DPICHANGED` (message + suggested rect at 144 dpi): caption height, button boxes
  and border width must scale by 1.5× within rounding; layout cache must miss exactly once.
- Double-scale fence: with metrics pre-scaled, grep the painters for `DpiScalingHelper` — the
  count must be **zero**; a source fence in the probe keeps it that way.
- Render at 96 vs simulated 144: differential must show scaling (blindness guard: two renders at
  96 must be identical first).
- Break-it-first: feed the suggested rect but skip the metrics reload — the 1.5× check must go
  red; re-add one painter-side `ScaleValue` — the fence must go red.
- Eyeball: 96/120/144 renders of one skin side by side.

## Exit criteria

`WM_DPICHANGED` handled; metrics pre-scaled with a zero-count fence over painter code; AutoScale
single-owner with the decision written down; checks and break-tests recorded; renders eyeballed.
