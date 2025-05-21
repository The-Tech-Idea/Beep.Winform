using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // ScrollList Fonts & Colors
        public TypographyStyle ScrollListTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ScrollListSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ScrollListUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(192, 192, 192),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ScrollListBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color ScrollListForeColor { get; set; } = Color.White;
        public Color ScrollListBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color ScrollListItemForeColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color ScrollListItemHoverForeColor { get; set; } = Color.White;
        public Color ScrollListItemHoverBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color ScrollListItemSelectedForeColor { get; set; } = Color.White;
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.FromArgb(192, 128, 0);
        public Color ScrollListItemBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public TypographyStyle ScrollListIItemFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(192, 192, 192),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ScrollListItemSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}