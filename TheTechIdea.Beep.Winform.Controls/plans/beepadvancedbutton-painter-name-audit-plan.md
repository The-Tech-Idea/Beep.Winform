# BeepAdvancedButton Painter Name Audit and Expansion Plan

## Goal
Ensure each painter name accurately reflects its rendered geometry, and ensure every concrete painter can be selected deterministically from the control API.

## Reference Inputs
- `plans/buttons1.png`
- `plans/buttons2.png`
- `plans/buttons3.png`
- `plans/buttons4.png`
- `plans/buttons5.png`

## Current Audit Snapshot

### 1. Name vs Render Consistency
- `CircleBadge24NewsPainter`: name matches code (left circle badge + banner body).
- `BreakingNewsRectanglePainter`: name matches code (rectangular badge/body split).
- `CircleBadgeAngledBannerPainter`: name matches code (circle badge + angled banner).
- `ChevronRightNewsPainter`: name matches code (right chevron section).
- `FakeNewsChevronPainter`: name matches code (chevron-angled multi-section layout).
- `IconCirclePillNewsPainter`: name matches code (icon circle + pill body).
- `LiveBreakingNewsPainter`: name matches code (`LIVE` section + breaking section).
- `PinkWhiteAngledBannerPainter`: name matches code (two-tone angled split).
- `HexagonWorldNewsPainter`: name matches code (hexagon-like side geometry).
- `FlatWebButtonPainter`: variant names match render intent (`LeftBadgeAction`, `RightNotchSearch`, `SegmentedIconAction`, `SearchPillNotch`, `ToolbarSegment`).
- `LowerThirdButtonPainter`: variant names match render intent (`HeadlineBar`, `LiveTagHeadline`, `ReportSplit`, `TickerStrip`, `TickerChevron`).

### 2. Deterministic Selection Gap
- Before this pass, many concrete `Painters/NewsBanner/*` classes existed but were not selectable from `NewsBannerVariant`.
- This produced a naming mismatch at API level: painter names existed, but control users could not target them directly.

## Completed in This Pass
- [x] Added deterministic `NewsBannerVariant` enum entries for additional concrete banner painters.
- [x] Updated `ButtonPainterFactory.CreateNewsBannerPainter(...)` to route those new variants to matching concrete classes.
- [x] Kept existing coarse variants for backward compatibility.
- [x] Added `StickerLabel` style family with deterministic variants and control/context API wiring.
- [x] Expanded `FlatWebVariant` and `LowerThirdVariant` with additional layouts from `buttons2.png` and `buttons5.png`.

## Remaining Work Plan

### Phase A: Expose Full Variant Catalog in Documentation
- [ ] Update `Buttons/BeepAdvancedButton/Readme.md` with a variant catalog table:
  - Variant enum value
  - Painter class
  - Visual motif
  - Reference image (buttons1..buttons5)

### Phase B: Add Missing Families from References
- [x] Add `StickerLabel` style family (from `buttons3.png`) with variants:
  - `SpeechBubble`
  - `CloudTag`
  - `BurstBadge`
  - `ComicRibbon`
- [x] Add factory routing and enum support for `StickerLabel`.

### Phase C: FlatWeb and LowerThird Coverage Expansion
- [x] Add extra `FlatWeb` variants for right-arrow tags and magnifier-tail badges seen in `buttons2.png`.
- [x] Add extra `LowerThird` variants for stacked location strip + headline block seen in `buttons5.png`.

### Phase D: Geometry/Name Guardrails
- [ ] Add a lightweight painter audit checklist in code comments for each painter:
  - Primary shape(s)
  - Expected text slots
  - Expected icon slots
- [ ] Add snapshot render tests for at least one sample per variant family.

## Acceptance Criteria
- Each variant name maps to one deterministic painter/layout.
- Painter class name and rendered dominant geometry are aligned.
- `buttons1/buttons2/buttons4/buttons5` are covered by existing styles and variants.
- `buttons3` coverage is provided by new `StickerLabel` style family.
