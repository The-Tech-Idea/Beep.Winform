using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // ScrollList Fonts & Colors
        public TypographyStyle ScrollListTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ScrollListSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ScrollListUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color ScrollListBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color ScrollListForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color ScrollListBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color ScrollListItemForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color ScrollListItemHoverForeColor { get; set; } = Color.White;
        public Color ScrollListItemHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color ScrollListItemSelectedForeColor { get; set; } = Color.White;
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color ScrollListItemBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public TypographyStyle ScrollListIItemFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ScrollListItemSelectedFont { get; set; } = new TypographyStyle
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
    }
}