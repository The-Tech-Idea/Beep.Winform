---
name: beep-forms-integrated
description: Guidance for the integrated Forms layer — BeepForms host, BeepFormsDefinition, and all extracted shell surfaces (BeepFormsHeader, BeepFormsCommandBar, BeepFormsQueryShelf, BeepFormsPersistenceShelf, BeepFormsToolbar, BeepFormsStatusStrip). Use when composing a multi-block form, wiring FormsManager to BeepForms, configuring form-level commands, handling master/detail coordination, savepoints, alerts, and shared view state across shell surfaces.
---

# BeepForms Integrated Controls

`BeepForms` is the non-visual WinForms coordinator that hosts one or more `IBeepBlockView` instances and bridges the `IUnitofWorksManager` (FormsManager) runtime to the visible UI. Shell chrome is owned by extracted satellite controls, not by `BeepForms` itself.

## Use this skill when
- Composing a form from `BeepForms` + one or more `BeepBlock` instances
- Wiring `FormsManager` into the integrated UI layer
- Configuring `BeepFormsDefinition` (definition-first form setup)
- Adding or customizing `BeepFormsHeader`, `BeepFormsCommandBar`, `BeepFormsQueryShelf`, `BeepFormsPersistenceShelf`, `BeepFormsToolbar`, or `BeepFormsStatusStrip`
- Handling master/detail events, form messages, savepoints, or alert workflows
- Routing form-level commands through `IBeepFormsCommandRouter`
- Extending the form with additional shell surfaces following the extracted-surface pattern

## Do not use this skill when
- The task is purely about block-level field editors — use [`beep-block`](../beep-block/SKILL.md)
- The task is about `FormsManager` orchestration APIs — use [`forms`](../forms/SKILL.md)
- The task is about connection management — use [`beep-data-connection`](../beep-data-connection/SKILL.md)

---

## Architecture — Components

### BeepForms (coordinator)
Non-visual `Control` that owns block hosting, manager bridging, and shared `BeepFormsViewState`. No shell chrome.

**Partial-class structure:**

| File | Responsibility |
| --- | --- |
| `BeepForms.cs` | Root control, block registration, `FormsManager` wiring, `Definition` property |
| `BeepForms.Layout.cs` | Non-visual block host layout only |
| `BeepForms.Commands.cs` | Form and block command routing (`IBeepFormsCommandRouter` delegation) |
| `BeepForms.Navigation.cs` | Record navigation wrappers forwarded to `FormsManager` |
| `BeepForms.Messages.cs` | Message publishing and severity mapping into shared `BeepFormsViewState` |
| `BeepForms.Events.cs` | Manager event subscriptions — field changes, block messages, error/warning propagation |
| `BeepForms.MasterDetail.cs` | Master/detail context, coordinated detail refresh messaging |
| `BeepForms.WorkflowShell.cs` | Savepoint and alert wrappers delegating to `FormsManager`/provider services |

### Extracted Shell Surfaces
Each shell surface is a standalone control that reads shared `BeepFormsViewState` exposed by the host.

| Control | Responsibility |
| --- | --- |
| `BeepFormsHeader` | Title / context header over shared host metadata |
| `BeepFormsCommandBar` | Form-level block selection and sync commands |
| `BeepFormsQueryShelf` | Query-mode entry and execution surface; selectable caption variants |
| `BeepFormsPersistenceShelf` | Commit / rollback surface with dirty-state awareness |
| `BeepFormsToolbar` | Savepoint and alert-preset toolbar; designer-time composition surface |
| `BeepFormsStatusStrip` | Renders shared `BeepFormsViewState` with designer-configurable line presets |

---

## Model Types

### `BeepFormsDefinition`
Top-level form definition. Serialized as `DesignerSerializationVisibility.Content` from `BeepForms.Definition`.

| Property | Type | Purpose |
| --- | --- | --- |
| `Id` | string | unique stable identifier |
| `FormName` | string | matches `BeepForms.FormName` |
| `Title` | string | display title for `BeepFormsHeader` |
| `Blocks` | `List<BeepBlockDefinition>` | block definitions materialised as `BeepBlock` via `AutoCreateBlocksFromDefinition` |
| `Metadata` | `Dictionary<string,string>` | key/value bag for custom shell state |

### `BeepFormsViewState`
Shared read-only state bag consumed by all shell surfaces.

