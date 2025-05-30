using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
//<<<<<<< HEAD
        // Stepper Fonts & Colors with Material Design defaults
        public TypographyStyle  StepperTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 14f, FontStyle.Bold);
        public TypographyStyle  StepperSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 12f, FontStyle.Bold);
        public TypographyStyle  StepperUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 12f, FontStyle.Regular);

        public Color StepperBackColor { get; set; } = Color.White;
        public Color StepperForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Color StepperBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300

        public Color StepperItemForeColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600
        public TypographyStyle  StepperItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 11f, FontStyle.Regular);
        public TypographyStyle  StepperSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 10f, FontStyle.Italic);

        public Color StepperItemHoverForeColor { get; set; } = Color.FromArgb(55, 71, 79); // Blue Grey 800
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(232, 245, 253); // Light Blue 50

        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700

        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(189, 189, 189); // Grey 400
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(100, 181, 246); // Blue 300

        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
    }
}
