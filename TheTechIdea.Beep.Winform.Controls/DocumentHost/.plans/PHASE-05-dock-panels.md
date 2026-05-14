# Phase 05 — Dock Panels (Tool Windows) + DockManager-Style Edges

> **Owner:** _unassigned_  · **Status:** ✅ Done · **Predecessor:** Phase 02

## Why This Phase Exists

DevExpress ships **DockManager** as a sibling component to DocumentManager. The
pair gives developers VS-style IDE layouts: documents in the centre, tool windows
on the edges, all floatable / dockable / auto-hideable.

Beep already has the **auto-hide flyout** primitive and the **drag overlay**
compass. This phase formalises **named dock panels** (tool windows) sitting on
the host's edges, with their own pinning, floating, and persistence — without
making them part of the document collection.

## Tasks

### A. Component & data

- [x] **A1** New non-visual component `BeepDockManager : Component` that pairs
  with `BeepDocumentManager` (or stands alone for forms with no documents).
- [x] **A2** Property `BeepDocumentHost? Host` (target).
- [x] **A3** `BindingList<DockPanelDescriptor> Panels { get; }`.
- [x] **A4** `DockPanelDescriptor` fields:
  - [x] `Id` (GUID)
  - [x] `Title`, `IconPath`
  - [x] `Edge` (Left / Right / Top / Bottom / Floating)
  - [x] `SizePercent` (0.0–1.0)
  - [x] `IsAutoHidden` (bool)
  - [x] `IsVisible` (bool)
  - [x] `Content` (Control reference — `DesignerSerializationVisibility.Hidden`)
  - [x] `PersistenceKey`

### B. Rendering

- [x] **B1** Add edge rails to `BeepDocumentHost` if `BeepDockManager` is bound:
  - [x] Left rail
  - [x] Right rail
  - [x] Bottom rail
  - [x] Top rail
- [x] **B2** Theme-consistent rendering via `ApplyTheme()` on each rail.
- [x] **B3** Layout integration (`ShrinkForDockRails` + `PositionDockRails` in `RecalculateLayout`).

### C. Auto-hide / pin / float

- [x] **C1** Auto-hide flyout overlay per rail.
- [x] **C2** Pin button promotes panel from auto-hidden → docked.
- [x] **C3** Float button creates a sizable tool window.

### D. Cross-host transfer

- [x] **D1** Static `BeepDockManager` registry for all live instances.
- [x] **D2** `AttachCrossHostDrag` hooks float window to detect drop onto another host.
- [x] **D3** `TransferPanel` moves descriptor to target manager.

### E. Designer

- [x] **E1** `BeepDockManagerDesigner` — verb "Edit Dock Panels…" (collection editor).
- [x] **E2** Smart-tag: Add Left/Right/Top/Bottom panel methods, Host property.
- [x] **E3** Verb "Auto-pair with DocumentHost on this form".

### F. Persistence

- [x] **F1** `LayoutSnapshotV2` extended with `DockPanels` array; schema bumped to v4.
- [x] **F2** `CollectDockPanelDtos` / `RestoreDockPanels` helpers in Serialisation partial.
- [x] **F3** `LayoutMigrationService` v3→v4 migration injects empty `dockPanels` array.

### G. Mini-toolbar / status bar / breadcrumb (opt-in)

- [x] **G1** `BeepDocumentManager.StatusStripOwner` populates a status strip with:
  - [x] Active document title (Spring label, left-aligned)
  - [x] Dirty indicator (● via `BeepDocumentPanel.ModifiedChanged`)
  - [x] Cursor position (automatic for `TextBoxBase` / `RichTextBox`; custom
    content can implement `IDocumentStatusInfoProvider`)
- [x] **G2** Document title shown in the left Spring label; group breadcrumb
  deferred (group name not yet exposed through manager API).
- [x] **G3** Labels created on `AttachStatusStrip`, removed + disposed on
  `DetachStatusStrip`; setter calls detach/attach so swapping strips is clean.

## Acceptance Criteria

- ✅ Drop a `BeepDockManager` on a form, pair with the existing
  `BeepDocumentHost`, define 3 panels (Solution / Properties / Output) →
  designer shows the rails immediately.
- ✅ Toggle auto-hide on Properties at design time → it collapses to the right rail.
- ✅ Drag the Output panel from bottom rail onto the centre dock compass →
  becomes a floating window.
- ✅ Layout JSON persists / restores panel state including auto-hide and edge.

## Out of Scope

- A separate `BeepDockSite` competitor to DockPanel Suite — we reuse host edges.
- Standalone dock-only forms — they can drop a hidden host + dock manager.

## Notes

- Beep already implements most of the **runtime** plumbing for auto-hide; this
  phase mostly adds the **declarative / design-time** layer.
