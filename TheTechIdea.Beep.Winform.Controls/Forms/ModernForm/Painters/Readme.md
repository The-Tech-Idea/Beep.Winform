# BeepiFormPro Painters

This folder contains painter classes for customizing the visual appearance of `BeepiFormPro` forms.

## Overview

The painter pattern allows complete separation of rendering logic from form behavior. Each painter implements the `IFormPainter` interface and provides distinct visual styles for:
- Form background
- Caption bar
- Window borders
- System buttons (Close, Maximize, Minimize)
- Theme/Style buttons (when enabled)

## Architecture

### IFormPainter Interface

The core interface that all form painters must implement:

```csharp
public interface IFormPainter
{
    void PaintBackground(Graphics g, BeepiFormPro owner);
    void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect);
    void PaintBorders(Graphics g, BeepiFormPro owner);
}
```

### IFormPainterMetricsProvider Interface

Painters should also implement this interface to provide layout metrics:

```csharp
public interface IFormPainterMetricsProvider
{
    FormPainterMetrics GetMetrics(BeepiFormPro owner);
    void CalculateLayoutAndHitAreas(BeepiFormPro owner);
}
```

## ? CRITICAL: CalculateLayoutAndHitAreas Implementation

ALL painters now properly implement `CalculateLayoutAndHitAreas` to support theme and style buttons. The hit-area manager now prioritizes the smallest matching rectangle, so registering the full-width caption region no longer blocks button clicks.

### Required Implementation Pattern (Applied Everywhere)

```csharp
public void CalculateLayoutAndHitAreas(BeepiFormPro owner)
{
    var layout = new PainterLayoutInfo();
    var metrics = GetMetrics(owner);
    
    int captionHeight = Math.Max(metrics.CaptionHeight, (int)(owner.Font.Height * metrics.FontHeightMultiplier));
    owner._hits.Clear();
    
    layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
    owner._hits.Register("caption", layout.CaptionRect, HitAreaType.Drag);
    
    int buttonWidth = metrics.ButtonWidth;
    int buttonX = owner.ClientSize.Width - buttonWidth;
    
    // Standard window buttons (right to left)
    layout.CloseButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
    owner._hits.RegisterHitArea("close", layout.CloseButtonRect, HitAreaType.Button);
    buttonX -= buttonWidth;
    
    layout.MaximizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
    owner._hits.RegisterHitArea("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
    buttonX -= buttonWidth;
    
    layout.MinimizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
    owner._hits.RegisterHitArea("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
    buttonX -= buttonWidth;
    
    // Style button (if shown)
    if (owner.ShowStyleButton)
    {
        layout.StyleButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
        owner._hits.RegisterHitArea("style", layout.StyleButtonRect, HitAreaType.Button);
        buttonX -= buttonWidth;
    }
    
    // Theme button (if shown)
    if (owner.ShowThemeButton)
    {
        layout.ThemeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
        owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
        buttonX -= buttonWidth;
    }
    
    // Custom action button (fallback)
    if (!owner.ShowThemeButton && !owner.ShowStyleButton)
    {
        layout.CustomActionButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
        owner._hits.RegisterHitArea("customAction", layout.CustomActionButtonRect, HitAreaType.Button);
    }
    
    // Icon and title positioning
    int iconX = metrics.IconLeftPadding;
    int iconY = (captionHeight - metrics.IconSize) / 2;
    layout.IconRect = new Rectangle(iconX, iconY, metrics.IconSize, metrics.IconSize);
    if (owner.ShowIcon && owner.Icon != null)
    {
        owner._hits.Register("icon", layout.IconRect, HitAreaType.Icon);
    }
    
    // Title width uses adjusted buttonX
    int titleX = layout.IconRect.Right + metrics.TitleLeftPadding;
    int titleWidth = buttonX - titleX - metrics.ButtonSpacing;
    layout.TitleRect = new Rectangle(titleX, 0, titleWidth, captionHeight);
    
    owner.CurrentLayout = layout;
}
```

## ? Available Painters (32 Total)

All painters now have proper theme/style button support.

## Usage

- Toggle `ShowThemeButton` and `ShowStyleButton` on `BeepiFormPro`.
- `CalculateLayoutAndHitAreas` will register hit areas and compute layout accordingly.

## Current Status (As of Latest Update)

- Progress: 32/32 painters fully implemented (100%)
- Next: Comprehensive testing with different DPI and states.
