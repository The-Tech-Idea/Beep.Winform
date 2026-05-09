# Phase 4: Accessibility, Design-Time, and Quality

## Objective

Finish the commercialization effort with a professional accessibility story, a designer workflow built on the new page model, strong samples and docs, and explicit quality gates that make the control safe to ship and maintain.

## In Scope

- accessibility and narration
- keyboard completion and focus rules
- RTL, high-contrast, and touch ergonomics
- design-time rebuilding around the Beep-owned page model
- samples, README, and regression checklists
- performance and stability validation

## Out of Scope

- new feature families outside the roadmap
- full docking-manager implementation

## Primary File Targets

- `Tabs/Hosts/BeepTabHeaderHost.Accessibility.cs`
- `Tabs/Hosts/BeepTabHeaderHost.Keyboard.cs`
- `Tabs/Hosts/BeepTabHeaderHost.HighContrast.cs`
- `Tabs/Hosts/BeepTabHeaderHost.Touch.cs`
- `Tabs/Helpers/BeepTabAccessibleObjectFactory.cs`
- `Tabs/Helpers/BeepTabFocusVisualHelper.cs`
- `Tabs/Helpers/BeepTabRtlLayoutHelper.cs`
- `Tabs/Helpers/BeepTabInputPolicy.cs`
- `TheTechIdea.Beep.Winform.Controls.Design.Server/Designers/BeepTabsDesigner.cs`
- `TheTechIdea.Beep.Winform.Controls.Design.Server/Designers/DesignRegistration.cs`
- `Tabs/Readme.md`
- `Beep.Sample.Winform.Features/Forms/BeepTabsDemoForm.cs`
- sample/regression artifacts created for tabs

## Dependencies

Phase 4 assumes the page-model cutover is complete. Design-time and accessibility should target the final ownership model, not the temporary `TabPage`-shaped bridge.

## Current Status

- The designer surface now uses page-centric verbs and smart-tag labels instead of `TabPage`-style wording.
- Selected-page smart-tag editing now reaches page-owned `BeepTabPage` metadata/workspace properties, covering title, icon path, subtext, badge settings, selection and close behavior, close-button visibility override, busy state, grouping, preview-slot reuse, and visible workspace state such as pinned, preview, and dirty markers.
- The smart-tag surface also includes a read-only preview section for current header actions and overflow state, using the live tabs runtime state rather than a separate design-time projection.
- Design-time header hit testing now converts designer screen coordinates to `BeepTabs` client coordinates before checking the header strip, so page switching through header clicks targets the rendered tab geometry reliably.
- The designer now refreshes selection and smart-tag state when a `BeepTabPage` is removed from the owner control tree, so delete/remove flows outside the custom verbs still stay aligned with the active Beep-owned page model.
- When content is dropped onto `BeepTabs` with no active page, the designer now creates/selects a `BeepTabPage` first so authored children still land inside the Beep-owned page model instead of on the shell control.
- Default-page creation now uses `InitializeNewComponent` instead of normal designer reload initialization, so a newly dropped `BeepTabs` still starts with a page while an intentionally empty saved control can remain empty after reopening the designer.
- Internal tab header/content host controls are hidden from the toolbox so page authoring stays on the `BeepTabs`/`BeepTabPage` design surface.
- Runtime state and command-like BeepTabs surfaces are explicitly hidden from designer serialization so the form designer persists only authored configuration, pages, and page children.
- Focused persistence smoke tests now pass for generated-`InitializeComponent`-style page/control rehydration, active-page child removal, page removal, metadata retention, and empty runtime tab sets (`BeepTabsPersistenceTests`: 3 passed, 0 failed).
- Accessible tab selection and close-button default actions now route through the same Beep-owned select/close command pipeline used by keyboard and pointer interactions instead of only moving focus.
- Selected-page metadata reset now clears page-owned tab metadata back to defaults through serializer-visible `BeepTabPage` properties and component-change notifications without discarding the page title, keeping smart-tag authoring aligned with designer save/reload behavior.
- Remaining design-time work should build from this page-owned metadata surface rather than reintroducing parallel tab-state editors.

## Workstreams

### 1. Complete Accessibility

The final control should expose accessible objects for:

- tab body
- close button
- overflow button
- add-page button
- action slots and important state markers where applicable

Required state narration:

- selected
- focused
- disabled
- dirty/modified
- pinned
- preview

### 2. Finalize Keyboard And Focus Rules

Document and validate the complete keyboard matrix, including:

- tab traversal
- direct `Ctrl+1` through `Ctrl+9` page shortcuts matching the advertised accessible shortcut text
- action-slot traversal
- document/workspace shortcuts
- popup/overflow dismissal
- focus-ring visibility and contrast

### 3. Harden RTL, High Contrast, And Touch

Validate and refine:

- mirrored layout behavior
- intentional action ordering in RTL
- high-contrast-safe color and glyph choices
- minimum touch targets for tabs and action slots

### 4. Rebuild Design-Time Around The New Page Model

The designer must stop thinking in `TabPage` terms and start thinking in Beep-owned pages.

Design-time page ownership is intentionally different from runtime hosting: authored pages remain under `BeepTabs.Controls` for serializer and drop-target behavior, while runtime projects those same `BeepTabPage` instances into the stable `BeepTabContentHost` page panel outside design mode. The designer must preserve that boundary instead of trying to mimic runtime reparenting.

Required designer capabilities:

- add/remove/reorder/select pages
- parent dropped controls into the selected page
- edit page metadata
- apply mode/style presets
- preview overflow-related settings
- manually verify generated `InitializeComponent` output after save/reopen/run for page add, page remove, child-control add/remove, and intentionally empty tab sets

### 5. Publish Samples And Docs

The tabs documentation and sample surface should present at least:

- navigation-tab scenario
- document-tab scenario
- workspace-tab scenario
- overflow/action-slot scenario

README, roadmap, tracker, and samples should all tell the same story.

### 6. Establish Quality Gates

Define and execute checks for:

- resize and DPI stability
- overflow determinism
- repeated add/remove/reorder stability
- keyboard and accessibility walkthroughs
- performance-sensitive layout/measurement paths
- designer reload and serialization behavior

## Deliverables

1. Accessible, keyboard-complete, high-contrast-safe tab behavior.
2. A Beep-owned designer workflow for tab pages and metadata.
3. Updated docs and sample coverage that match the final architecture.
4. A regression/quality matrix that can be rerun as the control evolves.

## Acceptance Criteria

- Accessibility, keyboard, RTL, high-contrast, and touch behavior are explicit and validated.
- Design-time editing works against the Beep-owned page model.
- README, `.plans/`, tracker, and demo form all reflect the same architecture.
- Performance-sensitive paths rely on cached/layout-driven behavior rather than ad hoc measurement churn.
- The control feels credible beside commercial tab products in both runtime and designer workflows.

## Final Delivery Standard

The finished control should behave like a professional Beep-native tab product: owned header and content model, rich metadata, strong runtime interaction, strong accessibility, and a designer surface that feels intentional rather than retrofitted.