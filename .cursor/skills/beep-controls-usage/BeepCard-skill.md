# BeepCard Skill

## Overview
`BeepCard` is a comprehensive card control with 40+ modern card styles, painter-based rendering, event-driven interactions, and Material Design aesthetics.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Cards;
```

## Key Properties

### Content Properties
| Property | Type | Description |
|----------|------|-------------|
| `HeaderText` | `string` | Card title |
| `ParagraphText` | `string` | Card description |
| `ImagePath` | `string` | Image/icon path |
| `ButtonText` | `string` | Primary button text |
| `SecondaryButtonText` | `string` | Secondary button text |
| `SubtitleText` | `string` | Subtitle/tagline |
| `PriceText` | `string` | Price for product cards |

### Style Properties
| Property | Type | Description |
|----------|------|-------------|
| `CardStyle` | `CardStyle` | Visual style (40 types) |
| `AccentColor` | `Color` | Accent color |
| `BorderRadius` | `int` | Corner radius |

### Badge Properties
| Property | Type | Description |
|----------|------|-------------|
| `BadgeText1` | `string` | Primary badge (PRO, NEW) |
| `Badge1BackColor` | `Color` | Primary badge background |
| `BadgeText2` | `string` | Secondary badge |
| `Tags` | `List<string>` | Tag/chip list |

### Rating & Status
| Property | Type | Description |
|----------|------|-------------|
| `Rating` | `int` | 0-5 star rating |
| `ShowRating` | `bool` | Display rating |
| `StatusText` | `string` | Status message |
| `StatusColor` | `Color` | Status indicator color |
| `ShowStatus` | `bool` | Display status |

### Visibility
| Property | Type | Description |
|----------|------|-------------|
| `ShowButton` | `bool` | Show primary button |
| `ShowSecondaryButton` | `bool` | Show secondary button |

## CardStyle Options (40+ types)

**Profile & User Cards:**
- `ProfileCard` - Vertical profile with avatar, bio, actions
- `CompactProfile` - Minimal profile badge
- `UserCard` - User with stats/badges
- `TeamMemberCard` - Team showcase with roles

**Content & Blog Cards:**
- `ContentCard` - Article with hero image
- `BlogCard` - Blog post with author, date, tags
- `NewsCard` - News headline with source
- `MediaCard` - Image-focused minimal text

**Feature & Service Cards:**
- `FeatureCard` - Icon-based feature highlight
- `ServiceCard` - Service with price/CTA
- `IconCard` - Centered icon with text
- `BenefitCard` - Benefits with checkmarks

**E-commerce Cards:**
- `ProductCard` - Product with price, rating, cart
- `PricingCard` - Pricing tier with features
- `OfferCard` - Special deal with discount
- `CartItemCard` - Shopping cart item

**Social Cards:**
- `SocialMediaCard` - Post with likes/comments
- `TestimonialCard` - Quote with rating
- `ReviewCard` - Review with feedback
- `CommentCard` - Nested comments

**Dashboard Cards:**
- `StatCard` - KPI with trend indicator
- `ChartCard` - Embedded chart
- `MetricCard` - Value with change %
- `ActivityCard` - Activity feed item

**Event Cards:**
- `EventCard` - Date badge, title, location
- `CalendarEventCard` - Calendar event
- `ScheduleCard` - Agenda item
- `TaskCard` - Todo with checkbox

## Usage Examples

### Basic Card
```csharp
var card = new BeepCard
{
    CardStyle = CardStyle.BasicCard,
    HeaderText = "Card Title",
    ParagraphText = "Card description",
    ShowButton = true,
    ButtonText = "Action"
};
```

### Profile Card
```csharp
var profile = new BeepCard
{
    CardStyle = CardStyle.ProfileCard,
    HeaderText = "John Doe",
    ParagraphText = "Senior Developer",
    ImagePath = "avatar.png",
    BadgeText1 = "PRO",
    ShowButton = true,
    ButtonText = "Follow"
};
```

### Product Card
```csharp
var product = new BeepCard
{
    CardStyle = CardStyle.ProductCard,
    HeaderText = "Premium Widget",
    ParagraphText = "High-quality widget",
    PriceText = "$99.99",
    ImagePath = "product.png",
    Rating = 4,
    ShowRating = true,
    ButtonText = "Add to Cart"
};
```

### Stat Dashboard Card
```csharp
var stat = new BeepCard
{
    CardStyle = CardStyle.StatCard,
    HeaderText = "Total Sales",
    ParagraphText = "$125,400",
    BadgeText1 = "+15%",
    Badge1BackColor = Color.Green
};
```

## Events
| Event | Description |
|-------|-------------|
| `ImageClicked` | Image area clicked |
| `HeaderClicked` | Header clicked |
| `ParagraphClicked` | Paragraph clicked |
| `ButtonClicked` | Button clicked |
| `BadgeClicked` | Badge clicked |
| `RatingClicked` | Rating clicked |

## Related Controls
- `BeepDialogBox` - Dialog containers
- `BeepTabs` - Tabbed containers
- `BeepPanel` - Basic container
