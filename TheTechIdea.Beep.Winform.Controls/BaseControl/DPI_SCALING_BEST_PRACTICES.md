# DPI Scaling Best Practices for .NET 8/9/10 WinForms

## 🔍 Critical Reality Check

**Microsoft's .NET 8/9/10 improvements DO NOT automatically fix DPI scaling for custom controls.**

The framework handles:
- ✅ Standard control sizes/positions via `AutoScaleMode`
- ✅ Font scaling for `Label`/`TextBox` when `AutoScaleMode.Font`
- ✅ Container layout adjustments

The framework does **NOT** handle:
- ❌ Custom-drawn graphics (your painter logic)
- ❌ Bitmap/icon rendering
- ❌ Border thicknesses in custom paint
- ❌ Hit-test regions for custom controls
- ❌ Animation timing (splash effects need DPI-aware duration)

## 🛠️ Required Implementation Checklist

### 1. Application Manifest (NON-NEGOTIABLE)

**Copy `app.manifest.template` to your project root as `app.manifest`**

```xml
<dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">true/PM</dpiAware>
<dpiAwareness xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">PerMonitorV2, PerMonitor</dpiAwareness>
```

**Add to `.csproj`:**
```xml
<PropertyGroup>
  <ApplicationManifest>app.manifest</ApplicationManifest>
</PropertyGroup>
```

**Why this is critical:**
Without this, Windows will bitmap-stretch your entire app when moving between monitors. No amount of code fixes this.

### 2. Top-Level Form Configuration

```csharp
public class MainForm : Form
{
    public MainForm()
    {
        // CRITICAL: Set before InitializeComponent()
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScaleDimensions = new SizeF(96f, 96f);
        
        InitializeComponent();
    }
}
```

### 3. BaseControl DPI Handling (Already Implemented ✅)

BaseControl now provides comprehensive DPI handling with GDI leak prevention:

#### TryScaleFontSafely with GDI Leak Prevention
```csharp
protected bool TryScaleFontSafely(float scaleFactor, bool force = false)
{
    if (scaleFactor <= 0.01f || IsDisposed || !IsHandleCreated) 
        return false;

    // Skip if already at target scale (prevents recursive scaling loops)
    if (!force && Math.Abs(scaleFactor - 1.0f) < 0.01f)
        return false;

    Font currentFont = Font;
    if (currentFont == null || currentFont == Control.DefaultFont)
        return false;

    // CRITICAL: Check if font was inherited from parent
    if (!IsFontExplicitlySet(currentFont))
        return false;

    try
    {
        Font? scaledFont = DpiScalingHelper.ScaleFont(currentFont, scaleFactor);
        
        if (scaledFont == null || scaledFont == currentFont)
            return false;

        // SAFE DISPOSAL: Assign new font BEFORE disposing old one
        Font = scaledFont;
        
        // Dispose old font ONLY if it's not shared/default
        if (currentFont != null && 
            currentFont != Control.DefaultFont && 
            currentFont != SystemFonts.DefaultFont &&
            !ReferenceEquals(currentFont, scaledFont))
        {
            currentFont.Dispose();
        }

        return true;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($\"Font scaling failed: {ex.Message}\");
        return false;
    }
}
```

#### IsFontExplicitlySet to Prevent Double-Scaling
```csharp
private bool IsFontExplicitlySet(Font font)
{
    if (font == null) return false;
    
    // Compare with parent's font to detect inheritance
    if (Parent != null && Parent.Font != null)
    {
        return !font.Equals(Parent.Font);
    }
    
    // Check if font matches system default
    return !font.Equals(SystemFonts.DefaultFont) && 
           !font.Equals(Control.DefaultFont);
}
```

