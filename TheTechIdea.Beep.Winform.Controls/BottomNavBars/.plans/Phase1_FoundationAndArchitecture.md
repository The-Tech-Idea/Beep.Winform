# Phase 1: Foundation and Architecture (Execution Spec)

## Objective
Establish a deterministic layout and interaction foundation for all distinct `BottomBarStyle` variants while keeping style rendering responsibilities in painters.

## In Scope
- Ergonomic touch target guarantees.
- Responsive layout constraints and overflow decision points.
- Hit-test precision for icon/label/item scopes.
- Hierarchical item architecture contract (`SimpleItem.Children`) and popup host contract.
- Typography/label policy baseline used by all painters.

## Out of Scope
- New style creation.
- Final animation polish and ripple physics (Phase 2).
- Badge visual system and advanced popup UX polish (Phase 3).

## File Targets
- `BottomNavBars/BottomBar.cs`
- `BottomNavBars/Helpers/BeepBottomBarLayoutHelper.cs`
- `BottomNavBars/Helpers/BottomBarHitTestHelper.cs`
- `BottomNavBars/BottomBarPainterContext.cs`
- `BottomNavBars/Painters/BaseBottomBarPainter.cs`

## Implementation Tasks

### 1) Ergonomics and Touch Targets
1. Introduce a minimum logical target size contract (`48dp` equivalent after DPI scaling).
2. Ensure each computed item rectangle in `BeepBottomBarLayoutHelper` respects minimum tap width/height before painter-specific inflation.
3. Add fallback rule: if width is too constrained for all items at min size, trigger overflow candidate handling (`More` strategy boundary only; full UX in Phase 3).

### 2) Layout Architecture Baseline
1. Define a layout policy enum (internal) for:
   - balanced distribution
   - CTA-priority distribution
   - overflow-aware distribution
2. Keep current CTA and selected width factors, but document deterministic precedence:
   - CTA factor first
   - selected factor second
   - clamp to min touch target
3. Add explicit layout invalidation triggers (font, DPI, item changes, style changes) and ensure hit-test cache refresh occurs immediately after layout reflow.

### 3) Hit Testing Refactor
1. Expand hit areas per item:
   - `itemRect`
   - `iconRect`
   - `labelRect`
   - optional `ctaHaloRect` for CTA-focused styles
2. Standardize click resolution order:
   1) CTA hit region
   2) icon region
   3) label region
   4) item fallback region
3. Add guard rules to prevent accidental neighboring item activation near segment boundaries.

### 4) Hierarchical Navigation Contract
1. Define parent-item behavior:
   - if `SimpleItem.Children.Count > 0`, primary action opens child popup instead of direct route activation.
2. Define popup host contract (to be implemented later if not already present):
   - anchored above triggering item
   - auto-clamped inside monitor working area
   - inherits theme tokens (surface, border, text, radius, shadow)
3. Add event contract placeholder in docs:
   - `ChildItemInvoked(parent, child)`
   - `PopupOpened(parent)`
   - `PopupClosed(parent, reason)`

### 5) Typography and Label Policy Baseline
1. Add standard label visibility policy modes:
   - `Always`
   - `SelectedOnly`
   - `IconOnly`
2. Ensure policy can be consumed by painters without duplicating logic in each painter.
3. Ensure label measurement and truncation are centralized and DPI-aware.

## Acceptance Criteria
- Every item has at least a 48dp-equivalent interactive area.
- Layout is stable under resize/DPI/font changes with no overlapping item bounds.
- Hit-testing consistently resolves icon/label/item regions.
- Parent items with children follow popup trigger contract in API design.
- Label visibility policy is explicitly defined and reusable across styles.

## Regression Checklist
- Resizing from wide to narrow keeps deterministic layout behavior.
- CTA styles preserve center intent while staying clickable.
- Keyboard focus index remains aligned with hit-test index mapping after reflow.
- No style is removed, merged, or behavior-aliased.

## Risks and Mitigations
- **Risk:** Width-factor + min-touch conflicts at narrow widths.  
  **Mitigation:** Introduce overflow threshold and defer full overflow UX to Phase 3.
- **Risk:** Hit-test complexity introduces edge-click ambiguity.  
  **Mitigation:** Enforce strict precedence and shared helper-level boundary tests.
