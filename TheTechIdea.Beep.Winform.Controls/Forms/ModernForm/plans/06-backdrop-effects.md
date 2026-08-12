# Stage 06 — Backdrops: decide who owns the client pixels

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

## The decision this stage actually needs

Who owns the client pixels? This is a product decision, not a cleanup, and it wants the owner's
call before any code moves:

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

## Work that is unconditional (whichever option wins)

> Under the recommended option A, items 1 and 3-4 stand; item 2 (`EffectiveBackdrop`) reduces to
> "the surviving property must not claim an effect it cannot produce".

1. **Collapse four overlapping switches into one.** `BackdropEffect` (`Core.cs:950`), `Backdrop`
   (`Backdrop.cs:28`), `EnableAcrylicForGlass` (`:51`), `EnableMicaBackdrop` (`:74`) all control
   one concept, and two of them call the same DWM attribute independently. One property survives;
   the removals are recorded in the tracker as public-member changes.
2. **`EffectiveBackdrop` (read-only)** — what the OS *and the architecture* actually granted, so a
   caller can ask instead of inferring from pixels. Under option A it reports the painter-level
   effect; under B/C it reports the DWM state, and reports `None` for a skin that paints opaque.
   **This is the property that makes the current failure impossible to repeat**: today the setter
   succeeds and the effect never appears, with nothing to query.
3. **`BeepLog` for the 9 `Debug.WriteLine`s** in `Backdrop.cs` — `FallbackOnce` for expected OS
   refusals, `Failure` for genuine errors.
4. **Contrast guard** on caption ink over whatever surface results, reusing the WCAG pick already
   shipped in ToolTips/Lovs — the `GlassToolTipPainter` "light ink on light frost" failure is the
   same shape.

## Verification

- **The check that would have caught this**: set a system backdrop, render, and assert the client
  pixels are **not** the painter's flat fill — i.e. that the backdrop is actually reachable. Under
  option A this check is inverted and asserts the painter-level tint changed the pixels. Either
  way, *no check may pass on the DWM call returning S_OK* — that is precisely what happens today.
- One-API fence: no references to the removed properties anywhere in the library.
- `EffectiveBackdrop` round-trip: request each kind on a supporting OS and assert the reported
  value matches what is observable; on refusal assert `None` **and** a `BeepLog` report.
- Contrast ≥ 4.5:1 for caption ink over the resulting surface, per theme (blindness guard: a
  deliberately wrong pairing fails).
- **Break-it-first**: make `DwmSetWindowAttribute` fail (wrong size argument) and confirm the
  `EffectiveBackdrop` and `BeepLog` checks go red; and — the important one — re-add an opaque
  full-client fill under option B/C and confirm the backdrop-reachability check goes red.
- Eyeball: backdrops × two themes on a Win11 host, before/after.

## Exit criteria

The pixel-ownership decision recorded here with the owner's ruling; one backdrop property with a
queryable `EffectiveBackdrop` that cannot report success for an invisible effect; DWM failures
reported through `BeepLog`; contrast green; break-tests named — including the opaque-fill
regression if option B or C is chosen.
