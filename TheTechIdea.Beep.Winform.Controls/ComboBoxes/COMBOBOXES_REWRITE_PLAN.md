# ComboBoxes Rewrite Plan

## Goal

Rewrite the ComboBoxes UX, painter layer, and popup behavior in a way that feels current and intentional, while preserving the existing Beep control architecture and the `SimpleItem` data model.

This plan covers:

- `BeepComboBox`
- `BeepDropDownCheckBoxSelect`
- shared painter infrastructure in `Painters/`
- shared layout and behavior helpers in `Helpers/`

This is a rewrite plan only. It does not implement the redesign yet.

## Non-Negotiable Constraints

### Data Contract

The rewrite must keep `SimpleItem` as the primary item model and preserve its existing usage patterns:

- selection is based on `SimpleItem`
- identity continues to use the existing `GetSimpleItemIdentity` / `IsSameSimpleItem` approach
- images still come from `SimpleItem.ImagePath`
- group headers, separators, subtext, checkable state, enabled state, and visibility remain supported
- multi-select remains list-of-`SimpleItem`, not a new token model

### Base Control Rules

The rewrite must continue to:

- inherit from `BaseControl`
- use `DrawingRect` for layout and painting
- treat `ApplyTheme()` as the single source of truth for theme propagation and visual refresh
- remain DPI-aware via `ScaleLogicalX`, `ScaleLogicalY`, and existing font helpers
- preserve accessibility hooks, keyboard navigation, and designer support

#### Drawing Method Convention

- In the main control itself, always use `protected override void DrawContent(Graphics g)` for painting inside `DrawingRect`. This is the standard owner-draw entry point for standalone Beep controls.
- The separate `Draw(Graphics graphics, Rectangle rectangle)` method is the entry point used when the control is hosted inside `BeepGridPro` or other container controls that supply their own target rectangle.
- Both paths must produce the same visual result; `DrawContent` delegates to the painter using `DrawingRect`, while `Draw` delegates using the caller-supplied `rectangle`.

#### Inline Editor Policy

The closed field of a combo box is **not always text**. Many variants display visual content: an image with label, a color swatch, an icon, chips, or a styled preview of the selected item. A `BeepTextBox` overlay is wrong for those cases.

**Rule: the inline editor is conditional, not default.**

When to show NO inline editor (field is fully owner-drawn):

- `select-only` variants — user clicks or keyboard-navigates to pick from a list; the closed field shows the selected item rendered by the painter (text, image, icon, or any visual)
- `visual-display` variants — color pickers, icon pickers, image pickers, status selectors where the field value is a rendered visual, not editable text
- `multi-select chip` variants in read-only mode — chips are painted; user clicks chip close buttons to remove
- `Draw(graphics, rectangle)` path for containers — always fully painted, never hosts a child control

When to show an inline text editor:

- `editable` variants — user can type freeform text that may or may not match a list item
- `searchable` variants — user can type to filter the popup list
- `freeform token` variants — user types and presses a delimiter to create tags

Implementation:

- the `BeepTextBox` inline editor is lazily created only when the variant requires it
- the editor is shown only during the active editing gesture and hidden afterward
- for select-only variants, keyboard typeahead is handled at the control level via `OnKeyDown` / `OnKeyPress` directly — no editor child is spawned
- painters always render the closed field as if no editor exists; the editor simply overlays the text area when active
- `DrawContent(g)` paints the full field every frame; the editor, if visible, sits on top and hides the painted text underneath
- `Draw(graphics, rectangle)` never involves an editor — it is always a static painted representation

Why this matters:

- avoids a child control overlay fighting with painted visuals for non-text content
- avoids unnecessary control creation and focus management for simple select-only dropdowns
- keeps the painter as the single source of truth for closed-field appearance
- makes `Draw()` for grid/container embedding naturally correct without special casing
- allows image combos, color combos, and icon combos without an awkward text overlay

### Rendering Infrastructure Rules

The rewrite must use the existing shared rendering infrastructure already present in the repo.

#### Image Painting

Use `StyledImagePainter` for combo field icons, row icons, chip close icons, status icons, and any variant-specific decorative icons.

Rules:

- prefer `StyledImagePainter.PaintWithTint(...)` for theme-aware icon tinting
- use `SimpleItem.ImagePath` as the primary row and chip image source
- do not introduce a parallel ad hoc icon renderer for combo painters
- when a painter needs a shaped image treatment, still route it through `StyledImagePainter`

