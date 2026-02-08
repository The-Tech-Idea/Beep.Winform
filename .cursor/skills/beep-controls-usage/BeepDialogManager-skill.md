# BeepDialogManager Skill

## Overview
`BeepDialogManager` provides static methods for showing dialogs: messages, inputs, file pickers, progress, and notifications.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.DialogsManagers;
```

## Dialog Categories

### Core Dialogs (BeepDialogManager.Core)
```csharp
// Simple message
BeepDialogManager.ShowMessage("Title", "Message");

// Confirmation
bool result = BeepDialogManager.ShowConfirm("Delete?", "Are you sure?");

// Warning
BeepDialogManager.ShowWarning("Warning", "This action cannot be undone");

// Error
BeepDialogManager.ShowError("Error", "An error occurred");
```

### Input Dialogs (BeepDialogManager.Input)
```csharp
// Text input
string name = BeepDialogManager.ShowTextInput("Name", "Enter your name:");

// Number input
int? value = BeepDialogManager.ShowNumberInput("Quantity", "Enter amount:");

// Dropdown selection
string choice = BeepDialogManager.ShowDropdown("Select", options);

// Date picker
DateTime? date = BeepDialogManager.ShowDatePicker("Select Date");
```

### File Dialogs (BeepDialogManager.File)
```csharp
// Open file
string file = BeepDialogManager.ShowOpenFile("Open", "*.txt;*.csv");

// Save file
string savePath = BeepDialogManager.ShowSaveFile("Save As", "*.txt");

// Folder picker
string folder = BeepDialogManager.ShowFolderPicker("Select Folder");
```

### Progress Dialogs (BeepDialogManager.Progress)
```csharp
// Show progress
var progress = BeepDialogManager.ShowProgress("Loading", "Please wait...");

// Update progress
progress.SetProgress(50, "Halfway there...");

// Complete
progress.Close();
```

### Notifications (BeepDialogManager.Notifications)
```csharp
// Toast notifications
BeepDialogManager.ShowInfoToast("Information");
BeepDialogManager.ShowSuccessToast("Success!");
BeepDialogManager.ShowWarningToast("Warning");
BeepDialogManager.ShowErrorToast("Error occurred");
```

## Preset Dialogs (DialogManager.Presets)
| Preset | Description |
|--------|-------------|
| `ConfirmDelete` | Delete confirmation |
| `UnsavedChanges` | Save/Discard/Cancel |
| `ConnectionSettings` | Connection config |
| `LoginDialog` | Username/password |

## Related Controls
- `BeepDialogBox` - Modal dialog control
- `BeepNotification` - Toast notifications
