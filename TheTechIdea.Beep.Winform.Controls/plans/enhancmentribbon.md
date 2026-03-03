# BeepRibbon Enhancement Plan (DevExpress + Fluent/Figma Aligned)

## Goal
Upgrade `Ribbon` to a modern, production-grade command surface aligned with current enterprise ribbon UX patterns (DevExpress, Telerik, Syncfusion) and design-system workflows (Fluent/Figma).

## Implementation Status (Current)
- Done: Phase 1-7 core APIs in `BeepRibbonControl` (layout modes, density, QAT personalization/persistence, backstage structure, search/key tips, theme tokens, contextual + merge readiness).
- Done: Commercial-like minimize flow with tab double-click toggle + minimized tab popup menu surface.
- Done: Backstage recents/pinned management helpers (`AddBackstageRecentItem`, `PinBackstageItem`, `UnpinBackstageItem`, clear APIs).
- Done: Search enhancements with provider fallback telemetry (`SearchExecuted`) and optional backstage search scope.
- Pending: true in-ribbon gallery controls and richer runtime customization UI surface (dialog/panel) for tab/group reorder UX.

## Current State (Code Snapshot)
- `Ribbon/BeepRibbonControl.cs`: custom control with `TabControl` + `ToolStrip` quick access + basic backstage dropdown + basic contextual group bands.
- `Ribbon/BeepRibbonGroup.cs`: simple `ToolStrip` group with large/small button helpers.
- `Ribbon/RibbonTheme.cs`: minimal color set, no density/token/state system.
- `Ribbon/RibbonThemeMapper.cs`: basic light/dark mapping.

## Mandatory Implementation Constraints (Project-Specific)
1. Font resolution:
- Use `BeepThemesManager.ToFont(...)` for all Ribbon fonts.
- Do not create ribbon fonts with ad-hoc `new Font(...)` unless wrapped through theme typography conversion.
- Base font tokens must come from `IBeepTheme` typography styles where possible.

2. Image/icon rendering:
- Use `Styling/ImagePainters/StyledImagePainter.cs` as the icon rendering/tint/shape pipeline.
- Use `IconsManagement` (`SvgsUI`, `Svgs`, `SvgsDatasources`, etc.) as the canonical icon path source.
- Avoid direct unmanaged image paint logic when `StyledImagePainter` supports the scenario.

3. Theme contract:
- Ribbon visual tokens must map from `TheTechIdea.Beep.Vis.Modules2.0/IBeepTheme.cs`.
- Keep `RibbonThemeMapper` as adapter from `IBeepTheme` to Ribbon-specific tokens.

4. Command/menu item model:
- Use `TheTechIdea.Beep.Vis.Modules2.0/SimpleItem.cs` as the command node model where practical.
- Use `SimpleItem.Children` to represent nested menu trees (Backstage sections, split buttons, overflow menus, context menus).

5. Internal guidance references to follow:
- `.cursor/skills/beep-winform/reference.md`
- `.cursor/skills/beep-controls-usage/reference.md`
- `.cursor/skills/beep-controls-usage/BeepRibbon-skill.md` (for usage intent; implementation should align with current codebase reality)
- `.cursor/skills/beep-controls-usage/BeepListBox-skill.md` (for `SimpleItem` list/tree + `ImagePath` usage pattern)

## SimpleItem Usage Pattern (Aligned with Existing Controls)
Use `SimpleItem` as the ribbon command node and keep icon paths in `ImagePath` exactly as done in other controls.

Conventions:
- `SimpleItem.Text` (or `DisplayField` when bound) -> visible command label.
- `SimpleItem.ImagePath` -> icon path (SVG preferred, from `IconsManagement` constants where possible).
- `SimpleItem.ToolTip` -> ribbon tooltip/super-tip body.
- `SimpleItem.ShortcutText` / `SimpleItem.Shortcut` -> key-tip or shortcut hint text.
- `SimpleItem.IsEnabled`, `SimpleItem.IsVisible`, `SimpleItem.IsChecked` -> command state.
- `SimpleItem.BadgeText`, `BadgeBackColor`, `BadgeForeColor` -> optional badges in ribbon items.
- `SimpleItem.Children` -> submenu commands, split button secondary actions, backstage navigation hierarchies.

Hierarchy examples:
- Tab node -> `Children` = groups.
- Group node -> `Children` = commands.
- Command node with flyout -> `Children` = menu/flyout items.

## Target UX Baseline (What We Align To)
1. Dual command layouts:
- Classic (multi-row grouped ribbon).
- Simplified/compact single-row mode with overflow.

2. Strong command discoverability:
- Search box in ribbon header (`Ctrl+F` focus behavior).
- Key Tips (`Alt` navigation overlay).
- Contextual tabs with explicit color bands and activation rules.