#### Font Resolution

Font selection must stay within the current repo infrastructure.

Use:

- `BeepThemesManager.ToFont(...)` as the first theme-driven font resolver
- `FontListHelper` where cached or named font lookup is needed for performance or explicit style selection

Rules:

- do not hardcode arbitrary font families into combo painters
- do not allocate transient fonts per paint when a cached or theme font is available
- keep field, popup, chip, helper, and header typography tied to theme-driven font resolution

#### Icon Management

When the combo variants need icons beyond the current chevron and clear affordances, use the existing icon management layer under `IconsManagement/`.

Use:

- `Svgs`
- `SvgsUI`
- related icon catalog/path helpers already in the repo

Rules:

- do not introduce a second icon catalog local to ComboBoxes
- keep icon keys/path resolution centralized through the repo icon infrastructure
- variant-specific icons should map to existing icon resources whenever possible

#### Theme Application

Every visual layer in the rewrite must derive its colors, text emphasis, fills, borders, and icon tint from `ApplyTheme()` state.

Rules:

- painters must consume theme-derived tokens instead of inventing local color systems
- popup content must receive the same resolved theme state as the closed control
- chips, search box, rows, separators, validation accents, and footer actions must all stay theme-driven
- any variant-specific design treatment must still degrade cleanly under the active Beep theme

### Compatibility Rules

The rewrite should preserve public behavior where practical:

- `ListItems`, `SelectedItem`, `SelectedItems`, `SelectedIndex`, `SelectedValue`
- `ComboBoxType`
- search, editable mode, multi-select, clear button, validation, loading, skeleton, select-all, grouping
- popup open/close events and selected-item change events

If an internal design changes, the public surface should stay stable unless there is a strong technical reason to deprecate something.

## Current State Summary

The current implementation already has solid foundations:

- partial class split for `BeepComboBox`
- painter interface and painter factory
- token catalog for variants
- inline editor support
- dropdown integration through `BeepContextMenu`
- secondary multi-select control using `BeepPopupForm`

The current issues are architectural rather than missing features:

- the field shell, value presentation, and popup list are not designed as one consistent system
- `BeepComboBox` and `BeepDropDownCheckBoxSelect` solve similar problems with different popup technologies
- painter classes still do too much view logic instead of consuming a normalized layout/state model
- popup UX is functional but not yet aligned with current design patterns from modern product UI
- multi-select chip presentation is useful but visually inconsistent across variants
- some variants are token-level variations instead of truly distinct interaction patterns

## Visual Direction

The design language should take inspiration from the references you provided, not by copying them literally, but by extracting the current UI patterns they share.

## Research Basis

The plan is now grounded in two sources:

- the image samples you provided
- current official guidance and commercial product patterns from Material 3, Fluent 2, Radix UI, MUI, Ant Design, Atlassian, Shopify Polaris, Apple HIG, and WAI-ARIA APG

### What Was Learned From The Image Samples

Across the supplied samples, the recurring high-quality patterns are:

- field shells are visually calm and usually rely on one strong signal at a time, such as border, radius, tint, or segmented trigger
- searchable dropdowns place search at the top and keep the list visually lightweight
- multi-select is usually shown as chips or tags inside the field, with a summary fallback like `+N`
- popups feel like lightweight cards or popovers, not menus from an older desktop stack
- list rows often support two levels of information, such as title plus supporting text
- selected, hovered, and keyboard-focused rows are visually distinct
- iconography is restrained and functional, mostly chevrons, checks, search, clear, status, and optional avatars or thumbnails
- filter pickers and location pickers frequently use a taller, more deliberate popup surface than a basic dropdown

### What Was Learned From Current Design-System Guidance

#### Material 3

- outlined and filled field variants should be used consistently by region, not mixed randomly in the same form cluster
- fields should always keep labels clear and visible, with supporting text and error text treated as structured field anatomy
- trailing icons for clear, error, dropdown, and status are standard and should be part of the field model
- dense variants are useful, but should not become the default if hit targets fall below comfortable interaction size
- menus and exposed dropdowns should auto-reposition when they would otherwise be clipped

#### Fluent 2

- a combobox is appropriate when the option set is long or when filtering/freeform entry is valuable
- multi-select popups should remain open until explicitly dismissed
- checkbox rows are the expected pattern for multi-select lists
- selected values should be shown outside the popup, commonly as tags in the field
- placeholder text is a hint only and must not replace the actual label

