using System;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme  : IBeepTheme
    {
        public CyberpunkNeonTheme()
        {   // Now the properties that depend on others can be set here
            HeadlineLarge = Heading1;
            HeadlineMedium = Heading2;
            HeadlineSmall = Heading3;

            TitleLarge = Heading4;
            TitleMedium = Heading5;
            TitleSmall = Heading6;

            BodyLarge = Paragraph;
            BodyMedium = Paragraph;
            BodySmall = SmallText;
        }
    }
}