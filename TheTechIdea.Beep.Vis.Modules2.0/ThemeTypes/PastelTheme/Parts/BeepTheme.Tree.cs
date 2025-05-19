using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Tree Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle TreeTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TreeNodeSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle TreeNodeUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color TreeBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for tree background
        public Color TreeForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for tree text
        public Color TreeBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for tree border
        public Color TreeNodeForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for node text
        public Color TreeNodeHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover text
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for hover background
        public Color TreeNodeSelectedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for selected node text
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for selected node background
        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for checked box text
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for checked box background
    }
}