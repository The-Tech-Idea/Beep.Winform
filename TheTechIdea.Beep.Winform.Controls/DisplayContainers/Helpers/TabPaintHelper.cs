using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers
{
    /// <summary>
    /// Helper class for professional tab rendering similar to BeepTabs
    /// </summary>
    internal class TabPaintHelper
    {
        private readonly IBeepTheme _theme;

        public TabPaintHelper(IBeepTheme theme)
        {
            _theme = theme;
        }

        /// <summary>
        /// Draws a professional tab with rounded corners, gradients, and theme colors
        /// </summary>
        public void DrawProfessionalTab(Graphics g, Rectangle bounds, string title, Font font, 
            bool isActive, bool isHovered, bool showCloseButton, bool isCloseHovered, 
            float animationProgress = 0f)
        {
            // Get theme colors
            var colors = GetTabColors(isActive, isHovered, animationProgress);
            
            // Draw tab background with gradient
            DrawTabBackground(g, bounds, colors, isActive, isHovered);
            
            // Draw tab border
            DrawTabBorder(g, bounds, colors.BorderColor, isActive);
            
            // Draw tab text
            var textBounds = showCloseButton ? 
                new Rectangle(bounds.X + 8, bounds.Y, bounds.Width - 28, bounds.Height) :
                new Rectangle(bounds.X + 8, bounds.Y, bounds.Width - 16, bounds.Height);
            
            DrawTabText(g, textBounds, title, font, colors.TextColor, isActive);
            
            // Draw close button if needed
            if (showCloseButton)
            {
                var closeRect = new Rectangle(bounds.Right - 20, bounds.Y + 8, 12, 12);
                DrawCloseButton(g, closeRect, isCloseHovered, colors.TextColor);
            }
        }

        private TabColors GetTabColors(bool isActive, bool isHovered, float animationProgress)
        {
            Color backColor, textColor, borderColor;

            if (isActive)
            {
                backColor = _theme?.TabSelectedBackColor ?? Color.White;
                textColor = _theme?.TabSelectedForeColor ?? Color.FromArgb(32, 32, 32);
                borderColor = _theme?.TabSelectedBorderColor ?? Color.FromArgb(120, 120, 120);
            }
            else if (isHovered)
            {
                var baseColor = _theme?.TabBackColor ?? Color.FromArgb(245, 245, 245);
                var hoverColor = _theme?.TabHoverBackColor ?? Color.FromArgb(235, 235, 235);
                
                backColor = InterpolateColor(baseColor, hoverColor, animationProgress);
                textColor = _theme?.TabHoverForeColor ?? _theme?.TabForeColor ?? Color.FromArgb(64, 64, 64);
                borderColor = _theme?.TabHoverBorderColor ?? Color.FromArgb(150, 150, 150);
            }
            else
            {
                backColor = _theme?.TabBackColor ?? Color.FromArgb(245, 245, 245);
                textColor = _theme?.TabForeColor ?? Color.FromArgb(64, 64, 64);
                borderColor = _theme?.TabBorderColor ?? Color.FromArgb(180, 180, 180);
            }

            return new TabColors
            {
                BackgroundColor = backColor,
                TextColor = textColor,
                BorderColor = borderColor
            };
        }

        private void DrawTabBackground(Graphics g, Rectangle bounds, TabColors colors, bool isActive, bool isHovered)
        {
            using (var path = CreateRoundedPath(bounds, 4))
            {
                if (isActive || isHovered)
                {
                    // Draw subtle gradient for active/hovered tabs
                    using (var brush = new LinearGradientBrush(bounds,
                        ControlPaint.Light(colors.BackgroundColor, 0.1f),
                        ControlPaint.Dark(colors.BackgroundColor, 0.05f),
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, path);
                    }
                }
                else
                {
                    // Solid color for normal tabs
                    using (var brush = new SolidBrush(colors.BackgroundColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
        }

        private void DrawTabBorder(Graphics g, Rectangle bounds, Color borderColor, bool isActive)
        {
            using (var path = CreateRoundedPath(bounds, 4))
            using (var pen = new Pen(borderColor, isActive ? 1.5f : 1f))
            {
                g.DrawPath(pen, path);
                
                // Add inner highlight for active tab
                if (isActive)
                {
                    var innerBounds = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2);
                    using (var innerPath = CreateRoundedPath(innerBounds, 3))
                    using (var innerPen = new Pen(ControlPaint.Light(borderColor, 0.3f), 0.5f))
                    {
                        g.DrawPath(innerPen, innerPath);
                    }
                }
            }
        }

        private void DrawTabText(Graphics g, Rectangle textBounds, string title, Font font, Color textColor, bool isActive)
        {
            using (var brush = new SolidBrush(textColor))
            {
                var actualFont = isActive ? new Font(font, FontStyle.Bold) : font;
                
                using (actualFont)
                {
                    var stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter,
                        FormatFlags = StringFormatFlags.NoWrap
                    };

                    // Add subtle text shadow for better readability
                    if (isActive)
                    {
                        using (var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
                        {
                            var shadowBounds = new Rectangle(textBounds.X + 1, textBounds.Y + 1, 
                                                           textBounds.Width, textBounds.Height);
                            g.DrawString(title, actualFont, shadowBrush, shadowBounds, stringFormat);
                        }
                    }

                    g.DrawString(title, actualFont, brush, textBounds, stringFormat);
                }
            }
        }

        private void DrawCloseButton(Graphics g, Rectangle closeRect, bool isHovered, Color baseColor)
        {
            // Draw close button background on hover
            if (isHovered)
            {
                using (var bgBrush = new SolidBrush(Color.FromArgb(120, Color.Red)))
                using (var path = CreateRoundedPath(closeRect, 2))
                {
                    g.FillPath(bgBrush, path);
                }
            }

            // Draw the X
            var color = isHovered ? Color.White : baseColor;
            using (var pen = new Pen(color, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                var centerX = closeRect.X + closeRect.Width / 2f;
                var centerY = closeRect.Y + closeRect.Height / 2f;
                var size = Math.Min(closeRect.Width, closeRect.Height) / 3f;

                g.DrawLine(pen, centerX - size, centerY - size, centerX + size, centerY + size);
                g.DrawLine(pen, centerX + size, centerY - size, centerX - size, centerY + size);
            }
        }

        private GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            
            if (diameter <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            try
            {
                var arc = new Rectangle(rect.X, rect.Y, diameter, diameter);
                path.AddArc(arc, 180, 90);
                
                arc.X = rect.Right - diameter;
                path.AddArc(arc, 270, 90);
                
                arc.Y = rect.Bottom - diameter;
                path.AddArc(arc, 0, 90);
                
                arc.X = rect.Left;
                path.AddArc(arc, 90, 90);
                
                path.CloseFigure();
            }
            catch
            {
                path.Reset();
                path.AddRectangle(rect);
            }

            return path;
        }

        private Color InterpolateColor(Color color1, Color color2, float progress)
        {
            progress = Math.Max(0, Math.Min(1, progress));
            
            return Color.FromArgb(
                (int)(color1.A + (color2.A - color1.A) * progress),
                (int)(color1.R + (color2.R - color1.R) * progress),
                (int)(color1.G + (color2.G - color1.G) * progress),
                (int)(color1.B + (color2.B - color1.B) * progress)
            );
        }

        private struct TabColors
        {
            public Color BackgroundColor;
            public Color TextColor;
            public Color BorderColor;
        }
    }
}