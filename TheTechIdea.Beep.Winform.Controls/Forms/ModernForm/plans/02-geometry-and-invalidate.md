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

## Outcome — ☑ done (items 1–4); item 5 deliberately not done

### Measured before the fix

```
FAIL  a burst of invalidates still produces a paint: 0 paint(s) after 8 invalidates 2ms apart
FAIL  a derived form's own shape survives a resize: 01011 -> 11111
```

A refinement of this document's own claim, worth keeping: **a resize alone did not overwrite the
derived shape.** The first version of the shape check resized and passed, and the defect looked
absent. It took a `FormStyle` change to reproduce — `ApplyFormStyle` calls both region updaters
explicitly, and the setter nulled `Region` before either ran. Had the check been trusted, the plan
would have been marked "not reproducible" while the bug that pushed `CustomToolTip` off this base
was still there.

### What changed

1. **`DebouncedInvalidate` is trailing-edge** (`BeepiFormPro.cs`). The first call in a quiet period
   still paints immediately, so nothing got slower; a suppressed call now widens the pending rect
   (`Rectangle.Union` — dropping either would repaint less than the caller asked for) and arms a
   one-shot 16 ms timer, so the last invalidate of a burst always lands.
2. **One release path** — `ReleaseManagedRegion()`. Four places open-coded
   `var old = Region; Region = null; old.Dispose();`. Guarding only the two *rebuild* methods (the
   first attempt) left the two *setters* still wiping a derivative's silhouette to null before the
   guarded rebuild declined to replace it — **strictly worse than the original bug**: the shape
   became a full rectangle rather than a stale one.
3. **`protected virtual bool OwnsWindowShape`** (added protected member). A derivative that supplies
   its own silhouette keeps it through resizes and style changes. This is the API `CustomToolTip`
   needed and never had.
4. **One geometry authority** — `BeepiFormPro.Geometry.cs`, new file. `ApplyWindowShape()` decides
   the shape once and applies it through a backend; `UpdateFormRegion` / `UpdateWindowRegion` are
   now private helpers it alone calls (verified by grep: zero other callers). The six call sites
   that invoked the pair back-to-back call `ApplyWindowShape()`, which is idempotent — a shape
   whose size/radius/backend has not changed is not rebuilt.
5. **A DWM backend** (`PreferSystemRoundedCorners`, added public member, default `true`). On
   Windows 11 the system rounds the window through `DWMWA_WINDOW_CORNER_PREFERENCE` and **no region
   is set at all** — that is what preserves antialiased corners and the native drop shadow. The
   region path remains for Win10, for per-corner radii, and for radii outside 2–12px, where DWM's
   two fixed radii would render a different design rather than an approximation of this one.
   The form is `FormBorderStyle.None` but adds `WS_SIZEBOX` (= `WS_THICKFRAME`) in `CreateParams`,
   which is what makes it eligible for DWM rounding at all — a plain borderless popup is not.

### Break-it-first — six breaks, all run

| break | result |
|---|---|
| A. `DebouncedInvalidate` back to leading-edge only | **red** — 0 invalidates after the suppressed call |
| B. `ReleaseManagedRegion` ignores `OwnsWindowShape` | **red** — base replaced the Region on `FormStyle` |
| C. `ApplyWindowShape` ignores a derived shape | **red** — 3 checks, managed *and* Win32 halves |
| D. `ShapeBackend` forgets `OwnsWindowShape` | **red** — Win32 half only |
| E. `ShapeBackend` never selects `Dwm` | **red** — 4 checks |
| F. DWM backend leaves a region in place | **red** — both region checks |
| all restored | green (15 checks across stages 01/02/02b) |

C and D going red on *different* checks is the point of having both: D leaves the managed `Region`
alone and only `SetWindowRgn` misbehaves, which reference identity cannot see.

### Three instruments that were wrong before the code was

Recorded because each nearly produced a false conclusion, and the pattern repeats:

