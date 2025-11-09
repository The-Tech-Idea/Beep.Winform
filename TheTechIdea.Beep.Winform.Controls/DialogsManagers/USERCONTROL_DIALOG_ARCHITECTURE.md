# Dialog System Architecture - UserControl Support & BeepStyling Integration

## Overview

The dialog system has been refactored to:
1. **Use BeepStyling** for all colors, radius, padding (no custom color schemes)
2. **Semantic Presets** define intent (Success, Warning, Danger, etc.) not visual appearance
3. **UserControl Support** for custom dialog content with validation and data binding
4. **DialogResult** class for comprehensive return values and data extraction

---

## DialogPreset (Semantic Intent)

### Available Presets

| Preset | Purpose | Button Colors | Icon |
|--------|---------|---------------|------|
| `None` | No preset, use Style | From BeepControlStyle | Custom |
| `Information` | Neutral info | Primary (Info) | Information |
| `Success` | Positive confirmation | Success (Green) | Success |
| `Warning` | Caution message | Warning (Yellow) | Warning |
| `Danger` | Destructive action | Error (Red) | Error |
| `Question` | User choice | Primary (Blue) | Question |

### Key Changes from Old System

**Old (Wrong):**
```csharp
// Custom color palettes in DialogPresetColors
DialogPreset.MaterialPositive => new DialogColorScheme {
    Background = Color.FromArgb(212, 237, 218),
    PrimaryButton = Color.FromArgb(40, 167, 69),
    // ... hardcoded colors
}
```

**New (Correct):**
```csharp
// Semantic meaning only, colors from BeepStyling
DialogPreset.Success => new ButtonEmphasis {
    PrimaryIsSuccess = true, // Uses BeepStyling.GetThemeColor("success")
    UseFilledButtons = true
}
```

---

## DialogPresetStyling Class

### Purpose
Maps semantic presets to button emphasis and gets colors from BeepStyling.

### Key Methods

```csharp
// Get button emphasis configuration
ButtonEmphasis GetButtonEmphasis(DialogPreset preset)
{
    Success => PrimaryIsSuccess = true (green from theme)
    Danger  => PrimaryIsDanger = true (red from theme)
    Warning => PrimaryIsWarning = true (yellow from theme)
}

// Get colors from BeepStyling
Color GetPrimaryButtonColor(ButtonEmphasis emphasis, BeepControlStyle style)
{
    if (emphasis.PrimaryIsDanger)
        return BeepStyling.GetThemeColor("error");
    if (emphasis.PrimaryIsWarning)
        return BeepStyling.GetThemeColor("warning");
    if (emphasis.PrimaryIsSuccess)
        return BeepStyling.GetThemeColor("success");
    // ...
}

// All other properties from BeepStyling
int GetCornerRadius(BeepControlStyle style) => BeepStyling.GetRadius(style);
int GetPadding(BeepControlStyle style) => BeepStyling.GetPadding(style);
Color GetBackgroundColor(DialogPreset preset, BeepControlStyle style) => BeepStyling.GetBackgroundColor(style);
Color GetForegroundColor(BeepControlStyle style) => BeepStyling.GetForegroundColor(style);
Color GetBorderColor(BeepControlStyle style) => BeepStyling.GetBorderColor(style);
```

---

## DialogResult Class

### Purpose
Comprehensive return value from dialogs with button tracking, custom data, and validation.

### Properties

```csharp
public class DialogResult
{
    // Core result
    BeepDialogButtons ButtonClicked { get; set; }
    BeepDialogResult Result { get; set; }
    bool Cancel { get; set; }
    bool Success => !Cancel && (Result == OK || Result == Yes);

    // Custom data
    Dictionary<string, object> UserData { get; set; }
    Control CustomControl { get; set; }
    string InputValue { get; set; }
    object SelectedItem { get; set; }

    // Validation
    List<string> ValidationErrors { get; set; }
    bool IsValid => ValidationErrors.Count == 0;
}
```

### Helper Methods

```csharp
// Typed data access
T GetData<T>(string key, T defaultValue = default)
string username = result.GetData<string>("username");

// Set data
void SetData(string key, object value)
result.SetData("email", "[email protected]");

// Get control as specific type
T GetControl<T>() where T : Control
var loginControl = result.GetControl<LoginDialogControl>();

// Factory methods
DialogResult.Ok(inputValue)
DialogResult.Cancelled()
DialogResult.Yes()
DialogResult.WithControl(control, button)
```

---

## DialogConfig Enhancements

### UserControl Support

```csharp
// Custom control hosting
Control? CustomControl { get; set; }
bool CustomControlFillsDialog { get; set; } = false;
int CustomControlMinHeight { get; set; } = 100;
int CustomControlMaxHeight { get; set; } = 0;
int CustomControlPadding { get; set; } = 12;
```

### Button Customization

