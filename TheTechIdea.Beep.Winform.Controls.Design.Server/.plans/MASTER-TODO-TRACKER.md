# MASTER TODO TRACKER — BeepDocumentHost Design-Time Stack

**Scope.** The end-to-end design-time experience for `BeepDocumentHost`,
`BeepDocumentManager`, `BeepTabbedView`, `BeepNativeMdiView`, and
`BeepDocumentPanel` inside `TheTechIdea.Beep.Winform.Controls.Design.Server`.

**Owner.** Beep Design-Time / Controls team.

**Drivers.**

- User report: *"I cannot select any document at design time"* when using
  `BeepTabbedView` + `BeepDocumentHost`.
- User report: *"It is not clear when to use TabbedView vs NativeView"*.
- Existing `BeepDocumentHostDesigner.cs` is over 2,400 lines in a single file
  — violates the workspace rule **"one file, one class, one responsibility"**.

**Workspace conventions applied.**

- Every phase is its own document (`documenthost-designtime-Phase-NN-*.md`).
- TODO checklists, target files, implementation steps, and verification gates
  are spelled out for each phase so the work is execution-ready.
- Code is split into partial classes per responsibility (no monoliths).
- All design-time mutations are transactional and route through
  `IComponentChangeService` for undo/redo.

---

## Phase status board

| #  | Phase                                                | Document                                                                                                       | Status           | Risk     |
| -- | ---------------------------------------------------- | -------------------------------------------------------------------------------------------------------------- | ---------------- | -------- |
| 01 | TabbedView vs NativeView clarity & auto-wire         | [Phase-01](./documenthost-designtime-Phase-01-view-mode-clarity.md)                                            | **Superseded by Phase 07** | Low      |
| 02 | Panel siting & design-time tab selection             | [Phase-02](./documenthost-designtime-Phase-02-panel-siting-and-selection.md)                                  | **Partially done** | Med      |
| 03 | Designer partial-class refactor                      | [Phase-03](./documenthost-designtime-Phase-03-designer-partial-refactor.md)                                    | ✅ **Shipped** (monolith 2,468 → 671 lines, 8 partials + 2 helper classes + 2 dialog Forms) | Med      |
| 04 | Undo / redo & transactions for every designer action | [Phase-04](./documenthost-designtime-Phase-04-undo-redo-transactions.md)                                       | ✅ **Shipped** (DefaultPolicy clone-and-assign, MutateDesignTimeDocuments helper, SetProperty transactional wrap, collection-editor CommitEditedCollection, dispose-suppress) | Med      |
| 05 | Onboarding wizards, templates & seeded layouts       | [Phase-05](./documenthost-designtime-Phase-05-onboarding-and-templates.md)                                     | **Partially done** | Low      |
| 06 | Property-grid editors & InitializeComponent codegen  | [Phase-06](./documenthost-designtime-Phase-06-property-grid-editors.md)                                        | **06A + 06B ✅ Shipped** (metadata audit, `[Editor]` wiring on DesignTimeDocuments, DocumentHostLayoutEditor) — **06C codegen ⏳ pending** (separate iteration) | High     |
| 07 | Commercial-grade design-time experience              | [Phase-07](./documenthost-designtime-Phase-07-commercial-grade-experience.md)                                  | **Partially done** ✦ polish: don't-show-again, existing-tabs hint, multi-host chooser, empty-host drop convergence, actionable status banners | Med      |
| 08 | IDisplayContainer integration for MDI host           | [Phase-08](./documenthost-designtime-Phase-08-display-container-integration.md)                                | ✅ **Shipped**     | Low      |

`Partially done` = the most user-visible defects are fixed in this
iteration; remaining hardening items are tracked inside the phase docs.
`Superseded` = the goals of the phase are subsumed by a later phase —
keep the doc for historical context but new work happens in the
successor.

---

## What shipped in this iteration

The user asked first for "revise the whole system to behave like
commercial MDI platforms, user-friendly and clear", then for "finish
pending then implement IDisplayContainer on the MDI". This iteration
delivers:

1. **Phase 07 commercial-grade design-time experience** (foundation +
   the three pending polish items: don't-show-again preference,
   existing-tabs hint, multi-host chooser).
2. **Phase 08 IDisplayContainer integration** — `BeepDocumentManager`
   itself now implements `IDisplayContainer`, so addin code that
   previously had to use `BeepDisplayContainer` can drop the manager
   in and get tabbed, browser-style, *or native MDI* document
   presentation.
