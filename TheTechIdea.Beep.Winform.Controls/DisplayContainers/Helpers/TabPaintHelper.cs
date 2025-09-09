using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
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
            // Validate input parameters
            if (g == null || bounds.Width <= 0 || bounds.Height <= 0 || font == null)
                return;

            // Ensure title is not null
            if (string.IsNullOrEmpty(title))
                title = "Tab";

            // Clamp animation progress
            animationProgress = Math.Max(0f, Math.Min(1f, animationProgress));

            try
            {
                // Get theme colors
                var colors = GetTabColors(isActive, isHovered, animationProgress);
                
                // Draw tab background with gradient
                DrawTabBackground(g, bounds, colors, isActive, isHovered);
                
                // Draw tab border
                DrawTabBorder(g, bounds, colors.BorderColor, isActive);
                
                // Calculate text bounds with proper margins
                var textBounds = showCloseButton ? 
                    new Rectangle(bounds.X + 8, bounds.Y + 2, Math.Max(0, bounds.Width - 36), Math.Max(0, bounds.Height - 4)) :
                    new Rectangle(bounds.X + 8, bounds.Y + 2, Math.Max(0, bounds.Width - 16), Math.Max(0, bounds.Height - 4));
                
                // Draw tab text only if there's space
                if (textBounds.Width > 10 && textBounds.Height > 10)
                {
                    DrawTabText(g, textBounds, title, font, colors.TextColor, isActive);
                }
                
                // Draw close button if needed and there's space
                if (showCloseButton && bounds.Width > 30 && bounds.Height > 16)
                {
                    var closeRect = new Rectangle(bounds.Right - 20, bounds.Y + (bounds.Height - 12) / 2, 12, 12);
                    DrawCloseButton(g, closeRect, isCloseHovered, colors.TextColor);
                }
            }
            catch (Exception ex)
            {
                // Log error for debugging
                System.Diagnostics.Debug.WriteLine($"DrawProfessionalTab error: {ex.Message}");
                
                // Fallback to simple rectangle drawing
                try
                {
                    using (var brush = new SolidBrush(isActive ? SystemColors.ControlLight : SystemColors.Control))
                    {
                        g.FillRectangle(brush, bounds);
                    }
                    using (var pen = new Pen(SystemColors.ControlDark))
                    {
                        g.DrawRectangle(pen, bounds);
                    }
                    
                    // Simple text rendering as fallback
                    if (!string.IsNullOrEmpty(title) && bounds.Width > 20 && bounds.Height > 10)
                    {
                        var textRect = new Rectangle(bounds.X + 4, bounds.Y + 2, bounds.Width - 8, bounds.Height - 4);
                        TextRenderer.DrawText(g, title, font, textRect, SystemColors.ControlText, 
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | 
                            TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
                    }
                }
                catch
                {
                    // If even the fallback fails, give up gracefully
                }
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
            // Validate parameters before drawing
            if (g == null || string.IsNullOrEmpty(title) || font == null || textBounds.Width <= 0 || textBounds.Height <= 0)
                return;

            using (var brush = new SolidBrush(textColor))
            {
                Font actualFont = null;
                try
                {
                    // Create font safely - don't dispose the original font
                    actualFont = isActive ? new Font(font, FontStyle.Bold) : font;
                    
                    using (var stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter,
                        FormatFlags = StringFormatFlags.NoWrap
                    })
                    {
                        // Add subtle text shadow for better readability
                        if (isActive)
                        {
                            using (var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
                            {
                                var shadowBounds = new Rectangle(textBounds.X + 1, textBounds.Y + 1, 
                                                               textBounds.Width, textBounds.Height);
                                
                                // Validate shadow bounds
                                if (shadowBounds.Width > 0 && shadowBounds.Height > 0)
                                {
                                    g.DrawString(title, actualFont, shadowBrush, shadowBounds, stringFormat);
                                }
                            }
                        }
                        
                        // Draw main text
                        g.DrawString(title, actualFont, brush, textBounds, stringFormat);
                    }
                }
                catch (ArgumentException ex)
                {
                    // Log the error details for debugging
                    System.Diagnostics.Debug.WriteLine($"DrawTabText error: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"TextBounds: {textBounds}");
                    System.Diagnostics.Debug.WriteLine($"Title: '{title}'");
                    System.Diagnostics.Debug.WriteLine($"Font: {font?.Name}, Size: {font?.Size}");
                    
                    // Fallback to simple text drawing without formatting
                    try
                    {
                        TextRenderer.DrawText(g, title, font, textBounds, textColor, 
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | 
                            TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
                    }
                    catch
                    {
                        // If all else fails, skip drawing this text
                    }
                }
                finally
                {
                    // Only dispose the font if we created it (i.e., for bold active tabs)
                    if (actualFont != null && actualFont != font)
                    {
                        actualFont.Dispose();
                    }
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
            
            // Validate rectangle
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return path; // Return empty path for invalid rectangles
            }
            
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            // Ensure radius doesn't exceed rectangle dimensions
            int maxRadius = Math.Min(rect.Width / 2, rect.Height / 2);
            radius = Math.Min(radius, maxRadius);
            
            int diameter = radius * 2;
            
            if (diameter <= 0 || diameter > Math.Min(rect.Width, rect.Height))
            {
                path.AddRectangle(rect);
                return path;
            }

            try
            {
                // Create rounded rectangle path
                var arc = new Rectangle(rect.X, rect.Y, diameter, diameter);
                
                // Top-left arc
                path.AddArc(arc, 180, 90);
                
                // Top-right arc
                arc.X = rect.Right - diameter;
                path.AddArc(arc, 270, 90);
                
                // Bottom-right arc
                arc.Y = rect.Bottom - diameter;
                path.AddArc(arc, 0, 90);
                
                // Bottom-left arc
                arc.X = rect.Left;
                path.AddArc(arc, 90, 90);
                
                path.CloseFigure();
            }
            catch (ArgumentException ex)
            {
                // If arc creation fails, fall back to rectangle
                System.Diagnostics.Debug.WriteLine($"CreateRoundedPath error: {ex.Message}");
                path.Reset();
                path.AddRectangle(rect);
            }
            catch (Exception ex)
            {
                // Handle any other unexpected errors
                System.Diagnostics.Debug.WriteLine($"CreateRoundedPath unexpected error: {ex.Message}");
                path.Reset();
                if (rect.Width > 0 && rect.Height > 0)
                {
                    path.AddRectangle(rect);
                }
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