# BeepTaskCard Enhancement Plan

## Overview
Enhance `BeepTaskCard` to follow the established Beep control patterns with centralized helpers, improved theme integration, font management, icon support, accessibility, tooltips, and modern UX/UI features.

## Current State Analysis

### Strengths
- ✅ Inherits from `BaseControl` (theme support, tooltips, accessibility foundation)
- ✅ Uses manual drawing with BeepButton/BeepLabel/BeepImage
- ✅ Basic theme integration in `ApplyTheme()`
- ✅ Gradient background support
- ✅ Shadow support
- ✅ Support for avatars, title, subtitle, metric, progress bar, and more icon

### Gaps Identified
- ❌ No centralized theme helpers (colors hardcoded in `ApplyTheme()`)
- ❌ No centralized font helpers (fonts created inline)
- ❌ No icon helpers (direct image path usage)
- ❌ Limited accessibility support (no ARIA attributes)
- ❌ No tooltip integration (BaseControl tooltips not utilized)
- ❌ Manual drawing code could use painter pattern
- ❌ No responsive sizing helpers
- ❌ Limited keyboard navigation support
- ❌ Progress bar is manually drawn (could use BeepProgressBar)

---

## Enhancement Architecture

### Phase 0: Helper Infrastructure (Prerequisite)
**Goal**: Create helper classes for centralized management

#### 0.1 Create TaskCardThemeHelpers
**File**: `Cards/Tasks/Helpers/TaskCardThemeHelpers.cs`

**Purpose**: Centralized theme color management

**Methods**:
- `GetTaskCardBackColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)`
- `GetTitleColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)`
- `GetSubtitleColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)`
- `GetMetricTextColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)`
- `GetProgressBarColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)`
- `GetProgressBarBackColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)`
- `GetIconColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)`
- `GetGradientColors(IBeepTheme theme, bool useThemeColors, Color? customStart = null, Color? customEnd = null)`
- `ApplyThemeColors(BeepTaskCard card, IBeepTheme theme, bool useThemeColors)`

#### 0.2 Create TaskCardFontHelpers
**File**: `Cards/Tasks/Helpers/TaskCardFontHelpers.cs`

**Purpose**: Centralized font management

**Methods**:
- `GetTitleFont(BeepTaskCard card, BeepControlStyle controlStyle)`
- `GetSubtitleFont(BeepTaskCard card, BeepControlStyle controlStyle)`
- `GetMetricFont(BeepTaskCard card, BeepControlStyle controlStyle)`
- `GetAvatarLabelFont(BeepTaskCard card, BeepControlStyle controlStyle)`
- `ApplyFontTheme(BeepTaskCard card, BeepControlStyle controlStyle)`

#### 0.3 Create TaskCardIconHelpers
**File**: `Cards/Tasks/Helpers/TaskCardIconHelpers.cs`

**Purpose**: Centralized icon management

**Methods**:
- `ResolveIconPath(string iconPath, string defaultIcon)`
- `GetRecommendedMoreIcon()`
- `GetRecommendedAvatarIcon()`
- `PaintIcon(Graphics g, Rectangle bounds, string iconPath, IBeepTheme theme, bool useThemeColors, Color? tintColor = null)`
- `PaintAvatar(Graphics g, Rectangle bounds, Image avatar, Color borderColor, int borderThickness = 2)`

#### 0.4 Create TaskCardAccessibilityHelpers
**File**: `Cards/Tasks/Helpers/TaskCardAccessibilityHelpers.cs`

**Purpose**: Accessibility support

**Methods**:
- `IsHighContrastMode()`
- `IsReducedMotionEnabled()`
- `GenerateAccessibleName(BeepTaskCard card, string customName = null)`
- `GenerateAccessibleDescription(BeepTaskCard card, string customDescription = null)`
- `ApplyAccessibilitySettings(BeepTaskCard card, string accessibleName = null, string accessibleDescription = null)`
- `GetHighContrastColors()`
- `AdjustColorsForHighContrast(...)`
- `CalculateContrastRatio(Color color1, Color color2)`
- `AdjustForContrast(Color foreColor, Color backColor, double minRatio = 4.5)`

#### 0.5 Create TaskCardLayoutHelpers
**File**: `Cards/Tasks/Helpers/TaskCardLayoutHelpers.cs`

**Purpose**: Responsive layout calculations

**Methods**:
- `CalculateAvatarBounds(Rectangle cardBounds, int avatarIndex, int avatarCount, Size avatarSize, int overlap, Padding padding)`
- `CalculateMoreIconBounds(Rectangle cardBounds, Size iconSize, Padding padding)`
- `CalculateTitleBounds(Rectangle cardBounds, Size avatarArea, Padding padding)`
- `CalculateSubtitleBounds(Rectangle cardBounds, Size titleSize, Padding padding)`
- `CalculateMetricBounds(Rectangle cardBounds, Padding padding)`
- `CalculateProgressBarBounds(Rectangle cardBounds, Size metricSize, Padding padding)`
- `GetOptimalCardSize(Padding padding)`

---

### Phase 1: Theme Integration 🎨
**Goal**: Integrate centralized theme helpers

#### 1.1 Update ApplyTheme()
**File**: `BeepTaskCard.cs`

**Changes**:
- Replace hardcoded color assignments with `TaskCardThemeHelpers.ApplyThemeColors()`
- Add `_isApplyingTheme` flag to prevent re-entrancy
- Ensure `UseThemeColors` property is respected
- Use theme helpers for gradient colors

#### 1.2 Update DrawContent()
**Changes**:
- Use theme helpers for all text colors
- Use theme helpers for icon colors
- Use theme helpers for progress bar colors

---

