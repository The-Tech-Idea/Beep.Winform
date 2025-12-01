# BeepToggle Enhancement Plan

## Overview

This document outlines a comprehensive plan to enhance the `BeepToggle` control by integrating it with the Beep styling system, theme management, and helper architecture. The goal is to make `BeepToggle` consistent with other Beep controls like `BeepLogin`, `ToolTips`, and leverage the established patterns.

---

## Current State Analysis

### ✅ **Strengths**
- **Painter Pattern**: Already uses `BeepTogglePainterBase` and factory pattern
- **Multiple Styles**: 24+ toggle styles (Classic, Material, Icon-based, etc.)
- **Animation System**: Smooth animations with easing functions
- **Value Types**: Supports Boolean, String, Numeric, YesNo, OnOff, etc.
- **Hit Testing**: Region-based hit testing for better UX
- **Layout Helper**: `BeepToggleLayoutHelper` for layout calculations
- **Icon Support**: Custom icon paths with SVG support

### ❌ **Gaps & Missing Features**
- **No Theme Integration**: Colors are hardcoded, no `ApplyTheme()` support
- **No Theme Helpers**: No centralized theme color management
- **No Font Helpers**: Fonts are not integrated with `BeepFontManager`
- **No Icon Helpers**: Icon management not using `StyledImagePainter` consistently
- **No Style Helpers**: No integration with `BeepControlStyle`
- **Limited Accessibility**: No ARIA attributes, high contrast support
- **No Tooltip Integration**: Not using enhanced ToolTip system
- **Color Management**: Colors don't use theme colors from `IBeepTheme`
- **No ControlStyle Sync**: Doesn't respect `ControlStyle` from `BaseControl`

---

## Enhancement Architecture

### **Phase 1: Theme Integration** 🎨
**Goal**: Integrate `BeepToggle` with `ApplyTheme()` and theme color system

#### 1.1 Create Theme Helpers
**File**: `Toggle/Helpers/ToggleThemeHelpers.cs`

**Purpose**: Centralize theme color retrieval for toggle states

**Methods**:
- `GetToggleOnColor(IBeepTheme theme, bool useThemeColors, Color? customColor)`
- `GetToggleOffColor(IBeepTheme theme, bool useThemeColors, Color? customColor)`
- `GetToggleThumbColor(IBeepTheme theme, bool useThemeColors, bool isOn, Color? customColor)`
- `GetToggleTrackColor(IBeepTheme theme, bool useThemeColors, bool isOn, Color? customColor)`
- `GetToggleTextColor(IBeepTheme theme, bool useThemeColors, bool isOn)`
- `ApplyThemeColors(BeepToggle toggle, IBeepTheme theme, bool useThemeColors)`

**Theme Color Mapping**:
- ON state → `theme.SuccessColor` or `theme.PrimaryColor`
- OFF state → `theme.SecondaryColor` or `theme.DisabledBackColor`
- Thumb → `theme.SurfaceColor` or `Color.White`
- Text → `theme.ForeColor`

#### 1.2 Integrate ApplyTheme()
**File**: `BeepToggle.cs`

**Changes**:
- Override `ApplyTheme()` method
- Use `ToggleThemeHelpers` to get theme colors
- Update `OnColor`, `OffColor`, `ThumbColor` from theme
- Add `UseThemeColors` property (inherited from `BaseControl`)
- Add `_isApplyingTheme` flag to prevent re-entrancy

#### 1.3 Update Painters
**Files**: All painter classes

**Changes**:
- Painters use `ToggleThemeHelpers` instead of hardcoded colors
- Pass `theme` and `useThemeColors` to painters
- Update `GetTrackColor()` and `GetThumbColor()` to use theme helpers

---

### **Phase 2: Helper Architecture Enhancement** 🛠️
**Goal**: Create comprehensive helper classes following established patterns

#### 2.1 Font Helpers
**File**: `Toggle/Helpers/ToggleFontHelpers.cs`

**Purpose**: Manage fonts for toggle labels and text