#### Radix UI

- trigger, value, icon, content, viewport, group, label, separator, item, and item indicator should be treated as separate structural parts
- popup positioning should support collision handling and width/height constraints relative to the trigger
- grouped items, separators, disabled items, custom rows, and trigger-width matching are baseline capabilities
- typeahead and full keyboard navigation should exist even for select-only experiences

#### MUI

- select and autocomplete are distinct interaction families; advanced filtering, async, creatable, and multi-select behavior belong in combobox/autocomplete flows, not in a basic select shell
- value state and input state should be treated as separate concepts
- grouped rows, disabled rows, async loading, virtualization, highlight rendering, and custom value rendering are common expectations
- multi-select chips and limit-tags behavior are standard commercial patterns

#### Ant Design

- large-data dropdowns, custom option rendering, grouped options, tags mode, token separators, loading, placement control, and clear affordances are all mainstream expectations now
- clear affordances, responsive tag collapsing, custom selected-label rendering, and status states are important for enterprise form controls
- popup structure is best treated semantically: root, content, placeholder, input, clear, suffix, popup root, list, and list item

#### Atlassian

- subtle and default appearances are both useful when applied intentionally
- clear affordances should be optimized for pointer interaction; keyboard users often rely on Delete and Backspace instead of tabbing to a clear button
- async loading, grouped options, and indicator customization are standard product needs

#### Shopify Polaris

- combobox should be treated as progressive enhancement for finding and choosing values faster, not as the only possible path to complete a task
- search filtering should be intentional, case-insensitive by default, and easy to override
- in multi-select flows, selected values can be moved to the top of the result list for better usability
- tag autocomplete and merchant-style filter picking reinforce the value of richer popup surfaces, not just bare list menus

#### Apple HIG

- use a picker for medium-to-long lists and keep it close to the field being edited
- compact trigger plus popover editing is a strong model when space is constrained
- ordering must be predictable so people can infer hidden options quickly

#### WAI-ARIA APG

- select-only and editable comboboxes are different patterns and should be modeled intentionally
- popup focus behavior, escape-to-cancel, and typeahead are not optional details; they define usability
- navigating options should not always commit value immediately
- `aria-expanded`, `aria-controls`, `aria-activedescendant`, and correct labeling semantics are core to robust accessibility

## Standards-Driven Product Direction

Based on the image samples plus current public standards, the rewrite should target a modern commercial combobox model rather than a dressed-up legacy WinForms combo.

### Product Rules

1. Treat `select-only`, `editable search`, and `multi-select` as distinct interaction patterns, even if they share a shell family.
2. Treat popup content as a composed surface with regions, not just a flat item list.
3. Keep field shell variants limited and deliberate: outlined, filled, pill, segmented, minimal, dense.
4. Keep labels, placeholder, helper text, validation, and status iconography structurally consistent.
5. Preserve keyboard exploration without prematurely committing selection.
6. Allow search-first flows, async/loading flows, tags/freeform flows, and grouped-option flows as first-class cases.
7. Keep popup positioning adaptive: below by default, above or constrained when space is limited.
8. Keep multi-select open until dismissal unless a specific immediate-close mode is requested.
9. Keep pointer and keyboard affordances distinct where that improves usability, especially for clear and chip remove actions.
10. Keep row height, padding, and chip density configurable, but avoid shipping the densest mode as the default.

### Design Cues To Adopt

- low-noise field shells with precise borders and soft elevation
- stronger hierarchy between field, popup, sections, and actions
- pill and rounded field treatments for modern variants
- search-first dropdowns for large datasets
- compact but readable multi-select chips
- list items with stronger hover, active, selected, and keyboard-focus states
- gentle segmentation where the trigger area is intentionally distinct
- clear spacing systems and density tiers
- support for placeholder, helper text, descriptions, and validation without clutter

### Design Cues To Avoid

- legacy Windows combo appearance
- overly decorative gradients
- cramped list rows
- ambiguous clickable regions
- painter-specific layout math duplicated in multiple classes

### Reference Patterns To Translate Into WinForms

From the supplied images, the most relevant patterns are:

- outlined fields with a clean, light popup panel and high legibility
- searchable lists instead of forcing scrolling for known input
- tokenized multi-select with inline add/remove actions
- segmented trigger fields where the arrow zone is clearly separated
- richer list rows that can support name plus secondary line
- sheet/popover-style location and filter pickers with strong rounded corners and calm shadows

