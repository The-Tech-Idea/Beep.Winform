using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } = Color.Transparent;
        public Color LabelForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color LabelBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(50, 205, 50);
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color LabelHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(34, 139, 34);
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color LabelSelectedForeColor { get; set; } = Color.White;
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(150, 150, 150);
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(180, 180, 180);
        public TypographyStyle LabelFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SubLabelFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            LineHeight = 1.2f,
            LetterSpacing = 0.1f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(100, 100, 100),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SubLabelForColor { get; set; } = Color.FromArgb(100, 100, 100);
        public Color SubLabelBackColor { get; set; } = Color.Transparent;
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color SubLabelHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
    }
}