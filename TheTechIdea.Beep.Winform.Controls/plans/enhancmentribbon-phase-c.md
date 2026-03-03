# BeepRibbon Phase C Implementation Plan (Commercial Parity)

## Date
- February 28, 2026

## Objective
Deliver a production-grade WinForms ribbon experience aligned with current DevExpress/Telerik/Syncfusion UX patterns and Figma token/component standards, building on completed Phase A/B foundations.

## Non-Negotiable Constraints
- Use `BeepThemesManager.ToFont(...)` for all ribbon font creation.
- Use `StyledImagePainter` for ribbon/backstage icon rendering.
- Use `SimpleItem` + `SimpleItem.Children` as the command tree contract.
- Keep `RibbonThemeMapper` as the bridge from `IBeepTheme` to ribbon tokens.

## Phase C Workstreams

### C1. Runtime Ribbon Customization Surface
Status (current):
- Implemented baseline in code:
  - `Ribbon/Customization/RibbonCustomizationDialog.cs`
  - `Ribbon/Customization/RibbonCustomizationModels.cs`
  - `Ribbon/BeepRibbonControl.cs` wiring (`ShowCustomizeRibbonDialog`, capture/apply APIs, reset/default baseline events).
- `Ribbon/Customization/RibbonCustomizationDialog.cs` updates:
  - Added command filters in Quick Access page (`Category`/`Group` + text search) for large command trees.
- `Ribbon/Customization/RibbonCustomizationModels.cs` + `Ribbon/BeepRibbonControl.cs` updates:
  - Added category metadata (`TabText`/`GroupText`) in command projection to support runtime filtering in customization UI.
- `Ribbon/BeepRibbonControl.cs` updates:
  - Added reusable `CreateCustomizeRibbonMenuItem(...)` helper for host-form/designer context menu wiring.
- Remaining polish:
  - None for planned C1 scope.

File-by-file tasks:
- `Ribbon/BeepRibbonControl.cs`
  - Add `ShowCustomizeRibbonDialog()` entry point and event contracts for apply/reset/cancel.
  - Add model projection helpers for tabs/groups/QAT into dialog view models.
  - Add reset-to-default layout API using merge/base snapshot.
- `Ribbon/RibbonEnums.cs`
  - Add enums for customization operations/state (for telemetry + deterministic command routing).
- `Ribbon/Customization/RibbonCustomizationDialog.cs` (new)
  - WinForms dialog with: tab visibility, group visibility, tab/group reordering, reset defaults.
- `Ribbon/Customization/RibbonCustomizationModels.cs` (new)
  - Strongly typed DTO/view models for dialog binding and persistence roundtrip.

Acceptance criteria:
- End users can reorder tabs/groups and toggle visibility without code.
- Dialog apply updates live ribbon and persists through `SaveCustomizationTo/LoadCustomizationFrom`.
- Reset restores baseline model and QAT in one action.
- No ad-hoc font creation in customization UI.

### C2. Ribbon Galleries and Rich Command Variants
Status (current):
- Foundation added in code:
  - `Ribbon/Gallery/BeepRibbonGallery.cs`
  - `Ribbon/Gallery/RibbonGalleryRenderer.cs`
- `Ribbon/BeepRibbonControl.cs` updates:
  - Gallery command detection and in-group gallery host rendering.
  - Gallery selection routes command invocation through existing `CommandInvoked` event.
  - Gallery theme/density propagation in ribbon theme pass.
  - Gallery overflow `More` menu for item sets larger than inline tile capacity.
  - Gallery state persistence hooks (`pinned/recent`) wired in `BeepRibbonControl` for rebuild-safe UX.
- `Ribbon/Gallery/BeepRibbonGallery.cs` updates:
  - Added grouped section headers in overflow menu (`GroupName` / category aware).
  - Added runtime pinned + recent sets with right-click context pin/unpin.
  - Added optional large popup gallery preview dialog with full-item surface.
- `Ribbon/Gallery/BeepRibbonGallery.cs` keyboard updates:
  - Added arrow-key selection navigation, enter activation, and escape handling.
  - Added first-item focus behavior when gallery overflow menu opens.
- `Ribbon/Gallery/BeepRibbonGallery.cs` accessibility/focus updates:
  - Added accessible role/name/description metadata for gallery surface and tile buttons.
  - Added selected-tile focus syncing for keyboard navigation and Enter/focus entry.
  - Added accessible metadata for overflow "More" gallery button.