| Property | Purpose |
| --- | --- |
| `ActiveBlockName` | currently focused block |
| `StatusText` | status strip text |
| `IsDirty` | unsaved changes flag |
| `CurrentMessage` | latest message text |
| `MessageSeverity` | `BeepFormsMessageSeverity` level |
| `HasActiveCoordination` | master/detail context active |
| `HasActiveSavepoint` | savepoint context active |
| `HasActiveAlert` | alert workflow active |

---

## Contracts

### `IBeepFormsHost`
```csharp
public interface IBeepFormsHost
{
    string FormName { get; set; }
    string? ActiveBlockName { get; }
    IUnitofWorksManager? FormsManager { get; set; }
    BeepFormsDefinition? Definition { get; set; }
    BeepFormsViewState ViewState { get; }
    IReadOnlyList<IBeepBlockView> Blocks { get; }

    event EventHandler? ActiveBlockChanged;
    event EventHandler? FormsManagerChanged;
    event EventHandler? ViewStateChanged;

    bool RegisterBlock(IBeepBlockView blockView);
    bool UnregisterBlock(string blockName);
    bool TrySetActiveBlock(string blockName);
    void SyncFromManager();
}
```

### `IBeepFormsCommandRouter`
Routes form-level commands such as `CommitAsync`, `RollbackAsync`, `EnterQueryAsync`, `ExecuteQueryAsync`, `NewRecord`, `DeleteRecord`, `NextRecord`, `PrevRecord` to the underlying `FormsManager`.

### `IBeepFormsNotificationService`
Publishes messages with severity into the shared `BeepFormsViewState`. Consumed by `BeepFormsStatusStrip`.

---

## Services

### `BeepFormsManagerAdapter`
Internal bridge that attaches to the `IUnitofWorksManager` instance and forwards events (`FieldChanged`, `BlockMessage`, `FormMessage`, `ErrorOccurred`) into `BeepForms` shared view state.

```csharp
_managerAdapter.Attach(_formsManager);
// Detached automatically when FormsManager property is replaced
```

### `BeepFormsCommandRouter`
Default implementation of `IBeepFormsCommandRouter`. Holds a reference to `FormsManager` and translates form-level button intents into manager calls.

### `BeepFormsMessageService`
Default implementation of `IBeepFormsNotificationService`. Stores the latest message and severity in `BeepFormsViewState` and raises `ViewStateChanged`.

---

## Key Behaviors

### Block registration
Three registration paths:
1. **Definition-driven** — `BeepForms.Definition.Blocks` → `AutoCreateBlocksFromDefinition = true` → `BeepBlock` instances created and registered automatically.
2. **Design-time drag** — Drop `BeepBlock` controls onto the form surface; `BeepForms` discovers them through `IBeepBlockView` scan and calls `RegisterBlock`.
3. **Runtime** — `formsHost.RegisterBlock(myBlock)` before or after `FormsManager` is attached.

### FormsManager attachment
```
BeepForms.FormsManager = myFormsManager;
```
Triggers `DetachFromFormsManager` on old instance, `AttachToFormsManager` on new one, then `SyncFromManager()` on all registered blocks.

### Master/detail coordination
`BeepForms.MasterDetail.cs` listens to `FormsManager` relationship events. When the master block advances a record, `BeepForms` publishes a coordinated refresh message to detail blocks via `IBeepFormsNotificationService`.

### Savepoints
`BeepForms.WorkflowShell.cs` delegates to `FormsManager.SavepointManager`:
- `CreateSavepoint(label)` — prompts via `BeepFormsDialogService` if manager does not supply its own alert UI
- `RestoreSavepoint(id)` — restores; updates `HasActiveSavepoint` in view state

### Alert workflow
`WorkflowShell.cs` delegates to `FormsManager`'s `DefaultAlertProvider`. Outcomes are reflected in `BeepFormsViewState.HasActiveAlert`. `BeepFormsToolbar` renders alert preset buttons.

### Error intent preservation
Trigger cancellations are preserved as `Warning` severity; true failures propagate as `Error`. Both flow through `IBeepFormsNotificationService` → `BeepFormsViewState` → `BeepFormsStatusStrip`.

### Command cancellation and rollback
Failed commands do not silently succeed. `BeepForms.Commands.cs` catches `FormsCommandException` and `OperationCanceledException`, maps them to severity, and publishes via notification service.

---

