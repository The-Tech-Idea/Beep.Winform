# BackgroundPainters ControlState Audit & Enhancement Plan

## Executive Summary
All 21 BackgroundPainters have `ControlState` parameter and ARE using it, but 18/21 use **GENERIC** helper methods while only 3/21 have **CUSTOM** state behaviors. This is unlike BorderPainters where ALL 24 have unique state behaviors per design system.

## Current State Analysis

### Generic State Handling (Used by 18/21 Painters)

**BackgroundPainterHelpers.ApplyState(color, state):**
- Hovered: Lighten 5%
- Pressed: Darken 10%
- Selected: Lighten 8%
- Disabled: Set alpha to 100
- Focused: Lighten 3%
- Normal: No change

**BackgroundPainterHelpers.GetStateOverlay(state):**
- Hovered: White @ 20 alpha
- Pressed: Black @ 30 alpha
- Selected: White @ 25 alpha
- Focused: White @ 15 alpha
- Disabled: Gray @ 80 alpha
- Normal: Transparent

### Painters Using GENERIC Helpers (18)
1. Material3BackgroundPainter
2. MaterialYouBackgroundPainter
3. Fluent2BackgroundPainter
4. Windows11MicaBackgroundPainter
5. MacOSBigSurBackgroundPainter
6. iOS15BackgroundPainter
7. AntDesignBackgroundPainter
8. BootstrapBackgroundPainter
9. ChakraUIBackgroundPainter
10. TailwindCardBackgroundPainter
11. StripeDashboardBackgroundPainter
12. FigmaCardBackgroundPainter
13. MinimalBackgroundPainter
14. NotionMinimalBackgroundPainter
15. VercelCleanBackgroundPainter
16. PillRailBackgroundPainter
17. DiscordStyleBackgroundPainter
18. GradientModernBackgroundPainter

### Painters with CUSTOM State Behaviors (3)

**1. DarkGlowBackgroundPainter**
```csharp
float glowMultiplier = state switch
{
    ControlState.Hovered => 1.3f,
    ControlState.Pressed => 0.7f,
    ControlState.Selected => 1.2f,
    ControlState.Disabled => 0.4f,
    ControlState.Focused => 1.1f,
    _ => 1.0f
};
```

**2. NeumorphismBackgroundPainter**
```csharp
Color highlightColor = state == ControlState.Pressed 
    ? Darken(baseColor, 0.1f) 
    : Lighten(baseColor, 0.1f);
```

**3. GlassAcrylicBackgroundPainter**
```csharp
int baseAlpha = state == ControlState.Pressed 
    ? 140 
    : baseColor.A;
```

## Question: Should BackgroundPainters Have Unique State Behaviors?

### Arguments FOR Unique Behaviors (Like BorderPainters):
1. **Design System Authenticity**: Each design system has unique visual language
2. **Consistency with BorderPainters**: We gave each border unique state behaviors
3. **Better User Experience**: Tailored state feedback per design system
4. **Examples**:
   - Material3: Elevation-based lighting (15% hover vs generic 5%)
   - iOS15: Translucent blur intensity changes (25% vs generic 20%)
   - Windows11Mica: Mica noise pattern intensity changes
   - NotionMinimal: Extremely subtle tints (3% vs generic 5%)

### Arguments AGAINST Unique Behaviors (Keep Generic):
1. **Simplicity**: Generic helpers are simpler to maintain
2. **Consistency Across Design Systems**: Users see predictable state changes
3. **Background vs Border**: Backgrounds are less prominent than borders
4. **Already Sufficient**: Current generic helpers may be "good enough"

## Recommendation: ALL PAINTERS GET UNIQUE CODE (Like BorderPainters)

**NO SHARED HELPER FUNCTIONS** - Each painter implements its own distinct state handling inline.

### All 21 Painters Get Custom State Behaviors:

**Complex design systems with distinctive state behaviors:**

