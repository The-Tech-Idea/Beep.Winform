# Beep Cards Commercial UI/UX Modernization Plan
**Created:** 2026-02-28  
**Scope:** `TheTechIdea.Beep.Winform.Controls/Cards/**`

---

## Goal

Elevate `BeepCard` and the card painter ecosystem to commercial-grade behavior and visuals (Figma/DevExpress-style interaction quality) while enforcing Beep framework rules:

- theme fonts via `BeepThemesManager.ToFont(...)` only,
- no inline `new Font(...)`,
- no `Font` property assignment,
- DPI scaling through `DpiScalingHelper`,
- icon/image painting via `StyledImagePainter`.

---

## Implemented Work

### 1) Painter Contract + Theme Font Pipeline

- Updated `ICardPainter.Initialize` to receive theme fonts:
  - `Font titleFont`
  - `Font bodyFont`
  - `Font captionFont`
- Updated all card painter implementations to the new signature.
- Updated `BeepCard` to resolve and cache `_titleFont`, `_bodyFont`, `_captionFont`, `_headerFont`, `_paragraphFont` from `BeepThemesManager.ToFont(...)`.
- Updated `BeepCard` draw path to use these theme-resolved fonts.

### 2) Inline Font Removal in Painters

- Removed inline `new Font(...)` creation from card painters.
- Removed painter dependence on control `.Font` for rendering.
- Updated `CardRenderingHelpers`:
  - removed local font cache/new-font creation path,
  - updated chips rendering to accept explicit font input from painters.

### 3) DPI Normalization in Card Painters

- Applied `DpiScalingHelper.ScaleValue(..., owner)` usage across card painter layouts/drawing calculations that used pixel constants.
- Preserved base constants as design tokens where needed, but all runtime layout arithmetic now scales through DPI helpers.

### 4) Interaction State Surface

- Expanded `LayoutContext` with interaction and state fields:
  - `IsHovered`, `IsPressed`, `IsSelected`, `IsLoading`
  - `HoverProgress`, `PressProgress`
  - `ElevationTier`, `IsFocused`
  - `RipplePoint`

### 5) Card Interaction Engine

- Added `Cards/Helpers/CardInteractionManager.cs`.
- Implemented timer-driven interaction interpolation:
  - hover progress animation,
  - press-depth progress animation,
  - ripple center/radius/alpha animation,
  - focus state propagation.
- Wired manager lifecycle and mouse/focus hooks through `BeepCard`.

### 6) Visual/UX Layers in `BeepCard`

- Added card-level UX overlays and affordances:
  - focus ring,
  - accent top strip (`AccentBarHeight`),
  - selection checkbox icon (`ShowSelectionCheckbox`, `IsSelected`),
  - overflow/context icon (`ContextMenuIcon`, default `SvgsUI.MoreVertical`),
  - collapse/expand chevron (`IsCollapsible`, `IsExpanded`),
  - ripple overlay.
- Added events:
  - `SelectionChanged`
  - `ContextMenuRequested`

### 7) Loading Skeleton + Shimmer

- Added `IsLoading` behavior in `BeepCard` draw pipeline.
- Added shimmer/skeleton helper rendering path in card visual helpers.
- During loading, normal content paint is skipped and placeholder shimmer is rendered.

---

## API Additions (BeepCard)

- `bool IsSelected`
- `bool ShowSelectionCheckbox`
- `bool IsLoading`
- `int AccentBarHeight`
- `string ContextMenuIcon`
- `bool IsCollapsible`
- `bool IsExpanded`
- `event EventHandler SelectionChanged`
- `event EventHandler ContextMenuRequested`

---

## Files Updated (Key)

- `TheTechIdea.Beep.Winform.Controls/Cards/Helpers/ICardPainter.cs`
- `TheTechIdea.Beep.Winform.Controls/Cards/Helpers/CardInteractionManager.cs` (new)
- `TheTechIdea.Beep.Winform.Controls/Cards/BeepCard.cs`
- `TheTechIdea.Beep.Winform.Controls/Cards/Painters/StyleCardPainters.cs`
- `TheTechIdea.Beep.Winform.Controls/Cards/Painters/*.cs` (full painter set)
- `TheTechIdea.Beep.Winform.Controls/IconsManagement/SvgsUI.cs` (added `MoreVertical` alias)

---

## Validation Notes

- Card-focused lints: no linter errors in `Cards` path.
- Build still has unrelated project-level errors outside cards in current workspace state.

---

## Guardrails Followed

- Fonts sourced from theme (`BeepThemesManager.ToFont`) in card pipeline.
- No direct `Font` property assignment introduced.
- Image/icon overlays painted using `StyledImagePainter`.
- DPI scaling applied through `DpiScalingHelper`.
