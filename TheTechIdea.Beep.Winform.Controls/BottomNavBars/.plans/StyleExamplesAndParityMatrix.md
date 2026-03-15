# BottomBar Style Examples and Parity Matrix

## Purpose
Single-source reference for style identity, behavior parity, and visual example mapping.  
All styles remain distinct and independently testable.

## Style Parity Matrix

| Style | Painter | Indicator Signature | CTA Signature | Label Strategy Default | Badge Support | Popup Parent Support | Keyboard Parity |
|---|---|---|---|---|---|---|---|
| `Classic` | `ClassicBottomBarPainter` | underline/notch | none | always | yes | yes | required |
| `FloatingCTA` | `FloatingCTABottomBarPainter` | line/segment + CTA emphasis | circular floating CTA | selected-only | yes | yes | required |
| `Bubble` | `BubbleBottomBarPainter` | bubble active fill | optional | always | yes | yes | required |
| `Pill` | `PillBottomBarPainter` | rounded pill active area | optional | always | yes | yes | required |
| `Diamond` | `DiamondBottomBarPainter` | CTA-focused accent + line | diamond CTA | selected-only | yes | yes | required |
| `NotionMinimal` | `NotionMinimalBottomBarPainter` | subtle line/dot | none | icon-first | optional | yes | required |
| `MovableNotch` | `MovableNotchBottomBarPainter` | animated notch track | optional outlined CTA | selected-only | yes | yes | required |
| `OutlineFloatingCTA` | `OutlineFloatingCTABottomBarPainter` | ring/halo + line | outlined circular CTA | selected-only | yes | yes | required |
| `SegmentedTrack` | `SegmentedTrackBottomBarPainter` | moving segment block | optional | always | yes | yes | required 
| `GlassAcrylic` | `GlassAcrylicBottomBarPainter` | translucent indicator | optional | selected-only | yes | yes | required |

## Canonical Usage Scenarios

### Scenario A: Messaging App
- Preferred styles: `Bubble`, `SegmentedTrack`, `GlassAcrylic`
- Required elements:
  - unread numeric badges on inbox tab
  - quick switch with smooth indicator transition
  - `More` overflow once width constraints are exceeded

### Scenario B: Commerce App
- Preferred styles: `Pill`, `FloatingCTA`, `OutlineFloatingCTA`
- Required elements:
  - center CTA for cart or quick action
  - high-contrast selected state
  - child popup for category tabs with `Children`

### Scenario C: Productivity App
- Preferred styles: `Classic`, `NotionMinimal`, `MovableNotch`
- Required elements:
  - keyboard-first navigation
  - icon-only mode with tooltip fallback
  - deterministic focus and selected announcements

## Image Reference Slots
Use this table to map user-provided screenshots/mockups to style intent.

| Ref ID | Source Path/URL | Intended Style | Key Features to Replicate | Notes |
|---|---|---|---|---|
| IMG-01 | TBD | TBD | TBD | placeholder |
| IMG-02 | TBD | TBD | TBD | placeholder |
| IMG-03 | TBD | TBD | TBD | placeholder |
| IMG-04 | TBD | TBD | TBD | placeholder |
| IMG-05 | TBD | TBD | TBD | placeholder |

## Cross-Style Acceptance Checklist
- Distinct visual identity preserved for each style.
- Selection indicator always visible in both normal and high contrast themes.
- CTA hit area remains aligned with rendered CTA geometry.
- Parent item popup behavior is consistent across all styles.
- Keyboard behavior parity is maintained across style switches.