1. **Material3BackgroundPainter** - Elevation-based lighting
2. **MaterialYouBackgroundPainter** - Dynamic color tinting
3. **Fluent2BackgroundPainter** - Acrylic blur intensity
4. **Windows11MicaBackgroundPainter** - Mica noise pattern
5. **MacOSBigSurBackgroundPainter** - Refined vibrancy
6. **iOS15BackgroundPainter** - Blur + translucency
7. **AntDesignBackgroundPainter** - Border-aware tinting
8. **TailwindCardBackgroundPainter** - Shadow elevation
9. **StripeDashboardBackgroundPainter** - Professional feedback
10. **FigmaCardBackgroundPainter** - Designer-friendly
11. **DiscordStyleBackgroundPainter** - Gaming glow
12. **GradientModernBackgroundPainter** - Gradient shifts

**Simple/Minimal design systems with subtle state behaviors:**

13. **MinimalBackgroundPainter** - Ultra-subtle 2% changes
14. **NotionMinimalBackgroundPainter** - Extremely refined 1-3% changes
15. **VercelCleanBackgroundPainter** - Clean 3% hover
16. **PillRailBackgroundPainter** - Simple 4% hover
17. **BootstrapBackgroundPainter** - Utilitarian 6% hover
18. **ChakraUIBackgroundPainter** - Modern simple 5% hover

**Already have custom behaviors (validate and refine):**

19. **DarkGlowBackgroundPainter** ✅ - Custom glow multiplier
20. **NeumorphismBackgroundPainter** ✅ - Inverted highlight
21. **GlassAcrylicBackgroundPainter** ✅ - Alpha adjustment

## Implementation Plan - ALL Painters Get Unique Code

### Phase 1: Material Design Systems (3 painters) ✅ COMPLETED
1. ✅ Material3BackgroundPainter - Inline elevation lighting (15% hover, 12% press, 20% selected, 8% focus, 80 alpha disabled)
2. ✅ MaterialYouBackgroundPainter - Inline primary color tinting (10% hover, 8% press, 15% selected, 7% focus, 90 alpha disabled)
3. ✅ AntDesignBackgroundPainter - Inline border-aware tinting (8% hover, 12% press/selected, 6% focus, 90 alpha disabled)

### Phase 2: Microsoft Design Systems (3 painters) ✅ COMPLETED
4. ✅ Fluent2BackgroundPainter - Inline acrylic opacity (10% hover, 8% press, 14% selected, 6% focus, 70 alpha disabled)
5. ✅ Windows11MicaBackgroundPainter - Inline Mica tint with primary color (8% hover, 12% press with 10% darken, 10% selected, 6% focus, 2% disabled at 75 alpha) + 2% gradient
6. ✅ DiscordStyleBackgroundPainter - Inline gaming glow (12% hover, 8% press, 18% selected, 10% focus, 80 alpha disabled) + primary color overlay on hover

### Phase 3: Apple Design Systems (2 painters)
7. MacOSBigSurBackgroundPainter - Remove helpers, add inline vibrancy
8. iOS15BackgroundPainter - Remove helpers, add inline blur/translucency

### Phase 4: Modern Card/Dashboard Systems (4 painters)
9. TailwindCardBackgroundPainter - Remove helpers, add inline shadow elevation
10. StripeDashboardBackgroundPainter - Remove helpers, add inline professional feedback
11. FigmaCardBackgroundPainter - Remove helpers, add inline designer-friendly
12. GradientModernBackgroundPainter - Remove helpers, add inline gradient shifts

### Phase 5: Minimal/Simple Systems (6 painters)
13. MinimalBackgroundPainter - Remove helpers, add inline ultra-subtle 2%
14. NotionMinimalBackgroundPainter - Remove helpers, add inline refined 1-3%
15. VercelCleanBackgroundPainter - Remove helpers, add inline clean 3%
16. PillRailBackgroundPainter - Remove helpers, add inline simple 4%
17. BootstrapBackgroundPainter - Remove helpers, add inline utilitarian 6%
18. ChakraUIBackgroundPainter - Remove helpers, add inline modern 5%