**Methods**:
- `GetToggleFont(BeepToggle toggle, ToggleStyle style, BeepControlStyle controlStyle)`
- `GetLabelFont(BeepToggle toggle, bool isOn)`
- `GetIconFont(BeepToggle toggle)` (if using font icons)

**Integration**:
- Uses `BeepFontManager` for font retrieval
- Respects `ControlStyle` for font selection
- Supports accessibility fonts

#### 2.2 Icon Helpers
**File**: `Toggle/Helpers/ToggleIconHelpers.cs`

**Purpose**: Centralize icon management using `StyledImagePainter`

**Methods**:
- `GetIconPath(ToggleStyle style, bool isOn)` - Get icon path for style
- `PaintIcon(Graphics g, Rectangle bounds, string iconPath, Color tint, bool isOn)`
- `GetIconSize(BeepToggle toggle, ToggleStyle style)` - Calculate icon size
- `ResolveIconPath(string iconPath, ToggleStyle style, bool isOn)` - Resolve icon from various sources

**Integration**:
- Uses `StyledImagePainter` for all icon rendering
- Supports SVG paths from `SvgsUI`, `SvgsDatasources`, `Svgs`
- Caching via `StyledImagePainter` cache
- Theme tinting support

#### 2.3 Style Helpers
**File**: `Toggle/Helpers/ToggleStyleHelpers.cs`

**Purpose**: Map `ToggleStyle` to `BeepControlStyle` and determine styling properties

**Methods**:
- `GetControlStyleForToggle(ToggleStyle toggleStyle)` - Map toggle style to control style
- `GetBorderRadius(ToggleStyle style, BeepControlStyle controlStyle)`
- `GetTrackShape(ToggleStyle style)` - Returns `ToggleTrackShape`
- `GetThumbShape(ToggleStyle style)` - Returns `ToggleThumbShape`
- `ShouldShowShadow(ToggleStyle style, BeepControlStyle controlStyle)`
- `GetShadowColor(IBeepTheme theme, bool useThemeColors)`

#### 2.4 Layout Helpers Enhancement
**File**: `Toggle/Helpers/BeepToggleLayoutHelper.cs` (enhance existing)

**Enhancements**:
- Add DPI scaling support
- Add responsive sizing based on font size
- Add accessibility sizing (larger touch targets)
- Add `CalculateMetrics()` method returning `ToggleLayoutMetrics` model

#### 2.5 Animation Helpers
**File**: `Toggle/Helpers/ToggleAnimationHelpers.cs`

**Purpose**: Centralize animation calculations and easing functions

**Methods**:
- `CalculateThumbPosition(float progress, AnimationEasing easing, bool isOn)`
- `CalculateColorTransition(Color from, Color to, float progress)`
- `CalculateScale(float progress, AnimationEasing easing)`
- `GetEasingFunction(AnimationEasing easing)` - Returns easing function delegate
- `CalculateGlowIntensity(float progress, bool isOn)`

**Easing Functions** (enhance existing):
- `EaseOutCubic`, `EaseInOutCubic`, `EaseOutElastic`, `EaseOutBounce`, `EaseOutBack`
- Add: `EaseInQuad`, `EaseOutQuad`, `EaseInOutQuad`, `EaseInOutExpo`, `EaseInOutSine`

---

### **Phase 3: Model Classes** 📦
**Goal**: Create data models for configuration and metrics

#### 3.1 ToggleStyleConfig
**File**: `Toggle/Models/ToggleStyleConfig.cs`

**Properties**:
- `BorderRadius` - Corner radius for track/thumb
- `TrackShape` - Shape of track
- `ThumbShape` - Shape of thumb
- `ShowShadow` - Enable shadow effect
- `ShadowColor` - Shadow color
- `ShadowOffset` - Shadow offset
- `UseGradient` - Use gradient for track
- `GradientType` - Type of gradient
- `ControlStyle` - Associated `BeepControlStyle`

#### 3.2 ToggleLayoutMetrics
**File**: `Toggle/Models/ToggleLayoutMetrics.cs`

