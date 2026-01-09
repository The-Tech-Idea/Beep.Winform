using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// Helper class for calculating tooltip layout rectangles
    /// Handles content layout, arrow positioning, and responsive sizing
    /// </summary>
    public static class ToolTipLayoutHelpers
    {
        /// <summary>
        /// Calculates layout rectangles for tooltip elements
        /// </summary>
        public static ToolTipLayoutMetrics CalculateLayout(
            Rectangle bounds,
            ToolTipConfig config,
            ToolTipPlacement placement,
            bool hasTitle,
            bool hasIcon,
            bool hasImage,
            int padding,
            int spacing)
        {
            var metrics = new ToolTipLayoutMetrics();

            int x = bounds.X + padding;
            int y = bounds.Y + padding;
            int availableWidth = bounds.Width - (padding * 2);
            int availableHeight = bounds.Height - (padding * 2);

            // Icon or image on left
            if (hasIcon || hasImage)
            {
                int iconSize = config.MaxImageSize.Height > 0 ? config.MaxImageSize.Height : 24;
                metrics.IconRect = new Rectangle(x, y, iconSize, iconSize);
                x += iconSize + spacing;
                availableWidth -= (iconSize + spacing);
            }

            // Title
            if (hasTitle)
            {
                int titleHeight = 18; // Approximate, will be measured in control
                metrics.TitleRect = new Rectangle(x, y, availableWidth, titleHeight);
                y += titleHeight + spacing;
            }

            // Text content
            if (!string.IsNullOrEmpty(config.Text))
            {
                metrics.TextRect = new Rectangle(x, y, availableWidth, availableHeight - (y - bounds.Y - padding));
            }

            // Arrow position (calculated based on placement)
            if (config.ShowArrow)
            {
                metrics.ArrowRect = CalculateArrowRect(bounds, placement, config.Offset);
            }

            return metrics;
        }

        /// <summary>
        /// Calculates arrow rectangle based on placement
        /// </summary>
        private static Rectangle CalculateArrowRect(Rectangle bounds, ToolTipPlacement placement, int offset)
        {
            const int arrowSize = 8;
            int arrowX = 0;
            int arrowY = 0;

            switch (placement)
            {
                case ToolTipPlacement.Top:
                case ToolTipPlacement.TopStart:
                case ToolTipPlacement.TopEnd:
                    arrowY = bounds.Bottom - arrowSize;
                    arrowX = bounds.Left + bounds.Width / 2 - arrowSize / 2;
                    break;

                case ToolTipPlacement.Bottom:
                case ToolTipPlacement.BottomStart:
                case ToolTipPlacement.BottomEnd:
                    arrowY = bounds.Top;
                    arrowX = bounds.Left + bounds.Width / 2 - arrowSize / 2;
                    break;

                case ToolTipPlacement.Left:
                case ToolTipPlacement.LeftStart:
                case ToolTipPlacement.LeftEnd:
                    arrowX = bounds.Right - arrowSize;
                    arrowY = bounds.Top + bounds.Height / 2 - arrowSize / 2;
                    break;

                case ToolTipPlacement.Right:
                case ToolTipPlacement.RightStart:
                case ToolTipPlacement.RightEnd:
                    arrowX = bounds.Left;
                    arrowY = bounds.Top + bounds.Height / 2 - arrowSize / 2;
                    break;
            }

            return new Rectangle(arrowX, arrowY, arrowSize, arrowSize);
        }

        /// <summary>
        /// Calculates optimal tooltip size based on content
        /// </summary>
        public static Size CalculateOptimalSize(
            Graphics g,
            ToolTipConfig config,
            int padding,
            int spacing,
            int minWidth,
            int maxWidth)
        {
            int width = minWidth;
            int height = padding * 2;

            // Measure title
            if (!string.IsNullOrEmpty(config.Title))
            {
                var titleFont = new Font("Segoe UI", 10.5f, FontStyle.Bold);
                var titleSize = TextRenderer.MeasureText(g, config.Title, titleFont);
                width = Math.Max(width, (int)titleSize.Width + padding * 2);
                height += (int)titleSize.Height + spacing;
            }

            // Measure text
            if (!string.IsNullOrEmpty(config.Text))
            {
                var textFont = config.Font ?? new Font("Segoe UI", 9.5f);
                var textSize = TextRenderer.MeasureText(g, config.Text, textFont, new Size(maxWidth - padding * 2, int.MaxValue), TextFormatFlags.WordBreak);
                width = Math.Max(width, Math.Min((int)textSize.Width + padding * 2, maxWidth));
                height += (int)textSize.Height;
            }

            // Add icon/image space if present
            if (config.Icon != null || !string.IsNullOrEmpty(config.IconPath) || !string.IsNullOrEmpty(config.ImagePath))
            {
                int iconSize = config.MaxImageSize.Height > 0 ? config.MaxImageSize.Height : 24;
                width += iconSize + spacing;
                height = Math.Max(height, iconSize + padding * 2);
            }

            // Constrain to min/max
            width = Math.Max(minWidth, Math.Min(width, maxWidth));
            height = Math.Max(40, height); // Minimum height

            return new Size(width, height);
        }
    }

    /// <summary>
    /// Layout metrics for tooltip elements
    /// </summary>
    public class ToolTipLayoutMetrics
    {
        public Rectangle IconRect { get; set; } = Rectangle.Empty;
        public Rectangle TitleRect { get; set; } = Rectangle.Empty;
        public Rectangle TextRect { get; set; } = Rectangle.Empty;
        public Rectangle ArrowRect { get; set; } = Rectangle.Empty;
    }
}
