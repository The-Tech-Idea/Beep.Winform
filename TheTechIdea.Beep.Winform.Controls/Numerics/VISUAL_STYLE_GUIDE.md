# BeepNumericUpDown - Visual Style Guide

Quick reference for all 16 painter styles with their unique characteristics.

---

## Light Themes (10 painters)

### 1. Material3 - Material Design 3
```
┌─────────────────────────────────────┐
│  [-]    50.00    [+]                │  ← Filled purple (103,80,164)
│                                     │  ← Elevated buttons with shadows
│  8px radius, translucent fill      │  ← Material You palette
└─────────────────────────────────────┘
```
**Key:** Filled container, elevated buttons, multi-layer shadows

### 2. iOS15 - iOS Translucent
```
┌─────────────────────────────────────┐
│  (−)    50.00    (+)                │  ← Pill-shaped buttons (full rounded)
│                                     │  ← Translucent (240 alpha)
│  SF Pro fonts, inner shadow        │  ← iOS blue (0,122,255)
└─────────────────────────────────────┘
```
**Key:** Pill buttons, translucent, elevation shadows

### 3. Fluent2 - Microsoft Fluent 2
```
┌─────────────────────────────────────┐
│  │-│   50.00   │+│                  │  ← Pure white, flat gray buttons
│  ╰─╯           ╰─╯                  │  ← Focus ring (double border)
│  4px radius, Segoe UI              │  ← Fluent blue (0,120,212)
└─────────────────────────────────────┘
```
**Key:** Clean white, focus ring, minimal decorations

### 4. Minimal - Ultra-Minimal
```
┌─────────────────────────────────────┐
│  [-│   50.00   │+]                  │  ← NO rounded corners (flat)
│                                     │  ← Thin 1px borders
│  Monochrome gray, no decorations   │  ← Simple separator lines
└─────────────────────────────────────┘
```
**Key:** Absolute simplicity, flat design, no shadows

### 5. AntDesign - Ant Design
```
┌─────────────────────────────────────┐
│  ╭─╮   50.00   ╭─╮                  │  ← 2px focus border
│  │-│           │+│                  │  ← Ant blue (24,144,255)
│  ╰─╯           ╰─╯                  │  ← Light blue hover (240,247,255)
└─────────────────────────────────────┘
```
**Key:** Clean borders, 2px focus, Ant Design system

### 6. MaterialYou - Material You
```
┌─────────────────────────────────────┐
│  (−)    50.00    (+)                │  ← Large 16px radius
│                                     │  ← Bold icons (14pt)
│  Tonal surface, prominent buttons  │  ← Dynamic theming
└─────────────────────────────────────┘
```
**Key:** Large radius, bold presence, pill buttons

### 7. Windows11Mica - Windows 11 Mica
```
┌─────────────────────────────────────┐
│  ╭─╮   50.00   ╭─╮                  │  ← Translucent Mica (245 alpha)
│  │-│           │+│                  │  ← Layered effect
│  ╰─╯           ╰─╯                  │  ← WinUI 3 styling
└─────────────────────────────────────┘
```
**Key:** Mica material, layered translucency, Segoe Fluent Icons

### 8. MacOSBigSur - macOS Big Sur
```
┌─────────────────────────────────────┐
│  ╭─╮   50.00   ╭─╮                  │  ← Vibrancy gradients
│  │-│           │+│                  │  ← Focus glow ring
│  ╰─╯           ╰─╯                  │  ← SF Pro fonts
└─────────────────────────────────────┘
```
**Key:** Gradient vibrancy, inner shadow, focus ring glow

### 9. ChakraUI - Chakra UI
```
┌─────────────────────────────────────┐
│  ╭─╮   50.00   ╭─╮                  │  ← Accessible colors
│  │-│           │+│                  │  ← Focus shadow glow
│  ╰─╯           ╰─╯                  │  ← Chakra blue (66,153,225)
└─────────────────────────────────────┘
```
**Key:** Accessibility-focused, warm grays, box-shadow focus

### 10. TailwindCard - Tailwind CSS
```
┌─────────────────────────────────────┐
│  ╭─╮   50.00   ╭─╮                  │  ← Card elevation (shadow)
│  │-│           │+│                  │  ← Ring focus (ring-2)
│  ╰─╯           ╰─╯                  │  ← Tailwind blue (59,130,246)
└─────────────────────────────────────┘
```
**Key:** Card design, shadow elevation, Tailwind ring

### 11. NotionMinimal - Notion Workspace
```
┌─────────────────────────────────────┐
│  ╭─╮   50.00   ╭─╮                  │  ← Warm white (251,251,250)
│  │-│           │+│                  │  ← Very subtle 3px radius
│  ╰─╯           ╰─╯                  │  ← Separator on hover only
└─────────────────────────────────────┘
```
**Key:** Workspace minimal, subtle grays, clean spacing