- `Ribbon/Gallery/BeepRibbonGallery.cs` keyboard traversal updates:
  - Added Tab/Shift+Tab traversal hand-off for composite/nested host containers.
  - Added child-tile key delegation + configurable tab-stop behavior (`TabNavigatesOutOfGallery`, `ChildTileTabStops`).
- Remaining:
  - None for planned C2 scope.

File-by-file tasks:
- `Ribbon/BeepRibbonControl.cs`
  - Add `SimpleItem` flags/metadata interpretation for gallery commands.
  - Add command routing for gallery item selection and preview.
- `Ribbon/BeepRibbonGroup.cs`
  - Add host support for gallery-style item blocks (small tile set + popup view).
- `Ribbon/Gallery/BeepRibbonGallery.cs` (new)
  - Gallery control with sections/groups, selected/recent item support.
- `Ribbon/Gallery/RibbonGalleryRenderer.cs` (new)
  - Theme-aware rendering via `RibbonTheme` tokens.

Acceptance criteria:
- Gallery commands render and select from `SimpleItem.Children`.
- Gallery works in classic and simplified layouts.
- Keyboard navigation (arrow keys, enter, escape) works in gallery popup.

### C3. Super Tooltips and Command Metadata UX
Status (current):
- Foundation added in code:
  - `Ribbon/Tooltips/RibbonSuperTooltipModel.cs`
  - `Ribbon/Tooltips/RibbonSuperTooltip.cs`
- `Ribbon/BeepRibbonControl.cs` updates:
  - Added `UseSuperToolTips` and `SuperTooltipDurationMs`.
  - Command hover now supports rich tooltip content (title/body/shortcut/footer) from `SimpleItem` metadata.
  - Added `SuperTooltipModelProvider` for per-command tooltip model customization.
- `Ribbon/Tooltips/RibbonSuperTooltip.cs` updates:
  - Added optional icon rendering from `ImagePath` using `StyledImagePainter`.
- `Ribbon/Tooltips/RibbonSuperTooltipModel.cs` updates:
  - Added preview metadata support (`preview:`/`thumbnail:` tokens in `SubText3`) and image URI fallback.
- `Ribbon/Tooltips/RibbonSuperTooltip.cs` updates:
  - Added optional rich media thumbnail panel + caption rendering in tooltip body.
- `Ribbon/BeepRibbonControl.cs` updates:
  - Added `TooltipActionRequested` hook (`F1` on hovered command) for app-level rich tooltip actions/help preview workflows.
- Remaining:
  - Optional default action implementation policy (e.g., auto-open local preview) remains application-specific.

File-by-file tasks:
- `Ribbon/BeepRibbonControl.cs`
  - Add super-tooltip host and command hover pipeline.
  - Map `SimpleItem` text/subtext/shortcut/help into structured tooltip payload.
- `Ribbon/Tooltips/RibbonSuperTooltip.cs` (new)
  - Reusable tooltip UI with title/body/shortcut/footer and optional image.
- `Ribbon/Tooltips/RibbonSuperTooltipModel.cs` (new)
  - Metadata model with theme-aware typography roles.

Acceptance criteria:
- Commands can display rich tooltips (not plain ToolTipText only).
- Tooltip content supports title, description, shortcut, and optional help/action hint.
- Tooltip fonts/colors come from `RibbonTheme` + `BeepThemesManager.ToFont(...)`.

### C4. Backstage 3.0 Templates and Recents UX
Status (current):
- Foundation added in code:
  - `Ribbon/Backstage/RibbonBackstageTemplateBuilder.cs`
  - `Ribbon/Backstage/RibbonRecentItemModel.cs`
  - `Ribbon/BeepRibbonControl.cs` APIs (`LoadStandardBackstageTemplate`, `BindBackstageRecentItems`).
  - `Ribbon/BeepRibbonControl.cs` footer action support via `BackstageFooterItems`.
  - Backstage action right-click pin/unpin affordances for recent/pinned items.
- `Ribbon/BeepRibbonControl.cs` updates:
  - Added inline pin/unpin button in backstage action rows (row panel + dedicated pin action).
  - Improved backstage footer renderer sizing + icon support from `ImagePath`.
  - Added MRU timestamp visual treatment in backstage action rows (relative time labels).
  - Added timestamp display options (`BackstageShowTimestamps`, `BackstageUseRelativeTimestamps`).
