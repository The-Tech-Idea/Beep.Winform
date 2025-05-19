using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Stepper Fonts & Colors
<<<<<<< HEAD
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
=======
        public TypographyStyle StepperTitleFont { get; set; }
        public TypographyStyle StepperSelectedFont { get; set; }
        public TypographyStyle StepperUnSelectedFont { get; set; }
        public Color StepperBackColor { get; set; }
        public Color StepperForeColor { get; set; }
        public Color StepperBorderColor { get; set; }
        public Color StepperItemForeColor { get; set; }
        public TypographyStyle StepperItemFont { get; set; }
        public TypographyStyle StepperSubTitleFont { get; set; }
        public Color StepperItemHoverForeColor { get; set; }
        public Color StepperItemHoverBackColor { get; set; }
        public Color StepperItemSelectedForeColor { get; set; }
        public Color StepperItemSelectedBackColor { get; set; }
        public Color StepperItemSelectedBorderColor { get; set; }
        public Color StepperItemBorderColor { get; set; }
        public Color StepperItemHoverBorderColor { get; set; }
        public Color StepperItemCheckedBoxForeColor { get; set; }
        public Color StepperItemCheckedBoxBackColor { get; set; }
        public Color StepperItemCheckedBoxBorderColor { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
