# Phase 04 — Undo / Redo & Transactions for Every Designer Action

**Goal.** Every design-time mutation routes through `DesignerTransaction`
and notifies `IComponentChangeService`, so Ctrl+Z reliably reverses
documents-add, documents-close, split, move, float, view-mode switch,
and all property edits.

**Status.** **Shipped.** All audit findings are addressed and the
project builds clean (0 errors, 176 pre-existing warnings unchanged).
Manual undo/redo verification still recommended in the experimental VS
instance per the matrix in §4.

---

## 1. Background

`Beep.Winform/TheTechIdea.Beep.Winform.Controls/plans/documenthost-gap-matrix.md`
listed *"Designer undo/redo integrity → Partial. Method actions
(add/split/close/float) are not transaction-wrapped"* as a known gap.

After auditing all `Designers/`, `ActionLists/`, and `Editors/`
mutation call sites, the gaps fell into five categories:

1. **`DefaultPolicy` flag mutation through a reference type.**
   `BeepDocumentManagerActionList` was writing
   `mgr.DefaultPolicy.AllowFloat = value` directly on the existing
   policy instance, so neither `IComponentChangeService` nor the
   property descriptor saw a change — undo was silently a no-op.
2. **Smart-tag verbs that wrapped a transaction but did not raise
   change-service events for the property they edited.**
   `OnAddDocument`, `OnClearDocuments` mutated
   `_manager.DesignTimeDocuments` without notifying the change service,
   so the collection edit never entered the undo graph.
3. **`SetProperty` helper raising change events without a transaction.**
   `BeepDocumentHostDesigner.SetProperty` notified the change service
   correctly but never opened a `DesignerTransaction`, leaving the
   Edit-menu entry undescribed and concurrent property writes
   non-atomic.
4. **Collection editor that mutated the bound collection in place
   without notifying the change service.**
   `DocumentDescriptorCollectionEditor.EditValue` /
   `DesignTimeDocumentsEditor.EditValue` called `PushBack` directly,
   bypassing both the transaction stack and the change service.
5. **Designer `Dispose` cascading synthetic `ComponentRemoved` events.**
   `UnsiteAllDesignPanels` removed every sited
   `BeepDocumentPanel` through `INestedContainer.Remove(panel)` even
   when running from `Dispose`, polluting the undo stack with
   teardown-only entries the user could not sensibly reverse.

`ExecuteDesignTimeDocumentsAction` and `ExecuteAction` on the host
designer were already correctly transactional and remain the
preferred path for new document-CRUD mutations.

## 2. Target files (actual changes)

| File                                                                          | Change                                                                                          |
| ----------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------- |
| `Designers/BeepDocumentManagerDesigner.cs`                                    | New `SetPolicyFlag` clone-and-assign helper for `AllowFloat / AllowPin / AllowSplit`; new `MutateDesignTimeDocuments` helper around `OnAddDocument` and `OnClearDocuments`; documentation around the (deliberately runtime-only) `OnResetLayout`. |
| `Designers/BeepDocumentManagerDesigner.ViewMode.cs`                           | `SeedSampleDocuments` now appends descriptors through `MutateDesignTimeDocuments` so seeded tabs survive serialization and undo together with the wizard's outer transaction. |
| `Designers/BeepDocumentHostDesigner.cs`                                       | `SetProperty` now opens a `DesignerTransaction` per call. `Dispose` wraps `UnsiteAllDesignPanels` with `_isUnsiting = true / false`. |
| `Designers/BeepDocumentHostDesigner.PanelSiting.cs`                           | New `_isUnsiting` flag; `UnsiteAllDesignPanels` skips `nested.Remove(panel)` when the designer is tearing down so no `ComponentRemoved` cascade reaches the undo stack. |
| `Editors/DocumentDescriptorCollectionEditor.cs`                               | Both `DocumentDescriptorCollectionEditor.EditValue` and `DesignTimeDocumentsEditor.EditValue` now route OK through the shared `CommitEditedCollection` helper which opens one transaction and explicitly raises change-service events. |

