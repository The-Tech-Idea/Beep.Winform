# BeepAdvancedButton Painter Fix Plan

## Scope
Update all painters under `Buttons/BeepAdvancedButton/Painters` and `Painters/NewsBanner` to align with modern, consistent rendering rules.

## Objectives
- Enforce shared painter contracts from `BaseButtonPainter`.
- Remove painter-local font creation and use `BeepThemesManager.ToFont(...)` through shared helpers.
- Standardize icon resolution and tint rendering via `StyledImagePainter`.
- Standardize loading/ripple pipelines for all painter styles.
- Remove ad-hoc text measurement code and use shared measurement helper.
- Keep all existing visual styles and public API behavior backward compatible.

## Work Items

### 1. Shared Contract Hardening
- [x] Add shared icon path accessors in base painter (`HasPrimaryIcon`, `GetPrimaryIconPath`).
- [x] Add shared text measurement helper using `TextRenderer`.
- [x] Add shared derived-font helper that resolves fonts through `BeepThemesManager.ToFont(...)`.
- [x] Add transition-aware color blending in base painter.

### 2. Interaction and Motion Consistency
- [x] Wire animated loading spinner angle from context in all main painters.
- [x] Wire real ripple state from control context instead of fixed disabled values.
- [x] Introduce reduced-motion behavior in control and context.
- [x] Add keyboard focus-visible contract and rendering hook.

### 3. Painter-Wide Refactor
- [x] Replace painter-local icon path reads with shared icon helpers.
- [x] Remove all `new Font(...)` usage in advanced button painters.
- [x] Remove `context.Font` usage in advanced button painters.
- [x] Remove temporary `Bitmap/Graphics` text measurement patterns.
- [x] Update all NewsBanner painter variants to use shared font/text measurement helpers.

### 4. Remaining Cleanup
- [x] Remove direct theme dependency in toggle painter content foreground fallback.
- [x] Normalize measurement helper signatures in banner painters to shared non-`Graphics` path.
- [x] Harden ripple clipping with safe graphics state restoration.

### 5. Documentation
- [x] Update `Buttons/BeepAdvancedButton/Readme.md` with intent, reduced motion, and focus-visible behavior.

## Verification Approach
- Static consistency checks across `Buttons/BeepAdvancedButton`:
  - no `new Font(` occurrences
  - no `context.Font` usage
  - no temporary bitmap+graphics text measurement
  - no per-painter mutable `ImagePainter` icon resolution patterns

## Notes
- Build execution was intentionally skipped when requested; verification is static/code-level.