#### OnDpiChangedAfterParent with Proper Scaling Order
```csharp
protected override void OnPaint(PaintEventArgs e)
{
    var g = e.Graphics;
    var originalTransform = g.Transform;
    try
    {
        // Apply DPI scale based on DeviceDpi (NOT Graphics.DpiX)
        float dpiScale = DeviceDpi / 96f;
        if (Math.Abs(dpiScale - 1f) > 0.01f)
        {
            g.ScaleTransform(dpiScale, dpiScale);
        }
        
        SafeDraw(g);
    }
    finally
    {
        g.Transform = originalTransform;
    }
}
```

#### OnDpiScaleChanged with Font Scaling
```csharp
protected virtual void OnDpiScaleChanged(float oldScaleX, float oldScaleY, 
                                        float newScaleX, float newScaleY)
{
    // Scale font with safe disposal
    if (Font != null && Math.Abs(newScaleX - oldScaleX) > 0.01f)
    {
        var ratio = newScaleX / oldScaleX;
        var oldFont = Font;
        var newSize = Math.Max(oldFont.Size * ratio, 6f);
        Font = new Font(oldFont.FontFamily, newSize, oldFont.Style, oldFont.Unit);
        
        // CRITICAL: Dispose old font to prevent GDI leaks
        if (oldFont != DefaultFont && !ReferenceEquals(oldFont, Font))
        {
            oldFont?.Dispose();
        }
    }
    
    UpdateDrawingRect();
    _painter?.UpdateLayout(this);
    PerformLayout();
    Invalidate(true);
}
```

### 4. DpiScalingHelper Utilities (Already Implemented ✅)

Use these methods in custom controls/painters:

```csharp
// Scale integer values
int scaledPadding = DpiScalingHelper.ScaleValue(10, control);

// Scale sizes
Size scaledSize = DpiScalingHelper.ScaleSize(new Size(20, 20), control);

// Scale rectangles
Rectangle scaledRect = DpiScalingHelper.ScaleRectangle(myRect, control);

// Scale fonts
Font scaledFont = DpiScalingHelper.ScaleFont(baseFont, control);

// Scale images (prevents blurring on hi-DPI)
Image scaledImage = DpiScalingHelper.ScaleImage(originalImage, control);

// Scale icons
Icon scaledIcon = DpiScalingHelper.ScaleIcon(originalIcon, dpiScale);
```

## 🚨 Common Pitfalls to Avoid

### ⚠️ Using Graphics.DpiX/Y in OnPaint
**WRONG:**
```csharp
protected override void OnPaint(PaintEventArgs e)
{
    float scale = e.Graphics.DpiX / 96f; // Gets printer DPI during print preview!
}
```

**CORRECT:**
```csharp
protected override void OnPaint(PaintEventArgs e)
{
    float scale = DeviceDpi / 96f; // Always use Control.DeviceDpi
}
```

### ⚠️ Scaling Twice (AutoScaleMode + Manual)
**WRONG:**
```csharp
public MyControl()
{
    AutoScaleMode = AutoScaleMode.Dpi; // Framework scales
    // Then in OnPaint: ScaleTransform(DeviceDpi/96) // Double scaling!
}
```

**CORRECT:**
```csharp
public MyControl()
{
    AutoScaleMode = AutoScaleMode.Inherit; // For child controls
    // BaseControl.OnPaint handles ScaleTransform once
}
```

### ⚠️ Not Disposing Old Fonts
**WRONG:**
```csharp
Font = new Font("Arial", newSize); // GDI handle leak!
```

**CORRECT:**
```csharp
var oldFont = Font;
Font = new Font(oldFont.FontFamily, newSize, oldFont.Style);
// CRITICAL: Dispose AFTER assignment to avoid flicker
if (oldFont != Control.DefaultFont && oldFont != SystemFonts.DefaultFont) 
{
    oldFont?.Dispose();
}
```

**BEST (Use BaseControl helper):**
```csharp
TryScaleFontSafely(scaleFactor); // Handles disposal automatically
```

### ⚠️ Scaling Inherited Fonts (Double-Scaling)
**WRONG:**
```csharp
// Parent already scaled font, scaling again causes exponential growth!
protected override void OnDpiChangedAfterParent(EventArgs e)
{
    base.OnDpiChangedAfterParent(e);
    Font = DpiScalingHelper.ScaleFont(Font, newScale); // BAD if inherited
}
```

