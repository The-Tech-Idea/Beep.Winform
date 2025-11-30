# BeepCard Enhancement & Optimization Plan (REVISED)

## Key Principle: Each Card Painter is DISTINCT

**NO shared `CardPainterBase` class.** Each painter:
- Implements `ICardPainter` directly
- Creates its own `LayoutContext` with style-specific rectangles
- Has its own spacing, fonts, and rendering logic
- Is completely self-contained and independent

This matches the approach used for `BeepMultiChipGroup` painters.

---

## Current Problems

### 1. Shared Base Class Anti-Pattern ❌
```csharp
// CURRENT - WRONG
internal abstract class CardPainterBase : ICardPainter
{
    protected const int DefaultPad = 12;      // Forces same padding
    protected const int HeaderHeight = 26;    // Forces same header height
    protected const int ButtonHeight = 32;    // Forces same button height
    // All painters inherit these constraints
}
```

### 2. Many Styles Share Painters ❌
```csharp
// CURRENT - WRONG: 4 different styles use same painter
case CardStyle.ProductCard:
case CardStyle.PricingCard:
case CardStyle.OfferCard:
case CardStyle.CartItemCard:
    _painter = new ProductCardPainter();  // One painter for 4 distinct styles!
```

### 3. Generic LayoutContext ❌
```csharp
// CURRENT - WRONG: One-size-fits-all context
internal sealed class LayoutContext
{
    public Rectangle ImageRect;      // Not all cards have images
    public Rectangle HeaderRect;     // Not all cards have headers
    public Rectangle ButtonRect;     // Not all cards have buttons
    // ... generic rectangles that don't fit all styles
}
```

---

## New Architecture: Distinct Painters

### 1. Remove `CardPainterBase.cs` ✅

Delete the base class entirely. Each painter stands alone.

### 2. Style-Specific LayoutContext ✅

Each painter defines its own context structure:

```csharp
// ProfileCardPainter.cs
internal sealed class ProfileCardPainter : ICardPainter
{
    // Profile-specific layout
    private struct ProfileLayout
    {
        public Rectangle BannerRect;      // Large hero image
        public Rectangle AvatarRect;      // Circular profile pic
        public Rectangle NameRect;        // User name
        public Rectangle TitleRect;       // Job title
        public Rectangle StatusRect;      // Online/Available status
        public Rectangle BadgeRect;       // PRO badge
        public Rectangle FollowButtonRect;
        public Rectangle MessageButtonRect;
    }
    
    private ProfileLayout _layout;
    // ...
}

// PricingCardPainter.cs  
internal sealed class PricingCardPainter : ICardPainter
{
    // Pricing-specific layout
    private struct PricingLayout
    {
        public Rectangle TierNameRect;     // "Professional"
        public Rectangle PriceRect;        // "$49/month"
        public Rectangle FeaturesListRect; // Bullet list area
        public Rectangle PopularBadgeRect; // "Most Popular"
        public Rectangle CTAButtonRect;    // "Choose Plan"
        public Rectangle CompareButtonRect;
    }
    
    private PricingLayout _layout;
    // ...
}
```

### 3. One Painter Per CardStyle ✅

Every `CardStyle` enum value gets its own dedicated painter file:

