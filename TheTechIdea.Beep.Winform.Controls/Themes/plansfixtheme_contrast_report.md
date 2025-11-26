# Theme Contrast Report (Heuristic)

This report summarizes a quick contrast assessment for the themes whose palettes are summarized in `plansfixtheme.md`. Because the environment is not running a scripting interpreter, I used a conservative luminance-based heuristic and the WCAG targets (4.5:1 for normal text / 3:1 for large text) to spot likely contrast issues.

This report includes:
- Summary of tokens checked for each theme
- Heuristic PASS/FAIL on those token pairs where contrast may be an issue
- Concrete, actionable fixes and suggested color replacements (where possible) to reach a >=4.5:1 or >=3:1 ratio.

---

## Methodology (heuristic)
- Tokens checked per theme:
  - ForeColor on Background
  - OnPrimaryColor on PrimaryColor
  - AppBarTitleForeColor on AppBarBackColor
  - BorderColor on Background
  - LinkColor on Background (for interactive text)
- As a fast heuristic I checked brightness/difference using linear-luminance-like approximation. This is conservative but will flag all likely problematic combinations.
- Where a token pair is **likely to fail** (soft gray on white, pastel on pastel, or subtle contrast on dark surfaces), I gave a corrective recommendation.
- If you'd like, I can run exact WCAG luminance calculations and provide *exact* contrast ratios and computed replacement hex values — I can do this if we can run Python in your environment or you allow me to run the small script I previously created.

---

## Quick Summary (probable problem highlights)
- DefaultBeepTheme — PASS in general, but: `CaptionStyle`, `SubLabelForColor`, and similar muted gray (e.g., C(107,114,128) / #6B7280) on standard white surfaces are likely below AA (4.5:1) for normal text. Action: darken muted gray to a darker gray (~#555B63 or #4B5563) or use larger font weight/size to meet contrast.

- ModernTheme — solid; most tokens are fine (black on near-white or blue on white). Some lighter hover grays may be borderline.

- TerminalTheme — PASS; high-contrast neon green on dark backgrounds is good.

- ArcLinuxTheme — caution: `ForeColor` (211,218,227) on `Background` (56,60,74) might be borderline (text is relatively light but background is moderately dark). Large text probably passes, small text might not. Action: slightly lighten `ForeColor` (or darken background) to increase contrast ~10-20%.

- BrutalistTheme — PASS (black on white, high contrast).

- CartoonTheme — PASS for main text tokens (`ForeColor` on background). AppBar buttons and subtler accents might need visual testing for small labels.

- ChatBubbleTheme — PASS (black on light cyan background is good).

- CyberpunkTheme — PASS (neon cyan text on very dark background is high contrast), but some neon `BorderColor` used as subtle separators may be too bright or too similar; this is a matter of design, not contrast failure.

- DraculaTheme — PASS for main text. Muted light accent colors (e.g., `PanelGradiantMiddleColor`) OK; check small secondary texts with light gray.

- FluentTheme — PASS in general; check lighter hover states and small text colors for AA compliance.

- GlassTheme — PASS for main text tokens; check border vs background contrast (border might be soft and low contrast).

- GNOMETheme — PASS but double-check `InactiveBorderColor` and subtle grays used in small text on white surfaces.

- GruvBoxTheme — PASS for main text; keep an eye on `SecondaryColor` used in small text on dark surfaces — it may be too dim; tends to be warm neutrals and should be OK for bold labeling but not small text.

- HolographicTheme — PASS in general; check any pastel/light border on dark surface (some pastel colors can be borderline). Also large gradient colors might hamper reading of small text — check foreground vs the gradient used in panels.

- iOSTheme — PASS; Apple uses strong contrast; check `SubTitle` gray in some contexts.

- KDETheme — PASS; check `BorderColor` on `Background` for visual accessibility of controls.

- MacOSTheme — PASS (similar to iOS), keep an eye on `BorderColor` in very light UI combos.

- Metro2Theme & MetroTheme — PASS; Windows-style tokens are usually tuned. Double-check `BorderColor = #0078D7` on white for small 1px lines (visibility rather than luminance) 

- MinimalTheme — PASS for high contrast, but `AccentColor` used as near-white or for small text may fail.

- NeoMorphismTheme — PASS; typical shallow shadows and low-contrast tokens should be reviewed for small text and small icons.

- NeonTheme — PASS; neon on dark tends to be high contrast; watch for cyan on very dark panels (should be fine).

- NordicTheme, NordTheme, OneDarkTheme, PaperTheme, SolarizedTheme, TokyoTheme — mostly PASS for typical text colors; some subtle gray tokens used for captions (e.g., `SubLabelForColor`) may be below 4.5:1 and require darkening.

- UbuntuTheme — PASS in general; watch `SubLabel` and some subtle grays on white surfaces.

---

## Concrete Fixes & Suggestions (applied across themes)
Below are practical changes you can apply universally or per-theme.

1) Muted gray on white surfaces (common failing scenario)
- Symptoms: Text uses mid-gray (e.g., RGB ~ (107,114,128) / #6B7280) on white/surface backgrounds; small caption text fails AA 4.5:1.
- Fix: Darken the color by about 30-40% or adjust to a darker gray (e.g., #6B7280 -> #4B5563 or #464F5A) until contrast hits 4.5:1.
- Example: DefaultBeepTheme `CaptionStyle` (#6B7280) -> Recommend `#4B5563`.
- Implementation: use your theme helper `Darken(Color, amount)` or set the color directly.

2) Light text on medium-dark backgrounds (small text)
- Symptoms: Light grey/white-ish text on darker panels (e.g., ArcLinux ForeColor on dark bluish background) might be marginal for small text.
- Fix: For smaller caption/secondary text, increase opacity/brightness by 10–25% (lighten foreground) or darken background slightly.
- Implementation: use `Lighten()` for text or `Darken()` for background to reach contrast.

3) Primary/OnPrimary pairing
- Most themes use whites on primary blues — typically PASS. If your Primary is light (e.g., bright yellow), `OnPrimary` should be dark; if `OnPrimary` is white and Primary is pale yellow, you will fail.
- Fix: Ensure `OnPrimaryColor` is the opposite luminance polarity: dark text on light Primary or white text on dark Primary.

4) Borders vs Background
- Many themes use subtle border colors (e.g., 200,200,200 on white) — while not a text contrast issue, they may be visually invisible to keyboard focus or sight-impaired users.
- Fix: Increase `BorderColor` contrast by either making it slightly darker or increasing thickness. For focus ring / outline, use `FocusIndicatorColor` with high contrast.

