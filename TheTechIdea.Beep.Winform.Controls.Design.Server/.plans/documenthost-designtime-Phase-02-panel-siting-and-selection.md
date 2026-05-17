# Phase 02 — Panel Siting & Design-Time Tab Selection

**Goal.** Make every `BeepDocumentPanel` selectable in the Visual Studio
designer, with its properties surfacing in the Properties window and
its child controls participating in normal designer workflows
(drag-drop, snap-lines, undo/redo, naming, codegen).

**Status.** Foundation landed in this iteration. Hardening items below
are pending.

**Status legend.** `[x]` shipped — `[~]` partially done — `[ ]` pending.

---

## 1. Background

### 1.1 The bug

`BeepDocumentHost.AddDocument(...)` (file
`TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentHost.Documents.cs`,
line ~242) creates panels with `new BeepDocumentPanel(id, title)` and
adds them to its private `_contentArea.Controls`. These panels are
*never* sited — `Site == null`, never registered in
`IDesignerHost.Container.Components`.

Consequences:

- `ISelectionService.SetSelectedComponents([panel])` silently no-ops, so
  clicking a tab header at design time does nothing visible in the
  Properties window.
- `BeepDocumentPanelDesigner` is registered but never instantiated for
  these panels because the designer host has no record of them.
- Toolbox drop-redirection in `BeepDocumentHostDesigner.OnDragDrop` ends
  up parenting controls to the `BeepDocumentHost` directly instead of
  inside the active document panel.
