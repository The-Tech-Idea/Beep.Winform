# ToolTip System Architecture

## Component Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                      ToolTipManager                             │
│  (Static Manager - Entry Point)                                 │
│  • ShowTooltipAsync()                                           │
│  • SetTooltip(Control)                                          │
│  • HideTooltipAsync()                                           │
└───────────────────────┬─────────────────────────────────────────┘
                        │
                        │ creates
                        ▼
┌─────────────────────────────────────────────────────────────────┐
│                   ToolTipInstance                               │
│  (Lifecycle Management)                                         │
│  • Manages CustomToolTip Form                                   │
│  • Handles show/hide timing                                     │
│  • Tracks expiration                                            │
└───────────────────────┬─────────────────────────────────────────┘
                        │
                        │ uses
                        ▼
┌─────────────────────────────────────────────────────────────────┐
│              CustomToolTip (Partial Classes)                    │
├─────────────────────────────────────────────────────────────────┤
│  CustomToolTip.Main.cs                                          │
│    • Properties, initialization                                 │
│    • Painter selection                                          │
│    • Configuration management                                   │
├─────────────────────────────────────────────────────────────────┤
│  CustomToolTip.Drawing.cs                                       │
│    • OnPaint override                                           │
│    • Delegates to IToolTipPainter                               │
│    • Background transparency                                    │
├─────────────────────────────────────────────────────────────────┤
│  CustomToolTip.Animation.cs                                     │
│    • AnimateInAsync()                                           │
│    • AnimateOutAsync()                                          │
│    • Easing functions                                           │
└───────────────────────┬─────────────────────────────────────────┘
                        │
                        │ delegates to
                        ▼
┌─────────────────────────────────────────────────────────────────┐
│                   IToolTipPainter                               │
│  (Interface for all painters)                                   │
│  • Paint()                                                      │
│  • PaintBackground()                                            │
│  • PaintBorder()                                                │
│  • PaintArrow()                                                 │
│  • PaintContent()                                               │
│  • PaintShadow()                                                │
│  • CalculateSize()                                              │
└───────────────────────┬─────────────────────────────────────────┘
                        │
         ┌──────────────┼──────────────┐
         │              │              │
         ▼              ▼              ▼
┌─────────────┐  ┌─────────────┐  ┌─────────────┐
│ Standard    │  │  Premium    │  │   Alert     │
│ ToolTip     │  │  ToolTip    │  │  ToolTip    │
│ Painter     │  │  Painter    │  │  Painter    │
└─────────────┘  └─────────────┘  └─────────────┘
     │                 │                 │
     └─────────────────┼─────────────────┘
                       │
                       │ uses
                       ▼
         ┌─────────────────────────┐
         │   ToolTipHelpers        │
         │  (Static Utilities)     │
         │  • GetThemeColors()     │
         │  • CalculatePosition()  │
         │  • CreateArrowPath()    │
         │  • MeasureContent()     │
         │  • Easing functions     │
         └─────────────────────────┘
                       │
         ┌─────────────┴─────────────┐
         │                           │
         ▼                           ▼
┌──────────────────┐       ┌──────────────────┐
│  IBeepTheme      │       │  ImagePainter    │
│  (Theme Colors)  │       │  (Image Render)  │
│  • AccentColor   │       │  • Load SVG/PNG  │
│  • SuccessColor  │       │  • Apply Theme   │
│  • ErrorColor    │       │  • Scale Image   │
│  • etc.          │       │  • Draw Image    │
└──────────────────┘       └──────────────────┘
```

## Data Flow

```
User Code
   │
   ▼
ToolTipManager.ShowTooltipAsync(ToolTipConfig)
   │
   ├─► Creates ToolTipInstance
   │      │
   │      ├─► Creates CustomToolTip Form
   │      │      │
   │      │      ├─► Selects IToolTipPainter based on Style
   │      │      │      │
   │      │      │      ▼
   │      │      │   StandardPainter / PremiumPainter / AlertPainter
   │      │      │      │
   │      │      │      ├─► Uses ToolTipHelpers.GetThemeColors(IBeepTheme)
   │      │      │      │
   │      │      │      ├─► Uses ImagePainter.DrawImage(ImagePath)
   │      │      │      │      │
   │      │      │      │      └─► Applies theme to SVG if enabled
   │      │      │      │
   │      │      │      └─► Draws: Background → Border → Arrow → Content → Shadow
   │      │      │
   │      │      └─► OnPaint() → Painter.Paint() → Rendered Tooltip
   │      │
   │      └─► AnimateInAsync() → Show with animation
   │
   └─► Returns tooltip key for tracking
