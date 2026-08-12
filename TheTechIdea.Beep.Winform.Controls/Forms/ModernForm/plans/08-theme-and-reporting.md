# Stage 08 — Themeable chrome, and failures that report

A sweep, not a redesign. It can run in parallel with anything after stage 02, and its fences
protect every later stage from re-importing what it removes.

## The numbers (folder-wide census)

| pattern | count | meaning |
|---|---:|---|
| `Color.FromArgb(r,g,b)` literals | **1,026** | the largest concentration in the repo — ListBoxs, the previous record, had 110 |
| named `Color.X` literals | 69 | `Color.White` pens for glyphs, `Color.Black` shadows |
| luminance shifts / blends | 73 | `ShiftLuminance`, `Lighten`, `Darken` — each an invented colour |
| `useThemeColors` sites | 71 | the flag disease: theme reachable only when a boolean says so |
| bare `catch { }` | **28** | rule 1 violations |
| `BeepLog` calls | **0** | in 26,741 lines |
| `Debug.WriteLine` | 9 | all in `Backdrop.cs`, hiding DWM failures |

At this density, a theme cannot reach most of the chrome: the theme is *consulted* where
`UseThemeColors` is true and a slot happens to be used, and *ignored* across a thousand hard-coded
values. That is what makes the skins look inconsistent under a dark theme, and it is invisible to
reading because it is spread over 38 files.

## Why this is two rules, not one stage of tidying

- **Rule 4 (nothing assigns colours).** Chrome that hard-codes `FromArgb(232,17,35)` for the close
  button's hover (`Core.cs:1357`) cannot follow a theme, a high-contrast mode, or a customer's
  brand — the three things enterprise buyers ask for first.
- **Rule 1 (never swallow).** 28 silent catches include `try { ApplyFormStyle(); } catch { }`
  (`Core.cs:68`) and `try { EnsureLayoutCalculated(); } catch { }` at four sites: if styling or
  layout fails, the form renders wrong and says nothing. This is precisely the class of bug that
  cost a full session elsewhere in this repo.

## The work

1. **Categorise before replacing.** The ListBoxs sweep is the model, including its lesson: a blind
   regex replace produced `Theme.X(30,30,30)` and a claim of "zero literals" that was wrong
   **three times** (a 4-argument form, a numeric-alpha form, and `?? Color.Empty` each escaped a
   different pass). Categories here:
   - glyph ink on a filled button → `OnPrimaryColor` / a WCAG pick between two theme slots
   - shadows → alpha veil over `ShadowColor`
   - borders / dividers → `BorderColor`
   - caption surface → the caption/panel slots
   - semantic (close-button red, error states) → `ErrorColor` — meaning preserved through the
     semantic slot, not a hue
   - decorative skin palettes (Cyberpunk neon, Dracula, GruvBox, Nord…) → **these are the skin's
     identity**; they move into the skin descriptor of stage 05 as declared data, not into theme
     slots. Deleting them would destroy the product.
2. **Retire `UseThemeColors`** where it gates a theme lookup that should always happen; keep it
   only if it means something a caller genuinely chooses (record the decision either way).
3. **28 catches → `BeepLog`**, `…Once` in paint/timer paths, keyed so a failing style or layout
   names itself; 9 `Debug.WriteLine` → the same.
4. **Luminance shifts**: keep where a skin *is* a gradient (Neumorphic, Glassmorphism), replace
   where a shift is standing in for a missing slot (hover/pressed states have real slots).

## Verification

- **Fence, with the right pattern.** A literal-colour scan that includes *all four* forms that
  escaped previously: `FromArgb(r,g,b)`, `FromArgb(a,r,g,b)`, named `Color.X`, and
  `?? Color.Empty`. Count must reach the agreed floor (skin-identity palettes excepted **by name**,
  listed in this file — an exception that is not enumerated is a hole).
- **Theme responsiveness, per skin**: render each skin under two themes; assert the renders differ
  (blindness guard: two renders under the *same* theme must be identical first). This is the check
  that actually proves a theme reaches the chrome — a literal count does not.
- **Reporting**: probe subscribes to `BeepLog.Reported`; force a style-application failure and
  assert exactly one report with an attributable key. Fence: zero bare `catch { }` and zero
  `Debug.WriteLine` in the folder.
- **Break-it-first**: restore one literal in a swept painter and confirm the per-skin theme
  differential goes red for that skin; re-add one bare catch and confirm the fence goes red.

## Exit criteria

Literal count at the agreed floor with skin-identity palettes enumerated by name; every skin's
render provably theme-responsive; zero bare catches and zero `Debug.WriteLine`; `BeepLog` keys
attributable; break-tests recorded. Any `UseThemeColors` removal recorded as a public-member
change in the tracker.
