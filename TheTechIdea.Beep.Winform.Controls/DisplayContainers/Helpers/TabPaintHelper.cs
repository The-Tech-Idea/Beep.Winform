using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls; // TabStyle enum
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers
{
    /// <summary>
    /// Helper class for professional tab rendering with modern styling using BeepStyling
    /// </summary>
    internal class TabPaintHelper
    {
        private IBeepTheme _theme;
        private BeepControlStyle _controlStyle = BeepControlStyle.Modern;
        private bool _isTransparent = false;
        private TabStyle _tabStyle = TabStyle.Capsule; // default for DC2

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
        public TabStyle TabStyle
        {
            get => _tabStyle;
            set => _tabStyle = value;
        }

        public bool IsTransparent
        {
            get => _isTransparent;
            set => _isTransparent = value;
        }

        public IBeepTheme Theme
        {
            get => _theme;
            set => _theme = value;
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
                
                // Draw tab background with gradient or style-specific background
                switch (_tabStyle)
                {
                    case TabStyle.Classic:
                        DrawTabBackground(g, bounds, colors, isActive, isHovered);
                        break;
                    case TabStyle.Capsule:
                        DrawCapsuleBackground(g, bounds, colors, isActive, isHovered);
                        break;
                    case TabStyle.Underline:
                        // Underline has no background; we'll draw underline separately later
                        break;
                    case TabStyle.Minimal:
                        // Minimal draws no background
                        break;
                    case TabStyle.Segmented:
                        DrawSegmentBackground(g, bounds, colors, isActive, isHovered);
                        break;
                    default:
                        DrawTabBackground(g, bounds, colors, isActive, isHovered);
                        break;
                }
                
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
                // If style is underline or minimal and active, draw underline accent
                if ((_tabStyle == TabStyle.Underline || _tabStyle == TabStyle.Minimal) && isActive)
                {
                    DrawUnderline(g, bounds, colors);
                }
            }
            catch
            {
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

        private void DrawUnderline(Graphics g, Rectangle bounds, TabColors colors)
        {
            // Draw a thin accent underline centered under the tab title region
            int thickness = Math.Max(2, bounds.Height / 8);
            var r = new Rectangle(bounds.X + 8, bounds.Bottom - thickness - 2, Math.Max(10, bounds.Width - 16), thickness);
            using (var brush = new SolidBrush(colors.TextColor))
            {
                g.FillRectangle(brush, r);
            }
        }

        private void DrawCapsuleBackground(Graphics g, Rectangle bounds, TabColors colors, bool isActive, bool isHovered)
        {
            // Draw rounded capsule background
            int radius = Math.Max(6, bounds.Height / 2);
            var path = CreateRoundedPath(bounds, radius);
            using (var brush = new SolidBrush(isActive ? colors.BackgroundColor : Color.FromArgb(240, colors.BackgroundColor)))
            {
                g.FillPath(brush, path);
            }
            DrawTabBorder(g, bounds, colors.BorderColor, isActive);
            path.Dispose();
        }

        private void DrawSegmentBackground(Graphics g, Rectangle bounds, TabColors colors, bool isActive, bool isHovered)
        {
            // Segmented style uses a subtle border and a selected fill
            using (var path = CreateRoundedPath(bounds, 6))
            {
                using (var brush = new SolidBrush(isActive ? colors.BackgroundColor : Color.FromArgb(245, 245, 245)))
                {
                    g.FillPath(brush, path);
                }
                using(var pen = new Pen(colors.BorderColor, 1f))
                {
                    g.DrawPath(pen, path);
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
                
                // Use BeepStyling.PaintControl for consistent rendering
                var contentPath = BeepStyling.PaintControl(
                    g,
                    tabPath,
                    _controlStyle,
                    _theme,
                    true, // useThemeColors
                    state,
                    _isTransparent
                );
                
                contentPath?.Dispose();
            }
            catch
            {
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
            if (g == null || string.IsNullOrEmpty(title) || font == null || textBounds.Width <= 0 || textBounds.Height <= 0)
                return;

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
                        textFont = font;
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
            catch
            {
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
            
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return path;
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