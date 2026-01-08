using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Material Design 2 shadow painter
    /// Classic Material elevation with key + ambient shadows
    /// State-aware for proper interactive feedback
    /// </summary>
    public static class MaterialShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Adjust elevation based on state (Material UX pattern)
            int effectiveLevel = (int)elevation;
            switch (state)
            {
                case ControlState.Hovered:
                    effectiveLevel = Math.Min(5, effectiveLevel + 2); // More elevation on hover
                    break;
                case ControlState.Pressed:
                    effectiveLevel = Math.Max(1, effectiveLevel); // Keep some elevation when pressed
                    break;
                case ControlState.Focused:
                    effectiveLevel = Math.Min(5, effectiveLevel + 1);
                    break;
                case ControlState.Selected:
                    effectiveLevel = Math.Min(5, effectiveLevel + 1);
                    break;
                case ControlState.Disabled:
                    effectiveLevel = 0;
                    break;
            }

            if (effectiveLevel == 0) return path;

            // Get shadow color - use darker theme color instead of pure black
            Color shadowColor = StyleShadows.GetShadowColor(style);
            if (useThemeColors && theme?.ShadowColor != null && theme.ShadowColor != Color.Empty)
            {
                shadowColor = theme.ShadowColor;
            }
            else
            {
                // Use darker gray for more realistic shadows
                shadowColor = Color.FromArgb(30, 30, 30);
            }

            // Selected state: Add accent color tint to shadow
            if (state == ControlState.Selected && useThemeColors && theme != null)
            {
                Color accentColor = theme.AccentColor;
                if (accentColor != Color.Empty)
                {
                    // Blend accent into shadow for selected indication
                    int r = (int)(shadowColor.R * 0.7 + accentColor.R * 0.3);
                    int gr = (int)(shadowColor.G * 0.7 + accentColor.G * 0.3);
                    int b = (int)(shadowColor.B * 0.7 + accentColor.B * 0.3);
                    shadowColor = Color.FromArgb(r, gr, b);
                }
            }

            // Use dual-layer shadow for authentic Material elevation
            return ShadowPainterHelpers.PaintDualLayerShadow(
                g, path, radius,
                effectiveLevel,
                shadowColor);
        }
    }
}
