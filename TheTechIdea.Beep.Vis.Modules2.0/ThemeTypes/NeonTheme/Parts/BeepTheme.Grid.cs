using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Grid Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle GridHeaderFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(26, 188, 156), // Neon turquoise
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
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
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
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
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
            TextColor = Color.FromArgb(30, 30, 50), // Dark for contrast
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
            TextColor = Color.FromArgb(241, 196, 15), // Neon yellow
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
            TextColor = Color.FromArgb(231, 76, 60), // Neon red
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
            TextColor = Color.FromArgb(46, 204, 113), // Neon green
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark blue-purple for grid background
        public Color GridForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for grid text
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for header
        public Color GridHeaderForeColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for header text
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for header border
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for header hover
        public Color GridHeaderHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for header hover text
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected header
        public Color GridHeaderSelectedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for selected header text
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for header hover border
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected header border
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for row hover
        public Color GridRowHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for row hover text
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected row
        public Color GridRowSelectedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for selected row text
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for row hover border
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected row border
        public Color GridLineColor { get; set; } = Color.FromArgb(60, 60, 80); // Subtle gray for grid lines
        public Color RowBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for rows
        public Color RowForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for row text
        public Color AltRowBackColor { get; set; } = Color.FromArgb(35, 35, 55); // Slightly darker blue-purple for alternate rows
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected row
        public Color SelectedRowForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for selected row text
    }
}