# ThemeFixColor Plan (Per Theme, No Helpers)

Rules:
- No `ThemeUtil.Lighten`, `ThemeUtil.Darken`, `ThemeUtil.Blend`
- No contrast helper, no batch runtime logic
- Use inline `Color.FromArgb(...)` values in each theme file
- Keep each theme distinct by deriving values from that theme's own palette/core files

Execution List:
1. ArcLinuxTheme
- `Themes/ArcLinuxTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/ArcLinuxTheme/Parts/BeepTheme.TextBox.cs`
2. BrutalistTheme
- `Themes/BrutalistTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/BrutalistTheme/Parts/BeepTheme.TextBox.cs`
3. CartoonTheme
- `Themes/CartoonTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/CartoonTheme/Parts/BeepTheme.TextBox.cs`
4. ChatBubbleTheme
- `Themes/ChatBubbleTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/ChatBubbleTheme/Parts/BeepTheme.TextBox.cs`
5. CyberpunkTheme
- `Themes/CyberpunkTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/CyberpunkTheme/Parts/BeepTheme.TextBox.cs`
6. DraculaTheme
- `Themes/DraculaTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/DraculaTheme/Parts/BeepTheme.TextBox.cs`
7. FluentTheme
- `Themes/FluentTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/FluentTheme/Parts/BeepTheme.TextBox.cs`
8. GlassTheme
- `Themes/GlassTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/GlassTheme/Parts/BeepTheme.TextBox.cs`
9. GNOMETheme
- `Themes/GNOMETheme/Parts/BeepTheme.Buttons.cs`
- `Themes/GNOMETheme/Parts/BeepTheme.TextBox.cs`
10. GruvBoxTheme
- `Themes/GruvBoxTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/GruvBoxTheme/Parts/BeepTheme.TextBox.cs`
11. HolographicTheme
- `Themes/HolographicTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/HolographicTheme/Parts/BeepTheme.TextBox.cs`
12. iOSTheme
- `Themes/iOSTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/iOSTheme/Parts/BeepTheme.TextBox.cs`
13. KDETheme
- `Themes/KDETheme/Parts/BeepTheme.Buttons.cs`
- `Themes/KDETheme/Parts/BeepTheme.TextBox.cs`
14. MacOSTheme
- `Themes/MacOSTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/MacOSTheme/Parts/BeepTheme.TextBox.cs`
15. Metro2Theme
- `Themes/Metro2Theme/Parts/BeepTheme.Buttons.cs`
- `Themes/Metro2Theme/Parts/BeepTheme.TextBox.cs`
16. MetroTheme
- `Themes/MetroTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/MetroTheme/Parts/BeepTheme.TextBox.cs`
17. MinimalTheme
- `Themes/MinimalTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/MinimalTheme/Parts/BeepTheme.TextBox.cs`
18. NeoMorphismTheme
- `Themes/NeoMorphismTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/NeoMorphismTheme/Parts/BeepTheme.TextBox.cs`
19. NeonTheme
- `Themes/NeonTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/NeonTheme/Parts/BeepTheme.TextBox.cs`
20. NordicTheme
- `Themes/NordicTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/NordicTheme/Parts/BeepTheme.TextBox.cs`
21. NordTheme
- `Themes/NordTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/NordTheme/Parts/BeepTheme.TextBox.cs`
22. OneDarkTheme
- `Themes/OneDarkTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/OneDarkTheme/Parts/BeepTheme.TextBox.cs`
23. PaperTheme
- `Themes/PaperTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/PaperTheme/Parts/BeepTheme.TextBox.cs`
24. SolarizedTheme
- `Themes/SolarizedTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/SolarizedTheme/Parts/BeepTheme.TextBox.cs`
25. TokyoTheme
- `Themes/TokyoTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/TokyoTheme/Parts/BeepTheme.TextBox.cs`
26. UbuntuTheme
- `Themes/UbuntuTheme/Parts/BeepTheme.Buttons.cs`
- `Themes/UbuntuTheme/Parts/BeepTheme.TextBox.cs`
