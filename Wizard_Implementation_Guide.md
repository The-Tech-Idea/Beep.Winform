# Wizard Manager - Complete Implementation Guide

## 📚 Overview

The WizardManager system provides a complete solution for multi-step processes in WinForms applications. It follows the same architecture as ToolTipManager with painters, helpers, and context management.

## 🎯 Key Concept: WizardContext for Data Management

The **WizardContext** is the heart of data management in wizards. It provides:

### ✅ **Strongly-typed Data Access**
```csharp
// Set values
context.SetValue("username", "john.doe");
context.SetValue("age", 25);
context.SetValue("settings", new UserSettings { ... });

// Get values with type safety
var username = context.GetValue<string>("username");
var age = context.GetValue<int>("age", 18); // with default
var settings = context.GetValue<UserSettings>("settings");
```

### ✅ **Validation State Tracking**
```csharp
// Mark step as valid/invalid
context.StepValidation[stepIndex] = isValid;

// Check if step is valid
if (context.StepValidation.TryGetValue(stepIndex, out var isValid) && isValid)
{
    // Step is validated
}
```

### ✅ **Navigation History**
```csharp
// Automatically managed by WizardInstance
// Enables smart back navigation
context.NavigationHistory.Push(previousStepIndex);
```

## 📋 Architecture

### File Structure
```
Wizards/
├── WizardManager.cs              # Static manager (like ToolTipManager)
├── WizardInstance.cs             # Instance managing single wizard
├── Helpers/
│   └── WizardHelpers.cs          # Drawing and layout helpers
└── Forms/
    ├── BaseWizardForm.cs         # Base class for all wizard forms
    ├── ModernWizardForm.cs       # Clean with progress bar
    ├── ClassicWizardForm.cs      # Traditional with sidebar
    ├── StepperWizardForm.cs      # Horizontal stepper
    ├── VerticalStepperWizardForm.cs  # Vertical timeline
    └── CardsWizardForm.cs        # Card-based selection
```

### Data Flow
```
User Code
    ↓
WizardManager.CreateWizard(config)
    ↓
WizardInstance (manages WizardContext)
    ↓
BaseWizardForm (renders UI based on style)
    ↓
UserControl (step content) implements IWizardStepControl
    ↓
Access WizardContext via SetWizardContext()
```

## 🚀 Usage Examples

### Example 1: Simple Installation Wizard

```csharp
// Create wizard configuration
var config = new WizardConfig
{
    Title = "Software Installation",
    Description = "Install Beep Application",
    Size = new Size(800, 600),
    Style = WizardStyle.Modern,
    Theme = myBeepTheme,
    
    Steps = new List<WizardStep>
    {
        new WizardStep
        {
            Title = "Welcome",
            Description = "Welcome to the installation wizard",
            Content = new WelcomeStepControl(),
            OnEnter = (ctx) =>
            {
                // Initialize context
                ctx.SetValue("installPath", @"C:\Program Files\MyApp");
            }
        },
        new WizardStep
        {
            Title = "License Agreement",
            Description = "Please read and accept the license",
            Content = new LicenseStepControl(),
            CanNavigateNext = (ctx) =>
            {
                // Can only proceed if license is accepted
                return ctx.GetValue<bool>("licenseAccepted", false);
            }
        },
        new WizardStep
        {
            Title = "Installation Settings",
            Description = "Choose installation options",
            Content = new SettingsStepControl(),
            OnLeave = (ctx) =>
            {
                // Validate settings before leaving
                var path = ctx.GetValue<string>("installPath");
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    MessageBox.Show("Invalid installation path");
                }
            }
        },
        new WizardStep
        {
            Title = "Installing",
            Description = "Please wait while files are copied",
            Content = new InstallationProgressControl()
        },
        new WizardStep
        {
            Title = "Complete",
            Description = "Installation completed successfully",
            Content = new CompletionStepControl()
        }
    },
    
    OnComplete = (ctx) =>
    {
        // Installation complete
        var installPath = ctx.GetValue<string>("installPath");
        var createShortcut = ctx.GetValue<bool>("createShortcut");
        
        // Perform installation...
        MessageBox.Show($"Installed to: {installPath}");
    },
    
    OnCancel = (ctx) =>
    {
        // Rollback any changes
        MessageBox.Show("Installation cancelled");
    }
};

// Show wizard as modal dialog
var result = WizardManager.ShowWizard(config);
```

