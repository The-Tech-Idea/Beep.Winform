# Phase 08 — IDisplayContainer integration for the MDI / document host stack

**Status:** ✅ **Shipped** (this iteration).
**Owner:** Beep Design-Time / Controls team.
**Companion code:**
`TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentManager.DisplayContainer.cs` *(new)*
`TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentManager.cs` *(sealed → sealed partial; Dispose hook)*

---

## 1. Problem we set out to solve

The Beep ecosystem already standardises on `IDisplayContainer`
(`TheTechIdea.Beep.Vis.Modules`) for hosting `IDM_Addin` instances:

```csharp
IDisplayContainer container = …;
container.AddControl("Orders", new OrdersAddin(), ContainerTypeEnum.TabbedPanel);
container.ShowControl("Orders", ordersAddin);
container.AddinAdded += (s, e) => Log("added " + e.TitleText);
```

Until this phase the **only** implementation was `BeepDisplayContainer` — a
panel-based tab host. If a developer wanted MDI semantics (cascading /
tiled child windows, native MDI menu merging, Window menu), they had to
fall back to raw WinForms MDI and lose the addin pipeline.

The request was: *"Can `IDisplayContainer` be implemented on the MDI
system, so I can use it instead of `BeepDisplayContainer` while still
getting MDI?"*

---

## 2. Solution

Make **`BeepDocumentManager` itself an `IDisplayContainer`.** The manager
already orchestrates three presentation modes (`BeepTabbedView`,
"Browser Tabs" styled tabs, `BeepNativeMdiView`) and exposes a clean
`AddDocument` / `RemoveDocument` / `ActivateDocument` façade. A thin
partial class bridges the IDisplayContainer surface onto the existing
pipeline so the developer gets a single drop-in replacement that picks
its presentation mode through the Phase-07 Setup Wizard.

```csharp
// Designer creates beepDocumentManager + view; the Setup Wizard picks
// "Native MDI" + wires IsMdiContainer = true; runtime code uses:
IDisplayContainer container = beepDocumentManager;
container.AddControl("Orders",   new OrdersAddin(),   ContainerTypeEnum.TabbedPanel);
container.AddControl("Invoices", new InvoicesAddin(), ContainerTypeEnum.TabbedPanel);
```

In Native MDI mode each `AddControl` becomes an MDI child Form.
In Tabbed / Browser modes each `AddControl` becomes a `BeepDocumentPanel`.
The developer never touches mode-specific code.

---

## 3. Architecture (high level)

```
                     IDisplayContainer (Beep.Vis.Modules)
                                  ▲
                                  │  implements
                  BeepDocumentManager  (sealed partial)
                       ┌───────────┼────────────┐
                       │           │            │
              BeepTabbedView   "Browser"     BeepNativeMdiView
                  │              │                 │
            BeepDocumentPanel   BeepDocumentPanel   MDI child Form
                  │              │                 │
              IDM_Addin       IDM_Addin         IDM_Addin
```

- `BeepDocumentManager` is now `sealed partial class` so the
  IDisplayContainer surface lives in its own file
  (`BeepDocumentManager.DisplayContainer.cs`).
- A small `AddinEntry` record tracks each hosted addin (`Title`,
  `DocumentId`, `MdiChild?`, `DocumentPanel?`).
- For panel modes (Tabbed / Browser) `AddControl` calls
  `manager.AddDocument(title)` then reparents the addin (Control or
  Form) into the returned `BeepDocumentPanel`.
- For Native MDI mode `AddControl` either:
  - reparents the addin Form directly (`form.MdiParent = …`), or
  - lets `BeepNativeMdiView.AddDocument` create the host form, then
    intercepts via the existing `DocumentFormCreated` event to attach
    the addin Control to the freshly created MDI child Form.
- Manager events (`DocumentRemoved`, `ActiveDocumentChanged`) are
  bridged to IDisplayContainer events (`AddinRemoved`, `AddinChanged`),
  so subscribers see a consolidated surface.

---

## 4. IDisplayContainer surface — per-member behaviour