### Phase 6: Validate Existing Custom Behaviors (3 painters)
19. DarkGlowBackgroundPainter - Verify no helpers used ✅
20. NeumorphismBackgroundPainter - Verify no helpers used ✅
21. GlassAcrylicBackgroundPainter - Verify no helpers used ✅

## Proposed Custom State Behaviors

### Material3BackgroundPainter - Inline Elevation Lighting
```csharp
// NO HELPER FUNCTIONS - All state logic inline
Color stateColor;
switch (state)
{
    case ControlState.Hovered:
        // Material3 elevation lighting: 15% lighter on hover
        int hR = Math.Min(255, backgroundColor.R + (int)(255 * 0.15f));
        int hG = Math.Min(255, backgroundColor.G + (int)(255 * 0.15f));
        int hB = Math.Min(255, backgroundColor.B + (int)(255 * 0.15f));
        stateColor = Color.FromArgb(backgroundColor.A, hR, hG, hB);
        break;
    case ControlState.Pressed:
        // Material3 pressed: 12% darker
        int pR = Math.Max(0, backgroundColor.R - (int)(backgroundColor.R * 0.12f));
        int pG = Math.Max(0, backgroundColor.G - (int)(backgroundColor.G * 0.12f));
        int pB = Math.Max(0, backgroundColor.B - (int)(backgroundColor.B * 0.12f));
        stateColor = Color.FromArgb(backgroundColor.A, pR, pG, pB);
        break;
    case ControlState.Selected:
        // Material3 selected: 20% lighter (bold elevation)
        int sR = Math.Min(255, backgroundColor.R + (int)(255 * 0.20f));
        int sG = Math.Min(255, backgroundColor.G + (int)(255 * 0.20f));
        int sB = Math.Min(255, backgroundColor.B + (int)(255 * 0.20f));
        stateColor = Color.FromArgb(backgroundColor.A, sR, sG, sB);
        break;
    case ControlState.Focused:
        // Material3 focused: 8% lighter
        int fR = Math.Min(255, backgroundColor.R + (int)(255 * 0.08f));
        int fG = Math.Min(255, backgroundColor.G + (int)(255 * 0.08f));
        int fB = Math.Min(255, backgroundColor.B + (int)(255 * 0.08f));
        stateColor = Color.FromArgb(backgroundColor.A, fR, fG, fB);
        break;
    case ControlState.Disabled:
        // Material3 disabled: 80 alpha
        stateColor = Color.FromArgb(80, backgroundColor);
        break;
    default: // Normal
        stateColor = backgroundColor;
        break;
}

using (SolidBrush brush = new SolidBrush(stateColor))
{
    g.FillRectangle(brush, rect);
}
// NO overlay needed for Material3
```

