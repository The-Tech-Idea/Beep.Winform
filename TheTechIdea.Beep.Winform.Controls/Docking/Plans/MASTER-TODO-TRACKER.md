# Beep Docking — MASTER TODO TRACKER

> Single source of truth for enhancing `TheTechIdea.Beep.Winform.Controls.Docking`. We keep
> the existing control names (`BeepDockingManager`, `DockPanel`, `BeepDockspace`,
> `BeepDockSplitter`), reuse the parts that work, refactor what's salvageable, and delete the
> dead/stubbed code.
>
> **Decisions locked for this program (from the owner):**
> 1. **Keep the existing control name** — the orchestrator stays **`BeepDockingManager`**
>    (no rename to `BeepDocks`). `DockPanel` keeps its name too.
> 2. This is an **enhancement of the current control**, salvaging working classes and
>    deleting the dead native-MDI path + stub duplicates (see `PHASE-00` disposition table).
> 3. **Layout persistence is the WinForms designer** — the layout lives in the **parent
>    Form/UserControl `*.Designer.cs`** as serialized components + properties. No runtime
>    XML/JSON layout files (see `PHASE-06`).

---

## Why enhance-in-place (not a greenfield control)

A deep read of the current `Docking/` tree found **three competing hosting models** and
**two dead/stubbed subsystems** layered on top of each other:

- **Working (WinForms controls):** `DockPanel`, `BeepDockspace`, `BeepDockSplitter` are real
  controls added to the host form and painted/positioned correctly.
- **Dead (native Win32 MDI):** `BeepDockingManager.CreateMdiClient` + `MdiPanelPositioner`
  (creates `STATIC` windows but **never parents content** — literal
  `// TODO: Parent to hwnd via SetParent`), `PanelWindowManager`, `ContentHosting`,
  `EventInterceptor`/`DockingMouseEventHandler`, `PainterIntegration`, `WindowChrome`,
  `Interop/Mdi*`. This path produces no usable UI.
- **Stubbed duplicate layout/splitter logic:** `Layout/SplitterManager` and
  `Runtime/SplitterDragHandler` + `PositioningUtilities` are full of `TODO`/`return null`
  and use a **0–100 split ratio** that conflicts with the model's **0.1–0.9** ratio.

The program standardizes `BeepDockingManager` on the **WinForms-control hosting model** and
removes the dead MDI and stub layers. Full file-by-file disposition is in `PHASE-00`.

---

## How to use this tracker

- Each phase has its **own document** (`PHASE-0X-*.md`) with: scope, **existing-code
  disposition** (Reuse / Refactor / Replace / Delete), target files, TODO checklist,
  implementation steps, and verification criteria.
- Update the **Status** column below as phases move.
- **Clean Code rule:** one class per file, one responsibility per class, partial classes for
  large controls, painters/helpers/layout-managers kept separate.

### Status legend
`⬜ Not started` · `🟦 In progress` · `✅ Done` · `🟥 Blocked` · `⏸ Deferred`

---

## Phase index

| # | Phase | Document | Status | Depends on |
|---|-------|----------|--------|-----------|
| 00 | Audit, Disposition & Architecture | `PHASE-00-Architecture-And-Gap-Analysis.md` | ⬜ | — |
| 01 | Theming & Painter Framework | `PHASE-01-Theming-Painter-Framework.md` | 🟦 | 00 |
| 02 | Drag & Drop Docking Engine | `PHASE-02-Drag-And-Drop-Docking.md` | 🟦 | 00, 01 |
| 03 | Layout Engine & Live Splitters | `PHASE-03-Layout-Engine-Splitters.md` | ✅ | 00 |
| 04 | Auto-Hide, Float & Animations | `PHASE-04-AutoHide-Float-Animations.md` | ✅ | 01, 02, 03 |
| 05 | Tabbed Groups (unified, no Document type) | `PHASE-05-Document-Area-Tabs.md` | ✅ | 01, 02, 03 |
| 06 | Designer.cs Layout Persistence | `PHASE-06-Layout-Persistence.md` | ✅ | 00, 03, 07 |
| 07 | Designer & Smart-Tag ActionList | `PHASE-07-Designer-ActionList.md` | ✅ | 00–05 |
| 08 | Performance, Docs & Testing | `PHASE-08-Performance-Docs-Testing.md` | 🟦 (code+docs done; sample/matrix = QA) | all |

---