**CORRECT:**
```csharp
// Only scale if font was explicitly set on this control
protected override void OnDpiChangedAfterParent(EventArgs e)
{
    base.OnDpiChangedAfterParent(e);
    TryScaleFontSafely(scaleFactor); // Checks IsFontExplicitlySet internally
}
```

### ⚠️ Hard-Coded Icon Sizes
**WRONG:**
```csharp
g.DrawImage(icon, 0, 0, 16, 16); // Blurry on 4K screens
```

**CORRECT:**
```csharp
var scaledSize = DpiScalingHelper.ScaleValue(16, this);
var scaledImage = DpiScalingHelper.ScaleImage(icon, this);
g.DrawImage(scaledImage, 0, 0, scaledSize, scaledSize);
```

### ⚠️ Ignoring WM_DPICHANGED
**WRONG:**
```csharp
// No OnDpiChangedAfterParent override
```

**CORRECT:**
```csharp
protected override void OnDpiChangedAfterParent(EventArgs e)
{
    base.OnDpiChangedAfterParent(e);
    DpiScalingHelper.RefreshScaleFactors(this, ref _dpiScaleX, ref _dpiScaleY);
    OnDpiScaleChanged(oldScaleX, oldScaleY, _dpiScaleX, _dpiScaleY);
}
```

## 🔬 Testing & Verification

### Verification Checklist
- [ ] Application manifest has `PerMonitorV2` DPI awareness
- [ ] Top-level form uses `AutoScaleMode.Dpi`
- [ ] `OnPaint` uses `Graphics.ScaleTransform(DeviceDpi/96f, ...)`
- [ ] Fonts scaled manually in `OnDpiChangedAfterParent` with safe disposal
- [ ] All hard-coded sizes replaced with `DpiScalingHelper.ScaleValue()`
- [ ] Bitmap resources scaled via `DpiScalingHelper.ScaleImage()`
- [ ] Tested moving window between 100% → 175% DPI monitors (no blurring/jumping)
- [ ] Verified no GDI handle leaks after 50+ DPI changes (Task Manager → GDI Objects)

### Testing Procedure
1. **Single Monitor Test (Easy)**
   - Run app at 100% DPI
   - Change DPI to 150% (Settings → Display → Scale)
   - Verify text/controls resize without blurring

2. **Multi-Monitor Test (Critical)**
   - Setup 2 monitors with different DPI (e.g., 100% and 175%)
   - Drag app window between monitors
   - Verify smooth resize, no bitmap stretching

3. **Leak Test**
   - Open Task Manager → Details → Your App → Select Columns → GDI Objects
   - Drag app between monitors 50 times
   - GDI Object count should stabilize (not grow linearly)

4. **Printer Test**
   - Open Print Preview
   - Verify UI doesn't explode (Graphics.DpiX returns 600+ DPI for printers!)

## 📚 Painter Implementation Guidelines

If you're creating a custom painter implementing `IBaseControlPainter`:

```csharp
public class MyCustomPainter : IBaseControlPainter
{
    public void UpdateLayout(BaseControl owner)
    {
        // CRITICAL: Use DpiScalingHelper for hard-coded values
        int shadowSize = DpiScalingHelper.ScaleValue(4, owner);
        int borderThickness = DpiScalingHelper.ScaleValue(2, owner);
        
        // Access owner._dpiScaleX/_dpiScaleY if needed (protected internal)
        var scale = owner.DeviceDpi / 96f;
        
        // Recalculate all cached rectangles
        _borderRect = new Rectangle(
            shadowSize,
            shadowSize,
            owner.Width - (shadowSize * 2),
            owner.Height - (shadowSize * 2)
        );
    }
    
    public void Paint(Graphics g, BaseControl owner)
    {
        // DO NOT use Graphics.DpiX/Y
        // BaseControl.OnPaint already applied ScaleTransform
        
        // If you need to scale images:
        var scaledImage = DpiScalingHelper.ScaleImage(originalImage, owner);
        g.DrawImage(scaledImage, _borderRect);
    }
}
```

