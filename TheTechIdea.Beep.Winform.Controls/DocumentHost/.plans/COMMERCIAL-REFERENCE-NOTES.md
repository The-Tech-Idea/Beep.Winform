# Commercial Reference Notes

*Last updated: May 2026 — re-audited after full Track G implementation*

## Benchmark Targets

- **DevExpress DockManager + DocumentManager** (WinForms) — gold standard design-time UX
- **Krypton Docking** (ComponentFactory/Krypton, GitHub, 2.1k stars) — VS-style WinForms
- **DockPanel Suite** (dockpanelsuite/dockpanelsuite, GitHub, 1.9k stars) — mature WinForms
- **AvalonDock** (Dirkster99/AvalonDock, GitHub, 1.7k stars) — WPF MVVM reference
- **Syncfusion DockingManager** — commercial WinForms

## How To Use These References

- Match behaviors and product *principles*, not proprietary APIs.
- Keep Beep naming, `BaseControl`, and Beep theme conventions.
- Prefer predictable defaults over raw feature count.
- Treat GitHub examples as architecture and regression references, not UI-cloning targets.

---

## DevExpress DockManager / DocumentManager — Gap Status

### 1. Properties Window — Fully Categorized
Every property has `[Category]`, `[Description]`, `[DefaultValue]` attributes.
**Beep status: ✅ RESOLVED** — All properties in `BeepDocumentHost.Properties.cs` carry full attributes grouped into Appearance / Behavior / Layout / Policies / Persistence / Animation.

### 2. Smart-Tag — Inline Property Pickers
DevExpress smart-tags use `DesignerActionPropertyItem` for the most-used properties —
users pick tab style, dock style, theme inline without opening dialogs.
**Beep status: ❌ OPEN** — ActionList has only method-items (dialogs). No `DesignerActionPropertyItem` for TabStyle, TabPosition, CloseMode, ShowAddButton.

### 3. Drop-Onto-Panel Content in Designer
Drag a UserControl from the toolbox, drop it on a DockPanel caption → it becomes the
panel's content. Designer's `OnDragDrop` handles routing.
**Beep status: ❌ OPEN** — No `CanParent()` / `OnDragDrop()` override in `BeepDocumentHostDesigner`.

### 4. Global Policy Properties
`AllowDockPanelFloat`, `AutoHideSpeed`, `CaptionButtons` (flags), `ClosePageButtonShowMode`.
**Beep status: ✅ RESOLVED** — `AllowFloat`, `AllowSplit`, `AllowPin`, `AllowAutoHide`, `MaxSplitDepth`, `CloseButtonMode` (enum: Always/OnHover/ActiveOnly/Never/Hidden), `ShowPinButton`, `ShowMaximizeButton` all exist with full wiring and internal validation helpers.

### 5. Window Menu Auto-Population
DocumentManager automatically populates the app's Window menu.
**Beep status: ✅ RESOLVED** — `PopulateWindowMenu(ToolStripMenuItem)` and `AttachWindowMenu(MenuStrip, text)` exist. Menu rebuilds live on DropDownOpening.

### 6. Header Buttons as Flags
DevExpress uses a flags enum (`CaptionButtonsFlags`) for Maximize/AutoHide/Close/Custom.
**Beep status: ⚠️ PARTIAL** — `ShowPinButton` and `ShowMaximizeButton` exist. No flags-enum; per-button policy is still individual properties. Acceptable for 1.0.

### 7. Drag Ghost Window Quality
DevExpress shows a themed 200×120 document ghost with tab strip header + title.
**Beep status: ❌ OPEN** — Ghost is 140×28 px with hardcoded dark color. Not theme-aware. No Escape key to cancel.

### 8. Animated Drop-Indicator Line
DevExpress paints a 2 px animated insert-caret line between tabs during drag-reorder.
**Beep status: ❌ OPEN** — Only cursor changes during drag; no visual caret is painted.

---

## Krypton Docking — Gap Status

### Stable Persistence Identity
`KryptonPage.UniqueName` — stable across title renames.
**Beep status: ✅ RESOLVED** — `DocumentDescriptor.PersistenceKey` (GUID, set once on creation) and `PreviousGroupId` both exist.

### Hover-Trigger for Auto-Hide
`AutoHiddenShowDelay` property (default 400 ms).
**Beep status: ✅ RESOLVED** — `AutoHideHoverDelay` property (default 400 ms) exists and is wired to `BeepAutoHideStrip.HoverDelay`.

