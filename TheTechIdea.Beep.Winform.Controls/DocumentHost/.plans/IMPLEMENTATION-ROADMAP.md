# BeepDocumentHost Implementation Roadmap

*Revised: May 2026 — Post Track-G re-audit. All originally-critical design-time gaps
are resolved. Remaining work is polish, two designer-authoring gaps (G2/G3), and sample coverage.*

## North Star

`BeepDocumentHost` should feel like the Beep WinForms answer to tabbed MDI:
**easy like DevExpress where you control everything in design time**, structured
like mature docking suites, and safe enough for everyday Visual Studio designer use.

## Current Position

Runtime is 91–93% commercial-grade. Design-time is 82%. The remaining gaps are
precision polish and two design-time authoring features (inline smart-tag pickers and
designer drop-onto-document), not structural blockers.

---

## What Is Already Done ✅

### Track G — Design-Time First (COMPLETE)

| Item | Status |
|------|--------|
| G1: `[Category]`/`[Description]`/`[DefaultValue]` on all properties | ✅ Done |
| G4: Global policy properties (AllowFloat/Split/Pin/AutoHide, MaxSplitDepth, CloseButtonShowMode) | ✅ Done |
| G5: `PopulateWindowMenu()` / `AttachWindowMenu()` helpers | ✅ Done |
| G6: Hover-trigger delay on auto-hide strips (`AutoHideHoverDelay` property, 400 ms default) | ✅ Done |
| G7: Remember last auto-hide flyout size (`_ahLastSize` dictionary) | ✅ Done |

### Track D — Persistence & Workspaces (COMPLETE)

| Item | Status |
|------|--------|
| `PersistenceKey` (stable GUID) on `DocumentDescriptor` | ✅ Done |
| `PreviousGroupId` on `DocumentDescriptor` for re-dock hints | ✅ Done |
| `BeepLayoutUndoRedo` — `PushUndoState`/`UndoLayout`/`RedoLayout` fully wired; Ctrl+Z/Y routed | ✅ Done |
| Named workspace save/load/switch via `WorkspaceManager` | ✅ Done |
| `RestoreDocumentFactory` callback model (AvalonDock-style cancel/skip) | ✅ Done |

### Track A — Core Hardening (COMPLETE)

All Track A items are done. Properties window categorized, context menus, icon rendering,
`OpenOrActivate`, `DocumentDockState`, `LayoutRestoreReport` diagnostics all implemented.

---

## Remaining Work — Ordered by Priority

---

### Track G2 — Smart-Tag Inline Property Pickers  ✅ DONE

**Implemented in** `DocumentHostActionList.GetSortedActionItems()` — all five inline
`DesignerActionPropertyItem` entries (TabStyle, TabPosition, CloseMode, ShowAddButton,
KeyboardShortcutsEnabled) plus all quick-action method items are present.

---

### Track G3 — Designer Drop-Onto-Document Area  ✅ DONE

**Implemented in** `BeepDocumentHostDesigner.cs` — `CanParent(Control, Type)` and
`OnDragDrop(DragEventArgs)` overrides are present.

---

### Track G8 — Designer Verb Expansion  ✅ DONE

**Implemented in** `BeepDocumentHostDesigner.cs` — "Export Layout Snapshot…",
"Clear All Documents", and "Customize Keyboard Shortcuts…" verbs are all present.

---

### Drag-Float Ghost Polish  ✅ DONE

Theme-aware ghost color, actual tab width, Escape-to-cancel, and `TabFloatDragStarted`
event are all implemented (`BeepDocumentTabStrip.Mouse.cs` + `Properties.cs`).

---

### Tab Insert-Caret Line During Drag  ✅ DONE

`DrawDragInsertBar()` is called at the end of `DrawContent` in `BeepDocumentTabStrip.cs`
and renders a 2 px accent-coloured vertical line at `_dragInsertIndex`.

---

### Auto-Hide Flyout Header  ✅ DONE

`BuildAhFlyoutHeader()` builds a 28 px header with title Label + Pin button + Close
button inside `ShowAhOverlay` (`BeepDocumentHost.AutoHide.cs`).

---

### Auto-Hide Overlay Auto-Collapse on Focus Loss  ✅ DONE

