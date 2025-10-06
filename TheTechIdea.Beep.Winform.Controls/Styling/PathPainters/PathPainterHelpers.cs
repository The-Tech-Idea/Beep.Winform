using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Helper utilities for painting filled graphics paths
    /// Provides common methods for solid fills, gradients, and effects
    /// </summary>
    public static class PathPainterHelpers
    {
       

        /// <summary>
        /// Creates a rounded rectangle path
        /// </summary>
        public static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Paints a solid filled path with state support
        /// </summary>
        public static void PaintSolidPath(Graphics g, GraphicsPath path, Color fillColor, ControlState state = ControlState.Normal)
        {
            Color adjustedColor = ApplyState(fillColor, state);
            using (var fillBrush = new SolidBrush(adjustedColor))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(fillBrush, path);
            }
        }

        /// <summary>
        /// Paints a gradient filled path (vertical)
        /// </summary>
        public static void PaintGradientPath(Graphics g, GraphicsPath path, Color color1, Color color2, ControlState state = ControlState.Normal)
        {
            Color adjustedColor1 = ApplyState(color1, state);
            Color adjustedColor2 = ApplyState(color2, state);

            RectangleF bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using (var gradientBrush = new LinearGradientBrush(bounds, adjustedColor1, adjustedColor2, 90f))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(gradientBrush, path);
            }
        }

        /// <summary>
        /// Paints a gradient filled path (custom angle)
        /// </summary>
        public static void PaintGradientPath(Graphics g, GraphicsPath path, Color color1, Color color2, float angle, ControlState state = ControlState.Normal)
        {
            Color adjustedColor1 = ApplyState(color1, state);
            Color adjustedColor2 = ApplyState(color2, state);

            RectangleF bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using (var gradientBrush = new LinearGradientBrush(bounds, adjustedColor1, adjustedColor2, angle))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(gradientBrush, path);
            }
        }

        /// <summary>
        /// Paints a radial gradient filled path
        /// </summary>
        public static void PaintRadialGradientPath(Graphics g, GraphicsPath path, Color centerColor, Color edgeColor, ControlState state = ControlState.Normal)
        {
            Color adjustedCenter = ApplyState(centerColor, state);
            Color adjustedEdge = ApplyState(edgeColor, state);

            RectangleF bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using (var gradientBrush = new PathGradientBrush(path))
            {
                gradientBrush.CenterColor = adjustedCenter;
                gradientBrush.SurroundColors = new Color[] { adjustedEdge };
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(gradientBrush, path);
            }
        }

        /// <summary>
        /// Applies state-based color modifications
        /// </summary>
        public static Color ApplyState(Color baseColor, ControlState state)
        {
            switch (state)
            {
                case ControlState.Hovered:
                    return Lighten(baseColor, 0.1f);
                case ControlState.Pressed:
                    return Darken(baseColor, 0.15f);
                case ControlState.Selected:
                    return Lighten(baseColor, 0.15f);
                case ControlState.Disabled:
                    return WithAlpha(baseColor, 100);
                case ControlState.Focused:
                    return Lighten(baseColor, 0.05f);
                default:
                    return baseColor;
            }
        }

        /// <summary>
        /// Lightens a color by a percentage
        /// </summary>
        public static Color Lighten(Color color, float percent)
        {
            return Color.FromArgb(
                color.A,
                Math.Min(255, color.R + (int)(255 * percent)),
                Math.Min(255, color.G + (int)(255 * percent)),
                Math.Min(255, color.B + (int)(255 * percent))
            );
        }

        /// <summary>
        /// Darkens a color by a percentage
        /// </summary>
        public static Color Darken(Color color, float percent)
        {
            return Color.FromArgb(
                color.A,
                Math.Max(0, color.R - (int)(color.R * percent)),
                Math.Max(0, color.G - (int)(color.G * percent)),
                Math.Max(0, color.B - (int)(color.B * percent))
            );
        }

        /// <summary>
        /// Creates a new color with specified alpha
        /// </summary>
        public static Color WithAlpha(Color color, int alpha)
        {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }

        /// <summary>
        /// Creates a new color with specified RGB and alpha
        /// </summary>
        public static Color WithAlpha(int r, int g, int b, int alpha)
        {
            return Color.FromArgb(alpha, r, g, b);
        }

        /// <summary>
        /// Gets a color from style or theme
        /// </summary>
        public static Color GetColorFromStyleOrTheme(IBeepTheme theme, bool useThemeColors, string themeColorKey, Color fallbackColor)
        {
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor(themeColorKey);
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            return fallbackColor;
        }
    }
}

