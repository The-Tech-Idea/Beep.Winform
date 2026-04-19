using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // Grid Fonts
        public TypographyStyle GridHeaderFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Medium,
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
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
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
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
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
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 99, 99),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridColumnFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color GridForeColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(45, 85, 120);
        public Color GridHeaderForeColor { get; set; } = Color.White;
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(80, 120, 160);
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(60, 100, 140);
        public Color GridHeaderHoverForeColor { get; set; } = Color.White;
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color GridHeaderSelectedForeColor { get; set; } = Color.White;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(120, 169, 255);
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color GridRowHoverForeColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color GridRowSelectedForeColor { get; set; } = Color.White;
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(120, 169, 255);
        public Color GridLineColor { get; set; } = Color.FromArgb(200, 220, 240);
        public Color RowBackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color RowForeColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color AltRowBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color SelectedRowForeColor { get; set; } = Color.White;
    }
}