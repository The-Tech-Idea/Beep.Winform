# Phase 2: Overflow, Header Actions, and Rich Tabs

## Objective

Once the page/content model is stable, deliver the rich header and overflow behavior users expect from commercial tab controls: adaptive overflow, first-class header action slots, richer per-tab metadata, and a render contract that is consistent across all painter styles.

## In Scope

- overflow policies and overflow popup behavior
- header action slots
- rich per-tab header content (icon, badge, subtext, dirty, busy, status)
- painter/render contract unification
- style recipes for document, workspace, and application-tab scenarios

## Out of Scope

- document/workspace behavioral policies such as pinned and preview rules
- docking/tear-out implementation
- full design-time editing of the new metadata model

## Primary File Targets

- `Tabs/Hosts/BeepTabHeaderHost.Painting.cs`
- `Tabs/Hosts/BeepTabHeaderHost.Mouse.cs`
- `Tabs/Hosts/BeepTabHeaderHost.Overflow.cs`
- `Tabs/Models/BeepTabItem.cs`
- `Tabs/Models/BeepTabHeaderAction.cs`
- `Tabs/Models/BeepTabOverflowPolicy.cs`
- `Tabs/Models/BeepTabAdornmentState.cs`
- `Tabs/Models/BeepTabRenderContext.cs`
- `Tabs/Helpers/BeepTabHeaderActionRouter.cs`
- `Tabs/Helpers/BeepTabOverflowCoordinator.cs`
- `Tabs/Helpers/BeepTabAdornmentLayoutHelper.cs`
- `Tabs/Helpers/TabIconHelpers.cs`
- `Tabs/Painters/ITabPainter.cs`
- `Tabs/Painters/BaseTabPainter.cs`
- `Tabs/Painters/*TabPainter.cs`

## Dependencies

Phase 2 assumes Phase 1 has completed the page-model cutover. Overflow and rich metadata should not be polished on top of unstable `TabPage`-shaped hosting semantics.

## Workstreams

### 1. Standardize The Render Contract

Unify painters around a single item-based render path built from:

- `BeepTabItem`
- `BeepTabHeaderItemLayout`
- `BeepTabAdornmentState`
- `BeepTabRenderContext`

The goal is one authoritative way to render a tab, regardless of painter style.

### 2. Deliver Explicit Overflow Policies

Support commercial-style overflow strategies:

- `None`
- `ScrollButtons`
- `OverflowMenu`
- `ShrinkToFit`
- `Multiline` or `Wrap` only if the final layout contract supports it cleanly

Overflow calculations must operate on premium tab items and action-slot geometry, not on stock native-tab assumptions.

### 3. Promote Header Action Slots

Formalize header actions as first-class UI elements, including at minimum:

- add page
- overflow/more
- close current
- scroll backward
- scroll forward

These actions need reusable layout, hover/press state, paint behavior, and command routing.

### 4. Finish Rich Header Metadata

Support commercial-quality header composition:

- icon path
- main title
- optional subtext
- badge text or badge kind
- dirty marker
- busy indicator
- status/adornment slots
- close visibility override

Metadata should already be coming from the Phase 1 page model. Phase 2 focuses on layout and rendering quality.

### 5. Define Deterministic Space-Priority Rules

When space is constrained, apply one shared fallback order:

1. keep the selected tab visible
2. keep required actions visible
3. keep close affordances on selected/dirty tabs first
4. collapse subtext before main text
5. collapse labels before icons only when configured
6. move remaining items into overflow according to the chosen policy

### 6. Document Style Recipes

Each tab style should have a documented behavioral recipe, not just a paint variation.

Examples:

- DevExpress-like document tabs
- VS Code-like workbench tabs
- Material/Ant-style application tabs
- segmented/button-style navigation tabs

## Deliverables

1. Unified item-based painter/render contract.
2. Stable overflow coordinator and popup behavior.
3. Reusable header action slots.
4. Rich-header layout that supports icon/badge/subtext/dirty/busy without collisions.
5. Style recipes documented in terms of behavior as well as appearance.

## Acceptance Criteria

- Overflow remains deterministic across all header positions.
- Header actions are independently hit-testable and theme-aware.
- Rich tab metadata renders without layout collisions.
- Painter implementations consume the same render contract.
- No Phase 2 work reintroduces `TabPage` into the premium-facing model.

## Risks And Mitigations

- Risk: richer headers create painter-specific layout hacks.
  Mitigation: centralize adornment bounds and overflow partitioning before painter-specific drawing.

- Risk: overflow and action slots behave differently by orientation.
  Mitigation: keep overflow as a logical header-run problem first, then map it to orientation-specific coordinates.