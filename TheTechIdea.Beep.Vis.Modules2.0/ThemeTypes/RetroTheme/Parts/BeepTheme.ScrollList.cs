using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // ScrollList Fonts & Colors
        public TypographyStyle ScrollListTitleFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 14, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle ScrollListSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.Black, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle ScrollListUnSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public Color ScrollListBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color ScrollListForeColor { get; set; } = Color.White;
        public Color ScrollListBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color ScrollListItemForeColor { get; set; } = Color.White;
        public Color ScrollListItemHoverForeColor { get; set; } = Color.White;
        public Color ScrollListItemHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color ScrollListItemSelectedForeColor { get; set; } = Color.Black;
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.FromArgb(203, 75, 22);
        public Color ScrollListItemBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public TypographyStyle ScrollListItemFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle ScrollListItemSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.Black, IsUnderlined = false, IsStrikeout = false };
    }
}