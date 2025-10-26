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
| ArcLinuxTheme | `FormStyle.ArcLinux` | Background: `#181A1F`; Border/Caption highlight: subtle white top line | ☐ In Progress | Palette adjusted; propagate through component parts next. |
| BrutalistTheme | `FormStyle.Brutalist` | Background: `#FFFFFF`; Border: `#000000`; Caption text/buttons: black | ☐ In Progress | Palette flattened; update component parts. |
| CartoonTheme | `FormStyle.Cartoon` | Background: `#FFF0FF`; Border: `#9664C8`; Caption text: purple accents | ☐ Pending | |
| ChatBubbleTheme | `FormStyle.ChatBubble` | Background: `#E6FAFF`; Border: `#C8E8F5`; Accent stripes: darkened teal | ☐ Pending | |
| CyberpunkTheme | `FormStyle.Cyberpunk` | Background: `#080B14`; Accent/Cyan: `#00FFFF`; Border glow: neon magenta | ☐ Pending | |
| DraculaTheme | `FormStyle.Dracula` | Background: `#282A36`; Caption text: `#F8F8F2`; Accent: pink/purple | ☐ Pending | |
| FluentTheme | `FormStyle.Fluent` | Background: `#FFFFFF`; Border/Caption: fluent accents | ☐ Pending | Align legacy fluent palette. |
| GlassTheme | `FormStyle.Glass` | Background: derived from caption color (light acrylic); Highlight: semi-white | ☐ Pending | |
| GNOMETheme | `FormStyle.GNOME` | Background: `#FFFFFF`; Caption: `#F5F5F5`; Accent: `#323232` dark buttons | ☐ Pending | |
| GruvBoxTheme | `FormStyle.GruvBox` | Background: `#3C3836`; Accent (glow): `#FBB86C`; Text: warm neutrals | ☐ Pending | |
| HolographicTheme | `FormStyle.Holographic` | Background: `#191123`; Gradient: magenta ↔ cyan; Border: `#8A6FFF` | ☐ Pending | |
| iOSTheme | `FormStyle.iOS` | Background: light neutral; Accent: soft blue; Text: dark grey | ☐ Pending | |
| KDETheme | `FormStyle.KDE` | Background: `#FFFFFF`; Accent: `#3DAEE9`; Glow overlay on hover/focus | ☐ Pending | |
| MacOSTheme | `FormStyle.MacOS` | Background: light grey; Caption gradient: subtle white to black | ☐ Pending | |
| MetroTheme | `FormStyle.Metro` | Background: `#FFFFFF`; Caption: `#0078D7`; Button colors: Windows traffic lights | ☐ Pending | |
| Metro2Theme | `FormStyle.Metro2` | Background: `#FFFFFF`; Border: `#0078D7`; Accent stripe/diagonal lines | ☐ Pending | |
| MinimalTheme | `FormStyle.Minimal` | Background: `#FFFFFF`; Border: `#C8C8C8`; Highlight: subtle vertical sheen | ☑ Completed | 2025-10-24: Full palette + component pass (buttons/app bar/cards/forms/nav/etc.) aligned with FormStyle metrics. |
| ModernTheme | `FormStyle.Modern` | Background: `#FFFFFF`; Caption: light grey; Button colors: dark neutrals | ☐ Pending | |
| NeoMorphismTheme | `FormStyle.NeoMorphism` | Background: `#F0F0F5`; Shadows: soft grey-blue; Text: `#32323C` | ☐ Pending | |
| NeonTheme | `FormStyle.Neon` | Background: deep navy; Accents: pink/cyan glow; Caption text: bright | ☐ In Progress | Neon background glow aligned (2025-10-24). Theme palette TBD. |
| NordicTheme | `FormStyle.Nordic` | Background: `#F2F5F8`; Gradient: subtle top lightening; Accent: icy blue | ☐ Pending | |
| NordTheme | `FormStyle.Nord` | Background: `#2E3440`; Accent: `#88C0D0`; Text: `#D8DEE9` | ☐ Pending | |
| OneDarkTheme | `FormStyle.OneDark` | Background: `#282C34`; Accent grid: `#ABB2BF`; Text: `#E06C75`/warm grey | ☐ Pending | |
| PaperTheme | `FormStyle.Paper` | Background: `#FAFAF8`; Accent: white highlight; Noise speckles | ☐ Pending | |
| SolarizedTheme | `FormStyle.Solarized` | Background: `#002B36`; Text: `#EEE8D5`; Accent: `#CB4B16` | ☐ Pending | |
| TokyoTheme | `FormStyle.Tokyo` | Background: `#1A1B27`; Accent: cyan glow `#7AA2F7`; Text: `#A9B1D6` | ☐ Pending | |
| UbuntuTheme | `FormStyle.Ubuntu` | Background gradient: `#E95420` → `#7F2A81`; Accent band: orange | ☐ Pending | |
| TerminalTheme | `FormStyle.Terminal` | Background: `#000000`; Accent: neon green; Caption text: bright green | ☑ Completed | Matched with ModernForm + background painter on 2025-10-24. |

**Working notes:**
- When updating each theme, prefer adjusting `Parts/BeepTheme.ColorPalette.cs` and related shared constants so individual component parts pick up the new values automatically.
- Record each completed theme above with date and short summary so the effort can resume mid-stream if needed.