| Member                                | Behaviour in tabbed/browser mode                                                      | Behaviour in Native MDI mode                                                                                          |
| ------------------------------------- | ------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------- |
| `AddControl(title, addin, type)`      | `AddDocument(title)` → reparents addin into `BeepDocumentPanel` (`Dock = Fill`).      | `_pendingMdiAddin = addin` → `AddDocument(title)` → `DocumentFormCreated` reparents the Control into the child Form. |
| `RemoveControl(title, addin)`         | `RemoveDocument(documentId)` → addin disposed.                                        | `mdiChild.Close()` → bridge raises `AddinRemoved`.                                                                    |
| `RemoveControlByGuidTag` / `…ByName`  | Linear lookup → `RemoveControl`.                                                      | Same.                                                                                                                  |
| `ShowControl(title, addin)`           | `ActivateDocument(documentId)` → tab focused.                                          | `mdiChild.Activate()` (+ maximise when `ContainerType == SinglePanel`).                                                |
| `IsControlExit(addin)`                | `_addinEntries.Values.Any(e => e.Addin == addin)`.                                    | Same.                                                                                                                  |
| `Clear`                               | Walks all entries, calls `RemoveControl`, then `CloseAllDocuments` as a guard.        | Same.                                                                                                                  |
| `ShowPopup(addin)`                    | Mirrors `BeepDisplayContainer` exactly: addin Form → centre-on-parent `Show()`. Addin Control → wrap in a `BeepiFormPro` with caption bar. | Same; popups are independent of the MDI container so they cooperate with menu-merging.                                |
| `ContainerType` (legacy enum)         | `TabbedPanel` → `TabPosition = Top`. `SinglePanel` → `TabPosition = Hidden`.          | `SinglePanel` → maximises the active child.                                                                            |
| `PressKey(KeyCombination)`            | Raises `KeyPressed`; returns `Errors.Ok` / `Errors.Failed`.                            | Same.                                                                                                                  |
| `AddinAdded` / `AddinRemoved` / `AddinChanged` | Raised directly by `AddControl` / `RemoveControl`, and re-raised by the view bridge so external callers (`BeepDocumentHost.RemoveDocument`, etc.) also fire them. | Same.                                                                          |
| `AddinMoved` / `AddinChanging`        | Reserved; not raised by the current bridge.                                            | Reserved.                                                                                                              |
| `PreCallModule`                       | Reserved.                                                                              | Reserved.                                                                                                              |
| `PreShowItem`                         | Raised at the top of `ShowControl` with full `PassedArgs`.                             | Same.                                                                                                                  |

---

## 5. Lifecycle invariants

1. **Initialize before content.** `IDM_Addin.Initialize()` is called
   before the addin is parented to any panel or MDI form. If it throws,
   `AddControl` returns `false` and no entry is registered.
2. **Single instance enforcement.** When the addin type carries
   `[Addin(ScopeCreateType = Single)]` the manager refuses a second
   `AddControl` with the same title — matches `BeepDisplayContainer`.
3. **One entry per title.** The internal dictionary uses
   `OrdinalIgnoreCase` to match the legacy contract.
4. **Dispose on remove.** `RemoveControl` always calls
   `addin.Dispose()` exactly once even when the underlying view rejects
   the close.
5. **Disposal of the manager.** `Component.Dispose(true)` invokes the
   private `DisposeDisplayContainer()` helper which unhooks the bridge
   subscriptions and disposes every remaining addin.

---

## 6. Native MDI hosting — implementation notes

`BeepNativeMdiView.AddDocument(title, …)` always creates a fresh `Form`
with `MdiParent` set. It also raises a synchronous
`DocumentFormCreated(MdiDocumentEventArgs)` event so callers can add
content controls to the child form before it becomes visible.

The DisplayContainer partial uses that hook in two ways:

- **Form addin** path — bypasses the view's form creation altogether.
  We reparent the addin's own Form directly (`MdiParent =
  view.ParentForm`) so addin-specific menus, icons, and `FormClosing`
  handlers are preserved.
- **Control addin** path — sets a `[ThreadStatic] _pendingMdiAddin`,
  calls `AddDocument(title)`, and the single `OnNativeMdiFormCreated`
  handler attaches the Control with `Dock = Fill`. The
  `_pendingMdiAddin` guard prevents accidental re-hosting when other
  code paths (template seeding, design-time documents) add MDI
  children.

`OnNativeMdiFormCreated` is attached lazily the first time `AddControl`
runs in Native MDI mode, and detached in `DisposeDisplayContainer`.

---

## 7. Event bridging

```text
                   manager.DocumentRemoved
                            │
                            ▼
       OnDisplayContainerDocumentRemoved(e)
                            │
                ┌───────────┴───────────┐
                ▼                       ▼
       _addinEntries.Remove(title)   AddinRemoved?.Invoke
```

The bridge is wired exactly once (`_displayContainerEventsBridged`
flag) and unwired in `DisposeDisplayContainer`. This guarantees the
IDisplayContainer surface always reflects reality even when the view
removes a document via right-click menu, middle-click, programmatic
`RemoveDocument`, or "Close All Documents".

---

## 8. Files touched

**Runtime (new):**

- `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentManager.DisplayContainer.cs`
  — full `IDisplayContainer` implementation in a dedicated partial.

**Runtime (in-place):**

- `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentManager.cs`
  - Class declaration: `public sealed class …` → `public sealed partial class …`.
  - `Dispose(bool)` calls `DisposeDisplayContainer()` before
    `DetachView()` so the bridge tears down with valid `_view`.

No design-server changes are required for this phase. The Phase-07
Setup Wizard already configures the manager's view; from the user's
perspective, *the same manager they wired through the wizard is now an
IDisplayContainer.*

---

## 9. Verification matrix

| Scenario                                                                                                                                                                       | Result |
| ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------ |
| `BeepDocumentManager` compiles with `: IDisplayContainer` and all 9 members implemented (runtime project: 0 errors).                                                            | ✅     |
| Design server still compiles after manager partial change (0 errors).                                                                                                            | ✅     |
| Setup Wizard "Native MDI" → `(IDisplayContainer)manager.AddControl("Orders", addin, …)` creates an MDI child Form with the addin as content.                                    | ✅ via DocumentFormCreated bridge |
| Setup Wizard "Tabbed Documents" → same call creates a `BeepDocumentPanel` with the addin docked Fill.                                                                            | ✅ via AddDocument |
| `RemoveControl` from either mode fires `AddinRemoved` once, even when the view also raises `DocumentRemoved`.                                                                     | ✅ via dedup in bridge |
| `IsControlExit(addin)` returns `true` after AddControl, `false` after RemoveControl.                                                                                              | ✅     |
| `ShowPopup(addin)` works while the manager is in any mode (popup is independent of the MDI parent).                                                                              | ✅     |
| `Clear()` removes all tracked addins and closes all documents.                                                                                                                    | ✅     |

---

## 10. Pending follow-ups (tracked for a later phase)

- **DML/Form-addin discovery in Setup Wizard.** Today the wizard seeds
  *sample tabs only.* A future enhancement could enumerate registered
  addins (via `IDMEEditor.ConfigEditor`) and let the developer pick a
  starter addin to be added on form load.
- **`AddinMoved` / `AddinChanging` / `PreCallModule` events.** Reserved
  on the bridge today; would require the views to surface "tab
  dragged between groups" and "about-to-show" hooks.
- **`ContainerType = SinglePanel` for Tabbed mode** could also collapse
  the splitter bars when only one group exists — currently we only
  hide the tab strip.
- **Designer affordance.** Consider adding a "Make this an
  IDisplayContainer (legacy API)" toggle in the smart-tag so junior
  developers see the contract surface without having to know it
  exists.

---

## 11. Workspace conventions applied

- One file = one responsibility (`BeepDocumentManager.DisplayContainer.cs`
  contains nothing but the IDisplayContainer surface and its private
  helpers).
- Partial classes used to extend the existing sealed type without
  touching the 1,300-line manager file.
- All comments explain *why*, never *what*.
- Zero new linter warnings introduced.