```
Cards/Painters/
├── ProfileCardPainter.cs           # CardStyle.ProfileCard
├── CompactProfileCardPainter.cs    # CardStyle.CompactProfile
├── UserCardPainter.cs              # CardStyle.UserCard
├── TeamMemberCardPainter.cs        # CardStyle.TeamMemberCard (NEW)
├── ContentCardPainter.cs           # CardStyle.ContentCard
├── BlogCardPainter.cs              # CardStyle.BlogCard
├── NewsCardPainter.cs              # CardStyle.NewsCard (NEW)
├── MediaCardPainter.cs             # CardStyle.MediaCard
├── FeatureCardPainter.cs           # CardStyle.FeatureCard
├── ServiceCardPainter.cs           # CardStyle.ServiceCard
├── IconCardPainter.cs              # CardStyle.IconCard (NEW)
├── BenefitCardPainter.cs           # CardStyle.BenefitCard (NEW)
├── ProductCardPainter.cs           # CardStyle.ProductCard
├── PricingCardPainter.cs           # CardStyle.PricingCard (NEW)
├── OfferCardPainter.cs             # CardStyle.OfferCard (NEW)
├── CartItemCardPainter.cs          # CardStyle.CartItemCard (NEW)
├── SocialMediaCardPainter.cs       # CardStyle.SocialMediaCard
├── TestimonialCardPainter.cs       # CardStyle.TestimonialCard
├── ReviewCardPainter.cs            # CardStyle.ReviewCard
├── CommentCardPainter.cs           # CardStyle.CommentCard (NEW)
├── StatCardPainter.cs              # CardStyle.StatCard
├── ChartCardPainter.cs             # CardStyle.ChartCard (NEW)
├── MetricCardPainter.cs            # CardStyle.MetricCard
├── ActivityCardPainter.cs          # CardStyle.ActivityCard (NEW)
├── NotificationCardPainter.cs      # CardStyle.NotificationCard (NEW)
├── MessageCardPainter.cs           # CardStyle.MessageCard (NEW)
├── AlertCardPainter.cs             # CardStyle.AlertCard (NEW)
├── AnnouncementCardPainter.cs      # CardStyle.AnnouncementCard (NEW)
├── EventCardPainter.cs             # CardStyle.EventCard
├── CalendarEventCardPainter.cs     # CardStyle.CalendarEventCard (NEW)
├── ScheduleCardPainter.cs          # CardStyle.ScheduleCard (NEW)
├── TaskCardPainter.cs              # CardStyle.TaskCard (NEW)
├── ListCardPainter.cs              # CardStyle.ListCard
├── DataCardPainter.cs              # CardStyle.DataCard
├── FormCardPainter.cs              # CardStyle.FormCard (NEW)
├── SettingsCardPainter.cs          # CardStyle.SettingsCard (NEW)
├── DialogCardPainter.cs            # CardStyle.DialogCard
├── BasicCardPainter.cs             # CardStyle.BasicCard
├── HoverCardPainter.cs             # CardStyle.HoverCard (NEW)
├── InteractiveCardPainter.cs       # CardStyle.InteractiveCard
├── ImageCardPainter.cs             # CardStyle.ImageCard (NEW)
├── VideoCardPainter.cs             # CardStyle.VideoCard (NEW)
├── DownloadCardPainter.cs          # CardStyle.DownloadCard (NEW)
└── ContactCardPainter.cs           # CardStyle.ContactCard (NEW)
```

**Total: 44 distinct painters (one per CardStyle)**

---

## Painter Template (No Base Class)

Each painter follows this template:

```csharp
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// [CARD_NAME] - [DESCRIPTION]
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class [CARD_NAME]CardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Style-specific fonts
        private Font _titleFont;
        private Font _subtitleFont;
        private Font _bodyFont;
        private Font _badgeFont;
        
        // Style-specific spacing
        private const int Padding = [VALUE];      // Unique to this style
        private const int TitleHeight = [VALUE];  // Unique to this style
        private const int Spacing = [VALUE];      // Unique to this style
        
        // Style-specific layout rectangles
        private Rectangle _[area1]Rect;
        private Rectangle _[area2]Rect;
        // ... more style-specific areas
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            // Create style-specific fonts
            var fontFamily = owner.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _subtitleFont?.Dispose(); } catch { }
            try { _bodyFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, [SIZE], FontStyle.Bold);
            _subtitleFont = new Font(fontFamily, [SIZE], FontStyle.Regular);
            _bodyFont = new Font(fontFamily, [SIZE], FontStyle.Regular);
            _badgeFont = new Font(fontFamily, [SIZE], FontStyle.Bold);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            // Calculate ALL layout rectangles specific to this card style
            // Each painter has completely different layout logic
            
            ctx.DrawingRect = drawingRect;
            
            // [STYLE-SPECIFIC LAYOUT CALCULATIONS]
            // Example for a profile card:
            // _bannerRect = new Rectangle(...)
            // _avatarRect = new Rectangle(...)
            // etc.
            
            // Map to generic LayoutContext for BeepCard compatibility
            ctx.ImageRect = _[relevant]Rect;
            ctx.HeaderRect = _[relevant]Rect;
            // ...
            
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Style-specific background rendering
            // Some cards have gradients, some have sections, some are plain
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Style-specific foreground elements
            // Badges, ratings, icons, progress bars, etc.
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Style-specific interactive areas
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _titleFont?.Dispose();
            _subtitleFont?.Dispose();
            _bodyFont?.Dispose();
            _badgeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}
```