From the external standards research, the most relevant product-level patterns are:

- select-only combos should still support typeahead and keyboard exploration
- editable combos should separate input text from committed selected value
- multi-select combos should support checkbox rows, chips, `+N` collapse, and optional footer actions
- grouped rows and separators should be first-class popup row types
- async, loading, empty, and no-results states should be designed deliberately, not treated as afterthoughts
- popup width should usually match or exceed trigger width, never feel narrower by accident
- trigger, value, placeholder, clear, suffix, popup, viewport, item, group label, separator, and selected indicator should each have explicit layout ownership

## Rewrite Objectives

### UX Objectives

1. Make the field itself feel modern before the popup opens.
2. Make every combo variant readable within one visual system.
3. Treat popup content as a first-class surface, not just a list container.
4. Improve discoverability for search, multi-select, and clear actions.
5. Make keyboard and mouse behavior consistent across all variants.

### Engineering Objectives

1. Separate shell painting, content painting, and popup rendering.
2. Normalize layout into reusable state/layout objects.
3. Remove duplicate chip and popup logic between controls.
4. Keep variants token-driven where possible and painter-driven only where interaction truly changes.
5. Make future variants cheap to add.

## Proposed Architecture

## 1. Split The Control Into Three Layers

### Layer A: Field Surface

This layer draws the closed control shell and inline content:

- field background
- border
- hover/focus/open/disabled/validation states
- text/value/placeholder
- leading icon or item image
- trailing chevron / search / clear / validation icon
- chips for multi-select

This layer should never decide popup item generation.

### Layer B: View Model Normalization

Introduce an internal normalized state object for painters and popup builders.

Suggested internal models:

- `ComboBoxRenderState`
- `ComboBoxLayoutSnapshot`
- `ComboBoxPopupModel`
- `ComboBoxPopupRowModel`

These models should be built from the current control state and `SimpleItem` collection. Painters consume these models instead of repeatedly reading raw owner fields.

### Layer C: Popup Surface

This layer owns:

- popup size and placement
- item virtualization strategy if needed later
- list row painting
- search box integration
- multi-select footer/actions
- keyboard focus row tracking
- empty state / loading state / no-results state

This layer should be shared between `BeepComboBox` and `BeepDropDownCheckBoxSelect`.

## 2. Popup Host Strategy (Per Painter)

### Problem

The folder currently has two popup paths:

- `BeepComboBox` uses `BeepContextMenu`
- `BeepDropDownCheckBoxSelect` uses `BeepPopupForm`

That creates different interaction models and doubles maintenance cost.

### Plan

Move both controls toward a per-painter popup host architecture.

Suggested internal interface:

`IComboBoxPopupHost`

Responsibilities:

- show popup relative to owner control
- close on outside click, escape, or selection rules
- expose popup state to owner
- host a search region, list region, optional footer region
- report selected row, hover row, and keyboard row

Required implementations (1:1 by `ComboBoxType`):

- `OutlineDefaultPopupHostForm`
- `OutlineSearchablePopupHostForm`
- `FilledSoftPopupHostForm`
- `RoundedPillPopupHostForm`
- `SegmentedTriggerPopupHostForm`
- `MultiChipCompactPopupHostForm`
- `MultiChipSearchPopupHostForm`
- `DenseListPopupHostForm`
- `MinimalBorderlessPopupHostForm`

Rule:

- Each field painter family owns one popup host form.
- Shared engines remain shared: popup model builder, search engine, theme tokens, icon/font pipelines, and `SimpleItem` translation.
- No host may duplicate business logic already covered by shared helper/model layers.

### Decision

- `BeepPopupForm` remains the base host technology.
- Popup host classes are split per painter family by design.
- `ComboBoxPopupHostForm` is considered a migration/legacy adapter, not the final architecture.

## 3. Replace Painter Responsibilities With Smaller Roles

### Current Problem

Painters currently mix:

- field rendering
- chip rendering
- interaction state usage
- layout assumptions
- icon decisions

### Plan

Refactor painter architecture into role-based painters/helpers.

Suggested pieces:

- `IComboBoxFieldPainter`
- `IComboBoxChipPainter`
- `IComboBoxPopupPainter`
- `ComboBoxPainterThemeTokens`
- `ComboBoxGeometryHelper`

Painter implementation rules:

