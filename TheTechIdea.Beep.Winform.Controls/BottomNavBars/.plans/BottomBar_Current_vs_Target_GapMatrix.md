# BottomBar Current vs Target Gap Matrix

## Purpose
This document audits the current `BottomBar` implementation and maps gaps to the four delivery phases.  
All existing `BottomBarStyle` values remain distinct.

## Current Architecture Snapshot
- Style routing is centralized in `BottomBar.InitializePainterFromStyle`.
- Shared rendering context is provided by `BottomBarPainterContext`.
- Layout distribution and CTA/selected width weighting are handled by `BeepBottomBarLayoutHelper`.
- Pointer interaction and basic focus index are handled in `BottomBarHitTestHelper`.
- Keyboard support is present for left/right/home/end/space/enter.
- Accessibility baseline exists (`AccessibleRole.MenuBar` and child `MenuItem` objects).

## Style Matrix (Current vs Target)

| Style | Current Strength | Current Gaps | Phase Mapping |
|---|---|---|---|
| `Classic` | Stable baseline paint and indicator | Missing explicit acceptance profile and label policy variants | P1, P2 |
| `FloatingCTA` | CTA rendering and notch radius tuning exist | No formal touch target and CTA fallback contract | P1, P2 |
| `Bubble` | Distinct bubble emphasis style exists | Needs animation/ripple quality targets and spacing contract | P2 |
| `Pill` | Distinct pill shape and active visual | Needs parity checklist for label/icon rules | P2 |
| `Diamond` | Distinct CTA shape style | Needs accessibility and hit area verification criteria | P1, P4 |
| `NotionMinimal` | Minimal visual language exists | Needs rulebook for icon-only + tooltip/microcopy behavior | P2, P3 |
| `MovableNotch` | Notch motion settings available | No formal behavior spec for notch + outline combinations | P2 |
| `OutlineFloatingCTA` | Ring/halo properties available | Missing concrete theming and contrast constraints | P2, P4 |
| `SegmentedTrack` | Segment track and indicator width tuning present | Missing track behavior contract under overflow/reflow | P2, P3 |
| `GlassAcrylic` | Acrylic opacity support exists | Missing blur fallback policy, HC fallback, and perf criteria | P2, P4 |

## Cross-Cutting Gap Areas

### 1. Foundation and Data/Structure
- Need explicit 48dp min target enforcement and item-count guidance.
- Need formal parent-item (`SimpleItem.Children`) popup contracts.
- Need overflow strategy (`More` consolidation) tied to layout helper rules.

### 2. Visual and Motion Fidelity
- Need per-style indicator taxonomy and required transitions.
- Need standardized animation budgets (duration/easing/frame budget).
- Need tokenized shadow/elevation and acrylic fallback behavior.

### 3. Advanced UX Flows
- Need concrete child-popup trigger/focus/escape contracts.
- Need badge type/placement spec and style-dependent visibility rules.
- Need icon-only tooltip behavior standards.

### 4. DX and Accessibility
- Need custom painter injection API contract and fallback resolution order.
- Need design-time editing guidance for hierarchical item structures.
- Need keyboard and screen-reader behavior parity matrix.

## Phase Mapping

| Gap Area | Delivering Phase(s) |
|---|---|
| Layout constraints, hit test boundaries, popup infrastructure | Phase 1 |
| Style recipes, indicator motion, CTA depth, acrylic/shadow | Phase 2 |
| Hierarchical popups, badges, tooltips, overflow collapse | Phase 3 |
| Extensibility API, design-time, a11y, high-contrast | Phase 4 |

## Acceptance Baseline for Plan Revisions
- Every phase document includes concrete file targets and non-goals.
- Every style appears in at least one explicit acceptance checklist.
- No consolidation or aliasing of styles.
