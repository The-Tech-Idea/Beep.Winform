# BorderPainters ControlState Integration Plan

## Problem Analysis

**Issue:** The `ControlState` parameter is passed to all BorderPainter `Paint()` methods but most painters only check for `Hovered` state in a limited way. The state parameter should drive visual changes for all states: Normal, Hovered, Pressed, Selected, Disabled, Focused.

**Current Behavior:**
- Most painters only change border color on hover
- `isFocused` parameter is separate from `state` (should be unified or complementary)
- Pressed, Selected, and Disabled states are largely ignored
- No consistent state-aware border width changes
- No state-aware border opacity/alpha changes

## Revision Strategy

All BorderPainters will be updated to properly respond to ControlState with design-system-appropriate visual feedback:

### State-Aware Border Changes:

1. **Normal State** - Default border from StyleBorders
2. **Hovered State** - Lighter/brighter border, possibly thicker
3. **Pressed State** - Darker/more prominent border, possibly thicker
4. **Selected State** - Accent color border, possibly thicker
5. **Disabled State** - Grayed out border, reduced opacity
6. **Focused State** - Already handled via `isFocused` parameter (rings, accent bars)

### Design-System Specific Behaviors:

- **Material Design** - Elevation changes via border thickness
- **Fluent Design** - Accent colors on interaction
- **Apple (iOS/macOS)** - Subtle color shifts
- **Minimal/Flat** - Very subtle changes
- **Dark/Glow** - Glow intensity changes by state
- **Neumorphism** - Shadow depth changes by state

## BorderPainter Classes State Integration Status

### Phase 1: Material Design Systems (4/4 COMPLETED ✅)

#### 1. Material3BorderPainter.cs
**Current State Handling:** Only isFocused (2px vs 1px)
**Missing States:** Hovered (has logic but not from state param), Pressed, Selected, Disabled
**Plan:**
- **Hovered:** Use primary color with 30% blend, normal border width
- **Pressed:** Use primary color with 60% blend, 1.5x border width (elevation feel)
- **Selected:** Use primary color full, 2px minimum border width
- **Disabled:** Use border color with 60 alpha (low contrast)
**Status:** ✅ **COMPLETED** - Material3 bold, clear state changes implemented

#### 2. MaterialYouBorderPainter.cs
**Current State Handling:** Only checks IsFilled, ignores state
**Missing States:** All states
**Plan:**
- **Hovered:** Primary color with 70% opacity border (dynamic hover tint)
- **Pressed:** Primary color with 120% opacity, 1.3x width (vibrant press state)
- **Selected:** Full adaptive primary color (saturated selection)
- **Disabled:** Border with 50 alpha (harmonious disabled state)
**Status:** ✅ **COMPLETED** - Material You adaptive, dynamic color behaviors implemented

#### 3. MaterialBorderPainter.cs
**Current State Handling:** Has hover logic, focus thickening
**Missing States:** Pressed, Selected, Disabled
**Plan:**
- **Hovered:** Keep existing 70% alpha primary hover
- **Pressed:** Primary color, 2.5x border width (bold Material press)
- **Selected:** Primary color, 1.5x border width or 2px minimum
- **Disabled:** Border with 40% opacity, 0.8x width (thinner disabled)
**Status:** ✅ **COMPLETED** - Original Material bold focus thickening with dramatic press state

#### 4. NeumorphismBorderPainter.cs
**Current State Handling:** Checks IsFilled only
**Missing States:** All states (Neumorphism is borderless but could affect background)
**Plan:**
- Keep borderless for Neumorphism style
- State changes handled by ShadowPainter instead
**Status:** ✅ **COMPLETED** - No changes needed (borderless design, state handled by shadows)

---

### Phase 2: Fluent Design Systems (3/3 COMPLETED ✅)

