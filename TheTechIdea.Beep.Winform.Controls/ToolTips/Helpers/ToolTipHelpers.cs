using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// Helper utilities for tooltip positioning, sizing, and common operations
    /// </summary>
    public static class ToolTipHelpers
    {
        /// <summary>
        /// Calculate optimal position for tooltip based on placement and screen bounds
        /// </summary>
        public static Point CalculateOptimalPosition(Point targetPosition, Size tooltipSize, 
            ToolTipPlacement preferredPlacement, int offset = 8)
        {
            var screen = Screen.FromPoint(targetPosition);
            var screenBounds = screen.WorkingArea;
            
            // Try preferred placement first
            var position = CalculatePositionForPlacement(targetPosition, tooltipSize, preferredPlacement, offset);
            var bounds = new Rectangle(position, tooltipSize);
            
            // If it fits, use it
            if (screenBounds.Contains(bounds))
            {
                return position;
            }
            
            // Try alternative placements
            var placements = new[]
            {
                ToolTipPlacement.Top, ToolTipPlacement.Bottom,
                ToolTipPlacement.Right, ToolTipPlacement.Left,
                ToolTipPlacement.TopStart, ToolTipPlacement.BottomStart
            };
            
            foreach (var placement in placements)
            {
                position = CalculatePositionForPlacement(targetPosition, tooltipSize, placement, offset);
                bounds = new Rectangle(position, tooltipSize);
                
                if (screenBounds.Contains(bounds))
                {
                    return position;
                }
            }
            
            // Fallback: clamp to screen
            position = CalculatePositionForPlacement(targetPosition, tooltipSize, ToolTipPlacement.Top, offset);
            position.X = Math.Max(screenBounds.Left, Math.Min(position.X, screenBounds.Right - tooltipSize.Width));
            position.Y = Math.Max(screenBounds.Top, Math.Min(position.Y, screenBounds.Bottom - tooltipSize.Height));
            
            return position;
        }

        /// <summary>
        /// Calculate position for specific placement
        /// </summary>
        public static Point CalculatePositionForPlacement(Point target, Size tooltipSize, 
            ToolTipPlacement placement, int offset)
        {
            return placement switch
            {
                ToolTipPlacement.Top => new Point(target.X - tooltipSize.Width / 2, target.Y - tooltipSize.Height - offset),
                ToolTipPlacement.TopStart => new Point(target.X, target.Y - tooltipSize.Height - offset),
                ToolTipPlacement.TopEnd => new Point(target.X - tooltipSize.Width, target.Y - tooltipSize.Height - offset),
                ToolTipPlacement.Bottom => new Point(target.X - tooltipSize.Width / 2, target.Y + offset),
                ToolTipPlacement.BottomStart => new Point(target.X, target.Y + offset),
                ToolTipPlacement.BottomEnd => new Point(target.X - tooltipSize.Width, target.Y + offset),
                ToolTipPlacement.Left => new Point(target.X - tooltipSize.Width - offset, target.Y - tooltipSize.Height / 2),
                ToolTipPlacement.LeftStart => new Point(target.X - tooltipSize.Width - offset, target.Y),
                ToolTipPlacement.LeftEnd => new Point(target.X - tooltipSize.Width - offset, target.Y - tooltipSize.Height),
                ToolTipPlacement.Right => new Point(target.X + offset, target.Y - tooltipSize.Height / 2),
                ToolTipPlacement.RightStart => new Point(target.X + offset, target.Y),
                ToolTipPlacement.RightEnd => new Point(target.X + offset, target.Y - tooltipSize.Height),
                _ => new Point(target.X - tooltipSize.Width / 2, target.Y - tooltipSize.Height - offset)
            };
        }

        /// <summary>
        /// Calculate arrow position based on placement
        /// </summary>
        public static Point CalculateArrowPosition(Rectangle tooltipBounds, ToolTipPlacement placement, int arrowSize)
        {
            return placement switch
            {
                ToolTipPlacement.Top or ToolTipPlacement.TopStart or ToolTipPlacement.TopEnd =>
                    new Point(tooltipBounds.Left + tooltipBounds.Width / 2, tooltipBounds.Bottom),
                
                ToolTipPlacement.Bottom or ToolTipPlacement.BottomStart or ToolTipPlacement.BottomEnd =>
                    new Point(tooltipBounds.Left + tooltipBounds.Width / 2, tooltipBounds.Top),
                
                ToolTipPlacement.Left or ToolTipPlacement.LeftStart or ToolTipPlacement.LeftEnd =>
                    new Point(tooltipBounds.Right, tooltipBounds.Top + tooltipBounds.Height / 2),
                
                ToolTipPlacement.Right or ToolTipPlacement.RightStart or ToolTipPlacement.RightEnd =>
                    new Point(tooltipBounds.Left, tooltipBounds.Top + tooltipBounds.Height / 2),
                
                _ => new Point(tooltipBounds.Left + tooltipBounds.Width / 2, tooltipBounds.Bottom)
            };
        }

        /// <summary>
        /// Create arrow path for tooltip
        /// </summary>
        public static GraphicsPath CreateArrowPath(Point position, ToolTipPlacement placement, int arrowSize)
        {
            var path = new GraphicsPath();
            
            switch (placement)
            {
                case ToolTipPlacement.Top:
                case ToolTipPlacement.TopStart:
                case ToolTipPlacement.TopEnd:
                    // Arrow pointing down
                    path.AddPolygon(new[]
                    {
                        new Point(position.X - arrowSize, position.Y),
                        new Point(position.X, position.Y + arrowSize),
                        new Point(position.X + arrowSize, position.Y)
                    });
                    break;
                
                case ToolTipPlacement.Bottom:
                case ToolTipPlacement.BottomStart:
                case ToolTipPlacement.BottomEnd:
                    // Arrow pointing up
                    path.AddPolygon(new[]
                    {
                        new Point(position.X - arrowSize, position.Y),
                        new Point(position.X, position.Y - arrowSize),
                        new Point(position.X + arrowSize, position.Y)
                    });
                    break;
                
                case ToolTipPlacement.Left:
                case ToolTipPlacement.LeftStart:
                case ToolTipPlacement.LeftEnd:
                    // Arrow pointing right
                    path.AddPolygon(new[]
                    {
                        new Point(position.X, position.Y - arrowSize),
                        new Point(position.X + arrowSize, position.Y),
                        new Point(position.X, position.Y + arrowSize)
                    });
                    break;
                
                case ToolTipPlacement.Right:
                case ToolTipPlacement.RightStart:
                case ToolTipPlacement.RightEnd:
                    // Arrow pointing left
                    path.AddPolygon(new[]
                    {
                        new Point(position.X, position.Y - arrowSize),
                        new Point(position.X - arrowSize, position.Y),
                        new Point(position.X, position.Y + arrowSize)
                    });
                    break;
            }
            
            return path;
        }

        /// <summary>
        /// Measure tooltip content size
        /// </summary>
        public static Size MeasureContentSize(Graphics g, ToolTipConfig config, Font font, int maxWidth = 300)
        {
            var textSize = Size.Empty;
            var titleSize = Size.Empty;
            
            if (!string.IsNullOrEmpty(config.Text))
            {
                textSize = TextRenderer.MeasureText(g, config.Text, font,
                    new Size(maxWidth, int.MaxValue), TextFormatFlags.WordBreak);
            }
            
            if (!string.IsNullOrEmpty(config.Title))
            {
                var titleFont = new Font(font.FontFamily, font.Size + 2, FontStyle.Bold);
                titleSize = TextRenderer.MeasureText(g, config.Title, titleFont);
            }
            
            // Calculate total size with padding
            var padding = 12;
            var iconSpace = (config.Icon != null || !string.IsNullOrEmpty(config.IconPath) || !string.IsNullOrEmpty(config.ImagePath)) ? 28 : 0;
            var titleSpacing = !string.IsNullOrEmpty(config.Title) ? 6 : 0;
            var closeButtonSpace = config.Closable ? 24 : 0;
            
            var width = Math.Max(textSize.Width, titleSize.Width) + padding * 2 + iconSpace + closeButtonSpace;
            var height = textSize.Height + titleSize.Height + titleSpacing + padding * 2;
            
            // Apply constraints
            if (config.MaxSize.HasValue)
            {
                width = Math.Min(width, config.MaxSize.Value.Width);
                height = Math.Min(height, config.MaxSize.Value.Height);
            }
            
            return new Size(width, height);
        }

        /// <summary>
        /// Get theme colors for tooltip from IBeepTheme
        /// </summary>
        public static (Color backColor, Color foreColor, Color borderColor) GetThemeColors(IBeepTheme theme, ToolTipType tooltipType)
        {
            if (theme == null)
            {
                // Fallback to default dark theme
                return (Color.FromArgb(45, 45, 48), Color.FromArgb(241, 241, 241), Color.FromArgb(60, 60, 60));
            }

            return tooltipType switch
            {
                ToolTipType.Default => (theme.ToolTipBackColor, theme.ToolTipForeColor, theme.BorderColor),
                ToolTipType.Info => (theme.DialogInformationButtonForeColor, theme.DialogInformationButtonForeColor, theme.BorderColor),
                ToolTipType.Warning => (theme.WarningColor, theme.WarningColor, theme.BorderColor),
                ToolTipType.Error => (theme.ErrorColor, theme.ErrorColor, theme.BorderColor),
                ToolTipType.Success => (theme.SuccessColor, theme.SuccessColor, theme.BorderColor),

                _ => (theme.ButtonBackColor, theme.ButtonForeColor, theme.BorderColor)
            };
        }

        /// <summary>
        /// Apply easing function for animations
        /// </summary>
        public static double EaseOutCubic(double t)
        {
            return 1 - Math.Pow(1 - t, 3);
        }

        public static double EaseInOutCubic(double t)
        {
            return t < 0.5 ? 4 * t * t * t : 1 - Math.Pow(-2 * t + 2, 3) / 2;
        }

        public static double EaseBounce(double t)
        {
            const double n1 = 7.5625;
            const double d1 = 2.75;

            if (t < 1 / d1)
                return n1 * t * t;
            else if (t < 2 / d1)
                return n1 * (t -= 1.5 / d1) * t + 0.75;
            else if (t < 2.5 / d1)
                return n1 * (t -= 2.25 / d1) * t + 0.9375;
            else
                return n1 * (t -= 2.625 / d1) * t + 0.984375;
        }
    }
}
