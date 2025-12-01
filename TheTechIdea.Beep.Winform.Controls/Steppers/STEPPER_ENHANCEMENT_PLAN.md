# BeepStepperBar Enhancement Plan

## Overview

This document outlines a comprehensive plan to enhance the `BeepStepperBar` and `BeepStepperBreadCrumb` controls by integrating them with the Beep styling system, theme management, font management, icon management, accessibility features, and tooltip integration. The goal is to make Steppers controls consistent with other Beep controls like `BeepToggle`, `BeepBreadcrump`, `BeepProgressBar`, and `BeepLogin`.

---

## Current State Analysis

### ✅ **Strengths**
- **Step Management**: Comprehensive step state management (Pending, Active, Completed, Error, Warning)
- **Animation System**: Smooth step transitions with timer-based animations
- **Orientation Support**: Both horizontal and vertical layouts
- **Display Modes**: StepNumber, CheckImage, SvgIcon modes
- **Business Logic**: Auto-progress, step navigation, validation events
- **Event System**: StepChanged, StepValidating, StepCompleted, AllStepsCompleted events
- **Connector Lines**: Visual connectors between steps
- **Basic Theme Integration**: `ApplyTheme()` method exists
- **Hit Testing**: Mouse click detection for step navigation
- **Legacy Support**: Backward compatibility with TasksView

### ❌ **Gaps & Missing Features**
- **No Centralized Theme Helpers**: Colors are retrieved directly from theme, no helper abstraction
- **No Font Helpers**: Fonts are hardcoded, not integrated with `BeepFontManager` and `StyleTypography`
- **No Icon Helpers**: Icon management not using `StyledImagePainter` consistently (uses `ImageLoader.LoadImageFromResource` directly)
- **Limited Accessibility**: No ARIA attributes, high contrast support, reduced motion preferences
- **No Tooltip Integration**: Not using enhanced ToolTip system from `BaseControl`
- **No ControlStyle Sync**: Doesn't fully respect `ControlStyle` from `BaseControl` for styling
- **Inherits from BeepControl**: Should inherit from `BaseControl` to get full tooltip and accessibility features
- **Manual Color Management**: Colors are hardcoded with no theme-aware fallbacks
- **No Font Management**: Uses hardcoded fonts instead of `BeepFontManager`
- **No Icon Management**: Uses `ImageLoader` directly instead of `StyledImagePainter`
- **Limited Theme Integration**: Basic theme colors but no comprehensive theme helper system

---

## Enhancement Architecture

### **Phase 1: Theme Integration** 🎨
**Goal**: Enhance theme integration with centralized theme helpers

#### 1.1 Create Theme Helpers
**File**: `Steppers/Helpers/StepperThemeHelpers.cs`

**Purpose**: Centralize theme color retrieval for stepper elements

**Methods**:
- `GetStepCompletedColor(IBeepTheme theme, bool useThemeColors, Color? customColor)`
- `GetStepActiveColor(IBeepTheme theme, bool useThemeColors, Color? customColor)`
- `GetStepPendingColor(IBeepTheme theme, bool useThemeColors, Color? customColor)`
- `GetStepErrorColor(IBeepTheme theme, bool useThemeColors, Color? customColor)`
- `GetStepWarningColor(IBeepTheme theme, bool useThemeColors, Color? customColor)`
- `GetConnectorLineColor(IBeepTheme theme, bool useThemeColors, StepState state, Color? customColor)`
- `GetStepTextColor(IBeepTheme theme, bool useThemeColors, StepState state, Color? customColor)`
- `GetStepLabelColor(IBeepTheme theme, bool useThemeColors, StepState state, Color? customColor)`
- `GetStepBackgroundColor(IBeepTheme theme, bool useThemeColors, Color? customColor)`
- `GetStepBorderColor(IBeepTheme theme, bool useThemeColors, StepState state, Color? customColor)`
- `ApplyThemeColors(BeepStepperBar stepper, IBeepTheme theme, bool useThemeColors)`
- `GetThemeColors(IBeepTheme theme, bool useThemeColors, StepState state)` - Returns tuple of all colors for a state

