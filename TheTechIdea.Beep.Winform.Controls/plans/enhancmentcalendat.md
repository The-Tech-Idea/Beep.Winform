# BeepCalendar Enhancement Plan
**Reference direction:** Latest Figma-style scheduling patterns (clean hierarchy, compact density options, strong interaction feedback, accessible contrast and focus)
**Target control:** `TheTechIdea.Beep.Winform.Controls/Calendar/BeepCalendar.cs`
**Last updated:** 2026-02-27

---

## Goal

Upgrade `BeepCalendar` to a modern, polished, Figma-aligned experience with:
- clearer visual hierarchy (header, grid, events, sidebar),
- consistent interaction states across Month/Week/Day/List,
- stronger accessibility and keyboard behavior,
- smoother repaint and layout performance.

---

## Current Observations

From review of `BeepCalendar` and renderer/painter contracts:
- The control already has a strong architecture (`CalendarLayoutManager`, `CalendarRenderer`, `ICalendarStylePainter`, view renderers).
- Some UI tokens are hardcoded in drawing paths (e.g., fixed event row heights, "+N more" typography, list card sizing).
- Hover and mouse move paths currently invalidate the whole control instead of targeted dirty regions.
- Header/title and top action layout can become crowded in smaller widths.
- Fonts in the calendar rely on direct `new Font(...)` defaults in properties rather than a unified theme-font manager path.

---

## UX/UI Standards to Apply

1. **Figma-like spacing system**
   - Use consistent spacing scale: `4 / 8 / 12 / 16 / 24`.
   - Standardize paddings for header, cell content, sidebar cards, event blocks.

2. **Clear state system**
   - Every interactive element must support: `default`, `hover`, `selected`, `focused`, `disabled`.
   - Day cells and event chips must not rely on color-only signaling.

3. **Legibility**
   - Keep title/date/event text readable at all densities.
   - Ensure placeholder and out-of-month labels remain visible but de-emphasized.

4. **Accessibility**
   - Visible focus rings for keyboard navigation.
   - Minimum click target for event chips and day cells.
   - Maintain sufficient contrast for text and selected states.

5. **Performance-first repaint**
   - Prefer targeted invalidation over full-canvas redraw where possible.
   - Cache repeated measurements per paint cycle.

---

## Implementation Constraints (Mandatory)

- **Font sourcing**
  - Always resolve runtime fonts via `BeepThemesManager.ToFont(...)`.
  - Do not use `control.Font` as the primary measurement/render source for calendar text sizing, to avoid DPI/scaling drift.
  - Keep typography mapping centralized in theme/font helpers so Month/Week/Day/List use the same scaling path.

- **Icons and image rendering**
  - Use `StyledImagePainter` for calendar icon/image painting where visual assets are rendered.
  - Resolve icon assets through `IconsManagement` (`Svgs` / `SvgsUI` / icon helpers) instead of ad-hoc drawing paths.
  - Keep icon tinting/state colors theme-driven and consistent with current style painter state (default/hover/selected/disabled).

- **Theme consistency**
  - Pull theme-aware colors and fonts from `BeepThemesManager` and active theme tokens.
  - Avoid hardcoded font-size overrides in render loops that bypass theme scaling behavior.

---

## Enhancement Backlog

## Phase 1 - Visual Foundations (P0)

### CAL-01: Calendar design tokens
- Add/standardize calendar-specific spacing and radius tokens in the style painter metrics.
- Normalize `HeaderHeight`, `DayHeaderHeight`, `EventBarHeight`, `CellPadding`, `SidebarWidth`.
- Apply same token usage across Month/Week/Day/List.

### CAL-02: Typography consistency
- Route default calendar fonts through theme typography conventions.
- Ensure header/day/event/time fonts are visually distinct and consistent by view.
- Define compact and comfortable text scales for dense schedules.

### CAL-03: Header and toolbar alignment
- Improve horizontal distribution of prev/next/today and view mode buttons.
- Add overflow behavior when width is constrained (collapse labels, preserve icons/priority actions).
- Keep title centered in available safe region.

---

## Phase 2 - Interaction and Accessibility (P0)

### CAL-04: Day-cell interaction polish
- Add explicit hover, selected, and keyboard-focused visuals for day cells.
- Ensure "today" remains visible even when selected or hovered.
- Add subtle weekend/out-of-month differentiation without low contrast.

