using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // List Fonts & Colors
        public TypographyStyle ListTitleFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 14, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle ListSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.Black, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle ListUnSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public Color ListBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color ListForeColor { get; set; } = Color.White;
        public Color ListBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color ListItemForeColor { get; set; } = Color.White;
        public Color ListItemHoverForeColor { get; set; } = Color.White;
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color ListItemSelectedForeColor { get; set; } = Color.Black;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(203, 75, 22);
        public Color ListItemBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(131, 148, 150);
    }
}