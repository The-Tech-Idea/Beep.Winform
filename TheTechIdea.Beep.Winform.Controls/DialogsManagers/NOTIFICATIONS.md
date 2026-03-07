# BeepDialogManager — Notification Usage Cookbook

Reference patterns for adopting the `BeepDialogManager` notification API.  
All methods live on `BeepDialogManager.Instance` (partial class in `BeepDialogManager.Notifications.cs`).

---

## Quick Setup

```csharp
using TheTechIdea.Beep.Winform.Controls.DialogsManagers;

// Inside a Control:
var dm = BeepDialogManager.Instance;
dm.SetHostForm(FindForm());          // resolves parent Form for positioning

// Inside a Form:
var dm = BeepDialogManager.Instance;
dm.SetHostForm(this);

// Inside a non-UI helper that holds a grid/control reference:
var dm = BeepDialogManager.Instance;
dm.SetHostForm(_grid.FindForm());
```

`SetHostForm(null)` is safe — notifications will appear without host anchoring.

---

## Patterns

### 1. Feature-Pending Stubs

Use when a method body is not yet implemented. Provides a consistent "coming soon" message with stable dedupe suppression so the user only sees it once per trigger sequence.

```csharp
// Minimal — dedupe key auto-generated from feature name
dialogManager.NotifyFeaturePending("Export to Excel");

// With shortcut hint shown in the message
dialogManager.NotifyFeaturePending("Export to Excel", "Ctrl+Shift+E");

// With explicit stable dedupe key (preferred for well-known stubs)
dialogManager.NotifyFeaturePending("Export to Excel", "Ctrl+Shift+E", "grid-export-excel-pending");
```

**Output:** `"Export to Excel (Ctrl+Shift+E) is coming in a future update."`

---

### 2. Import / Export Workflow Outcomes

Use after completing (or failing) a file-based import or export operation.

```csharp
// Success — shows count + file name
dialogManager.NotifyImportSuccess(fileName, importedCount);
dialogManager.NotifyExportSuccess(fileName, exportedCount);

// Success with undo — converts to snackbar with UNDO action
dialogManager.NotifyExportSuccess(fileName, exportedCount, undoAction: () => { /* revert */ });

// Failure — shows persistent error toast (durationMs: 0 = stays until dismissed)
dialogManager.NotifyImportFailure(fileName, ex.Message);
dialogManager.NotifyExportFailure(fileName, ex.Message);
```

**Pattern used in:** `BeepFilter.ImportFilters()`, `BeepFilter.ExportFilters()`, `BeepAdvancedFilterDialog` save/load.

---

### 3. Inline Validation Warnings

Use when a non-blocking validation fails (e.g. missing form input). Does **not** block the UI unlike `MessageBox`.

```csharp
dialogManager.ToastDeduped(
    "Please enter a value.",
    BeepDialogManager.ToastType.Warning,
    durationMs: 3000,
    dedupeKey: "my-control-value-required");
```

- Use a **stable, unique `dedupeKey`** per validation site to prevent toast spam on repeated fast clicks.
- `durationMs: 3000` is the recommended duration for warnings.

**Pattern used in:** `GridDialogHelper.InlineCriterionMenu.ApplyCriterion()`

---

### 4. Error Catch Blocks (Private Helper Pattern)

When multiple catch blocks in the same class need identical notification wiring, extract a private helper to avoid repetition.

```csharp
// In the helper/control class:
private void NotifyMyError(string message, string dedupeKey)
{
    var dm = BeepDialogManager.Instance;
    dm.SetHostForm(_grid.FindForm());   // or FindForm() / this
    dm.ToastDeduped(message, BeepDialogManager.ToastType.Error, durationMs: 0, dedupeKey: dedupeKey);
}

// At each catch site:
catch (Exception ex)
{
    NotifyMyError($"Copy failed: {ex.Message}", "grid-copy-error");
}
```

- `durationMs: 0` means the toast persists until manually dismissed — appropriate for unexpected errors.
- Prefix dedupe keys with the component name to avoid cross-component collisions.

**Pattern used in:** `GridClipboardHelper`

---

### 5. Direct Toast Convenience Wrappers

For one-off toasts where no deduplication is needed:

```csharp
dialogManager.ToastSuccess("Settings saved.");
dialogManager.ToastError("Connection failed.");
dialogManager.ToastWarning("Unsaved changes will be lost.");
dialogManager.ToastInfo("3 rows updated.");
```

---

### 6. Show Info Dialog (Blocking)

Use for help text or content that the user should acknowledge before continuing (e.g. keyboard shortcuts reference).

```csharp
dialogManager.ShowInfo("Keyboard Shortcuts", helpText);
```

---

### 7. Snackbar with Undo

Use after a reversible destructive action (delete, overwrite).

```csharp
dialogManager.SnackbarUndo(
    "3 filters deleted.",
    undoAction: () => RestoreFilters(snapshot),
    durationMs: 5000,
    dedupeKey: "filter-delete-undo");
```

---

### 8. Auto-Close Dialog (Countdown)

Set `AutoCloseTimeout` on any `DialogConfig` to auto-dismiss the dialog after a countdown. The primary button label shows remaining seconds (e.g. `"OK (5)"`). The dialog closes with `DialogResult.Cancel` when the timer expires — the user did not confirm.

```csharp
var config = DialogConfig.CreateSessionTimeout();
config.AutoCloseTimeout = 30_000;  // 30 seconds; label becomes "Sign In (30)"

var result = await dialogManager.ShowAsync(config);
if (result.Submit)
    NavigateToLogin();
// else: user dismissed or timer expired — stay on current screen
```

Common use-cases: session-timeout dialogs, success acknowledgement with 3-second auto-dismiss, non-blocking update prompts.

**Note:** `AutoCloseTimeout = 0` (the default) disables the countdown entirely.

---

## Dedupe Key Naming Convention

| Pattern | Example |
|---------|---------|
| Feature pending | `"grid-export-excel-pending"` |
| Validation site | `"grid-filter-value-required"` |
| Clipboard error | `"grid-copy-error"`, `"grid-paste-error"` |
| Import/export | auto-generated from file name + count |

**Rules:**
- All lowercase, hyphen-separated.
- Prefix with the component/area name (`"grid-"`, `"filter-"`, `"nav-"`).
- Suffix describes the action and state (`"-pending"`, `"-required"`, `"-error"`).
- Use explicit stable keys at all known repeat-trigger sites (buttons, keyboard hooks).

---

## Anti-Patterns

| ❌ Avoid | ✅ Use instead |
|---------|--------------|
| `MessageBox.Show("Error: " + ex.Message)` | `ToastDeduped(...)` with `ToastType.Error` |
| `MessageBox.Show("Done!")` | `ToastSuccess(...)` |
| `MessageBox.Show("Not implemented yet")` | `NotifyFeaturePending(...)` |
| `Toast(...)` without dedupe at a repeat-trigger site | `ToastDeduped(...)` with a stable key |
| `NotifyFeaturePending` without a dedupe key on a keyboard handler | Always supply explicit `dedupeKey` for key-repeat-safe handlers |
| `durationMs: 0` for a success message | Use `durationMs: 3000` (success should auto-dismiss) |
| `durationMs: 3000` for an unhandled exception message | Use `durationMs: 0` (errors should persist) |