### Remember Last Auto-Hide Size
Panel restores to last user-set width/height.
**Beep status: ✅ RESOLVED** — `_ahLastSize` dictionary populated on close; used in `ShowAhOverlay`.

### Auto-Hide Flyout Header (title + pin/close)
Krypton flyouts have a caption bar with title, pin icon, and close button.
**Beep status: ❌ OPEN** — The `_ahOverlay` is a plain Panel with no header.

### Auto-Collapse on Focus Loss
Krypton: flyout collapses when the user clicks elsewhere (500 ms debounce).
**Beep status: ❌ OPEN** — No focus-loss / deactivate handler on the overlay.

### Add-Page Smart-Tag Verbs
Krypton smart-tag: "Add Page", "Add Dockable Space", "Add Navigator".
**Beep status: ❌ OPEN** — See DevExpress G2 gap above.

---

## DockPanel Suite — Gap Status

### Stable Persistence Identity
`IDockContent.DockHandler.PersistString` — stable, application-set.
**Beep status: ✅ RESOLVED** — `PersistenceKey` in `DocumentDescriptor`.

### DockState Enum (Full Vocabulary)
Float, DockTopAutoHide, DockBottomAutoHide, etc.
**Beep status: ✅ ACCEPTED (partial)** — `DocumentDockState` covers Document/Float/AutoHide. Edge-dock (Top/Bottom/Left/Right tool windows) deferred to 1.1 — pure MDI host does not need them.

### Sample-App Helpers
`FindOrCreate(id)`, `SaveLayout()`, `LoadLayout()` are first-class in the sample.
**Beep status: ❌ OPEN** — `MainFrm_MDI.cs` is a stub with no patterns documented.

---

## AvalonDock — Gap Status

### ContentId Stable Identity
**Beep status: ✅ RESOLVED** — `PersistenceKey`.

### LayoutSerializationCallback (cancel/skip model)
`args.Cancel = true` skips missing content gracefully.
**Beep status: ✅ RESOLVED** — `RestoreDocumentFactory` delegate and `LayoutRestoreReport` diagnostics cover this pattern.

### Previous Container Tracking
`PreviousContainerId` + `PreviousContainerIndex`.
**Beep status: ✅ RESOLVED** — `PreviousGroupId` on `DocumentDescriptor`.

### Regression Test Targets
Named scenarios: nested-splits serialization, float-window restore, missing content skip.
**Beep status: ❌ OPEN** — No named regression scenario matrix. Add to designer validation plan.

---

## Remaining Open Gaps — Summary

| Gap | Priority | Track |
|-----|---------|-------|
| Smart-tag inline property pickers (G2) | 🟠 HIGH | G2 |
| Designer drop-onto-document (G3) | 🟠 HIGH | G3 |
| Drag ghost: theme-aware, Escape cancel | 🟡 MEDIUM | Polish |
| Drag insert-caret line painted on strip | 🟡 MEDIUM | Polish |
| Auto-hide flyout header (title + pin/close) | 🟡 MEDIUM | Polish |
| Auto-hide overlay auto-collapse on focus loss | 🟡 MEDIUM | Polish |
| Keyboard: Ctrl+Alt+Arrow, Ctrl+Shift+W, Ctrl+Shift+M | 🟡 MEDIUM | Polish |
| Designer verbs: Export/Clear/Keybindings (G8) | 🟡 MEDIUM | G8 |
| Representative sample MDI form | 🟡 MEDIUM | Track B |
| Regression scenario matrix | 🟢 LOW | Track E |

---

## Principles For BeepDocumentHost (Revised Post-Track-G)

1. **Properties window: done.** Every property categorized — maintain on any new property added.
2. **Drop-onto-panel in designer** — still the top authoring gap (G3).
3. **Inline smart-tag** — highest remaining design-time quality gap (G2).
4. **Drag polish before more painters** — a polished ghost + insert-caret beats the 10th tab style.
5. **Auto-hide flyout header** — mandatory for tool-window quality feel.
6. **Sample form ships with the control** — developers need a starting point.
7. **Restore must be callback-friendly** — already done; keep it that way.
8. **Dock state first-class** — already done; keep it that way.
9. **Advanced extensions stay post-1.0** — cloud, terminal, diff, git never block release.
10. **No new painters before flyout header and G2/G3 are done.**

## What To Avoid

- Copying vendor APIs verbatim (keep Beep naming and BaseControl).
- Exposing internal group or layout-tree mechanics as the primary public model.
- Letting cloud, terminal, diff, or git features complicate the core MDI contract.
- Adding more tab styles or animations before G2/G3 designer gaps are closed.