- Saving the form loses any structural information about the panels
  (they're just rebuilt at runtime from `DesignTimeDocuments`).

### 1.2 The supporting bug (Bug B)

`BeepDocumentHostDesigner.GetActiveDocumentPanel` (line ~1134) walked
`host.Controls` and matched `panel.Name == activeId`. Panels live under
`_contentArea`, not under `host`, and `panel.Name` is never set —
the lookup always returned null. This was independent from the siting
bug but blocked the same drop-redirection flow.

## 2. Target files

| File                                                                                            | Responsibility                                                                                 |
| ----------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------- |
| `Designers/BeepDocumentHostDesigner.cs`                                                         | Becomes `partial`. Fixes `GetActiveDocumentPanel`. Calls `SiteAllDesignPanels` at hook points. |
| `Designers/BeepDocumentHostDesigner.PanelSiting.cs` *(new)*                                     | `INestedContainer` plumbing, name sanitisation, `SiteAllDesignPanels`, `UnsiteAllDesignPanels`. |
| `Designers/BeepDocumentPanelDesigner.cs`                                                        | Already filters properties; will need `Locked` enforcement plus parented-via-host check.       |
| `Designers/DesignRegistration.cs`                                                               | Already registers `BeepDocumentPanelDesigner`; verify after siting fix that VS uses it for nested instances. |
| `Designers/ActionLists/DocumentHostActionList.cs`                                               | Verify "Edit Design-Time Documents…" path still seeds panels post-fix.                          |
| `Designers/Dialogs/DesignTimeDocumentsEditor*.cs`                                               | Verify that the editor's `OK` round-trip results in newly-sited panels.                         |
| `.plans/documenthost-designtime-Phase-02-panel-siting-and-selection.md`                         | This document.                                                                                  |

## 3. Implementation steps

### 3.1 Done

- [x] **`partial`** keyword on `BeepDocumentHostDesigner`.
- [x] **`GetActiveDocumentPanel` fix** — now `host.GetPanel(activeId)`.
  Inline note documents the historical bug for future maintainers.
- [x] **`BeepDocumentHostDesigner.PanelSiting.cs` partial** containing:
  - `_sitedPanels` HashSet for tracking which panels this designer
    sited (so `UnsiteAllDesignPanels` doesn't disturb foreign
    components).
  - `GetNestedContainer()` — resolves the host's `INestedContainer`
    through `_wiredHost.Site.GetService(typeof(INestedContainer))`.
  - `SiteDesignPanel(nested, panel, documentId)` — name-clash safe loop,
    swallows all exceptions (must never throw at design time).
  - `SiteAllDesignPanels()` — iterates `host.Groups` and sites every
    panel that is still un-sited.
  - `UnsiteAllDesignPanels()` — called from `Dispose`.
  - `BuildPanelComponentName` — sanitises a `DocumentId` into a
    legal C# identifier suffix for nested component naming.
  - `IsDesignTimeHost` — gates everything to design time only.
- [x] **Call-sites added**:
  - `Initialize`/`HandleCreated` lambda → `SiteAllDesignPanels()` so
    initial seeded panels (from `ApplyDesignTimeDocuments`) are sited as
    soon as the host gets a handle.
  - `ExecuteDesignTimeDocumentsAction` tail → `SiteAllDesignPanels()`
    so every add/split/move/close action re-sites new panels.
  - `Dispose` → `UnsiteAllDesignPanels()` so panels don't leak into the
    designer container.
- [x] Build verified: `TheTechIdea.Beep.Winform.Controls.Design.Server`
  → 0 errors.

### 3.2 Pending — hardening & coverage

- [ ] **Resurface lost panels.** When a panel is removed from
  `host._panels` but still exists in `_sitedPanels`, drop it from the
  nested container in the same designer transaction. Hook this from
  `BeepDocumentHost.DocumentClosed` (subscribe in `Initialize`, unhook
  in `Dispose`).
- [ ] **Rename support.** When the user renames a panel in the
  Properties window (i.e. changes the nested component `Name`),
  propagate the new name back to `BeepDocumentPanel.DocumentId` via
  `IComponentChangeService`. Today, `Name` and `DocumentId` are
  independent and the Properties window only shows `DocumentTitle`.
  Either:
  - hide the nested `Name` from the property grid (route through
    `BeepDocumentPanelDesigner.PreFilterProperties`), or
  - synchronise both fields on rename.
- [ ] **Floating-window panels.** Panels that are detached into a
  `FloatingDocumentWindow` should still be sited and remain selectable
  in the designer (floating windows are runtime-only today but the
  designer should not lose track of them).
- [ ] **`BeepDocumentPanelDesigner` parented-via-host invariant.** The
  designer's `CanBeParentedTo` already restricts parents to
  `BeepDocumentHost`; assert that all sited panels are actually
  parented under `_contentArea` (or a group's content panel), and log a
  diagnostic when this invariant breaks.
- [ ] **Properties-window filtering for nested panels.** Verify that
  `BeepDocumentPanelDesigner.PreFilterProperties` runs for sited
  instances (it ought to — designer is registered — but worth an
  explicit verification step).
- [ ] **DesignerSerializer compatibility.** Once nested components have
  names, the default `CodeDomSerializer` will emit them in
  `InitializeComponent`. Decide whether to suppress codegen for these
  panels (the runtime rebuilds them from `DesignTimeDocuments`, so
  serialised panel declarations would be duplicates). Most likely add a
  `DesignerSerializationVisibilityAttribute(Hidden)` on the nested
  components via a `TypeDescriptionProvider` injected during siting.
- [ ] **Smart-tag refresh after siting.** After `SiteAllDesignPanels()`,
  call `DesignerActionUIService.Refresh(host)` (already happens via
  `RefreshDesignerActionUI`) and also `Refresh(panel)` so the panel's
  smart-tag picks up its new sited identity.

## 4. Acceptance criteria

| #   | Test                                                                                                                                                            | Result |
| --- | --------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------ |
| 1   | Drop `BeepDocumentHost`, click "Add Design-Time Document" 3 times, click each tab header → each click puts the matching `BeepDocumentPanel` in the Properties window. | Pending verification |
| 2   | After (1), drop a `BeepButton` onto the active tab content area → button is parented inside the active `BeepDocumentPanel`, not under the host directly.        | Pending verification |
| 3   | Save the form → solution still builds, designer reopens without losing panels.                                                                                  | Pending verification |
| 4   | Edit `DesignTimeDocuments` via the collection editor, add a row, click OK → new panel appears in the host AND is selectable in Properties window.               | Pending verification |
| 5   | Close the form designer → panels are removed from the nested container (no orphans).                                                                            | Pending verification (covered by `UnsiteAllDesignPanels`) |
| 6   | Right-click the host → context menu items still work as before; left-click on a tab still selects the matching panel (regression check on Bug A fix).             | Verified (selection path now reaches a sited component) |
| 7   | Build both projects → 0 errors, no new warnings.                                                                                                                | Verified |
| 8   | Drop a toolbox control on the host's empty area when a tab is active → control parented into the active `BeepDocumentPanel` (regression on Bug B fix).            | Pending verification |

## 5. Risks & open questions

- **Out-of-process designer.** The Microsoft.DotNet.DesignTools server
  exposes services differently from the in-proc Windows Forms designer.
  `INestedContainer` is in `System.ComponentModel` so it must be
  available, but verification on both surfaces is required.
- **Duplicate component names.** The nested container scope is per
  parent component (the host). Two hosts on the same form can both
  contain a "documentPanel1" — that is by design. But the user could
  type the same `DocumentId` twice; we generate `name + "_" + n` to
  avoid clashes inside one host. Surface a designer warning when this
  happens.
- **Serialisation overlap.** With nested panels named, the standard
  serializer will emit declarations for them. Marking them
  `DesignerSerializationVisibility.Hidden` is the cleanest answer;
  Phase 06 owns the final decision.

## 6. Done definition

Phase 02 is done when all `[ ]` items are checked, acceptance tests 1–8
pass on both designer surfaces, and Phase 06 (serialisation) has
ratified the codegen approach for sited panels.
