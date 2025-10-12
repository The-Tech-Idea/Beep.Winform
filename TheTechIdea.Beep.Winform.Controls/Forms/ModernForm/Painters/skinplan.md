# Form Painter Skin Architecture Plan

## Overview
Based on DevExpress, Telerik, and other professional skinning systems, form painters should follow a clear separation of concerns and standardized painting order.

## Core Principles

### 1. Method Responsibilities
- **PaintBackground**: Paints ONLY the form background (solid colors, gradients, textures)
- **PaintCaption**: Paints ONLY the caption bar (title text, caption effects)
- **PaintBorders**: Paints ONLY the form borders (outlines, edge effects)
- **PaintWithEffects**: ORCHESTRATES the painting process, handles clipping, shadows, and effects

### 2. Painting Order (DevExpress Pattern)
```
1. Setup graphics quality
2. Calculate layout and hit areas
3. Draw shadow (if any)
4. Paint solid background for ENTIRE form (no clipping)
5. Apply clipping to rounded window path (entire form)
6. Paint background effects/decorations (gradients, textures, etc.) over entire background
7. Reset clipping
8. Paint borders
9. Paint caption
10. Paint system buttons/regions
```

### 3. Clipping Strategy
- Full base background: Paint with NO clipping across entire form
- Decorative effects: Clip to rounded window path and paint over entire background
- Borders/Caption: Paint with NO clipping

### 4. Control Interaction Requirements
- Use OnPaintBackground for form decorations (paints BEFORE controls)
- Never paint over ContentRect unless PaintOverContentArea = true
- Always reset Graphics.Clip after use
- Ensure controls have proper background color

## Painter Method Contracts

### PaintBackground(Graphics g, BeepiFormPro owner)
**Purpose**: Paint form background only
**Rules**:
- Paint solid base colors
- Paint background textures/patterns
- DO NOT paint gradients or overlays that should be clipped
- Should work with current Graphics.Clip
- Should provide readable background for controls

### PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
**Purpose**: Paint caption bar only
**Rules**:
- Paint caption background
- Paint title text
- Paint caption decorations
- Respect captionRect boundaries
- DO NOT paint system buttons (handled by regions)

### PaintBorders(Graphics g, BeepiFormPro owner)
**Purpose**: Paint form borders only
**Rules**:
- Paint outer border lines
- Paint border shadows/glows
- Paint corner decorations
- Use owner.ClientRectangle for boundaries

### PaintWithEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
**Purpose**: Orchestrate the complete painting process
**Rules**:
- Call other Paint methods in correct order
- Handle Graphics.Clip management
- Handle shadow effects
- Handle rounded corners
- Handle anti-aliasing setup
- DO NOT duplicate painting logic from other methods

## DevExpress/Telerik Pattern Implementation

### Standard PaintWithEffects Structure:
```csharp
public void PaintWithEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
{
    // 1. Setup
    var originalClip = g.Clip;
    var shadow = GetShadowEffect(owner);
    var radius = GetCornerRadius(owner);
    
    // 2. Shadow (behind everything)
    if (!shadow.Inner)
    {
        DrawShadow(g, rect, shadow, radius);
    }
    
    // 3. Base background (entire form, no clipping)
    PaintBackground(g, owner);
    
    // 4. Decorative effects (with clipping)
    using var path = CreateRoundedRectanglePath(rect, radius);
    g.Clip = new Region(path);

    PaintBackgroundEffects(g, owner); // Clipped effects over entire background
    
    g.Clip = originalClip;
    
    // 5. Borders (no clipping)
    PaintBorders(g, owner);
    
    // 6. Caption (no clipping)
    if (owner.ShowCaptionBar)
    {
        PaintCaption(g, owner, owner.CurrentLayout.CaptionRect);
    }
    
    // 7. Cleanup
    g.Clip = originalClip;
}
```

## New Method: PaintBackgroundEffects
Each painter should implement:
```csharp
private void PaintBackgroundEffects(Graphics g, BeepiFormPro owner)
{
    // Paint gradients, textures, overlays that should be clipped
    // This is called AFTER clipping is set up
}
```

## Painter-Specific Guidelines

### FluentFormPainter
- **PaintBackground**: Solid fluent blue-gray base
- **PaintBackgroundEffects**: Acrylic noise, gradient overlay
- **PaintCaption**: Reveal highlight, accent line, title text
- **PaintBorders**: Subtle rounded border

### GlassFormPainter  
- **PaintBackground**: Solid glass base color
- **PaintBackgroundEffects**: Mica gradient, noise texture
- **PaintCaption**: Frosted glass caption, title text
- **PaintBorders**: Glass glow border

### MaterialFormPainter
- **PaintBackground**: Solid material background
- **PaintBackgroundEffects**: Elevation shadows, gradients
- **PaintCaption**: Material caption bar, title text
- **PaintBorders**: Material design border

### CartoonFormPainter
- **PaintBackground**: Bright saturated base
- **PaintBackgroundEffects**: Comic effects, patterns
- **PaintCaption**: Comic book style caption
- **PaintBorders**: Bold cartoon border

### MacOSFormPainter
- **PaintBackground**: Solid macOS background
- **PaintBackgroundEffects**: Subtle macOS gradients
- **PaintCaption**: macOS title bar style
- **PaintBorders**: macOS window border

### MinimalFormPainter
- **PaintBackground**: Simple solid color
- **PaintBackgroundEffects**: None or very subtle
- **PaintCaption**: Clean minimal text
- **PaintBorders**: Simple line border

### ChatBubbleFormPainter
- **PaintBackground**: Solid bubble base
- **PaintBackgroundEffects**: Bubble gradients, shadows
- **PaintCaption**: Speech bubble caption
- **PaintBorders**: Bubble outline

## Implementation Steps

1. ✅ Create this plan
2. ✅ Refactor FluentFormPainter
3. ✅ Refactor GlassFormPainter  
4. ✅ Refactor MaterialFormPainter
5. ✅ Refactor CartoonFormPainter
6. ✅ Refactor MacOSFormPainter
7. ✅ Refactor MinimalFormPainter
8. ✅ Refactor ChatBubbleFormPainter
9. ⏳ Test all painters
10. ⏳ Document final patterns

## Benefits

### Code Quality
- Clear separation of concerns
- No duplicated painting logic
- Easier to maintain and debug
- Consistent patterns across all painters

### Performance
- Proper clipping reduces overdraw
- Graphics state managed correctly
- No unnecessary operations

### Reliability
- Consistent control interaction
- Proper Z-order painting
- Predictable behavior across all styles

### Extensibility
- Easy to add new effects
- Clear extension points
- Consistent API for all painters

## Testing Requirements
For each painter:
1. ✅ Controls selectable and movable in designer
2. ✅ No black areas in content region
3. ✅ Proper background colors for controls
4. ✅ Visual effects render correctly
5. ✅ Performance is acceptable
6. ✅ Works with PaintOverContentArea = true/false

## Success Criteria
- All 7 painters follow the same pattern
- All painters have clean method separation
- PaintWithEffects only orchestrates, doesn't paint
- Control interaction works perfectly in all styles
- Visual quality matches or exceeds current implementation