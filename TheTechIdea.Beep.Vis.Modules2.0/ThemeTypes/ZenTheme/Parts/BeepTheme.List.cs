using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // List Fonts & Colors
        public TypographyStyle ListTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(34, 34, 34),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ListSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
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
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(189, 189, 189),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ListBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color ListForeColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color ListBorderColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color ListItemForeColor { get; set; } = Color.FromArgb(189, 189, 189);
        public Color ListItemHoverForeColor { get; set; } = Color.White;
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(96, 195, 100);
        public Color ListItemBorderColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
    }
}