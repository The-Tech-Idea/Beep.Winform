using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
<<<<<<< HEAD
        // Stepper Fonts & Colors with Material Design defaults
        public Font StepperTitleFont { get; set; } = new Font("Roboto", 14f, FontStyle.Bold);
        public Font StepperSelectedFont { get; set; } = new Font("Roboto", 12f, FontStyle.Bold);
        public Font StepperUnSelectedFont { get; set; } = new Font("Roboto", 12f, FontStyle.Regular);

        public Color StepperBackColor { get; set; } = Color.White;
        public Color StepperForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Color StepperBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300

        public Color StepperItemForeColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600
        public Font StepperItemFont { get; set; } = new Font("Roboto", 11f, FontStyle.Regular);
        public Font StepperSubTitleFont { get; set; } = new Font("Roboto", 10f, FontStyle.Italic);

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
=======
        // Stepper Fonts & Colors
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
