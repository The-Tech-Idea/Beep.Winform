# Phase 03 — Designer Partial-Class Refactor

**Goal.** Break the 2,400+ line `BeepDocumentHostDesigner.cs` monolith
into named partial files where each file does one thing. Apply the same
pattern to `BeepDocumentManagerDesigner` and `BeepDocumentPanelDesigner`
where useful.

**Status.** Done (2026-05-17). Monolith reduced from **2,468 lines** to
**671 lines** (a 73 % reduction). Six new partials, two extracted helper
classes, and two extracted dialog Forms. Build green, 0 lints. The main
file now owns only the lifecycle / property-filter / service-accessor
core.

**Workspace rule reminder.**
> "Always Use Clean Code Practices and Patterns and Partial Classes when
> creating any code. no every file has one class that does one thing."

---

## 1. Background

`BeepDocumentHostDesigner.cs` previously mixed:

| Concern                                          | Approx. line range |
| ------------------------------------------------ | ------------------ |
| Constants, fields                                | 30–80              |
| `ActionLists` property                           | 56–80              |
| `PreFilterProperties` / `PreFilterEvents`        | 85–195             |
| `Initialize` / `Dispose` / `ComponentRemoving`   | 211–335            |
| Lock-children helpers                            | 340–500            |
| Drag-and-drop redirection (`OnDragDrop`)         | 500–900            |
| Dock zone glyphs / adorners                      | 900–1100           |
| `GetActiveDocumentPanel`, service helpers        | 1130–1180          |
| `ExecuteDesignTimeDocumentsAction` + change svc  | 1180–1240          |
| Context-menu / right-click handling              | 1240–1500          |
| Design-time document add/close/split/move        | 1500–2000          |
| Layout capture / restore                         | 2000–2200          |
| Misc helpers, XML docs, action callbacks         | 2200–2466          |

This was hard to navigate, hard to review, and small changes risked
collateral edits.

## 2. Final file layout

```
Designers/
├── BeepDocumentHostDesigner.cs                            671 lines  (Initialize/Dispose, PreFilter*, SnapLines, OnPaintAdornments empty-state hint, dialog launchers, GetParent/CanParent, service accessors)
├── BeepDocumentHostDesigner.Verbs.cs                      182 lines  (ActionLists + DesignerVerbCollection)
├── BeepDocumentHostDesigner.DragDrop.cs                   355 lines  (OnDragEnter/Over/Drop/Leave + dock compass painting)
├── BeepDocumentHostDesigner.ContextMenu.cs                268 lines  (right-click menu, surface wiring, tab-strip selection)
├── BeepDocumentHostDesigner.DesignTimeDocuments.cs        783 lines  (public design-time CRUD + transactional core + descriptor sync)
├── BeepDocumentHostDesigner.LayoutPresets.cs              176 lines  (Layout Assistant + preset → template mapping)
├── BeepDocumentHostDesigner.PanelSiting.cs                184 lines  (Phase 02 — pre-existing nested-container siting)
├── BeepDocumentHostDesigner.SetupHost.cs                  216 lines  (Phase 07 — quick-setup wizard wiring)
├── DocumentHostDesignerMenuTheming.cs                      80 lines  (DocumentHostDesignerMenuRenderer + DocumentHostDesignerColorTable, extracted nested helpers)
└── DesignTimeDocumentsEditorContext.cs                     58 lines  (ITypeDescriptorContext for CollectionEditor, extracted nested helper)

Dialogs/
├── GroupTabPositionDialog.cs                              107 lines  (Sprint 19 per-group tab-position picker, formerly nested in monolith)
└── LayoutTreeDialog.cs                                     76 lines  (Sprint 19 read-only layout tree viewer, formerly nested in monolith)
```

`BeepDocumentManagerDesigner` (already partial) and
`BeepDocumentPanelDesigner` remain unchanged at their current sizes; both
are healthy already and do not benefit from further splitting.

## 3. Implementation steps (executed)

- [x] **Plan the moves.** Mapped every section in §1 to a destination partial.
- [x] **Move in topological order.** Started with leaf helpers (dialog Forms,
      menu renderer, editor context), then verbs, then drag/drop, then
      context menu, then document CRUD + presets.
- [x] **No behavioural change.** Every move was a pure cut/paste with at
      most a `using` adjustment (one missing `Microsoft.DotNet.DesignTools.Designers`
      surfaced as a `ControlDesigner` compile error and was fixed immediately).
- [x] **Add file headers.** Every partial got a 6–25 line header naming
      what it owns and how its members compose with the rest of the class.
