# Caption Renderer Visual Style Templates

This document provides code templates for updating each caption renderer to have distinctive visual styles.

## Template Structure

Each renderer should follow this pattern:

```csharp
internal sealed class [Style]CaptionRenderer : ICaptionRenderer
{
    private Form _host;
    private Func<IBeepTheme> _theme;
    private Func<int> _captionHeight;
    private bool _showButtons = true;

    private Rectangle _closeRect, _maxRect, _minRect;
    private bool _hoverClose, _hoverMax, _hoverMin;

    public void UpdateHost(Form host, Func<IBeepTheme> themeProvider, Func<int> captionHeightProvider)
    {
        _host = host; _theme = themeProvider; _captionHeight = captionHeightProvider;
    }
    
    public void UpdateTheme(IBeepTheme theme) { /* style-specific theme handling */ }
    public void SetShowSystemButtons(bool show) => _showButtons = show;

    public void GetTitleInsets(Rectangle captionBounds, float scale, out int leftInset, out int rightInset)
    {
        // Calculate based on button position and size
    }

    public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
    {
        // Draw caption background (optional gradient/effects)
        // Position and draw buttons with style-specific appearance
        // Handle hover states with style-specific effects
    }

    public bool OnMouseMove(Point location, out Rectangle invalidatedArea)
    {
        // Track hover states and return invalidation area
    }

    public bool OnMouseDown(Point location, out CaptionButtonKind button, out Rectangle invalidatedArea)
    {
        // Handle button clicks
    }

    public void OnMouseLeave(out Rectangle invalidatedArea)
    {
        // Clear hover states
    }

    public bool IsPointInSystemButtons(Point location)
    {
        // Check if point is over any button
    }
}
```

## 1. Metro Style Template

```csharp
public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
{
    invalidatedArea = Rectangle.Empty;
    int pad = (int)(8 * scale);
    int btn = Math.Max(24, (int)(_captionHeight() - 8 * scale));
    int top = captionBounds.Top + Math.Max(2, (captionBounds.Height - btn) / 2);
    
    if (!_showButtons) { _closeRect = _maxRect = _minRect = Rectangle.Empty; return; }

    // Position buttons (right-aligned)
    int x = captionBounds.Right - pad - btn;
    _closeRect = new Rectangle(x, top, btn, btn);
    x -= (btn + pad);
    _maxRect = new Rectangle(x, top, btn, btn);
    x -= (btn + pad);
    _minRect = new Rectangle(x, top, btn, btn);

    using var pen = new Pen(theme?.AppBarButtonForeColor ?? _host.ForeColor, 1.5f) { Alignment = PenAlignment.Center };
    
    // Metro style: FLAT buttons, NO background fill
    // Hover effect: accent underline only
    
    // Minimize
    if (_hoverMin)
    {
        using var accent = new Pen(theme?.AccentColor ?? Color.FromArgb(0, 120, 215), 2f);
        g.DrawLine(accent, _minRect.Left, _minRect.Bottom - 2, _minRect.Right, _minRect.Bottom - 2);
    }
    CaptionGlyphProvider.DrawMinimize(g, pen, _minRect, scale);
    
    // Maximize/Restore
    if (_hoverMax)
    {
        using var accent = new Pen(theme?.AccentColor ?? Color.FromArgb(0, 120, 215), 2f);
        g.DrawLine(accent, _maxRect.Left, _maxRect.Bottom - 2, _maxRect.Right, _maxRect.Bottom - 2);
    }
    if (windowState == FormWindowState.Maximized)
        CaptionGlyphProvider.DrawRestore(g, pen, _maxRect, scale);
    else
        CaptionGlyphProvider.DrawMaximize(g, pen, _maxRect, scale);
    
    // Close
    if (_hoverClose)
    {
        using var accent = new Pen(theme?.ButtonErrorBackColor ?? Color.Red, 2f);
        g.DrawLine(accent, _closeRect.Left, _closeRect.Bottom - 2, _closeRect.Right, _closeRect.Bottom - 2);
    }
    CaptionGlyphProvider.DrawClose(g, pen, _closeRect, scale);
}
```