## Design-Time Workflow
1. Drag `BeepForms` onto the WinForms designer (non-visual component area).
2. Open the smart tag → **Edit Definition** → modal `BeepFormsDefinitionEditor`.
3. Add `BeepBlockDefinition` rows (reuses the same `BeepBlockDefinitionEditor`).
4. Set `AutoCreateBlocksFromDefinition = true`; blocks materialize on the form.
5. Drop `BeepFormsHeader`, `BeepFormsCommandBar`, `BeepFormsQueryShelf`, `BeepFormsPersistenceShelf`, `BeepFormsToolbar`, `BeepFormsStatusStrip` onto the form layout where needed.
6. Each shell surface has a `Host` property pointing back to the `BeepForms` instance.

---

## File Locations
```
TheTechIdea.Beep.Winform.Controls.Integrated/
  Forms/
    BeepForms/
      BeepForms.cs                   ← root, IBeepFormsHost
      BeepForms.Commands.cs
      BeepForms.Events.cs
      BeepForms.Layout.cs
      BeepForms.MasterDetail.cs
      BeepForms.Messages.cs
      BeepForms.Navigation.cs
      BeepForms.WorkflowShell.cs
    BeepFormsCommandBar/
      BeepFormsCommandBar.cs
    BeepFormsHeader/
      BeepFormsHeader.cs
    BeepFormsPersistenceShelf/
      BeepFormsPersistenceShelf.cs
    BeepFormsQueryShelf/
      BeepFormsQueryShelf.cs
    BeepFormsStatusStrip/
      BeepFormsStatusStrip.cs
    BeepFormsToolbar/
      BeepFormsToolbar.cs
      BeepFormsToolbar.Actions.cs
    Contracts/
      IBeepFormsCommandRouter.cs
      IBeepFormsHost.cs
      IBeepFormsNotificationService.cs
    Helpers/
      BeepFormsDialogService.cs
      BeepFormsDisplayTextResolver.cs
      BeepFormsHostResolver.cs
    Models/
      BeepFormsCommandBarButtons.cs
      BeepFormsDefinition.cs
      BeepFormsMessageSeverity.cs
      BeepFormsPersistenceShelfButtons.cs
      BeepFormsQueryShelfButtons.cs
      BeepFormsQueryShelfCaptionMode.cs
      BeepFormsToolbarConfiguration.cs
      BeepFormsViewState.cs
    Services/
      BeepFormsCommandRouter.cs
      BeepFormsManagerAdapter.cs
      BeepFormsMessageService.cs
```

---

## ⚠ Bootstrap Gap (Phase 7 — Not Yet Implemented)

**The schema/datasource initialization path is not yet wired.**

The runtime data flow is correct **once FormsManager already has registered blocks**: `SyncFromManager()` reads `EntityStructure` from `manager.GetBlock().UnitOfWork` and flows schema to every block. FormsManager event subscriptions and ViewState sync are all in place.

**What is missing:**

| Expected | Actual |
| --- | --- |
| Setting `BeepForms.Definition` + `BeepForms.FormsManager` triggers FormsManager to connect to datasources, register blocks, and load entity schemas | Developer must manually register blocks with FormsManager before assigning it to BeepForms |
| `BeepDataConnection` is wired to `BeepForms` as the service provider | `BeepDataConnection` and `BeepForms` are independent components with no glue |
| `BeepForms.InitializeAsync()` bootstraps everything end-to-end | No such method exists yet |

**Until Phase 7 is implemented**, bootstrapping requires manual code:
```csharp
// Manual bootstrap (current requirement)
await myFormsManager.RegisterBlockAsync("customers", "NorthwindDB", "Customers");
await myFormsManager.OpenFormAsync("OrderEntry");
this.beepForms1.FormsManager = myFormsManager; // SyncFromManager() fires here
```

**Planned auto-bootstrap path (Phase 7):**
```csharp
// Future — after Phase 7 is implemented
this.beepForms1.DataConnection = this.beepDataConnection1; // IBeepService provider
this.beepForms1.Definition     = myDefinition;            // has ConnectionName + EntityName
this.beepForms1.FormsManager   = myFormsManager;          // triggers IBeepFormsBootstrapper
// BeepForms internally calls FormsManager.RegisterBlockAsync() per block definition
// then SyncFromManager() flows schema to all BeepBlock instances
```

See `.plans/07-phase7-formsmanager-bootstrap.md` for the full design.

---

## Related Skills
- [`beep-block`](../beep-block/SKILL.md) — block-level UI surface hosted inside BeepForms
- [`forms`](../forms/SKILL.md) — FormsManager orchestration layer
- [`beep-data-connection`](../beep-data-connection/SKILL.md) — connection management

## Detailed Reference
Use [`reference.md`](./reference.md) for usage scenarios and code examples.