#### 5. Fluent2BorderPainter.cs
**Current State Handling:** Has hover logic for accent bar, not for border itself
**Missing States:** Border changes for Pressed, Selected, Disabled
**Plan:**
- **Hovered:** Keep current (accent bar + blend border color 30%)
- **Pressed:** Darker primary color border, thicker accent bar (1.5x)
- **Selected:** Primary color border with full accent bar
- **Disabled:** Border with 80 alpha, no accent bar
**Status:** ✅ **COMPLETED** - Fluent2 accent bar intensity changes implemented

#### 6. FluentBorderPainter.cs
**Current State Handling:** Has hover logic for color and accent bar
**Missing States:** Pressed, Selected, Disabled
**Plan:**
- **Hovered:** Keep existing 60% alpha primary + accent bar
- **Pressed:** Primary color border, 1.3x thicker accent bar
- **Selected:** Full primary border with accent bar
- **Disabled:** Muted border (70 alpha), no accent bar
**Status:** ✅ **COMPLETED** - Fluent layered feedback with rings + accent bars

#### 7. Windows11MicaBorderPainter.cs
**Current State Handling:** None (static border + focus accent)
**Missing States:** All states
**Plan:**
- **Hovered:** Subtle border color shift (20% blend)
- **Pressed:** Slightly darker border (40% blend)
- **Selected:** Accent color border + accent bar
- **Disabled:** Very subtle border (50 alpha for Mica subtlety)
**Status:** ✅ **COMPLETED** - Windows 11 Mica subtle state changes

---

### Phase 3: Apple Design Systems (3 painters)

#### 8. iOS15BorderPainter.cs
**Current State Handling:** Only focus ring, no state-based border changes
**Missing States:** All states
**Plan:**
- **Hovered:** Very subtle tint (15% blend for iOS refinement)
- **Pressed:** Slightly stronger tint (30% blend, still subtle)
- **Selected:** Accent color with 180 alpha
- **Disabled:** Lighter, more transparent (60 alpha)
**Status:** ✅ **COMPLETED** - iOS refined, subtle state tints

#### 9. MacOSBigSurBorderPainter.cs
**Current State Handling:** None
**Missing States:** All states
**Plan:**
- **Hovered:** Very subtle vibrancy tint (10% blend)
- **Pressed:** Slightly more prominent vibrancy (25% blend)
- **Selected:** Accent color with 160 alpha (vibrancy transparency)
- **Disabled:** Highly transparent (50 alpha for vibrancy)
**Status:** ✅ **COMPLETED** - macOS Big Sur vibrancy with subtle transitions

#### 10. AppleBorderPainter.cs
**Current State Handling:** Has hover logic
**Missing States:** Pressed, Selected, Disabled
**Plan:**
- **Hovered:** Keep existing 30 alpha primary (very subtle)
- **Pressed:** 60 alpha primary (still minimal)
- **Selected:** 120 alpha primary (subtle primary with transparency)
- **Disabled:** 40 alpha base border (refined disabled)
**Status:** ✅ **COMPLETED** - Apple minimal, refined state changes

---

### Phase 4: Web Framework Systems (5/5 COMPLETED ✅)

#### 11. TailwindCardBorderPainter.cs
**Current State Handling:** Only focus ring
**Missing States:** All border states
**Plan:**
- **Hovered:** Subtle darkening + ring preview (60 alpha)
- **Pressed:** Darker border + prominent ring (140 alpha)
- **Selected:** Primary border + ring (100 alpha)
- **Disabled:** Lighter disabled (70 alpha)
**Status:** ✅ **COMPLETED** - Tailwind utility-first with prominent rings

#### 12. BootstrapBorderPainter.cs
**Current State Handling:** Only focus accent bar
**Missing States:** All border states
**Plan:**
- **Hovered:** Primary tint on border (20% blend)
- **Pressed:** Bold primary border + 1.5x width + accent bar
- **Selected:** Primary border + accent bar
- **Disabled:** Gray disabled (60 alpha), no accent
**Status:** ✅ **COMPLETED** - Bootstrap bold state changes with accent bars