3. Personalization:
- Runtime ribbon customization (tabs/groups/QAT).
- QAT add/remove/reorder + above/below placement.
- Persistent layout save/restore with version-safe migration.

4. Backstage quality:
- Full backstage panel structure (left nav, right content pane, recent items/tasks).

5. Figma/Design-system compatibility:
- Componentized variant model (size/state/layout mode).
- Tokenized colors/typography/spacing/radius/elevation.
- Light/dark/high-contrast modes from one token source.

## Architecture Plan

### Phase 1: Ribbon Domain Model and API Hardening
Create explicit models/enums so behavior is deterministic and designer-friendly.

Add:
- `RibbonLayoutMode` (`Classic`, `Simplified`).
- `RibbonDensity` (`Comfortable`, `Compact`, `Touch`).
- `RibbonCommandPriority` (`Primary`, `Secondary`, `OverflowOnly`).
- `RibbonSearchMode` (`Off`, `Local`, `SmartService`).
- `RibbonPersonalizationOptions` (QAT + tab/group customization flags).
- `RibbonCommandNode` adapter (or direct use) backed by `SimpleItem`.

Model contracts:
- Tabs/groups/items should be serializable from `SimpleItem` tree.
- Child commands, gallery items, backstage submenus, and overflow items should come from `SimpleItem.Children`.

Files:
- `Ribbon/BeepRibbonControl.cs`
- `Ribbon/RibbonTheme.cs`
- `TheTechIdea.Beep.Vis.Modules2.0/SimpleItem.cs` (consumption/integration, not modification by default)
- new `Ribbon/Models/*` and `Ribbon/Enums/*`

### Phase 2: Modern Layout Engine (Classic + Simplified)
Replace fixed `ToolStrip` assumptions with layout logic that can collapse/overflow groups.

Implement:
- Group-level reduction rules (Large -> Small -> Popup).
- Simplified mode one-row command strip.
- Overflow popup for hidden commands.
- Resize breakpoints and stable ordering.

Files:
- `Ribbon/BeepRibbonControl.cs`
- `Ribbon/BeepRibbonGroup.cs`
- new `Ribbon/Layout/*`

### Phase 3: QAT and Runtime Customization
Bring customization behavior up to enterprise level.

Implement:
- Right-click: Add to QAT / Remove from QAT.
- QAT above/below ribbon.
- Ribbon customization dialog model (tabs/groups visibility, order).
- Safe persistence with schema version (`json` recommended over plain text labels).

Files:
- `Ribbon/BeepRibbonControl.cs`
- new `Ribbon/Customization/*`

### Phase 4: Backstage 2.0
Elevate backstage from simple dropdown host to structured workspace.

Implement:
- Left navigation sections (Info, New, Open, Save, Print, Options, Exit).
- Right content panel templating.
- Recent items + pinned items support.
- Backstage command contract/events.
- Data source model backed by `SimpleItem` nodes and `Children`.

Files:
- `Ribbon/BeepRibbonControl.cs`
- new `Ribbon/Backstage/*`

### Phase 5: Search, Key Tips, Accessibility
Improve discoverability and keyboard-first operation.

Implement:
- Ribbon search box with tags/keywords and command metadata.
- Optional smart-search provider interface (local first, service optional).
- Alt-key Key Tips overlay.
- Tab order map, focus rectangles, screen reader names/roles.
- WCAG contrast validation against ribbon theme tokens.

Files:
- `Ribbon/BeepRibbonControl.cs`
- `Ribbon/RibbonTheme.cs`
- new `Ribbon/Accessibility/*`, `Ribbon/Search/*`

### Phase 6: Visual System and Figma Token Pipeline
Make UI consistent, themeable, and design-tool aligned.

Implement:
- Extend `RibbonTheme` into token groups:
  - Surface/background layers
  - Text/icon roles
  - Border/separator
  - Hover/pressed/selected/focus
  - Elevation and corner radius
- Typography scale (caption, tab, group header, command label).
- Density-based sizing and spacing.
- Token import contract (JSON) compatible with Figma variables naming.
- Typography conversion must go through `BeepThemesManager.ToFont(...)`.
- Typography source should prefer `IBeepTheme` styles (`ButtonFont`, `HeaderFont`, etc. where semantically appropriate).
- Icon rendering for tokens/states should route via `StyledImagePainter` + `IconsManagement` paths.

Files:
- `Ribbon/RibbonTheme.cs`
- `Ribbon/RibbonThemeMapper.cs`
- new `Ribbon/Tokens/*`

### Phase 7: Contextual Tabs and MDI Merge Readiness
Strengthen advanced app-shell scenarios.

