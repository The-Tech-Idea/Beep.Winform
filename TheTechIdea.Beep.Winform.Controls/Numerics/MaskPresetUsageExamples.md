# NumericMaskPreset Usage Examples

## Overview
The `BeepNumericUpDown` control now supports **38 predefined mask presets** for common input scenarios, with automatic icon integration and validation.

## Quick Start

### Basic Usage
```csharp
// Phone number with icon
var phoneControl = new BeepNumericUpDown
{
    MaskPreset = NumericMaskPreset.PhoneUS,
    NumericStyle = NumericStyle.Phone
};
// Auto-configures: (###) ###-#### format, phone icon, validation

// Currency with credit card icon
var currencyControl = new BeepNumericUpDown
{
    MaskPreset = NumericMaskPreset.Currency,
    NumericStyle = NumericStyle.Currency
};
// Auto-configures: $#,###.## format, dollar icon, 2 decimal places

// Temperature with thermometer icon
var tempControl = new BeepNumericUpDown
{
    MaskPreset = NumericMaskPreset.TemperatureCelsius,
    NumericStyle = NumericStyle.Standard
};
// Auto-configures: ###.# °C format, thermometer icon
```

## Available Mask Presets

### 📞 Contact & Identity
| Preset | Format | Icon | Use Case |
|--------|--------|------|----------|
| `PhoneUS` | (###) ###-#### | 📱 Phone | US phone numbers |
| `PhoneInternational` | +## (###) ###-#### | 📱 Phone | International phones |
| `SSN` | ###-##-#### | 🆔 IdCard | Social Security |
| `EIN` | ##-####### | 🏢 Building | Tax ID |

### 💳 Financial
| Preset | Format | Icon | Use Case |
|--------|--------|------|----------|
| `Currency` | $#,###.## | 💵 DollarSign | Money amounts |
| `CreditCard` | #### #### #### #### | 💳 CreditCard | Card numbers |
| `AccountNumber` | ####-####-#### | 💳 CreditCard | Bank accounts |
| `RoutingNumber` | ######### | 🏢 Building | Bank routing |

### 📍 Location
| Preset | Format | Icon | Use Case |
|--------|--------|------|----------|
| `ZipCode` | ##### | 📍 MapPin | 5-digit ZIP |
| `ZipCodePlus4` | #####-#### | 📍 MapPin | ZIP+4 |
| `Latitude` | ##.###### | 📍 MapPin | GPS coordinates |
| `Longitude` | ###.###### | 📍 MapPin | GPS coordinates |

### 📅 Date & Time
| Preset | Format | Icon | Use Case |
|--------|--------|------|----------|
| `Time24Hour` | ##:## | 🕐 Clock | 24-hour time |
| `Time12Hour` | ##:## AM | 🕐 Clock | 12-hour time |
| `DateMMDDYYYY` | ##/##/#### | 📅 Calendar | US date format |
| `DateYYYYMMDD` | ####-##-## | 📅 Calendar | ISO date format |

### 🌐 Network
| Preset | Format | Icon | Use Case |
|--------|--------|------|----------|
| `IPAddress` | ###.###.###.### | 🌐 Globe | IPv4 addresses |
| `MACAddress` | ##:##:##:##:##:## | 📡 Wifi | MAC addresses |

### 📊 Measurements
| Preset | Format | Icon | Use Case |
|--------|--------|------|----------|
| `WeightKg` | #,###.## kg | ⚖️ Weight | Kilograms |
| `WeightLbs` | #,###.## lbs | ⚖️ Weight | Pounds |
| `TemperatureCelsius` | ###.# °C | 🌡️ Thermometer | Celsius temp |
| `TemperatureFahrenheit` | ###.# °F | 🌡️ Thermometer | Fahrenheit temp |
| `DistanceKm` | #,###.## km | 🧭 Navigation | Kilometers |
| `DistanceMiles` | #,###.## mi | 🧭 Navigation | Miles |
| `FileSizeMB` | #,###.## MB | 💾 HardDrive | Megabytes |
| `FileSizeGB` | #,###.## GB | 💾 HardDrive | Gigabytes |

### 🔢 Numeric
| Preset | Format | Icon | Use Case |
|--------|--------|------|----------|
| `Percentage` | ##.##% | % Percent | Percentages |
| `Decimal2Places` | #,###.## | # Hash | Money, precise values |
| `Decimal4Places` | #,###.#### | # Hash | Scientific data |
| `IntegerWithCommas` | #,### | # Hash | Large integers |
| `Scientific` | #.####E+## | ⚛️ Atom | Scientific notation |
| `Hexadecimal` | 0x######## | # Hash | Hex values |
| `Binary` | 0b######## | # Hash | Binary values |

## Advanced Usage

### Combining with NumericStyle
```csharp
// Phone input with country dropdown
var phoneControl = new BeepNumericUpDown
{
    MaskPreset = NumericMaskPreset.PhoneInternational,
    NumericStyle = NumericStyle.Phone,  // Adds country code dropdown
    BeepControlStyle = BeepControlStyle.Material3
};

// Currency with stepper buttons
var priceControl = new BeepNumericUpDown
{
    MaskPreset = NumericMaskPreset.Currency,
    NumericStyle = NumericStyle.CompactStepper,  // Vertical +/- buttons
    MinimumValue = 0,
    MaximumValue = 999999,
    IncrementValue = 0.01m
};

// Temperature with slider
var tempSlider = new BeepNumericUpDown
{
    MaskPreset = NumericMaskPreset.TemperatureFahrenheit,
    NumericStyle = NumericStyle.Slider,
    MinimumValue = 32,
    MaximumValue = 212
};
```

### Custom Masks
```csharp
var customControl = new BeepNumericUpDown
{
    MaskPreset = NumericMaskPreset.Custom,
    CustomMask = "###-###-####",  // Custom pattern
    MinimumValue = 0,
    MaximumValue = 9999999999
};
```

### Validation Helpers
```csharp
var control = new BeepNumericUpDown
{
    MaskPreset = NumericMaskPreset.SSN
};

// Manual validation
if (control.ValidateRange(123456789, out string error))
{
    Console.WriteLine("Valid SSN");
}

// Event-based validation
control.ValueValidatingEx += (sender, e) =>
{
    if (e.NewValue < 0)
    {
        e.Cancel = true;
        e.ValidationMessage = "SSN cannot be negative";
    }
};

control.RangeExceeded += (sender, e) =>
{
    MessageBox.Show($"Value {e.AttemptedValue} is out of range!");
};

control.InvalidInput += (sender, e) =>
{
    MessageBox.Show($"Invalid input: {e.Input}\nReason: {e.Reason}");
};
```

### Accessing Mask Configuration
```csharp
var control = new BeepNumericUpDown
{
    MaskPreset = NumericMaskPreset.Currency
};

// Get icon path
string iconPath = control.GetMaskIconPath();  // "SvgsUI.DollarSign"

// Check if should show icon
bool showIcon = control.ShouldShowMaskIcon();  // true

// Access full config
NumericMaskConfig config = control.MaskConfig;
Console.WriteLine($"Pattern: {config.MaskPattern}");        // "$#,###.##"
Console.WriteLine($"Decimal Places: {config.DecimalPlaces}"); // 2
Console.WriteLine($"Allow Negative: {config.AllowNegative}"); // true
```

## Validation Methods

### Available Validation Methods
```csharp
// Range validation
bool isValid = control.ValidateRange(value, out string error);

// Format validation (checks mask pattern)
bool isValid = control.ValidateFormat("(555) 123-4567", out string error);

// Step validation (checks increment compliance)
bool isValid = control.ValidateStep(10.5m, out string error);

// Decimal places validation
bool isValid = control.ValidateDecimalPlaces("123.456", out string error);

// Character-by-character validation
bool isValid = control.ValidateNumericInput('5', currentText, selectionStart);

// Comprehensive validation
bool isValid = control.ValidateValue(123.45m, out string error);
```

## Validation Events

### 7 Comprehensive Events
```csharp
// Before value changes (cancellable)
control.ValueValidatingEx += (sender, e) =>
{
    if (e.NewValue > 1000)
    {
        e.Cancel = true;
        e.ValidationMessage = "Maximum 1000 allowed";
    }
};

// After validation completes
control.ValueValidated += (sender, e) =>
{
    Console.WriteLine($"Changed from {e.OldValue} to {e.NewValue}");
};

// Range exceeded
control.RangeExceeded += (sender, e) =>
{
    if (e.IsMaximumExceeded)
        MessageBox.Show($"Maximum {e.MaximumValue} exceeded!");
    else
        MessageBox.Show($"Minimum {e.MinimumValue} not met!");
};

// Invalid input detected
control.InvalidInput += (sender, e) =>
{
    Console.WriteLine($"Invalid: {e.Input} - {e.Reason}");
};

// Format validation
control.FormatValidation += (sender, e) =>
{
    if (!e.IsValid)
        MessageBox.Show($"Expected: {e.ExpectedFormat}");
};

// Step validation failed
control.StepValidationFailed += (sender, e) =>
{
    MessageBox.Show("Value must match increment steps");
};

// Legacy validation failed
control.ValueValidationFailed += (sender, e) =>
{
    MessageBox.Show("Validation failed");
};
```

## Icon Integration

All mask presets automatically include appropriate icons from the `Svgs` and `SvgsUI` libraries:

- **Phone**: 📱 SvgsUI.Phone
- **Currency**: 💵 SvgsUI.DollarSign, 💳 SvgsUI.CreditCard
- **Location**: 📍 SvgsUI.MapPin
- **Time**: 🕐 SvgsUI.Clock
- **Date**: 📅 SvgsUI.Calendar
- **Network**: 🌐 SvgsUI.Globe, 📡 SvgsUI.Wifi
- **Temperature**: 🌡️ SvgsUI.Thermometer
- **Weight**: ⚖️ Svgs.Weight
- **Files**: 💾 SvgsUI.HardDrive
- **Identity**: 🆔 SvgsUI.IdCard
- **Building**: 🏢 SvgsUI.Building

Icons are rendered using `StyledImagePainter` and automatically tinted to match the current theme.

## Best Practices

1. **Choose appropriate NumericStyle for your use case**
   - `Standard`: General numeric input
   - `CompactStepper`: Quantity/dimensions
   - `Phone`: Phone numbers with country codes
   - `Currency`: Money with currency dropdown
   - `Inline`: Typography controls, adjustments

2. **Combine MaskPreset with validation**
   ```csharp
   control.MaskPreset = NumericMaskPreset.Currency;
   control.MinimumValue = 0;
   control.MaximumValue = 999999.99m;
   ```

3. **Use events for user feedback**
   ```csharp
   control.RangeExceeded += ShowErrorTooltip;
   control.InvalidInput += HighlightControl;
   ```

4. **Leverage auto-configuration**
   - Mask presets automatically set decimal places, ranges, icons, and units
   - No need to manually configure common scenarios

5. **Test with different BeepControlStyles**
   - Material3, iOS15, Fluent2, etc.
   - Icons and styling adapt automatically

## Complete Example: Order Form

```csharp
public class OrderForm : Form
{
    public OrderForm()
    {
        // Product quantity
        var quantityControl = new BeepNumericUpDown
        {
            Location = new Point(10, 10),
            MaskPreset = NumericMaskPreset.IntegerWithCommas,
            NumericStyle = NumericStyle.CompactStepper,
            MinimumValue = 1,
            MaximumValue = 9999,
            IncrementValue = 1
        };

        // Unit price
        var priceControl = new BeepNumericUpDown
        {
            Location = new Point(10, 50),
            MaskPreset = NumericMaskPreset.Currency,
            NumericStyle = NumericStyle.Currency,
            MinimumValue = 0.01m,
            MaximumValue = 999999.99m,
            IncrementValue = 0.01m
        };

        // Discount percentage
        var discountControl = new BeepNumericUpDown
        {
            Location = new Point(10, 90),
            MaskPreset = NumericMaskPreset.Percentage,
            NumericStyle = NumericStyle.Slider,
            MinimumValue = 0,
            MaximumValue = 100,
            IncrementValue = 5
        };

        // Phone number
        var phoneControl = new BeepNumericUpDown
        {
            Location = new Point(10, 130),
            MaskPreset = NumericMaskPreset.PhoneUS,
            NumericStyle = NumericStyle.Phone
        };

        // ZIP code
        var zipControl = new BeepNumericUpDown
        {
            Location = new Point(10, 170),
            MaskPreset = NumericMaskPreset.ZipCode,
            NumericStyle = NumericStyle.Standard
        };

        Controls.AddRange(new Control[]
        {
            quantityControl,
            priceControl,
            discountControl,
            phoneControl,
            zipControl
        });
    }
}
```