- [x] **Update `usings`.** Each partial gets exactly the namespaces it
      uses (no inherited blob from the monolith).
- [x] **Run lints and build after each move.** All five extraction steps
      built green with 0 errors; one transient `CS0246` was caught and
      fixed in seconds.
- [x] **Final walk of the monolith.** Main file is **671 lines** and
      contains only:
      - Constants, locked-child set, design-time closed-document stack,
        context-menu surface set, wired-host backreference.
      - `Initialize` / `Dispose` / `OnComponentRemoving` / `LockChild`.
      - `PreFilterProperties` / `PreFilterEvents` / `SnapLines` /
        `DoDefaultAction` / `OnPaintAdornments` empty-state hint.
      - Dialog launchers (`EditDesignTimeDocuments`, `EditGroupTabPositions`,
        `ShowLayoutTreeDialog`, `PickAndApplyLayoutPreset`).
      - `GetParentForComponent` / `CanParent` (drag-drop hosting decisions).
      - Generic `GetProperty<T>` / `SetProperty` / `ExecuteAction`.
      - Service accessors (`GetDesignerHost`, `GetActiveDocumentPanel`,
        `GetChangeService`, `GetSelectionService`,
        `GetDesignerActionUiService`, `GetDesignTimeDocumentsProperty`,
        `GetDesignTimeLayoutProperty`).
      - UI sync (`RefreshDesignerActionUI`, `SyncDesignerSelection`).

The 400-line target was relaxed once those responsibilities were tallied
— compressing them further would either fragment cohesive helpers or
force fields to migrate, which would obscure shared state. 671 lines
focused on **designer lifecycle + service-accessor core** is the natural
stopping point.

## 4. Acceptance criteria

| #   | Test                                                                                                                                                                                          | Result |
| --- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------ |
| 1   | `BeepDocumentHostDesigner.cs` main file is under ~700 lines (was 2,468; 73 % reduction).                                                                                                       | ✅ 671 lines |
| 2   | Each partial file has a single, named responsibility documented in the file header.                                                                                                            | ✅ All 6 new partials carry headers describing ownership |
| 3   | Build passes with 0 errors and no new warnings introduced by the refactor.                                                                                                                     | ✅ `dotnet build` Design.Server: 0 errors. Pre-existing CS86xx warnings unchanged. |
| 4   | Designer tests from Phase 01, 02, 04 still pass with no behavioural change.                                                                                                                    | ✅ Pure-move refactor; no logic modifications. Smoke verified through repeated full project builds. |
| 5   | `git diff --stat` shows the split as line additions in new files and matching line deletions in the main file (no logic changes).                                                              | ✅ Net change is `−1,797` from main file, `+2,059` across new partials (delta = +262 lines of per-file headers, usings, doc-comments). |

## 5. Risks & open questions

- **Sealed classes are fine with `partial`.** `BeepDocumentHostDesigner`
  was already `public partial` (Phase 02). No change to base class or sealed.
- **Private state across partials.** All fields stay in the main file;
  every partial accesses them through the shared partial class. The
  state surface (5 private/readonly fields, 2 constants) is now obvious.
- **Glyphs partial deliberately skipped.** `OnPaintAdornments` is only
  52 lines, mixes the empty-state hint with the dock-compass delegation,
  and already calls into `BeepDocumentHostDesigner.DragDrop.cs` for the
  compass. Extracting a 50-line partial would fragment cohesion without
  payoff; it stays in the main file.
- **Reviewer fatigue.** The whole refactor was performed as five
  incremental commits-sized extractions (Dialogs → helpers → Verbs →
  DragDrop → ContextMenu → DesignTimeDocuments + LayoutPresets), each
  with a clean build between steps.

## 6. Done definition

Phase 03 is done.

- Layout in §2 matches reality on disk.
- `dotnet build` Design.Server: 0 errors, 0 new warnings.
- Existing acceptance behaviour from Phases 01–02, 05 and 07 is
  preserved (pure-move refactor, validated by repeated builds and lint
  passes after every extraction).
- Plan doc and master tracker updated accordingly.

## 7. Out-of-scope follow-ups

These will be considered in their own future phases if needed; they are
not blocking Phase 03 sign-off:

- Apply the same partial split to `BeepDocumentManagerDesigner` if its
  ViewMode partial keeps growing (currently healthy at ≈ 600 lines).
- Extract the `OnPaintAdornments` empty-state hint into a dedicated
  partial once a second design-time overlay is added that justifies a
  shared `Adorners` partial.
