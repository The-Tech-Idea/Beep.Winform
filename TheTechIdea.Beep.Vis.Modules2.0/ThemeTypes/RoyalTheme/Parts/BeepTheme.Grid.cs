using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Grid Fonts
        public TypographyStyle GridHeaderFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridRowFont { get; set; } = new TypographyStyle
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
        public TypographyStyle GridCellFont { get; set; } = new TypographyStyle
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
        public TypographyStyle GridCellSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle GridCellHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellErrorFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 77, 77),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridColumnFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color GridForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color GridHeaderForeColor { get; set; } = Color.White;
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color GridHeaderHoverForeColor { get; set; } = Color.White;
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color GridHeaderSelectedForeColor { get; set; } = Color.White;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color GridRowHoverForeColor { get; set; } = Color.White;
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color GridRowSelectedForeColor { get; set; } = Color.White;
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color GridLineColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color RowBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color RowForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color AltRowBackColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color SelectedRowForeColor { get; set; } = Color.White;
    }
}