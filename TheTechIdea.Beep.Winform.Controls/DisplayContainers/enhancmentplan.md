# BeepDisplayContainer2 Header Enhancement + Fix Plan

## Goal
Stabilize `BeepDisplayContainer2` after last-tab removal and modernize tab-header behavior so empty, active, hover, overflow, and utility-button states feel coherent and professional.

## Problems Addressed
- Last-tab removal left stale strip state (`_activeTab`, scroll state, utility hit areas, drag/indicator state).
- Zero-tab mode did not render a usable header strip, making add-tab recovery confusing.
- Header `+` action had no real behavior contract (`OnNewTabRequested` was a no-op).
- Header spacing and utility metrics were too loose/inconsistent for compact professional tab UX.

## Architecture Guardrails
- Keep layout + hit-test state in container/layout helpers.
- Keep painter render-only (`TabPaintHelper`).
- Keep typography theme-driven through `ApplyTheme()` and `TextFont`.
- Keep all dimensions DPI-driven through `DpiScalingHelper` and `TabHeaderMetrics`.

## Implementation Phases

### 1) Zero-Tab Lifecycle Reset
Files:
- `BeepDisplayContainer2.TabManagement.cs`
- `BeepDisplayContainer2.Layout.cs`

Work:
- Fixed active-tab handoff in `RemoveTab()` so activation pipeline executes.
- Added explicit `ResetTabHeaderStateForEmpty()` to clear:
  - active/hover/previous tab references
  - scroll offset and utility button bounds
  - drag state and indicator animation state
- Added defensive empty-state reset path when tab list reaches zero.
- Ensured layout clears stale tab bounds when strip area is unavailable.

### 2) Keep Header Live in Empty State
Files:
- `BeepDisplayContainer2.Painting.cs`
- `BeepDisplayContainer2.Layout.cs`
- `BeepDisplayContainer2.Mouse.cs`
- `Helpers/TabLayoutHelper.cs`

Work:
- Always paint tab strip in `Tabbed` mode, even when there are no tabs.
- Added utility-button layout support for zero-tab mode.
- Kept `+` button visible and interactive even when scrolling is not needed.
- Updated mouse hit testing to use utility-button presence, not only `_needsScrolling`.

### 3) Add-Tab Behavior Contract
Files:
- `BeepDisplayContainer2.cs`
- `BeepDisplayContainer2.Events.cs`
- `BeepDisplayContainer2.Mouse.cs`

Work:
- Added `NewTabRequested` event on the container.
- Implemented `OnNewTabRequested()` event raiser with `ContainerEvents` payload.
- Routed header `+` click to the new event contract.

### 4) Header Metrics + Compact UX Refresh
Files:
- `Helpers/TabHeaderMetrics.cs`
- `Helpers/TabLayoutHelper.cs`

Work:
- Refined spacing and size tokens for compact, professional density:
  - text padding, tab gap, close-slot width, close glyph size
  - utility button size and reserved strip widths
- Updated tab width measurement to align with draw assumptions (removed forced bold measurement baseline).
- Improved utility button placement math (right-anchored and DPI-safe).
- Changed reserved utility space logic:
  - reserve only new-tab area in non-overflow mode
  - reserve full utility cluster in overflow mode

## Validation Matrix
- Remove only/last tab.
- Add new tab immediately after empty.
- Activate new tab and verify content bounds.
- Hover/press `+` in zero-tab and multi-tab states.
- Overflow tabs and verify left/right scroll + plus placement.
- Verify top/bottom/left/right tab positions.
- Verify keyboard navigation and close-button interactions still behave.

## Expected UX Outcomes
- Empty tab container remains visually complete and recoverable.
- Header action affordances remain visible and functional.
- Layout and paint behavior stay synchronized across density and DPI.
- Tab strip feels closer to modern document-host products while preserving Beep styling model.
