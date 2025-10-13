# BeepCard Refactoring Plan

## Summary
Complete redesign of BeepCard to paint directly without child controls, matching BeepBreadcrumb architecture.

## Changes Required

### 1. Remove All Child Controls
**Current (WRONG):**
```csharp
private BeepImage imageBox;
private BeepLabel headerLabel;
private BeepLabel paragraphLabel;
private BeepButton actionButton;
private BeepButton secondaryButton;
```

**New (CORRECT):**
- Remove these fields completely
- NO InitializeComponents() method
- Paint everything in DrawContent() like BeepBreadcrumb does

### 2. Constructor Changes
```csharp
public BeepCard() : base()
{
    IsChild = true;
    Padding = new Padding(12);
    PainterKind = BaseControlPainterKind.Classic;
    BoundProperty = "ParagraphText";
    Size = new Size(320, 200);
    ApplyThemeToChilds = false;
    CanBeHovered = true;
    
    InitializePainter();
    ApplyDesignTimeData(); // NEW: Add dummy data in designer
    ApplyTheme();
}
```

### 3. New DrawContent Implementation
```csharp
protected override void DrawContent(Graphics g)
{
    base.DrawContent(g);
    
    // Build layout context with design-time dummy data if needed
    var ctx = BuildLayoutContext();
    
    // Let painter adjust layout
    ctx = _painter?.AdjustLayout(DrawingRect, ctx) ?? ctx;
    _layoutContext = ctx;
    
    // Draw background (optional - BaseControl handles container)
    if (UseThemeColors && _currentTheme != null)
    {
        _painter?.DrawBackground(g, ctx);
    }
    else
    {
        BeepStyling.PaintStyleBackground(g, DrawingRect, ControlStyle);
    }
    
    // Paint image directly with StyledImagePainter
    if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
    {
        string pathToPaint = GetImagePath();
        if (!string.IsNullOrEmpty(pathToPaint))
        {
            try
            {
                StyledImagePainter.Paint(g, ctx.ImageRect, pathToPaint);
            }
            catch { /* swallow */ }
        }
    }
    
    // Paint text directly with TextRenderer
    if (!ctx.HeaderRect.IsEmpty && !string.IsNullOrEmpty(headerText))
    {
        TextRenderer.DrawText(g, headerText, 
            BeepThemesManager.ToFont(_currentTheme.CardHeaderStyle),
            ctx.HeaderRect,
            _currentTheme.CardHeaderStyle.TextColor,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
    }
    
    if (!ctx.ParagraphRect.IsEmpty && !string.IsNullOrEmpty(paragraphText))
    {
        TextRenderer.DrawText(g, paragraphText,
            BeepThemesManager.ToFont(_currentTheme.Paragraph),
            ctx.ParagraphRect,
            _currentTheme.CardTextForeColor,
            TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak);
    }
    
    // Paint buttons directly (simple rectangles + text)
    if (ctx.ShowButton && !ctx.ButtonRect.IsEmpty)
    {
        PaintButton(g, ctx.ButtonRect, buttonText, _accentColor, IsButtonHovered("Button"));
    }
    
    if (ctx.ShowSecondaryButton && !ctx.SecondaryButtonRect.IsEmpty)
    {
        PaintButton(g, ctx.SecondaryButtonRect, secondaryButtonText, 
            Color.FromArgb(120, _accentColor), IsButtonHovered("SecondaryButton"));
    }
    
    // Let painter draw accents (badges, stars, chips, etc.)
    _painter?.DrawForegroundAccents(g, ctx);
    
    // Register hit areas for interaction
    RefreshHitAreas(ctx);
}
```

### 4. Helper Methods Needed
```csharp
private LayoutContext BuildLayoutContext()
{
    return new LayoutContext
    {
        DrawingRect = DrawingRect,
        ShowImage = !string.IsNullOrEmpty(imagePath),
        ShowButton = showButton,
        ShowSecondaryButton = showSecondaryButton,
        Radius = BorderRadius,
        AccentColor = _accentColor,
        Tags = _tags,
        BadgeText1 = _badgeText1,
        Badge1BackColor = _badge1BackColor,
        Badge1ForeColor = _badge1ForeColor,
        BadgeText2 = _badgeText2,
        Badge2BackColor = _badge2BackColor,
        Badge2ForeColor = _badge2ForeColor,
        SubtitleText = _subtitleText,
        StatusText = _statusText,
        StatusColor = _statusColor,
        ShowStatus = _showStatus,
        Rating = _rating,
        ShowRating = _showRating
    };
}

private string GetImagePath()
{
    if (!string.IsNullOrEmpty(imagePath))
        return imagePath;
    
    // Design-time sample
    if (DesignMode)
        return GetDesignTimeSampleImage(_style);
    
    return null;
}

private void PaintButton(Graphics g, Rectangle rect, string text, Color backColor, bool isHovered)
{
    var color = isHovered ? ControlPaint.Light(backColor) : backColor;
    using (var brush = new SolidBrush(color))
    using (var path = CardRenderingHelpers.CreateRoundedPath(rect, 6))
    {
        g.FillPath(brush, path);
    }
    
    TextRenderer.DrawText(g, text, Font, rect, Color.White,
        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
}

private bool IsButtonHovered(string areaName)
{
    return _hoveredArea == areaName;
}
```