### 12. VercelClean - Vercel Monochrome
```
┌─────────────────────────────────────┐
│  ╭─╮   50.00   ╭─╮                  │  ← Pure black on white
│  │-│           │+│                  │  ← Bold 2px focus border
│  ╰─╯           ╰─╯                  │  ← High contrast (0,0,0)
└─────────────────────────────────────┘
```
**Key:** Maximum contrast, bold borders, geometric

### 13. StripeDashboard - Stripe Professional
```
┌─────────────────────────────────────┐
│  ╭─╮   50.00   ╭─╮                  │  ← Professional palette
│  │-│           │+│                  │  ← Stripe purple (99,91,255)
│  ╰─╯           ╰─╯                  │  ← Subtle shadow on focus
└─────────────────────────────────────┘
```
**Key:** Payment-focused, Stripe purple, professional

---

## Dark Themes (3 painters)

### 14. DarkGlow - Cyberpunk Neon
```
┌─────────────────────────────────────┐
│  ╔═╗   50.00   ╔═╗                  │  ← Dark blue-gray (26,32,44)
│  ║-║           ║+║                  │  ← Cyan glow (0,255,200)
│  ╚═╝           ╚═╝                  │  ← Multi-layer glow (8 layers)
└─────────────────────────────────────┘
       ◉◉◉ glow effect ◉◉◉
```
**Key:** Cyberpunk aesthetic, neon glow, multi-layer effects

### 15. DiscordStyle - Discord Dark
```
┌─────────────────────────────────────┐
│  ╭─╮   50.00   ╭─╮                  │  ← Discord dark (64,68,75)
│  │-│           │+│                  │  ← Blurple (88,101,242)
│  ╰─╯           ╰─╯                  │  ← Blurple buttons on hover
└─────────────────────────────────────┘
```
**Key:** Gaming dark theme, blurple accent, Discord colors

---

## Gradient Theme (1 painter)

### 16. GradientModern - Vibrant Gradients
```
┌─────────────────────────────────────┐
│  (─)   50.00   (+)                  │  ← Purple-pink gradient
│ ╰Purple ═══ Pink╯                   │  ← Diagonal 45° gradient
│  Glowing buttons, gradient text    │  ← Multi-layer glow
└─────────────────────────────────────┘
       ◉◉◉ gradient glow ◉◉◉
```
**Key:** Full gradient aesthetic, vibrant colors, glow effects

---

## Quick Style Selection Guide

### When to use each style:

**Material3** → Modern Android apps, Material Design projects  
**iOS15** → iOS/macOS apps, Apple-style interfaces  
**Fluent2** → Windows desktop apps, Microsoft 365 style  
**Minimal** → Clean business apps, data-heavy interfaces  
**AntDesign** → React apps, enterprise dashboards  
**MaterialYou** → Android 12+ apps, bold modern design  
**Windows11Mica** → Windows 11 native apps, modern Windows  
**MacOSBigSur** → macOS desktop apps, Mac-native look  
**ChakraUI** → Accessible web apps, React projects  
**TailwindCard** → Tailwind projects, utility-first design  
**NotionMinimal** → Workspace apps, productivity tools  
**VercelClean** → Landing pages, marketing sites  
**StripeDashboard** → Payment apps, financial dashboards  
**DarkGlow** → Gaming apps, cyberpunk themes, dark mode  
**DiscordStyle** → Chat apps, gaming communities  
**GradientModern** → Creative apps, colorful modern design  

---

## Button Layout

All painters follow the same layout:
```
┌──────────────────────────────────┐
│  DOWN    VALUE DISPLAY    UP     │
│   [-]      50.00         [+]     │
│  LEFT                    RIGHT   │
└──────────────────────────────────┘
```

- **Down button:** Always on the LEFT
- **Up button:** Always on the RIGHT
- **Value:** Centered between buttons
- **Layout:** Horizontal only

---

## Button Sizes

```
Small:       [12px buttons]
Standard:    [16px buttons]
Large:       [20px buttons]
ExtraLarge:  [24px buttons]
```

---

## Display Modes

```
Standard:      50
Percentage:    50%
Currency:      $50.00
CustomUnit:    $50.00 USD units
ProgressValue: 50
```

---

## Corner Radius Comparison

```
Minimal:        0px  (flat rectangles)
AntDesign:      2px  (very subtle)
NotionMinimal:  3px  (subtle)
Fluent2:        4px  (subtle)
VercelClean:    4px  (minimal)
ChakraUI:       6px  (moderate)
MacOSBigSur:    6px  (moderate)
StripeDashboard:6px  (moderate)
DiscordStyle:   8px  (rounded)
Material3:      8px  (rounded)
Windows11Mica:  8px  (rounded)
TailwindCard:   8px  (rounded-lg)
DarkGlow:       8px  (rounded)
GradientModern: 12px (large)
MaterialYou:    16px (extra large)
iOS15:          height/2 (pill)
```

---

## Focus Effects Comparison

