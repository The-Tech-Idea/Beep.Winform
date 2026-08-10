# FontManagement — resilience on machines without the theme's font (2026-08)

Reported symptom: deploying to a second PC produced wrong-looking text everywhere and a crash
surfacing from `Wizards/Helpers/WizardHelpers.cs`. Three independent defects, compounding.

## Root causes (all verified in code, all fixed)

### R1 — `IsFontAvailable` was a false positive, so everything collapsed to 9pt Regular

`BeepFontManager.IsFontAvailable` ORed in `EmbeddedFontFamilies` =
`BeepFontPaths.GetFontFamilyNames()` — the 25 families **declared** for embedding. There is no
`Fonts\` folder and **no .ttf anywhere in the repo**; the csproj carries the comment
`<!-- Embed all font files from Fonts folder -->` (line 64) with no `EmbeddedResource` glob under
it. So `IsFontAvailable("Roboto")` returned **true on a clean box** → `ToFont` accepted "Roboto"
and never tried its Arial/Segoe UI candidates → `GetFont("Roboto", 14pt, Bold)` failed every rung →
`GetOrCreateFont` cached `GetUltimateFallbackFont()`, hardcoded **9pt Regular**, *under the
14pt-Bold key*. Every heading, title and body rendered identical 9pt regular text. 17 shipped
themes rest entirely on absent families, so nearly the whole UI collapsed.

### R2 — the crash: cache-owned fonts were disposed by their callers

`WizardHelpers.GetFont(theme, style, fallbackSize, fallbackStyle)` returned a **shared,
cache-owned** `Font` and **ignored its last two parameters**, so `CardsPainter.cs:53-58` asked one
`BodyStyle` for 10f Bold / 8.5f Regular / 12f Bold and got **three references to one instance**,
disposed it up to three times per `ApplyTheme`, and kept drawing with it → `ArgumentException`
inside `OnPaint`, repeating every `WM_PAINT`. R1 is what made it lethal on the second PC: the
collapse funnelled the whole app onto a few shared cache keys, so the wizard's `Dispose()` took
out fonts other live controls held. `ClearFontCache` broke the same contract from the other side
by disposing entries on every DPI change.

### R3 — substitution destroyed character, and was silent

The only fallbacks were Arial/Segoe UI, so monospace theme families (JetBrains Mono, Fira Code,
Source Code Pro) became **proportional Arial**, breaking column alignment. Nothing was ever
logged: bare `catch {}` / `Debug.WriteLine`.

## What changed

**One authority.** Family resolution — including substitution — is now `FontListHelper`'s alone.
`BeepThemesManager.ToFont` and `BeepFontManager.GetFontForPainter` are thin callers.

- **New `FontSubstitutionMap.cs`** — classifies a family (`Monospace`/`SansUi`/`Serif`/`Display`)
  and returns an ordered substitute **chain** (mono → Consolas → Cascadia Mono → Courier New →
  Lucida Console; sans → Segoe UI → Tahoma → …). Name heuristic covers families added later.
- **`FontListHelper`** — `CreateSubstituteFont` (character-preserving, keeps the requested size and
  style, `WarnOnce` keyed on the FAMILY so one missing font logs one line, not 167);
  `GetUltimateFallbackFont(size, style)`; `GetOrCreateFont(cacheKey, family, size, style, factory)`
  reporting **outside** `fontCacheLock`; new public `ResolveFamily` (never null, cache-owned);
  `ClearFontCache` **drops references without disposing** and also clears the validation and
  resolved-family caches.
- **`BeepFontManager`** — honest `IsFontAvailable` (asks `ResolveFamily`); `GetFontForPainter`
  resolves the family **before** building the pixel font and keys the cache by the **resolved**
  name (`new Font(missingName, …)` does not throw, so the old `catch` never fired and a wrong font
  was cached under the right key); `ClearPixelFontCache` drops without disposing.
- **`BeepThemesManager`** — the second `{requested, Arial, Segoe UI}` cascade deleted; all `ToFont`
  paths return cache-owned fonts; `ToFontForControl` and the 4-arg overload no longer pre-empt a
  missing family to "Segoe UI" (that bypassed character-preserving substitution), and the 4-arg
  overload now honours the `fontWeight` it previously discarded.
- **`WizardHelpers.GetFont`** — family from the typography, **size and style from the caller**
  (parameters renamed `size`/`fontStyle`); source weight/underline/strikeout neutralised so they
  cannot re-add Bold to a Regular request. Three distinct requests → three distinct cache keys.
- **Ownership sweep, repo-wide** — 40 `using var` on cache APIs converted, 54 cache-owned field
  disposals removed across ~30 files, driven by an audit that classified every disposed font field
  by **all** its assignment sources: cache-only → stop disposing; `new Font` → **left alone** (27
  fields, correctly owned); mixed → single-sourced (`BeepContextMenu`, `BeepWebHeaderAppBar`).
  `BeepTextBox.IsSystemOrCachedFont` **deleted**: it was named "…OrCached" and never checked the
  cache, so it could not fail for the reason it was written (CLAUDE.md rule 2).
- **Theme sources** — `"Segoe UI Italic"` (a STYLE used as a family; `new FontFamily` throws on it)
  fixed at all 13 sites in 7 files. 8 already declared `FontStyle.Italic`; **5 carried the italic
  intent only in the broken name** and had `FontStyle = FontStyle.Italic` added so the italic is
  not silently lost.

## Verification — FontProbe 20/20

`scratchpad/FontProbe` reproduces "a PC without the theme's font" on any machine by requesting
`"Zz-Probe-Absent-<guid>"`. Blindness guards run first and fail (never skip): `IsFontAvailable`
honest both ways, `.Height` throws on a disposed font, identical requests hit the cache, and a
proportional font measures `i` and `M` differently.

Checks: size+style survive a missing family and the cache is not poisoned; WizardHelpers returns
distinct fonts for distinct sizes and honours both arguments; a monospace request stays monospace
**measured** (`i` width == `M` width → Consolas); a held font survives `ClearFontCache` and a DPI
change; the painter path reports the family it actually uses; substitution reports once per family
(and a second family reports again); CardsPainter's fonts survive two `ApplyTheme` cycles **and so
does a bystander's**; no theme uses a style as a family; no `BeepLog` failure during the run.

**Break-it-first (run, not assumed):**
| break | went red |
|---|---|
| restore the old `WizardHelpers.GetFont` body | C2a, C2b |
| restore disposal inside `ClearFontCache` | C5, C6 |
| make `CreateSubstituteFont` drop size/style | C1a, C1b (`Segoe UI 9pt bold=False` — the reported symptom) |
| before the theme fix | C9 (13 sites) |
| before the ownership sweep | C4c ("the painter disposed a font another control still holds") |

**C4c earned its place.** C4a/C4b passed even with the bug, because the painter re-fetches after
disposing and the cache's `.Height` self-heal rescues *the painter*. Only a **bystander** holding
the same instance sees the damage — which is exactly why the crash appeared far from the wizard.

## Not done / not verified

- Nine requested families (Noto Sans, SF Pro, Ubuntu, Source Code Pro, Sora, Poppins, Cantarell,
  Nunito Sans, Roboto Mono) are absent from the font library and stay substituted — correctly, and
  now logged once each. See "Embedding" below for what was embedded and why the rest was not.
- `ResetFontStoresForReload` still disposes the `PrivateFontCollection`; fonts built from embedded
  families break across a reload regardless of owner. The cache self-heals via the `.Height` probe;
  callers must re-fetch on `ThemeChanged`/`DpiChanged` rather than hold across a reload.
- No visual/PNG pass over the ~8 themes whose typography rests on absent families.

## Embedding — DONE (curated, 2.8 MB)

`TheTechIdea.Beep.Fonts` (Beep.Shared) already holds 575 font files, but it is **163 MB**
(Cascadia 43, Noto Color Emoji 24, Inter 20, Open Sans 18 — all weights + variable + otf/woff2),
so referencing it was rejected. `Controls\Fonts\` now carries a curated **2.8 MB** set: Regular +
Bold static TTFs for the six families the themes request that the library actually has usable
statics for — **Roboto, Inter, Montserrat, JetBrains Mono, Fira Code, Source Sans 3** — with their
OFL licences alongside, embedded by `<EmbeddedResource Include="Fonts\**\*.ttf" />`.

Deliberately excluded:
- **`consolas.ttf` and `Whitney/`** — Consolas is Microsoft-proprietary (and already on every
  Windows box) and Whitney is a commercial Hoefler&Co face. Neither is redistributable.
- **Nunito** (ships variable-only: GDI+ renders just the default instance, so Bold would not work)
  and **Rajdhani** (ships `.otf` only: `PrivateFontCollection` is unreliable with CFF outlines).
- Noto Sans, SF Pro, Ubuntu, Source Code Pro, Sora, Poppins, Cantarell, Nunito Sans, Roboto Mono —
  not in the library at all. All of these keep using `FontSubstitutionMap`, correctly and logged.

Two defects had to be fixed before embedding worked at all — the probe caught both:

1. **Embedded fonts were invisible to `TextRenderer`.** Registration used only
   `PrivateFontCollection.AddMemoryFont` (GDI+). `TextRenderer.DrawText/MeasureText` go through
   **GDI**, which cannot see a private collection — and this library calls TextRenderer in **569
   places**. The embedded JetBrains Mono loaded, reported the right family name, and then measured
   `i`=28 / `M`=73: silently substituted by GDI. Fixed by also registering the same memory block
   with `AddFontMemResourceEx` (gdi32). Now `i`=60 / `M`=60 — the real font.
2. **Optical-size families never matched.** Inter ships statics only as `Inter_18pt-*`, which
   register as `"Inter 18pt 18pt"`, so a theme asking for `"Inter"` matched nothing and was
   substituted despite being embedded. `FindPrivateFamilyByName` now strips trailing `<n>pt`
   groups and prefers the smallest optical size (the UI/body face), and the new public
   `FontListHelper.IsSameFamily` gives `IsFontAvailable` and the probe one tolerant comparison.

Verified by FontProbe C11 (all six resolve to themselves) with blindness guard G5 (a
non-embedded family — Poppins — must still report as substituted, so C11 cannot pass vacuously).

## Superseded: original embedding plan

Add `Fonts\<Family>\<Family>-{Regular,Bold}.ttf` plus `Fonts\LICENSES\<Family>-OFL.txt`, and the
glob under the existing csproj comment at line 64:
`<EmbeddedResource Include="Fonts\**\*.ttf" />`. No loader work is needed —
`BeepFontManager.Initialize` already scans `TheTechIdea.Beep.Winform.Controls.Fonts` and
`FontListHelper` `AddMemoryFont`s each into the `PrivateFontCollection`, which `GetFontInternal`
finds long before substitution. Embed only OFL-1.1/Apache-2.0 families (Inter, Roboto, Roboto Mono,
Noto Sans, JetBrains Mono, Fira Code, Montserrat, Nunito, Nunito Sans, Poppins, Rajdhani, Source
Sans 3, Source Code Pro, Sora, Cantarell). **Never embed SF Pro (Apple) or Segoe UI Variable
(Microsoft)** — they stay in `FontSubstitutionMap` permanently. Ubuntu is UFL-1.0, a distinct
licence. Static faces only: GDI+ renders only the default instance of a variable font. Reconcile
`BeepFontPaths.*` resource paths with the real filenames, then re-run FontProbe and confirm the
substitution warnings disappear for embedded families while SF Pro still reports one.

## Standing rule (added to CLAUDE.md)

A `Font` from `WizardHelpers.GetFont`, `BeepThemesManager.ToFont`,
`BeepFontManager.GetFont/GetCachedFont/GetFontForPainter` or `FontListHelper` is **cache-owned**:
never `Dispose`, never `using`; re-fetch on theme/DPI change. Only a font you built with
`new Font(...)` is yours to dispose — and a field must have exactly one of those two sources.