### 5. Hit Area Registration (like BeepBreadcrumb)
```csharp
private void RefreshHitAreas(LayoutContext ctx)
{
    ClearHitList();
    
    if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
    {
        AddHitArea("Image", ctx.ImageRect, null, () =>
        {
            ImageClicked?.Invoke(this, new BeepEventDataArgs("ImageClicked", this));
        });
    }
    
    if (!ctx.HeaderRect.IsEmpty)
    {
        AddHitArea("Header", ctx.HeaderRect, null, () =>
        {
            HeaderClicked?.Invoke(this, new BeepEventDataArgs("HeaderClicked", this));
        });
    }
    
    if (!ctx.ParagraphRect.IsEmpty)
    {
        AddHitArea("Paragraph", ctx.ParagraphRect, null, () =>
        {
            ParagraphClicked?.Invoke(this, new BeepEventDataArgs("ParagraphClicked", this));
        });
    }
    
    if (ctx.ShowButton && !ctx.ButtonRect.IsEmpty)
    {
        AddHitArea("Button", ctx.ButtonRect, null, () =>
        {
            ButtonClicked?.Invoke(this, new BeepEventDataArgs("ButtonClicked", this));
        });
    }
    
    if (ctx.ShowSecondaryButton && !ctx.SecondaryButtonRect.IsEmpty)
    {
        AddHitArea("SecondaryButton", ctx.SecondaryButtonRect, null, () =>
        {
            ButtonClicked?.Invoke(this, new BeepEventDataArgs("SecondaryButtonClicked", this));
        });
    }
    
    if (!ctx.BadgeRect.IsEmpty)
    {
        AddHitArea("Badge", ctx.BadgeRect, null, () =>
        {
            BadgeClicked?.Invoke(this, new BeepEventDataArgs("BadgeClicked", this));
        });
    }
    
    if (ctx.ShowRating && !ctx.RatingRect.IsEmpty)
    {
        AddHitArea("Rating", ctx.RatingRect, null, () =>
        {
            RatingClicked?.Invoke(this, new BeepEventDataArgs("RatingClicked", this));
        });
    }
}
```

### 6. Mouse Hover Tracking
```csharp
protected override void OnMouseMove(MouseEventArgs e)
{
    base.OnMouseMove(e);
    
    string newHovered = null;
    if (_layoutContext != null)
    {
        if (_layoutContext.ButtonRect.Contains(e.Location))
            newHovered = "Button";
        else if (_layoutContext.SecondaryButtonRect.Contains(e.Location))
            newHovered = "SecondaryButton";
        else if (_layoutContext.ImageRect.Contains(e.Location))
            newHovered = "Image";
        else if (_layoutContext.BadgeRect.Contains(e.Location))
            newHovered = "Badge";
        else if (_layoutContext.RatingRect.Contains(e.Location))
            newHovered = "Rating";
    }
    
    if (newHovered != _hoveredArea)
    {
        _hoveredArea = newHovered;
        Invalidate();
    }
}
```

