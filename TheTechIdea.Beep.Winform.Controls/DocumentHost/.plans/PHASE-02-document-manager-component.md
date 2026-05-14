# Phase 02 — `BeepDocumentManager` Non-Visual Component

> **Owner:** _unassigned_  · **Status:** � Done (A1-D5 implemented)  · **Predecessor:** Phase 01  · **Blocks:** 03, 04, 06

## Why This Phase Exists

> User: _"the MDI is not user-friendly like DevExpress. Like DevExpress there should
> be a component on form where its controls all the form MDI."_

DevExpress, Telerik, Syncfusion, and Infragistics all follow the same pattern:

- **A non-visual `Component`** (lives in the form's component tray, _not_ as a `Control`)
  is the **orchestrator**. It exposes the configuration surface, the documents
  collection, the events, the smart-tag, and the designer verbs.
- **A visible host `Control`** (often a `BaseView`) is **assigned to** the manager
  via a `View` / `Host` property. It only renders.
- The manager wires the form-level concerns: `Window` menu, status strip, keyboard,
  layout persistence, theming.

The Beep stack today has only the host (`BeepDocumentHost`). Adding the
**`BeepDocumentManager`** brings us to DevExpress-style UX where the developer
drags one component, picks the view, and everything _just works_.

## Component Architecture

```
Form
 ├─ components (tray)
 │    └─ BeepDocumentManager  ◀── orchestrator (this phase)
 │         ├─ Host           → BeepDocumentHost (Phase 02 enables, Phase 03 picks view)
 │         ├─ Theme          → string, fan-out to Host
 │         ├─ AutoSaveLayout → bool
 │         ├─ SessionFile    → string
 │         ├─ WindowMenu     → MenuStrip (auto-populate)
 │         ├─ StatusStripOwner → StatusStrip (breadcrumb, dirty)
 │         ├─ Documents      → BindingList<DocumentDescriptor> (Phase 04)
 │         ├─ DefaultPolicy  → AllowFloat / AllowPin / AllowSplit / MaxSplitDepth
 │         └─ AutoWireForm   → bool (default true)
 │
 └─ Controls
       └─ BeepDocumentHost (visible)  ◀── pure renderer
```

## Tasks

### A. Component skeleton

- [x] **A1** New file
  `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentManager.cs`.
- [x] **A2** Declare:
  ```csharp
  [ToolboxItem(true)]
  [ToolboxBitmap(typeof(BeepDocumentManager), "Resources.BeepDocumentManager.bmp")]
  [DesignerCategory("Component")]
  [Designer("TheTechIdea.Beep.Winform.Controls.Design.Server.Designers.BeepDocumentManagerDesigner, …")]
  [ProvideProperty("DocumentTitle", typeof(Control))]
  [ProvideProperty("DocumentIconPath", typeof(Control))]
  [DefaultEvent(nameof(ActiveDocumentChanged))]
  [DefaultProperty(nameof(Host))]
  public sealed class BeepDocumentManager : Component, IExtenderProvider, ISupportInitialize
  ```
- [x] **A3** `IExtenderProvider.CanExtend(object extendee)` → returns
  `extendee is Control c && c is not BeepDocumentManager`.

### B. Public surface

- [x] **B1** Properties: Host, ThemeName, AutoSaveLayout, SessionFile, WindowMenuOwner, WindowMenuText, StatusStripOwner, DefaultPolicy (BeepDocumentPolicy), AutoWireForm implemented.
- [x] **B2** Events: DocumentAdded, DocumentRemoved, ActiveDocumentChanged, DocumentClosing (TabClosingEventArgs), LayoutChanged (ManagerLayoutChangedEventArgs) implemented and bridged from host.
- [x] **B3** Methods: AddDocument, RemoveDocument, ActivateDocument, TryGetDocument (stub — Phase 04), BeginBatchAddDocuments, EndBatchAddDocuments, SaveLayout, LoadLayout, RegisterTemplate implemented.

### C. Wiring lifecycle

- [x] **C1** `ISupportInitialize.BeginInit()` — defers wiring. Done.
- [x] **C2** `ISupportInitialize.EndInit()` — calls `ApplyHostSettings()` which pushes theme, layout, policy, wires Window menu. Done.
- [x] **C3** `Dispose(bool)` — unwires host events, clears refs. Done.
- [x] **C4** `internal void DetachHost(BeepDocumentHost host)` implemented.

### D. IExtenderProvider — per-Control attached properties

This is the DevExpress trick that lets you set a friendly title on any child
control _through_ the manager.

- [x] **D1** `[DefaultValue(null)] string GetDocumentTitle(Control c)` implemented.
- [x] **D2** `void SetDocumentTitle(Control c, string? value)` implemented.
- [x] **D3** `GetDocumentIconPath` / `SetDocumentIconPath` implemented.
- [x] **D4** Storage in `ConditionalWeakTable<Control, AttachedDocInfo>` + parallel `HashSet<Control>` for enumeration.
- [x] **D5** `AutoAddExtendedControls()` called from `EndInit` — scans extended controls and opens them as document panels.

### E. Designer (`BeepDocumentManagerDesigner`)

Lives in the design-server assembly:
`TheTechIdea.Beep.Winform.Controls.Design.Server/Designers/BeepDocumentManagerDesigner.cs`

- [ ] **E1** Inherit `ComponentDesigner` (not `ControlDesigner`).
- [ ] **E2** `DesignerVerbs`:
  - [ ] "Add Document…"
  - [ ] "Edit Documents…" (opens collection editor — Phase 04)
  - [ ] "Pick Host" (drop-down listing `BeepDocumentHost` instances on the form)
  - [ ] "Auto-Add 'Window' Menu" (creates a MenuStrip and assigns it)
  - [ ] "Reset Layout"
  - [ ] "Export Layout JSON…" / "Import Layout JSON…"
- [ ] **E3** `ActionLists` (smart-tag) for `Host`, `Theme`, `AutoSaveLayout`,
  `DefaultPolicy`, `WindowMenuOwner`, plus a one-click *"Wire up everything on
  the active form"* button.
- [ ] **E4** Property-grid filtering — hide noise inherited from `Component`.
- [ ] **E5** Suppress events that don't make sense in the property grid
  (`Disposed`, etc.).

### F. Toolbox icon & resx

- [ ] **F1** Add `BeepDocumentManager.bmp` (16×16, magenta-key) to embedded resources.
- [ ] **F2** Decorate type with `[ToolboxBitmap]`.

### G. Default host bridge

- [ ] **G1** If `Host == null` and `AutoCreateHost == true`, create a transient
  hidden host so `AddDocument` works programmatically before a visible host is
  assigned. (Edge-case nice-to-have — defer if scope grows.)
- [ ] **G2** If `Host` already has documents when assigned, do not destroy them
  — merge the manager's `Documents` collection into the host (skip duplicates by
  `PersistenceKey`).