---

## Detailed Painter Specifications

### Profile & User Cards

#### 1. ProfileCardPainter
```
Layout:
┌─────────────────────────┐
│  [BANNER IMAGE 40%]     │
│  ┌─────┐                │
│  │BADGE│                │
│  └─────┘                │
├─────────────────────────┤
│  Alex Morgan            │
│  @alexmorgan            │
│  ● Available for work   │
├─────────────────────────┤
│  [ Follow ] [ Message ] │
└─────────────────────────┘

Spacing: Padding=16, TitleSize=16, SubtitleSize=11
Fonts: Title=Bold 16pt, Subtitle=Regular 11pt
```

#### 2. CompactProfileCardPainter
```
Layout:
┌─────────────────────────────────┐
│ ┌────┐  Jordan Chen      [PRO] │
│ │ 🧑 │  UI/UX Designer         │
│ └────┘  ● Active now           │
└─────────────────────────────────┘

Spacing: Padding=12, AvatarSize=48
Fonts: Name=Bold 12pt, Title=Regular 10pt
```

#### 3. UserCardPainter
```
Layout:
┌──────────────────────────────┐
│  ┌────┐  Taylor Swift        │
│  │    │  Product Manager     │
│  └────┘  San Francisco, CA   │
├──────────────────────────────┤
│  2.5K Followers  150 Posts   │
├──────────────────────────────┤
│  [ View Profile ]            │
└──────────────────────────────┘

Spacing: Padding=16, StatGap=24
```

#### 4. TeamMemberCardPainter
```
Layout:
┌──────────────────────────────┐
│      ┌──────────┐            │
│      │  AVATAR  │            │
│      └──────────┘            │
│      Morgan Lee              │
│      Lead DevOps Engineer    │
│      ─────────────           │
│      AWS Certified           │
├──────────────────────────────┤
│  [ Contact ] [ Schedule ]    │
└──────────────────────────────┘

Spacing: Padding=20, AvatarSize=80
```

### E-Commerce Cards

#### 5. ProductCardPainter
```
Layout:
┌──────────────────────────────┐
│ ┌────────────────────────┐   │
│ │                        │   │
│ │    PRODUCT IMAGE       │   │
│ │                        │   │
│ │ [-20%]                 │   │
│ └────────────────────────┘   │
│  Wireless Headphones Pro     │
│  ★★★★★  $299.99              │
│  Premium noise-cancelling    │
├──────────────────────────────┤
│  [ Add to Cart ]             │
└──────────────────────────────┘

Spacing: ImageHeight=50%, Padding=12
```

#### 6. PricingCardPainter (NEW)
```
Layout:
┌──────────────────────────────┐
│     [MOST POPULAR]           │
│                              │
│     Professional             │
│     ───────────              │
│         $49                  │
│       /month                 │
│                              │
│  ✓ Unlimited projects        │
│  ✓ Advanced features         │
│  ✓ Priority support          │
│  ✓ Custom integrations       │
│                              │
├──────────────────────────────┤
│  [ Choose Plan ]             │
│  [ Compare ]                 │
└──────────────────────────────┘

Spacing: Padding=24, FeatureGap=8
Fonts: Price=Bold 36pt, Features=Regular 11pt
```

#### 7. OfferCardPainter (NEW)
```
Layout:
┌──────────────────────────────┐
│ [HOT DEAL]                   │
│                              │
│  🔥 Black Friday Sale!       │
│  ─────────────────           │
│  Save up to 70%              │
│  on select items             │
│                              │
│  Ends in: 2d 14h 32m         │
├──────────────────────────────┤
│  [ Shop Now ]                │
└──────────────────────────────┘

Spacing: Padding=20
Accent: Red/Orange gradient background
```

