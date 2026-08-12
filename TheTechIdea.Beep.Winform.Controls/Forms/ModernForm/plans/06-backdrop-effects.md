# Stage 06 — One backdrop API: delete the placebo, keep the DWM one

> **Corrected after reading the code.** The first draft treated backdrops as *intended but
> unbuilt*. They are built — **twice** — and the paint path uses the fake one.

## The defect (F9)

There are **four overlapping controls for one concept**, split across two implementations:

| surface | where | what it actually does |
|---|---|---|
| `BackdropEffect` (`None/Mica/MicaAlt/Acrylic/Blur`) | `Core.cs:948` | applied by `ApplyBackdropEffect` (`Drawing.cs:193`) |
| `Backdrop` (`BackdropType`: `None/Acrylic/Mica/Tabbed/Transient/Blur`) | `Backdrop.cs:28` | applied by `ApplyBackdrop` → real DWM/composition |
| `EnableAcrylicForGlass` | `Backdrop.cs:51` | `TryEnableAcrylic`/`TryDisableAcrylic` |
| `EnableMicaBackdrop` | `Backdrop.cs:74` | `TryEnableMica`/`TryDisableMica` |

**`ApplyBackdropEffect` is a placebo.** It fills the client rectangle with a flat translucent
literal — `Color.FromArgb(20, 255, 255, 255)` for Mica, `(15,0,0,0)` for MicaAlt,
`(40,255,255,255)` for Acrylic, `(10,255,255,255)` for Blur. No blur, no system backdrop, no DWM.
Its own comment admits it: *"Full backdrop effects require Windows API calls and are simplified
here… In a production implementation, this would use DWM API for Mica/Acrylic effects."*

Meanwhile `Backdrop.cs` **does** the real thing: `DwmSetWindowAttribute(DWMWA_SYSTEMBACKDROP_TYPE)`,
`SetWindowCompositionAttribute` with `ACCENT_ENABLE_ACRYLICBLURBEHIND`, blur-behind, gated on
Windows 10 vs Windows 11 build 22000.

So a caller who sets the property they find first (`BackdropEffect`, on the main partial, in the
designer's Appearance category) gets a white wash and concludes the feature is broken; the one who
finds `Backdrop` gets real Mica. Two of the four also fight each other — `EnableMicaBackdrop` and
`Backdrop = Mica` both call into the same DWM attribute independently.

Additionally, this is where **9 of the folder's `Debug.WriteLine` calls** live (F11): every DWM
failure is invisible.

## The work

1. **One property survives: `Backdrop` (`BackdropType`)** — the one backed by real DWM. Record
   the removals in the tracker (public members): `BackdropEffect`, `EnableAcrylicForGlass`,
   `EnableMicaBackdrop`, `ApplyBackdropEffect`, and `BackdropEffect`'s enum if unused elsewhere.
   Per rule 3, they are deleted, not deprecated-and-kept.
2. **Delete `ApplyBackdropEffect` and its call in the paint path** (`Events.cs:87-90`). A
   translucent white wash is not a fallback for Mica — it is a different, wrong effect.
3. **Stated fallbacks, never silent.** `Backdrop` gets an `EffectiveBackdrop` (read-only): what
   the OS actually granted. Requesting Mica on Win10 reports `None` and logs once — the caller can
   ask, instead of guessing from pixels.
4. **Requires stage 02.** A `SetWindowRgn` region defeats the system backdrop and the DWM shadow;
   the DWM corner path must be in place first.
5. **`BeepLog` everywhere** — replace the 9 `Debug.WriteLine`s with `FallbackOnce` (DWM refusal is
   expected on older OSes and must not spam) / `Failure` for genuine errors.
6. **Contrast guard.** Caption ink resolved against the *composited* surface, reusing the WCAG
   pick already shipped in ToolTips and Lovs — a Mica surface under a light theme is exactly the
   "light ink on light frost" failure seen in `GlassToolTipPainter`.
7. **Motion** (smaller than first drafted, since `PaintWithEffects`/`SupportsAnimations`/
   `EnableAnimations` already exist in the paint path): open/close fade and maximize/restore
   easing behind the existing `EnableAnimations` flag, off in design mode, honouring
   reduced-motion.

## Verification

- **One-API fence**: source scan finds no `BackdropEffect` / `EnableMicaBackdrop` /
  `EnableAcrylicForGlass` references anywhere in the library after the sweep.
- **Round-trip**: set each `BackdropType`; read back via `DwmGetWindowAttribute`; assert match on
  a supporting OS, and on refusal assert `EffectiveBackdrop == None` *and* a `BeepLog` fallback
  was reported (probe subscribes to `BeepLog.Reported`).
- **Placebo regression**: assert no paint-path call fills the whole client rect with a flat
  translucent literal — the specific thing being deleted.
- **Contrast**: caption ink vs effective surface ≥ 4.5:1 for each backdrop × {light, dark}
  (blindness guard: a deliberately wrong pairing must fail).
- **Motion**: sample `Opacity` across an animated open — must traverse intermediate values; with
  reduced-motion simulated, must jump to 1.0.
- **Break-it-first**: make `DwmSetWindowAttribute` fail (wrong size argument) and confirm both the
  `EffectiveBackdrop` and the `BeepLog` checks go red without the handling.
- **Eyeball**: contact sheet of backdrops × two themes, on a Win11 host, before/after.

## Exit criteria

One backdrop property with a queryable effective value; the placebo deleted and fenced against;
DWM failures reported through `BeepLog`; contrast green in both themes; motion flag-gated and
reduced-motion aware; break-tests recorded; removals listed in the tracker.