- icon drawing flows through `StyledImagePainter` and icon resources from `IconsManagement`
- typography flows through `BeepThemesManager.ToFont(...)` and `FontListHelper` where caching is needed
- painter tokens are resolved from the active theme after `ApplyTheme()` has run

The variant painter should mostly choose tokens and composition, not own every drawing primitive.

### Intended Composition

- field painter draws shell and field chrome
- content renderer draws value, placeholder, chips, icons
- popup painter draws rows, headers, empty states, and selection affordances
- geometry helper computes consistent rectangles for all variants

## 4. Replace Variant Explosion With A Smaller Variant Matrix

The current enum is usable, but the actual UX can be organized into a cleaner matrix.

### Recommended Variant Families

#### A. Field Shell Families

- `Outline`
- `Filled`
- `Pill`
- `Segmented`
- `Minimal`

#### B. Behavior Families

- `StandardSelect`
- `SearchableSelect`
- `MultiSelect`
- `EditableSearch`
- `DenseData`

The public `ComboBoxType` enum can stay, but internally it should map to:

- shell family
- popup family
- density
- search mode
- chip mode

This reduces painter duplication.

### Canonical 1:1 Mapping Matrix

| ComboBoxType | Field Painter | Popup Host Form |
|---|---|---|
| `OutlineDefault` | `OutlineDefaultComboBoxPainter` | `OutlineDefaultPopupHostForm` |
| `OutlineSearchable` | `OutlineSearchableComboBoxPainter` | `OutlineSearchablePopupHostForm` |
| `FilledSoft` | `FilledSoftComboBoxPainter` | `FilledSoftPopupHostForm` |
| `RoundedPill` | `RoundedPillComboBoxPainter` | `RoundedPillPopupHostForm` |
| `SegmentedTrigger` | `SegmentedTriggerComboBoxPainter` | `SegmentedTriggerPopupHostForm` |
| `MultiChipCompact` | `MultiChipCompactComboBoxPainter` | `MultiChipCompactPopupHostForm` |
| `MultiChipSearch` | `MultiChipSearchComboBoxPainter` | `MultiChipSearchPopupHostForm` |
| `DenseList` | `DenseListComboBoxPainter` | `DenseListPopupHostForm` |
| `MinimalBorderless` | `MinimalBorderlessComboBoxPainter` | `MinimalBorderlessPopupHostForm` |

### Non-Negotiable Interaction Requirements

- full mouse interaction parity (hover, press, commit, outside-click close, chip remove)
- full keyboard parity (`Tab`, `Shift+Tab`, arrows, `Home`, `End`, `PageUp`, `PageDown`, `Enter`, `Esc`, `Backspace`, `Delete`)
- deterministic selection commit model (single, multi, apply/cancel)
- resilient focus management between trigger, search box, list rows, and footer actions
- consistent interactive states across all hosts (`normal`, `hover`, `pressed`, `focused`, `active`, `disabled`, `selected`, `error`, `loading`, `empty`)
- no dead UI zones (all visible affordances have working hit targets and keyboard equivalents)

## Detailed UX Plan

## A. Closed Field Experience

The closed field is **always owner-drawn** by the painter. The inline `BeepTextBox` editor is only layered on top when the variant explicitly needs user text input.

### Standard Select (No Inline Editor)

- the entire closed field is painted by `DrawContent` — no child control involved
- painter renders: shell, selected item text, optional leading image/icon, trailing chevron, clear button
- click anywhere on the field opens the popup
- keyboard typeahead is handled via `OnKeyDown` / `OnKeyPress` at the control level
- focused/open state should have a stronger border and subtle interior tint
- clear button appears only when meaningful and does not fight with the chevron
- chevron animation remains but should be smoother and more restrained

### Visual Select (No Inline Editor)

- for image combos, color combos, icon combos, or status selectors
- the closed field paints the selected item as a visual: image + label, color swatch + name, icon + text
- image rendering routes through `StyledImagePainter`
- no text input is possible — clicks open the popup, keyboard navigates the popup
- `Draw(graphics, rectangle)` for grid embedding paints the same visual representation

### Searchable Select (Inline Editor On Activation)

- closed state still looks like a field, not a search box pretending to be a combo
- clicking the text area or pressing a character key activates the inline `BeepTextBox` editor over the text area
- the editor is created lazily and shown only during the search gesture
- optional search icon can appear in the field for searchable variants
- when opened, search input should be immediately usable without layout shift
- when the editor is hidden, the painter takes over and draws the committed selected value