**Theme Color Mapping**:
- Completed → `theme.StepperCompletedColor` or `theme.SuccessColor`
- Active → `theme.StepperActiveColor` or `theme.PrimaryColor`
- Pending → `theme.StepperPendingColor` or `theme.DisabledBackColor`
- Error → `theme.StepperErrorColor` or `theme.ErrorColor`
- Warning → `theme.StepperWarningColor` or `theme.WarningColor`
- Connector (Completed) → `theme.StepperConnectorCompletedColor` or `theme.SuccessColor`
- Connector (Pending) → `theme.StepperConnectorPendingColor` or `theme.DisabledBackColor`
- Text (Active) → `theme.StepperActiveTextColor` or `theme.PrimaryTextColor`
- Text (Pending) → `theme.StepperPendingTextColor` or `theme.SecondaryTextColor`
- Label (Active) → `theme.StepperActiveLabelColor` or `theme.CardTitleForeColor`
- Label (Pending) → `theme.StepperPendingLabelColor` or `theme.CardSubTitleForeColor`
- Background → `theme.StepperBackColor` or `theme.CardBackColor`
- Border → `theme.StepperBorderColor` or `theme.BorderColor`

#### 1.2 Enhance ApplyTheme()
**File**: `BeepStepperBar.cs`

**Changes**:
- Use `StepperThemeHelpers` to get theme colors
- Update all color properties from theme helpers (`CompletedStepColor`, `ActiveStepColor`, `PendingStepColor`, `ErrorStepColor`, `WarningStepColor`)
- Ensure `UseThemeColors` property is properly utilized
- Add `_isApplyingTheme` flag to prevent re-entrancy
- Sync `ControlStyle` property with `BaseControl.ControlStyle` (if inheriting from BaseControl)
- Update connector line colors based on step states

#### 1.3 Update BeepStepperBreadCrumb
**File**: `BeepStepperBreadCrumb.cs`

**Changes**:
- Use `StepperThemeHelpers` for color management
- Integrate theme colors for chevron fills and borders
- Update `ApplyTheme()` to use helpers

---

### **Phase 2: Font Integration** 🔤
**Goal**: Integrate font management with `BeepFontManager` and `StyleTypography`

#### 2.1 Create Font Helpers
**File**: `Steppers/Helpers/StepperFontHelpers.cs`

**Purpose**: Manage fonts for stepper text, labels, and step numbers

**Methods**:
- `GetStepNumberFont(BeepStepperBar stepper, BeepControlStyle controlStyle)`
- `GetStepLabelFont(BeepStepperBar stepper, BeepControlStyle controlStyle, StepState state)`
- `GetStepTextFont(BeepStepperBar stepper, BeepControlStyle controlStyle)`
- `GetStepperFont(BeepStepperBar stepper, BeepControlStyle controlStyle)`
- `GetFontSizeForElement(BeepControlStyle controlStyle, StepperFontElement element)`
- `GetFontStyleForElement(BeepControlStyle controlStyle, StepperFontElement element)`
- `ApplyFontTheme(BeepStepperBar stepper, BeepControlStyle controlStyle)`

**Font Elements**:
- `StepNumber`: Number displayed inside step circle
- `StepLabel`: Label text below/next to step
- `StepText`: Additional text in step
- `Connector`: Text on connector lines (if any)

**Font Sizing**:
- Based on `BeepControlStyle` and `ButtonSize`
- Responsive to control size
- Uses `BeepFontManager.GetFont()` with appropriate family and size

#### 2.2 Enhance Font Usage
**File**: `BeepStepperBar.cs`

**Changes**:
- Replace hardcoded fonts with `StepperFontHelpers` calls
- Update `DrawStepNumber()` to use `StepperFontHelpers.GetStepNumberFont()`
- Update `DrawStepLabel()` to use `StepperFontHelpers.GetStepLabelFont()`
- Integrate font updates in `ApplyTheme()`

---

### **Phase 3: Icon Integration** 🎨
**Goal**: Integrate icon management with `StyledImagePainter` and icon helpers

