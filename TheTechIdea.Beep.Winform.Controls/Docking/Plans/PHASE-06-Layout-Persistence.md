# PHASE 06 — Layout Persistence in the Parent `*.Designer.cs`

**Goal:** The docking layout is persisted by the **WinForms designer** into the host
**Form/UserControl `*.Designer.cs`** — not into external XML/JSON files. When a developer
drops a `BeepDockingManager` on a form, adds tool windows/documents, and arranges them, Visual Studio
writes everything (`BeepDockingManager` properties + child `DockPanel`/`BeepDockspace` components +
their positions/state) into `InitializeComponent()`. Reopening the form restores it exactly.

**Depends on:** 00, 03, 07 · **Blocks:** —

> Owner decision: **designer.cs is the persistence mechanism.** No runtime layout files are
> required. (Optional runtime save/restore can be added later but is out of scope here.)

---

## 6.1 Why designer serialization (and what makes it work)

WinForms only serializes a child control into the parent's `*.Designer.cs` when the child is
a **component created through the designer host** (`IDesignerHost.CreateComponent`) and is
either added to a serialized `Controls` collection or exposed via a property marked
`[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]`. See `AGENTS.md`
(Custom Control Designer pattern). So persistence = "make the layout out of real, designer-
created components with serializable properties."

This means the **data model and the visual tree must be the same thing** at design time:
- `BeepDockingManager` is the sited orchestrator component on the host form; it owns the
  layout relationships (grouping, ratios, active tab, float/auto-hide state).
- `BeepDockspace`, `BeepDockDocumentWell`, `BeepDockSplitter`, and `DockPanel` are real
  designer-created components/controls sited on the host (Phase 07 designer does the creation).
- Their **layout-relevant properties** (`Key`, `Title`, `Kind`, `DockPosition`, `State`,
  `PreferredWidth/Height`, `MinWidth/Height`, group membership, `SplitRatio`, active/tab
  index) are public, browsable, and serializable.

---

## 6.2 Existing-code disposition (this phase)

| File | Disposition | What changes |
|------|-------------|--------------|
| `Models/PanelSerializationInfo.cs` | **Refactor or Retire** | If every persisted field is a serializable property on `DockPanel`/`DockGroup`, the designer serializes them directly and this DTO is unnecessary. Keep only if a compact code-emit helper is wanted. |
| `Models/DockGroup.cs` / `DockLayoutTree.cs` | **Refactor** | Group structure must be reconstructable from serialized properties. Either (a) expose a serializable `BeepDockingManager.LayoutDefinition` content property, or (b) make grouping derivable from panel properties (`DockPosition`, `GroupId`, `SplitRatio`, order). Pick (a) for fidelity (see §6.3). |
| `Layout/LayoutValidator.cs` | **Reuse** | Validate the deserialized tree on load. |
| `BeepDockingManager` persistence bits | **Replace → `BeepDockingManager.Persistence.cs`** | Hosts the serializable layout property + (de)materialization. |

---

## 6.3 Serialization design (recommended: explicit content property)

Expose a single browsable, content-serialized property on `BeepDockingManager`:

```csharp
[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
[DefaultValue(null)]
public DockLayoutDefinition LayoutDefinition { get; }   // get-only, content serialized
```

`DockLayoutDefinition` is a designer-serializable object graph mirroring the tree:
- `DockGroupDefinition { Position, SplitOrientation, SplitRatio, Children[], PanelKeys[], ActivePanelKey }`
- documents well + groups
- floating panels list (key + bounds + last dock target)
- auto-hidden panels list (key + edge)

Each `DockPanel` is itself a designer-created child component (so its own properties
serialize). `LayoutDefinition` records **structure/relationships** (who is grouped with whom,
ratios, active tab, float/auto-hide), referencing panels by `Key`.

On load, `BeepDockingManager` **materializes** the live tree from `LayoutDefinition` + the child panels
and calls the Phase 03 engine to lay out.

> Alternative (simpler, lower fidelity): derive grouping purely from per-panel properties.
> Rejected for v1 because nested splits + ratios + tab order are hard to reconstruct reliably.

---

## 6.4 Designer creation (ties to Phase 07)

