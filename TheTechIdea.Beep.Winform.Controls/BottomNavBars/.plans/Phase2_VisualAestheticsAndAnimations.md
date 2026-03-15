# Phase 2: Visual Aesthetics and Motion (Execution Spec)

## Objective
Define style-by-style rendering signatures and motion behavior for all existing `BottomBarStyle` variants, using current painter classes and shared context values.

## In Scope
- Indicator taxonomy and mapping to current styles.
- CTA rendering contracts for floating and outlined variants.
- Shadow/elevation and acrylic visual requirements.
- Transition quality and animation timing budgets.

## Out of Scope
- Popup hierarchy behavior and overflow UX (Phase 3).
- Design-time and a11y deepening (Phase 4).

## File Targets
- `BottomNavBars/Painters/BaseBottomBarPainter.cs`
- `BottomNavBars/Painters/ClassicBottomBarPainter.cs`
- `BottomNavBars/Painters/FloatingCTABottomBarPainter.cs`
- `BottomNavBars/Painters/BubbleBottomBarPainter.cs`
- `BottomNavBars/Painters/PillBottomBarPainter.cs`
- `BottomNavBars/Painters/DiamondBottomBarPainter.cs`
- `BottomNavBars/Painters/NotionMinimalBottomBarPainter.cs`
- `BottomNavBars/Painters/MovableNotchBottomBarPainter.cs`
- `BottomNavBars/Painters/OutlineFloatingCTABottomBarPainter.cs`
- `BottomNavBars/Painters/SegmentedTrackBottomBarPainter.cs`
- `BottomNavBars/Painters/GlassAcrylicBottomBarPainter.cs`
- `BottomNavBars/BottomBarPainterContext.cs`
- `BottomNavBars/BottomBar.cs`

## Style Rendering Contracts

| Style | Primary Indicator | CTA Behavior | Label Behavior | Motion Priority |
|---|---|---|---|---|
| `Classic` | underline/notch | none | always/selected-only policy driven | indicator glide |
| `FloatingCTA` | segment + elevated CTA emphasis | circular floating center | selected-first | CTA pulse + indicator |
| `Bubble` | bubble fill around active item | optional | always | scale + fill morph |
| `Pill` | rounded pill active background | optional | always | horizontal pill slide |
| `Diamond` | diamond CTA prominence | centered rotated CTA | selected-only | CTA hover pulse |
| `NotionMinimal` | minimal line/dot | none | icon-first | subtle opacity transition |
| `MovableNotch` | animated notch aligned to active | optional outlined CTA | selected-only | notch translation |
| `OutlineFloatingCTA` | ring + halo | outlined center CTA | selected-first | ring interpolation |
| `SegmentedTrack` | moving segmented track indicator | optional | always | track interpolation |
| `GlassAcrylic` | translucent indicator over acrylic base | optional | selected-only | soft fade/slide |

## Implementation Tasks

### 1) Indicator Taxonomy and Consistency
1. Standardize indicator drawing primitives in `BaseBottomBarPainter`:
   - line
   - dot
   - pill
   - segment block
2. Ensure all painters use `AnimatedIndicatorX` and `AnimatedIndicatorWidth` when applicable.
3. Define per-style fallback indicator for low-contrast themes.

### 2) CTA Visual Contract
1. Keep CTA styles distinct (`FloatingCTA`, `Diamond`, `OutlineFloatingCTA`, optional in `MovableNotch`).
2. Define CTA ring/halo/shadow layering order and z-depth.
3. Ensure CTA hit area and visual area remain synchronized.

### 3) Elevation, Shadow, Acrylic
1. Tokenize painter shadow recipe usage from context theme colors:
   - border tone
   - shadow tone
   - opacity scaling
2. For `GlassAcrylic`, define fallback path:
   - when performance is constrained -> reduced alpha and no expensive overlays.
3. Prevent style drift by documenting per-style allowable shadow strengths.

### 4) Motion and Animation Budgets
1. Set transition targets:
   - default selection animation 180–280 ms
   - no abrupt jumps unless style explicitly requires snap
2. Document easing:
   - default ease-out for indicator
   - optional pulse easing for CTA
3. Define redraw budget expectations to avoid excessive invalidation.

### 5) Optional Ripple Contract
1. Define ripple capability as optional per style (off by default unless enabled).
2. Ripple origin and clipping rules must respect item bounds and style geometry.

## Acceptance Criteria
- Each of the 10 styles has a documented visual signature and fallback behavior.
- Indicator motion is tied to animated context values.
- CTA variants have explicit visual + hit-test consistency requirements.
- Acrylic and shadow behavior include deterministic fallback rules.

## Regression Checklist
- Style switch (`BarStyle`) produces expected painter output and preserves selection.
- Animated indicator does not lag behind selected index under rapid keyboard navigation.
- CTA style variants remain clickable at all tested DPI scales.
- No style merges or shared “genericized” rendering that erases identity.

## Risks and Mitigations
- **Risk:** Over-unifying painter code can reduce style identity.  
  **Mitigation:** Share primitives only; keep per-style composition rules distinct.
- **Risk:** Excessive alpha/shadow passes can degrade performance.  
  **Mitigation:** Document and enforce per-style fallback recipes and redraw budget.
