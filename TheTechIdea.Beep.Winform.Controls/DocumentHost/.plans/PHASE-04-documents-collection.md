# Phase 04 — Design-Time `Documents` Collection Editor

> **Owner:** _Beep platform team_  · **Status:** 🟩 Done (E1–E2 deferred)  · **Predecessor:** Phase 02

## Why This Phase Exists

DevExpress, Telerik, Syncfusion, and Infragistics all expose a **first-class
collection editor** for documents:

> Right-click DocumentManager → **Edit Documents…** → modal dialog with
> add/remove/reorder + per-document property grid.

This phase brings the point-and-click experience to `BeepDocumentManager` and
wires the existing per-host editor into the manager's designer.

## Tasks

### A. Collection-editor host

- [x] **A1** `DesignTimeDocumentsEditor` (UITypeEditor) already exists in
  `Design.Server/Editors/DocumentDescriptorCollectionEditor.cs`.
  Opens the full `DocumentDescriptorEditorForm` (grid + icon + colour + reorder + preview).
- [x] **A2** `BeepDocumentManager.DesignTimeDocuments` property added:
  `Collection<DocumentDescriptor>` with `[DesignerSerializationVisibility(Content)]`.
- [x] **A3** `CreateNewItemTypes` / fresh `PersistenceKey` covered by the
  existing `DocumentDescriptorEditorForm.AddItem()` logic.
- [x] **A4** `BeepDocumentManager.ApplyDesignTimeDocuments()` pushes the
  collection to the view on `ApplyViewSettings()`.

### B. Custom editor dialog

- [x] **B1** `DocumentDescriptorEditorForm` is the custom dialog (sprint 17.2):
  DataGridView + icon picker + colour picker + reorder + live mini tab-strip preview.
- [x] **B2** Themed via `SystemColors` (designer-safe; no BeepThemesManager runtime call).

### C. Designer integration

- [x] **C1** Verb **"Edit Documents…"** added to `BeepDocumentManagerDesigner` — calls
  `OpenDesignTimeDocumentsEditor()` which uses `DesignTimeDocumentsEditorContext` +
  `DesignTimeDocumentsEditor.EditValue` then re-applies the collection to the view.
- [x] **C2** Verb **"Add Document"** already present (retained, now also in smart-tag
  under "Documents" group).
- [x] **C3** Verb **"Clear All Documents"** added with Yes/No confirmation;
  clears `DesignTimeDocuments` and calls `CloseAllDocuments()`.
- [x] **C4** `BeepDocumentHostDesigner` already has "Edit Design-Time Documents…",
  "Add Document", and "Clear All Documents" verbs — no changes needed.

### D. Smart-tag inline pickers

- [x] **D1** `TabStyle` dropdown in `BeepDocumentManagerActionList` (proxies to
  `BeepTabbedView.Host.TabStyle`; group shown only when view is `BeepTabbedView`).
- [x] **D2** `TabPosition` dropdown (proxies to `BeepTabbedView.Host.TabPosition`).
- [x] **D3** `CloseMode` dropdown (proxies to `BeepTabbedView.Host.CloseMode`).
- [x] **D4** `AllowFloat` / `AllowPin` / `AllowSplit` checkboxes in "Policy" group
  (mutate `BeepDocumentManager.DefaultPolicy` and call `PushPolicy`).

### E. Drop-onto-document

- [ ] **E1** Deferred — requires `IDesignerHost.CreateComponent` parent overload; low
  designer-safety ROI for the effort.
- [ ] **E2** Deferred.

### F. Serialisation

- [x] **F1** `DocumentDescriptor` is `INotifyPropertyChanged`; serialised via
  `[DesignerSerializationVisibility(Content)]` on both `BeepDocumentManager.DesignTimeDocuments`
  and `BeepDocumentHost.DesignTimeDocuments`.
- [x] **F2** Generated `.Designer.cs` produces
  `this.beepDocumentManager1.DesignTimeDocuments.Add(new DocumentDescriptor { … });`
  per document (same mechanism as `BeepDocumentHost`).
- [x] **F3** No duplication: manager serialises to `DesignTimeDocuments`; host
  serialises to its own `DesignTimeDocuments`; both are applied independently.

## Acceptance Criteria

- ✅ Right-click manager → **Edit Documents…** opens the custom grid editor.
- ✅ Add 3 documents, save form → `.Designer.cs` shows 3 `DesignTimeDocuments.Add(…)` lines.
- ✅ Smart-tag shows Tab Style / Tab Position / Close Mode dropdowns when view is `BeepTabbedView`.
- ✅ Smart-tag shows Allow Float / Allow Pin / Allow Split checkboxes under Policy.
- ✅ **Clear All Documents** asks for confirmation before acting.

## Out of Scope

- Runtime CRUD on documents (already works via `AddDocument`).
- Window-menu auto-population (Phase 06).
- Drop-onto-document at design time (deferred, E1–E2).
