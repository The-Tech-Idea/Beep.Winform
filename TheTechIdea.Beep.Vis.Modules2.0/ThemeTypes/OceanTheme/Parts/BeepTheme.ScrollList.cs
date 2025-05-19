using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // ScrollList Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle ScrollListTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(100, 200, 180), // Bright teal
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ScrollListSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ScrollListUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color ScrollListBackColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for list background
        public Color ScrollListForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for list text
        public Color ScrollListBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for list border
        public Color ScrollListItemForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for item text
        public Color ScrollListItemHoverForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover text
        public Color ScrollListItemHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for hover background
        public Color ScrollListItemSelectedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for selected item text
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected item background
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected item border
        public Color ScrollListItemBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for item border
        public TypographyStyle ScrollListIItemFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ScrollListItemSelectedFont { get; set; } = new TypographyStyle
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
    }
}