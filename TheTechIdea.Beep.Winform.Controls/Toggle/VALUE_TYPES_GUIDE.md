# BeepToggle Value Types - Usage Guide

The BeepToggle control supports multiple value types beyond simple boolean ON/OFF. This allows you to bind the toggle to different data types and use it in various scenarios.

## Value Type Options

### 1. Boolean (Default)
```csharp
var toggle = new BeepToggle();
toggle.ValueType = BeepToggle.ToggleValueType.Boolean;
toggle.OnValue = true;
toggle.OffValue = false;

// Usage
toggle.Value = true;  // Sets toggle to ON
bool isEnabled = (bool)toggle.Value;
```

### 2. String (Custom)
```csharp
var toggle = new BeepToggle();
toggle.ValueType = BeepToggle.ToggleValueType.String;
toggle.OnValue = "Approved";
toggle.OffValue = "Rejected";
toggle.OnText = "✓ Approved";
toggle.OffText = "✗ Rejected";

// Usage
toggle.Value = "Approved";  // Sets to ON
string status = toggle.Value.ToString();
```

### 3. Numeric (0/1 or Custom)
```csharp
var toggle = new BeepToggle();
toggle.ValueType = BeepToggle.ToggleValueType.Numeric;
toggle.OnValue = 1;
toggle.OffValue = 0;

// Or custom numbers
toggle.OnValue = 100;
toggle.OffValue = 0;

// Usage
toggle.Value = 1;  // Sets to ON
int numericValue = toggle.GetValue<int>() ?? 0;
```

### 4. Yes/No
```csharp
var toggle = new BeepToggle();
toggle.ValueType = BeepToggle.ToggleValueType.YesNo;
// Automatically sets:
//   OnValue = "Yes", OffValue = "No"
//   OnText = "Yes", OffText = "No"

// Usage
toggle.Value = "Yes";
```

### 5. On/Off
```csharp
var toggle = new BeepToggle();
toggle.ValueType = BeepToggle.ToggleValueType.OnOff;
// Automatically sets:
//   OnValue = "On", OffValue = "Off"
//   OnText = "On", OffText = "Off"
```

### 6. Enabled/Disabled
```csharp
var toggle = new BeepToggle();
toggle.ValueType = BeepToggle.ToggleValueType.EnabledDisabled;
// Automatically sets:
//   OnValue = "Enabled", OffValue = "Disabled"
//   OnText = "Enabled", OffText = "Disabled"
```

### 7. Active/Inactive
```csharp
var toggle = new BeepToggle();
toggle.ValueType = BeepToggle.ToggleValueType.ActiveInactive;
// Automatically sets:
//   OnValue = "Active", OffValue = "Inactive"
//   OnText = "Active", OffText = "Inactive"
```

## Advanced Usage Examples

### Database Integration (Bit Fields)
```csharp
// For SQL Server BIT columns (0/1)
var toggle = new BeepToggle
{
    ValueType = BeepToggle.ToggleValueType.Numeric,
    OnValue = 1,
    OffValue = 0
};

// Binding
toggle.Value = dataRow["IsActive"];  // From database

// Saving
command.Parameters.AddWithValue("@IsActive", toggle.Value);
```

### Enum Values
```csharp
enum Status { Inactive = 0, Active = 1 }

var toggle = new BeepToggle
{
    ValueType = BeepToggle.ToggleValueType.Numeric,
    OnValue = (int)Status.Active,
    OffValue = (int)Status.Inactive
};

// Usage
toggle.Value = (int)Status.Active;
Status currentStatus = (Status)(toggle.GetValue<int>() ?? 0);
```

### Multi-Language Support
```csharp
var toggle = new BeepToggle
{
    ValueType = BeepToggle.ToggleValueType.String,
    OnValue = Resources.Status_Enabled,   // "Activé" (French)
    OffValue = Resources.Status_Disabled, // "Désactivé" (French)
    OnText = Resources.Status_Enabled,
    OffText = Resources.Status_Disabled
};
```

