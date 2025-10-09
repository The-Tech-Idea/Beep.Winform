# BeepiFormPro Backdrop Effects

Comprehensive guide to backdrop effects in BeepiFormPro, including Acrylic, Mica, Blur, and system backdrop support.

## Overview

BeepiFormPro supports modern Windows backdrop effects that add depth and visual interest to your forms. These effects use Windows composition APIs to apply blur, translucency, and material effects behind your form.

## BackdropType Enum

The `BackdropType` property controls which backdrop effect is applied:

```csharp
public enum BackdropType
{
    None,       // No backdrop effect (default)
    Blur,       // Simple blur-behind effect
    Acrylic,    // Windows 10 Acrylic material
    Mica,       // Windows 11 Mica material
    Tabbed,     // Windows 11 tabbed window backdrop
    Transient   // Windows 11 transient window backdrop
}
```

## Properties

### Backdrop
Sets the active backdrop type.

```csharp
[Category("Beep Backdrop")]
[DefaultValue(BackdropType.None)]
public BackdropType Backdrop { get; set; }
```

**Usage:**
```csharp
var form = new BeepiFormPro();
form.Backdrop = BackdropType.Acrylic;
```

### EnableAcrylicForGlass
Automatically enables Acrylic effect when `FormStyle` is set to `Glass`.

```csharp
[Category("Beep Backdrop")]
[DefaultValue(true)]
public bool EnableAcrylicForGlass { get; set; }
```

**Usage:**
```csharp
form.FormStyle = FormStyle.Glass;
form.EnableAcrylicForGlass = true; // Acrylic automatically applied
```

### EnableMicaBackdrop
Enables Windows 11 Mica backdrop effect.

```csharp
[Category("Beep Backdrop")]
[DefaultValue(false)]
public bool EnableMicaBackdrop { get; set; }
```

**Usage:**
```csharp
form.EnableMicaBackdrop = true; // Mica effect on Windows 11
```

## Backdrop Types Explained

### None
No backdrop effect. Standard opaque window background.

**When to use:**
- Default forms
- When performance is critical
- Legacy system compatibility

### Blur
Simple blur-behind effect using DWM composition.

**Windows Version:** Windows Vista+  
**Effect:** Blurs content behind the window  
**Performance:** Moderate

**Example:**
```csharp
form.Backdrop = BackdropType.Blur;
form.Opacity = 0.95; // Adjust transparency
```

### Acrylic
Windows 10 Acrylic material effect - translucent with blur and subtle noise texture.

**Windows Version:** Windows 10 Fall Creators Update (1709)+  
**Effect:** Layered blur with color tint and noise texture  
**Performance:** Moderate (GPU accelerated)

**Features:**
- Depth and material perception
- Slight color tint (configurable)
- Noise texture for visual interest
- Responds to lighting conditions

**Example:**
```csharp
form.Backdrop = BackdropType.Acrylic;
form.FormBorderStyle = FormBorderStyle.None;
```

### Mica
Windows 11 Mica material - desktop wallpaper-based backdrop.

**Windows Version:** Windows 11 (Build 22000)+  
**Effect:** Tinted, blurred desktop wallpaper visible through window  
**Performance:** Excellent (highly optimized)

**Features:**
- Desktop wallpaper integration
- Subtle, unobtrusive
- Adapts to desktop background
- Lower GPU usage than Acrylic

**Example:**
```csharp
form.Backdrop = BackdropType.Mica;
form.EnableMicaBackdrop = true;
```

### Tabbed
Windows 11 tabbed window backdrop for applications with tab interfaces.

**Windows Version:** Windows 11 22H2+  
**Effect:** Optimized backdrop for tabbed windows  
**Performance:** Excellent

**When to use:**
- Multi-document interface (MDI)
- Tabbed applications
- Browser-like interfaces

**Example:**
```csharp
form.Backdrop = BackdropType.Tabbed;
```

### Transient
Windows 11 transient window backdrop for temporary/flyout windows.

**Windows Version:** Windows 11 22H2+  
**Effect:** Lightweight backdrop for temporary windows  
**Performance:** Excellent

**When to use:**
- Dialog windows
- Flyout menus
- Temporary overlays
- Tooltips