#### 3.1 Create Icon Helpers
**File**: `Steppers/Helpers/StepperIconHelpers.cs`

**Purpose**: Manage icons for stepper steps (checkmarks, step icons, SVG icons)

**Methods**:
- `GetCheckIconPath()` - Returns recommended check icon path
- `GetStepIconPath(SimpleItem item, StepState state)` - Resolves icon path from item or default
- `GetIconColor(IBeepTheme theme, bool useThemeColors, StepState state, Color? customColor)`
- `GetIconSize(BeepStepperBar stepper, Size buttonSize)` - Calculates icon size based on button size
- `CalculateIconBounds(Rectangle stepRect, Size iconSize)` - Calculates icon position within step
- `PaintIcon(Graphics g, Rectangle bounds, string iconPath, Color tint, float opacity, StepState state)` - Paints icon using `StyledImagePainter`
- `PaintCheckmarkIcon(Graphics g, Rectangle bounds, Color tint, float opacity)` - Paints checkmark icon
- `PaintStepIcon(Graphics g, Rectangle bounds, SimpleItem item, StepState state, IBeepTheme theme, bool useThemeColors)` - Paints step-specific icon
- `ResolveIconPath(string iconPath, StepDisplayMode displayMode, StepState state)` - Resolves icon path from multiple sources
- `GetRecommendedIcon(StepState state, StepDisplayMode displayMode)` - Returns recommended icon for state/mode

**Icon Sources**:
- `SimpleItem.ImagePath` or `SimpleItem.IconPath`
- `CheckImage` property for completed steps
- Default icons from `SvgsUI`, `SvgsDatasources`, `Svgs`
- State-based icons (check.svg for completed, warning.svg for warning, error.svg for error)

#### 3.2 Update Icon Painting
**File**: `BeepStepperBar.cs`

**Changes**:
- Replace `ImageLoader.LoadImageFromResource()` with `StepperIconHelpers.PaintIcon()`
- Replace manual checkmark drawing with `StepperIconHelpers.PaintCheckmarkIcon()`
- Update `DrawStepContent()` to use icon helpers
- Use `StyledImagePainter` for all icon rendering
- Support SVG icons with theme color tinting

---

### **Phase 4: Accessibility Enhancements** ♿
**Goal**: Add comprehensive accessibility features

#### 4.1 Create Accessibility Helpers
**File**: `Steppers/Helpers/StepperAccessibilityHelpers.cs`

**Purpose**: Centralize accessibility logic for steppers

**Methods**:
- `IsHighContrastMode()` - Detects Windows high contrast mode
- `IsReducedMotionEnabled()` - Detects reduced motion preference
- `GenerateAccessibleName(BeepStepperBar stepper, int stepIndex)` - Generates ARIA name
- `GenerateAccessibleDescription(BeepStepperBar stepper, int stepIndex)` - Generates ARIA description
- `GenerateAccessibleValue(BeepStepperBar stepper)` - Generates ARIA value (e.g., "Step 2 of 5")
- `ApplyAccessibilitySettings(BeepStepperBar stepper)` - Applies ARIA attributes
- `GetHighContrastColors(StepState state)` - Returns high contrast colors
- `AdjustColorsForHighContrast(Color color, Color background)` - Adjusts colors for high contrast
- `ApplyHighContrastAdjustments(BeepStepperBar stepper, IBeepTheme theme, bool useThemeColors)` - Applies high contrast mode
- `CalculateContrastRatio(Color foreground, Color background)` - WCAG contrast calculation
- `GetRelativeLuminance(Color color)` - Relative luminance for WCAG
- `EnsureContrastRatio(Color foreground, Color background, double minRatio)` - Ensures minimum contrast
- `AdjustForContrast(Color foreground, Color background, double minRatio)` - Adjusts color to meet contrast
- `ShouldDisableAnimations()` - Checks if animations should be disabled
- `GetAccessibleMinimumSize(Size currentSize)` - Ensures minimum touch target (44x44px)
- `GetAccessibleStepSpacing(int currentSpacing)` - Adjusts spacing for accessibility

