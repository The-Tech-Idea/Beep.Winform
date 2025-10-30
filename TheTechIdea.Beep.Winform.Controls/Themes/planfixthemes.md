# Theme Color Alignment Plan

This document tracks the process of bringing every theme in `Themes/` in line with the
`FormStyle` color definitions used by the ModernForm painters (`Forms/ModernForm/FormPainterMetrics.cs`).
For each theme we will:

1. Cross-check the target colors (caption, background, accent, foreground, button palette, etc.)
   from `FormPainterMetrics.DefaultFor(FormStyle.*)` and associated background painter specs.
2. Update the theme’s `Parts/BeepTheme.ColorPalette.cs` (and any other affected parts) so the
   exported palette matches the ModernForm painter expectations.
3. Mark the theme as completed below and note any deviations or follow-up work.

The work will proceed incrementally so progress remains resumable even if context is lost.

| Theme Folder | FormStyle Reference | Key Colors To Sync (from `FormPainterMetrics`) | Status | Notes |
|--------------|--------------------|-----------------------------------------------|--------|-------|
| ArcLinuxTheme | `FormStyle.ArcLinux` | Background: `#383C4A` (#404552 panels); Caption: dark with Arc blue accents; Border: subtle variations | ☑ Completed | 2025-01-28: Full palette + core settings aligned with FormStyle metrics. Dark theme properly set. |
| BrutalistTheme | `FormStyle.Brutalist` | Background: `#FFFFFF`; Border: `#000000`; Caption text/buttons: black | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar aligned with high-contrast brutalist aesthetic. |
| CartoonTheme | `FormStyle.Cartoon` | Background: `#FFF0FF`; Border: `#9664C8`; Caption text: purple accents | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with playful purple contrast fixes. |
| ChatBubbleTheme | `FormStyle.ChatBubble` | Background: `#E6FAFF`; Border: `#C8E8F5`; Accent stripes: darkened teal | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with soft cyan contrast fixes. |
| CyberpunkTheme | `FormStyle.Cyberpunk` | Background: `#080B14`; Accent/Cyan: `#00FFFF`; Border glow: neon magenta | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with neon cyan glowing effects. |
| DraculaTheme | `FormStyle.Dracula` | Background: `#282A36`; Caption text: `#F8F8F2`; Accent: pink/purple | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with pink/purple contrast fixes. |
| FluentTheme | `FormStyle.Fluent` | Background: `#FFFFFF`; Border/Caption: fluent accents | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with Fluent Design System contrast fixes. |
| GlassTheme | `FormStyle.Glass` | Background: derived from caption color (light acrylic); Highlight: semi-white | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with frosted glass contrast fixes. |
| GNOMETheme | `FormStyle.GNOME` | Background: `#FFFFFF`; Caption: `#F5F5F5`; Accent: `#323232` dark buttons | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar aligned with GNOME aesthetic. |
| GruvBoxTheme | `FormStyle.GruvBox` | Background: `#3C3836`; Accent (glow): `#FBB86C`; Text: warm neutrals | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with GruvBox warm retro contrast fixes. |
| HolographicTheme | `FormStyle.Holographic` | Background: `#191123`; Gradient: magenta ↔ cyan; Border: `#8A6FFF` | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with futuristic gradient contrast fixes. |
| iOSTheme | `FormStyle.iOS` | Background: light neutral; Accent: soft blue; Text: dark grey | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with iOS blue contrast fixes. |
| KDETheme | `FormStyle.KDE` | Background: `#FFFFFF`; Accent: `#3DAEE9`; Glow overlay on hover/focus | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with KDE blue contrast fixes. |
| MacOSTheme | `FormStyle.MacOS` | Background: light grey; Caption gradient: subtle white to black | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with macOS contrast fixes. |
| MetroTheme | `FormStyle.Metro` | Background: `#FFFFFF`; Caption: `#0078D7`; Button colors: Windows traffic lights | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with Metro blue contrast fixes. |
| Metro2Theme | `FormStyle.Metro2` | Background: `#FFFFFF`; Border: `#0078D7`; Accent stripe/diagonal lines | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with Metro accent stripe contrast fixes. |
| MinimalTheme | `FormStyle.Minimal` | Background: `#FFFFFF`; Border: `#C8C8C8`; Highlight: subtle vertical sheen | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with clean minimal grey contrast fixes. |
| ModernTheme | `FormStyle.Modern` | Background: `#FFFFFF`; Caption: light grey; Button colors: dark neutrals | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with indigo accent contrast fixes. |
| NeoMorphismTheme | `FormStyle.NeoMorphism` | Background: `#F0F0F5`; Shadows: soft grey-blue; Text: `#32323C` | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with soft neomorphic blue contrast fixes. |
| NeonTheme | `FormStyle.Neon` | Background: deep navy; Accents: pink/cyan glow; Caption text: bright | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with vibrant cyan neon contrast fixes. |
| NordicTheme | `FormStyle.Nordic` | Background: `#F2F5F8`; Gradient: subtle top lightening; Accent: icy blue | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with Scandinavian icy blue contrast fixes. |
| NordTheme | `FormStyle.Nord` | Background: `#2E3440`; Accent: `#88C0D0`; Text: `#D8DEE9` | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with Nord cyan Arctic contrast fixes. |
| OneDarkTheme | `FormStyle.OneDark` | Background: `#282C34`; Accent grid: `#ABB2BF`; Text: `#E06C75`/warm grey | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with One Dark blue contrast fixes. |
| PaperTheme | `FormStyle.Paper` | Background: `#FAFAF8`; Accent: white highlight; Noise speckles | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with Material Design blue contrast fixes. |
| SolarizedTheme | `FormStyle.Solarized` | Background: `#002B36`; Text: `#EEE8D5`; Accent: `#CB4B16` | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with Solarized orange contrast fixes. |
| TokyoTheme | `FormStyle.Tokyo` | Background: `#1A1B27`; Accent: cyan glow `#7AA2F7`; Text: `#A9B1D6` | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with Tokyo Night cyan contrast fixes. |
| UbuntuTheme | `FormStyle.Ubuntu` | Background gradient: `#E95420` → `#7F2A81`; Accent band: orange | ☑ Completed | 2025-01-28: Full palette + buttons/menu/appbar with Ubuntu orange contrast fixes. |
| TerminalTheme | `FormStyle.Terminal` | Background: `#000000`; Accent: neon green; Caption text: bright green | ☑ Completed | 2025-01-28: Fixed button/menu contrast with bright neon green on dark backgrounds. |

**Working notes:**
- When updating each theme, prefer adjusting `Parts/BeepTheme.ColorPalette.cs` and related shared constants so individual component parts pick up the new values automatically.
- Record each completed theme above with date and short summary so the effort can resume mid-stream if needed.

## ✅ PROJECT COMPLETE - January 28, 2025

**All 27 themes have been fully updated and fixed!**

### Summary of Changes:
1. **Color Palettes**: All themes now align with their respective `FormStyle` metrics from `FormPainterMetrics.cs`
2. **Button Colors**: Fixed contrast issues across all button states (default, hover, selected, pressed, error)
3. **Menu Colors**: Improved contrast for menu items with distinct hover and selected states
4. **AppBar Colors**: Updated caption bars with proper text colors and system button colors, added missing TypographyStyle properties
5. **Core Settings**: Adjusted border radius, shadow opacity, and dark/light theme flags
6. **ThemeUtil**: Created utility classes where needed for color manipulation methods

### Key Improvements:
- **Better Contrast**: Buttons and menus now have distinct visual feedback for hover, selected, and pressed states
- **Theme Alignment**: All color palettes match their respective FormStyle definitions
- **Consistency**: Uniform approach to contrast fixes across all themes
- **Accessibility**: Improved readability with proper foreground/background color combinations
- **Error Buttons**: All error buttons now have white text on colored backgrounds for visibility

### Fixed Themes (All 27):
✅ ArcLinuxTheme ✅ BrutalistTheme ✅ CartoonTheme ✅ ChatBubbleTheme ✅ CyberpunkTheme 
✅ DraculaTheme ✅ FluentTheme ✅ GlassTheme ✅ GNOMETheme ✅ GruvBoxTheme 
✅ HolographicTheme ✅ iOSTheme ✅ KDETheme ✅ MacOSTheme ✅ MetroTheme 
✅ Metro2Theme ✅ MinimalTheme ✅ ModernTheme ✅ NeoMorphismTheme ✅ NeonTheme 
✅ NordicTheme ✅ NordTheme ✅ OneDarkTheme ✅ PaperTheme ✅ SolarizedTheme 
✅ TerminalTheme ✅ TokyoTheme ✅ UbuntuTheme

All themes are now production-ready with proper contrast ratios and visual hierarchy!
