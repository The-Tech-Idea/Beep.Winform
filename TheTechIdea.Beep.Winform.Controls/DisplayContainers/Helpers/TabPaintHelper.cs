using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers
{
    /// <summary>
    /// Helper class for professional tab rendering with modern styling using BeepStyling
    /// </summary>
    internal class TabPaintHelper
    {
        private readonly IBeepTheme _theme;
        private BeepControlStyle _controlStyle = BeepControlStyle.Modern;
        private bool _isTransparent = false;

        public TabPaintHelper(IBeepTheme theme)
        {
            _theme = theme;
            // Default to Modern style for tabs - will be updated by container if needed
            _controlStyle = BeepControlStyle.Modern;
        }
        
        public TabPaintHelper(IBeepTheme theme, BeepControlStyle controlStyle)
        {
            _theme = theme;
            _controlStyle = controlStyle;
        }
        
        public TabPaintHelper(IBeepTheme theme, BeepControlStyle controlStyle, bool isTransparent)
        {
            _theme = theme;
            _controlStyle = controlStyle;
            _isTransparent = isTransparent;
        }
        
        public BeepControlStyle ControlStyle
        {
            get => _controlStyle;
            set => _controlStyle = value;
        }

        public bool IsTransparent
        {
            get => _isTransparent;
            set => _isTransparent = value;
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
                    System.Diagnostics.Debug.WriteLine($"DrawProfessionalTab: Drawing text '{title}' at {textBounds} with color {colors.TextColor}");
                    DrawTabText(g, textBounds, title, font, colors.TextColor, isActive);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"DrawProfessionalTab: Skipping text - bounds too small: {textBounds}");
                }
                
                // Draw close button if needed and there's space
                if (showCloseButton && bounds.Width > 30 && bounds.Height > 16)
                {
                    var closeRect = new Rectangle(bounds.Right - 20, bounds.Y + (bounds.Height - 12) / 2, 12, 12);
                    System.Diagnostics.Debug.WriteLine($"DrawProfessionalTab: Drawing close button at {closeRect}");
                    DrawCloseButton(g, closeRect, isCloseHovered, colors.TextColor);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"DrawProfessionalTab: Skipping close button - bounds too small or disabled: width={bounds.Width}, height={bounds.Height}, showClose={showCloseButton}");
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
            
            // In transparent mode, background is not painted, but we keep colors for borders/text
            // Border and text colors can optionally be adjusted for better visibility on transparent backgrounds
            if (_isTransparent)
            {
                // Make borders more visible on transparent backgrounds
                borderColor = Color.FromArgb(Math.Min(255, borderColor.A + 50), borderColor);
                // Text stays fully opaque for readability
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
            // In transparent mode, skip background painting entirely - let parent show through
            if (_isTransparent)
            {
                // Don't paint background - just return, borders and text will be drawn separately
                return;
            }
            
            // Use BeepStyling for consistent appearance with ControlStyle
            var tabPath = BeepStyling.CreateControlStylePath(bounds, _controlStyle);
            
            try
            {
                // Determine control state based on tab state
                ControlState state = ControlState.Normal;
                if (isActive)
                    state = ControlState.Selected;
                else if (isHovered)
                    state = ControlState.Hovered;
                
                // Use BeepStyling.PaintControl for consistent rendering with the rest of the framework
                // This automatically handles shadow, border, and background based on ControlStyle
                var contentPath = BeepStyling.PaintControl(
                    g,
                    tabPath,
                    _controlStyle,
                    _theme,
                    true, // useThemeColors
                    state,
                    _isTransparent
                );
                
                // Dispose content path if returned
                contentPath?.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DrawTabBackground error: {ex.Message}");
                // Ultimate fallback - simple rectangle fill
                using (var brush = new SolidBrush(colors.BackgroundColor))
                {
                    g.FillRectangle(brush, bounds);
                }
            }
            finally
            {
                tabPath?.Dispose();
            }
        }

        private void DrawTabBorder(Graphics g, Rectangle bounds, Color borderColor, bool isActive)
        {
            // BeepStyling.PaintControl already handles borders based on ControlStyle
            // This method is now a no-op since borders are painted by BeepStyling
            // Keeping it for backward compatibility but it does nothing
            
            // If you need custom border logic beyond what BeepStyling provides,
            // you can add it here, but typically BeepStyling handles all border rendering
        }

        private void DrawTabText(Graphics g, Rectangle textBounds, string title, Font font, Color textColor, bool isActive)
        {
            // Validate parameters before drawing
            if (g == null || string.IsNullOrEmpty(title) || font == null || textBounds.Width <= 0 || textBounds.Height <= 0)
                return;

            // Use TextRenderer for more reliable text rendering (avoids Font.Height exceptions)
            try
            {
                // Determine font style for active tabs
                Font textFont = font;
                if (isActive)
                {
                    try
                    {
                        textFont = new Font(font.FontFamily, font.Size, FontStyle.Bold, font.Unit);
                    }
                    catch
                    {
                        textFont = font; // Fallback to regular font
                    }
                }
                
                // Use TextRenderer for safe, high-quality text rendering
                var flags = TextFormatFlags.Left | 
                           TextFormatFlags.VerticalCenter | 
                           TextFormatFlags.EndEllipsis | 
                           TextFormatFlags.SingleLine |
                           TextFormatFlags.NoPadding;
                
                TextRenderer.DrawText(g, title, textFont, textBounds, textColor, flags);
                
                // Dispose font if we created it
                if (textFont != font && textFont != null)
                {
                    textFont.Dispose();
                }
            }
            catch (Exception ex)
            {
                // Log the error details for debugging
                System.Diagnostics.Debug.WriteLine($"DrawTabText error: {ex.Message}");
                
                // Final fallback to simple text rendering
                try
                {
                    TextRenderer.DrawText(g, title, font, textBounds, textColor, 
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | 
                        TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
                }
                catch
                {
                    // If all else fails, skip drawing this text
                }
            }
        }

        private void DrawCloseButton(Graphics g, Rectangle closeRect, bool isHovered, Color baseColor)
        {
            // Draw close button background on hover with modern rounded appearance
            if (isHovered)
            {
                // Use more modern hover color with better visibility
                using (var bgBrush = new SolidBrush(Color.FromArgb(200, Color.Red)))
                using (var path = CreateRoundedPath(closeRect, Math.Min(3, closeRect.Width / 4)))
                {
                    g.FillPath(bgBrush, path);
                }
            }
            else
            {
                // Subtle background on non-hover for better visibility
                using (var bgBrush = new SolidBrush(Color.FromArgb(30, baseColor)))
                using (var path = CreateRoundedPath(closeRect, Math.Min(3, closeRect.Width / 4)))
                {
                    g.FillPath(bgBrush, path);
                }
            }

            // Draw the X with modern styling
            var color = isHovered ? Color.White : Color.FromArgb(180, baseColor);
            using (var pen = new Pen(color, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                var centerX = closeRect.X + closeRect.Width / 2f;
                var centerY = closeRect.Y + closeRect.Height / 2f;
                var size = Math.Min(closeRect.Width, closeRect.Height) / 3.5f;

                // Draw X with smooth lines
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