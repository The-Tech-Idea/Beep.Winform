using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Card Colors & Fonts
        public TypographyStyle CardTitleFont { get; set; }
        public Color CardTextForeColor { get; set; }
        public Color CardBackColor { get; set; }
        public Color CardTitleForeColor { get; set; }
        public TypographyStyle CardSubTitleFont { get; set; }
        public Color CardSubTitleForeColor { get; set; }
        public TypographyStyle CardHeaderStyle { get; set; }
        public TypographyStyle CardparagraphStyle { get; set; }
        public TypographyStyle CardSubTitleStyle { get; set; }
        public Color CardrGradiantStartColor { get; set; }
        public Color CardGradiantEndColor { get; set; }
        public Color CardGradiantMiddleColor { get; set; }
        public LinearGradientMode CardGradiantDirection { get; set; }
    }
}
