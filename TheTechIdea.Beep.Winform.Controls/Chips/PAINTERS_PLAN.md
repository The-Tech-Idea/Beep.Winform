# Chip Painters Implementation Plan

Each ChipStyle has a distinct, self-contained painter. No base class - each painter is fully independent.

## ChipStyle Enum Values (18 total)

| Style | Painter Class | Visual Description |
|-------|--------------|---------------------|
| Default | DefaultChipGroupPainter | Standard filled chip, solid background, rounded corners |
| Modern | ModernChipGroupPainter | Material Design inspired, subtle shadows, smooth transitions |
| Classic | ClassicChipGroupPainter | Traditional outlined chip, thin border, transparent bg |
| Minimalist | MinimalistChipGroupPainter | Text-only, no background, subtle hover underline |
| Colorful | ColorfulChipGroupPainter | Vibrant gradient backgrounds, each chip different color |
| Professional | ProfessionalChipGroupPainter | Corporate style, muted colors, clean borders |
| Soft | SoftChipGroupPainter | Pastel colors, very rounded, gentle shadows |
| HighContrast | HighContrastChipGroupPainter | Bold borders, high visibility, accessibility focused |
| Pill | PillChipGroupPainter | Full stadium/capsule shape (height/2 radius) |
| Likeable | LikeableChipGroupPainter | Heart icon, pink/red theme, love/favorite style |
| Ingredient | IngredientChipGroupPainter | Tag style with checkmarks, food/recipe inspired |
| Shaded | ShadedChipGroupPainter | Gradient fill from top to bottom |
| Avatar | AvatarChipGroupPainter | Circular user image on left, user chip style |
| Elevated | ElevatedChipGroupPainter | Strong drop shadow, floating effect |
| Smooth | SmoothChipGroupPainter | Extra smooth corners, subtle inner glow |
| Square | SquareChipGroupPainter | Minimal rounding (2-4px), sharp modern look |
| Dashed | DashedChipGroupPainter | Dashed border style, add-new placeholder feel |
| Bold | BoldChipGroupPainter | Bold text, 30% opacity background tint |

---

## Painter Specifications

### 1. DefaultChipGroupPainter (EXISTS)
- **Background**: Solid color from theme (selected: primary, unselected: surface)
- **Border**: 1px solid, theme border color
- **Corner Radius**: 15px (medium), scales with ChipSize
- **Icons**: Leading icon from ImagePath, trailing X on selected
- **Selection Mark**: Checkmark on left when selected

### 2. ModernChipGroupPainter
- **Background**: Subtle elevation shadow, filled when selected
- **Border**: None when filled, 1px when outlined
- **Corner Radius**: 16px
- **Icons**: Material Design style icons using StyledImagePainter
- **Selection Mark**: Filled dot indicator
- **Special**: Ripple effect hint on hover (color pulse)

### 3. ClassicChipGroupPainter
- **Background**: Transparent, light tint on hover
- **Border**: 2px solid, theme accent color
- **Corner Radius**: 12px
- **Icons**: Standard icons, theme colored
- **Selection Mark**: Filled background when selected

### 4. MinimalistChipGroupPainter
- **Background**: None (transparent)
- **Border**: None
- **Corner Radius**: N/A (no visible shape)
- **Icons**: None
- **Selection Mark**: Underline or bold text
- **Special**: Text-only, hover shows subtle underline

### 5. ColorfulChipGroupPainter
- **Background**: Gradient (varies per chip index: red, orange, yellow, green, blue, purple cycle)
- **Border**: None
- **Corner Radius**: 20px (very rounded)
- **Icons**: White icons for contrast
- **Selection Mark**: White checkmark
- **Special**: Each chip gets unique color from palette

### 6. ProfessionalChipGroupPainter
- **Background**: Muted gray/blue tones
- **Border**: 1px solid, subtle gray
- **Corner Radius**: 8px (less rounded)
- **Icons**: Monochrome icons
- **Selection Mark**: Subtle dot
- **Special**: Corporate/business aesthetic

### 7. SoftChipGroupPainter
- **Background**: Pastel colors (light pink, light blue, light green, lavender)
- **Border**: None
- **Corner Radius**: 20px (pill-like)
- **Icons**: Matching pastel-tinted icons
- **Selection Mark**: Soft checkmark
- **Special**: Gentle, friendly appearance

### 8. HighContrastChipGroupPainter
- **Background**: Black or White (high contrast)
- **Border**: 3px solid, contrasting color
- **Corner Radius**: 10px
- **Icons**: High contrast icons
- **Selection Mark**: Bold checkmark
- **Special**: Accessibility focused, WCAG compliant

### 9. PillChipGroupPainter
- **Background**: Solid fill
- **Border**: Optional 1px
- **Corner Radius**: height / 2 (full stadium shape)
- **Icons**: Standard
- **Selection Mark**: Checkmark
- **Special**: Perfect capsule/stadium shape