#### 13. AntDesignBorderPainter.cs
**Current State Handling:** Has hover logic (60% primary overlay)
**Missing States:** Pressed, Selected, Disabled
**Plan:**
- **Hovered:** Keep existing 60 alpha primary (spec compliant)
- **Pressed:** Darker primary border, 2px (Ant Design spec)
- **Selected:** Primary border, 2px
- **Disabled:** Very light disabled (50 alpha)
**Status:** ✅ **COMPLETED** - Ant Design spec-compliant 2px focus border

#### 14. ChakraUIBorderPainter.cs
**Current State Handling:** Only focus ring
**Missing States:** All border states
**Plan:**
- **Hovered:** Teal tint on border (25% blend) + ring preview
- **Pressed:** Darker teal border + ring
- **Selected:** Full teal border + ring
- **Disabled:** Light disabled (55 alpha)
**Status:** ✅ **COMPLETED** - Chakra UI distinctive teal states

#### 15. WebFrameworkBorderPainter.cs
**Current State Handling:** Has hover logic
**Missing States:** Pressed, Selected, Disabled
**Plan:**
- **Hovered:** Keep existing 50 alpha primary + ring preview
- **Pressed:** Primary border with 1.2x width + ring
- **Selected:** Primary border + ring
- **Disabled:** Standard disabled (50 alpha)
**Status:** ✅ **COMPLETED** - Web Framework standardized behaviors

---

### Phase 5: Minimal/Clean Systems (5 painters)

#### 16. MinimalBorderPainter.cs
**Current State Handling:** Has hover logic for better contrast
**Missing States:** Pressed, Selected, Disabled
**Plan:**
- **Pressed:** Darker border (primary with 80%)
- **Selected:** Primary color border
### Phase 5: Minimal/Clean Systems (4/4 COMPLETED ✅)

#### 16. MinimalBorderPainter.cs
**Current State Handling:** Has hover logic for better contrast
**Missing States:** Pressed, Selected, Disabled
**Plan:**
- **Hovered:** Keep existing 60 alpha primary (better contrast)
- **Pressed:** 80 alpha primary (darker for clear press)
- **Selected:** Full primary color + ring (clear selection)
- **Disabled:** 40 alpha base border (very subtle minimalism)
**Status:** ✅ **COMPLETED** - Minimal subtle clarity with focus rings

#### 17. NotionMinimalBorderPainter.cs
**Current State Handling:** None
**Missing States:** All states
**Plan:**
- **Hovered:** Very subtle darkening (5% blend for zen-like hover)
- **Pressed:** Subtle darkening (15% blend, still very subtle)
- **Selected:** Notion gray border (30% blend) + ring
- **Disabled:** Extremely light disabled (30 alpha for zen)
**Status:** ✅ **COMPLETED** - Notion zen-like minimalism

#### 18. VercelCleanBorderPainter.cs
**Current State Handling:** None
**Missing States:** All states
**Plan:**
- **Hovered:** Clean darkening (8% blend for modern hover)
- **Pressed:** Cleaner darkening (20% blend for clear press)
- **Selected:** Dark border (50% blend) + ring for modern selection
- **Disabled:** Light disabled (35 alpha for clean disabled)
**Status:** ✅ **COMPLETED** - Vercel clean, modern minimalism

#### 19. PillRailBorderPainter.cs
**Current State Handling:** Has hover logic
**Missing States:** Pressed, Selected, Disabled
**Plan:**
- **Hovered:** Keep existing 40 alpha primary (very subtle tint)
- **Pressed:** 70 alpha primary (soft press)
- **Selected:** 140 alpha primary + ring (soft transparency)
- **Disabled:** 45 alpha base border (gentle disabled)
**Status:** ✅ **COMPLETED** - Pill Rail soft, rounded minimalism

**Note:** DiscordStyleBorderPainter does not exist in codebase (only 4 minimal painters total)

---

### Phase 6: Special Effects Systems (5 painters)

