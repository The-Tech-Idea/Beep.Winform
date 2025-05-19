using System;
using System.Drawing;
using System.Reflection;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Utility Methods

        public Font GetBlockHeaderFont() =>
            GetFont(FontFamily, FontSizeBlockHeader, FontStyleBold);

        public Font GetBlockTextFont() =>
            GetFont(FontFamily, FontSizeBlockText, FontStyleRegular);

        public Font GetQuestionFont() =>
            GetFont(FontFamily, FontSizeQuestion, FontStyleBold);

        public Font GetAnswerFont() =>
            GetFont(FontFamily, FontSizeAnswer, FontStyleRegular | FontStyle.Italic);

        public Font GetCaptionFont() =>
            GetFont(FontFamily, FontSizeCaption, FontStyleRegular);

        public Font GetButtonFont() =>
            GetFont(FontFamily, FontSizeButton, FontStyleBold);

        private Font GetFont(string fontName, float fontSize, FontStyle fontStyle)
        {
            try
            {
                return new Font(fontName, fontSize, fontStyle);
            }
            catch (ArgumentException)
            {
                // Fallback to system default sans-serif if specified font is unavailable
                return new Font(System.Drawing.FontFamily.GenericSansSerif, fontSize, fontStyle);
            }
        }

        /// <summary>
        /// Replaces all fully transparent Color properties in the theme with a fallback color.
        /// </summary>
        public void ReplaceTransparentColors(Color fallbackColor)
        {
            foreach (PropertyInfo prop in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.PropertyType == typeof(Color) && prop.CanWrite)
                {
                    Color color = (Color)prop.GetValue(this);
                    if (color.A == 0)
                    {
                        prop.SetValue(this, fallbackColor);
                    }
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is CandyTheme other)
            {
                // Compare additional fields for stricter equality if desired
                return this.ButtonBackColor == other.ButtonBackColor &&
                       this.ButtonForeColor == other.ButtonForeColor &&
                       this.PanelBackColor == other.PanelBackColor;
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Combine with ValueTuple for best practice
            return (ButtonBackColor, ButtonForeColor, PanelBackColor).GetHashCode();
        }
    }
}
