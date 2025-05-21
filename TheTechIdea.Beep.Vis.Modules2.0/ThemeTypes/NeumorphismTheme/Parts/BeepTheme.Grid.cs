using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Grid Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle GridHeaderFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(80, 80, 90), // Medium gray
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
            TextColor = Color.FromArgb(80, 80, 90), // Medium gray
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
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(90, 180, 90), // Soft green
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
            TextColor = Color.FromArgb(255, 90, 90), // Soft red
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
            TextColor = Color.FromArgb(80, 80, 90), // Medium gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for grid background
        public Color GridForeColor { get; set; } = Color.FromArgb(80, 80, 90); // Medium gray for grid text
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(220, 220, 225); // Slightly darker gray for header
        public Color GridHeaderForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for header text
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for header border
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Darker gray for header hover
        public Color GridHeaderHoverForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for header hover text
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected header
        public Color GridHeaderSelectedForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for selected header text
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for header hover border
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected header border
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Darker gray for row hover
        public Color GridRowHoverForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for row hover text
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected row
        public Color GridRowSelectedForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for selected row text
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for row hover border
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected row border
        public Color GridLineColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for grid lines
        public Color RowBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for rows
        public Color RowForeColor { get; set; } = Color.FromArgb(80, 80, 90); // Medium gray for row text
        public Color AltRowBackColor { get; set; } = Color.FromArgb(225, 225, 230); // Slightly darker gray for alternate rows
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected row
        public Color SelectedRowForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for selected row text
    }
}