```csharp
// Custom button labels
Dictionary<BeepDialogButtons, string> CustomButtonLabels { get; set; }
// Example: { BeepDialogButtons.Ok, "Submit" }

// Custom button colors (overrides theme)
Dictionary<BeepDialogButtons, Color> CustomButtonColors { get; set; }

// Button order
BeepDialogButtons[]? ButtonOrder { get; set; }
// Example: new[] { BeepDialogButtons.Cancel, BeepDialogButtons.Ok }

// Button sizing
int MinButtonWidth { get; set; } = 80;
int ButtonHeight { get; set; } = 36;
int ButtonSpacing { get; set; } = 8;
```

### Validation & Data Binding

```csharp
// Validation callback
Func<DialogResult, bool>? ValidationCallback { get; set; }
// Return true to allow close, false to keep dialog open

// Data extraction callback
Action<DialogResult>? DataExtractionCallback { get; set; }
// Extract data from CustomControl into DialogResult.UserData

// Initialization callback
Action<Control>? InitializationCallback { get; set; }
// Called after CustomControl is added to dialog

bool ValidateOnButtonClick { get; set; } = true;
bool ShowValidationErrors { get; set; } = true;
```

### Factory Methods

```csharp
// Semantic presets
DialogConfig.CreateInformation(title, message)
DialogConfig.CreateSuccess(title, message)
DialogConfig.CreateWarning(title, message)
DialogConfig.CreateDanger(title, message)
DialogConfig.CreateQuestion(title, message)

// UserControl dialog
DialogConfig.CreateWithUserControl(title, userControl, buttons...)

// Obsolete (for backward compatibility)
[Obsolete] DialogConfig.CreateConfirmAction(...) // Use CreateQuestion
[Obsolete] DialogConfig.CreateSmoothPositive(...) // Use CreateSuccess
[Obsolete] DialogConfig.CreateSmoothDanger(...) // Use CreateDanger
```

---

## Example UserControls

### LoginDialogControl

```csharp
public class LoginDialogControl : UserControl
{
    public string Username => txtUsername.Text;
    public string Password => txtPassword.Text;
    public bool RememberMe => chkRememberMe.Checked;

    public bool Validate(out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            errorMessage = "Username is required.";
            return false;
        }
        if (Password.Length < 6)
        {
            errorMessage = "Password must be at least 6 characters.";
            return false;
        }
        errorMessage = string.Empty;
        return true;
    }
}
```

### FormInputDialogControl

```csharp
public class FormInputDialogControl : UserControl
{
    public class InputField
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public InputType Type { get; set; } // Text, Number, Date, Dropdown, etc.
        public bool Required { get; set; }
        public object DefaultValue { get; set; }
    }

    public void AddFields(params InputField[] fields)
    public object GetValue(string fieldName)
    public Dictionary<string, object> GetAllValues()
    public bool Validate(out List<string> errors)
}
```

---

## Usage Examples

### Simple Success Dialog

```csharp
var result = DialogManager.ShowDialog(
    DialogConfig.CreateSuccess(
        "Operation Complete",
        "Your data has been saved successfully."
    )
);

if (!result.Cancel)
{
    // User clicked OK
}
```

### Danger Confirmation

```csharp
var result = DialogManager.ShowDialog(
    DialogConfig.CreateDanger(
        "Delete All Data?",
        "This action cannot be undone. All data will be permanently deleted."
    )
);

if (result.ButtonClicked == BeepDialogButtons.Ok)
{
    // User confirmed deletion
    DeleteAllData();
}
```

### Custom UserControl Dialog

```csharp
var loginControl = new LoginDialogControl();

var config = DialogConfig.CreateWithUserControl(
    "Login",
    loginControl,
    BeepDialogButtons.Cancel,
    BeepDialogButtons.Ok
);

config.CustomButtonLabels[BeepDialogButtons.Ok] = "Sign In";
config.ValidationCallback = (result) =>
{
    var ctrl = result.GetControl<LoginDialogControl>();
    return ctrl.Validate(out string error);
};

config.DataExtractionCallback = (result) =>
{
    var ctrl = result.GetControl<LoginDialogControl>();
    result.SetData("username", ctrl.Username);
    result.SetData("password", ctrl.Password);
    result.SetData("remember", ctrl.RememberMe);
};

var result = DialogManager.ShowDialog(config);

if (result.Success)
{
    string username = result.GetData<string>("username");
    string password = result.GetData<string>("password");
    bool remember = result.GetData<bool>("remember");
    
    // Perform login
}
```

### Form Input Dialog

