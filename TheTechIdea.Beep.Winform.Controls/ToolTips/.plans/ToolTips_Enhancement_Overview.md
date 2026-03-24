# ToolTips Enhancement Plan — Overview

**Date:** 2026-03-17  
**Scope:** UI/UX alignment, sizing, typography, and visual polish matching Figma/MD3 and latest web standards

---

## Current State

The ToolTips system already has a solid 3-layer architecture (ToolTipManager → ToolTipInstance → CustomToolTip) with 7 painters, 7 layout variants, 21 semantic types, 8 animation types, rich content model (`ToolTipContentItem`), and keyboard shortcut badges. Previous sprints delivered theme integration, smart positioning, accessibility basics, and the painter pattern.

## Existing Infrastructure to Leverage

> [!IMPORTANT]
> The project already has comprehensive styling infrastructure. **No new typography/spacing classes needed** — the plans wire existing APIs into ToolTip painters.

| System | File | Key APIs |
|--------|------|----------|
| **Font Management** | `FontManagement/BeepFontManager.cs` | `TooltipFont`, `GetCachedFont()`, `GetFontForPainter()` (DPI-aware), `UIElementType.Tooltip` |
| **Typography** | `Styling/Typography/StyleTypography.cs` | `GetFontFamily(style)`, `GetFontSize(style)`, `GetLineHeight(style)`, `GetLetterSpacing(style)` for 30+ styles |
| **Spacing** | `Styling/Spacing/StyleSpacing.cs` | `GetPadding(style)`, `GetItemSpacing(style)`, `GetIconSize(style)` per style |
| **Image Painting** | `Styling/ImagePainters/StyledImagePainter.cs` | Full caching, tinting, shape-based clipping |
| **Icons** | `IconsManagement/SvgsUI.cs` | SVG icon catalog with UI icons |
| **Styling Core** | `Styling/BeepStyling.cs` | `PaintStyleBackground()`, `PaintStyleBorder()`, `PaintStyleText()` for 50+ styles |

---

## Gaps Identified

| # | Gap | Severity |
|---|-----|----------|
| 1 | Hard-coded font sizes (`new Font("Segoe UI", 10.5f)`) in layout helpers/painters — should use `StyleTypography`/`BeepFontManager` | Critical |
| 2 | `ToolTipContentItem[]` declared but never rendered by main painter | High |
| 3 | Magic-number spacing (12, 8, 16 px) — should use `StyleSpacing` | High |
| 4 | `ToolTipLayoutMetrics` missing Footer, Divider, CloseButton rects | High |
| 5 | Shadow uses naive blur loop instead of MD3 elevation layers | Medium |
| 6 | Arrow border doesn't seamlessly join tooltip body | Medium |
| 7 | No RTL layout mirroring | Medium |
| 8 | `MinContrastRatio` declared but never enforced | Medium |
| 9 | Close button not drawn when `Closable == true` | Medium |

---

## Phase Summary

| Phase | Focus | Priority | Effort | Plan Document |
|-------|-------|----------|--------|---------------|
| **1** | Typography & Alignment — wire `StyleTypography`/`BeepFontManager` | CRITICAL | ~1.5 h | [Phase1](Phase1_TypographyAndAlignment.md) |
| **2** | Rich Content & Section Layout | HIGH | ~3 h | [Phase2](Phase2_RichContentRendering.md) |
| **3** | Sizing & Responsive — wire `StyleSpacing` | HIGH | ~1.5 h | [Phase3](Phase3_SizingAndResponsiveness.md) |
| **4** | Visual Polish & Animation | MEDIUM | ~2 h | [Phase4](Phase4_VisualPolishAndAnimations.md) |
| **5** | Accessibility & RTL | MEDIUM | ~2 h | [Phase5](Phase5_AccessibilityAndRTL.md) |
| **6** | Performance & Cleanup | MEDIUM | ~1.5 h | [Phase6](Phase6_PerformanceAndCleanup.md) |

**Total estimated effort:** ~12 hours

---

## Key Design Decision: Use Existing Infrastructure

The original plan proposed creating three new classes:
- ~~`Helpers/ToolTipTypographyScale.cs`~~ → **Use `StyleTypography` + `BeepFontManager`**
- ~~`Helpers/ToolTipSpacing.cs`~~ → **Use `StyleSpacing`**
- `Design/ToolTipConfigActionList.cs` → Still needed (Phase 6)

---

## Key Files to Modify

| File | Phases | Changes |
|---|---|---|
| `Helpers/ToolTipLayoutHelpers.cs` | 1, 2, 3, 5 | Fix hard-coded sizes, expand metrics, responsive max-width, RTL |
| `Painters/BeepStyledToolTipPainter.cs` | 1, 2, 4 | Wire `StyleTypography`, ContentItem rendering, MD3 shadows |
| `Painters/ToolTipPainterBase.cs` | 1, 3 | Font delegation via `BeepFontManager`, spacing via `StyleSpacing` |
| `ToolTipConfig.cs` | 1 | Add `TextHAlign`, `TextVAlign` |
| `CustomToolTip.Core.cs` | 2, 5 | Close-button state, RTL detection, focusability |
| `Helpers/ToolTipThemeHelpers.cs` | 5 | Contrast auto-correction |
| `Helpers/ToolTipAnimationHelpers.cs` | 4 | MD3 easing curves |
| `ToolTipEnums.cs` | 4 | MD3 easing enum values |

---

## Verification Strategy

No existing unit tests were found. Verification is visual + build:

1. **Build:** `dotnet build TheTechIdea.Beep.Winform.Controls.csproj` after each phase
2. **Visual:** Run sample app, set tooltip properties on controls, verify rendering
3. **DPI:** Test at different scale factors  
4. **User checkpoints:** Request user verification after each phase