3. The foundational fixes from Phases 01 and 02 remain in place.

**New files (commercial-grade UX):**

- `Dialogs/DocumentSetupWizardDialog.cs` — modal setup wizard with three
  visual mode tiles (Tabbed Documents / Browser Tabs / Native MDI),
  starter-content checkboxes, layout-template combo, live preview
  pane, and a `DocumentSetupResult` capture. Self-contained — paints
  icons with GDI+, no external assets required.
- `Designers/BeepDocumentHostDesigner.SetupHost.cs` — `partial` with
  `InitializeNewComponent` (first-drop wizard), `ShowQuickSetupWizard`,
  `DescribeHostState` (plain-English status banner).
- `Designers/BeepDocumentHostDesigner.PanelSiting.cs` — registers every
  `BeepDocumentPanel` with the host's `INestedContainer` so tab-header
  clicks at design time surface the panel in the Properties window.
- `Designers/BeepDocumentManagerDesigner.ViewMode.cs` — `partial` with
  the three apply methods (`ApplyTabbedViewMode`, `ApplyBrowserTabsMode`,
  `ApplyNativeMdiViewMode`), `ShowSetupWizard`, `ApplySetupResult`,
  `SeedSampleDocuments`, `ApplyLayoutTemplate`, and `DescribeViewMode`.

**In-place changes to existing files:**

- `Designers/BeepDocumentHostDesigner.cs`
  - Class is now `partial`.
  - `GetActiveDocumentPanel` switched from broken `panel.Name == activeId`
    lookup to `host.GetPanel(activeId)`.
  - `Initialize` HandleCreated hook + `ExecuteDesignTimeDocumentsAction`
    tail now call `SiteAllDesignPanels()`.
  - `Dispose` calls `UnsiteAllDesignPanels()`.
- `Designers/BeepDocumentHostDesigner.SetupHost.cs`
  - `InitializeNewComponent` honours
    `BeepDocumentManagerDesigner.ShouldAutoOpenWizard()` so the user's
    "Don't show again" preference applies to both component types.
  - `ShowQuickSetupWizard` now feeds the host's existing document
    count to the wizard and persists `DoNotShowAgain` on result.
- `Designers/BeepDocumentManagerDesigner.cs`
  - Class is now `sealed partial`.
  - `InitializeNewComponent` checks `ShouldAutoOpenWizard()` before
    launching the Setup Wizard via `SynchronizationContext.Post`.
  - Verbs reordered: ✨ **Setup Wizard…** is first; mode-switch verbs
    renamed for clarity (`Use Tabbed Documents` / `Use Browser Tabs` /
    `Use Native MDI`); new **Reset Setup Wizard Preference** verb.
  - Smart-tag action list now starts with a **Status** header + live
    plain-English banner, followed by the View group (wizard + property
    + three one-click switches).
  - `SetViewType` renamed `CreateAndAssignView` and made `internal`.
- `Designers/BeepDocumentManagerDesigner.ViewMode.cs`
  - `ShowSetupWizard` now collects the host's existing document count
    and the multi-host candidate list, feeds them to the upgraded
    wizard, and persists `DoNotShowAgain` on result.
  - `ApplySetupResult` honours `result.SelectedHostComponent` via
    `_pinnedHostForSetup` so subsequent `ResolveBeepDocumentHost`
    calls bind to the user's pick.
- `Designers/BeepDocumentManagerDesigner.WizardPrefs.cs` *(new)*
  - HKCU-backed `ShouldAutoOpenWizard` / `SetAutoOpenWizard` helpers
    + `OnResetWizardPreference` verb handler.
- `Dialogs/DocumentSetupWizardDialog.cs`
  - New overload
    `DocumentSetupWizardDialog(initial, existingDocumentCount, hostOptions)`.
  - New result members `SelectedHostComponent` + `DoNotShowAgain`.
  - New `HostPickerOption` model for the multi-host combo.
  - Yellow "Wire to host:" strip rendered when 2+ hosts exist.
  - Sample-tabs checkbox auto-disabled when host already has docs;
    sample-suffix label flips to yellow `"(skipped — N docs already
    present)"`.
  - Footer height bumped to 68 px to host the "Don't show again"
    checkbox under the status label.
- `ActionLists/DocumentHostActionList.cs`
  - Smart-tag now begins with **Status** header + live banner + ✨
    **Setup Wizard…** entry before the existing categories.

