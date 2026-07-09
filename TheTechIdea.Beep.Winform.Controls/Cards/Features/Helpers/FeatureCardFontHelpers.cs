using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Vis.Modules;

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
    /// Centralized font management for FeatureCard controls.
    /// Fonts are sourced from the theme's Card* TypographyStyle roles via
    /// <see cref="BeepThemesManager"/>; when a role is unset the control-style
    /// sizing tables below are used as a fallback. Returned fonts are owned by the
    /// theme-manager cache — callers must NOT dispose them.
    /// </summary>
    public static class FeatureCardFontHelpers
    {
        #region Font Retrieval Methods

        /// <summary>Get font for card title.</summary>
        public static Font GetTitleFont(BeepFeatureCard card, BeepControlStyle controlStyle)
            => FromRole(BeepThemesManager.CurrentTheme?.CardTitleFont)
               ?? ResolveFallback(card, controlStyle, FeatureCardFontElement.Title, 1f);

        /// <summary>Get font for card subtitle.</summary>
        public static Font GetSubtitleFont(BeepFeatureCard card, BeepControlStyle controlStyle)
            => FromRole(BeepThemesManager.CurrentTheme?.CardSubTitleStyle)
               ?? ResolveFallback(card, controlStyle, FeatureCardFontElement.Subtitle, 1f);

        /// <summary>Get font for bullet points.</summary>
        public static Font GetBulletPointFont(BeepFeatureCard card, BeepControlStyle controlStyle)
            => FromRole(BeepThemesManager.CurrentTheme?.CardparagraphStyle)
               ?? ResolveFallback(card, controlStyle, FeatureCardFontElement.BulletPoint, 1f);

        /// <summary>Get font for feature card based on element type.</summary>
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
        private static Font ResolveFallback(BeepFeatureCard card, BeepControlStyle controlStyle, FeatureCardFontElement element, float scale)
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
        /// Apply font theme to a feature card control.
        /// Fonts are resolved per-element in the painters via the Get*Font methods.
        /// </summary>
        public static void ApplyFontTheme(
            BeepFeatureCard card,
            BeepControlStyle controlStyle)
        {
            // Fonts are applied in painters via the Get*Font methods above.
        }

        #endregion
    }
}