- **Counting `Paint` events.** The probe's form lives off-screen and never receives `WM_PAINT`, so
  a paint counter reads 0 no matter what the debounce does. It could not go green; it was not a
  check. Counting `Invalidated` measures the call the debounce actually makes.
- **Hashing the region by sampled points.** The sample points scale with the form, so a region the
  base correctly *left alone* hashes differently after a resize — the diamond was cut for 420×300
  and is merely stale, which is the derivative's business to refresh. That read as "the base
  overwrote it" and nearly bought a second fix for a defect already gone. Reference identity answers
  the actual question: did the base *replace* the region?
- **`Thread.Sleep(2)` between the two calls.** Sleep is a floor, not a duration: on a machine still
  busy from the build it overshot past 16 ms, so the second call landed *outside* the debounce
  window and the leading-edge code invalidated normally. The break came back **green on a loaded
  machine and red on an idle one** — the worst possible failure mode. Back-to-back calls are inside
  the window by construction.

Two blindness guards were added as a result and both must pass: *an idle form does not invalidate
itself* (0 in 150 ms), and *this machine can round windows through DWM* — which **fails**, not
skips, on Windows 10, because there every DWM assertion below it is vacuously true.

### Deleted, not kept: `HasSystemDropShadow()`

Written to verify the stage's headline claim — DWM rounding keeps the shadow a region suppresses —
by comparing `DWMWA_EXTENDED_FRAME_BOUNDS` against the window rect. Measured on build 26200, **both
backends report the frame exactly equal to the window rect (L0 T0 R0 B0), on-screen and off**. The
attribute reports the invisible resize border, not the shadow, and cannot tell the two backends
apart for a borderless `WS_THICKFRAME` form. It answered "no shadow" for both, which is not an
answer, so it was removed rather than kept as a check that always agreed with itself.

**Therefore, stated plainly: the drop shadow and the antialiased corners are NOT measured.** What is
measured is the mechanism — under the DWM backend neither the managed `Region` nor `SetWindowRgn`
is set, and under the region backend both are. That a region hard-clips corners and suppresses the
system shadow is documented Windows behaviour this repo has not independently verified.

### Verified

- 15 checks green across stages 01, 02 and 02b; six breaks each seen red; restored green.
- Renders eyeballed: `02-shape-before/after-style-change.png` show the diamond silhouette surviving
  a Modern→Metro change — only the caption band changes colour, everything outside the diamond stays
  magenta (unpainted). Stage 01's renders re-checked after the refactor: caption and buttons still
  track the 898px width.
- Full library builds clean.

### Public / protected surface

| member | change |
|---|---|
| `protected virtual bool OwnsWindowShape` | **added** — a derivative owns its silhouette |
| `public bool PreferSystemRoundedCorners` | **added** — opt out of DWM rounding, force the region path |
| `internal void ApplyWindowShape(bool force = false)` | **added** — the one geometry entry point |
| `internal enum WindowShapeBackend` | **added** — `Derived` / `Dwm` / `Region` |
| `HasSystemDropShadow()` | **added then removed** — see above; never shipped |

### NOT done — item 5, live-resize correctness

The plan's item 5 (run the cheap geometry sync per-frame during an interactive drag, defer only the
expensive work) is **not done**, deliberately:

- The deferral it proposes to change is what fixed the missing right/bottom border during drags.
  Stage 01 preserved it on purpose; undoing it blind risks trading a fixed artefact for a new one.
- It cannot be verified here. `WM_ENTERSIZEMOVE` only arrives from a real mouse drag, which no probe
  in this repo can produce — the same limit that left stage 01's `DocumentDesigner` rebase open.
- On Win11 the DWM backend removes the per-frame region swap entirely (there is no region), so the
  cost item 5 targets is already gone on the platform where most of it was being paid. What remains
  is Win10 and the per-corner-radius styles.

It stays open in the tracker. A human dragging a derived form's frame on both Win10 and Win11 is
what would settle it.

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
