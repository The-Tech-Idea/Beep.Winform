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

### Track G2 — Smart-Tag Inline Property Pickers  🟠 HIGH

**Goal:** The smart-tag panel (► badge on the control in the designer) lets users change
the five most-used properties via inline drop-downs — no dialog required.

**File:** `BeepDocumentHostDesigner.cs` — `DocumentHostActionList.GetSortedActionItems()`

**What to add — `DesignerActionPropertyItem` for:**
- `TabStyle` — 9 options, most commonly changed
- `TabPosition` — Top/Bottom/Left/Right
- `CloseMode` — Always/OnHover/ActiveOnly/Never
- `ShowAddButton` — bool toggle
- `KeyboardShortcutsEnabled` — bool toggle

**Quick-action method items to add:**
- "Add Document" → creates one document in the active group
- "Clear All Documents" → calls `ClearDesignTimeDocuments()`
- "Copy Layout Snapshot" → copies `DesignTimeLayoutJson` to clipboard

**Exit criterion:** Opening the smart-tag panel shows the five inline property pickers
and three quick actions. Changing TabStyle updates the designer immediately.

---

### Track G3 — Designer Drop-Onto-Document Area  🟠 HIGH

**Goal:** Drag any `UserControl` or `Panel` from the Toolbox and drop it onto a document
area in the designer. The dropped control becomes the content of the active design-time document.

**File:** `BeepDocumentHostDesigner.cs`

**What to add:**
```csharp
public override bool CanParent(Control parent, Type controlType)
    => typeof(Control).IsAssignableFrom(controlType);

protected override void OnDragDrop(DragEventArgs de)
{
    base.OnDragDrop(de);
    var dropped = de.Data?.GetData(typeof(Control)) as Control
               ?? de.Data?.GetData(typeof(UserControl)) as Control;
    if (dropped == null) return;
    var host = (BeepDocumentHost)Control;
    var target = host.ActivePanel ?? host.GetOrCreateDefaultPanel();
    target.Controls.Add(dropped);
    dropped.Dock = DockStyle.Fill;
    RefreshSmartTag();
}
```

**Exit criterion:** Drag `Panel`, `UserControl`, or custom control from Toolbox → drops
inside the first/active document area. Designer can select, move, and resize it.

---

### Track G8 — Designer Verb Expansion  🟡 MEDIUM

**Goal:** Three additional right-click designer verbs on `BeepDocumentHost`.

**File:** `BeepDocumentHostDesigner.cs` — `Verbs` property

**Verbs to add:**
- "Export Layout Snapshot…" → `SaveFileDialog` → writes `DesignTimeLayoutJson` as .json
- "Clear All Documents" → calls `ClearDesignTimeDocuments()` + `RefreshSmartTag()`
- "Customize Keyboard Shortcuts…" → opens keybinding editor (re-uses `ShowShortcutHelp`)

---

### Drag-Float Ghost Polish  🟡 MEDIUM

**File:** `BeepDocumentTabStrip.Mouse.cs`

**Changes:**
1. **Theme-aware ghost color** — replace hardcoded `Color.FromArgb(48, 54, 70)` with `_currentTheme?.TabActiveBackColor`
2. **Bigger ghost** — use actual tab width (~200 px) instead of fixed 140 px; height 36 px instead of 28
3. **Escape key to cancel** — in `OnKeyDown` when `_dragFloating || _dragging`:
   dispose ghost, reset `_dragFloating`/`_dragging`/`_dragStartTab`, reset cursor, `Invalidate()`
4. **Wire dock overlay** — raise `TabFloatDragStarted` event when `_dragFloating` becomes true
   so the host activates `BeepDocumentDockOverlay`

---

### Tab Insert-Caret Line During Drag  🟡 MEDIUM

**File:** `BeepDocumentTabStrip.Painting.cs` — end of `OnPaint`

Paint a 2 px vertical line at `_dragInsertIndex` while `_dragging` is true:
```csharp
if (_dragging && _dragInsertIndex >= 0 && _dragInsertIndex <= _tabs.Count)
{
    int caretX = _dragInsertIndex < _tabs.Count
        ? _tabs[_dragInsertIndex].TabRect.Left
        : _tabs[_tabs.Count - 1].TabRect.Right;
    using var pen = new Pen(_currentTheme?.PrimaryColor ?? SystemColors.Highlight, S(2));
    g.DrawLine(pen, caretX, ClientRectangle.Top + 4, caretX, ClientRectangle.Bottom - 4);
}
```

---

### Auto-Hide Flyout Header  🟡 MEDIUM

**File:** `BeepDocumentHost.AutoHide.cs` — `ShowAhOverlay`

Add a 28 px header panel inside `_ahOverlay` containing:
- `Label` (title, fills remaining width) — `_currentTheme?.PanelForeColor`
- Pin `Button` (📌) → calls `RestoreAutoHideDocument(documentId)`, `DockStyle.Right`
- Close `Button` (✕) → calls `CloseAhOverlay(animate: true)`, `DockStyle.Right`
- Header background → `_currentTheme?.PanelBackColor`

---

### Auto-Hide Overlay Auto-Collapse on Focus Loss  🟡 MEDIUM

**File:** `BeepDocumentHost.AutoHide.cs`

After `_ahOverlay.BringToFront()`:
```csharp
_ahOverlay.Leave -= OnAhOverlayFocusLoss;
_ahOverlay.Leave += OnAhOverlayFocusLoss;
```

Handler:
```csharp
private void OnAhOverlayFocusLoss(object? sender, EventArgs e)
{
    var t = new System.Windows.Forms.Timer { Interval = 500 };
    t.Tick += (_, _) => { t.Stop(); t.Dispose();
        if (_ahOverlay != null && !_ahOverlay.ContainsFocus)
            CloseAhOverlay(animate: true); };
    t.Start();
}
```

---

### Additional Keyboard Shortcuts  🟡 MEDIUM

**File:** `BeepDocumentHost.Keyboard.cs`

| Shortcut | Action |
|----------|--------|
| `Ctrl+Alt+Left` | Move active tab to previous split group |
| `Ctrl+Alt+Right` | Move active tab to next split group |
| `Ctrl+Shift+W` | Close all tabs to the right of the active tab |
| `Ctrl+Shift+M` | Maximize / restore the active document panel |

`MoveActiveDocumentToAdjacentGroup(int direction)` — finds adjacent group in layout
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

1. **G2** — `DesignerActionPropertyItem` for TabStyle, TabPosition, CloseMode, ShowAddButton, KeyboardShortcutsEnabled
2. **G3** — `CanParent` / `OnDragDrop` override in `BeepDocumentHostDesigner`
3. **Drag ghost polish** — theme-aware color, correct size, Escape cancel, wire overlay
4. **Tab insert-caret** — 2 px vertical line during drag-reorder
5. **Auto-hide flyout header** — title label + Pin + Close in header bar
6. **Auto-hide focus-loss collapse** — 500 ms debounce collapse
7. **Keyboard shortcuts** — Ctrl+Alt+Arrow, Ctrl+Shift+W/M
8. **G8** — Export/Clear/Keybindings designer verbs
9. **Sample MDI form** — expand `MainFrm_MDI.cs` to full demo
10. **Track B** — manual designer validation walkthrough

---

## Non-Goals

- Do not clone DevExpress, Telerik, or Syncfusion APIs verbatim.
- Do not let cloud, diff, terminal, or git features block 1.0 release.
- Do not expose layout-tree internals as the main public programming model.
- Do not add more tab painters before G2/G3 are closed.
- Do not add breakpoint-style animations before flyout header is done.
