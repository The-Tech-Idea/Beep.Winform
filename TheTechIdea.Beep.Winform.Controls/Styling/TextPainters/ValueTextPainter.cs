using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.TextPainters
{
    /// <summary>
    /// Value text painter for numeric and date controls
    /// Uses centered alignment and appropriate fonts per Style
    /// </summary>
    public static class ValueTextPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, string text, bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text))
                return;
            
            Color textColor = GetColor(style, StyleColors.GetForeground, "Foreground", theme, useThemeColors);
            
            using (var font = StyleTypography.GetFont(style))
            using (var textBrush = new SolidBrush(textColor))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.None
                };
                
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