## Benchmark Targets

- **DevExpress DockManager + DocumentManager** (WinForms) — gold standard design-time UX
- **Krypton Docking** (ComponentFactory/Krypton, GitHub, 2.1k stars) — VS-style WinForms
- **DockPanel Suite** (dockpanelsuite/dockpanelsuite, GitHub, 1.9k stars) — mature WinForms
- **AvalonDock** (Dirkster99/AvalonDock, GitHub, 1.7k stars) — WPF MVVM reference
- **Syncfusion DockingManager** — commercial WinForms

## How To Use These References

- Match behaviors and product *principles*, not proprietary APIs.
- Keep Beep naming, `BaseControl`, and Beep theme conventions.
- Prefer predictable defaults over raw feature count.
- Treat GitHub examples as architecture and regression references, not UI-cloning targets.

---

## DevExpress DockManager / DocumentManager — Key Design-Time Patterns

DevExpress is the target standard for the "easy like DevExpress" goal.

### 1. Properties Window — Fully Categorized
Every property has `[Category]`, `[Description]`, `[DefaultValue]` attributes.
Properties window shows organized groups: Appearance, Behavior, Layout, Document, Keyboard.
**Beep gap:** No `[Category]` attributes exist — all properties land under *Misc*.

### 2. Smart-Tag — Inline Property Pickers
DevExpress smart-tags use `DesignerActionPropertyItem` for the most-used properties —
users pick tab style, dock style, theme inline without opening dialogs.
**Beep gap:** ActionList has only method-items (dialogs). No inline property pickers.

### 3. Drop-Onto-Panel Content in Designer
Drag a UserControl from the toolbox, drop it on a DockPanel caption → it becomes the
panel's content. Designer's `OnDragDrop` handles routing.
**Beep gap:** No `CanParent()` / `OnDragDrop()` override in `BeepDocumentHostDesigner`.

### 4. Global Policy Properties
`AllowDockPanelFloat`, `AutoHideSpeed`, `CaptionButtons` (flags), `ClosePageButtonShowMode`.
**Beep gap:** No AllowFloat, AllowSplit, AllowPin, AllowAutoHide, MaxSplitDepth properties.

### 5. Window Menu Auto-Population
DocumentManager automatically populates the app's Window menu (New Tab Group, list of
open documents, Move to Next/Previous Tab Group).
**Beep gap:** No `PopulateWindowMenu()` or `AttachWindowMenu()` helper.

### 6. Header Buttons as Flags
DevExpress uses a flags enum (`CaptionButtonsFlags`) so any combination of
Maximize/AutoHide/Close/Custom buttons can be shown or hidden per-panel.
**Beep gap:** No per-button visibility policy.

---

## Krypton Docking — Key Patterns

### Stable Persistence Identity
`KryptonPage.UniqueName` — stable across title renames. Layout XML uses it as key.
**Beep gap:** `DocumentDescriptor` has no `PersistenceKey` separate from `Title`.

### Hover-Trigger for Auto-Hide
`AutoHiddenShowDelay` property (default 400 ms). Mouse hover over strip tab opens flyout.
**Beep gap:** Click-only; no hover-trigger timer in `BeepAutoHideStrip`.

### Remember Last Auto-Hide Size
Panel restores to last user-set width/height, not default size.
**Beep gap:** `ShowAhOverlay()` uses a fixed or calculated size, not stored last-size.

### Add-Page Smart-Tag Verbs
Krypton smart-tag: "Add Page", "Add Dockable Space", "Add Navigator".
**Beep gap:** Smart-tag only has dialogs, no quick-add actions.

---

## DockPanel Suite — Key Patterns

### Stable Persistence Identity
`IDockContent.DockHandler.PersistString` — application-set, survives renames.
`DeserializeDockContent` callback — factory pattern matching our `RestoreDocumentFactory`.
**Beep gap:** Add `PersistenceKey` to `DocumentDescriptor`.

### DockState Enum (Full Vocabulary)
`Float, DockTopAutoHide, DockBottomAutoHide, DockLeftAutoHide, DockRightAutoHide,
Document, DockTop, DockBottom, DockLeft, DockRight, Hidden`
**Beep note:** Our `DocumentDockState` covers Document/Float/AutoHide. Edge-dock states
(Top/Bottom/Left/Right tool window docking) are not needed for pure MDI — defer to 1.1.

