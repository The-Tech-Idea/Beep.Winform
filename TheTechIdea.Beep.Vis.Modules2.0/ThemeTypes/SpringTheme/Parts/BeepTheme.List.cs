using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // List Fonts & Colors
        public TypographyStyle ListTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ListSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
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
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ListBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color ListForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ListBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color ListItemForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ListItemHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(34, 139, 34);
        public Color ListItemBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(50, 205, 50);
    }
}