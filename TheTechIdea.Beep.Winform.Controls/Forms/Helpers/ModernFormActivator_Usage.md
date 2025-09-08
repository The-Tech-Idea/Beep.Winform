# ModernFormActivator Usage Examples

The `ModernFormActivator` allows you to add modern Beep UI features to ANY Windows Form, not just BeepiForm.

## Basic Usage

```csharp
using TheTechIdea.Beep.Winform.Controls.Forms.Helpers;

// Enable modern features on any existing form
var controller = ModernFormActivator.Enable(myForm);

// Configure modern features
controller.BorderRadius = 10;
controller.ShowCaptionBar = true;
controller.EnableGlow = true;
controller.GlowColor = Color.Blue;
controller.ShadowDepth = 5;

// Apply a theme
controller.ApplyTheme(myBeepTheme);

// Add custom overlay painters
controller.RegisterOverlayPainter(g => {
    // Custom drawing code
    g.DrawString("Custom Overlay", Font, Brushes.Red, 10, 10);
});
```

## Retrofitting Legacy Forms

```csharp
// Take any existing Windows Form and make it modern
public partial class MyLegacyForm : Form
{
    private IModernFormController _modernController;

    public MyLegacyForm()
    {
        InitializeComponent();
        
        // Enable modern features
        _modernController = ModernFormActivator.Enable(this);
        _modernController.BorderRadius = 8;
        _modernController.ShowCaptionBar = true;
        _modernController.ApplyStyle(BeepFormStyle.Material);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _modernController?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

## Applying to Multiple Forms

```csharp
// Enable modern features across your application
public static class AppModernizer
{
    public static void ModernizeForm(Form form, BeepFormStyle style = BeepFormStyle.Modern)
    {
        var controller = ModernFormActivator.Enable(form);
        controller.ApplyStyle(style);
        controller.BorderRadius = 10;
        controller.EnableGlow = true;
        controller.ShowCaptionBar = true;
    }

    public static void ModernizeAllForms(IEnumerable<Form> forms)
    {
        foreach (var form in forms)
        {
            ModernizeForm(form);
        }
    }
}

// Usage:
AppModernizer.ModernizeForm(new MyDialogForm(), BeepFormStyle.Material);
```

## Design-Time Safe Usage

```csharp
public partial class MyForm : Form
{
    private IModernFormController _controller;

    public MyForm()
    {
        InitializeComponent();
        
        // Only enable in runtime, not in designer
        if (!DesignMode)
        {
            _controller = ModernFormActivator.Enable(this);
            _controller.BorderRadius = 12;
        }
    }
}
```

## Features Available

- ? **Rounded Borders**: Set `BorderRadius` and `BorderThickness`
- ? **Custom Caption Bar**: Control with `ShowCaptionBar`, `CaptionHeight`, `ShowSystemButtons`
- ? **Shadow & Glow Effects**: Configure `ShadowDepth`, `EnableGlow`, `GlowColor`
- ? **Theme Support**: Apply any `IBeepTheme` with `ApplyTheme()`
- ? **Style Presets**: Use `ApplyStyle()` with predefined styles
- ? **Custom Overlays**: Register custom painters with `RegisterOverlayPainter()`
- ? **Automatic Cleanup**: Dispose pattern handled automatically

## Benefits

1. **Non-invasive**: Works with existing forms without inheritance changes
2. **Gradual adoption**: Enable features incrementally
3. **Consistent API**: Same interface as BeepiForm
4. **Performance**: Only enabled features consume resources
5. **Design-time safe**: Helpers check for design mode automatically

The `ModernFormActivator` makes it possible to modernize entire applications without rewriting existing forms!