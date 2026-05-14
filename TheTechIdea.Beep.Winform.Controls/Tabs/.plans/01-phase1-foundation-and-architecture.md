# Phase 1: Foundation and Architecture

## Objective

Establish the commercial core of `BeepTabs` by replacing the remaining `TabPage`-shaped hosting model with a Beep-owned page/container model and by making the header/content hosts authoritative. This phase is the architectural cutover that the rest of the roadmap depends on.

## Current Status

Phase 1 is complete.

Completed in the current cutover:

- `BeepTabPage` exists as the canonical Beep-owned page/container model
- primary hosted-page API seams now use `BeepTabPage`
- runtime pages now attach under one stable `BeepTabContentHost` outside design mode
- `BeepTabContentHost` now owns runtime `BeepTabPage` instances and presents the selected page by bounds, visibility, and z-order instead of acting only as a selected-page swapper
- metadata ownership moved off the old page-keyed dictionary and onto the page model
- remaining in-repo callers were migrated off native `TabPage`
- the pass-through `BeepTabContentProjection` seam was removed during the cutover
- owner, helper, and painter access to basic selected-item/content/item-state flows were moved back onto `BeepTabs`
- `BeepTabHeaderHost` now snapshots directly from `BeepTabs`, and the old `BeepTabsRuntimeBridge` plus its unused header-metrics cache were removed
- design-time page initialization now uses the canonical `Pages` surface, page order self-heals between the Beep-owned page list and owner control tree for serializer reload stability, and reordering raises `SelectedIndexChanged` when the selected page's index changes
- selected-page tab metadata now persists through page-owned `BeepTabPage` properties and resets through the page model instead of living only in runtime-only smart-tag state
- page-centric members such as `AddPage(...)`, `InsertPageAt(...)`, and `ClearPages()` now carry the canonical hosted-page behavior, while older `...Tab...` wrappers are compatibility aliases only
- runtime/design-time hosted-content workflow unified: `BeepTabs_HandleCreated` calls `ProjectDesignerPagesToContentHost()` at runtime to sweep any designer-authored pages that are still parented to `BeepTabs.Controls` into `BeepTabContentHost`, finalising the content-host presentation state before the first layout/paint cycle

## In Scope

- define the Beep-owned page/container model
- remove `TabPage` from the premium-facing API
- move hosted-content ownership to `BeepTabContentHost`
- separate runtime page hosting from design-time serializer ownership
- move metadata ownership onto the page model instead of parallel dictionaries
- simplify runtime bridge/projection responsibilities
- keep `BeepTabHeaderHost` authoritative for header layout and interaction

## Out of Scope

- new feature families beyond what is needed to complete the cutover
- polishing overflow/action-slot visuals beyond what is needed for stable architecture
- full designer rebuild (that happens in Phase 4)
- docking manager implementation

## Primary File Targets

- `Tabs/BeepTabs.cs`
- `Tabs/BeepTabs.HostedContent.cs`
- `Tabs/BeepTabs.Layout.cs`
- `Tabs/BeepTabs.Metadata.cs`
- `Tabs/Hosts/BeepTabHeaderHost.cs`
- `Tabs/Hosts/BeepTabHeaderHost.Layout.cs`
- `Tabs/Hosts/BeepTabHeaderHost.Mouse.cs`
- `Tabs/Hosts/BeepTabHeaderHost.Painting.cs`
- `Tabs/Hosts/BeepTabContentHost.cs`
- `Tabs/Models/BeepTabItem.cs`
- `Tabs/Models/BeepTabHeaderLayoutSnapshot.cs`
- `Tabs/Models/BeepTabHeaderItemLayout.cs`
- `Tabs/Models/BeepTabRenderContext.cs`
- `Tabs/Helpers/BeepTabLayoutHelper.cs`
- `Tabs/Helpers/BeepTabHitTestHelper.cs`
- `Tabs/Adapters/BeepDocumentHostWorkspaceAdapter.cs`
- `Tabs/BeepTabPage.cs`

## Target Outcome

At the end of Phase 1, the commercial tab surface should think in terms of Beep-owned pages, not native tab pages.

The target ownership model is:

- `BeepTabs`
  Public shell/facade and orchestration surface.
- `BeepTabPage`
  Child container for arbitrary hosted controls plus serializable tab metadata.
- `BeepTabContentHost`
  Stable runtime page host/panel. It owns runtime page controls and presents the selected page by layout, visibility, and z-order.
- `BeepTabItem`
  Render snapshot built from page state.
- internal adapters
  Thin migration seams only, not part of the premium contract.

## Workstreams

### 1. Introduce `BeepTabPage`

Status: implemented.

Create a Beep-owned page/container type with the minimum commercial contract:

- child-control hosting
- title/text
- icon path
- badge/subtext
- dirty/busy state
- close/select/reorder flags
- workspace/document metadata

This type is now the content primitive for the control. Follow-up work should build on it rather than reopening native `TabPage` compatibility.