**Properties**:
- `TrackBounds` - Track rectangle
- `ThumbBounds` - Thumb rectangle
- `OnLabelBounds` - ON label rectangle
- `OffLabelBounds` - OFF label rectangle
- `IconBounds` - Icon rectangle
- `Padding` - Internal padding
- `ThumbSize` - Thumb dimensions
- `TrackSize` - Track dimensions

**Methods**:
- `HasRegion(string regionName)` - Check if region exists
- `GetRegionBounds(string regionName)` - Get bounds for region

#### 3.3 ToggleColorConfig
**File**: `Toggle/Models/ToggleColorConfig.cs`

**Properties**:
- `OnColor` - Color when ON
- `OffColor` - Color when OFF
- `ThumbColor` - Thumb color
- `OnThumbColor` - Thumb color when ON
- `OffThumbColor` - Thumb color when OFF
- `TextColor` - Text/label color
- `OnTextColor` - Text color when ON
- `OffTextColor` - Text color when OFF
- `BorderColor` - Border color
- `ShadowColor` - Shadow color

---

### **Phase 4: BaseControl Integration** 🔗
**Goal**: Better integration with `BaseControl` features

#### 4.1 ControlStyle Integration
**Changes**:
- Respect `ControlStyle` property from `BaseControl`
- Map `ToggleStyle` to appropriate `BeepControlStyle`
- Update colors when `ControlStyle` changes
- Use `BeepStyling` for background/border painting

#### 4.2 Tooltip Integration
**Changes**:
- Use enhanced ToolTip system from `BaseControl`
- Auto-generate tooltips based on state: "Toggle ON" / "Toggle OFF"
- Support custom tooltip text
- Use `TooltipType` based on toggle state (Success for ON, Default for OFF)

#### 4.3 Font Integration
**Changes**:
- Use `BeepFontManager` for font retrieval
- Respect `TextFont` property from `BaseControl`
- Support accessibility fonts
- Auto-adjust font size based on control size

#### 4.4 Icon Integration
**Changes**:
- Use `StyledImagePainter` for all icon rendering
- Support SVG icons from `SvgsUI`, `SvgsDatasources`, `Svgs`
- Theme tinting for icons
- Icon caching

---

### **Phase 5: Accessibility Enhancements** ♿
**Goal**: Improve accessibility for screen readers and keyboard navigation

#### 5.1 ARIA Attributes
**Changes**:
- Set `AccessibleName` based on `OnText`/`OffText`
- Set `AccessibleDescription` with current state
- Set `AccessibleRole` to `CheckBox`
- Set `AccessibleValue` to current value

#### 5.2 Keyboard Support
**Changes**:
- Space bar toggles state
- Enter key toggles state
- Tab navigation support
- Focus indicators (using glow effect)

#### 5.3 High Contrast Support
**Changes**:
- Detect `SystemInformation.HighContrast`
- Use high contrast colors when enabled
- Ensure minimum contrast ratios (WCAG AA)
- Disable animations in high contrast mode

#### 5.4 Reduced Motion Support
**Changes**:
- Detect `SystemParameters.MinimizeAnimation`
- Disable animations when reduced motion is preferred
- Instant state changes

---

### **Phase 6: Animation Enhancements** 🎬
**Goal**: Enhance animation system with more options and better performance

#### 6.1 Animation Types
**Add**:
- `AnimationType` enum: `Slide`, `Fade`, `Scale`, `Rotate`, `Bounce`, `Elastic`
- `AnimationDirection`: `LeftToRight`, `RightToLeft`, `TopToBottom`, `BottomToTop`
- `AnimationEasing` (already exists, enhance)

#### 6.2 Performance Optimizations
**Changes**:
- Use `CompositingMode` for better performance
- Cache animation frames when possible
- Reduce redraws during animation
- Use `Invalidate()` with specific regions

#### 6.3 Animation Events
**Add**:
- `AnimationStarted` event
- `AnimationCompleted` event
- `AnimationProgressChanged` event (for progress tracking)

---

### **Phase 7: Advanced Features** 🚀
**Goal**: Add advanced features for power users