#### 8. CartItemCardPainter (NEW)
```
Layout:
┌──────────────────────────────────────┐
│ ┌────┐  Premium T-Shirt    [ × ]     │
│ │IMG │  Size: L • Navy Blue          │
│ └────┘  $29.99                       │
│         [ - ] 2 [ + ]      $59.98    │
└──────────────────────────────────────┘

Spacing: Padding=12, ImageSize=60
Horizontal layout with quantity controls
```

### Dashboard Cards

#### 9. StatCardPainter
```
Layout:
┌──────────────────────────────┐
│  📈  Active Users            │
│                              │
│      12,458                  │
│      ↑ +18.2%                │
│      from last month         │
└──────────────────────────────┘

Spacing: Padding=20
Fonts: Value=Bold 32pt, Label=Regular 12pt
```

#### 10. ChartCardPainter (NEW)
```
Layout:
┌──────────────────────────────┐
│  Revenue Overview    [···]   │
│  ─────────────────           │
│  ┌────────────────────────┐  │
│  │                        │  │
│  │    [CHART AREA]        │  │
│  │                        │  │
│  └────────────────────────┘  │
│  $124,500 this month         │
├──────────────────────────────┤
│  [ View Report ]             │
└──────────────────────────────┘

Spacing: Padding=16, ChartHeight=60%
```

#### 11. MetricCardPainter
```
Layout:
┌──────────────────────────────┐
│  ┌────┐                      │
│  │ 📊 │  Conversion Rate     │
│  └────┘                      │
│         3.8%                 │
│         +0.5% vs last week   │
│  ─────────────────────────   │
│  ████████░░  Target: 4%      │
└──────────────────────────────┘

Spacing: Padding=16
Progress bar for target comparison
```

#### 12. ActivityCardPainter (NEW)
```
Layout:
┌──────────────────────────────────────┐
│ ┌──┐  New Order Placed       [NEW]   │
│ │📦│  Order #4567 by John Smith      │
│ └──┘  Total: $156.99                 │
│       5 minutes ago    [ View ]      │
└──────────────────────────────────────┘

Spacing: Padding=12
Timeline-style with icon and timestamp
```

### Communication Cards

#### 13. NotificationCardPainter (NEW)
```
Layout:
┌──────────────────────────────────────┐
│ ┌──┐  System Update Available        │
│ │ℹ️│  A new version is ready...      │
│ └──┘  Just now                       │
├──────────────────────────────────────┤
│  [ Update Now ]  [ Later ]           │
└──────────────────────────────────────┘

Spacing: Padding=16
Icon indicates type (info, warning, error)
```

#### 14. MessageCardPainter (NEW)
```
Layout:
┌──────────────────────────────────────┐
│ ┌────┐  Lisa Anderson                │
│ │ 🧑 │  10:32 AM                     │
│ └────┘                               │
│ ┌────────────────────────────────┐   │
│ │ Hey! Did you get a chance to   │   │
│ │ review the proposal?           │   │
│ └────────────────────────────────┘   │
│                       [ Reply ]      │
└──────────────────────────────────────┘

Spacing: Padding=12
Chat bubble style with avatar
```

#### 15. AlertCardPainter (NEW)
```
Layout:
┌──────────────────────────────────────┐
│ [URGENT]                             │
│ ┌──┐                                 │
│ │⚠️│  Action Required                │
│ └──┘                                 │
│ Your payment method will expire      │
│ soon. Please update your billing.    │
│                                      │
│ Expires in 7 days                    │
├──────────────────────────────────────┤
│  [ Update Payment ]                  │
└──────────────────────────────────────┘

Spacing: Padding=16
Color-coded by severity (red/orange/yellow)
```

### Task & Calendar Cards

#### 16. TaskCardPainter (NEW)
```
Layout:
┌──────────────────────────────────────┐
│ [High Priority]                      │
│ ☐ Complete Project Documentation     │
│   ─────────────────────────          │
│   Write comprehensive docs for       │
│   the new API endpoints              │
│                                      │
│   📅 Due: Dec 20, 2024               │
│   ● In Progress                      │
├──────────────────────────────────────┤
│  [ Mark Complete ]                   │
└──────────────────────────────────────┘

Spacing: Padding=16
Checkbox, priority badge, status indicator
```

