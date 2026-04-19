using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // List Fonts & Colors
        public TypographyStyle ListTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ListSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ListUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color ListBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color ListForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color ListBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color ListItemForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color ListItemHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color ListItemSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color ListItemBorderColor { get; set; } = Color.FromArgb(200, 180, 160);
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
    }
}