### Multi-Select (Chips Are Painted, Editor Is Conditional)

- selected items appear as compact chips painted by the chip painter — not child controls
- chip close affordance is a painted hit area, not a button control
- overflow degrades to `+N` summary chip or multi-line expansion based on property
- when `AllowFreeText` or search is enabled, an inline editor appears after the last chip for additional input
- when adding items is only via popup, no inline editor is shown — chips fill the field area
- input caret / text area must not collide with chips

### Dense Data Variant

- tighter height and lower padding
- smaller row height in popup
- still maintain accessible focus and hit targets

## B. Popup Experience

### Popup Layout Regions

Every popup should be able to render these regions in order:

1. optional search box
2. optional selected-summary strip
3. scrollable rows region
4. optional footer actions

Popup rendering rules:

- row icons and search/clear/check icons use `StyledImagePainter`
- popup typography uses theme fonts resolved through `BeepThemesManager.ToFont(...)` or `FontListHelper`
- popup chrome, separators, and states must be fed from the same theme token source as the field

### Popup Row Types

Support these row kinds from `SimpleItem` translation:

- normal selectable row
- selected row
- disabled row
- group header row
- separator row
- descriptive row with subtext
- checkbox multi-select row
- empty state row

### Popup Search UX

- search field stays pinned to top
- typed filtering should not rebuild the popup host from scratch every time
- keyboard focus stays predictable when the result set changes
- highlight the first best match, not just the first visible row
- select-only variants should still support typeahead against row text
- editable variants should keep input text state separate from committed selected value state
- freeform variants should define when typed input becomes a committed value or token

### Popup Positioning And Sizing UX

- default to opening below the field
- auto-flip above when space below is insufficient
- constrain popup height to available viewport space
- prefer popup width equal to trigger width for standard dropdowns
- allow wider popup layouts for search-first, descriptive, or multi-column row content when needed
- keep scrollbars persistent and visually intentional when content overflows

### Popup Commitment Rules

- single-select commits on click, Enter, or explicit accept action
- multi-select does not close on every selection by default
- Escape closes without forcing a new committed value
- clicking outside closes the popup and preserves the correct committed state
- keyboard exploration may move active focus without immediately replacing the current committed value

### Popup Footer For Multi-Select

Use optional footer actions instead of overloading row zero forever.

Suggested footer actions:

- `Select all`
- `Clear all`
- `Apply`
- `Cancel`

Keep existing close-on-click behavior configurable. Some multi-select flows should remain immediate, others should use apply/cancel.

## C. States To Design Explicitly

The rewrite should not rely on border color only.

Required field states:

- normal
- hover
- focused
- open
- disabled
- read-only
- loading
- skeleton
- error
- warning
- success

Required popup row states:

- normal
- hover
- keyboard-focused
- selected
- selected-hover
- disabled
- group-header

## Proposed File Plan

## Keep

- `BeepComboBox.cs`
- `BeepComboBox.Core.cs`
- `BeepComboBox.Properties.cs`
- `BeepComboBox.Events.cs`
- `BeepComboBox.Methods.cs`
- `BeepComboBox.Drawing.cs`
- `ComboBoxType.cs`

## Replace Or Heavily Rewrite

- `Helpers/BeepComboBoxHelper.cs`
- `Painters/BaseComboBoxPainter.cs`
- `Painters/DesignSystemComboBoxPainters.cs`
- `Painters/ComboBoxVisualTokens.cs`
- popup logic inside `BeepComboBox.Methods.cs`
- popup logic inside `BeepDropDownCheckBoxSelect.cs`

## Add

Suggested new files:

- `Helpers/ComboBoxLayoutEngine.cs`
- `Helpers/ComboBoxStateFactory.cs`
- `Helpers/ComboBoxPopupModelBuilder.cs`
- `Helpers/ComboBoxSearchEngine.cs`
- `Helpers/ComboBoxChipLayoutEngine.cs`
- `Painters/ComboBoxFieldPainterBase.cs`
- `Painters/ComboBoxFieldPainterCatalog.cs`
- `Painters/ComboBoxPopupPainter.cs`
- `Painters/ComboBoxChipPainter.cs`
- `Popup/ComboBoxPopupHostForm.cs`
- `Popup/ComboBoxPopupContent.cs`
- `Popup/ComboBoxPopupRow.cs`
- `Popup/ComboBoxPopupFooter.cs`
- `Popup/ComboBoxPopupPlacementHelper.cs`

