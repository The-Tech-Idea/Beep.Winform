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