## 2. Material (Mac-like) Style Template

```csharp
public void GetTitleInsets(Rectangle captionBounds, float scale, out int leftInset, out int rightInset)
{
    int pad = (int)(8 * scale);
    int btn = _showButtons ? Math.Max(24, (int)(_captionHeight() - 8 * scale)) : 0;
    int buttons = _showButtons ? 3 : 0;
    // Material: buttons on LEFT, title centered
    leftInset = buttons > 0 ? (buttons * btn + (buttons + 1) * pad) : pad;
    rightInset = pad;
}

public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
{
    invalidatedArea = Rectangle.Empty;
    int pad = (int)(8 * scale);
    int btn = Math.Max(20, (int)(_captionHeight() - 12 * scale)); // Slightly smaller
    int top = captionBounds.Top + Math.Max(2, (captionBounds.Height - btn) / 2);
    
    if (!_showButtons) { _closeRect = _maxRect = _minRect = Rectangle.Empty; return; }

    // Material: buttons on LEFT side (Mac style)
    int x = captionBounds.Left + pad;
    
    // Close (red circle) - LEFT-MOST
    _closeRect = new Rectangle(x, top, btn, btn);
    Color closeColor = Color.FromArgb(_hoverClose ? 255 : 220, 255, 95, 86);
    using (var closeBrush = new SolidBrush(closeColor))
        g.FillEllipse(closeBrush, _closeRect);
    
    if (_hoverClose)
    {
        using var pen = new Pen(Color.FromArgb(100, 0, 0, 0), 1.0f);
        CaptionGlyphProvider.DrawClose(g, pen, _closeRect, scale * 0.5f); // Smaller glyph
    }
    
    x += (btn + pad);
    
    // Minimize (yellow circle)
    _minRect = new Rectangle(x, top, btn, btn);
    Color minColor = Color.FromArgb(_hoverMin ? 255 : 220, 255, 189, 46);
    using (var minBrush = new SolidBrush(minColor))
        g.FillEllipse(minBrush, _minRect);
    
    if (_hoverMin)
    {
        using var pen = new Pen(Color.FromArgb(100, 0, 0, 0), 1.0f);
        CaptionGlyphProvider.DrawMinimize(g, pen, _minRect, scale * 0.5f);
    }
    
    x += (btn + pad);
    
    // Maximize (green circle)
    _maxRect = new Rectangle(x, top, btn, btn);
    Color maxColor = Color.FromArgb(_hoverMax ? 255 : 220, 53, 201, 71);
    using (var maxBrush = new SolidBrush(maxColor))
        g.FillEllipse(maxBrush, _maxRect);
    
    if (_hoverMax)
    {
        using var pen = new Pen(Color.FromArgb(100, 0, 0, 0), 1.0f);
        if (windowState == FormWindowState.Maximized)
            CaptionGlyphProvider.DrawRestore(g, pen, _maxRect, scale * 0.5f);
        else
            CaptionGlyphProvider.DrawMaximize(g, pen, _maxRect, scale * 0.5f);
    }
}
```

## 3. Neon Style Template

