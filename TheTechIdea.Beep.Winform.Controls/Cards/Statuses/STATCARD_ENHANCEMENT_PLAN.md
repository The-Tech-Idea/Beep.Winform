# BeepStatCard Enhancement Plan

## Overview
Enhance `BeepStatCard` to follow the established Beep control patterns. **Note**: This control already has a painter pattern implemented, so Phase 0 focuses on enhancing existing painters and adding helper infrastructure.

## Current State Analysis

### Strengths
- ✅ Inherits from `BaseControl` (theme support, tooltips, accessibility foundation)
- ✅ **Already has painter pattern** (`IStatCardPainter`, multiple painters)
- ✅ Uses `Dictionary<string, object>` for painter parameters
- ✅ Basic theme integration in `ApplyTheme()`
- ✅ Multiple painter implementations (SimpleKpi, HeartRate, EnergyActivity, Performance)
- ✅ Painter registry system

### Gaps Identified
- ❌ No centralized theme helpers (colors may be hardcoded in painters)
- ❌ No centralized font helpers (fonts may be created inline in painters)
- ❌ No icon helpers (direct image path usage in painters)
- ❌ Limited accessibility support (no ARIA attributes)
- ❌ No tooltip integration (BaseControl tooltips not utilized)
- ❌ Painters may not use theme helpers consistently
- ❌ No responsive sizing helpers

---

## Enhancement Architecture

### Phase 0: Enhance Existing Painters & Create Helpers (Prerequisite)
**Goal**: Create helper classes and enhance existing painters

#### 0.1 Create StatCardThemeHelpers
**File**: `Cards/Statuses/Helpers/StatCardThemeHelpers.cs`

**Purpose**: Centralized theme color management

**Methods**:
- `GetCardBackColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)`
- `GetHeaderColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)`
- `GetValueColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)`
- `GetDeltaColor(IBeepTheme theme, bool useThemeColors, bool isPositive, Color? customColor = null)`
- `GetInfoColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)`
- `GetTrendUpColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)`
- `GetTrendDownColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)`
- `ApplyThemeColors(BeepStatCard card, IBeepTheme theme, bool useThemeColors)`

#### 0.2 Create StatCardFontHelpers
**File**: `Cards/Statuses/Helpers/StatCardFontHelpers.cs`

**Purpose**: Centralized font management

**Methods**:
- `GetHeaderFont(BeepStatCard card, BeepControlStyle controlStyle)`
- `GetValueFont(BeepStatCard card, BeepControlStyle controlStyle)`
- `GetDeltaFont(BeepStatCard card, BeepControlStyle controlStyle)`
- `GetInfoFont(BeepStatCard card, BeepControlStyle controlStyle)`
- `GetLabelFont(BeepStatCard card, BeepControlStyle controlStyle)`
- `ApplyFontTheme(BeepStatCard card, BeepControlStyle controlStyle)`

#### 0.3 Create StatCardIconHelpers
**File**: `Cards/Statuses/Helpers/StatCardIconHelpers.cs`

**Purpose**: Centralized icon management

**Methods**:
- `ResolveIconPath(string iconPath, string defaultIcon)`
- `GetRecommendedTrendUpIcon()`
- `GetRecommendedTrendDownIcon()`
- `GetRecommendedCardIcon(string cardType = null)`
- `PaintIcon(Graphics g, Rectangle bounds, string iconPath, IBeepTheme theme, bool useThemeColors, Color? tintColor = null)`

#### 0.4 Create StatCardAccessibilityHelpers
**File**: `Cards/Statuses/Helpers/StatCardAccessibilityHelpers.cs`

**Purpose**: Accessibility support

**Methods**:
- `IsHighContrastMode()`
- `IsReducedMotionEnabled()`
- `GenerateAccessibleName(BeepStatCard card, string customName = null)`
- `GenerateAccessibleDescription(BeepStatCard card, string customDescription = null)`
- `ApplyAccessibilitySettings(BeepStatCard card, string accessibleName = null, string accessibleDescription = null)`
- `GetHighContrastColors()`
- `AdjustColorsForHighContrast(...)`
- `CalculateContrastRatio(Color color1, Color color2)`
- `AdjustForContrast(Color foreColor, Color backColor, double minRatio = 4.5)`

#### 0.5 Update Existing Painters
**Files**: All painter files in `Cards/Statuses/Painters/`

**Changes**:
- Update painters to use `StatCardThemeHelpers` for colors
- Update painters to use `StatCardFontHelpers` for fonts
- Update painters to use `StatCardIconHelpers` for icons
- Ensure consistent theme integration across all painters

---

### Phase 1: Theme Integration 🎨
**Goal**: Integrate centralized theme helpers

#### 1.1 Update ApplyTheme()
**File**: `BeepStatCard.Core.cs`

**Changes**:
- Replace hardcoded color assignments with `StatCardThemeHelpers.ApplyThemeColors()`
- Add `_isApplyingTheme` flag to prevent re-entrancy
- Ensure `UseThemeColors` property is respected

