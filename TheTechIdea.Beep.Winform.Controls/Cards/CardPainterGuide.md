# BeepCard Painter Helper Guide

## Overview
This guide explains how to create custom card painters for the BeepCard system. Each painter implements the `ICardPainter` interface and provides a specific visual style and layout.

## Architecture

### Core Components
- **`ICardPainter`** - Interface defining the painter contract
- **`CardPainterBase`** - Base class with common functionality
- **`LayoutContext`** - Data structure containing layout information
- **`CardRenderingHelpers`** - Static utility methods for common rendering tasks

## Available Card Styles

### 1. **ProfileCard** (`ProfileCardPainter.cs`)
- **Purpose**: Vertical profile cards with large images
- **Use Cases**: User profiles, team member cards, contact cards
- **Key Features**: Large image (40% of height), status indicator, badges, full-width button

### 2. **CompactProfile** (`CompactProfileCardPainter.cs`)
- **Purpose**: Smaller profile variants for lists
- **Use Cases**: User lists, contact lists, team rosters
- **Key Features**: Small circular avatar (48px), horizontal layout, compact badges

### 3. **ContentCard** (`ContentCardPainter.cs`)
- **Purpose**: Banner image top with content below
- **Use Cases**: Blog posts, course cards, article previews
- **Key Features**: Banner image (45% of height), tag chips, badge overlay

### 4. **FeatureCard** (`FeatureCardPainter.cs`)
- **Purpose**: Icon + title + description layout
- **Use Cases**: App features, service descriptions, capability highlights
- **Key Features**: Centered icon (64px), accent circle, clean design

### 5. **ListCard** (`ListCardPainter.cs`)
- **Purpose**: Horizontal layout with avatar/icon
- **Use Cases**: Directory listings, search results, contact lists
- **Key Features**: 40px avatar, multi-line content, rating or badge on right

### 6. **TestimonialCard** (`TestimonialCardPainter.cs`)
- **Purpose**: Quote style with avatar and rating
- **Use Cases**: Customer reviews, testimonials, feedback
- **Key Features**: Quote content, large quote mark, author info, 5-star rating

### 7. **DialogCard** (`DialogCardPainter.cs`)
- **Purpose**: Simple modal-style cards
- **Use Cases**: Confirmation dialogs, modal content, alerts
- **Key Features**: Optional icon, clear hierarchy, two-button layout

### 8. **BasicCard** (`BasicCardPainter.cs`)
- **Purpose**: Minimal card for general content
- **Use Cases**: General information, simple content, fallback style
- **Key Features**: Simple layout, minimal accent line, optional button

### 9. **ProductCard** (`ProductCardPainter.cs`)
- **Purpose**: E-commerce product display
- **Use Cases**: Product catalogs, shopping interfaces, marketplace
- **Key Features**: Square product image, price display, rating stars, sale badges

### 10. **ProductCompactCard** (`ProductCompactCardPainter.cs`)
- **Purpose**: Horizontal compact product for lists
- **Use Cases**: Product search results, comparison lists, cart items
- **Key Features**: Small product image, price on right, compact rating

### 11. **StatCard** (`StatCardPainter.cs`)
- **Purpose**: Statistics and KPIs display
- **Use Cases**: Dashboards, analytics, metrics displays
- **Key Features**: Large value display, trend indicators, accent line, icons

### 12. **EventCard** (`EventCardPainter.cs`)
- **Purpose**: Events, appointments, time-based content
- **Use Cases**: Calendar events, appointments, schedules
- **Key Features**: Colored accent bar, date block, time info, category tags

### 13. **SocialMediaCard** (`SocialMediaCardPainter.cs`)
- **Purpose**: Social posts, feeds, announcements
- **Use Cases**: Social media feeds, notifications, announcements
- **Key Features**: User avatar, timestamp, hashtags, interaction buttons

## Creating a Custom Painter

### Step 1: Create the Painter Class
```csharp
using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// CustomCard - Description of your custom card style
    /// </summary>
    internal sealed class CustomCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            // Define your layout here
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            // Set up rectangles for different areas
            ctx.HeaderRect = new Rectangle(/* your layout */);
            ctx.ImageRect = new Rectangle(/* your layout */);
            // ... etc
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Draw the card background
            DrawSoftShadow(g, ctx.DrawingRect, 12, layers: 4, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.CardBackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 12);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw decorative elements, badges, etc.
            CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, /* parameters */);
        }

        public override void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Optional: Add custom clickable areas
            if (!ctx.CustomRect.IsEmpty)
                owner.AddHitArea("CustomArea", ctx.CustomRect, null, () => notifyAreaHit?.Invoke("CustomArea", ctx.CustomRect));
        }
    }
}
```

