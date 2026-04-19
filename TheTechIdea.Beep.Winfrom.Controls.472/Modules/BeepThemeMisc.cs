using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Vis.Modules
{

    public class TypographyStyle
    {
        public TypographyStyle()
        {
            
        }
        public string FontFamily { get; set; }= "Arial";
        public float FontSize { get; set; }= 12; // In points
        public float LineHeight { get; set; } // Line height multiplier
        public float LetterSpacing { get; set; } // In points
        public FontWeight FontWeight { get; set; } = FontWeight.Normal;
        public FontStyle FontStyle { get; set; }= FontStyle.Regular;
        public Color TextColor { get; set; }= Color.Black;
        public bool IsUnderlined { get; set; } = false;
        public bool IsStrikeout { get; set; } = false;
    }

    public enum FontWeight
    {
        Thin = 100,
        ExtraLight = 200,
        Light = 300,
        Normal = 400,
        Regular = 400,
        Medium = 500,
        SemiBold = 600,
        Bold = 700,
        ExtraBold = 800,
        Black = 900
    }

    public enum ShadowStyle
    {
        None,
        Small,
        Medium,
        Large,
        ExtraLarge
    }

    public class ShadowProperties
    {
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float BlurRadius { get; set; }
        public float SpreadRadius { get; set; }
        public Color Color { get; set; }
    }
}
