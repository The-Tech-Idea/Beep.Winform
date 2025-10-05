# Wizard Manager - Quick Reference

## 🚀 Quick Start

```csharp
var config = new WizardConfig
{
    Title = "My Wizard",
    Style = WizardStyle.Modern,
    Steps = new List<WizardStep>
    {
        new WizardStep
        {
            Title = "Step 1",
            Content = new Step1Control()
        },
        new WizardStep
        {
            Title = "Step 2",
            Content = new Step2Control()
        }
    },
    OnComplete = (ctx) => MessageBox.Show("Done!")
};

WizardManager.ShowWizard(config);
```

## 📦 WizardContext - Data Management

### Set Value
```csharp
context.SetValue("key", value);
context.SetValue("user", new User { Name = "John" });
```

### Get Value
```csharp
var name = context.GetValue<string>("name");
var age = context.GetValue<int>("age", 18); // with default
var user = context.GetValue<User>("user");
```

### Check Exists
```csharp
if (context.ContainsKey("key")) { }
```

### Remove
```csharp
context.Remove("key");
```

### Clear All
```csharp
context.Clear();
```

## 🎨 Wizard Styles

| Style | Description | Use Case |
|-------|-------------|----------|
| `Modern` | Progress bar at top | Clean, modern apps |
| `Classic` | Left sidebar with steps | Traditional installers |
| `Stepper` | Horizontal numbered steps | Linear workflows |
| `VerticalStepper` | Timeline on left | Complex multi-step |
| `Cards` | Card selection | Non-linear choice |

## 📋 WizardStep Properties

```csharp
new WizardStep
{
    Key = "step1",                    // Unique identifier
    Title = "Step Title",              // Display name
    Description = "Step description",  // Subtitle
    Icon = "icons/step.svg",          // Icon path
    Content = new MyControl(),         // UserControl
    IsOptional = false,                // Can skip?
    CanNavigateNext = (ctx) => true,   // Validation
    CanNavigateBack = (ctx) => true,   // Allow back?
    OnEnter = (ctx) => { },            // Enter callback
    OnLeave = (ctx) => { }             // Leave callback
}
```

## 🎯 UserControl Implementation

```csharp
public class MyStepControl : UserControl, IWizardStepControl
{
    private WizardContext _context;
    
    public void SetWizardContext(WizardContext context)
    {
        _context = context;
        
        // Load data
        txtName.Text = _context.GetValue<string>("name", "");
    }
    
    private void SaveData()
    {
        // Save to context
        _context.SetValue("name", txtName.Text);
    }
}
```

## 🔧 Navigation Control

### CanNavigateNext - Validation
```csharp
CanNavigateNext = (ctx) =>
{
    // Return false to prevent navigation
    return !string.IsNullOrEmpty(ctx.GetValue<string>("name"));
}
```

### CanNavigateBack - Control Back Button
```csharp
CanNavigateBack = (ctx) =>
{
    // Prevent back after certain point
    return !ctx.GetValue<bool>("committed");
}
```

## 📊 Common Patterns

### Pattern 1: Simple Properties
```csharp
// Save
context.SetValue("username", "john");
context.SetValue("age", 25);

// Load
var username = context.GetValue<string>("username");
var age = context.GetValue<int>("age");
```

### Pattern 2: Single Root Object
```csharp
// Create model
public class WizardData
{
    public string Username { get; set; }
    public int Age { get; set; }
}

// Save
context.SetValue("data", new WizardData());

// Load
var data = context.GetValue<WizardData>("data");
data.Username = "john";
// No need to SetValue again (reference type)
```

### Pattern 3: Step Validation
```csharp
// In UserControl
private void ValidateStep()
{
    bool isValid = /* validation logic */;
    _context.StepValidation[stepIndex] = isValid;
}
```

## ⚙️ WizardConfig Options