### 10. LikeableChipGroupPainter ❤️
- **Background**: Pink gradient (#FF6B9D → #FF8FB1)
- **Border**: None
- **Corner Radius**: 16px
- **Icons**: Heart icon (SvgsUI.Heart) as leading icon
- **Selection Mark**: Filled heart (red)
- **Special**: Pink/red theme, love/favorite aesthetic
- **Unselected**: Outlined heart, light pink bg
- **Selected**: Filled heart, vibrant pink bg

### 11. IngredientChipGroupPainter 🏷️
- **Background**: Light green/cream tint
- **Border**: 1px dashed or solid green
- **Corner Radius**: 8px
- **Icons**: Checkmark (SvgsUI.Check) when selected
- **Selection Mark**: Green checkmark
- **Special**: Food/recipe tag style, organic feel
- **Unselected**: Empty checkbox style
- **Selected**: Green checkmark, slightly darker bg

### 12. ShadedChipGroupPainter
- **Background**: Linear gradient top-to-bottom (light to dark)
- **Border**: None
- **Corner Radius**: 14px
- **Icons**: Standard with subtle shadow
- **Selection Mark**: Gradient-aware checkmark
- **Special**: 3D shaded effect

### 13. AvatarChipGroupPainter 👤
- **Background**: Light gray
- **Border**: 1px solid
- **Corner Radius**: 16px
- **Icons**: Circular avatar on LEFT (from ImagePath or SvgsUI.User default)
- **Selection Mark**: Blue checkmark badge on avatar
- **Special**: User/person chips with profile pictures
- **Avatar Size**: 24px circle, clipped

### 14. ElevatedChipGroupPainter
- **Background**: White/light surface
- **Border**: None
- **Corner Radius**: 12px
- **Icons**: Standard
- **Selection Mark**: Elevated checkmark
- **Special**: Strong drop shadow (4px blur, 2px offset)
- **Hover**: Shadow increases

### 15. SmoothChipGroupPainter
- **Background**: Soft gradient with inner glow
- **Border**: None
- **Corner Radius**: 18px
- **Icons**: Smooth anti-aliased icons
- **Selection Mark**: Smooth checkmark
- **Special**: Extra smooth edges, subtle inner shadow/glow

### 16. SquareChipGroupPainter
- **Background**: Solid fill
- **Border**: 1px solid
- **Corner Radius**: 4px (minimal)
- **Icons**: Standard
- **Selection Mark**: Square checkbox style
- **Special**: Sharp, modern, minimal rounding

### 17. DashedChipGroupPainter
- **Background**: Transparent or very light
- **Border**: 2px dashed
- **Corner Radius**: 12px
- **Icons**: Plus icon (SvgsUI.Plus) for add-new feel
- **Selection Mark**: Solid border when selected
- **Special**: "Add new" placeholder style

### 18. BoldChipGroupPainter
- **Background**: 30% opacity of accent color
- **Border**: None
- **Corner Radius**: 10px
- **Icons**: Bold/thick icons
- **Selection Mark**: Bold checkmark
- **Special**: Bold text (FontStyle.Bold), semi-transparent bg

---

## Implementation Order

1. ✅ DefaultChipGroupPainter (exists)
2. ⬜ PillChipGroupPainter (simple shape change)
3. ⬜ SquareChipGroupPainter (simple shape change)
4. ⬜ LikeableChipGroupPainter (distinct visual)
5. ⬜ IngredientChipGroupPainter (distinct visual)
6. ⬜ AvatarChipGroupPainter (distinct visual)
7. ⬜ ElevatedChipGroupPainter (shadow effect)
8. ⬜ ShadedChipGroupPainter (gradient effect)
9. ⬜ ColorfulChipGroupPainter (multi-color)
10. ⬜ SoftChipGroupPainter (pastel colors)
11. ⬜ ModernChipGroupPainter (material design)
12. ⬜ ClassicChipGroupPainter (outlined)
13. ⬜ MinimalistChipGroupPainter (text-only)
14. ⬜ ProfessionalChipGroupPainter (corporate)
15. ⬜ HighContrastChipGroupPainter (accessibility)
16. ⬜ SmoothChipGroupPainter (smooth edges)
17. ⬜ DashedChipGroupPainter (dashed border)
18. ⬜ BoldChipGroupPainter (bold text)

---

## Icons Used (from SvgsUI)

- `SvgsUI.Heart` - Likeable style
- `SvgsUI.Check` - Selection marks
- `SvgsUI.CheckCircle` - Ingredient style
- `SvgsUI.User` - Avatar default
- `SvgsUI.Plus` - Dashed/add style
- `SvgsUI.X` - Close button
- `SvgsUI.Star` - Optional rating style

## Color Palettes

### Likeable (Pink/Red)
- Primary: #FF6B9D
- Selected: #E91E63
- Hover: #FF8FB1

### Ingredient (Green)
- Primary: #4CAF50
- Selected: #2E7D32
- Background: #E8F5E9

### Colorful Cycle
1. #EF5350 (Red)
2. #FF9800 (Orange)
3. #FFEB3B (Yellow)
4. #4CAF50 (Green)
5. #2196F3 (Blue)
6. #9C27B0 (Purple)

### Soft Pastels
- Pink: #FFCDD2
- Blue: #BBDEFB
- Green: #C8E6C9
- Purple: #E1BEE7
- Yellow: #FFF9C4

