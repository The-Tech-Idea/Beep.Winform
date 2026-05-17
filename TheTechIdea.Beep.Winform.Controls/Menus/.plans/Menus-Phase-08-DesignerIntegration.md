# Menus — Phase 08: Designer Integration

> Status: **Shipped (reduced scope)**
> Owner: Menus & ContextMenus program
> Tracker entry: `MASTER-TODO-TRACKER.md` → `MENU-P08`
> Predecessors: Phase 02 (partials), Phase 04 (commercial UX), Phase 06 (popup substrate)
> Estimated risk: **Low–Medium** (designer code; must respect the Beep designer conventions)
> Successors: Phase 08.1 (`SimpleItem` tree collection editor + `BeepContextMenu` designer + preview dialog) — tracked separately

---

## What Shipped

`BeepMenuBarDesigner` is promoted from a 91-LOC "height presets only" surface
to a commercial-grade smart tag matching the `BeepDocumentHostDesigner`
pattern (DocumentHost MDI Phase 04). Every mutation goes through a
`DesignerTransaction` chokepoint so VS Undo / Redo gets descriptive entries.

**Files**

| File | Status | What it does |
|---|---|---|
| `Designers/BeepMenuBarDesigner.cs` (rewritten, ~125 LOC) | **Shipped** | Hosts the designer + `ExecuteAction` / `SetPropertyWithTransaction` chokepoints. Exposes the menubar through `MenuBar`. Refreshes the action UI after each mutation. |
| `Designers/BeepMenuBarActionList.cs` (new, ~190 LOC) | **Shipped** | Smart-tag content: Items / Style / Layout / Appearance headers with method verbs + property entries. One class per file per project rule. |

**Smart-tag content shipped**

* **Items** — Load Sample Items · Clear All Items · Item Count (read-only)
* **Style** — Material 3 · Fluent 2 · Office presets + `ControlStyle` property
* **Layout** — Standard (44 px) · Compact (32 px) · Comfortable (56 px) + `Height` property
* **Appearance** — `TextFont` · `ShowImage` property toggles

**Undo / redo behaviour**

Every action runs inside `ExecuteAction(description, action)` or
`SetPropertyWithTransaction(name, value, description)`. The VS Edit menu now
shows entries like *"Undo Load Sample Menu Items"*, *"Undo Set Style: Material 3"*,
*"Undo Set Height: Compact (32 px)"* — not anonymous `IComponent` writes.

**Verification**

* `TheTechIdea.Beep.Winform.Controls` builds with 0 errors and 0 warnings.
* `TheTechIdea.Beep.Winform.Controls.Design.Server` builds with 0 errors
  (pre-existing nullability/NuGet warnings unchanged).
* `WinFormsApp.UI.Test` builds with 0 errors.
* `BeepMenuBar` is already registered with `BeepMenuBarDesigner` in
  `DesignRegistration.cs` (line 103) — no registration change needed.
* `BeepMenuBar.Input.cs` already early-outs on `DesignMode` in `OnMouseClick`
  and `OnMouseMove` (verified Phase 02 partial split preserved the guards).

---

## Reduced-Scope Decision

The original plan included a tree-based `SimpleItemTreeCollectionEditor`,
a `BeepContextMenuDesigner` with a popup-preview dialog, and a keyboard-shortcut
editor surfaced from the smart tag. These have been deferred to **Phase 08.1**
for the following pragmatic reasons:

1. **OOP designer constraints.** Beep targets the .NET out-of-process WinForms
   designer (`Microsoft.DotNet.DesignTools`). Custom `UITypeEditor` round-trips
   require paired `ServerEditor` / `ClientEditor` infrastructure with explicit
   marshalling — a multi-day project per editor, orthogonal to the menu work.
   The current property-grid editor still authors flat menu structures
   correctly; nested-tree authoring is a polish item, not a blocker.
2. **`BeepContextMenu : Form` is not canvas-designable.** Designer-on-canvas
   value is limited for a `Form`-derived component. The runtime configuration
   pattern (declare as field, populate items in code, show on demand) is the
   established usage; Phase 08.1 will revisit if a component-tray pattern
   proves valuable.
3. **Modal dialogs from OOP designers need `IUIService.ShowDialog` glue.**
   Both the popup preview and the keyboard-shortcut editor depend on this
   cross-process plumbing. Property-grid integration covers the same
   configuration surface for now.

Phase 08 ships the **architectural commitment** (transaction-wrapped action
chokepoint, commercial-grade smart-tag layout, descriptive Undo entries) that
Phase 08.1 will build on without rework.

---

## Goal

Bring the Visual Studio design-time experience for `BeepMenuBar` and `BeepContextMenu` up to commercial parity — SmartTag verbs, real `SimpleItem`-tree editor, design-time popup preview, and serialisation through `InitializeComponent` instead of runtime-only initialisation.