### Custom Business Values
```csharp
var subscriptionToggle = new BeepToggle
{
    ValueType = BeepToggle.ToggleValueType.String,
    OnValue = "PREMIUM",
    OffValue = "FREE",
    OnText = "Premium Plan",
    OffText = "Free Plan",
    OnColor = Color.Gold,
    OffColor = Color.Silver
};

// Usage
subscriptionToggle.Value = user.SubscriptionType;
```

## Data Binding

### Simple Binding
```csharp
// Bind to boolean property
toggle.DataBindings.Add("Value", dataSource, "IsEnabled");

// Bind to string property
toggle.DataBindings.Add("Value", dataSource, "Status");
```

### Event Handling
```csharp
toggle.ValueChanged += (sender, e) =>
{
    Console.WriteLine($"Value changed to: {toggle.Value}");
    
    // Get typed value
    if (toggle.ValueType == BeepToggle.ToggleValueType.Boolean)
    {
        bool boolValue = toggle.GetValue<bool>() ?? false;
        UpdateDatabase(boolValue);
    }
};

toggle.IsOnChanged += (sender, e) =>
{
    Console.WriteLine($"IsOn changed to: {toggle.IsOn}");
};
```

### Setting Values Programmatically
```csharp
// Method 1: Direct assignment
toggle.Value = "Yes";

// Method 2: Using IsOn (always works)
toggle.IsOn = true;

// Method 3: From string
toggle.SetValueFromString("Active");

// Method 4: Get display value
string display = toggle.GetDisplayValue();

// Method 5: Get typed value
int? numValue = toggle.GetValue<int>();
```

## Smart Value Detection

The toggle automatically detects common values:

```csharp
// These all set toggle to ON:
toggle.Value = true;
toggle.Value = "true";
toggle.Value = "True";
toggle.Value = "yes";
toggle.Value = "Yes";
toggle.Value = "on";
toggle.Value = "ON";
toggle.Value = "enabled";
toggle.Value = "active";
toggle.Value = 1;
toggle.Value = "1";

// These all set toggle to OFF:
toggle.Value = false;
toggle.Value = "false";
toggle.Value = "no";
toggle.Value = "off";
toggle.Value = "disabled";
toggle.Value = "inactive";
toggle.Value = 0;
toggle.Value = "0";
```

## Best Practices

1. **Set ValueType First**: Always set `ValueType` before setting `OnValue` and `OffValue`
2. **Match Types**: Ensure the values you assign match the ValueType
3. **Use GetValue<T>()**: For type-safe retrieval of values
4. **Listen to ValueChanged**: Use this event for data binding scenarios
5. **Null Handling**: The control treats null as OFF/false

## Common Patterns

### Feature Flag Toggle
```csharp
var featureToggle = new BeepToggle
{
    ValueType = BeepToggle.ToggleValueType.Boolean,
    OnText = "Enabled",
    OffText = "Disabled",
    ToggleStyle = ToggleStyle.MaterialPill
};
featureToggle.ValueChanged += (s, e) =>
{
    FeatureFlags.Set("NewFeature", (bool)featureToggle.Value);
};
```

### Status Indicator
```csharp
var statusToggle = new BeepToggle
{
    ValueType = BeepToggle.ToggleValueType.String,
    OnValue = "ONLINE",
    OffValue = "OFFLINE",
    OnColor = Color.LimeGreen,
    OffColor = Color.Gray,
    Enabled = false  // Read-only indicator
};
```

### Permission Toggle
```csharp
var permissionToggle = new BeepToggle
{
    ValueType = BeepToggle.ToggleValueType.String,
    OnValue = "ALLOWED",
    OffValue = "DENIED",
    OnText = "Allow",
    OffText = "Deny",
    ToggleStyle = ToggleStyle.ButtonStyle
};
```

## Migration from Old Code

### Before (Simple Boolean)
```csharp
// Old way - only boolean
checkBox.Checked = true;
bool value = checkBox.Checked;
```

### After (Flexible Values)
```csharp
// New way - any value type
toggle.ValueType = BeepToggle.ToggleValueType.YesNo;
toggle.Value = "Yes";
string value = toggle.Value.ToString();

// Or stay with boolean
toggle.ValueType = BeepToggle.ToggleValueType.Boolean;
toggle.IsOn = true;
bool boolValue = toggle.GetValue<bool>() ?? false;
```
