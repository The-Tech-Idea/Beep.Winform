using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Shared helper methods for all background painters
    /// Provides consistent state handling, color manipulation, and painting utilities
    /// </summary>
    public static class BackgroundPainterHelpers
    {
        #region Enums

        /// <summary>
        /// Intensity level for state color adjustments
        /// </summary>
        public enum StateIntensity
        {
            Subtle,    // 2-3% changes - for minimal designs
            Normal,    // 5% changes - standard
            Strong     // 8-10% changes - for bold designs
        }

        /// <summary>
        /// Side for accent stripe placement
        /// </summary>
        public enum StripeSide
        {
            Left,
            Right,
            Top,
            Bottom
        }

        #endregion

        #region Color Resolution

        /// <summary>
        /// Get color from style or theme with fallback
        /// </summary>
        public static Color GetColorFromStyleOrTheme(IBeepTheme theme, bool useThemeColors, 
            string themeColorKey, Color fallbackColor)
        {
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor(themeColorKey);
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            return fallbackColor;
        }

        /// <summary>
        /// Get color from style function or theme
        /// </summary>
        public static Color GetColor(BeepControlStyle style, Func<BeepControlStyle, Color> styleColorFunc, 
            string themeColorKey, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor(themeColorKey);
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            return styleColorFunc(style);
        }

        #endregion

        #region Color Manipulation

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
        /// Add alpha transparency to a color unless the base color is Color.Empty
        /// </summary>
        public static Color WithAlphaIfNotEmpty(Color color, int alpha)
        {
            if (color == Color.Empty) return Color.Empty;
            return WithAlpha(color, alpha);
        }

        /// <summary>
        /// Blend two colors by ratio (0.0 = color1, 1.0 = color2)
        /// </summary>
        public static Color BlendColors(Color color1, Color color2, float ratio)
        {
            ratio = Math.Max(0, Math.Min(1, ratio));
            return Color.FromArgb(
                (int)(color1.A * (1 - ratio) + color2.A * ratio),
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

        #endregion

        #region State Handling

        /// <summary>
        /// Get state-adjusted color with configurable intensity
        /// </summary>
        public static Color GetStateAdjustedColor(Color baseColor, ControlState state, 
            StateIntensity intensity = StateIntensity.Normal)
        {
            // Get adjustment percentages based on intensity
            float hoverAdjust, pressAdjust, selectAdjust, focusAdjust;
            int disabledAlpha;

            switch (intensity)
            {
                case StateIntensity.Subtle:
                    hoverAdjust = 0.02f;
                    pressAdjust = 0.04f;
                    selectAdjust = 0.03f;
                    focusAdjust = 0.015f;
                    disabledAlpha = 120;
                    break;
                case StateIntensity.Strong:
                    hoverAdjust = 0.08f;
                    pressAdjust = 0.12f;
                    selectAdjust = 0.10f;
                    focusAdjust = 0.05f;
                    disabledAlpha = 80;
                    break;
                default: // Normal
                    hoverAdjust = 0.05f;
                    pressAdjust = 0.08f;
                    selectAdjust = 0.06f;
                    focusAdjust = 0.03f;
                    disabledAlpha = 100;
                    break;
            }

            return state switch
            {
                ControlState.Hovered => Lighten(baseColor, hoverAdjust),
                ControlState.Pressed => Darken(baseColor, pressAdjust),
                ControlState.Selected => Lighten(baseColor, selectAdjust),
                ControlState.Focused => Lighten(baseColor, focusAdjust),
                ControlState.Disabled => WithAlpha(baseColor, disabledAlpha),
                _ => baseColor
            };
        }

        /// <summary>
        /// Apply state modification to a color (legacy - use GetStateAdjustedColor for new code)
        /// </summary>
        public static Color ApplyState(Color baseColor, ControlState state)
        {
            return GetStateAdjustedColor(baseColor, state, StateIntensity.Normal);
        }

        /// <summary>
        /// Get overlay color for state (for overlay painting approach)
        /// </summary>
        public static Color GetStateOverlay(ControlState state)
        {
            return state switch
            {
                ControlState.Hovered => WithAlpha(Color.White, 20),
                ControlState.Pressed => WithAlpha(Color.Black, 30),
                ControlState.Selected => WithAlpha(Color.White, 25),
                ControlState.Focused => WithAlpha(Color.White, 15),
                ControlState.Disabled => WithAlpha(Color.Gray, 80),
                _ => Color.Transparent
            };
        }

        #endregion

        #region Background Painting Methods

        /// <summary>
        /// Paint a solid background with state awareness
        /// Best for: Flat designs (Metro, Minimal, Brutalist)
        /// </summary>
        public static void PaintSolidBackground(Graphics g, GraphicsPath path,
            Color baseColor, ControlState state, StateIntensity intensity = StateIntensity.Normal)
        {
            if (g == null || path == null) return;

            Color fillColor = GetStateAdjustedColor(baseColor, state, intensity);
            var brush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(brush, path);
        }

        /// <summary>
        /// Paint a linear gradient background with state awareness
        /// Best for: Modern designs with depth (Material, Apple)
        /// </summary>
        public static void PaintGradientBackground(Graphics g, GraphicsPath path,
            Color startColor, Color endColor, LinearGradientMode mode, ControlState state,
            StateIntensity intensity = StateIntensity.Normal)
        {
            if (g == null || path == null) return;

            Color fillStart = GetStateAdjustedColor(startColor, state, intensity);
            Color fillEnd = GetStateAdjustedColor(endColor, state, intensity);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            var brush = PaintersFactory.GetLinearGradientBrush(bounds, fillStart, fillEnd, mode);
            g.FillPath(brush, path);
        }

        /// <summary>
        /// Paint a subtle gradient overlay on a solid background
        /// Best for: Refined designs (Modern, Apple, Fluent)
        /// </summary>
        /// <param name="gradientStrength">0.0 to 1.0, where 0.05-0.1 is typical</param>
        public static void PaintSubtleGradientBackground(Graphics g, GraphicsPath path,
            Color baseColor, float gradientStrength, ControlState state,
            StateIntensity intensity = StateIntensity.Normal)
        {
            if (g == null || path == null) return;

            // Paint solid base first
            Color fillColor = GetStateAdjustedColor(baseColor, state, intensity);
            var brush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(brush, path);

            // Add subtle gradient overlay
            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            int overlayAlpha = (int)(255 * gradientStrength);
            var overlay = PaintersFactory.GetLinearGradientBrush(
                bounds,
                Color.FromArgb(overlayAlpha, Color.White),
                Color.FromArgb(0, Color.White),
                LinearGradientMode.Vertical);
            g.FillPath(overlay, path);
        }

        /// <summary>
        /// Paint a frosted glass background effect
        /// Best for: Glassmorphism, Acrylic, Mica styles
        /// </summary>
        public static void PaintFrostedGlassBackground(Graphics g, GraphicsPath path,
            Color baseColor, int baseAlpha, ControlState state)
        {
            if (g == null || path == null) return;

            // Apply state to base color
            Color stateColor = GetStateAdjustedColor(baseColor, state, StateIntensity.Subtle);
            Color fillColor = WithAlpha(stateColor, baseAlpha);

            var brush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(brush, path);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Add frost pattern using clipping
            using (var clip = new ClipScope(g, path))
            {
                // Dotted pattern for frost simulation
                using (var frostBrush = new HatchBrush(HatchStyle.DottedGrid, 
                    Color.FromArgb(15, Color.White), Color.Transparent))
                {
                    g.FillRectangle(frostBrush, Rectangle.Round(bounds));
                }
            }

            // Top highlight for glass reflection
            var topRect = new RectangleF(bounds.Left, bounds.Top, bounds.Width, bounds.Height / 3f);
            var highlight = PaintersFactory.GetLinearGradientBrush(
                topRect,
                Color.FromArgb(35, Color.White),
                Color.Transparent,
                LinearGradientMode.Vertical);
            
            using (var clip = new ClipScope(g, path))
            {
                g.FillRectangle(highlight, topRect);
            }
        }

        /// <summary>
        /// Paint a neumorphic (soft 3D) background
        /// Best for: Neumorphism style
        /// </summary>
        public static void PaintNeumorphicBackground(Graphics g, GraphicsPath path,
            Color baseColor, ControlState state)
        {
            if (g == null || path == null) return;

            // Apply subtle state adjustment
            Color fillColor = GetStateAdjustedColor(baseColor, state, StateIntensity.Subtle);
            var brush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(brush, path);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Determine highlight color based on state
            Color highlightColor;
            if (state == ControlState.Pressed)
            {
                // Pressed: inner shadow effect (darken top)
                highlightColor = Darken(fillColor, 0.08f);
            }
            else
            {
                // Normal/Hover: inner highlight (lighten top)
                highlightColor = Lighten(fillColor, 0.08f);
            }

            // Paint top-half highlight/shadow
            using (var insetPath = GraphicsExtensions.CreateInsetPath(path, 2))
            {
                if (insetPath.PointCount > 0)
                {
                    var insetBounds = insetPath.GetBounds();
                    using (var highlightRegion = new Region(insetPath))
                    using (var clipRect = new GraphicsPath())
                    {
                        clipRect.AddRectangle(new RectangleF(
                            insetBounds.X, insetBounds.Y,
                            insetBounds.Width, insetBounds.Height / 2));
                        
                        highlightRegion.Intersect(clipRect);
                        g.SetClip(highlightRegion, CombineMode.Replace);
                        
                        var highlightBrush = PaintersFactory.GetSolidBrush(
                            WithAlpha(highlightColor, 60));
                        g.FillPath(highlightBrush, insetPath);
                        
                        g.ResetClip();
                    }
                }
            }
        }

        #endregion

        #region Overlay Effects

        /// <summary>
        /// Paint scanline overlay (Terminal/Retro effect)
        /// </summary>
        public static void PaintScanlineOverlay(Graphics g, Rectangle bounds, 
            Color lineColor, int spacing = 2)
        {
            if (g == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            var pen = PaintersFactory.GetPen(lineColor, 1f);
            for (int y = bounds.Top; y < bounds.Bottom; y += spacing)
            {
                g.DrawLine(pen, bounds.Left, y, bounds.Right, y);
            }

            g.SmoothingMode = prevMode;
        }

        /// <summary>
        /// Paint grid overlay (Terminal/Tech effect)
        /// </summary>
        public static void PaintGridOverlay(Graphics g, Rectangle bounds,
            Color lineColor, int gridSize = 20)
        {
            if (g == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            var pen = PaintersFactory.GetPen(lineColor, 1f);
            
            // Vertical lines
            for (int x = bounds.Left; x < bounds.Right; x += gridSize)
            {
                g.DrawLine(pen, x, bounds.Top, x, bounds.Bottom);
            }
            
            // Horizontal lines
            for (int y = bounds.Top; y < bounds.Bottom; y += gridSize)
            {
                g.DrawLine(pen, bounds.Left, y, bounds.Right, y);
            }

            g.SmoothingMode = prevMode;
        }

        /// <summary>
        /// Paint accent stripe on edge (Ubuntu/branded designs)
        /// </summary>
        public static void PaintAccentStripe(Graphics g, Rectangle bounds,
            Color accentColor, StripeSide side, int width = 4)
        {
            if (g == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            Rectangle stripeRect = side switch
            {
                StripeSide.Left => new Rectangle(bounds.Left, bounds.Top, 
                    Math.Min(width, bounds.Width), bounds.Height),
                StripeSide.Right => new Rectangle(bounds.Right - Math.Min(width, bounds.Width), 
                    bounds.Top, Math.Min(width, bounds.Width), bounds.Height),
                StripeSide.Top => new Rectangle(bounds.Left, bounds.Top, 
                    bounds.Width, Math.Min(width, bounds.Height)),
                StripeSide.Bottom => new Rectangle(bounds.Left, 
                    bounds.Bottom - Math.Min(width, bounds.Height), 
                    bounds.Width, Math.Min(width, bounds.Height)),
                _ => Rectangle.Empty
            };

            if (stripeRect.Width > 0 && stripeRect.Height > 0)
            {
                var brush = PaintersFactory.GetSolidBrush(accentColor);
                g.FillRectangle(brush, stripeRect);
            }
        }

        /// <summary>
        /// Paint a top highlight line (Material elevation hint)
        /// </summary>
        public static void PaintTopHighlight(Graphics g, GraphicsPath path, int alpha = 14)
        {
            if (g == null || path == null) return;

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 2) return;

            var highlightBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(alpha, Color.White));
            
            using (var highlightRegion = new Region(path))
            using (var clipRect = new GraphicsPath())
            {
                clipRect.AddRectangle(new RectangleF(bounds.X, bounds.Y, bounds.Width, 2));
                highlightRegion.Intersect(clipRect);
                g.SetClip(highlightRegion, CombineMode.Replace);
                g.FillPath(highlightBrush, path);
                g.ResetClip();
            }
        }

        #endregion

        #region Utility Scopes

        /// <summary>
        /// Convenience scope for clipping drawing operations to a graphics path
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

        /// <summary>
        /// Convenience scope for temporarily changing smoothing mode
        /// </summary>
        public readonly struct SmoothingScope : IDisposable
        {
            private readonly Graphics _graphics;
            private readonly SmoothingMode _previousMode;

            public SmoothingScope(Graphics graphics, SmoothingMode newMode)
            {
                _graphics = graphics;
                _previousMode = graphics.SmoothingMode;
                graphics.SmoothingMode = newMode;
            }

            public void Dispose()
            {
                _graphics.SmoothingMode = _previousMode;
            }
        }

        #endregion
    }
}
