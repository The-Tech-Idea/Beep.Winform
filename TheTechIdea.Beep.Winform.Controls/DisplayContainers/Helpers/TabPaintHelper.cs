using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls; // TabStyle enum
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;

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

        public Control OwnerControl { get; set; }

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

        /// <summary>
        /// Border radius of the owning container.  Used to shape the outer corners of the
        /// first and last visible tabs so they follow the container's rounded outline.
        /// Set to 0 when the container is not rounded.
        /// </summary>
        public int ContainerBorderRadius { get; set; } = 0;

        public IBeepTheme Theme
        {
            get => _theme;
            set => _theme = value;
        }

        /// <summary>
        /// Draws a professional tab with rounded corners, gradients, and theme colors.
        /// </summary>
        /// <param name="isFirst">True when this is the first visible tab — outer corner follows ContainerBorderRadius.</param>
        /// <param name="isLast">True when this is the last visible tab — outer corner follows ContainerBorderRadius.</param>
        /// <param name="tabPosition">Position of the tab strip so outer-corner direction can be determined.</param>
        public void DrawProfessionalTab(Graphics g, Rectangle bounds, string title, Font font,
            bool isActive, bool isHovered, bool showCloseButton, bool isCloseHovered,
            float animationProgress = 0f,
            bool isFirst = false, bool isLast = false,
            TabPosition tabPosition = TabPosition.Top)
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
                        DrawTabBackground(g, bounds, colors, isActive, isHovered, isFirst, isLast, tabPosition);
                        break;
                    case TabStyle.Capsule:
                        DrawCapsuleBackground(g, bounds, colors, isActive, isHovered, isFirst, isLast, tabPosition);
                        break;
                    case TabStyle.Underline:
                        // Underline has no background; we'll draw underline separately later
                        break;
                    case TabStyle.Minimal:
                        // Minimal draws no background
                        break;
                    case TabStyle.Segmented:
                        DrawSegmentBackground(g, bounds, colors, isActive, isHovered, isFirst, isLast, tabPosition);
                        break;
                    default:
                        DrawTabBackground(g, bounds, colors, isActive, isHovered, isFirst, isLast, tabPosition);
                        break;
                }
                
                // Draw tab border
                DrawTabBorder(g, bounds, colors.BorderColor, isActive);

                // Active indicator line — drawn for styles that don't already rely solely
                // on background fill to indicate selection.
                if (isActive)
                {
                    switch (_tabStyle)
                    {
                        case TabStyle.Classic:
                        case TabStyle.Card:
                        case TabStyle.Segmented:
                        case TabStyle.Button:
                            DrawActiveIndicator(g, bounds, tabPosition);
                            break;
                    }
                }
                
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
                int closeW = DpiScalingHelper.ScaleValue(20, OwnerControl);
                int closeSize = DpiScalingHelper.ScaleValue(12, OwnerControl);
                if (showCloseButton && bounds.Width > closeW + 10 && bounds.Height > closeSize + 4)
                {
                    var closeRect = new Rectangle(bounds.Right - closeW, bounds.Y + (bounds.Height - closeSize) / 2, closeSize, closeSize);
                    DrawCloseButton(g, closeRect, isCloseHovered, colors.TextColor);
                }
                // If style is underline or minimal and active, draw underline accent
                if ((_tabStyle == TabStyle.Underline || _tabStyle == TabStyle.Minimal) && isActive)
                {
                    DrawUnderline(g, bounds, colors, tabPosition);
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

        private void DrawUnderline(Graphics g, Rectangle bounds, TabColors colors, TabPosition tabPosition = TabPosition.Top)
        {
            // Draw a thin accent line on the content-facing edge of the tab.
            int thickness = Math.Max(2, bounds.Height / 8);
            Rectangle r;
            switch (tabPosition)
            {
                case TabPosition.Bottom:
                    r = new Rectangle(bounds.X + 8, bounds.Y + 2, Math.Max(10, bounds.Width - 16), thickness);
                    break;
                case TabPosition.Left:
                    r = new Rectangle(bounds.Right - thickness - 2, bounds.Y + 8, thickness, Math.Max(10, bounds.Height - 16));
                    break;
                case TabPosition.Right:
                    r = new Rectangle(bounds.X + 2, bounds.Y + 8, thickness, Math.Max(10, bounds.Height - 16));
                    break;
                default: // Top
                    r = new Rectangle(bounds.X + 8, bounds.Bottom - thickness - 2, Math.Max(10, bounds.Width - 16), thickness);
                    break;
            }
            using (var brush = new SolidBrush(IndicatorColor()))
                g.FillRectangle(brush, r);
        }

        /// <summary>
        /// Draws the active-tab accent indicator line — a short colored bar on the
        /// content-facing edge of the tab, styled after modern browser/IDE tab strips.
        /// Thickness: 3 px (DPI-scaled when an <see cref="OwnerControl"/> is set).
        /// Color: <c>theme.ActiveBorderColor</c> → <c>theme.TabSelectedBorderColor</c> → border fallback.
        /// </summary>
        private void DrawActiveIndicator(Graphics g, Rectangle bounds, TabPosition tabPosition)
        {
            int thickness = DpiScalingHelper.ScaleValue(3, OwnerControl);
            thickness = Math.Max(2, thickness);

            Color color = IndicatorColor();

            // Inset the bar slightly from the tab edges so it sits within the tab bounds.
            int inset = Math.Max(0, DpiScalingHelper.ScaleValue(4, OwnerControl));

            Rectangle bar;
            switch (tabPosition)
            {
                case TabPosition.Bottom:
                    // Tab strip is below content — indicator is at the top edge of the tab.
                    bar = new Rectangle(
                        bounds.X + inset,
                        bounds.Y,
                        Math.Max(4, bounds.Width - inset * 2),
                        thickness);
                    break;
                case TabPosition.Left:
                    // Tab strip is to the left — indicator is on the right edge of the tab.
                    bar = new Rectangle(
                        bounds.Right - thickness,
                        bounds.Y + inset,
                        thickness,
                        Math.Max(4, bounds.Height - inset * 2));
                    break;
                case TabPosition.Right:
                    // Tab strip is to the right — indicator is on the left edge of the tab.
                    bar = new Rectangle(
                        bounds.X,
                        bounds.Y + inset,
                        thickness,
                        Math.Max(4, bounds.Height - inset * 2));
                    break;
                default: // Top (most common)
                    // Tab strip is above content — indicator is at the bottom edge of the tab.
                    bar = new Rectangle(
                        bounds.X + inset,
                        bounds.Bottom - thickness,
                        Math.Max(4, bounds.Width - inset * 2),
                        thickness);
                    break;
            }

            // Round the indicator bar ends for a pill-shaped look.
            int barRadius = thickness / 2;
            if (barRadius >= 1 && bar.Width > barRadius * 2 && bar.Height > barRadius * 2)
            {
                using (var path = CreateRoundedPath(bar, barRadius))
                using (var brush = new SolidBrush(color))
                    g.FillPath(brush, path);
            }
            else
            {
                using (var brush = new SolidBrush(color))
                    g.FillRectangle(brush, bar);
            }
        }

        /// <summary>
        /// Returns the best available accent colour from the current theme to use as the
        /// active-tab indicator.  Preference order:
        /// <list type="number">
        ///   <item><c>ActiveBorderColor</c></item>
        ///   <item><c>TabSelectedBorderColor</c></item>
        ///   <item><c>TabSelectedForeColor</c></item>
        ///   <item>DodgerBlue (hard fallback)</item>
        /// </list>
        /// </summary>
        private Color IndicatorColor()
        {
            var c = _theme?.ActiveBorderColor ?? Color.Empty;
            if (c == Color.Empty || c.A == 0) c = _theme?.TabSelectedBorderColor ?? Color.Empty;
            if (c == Color.Empty || c.A == 0) c = _theme?.TabSelectedForeColor ?? Color.Empty;
            if (c == Color.Empty || c.A == 0) c = Color.DodgerBlue;
            return c;
        }

        private void DrawCapsuleBackground(Graphics g, Rectangle bounds, TabColors colors, bool isActive, bool isHovered,
            bool isFirst = false, bool isLast = false, TabPosition tabPosition = TabPosition.Top)
        {
            // Normal capsule radius (half-height)
            int radius = Math.Max(6, bounds.Height / 2);

            // For the outer corners of first/last tabs use the container radius so they
            // align with the container's overall rounded outline.
            if (ContainerBorderRadius > 0)
            {
                var path = CreateTabCornerPath(bounds, radius, ContainerBorderRadius, isFirst, isLast, tabPosition);
                using (var brush = new SolidBrush(isActive ? colors.BackgroundColor : Color.FromArgb(240, colors.BackgroundColor)))
                    g.FillPath(brush, path);
                DrawTabBorder(g, bounds, colors.BorderColor, isActive);
                path.Dispose();
            }
            else
            {
                var path = CreateRoundedPath(bounds, radius);
                using (var brush = new SolidBrush(isActive ? colors.BackgroundColor : Color.FromArgb(240, colors.BackgroundColor)))
                    g.FillPath(brush, path);
                DrawTabBorder(g, bounds, colors.BorderColor, isActive);
                path.Dispose();
            }
        }

        private void DrawSegmentBackground(Graphics g, Rectangle bounds, TabColors colors, bool isActive, bool isHovered,
            bool isFirst = false, bool isLast = false, TabPosition tabPosition = TabPosition.Top)
        {
            int radius = ContainerBorderRadius > 0 ? 0 : 6; // Segmented style uses minimal rounding
            GraphicsPath path = ContainerBorderRadius > 0
                ? CreateTabCornerPath(bounds, 6, ContainerBorderRadius, isFirst, isLast, tabPosition)
                : CreateRoundedPath(bounds, 6);

            using (path)
            {
                using (var brush = new SolidBrush(isActive ? colors.BackgroundColor : Color.FromArgb(245, 245, 245)))
                    g.FillPath(brush, path);
                using (var pen = new Pen(colors.BorderColor, 1f))
                    g.DrawPath(pen, path);
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

        private void DrawTabBackground(Graphics g, Rectangle bounds, TabColors colors, bool isActive, bool isHovered,
            bool isFirst = false, bool isLast = false, TabPosition tabPosition = TabPosition.Top)
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
                        textFont =FontListHelper.GetFont(font.FontFamily.Name, font.Size, FontStyle.Bold);
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

        /// <summary>
        /// Creates a rounded path for a tab where outer corners (touching the container's
        /// rounded outline) use <paramref name="containerRadius"/> and inner/inner-edge
        /// corners use the normal <paramref name="tabRadius"/>.
        /// </summary>
        private GraphicsPath CreateTabCornerPath(Rectangle bounds, int tabRadius, int containerRadius,
            bool isFirst, bool isLast, TabPosition tabPosition)
        {
            // Determine which two corners are "outer" (touching the container wall)
            // vs "inner" (touching the tab-strip interior / other tabs).
            bool tl, tr, bl, br; // true = use containerRadius
            switch (tabPosition)
            {
                case TabPosition.Top:
                    tl = isFirst; tr = isLast; bl = false; br = false;
                    break;
                case TabPosition.Bottom:
                    tl = false; tr = false; bl = isFirst; br = isLast;
                    break;
                case TabPosition.Left:
                    tl = isFirst; tr = false; bl = isLast; br = false;
                    break;
                case TabPosition.Right:
                    tl = false; tr = isFirst; bl = false; br = isLast;
                    break;
                default:
                    tl = isFirst; tr = isLast; bl = false; br = false;
                    break;
            }

            // Build path: the four arcs use either containerRadius or tabRadius.
            var path = new GraphicsPath();
            if (bounds.Width <= 0 || bounds.Height <= 0) return path;

            int rTL = Math.Min(tl ? containerRadius : tabRadius, Math.Min(bounds.Width / 2, bounds.Height / 2));
            int rTR = Math.Min(tr ? containerRadius : tabRadius, Math.Min(bounds.Width / 2, bounds.Height / 2));
            int rBL = Math.Min(bl ? containerRadius : tabRadius, Math.Min(bounds.Width / 2, bounds.Height / 2));
            int rBR = Math.Min(br ? containerRadius : tabRadius, Math.Min(bounds.Width / 2, bounds.Height / 2));

            try
            {
                if (rTL > 0) path.AddArc(bounds.X, bounds.Y, rTL * 2, rTL * 2, 180, 90);
                path.AddLine(bounds.X + (rTL > 0 ? rTL : 0), bounds.Y, bounds.Right - (rTR > 0 ? rTR : 0), bounds.Y);
                if (rTR > 0) path.AddArc(bounds.Right - rTR * 2, bounds.Y, rTR * 2, rTR * 2, 270, 90);
                path.AddLine(bounds.Right, bounds.Y + (rTR > 0 ? rTR : 0), bounds.Right, bounds.Bottom - (rBR > 0 ? rBR : 0));
                if (rBR > 0) path.AddArc(bounds.Right - rBR * 2, bounds.Bottom - rBR * 2, rBR * 2, rBR * 2, 0, 90);
                path.AddLine(bounds.Right - (rBR > 0 ? rBR : 0), bounds.Bottom, bounds.X + (rBL > 0 ? rBL : 0), bounds.Bottom);
                if (rBL > 0) path.AddArc(bounds.X, bounds.Bottom - rBL * 2, rBL * 2, rBL * 2, 90, 90);
                path.AddLine(bounds.X, bounds.Bottom - (rBL > 0 ? rBL : 0), bounds.X, bounds.Y + (rTL > 0 ? rTL : 0));
                path.CloseFigure();
            }
            catch
            {
                path.Reset();
                path.AddRectangle(bounds);
            }
            return path;
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