### 7. Design-Time Dummy Data
```csharp
private void ApplyDesignTimeData()
{
    if (!DesignMode) return;
    
    switch (_style)
    {
        case CardStyle.ProductCard:
            headerText = "Wireless Headphones";
            paragraphText = "Premium sound quality";
            _subtitleText = "$149.99";
            _rating = 4;
            _showRating = true;
            _badgeText1 = "SALE";
            _badge1BackColor = Color.FromArgb(244, 67, 54);
            _badge1ForeColor = Color.White;
            buttonText = "Add to Cart";
            showButton = true;
            imagePath = "hamburger.svg"; // Sample from GFX
            break;
            
        case CardStyle.ProfileCard:
            headerText = "Sarah Johnson";
            paragraphText = "Senior UI/UX Designer";
            _statusText = "Available for work";
            _showStatus = true;
            _statusColor = Color.Green;
            _badgeText1 = "PRO";
            _badge1BackColor = Color.FromArgb(33, 150, 243);
            buttonText = "View Profile";
            imagePath = "person.svg";
            break;
            
        case CardStyle.TestimonialCard:
            headerText = "John Smith";
            paragraphText = "\"This product exceeded my expectations. The quality is outstanding and the customer service is top-notch!\"";
            _rating = 5;
            _showRating = true;
            imagePath = "person.svg";
            showButton = false;
            break;
            
        case CardStyle.StatCard:
            headerText = "Total Revenue";
            _subtitleText = "$284,592";
            _statusText = "+12.5%";
            _showStatus = true;
            _statusColor = Color.FromArgb(76, 175, 80);
            paragraphText = "vs last month";
            showButton = false;
            imagePath = "sum.svg";
            break;
            
        case CardStyle.ContentCard:
            headerText = "Master Modern UI Design";
            paragraphText = "Learn the latest design trends";
            _tags = new List<string> { "Design", "UI/UX", "Web" };
            _badgeText1 = "PREMIUM";
            _badge1BackColor = Color.FromArgb(255, 193, 7);
            buttonText = "Enroll Now";
            showButton = true;
            imagePath = "beep.svg";
            break;
            
        case CardStyle.ListCard:
            headerText = "James Anderson";
            _subtitleText = "Product Manager";
            paragraphText = "15+ years experience";
            _rating = 5;
            _showRating = true;
            imagePath = "person.svg";
            showButton = false;
            break;
            
        case CardStyle.EventCard:
            headerText = "Design Workshop 2024";
            _subtitleText = "2:00 PM - 5:00 PM • Room 301";
            paragraphText = "Join us for an interactive workshop";
            _statusText = "MAR\n15";
            _tags = new List<string> { "Workshop", "Design" };
            buttonText = "RSVP";
            showButton = true;
            break;
            
        case CardStyle.SocialMediaCard:
            headerText = "Emily Davis";
            _subtitleText = "@emilydesigns";
            paragraphText = "Just launched our new design system! 🎨";
            _statusText = "3h ago";
            _tags = new List<string> { "#design", "#ui" };
            buttonText = "Like";
            secondaryButtonText = "Share";
            showSecondaryButton = true;
            imagePath = "person.svg";
            break;
            
        case CardStyle.DialogCard:
            headerText = "Confirm Action";
            paragraphText = "Are you sure you want to continue? This action cannot be undone.";
            buttonText = "Confirm";
            secondaryButtonText = "Cancel";
            showSecondaryButton = true;
            imagePath = "triangle-warning.svg";
            break;
            
        case CardStyle.BasicCard:
            headerText = "Simple Card";
            paragraphText = "Clean and minimal design for general content";
            buttonText = "Learn More";
            showButton = true;
            break;
            
        case CardStyle.CompactProfile:
            headerText = "Alex Martinez";
            _subtitleText = "Software Engineer";
            _statusText = "Online";
            _showStatus = true;
            _statusColor = Color.Green;
            _badgeText1 = "VERIFIED";
            imagePath = "person.svg";
            showButton = false;
            break;
            
        case CardStyle.ProductCompactCard:
            headerText = "MacBook Pro 16\"";
            _subtitleText = "$2,499";
            _rating = 5;
            _showRating = true;
            _badgeText1 = "NEW";
            _badge1BackColor = Color.FromArgb(76, 175, 80);
            imagePath = "hamburger.svg";
            showButton = false;
            break;
            
        case CardStyle.FeatureCard:
            headerText = "Easy Integration";
            paragraphText = "Seamlessly integrate with your existing workflow";
            imagePath = "check-circle.svg";
            showButton = false;
            break;
    }
}
```

## Missing Card Styles from Images

Add these new styles to CardStyle enum:
- PricingCard (from pricing table image)
- FoodCard (from food menu images)
- HotelCard (from hotel room images)

## Visual Polish Needed Per Painter
1. ProductCard: Rounded image corners, proper spacing
2. ProfileCard: Circular avatar, gradient background
3. TestimonialCard: Quote marks, proper typography
4. All: Proper hover states with color shifts

## Implementation Order
1. ✓ Remove child control fields
2. ✓ Update constructor
3. → Rewrite DrawContent method
4. → Add hit area registration
5. → Add design-time dummy data
6. → Add missing card styles
7. → Polish each painter visually
