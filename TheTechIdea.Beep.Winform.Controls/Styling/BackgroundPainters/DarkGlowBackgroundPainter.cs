using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Dark Glow background painter - Solid dark with 3-ring neon glow
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// Glow intensity modulated by state
    /// </summary>
    public static class DarkGlowBackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Dark Glow: Dark background with neon glow rings
            Color darkColor = Color.FromArgb(20, 20, 20);
            darkColor = BackgroundPainterHelpers.ApplyState(darkColor, state);
            Color glowColor = useThemeColors ? theme.AccentColor : Color.FromArgb(0, 255, 255);

            // Fill dark background
            using (var brush = new SolidBrush(darkColor))
            {
                if (path != null)
                    g.FillPath(brush, path);
                else
                    g.FillRectangle(brush, bounds);
            }

            // Glow intensity based on state
            float glowMultiplier = state switch
            {
                ControlState.Hovered => 1.3f,
                ControlState.Pressed => 0.7f,
                ControlState.Selected => 1.2f,
                ControlState.Disabled => 0.4f,
                ControlState.Focused => 1.1f,
                _ => 1.0f
            };

            // Glow ring 1 (80% alpha, 1px inset)
            Rectangle glow1 = BackgroundPainterHelpers.InsetRectangle(bounds, 1);
            int alpha1 = Math.Min(255, (int)(80 * glowMultiplier));
            using (var pen = new Pen(BackgroundPainterHelpers.WithAlpha(glowColor, alpha1), 1f))
            {
                if (path != null)
                {
                    using (var glowPath = BackgroundPainterHelpers.CreateRoundedRectangle(glow1, 4))
                    {
                        g.DrawPath(pen, glowPath);
                    }
                }
                else
                {
                    g.DrawRectangle(pen, glow1);
                }
            }

            // Glow ring 2 (40% alpha, 3px inset)
            Rectangle glow2 = BackgroundPainterHelpers.InsetRectangle(bounds, 3);
            int alpha2 = Math.Min(255, (int)(40 * glowMultiplier));
            using (var pen = new Pen(BackgroundPainterHelpers.WithAlpha(glowColor, alpha2), 1f))
            {
                if (path != null)
                {
                    using (var glowPath = BackgroundPainterHelpers.CreateRoundedRectangle(glow2, 4))
                    {
                        g.DrawPath(pen, glowPath);
                    }
                }
                else
                {
                    g.DrawRectangle(pen, glow2);
                }
            }

            // Glow ring 3 (20% alpha, 6px inset)
            Rectangle glow3 = BackgroundPainterHelpers.InsetRectangle(bounds, 6);
            int alpha3 = Math.Min(255, (int)(20 * glowMultiplier));
            using (var pen = new Pen(BackgroundPainterHelpers.WithAlpha(glowColor, alpha3), 1f))
            {
                if (path != null)
                {
                    using (var glowPath = BackgroundPainterHelpers.CreateRoundedRectangle(glow3, 4))
                    {
                        g.DrawPath(pen, glowPath);
                    }
                }
                else
                {
                    g.DrawRectangle(pen, glow3);
                }
            }

            // Apply state overlay
            Color stateOverlay = BackgroundPainterHelpers.GetStateOverlay(state);
            if (stateOverlay != Color.Transparent && state != ControlState.Disabled)
            {
                using (var brush = new SolidBrush(Color.FromArgb(stateOverlay.A / 3, stateOverlay.R, stateOverlay.G, stateOverlay.B)))
                {
                    if (path != null)
                        g.FillPath(brush, path);
                    else
                        g.FillRectangle(brush, bounds);
                }
            }
        }
    }
}
