# BeepAdvancedButton Image Alignment and New Styles Plan

## Goal
Align painter output with the reference images in `plans/buttons1.png` to `plans/buttons5.png`, then add missing painter styles where current painters cannot reproduce the target visuals.

## Progress Snapshot
- [x] Phase 1: deterministic variant API (`NewsBannerVariant`, `ContactVariant`, `ChevronVariant`, `FlatWebVariant`, `LowerThirdVariant`).
- [x] Phase 2: variant-aware factory routing for `NewsBanner`.
- [x] Phase 3: geometry normalization pass for core `NewsBanner`/`Chevron` painters.
- [x] Phase 4: new families added (`FlatWebButtonPainter`, `LowerThirdButtonPainter`, `StickerLabelButtonPainter`).
- [ ] Phase 5: snapshot regression tests and expanded style documentation.

## Reference Coverage Matrix

| Image | Visual Family | Current Coverage | Gap | Action |
|---|---|---|---|---|
| `buttons1.png` | Flat modern CTA, angled split sections, pill + icon section | Mostly covered by `ContactButtonPainter` and `NavigationChevronButtonPainter` | Some presets are heuristic-only and not explicitly selectable | Add explicit style selectors and deterministic layout IDs |
| `buttons2.png` | Flat web UI controls (left icon badges, right notches, search bars) | Covered by `FlatWeb` style variants | Minor spacing/angle tuning for some edge cases | Continue geometry token tuning and snapshot checks |
| `buttons3.png` | Comic/sticker labels, speech balloons, playful badges | Covered by `StickerLabel` style | Fine-tune geometry/details to match references more closely | Expand `StickerLabel` variants and tune proportions |
| `buttons4.png` | TV/news badges, 24/live/breaking ribbons | Largely covered by existing `NewsBanner` painters | Not all concrete variants are runtime-addressable from control API | Add explicit `NewsBannerVariant` selection and painter routing |
| `buttons5.png` | Broadcast lower-third bars, ticker strips, headline blocks | Covered by `LowerThird` style variants | Minor typography sizing and spacing tuning | Continue typography token tuning and snapshot checks |

## Critical Issues to Solve First
1. Runtime painter routing is too generic for `NewsBanner`:
- Current runtime path resolves to `NewsBannerButtonPainter` only.
- Specialized classes under `Painters/NewsBanner/*` are not first-class selectable via control API.

2. Variant selection depends on heuristics:
- Multiple painters auto-detect from text/icon content.
- This causes name-correct painter code to render unexpected layouts for user text.

3. Missing variant taxonomy:
- No single public enum set that maps one-to-one to visible reference variants.

## Implementation Phases

### Phase 1: Deterministic Variant API
- Add explicit variant enums on the control:
  - `NewsBannerVariant`
  - `ContactVariant`
  - `ChevronVariant`
  - `FlatWebVariant` (new)
  - `LowerThirdVariant` (new)
- Pass selected variants into `AdvancedButtonPaintContext`.
- Remove heuristic fallback from primary path; keep as fallback only when variant is `Auto`.

### Phase 2: Route to Concrete Painters
- Extend painter factory with variant-aware creation:
  - `CreatePainter(style, variantContext)`.
- Wire each `NewsBannerVariant` to concrete painter classes in `Painters/NewsBanner`.
- Ensure each variant name maps to exactly one visual structure.

### Phase 3: Align Existing Painters to References
- `buttons1` alignment pass:
  - Normalize diagonal cut angles, icon section widths, corner radii, and fixed spacing.
- `buttons4` alignment pass:
  - Normalize badge scale (`24`, `LIVE`) and two-line typography hierarchy.
- Add per-variant geometry constants to avoid ad-hoc values.

### Phase 4: Add New Painter Families
- Add `FlatWebButtonPainter`:
  - Left icon block + text bar
  - Right notch/search badge styles
  - Flat segmented variants from `buttons2`.
- Add `LowerThirdPainter`:
  - Headline + subline bars
  - Location/live tag blocks
  - Ticker strip variants from `buttons5`.
- Add `StickerLabelPainter`:
  - Cartoon bubble, outlined sticker, badge label variants inspired by `buttons3`.

### Phase 5: Quality and Regression Guardrails
- Add visual regression harness (render to bitmap and compare snapshots).
- Add naming consistency checks:
  - painter class name keyword must match geometry keywords (Circle/Pill/Chevron/Hexagon/etc.).
- Add usage docs with preview table per variant.

## New Style/Painter Backlog (Proposed)

### New `AdvancedButtonStyle` entries
- `FlatWeb`
- `LowerThird`
- `StickerLabel`

### New painters
- `FlatWebButtonPainter`
- `LowerThirdButtonPainter`
- `StickerLabelButtonPainter`

### New sub-painters (examples)
- `Painters/FlatWeb/RightNotchSearchPainter`
- `Painters/FlatWeb/LeftBadgeActionPainter`
- `Painters/LowerThird/HeadlineBarPainter`
- `Painters/LowerThird/TickerStripPainter`
- `Painters/Sticker/SpeechBubblePainter`

## Deliverables
1. Variant enums and control properties.
2. Variant-aware factory routing.
3. Existing painter alignment fixes for `buttons1` and `buttons4`.
4. New `FlatWeb` and `LowerThird` painter families.
5. `StickerLabel` painter family and variants.
6. Updated readme with reference-to-variant mapping.

## Acceptance Criteria
- Each selected variant produces one deterministic layout (no heuristic drift).
- `buttons1` and `buttons4` references are reproducible with current styles and variants.
- `buttons2` and `buttons5` are covered by new painter families.
- All painter names match actual geometry/layout behavior.
