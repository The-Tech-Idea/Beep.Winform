# PHASE 00 — Audit, Disposition & Architecture

**Goal:** Decide, file by file, what of the existing `Docking/` code is **reused, refactored,
replaced, or deleted**, and define the target architecture for the enhanced
**`BeepDockingManager`** (existing name kept). No behavior change ships in this phase — it is
the contract every later phase follows.

**Depends on:** none · **Blocks:** all

---

## 0.1 What actually exists today (verified by reading every file)

There are **three hosting models** glued together and **two stubbed subsystems**:

1. **WinForms-control hosting (WORKS).** `DockPanel : Panel`, `BeepDockspace : Panel`, and
   `BeepDockSplitter : Control` are real controls added to the host form. The manager
   positions them with `ApplyLayoutBounds`/`ApplyDockGroupBounds` and a per-panel
   `BeepDockSplitter` whose `SplitterMoved` event resizes panels live. Caption/tabs are
   custom-painted. **This is the path that renders.**

2. **Native Win32 MDI hosting (DEAD).** `BeepDockingManager.CreateMdiClient()` creates an
   MDICLIENT, and `MdiPanelPositioner` creates child windows via `WM_MDICREATE` of class
   `"STATIC"` — but **content is never parented into them** (literal
   `// TODO: Parent to hwnd via SetParent`). `PanelWindowManager`, `ContentHosting`,
   `EventInterceptor`/`DockingMouseEventHandler`, `PainterIntegration`, and `WindowChrome`
   all serve this path. **It produces no usable UI.**

