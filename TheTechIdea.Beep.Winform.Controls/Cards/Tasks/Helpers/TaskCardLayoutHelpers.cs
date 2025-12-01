using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Tasks.Helpers
{
    /// <summary>
    /// Centralized layout management for TaskCard controls
    /// Provides responsive layout calculations
    /// </summary>
    public static class TaskCardLayoutHelpers
    {
        #region Layout Calculations

        /// <summary>
        /// Calculate avatar bounds for overlapping avatars
        /// </summary>
        public static Rectangle CalculateAvatarBounds(
            Rectangle cardBounds,
            int avatarIndex,
            int avatarCount,
            Size avatarSize,
            int overlap,
            Padding padding)
        {
            int avatarX = cardBounds.Left + padding.Left;
            int avatarY = cardBounds.Top + padding.Top;
            int offsetX = avatarX + avatarIndex * (avatarSize.Width - overlap);

            return new Rectangle(offsetX, avatarY, avatarSize.Width, avatarSize.Height);
        }

        /// <summary>
        /// Calculate more icon bounds at top-right
        /// </summary>
        public static Rectangle CalculateMoreIconBounds(
            Rectangle cardBounds,
            Size iconSize,
            Padding padding)
        {
            return new Rectangle(
                cardBounds.Right - padding.Right - iconSize.Width,
                cardBounds.Top + padding.Top,
                iconSize.Width,
                iconSize.Height);
        }

        /// <summary>
        /// Calculate title bounds below avatar area
        /// </summary>
        public static Rectangle CalculateTitleBounds(
            Rectangle cardBounds,
            Size avatarArea,
            Padding padding)
        {
            return new Rectangle(
                cardBounds.Left + padding.Left,
                cardBounds.Top + padding.Top + avatarArea.Height + 10,
                cardBounds.Width - padding.Horizontal,
                24);
        }

        /// <summary>
        /// Calculate subtitle bounds below title
        /// </summary>
        public static Rectangle CalculateSubtitleBounds(
            Rectangle cardBounds,
            Size titleSize,
            Padding padding)
        {
            return new Rectangle(
                cardBounds.Left + padding.Left,
                cardBounds.Top + padding.Top + titleSize.Height + 34,
                cardBounds.Width - padding.Horizontal,
                20);
        }

        /// <summary>
        /// Calculate metric bounds near bottom
        /// </summary>
        public static Rectangle CalculateMetricBounds(
            Rectangle cardBounds,
            Padding padding)
        {
            return new Rectangle(
                cardBounds.Left + padding.Left,
                cardBounds.Bottom - 40,
                cardBounds.Width - padding.Horizontal,
                20);
        }

        /// <summary>
        /// Calculate progress bar bounds below metric
        /// </summary>
        public static Rectangle CalculateProgressBarBounds(
            Rectangle cardBounds,
            Size metricSize,
            Padding padding)
        {
            int barHeight = 6;
            int barX = cardBounds.Left + padding.Left;
            int barY = cardBounds.Bottom - 20;
            int barWidth = cardBounds.Width - barX - padding.Right;

            return new Rectangle(barX, barY, barWidth, barHeight);
        }

        /// <summary>
        /// Get optimal card size based on content
        /// </summary>
        public static Size GetOptimalCardSize(Padding padding)
        {
            return new Size(180, 240); // Default task card size
        }

        /// <summary>
        /// Calculate layout for all elements
        /// Returns a layout structure with all element bounds
        /// </summary>
        public static TaskCardLayout CalculateLayout(
            Rectangle cardBounds,
            int avatarCount,
            Size avatarSize,
            int overlap,
            Size iconSize,
            Padding padding)
        {
            int maxVisibleAvatars = 3;
            int displayedCount = Math.Min(avatarCount, maxVisibleAvatars);
            Size avatarArea = new Size(displayedCount * (avatarSize.Width - overlap) + overlap, avatarSize.Height);

            Rectangle[] avatarBounds = new Rectangle[displayedCount];
            for (int i = 0; i < displayedCount; i++)
            {
                avatarBounds[i] = CalculateAvatarBounds(cardBounds, i, avatarCount, avatarSize, overlap, padding);
            }

            var moreIconBounds = CalculateMoreIconBounds(cardBounds, iconSize, padding);
            var titleBounds = CalculateTitleBounds(cardBounds, avatarArea, padding);
            var subtitleBounds = CalculateSubtitleBounds(cardBounds, titleBounds.Size, padding);
            var metricBounds = CalculateMetricBounds(cardBounds, padding);
            var progressBarBounds = CalculateProgressBarBounds(cardBounds, metricBounds.Size, padding);

            return new TaskCardLayout
            {
                AvatarBounds = avatarBounds,
                MoreIconBounds = moreIconBounds,
                TitleBounds = titleBounds,
                SubtitleBounds = subtitleBounds,
                MetricBounds = metricBounds,
                ProgressBarBounds = progressBarBounds
            };
        }

        #endregion
    }

    /// <summary>
    /// Layout structure for task card elements
    /// </summary>
    public class TaskCardLayout
    {
        public Rectangle[] AvatarBounds { get; set; }
        public Rectangle MoreIconBounds { get; set; }
        public Rectangle TitleBounds { get; set; }
        public Rectangle SubtitleBounds { get; set; }
        public Rectangle MetricBounds { get; set; }
        public Rectangle ProgressBarBounds { get; set; }
    }
}

