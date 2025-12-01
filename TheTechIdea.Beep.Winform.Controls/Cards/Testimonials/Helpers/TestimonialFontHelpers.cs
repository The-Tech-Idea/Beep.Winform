using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

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
    /// Centralized font management for Testimonial controls
    /// Integrates with BeepFontManager and StyleTypography
    /// </summary>
    public static class TestimonialFontHelpers
    {
        #region Font Retrieval Methods

        /// <summary>
        /// Get font for testimonial text
        /// </summary>
        public static Font GetTestimonialFont(
            BeepTestimonial testimonial,
            BeepControlStyle controlStyle,
            object viewType = null) // TestimonialViewType, but using object to avoid dependency
        {
            if (testimonial == null)
                return new Font("Segoe UI", 10, FontStyle.Regular);

            Font baseFont = testimonial.Font ?? new Font("Segoe UI", 10, FontStyle.Regular);

            int fontSize = GetFontSizeForElement(controlStyle, TestimonialFontElement.Testimonial, viewType);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, TestimonialFontElement.Testimonial);

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
        /// Get font for name text
        /// </summary>
        public static Font GetNameFont(
            BeepTestimonial testimonial,
            BeepControlStyle controlStyle,
            object viewType = null)
        {
            if (testimonial == null)
                return new Font("Segoe UI", 10, FontStyle.Bold);

            Font baseFont = testimonial.Font ?? new Font("Segoe UI", 10, FontStyle.Bold);

            int fontSize = GetFontSizeForElement(controlStyle, TestimonialFontElement.Name, viewType);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, TestimonialFontElement.Name);

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
        /// Get font for details text (username, position)
        /// </summary>
        public static Font GetDetailsFont(
            BeepTestimonial testimonial,
            BeepControlStyle controlStyle,
            object viewType = null)
        {
            if (testimonial == null)
                return new Font("Segoe UI", 9, FontStyle.Regular);

            Font baseFont = testimonial.Font ?? new Font("Segoe UI", 9, FontStyle.Regular);

            int fontSize = GetFontSizeForElement(controlStyle, TestimonialFontElement.Details, viewType);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, TestimonialFontElement.Details);

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
        /// Get font for rating stars
        /// </summary>
        public static Font GetRatingFont(
            BeepTestimonial testimonial,
            BeepControlStyle controlStyle)
        {
            if (testimonial == null)
                return new Font("Segoe UI", 12, FontStyle.Bold);

            Font baseFont = testimonial.Font ?? new Font("Segoe UI", 12, FontStyle.Bold);

            int fontSize = GetFontSizeForElement(controlStyle, TestimonialFontElement.Rating, null);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, TestimonialFontElement.Rating);

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