```csharp
public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
{
    invalidatedArea = Rectangle.Empty;
    
    // Neon: Dark background with glowing buttons
    using var bgBrush = new SolidBrush(Color.FromArgb(20, 20, 30));
    g.FillRectangle(bgBrush, captionBounds);
    
    int pad = (int)(8 * scale);
    int btn = Math.Max(24, (int)(_captionHeight() - 8 * scale));
    int top = captionBounds.Top + Math.Max(2, (captionBounds.Height - btn) / 2);
    
    if (!_showButtons) { _closeRect = _maxRect = _minRect = Rectangle.Empty; return; }

    int x = captionBounds.Right - pad - btn;
    _closeRect = new Rectangle(x, top, btn, btn);
    x -= (btn + pad);
    _maxRect = new Rectangle(x, top, btn, btn);
    x -= (btn + pad);
    _minRect = new Rectangle(x, top, btn, btn);

    // Neon glow effect
    float glowWidth = _hoverMin ? 3f : 1.5f;
    Color minGlow = Color.FromArgb(0, 255, 255); // Cyan
    using (var glowPen = new Pen(minGlow, glowWidth) { Alignment = PenAlignment.Outset })
    {
        g.DrawRectangle(glowPen, _minRect);
        if (_hoverMin)
        {
            using var innerGlow = new Pen(Color.FromArgb(100, 0, 255, 255), glowWidth * 0.5f);
            var innerRect = Rectangle.Inflate(_minRect, -2, -2);
            g.DrawRectangle(innerGlow, innerRect);
        }
    }
    using var pen = new Pen(minGlow, 1.5f);
    CaptionGlyphProvider.DrawMinimize(g, pen, _minRect, scale);
    
    // Similar for max and close with different colors
    glowWidth = _hoverMax ? 3f : 1.5f;
    Color maxGlow = Color.FromArgb(255, 0, 255); // Magenta
    using (var glowPen = new Pen(maxGlow, glowWidth) { Alignment = PenAlignment.Outset })
    {
        g.DrawRectangle(glowPen, _maxRect);
        if (_hoverMax)
        {
            using var innerGlow = new Pen(Color.FromArgb(100, 255, 0, 255), glowWidth * 0.5f);
            var innerRect = Rectangle.Inflate(_maxRect, -2, -2);
            g.DrawRectangle(innerGlow, innerRect);
        }
    }
    using (var pen2 = new Pen(maxGlow, 1.5f))
    {
        if (windowState == FormWindowState.Maximized)
            CaptionGlyphProvider.DrawRestore(g, pen2, _maxRect, scale);
        else
            CaptionGlyphProvider.DrawMaximize(g, pen2, _maxRect, scale);
    }
    
    glowWidth = _hoverClose ? 3f : 1.5f;
    Color closeGlow = Color.FromArgb(255, 50, 50); // Red
    using (var glowPen = new Pen(closeGlow, glowWidth) { Alignment = PenAlignment.Outset })
    {
        g.DrawRectangle(glowPen, _closeRect);
        if (_hoverClose)
        {
            using var innerGlow = new Pen(Color.FromArgb(100, 255, 50, 50), glowWidth * 0.5f);
            var innerRect = Rectangle.Inflate(_closeRect, -2, -2);
            g.DrawRectangle(innerGlow, innerRect);
        }
    }
    using (var pen3 = new Pen(closeGlow, 1.5f))
        CaptionGlyphProvider.DrawClose(g, pen3, _closeRect, scale);
}
```

## 4. Gaming Style Template