### MaterialYouBackgroundPainter - Inline Dynamic Color Tinting
```csharp
// NO HELPER FUNCTIONS - All state logic inline with primary color tinting
Color primaryColor = theme.GetPrimary();
Color stateColor;

switch (state)
{
    case ControlState.Hovered:
        // MaterialYou hover: Blend 10% primary color
        int hR = (int)(backgroundColor.R * 0.90f + primaryColor.R * 0.10f);
        int hG = (int)(backgroundColor.G * 0.90f + primaryColor.G * 0.10f);
        int hB = (int)(backgroundColor.B * 0.90f + primaryColor.B * 0.10f);
        stateColor = Color.FromArgb(backgroundColor.A, hR, hG, hB);
        break;
    case ControlState.Pressed:
        // MaterialYou pressed: 8% darker
        int pR = Math.Max(0, backgroundColor.R - (int)(backgroundColor.R * 0.08f));
        int pG = Math.Max(0, backgroundColor.G - (int)(backgroundColor.G * 0.08f));
        int pB = Math.Max(0, backgroundColor.B - (int)(backgroundColor.B * 0.08f));
        stateColor = Color.FromArgb(backgroundColor.A, pR, pG, pB);
        break;
    case ControlState.Selected:
        // MaterialYou selected: Blend 15% primary color (personalized)
        int sR = (int)(backgroundColor.R * 0.85f + primaryColor.R * 0.15f);
        int sG = (int)(backgroundColor.G * 0.85f + primaryColor.G * 0.15f);
        int sB = (int)(backgroundColor.B * 0.85f + primaryColor.B * 0.15f);
        stateColor = Color.FromArgb(backgroundColor.A, sR, sG, sB);
        break;
    case ControlState.Focused:
        // MaterialYou focused: Blend 7% primary color
        int fR = (int)(backgroundColor.R * 0.93f + primaryColor.R * 0.07f);
        int fG = (int)(backgroundColor.G * 0.93f + primaryColor.G * 0.07f);
        int fB = (int)(backgroundColor.B * 0.93f + primaryColor.B * 0.07f);
        stateColor = Color.FromArgb(backgroundColor.A, fR, fG, fB);
        break;
    case ControlState.Disabled:
        // MaterialYou disabled: 90 alpha
        stateColor = Color.FromArgb(90, backgroundColor);
        break;
    default: // Normal
        stateColor = backgroundColor;
        break;
}

using (SolidBrush brush = new SolidBrush(stateColor))
{
    g.FillRectangle(brush, rect);
}
```

### MinimalBackgroundPainter - Ultra-Subtle Inline Changes
```csharp
// NO HELPER FUNCTIONS - Minimal design = ultra-subtle 2% changes
Color stateColor;
switch (state)
{
    case ControlState.Hovered:
        // Minimal hover: Only 2% lighter
        int hR = Math.Min(255, backgroundColor.R + (int)(255 * 0.02f));
        int hG = Math.Min(255, backgroundColor.G + (int)(255 * 0.02f));
        int hB = Math.Min(255, backgroundColor.B + (int)(255 * 0.02f));
        stateColor = Color.FromArgb(backgroundColor.A, hR, hG, hB);
        break;
    case ControlState.Pressed:
        // Minimal pressed: Only 3% darker
        int pR = Math.Max(0, backgroundColor.R - (int)(backgroundColor.R * 0.03f));
        int pG = Math.Max(0, backgroundColor.G - (int)(backgroundColor.G * 0.03f));
        int pB = Math.Max(0, backgroundColor.B - (int)(backgroundColor.B * 0.03f));
        stateColor = Color.FromArgb(backgroundColor.A, pR, pG, pB);
        break;
    case ControlState.Selected:
        // Minimal selected: Only 4% lighter
        int sR = Math.Min(255, backgroundColor.R + (int)(255 * 0.04f));
        int sG = Math.Min(255, backgroundColor.G + (int)(255 * 0.04f));
        int sB = Math.Min(255, backgroundColor.B + (int)(255 * 0.04f));
        stateColor = Color.FromArgb(backgroundColor.A, sR, sG, sB);
        break;
    case ControlState.Focused:
        // Minimal focused: Only 1% lighter (barely visible)
        int fR = Math.Min(255, backgroundColor.R + (int)(255 * 0.01f));
        int fG = Math.Min(255, backgroundColor.G + (int)(255 * 0.01f));
        int fB = Math.Min(255, backgroundColor.B + (int)(255 * 0.01f));
        stateColor = Color.FromArgb(backgroundColor.A, fR, fG, fB);
        break;
    case ControlState.Disabled:
        // Minimal disabled: 110 alpha (subtle)
        stateColor = Color.FromArgb(110, backgroundColor);
        break;
    default: // Normal
        stateColor = backgroundColor;
        break;
}

using (SolidBrush brush = new SolidBrush(stateColor))
{
    g.FillRectangle(brush, rect);
}
```