#### 17. CalendarEventCardPainter (NEW)
```
Layout:
┌──────────────────────────────────────┐
│ ┌─────┐                              │
│ │ 15  │  Team Meeting                │
│ │ DEC │  Q4 Planning & Strategy      │
│ └─────┘                              │
│         Today at 2:00 PM             │
│         📍 Conference Room A         │
├──────────────────────────────────────┤
│  [ Join Meeting ]                    │
└──────────────────────────────────────┘

Spacing: Padding=16, DateBadgeSize=60
Large date badge on left
```

### Media Cards

#### 18. VideoCardPainter (NEW)
```
Layout:
┌──────────────────────────────────────┐
│ ┌────────────────────────────────┐   │
│ │                                │   │
│ │         [▶ PLAY]               │   │
│ │                                │   │
│ │                        5:42    │   │
│ └────────────────────────────────┘   │
│  Product Demo Video                  │
│  Watch our 5-minute introduction     │
├──────────────────────────────────────┤
│  [ Play Video ]                      │
└──────────────────────────────────────┘

Spacing: Padding=12, ThumbnailHeight=60%
Play button overlay, duration badge
```

#### 19. ImageCardPainter (NEW)
```
Layout:
┌──────────────────────────────────────┐
│ ┌────────────────────────────────┐   │
│ │                                │   │
│ │                                │   │
│ │        FULL IMAGE              │   │
│ │                                │   │
│ │ ┌──────────────────────────┐   │   │
│ │ │ Beautiful Landscapes     │   │   │
│ │ │ Explore stunning...      │   │   │
│ │ └──────────────────────────┘   │   │
│ └────────────────────────────────┘   │
└──────────────────────────────────────┘

Spacing: Padding=0
Full-bleed image with overlay text
```

---

## Implementation Plan

### Step 1: Delete Base Class
- Remove `CardPainterBase.cs`
- Keep `CardRenderingHelpers.cs` as static utility (optional use)

### Step 2: Update Interface
```csharp
// ICardPainter.cs - Add IDisposable
internal interface ICardPainter : IDisposable
{
    void Initialize(BaseControl owner, IBeepTheme theme);
    LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx);
    void DrawBackground(Graphics g, LayoutContext ctx);
    void DrawForegroundAccents(Graphics g, LayoutContext ctx);
    void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit);
}
```

### Step 3: Create New Painters (22 NEW)
Priority order:
1. PricingCardPainter
2. TaskCardPainter
3. VideoCardPainter
4. AlertCardPainter
5. CalendarEventCardPainter
6. NotificationCardPainter
7. MessageCardPainter
8. ChartCardPainter
9. ActivityCardPainter
10. OfferCardPainter
11. CartItemCardPainter
12. CommentCardPainter
13. NewsCardPainter
14. TeamMemberCardPainter
15. IconCardPainter
16. BenefitCardPainter
17. ScheduleCardPainter
18. FormCardPainter
19. SettingsCardPainter
20. HoverCardPainter
21. ImageCardPainter
22. DownloadCardPainter

### Step 4: Refactor Existing Painters
Update all 22 existing painters to:
- Remove inheritance from `CardPainterBase`
- Implement `ICardPainter` directly
- Implement `IDisposable`
- Define own spacing constants
- Create own fonts
- Calculate own layout

### Step 5: Update BeepCard.cs
```csharp
private void InitializePainter()
{
    // Dispose old painter
    (_painter as IDisposable)?.Dispose();
    
    // One painter per style - no sharing
    _painter = _style switch
    {
        CardStyle.ProfileCard => new ProfileCardPainter(),
        CardStyle.CompactProfile => new CompactProfileCardPainter(),
        CardStyle.UserCard => new UserCardPainter(),
        CardStyle.TeamMemberCard => new TeamMemberCardPainter(),
        // ... one line per CardStyle (44 total)
        _ => new BasicCardPainter()
    };
    
    _painter?.Initialize(this, _currentTheme);
}
```

### Step 6: Add Painter Caching (Optional)
```csharp
// CardPainterFactory.cs - cache painters if needed
internal static class CardPainterFactory
{
    private static readonly Dictionary<CardStyle, ICardPainter> _cache = new();
    
    public static ICardPainter GetPainter(CardStyle style, BaseControl owner, IBeepTheme theme)
    {
        if (!_cache.TryGetValue(style, out var painter))
        {
            painter = CreatePainter(style);
            _cache[style] = painter;
        }
        painter.Initialize(owner, theme);
        return painter;
    }
}
```