3. **Dockspace cells (WORKS, parallel to #1).** `BeepDockspace` hosts `DockPanel`s as tab
   pages and paints its own header (`DrawHeader`) — nearly identical to `DockPanel.DrawCaption`.

**Stubbed/duplicated layout + splitter logic:**
- `Layout/DockingLayoutController` + `LayoutCalculator` — real recursive tree layout
  (ratio as **0.1–0.9 float**), but `GetSplitterBounds`/`FindSplitterAtPoint` are `TODO`.
- `Layout/SplitterManager` — **all stubs** (`FindSplitterAtPoint`→null, `GetDragDelta`→0).
- `Runtime/SplitterDragHandler` + `PositioningUtilities` — partial; uses a conflicting
  **0–100 ratio** and a naive `ApplySplitRatioChange`. Created by the manager but only ever
  driven by the dead MDI mouse path.
- `BeepDockSplitter` — **the real, working splitter** (live drag via `SplitterMoved`).

**Genuinely good, reuse as-is:**
- `DockingThemeColors` — robust `IBeepTheme`→palette resolver with WCAG contrast safeguards.
- `DockingCaptionPainter` — already paints icons via `StyledImagePainter` + `SvgsUIcons`
  with vector fallbacks.
- `DockGroup`, `DockLayoutTree` — clean model (ratio 0.1–0.9, tab reorder, recursion).
- `DockingEnums`, `DockingEventArgs`, `BeepDockingStrings`, `BeepDockingUpdate`.
- `AutoHideStrip` + `AutoHideSlidePanel` — **already animate** (100 ms slide timer).
- `FloatWindow`, `DockingGuideOverlay`, `DockingDropTarget`.

---

## 0.2 Per-file disposition table

> Legend: **Reuse** (keep ~as-is) · **Refactor** (keep, change internals/calls) ·
> **Replace** (rewrite the implementation, name kept) · **Delete** (remove).

### Top level
| File | Disposition | Action |
|------|-------------|--------|
| `BeepDockingManager.cs` | **Refactor** | Stays the orchestrator (name kept). Drop all MDI code (`CreateMdiClient`, `_mdiClientHwnd`, positioner/window-manager/content-hosting/event-interceptor wiring). Hosting = WinForms child controls. Replace ad-hoc positioning with the Phase-03 engine. Split into partial files: `BeepDockingManager.Core/.Lifecycle/.DragDrop/.Theme/.Persistence`. |
| `BeepDockspace.cs` | **Refactor** | Keep as the docked tab-cell host. Move its `DrawHeader` into the shared caption/tab renderer (Phase 01). Remove `new Font("Segoe UI")`. |
| `BeepDockingUpdate.cs` | **Reuse** | Batch-update scope. |
| `BeepDockingStrings.cs` | **Reuse** | Localized strings. |
| `Agents_Instructions.txt` | **Reuse** | Author guidance; keep. |

### Models
| File | Disposition | Action |
|------|-------------|--------|
| `Models/DockPanel.cs` | **Refactor** | Name kept. Move caption painting/hit-test out into `CaptionLayoutManager` + renderer (Phase 01). Add `Kind` (ToolWindow/Document, Phase 05), `MinWidth/MinHeight` (Phase 03). Keep designer attributes for serialization (Phase 06). |
| `Models/DockGroup.cs` | **Reuse** | Ratio stays **0.1–0.9**. |
| `Models/DockLayoutTree.cs` | **Reuse** | — |
| `Models/DockingEnums.cs` | **Reuse** (extend) | Add `DockItemKind` (Phase 05). |
| `Models/DockingEventArgs.cs` | **Reuse** | — |
| `Models/PanelSerializationInfo.cs` | **Refactor** | Repurpose as the design-time snapshot DTO used by the designer serializer (Phase 06), or retire if designer serialization covers it. |

### Painters / Theming
| File | Disposition | Action |
|------|-------------|--------|
| `Docking/DockingThemeColors.cs` | **Reuse** | Keep as palette resolver. |
| `Helpers/DockingCaptionPainter.cs` | **Reuse/Refactor** | Already correct (SVG via `StyledImagePainter`/`SvgsUIcons`). Becomes the icon utility the new renderers call. |
| `Painters/IDockingPainter.cs` | **Refactor** | Keep the strategy concept; introduce a `DockingPainterContext` and split into element renderers (caption/tab/splitter/guide/strip). |
| `Painters/DockingPainterAdapter.cs` | **Replace** | Rewrite: delegate to `BeepStyling` + factories instead of raw `SolidBrush`/`Pen`/`DrawString`. |
| `Painters/DockingPainterFactory.cs` | **Refactor** | Select renderer set by `BeepControlStyle`/theme (currently only caches by theme name → Null). |
| `Painters/NullDockingPainter.cs` | **Reuse** | Safe design-time/no-op painter. |

### Layout
| File | Disposition | Action |
|------|-------------|--------|
| `Layout/DockingLayoutController.cs` | **Refactor** | Canonical layout engine. Implement `GetSplitterBounds`/`FindSplitterAtPoint` (currently TODO). Becomes cache+invalidate wrapper around `LayoutCalculator`. |
| `Layout/LayoutCalculator.cs` | **Reuse** | Pure math helper (0.1–0.9 ratio). |
| `Layout/LayoutValidator.cs` | **Reuse/Refactor** | Keep validation; confirm it matches the canonical model. |
| `Layout/SplitterManager.cs` | **Delete** | All-stub duplicate of splitter drag. |

### Runtime
| File | Disposition | Action |
|------|-------------|--------|
| `Runtime/BeepDockSplitter.cs` | **Reuse** | The real working splitter. Theme via renderer (Phase 01). |
| `Runtime/AutoHideStrip.cs` | **Refactor** | Keep; theme via `AutoHideStripRenderer`; add hover-peek wiring (Phase 04). |
| `Runtime/AutoHideSlidePanel.cs` | **Reuse** | Already animates; add hover/peek hooks (Phase 04). |
| `Runtime/FloatWindow.cs` | **Refactor** | Keep; theme caption + drag-to-redock (Phase 02/04). |
| `Runtime/DockingGuideOverlay.cs` | **Refactor** | Keep 5-diamond overlay; render via guide renderer + add edge guides (Phase 02). Replace `Wingdings 3`/`SystemFonts` glyphs with `SvgsUIcons`. |
| `Runtime/DockingDropTarget.cs` | **Reuse** | Hit-test helper; extend for edge guides (Phase 02). |
| `Runtime/TabInteractionHandler.cs` | **Refactor** | Keep tab state/reorder/`MoveTab`; detach from `PanelWindowManager`; drive the manager directly. |
| `Runtime/SplitterDragHandler.cs` | **Delete** | Stub + 0–100 ratio conflict; superseded by `BeepDockSplitter` + `DockingLayoutController.DragSplitter`. |
| `Runtime/PositioningUtilities.cs` | **Delete** | Only used by the deleted handler; 0–100 ratio. (Salvage any clamp helpers into `LayoutCalculator` if needed.) |
| `Runtime/MdiPanelPositioner.cs` | **Delete** | Dead MDI; never parents content. |
| `Runtime/PanelWindowManager.cs` | **Delete** | Dead MDI lifecycle. |
| `Runtime/ContentHosting.cs` | **Delete** | Dead MDI `SetParent` hosting. |
| `Runtime/EventInterceptor.cs` (+ `DockingMouseEventHandler`) | **Delete** | Dead MDI message filter/mouse routing. |
| `Runtime/PainterIntegration.cs` | **Delete** | Dead MDI paint bridge. |
| `Runtime/WindowChrome.cs` | **Delete** | Dead MDI chrome (superseded by control `OnPaint` + renderers). |
| `Runtime/Phase4RuntimeExample.cs` | **Delete** | Example/scratch. |

### Interop
| File | Disposition | Action |
|------|-------------|--------|
| `Interop/MdiNativeApi.cs` | **Delete** | Only the dead MDI path used it. |
| `Interop/MdiConstants.cs` | **Delete** | Same. |
| `Interop/WindowBatchUpdater.cs` | **Evaluate→Delete** | If only the MDI path used it, delete; otherwise keep for `SetWindowPos` batching of child controls (unlikely needed). |

> **Net effect:** delete the entire native-MDI subsystem (9 files) and 3 stub files; keep
> the WinForms-control surfaces, the model, theming, caption icon painter, layout math, and
> the auto-hide/float/guide runtime — all under the existing `BeepDockingManager`.

---

## 0.3 Target architecture (existing names kept)

```
Docking/
├── BeepDockingManager.Core.cs        (orchestrator: registries, layout tree, painters, managers)
├── BeepDockingManager.Lifecycle.cs   (Add/Remove/Close/Reopen/Float/AutoHide/Dock + cancelable events)
├── BeepDockingManager.DragDrop.cs    (wires drag controller — Phase 02)
├── BeepDockingManager.Theme.cs       (ApplyTheme, Style, palette push — Phase 01)
├── BeepDockingManager.Persistence.cs (designer.cs hooks — Phase 06)
├── BeepDockspace.cs                  (docked tab cell — reused)
├── BeepDockDocumentWell.cs           (center document region — Phase 05)
├── Models/                           (DockPanel, DockGroup, DockLayoutTree, enums, snapshot DTO)
├── Layout/                           (DockingLayoutController + LayoutCalculator + LayoutValidator)
├── Painters/
│   ├── DockingPainterContext.cs      (style/theme/state bundle — Phase 01)
│   ├── Caption/ Tab/ Splitter/ Guide/ Strip/   (distinct renderers — Phase 01)
│   ├── DockingPainterFactory.cs      (style → renderer set)
│   └── NullDockingPainter.cs
├── Layoutmanagers/                   (Caption/TabStrip/Splitter/AutoHide layout managers — Phase 01)
├── Runtime/
│   ├── DragDrop/                     (controller, ghost, guides, resolver — Phase 02)
│   ├── BeepDockSplitter.cs  AutoHideStrip.cs  AutoHideSlidePanel.cs  FloatWindow.cs
│   ├── DockingGuideOverlay.cs  DockingDropTarget.cs  TabInteractionHandler.cs
│   └── Animation/                    (optional shared animator — Phase 04)
└── Plans/                            (this folder)
```

**Layering (no upward calls):**
`BeepDockingManager → LayoutManagers → Painters → (BeepStyling / StyledImagePainter / SvgsUIcons / BeepFontManager)`
and `BeepDockingManager → Layout (engine) → Models (tree)`.

**Hosting model (locked):** WinForms child controls only. `BeepDockingManager` hosts
`BeepDockspace`/`BeepDockDocumentWell`/`BeepDockSplitter`; those host `DockPanel`s.
No native MDI windows.

---

## 0.4 Gap analysis vs commercial products

| Area | Commercial baseline | Today | Action | Phase |
|------|---------------------|-------|--------|-------|
| Skinning | Painters honor full style | `DockingPainterAdapter` flat GDI | painter framework | 01 |
| Caption/tab paint | one renderer | duplicated in `DockPanel`+`BeepDockspace` | unify | 01 |
| Fonts | theme font manager | `new Font("Segoe UI")` in 3+ files | `BeepFontManager` | 01 |
| Drag to dock | caption/tab drag + guides + preview | none (button float only) | drag controller | 02 |
| Splitters | live + constraints + persist | works (`BeepDockSplitter`) but 2 stub dupes | delete stubs, finalize | 03 |
| Nested layout | recursive proportional | engine exists, hit-test TODO | finish engine | 03 |
| Auto-hide | animated + peek + pin | animates, no peek/pin wiring | add peek/pin | 04 |
| Float | themed + snap + redock | basic tool-window | theme + redock | 04 |
| Documents | document well + tab groups | none | document well | 05 |
| Persistence | layout save/restore | partial DTO only | **designer.cs** | 06 |
| Designer | smart tags + design-time children | minimal | full | 07 |

---

## 0.5 TODO checklist

- [ ] Approve the disposition table (§0.2) — especially the **deletes**.
- [ ] Confirm hosting model = WinForms child controls under `BeepDockingManager` (no MDI).
- [ ] Confirm split-ratio convention = **0.1–0.9 float** (delete the 0–100 paths).
- [ ] Write `CODE-DICTIONARY.md` (1-line responsibility + allowed deps per surviving class).
- [ ] Produce a target component diagram next to this doc.

---

## 0.6 Verification criteria

- Every existing file appears in §0.2 with a disposition.
- After the deletes, the project still compiles (the deleted MDI path is not referenced by
  any surviving WinForms-control code — verify references before removal).
- A reviewer can predict, for any feature, which `BeepDockingManager.*` partial or renderer to edit.

---

## 0.7 Decision record (fill during phase)

- **Name:** keep **`BeepDockingManager`** (no rename) — _confirmed by owner_.
- **`BeepDockingManager` form factor:** stays a sited orchestrator **component**
  (`ToolboxItem(true)`); panels remain `DockPanel` controls on the host — _confirm; optionally
  promote to `BaseControl` later if a single dock-site control is preferred_.
- **Hosting model:** WinForms child controls; delete native MDI — _confirm_.
- **Split ratio:** single 0.1–0.9 float — _confirm_.
- **Document model:** `Kind` flag on `DockPanel` vs separate `BeepDockDocument` — _decide in 05_.
- **Persistence:** designer.cs only (no runtime XML/JSON) — _confirmed by owner_.
