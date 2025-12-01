using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Features.Helpers
{
    /// <summary>
    /// Font element types for feature card controls
    /// </summary>
    public enum FeatureCardFontElement
    {
        Title,
        Subtitle,
        BulletPoint
    }

    /// <summary>
    /// Centralized font management for FeatureCard controls
    /// Integrates with BeepFontManager and StyleTypography
    /// </summary>
    public static class FeatureCardFontHelpers
    {
        #region Font Retrieval Methods

        /// <summary>
        /// Get font for card title
        /// </summary>
        public static Font GetTitleFont(
            BeepFeatureCard card,
            BeepControlStyle controlStyle)
        {
            if (card == null)
                return new Font("Segoe UI", 14, FontStyle.Bold);

            Font baseFont = card.Font ?? new Font("Segoe UI", 14, FontStyle.Bold);

            int fontSize = GetFontSizeForElement(controlStyle, FeatureCardFontElement.Title);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, FeatureCardFontElement.Title);

            try
            {
                var fontFamily = BeepFontManager.GetFontFamily(controlStyle) ?? baseFont.FontFamily;
                return BeepFontManager.GetFont(fontFamily.Name, fontSize, fontStyle);
            }
            catch
            {
                return new Font(baseFont.FontFamily, fontSize, fontStyle);
            }
        }

        /// <summary>
        /// Get font for card subtitle
        /// </summary>
        public static Font GetSubtitleFont(
            BeepFeatureCard card,
            BeepControlStyle controlStyle)
        {
            if (card == null)
                return new Font("Segoe UI", 10, FontStyle.Regular);

            Font baseFont = card.Font ?? new Font("Segoe UI", 10, FontStyle.Regular);

            int fontSize = GetFontSizeForElement(controlStyle, FeatureCardFontElement.Subtitle);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, FeatureCardFontElement.Subtitle);

            try
            {
                var fontFamily = BeepFontManager.GetFontFamily(controlStyle) ?? baseFont.FontFamily;
                return BeepFontManager.GetFont(fontFamily.Name, fontSize, fontStyle);
            }
            catch
            {
                return new Font(baseFont.FontFamily, fontSize, fontStyle);
            }
        }

        /// <summary>
        /// Get font for bullet points
        /// </summary>
        public static Font GetBulletPointFont(
            BeepFeatureCard card,
            BeepControlStyle controlStyle)
        {
            if (card == null)
                return new Font("Segoe UI", 9, FontStyle.Regular);

            Font baseFont = card.Font ?? new Font("Segoe UI", 9, FontStyle.Regular);

            int fontSize = GetFontSizeForElement(controlStyle, FeatureCardFontElement.BulletPoint);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, FeatureCardFontElement.BulletPoint);

            try
            {
                var fontFamily = BeepFontManager.GetFontFamily(controlStyle) ?? baseFont.FontFamily;
                return BeepFontManager.GetFont(fontFamily.Name, fontSize, fontStyle);
            }
            catch
            {
                return new Font(baseFont.FontFamily, fontSize, fontStyle);
            }
        }

        /// <summary>
        /// Get font for feature card based on element type
        /// </summary>
        public static Font GetFontForElement(
            BeepFeatureCard card,
            FeatureCardFontElement element,
            BeepControlStyle controlStyle)
        {
            return element switch
            {
                FeatureCardFontElement.Title => GetTitleFont(card, controlStyle),
                FeatureCardFontElement.Subtitle => GetSubtitleFont(card, controlStyle),
                FeatureCardFontElement.BulletPoint => GetBulletPointFont(card, controlStyle),
                _ => GetTitleFont(card, controlStyle)
            };
        }

        #endregion

        #region Font Size and Style Helpers

        /// <summary>
        /// Get font size for a specific element based on control style
        /// </summary>
        public static int GetFontSizeForElement(BeepControlStyle controlStyle, FeatureCardFontElement element)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);

            float elementSize = element switch
            {
                FeatureCardFontElement.Title => baseSize + 2f,      // Title: larger than body
                FeatureCardFontElement.Subtitle => baseSize - 1f,    // Subtitle: slightly smaller
                FeatureCardFontElement.BulletPoint => baseSize - 2f, // Bullet: smaller than body
                _ => baseSize
            };

            elementSize = AdjustSizeForControlStyle(elementSize, controlStyle);

            return Math.Max(8, (int)Math.Round(elementSize));
        }

        /// <summary>
        /// Get font style for a specific element based on control style
        /// </summary>
        public static FontStyle GetFontStyleForElement(BeepControlStyle controlStyle, FeatureCardFontElement element)
        {
            if (element == FeatureCardFontElement.Title)
            {
                return controlStyle switch
                {
                    BeepControlStyle.Modern => FontStyle.Bold,
                    BeepControlStyle.Material => FontStyle.Bold,
                    BeepControlStyle.Fluent => FontStyle.Bold,
                    BeepControlStyle.Minimal => FontStyle.Bold,
                    _ => FontStyle.Bold
                };
            }

            return FontStyle.Regular;
        }

        /// <summary>
        /// Adjust font size based on control style
        /// </summary>
        private static float AdjustSizeForControlStyle(float baseSize, BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Modern => baseSize * 1.05f,
                BeepControlStyle.Material => baseSize,
                BeepControlStyle.Fluent => baseSize * 0.95f,
                BeepControlStyle.Minimal => baseSize * 1.1f,
                _ => baseSize
            };
        }

        #endregion

        #region Bulk Font Application

        /// <summary>
        /// Apply font theme to a feature card control
        /// Updates all font properties based on theme and style
        /// </summary>
        public static void ApplyFontTheme(
            BeepFeatureCard card,
            BeepControlStyle controlStyle)
        {
            if (card == null)
                return;

            // Fonts are applied to child controls in BeepFeatureCard.ApplyTheme()
            // This method provides the font values that should be used
        }

        #endregion
    }
}

