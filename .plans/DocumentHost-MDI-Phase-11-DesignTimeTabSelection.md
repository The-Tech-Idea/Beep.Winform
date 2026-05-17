# DocumentHost MDI — Phase 11: Design-Time Tab Selection

## Why this phase exists

Phases 01–10 hardened the runtime story, integrated the setup wizard, and
brought `BeepDocumentManager` to `IDisplayContainer` parity, but a long-
standing design-time gap remained:

> *"when creating tabbed documents I cannot select any document on design
> time?"* — original Phase 02 feedback, reiterated by the user as
> *"still I cannot select a tab document? what's the point if I can't do
> it at design time?"*.

Concretely, clicking a tab header on the design surface did not:

1. Switch the property grid to show the selected document panel's
   properties (Title, Icon, CanClose, …).
2. Re-route subsequent toolbox drops onto the selected document body.
3. Visually indicate which panel was active by drawing the standard
   designer selection rectangle around it.

The root cause is the runtime tab strip raising `TabSelected` →
`SetActiveDocument` → `ActiveDocumentChanged` without anything in the
designer ever listening to that event. Selection promotion never
happened, so the user perceived "tabs can't be selected".

## Scope

- File-by-file, surgical changes inside
  `TheTechIdea.Beep.Winform.Controls.Design.Server\Designers`.
- No runtime behavior changes: the wiring is gated by `Component is
  BeepDocumentHost` and executes only at design time.
- All new entry points are public on the designer / action list so the
  wizard, smart tag, and verbs can reach them.

## Changes shipped

### File: `BeepDocumentHostDesigner.cs`

- Added two new private fields:
  - `EventHandler<DocumentEventArgs>? _activeDocumentChangedHandler`
  - `bool _syncingActiveDocumentSelection`
- `Initialize(IComponent)` now subscribes
  `host.ActiveDocumentChanged += OnHostActiveDocumentChanged` after the
  existing `ComponentRemoving` and `HandleCreated` wiring.
- `Dispose(bool)` unhooks the handler symmetrically alongside the other
  handler cleanup blocks.
- New `OnHostActiveDocumentChanged` resolves
  `BeepDocumentHost.ActivePanel`, guards re-entrancy with
  `_syncingActiveDocumentSelection`, and forwards into
  `SyncDesignerSelection(panel)` so the WinForms Properties window
  swaps to the document panel.
- New `SyncInitialActiveDocumentSelection` performs a one-shot sync
  right after `SiteAllDesignPanels()`, but only when the current
  primary selection is still the host (or a descendant). It refuses to
  steal selection from a control the user picked themselves.
- Added a `IsDescendantOfHost(Control, BeepDocumentHost)` static helper
  used by the one-shot sync.
- `HandleCreated` callback now calls `SyncInitialActiveDocumentSelection`
  immediately after `SiteAllDesignPanels` so a freshly-loaded form lands
  on its active document rather than the host shell.

### File (new): `BeepDocumentHostDesigner.TabSelection.cs`

A dedicated partial that owns hit-test pass-through and fallback verbs:

- `protected override bool GetHitTest(Point point)` — overrides
  `ParentControlDesigner.GetHitTest(Point)`. Receives mouse position in
  screen coordinates and returns `true` (i.e. "let the runtime control
  receive this click") whenever the point falls on a tab body, add
  button, scroll button, overflow button, or group-header strip. This
  is what allows `BeepDocumentTabStrip.OnMouseDown` to run at design
  time so the strip raises `TabSelected`. Outside the strip the call
  defers to `base.GetHitTest(point)` so dragging the host itself still
  selects/moves it.
- `internal bool ActivateTabAt(Point screenPoint)` — defensive fallback
  used by the new smart-tag verb. Hit-tests the cursor against every
  group's `TabStrip.Tabs[i].TabRect`, calls
  `host.SetActiveDocument(tabId)` on the first hit, and lets the
  resulting `ActiveDocumentChanged` event drive selection promotion.
- `internal void SelectDocumentAt(Point screenPoint)` — convenience
  helper. If the cursor is on a tab, calls `ActivateTabAt`; otherwise
  falls back to selecting the currently active panel.
- `TryHitTestTabStrip(host, screenPoint, out tabId, out group)` —
  shared hit-test pipeline. Iterates `host.Groups`, projects screen
  coords into each strip's client space, and short-circuits the tab-id
  resolution if any strip rectangle contains the point.

### File: `BeepDocumentHostDesigner.Verbs.cs`

- Added a new context-menu verb **"Select Tab Under Cursor"** that
  invokes `SelectDocumentAt(Cursor.Position)`. Provides a keyboard
  / right-click escape hatch for environments where the WinForms host
  intercepts the click before our `GetHitTest` returns.

### File: `ActionLists\DocumentHostActionList.cs`

- Added `public void SelectTabUnderCursor()` that delegates to the
  designer's `SelectDocumentAt(Cursor.Position)`.
- Registered a new `DesignerActionMethodItem` for the same method,
  filed under the existing "Documents" category alongside
  "Select Active Document Surface".

## Designer behavior matrix

| User gesture                                    | Pre-Phase 11 outcome                                              | Phase 11 outcome                                                                                                |
| ----------------------------------------------- | ----------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------- |
| Click tab "Tab 2" while "Tab 1" is selected     | Visual highlight may change; property grid stays on host; toolbox drops still go to old active panel | `GetHitTest` claims pass-through → strip raises `TabSelected` → host raises `ActiveDocumentChanged` → designer selection swaps to panel 2; property grid + toolbox drop target follow |
| Form load with multiple design-time docs        | Property grid shows host; user must drill in manually              | `SiteAllDesignPanels` followed by `SyncInitialActiveDocumentSelection` lands the property grid on the active document panel |
| Right-click host → "Select Tab Under Cursor"    | Verb did not exist                                                 | New verb is wired; activates whichever tab is hovered                                                            |
| Smart tag → Documents → "Select Tab Under Cursor" | Item did not exist                                                | Smart-tag item invokes the same designer entry point                                                             |
| Toolbox drop onto a tab                          | Already routed to `host.ActivePanel` via DragDrop partial          | Unchanged — the toolbox routing now follows whichever tab the user clicked, since clicks now switch the active panel |
| Drag/move host itself                            | Worked                                                             | Unchanged — `GetHitTest` only claims pass-through for the tab-strip area                                         |

## TODO checklist

- [x] `DOCMDI-P11-001` Add private fields + handler delegate in `BeepDocumentHostDesigner.cs`.
- [x] `DOCMDI-P11-002` Subscribe `ActiveDocumentChanged` in `Initialize`.
- [x] `DOCMDI-P11-003` Unsubscribe `ActiveDocumentChanged` in `Dispose`.
- [x] `DOCMDI-P11-004` Implement `OnHostActiveDocumentChanged` with re-entrancy guard.
- [x] `DOCMDI-P11-005` Implement `SyncInitialActiveDocumentSelection` + `IsDescendantOfHost`.
- [x] `DOCMDI-P11-006` Call `SyncInitialActiveDocumentSelection` from `HandleCreated` after `SiteAllDesignPanels`.
- [x] `DOCMDI-P11-007` Create `BeepDocumentHostDesigner.TabSelection.cs` partial.
- [x] `DOCMDI-P11-008` Implement `GetHitTest(Point)` override claiming pass-through for tab strip hits.
- [x] `DOCMDI-P11-009` Implement `TryHitTestTabStrip` shared pipeline.
- [x] `DOCMDI-P11-010` Implement `ActivateTabAt` + `SelectDocumentAt` defensive fallbacks.
- [x] `DOCMDI-P11-011` Add "Select Tab Under Cursor" verb in `BeepDocumentHostDesigner.Verbs.cs`.
- [x] `DOCMDI-P11-012` Surface the same action in `DocumentHostActionList.cs` smart tag.
- [x] `DOCMDI-P11-013` Build Controls — 0 errors, 0 warnings.
- [x] `DOCMDI-P11-014` Build Design.Server — 0 errors (all reported warnings are pre-existing).
- [x] `DOCMDI-P11-015` Document phase in `.plans/DocumentHost-MDI-Phase-11-DesignTimeTabSelection.md`.
- [x] `DOCMDI-P11-016` Update `.plans/MASTER-TODO-TRACKER.md` with Phase 11 ledger entries.
- [x] `DOCMDI-P11-017` Append "Design-Time Tab Selection" section to `DocumentHost/README.md`.
- [ ] `DOCMDI-P11-018` Manual VS designer regression: open a form with 3 documents, verify tab-click selects panel, property grid swaps, toolbox drops land on the clicked panel. (validation pending live VS run)
- [ ] `DOCMDI-P11-019` Manual VS designer regression: confirm Undo/Redo still rolls back tab activation when triggered through the wizard (existing transactions cover this).
- [ ] `DOCMDI-P11-020` Capture screenshots of selection rectangle + property grid + dropped child for the marketing/docs site.

## Verification criteria

1. After dropping a single `BeepDocumentManager` and running the setup
   wizard with three documents, clicking any tab header in the designer
   immediately shows that document panel's properties (`DocumentTitle`,
   `IconPath`, `CanClose`, `DocumentCategory`, `ShowStatusBar`) and
   draws the selection rectangle around the panel.