5) Small text / micro UI tokens
- Captions, placeholders, secondary labels often require higher color contrast than visual eye candy. If you must keep these lighter, increase text size weight or size for those tokens rather than lowering contrast.

6) Using `IsDarkTheme` correctly
- For preview or palettes where `IsDarkTheme = true` ensure that sub-tokens (e.g., `OnBackgroundColor`) are not pale grays that fall close to the background.
- Fix: Add a `ValidateContrast()` function in each theme's `ApplyCore()` to run a post-check and call `Darken()` or `Lighten()` for failing tokens.

---

## Suggested automatic validators / helper functions (developer action items)
1) Add a utility to compute WCAG contrast for a pair of Colors (we included an example script in `tools/contrast_report.py`, but execution requires Python to be available).
2) Add a `ValidateContrast()` method to the theme pipeline and optionally an `AutofixContrast()` routine.

Example: How to call the validator in a theme constructor
```csharp
ApplyColorPalette();
ApplyCore();
// run validator (report-only by default)
ValidateContrast(autofix: false);
```

3) Add a unit test or visual-diff for each theme using a small rendering of important UI controls to make sure contrast and border visibility meet accessibility targets.

---

## How I can help next
- I can run `tools/contrast_report.py` to compute exact WCAG contrast ratios and output a definitive `plansfixtheme_contrast_report.md` with exact numeric ratios and computed hex suggestions — I already wrote the script for that workflow. If you can install Python or give me permission to run it in this environment, I will run it and update the report with exact numbers and per-token suggested replacements.
- I can implement the `ValidateContrast()` helper method and add autofix helpers (e.g., `DarkenUntilContrast()`), and add unit tests to verify tokens for each theme.
- I can create a `theme-contrast-fix` PR that updates the small set of tokens we flagged (e.g., `CaptionColor` in DefaultBeepTheme; `ForeColor` in ArcLinux, and some `SubLabel` colors) with suggested replacement hexes.

---

If you'd like me to generate the precise WCAG-based numbers and exact hex fixes, say `Run exact contrast audit` and I’ll continue.

End of `plansfixtheme_contrast_report.md`.