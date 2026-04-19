using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Grid Fonts
        public TypographyStyle GridHeaderFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridRowFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellErrorFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 99, 71),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridColumnFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112),
            IsUnderlined = false,
            IsStrikeout = false
        };

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color GridForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color GridHeaderForeColor { get; set; } = Color.FromArgb(25, 25, 112);
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(150, 150, 150);
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color GridHeaderHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color GridHeaderSelectedForeColor { get; set; } = Color.White;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(50, 205, 50);
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(34, 139, 34);
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color GridRowHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color GridRowSelectedForeColor { get; set; } = Color.White;
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(50, 205, 50);
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(34, 139, 34);
        public Color GridLineColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color RowBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color RowForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color AltRowBackColor { get; set; } = Color.FromArgb(230, 240, 245);
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color SelectedRowForeColor { get; set; } = Color.White;
    }
}