#### 1.2 Update Painters
**Files**: All painter files

**Changes**:
- Use `StatCardThemeHelpers` for all color retrieval
- Pass theme and `useThemeColors` to painters
- Ensure painters respect theme colors

---

### Phase 2: Font Integration 📝
**Goal**: Integrate centralized font helpers

#### 2.1 Update Font Assignments
**File**: `BeepStatCard.Core.cs`

**Changes**:
- Call `StatCardFontHelpers.ApplyFontTheme()` in `ApplyTheme()`

#### 2.2 Update Painters
**Files**: All painter files

**Changes**:
- Use `StatCardFontHelpers` for all font retrieval
- Pass `BeepControlStyle` to font helpers
- Ensure consistent font usage across painters

---

### Phase 3: Icon Integration 🖼️
**Goal**: Integrate centralized icon helpers

#### 3.1 Update Icon Path Resolution
**File**: `BeepStatCard.Core.cs`

**Changes**:
- Use `StatCardIconHelpers.ResolveIconPath()` for icon paths
- Use `StatCardIconHelpers.GetRecommendedTrendUpIcon()` as default
- Use `StatCardIconHelpers.GetRecommendedTrendDownIcon()` as default

#### 3.2 Update Painters
**Files**: All painter files

**Changes**:
- Use `StatCardIconHelpers.PaintIcon()` for icon rendering
- Support SVG tinting based on theme colors
- Use recommended icons as defaults

---

### Phase 4: Accessibility Enhancements ♿
**Goal**: Implement ARIA attributes, high contrast, reduced motion

#### 4.1 Add Accessibility Properties
**File**: `BeepStatCard.Core.cs`

**Changes**:
- Call `StatCardAccessibilityHelpers.ApplyAccessibilitySettings()` in constructor
- Update accessibility attributes when properties change
- Set `AccessibleRole = AccessibleRole.StaticText`
- Set `AccessibleValue` to value text

#### 4.2 High Contrast Support
**Changes**:
- Call `StatCardAccessibilityHelpers.ApplyHighContrastAdjustments()` in `ApplyTheme()`
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
**File**: `BeepStatCard.Core.cs`

**Changes**:
- Add `AutoGenerateTooltip` property (default: `true`)
- Add `UpdateStatCardTooltip()` method
- Add `GenerateStatCardTooltip()` method

#### 5.2 Tooltip Content
**Methods**:
- Generate tooltip text based on header, value, delta, and info
- Include trend information in tooltip
- Set appropriate `ToolTipType` based on delta (positive = Success, negative = Warning)

#### 5.3 Convenience Methods
**Methods**:
- `SetStatCardTooltip(string text, string title = null, ToolTipType type = ToolTipType.Info)`
- `ShowStatCardNotification(string message, ToolTipType type = ToolTipType.Info)`

---

### Phase 6: UX/UI Enhancements ✨
**Goal**: Add modern UX/UI features

#### 6.1 Painter Enhancements
**Changes**:
- Add animation support to painters (value changes, sparklines)
- Add hover effects
- Add click interactions

#### 6.2 Interaction Enhancements
**Changes**:
- Add click events for card
- Add keyboard navigation support (Tab, Enter, Space)
- Add context menu support

#### 6.3 Visual Enhancements
**Changes**:
- Add optional shadow effects
- Add optional border highlights
- Add optional gradient backgrounds
- Support rounded corners configuration

---

## Implementation Steps

1. **Phase 0**: Create helper classes and update existing painters
2. **Phase 1**: Integrate theme helpers into `ApplyTheme()` and painters
3. **Phase 2**: Integrate font helpers into painters
4. **Phase 3**: Integrate icon helpers into painters
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
- **Painter Consistency**: All painters use same helper infrastructure

---

## Files to Create/Modify

### New Files
- `Cards/Statuses/Helpers/StatCardThemeHelpers.cs`
- `Cards/Statuses/Helpers/StatCardFontHelpers.cs`
- `Cards/Statuses/Helpers/StatCardIconHelpers.cs`
- `Cards/Statuses/Helpers/StatCardAccessibilityHelpers.cs`

### Modified Files
- `Cards/Statuses/BeepStatCard.Core.cs`
- `Cards/Statuses/Painters/BaseStatCardPainter.cs`
- `Cards/Statuses/Painters/SimpleKpiPainter.cs`
- `Cards/Statuses/Painters/HeartRatePainter.cs`
- `Cards/Statuses/Painters/EnergyActivityPainter.cs`
- `Cards/Statuses/Painters/PerformancePainter.cs`

---

## Testing Checklist

- [ ] Theme colors apply correctly across all painters
- [ ] Fonts scale appropriately across all painters
- [ ] Icons render with correct colors across all painters
- [ ] High contrast mode works
- [ ] Reduced motion is respected
- [ ] Tooltips display correctly
- [ ] Keyboard navigation works
- [ ] All painters respect theme consistently
- [ ] Sparklines and charts render correctly

