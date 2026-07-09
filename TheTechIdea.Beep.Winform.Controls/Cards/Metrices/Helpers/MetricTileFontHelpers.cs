using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Vis.Modules;

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
    /// Centralized font management for MetricTile controls.
    /// Fonts are sourced from the theme's Card*/StatsCard* TypographyStyle roles via
    /// <see cref="BeepThemesManager"/>; when a role is unset the control-style
    /// sizing tables below are used as a fallback. Returned fonts are owned by the
    /// theme-manager cache — callers must NOT dispose them.
    /// </summary>
    public static class MetricTileFontHelpers
    {
        #region Font Retrieval Methods

        /// <summary>Get font for tile title.</summary>
        public static Font GetTitleFont(
            BeepMetricTile tile,
            BeepControlStyle controlStyle)
            => FromRole(BeepThemesManager.CurrentTheme?.CardTitleFont)
               ?? ResolveFallback(tile, controlStyle, MetricTileFontElement.Title, 1f);

        /// <summary>Get font for metric value (large, bold).</summary>
        public static Font GetMetricValueFont(
            BeepMetricTile tile,
            BeepControlStyle controlStyle)
            => FromRole(BeepThemesManager.CurrentTheme?.StatsCardValueStyle)
               ?? ResolveFallback(tile, controlStyle, MetricTileFontElement.MetricValue, 1f);

        /// <summary>Get font for delta text.</summary>
        public static Font GetDeltaFont(
            BeepMetricTile tile,
            BeepControlStyle controlStyle)
            => FromRole(BeepThemesManager.CurrentTheme?.StatsCardTrendStyle)
               ?? ResolveFallback(tile, controlStyle, MetricTileFontElement.Delta, 1f);

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

        #region Typography-role + fallback resolution

        /// <summary>
        /// Builds a font from a theme TypographyStyle role (shared, cached — never disposed).
        /// Returns null when the role is unset so callers can fall back to control-style sizing.
        /// </summary>
        private static Font FromRole(TypographyStyle role, float scale = 1f)
        {
            if (role == null) return null;
            if (Math.Abs(scale - 1f) < 0.001f)
                return BeepThemesManager.ToFont(role);
            float size = (role.FontSize > 0 ? role.FontSize : 9f) * scale;
            return BeepThemesManager.ToFont(role.FontFamily, size, role.FontWeight, role.FontStyle);
        }

        /// <summary>
        /// Control-style-driven fallback used when the matching theme role is unset.
        /// Routes through BeepThemesManager (shared cache) — no consumer disposal.
        /// </summary>
        private static Font ResolveFallback(BeepMetricTile tile, BeepControlStyle controlStyle, MetricTileFontElement element, float scale)
        {
            var family = (tile?.Font ?? SystemFonts.DefaultFont).FontFamily.Name;
            int fontSize = GetFontSizeForElement(controlStyle, element);
            if (Math.Abs(scale - 1f) > 0.001f) fontSize = Math.Max(8, (int)(fontSize * scale));
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, element);
            var weight = fontStyle.HasFlag(FontStyle.Bold) ? FontWeight.Bold : FontWeight.Normal;
            return BeepThemesManager.ToFont(family, fontSize, weight, fontStyle);
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

