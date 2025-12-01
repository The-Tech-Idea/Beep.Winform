using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Features.Helpers
{
    /// <summary>
    /// Centralized layout management for FeatureCard controls
    /// Provides responsive layout calculations
    /// </summary>
    public static class FeatureCardLayoutHelpers
    {
        #region Layout Calculations

        /// <summary>
        /// Calculate logo bounds within card bounds
        /// </summary>
        public static Rectangle CalculateLogoBounds(
            Rectangle cardBounds,
            Size logoSize,
            Padding padding)
        {
            return new Rectangle(
                cardBounds.Left + padding.Left,
                cardBounds.Top + padding.Top,
                logoSize.Width,
                logoSize.Height);
        }

        /// <summary>
        /// Calculate title bounds based on logo position
        /// </summary>
        public static Rectangle CalculateTitleBounds(
            Rectangle cardBounds,
            Size logoSize,
            Size titleSize,
            Padding padding,
            int logoTitleSpacing = 10)
        {
            int titleX = cardBounds.Left + padding.Left + logoSize.Width + logoTitleSpacing;
            int titleY = cardBounds.Top + padding.Top;

            return new Rectangle(
                titleX,
                titleY,
                Math.Min(titleSize.Width, cardBounds.Width - titleX - padding.Right),
                titleSize.Height);
        }

        /// <summary>
        /// Calculate subtitle bounds based on title position
        /// </summary>
        public static Rectangle CalculateSubtitleBounds(
            Rectangle cardBounds,
            Size titleSize,
            Size subtitleSize,
            int titleX,
            int titleY,
            Padding padding,
            int titleSubtitleSpacing = 2)
        {
            int subtitleX = titleX;
            int subtitleY = titleY + titleSize.Height + titleSubtitleSpacing;

            return new Rectangle(
                subtitleX,
                subtitleY,
                Math.Min(subtitleSize.Width, cardBounds.Width - subtitleX - padding.Right),
                subtitleSize.Height);
        }

        /// <summary>
        /// Calculate action icons bounds at top right
        /// </summary>
        public static Rectangle[] CalculateActionIconsBounds(
            Rectangle cardBounds,
            Size iconSize,
            int iconCount,
            Padding padding,
            int iconSpacing = 5)
        {
            Rectangle[] bounds = new Rectangle[iconCount];
            int startX = cardBounds.Right - padding.Right - iconSize.Width;

            for (int i = 0; i < iconCount; i++)
            {
                int iconX = startX - (iconCount - 1 - i) * (iconSize.Width + iconSpacing);
                bounds[i] = new Rectangle(
                    iconX,
                    cardBounds.Top + padding.Top,
                    iconSize.Width,
                    iconSize.Height);
            }

            return bounds;
        }

        /// <summary>
        /// Calculate card icon bounds at top right
        /// </summary>
        public static Rectangle CalculateCardIconBounds(
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
        /// Calculate features list bounds below subtitle
        /// </summary>
        public static Rectangle CalculateFeaturesListBounds(
            Rectangle cardBounds,
            Size subtitleSize,
            int itemCount,
            int itemHeight,
            Padding padding,
            int subtitleListSpacing = 10)
        {
            int listTop = cardBounds.Top + padding.Top + subtitleSize.Height + subtitleListSpacing;
            int listHeight = itemCount * itemHeight;
            int availableHeight = cardBounds.Height - listTop - padding.Bottom;

            return new Rectangle(
                cardBounds.Left + padding.Left,
                listTop,
                cardBounds.Width - padding.Horizontal,
                Math.Min(listHeight, availableHeight));
        }

        /// <summary>
        /// Get optimal card size based on content
        /// </summary>
        public static Size GetOptimalCardSize(
            int itemCount,
            int itemHeight,
            Padding padding,
            Size logoSize,
            Size titleSize,
            Size subtitleSize,
            Size cardIconSize,
            int spacing = 10)
        {
            int width = Math.Max(300, titleSize.Width + logoSize.Width + cardIconSize.Width + padding.Horizontal + spacing * 2);
            int height = padding.Vertical 
                + Math.Max(logoSize.Height, Math.Max(titleSize.Height, cardIconSize.Height))
                + spacing
                + subtitleSize.Height
                + spacing
                + (itemCount * itemHeight)
                + spacing;

            return new Size(width, height);
        }

        /// <summary>
        /// Calculate layout for all elements
        /// Returns a layout structure with all element bounds
        /// </summary>
        public static FeatureCardLayout CalculateLayout(
            Rectangle cardBounds,
            Size logoSize,
            Size titleSize,
            Size subtitleSize,
            Size actionIconSize,
            int actionIconCount,
            Size cardIconSize,
            int itemCount,
            int itemHeight,
            Padding padding)
        {
            var logoBounds = CalculateLogoBounds(cardBounds, logoSize, padding);
            var titleBounds = CalculateTitleBounds(cardBounds, logoSize, titleSize, padding);
            var subtitleBounds = CalculateSubtitleBounds(cardBounds, titleSize, subtitleSize, titleBounds.X, titleBounds.Y, padding);
            var actionIconsBounds = CalculateActionIconsBounds(cardBounds, actionIconSize, actionIconCount, padding);
            var cardIconBounds = CalculateCardIconBounds(cardBounds, cardIconSize, padding);
            var featuresListBounds = CalculateFeaturesListBounds(cardBounds, subtitleSize, itemCount, itemHeight, padding);

            return new FeatureCardLayout
            {
                LogoBounds = logoBounds,
                TitleBounds = titleBounds,
                SubtitleBounds = subtitleBounds,
                ActionIconsBounds = actionIconsBounds,
                CardIconBounds = cardIconBounds,
                FeaturesListBounds = featuresListBounds
            };
        }

        #endregion
    }

    /// <summary>
    /// Layout structure for feature card elements
    /// </summary>
    public class FeatureCardLayout
    {
        public Rectangle LogoBounds { get; set; }
        public Rectangle TitleBounds { get; set; }
        public Rectangle SubtitleBounds { get; set; }
        public Rectangle[] ActionIconsBounds { get; set; }
        public Rectangle CardIconBounds { get; set; }
        public Rectangle FeaturesListBounds { get; set; }
    }
}

