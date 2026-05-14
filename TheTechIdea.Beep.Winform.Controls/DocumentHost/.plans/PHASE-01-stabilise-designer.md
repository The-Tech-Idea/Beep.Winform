# Phase 01 — Stabilise Designer + Fix Delete-Crash

> **Owner:** _unassigned_  · **Status:** 🟥 Not started  · **Predecessor:** none  · **Blocks:** every other phase

## Why This Phase Exists

The user reported that deleting `beepDocumentHost1` from `MainFrm_MDI` in the Visual
Studio designer immediately throws:

```
An error occurred while processing this command.
The connection to the server has been lost.
```

The "connection-to-server-lost" message means the **out-of-process designer host
crashed** (Visual Studio runs WinForms designers in a separate process). The user
cannot ship a control that crashes the IDE on delete, so this phase **must** ship
before any other work.

## Suspected Root Causes (verified during phase work)

| # | Suspect | Where | Likelihood |
|---|---------|-------|-----------|
| 1 | Static `BeepThemesManager.ThemeChanged` handler is **not unhooked** when `IsDesignTimeHost` branch runs in `BeepDocumentHost.Dispose(bool)`. The static keeps a reference to the disposed control; subsequent broadcasts or finalizer touches throw. | `BeepDocumentHost.cs`, line ≈ 425, `if (IsDesignTimeHost) { … return; }` | **High** |
| 2 | `BeepDocumentDragManager.Unregister(this)` only runs in the runtime branch. Static registry can hold the disposed host. | same file | **High** |
| 3 | `_dockAdorner` glyphs added in `InitializeDockAdorner` are **never removed** in `BeepDocumentHostDesigner.Dispose`. The adorner is owned by the parent designer's `BehaviorService`; leaking it can throw inside VS adorner-paint. | `BeepDocumentHostDesigner.cs` | **High** |
| 4 | The child controls (`_tabStrip`, `_dockOverlay`, `_splitterBar`) are disposed _before_ the designer's `IComponentChangeService.ComponentRemoving` fires; if any of them is currently the active glyph target, Behavior service touches a disposed control. | designer ↔ host race | **Medium** |
| 5 | The Window-menu wired via `AttachWindowMenu` (if used) keeps a `DropDownOpening` lambda capturing the host. | `BeepDocumentHost.Properties.cs` `PopulateWindowMenu` family | **Medium** |
| 6 | `BeepDocumentHost` registers itself with `BeepCommandRegistry` / `WorkspaceManager` (statics) — same leak class as 1/2. | `BeepDocumentHost.cs` ctor & Phase 6 wiring | **Medium** |
| 7 | The component manager partial (planned in Phase 02) is _not yet_ a source — but if `MainFrm_MDI` carries any **resx-serialised** static references (auto-restore), deletion may need to scrub the design-time JSON. | `DesignTimeLayoutJson` hidden prop | **Low** |

## Tasks

### A. Make `BeepDocumentHost.Dispose(bool)` defensive at design time

- [x] **A1** Always unhook **statics first**, _before_ the design-time short-circuit:
  - [x] `BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged` — never subscribed in design-time path so no-op; confirmed safe
  - [x] `BeepDocumentDragManager.Unregister(this)` — already present; verified idempotent
  - [x] Any other static event subscriptions — audited; only `ThemeChanged` and `DragManager`
- [x] **A2** Wrap every cleanup step in `try { … } catch { /* designer-safe */ }`.
- [x] **A3** Set `_isDisposingHost = true` _before_ touching children ✅
- [x] **A4** Stop all timers in design-time Dispose: `_chordTimer`, `_collapseAnimTimer`, `_ahSlideTimer` (via DisposeAutoHide), `_ahFocusTimer`
- [x] **A5** Skip `SaveLayoutToFile()` when `IsDesignTimeHost` ✅ (runtime-only path)
- [x] **A6** Dispose `_floatWindows`, `_panels`, `_groups` snapshots in design-time branch

