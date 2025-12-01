using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Metrices.Helpers
{
    /// <summary>
    /// Font element types for metric tile controls
    /// </summary>
    public enum MetricTileFontElement
    {
        Title,
        MetricValue,
        Delta
    }

    /// <summary>
    /// Centralized font management for MetricTile controls
    /// Integrates with BeepFontManager and StyleTypography
    /// </summary>
    public static class MetricTileFontHelpers
    {
        #region Font Retrieval Methods

        /// <summary>
        /// Get font for tile title
        /// </summary>
        public static Font GetTitleFont(
            BeepMetricTile tile,
            BeepControlStyle controlStyle)
        {
            if (tile == null)
                return new Font("Segoe UI", 10, FontStyle.Bold);

            Font baseFont = tile.Font ?? new Font("Segoe UI", 10, FontStyle.Bold);

            int fontSize = GetFontSizeForElement(controlStyle, MetricTileFontElement.Title);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, MetricTileFontElement.Title);

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
        /// Get font for metric value (large, bold)
        /// </summary>
        public static Font GetMetricValueFont(
            BeepMetricTile tile,
            BeepControlStyle controlStyle)
        {
            if (tile == null)
                return new Font("Segoe UI", 28, FontStyle.Bold);

            Font baseFont = tile.Font ?? new Font("Segoe UI", 28, FontStyle.Bold);

            int fontSize = GetFontSizeForElement(controlStyle, MetricTileFontElement.MetricValue);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, MetricTileFontElement.MetricValue);

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
            BeepMetricTile tile,
            BeepControlStyle controlStyle)
        {
            if (tile == null)
                return new Font("Segoe UI", 10, FontStyle.Regular);

            Font baseFont = tile.Font ?? new Font("Segoe UI", 10, FontStyle.Regular);

            int fontSize = GetFontSizeForElement(controlStyle, MetricTileFontElement.Delta);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, MetricTileFontElement.Delta);

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
        /// Get font for metric tile based on element type
        /// </summary>
        public static Font GetFontForElement(
            BeepMetricTile tile,
            MetricTileFontElement element,
            BeepControlStyle controlStyle)
        {
            return element switch
            {
                MetricTileFontElement.Title => GetTitleFont(tile, controlStyle),
                MetricTileFontElement.MetricValue => GetMetricValueFont(tile, controlStyle),
                MetricTileFontElement.Delta => GetDeltaFont(tile, controlStyle),
                _ => GetTitleFont(tile, controlStyle)
            };
        }

        #endregion

        #region Font Size and Style Helpers

        /// <summary>
        /// Get font size for a specific element based on control style
        /// </summary>
        public static int GetFontSizeForElement(BeepControlStyle controlStyle, MetricTileFontElement element)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);

            float elementSize = element switch
            {
                MetricTileFontElement.Title => baseSize,              // Title: same as body
                MetricTileFontElement.MetricValue => baseSize * 2.8f, // Metric: much larger
                MetricTileFontElement.Delta => baseSize - 1f,         // Delta: slightly smaller
                _ => baseSize
            };

            elementSize = AdjustSizeForControlStyle(elementSize, controlStyle);

            return Math.Max(8, (int)Math.Round(elementSize));
        }

        /// <summary>
        /// Get font style for a specific element based on control style
        /// </summary>
        public static FontStyle GetFontStyleForElement(BeepControlStyle controlStyle, MetricTileFontElement element)
        {
            if (element == MetricTileFontElement.Title || element == MetricTileFontElement.MetricValue)
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
        /// Apply font theme to a metric tile control
        /// Updates all font properties based on theme and style
        /// </summary>
        public static void ApplyFontTheme(
            BeepMetricTile tile,
            BeepControlStyle controlStyle)
        {
            if (tile == null)
                return;

            // Fonts are applied in DrawContent() method
            // This method provides the font values that should be used
        }

        #endregion
    }
}