## 🎯 Key Insight for .NET 8/9/10

Microsoft gives you **better tools** (`DeviceDpi`, `OnDpiChangedAfterParent`), but you still **own the scaling logic** for custom visuals.

There is **no magic "auto-scale everything" switch**.

Custom controls = Custom responsibility.

## 📖 References

- [Microsoft: High DPI Desktop Development](https://learn.microsoft.com/en-us/windows/win32/hidpi/high-dpi-desktop-application-development-on-windows)
- [WinForms High DPI Support](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/high-dpi-support-in-windows-forms)
- [DPI-Aware Controls in .NET](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/forms/autoscale)

## ✅ What BaseControl Already Provides

As of this implementation:
1. ✅ `OnPaint` with `ScaleTransform` based on `DeviceDpi`
2. ✅ `TryScaleFontSafely` with GDI leak prevention and inheritance detection
3. ✅ `IsFontExplicitlySet` to prevent double-scaling of inherited fonts
4. ✅ `OnDpiChangedAfterParent` with proper font-first scaling order
5. ✅ `OnFontChanged` to handle theme/style font updates
6. ✅ `InitLayout` with initial DPI scaling for explicitly set fonts
7. ✅ `DpiScalingHelper.ScaleImage()` for bitmap scaling
8. ✅ `DpiScalingHelper.ScaleIcon()` for icon scaling
9. ✅ Painter integration via `UpdateLayout()` on DPI change
10. ✅ Application manifest template with PerMonitorV2

## 🧠 Key Insights: Font Scaling Lifecycle

### Why Font Scaling is Complex

1. **Inheritance Chain**: Child controls inherit parent fonts by default
2. **Event Cascade**: Parent DPI change triggers child DPI change
3. **GDI Resources**: Fonts must be manually disposed to prevent leaks
4. **Layout Dependencies**: Font size affects control layout (must scale before layout)
5. **Recursive Risk**: Scaling inherited fonts causes exponential growth

### BaseControl's Solution

```
DPI Change Event Flow:
┌─────────────────────────────────────────┐
│ OnDpiChangedAfterParent                 │
├─────────────────────────────────────────┤
│ 1. Refresh scale factors (_dpiScaleX/Y)│
│ 2. TryScaleFontSafely (this control)    │  ← Font scaled FIRST
│    ├─ Check IsFontExplicitlySet         │  ← Prevents double-scaling
│    ├─ ScaleFont via DpiScalingHelper    │
│    └─ Dispose old font safely           │  ← Prevents GDI leaks
│ 3. ScaleControlTreeForDpiChange         │  ← Layout/children AFTER font
│    └─ scaleFont: false (already done)   │
│ 4. OnDpiScaleChanged (custom metrics)   │
│ 5. Invalidate & PerformLayout           │
└─────────────────────────────────────────┘

Font Change Event Flow (Theme/Style):
┌─────────────────────────────────────────┐
│ OnFontChanged                           │
├─────────────────────────────────────────┤
│ 1. base.OnFontChanged()                 │
│ 2. Get current DPI scale                │
│ 3. TryScaleFontSafely (force: true)     │  ← Re-scale NEW font
│ 4. Invalidate cached metrics            │
│ 5. Update painter & invalidate          │
└─────────────────────────────────────────┘
```

## 🚀 Next Steps for Developers

1. **Copy `app.manifest.template` to your project** and rename to `app.manifest`
2. **Update your main form** to use `AutoScaleMode.Dpi`
3. **Audit custom painters** to ensure they use `DpiScalingHelper`
4. **Test on multi-monitor setups** with different DPI settings
5. **Monitor GDI object leaks** in Task Manager during DPI changes

---

**Last Updated:** 2026-02-15  
**Applies To:** .NET 8/9/10 WinForms Applications with Custom Controls