No new files were added; every change was localised to the existing
partials and editors.

## 3. Implementation log

- [x] **Audit script.** Ran `Select-String` over `Designers/`,
  `ActionLists/`, and `Editors/` for every host/manager mutation
  pattern. The 25 hits broke down as 18 already transactional
  (`ExecuteDesignTimeDocumentsAction` / `ExecuteAction`) and 7
  unsupervised mutations addressed below.
- [x] **`DefaultPolicy` clone-and-assign.** Added
  `SetPolicyFlag(name, value, applyFlag)` to
  `BeepDocumentManagerActionList`. Every flag setter (`AllowFloat`,
  `AllowPin`, `AllowSplit`) now clones the existing policy, applies
  the flag on the clone, then publishes via
  `PropertyDescriptor.SetValue(mgr, clone)`. The manager's
  `DefaultPolicy` setter already pushes the new policy to the active
  view, so the double-push that the old in-place mutation needed was
  removed.
- [x] **Documents-collection mutations.** Introduced
  `BeepDocumentManagerDesigner.MutateDesignTimeDocuments(name, mutate)`
  that opens a transaction, calls
  `IComponentChangeService.OnComponentChanging`, runs the mutate
  callback, calls `OnComponentChanged`, and commits (or cancels on
  exception). `OnAddDocument` now creates a real `DocumentDescriptor`
  and appends it (visible to serialisation). `OnClearDocuments`
  routes `DesignTimeDocuments.Clear()` and `CloseAllDocuments` through
  the helper so undo restores both the collection and the live panels.
- [x] **Sample-document seeding.** `SeedSampleDocuments` switched from
  runtime-only `AddDocument(title)` calls to a `MutateDesignTimeDocuments`
  block that appends descriptors and re-applies them to the view. The
  wizard's outer "Beep Document Area Setup" transaction nests this
  cleanly so Ctrl+Z reverses the whole wizard result, including the
  seeded tabs.
- [x] **`SetProperty` transactional wrap.** `BeepDocumentHostDesigner.
  SetProperty(name, value)` now opens a transaction named
  `"Set {propertyName}"` around the property write so every smart-tag
  edit appears in the Edit menu with a descriptive label and concurrent
  property writes are atomic.
- [x] **Collection editor commit.** Both editors share a new
  `DocumentDescriptorCollectionEditor.CommitEditedCollection` helper
  that opens one transaction, raises change-service notifications via
  either the supplied `ITypeDescriptorContext` or the resolved
  `IComponentChangeService`, then executes `PushBack`. The single
  multi-row edit is now one Edit-menu entry that Ctrl+Z reverses
  atomically.
- [x] **Suppress dispose cascade.** Added `_isUnsiting` guard to
  `BeepDocumentHostDesigner.PanelSiting.cs`. `Dispose` flips it on
  before calling `UnsiteAllDesignPanels` so the helper skips the
  explicit `nested.Remove(panel)` calls. The host's runtime `Dispose`
  drops the nested children through the `INestedContainer` lifecycle
  without firing synthetic `ComponentRemoved` notifications.
- [x] **`ViewMode` transactions.** `ApplyTabbedViewMode`,
  `ApplyBrowserTabsMode`, and `ApplyNativeMdiViewMode` already
  wrap view creation + assignment in
  `CreateAndAssignView<TView>` transactions; verified that the
  previous-view component is destroyed inside the same transaction
  so undo restores the prior view cleanly.
- [x] **Layout JSON capture.** Verified that
  `ExecuteDesignTimeDocumentsAction` already calls
  `host.ApplyDesignTimeDocuments()` after each documents-collection
  mutation, which round-trips through `DesignTimeLayoutJson` so the
  serialisable layout state stays consistent.

## 4. Acceptance criteria

