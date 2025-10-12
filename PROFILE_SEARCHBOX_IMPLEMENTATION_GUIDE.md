# Profile Button and Search Box Feature Implementation Guide

## Overview
Adding Profile Button and Search Box options to all BeepiFormPro painters.

## Phase 1: Core Infrastructure ✅ COMPLETE

### 1. PainterLayoutInfo (IFormPainter.cs) ✅
Added two new properties:
```csharp
public Rectangle ProfileButtonRect { get; set; }
public Rectangle SearchBoxRect { get; set; }
```

### 2. BeepiFormPro.Core.cs ✅
**Added Private Fields:**
```csharp
private FormRegion _profileButton;
private FormRegion _searchBox;
private bool _showProfileButton = false;
private bool _showSearchBox = false;
```

**Added Internal Properties:**
```csharp
internal FormRegion ProfileButton => _profileButton;
internal FormRegion SearchBox => _searchBox;
```

**Added Public Properties:**
```csharp
public bool ShowProfileButton { get; set; }
public bool ShowSearchBox { get; set; }
```

**Added Events:**
```csharp
public event EventHandler ProfileButtonClicked;
public event EventHandler<string> SearchBoxTextChanged;
```

## Phase 2: Region Initialization (TODO)

### Need to add in InitializeBuiltInRegions method:

#### Profile Button Region
```csharp
// Profile button (user icon)
_profileButton = new FormRegion
{
    Id = "system:profile",
    Dock = RegionDock.Caption,
    OnPaint = (g, r) =>
    {
        if (r.Width <= 0 || r.Height <= 0 || !_showProfileButton) return;
        var style = BeepStyling.GetControlStyle();
        var isHovered = _interact?.IsHovered(_hits?.GetHitArea("profile")) ?? false;
        var isPressed = _interact?.IsPressed(_hits?.GetHitArea("profile")) ?? false;
        
        // Background on hover/press
        if (isPressed)
        {
            var pressed = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetPressed(style);
            using var brush = new SolidBrush(pressed);
            g.FillRectangle(brush, r);
        }
        else if (isHovered)
        {
            var hover = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetHover(style);
            using var brush = new SolidBrush(hover);
            g.FillRectangle(brush, r);
        }

        // Draw profile icon (👤 user icon)
        var fg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetForeground(style);
        using var font = new Font("Segoe UI Symbol", Font.Size + 2, FontStyle.Regular);
        TextRenderer.DrawText(g, "👤", font, r, fg,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
    }
};
```

#### Search Box Region
```csharp
// Search box
_searchBox = new FormRegion
{
    Id = "system:search",
    Dock = RegionDock.Caption,
    OnPaint = (g, r) =>
    {
        if (r.Width <= 0 || r.Height <= 0 || !_showSearchBox) return;
        var style = BeepStyling.GetControlStyle();
        var bg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetBackground(style);
        var fg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetForeground(style);
        var border = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetBorder(style);
        
        // Draw search box background
        using (var brush = new SolidBrush(Color.FromArgb(240, bg)))
        {
            var boxRect = new Rectangle(r.X + 2, r.Y + 4, r.Width - 4, r.Height - 8);
            g.FillRectangle(brush, boxRect);
        }
        
        // Draw border
        using (var pen = new Pen(Color.FromArgb(180, border), 1))
        {
            var boxRect = new Rectangle(r.X + 2, r.Y + 4, r.Width - 4, r.Height - 8);
            g.DrawRectangle(pen, boxRect);
        }
        
        // Draw search icon
        using var font = new Font("Segoe UI Symbol", 9f, FontStyle.Regular);
        var iconRect = new Rectangle(r.X + 6, r.Y, 16, r.Height);
        TextRenderer.DrawText(g, "🔍", font, iconRect, Color.FromArgb(160, fg),
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        
        // Draw placeholder text
        var textRect = new Rectangle(r.X + 24, r.Y, r.Width - 28, r.Height);
        TextRenderer.DrawText(g, "Search...", Font, textRect, Color.FromArgb(128, fg),
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }
};
```

### Event Handling in HandleRegionClick:
```csharp
case "system:profile":
    ProfileButtonClicked?.Invoke(this, EventArgs.Empty);
    break;
    
case "system:search":
    // Handle search box click (could open search dialog or activate inline search)
    break;
```

## Phase 3: Painter Updates (TODO)

Each painter's `CalculateLayoutAndHitAreas` method needs to add profile and search box positioning.

### Button Layout Order (Right to Left):
1. Close Button (rightmost)
2. Maximize Button
3. Minimize Button
4. **Profile Button** (new)
5. Style Button (if shown)
6. Theme Button (if shown)
7. Custom Action Button (if theme/style not shown)
8. **Search Box** (new - left side, flexible width)

