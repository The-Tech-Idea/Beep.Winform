using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Stepper Fonts & Colors
        public Font StepperTitleFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Bold);
        public Font StepperSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font StepperUnSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Regular);

        public Color StepperBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color StepperForeColor { get; set; } = Color.White;
        public Color StepperBorderColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33);

        public Color StepperItemForeColor { get; set; } = Color.White;
        public Font StepperItemFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font StepperSubTitleFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Italic);

        public Color StepperItemHoverForeColor { get; set; } = Color.White;
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E);

        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color StepperItemSelectedBorderColor { get; set; } = Color.White;

        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(0x44, 0x44, 0x44);
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1);

        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(0x23, 0xB9, 0x5C); // SuccessColor
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.White;
    }
}