`AttachAhFocusLossCollapse()` hooks `Leave` events on all overlay children and uses
a 500 ms timer to collapse the flyout when focus leaves (`BeepDocumentHost.AutoHide.cs`).

---

### Additional Keyboard Shortcuts  ✅ DONE

All four shortcuts are wired in `BeepDocumentHost.Events.cs`:
- `Ctrl+Alt+Left/Right` → `MoveActiveDocumentToGroup(next: bool)`
- `Ctrl+Shift+W` → `CloseAllDocumentsToRight()`
- `Ctrl+Shift+M` → `ToggleMaximizeActiveDocument()`
order and calls `MoveDocumentToGroup(activeDocId, targetGroupId)`.

---

### Representative MDI Sample Form  🟡 MEDIUM

**File:** `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Default.Views\MainFrm_MDI.cs`

**Minimum content:**
1. `BeepDocumentHost` filling client area with `AllowFloat = true`, `AllowSplit = true`
2. `MenuStrip` with File / View / Window menus
3. `AttachWindowMenu()` wired to Window menu item
4. `AutoSaveLayout = true`, `SessionFile` set to app-local path
5. Add-document button → adds a `Panel` content with typed title
6. `ActiveDocumentChanged` handler updating a `StatusStrip` label
7. View menu cycles `TabStyle` (Chrome → VSCode → Pill → back)

---

### Track B — Designer Validation  🟡 MEDIUM

Manual scenarios to run in the Visual Studio designer after G2/G3 are done:

1. Add `BeepDocumentHost` from Toolbox → Properties window shows categorized groups, no *Misc*.
2. Smart-tag panel shows inline property pickers → change `TabStyle` → designer updates immediately.
3. Drag `Panel` from Toolbox onto host → lands inside first document area.
4. Right-click host → "Export Layout Snapshot…" appears and exports JSON.
5. Open/close designer (F5/stop) → layout restores from `DesignTimeLayoutJson`.
6. Delete host in designer → no orphaned child controls remain.
7. Undo host add → restores without crash.

---

### Track E — Regression Scenario Matrix  🟢 LOW

| # | Scenario |
|---|---------|
| R1 | Save layout with 3 groups → reopen → groups restore exactly |
| R2 | Float document → close float window → docks back to original group |
| R3 | Restore layout with missing document ID → `LayoutRestoreReport` flags it |
| R4 | Rapid split/merge (10 cycles) → no layout tree corruption |
| R5 | Auto-hide 3 documents → close app → reopen → auto-hide state restored |
| R6 | Rename document title → `PersistenceKey` still matches after restore |
| R7 | 50 documents → scrolled tab strip renders without jank |
| R8 | High-contrast mode → all 9 tab styles readable |
| R9 | Ctrl+Z × 10 after splits → layout rolls back cleanly |
| R10 | Designer reopen after complex split → `DesignTimeLayoutJson` restores correctly |

---

### Track F — Optional Advanced Extensions  🟢 LOW (Post-1.0)

Do not start until G2/G3/G8, drag polish, flyout header, and sample form are complete.

| Item | Notes |
|------|-------|
| Wire `BeepDocumentBreadcrumb` | Surface exists, not integrated |
| Wire `BeepDocumentStatusBar` content | Stub; content API needed |
| Wire `BeepDocumentMiniToolbar` | Stub; content API needed |
| Terminal panel | Requires PTY wrapper — post-1.0 |
| Diff viewer | Requires diff algorithm — post-1.0 |
| Git status | Requires libgit2sharp — post-1.0 |
| Cloud sync | Azure Blob stubs exist; auth flow needed — post-1.0 |

---

## Immediate Next Order

All previously-listed polish items are now done. Remaining work:

1. **Track B** — Manual designer validation walkthrough (in VS designer, not code)
2. **Sample MDI form** — expand `MainFrm_MDI.cs` to a full feature demo
3. **Track E** — Regression scenario matrix (manual testing)
4. **Track F** — Optional advanced extensions (breadcrumb, status bar, terminal — post-1.0)

---

## Non-Goals

- Do not clone DevExpress, Telerik, or Syncfusion APIs verbatim.
- Do not let cloud, diff, terminal, or git features block 1.0 release.
- Do not expose layout-tree internals as the main public programming model.
- Do not add more tab painters before G2/G3 are closed.
- Do not add breakpoint-style animations before flyout header is done.