```csharp
// Helper method to create hexagonal button path
private GraphicsPath CreateHexPath(Rectangle rect)
{
    var path = new GraphicsPath();
    int inset = rect.Width / 6;
    
    Point[] points = new Point[]
    {
        new Point(rect.Left + inset, rect.Top),
        new Point(rect.Right - inset, rect.Top),
        new Point(rect.Right, rect.Top + rect.Height / 2),
        new Point(rect.Right - inset, rect.Bottom),
        new Point(rect.Left + inset, rect.Bottom),
        new Point(rect.Left, rect.Top + rect.Height / 2)
    };
    
    path.AddPolygon(points);
    return path;
}

public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
{
    invalidatedArea = Rectangle.Empty;
    
    // Gaming: Dark background with tech grid pattern
    using var bgBrush = new SolidBrush(Color.FromArgb(15, 15, 25));
    g.FillRectangle(bgBrush, captionBounds);
    
    int pad = (int)(10 * scale); // More spacing
    int btn = Math.Max(26, (int)(_captionHeight() - 6 * scale)); // Larger buttons
    int top = captionBounds.Top + Math.Max(2, (captionBounds.Height - btn) / 2);
    
    if (!_showButtons) { _closeRect = _maxRect = _minRect = Rectangle.Empty; return; }

    int x = captionBounds.Right - pad - btn;
    _closeRect = new Rectangle(x, top, btn, btn);
    x -= (btn + pad);
    _maxRect = new Rectangle(x, top, btn, btn);
    x -= (btn + pad);
    _minRect = new Rectangle(x, top, btn, btn);

    g.SmoothingMode = SmoothingMode.AntiAlias;
    
    // Minimize - Green hexagon
    using (var hexPath = CreateHexPath(_minRect))
    {
        Color minColor = Color.FromArgb(0, 255, 100);
        using var outline = new Pen(minColor, _hoverMin ? 2.5f : 1.5f);
        g.DrawPath(outline, hexPath);
        
        if (_hoverMin)
        {
            using var glow = new Pen(Color.FromArgb(80, 0, 255, 100), 4f) { Alignment = PenAlignment.Outset };
            g.DrawPath(glow, hexPath);
        }
        
        using var pen = new Pen(minColor, 1.5f);
        CaptionGlyphProvider.DrawMinimize(g, pen, _minRect, scale);
    }
    
    // Maximize - Blue hexagon
    using (var hexPath = CreateHexPath(_maxRect))
    {
        Color maxColor = Color.FromArgb(0, 150, 255);
        using var outline = new Pen(maxColor, _hoverMax ? 2.5f : 1.5f);
        g.DrawPath(outline, hexPath);
        
        if (_hoverMax)
        {
            using var glow = new Pen(Color.FromArgb(80, 0, 150, 255), 4f) { Alignment = PenAlignment.Outset };
            g.DrawPath(glow, hexPath);
        }
        
        using var pen = new Pen(maxColor, 1.5f);
        if (windowState == FormWindowState.Maximized)
            CaptionGlyphProvider.DrawRestore(g, pen, _maxRect, scale);
        else
            CaptionGlyphProvider.DrawMaximize(g, pen, _maxRect, scale);
    }
    
    // Close - Red hexagon
    using (var hexPath = CreateHexPath(_closeRect))
    {
        Color closeColor = Color.FromArgb(255, 50, 50);
        using var outline = new Pen(closeColor, _hoverClose ? 2.5f : 1.5f);
        g.DrawPath(outline, hexPath);
        
        if (_hoverClose)
        {
            using var glow = new Pen(Color.FromArgb(80, 255, 50, 50), 4f) { Alignment = PenAlignment.Outset };
            g.DrawPath(glow, hexPath);
        }
        
        using var pen = new Pen(closeColor, 1.5f);
        CaptionGlyphProvider.DrawClose(g, pen, _closeRect, scale);
    }
}
```

## 5. Industrial Style Template

```csharp
public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
{
    invalidatedArea = Rectangle.Empty;
    int pad = (int)(8 * scale);
    int btn = Math.Max(24, (int)(_captionHeight() - 8 * scale));
    int top = captionBounds.Top + Math.Max(2, (captionBounds.Height - btn) / 2);
    
    if (!_showButtons) { _closeRect = _maxRect = _minRect = Rectangle.Empty; return; }

    int x = captionBounds.Right - pad - btn;
    _closeRect = new Rectangle(x, top, btn, btn);
    x -= (btn + pad);
    _maxRect = new Rectangle(x, top, btn, btn);
    x -= (btn + pad);
    _minRect = new Rectangle(x, top, btn, btn);

    // Industrial: Metallic gradient buttons
    Color darkMetal = Color.FromArgb(60, 60, 70);
    Color lightMetal = Color.FromArgb(120, 120, 130);
    
    // Minimize
    using (var gradBrush = new LinearGradientBrush(_minRect, 
        _hoverMin ? lightMetal : darkMetal, 
        _hoverMin ? Color.FromArgb(140, 140, 150) : Color.FromArgb(80, 80, 90), 
        LinearGradientMode.Vertical))
    {
        g.FillRectangle(gradBrush, _minRect);
    }
    using (var border = new Pen(Color.FromArgb(180, 180, 190), 1f))
        g.DrawRectangle(border, _minRect);
    
    using var pen = new Pen(Color.FromArgb(220, 220, 230), 1.5f);
    CaptionGlyphProvider.DrawMinimize(g, pen, _minRect, scale);
    
    // Similar for max and close buttons
    // ... (repeat pattern for _maxRect and _closeRect)
}
```