### 2. Replace The Premium API Surface

Replace or retire the current public seams that still imply stock tab semantics. The premium surface should move toward:

- `Pages`
- `SelectedPage`
- `AddPage`
- `RemovePage`
- `MovePage`
- `CreatePage`

Avoid adding any new members that accept or return `TabPage`.

### 3. Move Content Ownership Into `BeepTabContentHost`

Status: complete.

`BeepTabs.HostedContent.cs` stops mixing collection ownership, selection, and direct control-parenting assumptions in one place. `BeepTabContentHost` is the only runtime page host and selected-content presenter.

Key rules:

- `BeepTabs` selects pages
- `BeepTabContentHost` owns runtime `BeepTabPage` controls after they are registered
- `BeepTabContentHost` shows the selected page by setting bounds, visibility, and z-order
- page containers own their child controls
- layout, paint, hover, and header synchronization must not reparent pages
- add/remove/reorder are the only normal operations that should move pages between parents
- wrapper code should not rely on `Controls.Add(TabPage)`-style semantics

Runtime/design-time split:

- Runtime: pages are attached once to `BeepTabContentHost`; selected page is visible and topmost; unselected pages are hidden or behind but remain hosted.
- Design time: pages stay under `BeepTabs.Controls` so WinForms designer serialization, page selection, and child-control dropping remain predictable.
- Transition: `BeepTabs_HandleCreated` calls `ProjectDesignerPagesToContentHost()` when not in design mode. This sweep calls `EnsureHostedPagesAttachedToContentHost` (moves any pages still parented to `BeepTabs.Controls` into `BeepTabContentHost`) and `SyncContentHostPageOrder` (aligns z-order with `_hostedPages`), then calls `ApplyHostedSourceContentBounds` to size and show the selected page. `OnControlAdded` handles per-page projection as `InitializeComponent` runs; the `HandleCreated` sweep seals the state and covers unusual initialisation orders.

Implemented:

- replace selected-page `SetContent` swapping with explicit `AddPage`, `RemovePage`, `ClearPages`, `SetSelectedPage`, and `UpdatePageBounds` host operations
- guard content-host layout with an internal applying-layout flag to prevent recursive reentry
- avoid setting `Visible = false` on the selected page during resize/layout paths
- keep header visibility based on page metadata/header snapshots, never on page control visibility
- `ProjectDesignerPagesToContentHost()` runtime sweep added to `BeepTabs.HostedContent.cs` and called from `BeepTabs_HandleCreated`

### 4. Move Metadata Ownership To The Page Model

Status: implemented.

Retire the page-keyed metadata dictionary in `BeepTabs.Metadata.cs` as the primary source of truth. Page metadata should live with the page/container and be projected once into `BeepTabItem` for header rendering.

### 5. Remove Temporary Runtime Bridge Seams

Status: implemented for the core tabs path.

The old `BeepTabsRuntimeBridge` bootstrap layer is no longer part of the tabs core path. Snapshot creation now flows directly through `BeepTabs`, and the dead header-metrics cache was removed with it.

Remaining adapter work should focus on genuine cross-surface integration seams such as document/workspace alignment, not on reintroducing a tabs-core proxy layer.

### 6. Preserve The Authoritative Header Host

The header host remains the owner of:

- header layout snapshots
- hit testing
- pointer state
- drag classification and feedback
- action-slot hit testing
- keyboard and accessibility entry points

Phase 1 is not about moving that behavior back into `BeepTabs`.

## Detailed Deliverables

1. A documented and implemented Beep-owned page/container model.
2. A page-centric public API plan, with current shell methods either replaced or clearly marked as migration seams.
3. Runtime page hosting and selected-content presentation routed through `BeepTabContentHost`.
4. Metadata sourced from the page model rather than a parallel dictionary.
5. Temporary tabs-core bridge/proxy seams removed from the owned runtime path.
6. Updated docs and tracker state reflecting the new ownership model.

## Acceptance Criteria

- No new premium-facing API exposes `TabPage`.
- The commercial architecture has a named page/container type.
- Runtime `BeepTabPage` controls are hosted under one stable `BeepTabContentHost` outside design mode.
- Ordinary layout, paint, hover, and header synchronization do not remove, add, or reparent hosted pages.
- Selecting a page updates selected-page bounds, visibility, z-order, events, and header state deterministically.
- Design-time pages remain serializer-visible under `BeepTabs.Controls`.
- Metadata has one canonical owner.
- Header layout and hit testing still flow through `BeepTabHeaderHost`.
- Existing styles can still render against the new page/item split.

## Risks And Mitigations

- Risk: changing content ownership can destabilize selection and layout.
  Mitigation: keep header-state and layout authority in the host while swapping only the page/content source model.

- Risk: runtime and design-time can diverge during the cutover.
  Mitigation: keep the Phase 1 API and ownership decisions explicit so Phase 4 designer work targets the final model rather than the temporary one.