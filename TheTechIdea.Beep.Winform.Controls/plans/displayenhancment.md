# BeepDisplayContainer2 Commercial UI/UX Enhancement Plan

## Objective

Refresh `BeepDisplayContainer2` so its tab header and overall display-container experience align with current commercial desktop controls and modern design-system expectations seen in Figma libraries, DevExpress-style desktop products, and current web application frameworks.

This plan focuses especially on the **tab header** because that is the main visual and interaction anchor of the control. The goal is to keep the existing Beep architecture, but raise the quality of spacing, typography, interaction states, layout consistency, and visual polish.

## Design Direction

The target UI should follow these product-level principles:

1. **Clear hierarchy**
   The active tab must read immediately as the current working surface, while inactive tabs remain visible but visually quieter.
2. **Commercial density**
   Spacing should feel intentional and efficient, similar to professional IDE/document hosts, not oversized or decorative.
3. **State clarity**
   Hover, pressed, focused, selected, dragged, and overflow states must be visually distinct.
4. **Theme-native typography**
   Tab typography must come from `BeepThemesManager.ToFont(...)`, not hardcoded fonts or local fallback font creation.
5. **Modern strip cohesion**
   The tab strip, content area, utility buttons, and empty state should feel like one coordinated control surface.
6. **DPI-safe rendering**
   All dimensions, hit targets, spacing, and adornments must remain crisp and correctly sized across DPI scales.

## Visual Benchmark Guidance

The intended visual language should take inspiration from these sources without copying them literally:

1. **DevExpress / commercial WinForms suites**
   Strong active-state legibility, compact measurement, controlled chrome, and high-clarity interaction affordances.
2. **Modern Figma UI kits**
   Consistent spacing systems, clear tokens for state layers, subtle elevation, and typography-driven sizing.
3. **Professional web frameworks**
   Browser-style tab readability, predictable close-button placement, and strong hover/focus behavior.
4. **Attached visual references in the plans folder**
   The visual references suggest segmented surfaces, angled or directional accents, compact banners, and strong label contrast. These should be interpreted as inspiration for hierarchy and state contrast, not as a direct skin.

## Current Architecture to Preserve

The current split is correct and should be preserved:

1. `BeepDisplayContainer2.cs`
   Owns control orchestration, theme application, `TextFont`, and helper coordination.
2. `BeepDisplayContainer2.Layout.cs`
   Owns strip/content layout and tab placement.
3. `BeepDisplayContainer2.Painting.cs`
   Owns top-level rendering composition, indicator animation, empty-state drawing, and helper integration.
4. `DisplayContainers/Helpers/TabLayoutHelper.cs`
   Owns tab measurement and positioning.
5. `DisplayContainers/Helpers/TabPaintHelper.cs`
   Owns tab chrome rendering and state visuals.

The enhancement should not collapse these responsibilities back into one file.

## Typography Standard

### Required Rule

All tab-header typography must use the font resolved from theme typography through `BeepThemesManager.ToFont(...)`.

### Authoritative Flow

The correct source already exists in `BeepDisplayContainer2.ApplyTheme()`:

```csharp
var tabTypography = _currentTheme.TabFont ?? _currentTheme.LabelFont;
var tabFont = BeepThemesManager.ToFont(tabTypography);
TextFont = tabFont;
```

### Plan Requirement

1. `TextFont` remains the single source of truth for tab header measurement and rendering.
2. `TabLayoutHelper` must consume the resolved theme font and stop owning hardcoded `Segoe UI` defaults as its normal behavior.
3. `TabPaintHelper` must render tab text using the same resolved font assumptions used during measurement.
4. Active-tab emphasis must be derived from the same theme family and size, not an unrelated ad-hoc font path.
5. Empty-state text should also be reviewed so it is visually consistent with the tab/header typography system.

## Major UX Gaps to Address

### 1. Tab Header Hierarchy