- `Ribbon/BeepRibbonControl.cs` updates:
  - Added `BackstageTimestampFormatter` delegate hook (`Func<SimpleItem, DateTime, string>`) to allow app-specific timestamp formatting per item.
- Remaining:
  - None for planned C4 scope.

File-by-file tasks:
- `Ribbon/BeepRibbonControl.cs`
  - Add section templates: `Info`, `Open`, `Save`, `Print`, `Options`, `Exit`.
  - Add pinned/recent visual grouping with pin/unpin actions and optional MRU timestamps.
  - Add bottom command area support (Options/Account/Exit style).
- `Ribbon/Backstage/RibbonBackstageTemplateBuilder.cs` (new)
  - Helpers to generate baseline backstage trees from `SimpleItem` contracts.
- `Ribbon/Backstage/RibbonRecentItemModel.cs` (new)
  - Recent/pinned metadata for display and state transitions.

Acceptance criteria:
- Backstage supports enterprise structure with deterministic section rendering.
- Pin/unpin and recent trimming rules are configurable and stable.
- Nested backstage menus continue to use `SimpleItem.Children`.

### C5. Advanced Search (Local + Smart Provider)
Status (current):
- Foundation added in code:
  - `Ribbon/Search/RibbonSearchRanking.cs`
  - `Ribbon/Search/RibbonSearchIndex.cs`
  - `Ribbon/Search/IRibbonSearchTelemetry.cs`
- `Ribbon/BeepRibbonControl.cs` updates:
  - Ranked local search using reusable search index/ranking.
  - Keyboard navigation for results (`Up/Down/Enter/Escape`) from ribbon search box.
  - Search history + command usage boost + optional telemetry sink.
  - Added recent-query suggestions in search drop-down when query is empty.
- `Ribbon/Search/RibbonSearchRanking.cs` updates:
  - Added fuzzy match scoring branch.
- `Ribbon/BeepRibbonControl.cs` updates:
  - Added search-history management actions (`Manage history...`, `Clear history`) in dropdown.
  - Added right-click remove on individual history entries.
- `Ribbon/Search/RibbonSearchRanking.cs` updates:
  - Upgraded fuzzy scoring with distance-based scoring + token fuzzy + subsequence fallback.
- `Ribbon/BeepRibbonControl.cs` updates:
  - Added `SearchMaxResults` runtime tuning knob (persisted via customization save/load) for large-vocabulary command sets.
- `Ribbon/Search/RibbonSearchRanking.cs` + `Ribbon/Search/RibbonSearchIndex.cs` updates:
  - Added optional score-boost provider hook for host-controlled ranking calibration.
- `Ribbon/BeepRibbonControl.cs` updates:
  - Added category/group search boost APIs (`SetSearchCategoryBoost`, `ClearSearchCategoryBoosts`) and integrated boost resolution into local ranking.
- `Ribbon/Search/RibbonSearchBenchmark.cs` + `Ribbon/BeepRibbonControl.cs` updates:
  - Added built-in local search benchmark harness (`RunLocalSearchBenchmark`) with per-query timings + aggregate P95/average metrics.
- Remaining:
  - Execute benchmark runs against real production-scale command datasets.

File-by-file tasks:
- `Ribbon/BeepRibbonControl.cs`
  - Add weighted ranking (exact text > starts with > contains > tooltip/tags).
  - Add keyboard-first search UX: enter execute top match, arrows for result list.
  - Add optional search history/recent queries.
- `Ribbon/Search/RibbonSearchIndex.cs` (new)
  - Build/cache search index from ribbon + optional backstage items.
- `Ribbon/Search/RibbonSearchRanking.cs` (new)
  - Scoring logic and match descriptors.
- `Ribbon/Search/IRibbonSearchTelemetry.cs` (new)
  - Optional telemetry interface for search mode/provider/fallback metrics.

Acceptance criteria:
- Local search feels instant on large command trees.
- Smart provider failures always fall back to local deterministically.
- Search results are ranked and keyboard executable.

### C6. Accessibility, Keyboard, and RTL Compliance
Status (current):
- Foundation added in code:
  - `Ribbon/Accessibility/RibbonAccessibilityHelper.cs`
  - `Ribbon/Accessibility/RibbonKeyboardMap.cs`
