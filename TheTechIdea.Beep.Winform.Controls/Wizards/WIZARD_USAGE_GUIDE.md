# Wizard Usage Guide

## Overview

The Beep Wizard system provides a comprehensive framework for creating multi-step wizards in Windows Forms applications. It supports multiple visual styles, validation, animations, and design-time configuration.

## Quick Start

### Basic Wizard

```csharp
var config = new WizardConfig
{
    Title = "Setup Wizard",
    Description = "Configure your application",
    Style = WizardStyle.Modern,
    Steps = new List<WizardStep>
    {
        new WizardStep
        {
            Key = "step1",
            Title = "Welcome",
            Description = "Welcome to the setup wizard",
            Content = new WelcomeStepControl()
        },
        new WizardStep
        {
            Key = "step2",
            Title = "Configuration",
            Description = "Configure your settings",
            Content = new ConfigurationStepControl()
        },
        new WizardStep
        {
            Key = "step3",
            Title = "Complete",
            Description = "Setup complete",
            Content = new CompleteStepControl()
        }
    },
    OnComplete = (context) =>
    {
        // Handle completion
        var data = context.GetValue<string>("configData");
        MessageBox.Show($"Setup complete! Data: {data}");
    }
};

var result = WizardManager.ShowWizard(config);
```

## Wizard Styles

### Modern Style
Clean design with top progress bar:
```csharp
config.Style = WizardStyle.Modern;
```

### Classic Style
Traditional wizard with left sidebar:
```csharp
config.Style = WizardStyle.Classic;
```

### Stepper Style
Horizontal stepper with numbered steps:
```csharp
config.Style = WizardStyle.Stepper;
```

### Vertical Stepper Style
Vertical timeline stepper:
```csharp
config.Style = WizardStyle.VerticalStepper;
```

### Cards Style
Card-based step selection:
```csharp
config.Style = WizardStyle.Cards;
```

## Step Validation

### Using Validators

```csharp
var step = new WizardStep
{
    Key = "userInfo",
    Title = "User Information",
    Validators = new List<IWizardStepValidator>
    {
        new RequiredFieldValidator("username", "Username is required"),
        new RequiredFieldValidator("email", "Email is required"),
        new RegexValidator("email", @"^[^@\s]+@[^@\s]+\.[^@\s]+$", "Invalid email format")
    }
};
```

### Custom Validation

```csharp
step.Validators.Add(new CustomValidator((context, step) =>
{
    var password = context.GetValue<string>("password");
    var confirmPassword = context.GetValue<string>("confirmPassword");
    
    if (password != confirmPassword)
    {
        return WizardValidationResult.Failure("Passwords do not match", "confirmPassword");
    }
    
    return WizardValidationResult.Success();
}));
```

## Step Templates

### Form Step Template

```csharp
var formTemplate = new FormStepTemplate(
    "userInfo",
    "User Information",
    "Enter your details",
    new List<FormField>
    {
        new FormField { Key = "username", Label = "Username", FieldType = FormFieldType.Text, IsRequired = true },
        new FormField { Key = "email", Label = "Email", FieldType = FormFieldType.Email, IsRequired = true }
    }
);

var step = formTemplate.GetStepConfig();
step.Content = formTemplate.CreateStepControl();
```

### Confirmation Step Template

```csharp
var confirmTemplate = new ConfirmationStepTemplate(
    "confirm",
    "Confirm",
    "Are you sure you want to proceed?",
    "This action cannot be undone."
);

var step = confirmTemplate.GetStepConfig();
step.Content = confirmTemplate.CreateStepControl();
```

### Summary Step Template

```csharp
var summaryTemplate = new SummaryStepTemplate(
    "summary",
    "Review",
    new List<SummaryItem>
    {
        new SummaryItem { Label = "Username", DataKey = "username" },
        new SummaryItem { Label = "Email", DataKey = "email" }
    }
);

var step = summaryTemplate.GetStepConfig();
step.Content = summaryTemplate.CreateStepControl();
```

## Data Context

### Setting Values

```csharp
context.SetValue("username", "john_doe");
context.SetValue("email", "john@example.com");
context.SetValue("age", 30);
```

### Getting Values

```csharp
var username = context.GetValue<string>("username");
var email = context.GetValue<string>("email");
var age = context.GetValue<int>("age", 0);
```

### Step-Specific Data

```csharp
// Set step-specific data
context.SetStepDataValue("step1", "field1", "value1");

// Get step-specific data
var value = context.GetStepDataValue<string>("step1", "field1");
```

## Navigation

### Programmatic Navigation

```csharp
var instance = WizardManager.CreateWizard(config);

// Navigate to next step
instance.NavigateNext();

// Navigate to previous step
instance.NavigateBack();

// Navigate to specific step
instance.NavigateToStep(2);
```

### Navigation Events

```csharp
config.OnStepChanging = (stepIndex, context) =>
{
    // Allow or prevent navigation
    return true; // Return false to prevent navigation
};

config.OnStepChanged = (stepIndex, context) =>
{
    // Handle step change
    MessageBox.Show($"Now on step {stepIndex + 1}");
};
```

## Animations

Animations are enabled by default. To disable:

```csharp
WizardManager.EnableAnimations = false;
```

## Localization

### Setting Up Localization

```csharp
// Set resource manager
var rm = new ResourceManager("YourApp.WizardResources", typeof(YourForm).Assembly);
WizardLocalizer.SetResourceManager(rm);

// Set culture
WizardLocalizer.SetCulture(new CultureInfo("fr-FR"));
```

## Best Practices

1. **Use strongly-typed models**: Prefer `context.GetValue<T>()` over dictionary access
2. **Validate early**: Add validators to steps to prevent invalid data
3. **Use templates**: Leverage step templates for common scenarios
4. **Handle errors**: Always check validation results before allowing navigation
5. **Extract data on leave**: Use `OnLeave` callbacks to extract data from step controls

## Examples

See the example wizards in the `Examples` directory for complete implementations.
