using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in tab controls
    /// Integrates with BeepFontManager and StyleTypography for consistent font usage
    /// </summary>
    public static class TabFontHelpers
    {
        private const string PrimaryFallbackFont = "Arial";
        private const string SecondaryFallbackFont = "Segoe UI";

        /// <summary>
        /// Gets the font for tab text
        /// Uses BeepFontManager with ControlStyle-aware sizing
        /// </summary>
        public static Font GetTabFont(
            BeepControlStyle controlStyle,
            bool isSelected = false)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            
            // Selected tabs are typically bold
            float tabSize = baseSize;
            FontStyle fontStyle = isSelected ? FontStyle.Bold : FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return GetFontWithFallback(primaryFont, tabSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for tab subtext/description
        /// </summary>
        public static Font GetTabSubtextFont(
            BeepControlStyle controlStyle,
            Control ownerControl = null)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float minSubtext = DpiScalingHelper.ScaleValue(8f, ownerControl);
            float subtextSize = Math.Max(minSubtext, baseSize - 2f); // Smaller for subtext
            FontStyle fontStyle = FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return GetFontWithFallback(primaryFont, subtextSize, fontStyle);
        }

        public static Font ResolveSafeFont(Font font, Control ownerControl = null)
        {
            if (IsFontUsable(font))
            {
                return font;
            }

            if (IsFontUsable(ownerControl?.Font))
            {
                return ownerControl.Font;
            }

            float fallbackSize = 9f;
            FontStyle fallbackStyle = FontStyle.Regular;

            try
            {
                if (font != null && font.Size > 0)
                {
                    fallbackSize = font.Size;
                    fallbackStyle = font.Style;
                }
                else if (ownerControl?.Font != null && ownerControl.Font.Size > 0)
                {
                    fallbackSize = ownerControl.Font.Size;
                    fallbackStyle = ownerControl.Font.Style;
                }
            }
            catch
            {
                fallbackSize = 9f;
                fallbackStyle = FontStyle.Regular;
            }

            return GetFontWithFallback(PrimaryFallbackFont, fallbackSize, fallbackStyle);
        }

        public static int GetSafeFontHeight(Font font, Control ownerControl = null)
        {
            Font safeFont = ResolveSafeFont(font, ownerControl);
            return Math.Max(1, FontListHelper.GetFontHeightSafe(safeFont, ownerControl));
        }

        public static int MeasureTextWidthSafe(string text, Font font, Control ownerControl = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            Font safeFont = ResolveSafeFont(font, ownerControl);
            try
            {
                return TextRenderer.MeasureText(text, safeFont).Width;
            }
            catch
            {
                return TextRenderer.MeasureText(text, SystemFonts.DefaultFont).Width;
            }
        }

        public static Font CreateDerivedSafeFont(Font baseFont, FontStyle style, Control ownerControl = null)
        {
            Font safeFont = ResolveSafeFont(baseFont, ownerControl);
            try
            {
                return safeFont.Style == style
                    ? new Font(safeFont.FontFamily, safeFont.Size, safeFont.Style, safeFont.Unit)
                    : new Font(safeFont, style);
            }
            catch
            {
                float fallbackSize = GetFallbackFontSize(safeFont, ownerControl);
                return GetFontWithFallback(PrimaryFallbackFont, fallbackSize, style);
            }
        }

        public static Font CreateSizedSafeFont(Font baseFont, float size, FontStyle style, Control ownerControl = null)
        {
            Font safeFont = ResolveSafeFont(baseFont, ownerControl);
            float safeSize = Math.Max(1f, size);
            try
            {
                return new Font(safeFont.FontFamily, safeSize, style, safeFont.Unit);
            }
            catch
            {
                return GetFontWithFallback(PrimaryFallbackFont, safeSize, style);
            }
        }

        private static float GetFallbackFontSize(Font font, Control ownerControl)
        {
            try
            {
                if (font != null && font.Size > 0)
                {
                    return font.Size;
                }

                if (ownerControl?.Font != null && ownerControl.Font.Size > 0)
                {
                    return ownerControl.Font.Size;
                }
            }
            catch
            {
                return 9f;
            }

            return 9f;
        }

        private static Font GetFontWithFallback(string primaryFont, float size, FontStyle style)
        {
            float safeSize = Math.Max(1f, size);
            string[] candidates =
            {
                primaryFont,
                PrimaryFallbackFont,
                SecondaryFallbackFont
            };

            foreach (string candidate in candidates)
            {
                if (string.IsNullOrWhiteSpace(candidate))
                {
                    continue;
                }

                try
                {
                    Font font = BeepFontManager.GetFont(candidate.Trim(), safeSize, style);
                    if (IsFontUsable(font))
                    {
                        return font;
                    }
                }
                catch
                {
                    // Try the next configured fallback family.
                }
            }

            return SystemFonts.DefaultFont;
        }

        private static bool IsFontUsable(Font font)
        {
            if (font == null)
            {
                return false;
            }

            try
            {
                return font.Height > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Applies font theme to tab control
        /// Updates the control's Font property based on ControlStyle
        /// </summary>
        public static void ApplyFontTheme(
            BeepControlStyle controlStyle)
        {
            // This is a helper for getting fonts, not for setting control font
            // The control should use these helpers when painting
        }
    }
}
