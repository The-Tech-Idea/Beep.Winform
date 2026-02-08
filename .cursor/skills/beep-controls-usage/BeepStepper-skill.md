# BeepStepper Skill

## Overview
`BeepStepperBreadCrumb` is a wizard-style stepper with chevron-shaped steps, tooltips, accessibility support, and animations.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Models;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `ListItems` | `BindingList<SimpleItem>` | Steps to display |
| `SelectedIndex` | `int` | Current step index |
| `SelectedItem` | `SimpleItem` | Current step item |
| `Orientation` | `Orientation` | Horizontal/Vertical |
| `Direction` | `ChevronDirection` | Chevron arrow direction |
| `CheckImage` | `string` | Image for completed steps |
| `AutoGenerateTooltips` | `bool` | Auto-generate step tooltips (true) |
| `AccessibleName` | `string` | Screen reader name |
| `AccessibleDescription` | `string` | Screen reader description |

## ChevronDirection
```csharp
public enum ChevronDirection
{
    Forward,   // ▶ pointing right/down
    Backward   // ◀ pointing left/up
}
```

## StepState
Steps are colored based on state:
- **Completed** - Filled with `StepCompletedColor`
- **Active** - Current step with `StepActiveColor`
- **Pending** - Future steps with `StepPendingColor`
- **Error** / **Warning** - Special states

## Usage Examples

### Basic Stepper
```csharp
var stepper = new BeepStepperBreadCrumb
{
    Orientation = Orientation.Horizontal
};

stepper.ListItems.Add(new SimpleItem { Name = "Step 1", Text = "Personal Info" });
stepper.ListItems.Add(new SimpleItem { Name = "Step 2", Text = "Address" });
stepper.ListItems.Add(new SimpleItem { Name = "Step 3", Text = "Confirm" });

stepper.SelectedItemChanged += (s, e) =>
{
    Console.WriteLine($"Now at: {e.SelectedItem?.Name}");
};
```

### Vertical Stepper
```csharp
var stepper = new BeepStepperBreadCrumb
{
    Orientation = Orientation.Vertical,
    Height = 300
};
```

### Custom Step Tooltips
```csharp
stepper.SetStepTooltip(0, "Enter your name and email");
stepper.SetStepTooltip(1, "Enter your shipping address");
stepper.SetStepTooltip(2, "Review and confirm your order");
```

### Update Step Progress
```csharp
// Mark step as completed
stepper.UpdateCurrentStep(1);  // Sets step 0 and 1 as checked

// Or update via item
stepper.UpdateCheckedState(item);
```

## Events
| Event | Description |
|-------|-------------|
| `SelectedItemChanged` | Step clicked/changed |

## Methods
| Method | Description |
|--------|-------------|
| `SetStepTooltip(index, text)` | Set custom tooltip |
| `GetStepTooltip(index)` | Get tooltip text |
| `UpdateCurrentStep(index)` | Mark step as current |
| `UpdateCheckedState(item)` | Mark item as checked |

## Accessibility Features
- Automatically applies ARIA settings
- High contrast mode support
- Reduced motion respects system settings
- Accessible minimum size enforced

## Theme Helpers
Uses `StepperThemeHelpers` for consistent colors:
- `GetStepCompletedColor()`, `GetStepActiveColor()`, `GetStepPendingColor()`
- `GetStepLabelColor()`, `GetStepBackgroundColor()`
