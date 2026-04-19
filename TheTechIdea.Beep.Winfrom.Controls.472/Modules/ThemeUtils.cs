using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public static class ThemeUtils
    {
        // Overload 1: Convert a Font object to TypographyStyle
        public static TypographyStyle ConvertFontToTypographyStyle(
            Font font,
            Color? textColor = null,
            float? lineHeight = null,
            float? letterSpacing = null)
        {
            // Map FontStyle to FontWeight
            FontWeight fontWeight = font.Style.HasFlag(FontStyle.Bold)
                ? FontWeight.Bold
                : font.Style.HasFlag(FontStyle.Regular)
                    ? FontWeight.Regular
                    : FontWeight.Normal;

            return new TypographyStyle
            {
                FontFamily = font.FontFamily.Name, // e.g., "Roboto", Fallback: Arial
                FontSize = font.Size,
                FontWeight = fontWeight,
                FontStyle = font.Style.HasFlag(FontStyle.Italic) ? FontStyle.Italic : FontStyle.Regular,
                TextColor = textColor ?? Color.FromArgb(236, 240, 241), // Default: Light gray
                LineHeight = lineHeight ?? 1.2f, // Default for readability
                LetterSpacing = letterSpacing ?? 0.2f, // Default for neon aesthetic
                IsUnderlined = font.Style.HasFlag(FontStyle.Underline),
                IsStrikeout = font.Style.HasFlag(FontStyle.Strikeout)
            };
        }

        // Overload 2: Create TypographyStyle from individual parameters
        public static TypographyStyle ConvertFontToTypographyStyle(
            string fontFamily,
            float fontSize,
            FontStyle fontStyle,
            Color? textColor = null,
            float? lineHeight = null,
            float? letterSpacing = null,
            FontWeight? fontWeight = null)
        {
            // Determine FontWeight if not provided
            FontWeight weight = fontWeight ?? (fontStyle.HasFlag(FontStyle.Bold)
                ? FontWeight.Bold
                : FontWeight.Regular);

            return new TypographyStyle
            {
                FontFamily = fontFamily, // e.g., "Roboto", Fallback: Arial
                FontSize = fontSize,
                FontWeight = weight,
                FontStyle = fontStyle.HasFlag(FontStyle.Italic) ? FontStyle.Italic : FontStyle.Regular,
                TextColor = textColor ?? Color.FromArgb(236, 240, 241), // Default: Light gray
                LineHeight = lineHeight ?? 1.2f, // Default for readability
                LetterSpacing = letterSpacing ?? 0.2f, // Default for neon aesthetic
                IsUnderlined = fontStyle.HasFlag(FontStyle.Underline),
                IsStrikeout = fontStyle.HasFlag(FontStyle.Strikeout)
            };
        }
    }
}