using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } =Color.FromArgb(96, 96, 96);
        public Color LabelForeColor { get; set; } = Color.White;
        public Color LabelBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(160, 160, 160);
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color LabelHoverForeColor { get; set; } = Color.White;
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color LabelSelectedForeColor { get; set; } = Color.White;
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(96, 96, 96);
        public TypographyStyle LabelFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SubLabelFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 10,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(192, 192, 192),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SubLabelForColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color SubLabelBackColor { get; set; } =Color.FromArgb(96, 96, 96);
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color SubLabelHoverForeColor { get; set; } = Color.White;
    }
}