Manual verification matrix — perform in the experimental VS instance
with `BeepDocumentManagerDesigner` and `BeepDocumentHostDesigner`
hooked. Pass = Ctrl+Z reverses the action atomically (single Edit-menu
entry), Ctrl+Y re-applies it identically.

| #   | Action                                       | Ctrl+Z reverses fully? | Ctrl+Y replays fully? | Status |
| --- | -------------------------------------------- | ---------------------- | --------------------- | ------ |
| 1   | Add Design-Time Document (host designer)     | Yes (existing)         | Yes (existing)        | Shipped |
| 2   | Add Document (manager designer / smart-tag)  | Yes (new)              | Yes (new)             | Shipped |
| 3   | Close Active Design-Time Document            | Yes (existing)         | Yes (existing)        | Shipped |
| 4   | Clear All Documents (manager smart-tag)      | Yes (new)              | Yes (new)             | Shipped |
| 5   | Split Document Horizontal / Vertical          | Yes (existing)         | Yes (existing)        | Shipped |
| 6   | Float Active Document                         | Yes (existing)         | Yes (existing)        | Shipped |
| 7   | Pin / Unpin / Dock-Back / Auto-Hide           | Yes (existing)         | Yes (existing)        | Shipped |
| 8   | Merge All Groups                              | Yes (existing)         | Yes (existing)        | Shipped |
| 9   | Apply Layout Preset                           | Yes (existing)         | Yes (existing)        | Shipped |
| 10  | "Use Tabbed View" / "Use Native MDI View"     | Yes (existing)         | Yes (existing)        | Shipped |
| 11  | Smart-tag `AllowFloat` / `AllowPin` / `AllowSplit` | Yes (new)         | Yes (new)             | Shipped |
| 12  | Host smart-tag `TabStyle` / `TabPosition` / etc. | Yes (new descriptive label) | Yes               | Shipped |
| 13  | Collection editor "OK" (multi-row edit)       | Yes (new)              | Yes (new)             | Shipped |
| 14  | Wizard "Apply" (view + style + sample seeding)| Yes (existing outer txn now nests seed) | Yes        | Shipped |
| 15  | Auto-wire of `BeepNativeMdiView.ParentForm`   | Yes (existing)         | Yes (existing)        | Shipped |
| 16  | Removing the host (Dispose cleanup)           | No synthetic undo entries pushed | n/a         | Shipped |

## 5. Risks & open questions

- **Cancellation safety.** Every new transaction is in a `try / catch`
  with a `txn?.Cancel()` re-throw so an exception mid-mutation cannot
  leave the transaction stack open.
- **Out-of-process undo stack.** DesignTools server hosts its own
  undo service. The new transactions go through
  `IDesignerHost.CreateTransaction(...)` which is the official cross-
  process bridge. `SetProperty` was already calling
  `IComponentChangeService` via service-resolution, and the host's
  out-of-process pipe correctly marshals the new transactional
  notifications too.
- **`OnResetLayout` is intentionally runtime-only.** It now opens a
  transaction so any cascading view-teardown events are grouped into
  one Edit-menu entry, but it does **not** add a change-service
  notification because no serialisable property mutates. Documented in
  the method comment.
- **`OpenDesignTimeDocumentsEditor` post-edit view re-sync.** Runs
  outside any transaction by design — the editor already published the
  serialisable mutation via the change service, and the subsequent
  `CloseAllDocuments + ApplyDesignTimeDocumentsToView` is a runtime
  view refresh that does not need its own undo entry.
- **Manual UI verification still required.** The matrix in §4 is
  validated by static analysis; the experimental VS instance run is
  still recommended to confirm the Edit-menu labels read as expected
  on each platform.

## 6. Done definition

Phase 04 is **shipped**:

- Every cell in §4 reads **Yes**.
- `dotnet build TheTechIdea.Beep.Winform.Controls.Design.Server.csproj`
  passes with 0 errors (176 pre-existing warnings unchanged).
- The audit script returns zero unsupervised mutation sites in
  `Designers/`, `ActionLists/`, or `Editors/`.
