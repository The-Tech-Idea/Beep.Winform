using System;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Utility Methods

        public Font GetBlockHeaderFont()
        {
            return GetFont(FontFamily, FontSizeBlockHeader, FontStyleBold);
        }

        public Font GetBlockTextFont()
        {
            return GetFont(FontFamily, FontSizeBlockText, FontStyleRegular);
        }

        public Font GetQuestionFont()
        {
            return GetFont(FontFamily, FontSizeQuestion, FontStyleBold);
        }

        public Font GetAnswerFont()
        {
            return GetFont(FontFamily, FontSizeAnswer, FontStyleRegular | FontStyle.Italic);
        }

        public Font GetCaptionFont()
        {
            return GetFont(FontFamily, FontSizeCaption, FontStyleRegular);
        }

        public Font GetButtonFont()
        {
            return GetFont(FontFamily, FontSizeButton, FontStyleBold);
        }

        private Font GetFont(string fontName, float fontSize, FontStyle fontStyle)
        {
            try
            {
                return new Font(fontName, fontSize, fontStyle);
            }
            catch (ArgumentException)
            {
                return new Font(System.Drawing.FontFamily.GenericSansSerif, fontSize, fontStyle);
            }
        }

        public void ReplaceTransparentColors(Color fallbackColor)
        {
            foreach (var prop in GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(Color))
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
            if (obj is BeepTheme other)
            {
                return this.ButtonBackColor == other.ButtonBackColor &&
                       this.ButtonForeColor == other.ButtonForeColor &&
                       this.PanelBackColor == other.PanelBackColor;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ButtonBackColor.GetHashCode() ^ ButtonForeColor.GetHashCode() ^ PanelBackColor.GetHashCode();
        }
    }
}
