# Beep Docking — `BeepDockingManager`

A commercial-grade docking system for WinForms, built on the Beep control framework
(`BeepStyling`, `StyledImagePainter`, `SvgsUIcons`, `BeepFontManager`). It provides
dockable tool windows, a document well, drag-and-drop docking with visual guides,
live splitters, auto-hide strips, themed floating windows, tabbed groups, and
**layout persistence into the host form's `*.Designer.cs`**.

---

## Hosting model

`BeepDockingManager` is a **non-visual component** (`IComponent`) that is sited on a
`Form` and manages a tree of `DockPanel` controls parented **directly on the host form**.
There is **one** hosting model — WinForms child controls. (The legacy native-MDI path was
removed in Phase 00; see _Migration_ below.)

```
Form (HostForm)
 └─ BeepDockingManager (component)
     ├─ DockLayoutTree                ← logical layout (edge groups + Fill well)
     │   └─ DockGroup (Left/Right/Top/Bottom/Fill)
     │       └─ DockPanel[]           ← tabbed within a group
     ├─ BeepDockSplitter[]            ← live edge splitters (created from the layout)
     ├─ AutoHideStrip[]               ← per-edge auto-hide tabs
     └─ FloatWindow[]                 ← borderless themed floating tool-windows
```

The **single source of truth** for geometry is `DockingLayoutController`. It computes an
immutable `DockLayoutResult` (panel bounds, group bounds, splitter rects) which
`BeepDockingManager.ApplyLayout()` applies. Renderers only paint; layout managers only
measure and hit-test.

---

## Quick start (runtime)

```csharp
var docking = new BeepDockingManager();
docking.ManageControl(this);          // 'this' is the host Form

docking.AddPanel("solution", "Solution Explorer", DockPosition.Left,  solutionTree);
docking.AddPanel("props",    "Properties",         DockPosition.Right, propertyGrid);
docking.AddPanel("output",   "Output",             DockPosition.Bottom, outputBox);
docking.AddPanel("editor",   "Editor",             DockPosition.Fill,  editorControl);

docking.AutoHidePanel("output");      // collapse to a strip; hover to peek, click to pin
docking.FloatPanel("props");          // pop out into a themed float window
```

Batch several operations into a single layout pass + repaint:

```csharp
using (new BeepDockingUpdate(docking))
{
    docking.ShowPanel("output");
    docking.FloatPanel("props");
    docking.MovePanel("solution", DockPosition.Right);
}   // one RecalculateLayout() fires here
```

---

## Design-time experience

Drop `BeepDockingManager` on a form. Its smart tag / context-menu verbs (provided by the
designer in `TheTechIdea.Beep.Winform.Controls.Design.Server`) let you:

- **Add Panel** at Left / Right / Top / Bottom / Fill.
- **Move / Stack / Reorder** the selected panel.
- **Appearance** — set `Style` (`BeepControlStyle`), toggle `UseThemeColors`, set `Theme`.
- **Validate Panels**, **Refresh Layout**, **Attach Host Form**.

Every mutation is routed through `IComponentChangeService`, and child panels are created
via `IDesignerHost.CreateComponent`, so the **arrangement is written into the host
`*.Designer.cs`** (per `AGENTS.md`). Rebuild → reopen the form → the layout is restored.

### Persistence

Layout state round-trips through the `BeepDockingManager.LayoutDefinition` property
(`[DesignerSerializationVisibility(Content)]`, backed by a stable instance with get-only
collections), so the designer emits `.Add(...)` calls into `InitializeComponent()`.
No external XML/JSON files are used for the default layout.

- `CaptureDefinition()` — snapshot the live tree into a `DockLayoutDefinition`.
- `LoadLayout()` / `SaveLayout()` — runtime convenience.
- On load, `ManageControl` materializes a deserialized `LayoutDefinition` once panels are
  registered.

---

## Public API surface

**Lifecycle:** `ManageControl(Form)`, `Dispose()`.

**Panels:** `AddPanel`, `RemovePanel`, `GetPanel`, `ContainsPanel`, `MovePanel`,
`StackPanel`, `MovePanelInStack`, `ActivatePanel`.

**Visibility / state:** `ShowPanel`, `HidePanel`, `ClosePanel`, `ReopenPanel`,
`IsPanelClosed`, `FloatPanel`, `DockFloatingPanel`, `AutoHidePanel`,
`RestoreAutoHiddenPanel`.