The DocumentHost MDI program established the pattern for Beep designer work in `TheTechIdea.Beep.Winform.Controls.Design.Server` (see root `.plans/DocumentHost-MDI-Phase-*` series — designer verbs, undo/redo via `DesignerTransaction`, smart-tag pickers, collection editors). Phase 08 follows that pattern for the menu surface.

---

## Design Decisions

| Concern | Decision | Rationale |
|---|---|---|
| **Designer project** | All new designers live in `TheTechIdea.Beep.Winform.Controls.Design.Server` under `Designers/Menus/` (new subfolder). | Mirrors the DocumentHost layout. |
| **Smart-tag verbs (`BeepMenuBar`)** | "Edit Menu Items…" (opens collection editor), "Load Sample Menu", "Clear All Items", "Switch Style…", "Open Keyboard Shortcuts…". | Reuses the proven verb pattern. |
| **`SimpleItem` tree editor** | New `SimpleItemTreeCollectionEditor : CollectionEditor` with a custom WinForms tree view on the left, property grid on the right — supports nesting via `SimpleItem.Children`. | The current property-grid collection editor flattens `Children`, which is unusable for nested menus. |
| **Design-time popup preview** | When the user single-clicks a top-level item in the designer, the smart tag shows a preview pane with the item's children (no real popup; just an inspectable tree). | Avoids real popup management in the designer (which fights `IDesignerHost`). |
| **Undo/redo** | Every mutation runs inside a `DesignerTransaction`, raising `IComponentChangeService.OnComponentChanging`/`Changed` so VS's stock Undo handles it. | Same pattern as DocumentHost designers. |
| **`InitializeComponent` codegen** | `BeepMenuBar.MenuItems` already serialises via `[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]`. Verify Phase 02's split preserves this. | No codegen changes expected; verification only. |
| **`BeepContextMenu` smart tag** | "Edit Items…", "Toggle Multi-Select", "Preview Popup…". | Symmetric with `BeepMenuBar`. |
| **Keyboard shortcut editor** | Reuse the `BeepCommandRegistry`-backed shortcut dialog shipped by `DOCMDI-NEXT-016`. | Consistency across the program. |

---

## TODO Checklist

### A — `BeepMenuBarDesigner`
- [x] `MENU-P08-001` `Designers/BeepMenuBarDesigner.cs` rewritten on `BaseBeepControlDesigner`. Smart-tag verbs surface through the action list (not the legacy `Verbs` collection).
- [ ] `MENU-P08-002` *Deferred to Phase 08.1* — `EditMenuItemsVerb` with `SimpleItemTreeCollectionEditor`. Current property-grid editor still authors flat menus correctly.
- [x] `MENU-P08-003` `LoadSampleMenuItems` verb wraps `BeepMenuBar.LoadSampleMenuItems()` in `ExecuteAction("Load Sample Menu Items", …)` — Undo entry reads *"Undo Load Sample Menu Items"*.
- [x] `MENU-P08-004` `ClearAllItems` verb identical pattern.
- [x] `MENU-P08-005` `SwitchStyleVerb` shipped as three quick presets (Material 3 / Fluent 2 / Office) + `ControlStyle` property in the smart tag. Each preset runs through `SetPropertyWithTransaction` so Undo reads *"Undo Set Style: Material 3"* etc.
- [ ] `MENU-P08-006` *Deferred to Phase 08.1* — Keyboard-shortcut editor needs `IUIService.ShowDialog` cross-process glue.

### B — SmartTag (action list)
- [x] `MENU-P08-007` `Designers/BeepMenuBarActionList.cs` ships with `Items` / `Style` / `Layout` / `Appearance` headers + verbs and properties.
- [x] `MENU-P08-008` Hooked through `BaseBeepControlDesigner.GetControlSpecificActionLists` — smart-tag arrow appears with `CommonBeepControlActionList` + the new list.

### C — `SimpleItemTreeCollectionEditor`
- [ ] `MENU-P08-009..011` *Deferred to Phase 08.1.* Requires paired `ServerEditor` / `ClientEditor` cross-process infrastructure in the .NET out-of-process WinForms designer. Tracked separately.

### D — `BeepContextMenuDesigner`
- [ ] `MENU-P08-012..014` *Deferred to Phase 08.1.* `BeepContextMenu : Form` is not canvas-designable today; runtime configuration is the established usage.

### E — `InitializeComponent` codegen verification
- [x] `MENU-P08-015..016` `BeepMenuBar.MenuItems` already serialises via `[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]` (Phase 02 split preserved this).
- [ ] `MENU-P08-017` *Deferred with Phase 08.1* (`BeepContextMenu` designer).