Per `AGENTS.md`, child components must be created via the host so they serialize:

```csharp
var panel = (DockPanel)host.CreateComponent(typeof(DockPanel));
changeService.OnComponentChanging(beepDocks, layoutProp);
beepDocks.AddPanel(panel, DockPosition.Left);     // updates LayoutDefinition
changeService.OnComponentChanged(beepDocks, layoutProp, null, panel);
```

This is the mechanism that makes the panel + the layout change appear in `*.Designer.cs`.
Phase 07 implements `BeepDockingManagerDesigner` smart-tag verbs that call this.

---

## 6.5 Implementation steps

1. Make all layout-relevant properties on `DockPanel`/`DockGroup` public + `[Browsable]`
   + sensible `[DefaultValue]` (so only non-defaults serialize).
2. Add `DockLayoutDefinition` (+ `DockGroupDefinition`, float/auto-hide entries) — plain
   serializable types; reference panels by `Key`.
3. Add `BeepDockingManager.LayoutDefinition` content property (`DesignerSerializationVisibility.Content`).
4. `BeepDockingManager.Persistence.cs`: `CaptureDefinition()` (live tree → definition) and
   `MaterializeFromDefinition()` (definition + child panels → live tree → engine layout).
5. Keep the definition in sync as the user docks/splits/floats at design time
   (`OnComponentChanging/Changed` via the designer, Phase 07).
6. On control `EndInit`/load, materialize + validate (`LayoutValidator`) + `ApplyLayout`.
7. Retire/trim `PanelSerializationInfo` if redundant.

---

## 6.6 TODO checklist

- [x] `DockLayoutDefinition` graph (`DockGroupDefinition` + `FloatingPanelInfo` + `AutoHiddenPanelInfo`): groups, split orientation/ratio, tab order, active tab, floats (key+bounds), auto-hidden (key+edge). Get-only `[Content]` collections so the designer emits `.Add(...)` calls.
- [x] `BeepDockingManager.LayoutDefinition` `[DesignerSerializationVisibility.Content]` property over a **stable backing instance** (required for designer round-trip into the host `*.Designer.cs`).
- [x] `CaptureDefinition()` / `FillDefinition()` / `MaterializeFromDefinition()` in `BeepDockingManager.Persistence.cs`; runtime `SaveLayout()`/`LoadLayout()` convenience.
- [x] Materialize on attach: `ManageControl` applies the deserialized `LayoutDefinition` once panels are registered (else falls back to `RecalculateLayout`).
- [ ] **Phase 07:** sync the definition on design-time dock/split/float/auto-hide via `IComponentChangeService` (`OnComponentChanging/Changed`) from `BeepDockingManagerDesigner`; child component creation through `IDesignerHost.CreateComponent`.
- [ ] (Optional) run the deserialized tree through `LayoutValidator` before `ApplyLayout`.

## 6.8 Status — CORE COMPLETE (design-time wiring lands in Phase 07)

- Both projects build with 0 errors.
- New: `Models/DockLayoutDefinition.cs`, `BeepDockingManager.Persistence.cs`.
- The serialization surface (Content property + stable backing instance + get-only collections) is in place so the **host Form/UserControl `*.Designer.cs`** is the persistence store — no XML/JSON.
- The actual emission of child `DockPanel` components + layout-change tracking into `InitializeComponent()` is performed by the Phase 07 designer (`IDesignerHost.CreateComponent` + `IComponentChangeService`); this phase provides the model + materialization the designer drives.
- **Runtime/designer validation pending:** confirm the generated `*.Designer.cs` round-trips in Visual Studio once the Phase 07 designer creates the panel components.

---

## 6.7 Verification criteria

- Drop `BeepDockingManager`, add panels/documents, arrange (split, float, auto-hide), save → all of it
  appears in the host `*.Designer.cs` `InitializeComponent()`.
- Close & reopen the form (or run it) → identical layout, ratios, active tabs, float/auto-hide.
- No external `.xml`/`.json` layout file is created or required.
- Editing a panel property in the Properties grid re-serializes correctly (round-trips).
- A second `BeepDockingManager` on the same form serializes independently.
