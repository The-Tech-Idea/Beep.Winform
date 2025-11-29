using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Shared rendering helpers for form painters and caption regions.
    /// Provides comprehensive utilities for button rendering, background effects,
    /// state handling, and common painting operations.
    /// </summary>
    public static class FormPainterRenderHelper
    {
        #region Enums

        /// <summary>
        /// Button visual state for state-aware rendering
        /// </summary>
        public enum ButtonState
        {
            Normal,
            Hovered,
            Pressed,
            Disabled
        }

        /// <summary>
        /// Button shape types for common button rendering
        /// </summary>
        public enum ButtonShape
        {
            Circle,
            RoundedRect,
            Square,
            Pill,
            Hexagon,
            Diamond,
            Triangle,
            Star,
            Octagon
        }

        #endregion

        #region Color Manipulation

        /// <summary>
        /// Lighten a color by a percentage (0.0 to 1.0)
        /// </summary>
        public static Color Lighten(Color color, float percent)
        {
            percent = Math.Max(0, Math.Min(1, percent));
            return Color.FromArgb(
                color.A,
                Math.Min(255, color.R + (int)((255 - color.R) * percent)),
                Math.Min(255, color.G + (int)((255 - color.G) * percent)),
                Math.Min(255, color.B + (int)((255 - color.B) * percent))
            );
        }

        /// <summary>
        /// Darken a color by a percentage (0.0 to 1.0)
        /// </summary>
        public static Color Darken(Color color, float percent)
        {
            percent = Math.Max(0, Math.Min(1, percent));
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
        /// Get state-adjusted color based on button state
        /// </summary>
        public static Color GetStateColor(Color baseColor, ButtonState state)
        {
            return state switch
            {
                ButtonState.Hovered => Lighten(baseColor, 0.15f),
                ButtonState.Pressed => Darken(baseColor, 0.15f),
                ButtonState.Disabled => WithAlpha(baseColor, 100),
                _ => baseColor
            };
        }

        /// <summary>
        /// Calculate luminance of a color (0-255)
        /// </summary>
        public static double GetLuminance(Color color)
        {
            return 0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B;
        }

        /// <summary>
        /// Determine if a color is dark (for contrast calculations)
        /// </summary>
        public static bool IsDarkColor(Color color)
        {
            return GetLuminance(color) < 128;
        }

        #endregion

        #region System Button Drawing

        /// <summary>
        /// Draw a system button (minimize/maximize/close) with optional hover outline and symbol.
        /// Colors are provided by caller (from FormPainterMetrics) to avoid coupling.
        /// </summary>
        public static void DrawSystemButton(Graphics g, Rectangle bounds, string symbol, bool isHover,
            bool isClose, Font baseFont, Color foregroundColor, Color hoverColor, Color? closeOutlineColor = null)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Hover indicator: outline around the button instead of background fill
            if (isHover)
            {
                var outlineColor = isClose ? (closeOutlineColor ?? Color.FromArgb(232, 17, 35)) : hoverColor;
                DrawHoverOutlineRect(g, bounds, outlineColor, 2, 6);
            }

            using var font = new Font(baseFont.FontFamily, baseFont.Size + 2, FontStyle.Regular);
            TextRenderer.DrawText(g, symbol, font, bounds, foregroundColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        /// <summary>
        /// Draw a rounded-rectangle outline for hover/pressed states.
        /// </summary>
        public static void DrawHoverOutlineRect(Graphics g, Rectangle bounds, Color color, int thickness = 2, int cornerRadius = 6)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;
            int inset = Math.Max(1, thickness);
            var rect = new Rectangle(bounds.X + inset, bounds.Y + inset, bounds.Width - inset * 2, bounds.Height - inset * 2);
            using var pen = new Pen(color, thickness);
            var old = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.DrawPath(pen, path);
            }
            g.SmoothingMode = old;
        }

        /// <summary>
        /// Draw a circular outline for hover/pressed states.
        /// </summary>
        public static void DrawHoverOutlineCircle(Graphics g, Rectangle bounds, Color color, int thickness = 2, int padding = 6)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;
            int size = Math.Min(bounds.Width, bounds.Height) - padding * 2;
            if (size <= 0) return;
            int cx = bounds.Left + (bounds.Width - size) / 2;
            int cy = bounds.Top + (bounds.Height - size) / 2;
            using var pen = new Pen(color, thickness);
            var old = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawEllipse(pen, cx, cy, size, size);
            g.SmoothingMode = old;
        }

        #endregion

        #region Button Shape Drawing

        /// <summary>
        /// Draw a circle button with state-aware styling
        /// </summary>
        public static void DrawCircleButton(Graphics g, Rectangle bounds, Color fillColor, 
            Color borderColor, ButtonState state, int padding = 4)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            int size = Math.Min(bounds.Width, bounds.Height) - padding * 2;
            if (size <= 0) return;

            int cx = bounds.Left + (bounds.Width - size) / 2;
            int cy = bounds.Top + (bounds.Height - size) / 2;
            var rect = new Rectangle(cx, cy, size, size);

            Color fill = GetStateColor(fillColor, state);

            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Fill
            using (var brush = new SolidBrush(fill))
            {
                g.FillEllipse(brush, rect);
            }

            // Border
            if (borderColor.A > 0)
            {
                using (var pen = new Pen(borderColor, 1f))
                {
                    g.DrawEllipse(pen, rect);
                }
            }

            g.SmoothingMode = prevMode;
        }

        /// <summary>
        /// Draw a rounded rectangle button with state-aware styling
        /// </summary>
        public static void DrawRoundedButton(Graphics g, Rectangle bounds, Color fillColor,
            int radius, ButtonState state, int padding = 4)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            var rect = new Rectangle(
                bounds.X + padding,
                bounds.Y + padding,
                bounds.Width - padding * 2,
                bounds.Height - padding * 2);

            if (rect.Width <= 0 || rect.Height <= 0) return;

            Color fill = GetStateColor(fillColor, state);

            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path = CreateRoundedRectanglePath(rect, radius))
            using (var brush = new SolidBrush(fill))
            {
                g.FillPath(brush, path);
            }

            g.SmoothingMode = prevMode;
        }

        /// <summary>
        /// Draw a pill-shaped button (fully rounded ends)
        /// </summary>
        public static void DrawPillButton(Graphics g, Rectangle bounds, Color fillColor,
            ButtonState state, int padding = 4)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            var rect = new Rectangle(
                bounds.X + padding,
                bounds.Y + padding,
                bounds.Width - padding * 2,
                bounds.Height - padding * 2);

            if (rect.Width <= 0 || rect.Height <= 0) return;

            int radius = Math.Min(rect.Width, rect.Height) / 2;
            Color fill = GetStateColor(fillColor, state);

            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path = CreateRoundedRectanglePath(rect, radius))
            using (var brush = new SolidBrush(fill))
            {
                g.FillPath(brush, path);
            }

            g.SmoothingMode = prevMode;
        }

        /// <summary>
        /// Draw traffic light style buttons (macOS/iOS style)
        /// </summary>
        public static void DrawTrafficLightButtons(Graphics g, Rectangle closeRect, Rectangle maxRect,
            Rectangle minRect, ButtonState closeState, ButtonState maxState, ButtonState minState,
            bool show3DEffect = true)
        {
            // Standard traffic light colors
            Color closeColor = Color.FromArgb(255, 95, 87);    // Red
            Color maxColor = Color.FromArgb(39, 201, 63);      // Green  
            Color minColor = Color.FromArgb(255, 189, 46);     // Yellow/Orange

            int buttonSize = 12;

            // Close button
            DrawTrafficLightButton(g, closeRect, closeColor, closeState, buttonSize, show3DEffect);

            // Maximize button (middle)
            DrawTrafficLightButton(g, maxRect, maxColor, maxState, buttonSize, show3DEffect);

            // Minimize button
            DrawTrafficLightButton(g, minRect, minColor, minState, buttonSize, show3DEffect);
        }

        private static void DrawTrafficLightButton(Graphics g, Rectangle bounds, Color baseColor,
            ButtonState state, int size, bool show3DEffect)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            int cx = bounds.Left + (bounds.Width - size) / 2;
            int cy = bounds.Top + (bounds.Height - size) / 2;
            var rect = new Rectangle(cx, cy, size, size);

            Color fill = GetStateColor(baseColor, state);

            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Main fill
            using (var brush = new SolidBrush(fill))
            {
                g.FillEllipse(brush, rect);
            }

            // 3D highlight effect
            if (show3DEffect && state != ButtonState.Pressed)
            {
                var highlightRect = new Rectangle(cx + 2, cy + 1, size - 4, size / 3);
                using (var highlightBrush = new LinearGradientBrush(
                    highlightRect,
                    Color.FromArgb(80, 255, 255, 255),
                    Color.FromArgb(0, 255, 255, 255),
                    LinearGradientMode.Vertical))
                {
                    g.FillEllipse(highlightBrush, highlightRect);
                }
            }

            // Border
            using (var pen = new Pen(Darken(fill, 0.2f), 0.5f))
            {
                g.DrawEllipse(pen, rect);
            }

            g.SmoothingMode = prevMode;
        }

        /// <summary>
        /// Draw a hexagon button (ArcLinux style)
        /// </summary>
        public static void DrawHexagonButton(Graphics g, Rectangle bounds, Color fillColor,
            ButtonState state, int padding = 4)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            int size = Math.Min(bounds.Width, bounds.Height) - padding * 2;
            if (size <= 0) return;

            int cx = bounds.Left + bounds.Width / 2;
            int cy = bounds.Top + bounds.Height / 2;
            int radius = size / 2;

            Color fill = GetStateColor(fillColor, state);

            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path = CreatePolygonPath(cx, cy, radius, 6))
            using (var brush = new SolidBrush(fill))
            {
                g.FillPath(brush, path);
            }

            g.SmoothingMode = prevMode;
        }

        /// <summary>
        /// Draw a diamond button (Solarized style)
        /// </summary>
        public static void DrawDiamondButton(Graphics g, Rectangle bounds, Color fillColor,
            ButtonState state, int padding = 4)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            int size = Math.Min(bounds.Width, bounds.Height) - padding * 2;
            if (size <= 0) return;

            int cx = bounds.Left + bounds.Width / 2;
            int cy = bounds.Top + bounds.Height / 2;
            int half = size / 2;

            Color fill = GetStateColor(fillColor, state);

            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var points = new Point[]
            {
                new Point(cx, cy - half),      // Top
                new Point(cx + half, cy),      // Right
                new Point(cx, cy + half),      // Bottom
                new Point(cx - half, cy)       // Left
            };

            using (var brush = new SolidBrush(fill))
            {
                g.FillPolygon(brush, points);
            }

            g.SmoothingMode = prevMode;
        }

        /// <summary>
        /// Draw a star button (Neon style)
        /// </summary>
        public static void DrawStarButton(Graphics g, Rectangle bounds, Color fillColor,
            ButtonState state, int points = 5, int padding = 4)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            int size = Math.Min(bounds.Width, bounds.Height) - padding * 2;
            if (size <= 0) return;

            int cx = bounds.Left + bounds.Width / 2;
            int cy = bounds.Top + bounds.Height / 2;
            int outerRadius = size / 2;
            int innerRadius = outerRadius / 2;

            Color fill = GetStateColor(fillColor, state);

            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path = CreateStarPath(cx, cy, outerRadius, innerRadius, points))
            using (var brush = new SolidBrush(fill))
            {
                g.FillPath(brush, path);
            }

            g.SmoothingMode = prevMode;
        }

        #endregion

        #region Background Effects

        /// <summary>
        /// Paint a solid background with optional state adjustment
        /// </summary>
        public static void PaintSolidBackground(Graphics g, Rectangle bounds, Color color)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using (var brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, bounds);
            }
        }

        /// <summary>
        /// Paint a gradient background
        /// </summary>
        public static void PaintGradientBackground(Graphics g, Rectangle bounds, Color startColor,
            Color endColor, LinearGradientMode mode)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using (var brush = new LinearGradientBrush(bounds, startColor, endColor, mode))
            {
                g.FillRectangle(brush, bounds);
            }
        }

        /// <summary>
        /// Paint scanline overlay effect (Terminal/Retro style)
        /// </summary>
        public static void PaintScanlineOverlay(Graphics g, Rectangle bounds, Color lineColor, int spacing = 2)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            using (var pen = new Pen(lineColor, 1f))
            {
                for (int y = bounds.Top; y < bounds.Bottom; y += spacing)
                {
                    g.DrawLine(pen, bounds.Left, y, bounds.Right, y);
                }
            }

            g.SmoothingMode = prevMode;
        }

        /// <summary>
        /// Paint a vignette effect (darkened edges)
        /// </summary>
        public static void PaintVignetteEffect(Graphics g, Rectangle bounds, Color vignetteColor, float intensity = 0.3f)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using (var path = new GraphicsPath())
            {
                path.AddEllipse(bounds.X - bounds.Width / 4, bounds.Y - bounds.Height / 4,
                    bounds.Width * 1.5f, bounds.Height * 1.5f);

                using (var brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.Transparent;
                    brush.SurroundColors = new[] { WithAlpha(vignetteColor, (int)(255 * intensity)) };
                    g.FillRectangle(brush, bounds);
                }
            }
        }

        /// <summary>
        /// Paint a top highlight band
        /// </summary>
        public static void PaintTopHighlight(Graphics g, Rectangle bounds, int height, int alpha = 20)
        {
            if (bounds.Width <= 0 || height <= 0) return;

            var highlightRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, Math.Min(height, bounds.Height));
            using (var brush = new LinearGradientBrush(
                highlightRect,
                Color.FromArgb(alpha, 255, 255, 255),
                Color.FromArgb(0, 255, 255, 255),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, highlightRect);
            }
        }

        /// <summary>
        /// Paint acrylic noise texture (Fluent style)
        /// </summary>
        public static void PaintAcrylicNoise(Graphics g, Rectangle bounds, int intensity = 3)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using (var brush = new HatchBrush(HatchStyle.Percent10,
                Color.FromArgb(intensity, 255, 255, 255), Color.Transparent))
            {
                g.FillRectangle(brush, bounds);
            }
        }

        /// <summary>
        /// Paint a halftone dot pattern (Cartoon style)
        /// </summary>
        public static void PaintHalftonePattern(Graphics g, Rectangle bounds, Color dotColor, int spacing = 8)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            using (var brush = new SolidBrush(dotColor))
            {
                for (int y = bounds.Top; y < bounds.Bottom; y += spacing)
                {
                    for (int x = bounds.Left; x < bounds.Right; x += spacing)
                    {
                        g.FillRectangle(brush, x, y, 1, 1);
                    }
                }
            }

            g.SmoothingMode = prevMode;
        }

        /// <summary>
        /// Paint grid overlay (OneDark style)
        /// </summary>
        public static void PaintGridOverlay(Graphics g, Rectangle bounds, Color lineColor, int gridSize = 40)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            using (var pen = new Pen(lineColor, 1f))
            {
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
            }

            g.SmoothingMode = prevMode;
        }

        #endregion

        #region Border Effects

        /// <summary>
        /// Paint a glow border effect
        /// </summary>
        public static void PaintGlowBorder(Graphics g, GraphicsPath path, Color glowColor, params float[] widths)
        {
            if (path == null || widths == null || widths.Length == 0) return;

            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw from outer to inner
            for (int i = widths.Length - 1; i >= 0; i--)
            {
                int alpha = (int)(glowColor.A * ((float)(i + 1) / widths.Length));
                using (var pen = new Pen(WithAlpha(glowColor, alpha), widths[i]))
                {
                    g.DrawPath(pen, path);
                }
            }

            g.SmoothingMode = prevMode;
        }

        /// <summary>
        /// Paint a neon border with multiple colors
        /// </summary>
        public static void PaintNeonBorder(Graphics g, GraphicsPath path, Color[] colors, float baseWidth = 2f)
        {
            if (path == null || colors == null || colors.Length == 0) return;

            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw glow layers for each color
            foreach (var color in colors)
            {
                // Outer glow
                using (var pen = new Pen(WithAlpha(color, 30), baseWidth + 6))
                {
                    g.DrawPath(pen, path);
                }

                // Mid glow
                using (var pen = new Pen(WithAlpha(color, 60), baseWidth + 3))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Core line with first color
            using (var pen = new Pen(colors[0], baseWidth))
            {
                g.DrawPath(pen, path);
            }

            g.SmoothingMode = prevMode;
        }

        /// <summary>
        /// Paint a 3D bevel border (Win95/Retro style)
        /// </summary>
        public static void Paint3DBevelBorder(Graphics g, Rectangle bounds, Color lightColor,
            Color darkColor, int width = 2)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            // Light edges (top and left)
            using (var lightPen = new Pen(lightColor, 1f))
            {
                for (int i = 0; i < width; i++)
                {
                    g.DrawLine(lightPen, bounds.Left + i, bounds.Top + i, bounds.Right - i - 1, bounds.Top + i);
                    g.DrawLine(lightPen, bounds.Left + i, bounds.Top + i, bounds.Left + i, bounds.Bottom - i - 1);
                }
            }

            // Dark edges (bottom and right)
            using (var darkPen = new Pen(darkColor, 1f))
            {
                for (int i = 0; i < width; i++)
                {
                    g.DrawLine(darkPen, bounds.Left + i, bounds.Bottom - i - 1, bounds.Right - i - 1, bounds.Bottom - i - 1);
                    g.DrawLine(darkPen, bounds.Right - i - 1, bounds.Top + i, bounds.Right - i - 1, bounds.Bottom - i - 1);
                }
            }

            g.SmoothingMode = prevMode;
        }

        #endregion

        #region Path Creation Utilities

        /// <summary>
        /// Create a rounded rectangle path
        /// </summary>
        public static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(rect);
                path.CloseFigure();
                return path;
            }

            // Top-left
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            // Top-right
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            // Bottom-right
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            // Bottom-left
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Create a regular polygon path
        /// </summary>
        public static GraphicsPath CreatePolygonPath(int cx, int cy, int radius, int sides)
        {
            var path = new GraphicsPath();
            var points = new PointF[sides];

            double angleStep = Math.PI * 2 / sides;
            double startAngle = -Math.PI / 2; // Start from top

            for (int i = 0; i < sides; i++)
            {
                double angle = startAngle + i * angleStep;
                points[i] = new PointF(
                    cx + (float)(radius * Math.Cos(angle)),
                    cy + (float)(radius * Math.Sin(angle))
                );
            }

            path.AddPolygon(points);
            return path;
        }

        /// <summary>
        /// Create a star path
        /// </summary>
        public static GraphicsPath CreateStarPath(int cx, int cy, int outerRadius, int innerRadius, int points)
        {
            var path = new GraphicsPath();
            var starPoints = new PointF[points * 2];

            double angleStep = Math.PI / points;
            double startAngle = -Math.PI / 2;

            for (int i = 0; i < points * 2; i++)
            {
                double angle = startAngle + i * angleStep;
                int radius = (i % 2 == 0) ? outerRadius : innerRadius;
                starPoints[i] = new PointF(
                    cx + (float)(radius * Math.Cos(angle)),
                    cy + (float)(radius * Math.Sin(angle))
                );
            }

            path.AddPolygon(starPoints);
            return path;
        }

        #endregion

        #region Utility Scopes

        /// <summary>
        /// Scope for managing compositing mode changes
        /// </summary>
        public readonly struct CompositingScope : IDisposable
        {
            private readonly Graphics _graphics;
            private readonly CompositingMode _previousMode;

            public CompositingScope(Graphics graphics, CompositingMode mode)
            {
                _graphics = graphics;
                _previousMode = graphics.CompositingMode;
                graphics.CompositingMode = mode;
            }

            public void Dispose()
            {
                _graphics.CompositingMode = _previousMode;
            }
        }

        /// <summary>
        /// Scope for managing smoothing mode changes
        /// </summary>
        public readonly struct SmoothingScope : IDisposable
        {
            private readonly Graphics _graphics;
            private readonly SmoothingMode _previousMode;

            public SmoothingScope(Graphics graphics, SmoothingMode mode)
            {
                _graphics = graphics;
                _previousMode = graphics.SmoothingMode;
                graphics.SmoothingMode = mode;
            }

            public void Dispose()
            {
                _graphics.SmoothingMode = _previousMode;
            }
        }

        /// <summary>
        /// Scope for managing clip region changes
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

            public ClipScope(Graphics graphics, Rectangle clipRect)
            {
                _graphics = graphics;
                _state = graphics.Save();
                graphics.SetClip(clipRect, CombineMode.Intersect);
            }

            public void Dispose()
            {
                _graphics.Restore(_state);
            }
        }

        #endregion
    }
}