```csharp
var formControl = new FormInputDialogControl();
formControl.AddFields(
    new FormInputDialogControl.InputField
    {
        Name = "name",
        Label = "Full Name",
        Type = FormInputDialogControl.InputType.Text,
        Required = true
    },
    new FormInputDialogControl.InputField
    {
        Name = "email",
        Label = "Email Address",
        Type = FormInputDialogControl.InputType.Text,
        Required = true
    },
    new FormInputDialogControl.InputField
    {
        Name = "age",
        Label = "Age",
        Type = FormInputDialogControl.InputType.Number,
        Required = false
    }
);

var config = DialogConfig.CreateWithUserControl(
    "User Information",
    formControl,
    BeepDialogButtons.Cancel,
    BeepDialogButtons.Ok
);

config.ValidationCallback = (result) =>
{
    var ctrl = result.GetControl<FormInputDialogControl>();
    return ctrl.Validate(out var errors);
};

config.DataExtractionCallback = (result) =>
{
    var ctrl = result.GetControl<FormInputDialogControl>();
    var values = ctrl.GetAllValues();
    foreach (var kvp in values)
    {
        result.SetData(kvp.Key, kvp.Value);
    }
};

var result = DialogManager.ShowDialog(config);

if (result.Success)
{
    string name = result.GetData<string>("name");
    string email = result.GetData<string>("email");
    int age = result.GetData<int>("age");
}
```

---

## Icon Painting with StyledImagePainter

### Usage in Dialog Painters

```csharp
// In PresetDialogPainter.PaintIcon()
Color iconTint = DialogPresetStyling.GetIconTint(preset, currentStyle);

// Use StyledImagePainter with tint
StyledImagePainter.PaintWithTint(
    g,
    iconPath,
    iconPath,
    iconTint,
    opacity: 1.0f,
    cornerRadius: 0
);
```

### Available StyledImagePainter Methods

```csharp
// Basic painting
void Paint(Graphics g, GraphicsPath path, string imagePath)
void Paint(Graphics g, Rectangle bounds, string imagePath)

// With tint
void PaintWithTint(Graphics g, GraphicsPath path, string imagePath, Color tint, float opacity, int cornerRadius)

// Shape-specific
void PaintInCircle(Graphics g, float centerX, float centerY, float radius, string imagePath, Color? tint, float opacity)
void PaintInHexagon(...)
void PaintInStar(...)
// ... many more shapes
```

---

## Migration Guide

### From Old Custom Colors

**Before:**
```csharp
var config = DialogConfig.CreateSmoothPositive("Success", "Operation complete");
// Used hardcoded green background Color.FromArgb(76, 175, 80)
```

**After:**
```csharp
var config = DialogConfig.CreateSuccess("Success", "Operation complete");
// Uses BeepStyling.GetThemeColor("success") - adapts to theme
```

### From Style-Only to Preset

**Before:**
```csharp
var config = new DialogConfig
{
    Title = "Warning",
    Message = "Are you sure?",
    Style = BeepControlStyle.Material3,
    IconType = BeepDialogIcon.Warning
};
```

**After:**
```csharp
var config = DialogConfig.CreateWarning("Warning", "Are you sure?");
// Preset handles icon and button colors automatically
```

---

## Best Practices

1. **Use Semantic Presets** - `Success`, `Warning`, `Danger` instead of visual names
2. **Let BeepStyling Handle Colors** - Don't hardcode colors, use theme colors
3. **Validate UserControls** - Always provide `ValidationCallback` for custom controls
4. **Extract Data Properly** - Use `DataExtractionCallback` to populate `DialogResult.UserData`
5. **Check DialogResult.Success** - Don't just check `!Cancel`, use the Success property
6. **Leverage StyledImagePainter** - For icon rendering with proper theme integration

---

## Architecture Benefits

✅ **Consistency** - All dialogs use BeepStyling colors, matching the rest of the UI  
✅ **Theme Support** - Dialogs adapt when theme changes (light/dark mode)  
✅ **Semantic Clarity** - Presets express intent (Success/Danger) not appearance (Green/Red)  
✅ **Maintainability** - No duplicate color definitions, single source of truth (BeepStyling)  
✅ **Flexibility** - UserControl support for complex dialog content  
✅ **Type Safety** - DialogResult provides typed data access  
✅ **Validation** - Built-in validation support with error display  

---

## Files Modified/Created

### Created:
- `DialogResult.cs` - Comprehensive result class with data extraction
- `LoginDialogControl.cs` - Example login UserControl
- `FormInputDialogControl.cs` - Dynamic form builder UserControl

### Modified:
- `DialogPreset.cs` - Refactored to semantic presets, removed custom colors
- `DialogConfig.cs` - Added UserControl support, validation callbacks, button customization
- `DialogPresetStyling.cs` - New helper class using BeepStyling instead of custom palettes

---

## Next Steps

1. Update `DialogManager` with UserControl methods (`ShowWithUserControl`, `ShowFormDialog`)
2. Update `PresetDialogPainter` to use `DialogPresetStyling` instead of old `DialogPresetColors`
3. Create more example UserControls (Settings, Search, FileSelection)
4. Add unit tests for validation and data extraction
5. Document keyboard navigation for UserControl dialogs
