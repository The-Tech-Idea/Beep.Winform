using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers
{
    /// <summary>
    /// Helper class for BeepAdvancedButton with layout and calculation utilities
    /// </summary>
    public class BeepAdvancedButtonHelper
    {
        private readonly BeepAdvancedButton _button;

        public BeepAdvancedButtonHelper(BeepAdvancedButton button)
        {
            _button = button ?? throw new ArgumentNullException(nameof(button));
        }

        /// <summary>
        /// Calculate the ideal width for the button based on content
        /// </summary>
        public int CalculateIdealWidth(string text, bool hasIcon, AdvancedButtonSize size)
        {
            var metrics = AdvancedButtonMetrics.GetMetrics(size);
            int width = metrics.PaddingHorizontal * 2;

            if (hasIcon)
            {
                width += metrics.IconSize;
            }

            if (!string.IsNullOrEmpty(text))
            {
                if (hasIcon)
                {
                    width += metrics.IconTextGap;
                }
                
                width += MeasureTextWidth(text, _button.Font);
            }

            return Math.Max(width, metrics.MinWidth);
        }

        /// <summary>
        /// Calculate content bounds for icon and text
        /// </summary>
        public void CalculateContentBounds(
            Rectangle buttonBounds,
            string text,
            bool hasIcon,
            AdvancedButtonSize size,
            out Rectangle iconBounds,
            out Rectangle textBounds)
        {
            var metrics = AdvancedButtonMetrics.GetMetrics(size);
            
            if (hasIcon && !string.IsNullOrEmpty(text))
            {
                // Both icon and text
                int textWidth = MeasureTextWidth(text, _button.Font);
                int totalWidth = metrics.IconSize + metrics.IconTextGap + textWidth;
                int startX = buttonBounds.X + (buttonBounds.Width - totalWidth) / 2;

                iconBounds = new Rectangle(
                    startX,
                    buttonBounds.Y + (buttonBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );

                textBounds = new Rectangle(
                    startX + metrics.IconSize + metrics.IconTextGap,
                    buttonBounds.Y,
                    textWidth,
                    buttonBounds.Height
                );
            }
            else if (hasIcon)
            {
                // Icon only
                iconBounds = new Rectangle(
                    buttonBounds.X + (buttonBounds.Width - metrics.IconSize) / 2,
                    buttonBounds.Y + (buttonBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                textBounds = Rectangle.Empty;
            }
            else
            {
                // Text only
                iconBounds = Rectangle.Empty;
                textBounds = new Rectangle(
                    buttonBounds.X + metrics.PaddingHorizontal,
                    buttonBounds.Y,
                    buttonBounds.Width - metrics.PaddingHorizontal * 2,
                    buttonBounds.Height
                );
            }
        }

        /// <summary>
        /// Check if a point is within the button's clickable area
        /// </summary>
        public bool IsPointInButton(Point point, Rectangle bounds, int borderRadius)
        {
            // For rounded rectangles, check if point is in corners
            if (!bounds.Contains(point))
                return false;

            // Quick check for non-corner areas
            Rectangle innerRect = new Rectangle(
                bounds.X + borderRadius,
                bounds.Y + borderRadius,
                bounds.Width - borderRadius * 2,
                bounds.Height - borderRadius * 2
            );

            if (innerRect.Contains(point))
                return true;

            // Check corners
            return IsPointInRoundedCorner(point, bounds, borderRadius);
        }

        /// <summary>
        /// Get appropriate cursor for button state
        /// </summary>
        public System.Windows.Forms.Cursor GetCursor(bool isEnabled, AdvancedButtonStyle style)
        {
            if (!isEnabled)
                return System.Windows.Forms.Cursors.No;

            return style == AdvancedButtonStyle.Link 
                ? System.Windows.Forms.Cursors.Hand 
                : System.Windows.Forms.Cursors.Default;
        }

        /// <summary>
        /// Calculate shadow offset based on button state
        /// </summary>
        public Point GetShadowOffset(AdvancedButtonState state)
        {
            return state switch
            {
                AdvancedButtonState.Pressed => new Point(1, 1),
                AdvancedButtonState.Hover => new Point(2, 3),
                _ => new Point(2, 4)
            };
        }

        #region "Private Helper Methods"

        private int MeasureTextWidth(string text, Font font)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            using (var bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                SizeF size = g.MeasureString(text, font);
                return (int)Math.Ceiling(size.Width);
            }
        }

        private bool IsPointInRoundedCorner(Point point, Rectangle bounds, int radius)
        {
            // Check each corner
            // Top-left
            if (point.X < bounds.X + radius && point.Y < bounds.Y + radius)
            {
                return IsPointInCircle(point, new Point(bounds.X + radius, bounds.Y + radius), radius);
            }
            
            // Top-right
            if (point.X > bounds.Right - radius && point.Y < bounds.Y + radius)
            {
                return IsPointInCircle(point, new Point(bounds.Right - radius, bounds.Y + radius), radius);
            }
            
            // Bottom-left
            if (point.X < bounds.X + radius && point.Y > bounds.Bottom - radius)
            {
                return IsPointInCircle(point, new Point(bounds.X + radius, bounds.Bottom - radius), radius);
            }
            
            // Bottom-right
            if (point.X > bounds.Right - radius && point.Y > bounds.Bottom - radius)
            {
                return IsPointInCircle(point, new Point(bounds.Right - radius, bounds.Bottom - radius), radius);
            }

            return true;
        }

        private bool IsPointInCircle(Point point, Point center, int radius)
        {
            int dx = point.X - center.X;
            int dy = point.Y - center.Y;
            return (dx * dx + dy * dy) <= (radius * radius);
        }

        #endregion
    }
}