### Step 2: Add to CardStyle Enum
```csharp
public enum CardStyle
{
    // ... existing styles ...
    CustomCard    // Your new style
}
```

### Step 3: Update BeepCard InitializePainter()
```csharp
private void InitializePainter()
{
    switch (_style)
    {
        // ... existing cases ...
        case CardStyle.CustomCard:
            _painter = new CustomCardPainter();
            break;
        // ...
    }
}
```

## Layout Guidelines

### Standard Padding
- **Small cards**: 12-16px padding
- **Medium cards**: 16-20px padding
- **Large cards**: 20-24px padding

### Common Dimensions
- **Compact avatars**: 32-48px
- **Standard avatars**: 48-64px
- **Feature icons**: 64-80px
- **Button heights**: 24-40px
- **Badge heights**: 16-24px

### Color Usage
- **Primary content**: Use theme colors
- **Accents**: Use `ctx.AccentColor`
- **Status indicators**: Use semantic colors (green, red, amber)
- **Badges**: Use contrasting colors for visibility

## Helper Methods Available

### CardRenderingHelpers Static Methods
```csharp
// Draw chip/tag elements
CardRenderingHelpers.DrawChips(g, owner, area, accentColor, tags);

// Draw pill-shaped badges
CardRenderingHelpers.DrawBadge(g, rect, text, backColor, foreColor, font);

// Draw rating stars
CardRenderingHelpers.DrawStars(g, rect, rating, color);

// Draw status indicators
CardRenderingHelpers.DrawStatus(g, rect, text, color, font);

// Create rounded paths
CardRenderingHelpers.CreateRoundedPath(rect, radius);

// Draw soft shadows
CardRenderingHelpers.DrawSoftShadow(g, rect, radius, layers, offset);
```

### CardPainterBase Protected Methods
```csharp
// Create rounded graphics paths
protected GraphicsPath CreateRoundedPath(Rectangle rect, int radius)

// Draw soft shadow effects
protected void DrawSoftShadow(Graphics g, Rectangle rect, int radius, int layers = 6, int offset = 3)
```

## LayoutContext Properties

### Rectangles
- `DrawingRect` - Main card area
- `ImageRect` - Image/icon area
- `HeaderRect` - Title/header area
- `ParagraphRect` - Main content area
- `ButtonRect` - Primary button area
- `SecondaryButtonRect` - Secondary button area
- `SubtitleRect` - Subtitle/secondary text
- `StatusRect` - Status indicator area
- `RatingRect` - Rating stars area
- `BadgeRect` - Badge area
- `TagsRect` - Tag/chip area
- `AvatarRect` - Avatar area (for testimonials)

### Display Flags
- `ShowImage` - Whether to show image
- `ShowButton` - Whether to show primary button
- `ShowSecondaryButton` - Whether to show secondary button
- `ShowStatus` - Whether to show status indicator
- `ShowRating` - Whether to show rating stars

### Content Properties
- `AccentColor` - Primary accent color
- `Tags` - List of tag strings
- `BadgeText1/2` - Badge text content
- `Badge1/2BackColor` - Badge background colors
- `Badge1/2ForeColor` - Badge text colors
- `SubtitleText` - Subtitle content
- `StatusText` - Status text
- `StatusColor` - Status indicator color
- `Rating` - Rating value (0-5)

## Best Practices

### 1. **Responsive Design**
- Use percentage-based sizing where appropriate
- Test with different card sizes
- Ensure minimum readable text sizes

### 2. **Accessibility**
- Use sufficient color contrast
- Provide clear visual hierarchy
- Consider screen reader compatibility

### 3. **Performance**
- Dispose graphics objects properly using `using` statements
- Avoid creating unnecessary objects in drawing methods
- Cache complex calculations when possible

### 4. **Consistency**
- Follow existing padding and spacing patterns
- Use theme colors appropriately
- Maintain consistent visual weight

### 5. **Hit Area Management**
- Register interactive areas with appropriate names
- Provide meaningful feedback for user interactions
- Consider touch-friendly sizing for interactive elements

## Testing Your Painter

### 1. **Create Sample Cards**
```csharp
public static BeepCard CreateCustomCard()
{
    return new BeepCard
    {
        Style = CardStyle.CustomCard,
        HeaderText = "Test Title",
        ParagraphText = "Test content",
        // ... set other properties
    };
}
```

### 2. **Test Different Sizes**
- Small: 200x150
- Medium: 300x200
- Large: 400x300

### 3. **Test Different Themes**
- Light theme
- Dark theme
- High contrast

### 4. **Test Interactions**
- Click events
- Hover states
- Focus indicators

This guide should help you create effective, consistent, and well-integrated card painters for the BeepCard system!