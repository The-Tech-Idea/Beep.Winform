# ToolTipManager Refactoring Summary

## Overview
The ToolTipManager has been completely refactored following best practices from modern UI frameworks (DevExpress, Material-UI, Ant Design) and your project's architecture patterns:
- **Helper pattern** for utilities
- **Partial class pattern** for separation of concerns
- **Painter pattern** for flexible rendering
- **One class per file** principle

## Architecture

### Design Patterns Used
1. **Singleton Pattern** - Static manager with single point of access
2. **Factory Pattern** - Creates and manages tooltip instances
3. **Observer Pattern** - Event-driven control interactions
4. **Strategy Pattern** - Painter pattern for different rendering styles
5. **Repository Pattern** - Concurrent dictionaries for instance management

### Inspiration from Modern UI Frameworks
- **DevExpress**: Rich tooltip configurations, multiple style variants
- **Material-UI**: Placement system, theme integration, animation support
- **Ant Design**: Clean API, global settings, accessibility features
- **Popper.js**: Smart positioning and collision detection
- **React**: Component lifecycle management (mount, update, unmount)

## File Structure

### Core Manager Files (Partial Classes)
```
ToolTipManager.cs              - Main class, fields, global settings, state
ToolTipManager.Api.cs           - Public API methods (Show, Hide, Update)
ToolTipManager.Controls.cs      - Control integration (SetTooltip, RemoveTooltip)
ToolTipManager.Events.cs        - Event handlers and cleanup timer
```

### Configuration and Data Classes
```
ToolTipConfig.cs                - Complete tooltip configuration class
ToolTipEnums.cs                 - All enumerations (Theme, Placement, Animation, Style)
ToolTipInstance.cs              - Internal instance lifecycle management
```

### Supporting Components
```
Helpers/
  ToolTipHelpers.cs             - Positioning, sizing, color utilities

Painters/
  IToolTipPainter.cs            - Painter interface
  StandardToolTipPainter.cs     - Standard style implementation
  PremiumToolTipPainter.cs      - Premium gradient style
  AlertToolTipPainter.cs        - Alert with accent bar
  NotificationToolTipPainter.cs - Notification/toast style
  StepToolTipPainter.cs         - Multi-step tutorial style
```

## Class Responsibilities

### 1. ToolTipManager (Partial Classes)

#### ToolTipManager.cs (Main)
- **Fields**: Active tooltip tracking, control mappings, cleanup timer
- **Static Constructor**: Initialize cleanup timer
- **Global Settings**: Default values for theme, delays, animations
- **State Properties**: Active counts and status

#### ToolTipManager.Api.cs
- **Show Methods**: 
  - `ShowTooltipAsync(ToolTipConfig)` - Full configuration
  - `ShowTooltipAsync(text, location, duration)` - Simple text
  - `ShowTooltipAsync(title, text, location, theme)` - With title
- **Hide Methods**:
  - `HideTooltipAsync(key)` - Hide specific tooltip
  - `HideAllTooltipsAsync()` - Hide all active tooltips
- **Update Methods**:
  - `UpdateTooltip(key, text, title)` - Update content
  - `UpdateTooltipPosition(key, position)` - Update position
- **Query Methods**:
  - `IsTooltipActive(key)` - Check if tooltip exists
  - `GetActiveTooltipKeys()` - Get all active keys

#### ToolTipManager.Controls.cs
- **Registration**:
  - `SetTooltip(control, text, config)` - Attach to control
  - `SetTooltip(control, title, text, theme)` - With title
  - `SetTooltip(control, text, style, theme)` - With style
- **Removal**:
  - `RemoveTooltip(control)` - Remove from control
  - `RemoveAllControlTooltips()` - Clear all
- **Updates**:
  - `UpdateControlTooltip(control, text)` - Update content
- **Queries**:
  - `HasTooltip(control)` - Check if control has tooltip
  - `GetControlTooltipKey(control)` - Get tooltip key
- **Batch Operations**:
  - `SetTooltipForControls(text, ...controls)` - Multiple controls

