# Beep Rating Enhancement Plan

## Gap Analysis vs. Commercial Standards

| Capability | Current | Target |
|---|---|---|
| Size variants | Manual `StarSize` int | `RatingSizeVariant` enum (XS→XL) |
| Layout modes | Horizontal only | Horizontal, Vertical, Grid |
| Shapes | 9 styles | + Diamond, Flag, NumericScale (NPS), Slider, CompactInline |
| Color grade | No | HSL interpolation red→gold→green |
| Fractional fill | Half-star only | Precise arbitrary fill (3.7 → 70% fill on star 4) |
| Histogram | No | Amazon-style distribution bars |
| Multi-category | No | Stacked rows per category (cleanliness, service…) |
| Submission burst | No | Particle burst animation on submit |
| Animated fill | No | Sequential fill animation left-to-right |
| NPS mode | No | 0–10 numeric pills with Promoter/Passive/Detractor bands |
| RTL | No | `RightToLeft.Yes` mirrors star order + hit-test |

---

## Sprint 1 — Size Variants & Layout Modes ✅
- `RatingEnums.cs`: `RatingSizeVariant` { XS=16, SM=20, MD=24, LG=32, XL=48 }, `RatingLayoutMode` { Horizontal, Vertical, Grid }, `RatingMode` { Stars, NPS }
- `BeepStarRating`: `SizeVariant` property, `LayoutMode` property
- `RatingPainterContext`: add `LayoutMode`, `SizeVariant`, `IsRightToLeft`, `UseColorGrade`, `ColorGradeStart`, `ColorGradeEnd`

## Sprint 2 — New Painter Styles ✅
- Add to `RatingStyle` enum: Diamond, Flag, NumericScale, Slider, CompactInline
- `DiamondRatingPainter`: rotated square, gradient fill
- `FlagRatingPainter`: bookmark/ribbon SVG-path shape
- `NumericScalePainter`: 1–10 pill buttons, HSL red→gold→green
- `SliderRatingPainter`: horizontal track + thumb, drag to PreciseRating
- `CompactInlinePainter`: single filled star + "4.5" text (read-only summary)
- Register all 5 in `RatingPainterFactory`

## Sprint 3 — Color Grade & Precise Fractional Fill ✅
- `UseColorGrade` bool + `ColorGradeStart` / `ColorGradeEnd` on `BeepStarRating`
- HSL interpolation helper in `RatingThemeHelpers`
- Fractional fill: clip-region technique in all painters (`width * fraction`)
- `ShowPreciseValue` property shows "3.7" text

## Sprint 4 — Histogram / Summary Bar ✅
- `Models/RatingHistogramData.cs`: `Dictionary<int,int> Distribution`, `TotalCount`, `AverageRating`
- `Painters/RatingHistogramPainter.cs`: 1★→5★ labelled bars, proportional width
- `ShowHistogram` bool + `HistogramData` on `BeepStarRating`

## Sprint 5 — Group/Category Rating ✅
- `Models/RatingCategory.cs`: Label, Rating, PreciseRating, AccentColor
- `Categories List<RatingCategory>` on `BeepStarRating`
- Stacked vertical layout, `RatingChanged` args extended with `CategoryIndex`

## Sprint 6 — Submission Burst & Animated Fill ✅
- Sequential fill animation: `_fillProgress[]` float array; `_fillTimer` drives 0→1 per star
- Particle burst: `BurstParticle` pool, `_burstTimer`, drawn post-painter
- `EnableSubmitBurst` bool + `SubmitBurstColor`

## Sprint 7 — NPS Mode, Accessibility & RTL ✅
- `RatingMode.NPS`: 0–10, `NumericScalePainter`, Promoter/Passive/Detractor colour bands
- `NpsPromoterThreshold` / `NpsPassiveThreshold`; `NpsScore`; `NpsScoreChanged` event
- `RatingAccessibilityHelpers`: extend with live-region announcement
- Keyboard: Tab → stars, arrows change rating, Enter/Space submits
- RTL: reverse draw order + mirror hit-test coords
