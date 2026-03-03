# BeepDocumentHost API Contract v2 (Draft for Freeze)

## Purpose
Define explicit behavior guarantees for `BeepDocumentHost` before deeper docking/designer enhancements.  
This contract is the target for implementation and tests; incompatible changes require a version note.

## Compatibility Policy
1. Keep existing v1 APIs functional unless marked obsolete.
2. New v2 events/methods should be additive first.
3. Any behavioral breaking change must be guarded by a compatibility switch.

---

## Core Object Model Contract
1. A document is identified by a stable `DocumentId` unique within one host.
2. A document can be in exactly one state at a time:
   - `Docked`
   - `Floated`
   - `AutoHidden`
   - `Closed`
3. A docked document belongs to exactly one group.
4. At most one document is active per host.

## Method Contracts
### `AddDocument(...)`
1. If `documentId` already exists, operation fails with deterministic error (`InvalidOperationException` or `false` API variant).
2. New document enters `Docked` state in target group (default active group for v2).
3. If activation requested, activation pipeline is executed.

### `SetActiveDocument(documentId)`
1. Returns `false` if missing/ineligible.
2. If already active, no-op and no duplicate events.
3. Must run activation event sequence exactly once.

### `CloseDocument(documentId)`
1. Must run close pipeline with cancellation support.
2. If canceled, no state mutation.
3. If closed, document is removed from tabs/group/layout tree and transitions to `Closed`.
4. Active fallback selection policy is deterministic (MRU-first recommended).

### `FloatDocument(documentId)` / `DockBackDocument(documentId)`
1. Float only from `Docked`; dock-back only from `Floated`.
2. Must preserve document identity, title, icon, modified/pinned metadata.
3. Float close (window X) behavior must be explicit:
   - either real close pipeline, or auto-dock fallback, but not ambiguous.

### `Split*` / `MoveDocumentToGroup`
1. Must preserve active doc unless target operation explicitly activates another.
2. Must keep layout tree valid after each operation.
3. Empty non-primary groups are removed deterministically.

### `SaveLayout` / `RestoreLayout`
1. Save must emit schema version.
2. Restore must validate schema and return structured diagnostics on failures.
3. Restore should support partial recovery and report skipped nodes.

---

## Event Contracts (Required Ordering)
### Activation
1. `ActiveDocumentChanging` (new, cancelable)
2. internal state switch
3. `ActiveDocumentChanged`

### Close
1. `DocumentClosing` (cancelable)
2. internal removal/layout update
3. `DocumentClosed`

### Float/Dock
1. `DocumentFloating` (new, cancelable)
2. state transition to floated
3. `DocumentFloated`
4. `DocumentDocking` (new, cancelable)
5. state transition to docked
6. `DocumentDocked`

### Transfer Between Hosts
1. source `DocumentDetaching` (cancelable)
2. source transition
3. target attach transition
4. target `DocumentAttached`

Rules:
1. No duplicate event firing for a single logical operation.
2. Cancelable events must leave state unchanged.
3. Events must be raised on UI thread.

---

## Designer-Time Contract
1. All smart-tag and verb actions that mutate state must run inside designer transactions.
2. All design-time mutations must participate in:
   - `IComponentChangeService` notifications
   - undo/redo integration
   - `InitializeComponent` serialization where applicable
3. Design-time should not open modal runtime UI unless explicitly user-triggered and safe.
4. Design-time placeholder documents must serialize deterministically (stable IDs optional, stable order required).

## Serialization Contract v2
Layout payload must include:
1. Schema version + optional migration metadata.
2. Document nodes:
   - id/title/icon
   - pinned/modified
   - optional custom state bag
3. Layout tree:
   - split nodes with orientation and ratio
   - tab groups with selected doc
4. Floating windows:
   - bounds + window state
5. Auto-hide state:
   - side + order + selected/expanded state
6. Active document and MRU snapshot (optional).

## Error and Diagnostics Contract
1. Public operations should have a non-throwing variant or deterministic exceptions only.
2. Restore operations must provide actionable diagnostics for:
   - invalid schema
   - unknown node type
   - missing document references
3. Add trace hooks for designer/runtime troubleshooting.

## Test Contract (Minimum)
1. Event ordering tests for activation/close/float/dock.
2. Layout invariants tests after split/move/close.
3. Serialization roundtrip including split/float/auto-hide.
4. Designer integration tests for smart-tag actions + undo/redo.

## Proposed New API Surface (Additive)
1. Events:
   - `ActiveDocumentChanging`
   - `DocumentFloating`
   - `DocumentDocking`
2. Methods:
   - `TryCloseDocument(string id, out CloseFailureReason reason)`
   - `TryRestoreLayout(string json, out LayoutRestoreReport report)`
3. Types:
   - `DocumentState`
   - `LayoutRestoreReport`
   - `IDocumentHostCommandService`

## Items to Defer Until v2.1
1. Keyboard-driven designer docking.
2. Advanced animation policy configuration.
3. Multi-monitor float persistence edge cases.
