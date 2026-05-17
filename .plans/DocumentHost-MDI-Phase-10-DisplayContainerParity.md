# DocumentHost MDI — Phase 10: BeepDisplayContainer Parity & Tab Style Presets

> Status: **Shipped** (DOCMDI-NEXT-019)
> Owner: DocumentHost MDI track
> Tracker entry: `MASTER-TODO-TRACKER.md` → `DOCMDI-NEXT-019`
> Predecessors: Phase 08 (Wizard Polish), Phase 09 (Runtime Wizard Demo)
> Code anchors:
>   - `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentManager.Presets.cs`
>   - `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentManager.DisplayContainer.cs`
>   - `WinFormsApp.UI.Test/DocumentHostMdiDemoForm.cs`

---

## Goal

Close the last ergonomic gap between the legacy `BeepDisplayContainer` and
the new `BeepDocumentManager` so application code can swap the two without
rewriting styling code, and so the Phase 09 demo stops using reflection.

`BeepDisplayContainer.SetTabStylePreset(TabStyle)` has shipped for years as
a one-call way to swap the entire tab-strip look. Until Phase 10 the new
manager had no equivalent — the Phase 09 demo had to call `TrySet` against
property names by reflection to flip add-button + close-button visibility
when entering "browser tabs" mode. This phase ships a real API and removes
the reflection.

---

## Design Decisions

| Concern | Decision | Why |
|---|---|---|
| **Where the API lives** | New partial `BeepDocumentManager.Presets.cs` | Keeps `BeepDocumentManager.cs` (~1326 lines) and `BeepDocumentManager.DisplayContainer.cs` (~592 lines) focused on their existing responsibilities. Partial classes are the project standard for splitting one type across responsibility areas. |
| **Primary entry point** | `SetTabStylePreset(DocumentTabStyle)` | Matches `BeepDisplayContainer.SetTabStylePreset(TabStyle)` ergonomic — same verb, same parameter shape, same return contract (bool). Migration becomes a literal name + enum swap. |
| **Why a new enum (`DocumentTabStyle`)** | DocumentTabStyle already exists on `BeepDocumentHost` and covers the document-host visual vocabulary (Chrome, VSCode, Underline, Pill, Flat, Rounded, Trapezoid). The classic `TabStyle` (Classic, Capsule, etc.) belongs to `BeepTabs` and isn't a 1:1 fit for the document host. | Avoids forcing the document host through `BeepTabs`'s visual contract; lets each enum stay aligned with its rendering surface. |
| **Convenience helpers** | `ApplyBrowserPreset()` → `DocumentTabStyle.Chrome`, `ApplyIdePreset()` → `DocumentTabStyle.VSCode` | Mirrors the language used by the Phase 07 wizard's `DocumentSetupMode.BrowserTabs` and `DocumentSetupMode.TabbedDocuments` tiles, so the same vocabulary flows from designer wizard → runtime API. |
| **Bundled property writes** | Each preset writes `TabStyle` + `TabPosition` + `ShowAddButton` + `CloseButtonMode` as one coherent bundle | Switching from VS Code style to Chrome style without also flipping the add-button and close-button defaults produces incoherent UI. The preset is a coordinated bundle, not a partial set. |
| **Hidden designer hint property** | `CurrentTabStyle { get; set; }` is `[Browsable(false)]` and `[DesignerSerializationVisibility(Hidden)]` | The underlying `BeepDocumentHost.TabStyle` / `ShowAddButton` / `CloseButtonMode` already serialise individually. A serialised manager-level mirror would write the same value twice and could drift. The property exists for code that wants a simple getter/setter without juggling the host reference. |
| **Native MDI mode** | `SetTabStylePreset` returns `false` when there is no tabbed host | Native MDI has no tab strip — there's nothing to style. Silently no-op rather than throw, so application code can call the preset regardless of mode. |
| **CloseButtonShowMode is nested** | `using CloseMode = BeepDocumentHost.CloseButtonShowMode;` | The enum is intentionally nested inside `BeepDocumentHost`; an alias keeps the preset code readable without polluting the namespace. |

---

## TODO Checklist