2. Dragging a Beep / WinForms control from the toolbox and dropping it
   on the active panel produces a child of that `BeepDocumentPanel`
   (verified via the existing `OnDragDrop` redirection — once selection
   follows the click, the panel is the natural drop target).
3. Selecting any non-host control (e.g. a button on a panel) and
   reloading the form preserves the user's last selection — the one-shot
   initial sync no longer fires because the primary selection is no
   longer the host nor a descendant we should override.
4. Right-clicking the host shows the new "Select Tab Under Cursor" verb,
   and the smart-tag actions panel lists it under the "Documents"
   category.
5. `dotnet build` of both `TheTechIdea.Beep.Winform.Controls` and
   `TheTechIdea.Beep.Winform.Controls.Design.Server` reports 0 errors
   and no new warnings.

## Design decisions

- **Why hook `ActiveDocumentChanged` and not `TabSelected`?**
  `ActiveDocumentChanged` is fired by the host after the strip event
  has been digested, so it captures both pointer-driven activation and
  programmatic `host.SetActiveDocument(id)` calls (used by verbs, the
  wizard, layout presets, etc.). One subscription covers every code
  path that switches the active document.

- **Why a re-entrancy guard?**
  Calling `ISelectionService.SetSelectedComponents` can trigger
  designer-side notifications that some downstream listeners react to
  by toggling the active document. The flag prevents an infinite ping-
  pong if a future listener does so.

- **Why a one-shot initial sync and not a continuous sync?**
  Stealing the user's selection every time the form reloads is
  intrusive. We only "land" on the active document when the property
  grid is still on the host shell (i.e. the developer has not selected
  anything specific yet).

- **Why also implement the `GetHitTest` override?**
  In the Microsoft DesignTools host, a non-sited child control like
  `BeepDocumentTabStrip` does not receive its `WM_LBUTTONDOWN` directly
  when the BehaviorService captures the mouse. The override tells the
  designer "this point belongs to the runtime control", which lets the
  strip's existing `OnMouseDown` pipeline run normally. This is the
  exact pattern DevExpress uses inside `XtraTabbedView`'s designer.

- **Why provide a fallback verb (`SelectDocumentAt`)?**
  Some hosting environments (legacy WinForms designer in older VS
  versions, third-party design surfaces) ignore `GetHitTest` for non-
  sited children. The verb gives users a deterministic escape hatch in
  any environment.

## Files touched

```text
TheTechIdea.Beep.Winform.Controls.Design.Server/
├── ActionLists/
│   └── DocumentHostActionList.cs                            [edited]
└── Designers/
    ├── BeepDocumentHostDesigner.cs                          [edited]
    ├── BeepDocumentHostDesigner.TabSelection.cs             [new]
    └── BeepDocumentHostDesigner.Verbs.cs                    [edited]

TheTechIdea.Beep.Winform.Controls/
└── DocumentHost/
    └── README.md                                            [edited]

.plans/
├── DocumentHost-MDI-Phase-11-DesignTimeTabSelection.md      [new]
└── MASTER-TODO-TRACKER.md                                   [edited]
```

## Related reading

- `.plans/DocumentHost-MDI-Phase-08-DesignTimeWizardPolish.md`
- `.plans/DocumentHost-MDI-Phase-10-DisplayContainerParity.md`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/README.md`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/.plans/README.md`