Optional specialized popup hosts if needed:

- `Popup/ComboBoxDropdownForm.cs`
- `Popup/ComboBoxSearchPopupForm.cs`
- `Popup/ComboBoxMultiSelectPopupForm.cs`

Rule:

Specialized popup forms are allowed only when they share the same popup model builder, theme pipeline, icon handling, and selection/search engines.

## Control-Specific Plan

## 1. BeepComboBox

### What To Keep

- public API shape
- inline editor concept **only for editable/searchable modes** — not for select-only or visual variants
- search and fuzzy-match capabilities
- multi-select support
- accessibility object approach
- clear button, validation, loading, skeleton behaviors

### What To Change

- stop showing an inline `BeepTextBox` for select-only and visual-display variants; those modes are fully owner-drawn
- handle keyboard typeahead for select-only variants at the control level (`OnKeyDown` / `OnKeyPress`), not via an editor overlay
- make the inline editor creation lazy and conditional on the behavior family (searchable, editable, freeform token)
- stop using popup item assembly directly in `ShowDropdown()` as the long-term rendering model
- move popup row creation into a reusable popup model builder
- move chip layout out of the painter class
- make `DrawContent()` consume a computed snapshot instead of recalculating ad hoc pieces in multiple places
- make `Draw(graphics, rectangle)` always produce a fully painted static image with no dependency on child controls
- treat open-state visuals as part of one field system

## 2. BeepDropDownCheckBoxSelect

### Current Role

This control is effectively a specialized multi-select combo with chips and a popup checklist.

### Plan

Bring it into the same architecture as `BeepComboBox` instead of keeping a parallel implementation.

Two acceptable end states:

1. keep it as a thin specialized wrapper over shared combo infrastructure
2. mark it as a specialized multi-select facade around the same popup and chip engines

Avoid keeping a second custom popup implementation with duplicated chip and row logic.

## Migration Strategy

## Phase 1. Foundation Refactor

- create normalized state objects
- create layout engine
- create popup model builder
- create chip layout engine
- keep old visuals temporarily while refactoring internals

Deliverable:

The controls still render roughly the same, but their data and layout pipeline becomes clean enough for the visual rewrite.

## Phase 2. Popup Host Rewrite (Per Painter)

- introduce per-painter popup hosts based on `BeepPopupForm`
- move search box, rows, and footer into popup content control
- add placement logic, flip logic, width calculation, and keyboard routing
- bridge old methods so `ShowDropdown`, `CloseDropdown`, and related events still work
- enforce 1:1 mapping between `ComboBoxType` and popup host class

Deliverable:

Both controls use one shared model/engine architecture with nine dedicated popup host forms mapped 1:1 to `ComboBoxType`.

## Phase 3. Field Painter Rewrite

- rewrite shell painters for outline, filled, pill, segmented, minimal, and dense variants
- unify icon placement and clear-button geometry
- move chips to dedicated chip painter
- clean up hover/focus/open states
- route all icon rendering through `StyledImagePainter` plus the existing icon catalogs
- resolve all typography through `BeepThemesManager.ToFont(...)` and `FontListHelper` as appropriate

Deliverable:

Closed control UI matches the new design direction.

## Phase 4. Popup Painter Rewrite

- modernize row visuals
- add better search region and empty states
- add grouped rows, check states, secondary text, and better keyboard focus visualization
- align popup spacing and corners with field shell design
- align popup structure to explicit parts: popup root, search region, viewport, row, row indicator, group label, separator, footer

Deliverable:

Open popup experience matches the field quality.

## Phase 5. Special Behavior Pass

- multi-select summary strategies
- apply/cancel footer mode
- editable free-text tokens
- RTL verification
- screen reader and keyboard regression pass

Deliverable:

Advanced behaviors remain intact after the redesign.

## Implementation Order

Recommended execution order:

1. create state/layout/popup model helpers
2. introduce per-painter popup hosts without changing public API
3. migrate `BeepDropDownCheckBoxSelect` to shared popup/content engines
4. rewrite field painters
5. rewrite popup painters
6. run cleanup pass on old helper logic and dead code

## Production Readiness Gates

Every host/painter path must pass all gates before completion:

