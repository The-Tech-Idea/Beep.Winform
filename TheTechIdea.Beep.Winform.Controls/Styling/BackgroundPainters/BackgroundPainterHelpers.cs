using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
  
    /// <summary>
    /// Shared helper methods for all background painters
    /// </summary>
    public static class BackgroundPainterHelpers
    {
      
        /// <summary>
        /// Lighten a color by a percentage
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
        /// Darken a color by a percentage
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
        /// Add alpha transparency to a color
        /// </summary>
        public static Color WithAlpha(Color color, int alpha)
        {
            return Color.FromArgb(Math.Max(0, Math.Min(255, alpha)), color);
        }

        /// <summary>
        /// Blend two colors
        /// </summary>
        public static Color BlendColors(Color color1, Color color2, float ratio)
        {
            ratio = Math.Max(0, Math.Min(1, ratio));
            return Color.FromArgb(
                (int)(color1.R * (1 - ratio) + color2.R * ratio),
                (int)(color1.G * (1 - ratio) + color2.G * ratio),
                (int)(color1.B * (1 - ratio) + color2.B * ratio)
            );
        }

        /// <summary>
        /// Create an inset rectangle
        /// </summary>
        public static Rectangle InsetRectangle(Rectangle rect, int inset)
        {
            return new Rectangle(
                rect.X + inset,
                rect.Y + inset,
                Math.Max(0, rect.Width - inset * 2),
                Math.Max(0, rect.Height - inset * 2)
            );
        }

        /// <summary>
        /// Apply state modification to a color
        /// </summary>
        public static Color ApplyState(Color baseColor, ControlState state)
        {
            switch (state)
            {
                case ControlState.Hovered:
                    return Lighten(baseColor, 0.05f);
                case ControlState.Pressed:
                    return Darken(baseColor, 0.1f);
                case ControlState.Selected:
                    return Lighten(baseColor, 0.08f);
                case ControlState.Disabled:
                    return WithAlpha(baseColor, 100);
                case ControlState.Focused:
                    return Lighten(baseColor, 0.03f);
                case ControlState.Normal:
                default:
                    return baseColor;
            }
        }

        /// <summary>
        /// Get overlay color for state
        /// </summary>
        public static Color GetStateOverlay(ControlState state)
        {
            switch (state)
            {
                case ControlState.Hovered:
                    return WithAlpha(Color.White, 20);
                case ControlState.Pressed:
                    return WithAlpha(Color.Black, 30);
                case ControlState.Selected:
                    return WithAlpha(Color.White, 25);
                case ControlState.Focused:
                    return WithAlpha(Color.White, 15);
                case ControlState.Disabled:
                    return WithAlpha(Color.Gray, 80);
                case ControlState.Normal:
                default:
                    return Color.Transparent;
            }
        }

        /// <summary>
        /// Convenience scope for clipping drawing operations to a graphics path.
        /// </summary>
        public readonly struct ClipScope : IDisposable
        {
            private readonly Graphics _graphics;
            private readonly GraphicsState _state;

            public ClipScope(Graphics graphics, GraphicsPath clipPath)
            {
                _graphics = graphics;
                _state = graphics.Save();
                graphics.SetClip(clipPath, CombineMode.Intersect);
            }

            public void Dispose()
            {
                _graphics.Restore(_state);
            }
        }
    }
}