#### 4.2 Integrate Accessibility
**File**: `BeepStepperBar.cs`

**Changes**:
- Add `AccessibleName`, `AccessibleDescription`, `AccessibleRole`, `AccessibleValue` properties
- Implement `ApplyAccessibilitySettings()` method
- Call `ApplyAccessibilitySettings()` in constructor and when step changes
- Integrate high contrast mode detection and color adjustments
- Respect reduced motion preferences (disable animations if enabled)
- Ensure minimum accessible sizes for step buttons
- Add keyboard navigation support (Arrow keys, Enter, Space)
- Update ARIA attributes when step state changes

#### 4.3 Update Animation
**File**: `BeepStepperBar.cs` (Animation section)

**Changes**:
- Disable animations if `StepperAccessibilityHelpers.IsReducedMotionEnabled()` returns true
- Respect reduced motion in `StartStepAnimation()`
- Make animation duration configurable and respect accessibility settings

---

### **Phase 5: Tooltip Integration** 💬
**Goal**: Integrate tooltip system from `BaseControl`

#### 5.1 Inherit Tooltip Features
**Note**: `BeepStepperBar` currently inherits from `BeepControl`. To get full tooltip support, it should inherit from `BaseControl`. However, if `BeepControl` already provides tooltip support, we can work with that.

**If inheriting from BaseControl**:
- All tooltip properties and methods are automatically available
- `TooltipText`, `TooltipTitle`, `TooltipType`, `EnableTooltip`, etc.

**If staying with BeepControl**:
- Add tooltip properties similar to `BaseControl`
- Integrate with `ToolTipManager` for tooltip display

#### 5.2 Add Auto-Generated Tooltips
**File**: `BeepStepperBar.cs`

**Properties**:
- `AutoGenerateTooltips` (bool) - Auto-generate tooltips from step labels

**Methods**:
- `UpdateStepperTooltip()` - Updates tooltip based on current step
- `GenerateStepTooltip(int stepIndex)` - Generates tooltip text for a step
- `SetStepTooltip(int stepIndex, string text, string title, ToolTipType type)` - Set custom tooltip for a step
- `ShowStepNotification(int stepIndex, bool showOnChange)` - Show notification when step changes

**Tooltip Content**:
- Current step: "Step X of Y: [Step Label]"
- Completed step: "Step X: [Step Label] (Completed)"
- Active step: "Current step: [Step Label]"
- Pending step: "Step X: [Step Label] (Pending)"
- Error step: "Step X: [Step Label] (Error)"
- Warning step: "Step X: [Step Label] (Warning)"

#### 5.3 Step-Level Tooltips
**File**: `BeepStepperBar.cs`

**Features**:
- Each step can have its own tooltip (from `SimpleItem.Tooltip` or auto-generated)
- Tooltip shown on hover over step
- Tooltip updates when step state changes
- Integration with `ToolTipManager` for consistent tooltip display

---

## Implementation Phases

### **Phase 1: Theme Integration**
**Estimated Time**: 1-2 days

**Tasks**:
1. Create `Steppers/Helpers/StepperThemeHelpers.cs`
2. Implement all theme color retrieval methods
3. Update `BeepStepperBar.ApplyTheme()` to use helpers
4. Update `BeepStepperBreadCrumb.ApplyTheme()` to use helpers
5. Update painting code to use theme helpers
6. Test with multiple themes

**Deliverables**:
- `StepperThemeHelpers.cs`
- Enhanced `ApplyTheme()` methods
- Updated painting code
- `PHASE1_IMPLEMENTATION.md`

---

### **Phase 2: Font Integration**
**Estimated Time**: 1 day

**Tasks**:
1. Create `Steppers/Helpers/StepperFontHelpers.cs`
2. Implement font retrieval methods
3. Update `DrawStepNumber()` to use font helpers
4. Update `DrawStepLabel()` to use font helpers
5. Integrate font updates in `ApplyTheme()`
6. Test with different `ControlStyle` values

