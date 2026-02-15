using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Helpers
{
    /// <summary>
    /// Helper class for calculating notification layout rectangles
    /// Handles different layout styles (Standard, Compact, Prominent, Banner, Toast)
    /// </summary>
    public static class NotificationLayoutHelpers
    {
        private static int ScaleVal(int value, Control ctrl)
            => ctrl != null ? DpiScalingHelper.ScaleValue(value, ctrl) : value;

        /// <summary>
        /// Calculates layout rectangles for notification elements
        /// </summary>
        public static NotificationLayoutMetrics CalculateLayout(
            Rectangle bounds,
            NotificationLayout layout,
            bool hasIcon,
            bool hasTitle,
            bool hasMessage,
            bool hasActions,
            bool showCloseButton,
            bool showProgressBar,
            int iconSize,
            int padding,
            int spacing,
            Control ownerControl = null)
        {
            var metrics = new NotificationLayoutMetrics();

            int x = bounds.X + padding;
            int y = bounds.Y + padding;
            int availableWidth = bounds.Width - (padding * 2);
            int availableHeight = bounds.Height - (padding * 2);

            if (showCloseButton)
            {
                int closeButtonSize = ScaleVal(20, ownerControl);
                metrics.CloseButtonRect = new Rectangle(
                    bounds.Right - padding - closeButtonSize,
                    y,
                    closeButtonSize,
                    closeButtonSize
                );
                availableWidth -= (closeButtonSize + spacing);
            }

            switch (layout)
            {
                case NotificationLayout.Standard:
                    metrics = CalculateStandardLayout(bounds, hasIcon, hasTitle, hasMessage, hasActions, showProgressBar, iconSize, padding, spacing);
                    break;

                case NotificationLayout.Compact:
                    metrics = CalculateCompactLayout(bounds, hasIcon, hasTitle, hasMessage, hasActions, showProgressBar, iconSize, padding, spacing);
                    break;

                case NotificationLayout.Prominent:
                    metrics = CalculateProminentLayout(bounds, hasIcon, hasTitle, hasMessage, hasActions, showProgressBar, iconSize, padding, spacing);
                    break;

                case NotificationLayout.Banner:
                    metrics = CalculateBannerLayout(bounds, hasIcon, hasTitle, hasMessage, hasActions, showProgressBar, iconSize, padding, spacing);
                    break;

                case NotificationLayout.Toast:
                    metrics = CalculateToastLayout(bounds, hasIcon, hasTitle, hasMessage, hasActions, showProgressBar, iconSize, padding, spacing);
                    break;
            }

            // Adjust close button position if needed
            if (showCloseButton && metrics.CloseButtonRect.IsEmpty)
            {
                int closeButtonSize = ScaleVal(20, ownerControl);
                metrics.CloseButtonRect = new Rectangle(
                    bounds.Right - padding - closeButtonSize,
                    bounds.Y + padding,
                    closeButtonSize,
                    closeButtonSize
                );
            }

            // Progress bar at bottom
            if (showProgressBar)
            {
                int progressBarHeight = ScaleVal(4, ownerControl);
                metrics.ProgressBarRect = new Rectangle(
                    bounds.X,
                    bounds.Bottom - progressBarHeight,
                    bounds.Width,
                    progressBarHeight
                );
            }

            return metrics;
        }

        private static NotificationLayoutMetrics CalculateStandardLayout(
            Rectangle bounds, bool hasIcon, bool hasTitle, bool hasMessage, bool hasActions,
            bool showProgressBar, int iconSize, int padding, int spacing, Control ownerControl = null)
        {
            var metrics = new NotificationLayoutMetrics();
            int x = bounds.X + padding;
            int y = bounds.Y + padding;
            int availableWidth = bounds.Width - (padding * 2);

            // Icon on left
            if (hasIcon)
            {
                metrics.IconRect = new Rectangle(x, y, iconSize, iconSize);
                x += iconSize + spacing;
                availableWidth -= (iconSize + spacing);
            }

            // Title and message
            int titleHeight = ScaleVal(20, ownerControl);
            int titleSpacing = ScaleVal(4, ownerControl);
            if (hasTitle)
            {
                // Title will be measured in the control
                metrics.TitleRect = new Rectangle(x, y, availableWidth, titleHeight);
                y += titleHeight + titleSpacing;
            }

            int progressBarHeight = ScaleVal(4, ownerControl);
            if (hasMessage)
            {
                // Message will be measured in the control
                metrics.MessageRect = new Rectangle(x, y, availableWidth, bounds.Height - y - padding - (showProgressBar ? progressBarHeight : 0));
            }

            // Actions below
            if (hasActions)
            {
                int actionHeight = ScaleVal(32, ownerControl);
                metrics.ActionsRect = new Rectangle(x, bounds.Bottom - padding - actionHeight - (showProgressBar ? progressBarHeight : 0), availableWidth, actionHeight);
            }

            return metrics;
        }

        private static NotificationLayoutMetrics CalculateCompactLayout(
            Rectangle bounds, bool hasIcon, bool hasTitle, bool hasMessage, bool hasActions,
            bool showProgressBar, int iconSize, int padding, int spacing, Control ownerControl = null)
        {
            var metrics = new NotificationLayoutMetrics();
            int x = bounds.X + padding;
            int y = bounds.Y + padding;
            int availableWidth = bounds.Width - (padding * 2);

            // Icon and text inline
            if (hasIcon)
            {
                metrics.IconRect = new Rectangle(x, y, iconSize, iconSize);
                x += iconSize + spacing;
                availableWidth -= (iconSize + spacing);
            }

            int compactHeight = ScaleVal(16, ownerControl);
            // Title and message on same line if possible
            if (hasTitle)
            {
                metrics.TitleRect = new Rectangle(x, y, availableWidth / 2, compactHeight);
            }

            if (hasMessage)
            {
                metrics.MessageRect = new Rectangle(x + (hasTitle ? availableWidth / 2 + spacing : 0), y, availableWidth - (hasTitle ? availableWidth / 2 + spacing : 0), compactHeight);
            }

            return metrics;
        }

        private static NotificationLayoutMetrics CalculateProminentLayout(
            Rectangle bounds, bool hasIcon, bool hasTitle, bool hasMessage, bool hasActions,
            bool showProgressBar, int iconSize, int padding, int spacing, Control ownerControl = null)
        {
            var metrics = new NotificationLayoutMetrics();
            int x = bounds.X + padding;
            int y = bounds.Y + padding;
            int availableWidth = bounds.Width - (padding * 2);

            // Large icon centered
            int largeIconSize = Math.Max(iconSize, ScaleVal(32, ownerControl));
            if (hasIcon)
            {
                metrics.IconRect = new Rectangle(
                    bounds.X + (bounds.Width - largeIconSize) / 2,
                    y,
                    largeIconSize,
                    largeIconSize
                );
                y += largeIconSize + spacing;
            }

            int prominentTitleHeight = ScaleVal(24, ownerControl);
            int prominentSpacing = ScaleVal(4, ownerControl);
            // Title centered
            if (hasTitle)
            {
                metrics.TitleRect = new Rectangle(x, y, availableWidth, prominentTitleHeight);
                y += prominentTitleHeight + prominentSpacing;
            }

            // Message centered
            int progressBarH = ScaleVal(4, ownerControl);
            if (hasMessage)
            {
                metrics.MessageRect = new Rectangle(x, y, availableWidth, bounds.Height - y - padding - (showProgressBar ? progressBarH : 0));
            }

            return metrics;
        }

        private static NotificationLayoutMetrics CalculateBannerLayout(
            Rectangle bounds, bool hasIcon, bool hasTitle, bool hasMessage, bool hasActions,
            bool showProgressBar, int iconSize, int padding, int spacing, Control ownerControl = null)
        {
            var metrics = new NotificationLayoutMetrics();
            int x = bounds.X + padding;
            int y = bounds.Y + padding;
            int availableWidth = bounds.Width - (padding * 2);

            // Icon on left
            if (hasIcon)
            {
                metrics.IconRect = new Rectangle(x, y, iconSize, iconSize);
                x += iconSize + spacing;
                availableWidth -= (iconSize + spacing);
            }

            // Title and message on same line
            if (hasTitle)
            {
                metrics.TitleRect = new Rectangle(x, y, availableWidth / 2, bounds.Height - (padding * 2));
            }

            if (hasMessage)
            {
                metrics.MessageRect = new Rectangle(x + (hasTitle ? availableWidth / 2 + spacing : 0), y, availableWidth - (hasTitle ? availableWidth / 2 + spacing : 0), bounds.Height - (padding * 2));
            }

            return metrics;
        }

        private static NotificationLayoutMetrics CalculateToastLayout(
            Rectangle bounds, bool hasIcon, bool hasTitle, bool hasMessage, bool hasActions,
            bool showProgressBar, int iconSize, int padding, int spacing, Control ownerControl = null)
        {
            var metrics = new NotificationLayoutMetrics();
            int x = bounds.X + padding;
            int y = bounds.Y + padding;
            int availableWidth = bounds.Width - (padding * 2);

            // Small icon on left
            if (hasIcon)
            {
                metrics.IconRect = new Rectangle(x, y, iconSize, iconSize);
                x += iconSize + spacing;
                availableWidth -= (iconSize + spacing);
            }

            // Title only (compact)
            if (hasTitle)
            {
                metrics.TitleRect = new Rectangle(x, y, availableWidth, bounds.Height - (padding * 2));
            }

            return metrics;
        }
    }

    /// <summary>
    /// Layout metrics for notification elements
    /// </summary>
    public class NotificationLayoutMetrics
    {
        public Rectangle IconRect { get; set; } = Rectangle.Empty;
        public Rectangle TitleRect { get; set; } = Rectangle.Empty;
        public Rectangle MessageRect { get; set; } = Rectangle.Empty;
        public Rectangle ActionsRect { get; set; } = Rectangle.Empty;
        public Rectangle CloseButtonRect { get; set; } = Rectangle.Empty;
        public Rectangle ProgressBarRect { get; set; } = Rectangle.Empty;
    }
}
