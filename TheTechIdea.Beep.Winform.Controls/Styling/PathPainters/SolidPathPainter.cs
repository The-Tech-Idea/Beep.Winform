using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// LEGACY: Path painter dispatcher - delegates to individual Style painters
    /// This class is maintained for backward compatibility
    /// Use individual painters (Material3PathPainter, iOS15PathPainter, etc.) directly for better maintainability
    /// </summary>
    [System.Obsolete("Use individual PathPainters (Material3PathPainter, iOS15PathPainter, etc.) instead", false)]
    public static class SolidPathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Paint(g, bounds, radius, style, theme, useThemeColors, ControlState.Normal);
        }

        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            switch (style)
            {
                case BeepControlStyle.Material3:
                    Material3PathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.MaterialYou:
                    MaterialYouPathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.iOS15:
                    iOS15PathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.MacOSBigSur:
                    MacOSBigSurPathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.Fluent2:
                    Fluent2PathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.Windows11Mica:
                    Windows11MicaPathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.Minimal:
                    MinimalPathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.NotionMinimal:
                    NotionMinimalPathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.VercelClean:
                    VercelCleanPathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.Neumorphism:
                    NeumorphismPathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.GlassAcrylic:
                    GlassAcrylicPathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.DarkGlow:
                    DarkGlowPathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.GradientModern:
                    GradientModernPathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.Bootstrap:
                    BootstrapPathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.TailwindCard:
                    TailwindCardPathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.StripeDashboard:
                    StripeDashboardPathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.FigmaCard:
                    FigmaCardPathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.DiscordStyle:
                    DiscordStylePathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.AntDesign:
                    AntDesignPathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.ChakraUI:
                    ChakraUIPathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.PillRail:
                    PillRailPathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
                default:
                    Material3PathPainter.Paint(g, bounds, radius, style, theme, useThemeColors, state);
                    break;
            }
        }
    }
}