### H. Migration helpers for existing forms

- [ ] **H1** Designer action "Convert: drop a manager onto this form" — adds a
  `BeepDocumentManager`, hooks `Host`, copies all settings from the host onto
  the manager.
- [ ] **H2** README / tutorial in `BeepDocumentManager.Readme.md` explaining how
  to migrate `MainFrm_MDI` from "host only" to "manager + host".

## Acceptance Criteria

- ✅ Drag `BeepDocumentManager` onto a form → designer shows it in the tray.
- ✅ Pick `Host` from the dropdown → host adopts the manager's settings.
- ✅ Save form → `.Designer.cs` contains a clean `new BeepDocumentManager()` plus
  property assignments; **no** copy of `Documents` is duplicated between
  manager and host.
- ✅ Reopen form → manager is reconstructed, host is rebound, documents reappear.
- ✅ Removing the manager does **not** crash the designer (Phase 01 ensures this).
- ✅ Removing the host while the manager is still on the form gracefully nulls
  `Host` and shows a property-grid warning.

## Out of Scope

- The collection editor UI (Phase 04 owns it).
- View modes (Phase 03).
- Dock panels (Phase 05).

## Risks

- Designer serialisation of an `IExtenderProvider` requires extra `ResourceManager`
  attention; test with a control that has `DocumentTitle = "X"`.
- `ISupportInitialize` order is fragile. Mitigation: defensive null checks
  everywhere, no exceptions during `EndInit`.

## Notes

- Mirror DevExpress's `DocumentManager.View` property naming — Phase 03 will rename
  `Host` to `View` if it makes the API feel more DevExpress-native (decide in
  Phase 03, not now).
- Document the parallel between Beep's `BeepDocumentManager` and DevExpress's
  `DocumentManager` in the README so .NET WinForms devs feel at home.