#### 7.1 Multi-State Toggle
**Add**:
- Support for 3+ states (e.g., Low/Medium/High)
- `ToggleState` enum with multiple values
- State-specific colors and icons

#### 7.2 Custom Painters
**Add**:
- `RegisterCustomPainter(ToggleStyle style, Type painterType)` - Register custom painters
- `CreateCustomPainter()` - Factory method for custom painters

#### 7.3 Data Binding Enhancements
**Enhance**:
- Better support for complex data binding
- Two-way binding with validation
- Binding error handling

#### 7.4 Validation Support
**Add**:
- `Validate()` method
- `ValidationError` event
- `IsValid` property
- Integration with validation framework

---

## Implementation Phases

### **Phase 1: Theme Integration** (Priority: HIGH)
**Estimated Time**: 2-3 hours

**Tasks**:
1. Create `ToggleThemeHelpers.cs`
2. Integrate `ApplyTheme()` in `BeepToggle.cs`
3. Update all painters to use theme helpers
4. Add `UseThemeColors` property
5. Test theme switching

**Deliverables**:
- ✅ Theme colors from `ApplyTheme()` are used
- ✅ Toggle colors update when theme changes
- ✅ All painters respect theme colors

---

### **Phase 2: Helper Architecture** (Priority: HIGH)
**Estimated Time**: 3-4 hours

**Tasks**:
1. Create `ToggleFontHelpers.cs`
2. Create `ToggleIconHelpers.cs`
3. Create `ToggleStyleHelpers.cs`
4. Enhance `BeepToggleLayoutHelper.cs`
5. Create `ToggleAnimationHelpers.cs`
6. Create model classes (`ToggleStyleConfig`, `ToggleLayoutMetrics`, `ToggleColorConfig`)

**Deliverables**:
- ✅ Centralized helper classes
- ✅ Consistent with other Beep controls
- ✅ Easy to extend and maintain

---

### **Phase 3: BaseControl Integration** (Priority: MEDIUM)
**Estimated Time**: 2-3 hours

**Tasks**:
1. Integrate `ControlStyle` property
2. Integrate ToolTip system
3. Integrate font management
4. Integrate icon management
5. Use `BeepStyling` for backgrounds/borders

**Deliverables**:
- ✅ Toggle respects `ControlStyle`
- ✅ Tooltips work automatically
- ✅ Fonts use `BeepFontManager`
- ✅ Icons use `StyledImagePainter`

---

### **Phase 4: Accessibility** (Priority: MEDIUM)
**Estimated Time**: 2-3 hours

**Tasks**:
1. Add ARIA attributes
2. Add keyboard support
3. Add high contrast detection
4. Add reduced motion support
5. Ensure WCAG compliance

**Deliverables**:
- ✅ Screen reader support
- ✅ Keyboard navigation
- ✅ High contrast mode
- ✅ Reduced motion support

---

### **Phase 5: Animation Enhancements** (Priority: LOW)
**Estimated Time**: 2-3 hours

**Tasks**:
1. Add more animation types
2. Optimize animation performance
3. Add animation events
4. Enhance easing functions

**Deliverables**:
- ✅ More animation options
- ✅ Better performance
- ✅ Animation event support

---

### **Phase 6: Advanced Features** (Priority: LOW)
**Estimated Time**: 3-4 hours

**Tasks**:
1. Multi-state toggle support
2. Custom painter registration
3. Enhanced data binding
4. Validation support

**Deliverables**:
- ✅ Multi-state toggles
- ✅ Custom painter support
- ✅ Better data binding
- ✅ Validation framework

---

## File Structure

