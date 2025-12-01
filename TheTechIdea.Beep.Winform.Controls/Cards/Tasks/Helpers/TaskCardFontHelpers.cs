using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Tasks.Helpers
{
    /// <summary>
    /// Font element types for task card controls
    /// </summary>
    public enum TaskCardFontElement
    {
        Title,
        Subtitle,
        Metric,
        AvatarLabel
    }

    /// <summary>
    /// Centralized font management for TaskCard controls
    /// Integrates with BeepFontManager and StyleTypography
    /// </summary>
    public static class TaskCardFontHelpers
    {
        #region Font Retrieval Methods

        /// <summary>
        /// Get font for task card title
        /// </summary>
        public static Font GetTitleFont(
            BeepTaskCard card,
            BeepControlStyle controlStyle)
        {
            if (card == null)
                return new Font("Segoe UI", 12, FontStyle.Bold);

            Font baseFont = card.Font ?? new Font("Segoe UI", 12, FontStyle.Bold);

            int fontSize = GetFontSizeForElement(controlStyle, TaskCardFontElement.Title);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, TaskCardFontElement.Title);

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
        /// Get font for task card subtitle
        /// </summary>
        public static Font GetSubtitleFont(
            BeepTaskCard card,
            BeepControlStyle controlStyle)
        {
            if (card == null)
                return new Font("Segoe UI", 10, FontStyle.Regular);

            Font baseFont = card.Font ?? new Font("Segoe UI", 10, FontStyle.Regular);

            int fontSize = GetFontSizeForElement(controlStyle, TaskCardFontElement.Subtitle);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, TaskCardFontElement.Subtitle);

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
        /// Get font for metric text
        /// </summary>
        public static Font GetMetricFont(
            BeepTaskCard card,
            BeepControlStyle controlStyle)
        {
            if (card == null)
                return new Font("Segoe UI", 9, FontStyle.Regular);

            Font baseFont = card.Font ?? new Font("Segoe UI", 9, FontStyle.Regular);

            int fontSize = GetFontSizeForElement(controlStyle, TaskCardFontElement.Metric);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, TaskCardFontElement.Metric);

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
        /// Get font for avatar label (+X)
        /// </summary>
        public static Font GetAvatarLabelFont(
            BeepTaskCard card,
            BeepControlStyle controlStyle)
        {
            if (card == null)
                return new Font("Segoe UI", 8, FontStyle.Regular);

            Font baseFont = card.Font ?? new Font("Segoe UI", 8, FontStyle.Regular);

            int fontSize = GetFontSizeForElement(controlStyle, TaskCardFontElement.AvatarLabel);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, TaskCardFontElement.AvatarLabel);

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
        /// Get font for task card based on element type
        /// </summary>
        public static Font GetFontForElement(
            BeepTaskCard card,
            TaskCardFontElement element,
            BeepControlStyle controlStyle)
        {
            return element switch
            {
                TaskCardFontElement.Title => GetTitleFont(card, controlStyle),
                TaskCardFontElement.Subtitle => GetSubtitleFont(card, controlStyle),
                TaskCardFontElement.Metric => GetMetricFont(card, controlStyle),
                TaskCardFontElement.AvatarLabel => GetAvatarLabelFont(card, controlStyle),
                _ => GetTitleFont(card, controlStyle)
            };
        }

        #endregion

        #region Font Size and Style Helpers

        /// <summary>
        /// Get font size for a specific element based on control style
        /// </summary>
        public static int GetFontSizeForElement(BeepControlStyle controlStyle, TaskCardFontElement element)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);

            float elementSize = element switch
            {
                TaskCardFontElement.Title => baseSize + 1f,      // Title: slightly larger
                TaskCardFontElement.Subtitle => baseSize - 1f,    // Subtitle: slightly smaller
                TaskCardFontElement.Metric => baseSize - 2f,      // Metric: smaller
                TaskCardFontElement.AvatarLabel => baseSize - 3f, // Avatar label: smallest
                _ => baseSize
            };

            elementSize = AdjustSizeForControlStyle(elementSize, controlStyle);

            return Math.Max(8, (int)Math.Round(elementSize));
        }

        /// <summary>
        /// Get font style for a specific element based on control style
        /// </summary>
        public static FontStyle GetFontStyleForElement(BeepControlStyle controlStyle, TaskCardFontElement element)
        {
            if (element == TaskCardFontElement.Title)
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
        /// Apply font theme to a task card control
        /// Updates all font properties based on theme and style
        /// </summary>
        public static void ApplyFontTheme(
            BeepTaskCard card,
            BeepControlStyle controlStyle)
        {
            if (card == null)
                return;

            // Fonts are applied in DrawContent() method
            // This method provides the font values that should be used
        }

        #endregion
    }
}

