using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Grid Fonts
        public TypographyStyle GridHeaderFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridRowFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellErrorFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(178, 34, 34), // Crimson
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridColumnFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color GridForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color GridHeaderForeColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(45, 45, 128); // Darker royal blue
        public Color GridHeaderHoverForeColor { get; set; } = Color.White;
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color GridHeaderSelectedForeColor { get; set; } = Color.White;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color GridRowHoverForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color GridRowSelectedForeColor { get; set; } = Color.White;
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color GridLineColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color RowBackColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color RowForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color AltRowBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Slightly darker beige
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color SelectedRowForeColor { get; set; } = Color.White;
    }
}