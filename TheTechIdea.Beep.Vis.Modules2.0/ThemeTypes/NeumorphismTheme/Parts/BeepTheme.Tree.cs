using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Tree Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle TreeTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(80, 80, 90), // Medium gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TreeBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for tree background
        public Color TreeForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for tree text
        public Color TreeBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for tree border
        public Color TreeNodeForeColor { get; set; } = Color.FromArgb(80, 80, 90); // Medium gray for node text
        public Color TreeNodeHoverForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover text
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Slightly darker gray for hover background
        public Color TreeNodeSelectedForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for selected node text
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected node background
        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for checked box text
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for checked box background
    }
}