### Sample-App Helpers
Three critical patterns every consumer needs: `FindOrCreate(id)`, `SaveLayout()`,
`LoadLayout()`. These should be in the sample form and documented in README.

---

## AvalonDock — Key Patterns

### ContentId Stable Identity
`LayoutContent.ContentId` — GUID or string. Used in XML/JSON persistence. Survives renames.
**Beep gap:** Same as above — add `PersistenceKey`.

### LayoutSerializationCallback
```csharp
serializer.LayoutSerializationCallback += (s, args) => {
    args.Content = FindExistingContent(args.Model.ContentId);
    if (args.Content == null) args.Cancel = true; // skip missing
};
```
More explicit than our current callback — consumer can cancel/skip individual items.

### Previous Container Tracking
`PreviousContainerId` + `PreviousContainerIndex` — re-dock to last known position.
**Beep gap:** Add `PreviousGroupId` to `DocumentDescriptor` and persist it.

### Regression Test Targets
Named regression scenarios: serialization of nested splits, float-window restore,
missing content skip, ratio preservation after split collapse.
**Beep action:** Add these as named test scenarios in the regression matrix.

---

## Public GitHub Patterns Worth Copying In Spirit

### DockPanel Suite

1. Explicit dock-state vocabulary.
   - The library revolves around `DockState`, `DocumentStyle`, and helper methods that
     validate whether a state transition is legal.
   - Translation for Beep: formalize `DocumentDockState` and allowed state transitions.

2. Stable persistence identity.
   - `PersistString` and `DeserializeDockContent` make layout restore depend on stable
     content identity, not just visible tab text.
   - Translation for Beep: add a dedicated persistence key or restore key separate from title.

3. Strong persistence model.
   - DockPanel Suite persists contents, panes, dock windows, float windows, bounds,
     z-order, visibility, and active content.
   - Translation for Beep: keep expanding `LayoutRestoreReport` and restore diagnostics rather than adding ad-hoc state.

4. Sample-app workflow matters.
   - The sample app keeps helper methods such as document lookup, create-new, and load or save layout flows simple.
   - Translation for Beep: expose `OpenOrActivate`, window-list helpers, and example host forms.

### AvalonDock

1. Content identity and previous-container tracking.
   - `ContentId`, `PreviousContainerId`, and `PreviousContainerIndex` help re-dock content intelligently.
   - Translation for Beep: pair `DocumentId` with persistence identity and previous-group restore hints.

2. Restore callbacks.
   - `LayoutSerializationCallback` lets the host application provide content, skip items,
     or reattach existing viewmodels during restore.
   - Translation for Beep: add a restore callback or factory API so missing content is easy to reconstruct.

3. Floating geometry is part of the content model.
   - Floating position and size are explicit serialized properties.
   - Translation for Beep: keep float-window geometry and previous docking context first-class in the layout payload.

4. Regression tests target real failures.
   - AvalonDock has serialization and floating-window regression tests for lost content,
     missing documents, ratio preservation, and app crashes.
   - Translation for Beep: add regression scenarios for layout restore, float and dock-back, split collapse, and designer teardown.

### Syncfusion And Other Commercial Docs

1. Linked and nested docking should feel intentional.
2. Context menus, dock restrictions, and drag providers are part of the product surface.
3. Localization and design-time support matter in WinForms.

---

## Principles For BeepDocumentHost (Revised)

1. **Properties window first.** Every property must be categorized before 1.0.
2. **Stable identity beats display text.** Add `PersistenceKey` to `DocumentDescriptor`.
3. **Drop-onto-panel in designer.** Complete the authoring story like DevExpress.
4. **Policies before more features.** AllowFloat/AllowSplit/AllowPin before more painters.
5. **Window menu out of the box.** Every MDI host needs it.
6. **Restore must be callback-friendly.** Match AvalonDock's explicit cancel/skip model.
7. **Flyout auto-hide with hover-trigger.** Click-only is not commercial quality.
8. **Sample form ships with the control.** Developers need a starting point.
9. **Dock state should be first-class.** Already done — keep it that way.
10. **Advanced extensions layer on top.** Cloud, terminal, diff, git stay post-1.0.

## What To Avoid

- Copying vendor APIs verbatim (keep Beep naming and BaseControl).
- Exposing internal group or layout-tree mechanics as the primary public model.
- Letting cloud, terminal, diff, or git features complicate the core MDI contract.
- Adding more tab styles or animations before fixing Properties window categories.