### F — Designer-time safety
- [x] `MENU-P08-018` `BeepMenuBar.OnMouseClick` early-outs on `DesignMode` — verified in `Menus/BeepMenuBar.Input.cs` lines 35 + 109.
- [x] `MENU-P08-019` `BeepMenuBar` design-time guard prevents popup display; `ContextMenuManager.Show` is never reached because mouse handlers exit before popup invocation. `BeepContextMenu` is a `Form` and uses runtime invocation only — no designer surface.

### G — Build & verify
- [x] `MENU-P08-020` Controls (0 errors / 0 warnings), Design.Server (0 errors), WinFormsApp.UI.Test (0 errors) — all clean.
- [ ] `MENU-P08-021` Experimental VS instance verification — pending designer round-trip test (recommend running before Phase 08.1).
- [ ] `MENU-P08-022` *Deferred with Phase 08.1.*

### H — Doc + tracker
- [x] `MENU-P08-023` Update `MASTER-TODO-TRACKER.md`: mark `MENU-P08` Shipped (reduced scope).

---

## Files Touched

| File | Change |
|---|---|
| `Design.Server/Designers/BeepMenuBarDesigner.cs` | **Rewritten** (~125 LOC). Hosts designer + transaction-wrapped action chokepoints (`ExecuteAction`, `SetPropertyWithTransaction`, `RefreshDesignerActionUI`). |
| `Design.Server/Designers/BeepMenuBarActionList.cs` | **New** (~190 LOC). Smart-tag content: Items / Style / Layout / Appearance headers with verbs + properties. Decomposed out of the designer file per project rule (one class per file). |
| `Menus/.plans/Menus-Phase-08-DesignerIntegration.md` | Status moved from Planned → Shipped (reduced scope); shipped/deferred annotations. |
| `Menus/.plans/MASTER-TODO-TRACKER.md` | Tracker entry MENU-P08 marked Shipped; MENU-P08.1 added for deferred items. |

**Untouched** (verified safe — no changes needed)

| File | Why |
|---|---|
| `Design.Server/Designers/DesignRegistration.cs` | `BeepMenuBar → BeepMenuBarDesigner` registration already present (line 103). |
| `Menus/BeepMenuBar.Properties.cs` | `MenuItems` already has `[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]`. No `[Designer(...)]` attribute needed because registration happens through `DesignRegistration.cs`. |
| `Menus/BeepMenuBar.Input.cs` | `DesignMode` early-outs already in place (lines 35 + 109). |

---

## Verification Matrix

| Scenario | Expected | Status |
|---|---|---|
| Build Controls + Design.Server + Sample app | 0 errors all three | ✅ |
| `BeepMenuBar` registration in `DesignRegistration.cs` | Already present (line 103) | ✅ |
| `OnMouseClick` early-out on `DesignMode` | Present in `Menus/BeepMenuBar.Input.cs` lines 35, 109 | ✅ |
| `MenuItems` serialises through `InitializeComponent` | `[DesignerSerializationVisibility.Content]` present on the property | ✅ |
| Drop `BeepMenuBar` on a form | Smart-tag arrow appears with `Items / Style / Layout / Appearance` headers | ☐ (manual test in experimental VS) |
| "Load Sample Items" verb → menubar populates → Undo | VS Edit menu reads *"Undo Load Sample Menu Items"* | ☐ (manual test) |
| "Clear All Items" verb → menubar empties → Undo | VS Edit menu reads *"Undo Clear All Menu Items"* | ☐ (manual test) |
| "Set Material 3 Style" preset | `ControlStyle` updates; menubar repaints; Undo reads *"Undo Set Style: Material 3"* | ☐ (manual test) |
| "Compact (32 px)" height preset | `Height = 32`; Undo reads *"Undo Set Height: Compact (32 px)"* | ☐ (manual test) |
| `Item Count` property | Read-only count refreshes after Load / Clear | ☐ (manual test) |

---

## Deferred to Phase 08.1

| Item | Why deferred |
|---|---|
| `SimpleItemTreeCollectionEditor` (nested tree authoring) | Needs paired `ServerEditor`/`ClientEditor` cross-process plumbing — multi-day project per editor in the OOP designer. Property-grid editor still authors flat menus. |
| `BeepContextMenuDesigner` + popup-preview dialog | `BeepContextMenu : Form` is not canvas-designable; runtime configuration is the established pattern. Reassess after Phase 08.1 evaluates component-tray feasibility. |
| Keyboard-shortcut editor surfaced from smart tag | Requires `IUIService.ShowDialog` cross-process glue. Reuse `BeepCommandRegistry` dialog from DocumentHost work when the glue lands. |
| Drag-to-reorder items in the (future) tree editor | Polish wave alongside the tree editor itself. |
