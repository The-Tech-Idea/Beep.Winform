# BottomNavBars Enhancement Plan
**Reference direction:** Modern mobile bottom navigation patterns (Material 3, iOS tab bars, fluent surfaces)  
**Target area:** `TheTechIdea.Beep.Winform.Controls/BottomNavBars`  
**Last updated:** 2026-02-27

---

## Goal

Upgrade `BottomBar` and its painter family to deliver:
- consistent icon/label rendering and state feedback across styles,
- reliable hover/focus/pressed/disabled and keyboard behavior,
- robust layout and hit targets under resize and DPI changes,
- safe lifecycle/resource handling with no stale subscriptions,
- theme-token-first visuals with predictable style fallback.

---

## Implementation Constraints (Mandatory)

- Keep all controls inheriting from `BaseControl`.
- Keep event/state mutation in control/helpers; painters remain rendering-focused.
- Use `ImagePainter`/SVG pipeline consistently for icon rendering.
- Prefer theme tokens first and use style tokens as fallback when theme is unavailable.
- Ensure layout helper and hit-test helper stay in sync after state/size/data changes.

---

## Backlog

## Phase 1 - Foundation and Reliability (P0)

### BNB-01: Lifecycle and event safety
Status: Completed
- Fix timer/event unsubscription and helper disposal.
- Eliminate duplicate click activation paths.
- Ensure hover updates are wired and keyboard handled keys suppress default keypress.

### BNB-02: Layout/hit-test synchronization
Status: Completed
- Recompute and sync layout/hit areas on resize/style/data mutation.
- Ensure selected index fallback does not rely on null-forgiving operators.

---

## Phase 2 - Input and Accessibility Parity (P0)

### BNB-03: Keyboard/focus parity
Status: Completed
- Normalize left/right/home/end/enter/space semantics and focus visuals.
- Ensure focused item and selected item transitions are coherent.

### BNB-04: Accessibility metadata and structure
Status: Completed
- Add stronger accessible metadata and child item exposure.
- Keep accessibility notifications synchronized with selection/focus changes.

---

## Phase 3 - Theme and Visual Consistency (P1)

### BNB-05: Theme token consistency in base painter
Status: Completed
- Remove hardcoded paint-time color assumptions in shared painter paths.
- Keep selected/hover text/icon semantics consistent between styles.

### BNB-06: Painter family harmonization
Status: Completed
- Align Classic/Floating/Bubble/Pill/Diamond with consistent spacing and state cues.
- Align Notion/MovableNotch/Outline/Segmented/Glass with matching interaction semantics.

---

## Phase 4 - Performance and DPI Robustness (P1)

### BNB-07: Targeted redraw and animation hygiene
Status: Completed
- Reduce broad invalidation where practical.
- Ensure indicator animation and ticker updates avoid unnecessary redraw churn.

### BNB-08: DPI and autosize reliability
Status: Completed
- Validate icon/text sizing and CTA dimensions under DPI scaling.
- Ensure no clipping or hit-target drift after DPI/font changes.

---

## Validation Checklist

- Build passes for `TheTechIdea.Beep.Winform.Controls.csproj` (excluding unrelated existing blockers).
- No new linter issues in modified BottomNavBars files.
- No duplicate click events for one interaction.
- Hover, focus, keyboard navigation, and selection behavior match visual feedback.
- Layout and hit targets remain correct after resize/style/items changes.