```
Toggle/
├── BeepToggle.cs (main control)
├── BeepToggle.Values.cs (value handling)
├── BeepToggle.Animation.cs (animation)
├── BeepToggle.HitTest.cs (hit testing)
├── ToggleEnums.cs (enums)
├── TOGGLE_ENHANCEMENT_PLAN.md (this file)
│
├── Helpers/
│   ├── BeepToggleLayoutHelper.cs (existing, enhance)
│   ├── ToggleThemeHelpers.cs (NEW)
│   ├── ToggleFontHelpers.cs (NEW)
│   ├── ToggleIconHelpers.cs (NEW)
│   ├── ToggleStyleHelpers.cs (NEW)
│   └── ToggleAnimationHelpers.cs (NEW)
│
├── Models/
│   ├── ToggleStyleConfig.cs (NEW)
│   ├── ToggleLayoutMetrics.cs (NEW)
│   └── ToggleColorConfig.cs (NEW)
│
└── Painters/
    ├── BeepTogglePainterBase.cs (existing, enhance)
    ├── BeepTogglePainterFactory.cs (existing, enhance)
    └── [Various painter implementations] (existing, enhance)
```

---

## Integration Points

### **With BeepStyling**
- Use `BeepStyling.PaintControl()` for backgrounds
- Use `BeepStyling.PaintStyleBorder()` for borders
- Use `BeepStyling.GetBackgroundColor()` for theme colors
- Use `BeepStyling.GetForegroundColor()` for text colors

### **With BeepFontManager**
- Use `BeepFontManager.GetFont()` for font retrieval
- Support accessibility fonts
- Respect font preferences

### **With StyledImagePainter**
- Use `StyledImagePainter.Paint()` for all icons
- Use `StyledImagePainter.PaintWithTint()` for themed icons
- Leverage caching

### **With ToolTipManager**
- Auto-generate tooltips
- Use `ToolTipType.Success` for ON state
- Use `ToolTipType.Default` for OFF state

### **With BaseControl**
- Inherit `UseThemeColors` property
- Use `_currentTheme` from `BaseControl`
- Respect `ControlStyle` property
- Use `ApplyTheme()` method

---

## Benefits

1. **Theme Consistency**: Toggle colors match application theme
2. **Maintainability**: Centralized helpers make code easier to maintain
3. **Extensibility**: Easy to add new styles and features
4. **Accessibility**: Better support for users with disabilities
5. **Performance**: Optimized rendering and caching
6. **Consistency**: Follows established Beep patterns

---

## Testing Checklist

### Theme Integration
- [ ] Toggle colors update when theme changes
- [ ] Theme colors are used when `UseThemeColors = true`
- [ ] Custom colors work when `UseThemeColors = false`
- [ ] All painters respect theme colors

### Helper Architecture
- [ ] Font helpers return correct fonts
- [ ] Icon helpers render icons correctly
- [ ] Style helpers map styles correctly
- [ ] Layout helpers calculate correct positions

### BaseControl Integration
- [ ] `ControlStyle` affects toggle appearance
- [ ] Tooltips work automatically
- [ ] Fonts use `BeepFontManager`
- [ ] Icons use `StyledImagePainter`

### Accessibility
- [ ] Screen readers announce toggle state
- [ ] Keyboard navigation works
- [ ] High contrast mode works
- [ ] Reduced motion is respected

---

## Migration Guide

### For Existing Code

**Before**:
```csharp
var toggle = new BeepToggle
{
    OnColor = Color.Green,
    OffColor = Color.Gray,
    ThumbColor = Color.White
};
```

**After** (with theme):
```csharp
var toggle = new BeepToggle
{
    UseThemeColors = true  // Uses theme colors automatically
};
toggle.ApplyTheme();  // Apply theme colors
```

**After** (custom colors):
```csharp
var toggle = new BeepToggle
{
    UseThemeColors = false,  // Use custom colors
    OnColor = Color.Green,
    OffColor = Color.Gray
};
```

---

## Next Steps

1. **Review this plan** with the team
2. **Prioritize phases** based on requirements
3. **Start with Phase 1** (Theme Integration)
4. **Iterate** through phases
5. **Test** thoroughly after each phase
6. **Document** changes and usage

---

## References

- `BeepLogin` - Example of painter/helper architecture
- `ToolTips` - Example of theme integration
- `BaseControl` - Base class with theme support
- `BeepStyling` - Central styling system
- `StyledImagePainter` - Icon rendering system
- `BeepFontManager` - Font management system

