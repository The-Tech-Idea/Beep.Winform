using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Testimonials.Helpers
{
    /// <summary>
    /// Font element types for testimonial controls
    /// </summary>
    public enum TestimonialFontElement
    {
        Testimonial,
        Name,
        Details,
        Rating
    }

    /// <summary>
    /// Centralized font management for Testimonial controls.
    /// Fonts are sourced from the theme's Card* TypographyStyle roles via
    /// <see cref="BeepThemesManager"/>; when a role is unset the control-style
    /// sizing tables below are used as a fallback. Returned fonts are owned by the
    /// theme-manager cache — callers must NOT dispose them.
    /// </summary>
    public static class TestimonialFontHelpers
    {
        #region Font Retrieval Methods

        /// <summary>Get font for testimonial text.</summary>
        public static Font GetTestimonialFont(
            BeepTestimonial testimonial,
            BeepControlStyle controlStyle,
            object viewType = null) // TestimonialViewType, but using object to avoid dependency
            => FromRole(BeepThemesManager.CurrentTheme?.CardparagraphStyle)
               ?? ResolveFallback(testimonial, controlStyle, TestimonialFontElement.Testimonial, viewType);

        /// <summary>Get font for name text.</summary>
        public static Font GetNameFont(
            BeepTestimonial testimonial,
            BeepControlStyle controlStyle,
            object viewType = null)
            => FromRole(BeepThemesManager.CurrentTheme?.CardTitleFont)
               ?? ResolveFallback(testimonial, controlStyle, TestimonialFontElement.Name, viewType);

        /// <summary>Get font for details text (username, position).</summary>
        public static Font GetDetailsFont(
            BeepTestimonial testimonial,
            BeepControlStyle controlStyle,
            object viewType = null)
            => FromRole(BeepThemesManager.CurrentTheme?.CardSubTitleStyle)
               ?? ResolveFallback(testimonial, controlStyle, TestimonialFontElement.Details, viewType);

        /// <summary>Get font for rating stars.</summary>
        public static Font GetRatingFont(
            BeepTestimonial testimonial,
            BeepControlStyle controlStyle)
            => FromRole(BeepThemesManager.CurrentTheme?.CardSubTitleStyle)
               ?? ResolveFallback(testimonial, controlStyle, TestimonialFontElement.Rating, null);

        /// <summary>
        /// Get font for testimonial based on element type
        /// </summary>
        public static Font GetFontForElement(
            BeepTestimonial testimonial,
            TestimonialFontElement element,
            BeepControlStyle controlStyle,
            object viewType = null)
        {
            return element switch
            {
                TestimonialFontElement.Testimonial => GetTestimonialFont(testimonial, controlStyle, viewType),
                TestimonialFontElement.Name => GetNameFont(testimonial, controlStyle, viewType),
                TestimonialFontElement.Details => GetDetailsFont(testimonial, controlStyle, viewType),
                TestimonialFontElement.Rating => GetRatingFont(testimonial, controlStyle),
                _ => GetTestimonialFont(testimonial, controlStyle, viewType)
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
        private static Font ResolveFallback(BeepTestimonial testimonial, BeepControlStyle controlStyle, TestimonialFontElement element, object viewType)
        {
            var family = (testimonial?.Font ?? SystemFonts.DefaultFont).FontFamily.Name;
            int fontSize = GetFontSizeForElement(controlStyle, element, viewType);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, element);
            var weight = fontStyle.HasFlag(FontStyle.Bold) ? FontWeight.Bold : FontWeight.Normal;
            return BeepThemesManager.ToFont(family, fontSize, weight, fontStyle);
        }

        #endregion

        #region Font Size and Style Helpers

        /// <summary>
        /// Get font size for a specific element based on control style and view type
        /// </summary>
        public static int GetFontSizeForElement(BeepControlStyle controlStyle, TestimonialFontElement element, object viewType)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);

            float elementSize = element switch
            {
                TestimonialFontElement.Testimonial => baseSize,      // Testimonial: same as body
                TestimonialFontElement.Name => baseSize,             // Name: same as body
                TestimonialFontElement.Details => baseSize - 1f,     // Details: slightly smaller
                TestimonialFontElement.Rating => baseSize + 2f,      // Rating: larger
                _ => baseSize
            };

            // Adjust for view type if needed
            if (viewType != null)
            {
                string viewTypeStr = viewType.ToString();
                if (viewTypeStr.Contains("Compact"))
                {
                    elementSize *= 0.9f; // Slightly smaller for compact view
                }
                else if (viewTypeStr.Contains("Minimal"))
                {
                    elementSize *= 1.05f; // Slightly larger for minimal view
                }
            }

            elementSize = AdjustSizeForControlStyle(elementSize, controlStyle);

            return Math.Max(8, (int)Math.Round(elementSize));
        }

        /// <summary>
        /// Get font style for a specific element based on control style
        /// </summary>
        public static FontStyle GetFontStyleForElement(BeepControlStyle controlStyle, TestimonialFontElement element)
        {
            if (element == TestimonialFontElement.Name || element == TestimonialFontElement.Rating)
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
        /// Apply font theme to a testimonial control
        /// Updates all font properties based on theme and style
        /// </summary>
        public static void ApplyFontTheme(
            BeepTestimonial testimonial,
            BeepControlStyle controlStyle)
        {
            if (testimonial == null)
                return;

            // Fonts are applied to child controls in BeepTestimonial.ApplyTheme()
            // This method provides the font values that should be used
        }

        #endregion
    }
}