- [x] `DOCMDI-P10-001` Create `BeepDocumentManager.Presets.cs` partial.
- [x] `DOCMDI-P10-002` Implement `SetTabStylePreset(DocumentTabStyle)` with seven preset bundles (Chrome, VSCode, Underline, Pill, Flat, Rounded, Trapezoid).
- [x] `DOCMDI-P10-003` Add `ApplyBrowserPreset()` / `ApplyIdePreset()` convenience helpers that map to the Phase 07 wizard vocabulary.
- [x] `DOCMDI-P10-004` Add `CurrentTabStyle` designer-hidden hint property with getter+setter so application code can read the live style.
- [x] `DOCMDI-P10-005` Use `using CloseMode = …CloseButtonShowMode;` alias to keep nested-enum references readable.
- [x] `DOCMDI-P10-006` `Browsable(false)` on `CurrentTabStyle` to keep the Property Grid free of duplicate / drift-prone entries.
- [x] `DOCMDI-P10-007` Update `DocumentHostMdiDemoForm.ApplySetupResult` to call `ApplyBrowserPreset()` / `ApplyIdePreset()` instead of `TrySet`.
- [x] `DOCMDI-P10-008` Remove the now-unused reflection helper `TrySet` from the demo to keep the sample free of legacy scaffolding.
- [x] `DOCMDI-P10-009` Build Controls / Design.Server / WinFormsApp.UI.Test — 0 errors.
- [x] `DOCMDI-P10-010` Author this Phase 10 plan doc.
- [x] `DOCMDI-P10-011` Add `DOCMDI-NEXT-019` tracker entry summarising the slice.

---

## API Reference

```csharp
// Drop-in equivalent of BeepDisplayContainer.SetTabStylePreset(TabStyle):
manager.SetTabStylePreset(DocumentTabStyle.VSCode);

// Phase 07 wizard vocabulary:
manager.ApplyBrowserPreset();   // Chrome tabs + add button + always-show close
manager.ApplyIdePreset();       // VS Code tabs + no add button + hover-show close

// Read-only access to the currently active style:
var current = manager.CurrentTabStyle;

// Round-trip is supported (set on this property routes through SetTabStylePreset):
manager.CurrentTabStyle = DocumentTabStyle.Pill;
```

Behavioural contract:

- All entry points are no-ops in **Native MDI** mode (returns `false`); the
  manager has no tabbed host to style in that mode.
- All entry points are **idempotent** — repeating the same preset is safe
  and doesn't fire host-side change events redundantly.
- All entry points are **safe to call before** `Visible = true` / before
  the form is shown; they route through `GetTabbedHost()` which returns
  `null` until the view is created, in which case they're a no-op.

---

## Migration Guide: `BeepDisplayContainer` → `BeepDocumentManager`

| Scenario | Before (`BeepDisplayContainer`) | After (`BeepDocumentManager`) |
|---|---|---|
| Construct + host | `var c = new BeepDisplayContainer { Dock = DockStyle.Fill };` | `var host = new BeepDocumentHost { Dock = DockStyle.Fill };`<br>`var mgr = new BeepDocumentManager();`<br>`mgr.View = new BeepTabbedView { Host = host };` |
| Add an addin | `c.AddControl("Orders", addin, ContainerTypeEnum.TabbedPanel);` | `mgr.AddControl("Orders", addin, ContainerTypeEnum.TabbedPanel);` — **identical signature** |
| Remove an addin | `c.RemoveControl(title, addin);` | `mgr.RemoveControl(title, addin);` |
| Clear all | `c.Clear();` | `mgr.Clear();` |
| Show popup | `await c.ShowPopup(addin);` | `await mgr.ShowPopup(addin);` |
| **Switch tab style** | `c.SetTabStylePreset(TabStyle.Underline);` | `mgr.SetTabStylePreset(DocumentTabStyle.Underline);` |
| Browser-style strip | _(no single call — set TabStyle + ShowCloseButtons + Padding by hand)_ | `mgr.ApplyBrowserPreset();` |
| IDE-style strip | _(same — multi-property)_ | `mgr.ApplyIdePreset();` |
| Listen for addin add | `c.AddinAdded += …;` | `mgr.AddinAdded += …;` |
| Listen for addin remove | `c.AddinRemoved += …;` | `mgr.AddinRemoved += …;` |
| Container type hint | `c.ContainerType = ContainerTypeEnum.SinglePanel;` | `mgr.ContainerType = ContainerTypeEnum.SinglePanel;` *(legacy shim; real mode is in `View` / setup wizard)* |
| Press a key | `c.PressKey(combo);` | `mgr.PressKey(combo);` |

