# HOWTO: Using BeepDialogManager and Toasts

This guide shows common recipes for dialogs and toasts.

Prerequisites
- Your Form must be a `BeepiForm` (or implement `IBeepModernFormHost`).
- Get the manager for the form via any child control: `var mgr = BeepDialogManager.For(this);`

Quick message dialog
```csharp
var mgr = BeepDialogManager.For(this);
await mgr.MessageAsync("Hello", "Operation completed");
```

Confirm dialog
```csharp
var mgr = BeepDialogManager.For(this);
var res = await mgr.ConfirmAsync(
    title: "Delete",
    message: "Are you sure you want to delete this item?",
    okText: "Delete",
    cancelText: "Cancel",
    options: new BeepDialogOptions { Kind = BeepDialogKind.Centered, DismissOnOverlayClick = false }
);
if (res == BeepDialogResult.OK)
{
    // proceed
}
```

Drawer / sheet dialogs
```csharp
var mgr = BeepDialogManager.For(this);
var options = new BeepDialogOptions { Kind = BeepDialogKind.RightDrawer, EscToClose = true };
var content = new BeepDialogContent
{
    Title = "Filters",
    Message = "Set your filters.",
    Buttons = new[]
    {
        new BeepDialogButton { Text = "Cancel", Result = BeepDialogResult.Cancel, IsCancel = true },
        new BeepDialogButton { Text = "Apply", Result = BeepDialogResult.OK, IsDefault = true }
    }
};
var result = await mgr.ShowAsync(content, options);
```

Custom body + validation
```csharp
var mgr = BeepDialogManager.For(this);
var tb = new TextBox { Dock = DockStyle.Top };
var body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };
body.Controls.Add(tb);

var content = new BeepDialogContent
{
    Title = "Enter name",
    CustomBody = body,
    ValidationRoot = body, // wire changes automatically
    Validator = () =>
    {
        var s = new ValidationState();
        if (string.IsNullOrWhiteSpace(tb.Text)) s.Errors.Add("Name is required");
        s.IsValid = s.Errors.Count == 0; return s;
    },
    Buttons = new[]
    {
        new BeepDialogButton { Text = "Cancel", Result = BeepDialogResult.Cancel, IsCancel = true },
        new BeepDialogButton { Text = "OK", Result = BeepDialogResult.OK, IsDefault = true }
    }
};
var res = await mgr.ShowAsync(content, new BeepDialogOptions { Kind = BeepDialogKind.Centered });
```

Progress wrapper (run async task with cancellable dialog)
```csharp
var mgr = BeepDialogManager.For(this);
bool ok = await mgr.RunWithProgressAsync(
    title: "Importing...",
    operation: async (progress, ct) =>
    {
        for (int i = 0; i <= 100; i++)
        {
            ct.ThrowIfCancellationRequested();
            progress.Report(i);
            await Task.Delay(50, ct);
        }
    },
    cancelText: "Stop",
    options: new BeepDialogOptions { Kind = BeepDialogKind.Centered }
);
```

Toasts
```csharp
var mgr = BeepDialogManager.For(this);
mgr.ShowToast(new BeepToastOptions
{
    Title = "Saved",
    Message = "Your changes have been saved.",
    Kind = BeepToastKind.Success,
    DurationMs = 3000
});

// With action
mgr.ShowToast(new BeepToastOptions
{
    Title = "Undo",
    Message = "Item deleted.",
    Kind = BeepToastKind.Info,
    ActionText = "Undo",
    Action = () => UndoDelete(),
    DurationMs = 6000
});
```

Tips
- Use `DismissOnOverlayClick` to allow clicking outside to close.
- `EscToClose` controls Esc behavior; default is true.
- For drawers/sheets the manager computes proper start/end bounds and animates.
- The manager resizes/relayouts dialogs and toasts on host form resize automatically.
- Theme changes are handled by `BeepDialogManager.Theme` partial; dialogs/toasts update live.
