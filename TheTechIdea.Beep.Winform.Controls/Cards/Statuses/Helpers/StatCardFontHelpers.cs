using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Vis.Modules;

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
    /// Centralized font management for StatCard controls.
    /// Fonts are sourced from the theme's StatsCard* TypographyStyle roles via
    /// <see cref="BeepThemesManager"/>; when a role is unset the control-style
    /// sizing tables below are used as a fallback. Returned fonts are owned by the
    /// theme-manager cache — callers must NOT dispose them.
    /// </summary>
    public static class StatCardFontHelpers
    {
        #region Font Retrieval Methods

        /// <summary>Get font for card header.</summary>
        public static Font GetHeaderFont(BeepStatCard card, BeepControlStyle controlStyle)
            => FromRole(BeepThemesManager.CurrentTheme?.StatsCardTitleStyle)
               ?? ResolveFallback(card, controlStyle, StatCardFontElement.Header, 1f);

        /// <summary>Get font for value text (large, bold).</summary>
        public static Font GetValueFont(BeepStatCard card, BeepControlStyle controlStyle, float scale = 1.6f)
            => FromRole(BeepThemesManager.CurrentTheme?.StatsCardValueStyle, scale)
               ?? ResolveFallback(card, controlStyle, StatCardFontElement.Value, scale);

        /// <summary>Get font for delta text.</summary>
        public static Font GetDeltaFont(BeepStatCard card, BeepControlStyle controlStyle)
            => FromRole(BeepThemesManager.CurrentTheme?.StatsCardTrendStyle)
               ?? ResolveFallback(card, controlStyle, StatCardFontElement.Delta, 1f);

        /// <summary>Get font for info text.</summary>
        public static Font GetInfoFont(BeepStatCard card, BeepControlStyle controlStyle)
            => FromRole(BeepThemesManager.CurrentTheme?.StatsCardInfoStyle)
               ?? ResolveFallback(card, controlStyle, StatCardFontElement.Info, 1f);

        /// <summary>Get font for label text.</summary>
        public static Font GetLabelFont(BeepStatCard card, BeepControlStyle controlStyle)
            => FromRole(BeepThemesManager.CurrentTheme?.StatsCardSubStyleStyle)
               ?? ResolveFallback(card, controlStyle, StatCardFontElement.Label, 1f);

        /// <summary>Get font for stat card based on element type.</summary>
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
        private static Font ResolveFallback(BeepStatCard card, BeepControlStyle controlStyle, StatCardFontElement element, float scale)
        {
            var family = (card?.Font ?? SystemFonts.DefaultFont).FontFamily.Name;
            int fontSize = GetFontSizeForElement(controlStyle, element);
            if (Math.Abs(scale - 1f) > 0.001f) fontSize = Math.Max(8, (int)(fontSize * scale));
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, element);
            var weight = fontStyle.HasFlag(FontStyle.Bold) ? FontWeight.Bold : FontWeight.Normal;
            return BeepThemesManager.ToFont(family, fontSize, weight, fontStyle);
        }

        #endregion

        #region Font Size and Style Helpers (fallback tables)

        /// <summary>Get font size for a specific element based on control style.</summary>
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

        /// <summary>Get font style for a specific element based on control style.</summary>
        public static FontStyle GetFontStyleForElement(BeepControlStyle controlStyle, StatCardFontElement element)
        {
            if (element == StatCardFontElement.Header || element == StatCardFontElement.Value)
                return FontStyle.Bold;

            return FontStyle.Regular;
        }

        /// <summary>Adjust font size based on control style.</summary>
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
        /// Apply font theme to a stat card control.
        /// Fonts are resolved per-element in the painters via the Get*Font methods.
        /// </summary>
        public static void ApplyFontTheme(BeepStatCard card, BeepControlStyle controlStyle)
        {
            // Fonts are applied in painters via the Get*Font methods above.
        }

        #endregion
    }
}