The active tab should feel like the selected working surface, not merely a colored variant.

Required improvements:

1. Stronger active/inactive contrast.
2. Better relationship between tab fill, border, and indicator.
3. Quieter inactive tabs so the eye naturally lands on the active one.
4. Better visual separation between hovered inactive and selected active states.

### 2. Measurement Consistency

Tab measurement should reflect how premium controls behave under real content load.

Required improvements:

1. Deterministic spacing for text, close button, optional icons, and strip padding.
2. Modern min/max width behavior closer to IDE/document tabs.
3. Stable ellipsis behavior.
4. No visual jump when active tabs become emphasized.
5. Layout assumptions shared between measurement and painting.

### 3. Utility Buttons

Scroll buttons and new-tab buttons should be part of the same product language as the tabs.

Required improvements:

1. Shared metrics with the tab strip.
2. Distinct hover/pressed/focus states.
3. Larger, DPI-safe hit targets.
4. Better alignment with the strip baseline and edge padding.

### 4. Interaction States

State behavior should feel deliberate and premium.

States to explicitly define:

1. Inactive
2. Hovered
3. Pressed
4. Active
5. Keyboard-focused
6. Dragged tab
7. Drop insertion target
8. Close-button hover
9. Scroll-button hover/press
10. New-tab hover/press
11. Overflowed strip

### 5. Empty State Cohesion

When no tabs are open, the control should still feel complete.

Required improvements:

1. The tab strip remains visually anchored.
2. Empty-state text and icon align with the theme typography and spacing system.
3. Background relationships between strip and content remain intentional.

## Proposed Enhancement Model

### Phase 1: Typography and Metrics Foundation

Target files:

1. `BeepDisplayContainer2.cs`
2. `DisplayContainers/Helpers/TabLayoutHelper.cs`
3. `DisplayContainers/Helpers/TabPaintHelper.cs`

Work:

1. Keep `ApplyTheme()` as the single place that resolves `TextFont` using `BeepThemesManager.ToFont(...)`.
2. Remove helper-level dependence on hardcoded `Segoe UI` for normal operation.
3. Introduce a shared internal metric model for:
   text padding, close slot width, utility-button spacing, indicator thickness, vertical insets, and minimum hit sizes.
4. Ensure `TextRenderer.MeasureText(...)` and painted text share the same font expectations.

Outcome:

Consistent typography, stable sizing, and a clean base for visual polish.

### Phase 2: Tab Header Visual Refresh

Target file:

1. `DisplayContainers/Helpers/TabPaintHelper.cs`

Work:

1. Refine each tab style so active tabs read as surfaces, not merely color blocks.
2. Improve active-indicator integration.
3. Reduce visual noise in inactive tabs.
4. Introduce clearer hover and pressed overlays.
5. Improve close-button affordance and balance.
6. Align rounded-corner logic with the strip and content surfaces more cleanly.

Outcome:

Professional tab-header rendering closer to commercial controls.

### Phase 3: Layout and Overflow Polish

Target files:

1. `BeepDisplayContainer2.Layout.cs`
2. `DisplayContainers/Helpers/TabLayoutHelper.cs`

Work:

1. Improve tab-width distribution under constrained width.
2. Make overflow feel intentional rather than clipped.
3. Align scroll buttons and new-tab button with the refined strip metrics.
4. Revisit vertical tab-strip spacing for `Left` and `Right` positions.
5. Keep `AutoTabHeight` driven by text metrics plus scaled padding.

Outcome:

More predictable, product-grade layout behavior across content densities and orientations.

### Phase 4: Interaction and Accessibility Pass

Target files:

1. `BeepDisplayContainer2.Mouse.cs`
2. `BeepDisplayContainer2.Painting.cs`
3. `DisplayContainers/Helpers/TabPaintHelper.cs`

Work:

