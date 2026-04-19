using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Tab Fonts & Colors
        public TypographyStyle TabFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TabHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TabSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TabBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color TabForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ActiveTabForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(240, 235, 215);
        public Color InactiveTabForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color TabBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color TabHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color TabSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
    }
}