- **Interaction gate:** full mouse + keyboard parity (`Tab`, `Shift+Tab`, arrows, `Home`, `End`, `PageUp`, `PageDown`, `Enter`, `Esc`, `Backspace`, `Delete`), no dead hit zones.
- **Accessibility gate:** role/name/value, focus order, announcements, contrast, high-DPI readability.
- **Performance gate:** bounded paint allocations, large-list behavior, search latency budget, popup open latency budget.
- **Stability gate:** null/empty/async/rapid-open-close resilience.
- **Telemetry gate:** popup open/close, search filter/update timings, commit/cancel paths, and failure/empty/loading transitions are observable.
- **Regression gate:** matrix coverage by `ComboBoxType` and popup host form.
- **Visual gate:** spacing, radius, icon alignment, and state transition quality stay consistent with modern standards.

## Regression Matrix by ComboBoxType and Host

| ComboBoxType | Popup Host Form | Mouse | Keyboard | Search | Multi-select | Validation | Loading/Skeleton |
|---|---|---|---|---|---|---|---|
| `OutlineDefault` | `OutlineDefaultPopupHostForm` | Required | Required | Optional | N/A | Required | Required |
| `OutlineSearchable` | `OutlineSearchablePopupHostForm` | Required | Required | Required | N/A | Required | Required |
| `FilledSoft` | `FilledSoftPopupHostForm` | Required | Required | Optional | N/A | Required | Required |
| `RoundedPill` | `RoundedPillPopupHostForm` | Required | Required | Optional | N/A | Required | Required |
| `SegmentedTrigger` | `SegmentedTriggerPopupHostForm` | Required | Required | Optional | N/A | Required | Required |
| `MultiChipCompact` | `MultiChipCompactPopupHostForm` | Required | Required | Optional | Required | Required | Required |
| `MultiChipSearch` | `MultiChipSearchPopupHostForm` | Required | Required | Required | Required | Required | Required |
| `DenseList` | `DenseListPopupHostForm` | Required | Required | Optional | N/A | Required | Required |
| `MinimalBorderless` | `MinimalBorderlessPopupHostForm` | Required | Required | Optional | N/A | Required | Required |

## Acceptance Criteria

- mapping matrix is complete and non-ambiguous
- no stale references to deleted legacy painter files
- popup architecture wording is consistent with the per-host strategy
- every TODO item maps to a concrete file/class target
- every host checklist includes interaction, accessibility, performance, and regression completion criteria

## Testing Plan

## Functional Coverage

- single select
- multi select
- searchable single select
- searchable multi select
- editable mode
- free-text token mode
- grouped items
- separators
- disabled items
- descriptions/subtext
- clear button
- validation states
- loading and skeleton states
- empty list and no-search-results list

## Interaction Coverage

- mouse open/close
- outside click close
- keyboard open/close
- keyboard up/down/home/end/page navigation
- enter selection
- escape cancel
- tab focus behavior
- chip removal by mouse and keyboard

## Rendering Coverage

- high DPI
- narrow width controls
- large fonts
- RTL
- disabled theme
- dark and light themes if supported by current theme system

## Risks

1. The popup rewrite may expose assumptions in `BeepContextMenu` and existing selection event flows.
2. The current painters may contain edge-case fixes that must be revalidated after cleanup.
3. Multi-select chip layout can become expensive if not normalized and cached carefully.
4. Search and keyboard focus logic can regress if popup filtering rebuilds state too aggressively.
5. `BeepDropDownCheckBoxSelect` may carry behavior that users depend on even if it is architecturally redundant.

## Success Criteria

The rewrite is successful when:

- both combo controls feel like one coherent design system
- popup and closed-field visuals feel intentionally related
- `SimpleItem` compatibility is preserved
- the new popup supports search, grouping, multi-select, descriptions, and validation cleanly
- field painters are smaller, cleaner, and easier to extend
- the secondary combo control no longer duplicates core combo infrastructure
- image, icon, and typography rendering all stay on the existing shared Beep infrastructure
- theme changes continue to flow through `ApplyTheme()` without local visual drift
- if distinct popup forms are introduced, they still behave like one coherent combo system instead of separate one-off controls
- the resulting controls feel aligned with current commercial combobox standards rather than legacy desktop dropdowns

## First Implementation Slice

The safest first coding slice after plan approval is:

1. add normalized state/layout helpers
2. add shared popup model builder
3. introduce popup host abstraction behind current `ShowDropdown()`
4. leave the current painter visuals in place for the first pass

That sequence reduces regression risk before the visual rewrite begins.