### Example 2: Data Collection Wizard with Context

```csharp
// Step 1 UserControl
public class Step1PersonalInfo : UserControl, IWizardStepControl
{
    private WizardContext _context;
    private TextBox txtName;
    private TextBox txtEmail;
    
    public void SetWizardContext(WizardContext context)
    {
        _context = context;
        
        // Load existing data if available
        txtName.Text = _context.GetValue<string>("name", "");
        txtEmail.Text = _context.GetValue<string>("email", "");
    }
    
    private void SaveData()
    {
        // Save to context
        _context.SetValue("name", txtName.Text);
        _context.SetValue("email", txtEmail.Text);
        
        // Mark as validated
        _context.StepValidation[0] = !string.IsNullOrEmpty(txtName.Text);
    }
}

// Step 2 UserControl
public class Step2Preferences : UserControl, IWizardStepControl
{
    private WizardContext _context;
    private CheckBox chkNewsletter;
    private ComboBox cboTheme;
    
    public void SetWizardContext(WizardContext context)
    {
        _context = context;
        
        // Access data from previous step
        var name = _context.GetValue<string>("name");
        lblWelcome.Text = $"Welcome, {name}!";
        
        // Load saved preferences
        chkNewsletter.Checked = _context.GetValue<bool>("newsletter", true);
        cboTheme.SelectedIndex = _context.GetValue<int>("themeIndex", 0);
    }
    
    private void SaveData()
    {
        _context.SetValue("newsletter", chkNewsletter.Checked);
        _context.SetValue("themeIndex", cboTheme.SelectedIndex);
    }
}

// Step 3 Review
public class Step3Review : UserControl, IWizardStepControl
{
    public void SetWizardContext(WizardContext context)
    {
        // Display all collected data
        var name = context.GetValue<string>("name");
        var email = context.GetValue<string>("email");
        var newsletter = context.GetValue<bool>("newsletter");
        
        lblSummary.Text = $@"
Name: {name}
Email: {email}
Newsletter: {(newsletter ? "Yes" : "No")}
        ";
    }
}
```

### Example 3: Complex Data with Custom Objects

```csharp
// Define your data model
public class UserRegistration
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserPreferences Preferences { get; set; }
    public List<string> SelectedFeatures { get; set; }
}

public class UserPreferences
{
    public string Theme { get; set; }
    public string Language { get; set; }
    public bool EmailNotifications { get; set; }
}

// Use in wizard
var config = new WizardConfig
{
    Steps = new List<WizardStep>
    {
        new WizardStep
        {
            Title = "Account Info",
            Content = new AccountInfoStep(),
            OnEnter = (ctx) =>
            {
                // Initialize data model
                if (!ctx.ContainsKey("registration"))
                {
                    ctx.SetValue("registration", new UserRegistration
                    {
                        Preferences = new UserPreferences(),
                        SelectedFeatures = new List<string>()
                    });
                }
            }
        },
        new WizardStep
        {
            Title = "Preferences",
            Content = new PreferencesStep(),
            CanNavigateNext = (ctx) =>
            {
                var reg = ctx.GetValue<UserRegistration>("registration");
                return reg.Preferences != null && 
                       !string.IsNullOrEmpty(reg.Preferences.Theme);
            }
        },
        new WizardStep
        {
            Title = "Features",
            Content = new FeaturesStep()
        }
    },
    
    OnComplete = (ctx) =>
    {
        // Get completed registration
        var registration = ctx.GetValue<UserRegistration>("registration");
        
        // Submit to API
        await SubmitRegistration(registration);
    }
};

// In UserControl
public class PreferencesStep : UserControl, IWizardStepControl
{
    private UserRegistration _registration;
    
    public void SetWizardContext(WizardContext context)
    {
        _registration = context.GetValue<UserRegistration>("registration");
        
        // Bind to UI
        cboTheme.SelectedItem = _registration.Preferences.Theme;
        cboLanguage.SelectedItem = _registration.Preferences.Language;
    }
    
    private void OnThemeChanged(object sender, EventArgs e)
    {
        // Update in-memory object (context already has reference)
        _registration.Preferences.Theme = cboTheme.SelectedItem.ToString();
    }
}
```

