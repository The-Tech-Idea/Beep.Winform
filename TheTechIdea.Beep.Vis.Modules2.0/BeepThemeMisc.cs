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
        public string FontFamily { get; set; }
        public float FontSize { get; set; }
        public float LineHeight { get; set; } // Line height multiplier
        public float LetterSpacing { get; set; } // In points
        public FontWeight FontWeight { get; set; }
        public FontStyle FontStyle { get; set; }
        public Color TextColor { get; set; }
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
