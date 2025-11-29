using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Material Design 3 shadow painter
    /// Authentic Material 3 elevation with key + ambient shadows
    /// State-aware elevation changes for proper UX feedback
    /// </summary>
    public static class Material3ShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Adjust elevation based on state (Material 3 UX pattern)
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

            // Get shadow color from style/theme
            Color shadowColor = StyleShadows.GetShadowColor(style);
            if (useThemeColors && theme?.ShadowColor != null && theme.ShadowColor != Color.Empty)
            {
                shadowColor = theme.ShadowColor;
            }

            // Use dual-layer shadow for proper Material 3 elevation
            return ShadowPainterHelpers.PaintDualLayerShadow(
                g, path, radius,
                effectiveLevel,
                shadowColor);
        }
    }
}