### B. Make `BeepDocumentHostDesigner.Dispose(bool)` defensive

- [x] **B1** No BehaviorService adorner objects were registered (InitializeDockAdorner only sets AllowDrop = true). No removal needed.
- [x] **B2** Unwire every event hooked in `Initialize`:
  - [x] `host.ControlAdded`, `host.ControlRemoved` — done in both `Dispose` and new `OnComponentRemoving`
  - [x] `host.HandleCreated` lambda → stored as `_handleCreatedHandler` field; unhooked in Dispose and ComponentRemoving
  - [x] `_contextMenuSurfaces` MouseUp handlers — done; each wrapped in try/catch
- [x] **B3** No `IComponentChangeService` / `ISelectionService` subscriptions exist (only call-sites).
- [x] **B4** No open `DesignerTransaction` paths exist at time of Dispose.

### C. Listen for `ComponentRemoving`, not just `Dispose`

- [x] **C1** Subscribe to `IComponentChangeService.ComponentRemoving` in `Initialize`. When the removed component is `_wiredHost`, do the designer-side scrub _before_ the host gets disposed.
- [x] **C2** Unsubscribed in `Dispose`.
- [x] **C3** Idempotent — `Dispose` no-ops on null `_wiredHost` / `_changeSvcSubscribed`.

### D. Make the host idempotent under double-dispose

- [x] **D1** Added `private bool _disposed;` guard at the top of `Dispose(bool)`; returns early if already disposed.
- [x] **D2** Panels, floats, tabstrip, splitters all nulled/cleared as part of the design-time cleanup.

### E. Add regression coverage

- [ ] **E1** Manual repro: open `MainFrm_MDI` in designer, delete
  `beepDocumentHost1`, save, close, reopen — must not crash and must not leave
  orphan references in `.Designer.cs`.
- [ ] **E2** Manual repro: drop new host, set `AutoSaveLayout = true`, save,
  delete host — must not leave a stray `SessionFile` write.
- [ ] **E3** Manual repro: undo (`Ctrl+Z`) the delete — host must come back and
  be functional.
- [ ] **E4** Add a checklist note in `Readme.md` under
  *Designer Safety Smoke Test* with the three repros above.

### F. Diagnostics for next time

- [ ] **F1** When `IsDesignTimeHost` and an exception escapes Dispose, write a
  one-line trace into `%TEMP%\beep-designer-crash.log` (overwrite-style) so the
  next crash is debuggable without attaching a second VS.
- [ ] **F2** Wrap the trace in `#if DEBUG_DESIGNER` to keep release builds quiet.

## Acceptance Criteria

- ✅ Deleting `BeepDocumentHost` from any form in the VS WinForms designer
  closes the control cleanly with **zero "connection to server lost"** dialogs.
- ✅ Undo after delete restores the host in working state.
- ✅ Repeatedly adding and deleting the host 10× in a single designer session
  shows no leaked event handlers (manual GC check optional).
- ✅ `MainFrm_MDI.cs` opens, designs, builds, and runs without modification.

## Out of Scope

- The new `BeepDocumentManager` component (Phase 02).
- Any new features. This phase is **bug-fix-only**.
- Anything that requires changing public API.

## Risks

- VS designer is single-shot — reproducing the crash deterministically may need
  multiple restarts. Mitigation: use F1 logging.
- Some leaks may originate in third-party static state we don't own (e.g.,
  `BeepThemesManager`). Mitigation: confirm leak ownership before patching the
  wrong class.

## Notes

- Use `_wiredHost?.IsHandleCreated == true` before any UI-touching call from the
  designer; on the designer-out-of-process host the handle may be torn down
  asynchronously.
- The same pattern should be applied to **every** Beep control that participates
  in static event registration (BeepTabs, BeepTree, etc.) — but that's tracked
  in those controls' own plans.
