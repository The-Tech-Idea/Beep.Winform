# Stage 06 — Backdrops: painted, not composited (**decided**)

**Decision — owner, this session: option A.** DWM system backdrops are dropped; the form keeps one
honestly-named painted effect. The options below are kept as the record of *why*, not as an open
question. Reversing to option C remains possible and its price is written down.

**Ordering note.** This stage used to be blocked on stage 02 ("a `SetWindowRgn` region defeats the
system backdrop") — void now that there is no system backdrop. The property deletions can land
immediately; the painted effect belongs with stage 05's skin descriptor. Stage 02 still owns the
*frame*-level DWM work (corner preference, shadow) that survives the ruling.

> **Rev 3 — the owner asked the question that inverts this stage: "backdrop effect, how can it
> affect my custom background?"** It cannot. Rev 2 said "delete the placebo, keep the real DWM
> one". That was wrong, and acting on it would have deleted the only backdrop code that produces
> a visible change while keeping code that cannot. The measurements below are why.

## The architecture makes a system backdrop invisible

A DWM system backdrop (Mica, Acrylic, Tabbed) is painted by the compositor **behind** the window
and shows only through pixels the window leaves transparent. Three measured facts:

1. **`DwmExtendFrameIntoClientArea` is declared and never called** (`Win32.cs:587` — the only
   occurrence in the folder). Without extending the frame (or a transparent brush + layered
   window), the client area is opaque to the compositor by definition.
2. **The painters fill the whole client rectangle** — 30 of 38 fill `owner.ClientRectangle`
   outright.
3. **36 of 38 painters set `CompositingMode.SourceCopy`** specifically to *replace* pixels.
   `GlassFormPainter` is explicit about why: *"Set compositing mode to SourceCopy to ensure we
   fully replace pixels — this prevents semi-transparent overlays from accumulating on repaint."*
   Even the "Glass" skin composites its own opaque approximation of glass.

So `DwmSetWindowAttribute(DWMWA_SYSTEMBACKDROP_TYPE)` and `DWMWA_MICA_EFFECT` **succeed** — they
return S_OK and throw nothing — and are then covered by the painter on the very next paint. The
API calls are real; the effect is unreachable. Nothing in the code is "broken" in a way a
try/catch would ever reveal, which is exactly why this has survived.

Meanwhile `ApplyBackdropEffect` — the one I called a placebo — is the only backdrop code that
changes what a user sees, precisely because it draws *over* the painted background. It is a poor
imitation (a flat translucent wash, no blur, no sampling of what is behind the window), but
calling it the fake one and the DWM path the real one had it backwards in terms of *effect*.

## The options as they were weighed (history — the decision is A)

Who owns the client pixels? This was the product decision behind the stage:

**Option A — painters keep the pixels (least disruption).**
Accept that true Mica/Acrylic cannot show through an opaque skin. Delete the DWM backdrop path
(or narrow it to the window frame/shadow only), and keep a *painter-level* effect that is honest
about what it is: a tint/gradient/noise layer the skin composites itself. `BackdropEffect` becomes
a skin feature, correctly named, with the theme supplying the tint.
*Cost:* no real Mica, ever. *Gain:* one coherent model; all 38 skins keep working unchanged.

**Option B — the compositor owns a region (true Mica).**
`DwmExtendFrameIntoClientArea` with `MARGINS { -1 }`, a transparent form brush, and painters that
**skip** filling the backdrop region (or fill with `Color.Transparent` under `SourceCopy`), so the
system backdrop shows through. Caption/borders/content still painted normally on top.
*Cost:* touches all 38 painters — they must learn "do not paint here"; interacts with stage 02's
region work (a `SetWindowRgn` region also defeats the backdrop) and with `AllowTransparency`
limitations in WinForms. *Gain:* genuine Win11 Mica/Acrylic, matching DevExpress's newer skins.

**Option C — hybrid, per skin.**
A skin declares `SupportsSystemBackdrop`. Those that do (Glass, Glassmorphism, Acrylic-like,
Mica-like — a handful) leave the backdrop region transparent; the other ~34 stay opaque and ignore
the setting. Requires stage 05's skin descriptor to exist first, so the capability is declared
rather than guessed.
*Cost:* two paths to maintain, but only the skins that want it pay. *Gain:* real Mica where it
matters, no churn where it does not.

## Recommendation: **A** — and the owner's instinct ("not practical in BeepiForm") is right

Rev 3 recommended C. On the evidence assembled since, **A** is the better call, and the reasoning
is recorded so nobody re-opens it on aesthetics:

1. **It fights the architecture's central premise.** The painter model is "one painter owns every
   pixel of this form" — 36 of 38 painters set `SourceCopy` *deliberately*, with a comment
   explaining it stops translucent layers accumulating across repaints. A system backdrop needs
   the opposite contract: leave pixels alone and let the compositor fill them. Options B/C ask 38
   skins to invert their own rule for one effect.
2. **WinForms makes the transparent path expensive.** Real per-pixel transparency means a layered
   window; `AllowTransparency` is colour-key based, disables ClearType, and interacts badly with
   child controls — and this form is explicitly a *container* with docked children. The cure would
   degrade text rendering in every hosted control to gain a background effect.
3. **It only shows on some machines.** Mica needs Win11 build ≥ 22000; acrylic Win10+. The code
   already gates on `Environment.OSVersion.Version.Build >= 22000`. Two rendering paths through 38
   skins, for an effect a large share of users never see, is a poor exchange.
4. **The skins already simulate it, and the simulation is portable.** `GlassFormPainter`
   composites its own glass; `Glassmorphism`, `NeoMorphism` and `Holographic` do the same. A
   painted approximation looks identical on Win10, over RDP and in a screenshot — where a real DWM
   backdrop degrades or vanishes.
5. **Commercial suites mostly do the same.** DevExpress and Telerik WinForms skins overwhelmingly
   paint their own surfaces rather than depend on DWM backdrops, for reasons 2-4.

**What is given up, stated plainly:** true Mica/Acrylic — the effect where the *actual desktop
content behind the window* blurs through and shifts as windows move. A painted layer cannot
reproduce that. If it ever becomes a requirement this decision reverses to C, at the cost of the
38-painter contract change described above.

### What A means concretely

- Delete the DWM **backdrop** path (`TryEnableMica`, `TryEnableAcrylic`, blur-behind, the
  `DWMWA_SYSTEMBACKDROP_TYPE` / `DWMWA_MICA_EFFECT` calls) and the properties fronting it —
  `Backdrop`/`BackdropType`, `EnableAcrylicForGlass`, `EnableMicaBackdrop`. They cannot work while
  a painter owns the pixels, and shipping a property that silently does nothing is the defect.
- **Keep the frame-level DWM calls, which work regardless of client opacity** — corner preference
  and shadow (stage 02) and `DWMWA_NCRENDERING_POLICY`. Those affect the frame, not the client
  area, so the opacity argument does not apply to them.
- Keep **one** honestly-named painted effect. `BackdropEffect` survives, renamed to say what it is
  (`SurfaceOverlay` / `SurfaceTint`), and stops being a flat literal wash: the tint comes from the
  theme, and skins wanting depth (Glass, Glassmorphism) render a real gradient/noise layer instead
  of `FromArgb(20,255,255,255)`.
- `EffectiveBackdrop` is no longer needed for DWM negotiation, but its *principle* stands: no
  property may report success for an effect that cannot appear.

## The work (option A)

1. **Delete the DWM backdrop path.** `TryEnableMica`, `TryEnableAcrylic`, `TryDisableMica`,
   `TryDisableAcrylic`, the blur-behind call, `ApplyBackdrop`, and the
   `SetWindowCompositionAttribute` / `ACCENT_STATE` / `WINDOWCOMPOSITIONATTRIBDATA` plumbing that
   exists only for them. **Keep** `DwmSetWindowAttribute` itself and the frame attributes stage 02
   uses (corner preference, shadow, `DWMWA_NCRENDERING_POLICY`) — those work regardless of client
   opacity.
2. **Collapse four public switches to one.** Removed: `Backdrop` (`BackdropType`),
   `EnableAcrylicForGlass`, `EnableMicaBackdrop`. Kept and renamed: `BackdropEffect` →
   `SurfaceOverlay` (name to confirm), because what survives is a painted surface treatment, not a
   **`BackdropType` has a consumer outside this folder — checked, not assumed:**
   `Managers/BeepFormUIManager.cs:135` exposes `public BackdropType? Backdrop` and applies it at
   `:397` (`_beepiForm.Backdrop = Backdrop.Value`). It is *not* on `IBeepFormUIManager`, so the
   interface is unaffected, but the manager property and its assignment go with the form's, and
   `Forms/Effects/BackdropType.cs` is deleted only once both are gone.
   **All removals recorded in the tracker as public-member changes** — a breaking change for any
   caller setting them today, and the fact that those callers currently get *nothing visible*
   does not make the break invisible to a compiler.
3. **Make the surviving effect real.** `ApplyBackdropEffect` currently fills
   `Color.FromArgb(20,255,255,255)` and similar — a flat literal wash that ignores the theme
   entirely. Replace with a theme-sourced treatment: tint from the theme's surface/overlay slot,
   and for skins that want depth (Glass, Glassmorphism, NeoMorphism, Holographic) a real
   gradient/noise layer rendered by the skin. Under stage 05 this becomes a skin-descriptor
   capability rather than a form-level switch.
4. **`BeepLog` for the 9 `Debug.WriteLine`s** in `Backdrop.cs`. Most of them disappear with the
   code they report on; whatever survives reports through `BeepLog` — `FallbackOnce` for expected
   OS refusals on the frame attributes, `Failure` for genuine errors.
5. **Contrast guard.** Caption ink resolved against the *painted* surface (base + overlay),
   reusing the WCAG pick already shipped in ToolTips and Lovs. `GlassToolTipPainter` shipped light
   ink on a light frost for exactly this reason; the overlay makes the same mistake available here.
6. **Motion stays modest.** `PaintWithEffects` / `SupportsAnimations` / `EnableAnimations` already
   exist in the paint path — open/close fade and maximize/restore easing go behind the existing
   flag, off in design mode, honouring reduced-motion.

## Verification

- **One-API fence**: no references to `Backdrop`, `BackdropType`, `EnableMicaBackdrop` or
  `EnableAcrylicForGlass` anywhere in the library after the sweep (including derived forms and
  designer-serialized files — a stale `.Designer.cs` assignment is a compile break, not a warning).
- **The overlay actually does something**: render with the effect off and on; the pixels must
  differ (blindness guard: two renders with it off must be identical first).
- **The overlay follows the theme**: same effect under two themes must produce different tints —
  this is the check the current flat literal would fail, and the reason it is worth writing.
- **Frame DWM survives**: corner preference and shadow still apply with the backdrop path deleted
  (stage 02's silhouette/shadow checks cover this; assert they stay green after the deletion).
- **Contrast**: caption ink vs the composited painted surface ≥ 4.5:1, per theme.
- **Motion**: sample `Opacity` across an animated open — intermediate values present; with
  reduced-motion simulated, straight to 1.0.
- **Break-it-first**: restore the flat literal wash and confirm the theme-tracking check goes red;
  delete the overlay call and confirm the on/off differential goes red.

**Retired, and recorded so it is not re-added:** the DWM attribute round-trip and
`EffectiveBackdrop`-on-refusal checks. They test a negotiation that no longer happens — and the
round-trip check **would have passed today**, because `DwmSetWindowAttribute` returns S_OK for an
effect the painter then covers. A green check over an invisible feature is the exact failure this
stage documents.

## Exit criteria

The DWM backdrop path and its three redundant properties deleted, with the removals listed in the
tracker; one surviving painted effect whose tint comes from the theme and demonstrably changes
pixels; frame-level DWM (corners, shadow) unaffected; `Debug.WriteLine` gone from `Backdrop.cs`;
contrast green in both themes; break-tests named. If anything here proves impossible, the decision
record above is the thing to revisit — not the code quietly.
