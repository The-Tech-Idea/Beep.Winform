# ToolTips System Enhancement Plan

## Executive Summary

This document outlines a comprehensive plan to enhance the Beep ToolTips system with modern UX/UI best practices, improved theme integration via `ApplyTheme()`, and enhanced accessibility features.

## Current State Analysis

### ✅ Strengths
- **Architecture**: Well-structured with Manager, Config, Painter pattern
- **Styling Integration**: Uses BeepStyling system for backgrounds/borders
- **Animation Support**: Fade, Scale, Slide, Bounce animations
- **Placement System**: Comprehensive placement options (Auto, Top, Bottom, Left, Right with variants)
- **Type System**: Rich semantic types (Default, Success, Warning, Error, Info, etc.)
- **Painter Pattern**: Extensible painter interface

### ⚠️ Areas for Improvement
1. **Theme Integration**: Theme colors not consistently applied via `ApplyTheme()`
2. **Color Management**: Colors scattered across multiple helpers, not centralized
3. **Accessibility**: Limited support for screen readers, high contrast, keyboard navigation
4. **UX Patterns**: Missing modern UX patterns (smart positioning, collision detection, responsive sizing)
5. **Performance**: No caching for frequently used tooltips
6. **Rich Content**: Limited support for rich content (markdown, formatted text, interactive elements)

---

## Phase 1: Theme Integration Enhancement

### 1.1 Add ApplyTheme() Method to CustomToolTip
**Goal**: Ensure tooltips properly respond to theme changes via `ApplyTheme()`

**Changes**:
- Add `ApplyTheme()` method to `CustomToolTip` class
- Store `_currentTheme` from BaseControl pattern
- Update all color retrieval to use `_currentTheme` when available
- Ensure theme changes trigger repaint

**Files**:
- `CustomToolTip.cs` - Add ApplyTheme method
- `ToolTipManager.cs` - Subscribe to theme changes
- `BeepStyledToolTipPainter.cs` - Use _currentTheme consistently

### 1.2 Centralize Theme Color Management
**Goal**: Create unified theme color helper that uses ApplyTheme colors

**New Helper**: `ToolTipThemeHelpers.cs`
- `GetToolTipBackColor(theme, type, useThemeColors)` - Uses theme.ToolTipBackColor
- `GetToolTipForeColor(theme, type, useThemeColors)` - Uses theme.ToolTipForeColor
- `GetToolTipBorderColor(theme, type, useThemeColors)` - Uses theme.BorderColor
- `GetSemanticColor(type, theme, useThemeColors)` - Maps ToolTipType to theme colors
- `ApplyThemeColors(config, theme)` - Applies theme colors to config

**Integration Points**:
- Use `IBeepTheme` properties: `ToolTipBackColor`, `ToolTipForeColor`, `ToolTipBorderColor`
- Fallback to semantic colors: `SuccessColor`, `WarningColor`, `ErrorColor`, `InfoColor`
- Support `UseThemeColors` flag from BaseControl

### 1.3 Update Painters to Use Theme Helpers
**Goal**: All painters use centralized theme helpers

**Changes**:
- `BeepStyledToolTipPainter` - Use `ToolTipThemeHelpers` instead of direct theme access
- `ToolTipStyleAdapter` - Integrate with theme helpers
- Remove duplicate color logic

---

## Phase 2: UX/UI Enhancements

### 2.1 Smart Positioning & Collision Detection
**Goal**: Tooltips never go off-screen, intelligently reposition

**New Helper**: `ToolTipPositioningHelpers.cs`
- `CalculateOptimalPlacement(targetRect, tooltipSize, preferredPlacement, screenBounds)` - Smart placement
- `DetectCollisions(bounds, screenBounds)` - Check if tooltip fits
- `FindBestPlacement(targetRect, tooltipSize, screenBounds)` - Auto-find best placement
- `AdjustForScreenEdges(position, size, screenBounds)` - Constrain to screen

**Features**:
- Automatic fallback placement (Top → Bottom → Left → Right)
- Minimum distance from screen edges (8px padding)
- Smart arrow repositioning when placement changes
- Multi-monitor support

### 2.2 Responsive Sizing
**Goal**: Tooltips adapt to content and screen size