### Phase 2: Font Integration 📝
**Goal**: Integrate centralized font helpers

#### 2.1 Update Font Assignments
**File**: `BeepTaskCard.cs`

**Changes**:
- Replace inline font creation with `TaskCardFontHelpers.GetTitleFont()`
- Replace inline font creation with `TaskCardFontHelpers.GetSubtitleFont()`
- Replace inline font creation with `TaskCardFontHelpers.GetMetricFont()`
- Call `TaskCardFontHelpers.ApplyFontTheme()` in `ApplyTheme()`

---

### Phase 3: Icon Integration 🖼️
**Goal**: Integrate centralized icon helpers

#### 3.1 Update Icon Path Resolution
**File**: `BeepTaskCard.cs`

**Changes**:
- Use `TaskCardIconHelpers.ResolveIconPath()` for icon paths
- Use `TaskCardIconHelpers.GetRecommendedMoreIcon()` as default
- Use `TaskCardIconHelpers.GetRecommendedAvatarIcon()` as default

#### 3.2 Update Icon Rendering
**Changes**:
- Use `TaskCardIconHelpers.PaintIcon()` for icon rendering
- Use `TaskCardIconHelpers.PaintAvatar()` for avatar rendering
- Support SVG tinting based on theme colors

---

### Phase 4: Accessibility Enhancements ♿
**Goal**: Implement ARIA attributes, high contrast, reduced motion

#### 4.1 Add Accessibility Properties
**File**: `BeepTaskCard.cs`

**Changes**:
- Call `TaskCardAccessibilityHelpers.ApplyAccessibilitySettings()` in constructor
- Update accessibility attributes when properties change
- Set `AccessibleRole = AccessibleRole.Grouping`
- Set `AccessibleValue` to progress value

#### 4.2 High Contrast Support
**Changes**:
- Call `TaskCardAccessibilityHelpers.ApplyHighContrastAdjustments()` in `ApplyTheme()`
- Adjust colors when high contrast mode is detected
- Ensure text meets WCAG contrast requirements

#### 4.3 Reduced Motion Support
**Changes**:
- Disable animations when reduced motion is enabled
- Respect user preferences for motion

---

### Phase 5: Tooltip Integration 💬
**Goal**: Integrate with BaseControl tooltip system

#### 5.1 Add Tooltip Properties
**File**: `BeepTaskCard.cs`

**Changes**:
- Add `AutoGenerateTooltip` property (default: `true`)
- Add `UpdateTaskCardTooltip()` method
- Add `GenerateTaskCardTooltip()` method

#### 5.2 Tooltip Content
**Methods**:
- Generate tooltip text based on title, subtitle, metric, and progress
- Include progress percentage in tooltip
- Set appropriate `ToolTipType` based on progress (high = Success, low = Warning)

#### 5.3 Convenience Methods
**Methods**:
- `SetTaskCardTooltip(string text, string title = null, ToolTipType type = ToolTipType.Info)`
- `ShowTaskCardNotification(string message, ToolTipType type = ToolTipType.Info)`

---

### Phase 6: UX/UI Enhancements ✨
**Goal**: Add modern UX/UI features

#### 6.1 Layout Improvements
**Changes**:
- Use `TaskCardLayoutHelpers` for responsive layout calculations
- Improve spacing and alignment
- Support different card sizes

#### 6.2 Interaction Enhancements
**Changes**:
- Add click events for card, avatars, and more icon
- Add hover effects for interactive elements
- Add keyboard navigation support (Tab, Enter, Space)
- Add drag-and-drop support for task reordering

#### 6.3 Visual Enhancements
**Changes**:
- Add optional animation for progress bar updates
- Add optional hover effects
- Add optional border highlights
- Support different gradient styles
- Add optional avatar tooltips showing names

#### 6.4 Progress Bar Enhancement
**Changes**:
- Consider using `BeepProgressBar` control instead of manual drawing
- Or enhance manual progress bar with animations and effects

---

## Implementation Steps

1. **Phase 0**: Create all helper classes
2. **Phase 1**: Integrate theme helpers into `ApplyTheme()` and `DrawContent()`
3. **Phase 2**: Integrate font helpers
4. **Phase 3**: Integrate icon helpers
5. **Phase 4**: Add accessibility features
6. **Phase 5**: Add tooltip integration
7. **Phase 6**: Add UX/UI enhancements

---

## Benefits

- **Consistency**: Follows same pattern as other Beep controls
- **Maintainability**: Centralized helpers make updates easier
- **Accessibility**: Full ARIA and high contrast support
- **Flexibility**: Easy to extend and customize
- **User Experience**: Modern interactions and visual feedback
- **Theme Integration**: Seamless theme color/font/icon support

---

## Files to Create/Modify

### New Files
- `Cards/Tasks/Helpers/TaskCardThemeHelpers.cs`
- `Cards/Tasks/Helpers/TaskCardFontHelpers.cs`
- `Cards/Tasks/Helpers/TaskCardIconHelpers.cs`
- `Cards/Tasks/Helpers/TaskCardAccessibilityHelpers.cs`
- `Cards/Tasks/Helpers/TaskCardLayoutHelpers.cs`

### Modified Files
- `Cards/Tasks/BeepTaskCard.cs`

---

## Testing Checklist

- [ ] Theme colors apply correctly
- [ ] Fonts scale appropriately
- [ ] Icons render with correct colors
- [ ] Avatars render correctly with borders
- [ ] Progress bar renders correctly
- [ ] High contrast mode works
- [ ] Reduced motion is respected
- [ ] Tooltips display correctly
- [ ] Keyboard navigation works
- [ ] Layout is responsive
- [ ] Gradient backgrounds work correctly

