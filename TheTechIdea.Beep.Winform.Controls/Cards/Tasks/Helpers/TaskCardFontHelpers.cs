using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Vis.Modules;

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
    /// Centralized font management for TaskCard controls.
    /// Fonts are sourced from the theme's TaskCard* TypographyStyle roles via
    /// <see cref="BeepThemesManager"/>; when a role is unset the control-style
    /// sizing tables below are used as a fallback. Returned fonts are owned by the
    /// theme-manager cache — callers must NOT dispose them.
    /// </summary>
    public static class TaskCardFontHelpers
    {
        #region Font Retrieval Methods

        /// <summary>Get font for task card title.</summary>
        public static Font GetTitleFont(BeepTaskCard card, BeepControlStyle controlStyle)
            => FromRole(BeepThemesManager.CurrentTheme?.TaskCardTitleStyle)
               ?? ResolveFallback(card, controlStyle, TaskCardFontElement.Title, 1f);

        /// <summary>Get font for task card subtitle.</summary>
        public static Font GetSubtitleFont(BeepTaskCard card, BeepControlStyle controlStyle)
            => FromRole(BeepThemesManager.CurrentTheme?.TaskCardSubStyleStyle)
               ?? ResolveFallback(card, controlStyle, TaskCardFontElement.Subtitle, 1f);

        /// <summary>Get font for metric text.</summary>
        public static Font GetMetricFont(BeepTaskCard card, BeepControlStyle controlStyle)
            => FromRole(BeepThemesManager.CurrentTheme?.TaskCardMetricTextStyle)
               ?? ResolveFallback(card, controlStyle, TaskCardFontElement.Metric, 1f);

        /// <summary>Get font for avatar label (+X).</summary>
        public static Font GetAvatarLabelFont(BeepTaskCard card, BeepControlStyle controlStyle)
            => FromRole(BeepThemesManager.CurrentTheme?.TaskCardSubStyleStyle)
               ?? ResolveFallback(card, controlStyle, TaskCardFontElement.AvatarLabel, 1f);

        /// <summary>Get font for task card based on element type.</summary>
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
        private static Font ResolveFallback(BeepTaskCard card, BeepControlStyle controlStyle, TaskCardFontElement element, float scale)
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
        /// Apply font theme to a task card control.
        /// Fonts are resolved per-element in the painters via the Get*Font methods.
        /// </summary>
        public static void ApplyFontTheme(
            BeepTaskCard card,
            BeepControlStyle controlStyle)
        {
            // Fonts are applied in painters via the Get*Font methods above.
        }

        #endregion
    }
}
