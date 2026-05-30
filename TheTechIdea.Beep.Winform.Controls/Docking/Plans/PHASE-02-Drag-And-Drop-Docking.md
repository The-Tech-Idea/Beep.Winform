# PHASE 02 — Drag & Drop Docking Engine

**Goal:** Let users grab a caption or tab and **float / dock / stack / reorder** panels with
live docking guides and a translucent preview — the headline commercial feature. Build on the
existing overlay/float/hit-test classes; **delete** the dead MDI mouse-routing path.

**Depends on:** 00, 01 · **Blocks:** 04

---

## 2.1 Existing-code disposition (this phase)

| File | Disposition | What changes |
|------|-------------|--------------|
| `Runtime/DockingGuideOverlay.cs` | **Refactor** | Keep the 5-diamond overlay form; paint via `GuideRenderer` (Phase 01); add **edge guides** (dock to dock-site edges) + center group guides; replace `Wingdings 3`/`SystemFonts` arrows with `SvgsUIcons`. |
| `Runtime/DockingDropTarget.cs` | **Reuse/Extend** | Keep point→`DockPosition`; extend to return a full **drop result** (target group, edge vs center, stack index). |
| `Runtime/FloatWindow.cs` | **Refactor** | Becomes the float host produced when a drag tears a panel out; themed caption + drag-to-redock (Phase 04 polish). |
| `Runtime/TabInteractionHandler.cs` | **Refactor** | Reuse tab reorder/`MoveTab`; detach from `PanelWindowManager`; call into the new controller. |
| `Runtime/EventInterceptor.cs` / `DockingMouseEventHandler` | **Delete** | Dead MDI `IMessageFilter` mouse routing. Replace with direct control mouse events on caption/tab via the layout managers. |
| `BeepDockingManager` float/drag bits | **Refactor → `BeepDockingManager.DragDrop.cs`** | Move into the controller. |

---

## 2.2 New components

```
Docking/Runtime/DragDrop/
├── DockDragController.cs   // owns a drag session; start/update/commit/cancel
├── DockDragSession.cs      // source panel/group, origin, hot target, modifier state
├── DockDragGhost.cs        // translucent layered preview window following cursor
├── DockTargetResolver.cs   // (cursor, dock-site) -> DockDropResult using guides + DockingDropTarget
├── DockDropResult.cs       // target group, DockPosition, edge|center|stack, insert index
└── DockGuideController.cs   // shows/hides DockingGuideOverlay + edge guides; highlights active diamond
```

`BeepDockingManager.DragDrop.cs` wires caption/tab mouse events (from `CaptionLayoutManager` /
`TabStripLayoutManager`) into `DockDragController`.

---

## 2.3 Interaction model

1. **Mouse-down** on caption/tab (layout manager reports the hit) → record candidate.
2. **Drag threshold** (`SystemInformation.DragSize`) exceeded → `DockDragController.Start`:
   tear panel into a `DockDragGhost` (translucent snapshot), show guides.
3. **Move** → `DockTargetResolver` maps cursor to a `DockDropResult`; `DockGuideController`
   highlights the active diamond/edge; ghost shows the would-be bounds.
4. **Mouse-up** → commit:
   - **edge of dock-site** → new docked group at that `DockPosition`;
   - **edge of a group** → split that group (Phase 03 engine);
   - **center of a group** → stack as a tab (insert index from resolver);
   - **no target** → leave floating in a `FloatWindow`.
5. **Esc / mouse-up off-target** → cancel, restore original.

Modifiers: hold a key (e.g. `Ctrl`) to suppress docking (force float) — configurable.

---

## 2.4 Implementation steps

1. `DockDragSession` + `DockDropResult` data types.
2. `DockDragGhost` (layered translucent window; reuse `StyledImagePainter` for content snapshot or a tinted rect).
3. `DockGuideController` wrapping the refactored `DockingGuideOverlay` (+ edge guides).
4. `DockTargetResolver` using `DockingDropTarget` + group bounds from the layout engine (Phase 03).
5. `DockDragController` orchestrating start/update/commit/cancel; raise cancelable
   `PanelDocking`/`PanelDocked`/`PanelFloating` events on `BeepDockingManager`.
6. Wire mouse events from caption/tab layout managers into the controller (`BeepDockingManager.DragDrop.cs`).
7. **Delete** `EventInterceptor`/`DockingMouseEventHandler`; remove `Application.AddMessageFilter` usage.
8. Commit paths call `BeepDockingManager` dock/stack/float APIs (which call the layout engine).

---

## 2.5 TODO checklist

- [x] `DockDragSession`, `DockDropResult`, `DockDragGhost`, `DockGuideController`,
      `DockTargetResolver`, `DockDragController` (+ `IDockDragHost` commit surface).
- [x] `DockingGuideOverlay` already paints SVG arrows (Phase 01). *(center diamonds done; dedicated edge guides + group-edge guides deferred to Phase 03 layout engine).*
- [~] `DockTargetResolver` produces a full `DockDropResult` (kind/position/group/index/preview);
      `DockingDropTarget` retained as the guide point→position helper.
- [x] Wire caption drag via `CaptionLayoutManager` (no message filter); tab-drag reorder deferred to Phase 05.
- [x] `EventInterceptor` / `DockingMouseEventHandler` already removed in Phase 00 (no `IMessageFilter`).
- [x] Cancelable docking events reused (`PageDockedRequest` / `PageFloatingRequest`) on commit.

---

## 2.6 Verification criteria

- Drag a caption → float; drag onto a guide → docks at the indicated edge/center.
- Drag a tab within a group → reorder; onto center of another group → stacks at drop index.
- Guides + ghost render themed (Phase 01) at runtime; no `Wingdings`/`SystemFonts`.
- No `IMessageFilter` remains; all drag input via control mouse events.
- Esc cancels and fully restores the original layout.