1. Add or strengthen keyboard focus visuals.
2. Make drag-reorder feedback more explicit.
3. Ensure all interactive elements preserve large enough click/touch targets after DPI scaling.
4. Review contrast and visibility across light, dark, and high-contrast themes.

Outcome:

Interaction behavior that feels deliberate and accessible.

### Phase 5: Empty State and Surface Cohesion

Target files:

1. `BeepDisplayContainer2.Painting.cs`
2. `BeepDisplayContainer2.Theme.cs`

Work:

1. Rework empty-state typography and spacing to align with the theme-resolved font system.
2. Improve the relationship between tab-strip background and content background.
3. Make the no-tab view feel like a finished control state, not just a placeholder.

Outcome:

The control remains visually coherent even with no active tabs.

## Specific Technical Recommendations

### A. Centralize Tab Metrics

Add an internal metrics model used by both `TabLayoutHelper` and `TabPaintHelper` for:

1. Horizontal padding
2. Vertical padding
3. Close-button slot width
4. Utility-button slot width
5. Indicator thickness
6. Corner radius rules
7. Minimum tab width
8. Maximum tab width
9. Gap between tabs

This avoids paint/layout divergence.

### B. Centralize Active Font Emphasis

If active tabs remain bold or semi-bold, derive that emphasis from the already resolved theme font family and size. The theme font should remain visually authoritative.

### C. Replace Helper Fallbacks Gradually

`TabLayoutHelper` currently carries hardcoded font fallback behavior. That should be converted from the normal path into a defensive last-resort path only.

### D. Use DPI Scaling Everywhere for Header Chrome

All of the following must remain scaled:

1. Padding
2. Button sizes
3. Indicator thickness
4. Border thickness
5. Hover outline thickness
6. Focus ring spacing
7. Drag insertion markers

## Validation Checklist

### Visual Validation

1. Active tab clearly stands out.
2. Inactive tabs remain readable but unobtrusive.
3. Hover and pressed states are distinct.
4. Close buttons feel aligned and intentional.
5. Utility buttons belong visually to the strip.
6. Empty state feels integrated.

### Typography Validation

1. `BeepThemesManager.ToFont(...)` remains the authoritative source of tab typography.
2. `TextFont` is used consistently in layout and paint.
3. No new hardcoded font family is introduced for tab headers.
4. Active emphasis does not break measurement.

### Behavior Validation

1. Top tab strip
2. Bottom tab strip
3. Left tab strip
4. Right tab strip
5. Overflowing tabs
6. Drag reorder
7. Keyboard navigation
8. Empty state

### Theme and DPI Validation

1. Light themes
2. Dark themes
3. High-contrast themes
4. 100% DPI
5. 125% DPI
6. 150% DPI
7. 200% DPI

## Success Criteria

This enhancement is complete when:

1. The tab header looks and behaves like a premium commercial control.
2. Typography is consistently theme-driven through `BeepThemesManager.ToFont(...)`.
3. Layout and paint use the same measurement assumptions.
4. Interaction states are explicit and polished.
5. The strip, content area, and utility actions feel visually unified.
6. The control holds up across themes, DPI scales, and tab positions.

## Recommended Next Implementation Order

1. Typography and helper cleanup
2. Shared metrics model
3. TabPaintHelper visual refresh
4. Layout/overflow alignment
5. Interaction-state polish
6. Empty-state cohesion

## Primary Files for the Actual Work

1. `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\DisplayContainers\BeepDisplayContainer2.cs`
2. `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\DisplayContainers\BeepDisplayContainer2.Painting.cs`
3. `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\DisplayContainers\BeepDisplayContainer2.Layout.cs`
4. `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\DisplayContainers\BeepDisplayContainer2.Theme.cs`
5. `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\DisplayContainers\BeepDisplayContainer2.Mouse.cs`
6. `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\DisplayContainers\Helpers\TabPaintHelper.cs`
7. `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\DisplayContainers\Helpers\TabLayoutHelper.cs`
