$painters = @(
    @{
        Name = "Fluent2BorderPainter"
        Comment = "Fluent2 border painter - 4px accent bar on left when focused + 1px border"
        FocusedColor = "Color.FromArgb(0, 120, 212)"  # Fluent blue
        NormalColor = "Color.FromArgb(96, 94, 92)"
        Special = "AccentBar"
    },
    @{
        Name = "Windows11MicaBorderPainter"
        Comment = "Windows11Mica border painter - Subtle 1px border with mica effect"
        FocusedColor = "Color.FromArgb(0, 120, 212)"
        NormalColor = "Color.FromArgb(96, 94, 92)"
        Special = "None"
    },
    @{
        Name = "MinimalBorderPainter"
        Comment = "Minimal border painter - Simple 1px border, always visible"
        FocusedColor = "Color.FromArgb(64, 64, 64)"
        NormalColor = "Color.FromArgb(224, 224, 224)"
        Special = "None"
    },
    @{
        Name = "NotionMinimalBorderPainter"
        Comment = "NotionMinimal border painter - Very subtle 1px border"
        FocusedColor = "Color.FromArgb(55, 53, 47)"
        NormalColor = "Color.FromArgb(227, 226, 224)"
        Special = "None"
    },
    @{
        Name = "VercelCleanBorderPainter"
        Comment = "VercelClean border painter - Clean 1px border"
        FocusedColor = "Color.FromArgb(0, 0, 0)"
        NormalColor = "Color.FromArgb(234, 234, 234)"
        Special = "None"
    },
    @{
        Name = "NeumorphismBorderPainter"
        Comment = "Neumorphism border painter - No visible border (embossed effect from background)"
        FocusedColor = "Color.Transparent"
        NormalColor = "Color.Transparent"
        Special = "None"
    },
    @{
        Name = "GlassAcrylicBorderPainter"
        Comment = "GlassAcrylic border painter - Frosted glass with subtle translucent border"
        FocusedColor = "BorderPainterHelpers.WithAlpha(255, 255, 255, 80)"
        NormalColor = "BorderPainterHelpers.WithAlpha(255, 255, 255, 60)"
        Special = "None"
    },
    @{
        Name = "DarkGlowBorderPainter"
        Comment = "DarkGlow border painter - Cyan glow border effect"
        FocusedColor = "Color.FromArgb(0, 255, 255)"
        NormalColor = "Color.FromArgb(0, 255, 255)"
        Special = "Glow"
    },
    @{
        Name = "GradientModernBorderPainter"
        Comment = "GradientModern border painter - 1px border matching gradient theme"
        FocusedColor = "Color.FromArgb(99, 102, 241)"
        NormalColor = "Color.FromArgb(148, 163, 184)"
        Special = "None"
    },
    @{
        Name = "BootstrapBorderPainter"
        Comment = "Bootstrap border painter - Bootstrap primary blue 1px border"
        FocusedColor = "Color.FromArgb(13, 110, 253)"
        NormalColor = "Color.FromArgb(206, 212, 218)"
        Special = "None"
    },
    @{
        Name = "TailwindCardBorderPainter"
        Comment = "TailwindCard border painter - 1px border + ring effect on focus"
        FocusedColor = "Color.FromArgb(59, 130, 246)"
        NormalColor = "Color.FromArgb(229, 231, 235)"
        Special = "Ring"
    },
    @{
        Name = "StripeDashboardBorderPainter"
        Comment = "StripeDashboard border painter - Subtle 1px border"
        FocusedColor = "Color.FromArgb(99, 91, 255)"
        NormalColor = "Color.FromArgb(225, 225, 225)"
        Special = "None"
    },
    @{
        Name = "FigmaCardBorderPainter"
        Comment = "FigmaCard border painter - Figma blue 1px border"
        FocusedColor = "Color.FromArgb(24, 160, 251)"
        NormalColor = "Color.FromArgb(227, 227, 227)"
        Special = "None"
    },
    @{
        Name = "DiscordStyleBorderPainter"
        Comment = "DiscordStyle border painter - Discord blurple 1px border"
        FocusedColor = "Color.FromArgb(88, 101, 242)"
        NormalColor = "Color.FromArgb(66, 70, 77)"
        Special = "None"
    },
    @{
        Name = "AntDesignBorderPainter"
        Comment = "AntDesign border painter - Ant blue 1px border"
        FocusedColor = "Color.FromArgb(24, 144, 255)"
        NormalColor = "Color.FromArgb(217, 217, 217)"
        Special = "None"
    },
    @{
        Name = "ChakraUIBorderPainter"
        Comment = "ChakraUI border painter - Chakra teal 1px border"
        FocusedColor = "Color.FromArgb(49, 151, 149)"
        NormalColor = "Color.FromArgb(226, 232, 240)"
        Special = "None"
    },
    @{
        Name = "PillRailBorderPainter"
        Comment = "PillRail border painter - Soft 1px border for pill-shaped controls"
        FocusedColor = "Color.FromArgb(107, 114, 128)"
        NormalColor = "Color.FromArgb(229, 231, 235)"
        Special = "None"
    }
)

foreach ($painter in $painters) {
    $content = @"
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Template;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// $($painter.Comment)
    /// </summary>
    public static class $($painter.Name)
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            BorderPainterHelpers.ControlState state = BorderPainterHelpers.ControlState.Normal)
        {
"@

    if ($painter.Special -eq "AccentBar") {
        $content += @"

            // Paint border first
            Color borderColor = isFocused
                ? BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", $($painter.FocusedColor))
                : BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", $($painter.NormalColor));

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, 1f, state);

            // Paint accent bar on focus
            if (isFocused)
            {
                var bounds = path.GetBounds();
                Color accentColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "AccentColor", $($painter.FocusedColor));
                BorderPainterHelpers.PaintAccentBar(g, Rectangle.Round(bounds), accentColor, 4);
            }
"@
    }
    elseif ($painter.Special -eq "Glow") {
        $content += @"

            // DarkGlow uses glow effect instead of solid border
            Color glowColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "AccentColor", $($painter.FocusedColor));
            float glowIntensity = isFocused ? 1.2f : 0.8f;
            
            BorderPainterHelpers.PaintGlowBorder(g, path, glowColor, 2f, glowIntensity);
"@
    }
    elseif ($painter.Special -eq "Ring") {
        $content += @"

            // Paint border first
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", $($painter.NormalColor));
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, 1f, state);

            // Paint ring effect on focus
            if (isFocused)
            {
                Color ringColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", $($painter.FocusedColor));
                Color translucentRing = BorderPainterHelpers.WithAlpha(ringColor, 60);
                BorderPainterHelpers.PaintRing(g, path, translucentRing, 3f, 2f);
            }
"@
    }
    else {
        $content += @"

            Color borderColor = isFocused
                ? BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", $($painter.FocusedColor))
                : BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", $($painter.NormalColor));

            float borderWidth = ($($painter.FocusedColor) -eq "Color.Transparent") ? 0f : 1f;
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);
"@
    }

    $content += @"

        }
    }
}
"@

    $filePath = "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Styling\BorderPainters\$($painter.Name).cs"
    $content | Out-File -FilePath $filePath -Encoding UTF8
    Write-Host "Created $($painter.Name).cs"
}

Write-Host "`nAll 17 border painters created successfully!"