**Runtime files (Phase 08):**

- `DocumentHost/BeepDocumentManager.cs`
  - Class declaration: `public sealed class …` → `public sealed partial class …`.
  - `Dispose(bool)` calls `DisposeDisplayContainer()` before
    `DetachView()` so the IDisplayContainer event bridge tears down
    cleanly with a valid `_view`.
- `DocumentHost/BeepDocumentManager.DisplayContainer.cs` *(new)*
  - Full `IDisplayContainer` implementation. Routes
    `AddControl` / `RemoveControl` / `ShowControl` through the active
    `IBeepDocumentManagerView` (tabbed → `BeepDocumentPanel`; native
    MDI → MDI child `Form` via `DocumentFormCreated` hook).
  - Bridges manager events → IDisplayContainer events.
  - Mirrors `BeepDisplayContainer.ShowPopup` semantics so addin code
    that branches on `AddControl` vs `ShowPopup` keeps working.

**Verified:**

- `TheTechIdea.Beep.Winform.Controls.Design.Server` builds with 0
  errors (pre-existing nullable warnings unchanged after Phase 07
  polish + Phase 08).
- `TheTechIdea.Beep.Winform.Controls` builds with 0 errors after
  `BeepDocumentManager` was switched to `sealed partial` and the new
  `BeepDocumentManager.DisplayContainer.cs` partial was added.
- Linter shows 0 errors across the seven files touched in this
  iteration.

---

## Diagnosis

### Bug A — Tabs cannot be selected at design time *(fixed in this iteration)*

`BeepDocumentHost.AddDocument(...)` constructs `BeepDocumentPanel`
instances directly with `new BeepDocumentPanel(id, title)` and adds them
to its internal `_contentArea.Controls`. Those panels:

- have `Site == null`,
- are not registered in `IDesignerHost.Container.Components`,
- therefore cannot be selected through `ISelectionService.SetSelectedComponents`,
- and consequently cannot show their filtered properties in the Properties
  window even though `BeepDocumentPanelDesigner` is correctly registered.

The `BeepDocumentHostDesigner.DesignTabStrip_MouseDown` handler already
identifies the right panel via `host.GetPanel(tabId)` and calls
`SetSelectedComponents([panel])`, but the selection service silently
no-ops because the component is not sited. The fix is to register every
known panel with the host's `INestedContainer` (the same pattern WinForms
uses for `SplitContainer.Panel1/2`).

### Bug B — `GetActiveDocumentPanel` walks the wrong collection *(fixed in this iteration)*

The previous implementation walked `host.Controls` and matched
`panel.Name == activeId`. Panels live under `_contentArea`, not under
`host`, and `panel.Name` is never set; the lookup always returned `null`
and broke toolbox-drop redirection to the active document. Now uses
`host.GetPanel(activeId)`.

### Bug C — TabbedView vs NativeView is confusing *(fixed in this iteration)*

`OnUseTabbedView` / `OnUseNativeMdiView` created the view component and
stopped. The user was left with:

- `BeepTabbedView.Host == null` → nothing rendered, no error.
- `BeepNativeMdiView.ParentForm == null` and `Form.IsMdiContainer == false`
  → MDI children silently dropped on the floor.

The new ViewMode partial:

- auto-wires `BeepTabbedView.Host` when exactly one `BeepDocumentHost`
  exists on the design surface (shows guidance otherwise),
- auto-wires `BeepNativeMdiView.ParentForm` to the root `Form` and
  flips `IsMdiContainer = true` via `IComponentChangeService` for undo,
- adds a "Choose View Mode…" verb + smart-tag entry that opens a chooser
  dialog explaining both modes side by side.

### Issue D — Designer file is a monolith *(addressed in Phase 03)*

`BeepDocumentHostDesigner.cs` is 2,400+ lines mixing initialise, drag/drop,
context menus, document operations, layout helpers, glyphs, and
serialisation. The workspace rule "every file has one class that does one
thing" is violated. Phase 03 splits this into partials.

### Issue E — Some designer actions were not transactional *(fixed in Phase 04)*

Phase 5 of the legacy `documenthost-designtime-commercial-parity-plan.md`
flagged that *method actions (add/split/close/float) are not
transaction-wrapped*. Phase 04 audited every mutation call site under
`Designers/`, `ActionLists/`, and `Editors/` and addressed the five
remaining gaps:

1. `BeepDocumentManagerActionList.AllowFloat / AllowPin / AllowSplit`
   mutated `DefaultPolicy` in place; replaced with a `SetPolicyFlag`
   clone-and-assign helper that writes through
   `PropertyDescriptor.SetValue(mgr, clone)`.
2. `OnAddDocument` / `OnClearDocuments` now route through a new
   `MutateDesignTimeDocuments` helper that raises
   `IComponentChangeService` events around the collection change.
3. `BeepDocumentHostDesigner.SetProperty` now opens a
   `DesignerTransaction` named `"Set {propertyName}"` per call so
   every smart-tag property edit carries a descriptive undo entry.
4. `DocumentDescriptorCollectionEditor` / `DesignTimeDocumentsEditor`
   share a new `CommitEditedCollection` helper that wraps OK in one
   transaction and explicitly notifies the change service via the
   supplied `ITypeDescriptorContext`.
5. `UnsiteAllDesignPanels` honours a new `_isUnsiting` guard so the
   designer's `Dispose` no longer cascades synthetic
   `ComponentRemoved` events into the undo stack.

### Issue F — `InitializeComponent` codegen is incomplete *(Phase 06 split)*

`DesignTimeDocuments` and `DesignTimeLayoutJson` round-trip through string
serialisation. Phase 06 was split into three sub-deliveries:

- **06A** (shipped) — property metadata audit + `[Editor]` wiring on
  `DesignTimeDocuments` so the Properties grid `[…]` button opens
  our custom `DocumentDescriptorEditorForm` directly. Adds
  `[Category("Document")]` to the six visible `BeepDocumentPanel`
  setters that previously lived under "(unsorted)".
- **06B** (shipped) — `DocumentHostLayoutEditor` (`UITypeEditor`
  wrapping the existing `LayoutTreeDialog` from Phase 03) so power
  users can open the layout-tree viewer from the Properties grid in
  addition to the smart-tag verb.
- **06C** (pending) — custom `BeepDocumentHostCodeDomSerializer`
  plus runtime `RegisterDesignTimePanelContent(docId, builder)` API.
  Deferred to its own iteration because a broken `CodeDomSerializer`
  can corrupt user source.

---

## Quick reference: design-time component model

```
BeepDocumentManager (component tray) ─┐
                                       │   View = IBeepDocumentManagerView
                                       ▼
                  ┌──────────────────────────────────┐
                  │ BeepTabbedView   (Component)     │  ───► Host  = BeepDocumentHost
                  │ BeepNativeMdiView (Component)    │  ───► ParentForm = Form
                  └──────────────────────────────────┘

BeepDocumentHost (Control on form)
   ├── _tabStrip                    (locked at design time)
   ├── _contentArea                 (locked at design time)
   │     ├── BeepDocumentPanel A    ◄── sited via INestedContainer
   │     ├── BeepDocumentPanel B    ◄── sited via INestedContainer
   │     └── BeepDocumentPanel ...  ◄── sited via INestedContainer
   └── splitter bar(s)              (locked at design time)
```

After this iteration:

- Selecting a tab header in the designer puts the matching
  `BeepDocumentPanel` in the Properties window with its filtered set of
  document properties (`DocumentId`, `DocumentTitle`, `IconPath`,
  `CanClose`, `DocumentCategory`, `ShowStatusBar`).
- Dropping a toolbox control on a tab header parents the new control
  inside the active `BeepDocumentPanel` (Bug B fix).
- The manager's smart-tag carries a "Choose View Mode…" entry that
  explains the trade-off up front.

---

## Verification matrix (run before any phase claims "done")

