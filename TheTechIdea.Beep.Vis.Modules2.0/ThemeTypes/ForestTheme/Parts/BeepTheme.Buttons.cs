using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Button Colors and Styles
        public TypographyStyle ButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public TypographyStyle ButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);
        public TypographyStyle ButtonSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);

        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(34, 139, 34); // ForestGreen
        public Color ButtonHoverForeColor { get; set; } = Color.White;
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 100, 0); // DarkGreen
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(0, 100, 0);
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(46, 139, 87); // SeaGreen
        public Color ButtonSelectedForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(0, 128, 0); // Green
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(0, 100, 0);

        public Color ButtonBackColor { get; set; } = Color.FromArgb(60, 179, 113); // MediumSeaGreen
        public Color ButtonForeColor { get; set; } = Color.White;
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(46, 139, 87); // SeaGreen
        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(178, 34, 34); // FireBrick
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(139, 0, 0); // DarkRed
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(34, 139, 34); // ForestGreen
        public Color ButtonPressedForeColor { get; set; } = Color.White;
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(0, 100, 0);
    }
}