## The docking control set (target shape — existing names kept)

- **`BeepDockingManager`** — the existing orchestrator that owns the layout tree, painters,
  layout managers, drag controller, auto-hide strips, and document well. **Kept and
  refactored**; the native-MDI hosting code is removed. Split into partial files
  (`BeepDockingManager.Core/.Lifecycle/.DragDrop/.Theme/.Persistence`).
- **`DockPanel`** — a tool-window/document page (a child control). **Kept**; caption painting
  moves into renderers + layout managers, and it gains a `Kind` (ToolWindow/Document) and
  min-size properties.
- **`BeepDockspace`** — a docked cell hosting a tab stack of `DockPanel`s (**reused**).
- **`BeepDockSplitter`** — the live splitter between cells (**reused**).
- **Documents** live in a **document well** region (Phase 05).
- **Persistence:** because the manager + panels/dockspaces/splitters are designer-sited
  components on the host, the WinForms designer serializes them (and the manager's layout
  property) into the host's `*.Designer.cs` (Phase 06 makes this robust).

---

## Guiding constraints (Beep house rules — apply to every phase)

1. New surfaces inherit **`BaseControl`** where practical.
2. Chrome painted through **distinct** painters using `BeepStyling` +
   `BackgroundPainterFactory`/`BorderPainterFactory`/`ShadowPainterFactory`,
   `StyledImagePainter` for icons, `SvgsUIcons`/`SvgsUI` for icon paths,
   `BeepFontManager` for fonts. **No base painter.**