#### ToolTipManager.Events.cs
- **Control Events**:
  - `OnControlMouseEnter` - Show on hover with delay
  - `OnControlMouseLeave` - Hide on exit with delay
  - `OnControlMouseMove` - Follow cursor support
- **Cleanup**:
  - `OnCleanupTimer` - Periodic expired instance removal
- **Helpers**:
  - `CalculateTooltipPosition` - Position relative to control

### 2. ToolTipConfig.cs
Complete configuration class organized by concern:
- **Identification**: Key, Tag
- **Content**: Text, Title, Html
- **Positioning**: Position, Placement, Offset, FollowCursor
- **Timing**: Duration, ShowDelay, HideDelay
- **Appearance - Theme/Style**: Theme, Style
- **Appearance - Colors**: BackColor, ForeColor, BorderColor
- **Appearance - Typography**: Font, MaxSize
- **Appearance - Visual**: ShowArrow, ShowShadow, Closable
- **Icons/Images**: Icon, IconPath, ImagePath, MaxImageSize
- **Animation**: Animation type, Duration
- **Step Properties**: CurrentStep, TotalSteps, StepTitle, ShowNavigationButtons
- **Events**: OnClose, OnShow callbacks

### 3. ToolTipEnums.cs
All enumerations with XML documentation:

#### ToolTipTheme
- Auto, Light, Dark
- Primary, Success, Warning, Error, Info
- Custom

#### ToolTipPlacement (Based on Popper.js)
- Auto
- Top, TopStart, TopEnd
- Bottom, BottomStart, BottomEnd
- Left, LeftStart, LeftEnd
- Right, RightStart, RightEnd

#### ToolTipAnimation (Inspired by Framer Motion)
- None, Fade, Scale, Slide, Bounce

#### ToolTipStyle (Inspired by DevExpress)
- Standard, Premium, Alert, Notification, Step

### 4. ToolTipInstance.cs
Internal lifecycle management:
- **Lifecycle**: ShowAsync, HideAsync
- **Updates**: UpdateContent, UpdatePosition
- **State**: IsExpired, IsVisible
- **Disposal**: Proper cleanup and resource management

## Best Practices Implemented

### 1. Separation of Concerns
✅ Each file has ONE class only
✅ Partial classes separate functionality logically
✅ Clear boundaries between API, control integration, and events

### 2. Modern UI Framework Patterns
✅ **Declarative API**: Configure once, framework handles the rest
✅ **Smart Defaults**: Reasonable defaults with easy overrides
✅ **Lifecycle Management**: Proper mount, update, unmount pattern
✅ **Event-Driven**: React to user interactions automatically
✅ **Theme Integration**: Consistent with application theme

### 3. Thread Safety
✅ ConcurrentDictionary for shared state
✅ Proper async/await patterns
✅ Cancellation token support
✅ Exception handling and logging

### 4. Memory Management
✅ Automatic cleanup timer (every 5 seconds)
✅ Proper disposal pattern
✅ Weak reference considerations for controls
✅ Timeout for indefinite tooltips

### 5. Accessibility
✅ ARIA attribute support (AccessibleDescription, AccessibleName)
✅ Keyboard navigation ready
✅ Screen reader compatible
✅ Can disable animations for accessibility

### 6. Performance
✅ Lazy initialization
✅ Efficient lookup with dictionaries
✅ Minimal allocations
✅ Background cleanup doesn't block UI

### 7. Extensibility
✅ Painter pattern for custom rendering
✅ Event callbacks for custom behavior
✅ Theme system for consistent styling
✅ Configuration inheritance

## Usage Examples

### Basic Usage
```csharp
// Simple text tooltip
var key = await ToolTipManager.ShowTooltipAsync("Hello World", new Point(100, 100));

// With configuration
var config = new ToolTipConfig
{
    Title = "Welcome",
    Text = "This is a modern tooltip",
    Theme = ToolTipTheme.Primary,
    Style = ToolTipStyle.Premium,
    Animation = ToolTipAnimation.Scale
};
await ToolTipManager.ShowTooltipAsync(config);
```

### Control Integration
```csharp
// Attach to control
ToolTipManager.SetTooltip(myButton, "Click me!");

// With theme
ToolTipManager.SetTooltip(myButton, "Warning", "Be careful!", ToolTipTheme.Warning);

// Remove tooltip
ToolTipManager.RemoveTooltip(myButton);
```

