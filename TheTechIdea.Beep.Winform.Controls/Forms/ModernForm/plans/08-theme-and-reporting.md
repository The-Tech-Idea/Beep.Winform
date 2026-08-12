# Stage 08 — Reporting, and the colour literals that are *not* skin identity

A sweep, not a redesign. Runs in parallel with anything after stage 02.

> **Scope correction (owner's ruling).** The form painters are **exempt from the literal-colour
> rule**: a skin painter's palette *is* the skin, and each skin's colours match its corresponding
> theme (Dracula painter ↔ Dracula theme, Nord ↔ Nord, GruvBox ↔ GruvBox, and so on). Those
> literals are declarative data, not hard-coded chrome, and sweeping them into theme slots would
> destroy the product's visual identity. Rev 2 of this plan treated all 1,026 as debt; that was
> wrong, and it would have been an expensive mistake to act on.

## Where the literals actually are

Measured, once the exemption made the split matter:

| group | literals | verdict |
|---|---:|---|
| `Painters/*FormPainter.cs` (38 skins) | **566** | **exempt** — skin identity, matches its theme |
| `FormPainterMetrics.cs` | **535** | mostly **exempt** — per-style palette defaults, same rationale; see below |
| `BeepiFormPro.Core.cs` | 17 | **in scope** |
| `Painters/FormPainterRenderHelper.cs` | 7 | **in scope** — shared helper, not a skin |
| `BeepiFormPro.Drawing.cs` | 6 | **in scope** — all in the stage-06 placebo |

So the real debt is **~30 sites**, not 1,026. The rest is the product.

### `FormPainterMetrics.cs` — mostly exempt, with two caveats

Its 535 are per-style palette defaults (`BackgroundColor`, `MinimizeButtonHoverColor`, …) — the
same declarative role as a painter's palette, so the same exemption applies. Two things still
warrant a look, and neither is a colour sweep:

- `Blend`, `Luma`, `EnsureMinLuma` (`:130-160`) *derive* colours at runtime (blend toward white to
  reach a target luma). That is a readability fix computed at paint time. Where a theme already
  publishes the readable pairing, prefer the slot; where it does not, keep the derivation and say
  so. This is a correctness question, not a literal-count one.
- `UseThemeColors = false` is the **default** here (`:130`), so the theme is off unless a caller
  opts in — worth a decision: is the default right for a product whose users expect themes to
  apply? Record either way.

## In scope: the ~30 non-skin literals

- **`Core.cs:1357` — `Color.FromArgb(232, 17, 35)`**: the close button's hover red, hard-coded in
  the *form*, not a skin. Every skin therefore gets Windows' close-red regardless of its palette.
  Semantic → `ErrorColor`, so it follows a dark or high-contrast palette while keeping its meaning.
- **`Core.cs:1505-1590`** — a light/dark ladder of greys for the search box (`255,255,255` /
  `245,245,245` / `60,60,60` / `45,45,45`), its border, icon, placeholder and text, selected by
  `UseThemeColors && _currentTheme.IsDarkTheme`. This is the form re-deriving what the theme
  already publishes; it is also why the search box looks the same under every skin.
- **`Core.cs:1525`** — `?? Color.FromArgb(99, 102, 241) // Default indigo` behind
  `_currentTheme?.AccentColor`: a fallback that fires exactly when there is no theme, which per
  the standing rule cannot happen.
- **`Core.cs:201`** — `_bordercolor = Color.LightGray` as a field initialiser.
- **`Drawing.cs`** — the six in `ApplyBackdropEffect`; **deleted by stage 06**, not swept here.
- **`FormPainterRenderHelper.cs`** — 7 in shared helper code used by all skins; a shared helper is
  not a skin, so these take theme slots or become parameters supplied by the skin.

## The reporting debt (unchanged, and the larger problem)

| pattern | count |
|---|---:|
| bare `catch { }` | **28** |
| `BeepLog` calls | **0** in 26,741 lines |
| `Debug.WriteLine` | 9 (all `Backdrop.cs`) |

`try { ApplyFormStyle(); } catch { }` (`Core.cs:68`) and `try { EnsureLayoutCalculated(); }
catch { }` at four sites mean a form that fails to style or lay out renders wrong and says
nothing. This is the class of defect that cost a full session elsewhere in this repo.

## The work

1. **~30 non-skin literals** → theme slots, by category: semantic (close red → `ErrorColor`),
   surfaces/ink (search box → the theme's textbox/panel slots), and one dead `??` fallback deleted.
2. **28 bare catches → `BeepLog`**, `…Once` in paint/timer paths, keys attributable
   (`form.style:{FormStyle}`, `form.layout`, …). 9 `Debug.WriteLine` → the same.
3. **Decide `UseThemeColors`'s default** in `FormPainterMetrics` and record it.
4. **Write the exemption down where a future sweeper will see it** — a comment at the top of the
   painters folder and of `FormPainterMetrics`, stating that these palettes are skin identity and
   are not to be converted. Without that, the next literal census re-opens this.

## Verification

- **Scoped fence, not a global one.** The literal scan covers `BeepiFormPro.*.cs` and
  `FormPainterRenderHelper.cs` only, and must reach zero there. `Painters/*FormPainter.cs` and
  `FormPainterMetrics.cs` are excluded **by name, in the fence source**, with the exemption quoted
  — an exclusion that isn't written down is a hole. Pattern covers all four escape forms
  (`FromArgb(r,g,b)`, `FromArgb(a,r,g,b)`, named `Color.X`, `?? Color.Empty`), because the
  ListBoxs sweep claimed "zero literals" three times and each form defeated a different pass.
- **Close-button semantics**: render the close button hover under two themes; the fills must
  differ, and both must equal each theme's `ErrorColor` family (blindness guard: two renders under
  the same theme identical first).
- **Search box**: same two-theme differential — this is the visible proof the grey ladder is gone.
- **Reporting**: probe subscribes to `BeepLog.Reported`; force a style-application failure and
  assert exactly one attributable report. Fences: zero bare `catch { }`, zero `Debug.WriteLine`
  folder-wide (these apply to painters too — the exemption is about colour, not silence).
- **Break-it-first**: restore the hard-coded close red and confirm its two-theme differential goes
  red; re-add one bare catch and confirm the fence goes red.

## Exit criteria

The ~30 non-skin literals on theme slots; the skin exemption documented in the code where a
future sweeper will hit it; zero bare catches and zero `Debug.WriteLine`; `BeepLog` keys
attributable; the `UseThemeColors` default decided and recorded; break-tests named.
