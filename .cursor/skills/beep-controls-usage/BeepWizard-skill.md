# BeepWizard Skill

## Overview
The Wizard system provides multi-step forms with 3 visual styles, step validation, navigation controls, and progress tracking.

## Implementation Rules
- Wizard forms must read visual colors from the active `IBeepTheme`.
- Wizard forms must resolve fonts through `BeepFontManager` instead of hardcoded `new Font(...)`.
- Step/form payloads should use strongly typed models; avoid `Dictionary<string, object>` and raw `object` values.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Wizards;
```

## WizardStyle (3 types)
```csharp
public enum WizardStyle
{
    HorizontalStepper,  // Icons at top, horizontal progress
    VerticalStepper,    // Timeline on left side
    Minimal             // Clean progress indicator
}
```

## WizardResult
```csharp
public enum WizardResult
{
    None,       // Still active
    Completed,  // Successfully finished
    Cancelled,  // User cancelled
    Failed      // Error occurred
}
```

## StepState
```csharp
public enum StepState
{
    Pending,    // Not visited
    Current,    // Currently active
    Completed,  // Finished
    Error,      // Validation error
    Skipped     // Optional, skipped
}
```

## Usage Example
```csharp
// Create wizard manager
var manager = new WizardManager();

// Define steps
var steps = new List<WizardStep>
{
    new WizardStep { Title = "Account", Description = "Enter account details" },
    new WizardStep { Title = "Profile", Description = "Set up your profile" },
    new WizardStep { Title = "Confirm", Description = "Review and confirm" }
};

// Show wizard
var result = await manager.ShowWizardAsync(steps, WizardStyle.HorizontalStepper);

if (result.Result == WizardResult.Completed)
{
    // Process collected data
    var data = result.CollectedData;
}
```

## Form Styles
- **HorizontalStepperWizardForm** - Horizontal stepper at top
- **VerticalStepperWizardForm** - Vertical timeline on left
- **MinimalWizardForm** - Clean, minimal progress

## Features
- Step validation with error states
- Optional/skippable steps
- Progress persistence
- Custom step controls
- Navigation (Next, Previous, Cancel)

## Related Controls
- `BeepDialogBox` - Simple dialogs
- `BeepStepper` - Standalone stepper
