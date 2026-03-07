# Nuggets Manage Wizard Display Fix

## Problem

`uc_NuggetsManageWizardLauncher` is discoverable from add-in metadata, but it does not show in `DisplayContainer2` when opened through `AppManager.ShowPage(...)`.

## Root Cause

1. `AppManager.ShowPage(...)` and `ShowPageAsync(...)` resolve the add-in `Type` from `DMEEditor.ConfigEditor.Addins`, but they do not register that route with `RoutingManager` before calling `NavigateTo(...)`.
2. `RoutingManager.NavigateTo(...)` requires the route name to already exist in its internal `_routes` dictionary and fails immediately when it is missing.
3. `uc_NuggetsManageWizardLauncher.Configure(...)` also duplicates data-loading work that belongs in `OnNavigatedTo(...)`, which makes initialization harder to reason about and differs from the working `uc_ConnnectionDrivers` pattern.

## Fix Scope

### 1. Route registration in `AppManager`

- Update synchronous `ShowPage(...)`
- Update asynchronous `ShowPageAsync(...)`
- Register the route on demand using the already-resolved add-in `Type`

### 2. Launcher lifecycle cleanup

- Keep `Configure(...)` limited to setup that is safe before navigation
- Leave service-dependent loading in `OnNavigatedTo(...)`

## Implementation Notes

- The route key must remain the class name because `ShowPage(...)` uses `ConfigEditor.Addins.className` as the navigation key.
- `RoutingManager.RegisterRoute(...)` is safe to call repeatedly because it overwrites the same key and clears cached state for that route.
- No changes are needed in `RoutingManager.NavigateTo(...)`; the missing registration should be fixed at the call site that already has the resolved `Type`.

## Files To Change

- `TheTechIdea.Beep.Desktop.Common/AppManager.cs`
- `TheTechIdea.Beep.Winform.Default.Views/NuggetsManage/uc_NuggetsManageWizardLauncher.cs`

## Expected Result

- The Nuggets Manage wizard launcher opens in `DisplayContainer2`
- Source list loads during navigation
- Existing add-in pages continue to open through the same path without requiring separate manual route registration