#### 20. DarkGlowBorderPainter.cs
**Current State Handling:** None
### Phase 6: Special Effects Systems (5/5 COMPLETED ✅)

#### 20. DarkGlowBorderPainter.cs
**Current State Handling:** Has hover/focus logic
**Missing States:** Pressed, Selected, Disabled (glow should vary by state)
**Plan:**
- **Normal:** Standard glow (60% intensity)
- **Hovered:** Brighter glow (80% intensity)
- **Pressed:** Intense glow (100% intensity + 1.3x thicker)
- **Selected:** Full intensity glow (120% extra bright)
- **Disabled:** Very dim glow (20% intensity)
**Status:** ✅ **COMPLETED** - DarkGlow dramatic intensity changes

#### 21. GlassAcrylicBorderPainter.cs
**Current State Handling:** Only focus glow
**Missing States:** All border states with glow variations
**Plan:**
- **Hovered:** Brighter glow (70% intensity)
- **Pressed:** Prominent glow (90% intensity)
- **Selected:** Full glow with border (100% intensity)
- **Disabled:** Minimal glow (15% intensity)
**Status:** ✅ **COMPLETED** - GlassAcrylic translucent glow opacity changes

#### 22. GradientModernBorderPainter.cs
**Current State Handling:** Has hover logic
**Missing States:** Pressed, Selected, Disabled
**Plan:**
- **Hovered:** Keep existing 50 alpha tint + ring preview
- **Pressed:** 80 alpha tint + ring
- **Selected:** Full primary + ring
- **Disabled:** Subdued disabled (40 alpha)
**Status:** ✅ **COMPLETED** - GradientModern gradient tints with rings

#### 23. StripeDashboardBorderPainter.cs
**Current State Handling:** Only focus accent bar + ring
**Missing States:** All border states
**Plan:**
- **Hovered:** Subtle primary tint (20% blend) + ring preview
- **Pressed:** Primary border + prominent ring + accent bar
- **Selected:** Primary border + ring + accent bar
- **Disabled:** Light disabled (50 alpha), no accent
**Status:** ✅ **COMPLETED** - Stripe prominent state changes with accent bars

#### 24. FigmaCardBorderPainter.cs
**Current State Handling:** Has hover logic
**Missing States:** Pressed, Selected, Disabled
**Plan:**
- **Hovered:** Keep existing 40 alpha blue tint + ring preview
- **Pressed:** 70 alpha tint + ring
- **Selected:** 140 alpha blue + prominent ring
- **Disabled:** Light disabled (45 alpha)
**Status:** ✅ **COMPLETED** - Figma subtle blue tints with prominent rings

---

## 🎉 ALL PHASES COMPLETE! 🎉

### Summary: 24/24 BorderPainters Updated (100%)

**Phase 1 - Material Design Systems (4/4):** ✅ Material3, MaterialYou, Material, Neumorphism
**Phase 2 - Fluent Design Systems (3/3):** ✅ Fluent2, Fluent, Windows11Mica
**Phase 3 - Apple Design Systems (3/3):** ✅ iOS15, MacOSBigSur, Apple
**Phase 4 - Web Framework Systems (5/5):** ✅ Tailwind, Bootstrap, AntDesign, ChakraUI, WebFramework
**Phase 5 - Minimal/Clean Systems (4/4):** ✅ Minimal, NotionMinimal, VercelClean, PillRail
**Phase 6 - Special Effects Systems (5/5):** ✅ DarkGlow, GlassAcrylic, GradientModern, StripeDashboard, FigmaCard

### Key Achievements:
- All 24 BorderPainters now respond to 6 ControlState values (Normal, Hovered, Pressed, Selected, Disabled, Focused)
- Each painter has unique design-system-appropriate state behaviors (not generic copy-paste)
- Switch statements replace if-checks for cleaner code
- BorderPainterHelpers.BlendColors() added for color mixing
- StyleBorders configuration fully integrated

