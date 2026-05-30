# PHASE 07 — Designer & Smart-Tag ActionList

**Goal:** A first-class Visual Studio design-time experience for `BeepDockingManager`: drop it, use
smart-tag verbs to add tool windows/documents and arrange them, and have every change
**serialize into the host `*.Designer.cs`** (Phase 06). Design-time child creation follows the
`AGENTS.md` `IDesignerHost.CreateComponent` pattern.

**Depends on:** 00–05 · **Blocks:** 06 (creation mechanism)

---

## 7.1 Existing-code disposition (this phase)

| Concern | Disposition | Notes |
|--------|-------------|------|
| Any designer code currently inside the control assembly | **Move** | Designer types live in `TheTechIdea.Beep.Winform.Controls.Design.Server\Docking\` (Beep house rule), not in the runtime control assembly. |
| `BeepDockingManager` design-time attributes | **Replace** | Re-applied to `BeepDockingManager` (`[Designer(typeof(BeepDockingManagerDesigner))]`, `[ToolboxItem(true)]`). |

---

## 7.2 New components (in `Design.Server\Docking\`)

```
BeepDockingManagerDesigner.cs          // ParentControlDesigner; verbs; creates child panels via host; manages LayoutDefinition changes
DockPanelDesigner.cs      // ControlDesigner for a tool-window/document page
BeepDockspaceDesigner.cs      // ControlDesigner for a docked tab cell (child surface enabled)
DockingManagerActionList.cs   // DesignerActionList: smart-tag items for BeepDockingManager
DockPanelActionList.cs        // DesignerActionList: per-panel verbs
DockingVerbs.cs               // shared verb command implementations
```

---

## 7.3 Smart-tag verbs

**On `BeepDockingManager`:**
- Add Tool Window (Left/Right/Top/Bottom) — creates a `DockPanel` via host.
- Add Document — creates a `DockPanel` (`Kind=Document`) in the document well.
- Style (`BeepControlStyle` dropdown), Theme, `UseThemeColors` toggle.
- Reset Layout / Clear.

**On `DockPanel`:**
- Toggle Float / Auto-Hide / Closeable.
- Set DockPosition; set as active tab.
- Rename `Key`/`Title`.

Each mutating verb wraps changes in `IComponentChangeService.OnComponentChanging/Changed`
against the `BeepDockingManager.LayoutDefinition` property so the change lands in `*.Designer.cs`.

---

## 7.4 Design-time child creation (AGENTS.md pattern)

```csharp
IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
var changeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
var layoutProp = TypeDescriptor.GetProperties(beepDocks)["LayoutDefinition"];

var panel = (DockPanel)host.CreateComponent(typeof(DockPanel));
changeSvc?.OnComponentChanging(beepDocks, layoutProp);
beepDocks.AddPanel(panel, position);          // updates live tree + LayoutDefinition
changeSvc?.OnComponentChanged(beepDocks, layoutProp, null, panel);
```

- Panels are created by the **host** → serialized into `*.Designer.cs`.
- Enable child-surface interaction (drag/drop at design time) via `EnableDesignMode` for
  nested surfaces where appropriate (see `winforms-control-designer-child-interaction-skill`).

---

## 7.5 Implementation steps

1. `BeepDockingManagerDesigner : ParentControlDesigner`; expose verbs + action list.
2. `DockingManagerActionList` / `DockPanelActionList`.
3. Design-time child creation via `IDesignerHost.CreateComponent` (panels, dockspaces, document well).
4. Hook `IComponentChangeService` around `LayoutDefinition` mutations.
5. `DockPanelDesigner` / `BeepDockspaceDesigner` for selection + child interaction.
6. Apply `[Designer(typeof(BeepDockingManagerDesigner))]` + `DesignerSerializationVisibility.Content`
   on `LayoutDefinition` (Phase 06).
7. Verify serialization into `*.Designer.cs` for all verbs.

---

## 7.6 TODO checklist

- [x] `BeepDockingManagerDesigner` (`ComponentDesigner` — manager is a tray component; verbs + action list + child creation).
- [x] `BeepDockingManagerActionList`, `DockPanelActionList` (verbs/items live on the action lists; no separate `DockingVerbs` file needed — shared logic lives in `BeepDockingDesignerWiring`).
- [x] `DockPanelDesigner`, `BeepDockspaceDesigner`.
- [x] `IComponentChangeService` wrapping for designer.cs round-trip (centralized in `BeepDockingDesignerWiring.SetProperty` + control/parent change notifications).
- [x] Design-time child creation via `IDesignerHost.CreateComponent` (`DockPanel` + `BeepDockspace`), wrapped in `DesignerTransaction` for undo.
- [x] `[Designer(...)]` string attributes on `BeepDockingManager` / `DockPanel` / `BeepDockspace`; designers registered through `BeepDockingTypeRoutingProvider`.
- [x] Appearance smart-tag items (`Style`, `UseThemeColors`, `Theme`) routed through the change service so they persist + re-render.
- [x] All designer types in `Design.Server\Docking\`.

> **Note:** `BeepDockingManager` is a non-visual `IComponent`, so its designer is a
> `ComponentDesigner` (not `ParentControlDesigner`). Design-time layout is represented by
> real `BeepDockspace` child controls + `DockPanel` properties (`DockPosition`, `TabIndex`,
> `PreferredWidth/Height`) which serialize directly into `*.Designer.cs`. The runtime
> `BeepDockingManager.LayoutDefinition` (Phase 06) is the complementary runtime/load path.

---

## 7.7 Verification criteria

- Dropping `BeepDockingManager` and using verbs adds panels/documents that **appear in `*.Designer.cs`**.
- Style/Theme changes via smart tag re-render and persist.
- Rebuild → designer reopens with the same layout (ties to Phase 06).
- Deleting `BeepDockingManager` cleans up its child panels from `*.Designer.cs`.
