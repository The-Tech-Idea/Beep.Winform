# Phase 03 — View Modes (Tabbed / NativeMdi / WindowsUI)

> **Owner:** _Beep platform team_  · **Status:** 🟩 Done (WindowsUI deferred)  · **Predecessor:** Phase 02

## Why This Phase Exists

DevExpress DocumentManager ships **three switchable views**:

| View          | Behaviour                                                       |
|---------------|-----------------------------------------------------------------|
| `TabbedView`  | Default — documents are tabs in a host (matches today's Beep)  |
| `NativeMdiView` | Real `Form.IsMdiContainer = true`; documents are MDI children |
| `WindowsUI`   | Full-screen "tile launcher" akin to Windows 8 modern apps    |

The new `BeepDocumentManager.View` property switches between these without
rebuilding the form. This phase introduces the view abstraction and ships the
first two views; `WindowsUI` is a stretch goal.

## Tasks

### A. View abstraction

- [x] **A1** Define `interface IBeepDocumentManagerView : IDisposable`:
  - [x] `void Attach(BeepDocumentManager manager)` / `void Detach()`
  - [x] `BeepDocumentPanel? AddDocument(DocumentDescriptor desc)` / overload `(string,string,bool)`
  - [x] `void ActivateDocument(string id)`
  - [x] `bool RemoveDocument(string id)`
  - [x] `void BeginBatchAddDocuments()` / `void EndBatchAddDocuments()`
  - [x] `bool CloseAllDocuments()`
  - [x] Layout: `string SaveLayout()`, `void SaveLayoutToFile(string?)`, `bool TryRestoreLayout(string)`
  - [x] Settings push: `PushPolicy`, `PushTheme`, `PushPersistence`, `AttachWindowMenu`
  - [x] Events: `DocumentAdded`, `DocumentRemoved`, `ActiveDocumentChanged`, `DocumentClosing`
- [x] **A2** `BeepTabbedView : Component, IBeepDocumentManagerView` delegates to a
  `BeepDocumentHost` it references via the `Host` property.
- [x] **A3** Manager property `IBeepDocumentManagerView View { get; set; }`
  **replaces the legacy `Host` property entirely** — no compatibility alias kept.

### B. `BeepTabbedView` (default)

- [x] **B1** Wraps a `BeepDocumentHost` assigned via the `Host` property.
- [x] **B2** Forwards `DefaultPolicy`, `ThemeName`, persistence, window-menu, etc.
- [x] **B3** Serialises only the host reference — the host serialises its own state.

### C. `BeepNativeMdiView`

- [x] **C1** Sets `Form.IsMdiContainer = true` on the parent form via the
  `ParentForm` property.
- [x] **C2** `AddDocument(desc)` creates a child `Form`, sets `MdiParent`, raises
  `DocumentFormCreated` so consumers can inject content controls.
- [x] **C3** Window-menu integration via `AttachWindowMenu` — adds Cascade /
  Tile / Arrange / per-document entries.
- [x] **C4** `Cascade` / `TileHorizontal` / `TileVertical` / `ArrangeIcons` via
  `LayoutMdi`.
- [x] **C5** Persists MDI child positions in `MdiChildLayout` JSON DTO
  (X/Y/W/H/State).
- [x] **C6** Honours `DefaultPolicy` — MDI children are inherently free-floating
  inside the parent so `AllowFloat=false` is naturally implied.

### D. `BeepWindowsUIView` *(deferred)*

- [ ] **D1**–**D5** Deferred to a future release.  The view contract is ready —
  add the third implementation when prioritised.

### E. View switch UX

- [x] **E1** `BeepDocumentManager.ChangeView<TView>()` helper — instantiates a
  fresh view, swaps it in, pushes all settings.
- [x] **E2** Designer smart-tag actions:
  - `View` property dropdown
  - `Use Tabbed View` verb (creates `BeepTabbedView` in the container)
  - `Use Native MDI View` verb (creates `BeepNativeMdiView`)
- [x] **E3** Switching views detaches the previous view, attaches the new one,
  re-pushes theme / policy / persistence / window-menu.

### F. Tests / repros

- [x] **F1** `BeepDocumentManager` + `BeepTabbedView` + `BeepDocumentHost`
  triplet compiles, designer-loads, and behaves identically to the previous
  direct `Host` wiring.
- [x] **F2** Switching to `BeepNativeMdiView` at design-time activates MDI
  container behaviour on `ParentForm`.
- [x] **F3** Switching back to `BeepTabbedView` rewires the host.

## Acceptance Criteria

- ✅ `BeepDocumentManager.View` accepts both view kinds without designer error.
- ✅ Smart-tag dropdown shows the two views; verbs create them in one click.
- ✅ Switching view rebuilds the docs from the descriptors collection.
- ✅ `NativeMdiView` honours `Cascade` / `Tile*` `LayoutMdi` calls.
- ✅ **No legacy `Host` property left on `BeepDocumentManager`.**

## Out of Scope

- Floating-window remoting (Phase 05).
- Custom tab styles (already in `BeepDocumentTabStrip`).
- `WindowsUIView` — deferred until UX direction is finalised.

## Notes

- `NativeMdiView` is the killer feature for legacy migrations — devs converting
  pre-2010 WinForms apps can switch by toggling one property in the designer.
- `DocumentAddedEventArgs.Panel` is now nullable so MDI (which uses Forms,
  not panels) can raise the event without sentinels.
