using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Tree Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle TreeTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(26, 188, 156), // Neon turquoise
            LineHeight = 1.4f,
            LetterSpacing = 0.4f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TreeNodeSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle TreeNodeUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color TreeBackColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark blue-purple for tree background
        public Color TreeForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for tree text
        public Color TreeBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for tree border
        public Color TreeNodeForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for node text
        public Color TreeNodeHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover text
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for hover background
        public Color TreeNodeSelectedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for selected node text
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected node background
        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for checked box text
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for checked box background
    }
}