| Scenario                                                                                                                       | Phase covered |
| ------------------------------------------------------------------------------------------------------------------------------ | ------------- |
| Drop `BeepDocumentManager` from toolbox → setup wizard opens automatically with three visual mode tiles                         | 07           |
| Pick "Tabbed Documents" + check "Add 3 sample tabs" + Apply → host wired, 3 tabs visible, status banner reflects "3 docs"      | 07           |
| Pick "Browser Tabs" + Apply → host `TabStyle = Chrome`, `ShowAddButton = true`, `CloseMode = Always`                            | 07           |
| Pick "Native MDI" + Apply → root form's `IsMdiContainer = true`, view's `ParentForm` wired                                      | 07           |
| Drop `BeepDocumentHost` directly → quick setup wizard opens; Native MDI tile is gracefully rejected                              | 07           |
| Smart-tag of either component shows a **Status** header at the top with plain-English current configuration                     | 07           |
| Drag `BeepDocumentHost` to form, click "Add Design-Time Document", click a tab → panel selected in Properties window           | 01, 02        |
| With Tabbed View wired, drop a `BeepButton` on the active tab → button is parented inside the active panel                      | 02            |
| One Ctrl+Z reverses a full wizard Apply (mode + sample tabs + template) in a single step                                        | 04, 07        |
| Undo a "Close Active Design-Time Document" action → panel reappears, selection restored                                          | 04            |
| Save form, reopen → `DesignTimeDocuments` collection round-trips, panel children re-emerge                                       | 06C (pending) |
| Click the `[…]` button on `DesignTimeDocuments` in the Properties grid → custom DocumentDescriptorEditorForm opens                | 06A           |
| Select a `BeepDocumentPanel` in the designer → Properties window groups its setters under a **Document** category header          | 06A           |
| Wire a `[Editor(typeof(DocumentHostLayoutEditor), typeof(UITypeEditor))]` test property → clicking `[…]` opens LayoutTreeDialog   | 06B           |
| Open `BeepDocumentHostDesigner.cs` in IDE → main file is **671 lines** (down from 2,468), responsibilities live in 6 new named partials + 2 extracted helper classes + 2 extracted Form Dialogs | 03            |
| Drop two `BeepDocumentHost` controls + a `BeepDocumentManager` → wizard shows yellow "Wire to host:" combo with both host names | 07           |
| Tick "Don't show this wizard automatically next time" in the wizard → drop another `BeepDocumentManager` → wizard does **not** open | 07           |
| Invoke "Reset Setup Wizard Preference" verb → next drop opens the wizard again                                                  | 07           |
| Run wizard against a host that already has 3 docs → "Add N sample tabs" section disabled, suffix shows "(skipped — 3 docs already present)" | 07           |
| `(IDisplayContainer)beepDocumentManager.AddControl("X", addin, …)` in Tabbed mode → addin appears inside a `BeepDocumentPanel` | 08           |
| Same call in Native MDI mode → addin appears inside an MDI child `Form` with proper `MdiParent`                                  | 08           |
| `RemoveControl` fires `AddinRemoved` exactly once even when triggered by the user clicking the tab close button                   | 08           |
| Drop a `BeepButton` on an empty `BeepDocumentHost` → a "Document 1" tab is created **and** the dropped control lands inside it (polished redirect path runs in either case) | 07           |
| Smart-tag of a freshly-dropped manager shows `"Tabbed Documents · 0 docs — drop a control on the host (creates a tab) or click 'Add Document'."` until the first doc is added | 07           |
| Smart-tag of a Native MDI manager shows the IDisplayContainer hint so the user knows how to add child Forms at runtime              | 07           |

---

## Files touched in this iteration

**Code (new):**

- `Dialogs/DocumentSetupWizardDialog.cs` — setup wizard (~570 lines, self-contained)
- `Dialogs/GroupTabPositionDialog.cs` — extracted from designer monolith (Phase 03)
- `Dialogs/LayoutTreeDialog.cs` — extracted from designer monolith (Phase 03)
- `Designers/BeepDocumentHostDesigner.PanelSiting.cs` — INestedContainer siting
- `Designers/BeepDocumentHostDesigner.SetupHost.cs` — first-drop wizard + host status banner
- `Designers/BeepDocumentHostDesigner.Verbs.cs` — Phase 03 partial: ActionLists + DesignerVerbCollection
- `Designers/BeepDocumentHostDesigner.DragDrop.cs` — Phase 03 partial: OnDragXxx overrides + 5-zone dock compass
- `Designers/BeepDocumentHostDesigner.ContextMenu.cs` — Phase 03 partial: right-click menu + tab-strip selection
- `Designers/BeepDocumentHostDesigner.DesignTimeDocuments.cs` — Phase 03 partial: public design-time CRUD + transactional core
- `Designers/BeepDocumentHostDesigner.LayoutPresets.cs` — Phase 03 partial: Layout Assistant + preset → template mapping
- `Designers/DocumentHostDesignerMenuTheming.cs` — Phase 03: extracted MenuRenderer + ColorTable helpers
- `Designers/DesignTimeDocumentsEditorContext.cs` — Phase 03: extracted ITypeDescriptorContext helper
- `Designers/BeepDocumentManagerDesigner.ViewMode.cs` — wizard apply + three modes + status