---

## Implementation Pattern

### Standard State Switch Pattern:

```csharp
public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
    BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
    ControlState state = ControlState.Normal)
{
    // Skip if filled style (no border)
    if (StyleBorders.IsFilled(style))
        return;

    // Get base colors
    Color borderColor = GetBaseColor(theme, useThemeColors, "Border");
    Color primaryColor = GetBaseColor(theme, useThemeColors, "Primary");
    float borderWidth = StyleBorders.GetBorderWidth(style);
    
    // State-specific border modifications
    switch (state)
    {
        case ControlState.Hovered:
            borderColor = BlendWithPrimary(borderColor, primaryColor, 0.4f);
            break;
            
        case ControlState.Pressed:
            borderColor = BlendWithPrimary(borderColor, primaryColor, 0.7f);
            borderWidth *= 1.3f;
            break;
            
        case ControlState.Selected:
            borderColor = primaryColor;
            borderWidth = Math.Max(borderWidth, 2.0f);
            break;
            
        case ControlState.Disabled:
            borderColor = BorderPainterHelpers.WithAlpha(borderColor, 30);
            break;
            
        case ControlState.Normal:
        default:
            // Use base border color
            break;
    }
    
    // Paint border
    BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);
    
    // Paint focus effects (rings, accent bars) if focused
    if (isFocused)
    {
        PaintFocusEffects(g, path, style, theme, useThemeColors);
    }
}
```

### Helper Method to Add:

```csharp
private static Color BlendWithPrimary(Color baseColor, Color primaryColor, float primaryAmount)
{
    int r = (int)(baseColor.R * (1 - primaryAmount) + primaryColor.R * primaryAmount);
    int g = (int)(baseColor.G * (1 - primaryAmount) + primaryColor.G * primaryAmount);
    int b = (int)(baseColor.B * (1 - primaryAmount) + primaryColor.B * primaryAmount);
    return Color.FromArgb(baseColor.A, r, g, b);
}
```

## Expected Visual Changes

### State Progression Examples:

**Material3:**
- Normal: Gray border (1px)
- Hover: Purple tint border (1px)
- Press: Darker purple border (1.5px)
- Selected: Full purple border (2px)
- Disabled: Light gray 40% opacity (1px)
- Focused: Purple border (2px) + no additional effects needed

**Fluent2:**
- Normal: Gray border + no accent
- Hover: Blue tint border + subtle accent bar
- Press: Blue border + prominent accent bar
- Selected: Blue border + full accent bar
- Disabled: Gray 30% opacity + no accent
- Focused: Blue border + accent bar + focus ring

**DarkGlow:**
- Normal: Purple glow 60%
- Hover: Purple glow 80%
- Press: Purple glow 100% + thicker
- Selected: Purple glow 100% + accent overlay
- Disabled: Purple glow 20%
- Focused: Purple glow 100% + focus ring

## Testing Strategy

### Per State Testing:
- Verify each state renders distinct border
- Check state transitions are smooth
- Ensure disabled state is clearly muted
- Validate selected state is prominent
- Test pressed state provides feedback

### Cross-Painter Testing:
- Test all 25 painters across all 6 states
- Verify consistent visual hierarchy
- Check design-system appropriateness
- Ensure no performance regression

## Success Criteria

✅ All painters respond to all ControlState values
✅ Visual feedback clear and design-system appropriate
✅ Disabled state always muted/grayed
✅ Selected state always prominent
✅ Pressed state provides clear interaction feedback
✅ No hardcoded state logic (use switch statements)
✅ Helper methods for color blending added
✅ Build succeeds with 0 errors

## Notes

- `isFocused` parameter remains separate for focus-specific effects (rings, accent bars)
- State changes should be subtle for Minimal/Clean designs
- State changes should be prominent for Material/Fluent designs
- Disabled state should always reduce opacity significantly
- Pressed state should give clear "click" feedback
- Selected state should indicate "this is active" clearly
