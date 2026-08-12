# Stage 02 — One geometry authority; an invalidate that cannot drop the last frame

## Why this is a bug stage, not a refactor

**Two places own the window's shape.** `UpdateFormRegion` sets the managed `Region` from a
rounded-rect path (`Drawing.cs:456-499`); `UpdateWindowRegion` sets a Win32 region via
`SetWindowRgn` (`Win32.cs:490-540`). Both rebuild on every size and style change, in different
code, from the same corner radius. Consequences already paid for in this repo:

- `CustomToolTip` had to abandon `BeepiFormPro` entirely because any silhouette it set was
  overwritten by these two (commit `0dc50a57`) — a derived form cannot own its own shape.
- Region-based rounding produces **aliased corners** (GDI regions have no AA) and **kills the DWM
  drop shadow**, which is the single most visible "not commercial" tell next to DevExpress.

**The invalidate can go stale.** `DebouncedInvalidate` (`BeepiFormPro.cs:54-63`) is
leading-edge-only with no trailing call: the last invalidate of any burst under 16 ms is dropped,
leaving the form one frame stale until something else pokes it. A debounce in a paint pipeline
must be *trailing-edge* (coalesce, then guarantee one final paint).

## The work

1. **Single owner: `FormGeometry`.** One internal component computes the border path, corner
   radii and (when needed) the region — everything else asks it. `UpdateFormRegion` and
   `UpdateWindowRegion` become calls into it; the duplicate math is deleted.
2. **DWM corners on Win11, region fallback elsewhere.** Where
   `DWMWA_WINDOW_CORNER_PREFERENCE` (attribute 33) is available, prefer it: no region at all,
   AA corners, native shadow, snap animations intact. Fall back to the region path on Win10 —
   through the same `FormGeometry`, so callers cannot tell. The DWM plumbing half-exists already
   (`Win32.cs:587-645`).
3. **A derived form can opt out.** `protected virtual bool OwnsWindowShape` (or equivalent): when
   a derivative supplies its own silhouette, `FormGeometry` stands down. This is the API the
   tooltip needed and never had; record it as an added public/protected member.
4. **Fix the debounce.** Coalesce bursts but always schedule the trailing repaint (a one-shot
   timer or `BeginInvoke` when the window is under the interval). Delete per-call-site workarounds
   that exist only to out-shout the broken debounce.
5. **Live-resize correctness.** With geometry in one place, revisit the `OnResize` deferral from
   stage 01: during interactive resize the *cheap* sync (path + clip) runs per-frame; only the
   expensive work (backdrop, region swap) stays deferred to `OnResizeEnd`. The right/bottom
   missing-border artefact that motivated the no-op is the thing the probe must show fixed rather
   than re-hidden.

## Verification

- Shape: silhouette hit-tests (corner outside, edge mid-points inside) against whichever backend
  is active; on Win11 additionally assert no managed `Region` is set.
- Shadow: with DWM corners active, `DwmGetWindowAttribute(DWMWA_EXTENDED_FRAME_BOUNDS)` differs
  from the window rect (shadow present); with a forced region, record that the shadow is absent —
  that asymmetry is the point of the stage.
- Debounce: fire N invalidates 1 ms apart, pump, assert the paint counter advanced *after the
  last call* — break it first by re-instating the leading-edge return and watch it go red.
- Derived-shape: a test subclass sets `OwnsWindowShape` and a diamond region; assert the base
  never overwrites it (the exact `CustomToolTip` failure, now as a regression check).
- Eyeball: one skin rendered at each corner mode, corners compared.

## Exit criteria

One geometry component; both old updaters reduced to delegating shims or deleted; debounce
trailing-edge with the break-test recorded; the derived-shape regression check green; renders
eyeballed. Public members added (`OwnsWindowShape`) and removed recorded here and in the tracker.
