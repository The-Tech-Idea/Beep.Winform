# Phase 07 — Persistence Pack & Workspaces (Wrap & Validate)

> **Owner:** _unassigned_  · **Status:** � Done (A1–A3, B1–B2, C1–C2, D1–D2, E1–E2, F1–F2 done ✅) · **Predecessor:** Phase 02

## Why This Phase Exists

Persistence is the area where Beep already has the deepest implementation:

- ✅ `LayoutSerializer` v1 → v2
- ✅ `BeepLayoutUndoRedo` (`Ctrl+Z` / `Ctrl+Y`)
- ✅ `WorkspaceManager` (named layout sets)
- ✅ MRU, pinned, preview, sticky
- ✅ Document templates (factory restore)
- ✅ Cloud sync extension
- ✅ Dirty / unsaved guard

This phase is therefore a **wrap-and-validate** pass: ensure the new manager
component (Phase 02) properly orchestrates the existing pieces.

## Tasks

### A. Manager bridge

- [x] **A1** Forward `AutoSaveLayout` and `SessionFile` from manager → host.
- [x] **A2** Token-expand `SessionFile` (`%AppData%`, `%LocalAppData%`, `%Profile%`).
- [x] **A3** When manager is disposed and `AutoSaveLayout = true`, flush once
  (calls `_view.SaveLayoutToFile(ExpandSessionPath(_sessionFile))` in `Dispose(bool)`).

### B. Workspace UI

- [x] **B1** Add manager designer verb **"Manage Workspaces…"** opening
  `WorkspaceManagerDialog` (created in `Design.Server/Dialogs/`).
- [x] **B2** Status-strip workspace dropdown when a `StatusStripOwner` is set
  (Phase 05 G). The manager now adds a workspace dropdown backed by the bound
  `BeepDocumentHost` workspace API and refreshes it from host workspace events.

### C. Undo / redo

- [x] **C1** Verified: `SaveLayout()` serialises `_settings` (which includes
  `ThemeName`, `AutoSaveLayout`, etc.) plus the document list, so manager-level
  edits are captured automatically.
- [x] **C2** `PushUndoState()` now has design-time guard: returns immediately when
  `IsDesignTimeHost` is `true`, so VS designer property-grid edits never seed the
  runtime undo stack.

### D. Layout migration

- [x] **D1** Verify v1→v2 migrator still passes after manager wiring.
  Migration chain v0→v1→v2→v3 confirmed correct in `LayoutMigrationService.cs`.
- [x] **D2** Bump schema to v3 if dock-panel rows (Phase 05) are added; ship
  v2→v3 migrator.
  `SaveLayout` now writes `SchemaVersion = 3` so `IsCurrentVersion` returns `true`
  immediately and avoids re-migrating on every restore. v2→v3 migrator stamps
  `nodeExtensions` bags on every tree node (pre-existing in the service).

### E. Cloud-sync hook

- [x] **E1** Cloud-sync extension reads from manager (not host) so swapping
  views (Phase 03) doesn't break sync.
  `BeepDocumentManager` now exposes `WorkspaceSaved`/`WorkspaceDeleted`/`WorkspaceSwitched`
  events (forwarded from current host, re-wired on every host change).
  `BeepCloudSyncManager` has a new `(ICloudSyncProvider, BeepDocumentManager, ...)` constructor
  that subscribes to manager-level events; the legacy `WorkspaceManager` overload remains.
- [x] **E2** Document the extension contract in `Cloud.Readme.md`.
  File created at `DocumentHost/Cloud/Readme.md` covering: quick start, provider contract,
  settings reference, event table, view-switch safety explanation, key layout, conflict
  resolution, and disposal guidance.

### F. Dirty-document guard

- [x] **F1** Verify `DocumentClosing` cancellation works when bubbled via
  manager (Phase 02 B2).
- [x] **F2** Sample form (Phase 08) must demonstrate the guard with a
  `BeepTextBox` that flips dirty on `TextChanged`.

## Acceptance Criteria

- ✅ All existing persistence regression tests still pass.
- ✅ Manager-driven persistence is indistinguishable from host-driven.
- ✅ Workspaces survive view switches (Phase 03).
- ✅ Cloud-sync extension still functions through the manager.

## Out of Scope

- Brand-new persistence features. This phase is **integration & validation only**.