### NotionMinimalBackgroundPainter - Extremely Refined Inline
```csharp
// NO HELPER FUNCTIONS - Notion minimal = extremely refined 1-3% changes
Color stateColor;
switch (state)
{
    case ControlState.Hovered:
        // Notion hover: Ultra-subtle 1.5% lighter
        int hR = Math.Min(255, backgroundColor.R + (int)(255 * 0.015f));
        int hG = Math.Min(255, backgroundColor.G + (int)(255 * 0.015f));
        int hB = Math.Min(255, backgroundColor.B + (int)(255 * 0.015f));
        stateColor = Color.FromArgb(backgroundColor.A, hR, hG, hB);
        break;
    case ControlState.Pressed:
        // Notion pressed: 2.5% darker
        int pR = Math.Max(0, backgroundColor.R - (int)(backgroundColor.R * 0.025f));
        int pG = Math.Max(0, backgroundColor.G - (int)(backgroundColor.G * 0.025f));
        int pB = Math.Max(0, backgroundColor.B - (int)(backgroundColor.B * 0.025f));
        stateColor = Color.FromArgb(backgroundColor.A, pR, pG, pB);
        break;
    case ControlState.Selected:
        // Notion selected: 3% lighter
        int sR = Math.Min(255, backgroundColor.R + (int)(255 * 0.03f));
        int sG = Math.Min(255, backgroundColor.G + (int)(255 * 0.03f));
        int sB = Math.Min(255, backgroundColor.B + (int)(255 * 0.03f));
        stateColor = Color.FromArgb(backgroundColor.A, sR, sG, sB);
        break;
    case ControlState.Focused:
        // Notion focused: 1% lighter (barely perceptible)
        int fR = Math.Min(255, backgroundColor.R + (int)(255 * 0.01f));
        int fG = Math.Min(255, backgroundColor.G + (int)(255 * 0.01f));
        int fB = Math.Min(255, backgroundColor.B + (int)(255 * 0.01f));
        stateColor = Color.FromArgb(backgroundColor.A, fR, fG, fB);
        break;
    case ControlState.Disabled:
        // Notion disabled: 115 alpha
        stateColor = Color.FromArgb(115, backgroundColor);
        break;
    default: // Normal
        stateColor = backgroundColor;
        break;
}

using (SolidBrush brush = new SolidBrush(stateColor))
{
    g.FillRectangle(brush, rect);
}
```
```csharp
// Acrylic opacity changes (already has acrylic blur)
int acrylicAlpha = state switch
{
    ControlState.Hovered => Math.Min(255, baseColor.A + 20),   // Increase opacity
    ControlState.Pressed => Math.Max(100, baseColor.A - 20),   // Decrease opacity
    ControlState.Selected => Math.Min(255, baseColor.A + 30),  // More opaque
    ControlState.Focused => Math.Min(255, baseColor.A + 10),   // Slightly more opaque
    ControlState.Disabled => 80,                                // Very transparent
    _ => baseColor.A
};
Color stateColor = Color.FromArgb(acrylicAlpha, baseColor);
```

### Windows11MicaBackgroundPainter
```csharp
// Mica tint intensity changes
float micaTintStrength = state switch
{
    ControlState.Hovered => 0.08f,    // Subtle hover tint
    ControlState.Pressed => 0.12f,    // More pronounced press
    ControlState.Selected => 0.10f,   // Noticeable selection
    ControlState.Focused => 0.06f,    // Gentle focus
    ControlState.Disabled => 0.02f,   // Very subtle when disabled
    _ => 0.05f                         // Default Mica tint
};
Color tintedMica = BlendColors(micaBase, theme.GetPrimary(), micaTintStrength);
```

### MacOSBigSurBackgroundPainter
```csharp
// Refined, subtle vibrancy changes (macOS is subtle)
Color stateColor = state switch
{
    ControlState.Hovered => Lighten(backgroundColor, 0.03f),   // Very subtle hover
    ControlState.Pressed => Darken(backgroundColor, 0.06f),    // Gentle press
    ControlState.Selected => Lighten(backgroundColor, 0.05f),  // Subtle selection
    ControlState.Focused => Lighten(backgroundColor, 0.02f),   // Barely noticeable focus
    ControlState.Disabled => WithAlpha(backgroundColor, 120),  // Translucent disabled
    _ => backgroundColor
};
Color overlay = state switch
{
    ControlState.Hovered => WithAlpha(Color.White, 10),   // Very subtle white overlay
    ControlState.Pressed => WithAlpha(Color.Black, 15),   // Gentle black overlay
    _ => Color.Transparent
};
```

