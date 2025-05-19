using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Grid Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle GridHeaderFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridRowFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridCellErrorFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(200, 100, 100), // Soft red
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle GridColumnFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(100, 100, 100), // Medium gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for grid background
        public Color GridForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for grid text
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink for header
        public Color GridHeaderForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for header text
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for header border
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for header hover
        public Color GridHeaderHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for header hover text
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for selected header
        public Color GridHeaderSelectedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for selected header text
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for header hover border
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for selected header border
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for row hover
        public Color GridRowHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for row hover text
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for selected row
        public Color GridRowSelectedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for selected row text
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for row hover border
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for selected row border
        public Color GridLineColor { get; set; } = Color.FromArgb(200, 200, 200); // Subtle gray for grid lines
        public Color RowBackColor { get; set; } = Color.FromArgb(255, 255, 255); // White for rows
        public Color RowForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for row text
        public Color AltRowBackColor { get; set; } = Color.FromArgb(240, 240, 240); // Slightly darker gray for alternate rows
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for selected row
        public Color SelectedRowForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for selected row text
    }
}