**Enhancements**:
- `CalculateOptimalSize(content, maxWidth, maxHeight, font)` - Smart sizing
- Word wrapping for long text
- Max width based on screen size (80% of screen width max)
- Min width for readability (120px minimum)
- Dynamic height based on content

### 2.3 Rich Content Support
**Goal**: Support formatted text, icons, images, and interactive elements

**New Features**:
- **Markdown Support**: Basic markdown rendering (bold, italic, links, lists)
- **Icon Integration**: Use `StyledImagePainter` for icons
- **Image Support**: Embedded images with proper sizing
- **Interactive Elements**: Buttons, links, checkboxes (for Interactive type)
- **Multi-line Text**: Proper line spacing and paragraph breaks

**New Helper**: `ToolTipContentRenderer.cs`
- `RenderText(g, bounds, text, font, color, format)` - Text rendering
- `RenderMarkdown(g, bounds, markdown, theme)` - Markdown rendering
- `RenderIcon(g, bounds, iconPath, theme)` - Icon rendering
- `RenderImage(g, bounds, imagePath, theme)` - Image rendering

### 2.4 Enhanced Animations
**Goal**: Smoother, more polished animations

**Improvements**:
- **Easing Functions**: Add more easing options (EaseInOut, EaseOutBack, EaseInOutCubic)
- **Staggered Animations**: Content fades in after background
- **Micro-interactions**: Subtle hover effects, scale on show
- **Performance**: Use GPU acceleration where possible

**New Helper**: `ToolTipAnimationHelpers.cs`
- `EaseInOutQuad(t)`, `EaseOutBack(t)`, `EaseInOutCubic(t)` - More easing functions
- `CalculateOpacity(progress, animationType)` - Opacity calculation
- `CalculateScale(progress, animationType)` - Scale calculation
- `CalculateOffset(progress, placement, animationType)` - Position offset

---

## Phase 3: Accessibility Enhancements

### 3.1 Screen Reader Support
**Goal**: Tooltips accessible to screen readers

**Features**:
- **ARIA Attributes**: Proper ARIA labels and descriptions
- **Announcements**: Screen reader announcements for tooltip content
- **Focus Management**: Proper focus handling when tooltip appears
- **Keyboard Navigation**: Navigate tooltips with keyboard

**Implementation**:
- Add `AccessibleName` and `AccessibleDescription` properties
- Use `System.Windows.Forms.AccessibleObject` for proper accessibility
- Integrate with Windows Narrator and NVDA

### 3.2 High Contrast Mode Support
**Goal**: Tooltips work well in high contrast mode

**Features**:
- Detect Windows high contrast mode
- Use system colors when in high contrast
- Ensure sufficient contrast ratios (WCAG AA: 4.5:1, AAA: 7:1)
- Thicker borders in high contrast mode

**Helper**: `ToolTipAccessibilityHelpers.cs`
- `IsHighContrastMode()` - Detect high contrast
- `GetHighContrastColors()` - Get system colors
- `EnsureContrastRatio(foreColor, backColor, minRatio)` - Verify contrast

### 3.3 Reduced Motion Support
**Goal**: Respect user's motion preferences

**Features**:
- Detect Windows "Reduce motion" setting
- Disable animations when motion is reduced
- Instant show/hide for accessibility
- Respect `prefers-reduced-motion` equivalent

### 3.4 Keyboard Shortcuts Display
**Goal**: Show keyboard shortcuts in tooltips (for Shortcut type)

**Features**:
- Format keyboard shortcuts (Ctrl+K, Alt+F4)
- Visual keyboard key representation
- Platform-specific shortcuts (Windows vs Mac)
- Customizable shortcut display style

---

## Phase 4: Performance Optimizations

### 4.1 Caching System
**Goal**: Cache frequently used tooltips for better performance

**Features**:
- **Size Cache**: Cache calculated sizes for same content
- **Render Cache**: Cache rendered tooltip images (for static content)
- **Theme Cache**: Cache theme color calculations
- **Font Cache**: Cache font measurements

**Implementation**:
- `ToolTipCache.cs` - Centralized caching
- LRU cache for size calculations
- Memory-aware cache limits

### 4.2 Lazy Loading
**Goal**: Load tooltip content only when needed

