using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // List Fonts & Colors
        public TypographyStyle ListTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ListSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ListUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 200),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ListBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color ListForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color ListBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color ListItemForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color ListItemHoverForeColor { get; set; } = Color.White;
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color ListItemBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
    }
}