### CAL-05: Event card hierarchy
- Improve event block visual hierarchy (title/time/category accent).
- Support compact truncation and clear overflow indicator style.
- Standardize selected and hovered event visuals across all views.

### CAL-06: Keyboard navigation completeness
- Add deterministic keyboard flow:
  - arrows for day movement,
  - `Home/End` for row bounds,
  - `PageUp/PageDown` for period navigation,
  - `Enter` to open/select event.
- Ensure focused element is always visible and announced via visual state.

### CAL-07: Focus and hit target compliance
- Ensure day cell and event targets meet minimum practical click size.
- Add clear focus outline color from theme focus token.

---

## Phase 3 - Performance and Rendering (P1)

### CAL-08: Targeted invalidation
- Replace broad `Invalidate()` in hover/mouse paths with region invalidation for changed cells/events.
- Track previous hover rect and invalidate only old/new regions.

### CAL-09: Paint-cycle caching
- Cache repeated text measurements and event layout calculations per paint cycle.
- Avoid re-querying event lists repeatedly inside nested loops for the same date/hour.

### CAL-10: Renderer harmonization
- Align behavior between painter system and legacy renderer.
- Ensure both paths produce same spacing and interaction semantics.

---

## Phase 4 - Advanced UX Features (P2)

### CAL-11: Density modes
- Add `Compact / Comfortable` density property.
- Scale row heights, event bars, and sidebar cards via metrics.

### CAL-12: Sidebar event insights
- Improve sidebar event detail card structure (metadata rows, tags, organizer, location).
- Add empty-state messaging and quick-create affordance.

### CAL-13: Mini-calendar UX
- Improve mini-calendar active month/day highlighting and navigation affordance.
- Synchronize selected date feedback with main view instantly.

### CAL-14: List view redesign
- Convert list events into clear card rows with status/category chip, time range, and quick actions.

---

## File-by-File Implementation Map

- `Calendar/BeepCalendar.cs`
  - Toolbar alignment, keyboard/focus orchestration, targeted invalidation orchestration.
- `Calendar/Helpers/CalendarLayoutManager.cs`
  - Spacing tokens, density handling, responsive rect allocation.
- `Calendar/Rendering/ICalendarStylePainter.cs`
  - Expand state/render contract for focus/density/event hierarchy.
- `Calendar/Rendering/CalendarRenderContext.cs`
  - Carry new state and token values through render pipeline.
- `Calendar/Rendering/CalendarRenderer.cs`
  - Harmonize with painter semantics, targeted redraw support.
- `Calendar/Rendering/MonthViewRenderer.cs`
  - Day-cell states, event overflow visual improvements.
- `Calendar/Rendering/WeekViewRenderer.cs`
  - Time-slot readability, overlapping event treatment.
- `Calendar/Rendering/DayViewRenderer.cs`
  - Detail-first event block readability and hit zones.
- `Calendar/Rendering/ListViewRenderer.cs`
  - Card-style list redesign and action affordances.
- `Calendar/Rendering/StylePainters/MaterialCalendarPainter.cs`
  - Material/Figma-like baseline visual language.
- `Calendar/Rendering/StylePainters/MinimalCalendarPainter.cs`
  - Minimal variant preserving structure and contrast.
- `Calendar/Rendering/CommonDrawing.cs`
  - Shared focus ring, truncation badges, event accent helpers.

---

## Acceptance Criteria

- Calendar remains visually coherent in all four views with no clipped text/chips.
- Hover/select/focus states are clear and consistent for day cells and events.
- Keyboard navigation works without visual ambiguity.
- Reduced full-control repaints during hover/mouse movement.
- Sidebar and mini-calendar align with main-view selection state.
- Light and dark themes preserve readability and action discoverability.

---

## Validation Checklist

- Visual checks
  - Month/Week/Day/List with sparse and dense event datasets.
  - Narrow, medium, wide control sizes.
  - Light/dark/high-contrast style combinations.
- Interaction checks
  - Mouse hover transitions over day cells and event cards.
  - Keyboard-only navigation and selection flows.
  - Today/selected/out-of-month state transitions.
- Performance checks
  - Smooth hover movement without visible flicker.
  - No major frame drops when moving across heavily populated month grids.

---

## Suggested Execution Order

1. CAL-01, CAL-02, CAL-03  
2. CAL-04, CAL-05, CAL-06, CAL-07  
3. CAL-08, CAL-09, CAL-10  
4. CAL-11, CAL-12, CAL-13, CAL-14

