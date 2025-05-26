using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
//<<<<<<< HEAD
        // Tab Fonts & Colors with Material Design defaults
        public TypographyStyle  TabFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 14f, FontStyle.Regular);
        public TypographyStyle  TabHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 14f, FontStyle.Bold);
        public TypographyStyle  TabSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 14f, FontStyle.Bold);

        public Color TabBackColor { get; set; } = Color.FromArgb(250, 250, 250); // Grey 50
        public Color TabForeColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600
        public Color ActiveTabBackColor { get; set; } = Color.White;
        public Color ActiveTabForeColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(238, 238, 238); // Grey 200
        public Color InactiveTabForeColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600

        public Color TabBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(232, 234, 246); // Light Blue 50
        public Color TabHoverForeColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
        public Color TabSelectedBackColor { get; set; } = Color.White;
        public Color TabSelectedForeColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(197, 202, 233); // Indigo 100
    }
}
