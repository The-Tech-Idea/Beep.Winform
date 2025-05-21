using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // List Fonts & Colors
        public TypographyStyle ListTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 16,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ListSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ListUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ListBackColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color ListForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color ListBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color ListItemForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color ListItemHoverForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color ListItemBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
    }
}