**Example:**
```csharp
form.Backdrop = BackdropType.Transient;
```

## Complete Examples

### Example 1: Acrylic Form with Custom Border

```csharp
var form = new BeepiFormPro
{
    Text = "Acrylic Demo",
    Size = new Size(800, 600),
    FormBorderStyle = FormBorderStyle.None,
    FormStyle = FormStyle.Modern,
    Backdrop = BackdropType.Acrylic,
    BorderRadius = 12,
    BorderThickness = 1,
    BorderColor = Color.FromArgb(150, Color.White)
};

form.Show();
```

### Example 2: Mica Form (Windows 11)

```csharp
var form = new BeepiFormPro
{
    Text = "Mica Demo",
    Size = new Size(1000, 700),
    FormBorderStyle = FormBorderStyle.None,
    FormStyle = FormStyle.Fluent,
    EnableMicaBackdrop = true,
    ShowCaptionBar = true,
    CaptionHeight = 40
};

form.Show();
```

### Example 3: Glass Form with Auto-Acrylic

```csharp
var form = new BeepiFormPro
{
    Text = "Glass Demo",
    Size = new Size(900, 650),
    FormStyle = FormStyle.Glass,
    EnableAcrylicForGlass = true, // Automatic Acrylic
    ActivePainter = new GlassFormPainter(),
    BorderRadius = 8
};

form.Show();
```

### Example 4: Dynamic Backdrop Switching

```csharp
var form = new BeepiFormPro
{
    Text = "Dynamic Backdrop Demo",
    Size = new Size(800, 600)
};

// Add buttons to switch backdrops
var btnAcrylic = new Button { Text = "Acrylic", Location = new Point(10, 50) };
btnAcrylic.Click += (s, e) => form.Backdrop = BackdropType.Acrylic;

var btnMica = new Button { Text = "Mica", Location = new Point(100, 50) };
btnMica.Click += (s, e) => form.Backdrop = BackdropType.Mica;

var btnBlur = new Button { Text = "Blur", Location = new Point(190, 50) };
btnBlur.Click += (s, e) => form.Backdrop = BackdropType.Blur;

var btnNone = new Button { Text = "None", Location = new Point(280, 50) };
btnNone.Click += (s, e) => form.Backdrop = BackdropType.None;

form.Controls.AddRange(new Control[] { btnAcrylic, btnMica, btnBlur, btnNone });
form.Show();
```

## Combining Backdrops with Painters

Backdrop effects work seamlessly with form painters:

```csharp
// Material Design with Acrylic
var form = new BeepiFormPro
{
    FormStyle = FormStyle.Material,
    Backdrop = BackdropType.Acrylic,
    ActivePainter = new MaterialFormPainter()
};

// Glass painter with Acrylic auto-enabled
var form2 = new BeepiFormPro
{
    FormStyle = FormStyle.Glass,
    EnableAcrylicForGlass = true,
    ActivePainter = new GlassFormPainter()
};

// Minimal with Mica (Windows 11)
var form3 = new BeepiFormPro
{
    FormStyle = FormStyle.Minimal,
    EnableMicaBackdrop = true,
    ActivePainter = new MinimalFormPainter()
};
```

## Best Practices

### 1. Windows Version Detection

Check Windows version before enabling advanced effects:

```csharp
bool IsWindows11OrGreater()
{
    return Environment.OSVersion.Version.Build >= 22000;
}

if (IsWindows11OrGreater())
{
    form.EnableMicaBackdrop = true;
}
else
{
    form.Backdrop = BackdropType.Acrylic;
}
```

### 2. Performance Considerations

- **Acrylic**: Moderate GPU usage, good for most scenarios
- **Mica**: Low GPU usage, excellent for Windows 11
- **Blur**: Higher CPU/GPU usage, use sparingly
- **None**: Best performance, no effects

### 3. Form Opacity

Adjust form opacity to enhance backdrop visibility:

```csharp
form.Backdrop = BackdropType.Acrylic;
form.Opacity = 0.95; // Slight transparency enhances effect
```

### 4. Border Considerations

For best visual results with backdrops:

```csharp
form.FormBorderStyle = FormBorderStyle.None;
form.BorderRadius = 8; // Rounded corners
form.BorderThickness = 1; // Thin border
form.BorderColor = Color.FromArgb(100, Color.White); // Semi-transparent
```

