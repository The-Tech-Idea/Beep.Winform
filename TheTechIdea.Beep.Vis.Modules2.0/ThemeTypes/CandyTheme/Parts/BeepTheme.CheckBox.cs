using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // CheckBox properties

        // Unchecked state: lemon yellow background, soft gray border, navy text
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(44, 62, 80);    // Navy
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(206, 183, 255); // Pastel Lavender

        // Checked state: pastel mint background, candy pink check, mint border
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint
        public Color CheckBoxCheckedForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint

        // Hover state: pastel blue, pink border, navy text
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Baby Blue
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(44, 62, 80);    // Navy
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink

        // Fonts: playful, but legible
        public TypographyStyle CheckBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Regular);
        public TypographyStyle CheckBoxCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 10.5f, FontStyle.Bold);
    }
}
