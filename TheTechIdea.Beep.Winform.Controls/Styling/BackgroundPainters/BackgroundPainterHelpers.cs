using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Shared helper methods for all background painters
    /// Provides consistent state handling, color manipulation, and painting utilities
    /// </summary>
    public static class BackgroundPainterHelpers
    {
        /// <summary>
        /// Runtime pipeline guard set by BeepStyling during full control painting.
        /// When true, background painters should avoid border-like edge strokes.
        /// </summary>
        internal static bool SuppressEdgeStrokes { get; set; } = false;

        /// <summary>
        /// Determines whether decorative edge strokes should be painted by background painters.
        /// </summary>
        public static bool ShouldPaintDecorativeEdgeStroke(BeepControlStyle style)
        {
            if (!SuppressEdgeStrokes)
                return true;

            // If a style effectively has no border, allow background edge accents.
            return StyleBorders.GetBorderWidth(style) <= 0f;
        }

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
                var themeColor = BeepStyling.GetThemeColor(theme, themeColorKey);
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
                var themeColor = BeepStyling.GetThemeColor(theme, themeColorKey);
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            return styleColorFunc(style);
        }

        #endregion

        #region Color Manipulation

        /// <summary>
        /// Lighten a color by a percentage
        /// Delegates to ColorAccessibilityHelper for HSL-based color manipulation (more natural results)
        /// Maintained for backward compatibility
        /// </summary>
        public static Color Lighten(Color color, float percent)
        {
            // Use HSL-based color manipulation for more natural results
            return ColorAccessibilityHelper.LightenColor(color, percent);
        }

        /// <summary>
        /// Darken a color by a percentage
        /// Delegates to ColorAccessibilityHelper for HSL-based color manipulation (more natural results)
        /// Maintained for backward compatibility
        /// </summary>
        public static Color Darken(Color color, float percent)
        {
            // Use HSL-based color manipulation for more natural results
            return ColorAccessibilityHelper.DarkenColor(color, percent);
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
        /// Enhanced with better blur simulation and backdrop effects
        /// Best for: Glassmorphism, Acrylic, Mica styles
        /// </summary>
        public static void PaintFrostedGlassBackground(Graphics g, GraphicsPath path,
            Color baseColor, int baseAlpha, ControlState state, bool includeEdgeHighlight = true)
        {
            if (g == null || path == null) return;

            // Apply state to base color
            Color stateColor = GetStateAdjustedColor(baseColor, state, StateIntensity.Subtle);
            Color fillColor = WithAlpha(stateColor, baseAlpha);

            var brush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(brush, path);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Enhanced blur simulation using multiple layers
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingMode = CompositingMode.SourceOver;

            // Add subtle backdrop blur effect (multi-layer simulation)
            int blurLayers = 3;
            for (int i = 0; i < blurLayers; i++)
            {
                int blurAlpha = (int)(baseAlpha * 0.3f * (1.0f - i / (float)blurLayers));
                if (blurAlpha > 0)
                {
                    using (var blurPath = (GraphicsPath)path.Clone())
                    using (var matrix = new Matrix())
                    {
                        float offset = i * 0.5f;
                        matrix.Translate(offset, offset);
                        blurPath.Transform(matrix);
                        
                        var blurBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(blurAlpha, stateColor));
                        g.FillPath(blurBrush, blurPath);
                    }
                }
            }

            // Add frost pattern using clipping
            using (var clip = new ClipScope(g, path))
            {
                // Enhanced dotted pattern for better frost simulation
                using (var frostBrush = new HatchBrush(HatchStyle.DottedGrid, 
                    Color.FromArgb(20, Color.White), Color.Transparent))
                {
                    g.FillRectangle(frostBrush, Rectangle.Round(bounds));
                }
            }

            // Enhanced top highlight for glass reflection
            var topRect = new RectangleF(bounds.Left, bounds.Top, bounds.Width, bounds.Height / 3f);
            var highlight = PaintersFactory.GetLinearGradientBrush(
                topRect,
                Color.FromArgb(45, Color.White),
                Color.Transparent,
                LinearGradientMode.Vertical);
            
            using (var clip = new ClipScope(g, path))
            {
                g.FillRectangle(highlight, topRect);
            }

            if (!includeEdgeHighlight)
                return;

            // Add subtle border highlight for glass edge
            using (var borderPath = (GraphicsPath)path.Clone())
            {
                var borderPen = PaintersFactory.GetPen(Color.FromArgb(60, Color.White), 1f);
                g.DrawPath(borderPen, borderPath);
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
            Color lineColor, int spacing = 2, GraphicsPath clipPath = null)
        {
            if (g == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            using (var clip = new ClipScope(g, clipPath))
            {
                var pen = PaintersFactory.GetPen(lineColor, 1f);
                for (int y = bounds.Top; y < bounds.Bottom; y += spacing)
                {
                    g.DrawLine(pen, bounds.Left, y, bounds.Right, y);
                }
            }

            g.SmoothingMode = prevMode;
        }

        /// <summary>
        /// Paint grid overlay (Terminal/Tech effect)
        /// </summary>
        public static void PaintGridOverlay(Graphics g, Rectangle bounds,
            Color lineColor, int gridSize = 20, GraphicsPath clipPath = null)
        {
            if (g == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            using (var clip = new ClipScope(g, clipPath))
            {
                var pen = PaintersFactory.GetPen(lineColor, 1f);

                for (int x = bounds.Left; x < bounds.Right; x += gridSize)
                {
                    g.DrawLine(pen, x, bounds.Top, x, bounds.Bottom);
                }

                for (int y = bounds.Top; y < bounds.Bottom; y += gridSize)
                {
                    g.DrawLine(pen, bounds.Left, y, bounds.Right, y);
                }
            }

            g.SmoothingMode = prevMode;
        }

        /// <summary>
        /// Paint accent stripe on edge (Ubuntu/branded designs)
        /// </summary>
        public static void PaintAccentStripe(Graphics g, Rectangle bounds,
            Color accentColor, StripeSide side, int width = 4, GraphicsPath clipPath = null)
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
                using (var clip = new ClipScope(g, clipPath))
                {
                    var brush = PaintersFactory.GetSolidBrush(accentColor);
                    g.FillRectangle(brush, stripeRect);
                }
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
            private readonly bool _active;

            public ClipScope(Graphics graphics, GraphicsPath clipPath)
            {
                _graphics = graphics;
                if (clipPath != null && clipPath.PointCount > 0)
                {
                    _state = graphics.Save();
                    graphics.SetClip(clipPath, CombineMode.Intersect);
                    _active = true;
                }
                else
                {
                    _state = null;
                    _active = false;
                }
            }

            public void Dispose()
            {
                if (_active && _graphics != null)
                {
                    _graphics.Restore(_state);
                }
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

        #region Advanced Visual Effects

        /// <summary>
        /// Paint advanced blur background using multi-layer simulation
        /// Better blur effect than simple transparency
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="path">Control path</param>
        /// <param name="baseColor">Base color</param>
        /// <param name="blurRadius">Blur radius in pixels (8, 16, 24 typical)</param>
        /// <param name="state">Control state</param>
        public static void PaintAdvancedBlurBackground(Graphics g, GraphicsPath path,
            Color baseColor, int blurRadius, ControlState state)
        {
            if (g == null || path == null) return;

            // Apply state to base color
            Color stateColor = GetStateAdjustedColor(baseColor, state, StateIntensity.Subtle);
            
            // Multi-layer blur simulation
            // Draw multiple offset layers with decreasing opacity
            int layers = Math.Min(blurRadius / 2, 12); // Cap layers for performance
            if (layers < 1) layers = 1;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingMode = CompositingMode.SourceOver;

            for (int i = layers; i > 0; i--)
            {
                float offset = (blurRadius * i) / (float)layers;
                int alpha = (int)(200 * (i / (float)layers));
                alpha = Math.Max(10, Math.Min(255, alpha));

                // Create offset path
                using (var offsetPath = (GraphicsPath)path.Clone())
                using (var matrix = new Matrix())
                {
                    // Slight random offset for more realistic blur
                    float offsetX = offset * 0.3f;
                    float offsetY = offset * 0.7f;
                    matrix.Translate(offsetX, offsetY);
                    offsetPath.Transform(matrix);

                    var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(alpha, stateColor));
                    g.FillPath(brush, offsetPath);
                }
            }

            // Fill base path with semi-transparent color
            Color fillColor = WithAlpha(stateColor, 200);
            var baseBrush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(baseBrush, path);
        }

        /// <summary>
        /// Paint radial gradient background
        /// Best for: Circular controls, buttons, badges, spotlight effects
        /// </summary>
        public static void PaintRadialGradientBackground(Graphics g, GraphicsPath path,
            Color centerColor, Color edgeColor, ControlState state,
            StateIntensity intensity = StateIntensity.Normal)
        {
            if (g == null || path == null) return;

            Color fillCenter = GetStateAdjustedColor(centerColor, state, intensity);
            Color fillEdge = GetStateAdjustedColor(edgeColor, state, intensity);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using (var gradientBrush = new PathGradientBrush(path))
            {
                gradientBrush.CenterColor = fillCenter;
                gradientBrush.SurroundColors = new[] { fillEdge };
                
                // Set center point to middle of bounds
                gradientBrush.CenterPoint = new PointF(
                    bounds.X + bounds.Width / 2f,
                    bounds.Y + bounds.Height / 2f
                );

                g.FillPath(gradientBrush, path);
            }
        }

        /// <summary>
        /// Paint conic (angular) gradient background
        /// Best for: Circular progress indicators, color wheels, radial menus
        /// </summary>
        public static void PaintConicGradientBackground(Graphics g, GraphicsPath path,
            Color[] colors, float startAngle, ControlState state)
        {
            if (g == null || path == null || colors == null || colors.Length < 2) return;

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Simulate conic gradient using multiple linear gradients
            // This is an approximation since GDI+ doesn't support conic gradients natively
            int segments = Math.Min(colors.Length * 4, 32); // More segments = smoother gradient
            float angleStep = 360f / segments;

            PointF center = new PointF(
                bounds.X + bounds.Width / 2f,
                bounds.Y + bounds.Height / 2f
            );

            float radius = Math.Max(bounds.Width, bounds.Height) / 2f;

            for (int i = 0; i < segments; i++)
            {
                float angle1 = startAngle + (i * angleStep);
                float angle2 = startAngle + ((i + 1) * angleStep);

                // Get colors for this segment
                int colorIndex1 = (i * (colors.Length - 1)) / segments;
                int colorIndex2 = Math.Min(colors.Length - 1, colorIndex1 + 1);
                Color color1 = colors[colorIndex1];
                Color color2 = colors[colorIndex2];

                // Create pie-shaped path for this segment
                using (var segmentPath = new GraphicsPath())
                {
                    segmentPath.AddPie(
                        bounds.X, bounds.Y,
                        bounds.Width, bounds.Height,
                        angle1, angleStep
                    );

                    // Clip to original path
                    using (var region = new Region(path))
                    {
                        region.Intersect(segmentPath);
                        g.SetClip(region, CombineMode.Replace);

                        // Draw gradient for this segment
                        var rect = segmentPath.GetBounds();
                        if (rect.Width > 0 && rect.Height > 0)
                        {
                            var brush = PaintersFactory.GetLinearGradientBrush(
                                rect, color1, color2, angle1);
                            g.FillPath(brush, segmentPath);
                        }

                        g.ResetClip();
                    }
                }
            }
        }

        /// <summary>
        /// Paint multi-stop gradient background
        /// Best for: Complex gradients with multiple color stops
        /// </summary>
        /// <param name="stops">Array of gradient stops (position 0.0-1.0, color)</param>
        /// <param name="mode">Gradient direction</param>
        public static void PaintMultiStopGradientBackground(Graphics g, GraphicsPath path,
            (float position, Color color)[] stops, LinearGradientMode mode, ControlState state,
            StateIntensity intensity = StateIntensity.Normal)
        {
            if (g == null || path == null || stops == null || stops.Length < 2) return;

            // Sort stops by position
            var sortedStops = stops.OrderBy(s => s.position).ToArray();

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Draw gradient segments between stops
            for (int i = 0; i < sortedStops.Length - 1; i++)
            {
                var stop1 = sortedStops[i];
                var stop2 = sortedStops[i + 1];

                Color color1 = GetStateAdjustedColor(stop1.color, state, intensity);
                Color color2 = GetStateAdjustedColor(stop2.color, state, intensity);

                // Calculate segment bounds based on gradient mode
                RectangleF segmentBounds = CalculateGradientSegmentBounds(bounds, stop1.position, stop2.position, mode);

                if (segmentBounds.Width > 0 && segmentBounds.Height > 0)
                {
                    var brush = PaintersFactory.GetLinearGradientBrush(segmentBounds, color1, color2, mode);
                    
                    // Clip to segment area
                    using (var clipPath = new GraphicsPath())
                    {
                        clipPath.AddRectangle(segmentBounds);
                        using (var region = new Region(path))
                        {
                            region.Intersect(clipPath);
                            g.SetClip(region, CombineMode.Replace);
                            g.FillPath(brush, path);
                            g.ResetClip();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculate bounds for a gradient segment
        /// </summary>
        private static RectangleF CalculateGradientSegmentBounds(RectangleF bounds, float startPos, float endPos, LinearGradientMode mode)
        {
            switch (mode)
            {
                case LinearGradientMode.Vertical:
                    float height = bounds.Height * (endPos - startPos);
                    return new RectangleF(
                        bounds.X,
                        bounds.Y + (bounds.Height * startPos),
                        bounds.Width,
                        height
                    );
                case LinearGradientMode.Horizontal:
                    float width = bounds.Width * (endPos - startPos);
                    return new RectangleF(
                        bounds.X + (bounds.Width * startPos),
                        bounds.Y,
                        width,
                        bounds.Height
                    );
                default:
                    return bounds;
            }
        }

        #endregion
    }
}
