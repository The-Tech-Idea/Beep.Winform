using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // ScrollList Fonts & Colors
        public TypographyStyle ScrollListTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ScrollListSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ScrollListUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color ScrollListBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color ScrollListForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color ScrollListBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color ScrollListItemForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color ScrollListItemHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color ScrollListItemHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color ScrollListItemSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color ScrollListItemBorderColor { get; set; } = Color.FromArgb(200, 180, 160);
        public TypographyStyle ScrollListIItemFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ScrollListItemSelectedFont { get; set; } = new TypographyStyle
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
    }
}