**Deliverables**:
- `StepperFontHelpers.cs`
- Updated drawing methods
- Font integration in `ApplyTheme()`
- `PHASE2_IMPLEMENTATION.md`

---

### **Phase 3: Icon Integration**
**Estimated Time**: 1-2 days

**Tasks**:
1. Create `Steppers/Helpers/StepperIconHelpers.cs`
2. Implement icon path resolution methods
3. Implement icon painting methods using `StyledImagePainter`
4. Update `DrawStepContent()` to use icon helpers
5. Replace `ImageLoader` calls with `StyledImagePainter`
6. Support SVG icons with theme color tinting
7. Test with different icon sources

**Deliverables**:
- `StepperIconHelpers.cs`
- Updated icon painting code
- SVG icon support
- `PHASE3_IMPLEMENTATION.md`

---

### **Phase 4: Accessibility Enhancements**
**Estimated Time**: 2 days

**Tasks**:
1. Create `Steppers/Helpers/StepperAccessibilityHelpers.cs`
2. Implement system detection methods (high contrast, reduced motion)
3. Implement ARIA attribute generation methods
4. Implement WCAG contrast calculation and adjustment
5. Add accessibility properties to `BeepStepperBar`
6. Implement `ApplyAccessibilitySettings()` method
7. Integrate accessibility into constructor and step change events
8. Add keyboard navigation support
9. Update animation to respect reduced motion
10. Test with screen readers and accessibility tools

**Deliverables**:
- `StepperAccessibilityHelpers.cs`
- Accessibility properties and methods
- Keyboard navigation support
- High contrast and reduced motion support
- `PHASE4_IMPLEMENTATION.md`

---

### **Phase 5: Tooltip Integration**
**Estimated Time**: 1 day

**Tasks**:
1. Determine if `BeepStepperBar` should inherit from `BaseControl` (recommended) or add tooltip support to `BeepControl`
2. Add `AutoGenerateTooltips` property
3. Implement `UpdateStepperTooltip()` method
4. Implement `GenerateStepTooltip()` method
5. Implement step-level tooltip management
6. Integrate tooltip updates into step change events
7. Add convenience methods (`SetStepTooltip`, `ShowStepNotification`)
8. Test tooltip display and updates

**Deliverables**:
- Tooltip integration code
- Auto-generated tooltip support
- Step-level tooltip management
- `PHASE5_IMPLEMENTATION.md`

---

## Architecture Decisions

### **Inheritance Decision**
**Current**: `BeepStepperBar` inherits from `BeepControl`
**Recommendation**: Consider inheriting from `BaseControl` to get:
- Full tooltip support
- Enhanced accessibility features
- Better theme integration
- Consistent API with other Beep controls

**If changing inheritance**:
- Ensure backward compatibility
- Test all existing functionality
- Update any `BeepControl`-specific code

**If keeping `BeepControl`**:
- Add tooltip support to `BeepControl` or integrate with `ToolTipManager` directly
- Ensure accessibility features are available

### **Helper Pattern**
Following the established pattern from other controls:
- Centralized helpers in `Steppers/Helpers/` directory
- Static methods for easy access
- Theme, font, icon, accessibility, and tooltip helpers
- Consistent API across all helpers

### **Painter Pattern** (Future Enhancement)
The existing `plan.md` discusses a painter pattern for visual styles. This is separate from the helper-based enhancement and can be implemented later. The current enhancement focuses on:
- Theme integration
- Font management
- Icon management
- Accessibility
- Tooltip integration

The painter pattern can be added as Phase 6 if desired.

---

## Benefits

### **Consistency**
- Steppers controls will be consistent with other Beep controls
- Same helper pattern and API structure
- Unified theme, font, and icon management

### **Maintainability**
- Centralized logic in helper classes
- Easy to update and extend
- Clear separation of concerns

### **User Experience**
- Better accessibility support
- Rich tooltips for better user guidance
- Theme-aware colors and fonts
- Responsive to user preferences (high contrast, reduced motion)

### **Developer Experience**
- Easy to use helper methods
- Consistent API across controls
- Well-documented enhancement phases

---

## Testing Strategy

