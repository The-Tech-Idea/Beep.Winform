# Phase 4: Developer Experience and Accessibility (Execution Spec)

## Objective
Finalize BottomNavBars as an enterprise-ready component with a clear extensibility model, design-time productivity, and robust accessibility/high-contrast behavior.

## In Scope
- Custom painter injection and fallback resolution.
- Design-time editing and discoverability improvements.
- Keyboard and popup accessibility parity.
- Screen reader semantics and automation metadata completeness.
- High contrast and theme-token compliance rules.

## File Targets
- `BottomNavBars/BottomBar.cs`
- `BottomNavBars/Painters/IBottomBarPainter.cs`
- `BottomNavBars/Painters/BaseBottomBarPainter.cs`
- `BottomNavBars/Helpers/BottomBarHitTestHelper.cs`
- `BottomNavBars/BottomBarPainterContext.cs`
- `BottomNavBars/*Designer*` (if/when introduced)

## Implementation Tasks

### 1) Extensibility and Custom Painter API
1. Define painter resolution order:
   1) explicit custom painter instance (if provided)
   2) registered painter factory for current style
   3) built-in style painter fallback
2. Define extension points:
   - optional custom layout provider hook
   - optional custom badge renderer hook
3. Add usage examples for external consumers creating new painter classes.

### 2) Design-Time and Configuration Experience
1. Provide design-time quick actions for:
   - style switch
   - CTA index selection
   - animation duration
   - label policy
2. Improve item editing workflow:
   - hierarchical `Children` editing guidance
   - badge and tooltip fields visibility
3. Add “safe defaults” matrix for common business app scenarios.

### 3) Accessibility Contract
1. Keyboard model:
   - left/right/home/end navigation
   - space/enter activation
   - popup navigation up/down/enter/esc when child menu is open
2. Screen reader semantics:
   - `MenuBar` role on control
   - `MenuItem` role per child
   - announce selected, focused, and has-popup states
3. Focus visibility:
   - style-independent focus ring minimum contrast and thickness.

### 4) High Contrast and Theme Compliance
1. Define contrast checks for key token pairs:
   - bar background vs item text
   - selected indicator vs on-accent text
   - badge background vs badge text
2. Define high-contrast overrides:
   - disable low-contrast visual effects (subtle shadows, low-alpha fills)
   - ensure indicator remains perceivable without color-only signaling.
3. Include runtime validation checklist for HC mode toggling.

### 5) Documentation and Consumer Guidance
1. Add API usage snippets for:
   - style selection
   - custom painter injection
   - item hierarchy usage
   - a11y-focused configuration
2. Add troubleshooting section:
   - hit-testing mismatch symptoms
   - style fallback behavior
   - popup focus lock issues

## Acceptance Criteria
- Custom painter extension path is documented with deterministic fallback order.
- Designers can configure core BottomBar settings without code.
- Keyboard + popup navigation behavior is fully specified.
- Screen reader semantics include selection/focus/has-popup announcements.
- High contrast behavior avoids color-only affordances.

## Regression Checklist
- Existing style-to-painter mapping remains intact.
- Accessibility metadata updates on item/selection changes.
- Popup open state does not break tab/focus traversal.
- High contrast mode remains legible across all distinct styles.

## Risks and Mitigations
- **Risk:** Extensibility APIs introduce conflicting runtime behavior.  
  **Mitigation:** enforce clear precedence and fallback contracts in docs and examples.
- **Risk:** Accessibility additions may regress visual polish in some styles.  
  **Mitigation:** separate a11y overlays from style rendering layers and test both paths.