### 5. Content Layering

Ensure content has proper backgrounds when using backdrops:

```csharp
// Add panel with solid background for content areas
var contentPanel = new Panel
{
    Dock = DockStyle.Fill,
    BackColor = Color.FromArgb(240, 50, 50, 50), // Semi-transparent dark
    Padding = new Padding(10)
};
form.Controls.Add(contentPanel);
```

## Troubleshooting

### Backdrop Not Visible

**Problem:** Backdrop effect not appearing.

**Solutions:**
1. Ensure `FormBorderStyle = FormBorderStyle.None`
2. Set `form.Opacity < 1.0` for better visibility
3. Check Windows version compatibility
4. Verify DWM (Desktop Window Manager) is enabled

### Mica Not Working

**Problem:** Mica backdrop not applying.

**Solutions:**
1. Verify Windows 11 (Build 22000+)
2. Check `EnableMicaBackdrop = true`
3. Ensure desktop wallpaper is set (Mica uses wallpaper)
4. Try restarting DWM: `net stop uxsms & net start uxsms` (admin)

### Acrylic Appears Too Dark

**Problem:** Acrylic effect is too opaque or dark.

**Solutions:**
1. Adjust form opacity: `form.Opacity = 0.95`
2. Modify background colors to lighter shades
3. Use `EnableAcrylicForGlass` with `GlassFormPainter`
4. Reduce painter background opacity

### Performance Issues

**Problem:** Form rendering is slow with backdrop effects.

**Solutions:**
1. Switch from Acrylic to Mica (Windows 11)
2. Use `BackdropType.None` on lower-end systems
3. Reduce animation and transparency
4. Disable backdrop when window is minimized

## Windows API Details

### SetWindowCompositionAttribute

Used for Acrylic and Blur effects.

```csharp
[DllImport("user32.dll")]
private static extern int SetWindowCompositionAttribute(
    IntPtr hwnd, 
    ref WINDOWCOMPOSITIONATTRIBDATA data
);
```

**Accent States:**
- `ACCENT_DISABLED` (0): No effect
- `ACCENT_ENABLE_BLURBEHIND` (3): Blur effect
- `ACCENT_ENABLE_ACRYLICBLURBEHIND` (4): Acrylic effect

### DwmSetWindowAttribute

Used for Mica and system backdrop effects.

```csharp
[DllImport("dwmapi.dll")]
private static extern int DwmSetWindowAttribute(
    IntPtr hwnd, 
    DWMWINDOWATTRIBUTE attr, 
    ref int attrValue, 
    int attrSize
);
```

**Attributes:**
- `DWMWA_MICA_EFFECT` (1029): Mica backdrop
- `DWMWA_SYSTEMBACKDROP_TYPE` (38): System backdrop type

## Platform Support Matrix

| Backdrop Type | Windows Vista | Windows 7 | Windows 8/8.1 | Windows 10 | Windows 11 |
|--------------|---------------|-----------|---------------|------------|------------|
| None         | ✅            | ✅        | ✅            | ✅         | ✅         |
| Blur         | ✅            | ✅        | ✅            | ✅         | ✅         |
| Acrylic      | ❌            | ❌        | ❌            | ✅ (1709+) | ✅         |
| Mica         | ❌            | ❌        | ❌            | ❌         | ✅         |
| Tabbed       | ❌            | ❌        | ❌            | ❌         | ✅ (22H2+) |
| Transient    | ❌            | ❌        | ❌            | ❌         | ✅ (22H2+) |

## Related Properties

- `FormStyle`: Controls overall form appearance
- `ActivePainter`: Visual rendering strategy
- `BorderRadius`: Rounded corners (pairs well with backdrops)
- `BorderColor`: Border color (use semi-transparent for best results)
- `ShowCaptionBar`: Custom caption bar rendering
- `Opacity`: Form transparency level

## See Also

- [Painters Documentation](./Readme.md)
- [FormStyle Documentation](../Readme.md)
- [BeepiForm Backdrop Effects](../../BeepiForm.cs)
- [Windows Desktop Window Manager API](https://docs.microsoft.com/en-us/windows/win32/dwm/dwm-overview)
