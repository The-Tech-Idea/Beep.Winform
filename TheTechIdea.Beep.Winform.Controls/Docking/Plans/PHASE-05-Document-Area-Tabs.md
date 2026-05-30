# PHASE 05 — Tabbed Groups (unified, no separate Document type)

**Goal:** Finish the **tabbed group** experience so *any* `DockGroup` — including the center
`Fill` group that acts as the "document area" — behaves like a commercial editor well:
tab **overflow chevron**, **reorder by drag**, and **middle-click close**. There is **no separate
`Document` control and no `Kind` flag**: a `DockPanel` is a `DockPanel` everywhere; whether it
"is a document" is purely a function of which region its group occupies (`Fill` = center well,
edges = tool windows). All features live on the one `DockPanel` / `DockGroup` / caption pipeline.

**Depends on:** 01, 02, 03 · **Blocks:** —

> **Design decision (supersedes the earlier draft):** we do **not** add `DockItemKind`,
> `BeepDockDocumentWell`, or `DocumentGroup`. Grouped panels already render as tabs in the shared
> caption (`CaptionRenderer` + `CaptionLayoutManager`). The center document area is simply a
> `DockGroup` at `DockPosition.Fill`. One unified `DockPanel` supports edge-dock, float, auto-hide,
> and tabbed grouping.

---

## 5.1 Existing-code disposition (this phase)

| File | Disposition | What changes |
|------|-------------|--------------|
| `Layoutmanagers/CaptionLayoutManager.cs` | **Refactor** | Reserve an overflow **chevron** slot and only lay out the tabs that fit (keeping the active tab visible); expose `HasOverflow`, `OverflowButtonRect`, `OverflowTabs`, `HitTestOverflow`. |
| `Painters/Caption/CaptionRenderer.cs` | **Refactor** | Paint the chevron when `layout.HasOverflow`. |
| `Models/DockPanel.cs` | **Refactor** | Overflow dropdown menu; **middle-click close**; **tab reorder** drag (stays a reorder while the cursor is inside the caption strip, hands off to the float/dock drag when it leaves). |
| `Models/DockGroup.cs` | **Reuse** | `MovePanelToIndex` / `GetPanelIndex` already exist — used for reorder. |

No new files.

---

## 5.2 Behavior

- A group with >1 panel shows a tab per panel in its caption (already true).
- Tabs that don't fit collapse into a **chevron** (`SvgsUIcons` dropdown); clicking it lists the
  overflow tabs and activates the chosen one.
- The **active tab is always kept visible** even when others overflow.
- **Drag a tab** left/right within the caption to reorder it; drag it **out** of the caption to
  float / re-dock (existing Phase 02 drag).
- **Middle-click** a tab closes it (honoring `CanClose`).
- The center `Fill` group is the document area; edge groups are tool windows — same code.

---

## 5.3 Implementation steps

1. `CaptionLayoutManager`: overflow-aware tab layout + chevron rect + `HitTestOverflow`.
2. `CaptionRenderer`: paint the chevron on overflow.
3. `DockPanel`: chevron dropdown (overflow list → activate).
4. `DockPanel`: middle-click close.
5. `DockPanel`: in-strip tab reorder via `DockGroup.MovePanelToIndex`, with hand-off to the
   existing caption drag when the cursor leaves the strip.

---

## 5.4 TODO checklist

- [x] Overflow-aware `CaptionLayoutManager` + chevron geometry/hit-test (`HasOverflow`, `OverflowButtonRect`, `OverflowTabs`, `HitTestOverflow`; active tab kept visible).
- [x] `CaptionRenderer` paints the chevron on overflow (SVG `DropDown` icon + fallback).
- [x] Overflow dropdown menu in `DockPanel` (`ShowOverflowMenu` → `ActivatePanel`).
- [x] Middle-click close (`OnMouseClick` middle button → `ClosePanel`).
- [x] In-strip tab reorder with float/dock hand-off (`ReorderUnderCursor` via `DockGroup.MovePanelToIndex`; leaves the strip → `BeginCaptionDrag`).

## 5.6 Status — COMPLETE

- Both `TheTechIdea.Beep.Winform.Controls` and `TheTechIdea.Beep.Winform.Controls.Design.Server` build with 0 errors.
- No new files; no `Document`/`Kind` type. Changed: `Layoutmanagers/CaptionLayoutManager.cs`, `Painters/Caption/CaptionRenderer.cs`, `Models/DockPanel.cs`.
- **Runtime validation pending:** chevron dropdown, reorder feel, and the reorder→float hand-off threshold need an interactive run at 100–200% DPI.

---

## 5.5 Verification criteria

- A `Fill` group with many panels keeps the active tab visible and lists the rest under the chevron.
- Reordering a tab updates the caption order without floating the panel.
- Dragging a tab out of the strip still floats/re-docks (Phase 02 unaffected).
- Middle-click closes a closable tab.
- Chrome is themed by the same renderers as tool windows (Phase 01) at 100–200% DPI.