```

## Painter Selection Logic

```
CustomToolTip.SelectPainter(config)
   │
   ├─► config.Style == Standard? → StandardToolTipPainter
   │
   ├─► config.Style == Premium? → PremiumToolTipPainter
   │
   ├─► config.Style == Alert? → AlertToolTipPainter
   │
   └─► default → StandardToolTipPainter
```

## Theme Color Resolution

```
ToolTipHelpers.GetThemeColors(IBeepTheme, ToolTipTheme)
   │
   ├─► theme == null? → Use fallback colors
   │
   ├─► ToolTipTheme.Primary? → theme.AccentColor
   │
   ├─► ToolTipTheme.Success? → theme.SuccessColor
   │
   ├─► ToolTipTheme.Error? → theme.ErrorColor
   │
   ├─► ToolTipTheme.Warning? → theme.WarningColor
   │
   ├─► ToolTipTheme.Info? → theme.InfoColor
   │
   ├─► ToolTipTheme.Light? → theme.BackColor + theme.ForeColor
   │
   └─► ToolTipTheme.Dark? → theme.SecondaryBackColor + theme.ForeColor
```

## Image Loading Pipeline

```
Painter.DrawImageFromPath(config)
   │
   ├─► Get path: config.ImagePath ?? config.IconPath
   │
   ├─► Path exists?
   │      │
   │      ├─► YES: Create ImagePainter(path, theme)
   │      │      │
   │      │      ├─► Set ApplyThemeOnImage = config.ApplyThemeOnImage
   │      │      │
   │      │      ├─► Set ScaleMode = KeepAspectRatio
   │      │      │
   │      │      ├─► config.ApplyThemeOnImage && theme != null?
   │      │      │      │
   │      │      │      └─► YES: ApplyThemeToSvg()
   │      │      │
   │      │      └─► DrawImage(graphics, rectangle)
   │      │
   │      └─► NO: config.Icon exists?
   │             │
   │             └─► YES: g.DrawImage(config.Icon, rect)
   │
   └─► Done
```

## Animation Sequence

```
CustomToolTip.ShowAsync()
   │
   ├─► Calculate optimal position
   │      │
   │      └─► ToolTipHelpers.CalculateOptimalPosition()
   │             │
   │             ├─► Try preferred placement
   │             ├─► Check screen bounds
   │             └─► Fallback to alternative placements
   │
   ├─► Set Location
   │
   └─► config.Animation != None?
          │
          ├─► YES: AnimateInAsync()
          │      │
          │      ├─► Animation == Fade? → Opacity transition
          │      │
          │      ├─► Animation == Scale? → Size + Opacity
          │      │
          │      ├─► Animation == Slide? → Position + Opacity
          │      │
          │      └─► Animation == Bounce? → Easing function
          │
          └─► NO: Show immediately with Opacity = 1.0
```

## Class Relationships

```
ToolTipConfig
   ├─► string ImagePath          (Primary image path)
   ├─► string IconPath           (Fallback path)
   ├─► Image Icon                (Fallback image object)
   ├─► Size MaxImageSize         (Max 24x24 default)
   ├─► bool ApplyThemeOnImage    (Apply theme to SVG)
   ├─► ToolTipTheme Theme        (Color scheme)
   ├─► ToolTipStyle Style        (Painter selection)
   ├─► ToolTipPlacement Placement
   ├─► ToolTipAnimation Animation
   └─► ... (other properties)

IToolTipPainter (Interface)
   ├─► Paint(g, bounds, config, placement, theme)
   ├─► PaintBackground(g, bounds, config, theme)
   ├─► PaintBorder(g, bounds, config, theme)
   ├─► PaintArrow(g, position, placement, config, theme)
   ├─► PaintContent(g, bounds, config, theme)
   ├─► PaintShadow(g, bounds, config)
   └─► CalculateSize(g, config)

StandardToolTipPainter : IToolTipPainter
PremiumToolTipPainter : IToolTipPainter
AlertToolTipPainter : IToolTipPainter
```

## Key Design Patterns

1. **Partial Classes** - Separation of concerns (Main, Drawing, Animation)
2. **Strategy Pattern** - IToolTipPainter for different styles
3. **Factory Pattern** - SelectPainter() creates appropriate painter
4. **Dependency Injection** - IBeepTheme passed to painters
5. **Disposal Pattern** - ImagePainter with `using` statement
6. **Static Manager** - ToolTipManager for global access
7. **Async/Await** - For animations and timing

## Benefits of This Architecture

✅ **Separation of Concerns** - Each component has single responsibility
✅ **Extensibility** - Easy to add new painter styles
✅ **Testability** - Each component can be tested independently
✅ **Theme Integration** - Consistent with Beep control system
✅ **Reusability** - ToolTipHelpers and ImagePainter are reusable
✅ **Maintainability** - Clear structure, easy to understand
✅ **Performance** - Caching in ImagePainter, smart invalidation