2b. **Event detection in a LayoutManager/Helper; painters only paint** the resolved state.
3. `ApplyTheme()` override on every surface.
4. Partial-class + helper methodology for large controls/managers.
5. Designer & ActionList features live in
   `TheTechIdea.Beep.Winform.Controls.Design.Server\Docking\`.

---

## Reference material

| Concern | File |
|--------|------|
| Painter architecture | `Styling/PAINTER_ARCHITECTURE.md` |
| Per-style painter plan | `Styling/FORM_STYLE_PAINTER_PLAN.md` |
| Styling entry point | `Styling/BeepStyling.cs` |
| Image/SVG painting | `Styling/ImagePainters/StyledImagePainter.cs` |
| Icon catalog | `IconsManagement/SvgsUIcons.cs` |
| External references | `Standard-Toolkit-master/.../Krypton.Docking`, `dockpanelsuite-master/WinFormsUI` |

---

## Rolled-up TODO summary

### Phase 00 — Audit, Disposition & Architecture
- [ ] Finalize per-file disposition table (Reuse/Refactor/Replace/Delete).
- [ ] Confirm hosting model = WinForms child controls (no MDI) under `BeepDockingManager`.
- [ ] Record decisions (component vs control, document representation, ratio convention).

### Phase 01 — Theming & Painters  🟦 in progress
- [x] Replace `DockingPainterAdapter` GDI primitives with painter-framework renderers
      (adapter is now a theme/metrics provider; caption + splitter painted by renderers).
- [x] Unify the duplicated caption/tab painting in `DockPanel.DrawCaption` and
      `BeepDockspace.DrawHeader` into one renderer (`CaptionRenderer`) + layout manager (`CaptionLayoutManager`).
- [x] Route all fonts through `BeepFontManager` (removed all `new Font(...)` in `Docking/`).
- [x] Keep & extend `DockingThemeColors` + `DockingCaptionPainter`.
- [x] Add `Style : BeepControlStyle` to `BeepDockingManager` + propagate to every surface/context.
- [x] `SplitterRenderer` + `BeepDockSplitter` paint refactor (hover/drag grip).
- [x] `DockingGuideOverlay` glyphs migrated to `SvgsUIcons`.
- [x] `DockingPainterFactory` vends a per-style renderer set (cached).
- [x] `AutoHideStripRenderer` + `AutoHideStripLayoutManager`; `AutoHideStrip` delegates paint + hit-test.
- [ ] `TabStripRenderer` + `TabStripLayoutManager` (document tabs — deferred to Phase 05).

### Phase 02 — Drag & Drop  🟦 in progress
- [x] Real drag controller (`DockDragController`) with session/result/ghost/guides/resolver;
      caption drag → float / dock-to-edge / center-stack, with Esc cancel.
- [x] Reuse `DockingGuideOverlay` (via `DockGuideController`) + translucent `DockDragGhost` preview.
- [x] Dead MDI mouse path (`EventInterceptor`/`DockingMouseEventHandler`) already removed; no `IMessageFilter`.
- [ ] Tab-drag reorder + group-edge split guides (lands with Phase 03 layout engine / Phase 05 tabs).

### Phase 03 — Layout & Splitters  ✅ done (runtime drag-resize to be validated in sample)
- [x] Canonicalize on `DockingLayoutController` + `LayoutCalculator` + `BeepDockSplitter`
      (single dock-site edge engine → `DockLayoutResult`; manager `ApplyLayout()` is the sole path).
- [x] **Delete** `Layout/SplitterManager`, `Runtime/SplitterDragHandler`, `PositioningUtilities` (Phase 00).
- [x] Single 0.1–0.9 ratio convention; implemented `FindSplitterAtPoint`/`GetSplitterBounds`.
- [x] `MinWidth`/`MinHeight` on panel + group; `ClampSplit` in `LayoutCalculator`.
- [x] `BeepDockSplitter` live drag → `DockingLayoutController.DragSplitter(groupId, delta)` → re-`ApplyLayout`.

### Phase 04 — Auto-Hide, Float & Animations  ✅ done (runtime feel to be validated in sample)
- [x] Kept `AutoHideSlidePanel` animation; added **hover-peek** (`PeekPanel`) + poll-based slide-back + pin/unpin (`RestoreAutoHiddenPanel`).
- [x] Themed borderless `FloatWindow` (Phase 01 caption renderer); native move, resize borders, double-click redock, owner-edge snap; auto-hide/float/redock lifecycle routed through the engine.
- [x] Shared animation primitives `Easing`/`AnimationTrack`/`DockAnimator` under `Runtime/Animation/`.

### Phase 05 — Tabbed Groups (unified, no Document type)  ✅ done (runtime feel to be validated)
- [x] **No** `DockItemKind`/`Document` control — grouped panels already tab via the shared caption; the `Fill` group is the document area.
- [x] Overflow **chevron** (active tab kept visible) + dropdown; **middle-click close**; **in-strip tab reorder** with hand-off to the Phase 02 float/dock drag.

### Phase 06 — Designer.cs Persistence  ✅
- [x] `DockLayoutDefinition` graph + `BeepDockingManager.LayoutDefinition` `[Content]` property over a stable backing instance (host `*.Designer.cs` is the store — no XML/JSON).
- [x] `CaptureDefinition`/`FillDefinition`/`MaterializeFromDefinition` + `SaveLayout`/`LoadLayout`; `ManageControl` materializes the deserialized layout once panels register.
- [x] **Phase 07:** child `DockPanel`/`BeepDockspace` components emitted + layout changes tracked into `InitializeComponent()` via `IDesignerHost.CreateComponent` + `IComponentChangeService`.

### Phase 07 — Designer & ActionList  ✅
- [x] `BeepDockingManagerDesigner` (`ComponentDesigner`) + `DockPanelDesigner` + `BeepDockspaceDesigner`; registered via `BeepDockingTypeRoutingProvider`, `[Designer(...)]` attributes on the runtime types.
- [x] Design-time child creation per `AGENTS.md` (`host.CreateComponent` + `DesignerTransaction`) in `BeepDockingDesignerWiring`; all mutations through `IComponentChangeService` → `*.Designer.cs`.
- [x] Smart-tag verbs/items: add/move/stack/reorder panels per edge, validate, attach host form, refresh layout, and **Appearance** (`Style` / `UseThemeColors` / `Theme`).

### Phase 08 — Perf, Docs, Testing  🟦 (code + docs done; sample/matrix = interactive QA)
- [x] Batch update (`BeepDockingUpdate`), cached `DockLayoutResult` + invalidation on every tree mutation, double-buffering on all painted surfaces, cached SVG glyphs, shared `DockAnimator`.
- [x] `Docking/README.md`, refreshed `CODE-DICTIONARY.md`, XML docs on public API, migration note.
- [ ] **QA follow-up:** design-time arranged sample form (needs VS designer).
- [ ] **QA follow-up:** full style/theme/DPI/scale verification matrix.

---

## Change log

| Date | Phase | Note |
|------|-------|------|
| _rev3_ | — | Reverted naming: keep `BeepDockingManager`/`DockPanel` (no `BeepDocks`). |
| _rev2_ | — | Existing-code disposition + designer.cs persistence. |
| _init_ | — | First (generic) draft. |