```csharp
var config = new WizardConfig
{
    // Display
    Title = "Wizard Title",
    Description = "Description",
    Size = new Size(800, 600),
    Style = WizardStyle.Modern,
    Theme = myBeepTheme,
    
    // Behavior
    ShowProgressBar = true,
    ShowStepList = true,
    AllowCancel = true,
    AllowBack = true,
    AllowSkip = false,
    ShowHelp = false,
    HelpUrl = "https://help.example.com",
    
    // Button Text
    NextButtonText = "Next",
    BackButtonText = "Back",
    FinishButtonText = "Finish",
    CancelButtonText = "Cancel",
    
    // Events
    OnComplete = (ctx) => { },
    OnCancel = (ctx) => { },
    OnStepChanging = (newIndex, ctx) => true,
    OnStepChanged = (newIndex, ctx) => { },
    
    // Steps
    Steps = new List<WizardStep> { }
};
```

## 🎬 Callbacks

### OnComplete - Wizard Finished
```csharp
OnComplete = (ctx) =>
{
    var data = ctx.GetAllData();
    SaveToDatabase(data);
}
```

### OnCancel - User Cancelled
```csharp
OnCancel = (ctx) =>
{
    RollbackChanges();
}
```

### OnStepChanging - Before Navigation
```csharp
OnStepChanging = (newStepIndex, ctx) =>
{
    // Return false to prevent navigation
    if (!ValidateCurrentStep(ctx))
    {
        MessageBox.Show("Please complete all fields");
        return false;
    }
    return true;
}
```

### OnStepChanged - After Navigation
```csharp
OnStepChanged = (newStepIndex, ctx) =>
{
    Console.WriteLine($"Now on step {newStepIndex}");
}
```

## 🔍 Programmatic Navigation

```csharp
// Get wizard instance
var wizard = WizardManager.GetWizard(key);

// Navigate
wizard.NavigateNext();
wizard.NavigateBack();
wizard.NavigateToStep(2);

// Complete/Cancel
wizard.Complete();
wizard.Cancel();

// Access context
var context = wizard.Context;
context.SetValue("key", "value");
```

## ✅ Validation Examples

### Step-Level Validation
```csharp
new WizardStep
{
    CanNavigateNext = (ctx) =>
    {
        var email = ctx.GetValue<string>("email");
        return IsValidEmail(email);
    }
}
```

### Cross-Step Validation
```csharp
OnStepChanging = (newIndex, ctx) =>
{
    // Validate all previous steps
    for (int i = 0; i < newIndex; i++)
    {
        if (ctx.StepValidation.TryGetValue(i, out var isValid) && !isValid)
            return false;
    }
    return true;
}
```

### Global Validation
```csharp
OnComplete = (ctx) =>
{
    if (!ValidateAllSteps(ctx))
    {
        MessageBox.Show("Please complete all required fields");
        return;
    }
    
    // Proceed with completion
}
```

## 💡 Tips

1. **Always implement `IWizardStepControl`** for data injection
2. **Use strongly-typed context access** with `GetValue<T>()`
3. **Validate in `CanNavigateNext`** for immediate feedback
4. **Save data in `OnLeave`** to persist step data
5. **Use single root object** for complex data models
6. **Check `StepValidation`** dictionary for validation state
7. **Clean up context** in `OnComplete`/`OnCancel`

## 🚨 Common Mistakes

❌ **DON'T: Forget to implement IWizardStepControl**
```csharp
public class MyStep : UserControl { } // Won't get context!
```

✅ **DO: Implement the interface**
```csharp
public class MyStep : UserControl, IWizardStepControl
{
    public void SetWizardContext(WizardContext context) { }
}
```

❌ **DON'T: Cast context values loosely**
```csharp
var user = context.GetValue<object>("user") as User;
```

✅ **DO: Use strongly-typed access**
```csharp
var user = context.GetValue<User>("user");
```

❌ **DON'T: Forget to save data**
```csharp
// Data lost when navigating away
private void OnTextChanged(object sender, EventArgs e)
{
    // Not saved to context!
}
```

✅ **DO: Save to context**
```csharp
private void OnTextChanged(object sender, EventArgs e)
{
    _context.SetValue("name", txtName.Text);
}
```
