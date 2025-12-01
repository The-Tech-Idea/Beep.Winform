using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.StatusCards.Helpers
{
    /// <summary>
    /// Font element types for stat card controls
    /// </summary>
    public enum StatCardFontElement
    {
        Header,
        Value,
        Delta,
        Info,
        Label
    }

    /// <summary>
    /// Centralized font management for StatCard controls
    /// Integrates with BeepFontManager and StyleTypography
    /// </summary>
    public static class StatCardFontHelpers
    {
        #region Font Retrieval Methods

        /// <summary>
        /// Get font for card header
        /// </summary>
        public static Font GetHeaderFont(
            BeepStatCard card,
            BeepControlStyle controlStyle)
        {
            if (card == null)
                return new Font("Segoe UI", 10, FontStyle.Bold);

            Font baseFont = card.Font ?? new Font("Segoe UI", 10, FontStyle.Bold);

            int fontSize = GetFontSizeForElement(controlStyle, StatCardFontElement.Header);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, StatCardFontElement.Header);

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
        /// Get font for value text (large, bold)
        /// </summary>
        public static Font GetValueFont(
            BeepStatCard card,
            BeepControlStyle controlStyle,
            float scale = 1.6f)
        {
            if (card == null)
                return new Font("Segoe UI", (int)(12 * scale), FontStyle.Bold);

            Font baseFont = card.Font ?? new Font("Segoe UI", (int)(12 * scale), FontStyle.Bold);

            int fontSize = GetFontSizeForElement(controlStyle, StatCardFontElement.Value);
            fontSize = (int)(fontSize * scale);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, StatCardFontElement.Value);

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
        /// Get font for delta text
        /// </summary>
        public static Font GetDeltaFont(
            BeepStatCard card,
            BeepControlStyle controlStyle)
        {
            if (card == null)
                return new Font("Segoe UI", 10, FontStyle.Regular);

            Font baseFont = card.Font ?? new Font("Segoe UI", 10, FontStyle.Regular);

            int fontSize = GetFontSizeForElement(controlStyle, StatCardFontElement.Delta);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, StatCardFontElement.Delta);

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
        /// Get font for info text
        /// </summary>
        public static Font GetInfoFont(
            BeepStatCard card,
            BeepControlStyle controlStyle)
        {
            if (card == null)
                return new Font("Segoe UI", 9, FontStyle.Regular);

            Font baseFont = card.Font ?? new Font("Segoe UI", 9, FontStyle.Regular);

            int fontSize = GetFontSizeForElement(controlStyle, StatCardFontElement.Info);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, StatCardFontElement.Info);

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
        /// Get font for label text
        /// </summary>
        public static Font GetLabelFont(
            BeepStatCard card,
            BeepControlStyle controlStyle)
        {
            if (card == null)
                return new Font("Segoe UI", 8, FontStyle.Regular);

            Font baseFont = card.Font ?? new Font("Segoe UI", 8, FontStyle.Regular);

            int fontSize = GetFontSizeForElement(controlStyle, StatCardFontElement.Label);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, StatCardFontElement.Label);

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
        /// Get font for stat card based on element type
        /// </summary>
        public static Font GetFontForElement(
            BeepStatCard card,
            StatCardFontElement element,
            BeepControlStyle controlStyle,
            float scale = 1.0f)
        {
            return element switch
            {
                StatCardFontElement.Header => GetHeaderFont(card, controlStyle),
                StatCardFontElement.Value => GetValueFont(card, controlStyle, scale),
                StatCardFontElement.Delta => GetDeltaFont(card, controlStyle),
                StatCardFontElement.Info => GetInfoFont(card, controlStyle),
                StatCardFontElement.Label => GetLabelFont(card, controlStyle),
                _ => GetHeaderFont(card, controlStyle)
            };
        }

        #endregion

        #region Font Size and Style Helpers

        /// <summary>
        /// Get font size for a specific element based on control style
        /// </summary>
        public static int GetFontSizeForElement(BeepControlStyle controlStyle, StatCardFontElement element)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);

            float elementSize = element switch
            {
                StatCardFontElement.Header => baseSize,              // Header: same as body
                StatCardFontElement.Value => baseSize * 1.5f,        // Value: larger
                StatCardFontElement.Delta => baseSize - 1f,          // Delta: slightly smaller
                StatCardFontElement.Info => baseSize - 2f,           // Info: smaller
                StatCardFontElement.Label => baseSize - 3f,          // Label: smallest
                _ => baseSize
            };

            elementSize = AdjustSizeForControlStyle(elementSize, controlStyle);

            return Math.Max(8, (int)Math.Round(elementSize));
        }

        /// <summary>
        /// Get font style for a specific element based on control style
        /// </summary>
        public static FontStyle GetFontStyleForElement(BeepControlStyle controlStyle, StatCardFontElement element)
        {
            if (element == StatCardFontElement.Header || element == StatCardFontElement.Value)
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
        /// Apply font theme to a stat card control
        /// Updates all font properties based on theme and style
        /// </summary>
        public static void ApplyFontTheme(
            BeepStatCard card,
            BeepControlStyle controlStyle)
        {
            if (card == null)
                return;

            // Fonts are applied in painters
            // This method provides the font values that should be used
        }

        #endregion
    }
}