Implement:
- Contextual group activation rules by document context.
- Animation for contextual tab appearance/disappearance (subtle, optional).
- Child/parent ribbon merge contract (if app uses MDI/document host).

Files:
- `Ribbon/BeepRibbonControl.cs`
- new `Ribbon/Merging/*` (if needed)

## Implementation Order (Recommended)
1. Phase 1 (API/model foundation)
2. Phase 2 (layout engine)
3. Phase 3 (customization/QAT persistence)
4. Phase 4 (backstage)
5. Phase 5 (search, key tips, accessibility)
6. Phase 6 (tokens + Figma alignment)
7. Phase 7 (contextual tab polish + merge support)

## Acceptance Criteria
- Ribbon supports both `Classic` and `Simplified` modes at runtime.
- QAT supports add/remove/reorder and above/below placement with persistence.
- Backstage has structured sections and event-driven content.
- Search can find commands by text + tags; keyboard focus shortcut works.
- Key Tips available on `Alt`.
- Contextual tabs are deterministic and theme-compliant.
- Theme tokens drive all visual states; light/dark/high-contrast supported.
- Accessibility checks pass: focus visibility, keyboard-only operation, readable contrast.
- Ribbon command trees are represented by `SimpleItem` with `Children` for nested structures.
- Ribbon font creation paths use `BeepThemesManager.ToFont(...)` consistently.
- Ribbon icons render through `StyledImagePainter` with `IconsManagement` sources.

## Non-Goals (for this pass)
- Full AI smart search implementation (only provider contract and local search in scope).
- Pixel-perfect cloning of any one vendor skin.
- Complex animation system beyond essential transitions.

## Risks and Mitigations
- Risk: layout regressions on resize.
  - Mitigation: snapshot render tests at fixed widths.
- Risk: personalization file incompatibility across versions.
  - Mitigation: versioned schema and migration path.
- Risk: theme inconsistency across controls.
  - Mitigation: token-role naming aligned with existing `BeepTheme` roles.

## Deliverables
1. Upgraded ribbon API (layout/density/search/customization options).
2. New layout/reduction engine with overflow.
3. QAT and ribbon customization workflows + persistence.
4. Structured backstage framework.
5. Key tips + search + accessibility improvements.
6. Tokenized theme layer compatible with Figma-style component workflows and `IBeepTheme`.
7. `SimpleItem` (`Children`)-based command data model integration.
8. `BeepThemesManager.ToFont(...)`-based typography pipeline and `StyledImagePainter` icon pipeline.

## Reference Links (Current UX Baselines)
- DevExpress WinForms Ribbon overview (updated May 24, 2025): https://docs.devexpress.com/WindowsForms/2500/controls-and-libraries/ribbon-bars-and-menu/ribbon
- DevExpress `CommandLayout` (Classic/Simplified): https://docs.devexpress.com/WindowsForms/DevExpress.XtraBars.Ribbon.RibbonControl.CommandLayout
- DevExpress Search Menu: https://docs.devexpress.com/WindowsForms/400621/controls-and-libraries/ribbon-bars-and-menu/ribbon/visual-elements/search-menu
- DevExpress Runtime Customization: https://docs.devexpress.com/WindowsForms/11528/controls-and-libraries/ribbon-bars-and-menu/ribbon/runtime-capabilities/runtime-customization
- DevExpress Quick Access Toolbar: https://docs.devexpress.com/WindowsForms/2496/Controls-and-Libraries/Ribbon-Bars-and-Menu/Ribbon/Visual-Elements/Quick-Access-Toolbar
- Telerik WinForms Ribbon overview: https://docs.telerik.com/devtools/winforms/controls/ribbonbar/overview
- Syncfusion WinForms Ribbon overview (Feb 4, 2025): https://help.syncfusion.com/windowsforms/ribbon/overview
- Syncfusion WinForms QAT: https://help.syncfusion.com/windowsforms/ribbon/quick-access-toolbar
- Microsoft Command Bar guidance (updated 2025): https://learn.microsoft.com/en-us/windows/apps/design/controls/command-bar
- Microsoft Keyboard Accessibility: https://learn.microsoft.com/en-us/windows/apps/design/accessibility/keyboard-accessibility
- Figma Auto Layout guide: https://help.figma.com/hc/en-us/articles/360040451373-Explore-auto-layout-properties
- Figma Variants: https://help.figma.com/hc/en-us/articles/360056440594-Create-and-use-variants
- Figma Interactive Components: https://help.figma.com/hc/en-us/articles/360061175334-Create-interactive-components-with-variants
- Figma Publish Library: https://help.figma.com/hc/en-us/articles/360025508373-Publish-styles-and-components
- Figma Variables Modes: https://help.figma.com/hc/en-us/articles/15343816063383-Modes-for-variables