**Border:**
- Minimal: Thin border
- AntDesign: 2px blue border
- Fluent2: Double ring
- Material3: 2px purple border
- Windows11Mica: 2px blue border
- VercelClean: 2px black border

**Glow:**
- iOS15: Blue glow
- MacOSBigSur: Blue glow ring
- ChakraUI: Shadow glow
- TailwindCard: Ring glow
- StripeDashboard: Purple shadow
- DarkGlow: Multi-layer cyan glow (8 layers)
- DiscordStyle: Blurple glow
- GradientModern: Gradient glow (3 layers)

**None:**
- NotionMinimal: Dark border only
- MaterialYou: 3px border

---

## Color Palettes

### Blue Family
- **iOS15:** RGB(0, 122, 255) - iOS Blue
- **Fluent2:** RGB(0, 120, 212) - Fluent Blue
- **Windows11Mica:** RGB(0, 120, 212) - Windows Blue
- **MacOSBigSur:** RGB(0, 122, 255) - macOS Blue
- **AntDesign:** RGB(24, 144, 255) - Ant Blue
- **ChakraUI:** RGB(66, 153, 225) - Chakra Blue
- **TailwindCard:** RGB(59, 130, 246) - Tailwind Blue

### Purple Family
- **Material3:** RGB(103, 80, 164) - Material Purple
- **MaterialYou:** RGB(103, 80, 164) - Material Purple
- **StripeDashboard:** RGB(99, 91, 255) - Stripe Purple
- **GradientModern:** RGB(138, 43, 226) - Vibrant Purple

### Special Colors
- **VercelClean:** RGB(0, 0, 0) - Pure Black
- **DarkGlow:** RGB(0, 255, 200) - Cyan Neon
- **DiscordStyle:** RGB(88, 101, 242) - Discord Blurple

### Gray/Minimal
- **Minimal:** RGB(80, 80, 80) - Neutral Gray
- **NotionMinimal:** Warm grays (Notion palette)

---

## Font Comparison

- **Material3:** Segoe UI 11pt
- **iOS15:** SF Pro Text 11pt / SF Pro Display 13pt
- **Fluent2:** Segoe UI 10.5pt
- **Minimal:** Segoe UI 10pt
- **AntDesign:** Segoe UI 10pt
- **MaterialYou:** Segoe UI Variable 12pt
- **Windows11Mica:** Segoe UI Variable 10.5pt, Segoe Fluent Icons
- **MacOSBigSur:** SF Pro Text 11pt
- **ChakraUI:** Segoe UI 10pt
- **TailwindCard:** Inter 10pt (fallback Segoe UI)
- **NotionMinimal:** Segoe UI 10pt
- **VercelClean:** Inter 10pt (fallback Segoe UI)
- **StripeDashboard:** Segoe UI 10.5pt
- **DarkGlow:** Consolas 11pt (monospace)
- **DiscordStyle:** Segoe UI 10.5pt
- **GradientModern:** Segoe UI 11pt Bold

---

## Visual Effects Summary

| Painter | Shadow | Glow | Gradient | Translucent |
|---------|--------|------|----------|-------------|
| Material3 | ✓ | − | − | ✓ |
| iOS15 | ✓ | − | − | ✓ |
| Fluent2 | − | ✓ | − | − |
| Minimal | − | − | − | − |
| AntDesign | − | − | − | − |
| MaterialYou | − | − | − | ✓ |
| Windows11Mica | − | − | − | ✓ |
| MacOSBigSur | ✓ | ✓ | ✓ | − |
| ChakraUI | − | ✓ | − | − |
| TailwindCard | ✓ | ✓ | − | − |
| NotionMinimal | − | − | − | − |
| VercelClean | − | − | − | − |
| StripeDashboard | ✓ | − | − | − |
| DarkGlow | ✓ | ✓✓✓ | − | − |
| DiscordStyle | ✓ | ✓ | − | − |
| GradientModern | − | ✓✓✓ | ✓✓✓ | − |

✓ = Has effect  
✓✓✓ = Multi-layer effect  
− = No effect

---

## Usage Recommendation by Industry

**Enterprise/Business:**
- Minimal, Fluent2, AntDesign, StripeDashboard

**Creative/Design:**
- GradientModern, MaterialYou, MacOSBigSur

**Gaming/Entertainment:**
- DarkGlow, DiscordStyle

**Productivity/Workspace:**
- NotionMinimal, TailwindCard, ChakraUI

**Platform-Specific:**
- iOS15 (iOS/macOS apps)
- Fluent2/Windows11Mica (Windows apps)
- Material3/MaterialYou (Android apps)

**Web/Landing Pages:**
- VercelClean, TailwindCard, ChakraUI

**Financial/Payment:**
- StripeDashboard, AntDesign

---

*Complete visual style guide for BeepNumericUpDown*  
*16 distinct painters, each with unique characteristics*  
*Generated: 2025-10-04*