### Global Settings
```csharp
// Configure defaults
ToolTipManager.DefaultTheme = ToolTipTheme.Dark;
ToolTipManager.DefaultShowDelay = 300;
ToolTipManager.EnableAnimations = true;
ToolTipManager.EnableAccessibility = true;
```

### Advanced Features
```csharp
// Follow cursor
var config = new ToolTipConfig
{
    Text = "Follows your mouse",
    FollowCursor = true
};

// Step-by-step tutorial
var stepConfig = new ToolTipConfig
{
    Style = ToolTipStyle.Step,
    CurrentStep = 1,
    TotalSteps = 5,
    StepTitle = "Getting Started",
    Text = "Welcome to the tutorial!",
    ShowNavigationButtons = true
};

// With callbacks
config.OnShow = (key) => Console.WriteLine($"Tooltip {key} shown");
config.OnClose = (key) => Console.WriteLine($"Tooltip {key} closed");
```

## Comparison with Modern Frameworks

### Material-UI (React)
```javascript
<Tooltip title="Simple" placement="top" arrow>
  <Button>Hover me</Button>
</Tooltip>
```

**Our Implementation:**
```csharp
ToolTipManager.SetTooltip(button, "Simple", new ToolTipConfig
{
    Placement = ToolTipPlacement.Top,
    ShowArrow = true
});
```

### Ant Design (React)
```javascript
<Tooltip title="Prompt Text" color="blue" mouseEnterDelay={0.5}>
  <span>Tooltip will show on mouse enter.</span>
</Tooltip>
```

**Our Implementation:**
```csharp
ToolTipManager.SetTooltip(label, "Prompt Text", new ToolTipConfig
{
    Theme = ToolTipTheme.Primary,
    ShowDelay = 500
});
```

### DevExpress (WinForms)
```csharp
toolTipController1.SetSuperTip(button, new SuperToolTip());
```

**Our Implementation:**
```csharp
ToolTipManager.SetTooltip(button, "Text", new ToolTipConfig
{
    Style = ToolTipStyle.Premium
});
```

## Benefits of This Architecture

1. **Maintainability**: Each file has single responsibility
2. **Scalability**: Easy to add new features in separate partial classes
3. **Testability**: Clear interfaces and separation of concerns
4. **Readability**: Organized by functionality, not by size
5. **Reusability**: Painter pattern allows custom styles
6. **Consistency**: Follows project patterns (Helper, Partial, Painter)
7. **Modern**: Based on industry-leading UI frameworks
8. **Professional**: Enterprise-grade architecture

## Migration from Old Code

If you have existing tooltip code using the old structure:

### Before:
```csharp
// Multiple classes in one file
// Direct instantiation
var tooltip = new CustomToolTip();
tooltip.Show(text, position);
```

### After:
```csharp
// Clean API through manager
await ToolTipManager.ShowTooltipAsync(text, position);

// Or attach to controls
ToolTipManager.SetTooltip(control, text);
```

## Future Enhancements

Possible additions without breaking current architecture:

1. **ToolTipManager.Configuration.cs** - Advanced configuration management
2. **ToolTipManager.Analytics.cs** - Usage tracking and metrics
3. **ToolTipManager.Presets.cs** - Pre-configured tooltip templates
4. **ToolTipFactory.cs** - Builder pattern for complex configurations
5. **ToolTipAnimator.cs** - Advanced animation engine
6. **ToolTipPositioner.cs** - Advanced positioning algorithms

## Summary

The ToolTipManager has been transformed from a monolithic file with multiple classes into a well-organized, modern, enterprise-grade tooltip system that:

✅ Follows your project's architecture patterns
✅ Implements best practices from leading UI frameworks
✅ Maintains one class per file
✅ Uses partial classes for logical separation
✅ Supports painter pattern for rendering
✅ Provides a clean, intuitive API
✅ Handles lifecycle and memory management automatically
✅ Is thread-safe and performant
✅ Supports accessibility features
✅ Is extensible and maintainable

This is production-ready code suitable for enterprise applications.