## 🎨 Wizard Styles

### 1. Modern (Default)
Clean design with top progress bar
```csharp
Style = WizardStyle.Modern
```

### 2. Classic
Traditional wizard with left sidebar showing all steps
```csharp
Style = WizardStyle.Classic
```

### 3. Stepper
Horizontal numbered stepper at top
```csharp
Style = WizardStyle.Stepper
```

### 4. VerticalStepper
Timeline-style vertical stepper on left
```csharp
Style = WizardStyle.VerticalStepper
```

### 5. Cards
Card-based step selection
```csharp
Style = WizardStyle.Cards
```

## 🔧 Advanced Features

### Conditional Navigation

```csharp
new WizardStep
{
    Title = "Payment",
    CanNavigateNext = (ctx) =>
    {
        var isPaid = ctx.GetValue<bool>("paymentComplete");
        if (!isPaid)
        {
            MessageBox.Show("Please complete payment");
            return false;
        }
        return true;
    },
    CanNavigateBack = (ctx) =>
    {
        // Prevent going back after payment
        return !ctx.GetValue<bool>("paymentComplete");
    }
}
```

### Dynamic Step Skipping

```csharp
config.AllowSkip = true;

// In step
new WizardStep
{
    Title = "Optional Settings",
    IsOptional = true,
    Content = new OptionalSettingsControl()
}
```

### Step-level Callbacks

```csharp
new WizardStep
{
    OnEnter = (ctx) =>
    {
        // Called when entering step
        Console.WriteLine("Entering step");
    },
    OnLeave = (ctx) =>
    {
        // Called when leaving step
        Console.WriteLine("Leaving step");
    }
}
```

### Global Navigation Events

```csharp
config.OnStepChanging = (newStepIndex, ctx) =>
{
    // Called before step changes
    // Return false to cancel navigation
    return ValidateCurrentStep(ctx);
};

config.OnStepChanged = (newStepIndex, ctx) =>
{
    // Called after step changes
    LogStepChange(newStepIndex);
};
```

## 💡 Best Practices

### 1. **Always Implement IWizardStepControl**
```csharp
public class MyStepControl : UserControl, IWizardStepControl
{
    private WizardContext _context;
    
    public void SetWizardContext(WizardContext context)
    {
        _context = context;
        LoadData();
    }
}
```

### 2. **Use Strongly-Typed Context Access**
```csharp
// Good
var user = context.GetValue<User>("user");

// Avoid
var user = context.GetValue<object>("user") as User;
```

### 3. **Validate in CanNavigateNext**
```csharp
CanNavigateNext = (ctx) =>
{
    var data = ctx.GetValue<MyData>("data");
    return data != null && data.IsValid();
}
```

### 4. **Clean Up in OnComplete/OnCancel**
```csharp
OnComplete = (ctx) =>
{
    // Save data
    SaveToDatabase(ctx.GetAllData());
    ctx.Clear(); // Clean up
};
```

### 5. **Use OnLeave for Validation**
```csharp
OnLeave = (ctx) =>
{
    // Validate and save before leaving step
    SaveCurrentStepData(ctx);
}
```

## 📊 Context Data Patterns

### Pattern 1: Single Root Object
```csharp
// Best for complex wizards
context.SetValue("wizardData", new WizardDataModel());
```

### Pattern 2: Individual Properties
```csharp
// Best for simple wizards
context.SetValue("name", "John");
context.SetValue("age", 25);
```

### Pattern 3: Step-Scoped Data
```csharp
// Namespace data by step
context.SetValue("step1.username", "john");
context.SetValue("step2.preferences", prefs);
```

## 🎯 Complete Working Example

See the next file for a full working example with all steps implemented.