---

## Files to Create (22 NEW Painters)

```
Cards/Painters/
├── TeamMemberCardPainter.cs      # NEW
├── NewsCardPainter.cs            # NEW
├── IconCardPainter.cs            # NEW
├── BenefitCardPainter.cs         # NEW
├── PricingCardPainter.cs         # NEW
├── OfferCardPainter.cs           # NEW
├── CartItemCardPainter.cs        # NEW
├── CommentCardPainter.cs         # NEW
├── ChartCardPainter.cs           # NEW
├── ActivityCardPainter.cs        # NEW
├── NotificationCardPainter.cs    # NEW
├── MessageCardPainter.cs         # NEW
├── AlertCardPainter.cs           # NEW
├── AnnouncementCardPainter.cs    # NEW
├── CalendarEventCardPainter.cs   # NEW
├── ScheduleCardPainter.cs        # NEW
├── TaskCardPainter.cs            # NEW
├── FormCardPainter.cs            # NEW
├── SettingsCardPainter.cs        # NEW
├── HoverCardPainter.cs           # NEW
├── ImageCardPainter.cs           # NEW
├── VideoCardPainter.cs           # NEW
├── DownloadCardPainter.cs        # NEW
└── ContactCardPainter.cs         # NEW
```

## Files to Modify (22 Existing Painters)

```
Cards/Painters/
├── ProfileCardPainter.cs         # Remove base, add IDisposable
├── CompactProfileCardPainter.cs  # Remove base, add IDisposable
├── UserCardPainter.cs            # Remove base, add IDisposable
├── ContentCardPainter.cs         # Remove base, add IDisposable
├── BlogCardPainter.cs            # Remove base, add IDisposable
├── MediaCardPainter.cs           # Remove base, add IDisposable
├── FeatureCardPainter.cs         # Remove base, add IDisposable
├── ServiceCardPainter.cs         # Remove base, add IDisposable
├── ProductCardPainter.cs         # Remove base, add IDisposable
├── SocialMediaCardPainter.cs     # Remove base, add IDisposable
├── TestimonialCardPainter.cs     # Remove base, add IDisposable
├── ReviewCardPainter.cs          # Remove base, add IDisposable
├── StatCardPainter.cs            # Remove base, add IDisposable
├── MetricCardPainter.cs          # Remove base, add IDisposable
├── CommunicationCardPainter.cs   # Remove base, add IDisposable
├── EventCardPainter.cs           # Remove base, add IDisposable
├── CalendarCardPainter.cs        # Remove base, add IDisposable
├── ListCardPainter.cs            # Remove base, add IDisposable
├── DataCardPainter.cs            # Remove base, add IDisposable
├── DialogCardPainter.cs          # Remove base, add IDisposable
├── BasicCardPainter.cs           # Remove base, add IDisposable
└── InteractiveCardPainter.cs     # Remove base, add IDisposable
```

## Files to Delete

```
Cards/Helpers/CardPainterBase.cs  # DELETE - no more base class
```

---

## Summary

| Item | Count |
|------|-------|
| CardStyle enum values | 44 |
| Painters to create (NEW) | 22 |
| Painters to refactor (existing) | 22 |
| Total distinct painters | 44 |
| Files to delete | 1 |

**Principle: 1 CardStyle = 1 Distinct Painter**

Each painter is completely self-contained with its own:
- Layout calculations
- Spacing constants
- Font definitions
- Rendering logic
- Hit areas

No shared base class. No shared painters between styles.

---

## Ready to Implement?

Would you like me to start implementing the new distinct painters? I'll begin with the highest priority ones:

1. **PricingCardPainter** - SaaS pricing tables
2. **TaskCardPainter** - Kanban/todo boards  
3. **VideoCardPainter** - Media with play overlay
4. **AlertCardPainter** - Notifications with severity
5. **CalendarEventCardPainter** - Calendar events

Or would you like me to first refactor the existing painters to remove the base class?