## 6. HighContrast Style Template

```csharp
public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
{
    invalidatedArea = Rectangle.Empty;
    int pad = (int)(8 * scale);
    int btn = Math.Max(28, (int)(_captionHeight() - 6 * scale)); // Larger for visibility
    int top = captionBounds.Top + Math.Max(2, (captionBounds.Height - btn) / 2);
    
    if (!_showButtons) { _closeRect = _maxRect = _minRect = Rectangle.Empty; return; }

    int x = captionBounds.Right - pad - btn;
    _closeRect = new Rectangle(x, top, btn, btn);
    x -= (btn + pad);
    _maxRect = new Rectangle(x, top, btn, btn);
    x -= (btn + pad);
    _minRect = new Rectangle(x, top, btn, btn);

    // HighContrast: THICK black outlines, white background
    float strokeWidth = 3f * scale; // Very thick
    
    // Minimize
    using (var bgBrush = new SolidBrush(Color.White))
        g.FillRectangle(bgBrush, _minRect);
    using (var outline = new Pen(Color.Black, strokeWidth))
        g.DrawRectangle(outline, _minRect);
    using (var pen = new Pen(Color.Black, strokeWidth))
        CaptionGlyphProvider.DrawMinimize(g, pen, _minRect, scale);
    
    // Maximize
    using (var bgBrush = new SolidBrush(Color.White))
        g.FillRectangle(bgBrush, _maxRect);
    using (var outline = new Pen(Color.Black, strokeWidth))
        g.DrawRectangle(outline, _maxRect);
    using (var pen = new Pen(Color.Black, strokeWidth))
    {
        if (windowState == FormWindowState.Maximized)
            CaptionGlyphProvider.DrawRestore(g, pen, _maxRect, scale);
        else
            CaptionGlyphProvider.DrawMaximize(g, pen, _maxRect, scale);
    }
    
    // Close
    using (var bgBrush = new SolidBrush(Color.White))
        g.FillRectangle(bgBrush, _closeRect);
    using (var outline = new Pen(Color.Black, strokeWidth))
        g.DrawRectangle(outline, _closeRect);
    using (var pen = new Pen(Color.Black, strokeWidth))
        CaptionGlyphProvider.DrawClose(g, pen, _closeRect, scale);
}
```

## Quick Reference: Key Differences

| Style | Button Shape | Colors | Hover Effect | Position |
|-------|-------------|---------|--------------|----------|
| Windows | Rectangle | Theme | Light fill | Right |
| Metro | Rectangle | Theme | Underline | Right |
| Material | Circle | RGB fixed | Brightness | LEFT |
| Gnome | Rectangle | Theme | None | Right |
| Kde | Rounded rect | Theme + gradient | Light blue | Right |
| Neon | Rectangle | Cyan/Magenta | Intense glow | Right |
| Gaming | Hexagon | RGB bright | Glow | Right |
| Industrial | Rectangle | Gray gradient | Lighter metal | Right |
| HighContrast | Rectangle | Black/White | None | Right |
| Soft | Rounded rect | Pastels | Light fill | Right |
| Artistic | Circle | Rainbow | Color shift | Right |
| Retro | Square | Bright magenta/cyan | Pixel border | Right |

## Testing Each Renderer

For each renderer, test:
1. Normal hover states
2. Click feedback
3. Maximized vs Normal state
4. DPI scaling (100%, 150%, 200%)
5. Light and dark themes
6. Title text doesn't overlap buttons
7. Extra buttons (theme/style) don't collide