**Bulk ops:** `ShowPanels`, `HidePanels`, `ShowAllPanels`, `HideAllPanels`,
`StorePanel`/`RestoreStoredPanel`, `StoreAllPanels`/`RestoreAllStoredPanels`.

**Batch / layout:** `BeginUpdate`/`EndUpdate` (prefer `BeepDockingUpdate`),
`RecalculateLayout`, `ApplyLayout`, `LayoutController`, `GetPanelBounds`,
`GetPanelContentBounds`, `GetDiagnostics`.

**Theming:** `Theme`, `UseThemeColors`, `Style`, `CurrentTheme`, `ApplyTheme(...)`,
`RegisterThemeHook`.

**Persistence:** `LayoutDefinition`, `CaptureDefinition`, `SaveLayout`, `LoadLayout`.

**Events:** `PanelActivated`, `PanelAdded`, `PanelRemoved`, `PanelFloated`,
`PanelAutoHidden`, `PanelHidden`, `PanelShown`, `PanelClosed`, `PanelReopened`,
`ThemeChanged`, plus cancelable request events `PageCloseRequest`, `PageDockedRequest`,
`PageAutoHiddenRequest`, `PageFloatingRequest`, and infrastructure events for
floating-window / auto-hidden-group / dockspace lifecycle and separator resize.

---

## Performance notes

- **Batch updates:** `BeginUpdate`/`EndUpdate` (depth-counted) suspend layout; a single
  `RecalculateLayout()` runs when the outermost scope exits.
- **Layout caching:** `DockingLayoutController` caches its `DockLayoutResult` and only
  recomputes when `ContainerBounds` changes or `InvalidateLayout()` is called. The manager
  invalidates the cache on every tree mutation (add/remove/move/stack/float/auto-hide/…).
- **Double buffering:** every custom-painted surface (`DockPanel`, `BeepDockspace`,
  `BeepDockSplitter`, `AutoHideStrip`, `AutoHideSlidePanel`, `DockingGuideOverlay`,
  `FloatWindow`, `DockDragGhost`) sets `OptimizedDoubleBuffer | AllPaintingInWmPaint`.
- **Cached glyphs:** icons render through `DockingCaptionPainter` → `StyledImagePainter`
  (framework SVG cache) and `SvgsUIcons`; no per-paint SVG reload.
- **Animations:** a single shared `DockAnimator` timer drives all auto-hide / float tweens.

---

## Migration

- The **native-MDI hosting path was removed**. `BeepDockingManager` hosts panels purely as
  WinForms child controls; there is no MDI parent/child window usage.
- The stub splitter classes (`SplitterManager`, `SplitterDragHandler`,
  `PositioningUtilities`) were removed; `BeepDockSplitter` driven by
  `DockingLayoutController` is the only splitter.
- Caption / chrome painting moved off raw GDI onto the renderer framework
  (`CaptionRenderer` + `CaptionLayoutManager`, `SplitterRenderer`, `AutoHideStripRenderer`).

---

## Folder map

| Folder | Contents |
|--------|----------|
| `Docking/` | `BeepDockingManager` (+ `.DragDrop`, `.Persistence` partials), `BeepDockspace`, batch-update scope, theme colors. |
| `Docking/Models/` | `DockPanel`, `DockGroup`, `DockLayoutTree`, `DockLayoutDefinition`, enums, event args. |
| `Docking/Layout/` | `DockingLayoutController`, `LayoutCalculator`, `DockLayoutResult`, `LayoutValidator`. |
| `Docking/Painters/` | Renderer set + factory + context; `Caption/`, `Splitter/`, `AutoHide/` element renderers. |
| `Docking/Layoutmanagers/` | `CaptionLayoutManager`, `AutoHideStripLayoutManager` (geometry + hit-testing). |
| `Docking/Runtime/` | `BeepDockSplitter`, `AutoHideStrip`, `AutoHideSlidePanel`, `FloatWindow`, `DockingGuideOverlay`, `Animation/`, `DragDrop/`. |
| `Docking/Helpers/` | `DockingCaptionPainter` (icon utility). |
| `Docking/Plans/` | Phase plans + `CODE-DICTIONARY.md` + `MASTER-TODO-TRACKER.md`. |

See `Plans/CODE-DICTIONARY.md` for a per-file responsibility table.