### iOS15BackgroundPainter
```csharp
// Blur + translucent overlay intensity
int blurAlpha = state switch
{
    ControlState.Hovered => Math.Min(255, baseColor.A + 30),   // Increase blur opacity
    ControlState.Pressed => Math.Min(255, baseColor.A + 50),   // Strong press feedback
    ControlState.Selected => Math.Min(255, baseColor.A + 40),  // Noticeable selection
    ControlState.Focused => Math.Min(255, baseColor.A + 20),   // Gentle focus
    ControlState.Disabled => 60,                                // Very translucent
    _ => baseColor.A
};
Color stateColor = Color.FromArgb(blurAlpha, baseColor);
```

### AntDesignBackgroundPainter
```csharp
// Ant Design subtle tinting with border awareness
Color stateColor = state switch
{
    ControlState.Hovered => Lighten(backgroundColor, 0.08f),   // Noticeable hover
    ControlState.Pressed => Darken(backgroundColor, 0.12f),    // Strong press
    ControlState.Selected => Lighten(backgroundColor, 0.12f),  // Bold selection
    ControlState.Focused => Lighten(backgroundColor, 0.06f),   // Gentle focus
    ControlState.Disabled => WithAlpha(backgroundColor, 90),
    _ => backgroundColor
};
```

### TailwindCardBackgroundPainter
```csharp
// Shadow elevation changes (background stays same, but shadow intensifies)
// Note: This might need coordination with shadow painting
Color stateColor = state switch
{
    ControlState.Hovered => Lighten(backgroundColor, 0.04f),   // Subtle lift
    ControlState.Pressed => Darken(backgroundColor, 0.08f),    // Pressed down
    ControlState.Selected => Lighten(backgroundColor, 0.06f),  // Selected lift
    ControlState.Focused => backgroundColor,                    // No background change on focus
    ControlState.Disabled => WithAlpha(backgroundColor, 100),
    _ => backgroundColor
};
// Add shadow elevation state in companion shadow painter
```

### StripeDashboardBackgroundPainter
```csharp
// Professional, refined, subtle feedback
Color stateColor = state switch
{
    ControlState.Hovered => Lighten(backgroundColor, 0.04f),   // Very professional subtle hover
    ControlState.Pressed => Darken(backgroundColor, 0.06f),    // Gentle press
    ControlState.Selected => Lighten(backgroundColor, 0.07f),  // Noticeable but refined selection
    ControlState.Focused => Lighten(backgroundColor, 0.03f),   // Barely noticeable focus ring
    ControlState.Disabled => WithAlpha(backgroundColor, 110),  // Professional disabled state
    _ => backgroundColor
};
```

### FigmaCardBackgroundPainter
```csharp
// Designer-friendly hover effects (brightness increase)
Color stateColor = state switch
{
    ControlState.Hovered => Lighten(backgroundColor, 0.10f),   // Clear hover feedback
    ControlState.Pressed => Darken(backgroundColor, 0.08f),    // Press feedback
    ControlState.Selected => Lighten(backgroundColor, 0.15f),  // Bold selection for designers
    ControlState.Focused => Lighten(backgroundColor, 0.07f),   // Clear focus
    ControlState.Disabled => WithAlpha(backgroundColor, 100),
    _ => backgroundColor
};
```