### Example Implementation Pattern:
```csharp
public void CalculateLayoutAndHitAreas(BeepiFormPro owner)
{
    var layout = new PainterLayoutInfo();
    var captionHeight = owner.Font.Height + 16;
    layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
    layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);
    
    var buttonSize = new Size(32, captionHeight);
    var buttonY = 0;
    var buttonX = owner.ClientSize.Width - buttonSize.Width;
    
    // System buttons (right side)
    layout.CloseButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
    owner._hits.RegisterHitArea("close", layout.CloseButtonRect, HitAreaType.Button);
    buttonX -= buttonSize.Width;
    
    layout.MaximizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
    owner._hits.RegisterHitArea("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
    buttonX -= buttonSize.Width;
    
    layout.MinimizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
    owner._hits.RegisterHitArea("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
    buttonX -= buttonSize.Width;
    
    // Profile button (NEW)
    if (owner.ShowProfileButton)
    {
        layout.ProfileButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
        owner._hits.RegisterHitArea("profile", layout.ProfileButtonRect, HitAreaType.Button);
        buttonX -= buttonSize.Width;
    }
    
    // Style button
    if (owner.ShowStyleButton)
    {
        layout.StyleButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
        owner._hits.RegisterHitArea("style", layout.StyleButtonRect, HitAreaType.Button);
        buttonX -= buttonSize.Width;
    }
    
    // Theme button
    if (owner.ShowThemeButton)
    {
        layout.ThemeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
        owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
        buttonX -= buttonSize.Width;
    }
    
    // Left side elements
    var leftX = 8;
    
    // Icon
    var iconSize = 16;
    layout.IconRect = new Rectangle(leftX, (captionHeight - iconSize) / 2, iconSize, iconSize);
    owner._hits.RegisterHitArea("icon", layout.IconRect, HitAreaType.Icon);
    leftX += iconSize + 8;
    
    // Search box (NEW)
    if (owner.ShowSearchBox)
    {
        var searchWidth = 200;  // or make it configurable
        layout.SearchBoxRect = new Rectangle(leftX, 0, searchWidth, captionHeight);
        owner._hits.RegisterHitArea("search", layout.SearchBoxRect, HitAreaType.TextBox);
        leftX += searchWidth + 8;
    }
    
    // Title (fills remaining space)
    var titleWidth = buttonX - leftX - 8;
    layout.TitleRect = new Rectangle(leftX, 0, titleWidth, captionHeight);
    owner._hits.RegisterHitArea("title", layout.TitleRect, HitAreaType.Caption);
    
    owner.CurrentLayout = layout;
}
```

## Painters to Update:

1. **FluentFormPainter.cs** - Add profile & search in CalculateLayoutAndHitAreas
2. **GlassFormPainter.cs** - Add profile & search in CalculateLayoutAndHitAreas
3. **MaterialFormPainter.cs** - Add profile & search in CalculateLayoutAndHitAreas
4. **MinimalFormPainter.cs** - Add profile & search in CalculateLayoutAndHitAreas
5. **CartoonFormPainter.cs** - Add profile & search in CalculateLayoutAndHitAreas
6. **ChatBubbleFormPainter.cs** - Add profile & search in CalculateLayoutAndHitAreas
7. **MacOSFormPainter.cs** - Add profile & search in CalculateLayoutAndHitAreas (note: different button layout)

## Phase 4: Drawing Code (TODO)

### In BeepiFormPro.Drawing.cs OnPaint method:
```csharp
// Draw built-in regions
if (FormStyle == FormStyle.Modern || FormStyle == FormStyle.Minimal || FormStyle == FormStyle.Material)
{
    _iconRegion?.OnPaint?.Invoke(e.Graphics, CurrentLayout.IconRect);
    _titleRegion?.OnPaint?.Invoke(e.Graphics, CurrentLayout.TitleRect);
    
    // Draw profile button if visible
    if (ShowProfileButton)
        _profileButton?.OnPaint?.Invoke(e.Graphics, CurrentLayout.ProfileButtonRect);
    
    // Draw search box if visible
    if (ShowSearchBox)
        _searchBox?.OnPaint?.Invoke(e.Graphics, CurrentLayout.SearchBoxRect);
    
    // Draw theme and style buttons if visible
    if (ShowThemeButton)
        _themeButton?.OnPaint?.Invoke(e.Graphics, CurrentLayout.ThemeButtonRect);
        
    if (ShowStyleButton)
        _styleButton?.OnPaint?.Invoke(e.Graphics, CurrentLayout.StyleButtonRect);
    
    // ... rest of buttons
}
```

## Configuration Properties

### Search Box Width
Consider adding a property to control search box width:
```csharp
[Category("Beep Caption")]
[DefaultValue(200)]
[Description("Width of the search box in pixels")]
public int SearchBoxWidth { get; set; } = 200;
```

## Usage Example

```csharp
// Enable profile button
myForm.ShowProfileButton = true;
myForm.ProfileButtonClicked += (s, e) => {
    // Show user profile menu
};

// Enable search box
myForm.ShowSearchBox = true;
myForm.SearchBoxTextChanged += (s, text) => {
    // Handle search
};
```

## Status
- ✅ Phase 1: Core infrastructure complete
- ⏳ Phase 2: Need to implement region initialization in InitializeBuiltInRegions
- ⏳ Phase 3: Need to update all 7 painters' CalculateLayoutAndHitAreas methods
- ⏳ Phase 4: Need to update BeepiFormPro.Drawing.cs OnPaint method

## Next Steps
1. Implement _profileButton and _searchBox initialization in InitializeBuiltInRegions
2. Add event handling in HandleRegionClick
3. Update all 7 painters with profile and search layout calculations
4. Update BeepiFormPro.Drawing.cs to paint the new elements
5. Test in all form styles