### Surface coverage matrix

| IDisplayContainer member | `BeepDisplayContainer` | `BeepDocumentManager` |
|---|---|---|
| `AddControl` / `RemoveControl` / `ShowControl` | ✅ | ✅ |
| `RemoveControlByGuidTag` / `RemoveControlByName` | ✅ | ✅ |
| `IsControlExit` / `Clear` | ✅ | ✅ |
| `ContainerType` | ✅ | ✅ (legacy shim; wizard owns real mode) |
| `ShowPopup` | ✅ | ✅ |
| `PressKey` | ✅ | ✅ |
| `AddinAdded` / `AddinRemoved` / `AddinChanged` | ✅ fires | ✅ fires |
| `AddinChanging` / `AddinMoved` / `PreCallModule` | declared but never fired by either implementation | declared but never fired by either implementation |
| `KeyPressed` | ✅ fires from `PressKey` | ✅ fires from `PressKey` |
| `PreShowItem` | ✅ fires from `ShowControl` | ✅ fires from `ShowControl` |
| `SetTabStylePreset` *(non-interface extension)* | ✅ `TabStyle` overload | ✅ `DocumentTabStyle` overload (Phase 10) |

There are **zero IDisplayContainer surface gaps** between the two
implementations as of Phase 10. Migration is a literal type swap plus the
preset enum rename.

---

## Verification

- [x] `dotnet build TheTechIdea.Beep.Winform.Controls.csproj` — `0 Warning(s), 0 Error(s)`.
- [x] `dotnet build TheTechIdea.Beep.Winform.Controls.Design.Server.csproj` — `0 Error(s)`.
- [x] `dotnet build WinFormsApp.UI.Test.csproj` — `0 Error(s)`.
- [x] Phase 09 demo (`--demo document-host-mdi`) no longer references the reflection helper; runs the wizard, switches between Tabbed / Browser / Native MDI, and the browser preset visibly toggles the add-button + always-on close button.
- [x] `manager.SetTabStylePreset(...)` is callable before the form is shown and is a no-op in Native MDI mode.
- [x] `manager.CurrentTabStyle` round-trips (write then read returns the same enum value).

---

## Files Touched

| File | Purpose |
|---|---|
| `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentManager.Presets.cs` | **New.** Partial class hosting `SetTabStylePreset`, `ApplyBrowserPreset`, `ApplyIdePreset`, `CurrentTabStyle`. |
| `WinFormsApp.UI.Test/DocumentHostMdiDemoForm.cs` | Replaced `TrySet`-based reflection with `ApplyBrowserPreset` / `ApplyIdePreset`; removed unused `TrySet` helper. |
| `.plans/MASTER-TODO-TRACKER.md` | Added `DOCMDI-NEXT-019`. |
| `.plans/DocumentHost-MDI-Phase-10-DisplayContainerParity.md` | **New.** This document. |

---

## Deferred (Future Waves)

- [ ] `DOCMDI-P10-D-001` Honour `ContainerType = SinglePanel` end-to-end in Native MDI mode by maximising the active child *and* hiding the MDI client's caption row (currently maximises only).
- [ ] `DOCMDI-P10-D-002` Add a smart-tag verb on the manager designer that surfaces the four presets as one-click items (Chrome / VSCode / Underline / Pill) so picking a style in the designer mirrors the runtime API.
- [ ] `DOCMDI-P10-D-003` Wire the wizard's "Browser tabs" / "Tabbed documents" tiles to call `ApplyBrowserPreset` / `ApplyIdePreset` directly when generating the design-time host setup, so the wizard output matches what runtime code would have produced.
- [ ] `DOCMDI-P10-D-004` Add a Phase 10 entry to the docs site / README once the wizard tiles are wired so a side-by-side comparison (`BeepDisplayContainer` ↔ `BeepDocumentManager`) is publicly browsable.
