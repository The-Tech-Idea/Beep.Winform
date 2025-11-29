using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Material You shadow painter
    /// Material 3 elevation with dynamic color adaptation
    /// Shadows can be tinted by surface color for cohesive feel
    /// </summary>
    public static class MaterialYouShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Adjust elevation based on state
            int effectiveLevel = (int)elevation;
            switch (state)
            {
                case ControlState.Hovered:
                    effectiveLevel = Math.Min(5, effectiveLevel + 1);
                    break;
                case ControlState.Pressed:
                    effectiveLevel = Math.Max(0, effectiveLevel - 1);
                    break;
                case ControlState.Focused:
                    effectiveLevel = Math.Min(5, effectiveLevel + 1);
                    break;
                case ControlState.Disabled:
                    effectiveLevel = 0;
                    break;
            }

            if (effectiveLevel == 0) return path;

            // Material You: Dynamic shadow color from theme
            Color shadowColor = StyleShadows.GetShadowColor(style);
            
            // Adapt shadow color for Material You dynamic theming
            if (useThemeColors && theme != null)
            {
                // Material You can tint shadows slightly with surface color
                Color surfaceColor = theme.SurfaceColor;
                if (surfaceColor != Color.Empty)
                {
                    // Blend a hint of surface color into shadow for cohesion
                    int r = (int)(shadowColor.R * 0.85 + surfaceColor.R * 0.15);
                    int gr = (int)(shadowColor.G * 0.85 + surfaceColor.G * 0.15);
                    int b = (int)(shadowColor.B * 0.85 + surfaceColor.B * 0.15);
                    shadowColor = Color.FromArgb(r, gr, b);
                }
            }

            // Use dual-layer shadow for proper Material elevation
            return ShadowPainterHelpers.PaintDualLayerShadow(
                g, path, radius,
                effectiveLevel,
                shadowColor);
        }
    }
}
