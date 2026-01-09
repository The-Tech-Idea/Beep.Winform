# Wizard Design-Time Guide

## Overview

The wizard system provides comprehensive design-time support through property editors, type converters, and action lists.

## Property Grid Configuration

### WizardConfig Properties

When a `WizardConfig` object is used in the property grid, you'll see:

- **Wizard Category**:
  - `Title`: Wizard title
  - `Description`: Wizard description
  - `Style`: Visual style (Modern, Classic, Stepper, etc.)
  - `ShowProgressBar`: Show progress indicator
  - `AllowCancel`: Allow cancel button
  - `AllowBack`: Allow back navigation
  - `StepCount`: Read-only count of steps

- **Buttons Category**:
  - `NextButtonText`: Text for Next button
  - `BackButtonText`: Text for Back button
  - `FinishButtonText`: Text for Finish button
  - `CancelButtonText`: Text for Cancel button

- **Steps Category**:
  - `Steps`: Collection editor for managing steps

### WizardStep Properties

Each step in the collection editor shows:

- **Step Category**:
  - `Key`: Unique identifier
  - `Title`: Step title
  - `Description`: Step description
  - `Icon`: SVG icon path
  - `IsOptional`: Whether step can be skipped
  - `IsCompleted`: Read-only completion status

### WizardContext Properties

The wizard context shows:

- **Navigation Category**:
  - `CurrentStepIndex`: Current step
  - `TotalSteps`: Total number of steps
  - `CompletionPercentage`: Read-only completion percentage
  - `StepsCompleted`: Read-only count of completed steps

- **Progress Category**:
  - `CompletionPercentage`: Completion percentage
  - `StepsCompleted`: Number of completed steps

## Smart Tags (Action Lists)

Right-click on a wizard configuration in the designer to see smart tags:

### Wizard Configuration Actions

- **Add Step**: Add a new step to the wizard
- **Remove Last Step**: Remove the last step
- **Clear All Steps**: Remove all steps
- **Preview Wizard**: Open wizard in a test dialog

## Collection Editor

### Managing Steps

1. Click the ellipsis (...) next to the `Steps` property
2. Use the collection editor to:
   - Add new steps
   - Remove steps
   - Reorder steps (drag and drop)
   - Configure step properties

### Step Configuration

In the collection editor, expand each step to configure:
- Title and description
- Icon path
- Optional flag
- Validators (programmatically)

## Type Converters

### Expandable Objects

- `WizardConfig`: Expandable in property grid
- `WizardStep`: Expandable in collection editor
- `WizardContext`: Expandable for inspection

## Best Practices

1. **Use the Collection Editor**: Manage steps visually in the designer
2. **Preview Often**: Use the Preview Wizard action to test your configuration
3. **Set Keys**: Always set unique keys for steps for programmatic access
4. **Configure Validators**: Add validators programmatically in code-behind

## Limitations

- Step content (UserControl) must be set programmatically
- Validators are configured in code, not in the designer
- Callbacks (OnComplete, OnCancel) are set in code