**Features**:
- Defer content calculation until show
- Lazy load images/icons
- Async content loading for rich content

### 4.3 Rendering Optimizations
**Goal**: Faster rendering with less flicker

**Features**:
- Double buffering (already present, ensure it's optimal)
- Partial invalidation (only redraw changed areas)
- GPU-accelerated rendering where possible
- Reduce overdraw

---

## Phase 5: Advanced Features

### 5.1 Tooltip Groups & Stacks
**Goal**: Manage multiple tooltips intelligently

**Features**:
- **Tooltip Stacking**: Show multiple tooltips in sequence
- **Group Management**: Group related tooltips
- **Auto-dismiss**: Dismiss previous tooltip when new one shows
- **Priority System**: Important tooltips take precedence

### 5.2 Context-Aware Tooltips
**Goal**: Tooltips adapt to context

**Features**:
- **Context Detection**: Detect control type, state, user action
- **Dynamic Content**: Content changes based on context
- **Smart Timing**: Adjust delay based on context
- **Progressive Disclosure**: Show more info on hover hold

### 5.3 Tooltip Templates
**Goal**: Reusable tooltip templates

**Features**:
- **Template System**: Define reusable tooltip templates
- **Template Variables**: Parameterized templates
- **Style Inheritance**: Templates inherit from base styles
- **Template Library**: Pre-built templates for common scenarios

### 5.4 Analytics & Telemetry
**Goal**: Track tooltip usage for UX insights

**Features**:
- **Usage Tracking**: Track which tooltips are shown
- **Interaction Tracking**: Track user interactions with tooltips
- **Performance Metrics**: Track render times, animation performance
- **A/B Testing**: Test different tooltip styles

---

## Implementation Phases

### Phase 1: Foundation (Week 1-2)
1. ✅ Add `ApplyTheme()` to `CustomToolTip`
2. ✅ Create `ToolTipThemeHelpers.cs`
3. ✅ Update painters to use theme helpers
4. ✅ Test theme integration

### Phase 2: UX Enhancements (Week 3-4)
1. ✅ Smart positioning & collision detection
2. ✅ Responsive sizing
3. ✅ Enhanced animations
4. ✅ Rich content support (basic)

### Phase 3: Accessibility (Week 5)
1. ✅ Screen reader support
2. ✅ High contrast mode
3. ✅ Reduced motion support
4. ✅ Keyboard shortcuts display

### Phase 4: Performance (Week 6)
1. ✅ Caching system
2. ✅ Lazy loading
3. ✅ Rendering optimizations

### Phase 5: Advanced Features (Week 7-8)
1. ✅ Tooltip groups & stacks
2. ✅ Context-aware tooltips
3. ✅ Tooltip templates
4. ✅ Analytics (optional)

---

## File Structure

```
ToolTips/
├── CustomToolTip.cs (enhanced with ApplyTheme)
├── ToolTipManager.cs (enhanced with theme subscription)
├── ToolTipConfig.cs (enhanced with accessibility options)
├── ToolTipInstance.cs (existing)
├── ToolTipExtensions.cs (existing)
├── ToolTipEnums.cs (existing)
├── Helpers/
│   ├── ToolTipHelpers.cs (existing, enhanced)
│   ├── ToolTipStyleAdapter.cs (existing, enhanced)
│   ├── ToolTipThemeHelpers.cs (NEW - theme color management)
│   ├── ToolTipPositioningHelpers.cs (NEW - smart positioning)
│   ├── ToolTipContentRenderer.cs (NEW - rich content)
│   ├── ToolTipAnimationHelpers.cs (NEW - enhanced animations)
│   └── ToolTipAccessibilityHelpers.cs (NEW - accessibility)
├── Painters/
│   ├── IToolTipPainter.cs (existing)
│   ├── ToolTipPainterBase.cs (existing, enhanced)
│   └── BeepStyledToolTipPainter.cs (enhanced with theme helpers)
└── Models/
    └── ToolTipTemplate.cs (NEW - template system)
```

---

## Theme Integration Details

### ApplyTheme() Implementation

```csharp
public void ApplyTheme()
{
    // Get current theme from BeepThemesManager or BaseControl
    _currentTheme = BeepThemesManager.CurrentTheme ?? BaseControl.Theme;
    
    // Apply theme colors using ToolTipThemeHelpers
    if (_currentTheme != null && _config != null)
    {
        var colors = ToolTipThemeHelpers.GetThemeColors(
            _currentTheme, 
            _config.Type, 
            _config.UseBeepThemeColors);
        
        // Update config with theme colors (if not overridden)
        if (!_config.BackColor.HasValue)
            _config.BackColor = colors.backColor;
        if (!_config.ForeColor.HasValue)
            _config.ForeColor = colors.foreColor;
        if (!_config.BorderColor.HasValue)
            _config.BorderColor = colors.borderColor;
    }
    
    // Trigger repaint
    Invalidate();
}
```

### Theme Color Priority

1. **Custom Colors** (highest priority) - `config.BackColor`, `config.ForeColor`, `config.BorderColor`
2. **Theme Colors** - From `IBeepTheme` via `ToolTipThemeHelpers`
3. **Semantic Colors** - Based on `ToolTipType` (Success, Warning, Error, etc.)
4. **Default Colors** - Fallback colors

---

## UX/UI Best Practices

### 1. Timing
- **Show Delay**: 500ms (default) - prevents accidental triggers
- **Hide Delay**: 3000ms (default) - enough time to read
- **Context-Sensitive**: Reduce delay for interactive elements

### 2. Positioning
- **Smart Placement**: Auto-detect best position
- **Collision Avoidance**: Never go off-screen
- **Arrow Positioning**: Arrow points to target accurately
- **Multi-Monitor**: Work across multiple monitors

### 3. Sizing
- **Content-Based**: Size based on content, not fixed
- **Max Width**: 80% of screen width maximum
- **Min Width**: 120px minimum for readability
- **Word Wrapping**: Wrap long text properly

### 4. Visual Design
- **Consistent Styling**: Use BeepControlStyle for consistency
- **Proper Contrast**: WCAG AA compliance (4.5:1 ratio)
- **Subtle Shadows**: Soft shadows for depth
- **Rounded Corners**: Modern rounded corners (8-12px)

### 5. Animations
- **Smooth Transitions**: 150-200ms fade in/out
- **Respect Preferences**: Disable for reduced motion
- **Performance**: 60 FPS animations
- **Easing**: Natural easing functions

### 6. Accessibility
- **Screen Readers**: Proper ARIA labels
- **Keyboard**: Full keyboard navigation
- **High Contrast**: System color support
- **Focus Indicators**: Clear focus indicators

---

## Testing Checklist

### Theme Integration
- [ ] Tooltips use theme colors from ApplyTheme()
- [ ] Theme changes trigger tooltip repaint
- [ ] Custom colors override theme colors
- [ ] Semantic colors work correctly
- [ ] All ToolTipType variants use correct theme colors

### UX/UI
- [ ] Smart positioning works correctly
- [ ] Collision detection prevents off-screen tooltips
- [ ] Responsive sizing adapts to content
- [ ] Animations are smooth (60 FPS)
- [ ] Rich content renders correctly

### Accessibility
- [ ] Screen readers announce tooltip content
- [ ] High contrast mode works
- [ ] Reduced motion disables animations
- [ ] Keyboard navigation works
- [ ] Focus management is correct

### Performance
- [ ] Caching improves performance
- [ ] No memory leaks
- [ ] Smooth animations
- [ ] Fast rendering

---

## Success Metrics

1. **Theme Integration**: 100% of tooltips use ApplyTheme() colors
2. **Accessibility**: WCAG AA compliance
3. **Performance**: <16ms render time (60 FPS)
4. **UX**: <500ms show delay, smart positioning works 100% of time
5. **Code Quality**: All helpers follow single responsibility principle

---

## Next Steps

1. Review and approve this plan
2. Start Phase 1: Theme Integration
3. Implement incrementally with testing
4. Document as we go
5. Gather feedback and iterate

---

## References

- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [Material Design Tooltips](https://material.io/components/tooltips)
- [Ant Design Tooltips](https://ant.design/components/tooltip/)
- [Fluent UI Tooltips](https://developer.microsoft.com/en-us/fluentui#/controls/web/tooltip)
- [Popper.js Positioning](https://popper.js.org/)

