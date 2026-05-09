# Phase 3: Document Workspace and Advanced Interactions

## Objective

Build the document/workspace behavior that makes the control feel like a real IDE or workbench tab system rather than only a styled navigation surface.

## In Scope

- explicit operating modes
- pinned and preview tabs
- dirty state and close guards
- close-current, close-others, close-all, close-to-right, pin, move, reopen, and reveal commands
- MRU switching and quick-switch behavior
- context menus and drag/detach seams
- `DocumentHost` alignment

## Out of Scope

- full docking manager implementation
- multi-window hosting implementation beyond the seam/event contract

## Primary File Targets

- `Tabs/BeepTabs.WorkspaceCommands.cs`
- `Tabs/BeepTabs.ClosedTabHistory.cs`
- `Tabs/BeepTabs.WorkspaceMru.cs`
- `Tabs/BeepTabs.ContextMenu.cs`
- `Tabs/BeepTabs.Keyboard.cs`
- `Tabs/Hosts/BeepTabHeaderHost.Keyboard.cs`
- `Tabs/Hosts/BeepTabHeaderHost.ContextMenu.cs`
- `Tabs/Hosts/BeepTabHeaderHost.Mouse.cs`
- `Tabs/Hosts/BeepTabHeaderHost.Overflow.cs`
- `Tabs/Models/BeepTabWorkspaceState.cs`
- `Tabs/Helpers/BeepTabCommandRouter.cs`
- `Tabs/Helpers/BeepTabWorkspaceMruTracker.cs`
- `Tabs/Helpers/BeepTabDetachCoordinator.cs` (planned)
- `Tabs/Adapters/BeepDocumentHostWorkspaceAdapter.cs`
- `DocumentHost/BeepDocumentTab.cs`
- `DocumentHost/BeepDocumentPanel.cs`
- `DocumentHost/BeepDocumentTabStrip.cs`
- `DocumentHost/BeepDocumentHost.Documents.cs`
- `DocumentHost/BeepDocumentHost.Events.cs`

## Dependencies

Phase 3 assumes Phase 1 has established the Beep-owned page/container model and Phase 2 has stabilized the header/render contract.

## Workstreams

### 1. Formalize Tab Modes

Support and document three explicit modes:

- `Navigation`
- `Documents`
- `Workspace`

Mode must control behavioral policy rather than simply toggling visuals.

### 2. Finish Workspace Metadata And Rules

The per-page/workspace model should support at least:

- pinned
- preview
- dirty
- reuse-preview-slot
- can-close
- can-reorder
- grouping key or affinity hint

Rules should include:

- pinned pages sort or remain ahead according to policy
- preview pages reuse a preview slot until promoted
- dirty pages trigger close guards and visual indicators

### 3. Centralize Commands

All document/workspace commands should route through one command model rather than being split across click handlers and context-menu code.

Required commands:

- close current
- close others
- close all
- close to the right
- pin or unpin
- move left or right
- reopen last closed
- reveal in overflow / quick switch

### 4. Complete Keyboard And MRU Behavior

Support product-grade keyboard behavior:

- `Ctrl+Tab` and `Ctrl+Shift+Tab`
- arrow navigation
- `Home` and `End`
- `Delete` and `Ctrl+W`
- `Ctrl+Shift+T`
- `Ctrl+P`
- `Ctrl+1` through `Ctrl+9` for direct page selection, including numpad equivalents
- context-menu key / `Shift+F10`

MRU should be explicit and deterministic rather than incidental.

### 5. Define Drag/Detach Seams

The first commercialization pass does not need a full docking manager, but it does need a clean seam for future transfer scenarios.

Define and document the event/coordination surface for:

- reorder
- drag start
- potential detach
- transfer request

### 6. Decide The `DocumentHost` Relationship

This phase should make one decision explicit:

- either `BeepTabs` becomes the shared substrate for document tabs
- or `BeepDocumentHostWorkspaceAdapter` becomes the stable bridge and the contracts between the two are fixed

Avoid letting two premium tab stacks drift independently.

## Deliverables

1. Stable mode-aware behavior.
2. Canonical command routing for document/workspace scenarios.
3. Deterministic MRU, quick-switch, and close-selection policies.
4. Explicit drag/detach seam for future host transfer.
5. A documented `DocumentHost` integration direction.

## Acceptance Criteria

- `BeepTabs` behaves credibly in document/workspace scenarios.
- Commands and context menus route through one shared command model.
- MRU and quick-switch are deterministic and documented.
- Pinned, preview, and dirty rules are explicit and testable.
- The long-term relationship between `BeepTabs` and `DocumentHost` is no longer ambiguous.

## Risks And Mitigations

- Risk: document behavior bleeds into navigation scenarios.
  Mitigation: keep mode policy explicit and centralized.

- Risk: `DocumentHost` and `BeepTabs` diverge into two premium tab implementations.
  Mitigation: make the ownership decision a named Phase 3 deliverable rather than leaving it implicit.