### DiscordStyleBackgroundPainter
```csharp
// Gaming-style glow/brightness (Discord dark theme vibes)
Color stateColor = state switch
{
    ControlState.Hovered => Lighten(backgroundColor, 0.12f),   // Noticeable gaming hover
    ControlState.Pressed => Darken(backgroundColor, 0.08f),    // Pressed feedback
    ControlState.Selected => Lighten(backgroundColor, 0.18f),  // Bold gaming selection
    ControlState.Focused => Lighten(backgroundColor, 0.10f),   // Clear focus
    ControlState.Disabled => WithAlpha(backgroundColor, 80),   // Very dim when disabled
    _ => backgroundColor
};
// Optional: Add subtle glow overlay on hover
Color glowOverlay = state == ControlState.Hovered 
    ? WithAlpha(theme.GetPrimary(), 20) 
    : Color.Transparent;
```

### GradientModernBackgroundPainter
```csharp
// Gradient shift/intensity on state changes
// Adjust gradient stop positions or colors
float gradientIntensity = state switch
{
    ControlState.Hovered => 1.15f,    // Intensify gradient 15%
    ControlState.Pressed => 0.90f,    // Reduce gradient 10%
    ControlState.Selected => 1.25f,   // Bold gradient 25%
    ControlState.Focused => 1.08f,    // Subtle gradient increase
    ControlState.Disabled => 0.60f,   // Dim gradient
    _ => 1.0f
};
// Apply intensity to gradient colors
Color gradientStart = MultiplyColorBrightness(startColor, gradientIntensity);
Color gradientEnd = MultiplyColorBrightness(endColor, gradientIntensity);
```

## Validation Criteria

For each painter with custom state behaviors:
1. ✅ Hover state is visually distinct from Normal
2. ✅ Pressed state is visually distinct from Hover
3. ✅ Selected state is clearly visible
4. ✅ Disabled state is obviously different (dimmed/grayed)
5. ✅ Focused state is subtly visible (for accessibility)
6. ✅ State behavior matches design system's visual language
7. ✅ No jarring transitions between states

## Success Metrics

- **21/21 painters** get unique inline state handling (NO shared helper functions)
- **Each painter** has distinct state behavior matching its design system
- **NO ApplyState() or GetStateOverlay()** calls - all logic inline
- Build succeeds with no errors
- Visual testing confirms state behaviors match design systems
- Code is self-contained and readable within each painter

## Key Implementation Rules

1. ✅ **NO BackgroundPainterHelpers.ApplyState()** - Remove all calls
2. ✅ **NO BackgroundPainterHelpers.GetStateOverlay()** - Remove all calls
3. ✅ **Inline switch statements** - Each painter has its own switch(state)
4. ✅ **Distinct percentages** - Each design system uses unique values
5. ✅ **Self-contained code** - All color math inline (no Lighten/Darken helpers)
6. ✅ **Unique to design system** - Material3 ≠ iOS15 ≠ Minimal

## Next Steps

1. User confirms approach: **All 21 painters get unique inline code**
2. Implement Phase 1: Material Design Systems (3 painters)
   - Remove ApplyState/GetStateOverlay calls
   - Add inline switch statements with distinct percentages
3. Build and test Phase 1
4. Implement Phases 2-6 sequentially
5. Final validation and documentation

---

**Phase 1**: ✅ COMPLETE - Material Design Systems (3/21 painters)
- Material3BackgroundPainter: 15% hover, 12% press, 20% selected, 8% focus, 80 alpha disabled
- MaterialYouBackgroundPainter: 10% primary blend hover, 8% press, 15% blend selected, 7% focus, 90 alpha disabled
- AntDesignBackgroundPainter: 8% hover, 12% press/selected, 6% focus, 90 alpha disabled

**Phase 2**: ✅ COMPLETE - Microsoft Design Systems (6/21 painters)
- Fluent2BackgroundPainter: 10% hover, 8% press, 14% selected, 6% focus, 70 alpha disabled (acrylic-inspired)
- Windows11MicaBackgroundPainter: 8% hover tint, 12% press tint + 10% darken, 10% selected, 6% focus, 2% disabled at 75 alpha (maintains 2% vertical gradient)
- DiscordStyleBackgroundPainter: 12% hover, 8% press, 18% selected, 10% focus, 80 alpha disabled (+ primary color glow overlay on hover at 20 alpha)

