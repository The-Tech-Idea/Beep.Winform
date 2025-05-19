using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Grid Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle GridHeaderFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(100, 200, 180), // Bright teal
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(150, 180, 200), // Soft aqua
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
            TextColor = Color.FromArgb(255, 90, 90), // Coral red
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
            TextColor = Color.FromArgb(150, 180, 200), // Soft aqua
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for grid background
        public Color GridForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for grid text
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for header
        public Color GridHeaderForeColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for header text
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for header border
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for header hover
        public Color GridHeaderHoverForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for header hover text
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected header
        public Color GridHeaderSelectedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for selected header text
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for header hover border
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected header border
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for row hover
        public Color GridRowHoverForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for row hover text
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected row
        public Color GridRowSelectedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for selected row text
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for row hover border
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected row border
        public Color GridLineColor { get; set; } = Color.FromArgb(50, 80, 110); // Lighter ocean blue for grid lines
        public Color RowBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for rows
        public Color RowForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for row text
        public Color AltRowBackColor { get; set; } = Color.FromArgb(15, 30, 60); // Slightly darker blue for alternate rows
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected row
        public Color SelectedRowForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for selected row text
    }
}