### **Theme Testing**
- Test with all available themes
- Verify color mapping and fallbacks
- Test `UseThemeColors` property

### **Font Testing**
- Test with different `ControlStyle` values
- Verify font sizing and styling
- Test responsive font sizing

### **Icon Testing**
- Test with different icon sources (SVG, raster, embedded)
- Verify theme color tinting
- Test icon sizing and positioning

### **Accessibility Testing**
- Test with screen readers (NVDA, JAWS)
- Test high contrast mode
- Test reduced motion preferences
- Verify keyboard navigation
- Test WCAG contrast compliance

### **Tooltip Testing**
- Test auto-generated tooltips
- Test custom step tooltips
- Test tooltip updates on step changes
- Verify tooltip positioning and styling

---

## Acceptance Criteria

### **Phase 1: Theme Integration**
- ✅ `StepperThemeHelpers` created with all color methods
- ✅ `ApplyTheme()` uses theme helpers
- ✅ All step colors are theme-aware
- ✅ Connector line colors are theme-aware
- ✅ Works with all available themes

### **Phase 2: Font Integration**
- ✅ `StepperFontHelpers` created with all font methods
- ✅ Step numbers use font helpers
- ✅ Step labels use font helpers
- ✅ Fonts are responsive to `ControlStyle`
- ✅ Fonts integrate with `BeepFontManager`

### **Phase 3: Icon Integration**
- ✅ `StepperIconHelpers` created with all icon methods
- ✅ Icons use `StyledImagePainter`
- ✅ SVG icons supported with theme tinting
- ✅ Icon paths resolved from multiple sources
- ✅ Checkmark icons use helpers

### **Phase 4: Accessibility**
- ✅ `StepperAccessibilityHelpers` created
- ✅ ARIA attributes implemented
- ✅ High contrast mode supported
- ✅ Reduced motion respected
- ✅ Keyboard navigation works
- ✅ WCAG contrast compliance
- ✅ Minimum accessible sizes enforced

### **Phase 5: Tooltip Integration**
- ✅ Auto-generated tooltips work
- ✅ Step-level tooltips supported
- ✅ Tooltips update on step changes
- ✅ Tooltips use theme colors
- ✅ Tooltips are accessible

---

## Files to Create/Modify

### **New Files**
- `Steppers/Helpers/StepperThemeHelpers.cs`
- `Steppers/Helpers/StepperFontHelpers.cs`
- `Steppers/Helpers/StepperIconHelpers.cs`
- `Steppers/Helpers/StepperAccessibilityHelpers.cs`
- `Steppers/STEPPER_ENHANCEMENT_PLAN.md` (this file)
- `Steppers/PHASE1_IMPLEMENTATION.md`
- `Steppers/PHASE2_IMPLEMENTATION.md`
- `Steppers/PHASE3_IMPLEMENTATION.md`
- `Steppers/PHASE4_IMPLEMENTATION.md`
- `Steppers/PHASE5_IMPLEMENTATION.md`

### **Modified Files**
- `Steppers/BeepStepperBar.cs`
- `Steppers/BeepStepperBreadCrumb.cs`

---

## Next Steps

1. **Review and Approve Plan**: Review this enhancement plan and approve the approach
2. **Start Phase 1**: Begin with Theme Integration
3. **Iterative Implementation**: Complete each phase before moving to the next
4. **Testing**: Test each phase thoroughly before proceeding
5. **Documentation**: Document each phase's implementation

---

## Notes

- This plan follows the same pattern as `BeepProgressBar`, `BeepToggle`, `BeepBreadcrump`, and `BeepLogin` enhancements
- The existing `plan.md` discusses a painter pattern for visual styles, which is separate and can be implemented later
- Consider whether `BeepStepperBar` should inherit from `BaseControl` for full tooltip and accessibility support
- All helpers follow the established pattern from other Beep controls
- Theme colors use fallbacks if specific theme properties don't exist

---

*Plan created on: [Current Date]*
*Based on enhancement patterns from: BeepProgressBar, BeepToggle, BeepBreadcrump, BeepLogin*

