# Phase 3: Advanced UX and Interactions (Execution Spec)

## Objective
Introduce hierarchy-aware interaction behavior, production-grade badges/tooltips, and overflow handling while preserving style-specific visuals.

## In Scope
- Child popup flows for items with `Children`.
- Badge semantics and placement rules.
- Icon-only tooltip/microcopy behavior.
- Overflow handling and `More` item model.
- Interaction event contracts.

## Out of Scope
- Low-level painter primitive refactors already covered by Phase 2.
- Custom painter API and design-time extensibility deep-dive (Phase 4).

## File Targets
- `BottomNavBars/BottomBar.cs`
- `BottomNavBars/BottomBarPainterContext.cs`
- `BottomNavBars/Helpers/BottomBarHitTestHelper.cs`
- `BottomNavBars/Helpers/BeepBottomBarLayoutHelper.cs`
- `BottomNavBars/Painters/BaseBottomBarPainter.cs`
- `BottomNavBars/Painters/*BottomBarPainter.cs`

## Implementation Tasks

### 1) Hierarchical Popup Interaction
1. Define trigger rule:
   - if `item.Children` has entries, click/enter opens child popup instead of immediate navigation activation.
2. Define popup host behavior:
   - anchored above source item
   - clamped to working area
   - closes on outside click / `Esc`
   - keyboard navigation with up/down/enter
3. Add event contract:
   - `ChildItemSelected(parent, child)`
   - `PopupOpened(parent)`
   - `PopupClosed(parent, reason)`

### 2) Badge and Notification Model
1. Badge variants:
   - numeric (`1`, `3`, `99+`)
   - dot (unread/attention)
2. Placement contract:
   - top-right overlap on icon region
   - must not obscure icon centerline or active indicator
3. Style modifiers:
   - CTA styles use stricter badge clipping and ring-safe placement.

### 3) Tooltip and Microcopy
1. In icon-only or selected-only label modes, hover/focus must show text affordance.
2. Tooltips should consume item text and optional subtext.
3. Tooltip visibility should be suppressed while popup is open for the same item.

### 4) Overflow and Responsive Collapse
1. Define overflow threshold using layout helper:
   - if all items cannot satisfy min target, move trailing items into `More`.
2. `More` behavior:
   - opens vertical popup list of overflow items
   - selecting overflow item behaves like selecting a normal item
3. Keep visual style parity:
   - `More` trigger adopts active style’s icon/label affordances.

### 5) Interaction Contract Matrix
For each style, document:
- main click behavior
- child-item behavior
- badge visibility level
- tooltip requirement level
- overflow trigger behavior

## Acceptance Criteria
- Parent items with children open a popup and do not fire direct route click by default.
- Badge rendering works without covering icon centerline.
- Tooltip behavior is present for icon-suppressed label modes.
- Overflow collapse to `More` is deterministic and keyboard-accessible.

## Regression Checklist
- Standard item activation still works across all styles.
- Popup open/close does not break hover/focus state in hit-test helper.
- Badge rendering remains theme-aware in normal and high-contrast modes.
- Overflow does not reorder non-overflow items unexpectedly.

## Risks and Mitigations
- **Risk:** Popup management can conflict with existing click routing.  
  **Mitigation:** Introduce explicit routing priority: popup-trigger first, then selection.
- **Risk:** Overflow behavior can alter user mental model.  
  **Mitigation:** Keep deterministic right-side overflow strategy with stable ordering.