- `Ribbon/BeepRibbonControl.cs` updates:
  - Added `KeyboardMap` with default `F6` / `Shift+F6` pane traversal registration.
  - Centralized command accessibility assignment through helper.
  - Added base control accessibility metadata for key ribbon/backstage surfaces.
  - Added `RibbonRightToLeft` propagation to tabs/QAT/backstage surfaces.
- `Ribbon/BeepRibbonControl.cs` updates:
  - Expanded pane traversal to include search and backstage footer surfaces.
  - Added role-aware accessibility mapping for command items, gallery hosts, backstage actions/footer, and search menus.
  - Added deeper RTL propagation into tab pages, ribbon group hosted controls, and backstage child controls.
  - Added explicit pane tab-order tuning hooks for search/ribbon/backstage surfaces.
- `Ribbon/Accessibility/RibbonAccessibilityAudit.cs` + `Ribbon/BeepRibbonControl.cs` updates:
  - Added programmatic accessibility audit report support (`RunAccessibilityAudit`) to identify missing accessible names/roles across control tree and toolstrip items.
- Remaining:
  - End-to-end screen-reader audit with NVDA/Narrator across full app integration scenarios.

File-by-file tasks:
- `Ribbon/BeepRibbonControl.cs`
  - Add F6 pane traversal behavior and stronger focus order.
  - Improve accessible names/roles/descriptions for ribbon groups and backstage controls.
  - Add RTL layout switching for tabs/groups/QAT/backstage.
- `Ribbon/Accessibility/RibbonAccessibilityHelper.cs` (new)
  - Centralized accessibility metadata and focus outline helpers.
- `Ribbon/Accessibility/RibbonKeyboardMap.cs` (new)
  - Key map registry (`Alt`, key tips, F6, arrow navigation, escape collapse).

Acceptance criteria:
- Keyboard-only users can reach all actionable commands.
- Accessible metadata is present for all major interactive elements.
- RTL mode renders correctly without command overlap.

### C7. Figma Token Pipeline and Variant Matrix
Status (current):
- Foundation added in code:
  - `Ribbon/Tokens/RibbonTokenImporter.cs`
  - `Ribbon/Tokens/RibbonVariantMatrix.cs`
- `Ribbon/BeepRibbonControl.cs` updates:
  - Added `VariantMatrix` property.
  - Added `LoadThemeFromTokenFile(...)` for token-driven theme loading.
  - Added `ApplyVariant(...)` for `layout x density x state` variant application.
  - Added token reference/alias resolution support for `$value` and `{token.path}` style references.
- `Ribbon/Tokens/RibbonTokenImporter.cs` updates:
  - Added diagnostics-capable import APIs (`ImportWithDiagnosticsFromJson/File`).
  - Added flattening for nested DTCG token structures and mode token overlays.
  - Added validation diagnostics for unresolved references, likely type mismatches (color/numeric), and cycle detection.
  - Added reference parsing for both `{token.path}` and `$token.path` alias forms.
- `Ribbon/BeepRibbonControl.cs` updates:
  - Added `LastTokenImportDiagnostics` surface to expose token import validation output.
  - `LoadThemeFromTokenFile(...)` now stores diagnostics from importer.
- `Ribbon/Tokens/RibbonTokenImporter.cs` updates:
  - Added mode-graph validation pass for inheritance across all modes (missing parent-mode diagnostics + graph-cycle diagnostics), not only selected-mode merge path checks.
- Remaining:
  - None for planned C7 scope.

File-by-file tasks:
- `Ribbon/RibbonTheme.cs`
  - Add missing visual tokens: elevation layers, disabled states, selection layers, focus variants.
- `Ribbon/RibbonThemeMapper.cs`
  - Expand mapping from `IBeepTheme` to all phase-C ribbon tokens.
- `Ribbon/Tokens/RibbonTokenImporter.cs` (new)
  - Import token JSON (Figma/DTCG-like) with mode support (light/dark/high-contrast).
- `Ribbon/Tokens/RibbonVariantMatrix.cs` (new)
  - Variant definitions for `layout x density x state x emphasis`.

Acceptance criteria:
- Ribbon visuals can be fully driven by token files.
- Theme switching updates all ribbon surfaces and states consistently.
- No direct hardcoded fonts/colors outside token-mapped fallback paths.

### C8. Visual Polish for Commercial Ribbon Feel
Status (current):
- Foundation added in code:
  - `Ribbon/Rendering/BeepRibbonRenderer.cs`
