# BeepRating Skill

## Overview
`BeepStarRating` is a rating control with 9 visual styles including stars, hearts, thumbs, emoji, circles, and bars.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Ratings;
```

## RatingStyle (9 types)
```csharp
public enum RatingStyle
{
    ClassicStar,    // Traditional 5-pointed stars (default)
    ModernStar,     // Rounded, softer stars
    Heart,          // Heart icons
    Thumb,          // Thumbs up/down
    Circle,         // Filled circles
    Emoji,          // 😀😐😢 emoji
    Bar,            // Horizontal segments
    GradientStar,   // Gradient-filled stars
    Minimal         // Clean minimal design
}
```

## Painters
| Painter | Description |
|---------|-------------|
| `CircleRatingPainter` | Circle dots |
| `HeartRatingPainter` | Heart icons |
| `ThumbRatingPainter` | Thumb up/down |
| `EmojiRatingPainter` | Emoji faces |
| `BarRatingPainter` | Horizontal bars |
| `MinimalRatingPainter` | Minimal style |

## Usage Examples

### Basic Star Rating
```csharp
var rating = new BeepStarRating
{
    RatingStyle = RatingStyle.ClassicStar,
    Value = 3,
    MaxRating = 5
};

rating.ValueChanged += (s, e) =>
{
    Console.WriteLine($"Rating: {rating.Value}");
};
```

### Heart Rating
```csharp
var rating = new BeepStarRating
{
    RatingStyle = RatingStyle.Heart,
    MaxRating = 5,
    AllowHalfRating = true
};
```

### Emoji Rating
```csharp
var rating = new BeepStarRating
{
    RatingStyle = RatingStyle.Emoji,
    MaxRating = 5
};
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `Value` | `int/double` | Current rating |
| `MaxRating` | `int` | Maximum rating value (5) |
| `RatingStyle` | `RatingStyle` | Visual style |
| `AllowHalfRating` | `bool` | Enable 0.5 increments |
| `ReadOnly` | `bool` | Disable interaction |

## Events
| Event | Description |
|-------|-------------|
| `ValueChanged` | Rating value changed |

## Related Controls
- `BeepCard` (Testimonial) - Reviews with ratings