**Phase 3**: ✅ COMPLETE - Apple Design Systems (8/21 painters)
- MacOSBigSurBackgroundPainter: 3% hover, 6% press, 5% selected, 2% focus, 120 alpha disabled (maintains 5% gradient + 10 alpha white overlay hover, 15 alpha black press)
- iOS15BackgroundPainter: Blur opacity modulation - hover +30 alpha, press +50, selected +40, focus +20, disabled 60 alpha (translucent white overlay 38 base alpha)

**Phase 4**: ✅ COMPLETE - Modern Card/Dashboard Systems (12/21 painters)
- TailwindCardBackgroundPainter: 7% hover, 5% press, 10% selected, 5% focus, 100 alpha disabled (maintains 5% bottom gradient for shadow elevation)
- StripeDashboardBackgroundPainter: 4% hover, 6% press, 7% selected, 3% focus, 110 alpha disabled (maintains 3% top gradient for professional polish)
- FigmaCardBackgroundPainter: 10% hover, 8% press, 15% selected (bold for designers), 7% focus, 100 alpha disabled (designer-friendly clear feedback)
- GradientModernBackgroundPainter: Gradient intensity modulation - 1.15x hover, 0.90x press, 1.25x selected, 1.08x focus, 0.60x disabled (maintains 30% darker bottom)

**Phase 5**: ✅ COMPLETE - Minimal/Simple Systems (18/21 painters)
- MinimalBackgroundPainter: 2% hover (ultra-subtle), 3% press, 4% selected, 1.5% focus, 120 alpha disabled (true minimalism with barely noticeable changes)
- NotionMinimalBackgroundPainter: 1.5% hover (zen), 2.5% press, 3% selected, 1% focus (MOST SUBTLE), 130 alpha disabled (extremely refined for zen aesthetic)
- VercelCleanBackgroundPainter: 3% hover (clean), 4% press, 5% selected, 2% focus, 110 alpha disabled (pristine clean design with subtle feedback)
- PillRailBackgroundPainter: 4% hover (simple), 5% press, 6% selected, 3% focus, 105 alpha disabled (soft pill-shaped rail with simple feedback)
- BootstrapBackgroundPainter: 6% hover (utilitarian), 7% press, 9% selected (HIGHEST IN MINIMAL GROUP), 4% focus, 95 alpha disabled (practical noticeable feedback)
- ChakraUIBackgroundPainter: 5% hover (balanced), 6% press, 8% selected, 3.5% focus, 100 alpha disabled (modern balanced feedback)

**Phase 6**: ✅ COMPLETE - Complex Effects Systems (21/21 painters) **ALL COMPLETE!**
- DarkGlowBackgroundPainter: 5% hover, 3% press, 7% selected, 4% focus, 130 alpha disabled + INLINE glow intensity modulation (1.3x hover, 0.7x press, 1.2x selected, 1.1x focus, 0.4x disabled) + INLINE geometry (InsetRectangle, CreateRoundedRectangle, WithAlpha for 3 glow rings)
- NeumorphismBackgroundPainter: 3% hover, 6% press (darkened for depth), 5% selected, 2% focus, 110 alpha disabled + INLINE inverted highlight (10% lighten normal, 10% darken pressed) + INLINE CreateRoundedRectangle + INLINE WithAlpha
- GlassAcrylicBackgroundPainter: Base alpha modulation (pressed 140, hover +20, selected +30, focus +15, disabled 70) + INLINE multi-layer frosted glass (3 layers with CreateRoundedRectangle, WithAlpha) + NO background tinting (pure alpha-based translucency)

**Status**: ✅ **21/21 painters COMPLETE (100%)**  
**Completion**: All BackgroundPainters now have unique inline ControlState handling  
**Achievement**: NO shared helper functions - every painter self-contained per user mandate  
**Result**: Consistent with BorderPainters approach - all 24 BorderPainters + 21 BackgroundPainters = 45 painters with unique behaviors
