# BeepDialogBox Skill

## Overview
`BeepDialogBox` is a customizable dialog panel with title, icon, up to 3 action buttons, close button, and theme support.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Vis.Modules;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `TitleText` | `string` | Dialog title |
| `DialogResult` | `BeepDialogResult` | Result state |
| `PrimaryButtonText` | `string` | OK button text |
| `SecondaryButtonText` | `string` | Cancel button text |
| `PrimaryButtonColor` | `Color` | OK button color |
| `SecondaryButtonColor` | `Color` | Cancel button color |

## BeepDialogResult
| Value | Description |
|-------|-------------|
| `OK` | Primary confirmed |
| `Cancel` | Secondary cancelled |
| `Yes` / `No` | Yes/No responses |
| `Abort` / `Retry` / `Ignore` | Abort/Retry/Ignore |

## Usage Example
```csharp
var dialog = new BeepDialogBox
{
    TitleText = "Confirm Action",
    PrimaryButtonText = "Confirm",
    SecondaryButtonText = "Cancel",
    Size = new Size(400, 200)
};

dialog.PrimaryButtonClicked += (s, e) =>
{
    dialog.DialogResult = BeepDialogResult.OK;
    dialog.Hide();
};

dialog.SecondaryButtonClicked += (s, e) =>
{
    dialog.DialogResult = BeepDialogResult.Cancel;
    dialog.Hide();
};
```

## Events
| Event | Description |
|-------|-------------|
| `PrimaryButtonClicked` | Primary button clicked |
| `SecondaryButtonClicked` | Secondary button clicked |
| `ThirdButtonClicked` | Third button clicked |
| `CloseButtonClicked` | Close button clicked |

## Using BeepDialogManager (Recommended)
For simpler dialogs:
```csharp
var manager = new BeepDialogManager();
var result = await manager.ShowConfirmAsync("Confirm", "Are you sure?");
await manager.ShowInfoAsync("Info", "Message here");
await manager.ShowErrorAsync("Error", "Error occurred");
```

## Related Controls
- `BeepNotification` - Non-modal notifications
- `BeepWizard` - Multi-step dialogs
