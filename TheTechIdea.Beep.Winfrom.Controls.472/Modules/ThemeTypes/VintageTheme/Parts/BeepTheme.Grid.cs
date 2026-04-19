using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Grid Fonts
        public TypographyStyle GridHeaderFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridRowFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 11,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 11,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 11,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 11,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellErrorFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 11,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(178, 34, 34),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridColumnFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color GridForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(188, 143, 143);
        public Color GridHeaderForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color GridHeaderHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color GridHeaderSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color GridRowHoverForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color GridRowSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color GridLineColor { get; set; } = Color.FromArgb(200, 180, 160);
        public Color RowBackColor { get; set; } = Color.FromArgb(240, 235, 215);
        public Color RowForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color AltRowBackColor { get; set; } = Color.FromArgb(230, 225, 205);
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color SelectedRowForeColor { get; set; } = Color.FromArgb(255, 245, 238);
    }
}