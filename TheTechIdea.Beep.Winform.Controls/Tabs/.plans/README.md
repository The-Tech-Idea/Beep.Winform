# BeepTabs Enhancement Plan

## Goal

Turn `BeepTabs` into a fully owned commercial tab system with strong parity against:

- DevExpress document tabs and `XtraTabControl`
- Telerik and Syncfusion desktop tab controls
- Visual Studio and VS Code document/workbench tabs
- Material UI and Ant Design application tabs

The target is not visual imitation alone. It is product-grade behavior, architecture, and design-time workflow.

## Plan Status

This roadmap is the active commercialization plan as of May 2026. It replaces older status language that implied the work was already complete. The codebase contains meaningful scaffolding, but the premium cutover is still in progress.

## Design Principles

- Prefer a clean break over preserving stock `TabPage` semantics in the premium surface.
- Keep `BeepTabs` as a facade/orchestrator, not as the owner of every low-level behavior.
- Keep `BeepTabHeaderHost` authoritative for header layout, hit testing, overflow, actions, keyboard, accessibility, and pointer state.
- Keep `BeepTabContentHost` authoritative for runtime page hosting and selected-content presentation.
- At runtime, keep `BeepTabPage` instances attached to one stable content host/panel; selection changes visibility and z-order only, while ordinary layout, paint, and hover must not remove/add/reparent pages.
- At design time, keep authored `BeepTabPage` instances on the `BeepTabs.Controls` tree for serializer and designer drop behavior, then project that same page model into the runtime host.
- Use `BeepTabItem` as the immutable render/header snapshot.
- Move content ownership to a Beep-owned page/container model instead of `TabPage`.
- Treat adapters as temporary internal seams, not as long-term premium API.

## Plan Index

- `00-overview-gap-matrix.md`
  Maps the commercial capability set, current blockers, and phase ownership.
- `01-phase1-foundation-and-architecture.md`
  Defines the page-model cutover, runtime simplification, page-owned metadata persistence, and content-host authority.
- `02-phase2-overflow-header-actions-and-rich-tabs.md`
  Delivers overflow policies, header action slots, rich header metadata, and painter unification.
- `03-phase3-document-workspace-and-advanced-interactions.md`
  Adds document/workspace policies, commands, MRU, quick switch, and `DocumentHost` alignment.
- `04-phase4-accessibility-design-time-and-quality.md`
  Finishes accessibility, RTL/high-contrast/touch, design-time, samples, and quality gates.

## Canonical Ownership Model

### Premium Runtime Surfaces

- `Tabs/BeepTabs.cs`
  Public shell/facade and orchestration surface.
- `Tabs/BeepTabPage.cs`
  Beep-owned child container for hosted content and serializable tab metadata. This is now the canonical premium page model, including persisted selected-page metadata edited through the designer; remaining Phase 1 work is follow-through in the host and designer seams.
- `Tabs/Hosts/BeepTabHeaderHost.cs`
  Root premium header host and behavior owner.
- `Tabs/Hosts/BeepTabContentHost.cs`
  Runtime child-panel/content host that owns Beep-owned pages after creation and presents the selected page through stable layout, visibility, and z-order.

### Core Models

- `Tabs/Models/BeepTabItem.cs`
  Canonical render snapshot.
- `Tabs/Models/BeepTabHeaderLayoutSnapshot.cs`
  Shared header layout snapshot.
- `Tabs/Models/BeepTabHeaderItemLayout.cs`
  Per-item layout slices.
- `Tabs/Models/BeepTabRenderContext.cs`
  Painter-facing render payload.
- `Tabs/Models/BeepTabWorkspaceState.cs`
  Workspace/document semantics.
- `Tabs/Models/BeepTabAdornmentState.cs`
  Badge, subtext, busy, dirty, and status layout inputs.
- `Tabs/Models/BeepTabHeaderAction.cs`
  Header action-slot definition.
- `Tabs/Models/BeepTabOverflowPolicy.cs`
  Overflow strategy model.

### Shared Helpers and Hosts

- `Tabs/Hosts/BeepTabHeaderHost.*.cs`
  Layout, painting, mouse, keyboard, context menu, overflow, accessibility, high-contrast, and touch behavior.
- `Tabs/Helpers/BeepTabLayoutHelper.cs`
  Core header layout computations.
- `Tabs/Helpers/BeepTabHitTestHelper.cs`
  Header/action/adornment hit testing.
- `Tabs/Helpers/BeepTabOverflowCoordinator.cs`
  Visible/overflow partitioning.
- `Tabs/Helpers/BeepTabAdornmentLayoutHelper.cs`
  Rich-header geometry.
- `Tabs/Helpers/BeepTabCommandRouter.cs`
  Workspace/document command routing.
- `Tabs/Helpers/BeepTabWorkspaceMruTracker.cs`
  MRU switching.
- `Tabs/Helpers/BeepTabAccessibleObjectFactory.cs`
  Accessibility object creation.
- `Tabs/Helpers/BeepTabFocusVisualHelper.cs`
  Focus visuals.
- `Tabs/Helpers/BeepTabRtlLayoutHelper.cs`
  RTL mirroring.
- `Tabs/Helpers/BeepTabInputPolicy.cs`
  Input policy surface.

### Internal Bootstrap Seams

- `Tabs/Adapters/BeepDocumentHostWorkspaceAdapter.cs`

These are allowed while the cutover is in progress, but they must not define the premium public contract.

## Execution Order and Dependencies

1. Phase 1: complete the Beep-owned page model cutover, make runtime content hosting stable under `BeepTabContentHost`, and remove the remaining `TabPage`-shaped assumptions from the premium surface.
2. Phase 2: unify rich header rendering, overflow, and action slots once the page model is stable.
3. Phase 3: finalize document/workspace behavior and decide the long-term `DocumentHost` relationship.
4. Phase 4: rebuild design-time around the new page model and complete accessibility, samples, and quality gates.

Phase 2 depends on Phase 1. Phase 3 depends on Phase 1 and benefits from Phase 2. Phase 4 depends on the new page model and should not be finalized against the old `TabPage`-oriented designer surface.

## Tracking and Validation

- Progress tracker: `MASTER-TODO-TRACKER.md`
- Tabs README: `Tabs/Readme.md`
- Demo surface: `Beep.Sample.Winform.Features/Forms/BeepTabsDemoForm.cs`

Each phase should end with runtime validation, designer validation where applicable, and documentation updates that keep the roadmap, tracker, and README aligned.

## Non-Goals

- Preserving a permanent premium API based on `TabPage`
- Extending native `TabControl` header behavior as the long-term architecture
- Shipping a full docking manager as part of the first commercialization pass
- Cloning a third-party library API exactly

## Expected Outcome

After these phases, `BeepTabs` should present as a credible commercial tab control: fully owned header and content architecture, rich tab metadata, overflow and action slots, document/workspace semantics, strong accessibility, and a professional design-time experience, without relying on native tab header behavior or `TabPage` in the premium API.