**Code (new — Phase 06A/06B):**

- `Editors/DocumentHostLayoutEditor.cs` — UITypeEditor wrapping `LayoutTreeDialog` so power users can open the layout-tree viewer from the Properties grid via the `[…]` button in addition to the smart-tag verb.

**Code (in-place — runtime, Phase 06A):**

- `DocumentHost/BeepDocumentHost.Properties.cs` — `[Editor]` wired on `DesignTimeDocuments` pointing at `Editors.DesignTimeDocumentsEditor` in `TheTechIdea.Beep.Winform.Controls.Design.Server`.
- `DocumentHost/BeepDocumentManager.cs` — same `[Editor]` wired on `DesignTimeDocuments`.
- `DocumentHost/BeepDocumentPanel.cs` — added `[Category("Document")]` to the six visible setters (`DocumentTitle`, `IsModified`, `CanClose`, `IconPath`, `DocumentCategory`, `ShowStatusBar`) so they group correctly in the Properties window.

**Code (in-place):**

- `Designers/BeepDocumentHostDesigner.cs` — Phase 03 monolith trimmed from 2,468 → 671 lines; now owns only lifecycle, PreFilter*, SnapLines, OnPaintAdornments empty-state hint, dialog launchers, GetParent/CanParent, and service accessors. Phase 04: `SetProperty` now opens its own `DesignerTransaction("Set …")`; `Dispose` flips `_isUnsiting` around `UnsiteAllDesignPanels` so teardown removals are not pushed into the undo stack.
- `Designers/BeepDocumentHostDesigner.PanelSiting.cs` — Phase 04: new `_isUnsiting` flag; `UnsiteAllDesignPanels` skips the explicit `nested.Remove(panel)` call when set so `IComponentChangeService.ComponentRemoved` events do not cascade during designer dispose.
- `Designers/BeepDocumentManagerDesigner.cs` — `sealed partial`, `InitializeNewComponent`, smart-tag re-layout with Status header, three mode switch verbs. Phase 04: `OnAddDocument` / `OnClearDocuments` route through new `MutateDesignTimeDocuments` helper; new `SetPolicyFlag` clone-and-assign for `AllowFloat / AllowPin / AllowSplit`.
- `Designers/BeepDocumentManagerDesigner.ViewMode.cs` — Phase 04: `SeedSampleDocuments` now appends to `DesignTimeDocuments` through `MutateDesignTimeDocuments` so seeded tabs survive serialization and undo together with the wizard's outer transaction.
- `Editors/DocumentDescriptorCollectionEditor.cs` — Phase 04: new `CommitEditedCollection` helper shared by `DocumentDescriptorCollectionEditor.EditValue` and `DesignTimeDocumentsEditor.EditValue` opens one designer transaction and raises change-service notifications via the supplied `ITypeDescriptorContext`.
- `ActionLists/DocumentHostActionList.cs` — Status header + Setup Wizard entry prepended.

**Plans (new):**

- `.plans/MASTER-TODO-TRACKER.md` *(this file)*
- `.plans/documenthost-designtime-Phase-01-view-mode-clarity.md`
- `.plans/documenthost-designtime-Phase-02-panel-siting-and-selection.md`
- `.plans/documenthost-designtime-Phase-03-designer-partial-refactor.md`
- `.plans/documenthost-designtime-Phase-04-undo-redo-transactions.md`
- `.plans/documenthost-designtime-Phase-05-onboarding-and-templates.md`
- `.plans/documenthost-designtime-Phase-06-property-grid-editors.md`
- `.plans/documenthost-designtime-Phase-07-commercial-grade-experience.md`
- `.plans/documenthost-designtime-Phase-08-display-container-integration.md`

---

## Related, untouched plans

- `Beep.Winform/.plans/DocumentHost-MDI-Phase-01-Foundation.md` … `-05-DesignerAndRelease.md`
  *(higher-level runtime feature plan — left intact)*
- `Beep.Winform/TheTechIdea.Beep.Winform.Controls/plans/documenthost-designtime-commercial-parity-plan.md`
  *(historical commercial-parity plan — Phase 5 there overlaps with phases 02–06 here; this tracker supersedes its design-time items)*
- `Beep.Winform/TheTechIdea.Beep.Winform.Controls/plans/documenthost-gap-matrix.md`
  *(gap matrix — items "Designer undo/redo integrity" and "Designer serialization to InitializeComponent" map to Phase 04 and Phase 06 here)*
