using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.TextPainters
{
    /// <summary>
    /// Text painter for monospace/developer styles (DarkGlow)
    /// Uses monospace fonts like JetBrains Mono
    /// </summary>
    public static class MonospaceTextPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, string text, bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text))
                return;
            
            Color textColor = GetColor(style, StyleColors.GetForeground, "Foreground", theme, useThemeColors);
            FontStyle fontStyle = isFocused ? 
                StyleTypography.GetActiveFontStyle(style) : 
                StyleTypography.GetFontStyle(style);
            
            using (var font = StyleTypography.GetFont(style, fontStyle: fontStyle))
            using (var textBrush = new SolidBrush(textColor))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                };
                
                // Monospace uses slightly different rendering
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                g.DrawString(text, font, textBrush, bounds, sf);
            }
        }
        
        private static Color GetColor(BeepControlStyle style, System.Func<BeepControlStyle, Color> styleColorFunc, string themeColorKey, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor(themeColorKey);
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            return styleColorFunc(style);
        }
    }
}
