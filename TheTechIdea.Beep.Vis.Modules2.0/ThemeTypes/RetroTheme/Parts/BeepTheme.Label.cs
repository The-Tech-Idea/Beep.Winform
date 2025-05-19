using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color LabelForeColor { get; set; } = Color.White;
        public Color LabelBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(131, 148, 150);
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color LabelHoverForeColor { get; set; } = Color.White;
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(203, 75, 22);
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color LabelSelectedForeColor { get; set; } = Color.Black;
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(108, 123, 127);
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public TypographyStyle LabelFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 14, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle SubLabelFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.FromArgb(147, 161, 161), IsUnderlined = false, IsStrikeout = false };
        public Color SubLabelForeColor { get; set; } = Color.FromArgb(147, 161, 161);
        public Color SubLabelBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color SubLabelHoverForeColor { get; set; } = Color.White;
    }
}