- `Ribbon/BeepRibbonControl.cs` updates:
  - Centralized item and surface drawing in toolstrip renderer through shared rendering helper.
  - Existing minimized/context and hover/pressed state visuals continue using tokenized colors.
  - Added optional contextual tab transition timing controls (`EnableContextTransitions`, `ContextTransitionDurationMs`).
  - Added optional backstage open transition timing controls (`EnableBackstageTransitions`, `BackstageTransitionDurationMs`).
- `Ribbon/RibbonThemeMapper.cs` updates:
  - Added style-preset overlay pass (`Office`/`Fluent`/`HighContrast`) after theme + form-style mapping.
  - Preset overlay now tunes spacing, corner radius, focus thickness, and interaction surfaces while staying token-derived.
- `Ribbon/Rendering/BeepRibbonRenderer.cs` updates:
  - Added token-driven interactive elevation layering (`ElevationLevel`/`ElevationStrongLevel`) for hover/pressed/selected states.
- `Ribbon/BeepRibbonGroup.cs` updates:
  - Added token-aware spacing/separator rhythm pass using `ItemSpacing` + `GroupSpacing` and density-sensitive margins.
- `Ribbon/BeepRibbonControl.cs` updates:
  - Added adaptive transition calibration (`AdaptiveTransitionTiming`) with style/density-aware effective timing for contextual and backstage transitions.
  - Added reduced-motion policy controls (`RespectSystemReducedMotion`, `ReducedMotion`) and animation short-circuiting.
  - Finalized tab-header context menu parity actions (layout, density, dark mode, search scope, reduced motion, customization).
- Remaining:
  - None for planned C8 scope.

File-by-file tasks:
- `Ribbon/BeepRibbonControl.cs`
  - Add subtle transitions for contextual tab appearance/disappearance and backstage open.
  - Add tab header context menu parity actions (already started) and finalize UX.
- `Ribbon/BeepRibbonGroup.cs`
  - Improve spacing and separator rhythm to match commercial visual density.
- `Ribbon/Rendering/BeepRibbonRenderer.cs` (new)
  - Unified renderer abstraction for tab/group/item/background layers.

Acceptance criteria:
- Contextual tabs and state transitions feel intentional and non-jarring.
- Visual rhythm (padding/separators/hover/pressed) is consistent across groups.
- Rendering stays theme-token driven.

## Cross-Cutting Integration Files
- `Styling/ImagePainters/StyledImagePainter.cs`
  - Validate and extend icon paint hooks needed by galleries/super tooltips/backstage.
- `ThemeManagement/BeepThemesManager.cs`
  - Ensure typography mapping supports all new ribbon text roles.
- `IconsManagement/*`
  - Confirm canonical icon paths/constants are used by new ribbon surfaces.
- `.cursor/skills/beep-winform/reference.md`
- `.cursor/skills/beep-controls-usage/reference.md`

## Suggested Delivery Order
1. C1 Runtime Customization Surface
2. C4 Backstage 3.0
3. C5 Advanced Search
4. C2 Galleries
5. C3 Super Tooltips
6. C6 Accessibility + RTL
7. C7 Token Pipeline
8. C8 Visual Polish

## Global Exit Criteria (Phase C Complete)
- Ribbon has end-user customization UI with reliable persistence/reset.
- Backstage supports enterprise workflows (section templates, pinned/recent, action footer).
- Search is ranked, keyboard-friendly, and robust with provider fallback.
- Gallery + super-tooltip patterns are implemented and data-driven from `SimpleItem`.
- Accessibility/keyboard/RTL parity is achieved for core ribbon workflows.
- Theme/token pipeline supports light/dark/high-contrast and variant-based styling.

## Test and Validation Checklist
- Functional:
  - Layout switching (`Classic`/`Simplified`) with no command loss.
  - Customization save/load roundtrip and reset default behavior.
  - Backstage pin/unpin and recent list constraints.
  - Search local/provider fallback and top-hit execution.
- UX:
  - Key tips + keyboard paths for all core command routes.
  - Consistent hover/pressed/focus states across tabs/groups/QAT/backstage.
  - Tooltip readability and command metadata clarity.
- Accessibility:
  - Screen-reader friendly names/descriptions on major controls.
  - Focus visibility in all interaction states.
  - RTL rendering sanity checks.
